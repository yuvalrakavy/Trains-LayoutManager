using LayoutManager.Components;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

#pragma warning disable IDE0051, IDE0052, IDE0060, CA1032
#nullable enable
namespace LayoutManager.Model {
    /// <summary>
    /// Connection point functionality. A component which can be "wired" to a connection point can declare
    /// to what type of connection point it may be connected
    /// </summary>
    public static class ControlConnectionPointTypes {
        public const string Output = "Output";
        public const string OutputOnOff = Output + "OnOff";
        public const string OutputSolenoid = Output + "Solenoid";
        public const string OutputRelay = Output + "Relay";
        public const string OutputFlashing = Output + "Flashing";

        public const string Input = "Input";
        public const string InputDry = Input + "Dry";
        public const string InputVoltage = Input + "Voltage";
        public const string InputCurrent = Input + "Current";

        public const string Trigger = "Trigger";
        public const string Level = "Level";

        public const string InputDryTrigger = InputDry + Trigger;
        public const string InputDryLevel = InputDry + Level;
        public const string InputVoltageTrigger = InputVoltage + Trigger;
        public const string InputVoltageLevel = InputVoltage + Level;
        public const string InputCurrentLevel = InputCurrent + Level;
    }

    public enum ControlConnectionPointUsage {
        Input,              // Connection point is used to input information
        Output,             // Connection point is used to output information
        Both,               // Both input & output (applicable for bus usage)
    }

    /// <summary>
    /// Various options on how the connection point label is to be formatted
    /// </summary>
    [Flags]
    public enum ControlConnectionPointLabelFormatOptions {
        Numeric = 0x00000000,   // Connection point index (if more than one connection point per address) is shown as digit
        Alpha = 0x00000001, // Connection point index is shown as alpha numeric
        AlphaLowercase = 0x00000000,    // Show alpha as lower case (a)
        AlphaUppercase = 0x00000004,    // Show alpha as upper case (A)
        AttachAddress = 0x00000000, // Connection point address is shown before connection point index (e.g. 100A)
        NoAttachedAddress = 0x00000010, // Connection point address is not shown before connection point index (e.g. A)
        ConnectionPointAddress = 0x00000000,    // Show connection point address
        ConnectionPointIndex = 0x00000100,  // Show connection point index
        Custom = 0x00001000,    // Use control-get-connection-point-label event to get the label
    }

    [Flags]
    public enum ControlModuleConnectionPointArrangementOptions {
        TopRow = 0x00000001,    // Top row has connection points
        BottomRow = 0x00000002, // Bottom Row has connection points
        BothRows = TopRow | BottomRow,              // Both rows have connection points

        StartOnTop = 0x00000000,    // First connection point on top row
        StartOnBottom = 0x00000004, // First connection point on bottom row

        TopRightToLeft = 0x00000010,    // Top row is right to left (4 3 2 1)
        TopLeftToRight = 0x00000000,    // Top row is left to right (1 2 3 4)

        BottomRightToLeft = 0x00000020, // bottom row is right to left (4 3 2 1)
        BottomLeftToRight = 0x00000000, // bottom row is left to right (1 2 3 4)
    }

    /// <summary>
    /// Define the addressing method used on a bus
    /// </summary>
    public enum ControlBusTopology {
        /// <summary>
        /// Each module can have its own address
        /// </summary>
        RandomAddressing,

        /// <summary>
        /// The modules are chained on the bus, the address of a module is its location in the chain
        /// </summary>
        DaisyChain,

        /// <summary>
        /// Bus is fixed - modules cannot be added or removed
        /// </summary>
        Fixed,
    }

    /// <summary>
    /// Define the addressing method used to address a connection point residing on a bus
    /// </summary>
    public enum ControlAddressingMethod {
        /// <summary>
        /// Address is of a connection point
        /// </summary>
        DirectConnectionPointAddressing,

        /// <summary>
        /// Address is of a module, then 'index' selects a connection point within this module
        /// </summary>
        ModuleConnectionPointAddressing,
    }

    public interface IControlSupportUserAction {
        LayoutControlManager ControlManager { get; }

        bool UserActionRequired { get; set; }
    }

    #region Exception classes

    public class LayoutControlException : LayoutException {
        public LayoutControlException(string message) : base(message) {
        }

        public LayoutControlException(object subject, string message) : base(subject, message) {
        }

        public LayoutControlException() : base() {
        }

        public LayoutControlException(object subject, string message, Exception inner) : base(subject, message, inner) {
        }
    }

    public class ControlAddressInUseException : LayoutControlException {
        public ControlAddressInUseException(ControlModuleType moduleType, int address, ControlModule moduleUsingAddress) :
            base($"The address {address} is already being used by another module") {
            this.ModuleType = moduleType;
            this.Address = address;
            this.ModuleUsingAddress = moduleUsingAddress;
        }

        public ControlModuleType ModuleType { get; }

        public int Address { get; }

        public ControlModule ModuleUsingAddress { get; }
    }

    public class ControlAddressNotValidException : LayoutControlException {
        public ControlAddressNotValidException(ControlBus bus, int address) : base("Address " + address + " is illegal") {
            this.Bus = bus;
            this.Address = address;
        }

        public ControlBus Bus { get; }

        public int Address { get; }
    }

    public class ControlModuleAddressAlignmentException : LayoutControlException {
        private readonly ControlModuleType moduleType;

        public ControlModuleAddressAlignmentException(ControlBus bus, ControlModuleType moduleType, int address) : base("Address " + address + " is illegal, it must divide by " + moduleType.AddressAlignment) {
            this.Bus = bus;
            this.moduleType = moduleType;
            this.Address = address;
        }

        public ControlBus Bus { get; }

        public int Address { get; }
    }

    public class ControlBusFullException : LayoutControlException {
        public ControlBusFullException(ControlBus bus) : base("No space to add more modules to this bus") {
            this.Bus = bus;
        }

        public ControlBus Bus { get; }
    }

    #endregion

    /// <summary>
    /// Information about a connection from a component to control module. A collection of such structures is returned
    /// by components that can be connected to control module. The common case is that a component has one possible
    /// connection. But some components (e.g turnout with feedback) may require more than one connection
    /// </summary>
    public struct ModelComponentControlConnectionDescription {
        /// <summary>
        /// Describe a possible connection between component and control module
        /// </summary>
        /// <param name="connectionTypes">a comma delimited connection types (such as Solenoid, Relay, DryContact) that can be used</param>
        /// <param name="name">Unique connection name (language neutral)</param>
        /// <param name="displayName">User friendly name and language dependent name for the connection</param>
        /// <param name="requiredControlModuleTypeName">Optional, if this connection must be made to a control module of specific type</param>
        public ModelComponentControlConnectionDescription(string connectionTypes, string name, string displayName, string? requiredControlModuleTypeName = null) : this() {
            this.ConnectionTypes = connectionTypes;
            this.Name = name;
            this.DisplayName = displayName;
            this.RequiredControlModuleTypeName = requiredControlModuleTypeName;
        }

        /// <summary>
        /// What type of connection (solenoid, dry contact etc.) is compatible with this connection point
        /// </summary>
        public string ConnectionTypes {
            get;
        }

        /// <summary>
        /// Unique connection name
        /// </summary>
        /// <value>A unique language neutral connection name</value>
        public string Name {
            get;
        }

        /// <summary>
        /// Friendly (display) name
        /// </summary>
        /// <value>User friendly name and language dependent name for the connection</value>
        public string DisplayName {
            get;
        }

        /// <summary>
        /// Optional module type that this connection requires
        /// </summary>
        /// <value>Module type name or null if this component connection can be connected to any recommended control module</value>
        public string? RequiredControlModuleTypeName {
            get;
        }

        /// <summary>
        /// Check if this connection is compatible with a given connection type. This is based on the fact that the connection description
        /// name has format of (connection-type, ...)-name. For example Solenoid,Relay-Turnout if the connection can be to a relay or Solenoid
        /// </summary>
        /// <param name="connectionType"></param>
        /// <returns>True - can be connected to this kind of connection</returns>
        public bool IsCompatibleWith(params string[] connectionTypes) {
            string myConnectionTypes = ConnectionTypes;

            return connectionTypes.Any(connectionType => myConnectionTypes.Contains(connectionType));
        }
    }

    /// <summary>
    /// Fully specify control module connection destination. Both the component and the connection are
    /// provided
    /// </summary>
    public class ControlConnectionPointDestination {
        /// <summary>
        /// Construct new ControlConnectionPointDestination
        /// </summary>
        /// <param name="component">The connection destination component</param>
        /// <param name="connectionDescription">The connection for this component</param>
        public ControlConnectionPointDestination(IModelComponentConnectToControl component, ModelComponentControlConnectionDescription connectionDescription) {
            this.Component = component;
            this.ConnectionDescription = connectionDescription;

            Debug.Assert(new List<ModelComponentControlConnectionDescription>(component.ControlConnectionDescriptions).Exists((ModelComponentControlConnectionDescription cd) => cd.Name == connectionDescription.Name), "Unsupported connection for this component");
        }

        /// <summary>
        /// Destination component
        /// </summary>
        /// <value>Component that can be connected to control module</value>
        public IModelComponentConnectToControl Component { get; }

        /// <summary>
        /// The connection description
        /// </summary>
        /// <value>A connection description that is supported by the component</value>
        public ModelComponentControlConnectionDescription ConnectionDescription { get; }
    }

    /// <summary>
    /// This class represent a connection point between a control module and a layout component (ModelCompnent). Think of it as
    /// representing the wiring from a control module connection point to the component.
    /// </summary>
    public class ControlConnectionPoint : LayoutXmlWrapper, IControlSupportUserAction {
        private const string A_Index = "Index";
        private const string A_ComponentId = "ComponentID";
        private const string A_DisplayName = "DisplayName";
        private const string A_Name = "Name";
        private const string A_Type = "Type";
        private const string A_Usage = "Usage";
        private const string A_UserActionRequired = "UserActionRequired";

        public ControlConnectionPoint(ControlModule module, XmlElement connectionPointElement) : base(connectionPointElement) {
            this.Module = module;
        }

        /// <summary>
		/// The ID of the component that this connection point is connected to, or Guid.Empty if this connection
		/// point is not connected.
		/// </summary>
		public Guid ComponentId => (Guid?)AttributeValue(A_ComponentId) ?? Guid.Empty;

        /// <summary>
        /// The connection name to which this control module connect to
        /// </summary>
        /// <value></value>
        public string? Name {
            get {
                if (Component != null) {
                    if (!HasAttribute(A_DisplayName)) {     // Convert old connection to newer format
                        Debug.Assert(Component.ControlConnectionDescriptions.Count > 0);
                        SetAttributeValue(A_Name, Component.ControlConnectionDescriptions[0].Name);
                        SetAttributeValue(A_DisplayName, Component.ControlConnectionDescriptions[0].DisplayName);
                    }
                }

                return HasAttribute(A_Name) ? GetAttribute(A_Name) : null;
            }

            set => SetAttributValue(A_Name, value, removeIf: null);
        }

        /// <summary>
        /// The module containing this connection point
        /// </summary>
        public ControlModule Module { get; }

        public LayoutControlManager ControlManager => Module.ControlManager;

        /// <summary>
        /// Get or set the component to which this connection point is connected.
        /// </summary>
        public IModelComponentConnectToControl? Component {
            get => LayoutModel.Component<IModelComponentConnectToControl>(ComponentId, LayoutModel.ActivePhases);

            set {
                ModelComponent? previousComponent = null;

                Module.OnConnectionChanged();

                if (HasAttribute(A_ComponentId)) {
                    previousComponent = LayoutModel.Component<ModelComponent>(ComponentId, LayoutModel.ActivePhases);

                    if (previousComponent != null)
                        previousComponent.EraseImage();

                    Module.ControlManager.ConnectionPoints.Remove(this);

                    if (previousComponent != null)
                        previousComponent.Redraw();
                }

                if (value == null) {
                    Element.RemoveAttribute(A_ComponentId);
                }
                else {
                    ModelComponent theComponent = (ModelComponent)value;

                    theComponent.EraseImage();

                    SetAttributeValue(A_ComponentId, value.Id);
                    Module.ControlManager.ConnectionPoints.Add(this);

                    theComponent.Redraw();
                }

                if (previousComponent != null)
                    Dispatch.Notification.OnComponentDisconnectedFromControlModule(previousComponent, connectionPoint: this);

                if (value != null)
                    Dispatch.Notification.OnComponentConnectedToControlModule(value, this);
            }
        }

        public bool IsConnected => HasAttribute(A_ComponentId);

        /// <summary>
        /// The index of this connection point relative to the control module. Index = 0 means the first connection
        /// point that is supported by the module
        /// </summary>
        public int Index {
            get => (int?)AttributeValue(A_Index) ?? -1;
            set => SetAttributeValue(A_Index, value);
        }

        /// <summary>
        /// The connection point output or input type
        /// </summary>
        public string Type {
            get => GetOptionalAttribute(A_Type) ?? Module.DefaultControlConnectionPointType;
            set => SetAttributValue(A_Type, value, removeIf: Module.DefaultControlConnectionPointType);
        }

        public ControlConnectionPointUsage Usage {
            get {
                ControlConnectionPointUsage GetBusTypeUsage() {
                    if (Module.Bus == null)
                        throw new ArgumentException("Module has no bus");

                    ControlConnectionPointUsage busUsage = Module.Bus.BusType.Usage;
                    if (busUsage == ControlConnectionPointUsage.Both)
                        throw new ArgumentException("Unable to determine connection point usage");
                    return busUsage;
                }

                return AttributeValue(A_Usage).Enum<ControlConnectionPointUsage>() ?? GetBusTypeUsage();
            }

            set => SetAttributeValue(A_Usage, value);
        }

        /// <summary>
        /// The collection friendly name
        /// </summary>
        /// <value></value>
        public string DisplayName {
            get {
                if (!HasAttribute(A_DisplayName) && Component != null) {     // Convert old connection to newer format
                    Debug.Assert(Component.ControlConnectionDescriptions.Count > 0);
                    SetAttributeValue(A_Name, Component.ControlConnectionDescriptions[0].Name);
                    SetAttributeValue(A_DisplayName, Component.ControlConnectionDescriptions[0].DisplayName);
                }

                return GetAttribute(A_DisplayName);
            }

            set => SetAttributeValue(A_DisplayName, value);
        }

        /// <summary>
        /// Some action is required on this connection point, for example, wiring
        /// </summary>
        public bool UserActionRequired {
            get => (bool?)AttributeValue(A_UserActionRequired) ?? false;
            set => SetAttributeValue(A_UserActionRequired, value, removeIf: false);
        }
    }

    /// <summary>
    /// A collection of all the connection points of a given control module. The collection is indexed by the connection point
    /// index
    /// </summary>
    public class ControlConnectionPointCollection : XmlIndexedCollection<ControlConnectionPoint, int> {
        private readonly ControlModule module;

        public ControlConnectionPointCollection(ControlModule module) : base(module.ConnectionPointsElement) {
            this.module = module;
        }

        protected override int GetItemKey(ControlConnectionPoint item) => item.Index;

        protected override ControlConnectionPoint FromElement(XmlElement itemElement) => new(module, itemElement);

        /// <summary>
        /// Return a connection point for a given index. If there is no connection point for this index, a new one is created
        /// </summary>
        public new ControlConnectionPoint this[int index] {
            get {
                var connectionPoint = base[index];

                if (connectionPoint == null) {
                    XmlElement connectionPointElement = module.Element.OwnerDocument.CreateElement("ConnectionPoint");

                    connectionPoint = new ControlConnectionPoint(module, connectionPointElement) {
                        Index = index
                    };
                    Add(connectionPoint);
                }

                return connectionPoint;
            }
        }

        /// <summary>
        /// Add a connection point by providing its XML element. This method is usually called to re-insert a connection
        /// point that was removed. For example, by an Undo of a disconnection command
        /// </summary>
        /// <param name="inConnectionPointElement">The connection point element</param>
        /// <returns>The connection point</returns>
        public ControlConnectionPoint Add(XmlElement connectionPointElement) {
            ControlConnectionPoint connectionPoint = new(module, connectionPointElement);

            if (IsDefined(connectionPoint.Index))
                throw new ArgumentException("Trying to add a connection point which is already defined");

            if (connectionPointElement.Name != "ConnectionPoint")
                throw new ArgumentException("Trying to add a none connection point element");

            Add(connectionPoint);
            connectionPoint = this[connectionPoint.Index];      // Get the connection point that was actually added (the XmlElement may have been imported)

            if (connectionPoint.ComponentId != Guid.Empty) {
                var theComponent = (ModelComponent?)connectionPoint.Component;

                if (theComponent != null) {
                    theComponent.EraseImage();
                    module.ControlManager.ConnectionPoints.Add(connectionPoint);
                    theComponent.Redraw();

                    Dispatch.Notification.OnComponentConnectedToControlModule((IModelComponentConnectToControl)theComponent, connectionPoint);
                }
            }

            module.OnConnectionChanged();

            return connectionPoint;
        }

        /// <summary>
        /// Disconnect a connection point given by an index
        /// </summary>
        /// <param name="index">The index of the connection point to disconnect</param>
        public void Disconnect(int index) {
            if (ContainsKey(index)) {
                var connectionPoint = this[index];

                connectionPoint.Component = null;       // Disconnect
                Remove(index);
            }

            module.OnConnectionChanged();
        }

        /// <summary>
        /// Disconnect all connection points of this module
        /// </summary>
        public void DisconnectAll() {
            foreach (var connectionPoint in this)
                if (connectionPoint != null)
                    connectionPoint.Component = null;

            Clear();
            module.OnConnectionChanged();
        }

        /// <summary>
        /// Check whether a connection point is defined (it may be defined, but not connected)
        /// </summary>
        /// <param name="index">The connection point type</param>
        /// <returns>True - this connection point was defined</returns>
        public bool IsDefined(int index) => ContainsKey(index);

        /// <summary>
        /// Check if a given connection point is connected to a component
        /// </summary>
        /// <param name="index">The connection point index</param>
        /// <returns>True if this connection point is connected</returns>
        public bool IsConnected(int index) => IsDefined(index) && this[index].IsConnected;

        /// <summary>
        /// Return true if user action is required for a given connection point
        /// </summary>
        /// <param name="index">The connection point index</param>
        /// <returns>True if user action is required</returns>
        public bool IsUserActionRequired(int index) => IsDefined(index) && this[index].UserActionRequired;

        /// <summary>
        /// The printable label for this connection point
        /// </summary>
        public string GetLabel(int index, bool fullPath) => module.ModuleType.GetConnectionPointAddressText(module.ModuleType, module.Address, index, fullPath);

        public string GetLabel(int index) => GetLabel(index, false);

        /// <summary>
        /// Return the type of a given connection point index
        /// </summary>
        /// <param name="index">The connection point index</param>
        /// <returns>The connection point type</returns>
        public string GetConnectionPointType(int index) {
            return IsDefined(index) ? this[index].Type : module.DefaultControlConnectionPointType;
        }

        /// <summary>
        /// Check if a component can be connected to a given connection point
        /// </summary>
        /// <param name="component"></param>
        /// <param name="connectionDescription">The component connection to check</param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CanBeConnected(ControlConnectionPointDestination connectionDestination, int index) {
            if (IsConnected(index))
                return false;

            if (connectionDestination.ConnectionDescription.RequiredControlModuleTypeName != null && module.ModuleTypeName != connectionDestination.ConnectionDescription.RequiredControlModuleTypeName)
                return false;

            // Check if this module can be connected to this type of component
            IList<string> moduleTypeNames = module.Bus.BusType.GetConnectableControlModuleTypeNames(connectionDestination);
            bool moduleTypeFound = false;

            if (moduleTypeNames != null) {
                // Check if the type of this module is in one of the types that this component can be connected to
                foreach (string moduleTypeName in moduleTypeNames)
                    if (moduleTypeName == module.ModuleTypeName) {
                        moduleTypeFound = true;
                        break;
                    }
            }

            return moduleTypeFound && (Dispatch.Call.CanControlBeConnected(connectionDestination, module.Id, index, module.ModuleTypeName) ?? true);
        }

        /// <summary>
        /// Return all the connection point descriptions that a control connection point may be connected to
        /// </summary>
        /// <param name="component">The component</param>
        /// <param name="index">The control connection point index</param>
        /// <returns>List of ModelComponentControlConnectDescription</returns>
        public IList<ModelComponentControlConnectionDescription> GetPossibleConnectionDescriptions(IModelComponentConnectToControl component, int index) {
            List<ModelComponentControlConnectionDescription> possibleConnectionDescriptions = new();

            foreach (ModelComponentControlConnectionDescription connectionDescription in component.ControlConnectionDescriptions)
                if (LayoutModel.ControlManager.ConnectionPoints[component, connectionDescription.Name] == null && CanBeConnected(new ControlConnectionPointDestination(component, connectionDescription), index))
                    possibleConnectionDescriptions.Add(connectionDescription);

            return possibleConnectionDescriptions.AsReadOnly();
        }

        public ControlConnectionPointUsage Usage(int index) {
            return IsDefined(index) ? this[index].Usage : module.Bus.BusType.Usage;
        }

        /// <summary>
        /// Get enumerator for the connection points sorted by index
        /// </summary>
        /// <returns>Enumerator for connection point sorted by ascending index</returns>
        public new IEnumerator<ControlConnectionPoint> GetEnumerator() => GetSortedIterator((KeyValuePair<int, XmlElement> pair1, KeyValuePair<int, XmlElement> pair2) => pair1.Key - pair2.Key);
    }

    /// <summary>
    /// A reference to a connection point. The connection point is reference via module and index
    /// </summary>
    public class ControlConnectionPointReference : IControlSupportUserAction {
        private readonly Guid moduleID;

        public ControlConnectionPointReference(ControlModule module, int index) {
            this.ControlManager = module.ControlManager;
            this.moduleID = module.Id;
            this.Index = index;
        }

        public ControlConnectionPointReference(ControlBus bus, int address, int index) {
            this.ControlManager = bus.ControlManager;

            var module = bus.GetModuleUsingAddress(address);

            if (module != null) {
                int addressOffset = address - module.Address;

                Debug.Assert(addressOffset < module.ModuleType.NumberOfAddresses);
                int indexOffset = addressOffset * module.ModuleType.ConnectionPointsPerAddress;

                this.moduleID = module == null ? Guid.Empty : module.Id;
                this.Index = indexOffset + index;
            }
            else
                this.moduleID = Guid.Empty;
        }

        public ControlConnectionPointReference(ControlBus bus, int address) {
            this.ControlManager = bus.ControlManager;

            var module = bus.GetModuleUsingAddress(address);

            if (module != null) {
                this.moduleID = module.Id;
                this.Index = address - module.Address;
            }
            else
                this.moduleID = Guid.Empty;
        }

        public ControlConnectionPointReference(ControlConnectionPoint connectionPoint) {
            this.ControlManager = connectionPoint.Module.ControlManager;
            this.moduleID = connectionPoint.Module.Id;
            this.Index = connectionPoint.Index;
        }

        public LayoutControlManager ControlManager { get; }

        public Guid ModuleId => moduleID;

        public int Index { get; }

        public ControlModule? OptionalModule => moduleID == Guid.Empty ? null : ControlManager.GetModule(moduleID);

        public ControlModule Module => Ensure.NotNull<ControlModule>(OptionalModule);

        public ControlModuleReference ModuleReference => new(ControlManager, moduleID);

        public ControlConnectionPoint? OptionalConnectionPoint => Module?.ConnectionPoints[Index];

        public ControlConnectionPoint ConnectionPoint => Ensure.NotNull<ControlConnectionPoint>(OptionalConnectionPoint);

        public bool IsConnected => Module?.ConnectionPoints.IsConnected(Index) ?? false;

        public bool IsDefined {
            get {
                var module = Module;

                return module?.ConnectionPoints.IsDefined(Index) ?? false;
            }
        }

        public bool IsModuleDefined() => Module != null;

        /// <summary>
        /// Check if a given component can be connected to the referenced connection point
        /// </summary>
        /// <param name="component"></param>
        /// <param name="connectionDescription">The component connection to check</param>
        /// <returns></returns>
        public bool CanBeConnected(ControlConnectionPointDestination connectionDestination) {
            var module = Module;

            return module?.ConnectionPoints.CanBeConnected(connectionDestination, Index) ?? false;
        }

        /// <summary>
        /// Return all the connection point descriptions that a control connection point may be connected to
        /// </summary>
        /// <param name="component">The component</param>
        /// <param name="index">The control connection point index</param>
        /// <returns>List of ModelComponentControlConnectDescription</returns>
        public IList<ModelComponentControlConnectionDescription>? GetPossibleConnectionDescriptions(IModelComponentConnectToControl component, int index) {
            var module = Module;

            return module?.ConnectionPoints.GetPossibleConnectionDescriptions(component, index);
        }

        /// <summary>
        /// Return whether user action is required on this control connection point
        /// </summary>
        /// <value></value>
        public bool UserActionRequired {
            get => IsDefined && ConnectionPoint!.UserActionRequired;

            set {
                if (ConnectionPoint != null) {
                    ConnectionPoint.UserActionRequired = value;
                }
            }
        }
    }

    /// <summary>
    /// <para>
    /// A control module is a device that control one or more controlled components. Examples of control
    /// modules are accessories decoders, and feedback units.
    /// </para>
    /// <para>
    /// Each control module supports one or more connection points. A connection point may be "wired" to
    /// a controlled component.
    /// </para>
    /// </summary>
#pragma warning disable CA1036
    public class ControlModule : LayoutXmlWrapper, IComparable<ControlModule>, IControlSupportUserAction, IHasDecoder {
        private const string A_DefaultConnectionPointType = "DefaultConnectionPointType";
        private const string A_Address = "Address";
        private const string A_Label = "Label";
        private const string A_UserActionRequired = "UserActionRequired";
        private const string A_AddressProgrammingRequired = "AddressProgrammingRequired";
        private const string A_BusId = "BusID";
        private const string A_LocationId = "LocationID";
        private const string A_NumberOfConnectionPoints = "NumberOfConnectionPoints";
        private ControlModuleType? moduleType;
        private LayoutPhase _phase = LayoutPhase.None;

        public ControlModule(LayoutControlManager controlManager, XmlElement moduleElement) : base(moduleElement) {
            this.ControlManager = controlManager;
        }

        public LayoutControlManager ControlManager { get; }

        /// <summary>
        /// The type name of this control module. This is used to get information about modules of this type
        /// </summary>
        public string ModuleTypeName {
            get => GetAttribute("ModuleTypeName");

            set => SetAttributeValue("ModuleTypeName", value);
        }

        /// <summary>
        /// Return the module type object of this module
        /// </summary>
        public ControlModuleType ModuleType {
            get {
                return moduleType ??= Dispatch.Call.GetControlModuleType(ModuleTypeName);
            }

            set => ModuleTypeName = value.TypeName;
        }

        /// <summary>
        /// The ID of the bus to which this module is connected
        /// </summary>
        public Guid BusId {
            get => (Guid?)AttributeValue(A_BusId) ?? Guid.Empty;
            set => SetAttributeValue(A_BusId, value, removeIf: Guid.Empty);
        }

        /// <summary>
        /// Get the bus object to which this control module is attached
        /// </summary>
        public ControlBus Bus {
            get => Ensure.NotNull<ControlBus>(ControlManager.Buses[BusId], $"Bus for id: {BusId}");
            set => BusId = value.Id;
        }

        /// <summary>
        /// The total number of connection points of this specific module. The default is the number
        /// of connection points of modules of this type.
        /// </summary>
        public int NumberOfConnectionPoints {
            get => (int?)AttributeValue(A_NumberOfConnectionPoints) ?? ModuleType.NumberOfConnectionPoints;
            set => SetAttributeValue(A_NumberOfConnectionPoints, value, removeIf: ModuleType.NumberOfConnectionPoints);
        }

        public XmlElement ConnectionPointsElement {
            get {
                XmlElement? connectionPointsElement = Element["ConnectionPoints"];

                if (connectionPointsElement == null) {
                    connectionPointsElement = Element.OwnerDocument.CreateElement("ConnectionPoints");
                    Element.AppendChild(connectionPointsElement);
                }

                return connectionPointsElement;
            }
        }

        /// <summary>
        /// The collection of this control module's connection points
        /// </summary>
        public ControlConnectionPointCollection ConnectionPoints => new(this);

        public string DefaultControlConnectionPointType {
            get => GetOptionalAttribute(A_DefaultConnectionPointType) ?? ModuleType.DefaultControlConnectionPointType;
            set => SetAttributValue(A_DefaultConnectionPointType, value, removeIf: ModuleType.DefaultControlConnectionPointType);
        }

        /// <summary>
        /// The control module address (in many cases this is the address of the first connection point)
        /// </summary>
        public int Address {
            get => (int?)AttributeValue(A_Address) ?? -1;
            set => SetAttributeValue(A_Address, value);
        }

        /// <summary>
        /// The ID of a location in which the control module is located
        /// </summary>
        public Guid? LocationId {
            get => (Guid?)AttributeValue(A_LocationId) ?? Guid.Empty;
            set => SetAttributeValue(A_LocationId, value ?? Guid.Empty, removeIf: Guid.Empty);
        }

        /// <summary>
        /// Return the control module location component that is assigned to this module (if any)
        /// </summary>
        public LayoutControlModuleLocationComponent? Location => LocationId == null || LocationId == Guid.Empty
                    ? null
                    : LayoutModel.Component<LayoutControlModuleLocationComponent>((Guid)LocationId, LayoutModel.ActivePhases);

        /// <summary>
        /// The module label
        /// </summary>
        public string? Label {
            get => GetOptionalAttribute(A_Label);
            set => SetAttributValue(A_Label, value, removeIf: null);
        }

        /// <summary>
        /// Some user action is required on this module (for example, wiring it.)
        /// </summary>
        public bool UserActionRequired {
            get => (bool?)AttributeValue(A_UserActionRequired) ?? false;
            set => SetAttributeValue(A_UserActionRequired, value, removeIf: false);
        }

        /// <summary>
        /// Address programming is required for this module
        /// </summary>
        public bool AddressProgrammingRequired {
            get => (bool?)AttributeValue(A_AddressProgrammingRequired) ?? false;
            set => SetAttributeValue(A_AddressProgrammingRequired, value, removeIf: false);
        }

        /// <summary>
        /// Returns true if a given address is used by this module
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool UsingAddress(int address) => this.Address <= address && address < this.Address + ModuleType.NumberOfAddresses;

        /// <summary>
        /// The module phase (planned, in construction, operational). It is the phase for which the module must be present.
        /// </summary>
        public LayoutPhase Phase {
            get {
                if (_phase == LayoutPhase.None) {
                    _phase = LayoutPhase.Planned;       // If no connection point is connected, the module is Planned

                    for (int index = 0; index < NumberOfConnectionPoints; index++) {
                        if (ConnectionPoints.IsConnected(index)) {
                            var component = ConnectionPoints[index].Component;
                            var componentPhase = LayoutPhase.Planned;

                            if (component != null)
                                componentPhase = component.Spot.Phase;

                            if (_phase == LayoutPhase.Planned && componentPhase != LayoutPhase.Planned)
                                _phase = componentPhase;
                            else if (_phase == LayoutPhase.Construction && componentPhase == LayoutPhase.Operational)
                                _phase = componentPhase;

                            if (_phase == LayoutPhase.Operational)
                                break;
                        }
                    }
                }

                return _phase;
            }
        }

        /// <summary>
        /// Called when the module connection has changed
        /// </summary>
        internal void OnConnectionChanged() {
            _phase = LayoutPhase.None;
        }

        #region IHasDecoder Members

        /// <summary>
        /// Return the decoder associated with this module (or null if non is associated)
        /// </summary>
        public DecoderTypeInfo DecoderType => ModuleType.DecoderType;

        #endregion

        #region IComparable<ControlModule> Members

        public int CompareTo(ControlModule? other) => other != null ? Address - other.Address : throw new ArgumentNullException(nameof(other));

        public bool Equals(ControlModule other) => this == other;
        #endregion
    }

    public static class ControlDispatchFilters {
        static readonly Dictionary<string, Regex> regexMap = new();

        static bool ModuleTypeParameterFilter(object? filterValue, object? targetObject, object? parameterValue) {
            string moduleTypeFilter = filterValue as string ?? throw new DispatchFilterException("Filter value must be string with control module type name");

            if (!regexMap.TryGetValue(moduleTypeFilter, out Regex? moduleTypeRegEx)) {
                moduleTypeRegEx = new Regex(moduleTypeFilter);
                regexMap.Add(moduleTypeFilter, moduleTypeRegEx);
            }

            string? parameterModuleTypeName = parameterValue switch {
                ControlModule module => module.ModuleTypeName,
                ControlModuleReference moduleReference => moduleReference.Module.ModuleTypeName,
                _ => null,
            };


            return parameterModuleTypeName != null && moduleTypeRegEx.IsMatch(parameterModuleTypeName);
        }

        [DispatchTarget]
        static void AddDispatcherFilters() {
            Dispatch.AddCustomParameterFilter("ModuleType", ModuleTypeParameterFilter);
        }
    }
    /// <summary>
    /// Hold long term reference to control module.
    /// </summary>
    public class ControlModuleReference {
        private readonly Guid _moduleId;

        public ControlModuleReference(ControlModule module) {
            this.ControlManager = module.ControlManager;
            this._moduleId = module.Id;
        }

        public ControlModuleReference(LayoutControlManager controlManager, Guid moduleID) {
            this.ControlManager = controlManager;
            this._moduleId = moduleID;
        }

        public LayoutControlManager ControlManager { get; }

        public ControlModule? OptionalModule => ControlManager.GetModule(_moduleId);

        public ControlModule Module => Ensure.NotNull<ControlModule>(OptionalModule);

        public Guid ModuleId => _moduleId;

        public static implicit operator ControlModule(ControlModuleReference moduleRef) => moduleRef.Module;
    }

    /// <summary>
    /// Information about a "type" of a control module. The underlying XML object is returned by the control module
    /// handler to describe that control module.
    /// </summary>
    public class ControlModuleType {
        /// <summary>
        /// Create a new module type, the element is appended as a child of parent.
        /// </summary>
        /// <param name="typeName">The module type name</param>
        /// <param name="name">User friendly module name</param>
        public ControlModuleType(string typeName, string name) {
            TypeName = typeName;
            Name = name;
        }

        /// <summary>
        /// Internal type name, used as a key by the control module to select this type
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Human readable type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The default of each connection point for modules in this type
        /// </summary>
        public string DefaultControlConnectionPointType { get; set; } = ControlConnectionPointTypes.OutputSolenoid;

        /// <summary>
        /// Number of addresses that are taken by each module. In many cases it will be 1 (for example a decoder
        /// for a single turnout), however it may be more than one, for example for Marklin K83 or LGB 55025 which
        /// supports 4 turnouts, the value is 4
        /// </summary>
        public int NumberOfAddresses { get; set; } = 1;

        /// <summary>
        /// The first address assigned to this module must be a multiple of the address alignment. For example, if
        /// the address alignment is 4 than the module address must divide by 4 (0, 4, 8 etc.)
        /// </summary>
        int? _addressAlignment = null;
        public int AddressAlignment { get => _addressAlignment ?? NumberOfAddresses; set => _addressAlignment  = value; }

        /// <summary>
        /// The number of connection points for each address on the bus.
        /// </summary>
        public int ConnectionPointsPerAddress { get; set; } = 1;

        /// <summary>
        /// The total number of connection points
        /// </summary>
        int? _numberOfConnectionPoints;
        public int NumberOfConnectionPoints {
            get => _numberOfConnectionPoints ?? NumberOfAddresses * ConnectionPointsPerAddress;
            set => _numberOfConnectionPoints = value;
        }

        /// <summary>
        /// Provide information how a connection point label is to be displayed
        /// </summary>
        public ControlModuleConnectionPointArrangementOptions ConnectionPointArrangement { get; set; } = ControlModuleConnectionPointArrangementOptions.BottomRow | ControlModuleConnectionPointArrangementOptions.StartOnBottom;

        /// <summary>
        /// Provide information how a connection point label is to be displayed
        /// </summary>
        public ControlConnectionPointLabelFormatOptions ConnectionPointLabelFormat { get; set; } = ControlConnectionPointLabelFormatOptions.Numeric;

        /// <summary>
        /// The origin of connection point index, usually it will be either 0, or 1
        /// </summary>
        public int ConnectionPointIndexBase { get; set; } = 0;

        /// <summary>
        /// The control module is connected using those buses (e.g. DCC, Motorola, LGBBUS, MarklinDigital etc.
        /// </summary>
        public List<string> BusTypeNames {get; } = new();

        /// <summary>
        /// Return minimum address allowed for this module (if any or -1 if use the default bus addressing limits)
        /// </summary>
        public int FirstAddress { get; set; } = -1;

        /// <summary>
        /// Return maximum address allowed for this module (if any or -1 if use the default bus addressing limits)
        /// </summary>
        public int LastAddress { get; set; } = -1;

        /// <summary>
        /// True if this control module type is "built-in" a component. For example, a decoder which snap in to a turnout
        /// (e.g. Marlin C-track turnouts).
        /// </summary>
        public bool BuiltIn { get; set; } = false;

        /// <summary>
        /// The name of the decoder type built into module of this type (or null if modules of this type has no decoder)
        /// </summary>
        public string DecoderTypeName { get; set; } = "GenericDCC";

        /// <summary>
        /// Get the decoder type associated with modules of this type
        /// </summary>
        public DecoderTypeInfo DecoderType => Dispatch.Call.GetDecoderType(DecoderTypeName);

        /// <summary>
        /// Add bus types on which this control modules can be
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="busTypes"></param>
        public void AddBusTypes(params string[] busTypes) {
            BusTypeNames.AddRange(busTypes);
        }

        /// <summary>
        /// The printable label for an address
        /// </summary>
        public string GetConnectionPointAddressText(ControlModuleType moduleType, int baseAddress, int index, bool fullAddressPath) {
            ControlConnectionPointLabelFormatOptions format = ConnectionPointLabelFormat;
            string result;

            if ((format & ControlConnectionPointLabelFormatOptions.Custom) != 0)
                result = Dispatch.Call.GetControlConnectPointLabel(this, baseAddress, index);
            else {
                int address = baseAddress + (index / ConnectionPointsPerAddress);
                string addressText;

                if ((format & ControlConnectionPointLabelFormatOptions.ConnectionPointIndex) != 0)
                    addressText = (index + moduleType.ConnectionPointIndexBase).ToString();
                else
                    addressText = address.ToString();

                if (ConnectionPointsPerAddress == 1)
                    result = addressText;
                else {
                    string indexText;
                    string sep = "";

                    if ((format & ControlConnectionPointLabelFormatOptions.Alpha) != 0)
                        indexText = new string(Convert.ToChar((index % ConnectionPointsPerAddress) +
                            (((format & ControlConnectionPointLabelFormatOptions.AlphaUppercase) != 0) ? 'A' : 'a')), 1);
                    else {
                        int v = (index % ConnectionPointsPerAddress) + ConnectionPointIndexBase;

                        indexText = v.ToString();
                        sep = "/";
                    }

                    if (fullAddressPath || (format & ControlConnectionPointLabelFormatOptions.NoAttachedAddress) == 0)
                        result = addressText + sep + indexText;
                    else
                        result = indexText;
                }
            }

            return result;
        }

        public string GetConnectionPointAddressText(ControlModuleType moduleType, int baseAddress, int index) => GetConnectionPointAddressText(moduleType, baseAddress, index, false);
    }

    /// <summary>
    /// Bus object. A bus is connected to one or more control modules. Each bus is connected to a command station
    /// </summary>
    public class ControlBus : LayoutXmlWrapper {
        private const string A_CommandStationId = "CommandStationID";
        private const string A_BusProviderId = "BusProviderID";
        private const string A_BusTypeName = "BusTypeName";
        private ControlBusType? busType;
        private Dictionary<int, ControlModule>? addressModuleMap;

        public ControlBus(LayoutControlManager controlManager, XmlElement busElement) : base(busElement) {
            this.ControlManager = controlManager;
        }

        public LayoutControlManager ControlManager { get; }

        /// <summary>
        /// The ID of the command station to which this bus is connected
        /// </summary>
        public Guid BusProviderId {
            get {
                Guid? DefaultBusProviderId() {
                    var id = (Guid?)AttributeValue(A_CommandStationId);

                    if (id.HasValue)
                        BusProviderId = id.Value;
                    return id;
                }

                return (Guid?)AttributeValue(A_BusProviderId) ?? DefaultBusProviderId() ?? Guid.Empty;
            }

            set => SetAttributeValue(A_BusProviderId, value);
        }

        /// <summary>
        /// The command station to which this bus is connected
        /// </summary>
        public IModelComponentIsBusProvider BusProvider {
            get => Ensure.NotNull<IModelComponentIsBusProvider>(LayoutModel.Component<IModelComponentIsBusProvider>(BusProviderId, LayoutModel.ActivePhases), "BusProvider");
            set => BusProviderId = value.Id;
        }

        /// <summary>
        /// The bus type name of this bus
        /// </summary>
        public string BusTypeName {
            get => GetAttribute(A_BusTypeName);
            set => SetAttributeValue(A_BusTypeName, value);
        }

        /// <summary>
        /// User friendly bus name
        /// </summary>
        public string Name => BusType.Name;

        /// <summary>
        /// The bus type object that define properties of this bus
        /// </summary>
        public ControlBusType BusType => busType ??= Dispatch.Call.GetControlBusType(BusTypeName);

        /// <summary>
        /// Return the modules attached to this bus
        /// </summary>
        public IList<ControlModule> Modules {
            get {
                var moduleElements = ControlManager.ModulesElement.SelectNodes("Module[@BusID='" + this.Id.ToString() + "']");
                List<ControlModule> modules = new(moduleElements?.Count ?? 0);

                if (moduleElements != null) {
                    foreach (XmlElement moduleElement in moduleElements)
                        modules.Add(new ControlModule(ControlManager, moduleElement));
                }

                return modules;
            }
        }

        public void ResetAddressModuleMap() {
            addressModuleMap = null;
        }

        public ControlModule? GetModuleUsingAddress(int address) {
            if (addressModuleMap == null) {
                addressModuleMap = new Dictionary<int, ControlModule>();

                foreach (ControlModule module in Modules) {
                    for (int addressCount = 0; addressCount < module.ModuleType.NumberOfAddresses; addressCount++)
                        addressModuleMap.Add(module.Address + addressCount, module);
                }
            }

            addressModuleMap.TryGetValue(address, out ControlModule? result);
            return result;
        }

        /// <summary>
        /// Do the actual work of creating a new module object
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private ControlModule DoAdd(Guid? controlModuleLocationId, ControlModuleType moduleType, int address) {
            XmlElement moduleElement = ControlManager.ModulesElement.OwnerDocument.CreateElement("Module");
            ControlModule module = new(ControlManager, moduleElement) {
                ModuleType = moduleType,
                Bus = this,
                Address = address,
                LocationId = controlModuleLocationId
            };

            if (module.DecoderType != null)
                module.AddressProgrammingRequired = true;

            ResetAddressModuleMap();

            ControlManager.ModulesElement.AppendChild(moduleElement);
            Dispatch.Notification.OnControlModuleAdded(module);

            return module;
        }

        /// <summary>
        /// Return the address of a module assuming that this module is added at the end of the chain
        /// </summary>
        /// <param name="modules"></param>
        /// <returns></returns>
        public int NextModuleAddress {
            get {
                int address = BusType.FirstAddress;

                // Figure the address a module at the end of the chain
                foreach (ControlModule module in Modules)
                    if (module.Address >= address)
                        address = module.Address + module.ModuleType.NumberOfAddresses;

                return address;
            }
        }

        public ControlModule Add(XmlElement moduleElement) {
            ResetAddressModuleMap();

            if (moduleElement.OwnerDocument != ControlManager.ModulesElement.OwnerDocument)
                moduleElement = (XmlElement)ControlManager.ModulesElement.OwnerDocument.ImportNode(moduleElement, true);

            var module = new ControlModule(ControlManager, moduleElement);
            ControlManager.ModulesElement.AppendChild(moduleElement);

            Dispatch.Notification.OnControlModuleAdded(module);

            return module;
        }

        /// <summary>
        /// Add a new module at a given address
        /// </summary>
        /// <param name="moduleType">The type of the module to add</param>
        /// <param name="address">The address of the new module</param>
        /// <returns>The new module</returns>
        public ControlModule Add(Guid? controlModuleLocationId, ControlModuleType moduleType, int address) {
            if (BusType.Topology == ControlBusTopology.DaisyChain)
                throw new ArgumentException("You cannot add module with address to daisy chained bus");

            int firstAddress = moduleType.FirstAddress < 0 ? BusType.FirstAddress : moduleType.FirstAddress;
            int lastAddress = moduleType.LastAddress < 0 ? BusType.LastAddress : moduleType.LastAddress;

            // Check that the address is valid
            if (address < firstAddress || address > lastAddress)
                throw new ControlAddressNotValidException(this, address);

            if ((address - firstAddress) % moduleType.AddressAlignment != 0)
                throw new ControlModuleAddressAlignmentException(this, moduleType, address);

            // Check that the address is not used
            foreach (ControlModule module in Modules)
                if (module.UsingAddress(address))
                    throw new ControlAddressInUseException(moduleType, address, module);

            return DoAdd(controlModuleLocationId, moduleType, address);
        }

        public ControlModule Add(Guid? controlModuleLocationId, string moduleTypeName, int address) => Add(controlModuleLocationId, Dispatch.Call.GetControlModuleType(moduleTypeName), address);

        /// <summary>
        /// Add a new module at the end of a daisy chained bus.
        /// </summary>
        /// <param name="moduleType">The type of the module to add</param>
        /// <returns>The new added module</returns>
        public ControlModule Add(Guid? controlModuleLocationId, ControlModuleType moduleType) {
            if (BusType.Topology != ControlBusTopology.DaisyChain)
                throw new ArgumentException("You cannot add module to the end of a bus only if the bus is daisy chained");

            int address = NextModuleAddress;
            int lastAddress = moduleType.LastAddress < 0 ? BusType.LastAddress : moduleType.LastAddress;

            if (address > lastAddress)
                throw new ControlBusFullException(this);

            return DoAdd(controlModuleLocationId, moduleType, address);
        }

        public ControlModule Add(Guid? controlModuleLocationId, string moduleTypeName) => Add(controlModuleLocationId, Dispatch.Call.GetControlModuleType(moduleTypeName));

        /// <summary>
        /// Insert a new module after a given module in a daisy chained bus
        /// </summary>
        /// <param name="insertBeforeModule">The module after which the new module is to be added</param>
        /// <param name="moduleType">The type of the module to add</param>
        /// <returns>The new inserted module</returns>
        public ControlModule Insert(Guid? controlModuleLocationId, ControlModule insertBeforeModule, ControlModuleType moduleType) {
            if (BusType.Topology != ControlBusTopology.DaisyChain)
                throw new ArgumentException("Insert module in a bus only if the bus is daisy chained");

            if (NextModuleAddress > BusType.LastAddress)
                throw new ControlBusFullException(this);

            int address = insertBeforeModule.Address;

            // Bump up the address of all modules which have greater or equal address to the new module address

            foreach (ControlModule module in Modules) {
                if (module.Address >= address)
                    module.Address += moduleType.NumberOfAddresses;
            }

            return DoAdd(controlModuleLocationId, moduleType, address);
        }

        public ControlModule Insert(Guid? controlModuleLocationId, ControlModule insertBeforeModule, string moduleTypeName) =>
            Insert(controlModuleLocationId, insertBeforeModule, Dispatch.Call.GetControlModuleType(moduleTypeName));

        public ControlModule Insert(int address, XmlElement moduleElement) {
            if (BusType.Topology != ControlBusTopology.DaisyChain)
                throw new ArgumentException("Insert module in a bus only if the bus is daisy chained");

            ResetAddressModuleMap();

            if (moduleElement.OwnerDocument != ControlManager.ModulesElement.OwnerDocument)
                moduleElement = (XmlElement)ControlManager.ModulesElement.OwnerDocument.ImportNode(moduleElement, true);

            var insertedModule = new ControlModule(ControlManager, moduleElement);

            foreach (ControlModule module in Modules) {
                if (module.Address >= address)
                    module.Address += insertedModule.ModuleType.NumberOfAddresses;
            }

            ControlManager.ModulesElement.AppendChild(moduleElement);
            Dispatch.Notification.OnControlModuleAdded(insertedModule);

            return insertedModule;
        }

        /// <summary>
        /// Remove a module from the bus. The module is disconnected from all components
        /// </summary>
        /// <param name="module">The module to be removed</param>
        public void Remove(ControlModule module) {
            module.ConnectionPoints.DisconnectAll();

            if (BusType.Topology == ControlBusTopology.DaisyChain) {
                foreach (ControlModule m in Modules) {
                    if (m.Address > module.Address)
                        m.Address -= module.ModuleType.NumberOfAddresses;
                }
            }

            ResetAddressModuleMap();
            ControlManager.ModulesElement.RemoveChild(module.Element);

            Dispatch.Notification.OnControlModuleRemoved(module);
        }

        /// <summary>
        /// Allocate free address on the bus for being used by a module of a given module type
        /// </summary>
        /// <param name="moduleType">The module type</param>
        /// <param name="startAddress">Start searching for free address starting from this address</param>
        /// <returns>An free address for the module, or -1 if no free address can be found</returns>
        public int AllocateFreeAddress(ControlModuleType moduleType, int startAddress) {
            int lastAddress = moduleType.LastAddress < 0 ? BusType.LastAddress : moduleType.LastAddress;
            startAddress = BusType.GetAlignedAddress(moduleType, startAddress);

            while (startAddress <= lastAddress) {
                if (GetModuleUsingAddress(startAddress) == null) {
                    bool allFree = true;

                    for (int a = startAddress; a < startAddress + moduleType.NumberOfAddresses; a++)
                        if (GetModuleUsingAddress(a) != null) {
                            allFree = false;
                            break;
                        }

                    if (allFree)
                        return startAddress;
                }

                startAddress += moduleType.NumberOfAddresses;
            }

            return -1;
        }

        /// <summary>
        /// Allocate free address for module of a given type optionally assuming that this module will be positioned in a
        /// given control module location
        /// </summary>
        /// <param name="moduleType">The module type</param>
        /// <param name="controlModuleLocation">The control module location or null</param>
        /// <returns>A suggested address, or -1 if no free address could be found</returns>
        public int AllocateFreeAddress(ControlModuleType moduleType, LayoutControlModuleLocationComponent? controlModuleLocation) {
            int defaultAddress = -1;

            if (controlModuleLocation != null) {
                var busDefault = controlModuleLocation.Info.Defaults[this.Id];

                if (busDefault != null && busDefault.DefaultStartAddress >= 0)
                    defaultAddress = AllocateFreeAddress(moduleType, busDefault.DefaultStartAddress);
            }

            if (defaultAddress < 0)
                defaultAddress = AllocateFreeAddress(moduleType, BusType.RecommendedStartAddress);

            if (defaultAddress < 0)
                defaultAddress = AllocateFreeAddress(moduleType, BusType.FirstAddress);

            return defaultAddress;
        }

        /// <summary>
        /// Allocate free address for module of a given type optionally assuming that this module will be positioned in a
        /// given control module location
        /// </summary>
        /// <param name="moduleType">The module type</param>
        /// <param name="controlModuleLocationId">The ID of a control module location (or Guid.Empty)</param>
        /// <returns>A suggested address, or -1 if no free address could be found</returns>
        public int AllocateFreeAddress(ControlModuleType moduleType, Guid controlModuleLocationId) {
            LayoutControlModuleLocationComponent? controlModuleLocation = null;

            if (controlModuleLocationId != Guid.Empty)
                controlModuleLocation = LayoutModel.Component<LayoutControlModuleLocationComponent>(controlModuleLocationId, LayoutPhase.All);

            return AllocateFreeAddress(moduleType, controlModuleLocation);
        }
    }

    /// <summary>
    /// This object represent a bus type.
    /// </summary>
    public class ControlBusType : LayoutXmlWrapper {
        private const string A_BusFamilyName = "BusFamilyName";
        private const string A_BusTypeName = "BusTypeName";
        private const string A_Name = "Name";
        private const string A_Topology = "Topology";
        private const string A_AddressingMethod = "AddressingMethod";
        private const string A_FirstAddress = "FirstAddress";
        private const string A_LastAddress = "LastAddress";
        private const string A_RecommendedStartAddress = "RecommendedStartAddress";
        private const string A_Usage = "Usage";
        private const string A_ClickToAddEventName = "ClickToAddEventName";
        private const string A_CanChangeAddress = "CanChangeAddress";
        private const string A_CanChangeLabel = "CanChangeLabel";
        private const string A_AllowEmptyLabel = "AllowEmptyLabel";
        private const string E_ModuleTypes = "ModuleTypes";

        public ControlBusType() {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            doc.LoadXml("<BusType />");
            Element = doc.DocumentElement!;
        }

        /// <summary>
        /// Generic bus type - for example DCC or MotorolaDCC
        /// </summary>
        public string BusFamilyName {
            get => GetAttribute(A_BusFamilyName);
            set => SetAttributeValue(A_BusFamilyName, value);
        }

        /// <summary>
        /// Bus type name is used as a key for bus objects to get their bus type information
        /// </summary>
        public string BusTypeName {
            get => GetAttribute(A_BusTypeName);
            set => SetAttributeValue(A_BusTypeName, value);
        }

        /// <summary>
        /// User friendly name for this bus
        /// </summary>
        public string Name {
            get => GetAttribute(A_Name);
            set => SetAttributeValue(A_Name, value);
        }

        /// <summary>
        /// The addressing method for this bus
        /// </summary>
        public ControlBusTopology Topology {
            get => AttributeValue(A_Topology).Enum<ControlBusTopology>() ?? ControlBusTopology.RandomAddressing;
            set => SetAttributeValue(A_Topology, value);
        }

        public ControlAddressingMethod AddressingMethod {
            get => AttributeValue(A_AddressingMethod).Enum<ControlAddressingMethod>() ?? ControlAddressingMethod.DirectConnectionPointAddressing;
            set => SetAttributeValue(A_AddressingMethod, value);
        }

        public bool CanChangeAddress {
            get => (bool?)AttributeValue(A_CanChangeAddress) ?? true;
            set => SetAttributeValue(A_CanChangeAddress, value, removeIf: true);
        }

        public bool CanChangeLabel {
            get => (bool?)AttributeValue(A_CanChangeLabel) ?? true;
            set => SetAttributeValue(A_CanChangeLabel, value, removeIf: true);
        }

        public bool AllowEmptyLabel {
            get => (bool?)AttributeValue(A_AllowEmptyLabel) ?? true;
            set => SetAttributeValue(A_AllowEmptyLabel, value, removeIf: true);
        }

        /// <summary>
        /// First valid address on the bus
        /// </summary>
        public int FirstAddress {
            get => (int?)AttributeValue(A_FirstAddress) ?? 0;
            set => SetAttributeValue(A_FirstAddress, value);
        }

        /// <summary>
        /// The last valid address on the bus
        /// </summary>
        public int LastAddress {
            get => (int?)AttributeValue(A_LastAddress) ?? 255;
            set => SetAttributeValue(A_LastAddress, value);
        }

        public int RecommendedStartAddress {
            get => (int?)AttributeValue(A_RecommendedStartAddress) ?? FirstAddress;
            set => SetAttributeValue(A_RecommendedStartAddress, value);
        }

        public ControlConnectionPointUsage Usage {
            get => AttributeValue(A_Usage).Enum<ControlConnectionPointUsage>() ?? ControlConnectionPointUsage.Both;
            set => SetAttributeValue(A_Usage, value);
        }

        public string ClickToAddModuleDispatchSource {
            get => (string?)AttributeValue(A_ClickToAddEventName) ?? "ClickToAddControlModule";
            set => SetAttributeValue(A_ClickToAddEventName, value);
        }

        /// <summary>
        /// Return an array of all module types that can be attached to this bus
        /// </summary>
        public IEnumerable<ControlModuleType> ModuleTypes => from controlModule in Dispatch.Call.EnumControlModuleTypes() where controlModule.BusTypeNames.Contains(BusFamilyName) select controlModule;

        /// <summary>
        /// Get a list of all module types of this bus that can be connected to the given connection destination (component/connection description)
        /// </summary>
        /// <param name="connectionDestinaion">The connection destination (component/connection description)</param>
        /// <returns>A list of all module types that can be connected. Empty list if no module (of this bus) can be connected</returns>
        public IList<string> GetConnectableControlModuleTypeNames(ControlConnectionPointDestination connectionDestination) {
            List<string> applicableModuleTypes = new();

            Dispatch.Call.RecommendControlModuleTypes(connectionDestination, applicableModuleTypes, BusTypeName, BusFamilyName);

            return applicableModuleTypes.AsReadOnly();
        }

        /// <summary>
        /// Return an address that meets the alignment requirements of a given module type.
        /// </summary>
        /// <param name="moduleType">The module type</param>
        /// <param name="address">The address will be the first properly aligned address equal or larger than this</param>
        /// <returns>A properly align address for module of the given type</returns>
        public int GetAlignedAddress(ControlModuleType moduleType, int address) {
            int a = address - FirstAddress + moduleType.AddressAlignment - 1;

            return a - (a % moduleType.AddressAlignment) + FirstAddress;
        }

        /// <summary>
        /// Check if the given component/connection description can be connected to a bus of this type
        /// </summary>
        /// <param name="component">The component</param>
        /// <param name="connectionDescription">The connection description</param>
        /// <returns>True: it is possible to connect to a module on this bus. False: It is not possible to connect this to that bus</returns>
        public bool CanBeConnected(ControlConnectionPointDestination connectionDestination) => GetConnectableControlModuleTypeNames(connectionDestination).Count > 0;
    }

    /// <summary>
    /// Entry in the component to connection point map. Please note that a component may be wired to more than
    /// one connection point
    /// </summary>
    internal class ControlConnectionPointMapEntry {
        private ControlConnectionPoint? connectionPoint;         // If there is a single connection point (common case)
        private List<ControlConnectionPoint>? connectionPoints; // If the component is "wired" to more than one connection point

        public ControlConnectionPointMapEntry(ControlConnectionPoint connectionPoint) {
            this.connectionPoint = connectionPoint;
            this.connectionPoints = null;
        }

        /// <summary>
        /// Add another connection point which is wired to this component
        /// </summary>
        /// <param name="connectionPoint">The connection point</param>
        public void Add(ControlConnectionPoint connectionPoint) {
            if (connectionPoints == null) {
                Debug.Assert(this.connectionPoint != null);
                connectionPoints = new List<ControlConnectionPoint> {
                    this.connectionPoint ?? throw new LayoutException("ControlConnectionPointMapEntry - assert failed")
                };
            }

            connectionPoints.Add(connectionPoint);
            this.connectionPoint = null;
        }

        /// <summary>
        /// Remove a given connection point from the map
        /// </summary>
        /// <param name="connectionPointID">The ID of the connection point to be removed</param>
        /// <returns>True - the entry is empty and should be removed from the map</returns>
        public bool Remove(Guid connectionPointID) {
            if (connectionPoints != null) {
                connectionPoints.RemoveAll((ControlConnectionPoint cp) => cp.Id == connectionPointID);

                if (connectionPoints.Count == 1) {
                    this.connectionPoint = (ControlConnectionPoint)connectionPoints[0];
                    connectionPoints = null;
                }

                return false;
            }
            else
                return this.connectionPoint?.Id == connectionPointID;
        }

        public bool Remove(ControlConnectionPoint connectionPoint) => Remove(connectionPoint.Id);

        /// <summary>
        /// Return the one or more connection points which are "wired" to a component
        /// </summary>
        public IList<ControlConnectionPoint> ConnectionPoints {
            get {
                if (connectionPoints == null) {
                    Debug.Assert(this.connectionPoint != null);
                    List<ControlConnectionPoint> theList = new() {
                        connectionPoint ?? throw new LayoutException("ControlConnectionPointMapEntry - assert failed")
                    };
                    return theList.AsReadOnly();
                }
                else
                    return connectionPoints.AsReadOnly();
            }
        }
    }

    /// <summary>
    /// Map from component ID to the connection point objects that are wired to this component. 
    /// This map allows quick access to the connection point object from which one can access the module
    /// object and the bus object
    /// </summary>
    public class ControlConnectionPointsMap : LayoutXmlWrapper {
        private readonly Dictionary<Guid, ControlConnectionPointMapEntry> idMap = new();
        private readonly LayoutControlManager controlManager;

        internal ControlConnectionPointsMap(LayoutControlManager controlManager, XmlElement modulesElement) {
            this.controlManager = controlManager;

            foreach (XmlElement moduleElement in modulesElement) {
                ControlModule module = new(controlManager, moduleElement);

                foreach (XmlElement connectionPointElement in module.ConnectionPointsElement) {
                    ControlConnectionPoint connectionPoint = new(module, connectionPointElement);

                    if (connectionPoint.IsConnected) {
                        if (idMap.TryGetValue(connectionPoint.ComponentId, out ControlConnectionPointMapEntry? entry))
                            entry.Add(connectionPoint);
                        else {
                            entry = new ControlConnectionPointMapEntry(connectionPoint);
                            idMap.Add(connectionPoint.ComponentId, entry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Return all control connection points connected to a component
        /// </summary>
        /// <param name="componentId">The component ID</param>
        /// <returns>A list of all control connection points that are connected to the component</returns>
        public IList<ControlConnectionPoint>? this[Guid componentId] {
            get {
                return idMap.TryGetValue(componentId, out ControlConnectionPointMapEntry? entry)
                    ? (IList<ControlConnectionPoint>)entry.ConnectionPoints
                    : null;
            }
        }

        /// <summary>
        /// Return all control connection points connected to a component
        /// </summary>
        /// <param name="component">The component</param>
        /// <returns>A list of all control connection points that are connected to the component</returns>
        public IList<ControlConnectionPoint>? this[IModelComponentHasId component] => this[component.Id];

        /// <summary>
        /// Get a connection for a specific function
        /// </summary>
        /// <param name="componentId">The component ID</param>
        /// <param name="connectionName">The required function name</param>
        /// <returns>Connection point or null if non is defined</returns>
        public ControlConnectionPoint? this[Guid componentId, string? connectionName] => GetConnection(componentId, connectionName);

        /// <summary>
        /// Get a connection for a specific function
        /// </summary>
        /// <param name="componentId">The component</param>
        /// <param name="connectionName">The required function name</param>
        /// <returns>Connection point or null if non is defined</returns>
        public ControlConnectionPoint? this[IModelComponentHasId component, string connectionName] => GetConnection(component, connectionName);

        /// <summary>
        /// Check if a component is connected to at least one control module
        /// </summary>
        /// <param name="componentId">The component ID</param>
        /// <returns>True if the component is connected to at least one control module</returns>
        public bool IsConnected(Guid componentId) => idMap.ContainsKey(componentId);

        /// <summary>
        /// Check if a component is connected to at least one control module
        /// </summary>
        /// <param name="componentId">The component</param>
        /// <returns>True if the component is connected to at least one control module</returns>
        public bool IsConnected(IModelComponentHasId component) => idMap.ContainsKey(component.Id);

        /// <summary>
        /// Check if a specific connection exist to a component
        /// </summary>
        /// <param name="componentId">The component ID</param>
        /// <param name="connectionName">The connection type</param>
        /// <returns></returns>
        public bool IsConnected(Guid componentId, string connectionName) => GetConnection(componentId, connectionName) != null;

        /// <summary>
        /// Check if a specific connection exist to a component
        /// </summary>
        /// <param name="componentId">The component</param>
        /// <param name="connectionName">The connection type</param>
        /// <returns></returns>
        public bool IsConnected(IModelComponentHasId component, string connectionName) => GetConnection(component.Id, connectionName) != null;

        /// <summary>
        /// Get a control connection point for a specific connection
        /// </summary>
        /// <param name="componentId">The component ID to which the control module is connected</param>
        /// <param name="connectionName">The connection name</param>
        /// <returns>The control connection point, or null if the required connection between this component and a control module does not exist</returns>
        public ControlConnectionPoint? GetConnection(Guid componentId, string? connectionName) {
            var connectionPoints = this[componentId];

            if (connectionPoints != null) {
                if (connectionName == null)
                    return connectionPoints[0];

                foreach (ControlConnectionPoint cp in connectionPoints)
                    if (cp.Name == connectionName)
                        return cp;
            }

            return null;
        }

        /// <summary>
        /// Get a control connection point for a specific connection
        /// </summary>
        /// <param name="componentId">The component to which the control module is connected</param>
        /// <param name="connectionName">The connection name</param>
        /// <returns>The control connection point, or null if the required connection between this component and a control module does not exist</returns>
        public ControlConnectionPoint? GetConnection(IModelComponentHasId component, string connectionName) => GetConnection(component.Id, connectionName);

        /// <summary>
        /// Return true if all the connections to control module required by a component are indeed connected
        /// </summary>
        /// <param name="componentId">The component ID</param>
        /// <returns>True: all connections are connected; false: At least one connection has not been defined</returns>
        public bool IsFullyConnected(Guid componentId) {
            return LayoutModel.Component<ModelComponent>(componentId, LayoutModel.ActivePhases) is IModelComponentConnectToControl component
                && IsFullyConnected(component);
        }

        /// <summary>
        /// Return true if all the connections to control module required by a component are indeed connected
        /// </summary>
        /// <param name="componentId">The component</param>
        /// <returns>True: all connections are connected; false: At least one connection has not been defined</returns>
        public bool IsFullyConnected(IModelComponentConnectToControl component) {
            if (component.ControlConnectionDescriptions.Count > 0) {
                var connectionsToComponent = this[component.Id];

                if (connectionsToComponent != null && connectionsToComponent.Count > 0) {
                    return connectionsToComponent.Count == component.ControlConnectionDescriptions.Count;
                }
                else
                    return false;       // Component is not connected at all
            }
            else
                return true;        // Component has no possible connections so it is fully connected
        }

        internal void Add(ControlConnectionPoint connectionPoint) {
            if (idMap.TryGetValue(connectionPoint.ComponentId, out ControlConnectionPointMapEntry? entry))
                entry.Add(connectionPoint);
            else {
                entry = new ControlConnectionPointMapEntry(connectionPoint);
                idMap.Add(connectionPoint.ComponentId, entry);
            }
        }

        internal void Remove(ControlConnectionPoint connectionPoint) {
            if (idMap.TryGetValue(connectionPoint.ComponentId, out ControlConnectionPointMapEntry? entry)) {
                if (entry.Remove(connectionPoint))
                    idMap.Remove(connectionPoint.ComponentId);
            }
        }
    }

    /// <summary>
    /// A collection of all the bus objects that are defined
    /// </summary>
    public class ControlBusCollection : XmlIndexedCollection<ControlBus, Guid> {
        private readonly LayoutControlManager controlManager;

        internal ControlBusCollection(LayoutControlManager controlManager) : base(controlManager.BusesElement) {
            this.controlManager = controlManager;
        }

        protected override Guid GetItemKey(ControlBus item) => item.Id;

        protected override XmlElement CreateElement(ControlBus item) => Element.OwnerDocument.CreateElement("Bus");

        protected override ControlBus FromElement(XmlElement itemElement) => new(controlManager, itemElement);

        /// <summary>
        /// Return an array of all buses mastered by a given command station
        /// </summary>
        /// <param name="busProviderId">The bus provider ID</param>
        /// <returns>An array of bus objects</returns>
        public IEnumerable<ControlBus> Buses(Guid busProviderId) {
            XmlNodeList? busElements = Element.SelectNodes("Bus[@BusProviderID='" + busProviderId.ToString() + "']");

            if (busElements != null) {
                foreach (XmlElement busElement in busElements)
                    yield return FromElement(busElement);
            }
        }

        /// <summary>
        /// Return an array of all buses mastered by a given command station
        /// </summary>
        /// <param name="busProvider">The command station</param>
        /// <returns>An array of bus objects</returns>
        public IEnumerable<ControlBus> Buses(IModelComponentIsBusProvider busProvider) => Buses(busProvider.Id);

        /// <summary>
        /// Get a bus of a given type of a command station
        /// </summary>
        /// <param name="busProviderId">The bus provider ID</param>
        /// <param name="busTypeName">The bus type name</param>
        /// <returns>or bus</returns>
        public ControlBus? GetBus(Guid busProviderId, string busTypeName) {
            var busElement = (XmlElement?)Element.SelectSingleNode("Bus[@BusProviderID='" + busProviderId.ToString() + "' and @BusTypeName='" + busTypeName + "']");

            return busElement == null ? null : FromElement(busElement);
        }

        public ControlBus? GetBus(IModelComponentIsBusProvider busProvider, string busTypeName) => GetBus(busProvider.Id, busTypeName);

        /// <summary>
        /// Check if a bus of a given type is defined for a given command station
        /// </summary>
        /// <param name="busProviderId">The bus provider</param>
        /// <param name="busTypeName">The bus type name</param>
        /// <returns>True - this bus is defined</returns>
        public bool IsBusDefined(Guid busProviderId, string busTypeName) => GetBus(busProviderId, busTypeName) != null;

        /// <summary>
        /// Check if a bus of a given type is defined for a given command station
        /// </summary>
        /// <param name="busProviderId">The bus provider</param>
        /// <param name="busTypeName">The bus type name</param>
        /// <returns>True - this bus is defined</returns>
        public bool IsBusDefined(IModelComponentIsBusProvider busProvider, string busTypeName) => IsBusDefined(busProvider.Id, busTypeName);

        /// <summary>
        /// Add a new bus provider (in most cases this is a command station) - should be called when a new command station object is added to the model
        /// </summary>
        /// <param name="busProvider">The new command station component</param>
        public void AddBusProvider(IModelComponentIsBusProvider busProvider) {
            // TODO: Enumerate the bus types that this command station supports
            foreach (string busTypeName in busProvider.BusTypeNames) {
                if (!IsBusDefined(busProvider, busTypeName)) {
                    XmlElement busElement = Element.OwnerDocument.CreateElement("Bus");
                    ControlBus bus = new(controlManager, busElement) {
                        BusProvider = busProvider,
                        BusTypeName = busTypeName
                    };

                    Add(bus);
                }
            }

            Dispatch.Notification.OnControlBusAdded(busProvider);
        }

        /// <summary>
        /// Remove all buses that are mastered by a given bus provider
        /// </summary>
        /// <param name="busProvider"></param>
        public void RemoveBusProvider(IModelComponentIsBusProvider busProvider) {
            IEnumerable<LayoutControlModuleLocationComponent> controlModuleLocations = LayoutModel.Components<LayoutControlModuleLocationComponent>(LayoutPhase.All);

            foreach (ControlBus bus in Buses(busProvider)) {
                foreach (ControlModule module in bus.Modules)
                    bus.Remove(module);

                // Remove the bus from the settings of each control module location component
                foreach (LayoutControlModuleLocationComponent controlModuleLocation in controlModuleLocations)
                    controlModuleLocation.Info.Defaults.Remove(bus);

                Remove(bus);
            }

            Dispatch.Notification.OnControlBusRemoved(busProvider);
        }
    }

    public class LayoutControlManager : LayoutXmlWrapper {
        private ControlConnectionPointsMap? connectionPointsMap;
        private ControlBusCollection? buses;

        public LayoutControlManager() {
            XmlDocument xmlDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            xmlDoc.LoadXml("<LayoutControl><Modules /><Buses /></LayoutControl>");
            Element = xmlDoc.DocumentElement!;
        }

        public LayoutControlManager(XmlReader r) {
            XmlDocument xmlDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            xmlDoc.Load(r);
            Element = Ensure.NotNull<XmlElement>(xmlDoc.DocumentElement);
        }

        public void WriteXml(XmlWriter w) {
            Element.WriteTo(w);
        }

        public XmlElement ModulesElement {
            get {
                XmlElement? modulesElement = Element["Modules"];

                if (modulesElement == null) {
                    modulesElement = Element.OwnerDocument.CreateElement("Modules");
                    Element.AppendChild(modulesElement);
                }

                return modulesElement;
            }
        }

        public XmlElement BusesElement {
            get {
                XmlElement? busesElement = Element["Buses"];

                if (busesElement == null) {
                    busesElement = Element.OwnerDocument.CreateElement("Buses");
                    Element.AppendChild(busesElement);
                }

                return busesElement;
            }
        }

        /// <summary>
        /// Return all modules which are "stored" to a given location
        /// </summary>
        /// <param name="locationID">The ID of the location</param>
        /// <returns>An array of all modules in this location</returns>
        public ControlModule[] GetModulesAtLocation(Guid locationId) {
            var modulesElement = ModulesElement.SelectNodes("Module[@LocationID='" + locationId.ToString() + "']");

            ControlModule[] modules = new ControlModule[modulesElement?.Count ?? 0];

            if (modulesElement != null) {
                int i = 0;
                foreach (XmlElement moduleElement in modulesElement)
                    modules[i++] = new ControlModule(this, moduleElement);
            }

            return modules;
        }

        /// <summary>
        /// Return all modules which are "stored" to a given location
        /// </summary>
        /// <param name="moduleLocationComponent">The control module location component</param>
        /// <returns>An array of all modules in this location</returns>
        public ControlModule[] GetModulesAtLocation(LayoutControlModuleLocationComponent moduleLocationComponent) => GetModulesAtLocation(moduleLocationComponent.Id);

        /// <summary>
        /// Return a map from components (or component IDs) to connection points (usually one) that are connected to
        /// the component
        /// </summary>
        public ControlConnectionPointsMap ConnectionPoints => connectionPointsMap ??= new ControlConnectionPointsMap(this, ModulesElement);

        public ControlBusCollection Buses => buses ??= new ControlBusCollection(this);

        /// <summary>
        /// Get a module given its ID
        /// </summary>
        /// <param name="moduleID">The module ID</param>
        /// <returns>A ControlModule object</returns>
        public ControlModule? GetModule(Guid moduleId) {
            var moduleElement = (XmlElement?)ModulesElement.SelectSingleNode("Module[@ID='" + moduleId.ToString() + "']");

            return moduleElement == null ? null : new ControlModule(this, moduleElement);
        }
    }
}