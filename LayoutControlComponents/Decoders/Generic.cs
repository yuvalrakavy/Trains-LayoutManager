using System.Collections.Generic;
using MethodDispatcher;
using LayoutManager.Model;

namespace LayoutManager.Decoders {
    [LayoutModule("Generic decoders")]
    internal class GenericDecoders : LayoutModuleBase {
        public static string GenericDecodersVersion = "1.0";

        [DispatchTarget]
        private DecoderTypeInfo GetDecoderType_GenericDcc([DispatchFilter("RegEx", "(GenericDCC|ALL)")] string decoderTypeName) {
            return new DccDecoderTypeInfo() {
                Manufacturer = "Generic",
                Name = "DCC",
                TrackGuages = TrackGauges.G_or_I | TrackGauges.HO | TrackGauges.N | TrackGauges.O | TrackGauges.OO | TrackGauges.Proto48 | TrackGauges.S | TrackGauges.TT | TrackGauges.Z,
                Description = "Generic DCC (NRMA) decoder"
            };
        }
    }
}

