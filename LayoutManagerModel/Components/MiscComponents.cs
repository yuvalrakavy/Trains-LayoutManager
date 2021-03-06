using System;
using System.Diagnostics;
using System.Xml;
using System.Linq;

using LayoutManager.Model;
using System.Collections.Generic;

#nullable enable
namespace LayoutManager.Components {
    /// <summary>
    /// Implement a connection of track to power source (command station, programming controller etc.)
    /// </summary>
    ///
    public class LayoutTrackPowerConnectorComponent : ModelComponent, IModelComponentHasId, IModelComponentLayoutLockResource {
        //bool switchingInProgress = false;

        public LayoutTrackPowerConnectorComponent() {
            XmlDocument.LoadXml("<PowerConnector />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.PowerConnector;

        public override String ToString() => "track power connector";

        public LayoutTrackPowerConnectorInfo Info => new LayoutTrackPowerConnectorInfo(this);

        public LayoutPowerInlet Inlet => Info.Inlet;

        public override bool DrawOutOfGrid => true;

        /// <summary>
        /// Locomotives that currently receive power from this connector. This can be useful for detecting too many locomotive (power overuse) or that
        /// only single locomotive is on tracks receiving power from this connector if switch to programming mode
        /// </summary>
        public IEnumerable<TrainLocomotiveInfo> Locomotives {
            get {
                var blockWithTrainsDefinitions = from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases)
                                                 where
                                                     blockDefinition.PowerConnector.Id == this.Id && blockDefinition.Block.HasTrains
                                                 select blockDefinition;

                return blockWithTrainsDefinitions.SelectMany(b => b.Block.Locomotives);
            }
        }

        /// <summary>
        /// Train locations that currently receive power from this connector (a train may span more than one train location if it spans more than one block)
        /// </summary>
        public IEnumerable<TrainLocationInfo> TrainLocations {
            get {
                var blockWithTrainsDefinitions = from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases)
                                                 where
                                                     blockDefinition.PowerConnector.Id == this.Id && blockDefinition.Block.HasTrains
                                                 select blockDefinition;

                return blockWithTrainsDefinitions.SelectMany(b => b.Block.Trains);
            }
        }

        /// <summary>
        /// Trains that currently receive power from this connector
        /// </summary>
        public IEnumerable<TrainStateInfo> Trains => (from trainLocation in TrainLocations select trainLocation.Train).Distinct();

        /// <summary>
        /// Block definitios that recieve power from this connector
        /// </summary>
        public IEnumerable<LayoutBlockDefinitionComponent> BlockDefinitions => from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.PowerConnector != null && blockDefinition.PowerConnector.Id == this.Id select blockDefinition;

        /// <summary>
        /// Blocks that receive power from this connector - this can be useful for locking the region (for example when connecting it to programming power, or disconnecting it)
        /// </summary>
        public IEnumerable<LayoutBlock> Blocks => from blockDefinition in BlockDefinitions select blockDefinition.Block;

        public bool IsResourceReady() => Info.Inlet.ConnectedOutlet.Power.Type != LayoutPowerType.Disconnected;

        public void MakeResourceReady() {
            if (Info.Inlet.ConnectedOutlet.Power.Type == LayoutPowerType.Disconnected/* && !switchingInProgress*/) {
                if (Info.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Digital)) {
                    var switchingCommands = new List<SwitchingCommand>();

                    if (Info.Inlet.ConnectedOutlet.SelectPower(LayoutPowerType.Digital, switchingCommands)) {
                        //switchingInProgress = true;

                        EventManager.Event(new LayoutEvent<LayoutTrackPowerConnectorComponent>("register-power-connected-lock", this));

                        var power = (from p in this.Inlet.ConnectedOutlet.ObtainablePowers where p.Type == LayoutPowerType.Digital select p).FirstOrDefault();

                        foreach (var train in this.Trains)
                            EventManager.Event(new LayoutEvent<TrainStateInfo, ILayoutPower>("change-train-power", train, power));

                        if (switchingCommands.Count > 0)
                            EventManager.AsyncEvent(new LayoutEvent("set-track-components-state", this, switchingCommands)
#if NOTUSED
                                .ContinueWith(
                                (t) => {
                                    switchingInProgress = false;
                                    return Task.FromResult(0);
                                }
#endif
                            );
                    }
                }
            }
        }

        public void FreeResource() {
            if (Info.Inlet.ConnectedOutlet.Power.Type != LayoutPowerType.Disconnected && Info.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Disconnected)) {
                var switchingCommands = new List<SwitchingCommand>();

                if (Info.Inlet.ConnectedOutlet.SelectPower(LayoutPowerType.Disconnected, switchingCommands)) {
                    var power = (from p in this.Inlet.ConnectedOutlet.ObtainablePowers where p.Type == LayoutPowerType.Disconnected select p).FirstOrDefault();

                    foreach (var train in this.Trains)
                        EventManager.Event(new LayoutEvent<TrainStateInfo, ILayoutPower>("change-train-power", train, power));

                    if (switchingCommands.Count > 0)
                        EventManager.AsyncEvent(new LayoutEvent("set-track-components-state", this, switchingCommands));
                }
            }
        }

        public LayoutTextInfo NameProvider => new LayoutTrackPowerConnectorNameInfo(this);

        public class LayoutTrackPowerConnectorNameInfo : LayoutTextInfo {
            const string ElementName = "PowerConnectorName";

            public LayoutTrackPowerConnectorNameInfo(ModelComponent component, String elementName)
                : base(component, elementName) {
            }

            public LayoutTrackPowerConnectorNameInfo(ModelComponent component)
                : base(component, ElementName) {
            }

            public LayoutTrackPowerConnectorNameInfo(XmlElement containerElement)
                : base(containerElement, ElementName) {
            }

            public void CreateElement(LayoutXmlInfo xmlInfo) {
                Element = CreateProviderElement(xmlInfo, ElementName);
            }

            public string Description => ((LayoutTrackPowerConnectorComponent)Component).Info.Name;

            public override string Name => ((LayoutTrackPowerConnectorComponent)Component).Info.Name;
        }

    }

    public class LayoutTrackPowerConnectorInfo : LayoutTextInfo {
        public LayoutTrackPowerConnectorInfo(ModelComponent component, String elementName)
            : base(component, elementName) {
        }

        public LayoutTrackPowerConnectorInfo(ModelComponent component)
            : base(component, "TrackPowerConnector") {
        }

        public LayoutTrackPowerConnectorInfo(XmlElement containerElement)
            : base(containerElement, "TrackPowerConnector") {
        }


        public override String Name {
            get {
                LayoutTrackPowerConnectorComponent powerConnectorComponent = (LayoutTrackPowerConnectorComponent)Component;

                return "Power from: " + powerConnectorComponent.Inlet.ToString();
            }
        }

        public override String Text => Name;

        public LayoutPowerInlet Inlet => new LayoutPowerInlet(Element, "PowerInlet");

        public TrackGauges TrackGauge {
            get {
                if (!Element.HasAttribute("Gauge"))
                    return TrackGauges.HO;
                else
                    return (TrackGauges)Enum.Parse(typeof(TrackGauges), Element.GetAttribute("Gauge"));
            }

            set {
                Element.SetAttribute("Gauge", value.ToString());
            }
        }

        public bool CheckReverseLoops {
            get {
                return XmlConvert.ToBoolean(GetOptionalAttribute("CheckReverseLoops") ?? "true");
            }

            set {
                SetAttribute("CheckReverseLoops", value, removeIf: true);
            }
        }
    }

    public class LayoutPowerSelectorComponent : ModelComponentWithSwitchingState, IModelComponentHasPowerOutlets,
      IModelComponentHasReverseLogic, IModelComponentConnectToControl, IModelComponentHasName {
        public const string SwitchingMethodAttribute = "SwitchingMethod";
        public const string HasOnOffRelayAttribute = "OnOffRelay";
        public const string ReverseOnOffRelayAttribute = "RevereseOnOffRelay";

        public const string PowerSelector1ConnectionPoint = "PowerControl1";
        public const string PowerSelector2ConnectionPoint = "PowerControl2";
        public const string PowerOnOffConnectionPoint = "PowerOnOff";

        ILayoutPowerInlet? _inlet1;
        ILayoutPowerInlet? _inlet2;

        public LayoutPowerSelectorComponent() {
            XmlDocument.LoadXml("<PowerSelector />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.ControlComponent;

        public override string ToString() => IsSwitch ? "Power switch" : "Powser selector";

        public override bool DrawOutOfGrid => true;

        public override void OnXmlInfoChanged() {
            base.OnXmlInfoChanged();

            _inlet1 = null;
            _inlet2 = null;
        }

        public enum RelaySwitchingMethod { DPDSrelay, TwoSPDSrelays };

        public RelaySwitchingMethod SwitchingMethod {
            get {
                return Element.HasAttribute(SwitchingMethodAttribute) ? (RelaySwitchingMethod)Enum.Parse(typeof(RelaySwitchingMethod), Element.GetAttribute(SwitchingMethodAttribute)) : RelaySwitchingMethod.DPDSrelay;
            }

            set {
                Element.SetAttribute(SwitchingMethodAttribute, value.ToString());
            }
        }

        public bool HasOnOffRelay {
            get {
                return Element.HasAttribute(HasOnOffRelayAttribute) ? XmlConvert.ToBoolean(Element.GetAttribute(HasOnOffRelayAttribute)) : false;
            }

            set {
                Element.SetAttribute(HasOnOffRelayAttribute, XmlConvert.ToString(value));
            }
        }

        public bool RevereseOnOffRelay {
            get {
                return Element.HasAttribute(ReverseOnOffRelayAttribute) ? XmlConvert.ToBoolean(Element.GetAttribute(ReverseOnOffRelayAttribute)) : false;
            }

            set {
                Element.SetAttribute(ReverseOnOffRelayAttribute, XmlConvert.ToString(value));
            }
        }

        protected override SwitchingStateSupport GetSwitchingStateSupporter() => new LayoutPowerSelectorSwitchingStateSupporter(this);

        public ILayoutPowerInlet Inlet1 {
            get {
                if (_inlet1 == null)
                    _inlet1 = new LayoutPowerInlet(this, "Inlet1");
                return _inlet1;
            }
        }

        public ILayoutPowerInlet Inlet2 {
            get {
                if (_inlet2 == null)
                    _inlet2 = new LayoutPowerInlet(this, "Inlet2");
                return _inlet2;
            }
        }

        public bool IsSwitch => !Inlet1.IsConnected ^ !Inlet2.IsConnected;

        public bool IsSelector => !IsSwitch;

        public bool IsPowerConnected {
            get {
                var hasPower = GetSwitchState(PowerOnOffConnectionPoint) == 1;

                if (RevereseOnOffRelay)
                    hasPower = !hasPower;

                return hasPower;
            }
        }
        public ILayoutPowerInlet? CurrentSelectedInlet {
            get {
                if (IsSwitch) {
                    if (IsPowerConnected)
                        return Inlet1.IsConnected ? Inlet1 : Inlet2;
                    else
                        return null;
                }
                else {
                    bool hasPower = true;

                    if (HasOnOffRelay)
                        hasPower = IsPowerConnected;

                    if (hasPower) {
                        if (GetSwitchState(PowerSelector1ConnectionPoint) == 0 && !ReverseLogic)
                            return Inlet1;
                        else
                            return Inlet2;
                    }
                    else
                        return null;
                }
            }
        }

        public ILayoutPowerInlet SwitchedInlet {
            get {
                Debug.Assert(IsSwitch);

                return Inlet1.IsConnected ? Inlet1 : Inlet2;
            }
        }

        public int SwitchedInletIndex {
            get {
                Debug.Assert(IsSwitch);
                return Inlet1.IsConnected ? 0 : 1;
            }
        }

        public LayoutTextInfo NameProvider => new LayoutPowerSelectorNameInfo(this);

        #region IModelComponentHasPowerOutlets Members

        public IList<ILayoutPowerOutlet> PowerOutlets => Array.AsReadOnly<ILayoutPowerOutlet>(new ILayoutPowerOutlet[] { new LayoutPowerSelectorSwitchOutlet(this) });

        #endregion

        #region IModelComponentConnectToControl Members

        public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions {
            get {
                var connectionPoints = new List<ModelComponentControlConnectionDescription>();

                if (IsSwitch)
                    connectionPoints.Add(new ModelComponentControlConnectionDescription("Relay", PowerOnOffConnectionPoint, "Power On/Off relay"));
                else {
                    if (SwitchingMethod == RelaySwitchingMethod.DPDSrelay)
                        connectionPoints.Add(new ModelComponentControlConnectionDescription("Relay", PowerSelector1ConnectionPoint, "Power Selector Control"));
                    else {
                        connectionPoints.Add(new ModelComponentControlConnectionDescription("Relay", PowerSelector1ConnectionPoint, "Power Selector wire 1 relay"));
                        connectionPoints.Add(new ModelComponentControlConnectionDescription("Relay", PowerSelector2ConnectionPoint, "Power Selector wire 2 relay"));
                    }

                    if (HasOnOffRelay)
                        connectionPoints.Add(new ModelComponentControlConnectionDescription("Relay", PowerOnOffConnectionPoint, "Power On/Off relay"));
                }

                return connectionPoints.AsReadOnly();
            }
        }

        #endregion

        #region PowerSelectorComponent related classes

        public class LayoutPowerSelectorSwitchingStateSupporter : SwitchingStateSupport {
            public LayoutPowerSelectorSwitchingStateSupporter(IModelComponentHasSwitchingState component)
                : base(component) {
            }

            public override void SetSwitchState(ControlConnectionPoint controlConnectionPoint, int switchState, string connectionPointName) {
                base.SetSwitchState(controlConnectionPoint, switchState, connectionPointName);

                var powerSelector = (LayoutPowerSelectorComponent)Component;
                ILayoutPowerOutlet outlet = powerSelector.PowerOutlets[0];

                EventManager.Event(new LayoutEvent<ILayoutPowerOutlet, ILayoutPower>("power-outlet-changed-state-notification", outlet, outlet.Power));
            }
        }

        public class LayoutPowerSelectorSwitchOutlet : ILayoutPowerOutlet {
            LayoutPowerSelectorComponent PowerSelectorComponent { get; }

            readonly ILayoutPower NoPower;

            public LayoutPowerSelectorSwitchOutlet(LayoutPowerSelectorComponent powerSelectorComponent) {
                this.PowerSelectorComponent = powerSelectorComponent;
                this.NoPower = new LayoutPower(PowerSelectorComponent, LayoutPowerType.Disconnected, DigitalPowerFormats.None, "Disconnected");
            }

            #region ILayoutPowerOutlet Members

            public IModelComponentHasPowerOutlets OutletComponent => PowerSelectorComponent;

            public string OutletDescription => PowerSelectorComponent.IsSwitch ? "Switch output" : "Selector output";

            public ILayoutPower ConnectedInletPower => PowerSelectorComponent.Inlet1.IsConnected ? PowerSelectorComponent.Inlet1.ConnectedOutlet.Power : PowerSelectorComponent.Inlet2.ConnectedOutlet.Power;

            public ILayoutPower Power => PowerSelectorComponent.CurrentSelectedInlet?.ConnectedOutlet.Power ?? NoPower;

            /// <summary>
            /// The power that can be obtained from this component, this is the union of the one obtainable from inlet1 and from inlet2
            /// </summary>
            public IEnumerable<ILayoutPower> ObtainablePowers {
                get {
                    var result = new List<ILayoutPower>();

                    if (PowerSelectorComponent.IsSwitch) {
                        AddObtainablePowerFromInlet(PowerSelectorComponent.SwitchedInlet, result);
                        result.Add(NoPower);
                    }
                    else {
                        AddObtainablePowerFromInlet(PowerSelectorComponent.Inlet1, result);
                        AddObtainablePowerFromInlet(PowerSelectorComponent.Inlet2, result);

                        if (PowerSelectorComponent.HasOnOffRelay)
                            result.Add(NoPower);
                    }

                    return result;
                }
            }

            private void AddObtainablePowerFromInlet(ILayoutPowerInlet inlet, IList<ILayoutPower> obtainablePowers) {
                if (inlet.IsConnected) {
                    foreach (ILayoutPower power in inlet.ConnectedOutlet.ObtainablePowers)
                        obtainablePowers.Add(power);
                }
                else if (!obtainablePowers.Any(p => p.Type == LayoutPowerType.Disconnected))
                    obtainablePowers.Add(NoPower);
            }

            public bool SelectPower(LayoutPowerType powerType, IList<SwitchingCommand> switchingCommands) {
                bool ok = true;

                if (PowerSelectorComponent.IsSwitch) {
                    if (PowerSelectorComponent.SwitchedInlet.IsConnected && PowerSelectorComponent.SwitchedInlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == powerType)) {
                        PowerSelectorComponent.SwitchedInlet.ConnectedOutlet.SelectPower(powerType, switchingCommands);
                        PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 0 : 1, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);
                    }
                    else {
                        PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 1 : 0, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);
                        ok = false;
                    }

                }
                else {
                    if (powerType == LayoutPowerType.Disconnected && PowerSelectorComponent.HasOnOffRelay) {
                        PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 1 : 0, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);
                    }
                    else {
                        if (PowerSelectorComponent.Inlet1.IsConnected && PowerSelectorComponent.Inlet1.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == powerType)) {
                            PowerSelectorComponent.Inlet1.ConnectedOutlet.SelectPower(powerType, switchingCommands);

                            if (PowerSelectorComponent.SwitchingMethod == RelaySwitchingMethod.DPDSrelay) {
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 0);

                                if (PowerSelectorComponent.HasOnOffRelay)
                                    PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 0 : 1, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);
                            }
                            else if (PowerSelectorComponent.SwitchingMethod == RelaySwitchingMethod.TwoSPDSrelays) {
                                // Switch power off
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 1 : 0, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);

                                // Switch two wires of the power from one source to the other
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 0, LayoutPowerSelectorComponent.PowerSelector1ConnectionPoint);
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 0, LayoutPowerSelectorComponent.PowerSelector2ConnectionPoint);

                                // Switch power back on
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 0 : 1, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);
                            }
                            else
                                Debug.Assert(false);
                        }
                        else if (PowerSelectorComponent.Inlet2.IsConnected && PowerSelectorComponent.Inlet2.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == powerType)) {
                            PowerSelectorComponent.Inlet2.ConnectedOutlet.SelectPower(powerType, switchingCommands);

                            if (PowerSelectorComponent.SwitchingMethod == RelaySwitchingMethod.DPDSrelay)
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 1);
                            else if (PowerSelectorComponent.SwitchingMethod == RelaySwitchingMethod.TwoSPDSrelays) {
                                // Switch power off
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 1 : 0, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);

                                // Switch two wires of the power from one source to the other
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 1, LayoutPowerSelectorComponent.PowerSelector1ConnectionPoint);
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 1, LayoutPowerSelectorComponent.PowerSelector2ConnectionPoint);

                                // Switch power back on
                                PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.RevereseOnOffRelay ? 0 : 1, LayoutPowerSelectorComponent.PowerOnOffConnectionPoint);
                            }
                            else
                                Debug.Assert(false);
                        }
                        else
                            ok = false;
                    }
                }

                return ok;
            }

            #endregion
        }

        public class LayoutPowerSelectorNameInfo : LayoutTextInfo {
            public LayoutPowerSelectorNameInfo(ModelComponent component, String elementName)
                : base(component, elementName) {
            }

            public LayoutPowerSelectorNameInfo(ModelComponent component)
                : base(component, "PowerSelectorName") {
            }

            public LayoutPowerSelectorNameInfo(XmlElement containerElement)
                : base(containerElement, "PowerSelectorName") {
            }

            public void CreateElement(LayoutXmlInfo xmlInfo) {
                Element = CreateProviderElement(xmlInfo, "PowerSelectorName");
            }

            public string Description {
                get {
                    LayoutPowerSelectorComponent powerSelectorComponent = (LayoutPowerSelectorComponent)Component;

                    if (powerSelectorComponent.IsSwitch)
                        return Name + " (On/Off switch of " + powerSelectorComponent.SwitchedInlet.ToString() + ")";
                    else
                        return Name + " (Select between:" + powerSelectorComponent.Inlet1.ToString() + " or " + powerSelectorComponent.Inlet2.ToString() + ")";
                }
            }

            public override string Text => Name;
        }

        #endregion

    }


    public class LayoutTrackIsolationComponent : ModelComponent {

        public LayoutTrackIsolationComponent() {
            XmlDocument.LoadXml("<TrackIsolation />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.TrackIsolation;

        public static bool Is(LayoutModelSpotComponentCollection spot) => spot[ModelComponentKind.TrackIsolation] != null && spot[ModelComponentKind.TrackIsolation] is LayoutTrackIsolationComponent;

        public override bool DrawOutOfGrid => false;

        public override String ToString() => "track power isolation";
    }

    public class LayoutTrackReverseLoopModule : ModelComponent {

        public LayoutTrackReverseLoopModule() {
            XmlDocument.LoadXml("<TractReverseLoopModule />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.TrackIsolation;

        public static bool Is(LayoutModelSpotComponentCollection spot) => spot[ModelComponentKind.TrackIsolation] != null && spot[ModelComponentKind.TrackIsolation] is LayoutTrackReverseLoopModule;

        public override bool DrawOutOfGrid => false;

        public override String ToString() => "track reverse loop module";
    }

    public class LayoutTrackLink : IComparable<LayoutTrackLink> {
        Guid areaGuid;
        Guid trackLinkGuid;

        public LayoutTrackLink(XmlReader r) {
            ReadXml(r);
        }

        public LayoutTrackLink(Guid areaGuid, Guid trackLinkGuid) {
            this.areaGuid = areaGuid;
            this.trackLinkGuid = trackLinkGuid;
        }

        public Guid AreaGuid => areaGuid;

        public Guid TrackLinkGuid => trackLinkGuid;

        public LayoutTrackLinkComponent ResolveLink() {
            LayoutModelArea area = LayoutModel.Areas[AreaGuid];

            Debug.Assert(area != null, "Cannot resolve track link");
            return area.TrackLinks[TrackLinkGuid];
        }

        public void WriteXml(XmlWriter w) {
            w.WriteStartElement("Link");
            w.WriteAttributeString("AreaGuid", XmlConvert.EncodeName(areaGuid.ToString()));
            w.WriteAttributeString("TrackLinkGuid", XmlConvert.EncodeName(trackLinkGuid.ToString()));
            w.WriteEndElement();
        }

        public void ReadXml(XmlReader r) {
            if (r.Name == "Link") {
                areaGuid = new Guid(XmlConvert.DecodeName(r.GetAttribute("AreaGuid")));
                trackLinkGuid = new Guid(XmlConvert.DecodeName(r.GetAttribute("TrackLinkGuid")));
                r.Read();
            }
            else
                r.Skip();
        }

        public override int GetHashCode() => trackLinkGuid.GetHashCode();

        #region IComparable<LayoutTrackLink> Members

        public int CompareTo(LayoutTrackLink other) {
            if (Equals(other))
                return 0;
            else
                return GetHashCode() - other.GetHashCode();
        }

        public bool Equals(LayoutTrackLink other) => areaGuid == other.areaGuid && trackLinkGuid == other.trackLinkGuid;

        #endregion
    }

    public class LayoutTrackLinkComponent : ModelComponent, IModelComponentHasId {
        Guid trackLinkGuid;
        LayoutTrackLink? link;

        public LayoutTrackLinkComponent() {
            trackLinkGuid = Guid.NewGuid();
            XmlInfo.XmlDocument.LoadXml("<TrackLink/>");
        }

        public override ModelComponentKind Kind => ModelComponentKind.TrackLink;

        public Guid TrackLinkGuid => trackLinkGuid;

        public override bool DrawOutOfGrid => true;


        public LayoutTrackLinkComponent? LinkedComponent {
            get {
                if (link == null)
                    return null;        // Not linked
                else
                    return link.ResolveLink();
            }
        }

        public LayoutTrackComponent? LinkedTrack {
            get {
                var trackLink = LinkedComponent;

                if (trackLink != null)
                    return trackLink.Spot.Track;
                return null;
            }
        }

        public LayoutTrackComponent Track => Spot.Track;

        /// <summary>
        /// Return a LayoutTrackLink for this component
        /// </summary>
        public LayoutTrackLink ThisLink => new LayoutTrackLink(Spot.Area.AreaGuid, trackLinkGuid);

        /// <summary>
        /// Return the LayoutTrackLink of the component that this component is linked to.
        /// If set, link this component to another one, the other component is linked to this one.
        /// </summary>
        public LayoutTrackLink? Link {
            get {
                return link;
            }

            set {
                link = value;
            }
        }

        public override String ToString() => "track link";

        public override void OnAddedToModel() {
            base.OnAddedToModel();

            Spot.Area.TrackLinks.Add(this);

            if (!LayoutModel.Instance.ModelIsLoading) {
                var linkedComponent = LinkedComponent;

                // If this component was linked to another one, and the other component was not linked
                // to another component, recreate the bidirectional link.
                if (linkedComponent != null && linkedComponent.Link == null) {
                    linkedComponent.EraseImage();
                    linkedComponent.Link = ThisLink;
                    linkedComponent.Redraw();
                }
                else
                    link = null;        // Otherwise, clear the link
            }
        }

        public override void OnRemovingFromModel() {
            base.OnRemovingFromModel();

            var linkedComponent = LinkedComponent;

            if (linkedComponent != null)
                linkedComponent.EraseImage();

            Spot.Area.TrackLinks.Remove(this);

            // If this component was linked to another component. Clear the other component link
            // so there will be no dangling link

            if (linkedComponent != null) {
                linkedComponent.Link = null;
                linkedComponent.Redraw();
            }
        }

        public override void WriteXmlFields(XmlWriter w) {
            w.WriteStartElement("TrackLink");
            w.WriteAttributeString("ID", XmlConvert.EncodeName(trackLinkGuid.ToString()));
            w.WriteEndElement();

            if (link != null)
                link.WriteXml(w);
        }

        protected override bool ReadXmlField(XmlReader r) {
            ILayoutXmlContext context = (ILayoutXmlContext)r;

            if (r.Name == "Link") {
                link = new LayoutTrackLink(r);

                if (context.ReadXmlContext == LayoutReadXmlContext.PasteComponents) {
                    // This is due to pasting component, check that the link can be resolved.
                    try {
                        link.ResolveLink();
                    }
                    catch (ApplicationException) {
                        link = null;        // Link could not be resolved, get rid of it
                    }
                }

                return true;
            }
            else if (r.Name == "TrackLink") {
                Guid id = new Guid(XmlConvert.DecodeName(r.GetAttribute("ID")));

                if (context.ReadXmlContext == LayoutReadXmlContext.PasteComponents) {
                    bool idFound = false;

                    // This is due to pasting component, if the id does not appear in the model
                    // use this id, otherwise, create a new ID
                    foreach (LayoutModelArea area in LayoutModel.Areas)
                        if (area.TrackLinks.ContainsKey(id)) {
                            idFound = true;
                            break;
                        }

                    if (idFound)
                        id = Guid.NewGuid();
                }

                trackLinkGuid = id;

                r.Read();
                return true;
            }
            return false;
        }
    }

    public class LayoutTrackLinkTextInfo : LayoutTextInfo {
        readonly LayoutTrackLinkComponent trackLinkComponent;

        public LayoutTrackLinkTextInfo(ModelComponent component, String elementPath)
            : base(component, elementPath) {
            this.trackLinkComponent = (LayoutTrackLinkComponent)component;
        }

        public override String Text {
            get {
                if (trackLinkComponent.Link == null || trackLinkComponent.LinkedComponent == null)
                    return base.Text;
                else {
                    String name = base.Text;
                    LayoutTrackLinkComponent destinationTrackLinkComponent = trackLinkComponent.LinkedComponent;
                    LayoutTextInfo destinationTextProvider = new LayoutTextInfo(destinationTrackLinkComponent);
                    String destinationName = destinationTextProvider.Text;

                    if (trackLinkComponent.ThisLink.AreaGuid == destinationTrackLinkComponent.ThisLink.AreaGuid)
                        return name + " (to " + destinationName + ")";
                    else
                        return name + " (to " + LayoutModel.Areas[destinationTrackLinkComponent.ThisLink.AreaGuid].Name + ":" + destinationName + ")";
                }
            }
        }
    }

    /// <summary>
    /// Text label component
    /// </summary>
    public class LayoutTextComponent : ModelComponent {
        public LayoutTextComponent() {
            XmlInfo.XmlDocument.LoadXml("<TextLabel/>");
        }

        public override ModelComponentKind Kind => ModelComponentKind.Annotation;

        public LayoutTextInfo TextProvider => new LayoutTextInfo(this, "Text");

        public override String ToString() => "text: " + TextProvider.Text;

        public override bool DrawOutOfGrid => true;

    }

    public class LayoutImageComponent : ModelComponent {
        public LayoutImageComponent() {
            XmlInfo.XmlDocument.LoadXml("<Image />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.Background;

        public override void OnRemovingFromModel() {
            LayoutImageInfo imageProvider = new LayoutImageInfo(Element);

            EventManager.Event(new LayoutEvent("remove-image-from-cache", this, imageProvider.ImageFile, imageProvider.ImageCacheEventXml));

            base.OnRemovingFromModel();
        }

        public override void OnAddedToModel() {
            var _ = new LayoutImageInfo(Element) {
                ImageError = false
            };
            base.OnAddedToModel();
        }

        public override void OnXmlInfoChanged() {
            var _ = new LayoutImageInfo(Element) {
                ImageError = false
            };
            base.OnXmlInfoChanged();
        }

        public override bool DrawOutOfGrid => true;


        public override String ToString() => "Image";
    }

    public abstract class LayoutTrackAnnotationComponent : ModelComponent {
        public LayoutStraightTrackComponent? Track => Spot.Track as LayoutStraightTrackComponent;
    }

    public class LayoutBridgeComponent : LayoutTrackAnnotationComponent {
        public LayoutBridgeComponent() {
            XmlDocument.LoadXml("<Bridge />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.Background;

        public override bool DrawOutOfGrid => false;


        public override string ToString() => "bridge";
    }

    public class LayoutTunnelComponent : LayoutTrackAnnotationComponent {
        public LayoutTunnelComponent() {
            XmlDocument.LoadXml("<Tunnel />");
        }

        public override ModelComponentKind Kind => ModelComponentKind.Background;

        public override bool DrawOutOfGrid => false;


        public override string ToString() => "tunnel";
    }

    public enum LayoutGateState {
        /// <summary>
        /// Gate is open - train can pass
        /// </summary>
        Open,

        /// <summary>
        /// Gate is closed - train cannot pass
        /// </summary>
        Close,

        /// <summary>
        /// In transition to open state - train cannot pass
        /// </summary>
        Opening,

        /// <summary>
        /// In transition to close state - train cannot pass
        /// </summary>
        Closing,
    }

    public class LayoutGateComponentInfo : LayoutInfo {
        public LayoutGateComponentInfo(XmlElement element)
            : base(element) {
        }

        public enum FeedbackTypes { NoFeedback, OneSensor, TwoSensors };

        private const string TwoDirectionRelaysAttribute = "TwoDirectionRelays";
        private const string FeedbackTypeAttribute = "FeedbackType";
        private const string ReverseDirectionAttribute = "ReverseDirection";
        private const string ReverseMotionAttribute = "ReverseMotion";
        private const string MotionTimeoutAttribute = "MotionTimeout";
        private const string OpenUpOrLeftAttribute = "OpenUpOrLeft";
        private const string MotionTimeAttribute = "MotionTime";

        public bool TwoDirectionRelays {
            get { return XmlConvert.ToBoolean(GetOptionalAttribute(TwoDirectionRelaysAttribute) ?? "false"); }
            set { SetAttribute(TwoDirectionRelaysAttribute, value); }
        }

        public FeedbackTypes FeedbackType {
            get { return HasAttribute(FeedbackTypeAttribute) ? (FeedbackTypes)Enum.Parse(typeof(FeedbackTypes), GetAttribute(FeedbackTypeAttribute)) : FeedbackTypes.NoFeedback; }
            set { SetAttribute(FeedbackTypeAttribute, value.ToString()); }
        }

        public bool ReverseDirection {
            get { return XmlConvert.ToBoolean(GetOptionalAttribute(ReverseDirectionAttribute) ?? "false"); }
            set { SetAttribute(ReverseDirectionAttribute, value); }
        }

        public bool ReverseMotion {
            get { return XmlConvert.ToBoolean(GetOptionalAttribute(ReverseMotionAttribute) ?? "false"); }
            set { SetAttribute(ReverseMotionAttribute, value); }
        }

        /// <summary>
        /// The time (in seconds) that the gate is opened/closed.
        /// </summary>
        /// <remarks>
        /// Time to wait for feedback sensor to report that the gate motion is done
        /// </remarks>
        public int MotionTimeout {
            get { return XmlConvert.ToInt32(GetOptionalAttribute(MotionTimeoutAttribute) ?? "10"); }
            set { SetAttribute(MotionTimeoutAttribute, value); }
        }

        /// <summary>
        /// Gate motion time if the gate has no motion done feedback
        /// </summary>
        public int MotionTime {
            get { return XmlConvert.ToInt32(GetOptionalAttribute(MotionTimeAttribute) ?? "10"); }
            set { SetAttribute(MotionTimeAttribute, value); }
        }

        /// <summary>
        /// Gate orientation
        /// </summary>
        public bool OpenUpOrLeft {
            get { return XmlConvert.ToBoolean(GetAttribute(OpenUpOrLeftAttribute)); }
            set { SetAttribute(OpenUpOrLeftAttribute, XmlConvert.ToString(value)); }
        }
    }

    public class LayoutGateComponent : LayoutTrackAnnotationComponent, IModelComponentHasNameAndId, IModelComponentConnectToControl, IModelComponentLayoutLockResource {
        public LayoutGateComponent() {
            XmlDocument.LoadXml("<Gate OpenUpOrLeft=\"false\"/>");
        }

        const string GateStateTopic = "Gate";

        public override ModelComponentKind Kind => ModelComponentKind.Gate;

        public override bool DrawOutOfGrid => false;

        public LayoutGateComponentInfo Info => new LayoutGateComponentInfo(Element);

        /// <summary>
        /// The gate open/close (or transition state)
        /// </summary>
        public LayoutGateState GateState {
            get {
                if (LayoutModel.StateManager.Components.Contains(Id))
                    return (LayoutGateState)Enum.Parse(typeof(LayoutGateState), LayoutModel.StateManager.Components.StateOf(Id, GateStateTopic).GetAttribute("State"));

                return LayoutGateState.Close;
            }

            set {
                LayoutModel.StateManager.Components.StateOf(Id, GateStateTopic).SetAttribute("State", value.ToString());
                OnComponentChanged();

                if (value == LayoutGateState.Open)
                    EventManager.Event(new LayoutEvent("layout-lock-resource-ready", this));
            }
        }

        public override string ToString() => "gate";


        #region IModelComponentConnectToControl Members

        public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions {
            get {
                var list = new List<ModelComponentControlConnectionDescription>();


                if (Info.TwoDirectionRelays) {
                    list.Add(new ModelComponentControlConnectionDescription($"{ControlConnectionPointTypes.OutputSolenoid},{ControlConnectionPointTypes.OutputRelay}", StandardGateDrivers.Direction1ConnectionPoint, "Gate control 1"));
                    list.Add(new ModelComponentControlConnectionDescription($"{ControlConnectionPointTypes.OutputSolenoid},{ControlConnectionPointTypes.OutputRelay}", StandardGateDrivers.Direction2ConnectionPoint, "Gate control 2"));
                }
                else {
                    list.Add(new ModelComponentControlConnectionDescription($"{ControlConnectionPointTypes.OutputSolenoid},{ControlConnectionPointTypes.OutputRelay}", StandardGateDrivers.Direction1ConnectionPoint, "Gate direction control (open/close)"));
                    list.Add(new ModelComponentControlConnectionDescription($"{ControlConnectionPointTypes.OutputSolenoid},{ControlConnectionPointTypes.OutputRelay}", StandardGateDrivers.MotionConnectionPoint, "Gate motion control"));
                }

                if (Info.FeedbackType == LayoutGateComponentInfo.FeedbackTypes.TwoSensors) {
                    list.Add(new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.Input, StandardGateDrivers.GateOpenConnectionPoint, "Gate open sensor"));
                    list.Add(new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.Input, StandardGateDrivers.GateCloseConnectionPoint, "Gate closed sensor"));
                }
                else if (Info.FeedbackType == LayoutGateComponentInfo.FeedbackTypes.OneSensor)
                    list.Add(new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.Input, StandardGateDrivers.GateMotionDoneConnectionPoint, "Gate motion done sensor"));

                return list;
            }
        }

        #endregion

        #region ILayoutLockResource Members

        public bool IsResourceReady() => GateState == LayoutGateState.Open;

        public void MakeResourceReady() {
            if (GateState != LayoutGateState.Open)
                EventManager.Event(new LayoutEvent("open-gate-request", this));
        }

        public void FreeResource() {
            if (GateState != LayoutGateState.Close && GateState != LayoutGateState.Closing)
                EventManager.Event(new LayoutEvent("close-gate-request", this));
        }

        #endregion

        #region IObjectHasName Members

        public LayoutTextInfo NameProvider => new LayoutTextInfo(this);

        #endregion
    }

#pragma warning disable IDE0051
    [LayoutModule("Standard Gate Drivers", UserControl = false)]
    class StandardGateDrivers : LayoutModuleBase {
        readonly Dictionary<Guid, LayoutDelayedEvent> pendingEvents = new Dictionary<Guid, LayoutDelayedEvent>();

        internal const string Direction1ConnectionPoint = "Direction1";
        internal const string Direction2ConnectionPoint = "Direction2";
        internal const string MotionConnectionPoint = "Motion";
        internal const string GateOpenConnectionPoint = "GateOpen";
        internal const string GateCloseConnectionPoint = "GateClose";
        internal const string GateMotionDoneConnectionPoint = "GateMotionDone";

        [LayoutEvent("open-gate-request", SenderType = typeof(LayoutGateComponent))]
        private void openGateRequest(LayoutEvent e) {
            var gateComponent = Ensure.NotNull<LayoutGateComponent>(e.Sender, "gateComponent");

            if (pendingEvents.TryGetValue(gateComponent.Id, out LayoutDelayedEvent pendingEvent)) {
                pendingEvent.Cancel();
                pendingEvents.Remove(gateComponent.Id);
            }

            if (LayoutController.IsOperationMode) {
                int directionState = gateComponent.Info.ReverseDirection ? 0 : 1;

                StartGateMotion(gateComponent, directionState);

                if (gateComponent.Info.FeedbackType == LayoutGateComponentInfo.FeedbackTypes.NoFeedback || LayoutController.IsOperationSimulationMode)
                    EventManager.DelayedEvent(gateComponent.Info.MotionTime * 1000, new LayoutEvent("gate-is-open", gateComponent));
                else
                    pendingEvents.Add(gateComponent.Id, EventManager.DelayedEvent(gateComponent.Info.MotionTimeout * 1000, new LayoutEvent("gate-open-timeout", gateComponent)));
            }

            gateComponent.GateState = LayoutGateState.Opening;
        }


        [LayoutEvent("close-gate-request", SenderType = typeof(LayoutGateComponent))]
        private void closeGateRequest(LayoutEvent e) {
            var gateComponent = Ensure.NotNull<LayoutGateComponent>(e.Sender, "gateComponent");

            if (pendingEvents.TryGetValue(gateComponent.Id, out LayoutDelayedEvent pendingEvent)) {
                pendingEvent.Cancel();
                pendingEvents.Remove(gateComponent.Id);
            }

            else if (LayoutController.IsOperationMode) {
                int directionState = gateComponent.Info.ReverseDirection ? 1 : 0;

                StartGateMotion(gateComponent, directionState);

                if (gateComponent.Info.FeedbackType == LayoutGateComponentInfo.FeedbackTypes.NoFeedback || LayoutController.IsOperationSimulationMode)
                    EventManager.DelayedEvent(gateComponent.Info.MotionTime * 1000, new LayoutEvent("gate-is-closed", gateComponent));
                else
                    pendingEvents.Add(gateComponent.Id, EventManager.DelayedEvent(gateComponent.Info.MotionTimeout * 1000, new LayoutEvent("gate-close-timeout", gateComponent)));
            }

            gateComponent.GateState = LayoutGateState.Closing;
        }

        private static ControlConnectionPointReference cprOf(LayoutGateComponent gateComponent, string name) {
            var cp = Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[gateComponent, name], name);
            return new ControlConnectionPointReference(cp);
        }

        private static void StartGateMotion(LayoutGateComponent gateComponent, int directionState) {
            ControlConnectionPointReference cprOf(string name) => StandardGateDrivers.cprOf(gateComponent, name);

            if (gateComponent.Info.TwoDirectionRelays) {
                var gateRelay1Reference = cprOf(Direction1ConnectionPoint);
                var gateRelay2Reference = cprOf(Direction2ConnectionPoint);

                // Put relays in opposite state, so one end is connected to one pole and the other end is connected to the other pole.
                // which poles are connected is controlled via the direction state
                EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", gateRelay1Reference, directionState).SetCommandStation(gateRelay1Reference));
                EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", gateRelay2Reference, 1 - directionState).SetCommandStation(gateRelay2Reference));
            }
            else {
                var gateDirectionReference = cprOf(Direction1ConnectionPoint);
                var gateMotionReference = cprOf(MotionConnectionPoint);
                int motionState = gateComponent.Info.ReverseMotion ? 0 : 1;

                EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", gateDirectionReference, directionState).SetCommandStation(gateDirectionReference));
                EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", gateMotionReference, motionState).SetCommandStation(gateMotionReference));
            }
        }

        private static void StopGateMotion(LayoutGateComponent gateComponent) {
            ControlConnectionPointReference cprOf(string name) => StandardGateDrivers.cprOf(gateComponent, name);

            if (gateComponent.Info.TwoDirectionRelays) {
                var gateRelay1Reference = cprOf(Direction1ConnectionPoint);
                var gateRelay2Reference = cprOf(Direction2ConnectionPoint);

                // Put both relays in the same state, thus the same wire is connected to both ends...
                EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", gateRelay1Reference, 0).SetCommandStation(gateRelay1Reference));
                EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", gateRelay2Reference, 0).SetCommandStation(gateRelay2Reference));
            }
            else {
                int motionState = gateComponent.Info.ReverseMotion ? 1 : 0;
                var gateMotionReference = cprOf(MotionConnectionPoint);

                EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", gateMotionReference, motionState).SetCommandStation(gateMotionReference));
            }
        }

        [LayoutEvent("gate-is-open", SenderType = typeof(LayoutGateComponent))]
        private void gateIsOpen(LayoutEvent e) {
            var gateComponent = Ensure.NotNull<LayoutGateComponent>(e.Sender, "gateComponent");

            StopGateMotion(gateComponent);

            if (pendingEvents.TryGetValue(gateComponent.Id, out LayoutDelayedEvent pendingEvent)) {
                pendingEvent.Cancel();
                pendingEvents.Remove(gateComponent.Id);
            }

            gateComponent.GateState = LayoutGateState.Open;
        }

        [LayoutEvent("gate-is-closed", SenderType = typeof(LayoutGateComponent))]
        private void gateIsClosed(LayoutEvent e) {
            var gateComponent = Ensure.NotNull<LayoutGateComponent>(e.Sender, "gateComponent");

            StopGateMotion(gateComponent);

            if (pendingEvents.TryGetValue(gateComponent.Id, out LayoutDelayedEvent pendingEvent)) {
                pendingEvent.Cancel();
                pendingEvents.Remove(gateComponent.Id);
            }

            gateComponent.GateState = LayoutGateState.Close;
        }

        [LayoutEvent("gate-open-timeout", SenderType = typeof(LayoutGateComponent))]
        private void gateOpenTimeout(LayoutEvent e) {
            var gateComponent = Ensure.NotNull<LayoutGateComponent>(e.Sender, "gateComponent");

            Error(gateComponent, "Timeout while waiting for gate to open");
            EventManager.Event(new LayoutEvent("gate-is-open", gateComponent));
        }

        [LayoutEvent("gate-close-timeout", SenderType = typeof(LayoutGateComponent))]
        private void gateCloseTimeout(LayoutEvent e) {
            var gateComponent = Ensure.NotNull<LayoutGateComponent>(e.Sender, "gateComponent");

            Error(gateComponent, "Timeout while waiting for gate to close");
            EventManager.Event(new LayoutEvent("gate-is-closed", gateComponent));
        }

        [LayoutEvent("control-connection-point-state-changed-notification")]
        private void controlConnectionPointStateChangedNotification(LayoutEvent e) {
            var connectionPointRef = Ensure.NotNull<ControlConnectionPointReference>(e.Sender, "connectionPointRef");

            if (connectionPointRef.IsConnected) {
                var connectionPoint = connectionPointRef.ConnectionPoint;

                if (connectionPoint?.Component is LayoutGateComponent component) {
                    if (component.Info.FeedbackType == LayoutGateComponentInfo.FeedbackTypes.TwoSensors) {
                        if (connectionPoint.Name == GateOpenConnectionPoint)
                            EventManager.Event(new LayoutEvent("gate-is-open", component));
                        else if (connectionPoint.Name == GateCloseConnectionPoint)
                            EventManager.Event(new LayoutEvent("gate-is-closed", component));
                    }
                    else if (component.Info.FeedbackType == LayoutGateComponentInfo.FeedbackTypes.OneSensor) {
                        if (connectionPoint.Name == GateMotionDoneConnectionPoint) {
                            if (component.GateState == LayoutGateState.Closing)
                                EventManager.Event(new LayoutEvent("gate-is-closed", component));
                            else if (component.GateState == LayoutGateState.Opening)
                                EventManager.Event(new LayoutEvent("gate-is-open", component));
                        }
                    }
                }
            }
        }
    }
}
