using System.Collections.Generic;
using LayoutManager.Model;

namespace LayoutManager.Decoders {
    [LayoutModule("Generic decoders")]
    internal class GenericDecoders : LayoutModuleBase {
        public static string GenericDecodersVersion = "1.0";

        [LayoutEvent("enum-decoder-types")]
        [LayoutEvent("get-decoder-type", IfEvent = "LayoutEvent[Options/@DecoderType='GenericDCC']")]
        private void GetGenericDcc(LayoutEvent e) {
            var list = Ensure.NotNull<List<DecoderTypeInfo>>(e.Sender);
            var decoder = new DccDecoderTypeInfo() {
                Manufacturer = "Generic",
                Name = "DCC",
                TrackGuages = TrackGauges.G_or_I | TrackGauges.HO | TrackGauges.N | TrackGauges.O | TrackGauges.OO | TrackGauges.Proto48 | TrackGauges.S | TrackGauges.TT | TrackGauges.Z,
                Description = "Generic DCC (NRMA) decoder"
            };

            list.Add(decoder);
        }
    }
}

