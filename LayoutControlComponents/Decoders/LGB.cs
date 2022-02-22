using System.Collections.Generic;
using MethodDispatcher;
using LayoutManager.Model;

namespace LayoutManager.Decoders {
    public class LGBdecoderTypeInfo : DccDecoderTypeInfo {
        public LGBdecoderTypeInfo() {
            this.Manufacturer = "LGB";
            this.RecoomededLowestAddress = 1;
            this.TrackGuages = Model.TrackGauges.G;
        }
    }

    [LayoutModule("LGB decoders")]
    internal class LGBdecoders : LayoutModuleBase {
        [DispatchTarget]
        private DecoderTypeInfo GetDecoderType_LGB55020([DispatchFilter("RegEx", "(LGB55020|ALL)")] string decoderTypeName) {
            return new LGBdecoderTypeInfo() {
                Name = "55020",
                HighestAddress = 22,
                ParallelFunctionSupport = false,
                ProgrammingMethod = DccProgrammingMethod.Register,
                Description = "Original MTS decoder"
            };
        }

        [DispatchTarget]
        private DecoderTypeInfo GetDecoderType_LGB55021([DispatchFilter("RegEx", "(LGB55021|ALL)")] string decoderTypeName) {
            return new LGBdecoderTypeInfo() {
                Name = "55021",
                Description = "MTS decoder with Back-EMF"
            };
        }

        [DispatchTarget]
        private DecoderTypeInfo GetDecoderType_LGB55022([DispatchFilter("RegEx", "(LGB55022|ALL)")] string decoderTypeName) {
            return new LGBdecoderTypeInfo() {
                Name = "55022",
                Description = "Small loco decoder"
            };
        }
    }
}

