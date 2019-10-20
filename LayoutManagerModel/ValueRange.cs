using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace LayoutManager.Model {

    public partial class SortedVector<T> {
        public class ValueRange : IEnumerable<T> {
            private readonly SortedVector<T> list;
            private readonly int from;
            private readonly int to;

            internal ValueRange(SortedVector<T> list, int from, int to) {
                this.list = list;
                this.from = from;
                this.to = to;
            }

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator() {
                foreach (KeyValuePair<int, T> entry in list) {
                    if (entry.Key > to)
                        yield break;
                    if (entry.Key >= from)
                        yield return entry.Value;
                }
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }
    }
}
