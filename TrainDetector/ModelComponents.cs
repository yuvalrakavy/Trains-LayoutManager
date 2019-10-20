using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Net;

#nullable enable
#pragma warning disable IDE0051
namespace TrainDetector {
    public class TrainDetectorsComponent : LayoutBusProviderSupport, IModelComponentIsBusProvider {
        public static LayoutTraceSwitch TraceTrainDetector = new LayoutTraceSwitch("TrainDetectors", "VillaRakavy TrainDetectors");

        private ControlBus? _trainDetectorsBus = null;

        public ControlBus TrainDetectorsBus => _trainDetectorsBus ?? (_trainDetectorsBus = Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(this, "TrainDetectorsBus"), "TrainDetectorBus"));

        public TrainDetectorsComponent() {
            this.XmlDocument.LoadXml("<TrainDetectors />");
        }

        #region Model Component overrides

        public override void OnAddedToModel() {
            base.OnAddedToModel();

            LayoutModel.ControlManager.Buses.AddBusProvider(this);
        }

        public override void OnRemovingFromModel() {
            base.OnRemovingFromModel();

            LayoutModel.ControlManager.Buses.RemoveBusProvider(this);
        }

        public override ModelComponentKind Kind => ModelComponentKind.ControlComponent;

        public override bool DrawOutOfGrid => NameProvider.Element != null && NameProvider.Visible;

        #endregion

        public TrainDetectorsInfo Info => new TrainDetectorsInfo(this, Element);

        #region IModelComponentIsBusProvider Members
        public override IList<string> BusTypeNames => Array.AsReadOnly<string>(new string[] { "TrainDetectorsBus" });

        #endregion

        #region Event Handlers

        [LayoutEvent("begin-design-time-layout-activation")]
        private void beginDesignTimeLayoutActivation(LayoutEvent e) {
            e.Info = true;
        }

        [LayoutEvent("end-design-time-layout-activation")]
        private void EndDesignTimeLayoutActivation(LayoutEvent e) {
            e.Info = true;
        }

        #endregion

    }

    public class TrainDetectorsInfo : LayoutInfo {
        const string A_AutoDetect = "AutoDetect";

        public TrainDetectorsComponent TrainDetectorsComponent { get; private set; }

        public TrainDetectorsInfo(TrainDetectorsComponent component, XmlElement element) : base(element) {
            this.TrainDetectorsComponent = component;

        }
        public bool AutoDetect {
            get => (bool?)AttributeValue(A_AutoDetect) ?? false;
            set => SetAttributeValue(A_AutoDetect, value, removeIf: false);
        }
    }
}
