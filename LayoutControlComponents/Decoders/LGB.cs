using System.Collections.Generic;
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
        [LayoutEvent("enum-decoder-types")]
        [LayoutEvent("get-decoder-type", IfEvent = "LayoutEvent[Options/@DecoderType='LGB55020']")]
        private void Get55020(LayoutEvent e) {
            var list = Ensure.NotNull<List<DecoderTypeInfo>>(e.Sender);
            var decoder = new LGBdecoderTypeInfo() {
                Name = "55020",
                HighestAddress = 22,
                ParallelFunctionSupport = false,
                ProgrammingMethod = DccProgrammingMethod.Register,
                Description = "Original MTS decoder"
            };

            list.Add(decoder);
        }

        [LayoutEvent("enum-decoder-types")]
        [LayoutEvent("get-decoder-type", IfEvent = "LayoutEvent[Options/@DecoderType='LGB55021']")]
        private void Get55021(LayoutEvent e) {
            var list = Ensure.NotNull<List<DecoderTypeInfo>>(e.Sender);
            var decoder = new LGBdecoderTypeInfo() {
                Name = "55021",
                Description = "MTS decoder with Back-EMF"
            };

            list.Add(decoder);
        }

        [LayoutEvent("enum-decoder-types")]
        [LayoutEvent("get-decoder-type", IfEvent = "LayoutEvent[Options/@DecoderType='LGB55022']")]
        private void Get55022(LayoutEvent e) {
            var list = Ensure.NotNull<List<DecoderTypeInfo>>(e.Sender);
            var decoder = new LGBdecoderTypeInfo() {
                Name = "55022",
                Description = "Small loco decoder"
            };

            list.Add(decoder);
        }
    }
}

