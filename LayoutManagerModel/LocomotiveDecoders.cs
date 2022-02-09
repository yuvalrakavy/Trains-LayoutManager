using System.Collections.Generic;

namespace LayoutManager.Model {
    public abstract class DecoderTypeInfo {
        public string Name {
            get;
            set;
        } = string.Empty;

        public string Manufacturer {
            get;
            set;
        } = "Unknown";

        public string Description {
            get;
            set;
        } = string.Empty;

        public DigitalPowerFormats SupportedDigitalPowerFormats {
            get;
            set;
        } = DigitalPowerFormats.None;

        public TrackGauges TrackGuages {
            get;
            set;
        } = TrackGauges.Unknown;

        public string TypeName => Manufacturer + Name;

        public static DecoderTypeInfo GetDecoderType(string decoderTypeName) {
            List<DecoderTypeInfo> decoders = new();

            EventManager.Event(new LayoutEvent("get-decoder-type", decoders).SetOption("DecoderType", decoderTypeName));

            if (decoders.Count > 0)
                return decoders[0];
            else
                throw new LayoutException("Unable to get decoder type " + decoderTypeName);
        }
    }

    public class DecoderWithNumericAddressTypeInfo : DecoderTypeInfo {
        public int LowestAddress {
            get;
            set;
        }

        public int HighestAddress {
            get;
            set;
        }

        public int RecoomededLowestAddress {
            get;
            set;
        }
    }

    public enum DccProgrammingMethod {
        Register,
        Cv,
    }

    public class DccDecoderTypeInfo : DecoderWithNumericAddressTypeInfo {
        public DccDecoderTypeInfo() {
            LowestAddress = 1;
            HighestAddress = 10239;
            RecoomededLowestAddress = 24;                       // Above old LGB decoders which support only 1-23...
            ProgrammingMethod = DccProgrammingMethod.Cv;
            SupportPOMprogramming = true;
            ParallelFunctionSupport = true;                     // Can process parallel function instruction, or use the old funky LGB serial function encoding
            SupportedDigitalPowerFormats = DigitalPowerFormats.NRMA;
        }

        public DccProgrammingMethod ProgrammingMethod {
            get;
            set;
        }

        public bool SupportPOMprogramming {
            get;
            set;
        }

        public bool ParallelFunctionSupport {
            get;
            set;
        }
    }
}
