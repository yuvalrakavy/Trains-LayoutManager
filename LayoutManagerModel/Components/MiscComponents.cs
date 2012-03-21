using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Threading;
using System.Linq;

using LayoutManager.Model;
using System.Collections.Generic;

namespace LayoutManager.Components {
	/// <summary>
	/// Implement a connection of track to power source (command station, programming controller etc.)
	/// </summary>
	///
	public class LayoutTrackPowerConnectorComponent : ModelComponent, IModelComponentHasId {
		public LayoutTrackPowerConnectorComponent() {
			XmlDocument.LoadXml("<PowerConnector />");
		}

		public override ModelComponentKind Kind {
			get {
				return ModelComponentKind.PowerConnector;
			}
		}
		
		public override String ToString() {
			return "track power connector";
		}

		public LayoutTrackPowerConnectorInfo Info {
			get {
				return new LayoutTrackPowerConnectorInfo(this);
			}
		}

		public LayoutPowerInlet Inlet {
			get { return Info.Inlet; }
		}

		public override bool DrawOutOfGrid {
			get {
				return true;
			}
		}

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
		public IEnumerable<TrainStateInfo> Trains {
			get {
				return (from trainLocation in TrainLocations select trainLocation.Train).Distinct();
			}
		}

		/// <summary>
		/// Blocks that receive power from this connector - this can be useful for locking the region (for example when connecting it to programming power, or disconnecting it)
		/// </summary>
		public IEnumerable<LayoutBlock> Blocks {
			get {
				return from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.PowerConnector.Id == this.Id select blockDefinition.Block;
			}
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

		public override String Text {
			get {
				return Name;
			}
		}

		public LayoutPowerInlet Inlet {
			get {
				return new LayoutPowerInlet(Element, "PowerInlet");
			}
		}

		public TrackGauges TrackGauge {
			get {
				if(!Element.HasAttribute("Gauge"))
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
				return XmlConvert.ToBoolean(Element.GetAttribute("CheckReverseLoops", "true"));
			}

			set {
                SetAttribute("CheckReverseLoops", value, removeIf: true);
			}
		}
	}

	public class LayoutPowerSelectorComponent : ModelComponentWithSwitchingState, IModelComponentHasPowerOutlets,
	  IModelComponentHasReverseLogic, IModelComponentConnectToControl, IModelComponentHasName {
		ILayoutPowerInlet _inlet1;
		ILayoutPowerInlet _inlet2;

		public LayoutPowerSelectorComponent() {
			XmlDocument.LoadXml("<PowerSelector />");
		}

		public override ModelComponentKind Kind {
			get { return ModelComponentKind.ControlComponent; }
		}

		public override string ToString() {
			return IsSwitch ? "Power switch" : "Powser selector";
		}

		public override bool DrawOutOfGrid {
			get { return true; }
		}

		public override void OnXmlInfoChanged() {
			base.OnXmlInfoChanged();

			_inlet1 = null;
			_inlet2 = null;
		}

		protected override SwitchingStateSupport GetSwitchingStateSupporter() {
			return new LayoutPowerSelectorSwitchingStateSupporter(this);
		}

		public ILayoutPowerInlet Inlet1 {
			get {
				if(_inlet1 == null)
					_inlet1 = new LayoutPowerInlet(this, "Inlet1");
				return _inlet1;
			}
		}

		public ILayoutPowerInlet Inlet2 {
			get {
				if(_inlet2 == null)
					_inlet2 = new LayoutPowerInlet(this, "Inlet2");
				return _inlet2;
			}
		}

		public bool IsSwitch {
			get {
				return !Inlet1.IsConnected ^ !Inlet2.IsConnected;
			}
		}

		public bool IsSelector {
			get {
				return !IsSwitch;
			}
		}

		public ILayoutPowerInlet CurrentSelectedInlet {
			get {
				if(CurrentSwitchState == 0 && !ReverseLogic)
					return Inlet1;
				else
					return Inlet2;
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

		public LayoutTextInfo NameProvider {
			get { return new LayoutPowerSelectorNameInfo(this); }
		}

		#region IModelComponentHasPowerOutlets Members

		public IList<ILayoutPowerOutlet> PowerOutlets {
			get { return Array.AsReadOnly<ILayoutPowerOutlet>(new ILayoutPowerOutlet[] { new LayoutPowerSelectorSwitchOutlet(this) }); }
		}

		#endregion

		#region IModelComponentConnectToControl Members

		public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions {
			get {
				return Array.AsReadOnly<ModelComponentControlConnectionDescription>(new ModelComponentControlConnectionDescription[] {
					 new ModelComponentControlConnectionDescription("Relay", "PowerSelector", "Power Selector Control")
				});
			}
		}

		#endregion

		#region PowerSelectorComponent related classes

		public class LayoutPowerSelectorSwitchingStateSupporter : SwitchingStateSupport {
			public LayoutPowerSelectorSwitchingStateSupporter(IModelComponentHasSwitchingState component)
				: base(component) {
			}

			public override void SetSwitchState(ControlConnectionPoint controlConnectionPoint, int switchState) {
				base.SetSwitchState(controlConnectionPoint, switchState);

				var powerSelector = (LayoutPowerSelectorComponent)Component;
				ILayoutPowerOutlet outlet = powerSelector.PowerOutlets[0];
				
				EventManager.Event(new LayoutEvent(outlet, "power-outlet-changed-state-notification", null, outlet.Power));
			}
		}

		public class LayoutPowerSelectorSwitchOutlet : ILayoutPowerOutlet {
			LayoutPowerSelectorComponent PowerSelectorComponent { get; set; }

			public LayoutPowerSelectorSwitchOutlet(LayoutPowerSelectorComponent powerSelectorComponent) {
				this.PowerSelectorComponent = powerSelectorComponent;
			}

			#region ILayoutPowerOutlet Members

			public string OutletDescription {
				get { return PowerSelectorComponent.IsSwitch ? "Switch output" : "Selector output"; }
			}

			public ILayoutPower Power {
				get {
					ILayoutPowerInlet selectedInlet = PowerSelectorComponent.CurrentSelectedInlet;

					if(selectedInlet.IsConnected)
						return selectedInlet.ConnectedOutlet.Power;
					else
						return new LayoutPower(PowerSelectorComponent, LayoutPowerType.Disconnected, DigitalPowerFormats.None, "Disconnected");
				}
			}

			/// <summary>
			/// The power that can be obtained from this component, this is the union of the one obtainable from inlet1 and from inlet2
			/// </summary>
			public IEnumerable<ILayoutPower> ObtainablePowers {
				get {
					var result = new List<ILayoutPower>();

					AddIObtainablePowerFromInlet(PowerSelectorComponent.Inlet1, result);
					AddIObtainablePowerFromInlet(PowerSelectorComponent.Inlet2, result);

					return result;
				}
			}

			private void AddIObtainablePowerFromInlet(ILayoutPowerInlet inlet, IList<ILayoutPower> obtainablePowers) {
				if(inlet.IsConnected) {
					foreach(ILayoutPower power in inlet.ConnectedOutlet.ObtainablePowers)
						obtainablePowers.Add(power);
				}
				else if(!obtainablePowers.Any(p => p.Type == LayoutPowerType.Disconnected))
					obtainablePowers.Add(new LayoutPower(PowerSelectorComponent, LayoutPowerType.Disconnected, DigitalPowerFormats.None, "Disconnected"));
			}

			public bool SelectPower(LayoutPowerType powerType, IList<SwitchingCommand> switchingCommands) {
				bool ok = true;

				if(PowerSelectorComponent.IsSwitch) {
					if(PowerSelectorComponent.SwitchedInlet.IsConnected && PowerSelectorComponent.SwitchedInlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == powerType)) {
						PowerSelectorComponent.SwitchedInlet.ConnectedOutlet.SelectPower(powerType, switchingCommands);
						PowerSelectorComponent.AddSwitchingCommands(switchingCommands, PowerSelectorComponent.SwitchedInletIndex);
					}
					else {
						PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 1 - PowerSelectorComponent.SwitchedInletIndex);
						ok = false;
					}

				}
				else {
					if(PowerSelectorComponent.Inlet1.IsConnected && PowerSelectorComponent.Inlet1.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == powerType)) {
						PowerSelectorComponent.Inlet1.ConnectedOutlet.SelectPower(powerType, switchingCommands);
						PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 0);
					}
					else if(PowerSelectorComponent.Inlet2.IsConnected && PowerSelectorComponent.Inlet2.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == powerType)) {
						PowerSelectorComponent.Inlet2.ConnectedOutlet.SelectPower(powerType, switchingCommands);
						PowerSelectorComponent.AddSwitchingCommands(switchingCommands, 1);
					}
					else
						ok = false;
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

					if(powerSelectorComponent.IsSwitch)
						return Name + " (On/Off switch of " + powerSelectorComponent.SwitchedInlet.ToString() + ")";
					else
						return Name + " (Select between:" + powerSelectorComponent.Inlet1.ToString() + " or " + powerSelectorComponent.Inlet2.ToString() + ")";
				}
			}

			public override string Text {
				get {
					return Name;
				}
			}
		}

		#endregion

	}


	public class LayoutTrackIsolationComponent : ModelComponent {

		public LayoutTrackIsolationComponent() {
			XmlDocument.LoadXml("<TrackIsolation />");
		}

		public override ModelComponentKind Kind {
			get { return ModelComponentKind.TrackIsolation;	}
		}

		public static bool Is(LayoutModelSpotComponentCollection spot) {
			return spot[ModelComponentKind.TrackIsolation] != null && spot[ModelComponentKind.TrackIsolation] is LayoutTrackIsolationComponent;
		}

		public override bool DrawOutOfGrid {
			get { return false; }
		}

		public override String ToString() {
			return "track power isolation";
		}
	}

	public class LayoutTrackReverseLoopModule : ModelComponent {

		public LayoutTrackReverseLoopModule() {
			XmlDocument.LoadXml("<TractReverseLoopModule />");
		}

		public override ModelComponentKind Kind { 
			get { return ModelComponentKind.TrackIsolation; }
		}

		public static bool Is(LayoutModelSpotComponentCollection spot) {
			return spot[ModelComponentKind.TrackIsolation] != null && spot[ModelComponentKind.TrackIsolation] is LayoutTrackReverseLoopModule;
		}

		public override bool DrawOutOfGrid {
			get { return false; }
		}

		public override String ToString() {
			return "track reverse loop module";
		}
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

		public Guid AreaGuid {
			get {
				return areaGuid;
			}
		}

		public Guid TrackLinkGuid {
			get {
				return trackLinkGuid;
			}
		}

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
			if(r.Name == "Link") {
				areaGuid = new Guid(XmlConvert.DecodeName(r.GetAttribute("AreaGuid")));
				trackLinkGuid = new Guid(XmlConvert.DecodeName(r.GetAttribute("TrackLinkGuid")));
				r.Read();
			}
			else
				r.Skip();
		}

		public override int GetHashCode() {
			return trackLinkGuid.GetHashCode();
		}

		#region IComparable<LayoutTrackLink> Members

		public int CompareTo(LayoutTrackLink other) {
			if(Equals(other))
				return 0;
			else
				return GetHashCode() - other.GetHashCode();
		}

		public bool Equals(LayoutTrackLink other) {
			return areaGuid == other.areaGuid && trackLinkGuid == other.trackLinkGuid;
		}

		#endregion
	}

	public class LayoutTrackLinkComponent : ModelComponent, IModelComponentHasId {
		Guid trackLinkGuid;
		LayoutTrackLink link;

		public LayoutTrackLinkComponent() {
			trackLinkGuid = Guid.NewGuid();
			XmlInfo.XmlDocument.LoadXml("<TrackLink/>");
		}

		public override ModelComponentKind Kind {
			get {
				return ModelComponentKind.TrackLink;
			}
		}

		public Guid TrackLinkGuid {
			get {
				return trackLinkGuid;
			}
		}

		public override bool DrawOutOfGrid {
			get {
				return true;
			}
		}


		public LayoutTrackLinkComponent LinkedComponent {
			get {
				if(link == null)
					return null;		// Not linked
				else
					return link.ResolveLink();
			}
		}

		public LayoutTrackComponent LinkedTrack {
			get {
				LayoutTrackLinkComponent trackLink = LinkedComponent;

				if(trackLink != null)
					return trackLink.Spot.Track;
				return null;
			}
		}

		public LayoutTrackComponent Track {
			get {
				return Spot.Track;
			}
		}

		/// <summary>
		/// Return a LayoutTrackLink for this component
		/// </summary>
		public LayoutTrackLink ThisLink {
			get {
				return new LayoutTrackLink(Spot.Area.AreaGuid, trackLinkGuid);
			}
		}

		/// <summary>
		/// Return the LayoutTrackLink of the component that this component is linked to.
		/// If set, link this component to another one, the other component is linked to this one.
		/// </summary>
		public LayoutTrackLink Link {
			get {
				return link;
			}

			set {
				link = value;
			}
		}

		public override String ToString() {
			return "track link";
		}

		public override void OnAddedToModel() {
			base.OnAddedToModel();

			Spot.Area.TrackLinks.Add(this);

			if(!LayoutModel.Instance.ModelIsLoading) {
				LayoutTrackLinkComponent linkedComponent = LinkedComponent;

				// If this component was linked to another one, and the other component was not linked
				// to another component, recreate the bidirectional link.
				if(linkedComponent != null && linkedComponent.Link == null) {
					linkedComponent.EraseImage();
					linkedComponent.Link = ThisLink;
					linkedComponent.Redraw();
				}
				else
					link = null;		// Otherwise, clear the link
			}
		}

		public override void OnRemovingFromModel() {
			base.OnRemovingFromModel();

			LayoutTrackLinkComponent linkedComponent = LinkedComponent;

			if(linkedComponent != null)
				linkedComponent.EraseImage();

			Spot.Area.TrackLinks.Remove(this);

			// If this component was linked to another component. Clear the other component link
			// so there will be no dangling link

			if(linkedComponent != null) {
				linkedComponent.Link = null;
				linkedComponent.Redraw();
			}
		}

		public override void WriteXmlFields(XmlWriter w) {
			w.WriteStartElement("TrackLink");
			w.WriteAttributeString("ID", XmlConvert.EncodeName(trackLinkGuid.ToString()));
			w.WriteEndElement();

			if(link != null)
				link.WriteXml(w);
		}

		protected override bool ReadXmlField(XmlReader r) {
			ILayoutXmlContext context = (ILayoutXmlContext)r;

			if(r.Name == "Link") {
				link = new LayoutTrackLink(r);

				if(context.ReadXmlContext == LayoutReadXmlContext.PasteComponents) {
					// This is due to pasting component, check that the link can be resolved.
					try {
						link.ResolveLink();
					}
					catch(ApplicationException) {
						link = null;		// Link could not be resolved, get rid of it
					}
				}

				return true;
			}
			else if(r.Name == "TrackLink") {
				Guid id = new Guid(XmlConvert.DecodeName(r.GetAttribute("ID")));

				if(context.ReadXmlContext == LayoutReadXmlContext.PasteComponents) {
					bool idFound = false;

					// This is due to pasting component, if the id does not appear in the model
					// use this id, otherwise, create a new ID
					foreach(LayoutModelArea area in LayoutModel.Areas)
						if(area.TrackLinks.ContainsKey(id)) {
							idFound = true;
							break;
						}

					if(idFound)
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
		LayoutTrackLinkComponent trackLinkComponent;

		public LayoutTrackLinkTextInfo(ModelComponent component, String elementPath)
			: base(component, elementPath) {
			this.trackLinkComponent = (LayoutTrackLinkComponent)component;
		}

		public override String Text {
			get {
				if(trackLinkComponent.Link == null || trackLinkComponent.LinkedComponent == null)
					return base.Text;
				else {
					String name = base.Text;
					LayoutTrackLinkComponent destinationTrackLinkComponent = trackLinkComponent.LinkedComponent;
					LayoutTextInfo destinationTextProvider = new LayoutTextInfo(destinationTrackLinkComponent);
					String destinationName = destinationTextProvider.Text;

					if(trackLinkComponent.ThisLink.AreaGuid == destinationTrackLinkComponent.ThisLink.AreaGuid)
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

		public override ModelComponentKind Kind {
			get {
				return ModelComponentKind.Annotation;
			}
		}

		public LayoutTextInfo TextProvider {
			get {
				return new LayoutTextInfo(this, "Text");
			}
		}

		public override String ToString() {
			return "text: " + TextProvider.Text;
		}

		public override bool DrawOutOfGrid {
			get {
				return true;
			}
		}

	}

	public class LayoutImageComponent : ModelComponent {
		public LayoutImageComponent() {
			XmlInfo.XmlDocument.LoadXml("<Image />");
		}

		public override ModelComponentKind Kind {
			get {
				return ModelComponentKind.Background;
			}
		}

		public override void OnRemovingFromModel() {
			LayoutImageInfo imageProvider = new LayoutImageInfo(Element);

			EventManager.Event(new LayoutEvent(this, "remove-image-from-cache", imageProvider.ImageCacheEventXml, imageProvider.ImageFile));

			base.OnRemovingFromModel();
		}

		public override void OnAddedToModel() {
			LayoutImageInfo imageProvider = new LayoutImageInfo(Element);

			imageProvider.ImageError = false;
			base.OnAddedToModel();
		}

		public override void OnXmlInfoChanged() {
			LayoutImageInfo imageProvider = new LayoutImageInfo(Element);

			imageProvider.ImageError = false;
			base.OnXmlInfoChanged();
		}

		public override bool DrawOutOfGrid {
			get {
				return true;
			}
		}


		public override String ToString() {
			return "Image";
		}
	}

	public abstract class LayoutTrackAnnotationComponent : ModelComponent {
		public LayoutStraightTrackComponent Track {
			get {
				return Spot.Track as LayoutStraightTrackComponent;
			}
		}
	}

	public class LayoutBridgeComponent : LayoutTrackAnnotationComponent {
		public LayoutBridgeComponent() {
			XmlDocument.LoadXml("<Bridge />");
		}

		public override ModelComponentKind Kind {
			get {
				return ModelComponentKind.Background;
			}
		}

		public override bool DrawOutOfGrid {
			get {
				return false;
			}
		}


		public override string ToString() {
			return "bridge";
		}
	}

	public class LayoutTunnelComponent : LayoutTrackAnnotationComponent {
		public LayoutTunnelComponent() {
			XmlDocument.LoadXml("<Tunnel />");
		}

		public override ModelComponentKind Kind {
			get {
				return ModelComponentKind.Background;
			}
		}

		public override bool DrawOutOfGrid {
			get {
				return false;
			}
		}


		public override string ToString() {
			return "tunnel";
		}
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
		LayoutGateComponent component;

		public LayoutGateComponentInfo(LayoutGateComponent component, XmlElement element)
			: base(element) {
			this.component = component;
		}

		public bool ReverseLogic {
			get { return XmlConvert.ToBoolean(GetAttribute("ReverseLogic", "false")); }
			set { SetAttribute("ReverseLogic", value); }
		}
		/// <summary>
		/// Gate has sensor that sense when gate is open (train can pass)
		/// </summary>
		public bool HasGateOpenSensor {
			get { return XmlConvert.ToBoolean(GetAttribute("HasGateOpenSensor", "true")); }
			set { SetAttribute("HasGateOpenSensor", value);	}
		}

		/// <summary>
		/// State of gate open state when door is open
		/// </summary>
		public bool GateOpenSensorActiveState {
			get { return XmlConvert.ToBoolean(GetAttribute("GateOpenSensorActiveState", "true"));  }
			set { SetAttribute("GateOpenSensorActiveState", value);  }
		}

		/// <summary>
		/// Gate has sensor that sense when gate is closed
		/// </summary>
		public bool HasGateClosedSensor {
			get { return XmlConvert.ToBoolean(GetAttribute("HasGateClosedSensor", "false")); }
			set { SetAttribute("HasGateClosedSensor", value); }
		}

		/// <summary>
		/// State of gate close state when door is closed
		/// </summary>
		public bool GateClosedSensorActiveState {
			get { return XmlConvert.ToBoolean(GetAttribute("GateClosedSensorActiveState", "true")); }
			set { SetAttribute("GateClosedSensorActiveState", value); }
		}

		/// <summary>
		/// The time (in seconds) that the gate is opened/closed.
		/// </summary>
		/// <remarks>
		/// This time is used during layout simulation or for operation if gate does not have an appropriate sensor. If the gate
		/// has a sensor, than an error will be generated if the gate did not reach to the desired state after this amount of time.
		/// </remarks>
		public int OpenCloseTimeout {
			get { return XmlConvert.ToInt32(GetAttribute("OpenCloseTimeout", "10")); }
			set { SetAttribute("OpenCloseTimeout", value); }
		}

		/// <summary>
		/// Gate orientation
		/// </summary>
		public bool OpenUpOrLeft {
			get { return XmlConvert.ToBoolean(GetAttribute("OpenUpOrLeft")); }
			set { SetAttribute("OpenUpOrLeft", XmlConvert.ToString(value)); }
		}


	}

	public class LayoutGateComponent : LayoutTrackAnnotationComponent, IModelComponentHasName, IModelComponentConnectToControl, IModelComponentLayoutLockResource {
		LayoutLockRequest lockRequest;

		public LayoutGateComponent() {
			XmlDocument.LoadXml("<Gate OpenUpOrLeft=\"false\"/>");
		}

		public override ModelComponentKind Kind {
			get { return ModelComponentKind.Gate; }
		}

		public override bool DrawOutOfGrid {
			get { return false; }
		}

		public LayoutGateComponentInfo Info {
			get {
				return new LayoutGateComponentInfo(this, Element);
			}
		}

		/// <summary>
		/// The gate open/close (or transition state)
		/// </summary>
		public LayoutGateState GateState {
			get {
				if(LayoutController.IsOperationMode) {
					if(LayoutModel.StateManager.Components.Contains(Id))
						return (LayoutGateState)Enum.Parse(typeof(LayoutGateState), LayoutModel.StateManager.Components.StateOf(Id).GetAttribute("GateState"));
				}

				return LayoutGateState.Close;
			}

			set {
				EraseImage();
				LayoutModel.StateManager.Components.StateOf(Id).SetAttribute("OpenClearance", value.ToString());
				Redraw();

				if(value == LayoutGateState.Open)
					EventManager.Event(new LayoutEvent(this, "layout-lock-resource-ready"));
			}
		}

		public override string ToString() {
			return "gate";
		}


		#region IModelComponentConnectToControl Members

		public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions {
			get {
				var list = new List<ModelComponentControlConnectionDescription>();

				list.Add(new ModelComponentControlConnectionDescription("Relay,Solenoid", "GateControl", "Gate Open/Close command"));

				if(Info.HasGateOpenSensor)
					list.Add(new ModelComponentControlConnectionDescription("DryContact", "GateOpen", "Gate open sensor"));
				if(Info.HasGateClosedSensor)
					list.Add(new ModelComponentControlConnectionDescription("DryContact", "GateClosed", "Gate closed sensor"));

				return list;
			}
		}

		#endregion

		#region ILayoutLockResource Members

		public LayoutLockRequest LockRequest {
			get {
				return lockRequest;
			}

			set {
				if(lockRequest != null) {	// Was not locked
					if(value == null && GateState != LayoutGateState.Close)
						EventManager.Event(new LayoutEvent(this, "close-gate-request"));
				}

				lockRequest = value;
			}
		}

		public bool IsResourceReady() {
			if(GateState != LayoutGateState.Open)
				EventManager.Event(new LayoutEvent(this, "open-gate-request"));

			return GateState == LayoutGateState.Open;		// Resource is ready when the gate is open and train can pass
		}

		#endregion

		#region IObjectHasName Members

		public LayoutTextInfo NameProvider {
			get {
				return new LayoutTextInfo(this);
			}
		}

		#endregion
	}

	[LayoutModule("Standard Gate Drivers", UserControl=false)]
	class StandardGateDrivers : LayoutModuleBase {
		Dictionary<Guid, LayoutDelayedEvent> pendingEvents = new Dictionary<Guid, LayoutDelayedEvent>();

		[LayoutEvent("open-gate-request", SenderType=typeof(LayoutGateComponent))]
		private void openGateRequest(LayoutEvent e) {
			LayoutGateComponent gateComponent = (LayoutGateComponent)e.Sender;
			LayoutDelayedEvent pendingEvent;

			if(pendingEvents.TryGetValue(gateComponent.Id, out pendingEvent)) {
				pendingEvent.Cancel();
				pendingEvents.Remove(gateComponent.Id);
			}

			if(LayoutController.IsOperationSimulationMode)
				pendingEvents.Add(gateComponent.Id, EventManager.DelayedEvent(750, new LayoutEvent(gateComponent, "gate-is-open")));
			else if(LayoutController.IsOperationMode) {
				var gateControlReference = new ControlConnectionPointReference(LayoutModel.ControlManager.ConnectionPoints[gateComponent, "GateControl"]);
				int state = gateComponent.Info.ReverseLogic ? 0 : 1;

				EventManager.AsyncEvent(new LayoutEvent<ControlConnectionPointReference, int>("change-track-component-state-command", gateControlReference, state).SetCommandStation(gateControlReference));
				pendingEvents.Add(gateComponent.Id, EventManager.DelayedEvent(gateComponent.Info.OpenCloseTimeout*1000, new LayoutEvent(gateComponent, "gate-open-timeout")));
			}

			gateComponent.GateState = LayoutGateState.Opening;
		}


		[LayoutEvent("close-gate-request", SenderType=typeof(LayoutGateComponent))]
		private void closeGateRequest(LayoutEvent e) {
			LayoutGateComponent gateComponent = (LayoutGateComponent)e.Sender;
			LayoutDelayedEvent pendingEvent;

			if(pendingEvents.TryGetValue(gateComponent.Id, out pendingEvent)) {
				pendingEvent.Cancel();
				pendingEvents.Remove(gateComponent.Id);
			}

			if(LayoutController.IsOperationSimulationMode)
				pendingEvents.Add(gateComponent.Id, EventManager.DelayedEvent(750, new LayoutEvent(gateComponent, "gate-is-closed")));
			else if(LayoutController.IsOperationMode) {
				var gateControlReference = new ControlConnectionPointReference(LayoutModel.ControlManager.ConnectionPoints[gateComponent, "GateControl"]);
				int state = gateComponent.Info.ReverseLogic ? 1 : 0;

				EventManager.AsyncEvent(new LayoutEvent<ControlConnectionPointReference, int>("change-track-component-state-command", gateControlReference, state).SetCommandStation(gateControlReference));
				pendingEvents.Add(gateComponent.Id, EventManager.DelayedEvent(gateComponent.Info.OpenCloseTimeout * 1000, new LayoutEvent(gateComponent, "gate-close-timeout")));

			}

			gateComponent.GateState = LayoutGateState.Closing;
		}

		[LayoutEvent("gate-is-open", SenderType = typeof(LayoutGateComponent))]
		private void gateIsOpen(LayoutEvent e) {
			LayoutGateComponent gateComponent = (LayoutGateComponent)e.Sender;
			LayoutDelayedEvent pendingEvent;

			if(pendingEvents.TryGetValue(gateComponent.Id, out pendingEvent)) {
				pendingEvent.Cancel();
				pendingEvents.Remove(gateComponent.Id);
			}

			gateComponent.GateState = LayoutGateState.Open;
		}

		[LayoutEvent("gate-is-closed", SenderType = typeof(LayoutGateComponent))]
		private void gateIsClosed(LayoutEvent e) {
			LayoutGateComponent gateComponent = (LayoutGateComponent)e.Sender;
			LayoutDelayedEvent pendingEvent;

			if(pendingEvents.TryGetValue(gateComponent.Id, out pendingEvent)) {
				pendingEvent.Cancel();
				pendingEvents.Remove(gateComponent.Id);
			}

			gateComponent.GateState = LayoutGateState.Close;
		}

		[LayoutEvent("gate-open-timeout", SenderType=typeof(LayoutGateComponent))]
		private void gateOpenTimeout(LayoutEvent e) {
			var gateComponent = (LayoutGateComponent)e.Sender;

			if(gateComponent.Info.HasGateOpenSensor)
				Error(gateComponent, "Timeout while waiting for gate to open");
			else
				EventManager.Event(new LayoutEvent(gateComponent, "gate-is-open"));
		}

		[LayoutEvent("gate-close-timeout", SenderType=typeof(LayoutGateComponent))]
		private void gateCloseTimeout(LayoutEvent e) {
			var gateComponent = (LayoutGateComponent)e.Sender;

			if(gateComponent.Info.HasGateClosedSensor)
				Error(gateComponent, "Timeout while waiting for gate to close");
			else
				EventManager.Event(new LayoutEvent(gateComponent, "gate-is-closed"));
		}

		[LayoutEvent("control-connection-point-state-changed-notification")]
		private void controlConnectionPointStateChangedNotification(LayoutEvent e) {
			var connectionPointRef = (ControlConnectionPointReference)e.Sender;

			if(connectionPointRef.IsConnected) {
				ControlConnectionPoint connectionPoint = connectionPointRef.ConnectionPoint;
				var component = connectionPoint.Component as LayoutGateComponent;

				if(component != null) {
					bool state = (int)e.Info != 0;

					if(connectionPoint.Name == "GateOpen" && state == component.Info.GateOpenSensorActiveState)
						EventManager.Event(new LayoutEvent(component, "gate-is-open"));
					else if(connectionPoint.Name == "GateClosed" && state == component.Info.GateClosedSensorActiveState)
						EventManager.Event(new LayoutEvent(component, "gate-is-closed"));
				}
			}
		}
	}
}
