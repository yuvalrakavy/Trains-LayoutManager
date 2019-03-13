using System;

namespace LayoutManager {
    public class PackedArray {
        readonly ulong[] storage;

        public int Length { get; }
        public int BitsPerElement { get; }
        public int ElementsPerStorageUnit { get; }
        public static readonly int BitsPerStorageUnit = sizeof(ulong) * 8;
        readonly ulong elementMask;

        public PackedArray(int length, int bitsPerElement = 1) {
            this.Length = length;
            this.BitsPerElement = bitsPerElement;

            ElementsPerStorageUnit = BitsPerStorageUnit / BitsPerElement;

            int storageLength = (length + ElementsPerStorageUnit - 1) / ElementsPerStorageUnit;

            for (int i = 0; i < bitsPerElement; i++)
                elementMask = (elementMask << 1) | 1;

            storage = new ulong[storageLength];
        }

        public int this[int index] {
            get {
                if (index >= Length)
                    throw new IndexOutOfRangeException("Index: " + index + " is out of range " + Length);

                int storageIndex = index / ElementsPerStorageUnit;
                int inStorageElementIndex = index % ElementsPerStorageUnit;

                return (int)((storage[storageIndex] >> (BitsPerElement * inStorageElementIndex)) & elementMask);
            }

            set {
                if (index >= Length)
                    throw new IndexOutOfRangeException("Index: " + index + " is out of range " + Length);

                if (((ulong)value & ~elementMask) != 0)
                    throw new ArgumentOutOfRangeException("Element can be in range of 0 to " + elementMask);

                int storageIndex = index / ElementsPerStorageUnit;
                int inStorageElementIndex = index % ElementsPerStorageUnit;
                int elementShift = inStorageElementIndex * BitsPerElement;

                storage[storageIndex] &= ~(elementMask << elementShift);
                storage[storageIndex] |= (ulong)value << elementShift;
            }
        }
    }

    public class EnumPackedArray<T> {
        readonly PackedArray array;

        public EnumPackedArray(int length) {
            int bitsPerElement = 0;

            foreach (int v in typeof(T).GetEnumValues()) {
                int valueBitsPerElement = 0;
                int mask = 0;

                do {
                    mask = (mask << 1) | 1;
                    valueBitsPerElement++;
                } while ((v & ~mask) != 0);

                if (valueBitsPerElement > bitsPerElement)
                    bitsPerElement = valueBitsPerElement;
            }

            array = new PackedArray(length, bitsPerElement);
        }

        public int Length => array.Length;

        public T this[int index] {
            get {
                return (T)Enum.ToObject(typeof(T), array[index]);
            }

            set {
                array[index] = Convert.ToInt32(value);
            }
        }
    }


}
