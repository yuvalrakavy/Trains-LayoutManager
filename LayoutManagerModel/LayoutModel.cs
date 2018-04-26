using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

using LayoutManager.Components;
using System.Runtime.Serialization;

namespace LayoutManager.Model {

	public enum ModelComponentKind {
		Background,
		Track,
		Annotation,
		BlockEdge,
		TrackIsolation,
		BlockInfo,
		PowerConnector,
		TrackLink,
		ControlComponent,
		Signal,
		Gate,		// Gate or road block
	}

	public enum LayoutPhase {
		Planned = 0x0001,			// Component is planned
		Construction = 0x0002,		// Component is under construction
		Operational = 0x0004,		// Component is operationl

		None = 0x0000,
		All = 0x0007,
		NotPlanned = 0x0006,
	}

	public enum LayoutReadXmlContext {
		LoadingModel,
		PasteComponents,
	}

	interface ILayoutXmlContext {
		LayoutReadXmlContext ReadXmlContext {
			get;
		}
	}

	public class OperationModeParameters {
		public bool Simulation { get; set; }
		public LayoutPhase Phases { get; set; }
	}

	/// <summary>
	/// A collection of policy ID. An object that is associated with policies has a member of this type
	/// with the IDs of the policy that should be active for that object.
	/// </summary>
	public class LayoutPolicyIdCollection: XmlCollection<Guid>, ICollection<Guid> {
		public LayoutPolicyIdCollection(XmlElement element) : base(element, "Policies") {
		}

		protected override XmlElement CreateElement(Guid item) {
			XmlElement itemElement = Element.OwnerDocument.CreateElement("Policy");

			itemElement.SetAttribute("PolicyID", XmlConvert.ToString(item));
			return itemElement;
		}

        protected override Guid FromElement(XmlElement itemElement) => XmlConvert.ToGuid(itemElement.GetAttribute("PolicyID"));

        public void CheckIntegrity(string scope) {
			IDictionary<Guid, LayoutPolicyInfo>	map = LayoutModel.StateManager.Policies(scope).IdToPolicyMap;
			List<Guid> removeList = new List<Guid>();

			foreach(Guid policyID in this)
				if(!map.ContainsKey(policyID))
					removeList.Add(policyID);

			foreach(Guid policyID in removeList)
				Remove(policyID);
		}
	}

	/// <summary>
	/// Base class for components to be included in the layout model.
	/// All model components should be derived from this class.
	/// </summary>
	public abstract class ModelComponent : LayoutObject, IComparable<ModelComponent>, IModelComponent {
		LayoutModelSpotComponentCollection _spot;

		/// <summary>
		/// Return the kind of this component. Each layer in a given spot may
		/// contain at most one component of a given kind
		/// </summary>
		public abstract ModelComponentKind Kind {
			get;
		}

		/// <summary>
		/// The in which this component is located on the layout model grid.
		/// Using the spot, you can gain access to other components which are
		/// located in other layers or of another kind in this location
		/// </summary>
		public LayoutModelSpotComponentCollection Spot {		// Implemented by ModelComponentBase
			internal set {
				_spot = value;
			}

			get {
				return _spot;
			}
		}

        /// <summary>
        /// The location (x, y) in which this component is located in the model
        /// grid.
        /// </summary>
        public Point Location => _spot.Location;

        /// <summary>
        /// Return the Z order of this component. When presented, components with smaller Z order
        /// should be shown behind those of a larger Z order.
        /// </summary>
        public int ZOrder => (int)Kind;

        /// <summary>
        /// Return whether this component has associated attributes
        /// </summary>
        public bool HasAttributes => new AttributesOwner(Element).HasAttributes;

        /// <summary>
        /// If a component has attributes associated with it, returns those attributes
        /// </summary>
        public AttributesInfo Attributes => new AttributesOwner(Element).Attributes;

        /// <summary>
        /// Fire an event to indicate that this component was changed
        /// </summary>
        public virtual void OnComponentChanged() {
			_spot.Area.OnComponentChanged(this);
		}

		/// <summary>
		/// Called when the component XML document is changed.
		/// </summary>
		public virtual void OnXmlInfoChanged() {
		}

		/// <summary>
		/// This method is called when the component is added to a model.
		/// </summary>
		/// <remarks>The default implementation is to do nothing.</remarks>
		/// <param name="e"></param>
		public virtual void OnAddedToModel() {
			EventManager.AddObjectSubscriptions(this);

			if(this is IObjectHasId) {
				if(LayoutModel.Component<ModelComponent>(this.Id, LayoutPhase.All) != null)
					this.Id = Guid.NewGuid();

				LayoutModel.Instance.AddComponentReference(this.Id, this);
			}

			EventManager.Event(new LayoutEvent(this, "added-to-model"));
		}

		/// <summary>
		/// This method is called when the component is removed from a model.
		/// </summary>
		/// <remarks>The default implementation is to do nothing.</remarks>
		/// <param name="e"></param>
		public virtual void OnRemovingFromModel() {

			if(this is IObjectHasId)
				LayoutModel.Instance.RemoveComponentReference(this.Id);

			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
			EventManager.Event(new LayoutEvent(this, "removed-from-model"));
		}

		/// <summary>
		/// Cause the component to redraw itself (send event to all views)
		/// </summary>
		public virtual void Redraw() {
			if(_spot != null)
				OnComponentChanged();
		}

		/// <summary>
		/// Called to erase this component image. This is needed if the component draw outside its
		/// grid location, and the image will be modified.
		/// </summary>
		public virtual void EraseImage() {
			_spot.Area.OnEraseComponentImage(this);
		}

        #region Support for connected components

        /// <summary>
        /// Is this component connected to control module (note, if this connection has more than one control module
        /// connection, then this will be true if at least one connection is connected)
        /// </summary>
        public virtual bool IsConnected => LayoutModel.ControlManager.ConnectionPoints.IsConnected(Id);

        /// <summary>
        /// True if all control module connection of this component are connected to control modules
        /// </summary>
        public virtual bool FullyConnected => LayoutModel.ControlManager.ConnectionPoints.IsFullyConnected(Id);

        /// <summary>
        /// No specific control module is requried
        /// </summary>
        public virtual string RequiredControlModuleTypeName => null;

        #endregion

        /// <summary>
        /// Write the inner part of a component XML
        /// </summary>
        /// <param name="w"></param>
        public void WriteInnerXml(XmlWriter w) {
			if(XmlInfo != null && XmlInfo.DocumentElement != null) {
				w.WriteStartElement("Info");
				XmlInfo.DocumentElement.WriteTo(w);
				w.WriteEndElement();
			}

			WriteXmlFields(w);
		}


		/// <summary>
		/// Write the XML representation of a the component. 
		/// </summary>
		/// <remarks>
		/// If a component stores all its information in
		/// XmlInfo, then there is no need to do anything more to store the component information. However
		/// if the component would like to save additional information, it must override the WriteXmlFields
		/// method.
		/// </remarks>
		/// <param name="w">Xml Writer to save the component in</param>
		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("Component");
			w.WriteAttributeString("Class", XmlConvert.EncodeName(GetType().FullName));
			WriteInnerXml(w);
			w.WriteEndElement();
		}

		/// <summary>
		/// Write any additional component fields.
		/// </summary>
		/// <remarks>The default implementation does not write any additional fields</remarks>
		/// <param name="w"></param>
		public virtual void WriteXmlFields(XmlWriter w) {
		}

		// On entry <Component>
		public void ReadXml(XmlReader r) {
			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();		// <Component>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Info") {
						r.Read();		// <Info>
						XmlNode	infoNode = XmlInfo.XmlDocument.ReadNode(r);

						XmlInfo.XmlDocument.RemoveAll();
						XmlInfo.XmlDocument.AppendChild(infoNode);

						r.Read();		// </Info>
					}
					else {
						// If the field was not parsed, skip it
						if(!ReadXmlField(r))
							r.Skip();
					}
				}
				r.Read();		// </Component>
			}
		}

        /// <summary>
        /// Parse a component field from the XML stream. On exit the reader should be on
        /// an EndElement node.
        /// </summary>
        /// <remarks>The default implementation read no fields</remarks>
        /// <param name="r"></param>
        /// <returns>True if the field was parsed</returns>
        protected virtual bool ReadXmlField(XmlReader r) => false;

        public virtual void Error(Object subject, String message) {
			EventManager.Event(new LayoutEvent(subject, "add-error", null, message));
		}

		public void Error(String message) {
			Error(this, message);
		}

		public virtual void Warning(Object subject, String message) {
			EventManager.Event(new LayoutEvent(subject, "add-warning", null, message));
		}

		public void Warning(String message) {
			Warning(this, message);
		}

		public String FullDescription {
			get {
				string	location = (Spot == null) ? "Not placed" : "at " + Spot.Area.Name + ": " + Spot.Location;
				string phaseName = Spot == null ? LayoutModel.Instance.DefaultPhase.ToString() : Spot.Phase.ToString();
                string typeName = this.GetType().Name;
				LayoutTextInfo	nameProvider = new LayoutTextInfo(this);

                if (typeName.StartsWith("Layout"))
                    typeName = typeName.Substring("Layout".Length);       // Strip out the Layout
                if (typeName.EndsWith("Component"))
                    typeName = typeName.Remove(typeName.Length - "Component".Length);

                return $"{typeName} {location} {(phaseName != "Operational" ? ($"[{phaseName}]") : "")}{(!string.IsNullOrWhiteSpace(nameProvider.Name) ? ($" ({nameProvider.Name})") : "")}";
            }
		}

        public override String ToString() => FullDescription;

        public abstract bool DrawOutOfGrid {
			get;
		}

        #region IComparable<ModelComponent> Members

        /// <summary>
        /// Find out if another component is "above" this component
        /// </summary>
        /// <param name="objModelComponent">The component to compare to</param>
        /// <returns>+n if this component is above, 0 if same, -n if below</returns>
        public int CompareTo(ModelComponent other) => Kind - other.Kind;

        public bool Equals(ModelComponent other) => this == other;

        #endregion
    }

	[Serializable]
	public struct LayoutComponentConnectionPoint : IComparable<LayoutComponentConnectionPoint> {
		int		cp;

		public LayoutComponentConnectionPoint(int cpValue) {
			cp = cpValue;
		}

        public static implicit operator int (LayoutComponentConnectionPoint v) => v.cp;

        public static implicit operator LayoutComponentConnectionPoint(int v) => new LayoutComponentConnectionPoint(v);

        // Default value for standard connection points

        public const int Empty = -1;
		public const int T = 0;
		public const int B = 1;
		public const int R = 2;
		public const int L = 3;

		public static LayoutComponentConnectionPoint Parse(string t) {
			if(t == "T")
				return T;
			else if(t == "B")
				return B;
			else if(t == "R")
				return R;
			else if(t == "L")
				return L;
			else
				return Int32.Parse(t);
		}

		public override string ToString() {
			switch(cp) {
				case -1:	return "Empty";
				case 0:		return "T";
				case 1:		return "B";
				case 2:		return "R";
				case 3:		return "L";
				default:	return cp.ToString();
			}
		}


		public override bool Equals(object obj) {
			if(obj != null) {
				LayoutComponentConnectionPoint other = (LayoutComponentConnectionPoint)obj;

				return Equals(other);
			}
			return false;
		}

        public static bool operator ==(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) => cp1.Equals(cp2);

        public static bool operator !=(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) => !cp1.Equals(cp2);

        public override int GetHashCode() => cp.GetHashCode();

        #region IComparable<LayoutComponentConnectionPoint> Members

        public int CompareTo(LayoutComponentConnectionPoint other) => cp - other.cp;

        public bool Equals(LayoutComponentConnectionPoint other) => cp == other.cp;

        #endregion
    }

	public class LayoutComponentConnectionPointConverter : System.ComponentModel.TypeConverter {
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType) => destinationType == typeof(string);

        public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value is LayoutComponentConnectionPoint)
				return ((LayoutComponentConnectionPoint)value).ToString();
			throw new NotSupportedException("Cannot LayoutComponentConnectionPoint convert to " + destinationType.FullName);
		}
	}


	/// <summary>
	/// Combination of a track component and a connection point. This is an is used to
	/// describe a topological pass of the track. Some track component (for example
	/// two parallel diagonal tracks) contains more than one topological pass. The combination
	/// of a track component and a connection point, describe a unique pass.
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(TrackEdgeConverter))]
	public struct TrackEdge : IComparable<TrackEdge> {
		LayoutTrackComponent			track;
		LayoutComponentConnectionPoint	cp;

		public static readonly TrackEdge Empty = new TrackEdge((LayoutTrackComponent)null, 0);

		public TrackEdge(LayoutTrackComponent track, LayoutComponentConnectionPoint cp) {
			this.track = track;
			this.cp = cp;
		}

		public TrackEdge(IModelComponentIsMultiPath track, LayoutComponentConnectionPoint cp)
			: this((LayoutTrackComponent)track, cp) {
		}

        public LayoutTrackComponent Track => track;

        public IModelComponentIsMultiPath Split => (IModelComponentIsMultiPath)Track;

        public LayoutComponentConnectionPoint ConnectionPoint => cp;

        public static bool operator ==(TrackEdge edge1, TrackEdge edge2) => edge1.Equals(edge2);

        public static bool operator !=(TrackEdge edge1, TrackEdge edge2) => !edge1.Equals(edge2);

        public static bool operator <(TrackEdge edge1, TrackEdge edge2) => edge1.CompareTo(edge2) < 0;

        public static bool operator >(TrackEdge edge1, TrackEdge edge2) => edge1.CompareTo(edge2) > 0;

        public override bool Equals(object obj) {
			if(obj != null) {
				TrackEdge other = (TrackEdge)obj;

				return Equals(other);
			}
			return false;
		}

        public override int GetHashCode() => ((ModelComponent)track).GetHashCode();

        public override String ToString() => ((LayoutTrackComponent)Track).FullDescription + " (" + cp + ")";

        public LayoutComponentConnectionPoint OtherConnectionPoint {
			get {
				IModelComponentIsMultiPath multiPath = Track as IModelComponentIsMultiPath;

				if(multiPath != null)
					return multiPath.ConnectTo(ConnectionPoint, multiPath.CurrentSwitchState);
				else
					return Track.ConnectTo(cp, LayoutComponentConnectionType.Topology)[0];
			}
		}

        public TrackEdge OtherSide => new TrackEdge(Track, OtherConnectionPoint);

        #region IComparable<TrackEdge> Members

        /// <summary>
        /// Compare this object to another TrackEdge object. The two objects
        /// are considered to be equal if they describe the same topological
        /// pass in the same track component
        /// </summary>
        /// <param name="edgeObj">The TrackEdge to compare to</param>
        /// <returns>0 if the two objects are equal, non-zero otherwise</returns>
        public int CompareTo(TrackEdge other) {
			if(other.track == track)
				return ConnectionPoint - other.ConnectionPoint;
			else {
				if(track.Location.X == other.track.Location.X)
					return track.Location.Y - other.track.Location.Y;
				return track.Location.X - other.track.Location.X;
			}
		}

        public bool Equals(TrackEdge other) => track == other.track && cp == other.cp;

        #endregion
    }

	/// <summary>
	/// A combination of a track ID and a connection point. Similar to track edge, but instead of holding the track component, 
	/// hold its ID
	/// </summary>
	public struct TrackEdgeId {
		Guid							trackId;
		LayoutComponentConnectionPoint	cp;

		public static readonly TrackEdgeId Empty = new TrackEdgeId(Guid.Empty, 0);

		public TrackEdgeId(Guid trackID, LayoutComponentConnectionPoint cp) {
			this.trackId = trackID;
			this.cp = cp;
		}

		public TrackEdgeId(LayoutTrackComponent track, LayoutComponentConnectionPoint cp) {
			this.trackId = track.Id;
			Debug.Assert(track.HasConnectionPoint(cp));

			this.cp = cp;
		}

		public TrackEdgeId(TrackEdge edge) {
			trackId = edge.Track.Id;
			cp = edge.ConnectionPoint;
		}

        public Guid TrackId => trackId;

        public LayoutComponentConnectionPoint ConnectionPoint => cp;

        public override bool Equals(object obj) {
			if(obj == null || !(obj is TrackEdgeId))
				return false;

			TrackEdgeId	other = (TrackEdgeId)obj;
			return trackId == other.trackId && cp == other.ConnectionPoint;
		}

        public static bool operator ==(TrackEdgeId edge1, TrackEdgeId edge2) => edge1.Equals(edge2);

        public bool IsEmpty => trackId == Guid.Empty || LayoutModel.Component<LayoutTrackComponent>(trackId, LayoutModel.ActivePhases) == null;

        public static bool operator !=(TrackEdgeId edge1, TrackEdgeId edge2) => !edge1.Equals(edge2);

        public TrackEdge ToTrackEdge() => new TrackEdge(LayoutModel.Component<LayoutTrackComponent>(trackId, LayoutModel.ActivePhases), cp);

        public new string ToString() => ToTrackEdge().ToString();

        public override int GetHashCode() => TrackId.GetHashCode() ^ cp.GetHashCode();
    }

	public class TrackEdgeDictionary : Dictionary<TrackEdge, object>, IEnumerable<TrackEdge> {

		public void Add(TrackEdge edge) {
			Add(edge, null);
		}

        public bool Contains(TrackEdge edge) => ContainsKey(edge);

        #region IEnumerable<TrackEdge> Members

        public new IEnumerator<TrackEdge> GetEnumerator() => Keys.GetEnumerator();

        #endregion
    }



	public class TrackEdgeConverter : System.ComponentModel.TypeConverter {
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return false;
		}

		public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value is TrackEdge)
				return ((TrackEdge)value).ToString();
			throw new NotSupportedException("Cannot TrackEdge convert to " + destinationType.FullName);
		}
	}


	/// <summary>
	/// A list of TrackEdges
	/// </summary>
	public class TrackEdgeCollection : List<TrackEdge> {
		public void Add(LayoutTrackComponent track, LayoutComponentConnectionPoint cp) {
			Add(new TrackEdge(track, cp));
		}
	}

	/// <summary>
	/// Standard implementation of power class
	/// </summary>
	public class LayoutPower : ILayoutPower {
		public LayoutPower(IModelComponentHasPowerOutlets originComponent, LayoutPowerType type, DigitalPowerFormats digitalFormats, String name) {
			this.PowerOriginComponent = originComponent;
			this.Type = type;
			this.DigitalFormats = digitalFormats;
			this.Name = name;
		}

		#region ILayoutPower Members

		public LayoutPowerType Type {
            get;

        }

        public DigitalPowerFormats DigitalFormats {
            get;

        }

        public String Name {
            get;

        }

        public Guid PowerOriginComponentId => PowerOriginComponent.Id;

        public IModelComponentHasPowerOutlets PowerOriginComponent {
            get;

        }

        #endregion
    }

    public class LayoutPowerOutlet : LayoutInfo, ILayoutPowerOutlet {
        ILayoutPower power;

        public IEnumerable<ILayoutPower> ObtainablePowers { get; }

        public LayoutPowerOutlet(IModelComponentHasPowerOutlets component, string outletDescription, IEnumerable<ILayoutPower> obtainablePowers) {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            Element = doc.CreateElement("PowerOutlet");
            doc.AppendChild(Element);

            OutletDescription = outletDescription;
            ObtainablePowers = obtainablePowers;
            OutletComponent = component;
        }

        public IModelComponentHasPowerOutlets OutletComponent {get; }

		public LayoutPowerOutlet(IModelComponentHasPowerOutlets component, string outletDescription, ILayoutPower obtainablePower)
			: this(component, outletDescription, Array.AsReadOnly(new ILayoutPower[] { obtainablePower })) {
		}

		public string OutletDescription {
			get {
				return GetAttribute("OutletDescription");
			}

			set {
				SetAttribute("OutletDescription", value);
			}
		}

		public ILayoutPower Power {
			get {
				return power;
			}

			set {
				power = value;
				EventManager.Event(new LayoutEvent<ILayoutPowerOutlet, ILayoutPower>("power-outlet-changed-state-notification", this, power));
			}
		}

        public bool SelectPower(LayoutPowerType powerType, IList<SwitchingCommand> switchingCommands) => Power != null && powerType == Power.Type;
    }

	public class LayoutComponentPowerOutletDescription {
		IModelComponentHasPowerOutlets component;
		int outletIndex;

		public LayoutComponentPowerOutletDescription(IModelComponentHasPowerOutlets component, int outletIndex) {
			this.component = component;
			this.outletIndex = outletIndex;
		}

        public IModelComponentHasPowerOutlets Component => this.component;

        public int OutletIndex => this.outletIndex;

        public ILayoutPowerOutlet PowerOutlet => component.PowerOutlets[this.outletIndex];

        public override string ToString() => component.NameProvider.Name + " (" + PowerOutlet.OutletDescription + ")";
    }

	/// <summary>
	/// A power inlet to which another component power outlet can be connected
	/// </summary>
	public class LayoutPowerInlet : LayoutInfo, ILayoutPowerInlet {

		public LayoutPowerInlet(ModelComponent component, string elementName) : base(component, elementName) {
			if(Element == null)
				Element = CreateProviderElement(component, elementName);
		}

		public LayoutPowerInlet(XmlElement containerElement, string name)
			: base(containerElement, name) {
				if(Element == null)
					Element = CreateProviderElement(containerElement, name);
		}

		#region ILayoutPowerInlet Members

		/// <summary>
		/// The component Id to which this inlet is connected. If this inlet is not connected then get/set ID to Guid.Empty
		/// </summary>
		public Guid OutletComponentId {
			get {
				if(HasAttribute("OutletId"))
					return XmlConvert.ToGuid(GetAttribute("OutletId"));
				else
					return Guid.Empty;
			}

			set {
                SetAttribute("OutletId", value, removeIf: Guid.Empty);
			}
		}

		/// <summary>
		/// The component to which this inlet is connected (null if inlet is not connected)
		/// </summary>
		public IModelComponentHasPowerOutlets GetOutletComponent(LayoutPhase phase) {
			if(OutletComponentId == Guid.Empty)
				return null;
			else
				return LayoutModel.Component<IModelComponentHasPowerOutlets>(OutletComponentId, phase);
		}

		/// <summary>
		/// The component to which this inlet is connected (null if inlet is not connected)
		/// </summary>
		public IModelComponentHasPowerOutlets OutletComponent {
			get {
				if(OutletComponentId == Guid.Empty)
					return null;
				else
					return LayoutModel.Component<IModelComponentHasPowerOutlets>(OutletComponentId, LayoutModel.ActivePhases);
			}

			set {
				if(value == null)
					OutletComponentId = Guid.Empty;
				else
					OutletComponentId = value.Id;
			}
		}

		/// <summary>
		/// A component with power outlets may have more than 1 outlet. This property contains the outlet index to which this inlet is connected
		/// </summary>
		public int OutletIndex {
			get {
				return XmlConvert.ToInt32(GetAttribute("OutletIndex"));
			}

			set {
				Debug.Assert(OutletComponent != null);
				SetAttribute("OutletIndex", XmlConvert.ToString(value));
			}
		}

		/// <summary>
		/// The power outlet to which this inlet is connected
		/// </summary>
		public ILayoutPowerOutlet ConnectedOutlet {
			get {
				IModelComponentHasPowerOutlets componentWithPowerOutlets = OutletComponent;

				if(componentWithPowerOutlets == null)
					return null;
				else
					return componentWithPowerOutlets.PowerOutlets[OutletIndex];
			}
		}

        /// <summary>
        /// Is this inlet connected
        /// </summary>
        public bool IsConnected => OutletComponent != null;

        public override string ToString() {
			if(IsConnected) {
				if(OutletComponent.PowerOutlets.Count == 1)
					return OutletComponent.NameProvider.Name;
				else // More than one outlet, show return the outlet name
					return OutletComponent.NameProvider.Name + " (" + ConnectedOutlet.OutletDescription + ")";
			}
			else
				return "[Not connected]";
		}

		#endregion
	}

	public class ModelComponentCollection : List<ModelComponent> {
	}

	/// <summary>
	/// This class represents one spot in the model. It may represent one than
	/// one layer of components. In a given layer, components are arranged by kind.
	/// That is a given spot in a given layer may contain at most one track, one signal,
	/// on track contact etc.
	/// </summary>
	public class LayoutModelSpotComponentCollection : IEnumerable<ModelComponent> {
		ModelComponentCollection spotComponents = new ModelComponentCollection();
		bool sorted;
	
		/// <summary>
		/// Construct a new spot object
		/// </summary>
		/// <param name="model">The layout model to which this spot belongs</param>
		/// <param name="p">The location of this spot</param>
		public LayoutModelSpotComponentCollection(LayoutModelArea area, Point p, LayoutPhase phase) {
			this.Location = p;
			this.Area = area;
			this.Phase = phase;
		}

        /// <summary>
        /// Get the component of a given kind in a given layer
        /// </summary>
        /// <example>c = aSpot[ModelComponentKind.Track];</example>
        public ModelComponent this[ModelComponentKind kind] => spotComponents.Find(delegate (ModelComponent c) { return c.Kind == kind; });

        /// <summary>
        /// Get the track in the current spot
        /// </summary>
        /// <value>Track component or null if this spot does not have track component</value>
        public LayoutTrackComponent Track => (LayoutTrackComponent)this[ModelComponentKind.Track];

        /// <summary>
        /// Collection of all the components in this spot
        /// </summary>
        /// <value></value>
        public ModelComponentCollection Components => spotComponents;

        /// <summary>
        /// Current draw level
        /// </summary>
        public int DrawLevel {get; set; }

		/// <summary>
		/// Planned/UnderConstruction/Operation phase of this spot
		/// </summary>
		public LayoutPhase Phase { get; set; }


		public bool DrawOutOfGrid {
			get {
				foreach(ModelComponent component in spotComponents)
					if(component.DrawOutOfGrid)
						return true;

				return false;
			}
		}

		/// <summary>
		/// Add a model component to the spot
		/// </summary>
		/// <param name="c">The component to be added</param>
		public void Add(ModelComponent c) {
			ModelComponent	old = this[c.Kind];

			if(old != null) {
				old.EraseImage();
				Remove(old);
			}

			c.Spot = this;
			spotComponents.Add(c);
			sorted = false;

			c.OnAddedToModel();
		}

		/// <summary>
		/// Remove a component from a grid spot
		/// </summary>
		/// <param name="c">The component to be removed</param>
		public void Remove(ModelComponent c) {
			c.OnRemovingFromModel();
			spotComponents.Remove(c);
			Area.OnComponentDeleted(this, c);
		}

		/// <summary>
		/// The location of this spot in the model grid
		/// </summary>
		public Point Location { get; private set; }

		/// <summary>
		/// Return the model to which this spot belongs
		/// </summary>
		public LayoutModelArea Area { get; }

        /// <summary>
        /// Relocate this spot to a new location. The new location must be empty
        /// </summary>
        /// <param name="newLocation"></param>
        public void MoveTo(Point newLocation) {
			Debug.Assert(Area.Grid.ContainsKey(Location));

			Area.RemoveSpotLocation(this);
			Area.Grid.Remove(Location);
			Location = newLocation;
			Area.Grid.Add(Location, this);
			Area.AddSpotLocation(this);
		}

		// Implementation of IEnumerable

		/// <summary>
		/// Return an enumerator that will enumerate the components starting from the
		/// most bottom layer (and the most bottom components of this layer) upwards
		/// </summary>
		public IEnumerator<ModelComponent> GetEnumerator() {
			if(!sorted) {
				spotComponents.Sort();
				sorted = true;
			}

			return spotComponents.GetEnumerator();
		}

        // Implementation of ICollection<ModelComponent>

        /// <summary>
        /// Return the number of components in this spot
        /// </summary>
        public int Count => spotComponents.Count;

        /// <summary>
        /// Copy the model components in this spot to an array
        /// </summary>
        /// <param name="a"></param>
        /// <param name="i"></param>
        public void CopyTo(ModelComponent[] a, int i) {
			spotComponents.CopyTo(a, i);
		}

		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("Location");
			w.WriteAttributeString("x", XmlConvert.ToString(Location.X));
			w.WriteAttributeString("y", XmlConvert.ToString(Location.Y));

			if(Phase != LayoutPhase.Operational)
				w.WriteAttributeString("Phase", Phase.ToString());

			foreach(ModelComponent component in spotComponents)
				component.WriteXml(w);

			w.WriteEndElement();
		}

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

	/// <summary>
	/// Track links in this area.
	/// </summary>
	public class LayoutAreaTrackLinksDictionary : Dictionary<Guid, LayoutTrackLinkComponent>, IEnumerable<LayoutTrackLinkComponent> {
		public void Add(LayoutTrackLinkComponent trackLink) {
			base.Add(trackLink.TrackLinkGuid, trackLink);
		}

		public void Remove(LayoutTrackLinkComponent trackLink) {
			base.Remove(trackLink.TrackLinkGuid);
		}

        public new IEnumerator<LayoutTrackLinkComponent> GetEnumerator() => this.Values.GetEnumerator();
    }

	public class LayoutModelArea {
		String				name;							// The area's name
		Dictionary<Point, LayoutModelSpotComponentCollection> grid = new Dictionary<Point, LayoutModelSpotComponentCollection>();				// grid of spots
		Rectangle			mlBounds = new Rectangle();		// Model bounds
		bool				recalcBounds = true;			// need to recalculate bounds
		SortedVector<SortedVector<LayoutModelSpotComponentCollection>> sortedRows = new SortedVector<SortedVector<LayoutModelSpotComponentCollection>>();
		List<LayoutModelSpotComponentCollection> outOfGridSpots = new List<LayoutModelSpotComponentCollection>();	// Spots which have at least one component that is drawn out of its grid
		Guid				id;								// The area ID

		// All track links found in this area
		LayoutAreaTrackLinksDictionary	trackLinks = new LayoutAreaTrackLinksDictionary();

		/// <summary>
		/// Event that is fired when a compomnent changes
		/// </summary>
		public event EventHandler AreaChanged;
		public event EventHandler EraseComponentImage;
		public event EventHandler AreaBoundsChanged;

		public LayoutModelArea() {
			id = Guid.NewGuid();
		}

		#region Property accessors

		public String Name {
			get {
				return name;
			}

			set {
				name = value;
				LayoutModel.Areas.OnAreaRenamed(this);
			}
		}

        /// <summary>
        /// Return the layout grid. The grid is a hashtable in which the keys are points providing the
        /// location on the grid, and the values are LayoutModelSpot which contains all the components that
        /// are in the given location.
        /// </summary>
        public IDictionary<Point, LayoutModelSpotComponentCollection> Grid => grid;

        /// <summary>
        /// Unique identifier for this area
        /// </summary>
        public Guid AreaGuid => id;

        /// <summary>
        /// Get a hashtable containing all the track links that are in this area.
        /// </summary>
        public LayoutAreaTrackLinksDictionary TrackLinks => trackLinks;

        public SortedVector<SortedVector<LayoutModelSpotComponentCollection>> SortedRows => sortedRows;

        public IList<LayoutModelSpotComponentCollection> OutOfGridSpots => outOfGridSpots;

        #endregion

        #region Indexers for accessing spots or components

        /// <summary>
        /// Return a layout spot (all components in a given location, regardless of layer
        /// or their kind)
        /// </summary>
        public LayoutModelSpotComponentCollection this[Point p, LayoutPhase phase] {
			get {
				LayoutModelSpotComponentCollection	spot;

				grid.TryGetValue(p, out spot);

				Debug.Assert(spot == null || spot.Location == p, "Spot location != grid location");

				if(spot != null && (spot.Phase & phase) == 0)
					spot = null;

				return spot;
			}
		}

		public LayoutModelSpotComponentCollection this[Point p] {
			set {
				grid[p] = value;
			}
		}

		/// <summary>
		/// Get the phase (design/construction/opertional) of a given location
		/// </summary>
		/// <param name="p">Location</param>
		/// <returns>Phase of this location</returns>
		public LayoutPhase Phase(Point p) {
			if(grid.ContainsKey(p))
				return this[p, LayoutPhase.All].Phase;
			else
				return LayoutModel.Instance.DefaultPhase;
		}

        /// <summary>
        /// Return the phases (operational, in-construction, planned) used in this area
        /// </summary>
        public LayoutPhase AreaPhases => Grid.Values.Aggregate<LayoutModelSpotComponentCollection, LayoutPhase>(LayoutPhase.None, (p, spot) => p | spot.Phase);

        #endregion

        #region Adding/Removing spots and components

        public void AddSpotLocation(LayoutModelSpotComponentCollection spot) {
			Point	p = spot.Location;

			SortedVector<LayoutModelSpotComponentCollection> sortedColumns;

			if(!sortedRows.TryGetValue(p.Y, out sortedColumns)) {
				sortedColumns = new SortedVector<LayoutModelSpotComponentCollection>();
				sortedRows[p.Y] = sortedColumns;
			}

			sortedColumns[p.X] = spot;
		}

		public void RemoveSpotLocation(LayoutModelSpotComponentCollection spot) {
			SortedVector<LayoutModelSpotComponentCollection>	sortedColumns = sortedRows[spot.Location.Y];

			sortedColumns.Remove(spot.Location.X);
			if(sortedColumns.Count == 0)
				sortedRows.Remove(spot.Location.Y);

			if(outOfGridSpots.Contains(spot))
				outOfGridSpots.Remove(spot);
		}

		/// <summary>
		/// Set or add component to given location in the grid (without generating
		/// any event)
		/// </summary>
		/// <param name="p"></param>
		/// <param name="c"></param>
		void addNoEvent(Point p, ModelComponent c, LayoutPhase phase) {
			LayoutModelSpotComponentCollection	spot;

			if(!grid.TryGetValue(p, out spot)) {
				spot = new LayoutModelSpotComponentCollection(this, p, phase);
				this[p] = spot;			// Add to the grid

				AddSpotLocation(spot);

				if(adjustBounds(p))		// Adjust the model bounds
					OnAreaBoundsChanged();
			}

			spot.Add(c);

			if(c.DrawOutOfGrid && !outOfGridSpots.Contains(spot))
				outOfGridSpots.Add(spot);
		}

		/// <summary>
		/// Set or add a model component to a given location and layer in the grid
		/// </summary>
		public void Add(Point p, ModelComponent c, LayoutPhase phase) {
			addNoEvent(p, c, phase);
			OnComponentChanged(c);
		}

		/// <summary>
		/// Remove a spot from the model. All the components in the spot are
		/// removed as well
		/// </summary>
		/// <param name="spot">The spot to be removed</param>
		public void RemoveSpot(LayoutModelSpotComponentCollection spot) {
			foreach(ModelComponent c in spot) {
				spot.Remove(c);
			}

			if(adjustBounds(spot.Location)) {
				recalcBounds = true;			// Removed a spot that was on the model bounding rectangle
				OnAreaBoundsChanged();
			}

			// need to calculate the bounding rect

			grid.Remove(spot.Location);
			RemoveSpotLocation(spot);
		}

		#endregion

		#region Area boundries handling

		/// <summary>
		/// Return a rectangle that bounds all components in the model
		/// </summary>
		public Rectangle Bounds {
			get {
				if(recalcBounds)
					calculateAreaBounds();

				return mlBounds;
			}
		}

		/// <summary>
		/// Ensure that the model boundries contain the given point
		/// </summary>
		/// <param name="ml"></param>
		/// <returns>True if model boundries were modified</returns>
		bool adjustBounds(Point ml) {
			bool adjusted = false;
			int top = mlBounds.Top;
			int bottom = mlBounds.Bottom;
			int left = mlBounds.Left;
			int right = mlBounds.Right;

			if(ml.X <= mlBounds.Left) {
				left = ml.X;
				adjusted = true;
			}
			
			if(ml.X >= mlBounds.Right) {
				right = ml.X;
				adjusted = true;
			}

			if(ml.Y <= mlBounds.Top) {
				top = ml.Y;
				adjusted = true;
			}

			if(ml.Y >= mlBounds.Bottom) {
				bottom = ml.Y;
				adjusted = true;
			}

			if(adjusted)
				mlBounds = Rectangle.FromLTRB(left, top, right, bottom);

			return adjusted;
		}

		/// <summary>
		/// Calcuate the rectangle that bounds the model
		/// </summary>
		void calculateAreaBounds() {
			mlBounds = new Rectangle(0, 0, 0, 0);

			foreach(LayoutModelSpotComponentCollection spot in grid.Values)
				adjustBounds(spot.Location);

			recalcBounds = false;		// Bounds are calculated
		}

		#endregion

		#region Methods to wire events to the model

		/// <summary>
		/// Called to indicate that a component was changed
		/// </summary>
		/// <param name="c">The component that was changed</param>
		internal void OnComponentChanged(ModelComponent c) {
			if(AreaChanged != null)
				AreaChanged(c, new EventArgs());

			if(c.DrawOutOfGrid && !outOfGridSpots.Contains(c.Spot))
				outOfGridSpots.Add(c.Spot);
			else if(!c.Spot.DrawOutOfGrid && outOfGridSpots.Contains(c.Spot))
				outOfGridSpots.Remove(c.Spot);
		}

		/// <summary>
		/// Called to inidcate that a new component was added
		/// </summary>
		/// <param name="c">The component that was added</param>
		internal void OnComponentAdded(ModelComponent c) {
			if(AreaChanged!= null)
				AreaChanged(c, new EventArgs());

			if(c.DrawOutOfGrid && !outOfGridSpots.Contains(c.Spot))
				outOfGridSpots.Add(c.Spot);
		}

		/// <summary>
		/// Called when a component was deleted
		/// </summary>
		/// <param name="spot">The spot from which the component is deleted</param>
		/// <param name="component">The component that was deleted</param>
		internal void OnComponentDeleted(LayoutModelSpotComponentCollection spot, ModelComponent component) {
			if(AreaChanged != null)
				AreaChanged(spot, new EventArgs());

			if(!spot.DrawOutOfGrid)
				outOfGridSpots.Remove(spot);
		}

		/// <summary>
		/// Erase the image of a component. This should be called when a component would like
		/// to erase its current image (for example, when it is deleted or modified). Explicit
		/// erasing of the component image is needed only for components that draw outside their
		/// grid location (for example, label component)
		/// </summary>
		/// <param name="component"></param>
		internal void OnEraseComponentImage(ModelComponent component) {
			if(EraseComponentImage != null)
				EraseComponentImage(component, new EventArgs());
		}

		internal void OnAreaBoundsChanged() {
			if(AreaBoundsChanged != null)
				AreaBoundsChanged(this, new EventArgs());
		}

		/// <summary>
		/// Redraw the whole area
		/// </summary>
		public void Redraw() {
			if(AreaChanged != null)
				AreaChanged(this, new EventArgs());
		}


		#endregion

		#region XML saving/loading

		void writeXmlGrid(XmlWriter w) {
			w.WriteStartElement("Grid");
			foreach(LayoutModelSpotComponentCollection spot in grid.Values)
				spot.WriteXml(w);
			w.WriteEndElement();
		}

		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("Area");
			w.WriteAttributeString("Name", XmlConvert.EncodeName(Name));
			w.WriteAttributeString("ID", XmlConvert.EncodeName(id.ToString()));

			writeXmlGrid(w);
			w.WriteEndElement();
		}

		// On entry <Location> on exit </Location>
		void parseLocation(XmlReader r, Point ml, LayoutPhase phase) {
			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();		// <Location>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Component") {
						String			componentTypeName = XmlConvert.DecodeName(r.GetAttribute("Class"));

						// Patch for backward compatability
						if(componentTypeName == "LayoutManager.Components.LayoutBlockInfoComponent")
							componentTypeName = "LayoutManager.Components.LayoutBlockDefinitionComponent";

						ModelComponent component = LayoutModel.CreateModelComponent(componentTypeName);

						if(component != null) {
							if(!r.IsEmptyElement)
								component.ReadXml(r);
							else
								r.Read();

							addNoEvent(ml, component, phase);			// Add the component to the grid
						}
						else {
							// TODO: Generate a warning that component implementation was not found
							Debug.WriteLine("Implementation for component: " + componentTypeName + " not found, component module probably not loaded");
						}
					}
					else
						r.Skip();
				}

				r.Read();
			}
		}

		// On entry <Grid> on exit </Grid>
		void parseGrid(XmlReader r) {
			grid.Clear();

			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();		// <Grid>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Location") {
						string phaseAttributeValue = r.GetAttribute("Phase");
						LayoutPhase phase = phaseAttributeValue == null ? LayoutPhase.Operational : (LayoutPhase)Enum.Parse(typeof(LayoutPhase), phaseAttributeValue);

						parseLocation(r, new Point(XmlConvert.ToInt32(r.GetAttribute("x")), XmlConvert.ToInt32(r.GetAttribute("y"))), phase);
					}
					else
						r.Skip();
				}

				r.Read();
			}
		}

		// On entry <Area> on exit </Area>
		public void ReadXml(XmlReader r) {
			Name = XmlConvert.DecodeName(r.GetAttribute("Name"));
			id = new Guid(XmlConvert.DecodeName(r.GetAttribute("ID")));

			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();			// <Area>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Grid")
						parseGrid(r);
					else
						r.Skip();
				}

				r.Read();	// </Area>

				if(AreaChanged != null)
					AreaChanged(this, new EventArgs());
			}
		}
	
		#endregion
	}

	public class LayoutModelAreaDictionary : Dictionary<Guid, LayoutModelArea>, IEnumerable<LayoutModelArea> {
		public event EventHandler AreaAdded;
		public event EventHandler AreaRemoved;
		public event EventHandler AreaRenamed;

		public LayoutModelAreaDictionary() {
		}

		public void Add(LayoutModelArea area) {
			Add(area.AreaGuid, area);
			OnAreaAdded(area);
		}

		public void Remove(LayoutModelArea area) {
			Remove(area.AreaGuid);
			OnAreaRemoved(area);
		}

		protected void OnAreaAdded(LayoutModelArea area) {
			if(AreaAdded != null)
				AreaAdded(area, new EventArgs());
		}

		protected void OnAreaRemoved(LayoutModelArea area) {
			if(AreaRemoved != null)
				AreaRemoved(area, new EventArgs());
		}

		public void OnAreaRenamed(LayoutModelArea area) {
			if(AreaRenamed != null)
				AreaRenamed(area, new EventArgs());
		}

        public new IEnumerator<LayoutModelArea> GetEnumerator() => Values.GetEnumerator();
    }

	public class SortedVector<T> : SortedDictionary<int, T> {
        public ValueRange RangeValues(int from, int to) => new ValueRange(this, from, to);

        public class ValueRange : IEnumerable<T> {
			SortedVector<T> list;
			int from;
			int to;

			internal ValueRange(SortedVector<T> list, int from, int to) {
				this.list = list;
				this.from = from;
				this.to = to;
			}

			#region IEnumerable<T> Members

			public IEnumerator<T> GetEnumerator() {
				foreach(KeyValuePair<int, T> entry in list) {
					if(entry.Key > to)
						yield break;
					if(entry.Key >= from)
						yield return entry.Value;
				}
			}

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }
	}

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[LayoutModule("Layout Model", UserControl=false)]
	public class LayoutModel : ILayoutModuleSetup, IObjectHasXml, IDisposable {
		LayoutModelAreaDictionary			areas;
		LayoutModelBlockDictionary			blocks = new LayoutModelBlockDictionary();
		LayoutXmlInfo				modelXmlInfo = new LayoutXmlInfo();
		String						modelXmlInfoFilename;
		bool						modelIsLoading;
		Hashtable					componentReferences = new Hashtable();		// Component ID to component
		LayoutModule				module;
		LocomotiveCollectionInfo	locomotiveCollection;
		static LayoutModel			instance;
		static LayoutStateManager	stateManager;
		LayoutControlManager		controlManager;
		MotionRampCollection		ramps;
		int							drawLevel;

		// Events that can be fired by the model

		public LayoutModel() {
			// Add an initial layer
			instance = this;
			areas = new LayoutModelAreaDictionary();
			Clear();
		}

		[LayoutEvent("get-model")]
		void GetModel(LayoutEvent e) {
			e.Info = this;
		}

        public static LayoutModel Instance => instance;

        public static LayoutStateManager StateManager {
			get {
				if(stateManager == null) {
					stateManager = new LayoutStateManager();

					EventManager.AddObjectSubscriptions(stateManager);
				}

				return stateManager;
			}
		}


		public void Clear() {
			foreach(LayoutModelArea area in Areas) {
				foreach(LayoutModelSpotComponentCollection spot in area.Grid.Values) {
					foreach(ModelComponent component in spot)
						component.OnRemovingFromModel();
				}
			}

			DefaultPhase = LayoutPhase.Operational;
			ActivePhases = LayoutPhase.All;

			areas.Clear();
			componentReferences.Clear();
			if(stateManager != null)
				stateManager.Clear();

			controlManager = new LayoutControlManager();
		}

		/// <summary>
		/// Draw level is used to quick figure out which spots where drawn and which are not
		/// </summary>
		public int DrawLevel {
			get {
				return drawLevel;
			}

			set {
				drawLevel = value;
			}
		}

		#region Implementation of ILayoutModuleSetup

		public LayoutModule Module {
			set {
				module = value;
			}

			get {
				return module;
			}
		}

		#endregion

		[LayoutEvent("get-control-manager")]
		private void getControlManager(LayoutEvent e) {
			e.Info = ControlManager;
		}

        protected LayoutModelAreaDictionary MyAreas => areas;

        /// <summary>
        /// Return the list of areas in the model
        /// </summary>
        public static LayoutModelAreaDictionary Areas => Instance.MyAreas;

        /// <summary>
        /// Return the phases (operational, in-construction, planned) used by this model.
        /// </summary>
        /// <remarks>This operation passes on all the spots in the model, so use it with care</remarks>
        public static LayoutPhase ModelPhases => Areas.Aggregate<LayoutModelArea, LayoutPhase>(LayoutPhase.None, (p, area) => p | area.AreaPhases);

        protected LayoutModelBlockDictionary MyBlocks => blocks;

        public static LayoutModelBlockDictionary Blocks => Instance.MyBlocks;

        public LayoutXmlInfo XmlInfo => modelXmlInfo;

        public XmlElement Element => modelXmlInfo.Element;

        /// <summary>
        /// The locomotive catalog. This is a catalog of available locomotive types
        /// </summary>
        public static LocomotiveCatalogInfo LocomotiveCatalog => new LocomotiveCatalogInfo();

        /// <summary>
        /// The locomotive collection. This contains the locomotive that the user has.
        /// In addition the collection contains locomotive set elements
        /// </summary>
        protected LocomotiveCollectionInfo MyLocomotiveCollection {
			get {
				if(locomotiveCollection == null) {
					locomotiveCollection = new LocomotiveCollectionInfo();
					locomotiveCollection.Load();
					EventManager.AddObjectSubscriptions(locomotiveCollection);
					locomotiveCollection.EnsureReferentialIntegrity();
				}

				return locomotiveCollection;
			}
		}

        public static LocomotiveCollectionInfo LocomotiveCollection => Instance.MyLocomotiveCollection;

        protected LayoutControlManager MyControlManager => controlManager;

        public static LayoutControlManager ControlManager => Instance.MyControlManager;

        /// <summary>
        /// Return ramps collection
        /// </summary>
        public MotionRampCollection Ramps {
			get {
				if(ramps == null) {
					XmlElement	rampsElement = Element["Ramps"];

					if(rampsElement == null) {
						rampsElement = Element.OwnerDocument.CreateElement("Ramps");
						Element.AppendChild(rampsElement);
					}

					ramps = new MotionRampCollection(rampsElement);
				}

				return ramps;
			}
		}

		public XmlElement GlobalPoliciesElement {
			get {
				XmlElement	globalPoliciesElement = Element["Policies"];

				if(globalPoliciesElement == null) {
					globalPoliciesElement = Element.OwnerDocument.CreateElement("Policies");
					Element.AppendChild(globalPoliciesElement);
				}

				return globalPoliciesElement;
			}
		}

        public TripPlanIconListInfo TripPlanIconList {
            get {
                XmlElement tripPlanIconListElement = Element["TripPlanIconList"];

                if(tripPlanIconListElement == null) {
                    tripPlanIconListElement = Element.OwnerDocument.CreateElement("TripPlanIconList");
                    Element.AppendChild(tripPlanIconListElement);
                }

                return new TripPlanIconListInfo(tripPlanIconListElement);
            }
        }

        public bool ModelIsLoading => modelIsLoading;

        /// <summary>
        /// The number of logical speed steps. The train speed is expressed in logical speed steps.
        /// </summary>
        public int LogicalSpeedSteps => 14;

        /// <summary>
        /// Phase (design, construction, operational) of new components added to model
        /// </summary>
        public LayoutPhase DefaultPhase {
			get;
			set;
		}

		/// <summary>
		/// Active phase, if a component access method get LayoutPhase.Default as phase parameter, it will use ActivePhase value
		/// </summary>
		protected LayoutPhase MyActivePhase {
			get;
			set;
		}

		/// <summary>
		/// Active phase, if a component access method get LayoutPhase.Default as phase parameter, it will use ActivePhase value
		/// </summary>
		public static LayoutPhase ActivePhases {
			get {
				return Instance.MyActivePhase;
			}

			set {
				Instance.MyActivePhase = value;
			}
		}

		public void Redraw() {
			foreach(LayoutModelArea area in Areas)
				area.Redraw();
		}

		#region Component reference management

		public void AddComponentReference(Guid id, ModelComponent component) {
			componentReferences.Add(id, component);
		}

		public void RemoveComponentReference(Guid id) {
			componentReferences.Remove(id);
		}

		public TComponentType GetComponentById<TComponentType>(Guid id, LayoutPhase phase) where TComponentType : class, IModelComponent {
//			TComponentType component = (TComponentType)componentReferences[id];
            TComponentType component = componentReferences[id] as TComponentType;

            if (component != null && (component.Spot.Phase & phase) == 0)
				component = default(TComponentType);

			return component;
		}

        public ModelComponent this[Guid id, LayoutPhase phase] => GetComponentById<ModelComponent>(id, phase);

        public static TComponentType Component<TComponentType>(Guid id, LayoutPhase phases = LayoutPhase.All) where TComponentType : class, IModelComponent => Instance.GetComponentById<TComponentType>(id, phases);

        /// <summary>
        /// Return a list of all component of a given type or that implement a given interface
        /// </summary>
        /// <typeparam name="ComponentType">Type or interface</typeparam>
        /// <returns>List of components of this type or that implement that interface</returns>
        protected IEnumerable<ComponentType> MyComponents<ComponentType>(LayoutPhase phases = LayoutPhase.All) where ComponentType : class, IModelComponentHasId {
			foreach(ModelComponent modelComponent in componentReferences.Values) {
				ComponentType component = modelComponent as ComponentType;

				if(component != null && (component.Spot.Phase & phases) != 0)
					yield return component;
			}
		}

		protected IEnumerable<ComponentType> MyComponents<ComponentType>(Predicate<ComponentType> filter, LayoutPhase phases) where ComponentType : class, IModelComponentHasId {
			foreach(ModelComponent modelComponent in componentReferences.Values) {
				ComponentType component = modelComponent as ComponentType;

				if(component != null && filter(component) && (component.Spot.Phase & phases) != 0)
					yield return component;
			}
		}

		protected IEnumerable<ComponentType> AllMyComponents<ComponentType>(LayoutPhase phases, Predicate<ComponentType> filter = null) where ComponentType : class, IModelComponent {
			foreach(var area in Areas) {
				foreach(var spot in area.Grid.Values) {
					if((spot.Phase & phases) != 0) {
						foreach(var modelComponent in spot) {
							ComponentType component = modelComponent as ComponentType;

							if(component != null) {
								if(filter == null || filter(component))
									yield return component;
							}
						}
					}
				}
			}
		}

		protected IEnumerable<LayoutModelSpotComponentCollection> MySpots {
			get {
				foreach(var area in Areas) {
					foreach(var spot in area.Grid.Values) {
						yield return spot;
					}
				}
			}
		}

        public static IEnumerable<ComponentType> Components<ComponentType>(LayoutPhase phases) where ComponentType : class, IModelComponentHasId => Instance.MyComponents<ComponentType>(phases);

        public static IEnumerable<ComponentType> Components<ComponentType>(LayoutPhase phases, Predicate<ComponentType> filter) where ComponentType : class, IModelComponentHasId => Instance.MyComponents<ComponentType>(filter, phases);

        public static IEnumerable<ComponentType> AllComponents<ComponentType>(LayoutPhase phases, Predicate<ComponentType> filter = null) where ComponentType : class, IModelComponent => Instance.AllMyComponents<ComponentType>(phases, filter);

        public static IEnumerable<LayoutModelSpotComponentCollection> Spots => Instance.MySpots;

        public static bool HasPhase(LayoutPhase phases) => Spots.Any(spot => (spot.Phase & phases) != 0);

        #endregion

        #region Saving/Loading model from XML files

        void writeXmlAreas(XmlWriter w) {
			w.WriteStartElement("Areas");

			foreach(var area in Areas)
				area.WriteXml(w);

			w.WriteEndElement();
		}

		/// <summary>
		/// Generate a "LayoutModel" XML node
		/// </summary>
		/// <param name="w"></param>
		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("LayoutModel");
			w.WriteAttributeString("DefaultPhase", DefaultPhase.ToString());
			writeXmlAreas(w);
			controlManager.WriteXml(w);
			w.WriteEndElement();
		}

		// On entry: <Layers> (For backward compatability, read and ignore it)
		private void parseLayers(XmlReader r) {
			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();			// <Layer>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Layer")
						r.Read();
					else
						r.Skip();
				}

				r.Read();
			}
		}

		// On entry: <Areas>
		private void parseAreas(XmlReader r) {
			componentReferences.Clear();
			areas.Clear();

			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();									// <Area>

				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Area") {
						LayoutModelArea	area = new LayoutModelArea();

						area.ReadXml(r);
						areas.Add(area);
					}
					else
						r.Skip();
				}

				r.Read();
			}
		}

		// On entry <LayoutControl>
		private void parseLayoutControl(XmlReader r) {
			controlManager = new LayoutControlManager(r);
		}

		// On entry reader is on <LayoutModel> on exit it is on </LayoutModel>
		public void ReadXml(XmlReader r) {

			modelIsLoading = true;

			string defaultPhase = r.GetAttribute("DefaultPhase");
			DefaultPhase = defaultPhase == null ? LayoutPhase.Operational : (LayoutPhase)Enum.Parse(typeof(LayoutPhase), defaultPhase);

			if(r.IsEmptyElement)
				r.Read();
			else {
				r.Read();		// <LayoutModel>


				while(r.NodeType == XmlNodeType.Element) {
					if(r.Name == "Layers")
						parseLayers(r);
					else if(r.Name == "Areas")
						parseAreas(r);
					else if(r.Name == "LayoutControl")
						parseLayoutControl(r);
					else
						r.Skip();
				}

				r.Read();	// </LayoutModel>
			}

			modelIsLoading = false;
			EventManager.Event(new LayoutEvent(this, "model-loaded"));
		}

		/// <summary>
		/// Read XML file that contain model wide information (such as definitions for standard fonts etc.)
		/// </summary>
		/// <param name="path">The path from which the file need to be read</param>
		public bool ReadModelXmlInfo(String path) {
			bool	result = false;

			modelXmlInfoFilename = path + System.IO.Path.DirectorySeparatorChar + "ModelInfo.Xml";

			if(new FileInfo(modelXmlInfoFilename).Exists) {
				using(FileStream inStream = new FileStream(modelXmlInfoFilename, FileMode.Open)) {
					modelXmlInfo.XmlDocument.Load(inStream);
				}
				result = true;
			}
			else {
				// TODO: Initialize with a default model document
                modelXmlInfo.XmlDocument.LoadXml("<ModelInfo />");
			}

			return result;
		}

		/// <summary>
		/// Save the current state of the model wide information. The old file is renamed and saved as a backup file
		/// </summary>
		protected void MyWriteModelXmlInfo() {
			if(modelXmlInfo.XmlDocument != null) {
				String					backupModelXmlInfoFilename = modelXmlInfoFilename + ".backup";
				FileInfo				modelXmlInfoFileInfo = new FileInfo(modelXmlInfoFilename);

				new FileInfo(backupModelXmlInfoFilename).Delete();
				
				if(modelXmlInfoFileInfo.Exists)
					modelXmlInfoFileInfo.MoveTo(backupModelXmlInfoFilename);

				using(FileStream fileOut = new FileStream(modelXmlInfoFilename, FileMode.Create, FileAccess.Write)) {
					modelXmlInfo.XmlDocument.Save(fileOut);
				}
			}
		}

		public static void WriteModelXmlInfo() {
			Instance.MyWriteModelXmlInfo();
		}

		#endregion

		public static ModelComponent CreateModelComponent(String componentTypeName) {
			Assembly[]		loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			ModelComponent	component = null;
			
			foreach(Assembly a in loadedAssemblies) {
				component = (ModelComponent)a.CreateInstance(componentTypeName);

				if(component != null)
					break;
			}

			return component;
		}

		#region IDisposable Members

		public void Dispose() {
			blocks.Dispose();
		}

		#endregion
	}

	public static class PhaseLayoutEventExtender {
		public static LayoutEvent SetPhases(this LayoutEvent theEvent, LayoutPhase phase) {
			theEvent.SetOption("Phases", phase.ToString());
			return theEvent;
		}

		public static LayoutPhase GetPhases(this LayoutEvent theEvent) {
			var phase = (LayoutPhase)Enum.Parse(typeof(LayoutPhase), theEvent.GetOption("Phases", defaultValue: LayoutModel.ActivePhases.ToString()));

			return phase;
		}
	}

	public class LayoutXmlTextReader : XmlTextReader, ILayoutXmlContext {
		LayoutReadXmlContext	context;

		public LayoutXmlTextReader(String filename, LayoutReadXmlContext context) : base(filename) {
			this.context = context;
		}

		public LayoutXmlTextReader(Stream s, LayoutReadXmlContext context) : base(s) {
			this.context = context;
		}

        public LayoutReadXmlContext ReadXmlContext => context;
    }
}
