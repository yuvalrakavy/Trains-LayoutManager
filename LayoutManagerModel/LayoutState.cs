using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using LayoutManager.Components;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace LayoutManager.Model {
    #region State Management Base classes

    public class LayoutStateInfoBase : LayoutInfo {
        public LayoutStateInfoBase(XmlElement element)
            : base(element) {
        }
    }

    public class LayoutStateTopicBase : LayoutStateInfoBase {
        public LayoutStateTopicBase(XmlElement element)
            : base(element) {
        }

        public XmlElement ComponentStateElement => (XmlElement)Element.ParentNode;

        public Guid ComponentId => (Guid)ComponentStateElement.AttributeValue("ID");
    }

    #endregion

    #region Locomotive Placment validity check result

    public enum CanPlaceTrainStatus {
        CanPlaceTrain,
        TrainLocomotiveAlreadyUsed,
        LocomotiveHasNoAddress,
        LocomotiveDuplicateAddress,
        LocomotiveAddressAlreadyUsed,
        LocomotiveNotCompatible,
        LocomotiveGuageNotCompatibleWithTrack,
        LocomotiveDigitalFormatNotCompatible,
    };

    public enum CanPlaceTrainResolveMethod {
        Resolved,           // No need to resolve anything
        NotPossible,        // Not possible to resolve - train cannot be placed on track
        ReprogramAddress,   // Locomotive address need to change in order to place it on track
    }

    public class CanPlaceTrainResult {
        private TrainCommonInfo? train;
        private TrainLocomotiveInfo? trainLocomotive;
        private LocomotiveInfo? loco1;
        private LocomotiveInfo? loco2;
        private LayoutBlockDefinitionComponent? blockDefinition;

        public CanPlaceTrainResult() {
            Status = CanPlaceTrainStatus.CanPlaceTrain;
            CanBeResolved = true;
            ResolveMethod = CanPlaceTrainResolveMethod.Resolved;
        }

        public TrainCommonInfo Train { get => Ensure.NotNull<TrainCommonInfo>(train, "Train"); set => train = value; }

        public TrainLocomotiveInfo TrainLocomotive { get => Ensure.NotNull<TrainLocomotiveInfo>(trainLocomotive, "TrainLocomotive"); set => trainLocomotive = value; }

        public CanPlaceTrainStatus Status { set; get; }

        public bool CanBeResolved { get; set; }

        public CanPlaceTrainResolveMethod ResolveMethod { get; set; }

        public LocomotiveInfo Loco1 { get => Ensure.NotNull<LocomotiveInfo>(loco1, "Loco1"); set => loco1 = value; }
        public LocomotiveInfo Loco2 { get => Ensure.NotNull<LocomotiveInfo>(loco2, "Loco2"); set => loco2 = value; }

        public LocomotiveInfo Locomotive {
            set => Loco1 = value;
            get => Loco1;
        }

        public LayoutBlockDefinitionComponent BlockDefinition { get => Ensure.NotNull<LayoutBlockDefinitionComponent>(blockDefinition, "BlockDefinition"); set => blockDefinition = value; }

        public override string ToString() {
            switch (Status) {
                case CanPlaceTrainStatus.CanPlaceTrain:
                    return "Can place train";

                case CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed:
                    return "Locomotive " + Loco1.DisplayName + " address is already used by another locomotive (" + Train.DisplayName + ")";

                case CanPlaceTrainStatus.LocomotiveDuplicateAddress:
                    return "Train " + Train.DisplayName + " members: '" + Loco1.DisplayName + "' and '" + Loco2.DisplayName + "' have the same address";

                case CanPlaceTrainStatus.LocomotiveHasNoAddress:
                    return "Locomotive " + Locomotive.DisplayName + " has no assigned address";

                case CanPlaceTrainStatus.TrainLocomotiveAlreadyUsed:
                    return "Locomotive " + Locomotive.DisplayName + " address is already used by another locomotive (" + Train.DisplayName + ")";

                case CanPlaceTrainStatus.LocomotiveNotCompatible:
                    return "Locomotive " + Locomotive.DisplayName + " is not compatible with locomotives in train '" + Train.DisplayName + "' (e.g. # of speed steps)";

                case CanPlaceTrainStatus.LocomotiveGuageNotCompatibleWithTrack:
                    return "Locomotive " + Locomotive.DisplayName + " guage (" + Locomotive.Guage.ToString() + ") not compatible with track guage (" + BlockDefinition.Guage.ToString() + ")";

                case CanPlaceTrainStatus.LocomotiveDigitalFormatNotCompatible:
                    return "Locomotive " + Locomotive.DisplayName + " decoder is not supported by command station";

                default:
                    return "*No error text is defined for: " + Status.ToString();
            }
        }
    }

    #endregion

    #region Locomotive Address Map - map locomotive address to the object having this address (usually locomotive)

    public abstract class LocomotiveAddressMapEntryBase {
    }

    public abstract class LocomotiveAddressMapEntry : LocomotiveAddressMapEntryBase {
        public LocomotiveInfo Locomotive { get; }

        protected LocomotiveAddressMapEntry(LocomotiveInfo locomotive) {
            this.Locomotive = locomotive;
        }
    }

    public abstract class OnTrackLocomotiveAddressMapEntry : LocomotiveAddressMapEntry {
        public TrainStateInfo Train { get; }

        protected OnTrackLocomotiveAddressMapEntry(LocomotiveInfo locomotive, TrainStateInfo train)
            : base(locomotive) {
            this.Train = train;
        }
    }

    public class OnPoweredTrackLocomotiveAddressMapEntry : OnTrackLocomotiveAddressMapEntry {
        public OnPoweredTrackLocomotiveAddressMapEntry(LocomotiveInfo locomotive, TrainStateInfo train)
            : base(locomotive, train) {
        }
    }

    public class OnDisconnectedTrackLocomotiveAddressMapEntry : OnTrackLocomotiveAddressMapEntry {
        public OnDisconnectedTrackLocomotiveAddressMapEntry(LocomotiveInfo locomotive, TrainStateInfo train)
            : base(locomotive, train) {
        }
    }

    public class OnShelfLocomotiveAddressMapEntry : LocomotiveAddressMapEntry {
        public OnShelfLocomotiveAddressMapEntry(LocomotiveInfo locomotive)
            : base(locomotive) {
        }
    }

    public class LocomotiveBusModuleAddressMapEntry : LocomotiveAddressMapEntryBase {
        public ControlModule Module { get; }

        public LocomotiveBusModuleAddressMapEntry(ControlModule module) {
            this.Module = module;
        }
    }

    public class LocomotiveAddressMap : Dictionary<int, LocomotiveAddressMapEntryBase> {
        public new LocomotiveAddressMapEntryBase? this[int address] {
            get {
                if (TryGetValue(address, out LocomotiveAddressMapEntryBase entry))
                    return entry;
                return null;
            }
        }
    }

    public class OnTrackLocomotiveAddressMap : LocomotiveAddressMap {
        public new OnTrackLocomotiveAddressMapEntry? this[int address] => base[address] as OnTrackLocomotiveAddressMapEntry;
    }

    #endregion

    #region Ongoing operations state

    public interface IOperationState {
    }

    public class PerOperationStates : IEnumerable<KeyValuePair<object, IOperationState>> {
        private readonly Dictionary<object, IOperationState> states = new Dictionary<object, IOperationState>();

        public bool HasState(object key) => states.ContainsKey(key);

        public void Remove(object key) {
            states.Remove(key);
        }

        public T? Get<T>(object key) where T : class, IOperationState => states[key] as T;

        public void Set<T>(object key, T state) where T : class, IOperationState {
            states.Add(key, state);
        }

        #region IEnumerable<KeyValuePair<object,IOperationState>> Members

        public IEnumerator<KeyValuePair<object, IOperationState>> GetEnumerator() => states.GetEnumerator();

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion
    }

    public class OperationStates {
        private readonly Dictionary<string, PerOperationStates> operationStates = new Dictionary<string, PerOperationStates>();

        public PerOperationStates this[string operationName] {
            get {
                if (!operationStates.TryGetValue(operationName, out PerOperationStates perOperationStates)) {
                    perOperationStates = new PerOperationStates();
                    operationStates.Add(operationName, perOperationStates);
                }

                return perOperationStates;
            }
        }
    }

    #endregion

    #region LayoutOperationContext

    /// <summary>
    /// Context for ongoing operation (for example, placing locomotive on track). The context make it possible to figure out that an
    /// operation is in progress, it also allow one to cancel it.
    /// </summary>
    /// <example>
    /// 
    /// Invoking the operation: 
    /// 
    /// using(var context = new LayoutOperationContext("Placement", "placing loco on track", loco, blockDefinition)) {
    ///		await EventManager.AsyncEvent(new LayoutEvent(...).SetOpertationContext(context);
    /// }
    /// 
    /// When building context menu for block definition:
    /// 
    /// if(LayoutOperationContext.HasPendingOperation("Placement", blockDefinition)) {
    ///		var context = LayoutOperationContext.GetPendingOperation("Placement", blockDefinition);
    ///		
    ///		m.MenuItems.Add("Cancel " + context.Description, (s, ev) => context.Cancel());
    /// }
    /// 
    /// </example>
    public class LayoutOperationContext : IObjectHasId, IOperationState, IDisposable {
        public const string ContextSectionName = "OperationContext";

        /// <summary>
        /// Define a context for ongoing operation (e.g. placing a locomotive)
        /// </summary>
        /// <param name="operationName">The operation name (e.g. "Placement"</param>
        /// <param name="description">Human readable description (e.g. "placing 'Red crok' on track')</param>
        /// <param name="participants">Objects that participate in the operation (e.g. the locomotive and the block definition on which the locomotive is placed)</param>
        public LayoutOperationContext(string operationName, string description, params IObjectHasId[] participants) {
            OperationName = operationName;
            Description = description;
            Id = Guid.NewGuid();
            CancellationTokenSource = new CancellationTokenSource();

            PerOperationStates operationContexts = LayoutModel.StateManager.OperationStates[ContextSectionName];

            operationContexts.Set(GetKey(this), this);
            foreach (var participant in participants)
                operationContexts.Set(GetKey(participant), this);
        }

        /// <summary>
        /// Dispose the operation context
        /// </summary>
        public void Dispose() {
            PerOperationStates operationContexts = LayoutModel.StateManager.OperationStates[ContextSectionName];
            var removeList = new List<string>(10);
            string myKey = Key;

            // Pass on all the pending operation and remove all references based on the operation id, or any operation's participant id
            foreach (var pair in operationContexts) {
                string key = (string)pair.Key;

                if (pair.Value is LayoutOperationContext operationContext && myKey == GetKey(operationContext))
                    removeList.Add(key);
            }

            foreach (var key in removeList)
                operationContexts.Remove(key);
        }

        /// <summary>
        /// Get a composed key for a given operation and a given participant
        /// </summary>
        /// <param name="operationName">The operation name</param>
        /// <param name="participant">The participant object</param>
        /// <returns>A composed key</returns>
        protected static string GetKey(string operationName, IObjectHasId participant) => operationName + "-" + participant.Id.ToString();

        /// <summary>
        /// Get key for the current operation context and given participant
        /// </summary>
        /// <param name="participant">The  participant</param>
        /// <returns>A composed key</returns>
        protected string GetKey(IObjectHasId participant) => GetKey(OperationName, participant);

        #region Properties

        /// <summary>
        /// Operation context Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The operation name
        /// </summary>
        public string OperationName { get; }

        /// <summary>
        /// Human readable description of this operation
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Cancellation token source for the ongoing operation
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Cancellation token for the ongoing operation
        /// </summary>
        public CancellationToken CancelationToken => CancellationTokenSource.Token;

        /// <summary>
        /// Return a key that can be used to find this operation in the state map
        /// </summary>
        public string Key => GetKey(this);

        #endregion

        #region Operations

        /// <summary>
        /// Cancel the ongoing operation
        /// </summary>
        public void Cancel() {
            if (HasPendingOperation(OperationName, this)) {             // If operation is still going on
                if (!CancellationTokenSource.IsCancellationRequested)   // and it was not already cancelled
                    CancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Check if a given object has a pending operation
        /// </summary>
        /// <param name="operationName">The operation name</param>
        /// <param name="participant">The object for which to check if there is a pending operation</param>
        /// <returns></returns>
        public static bool HasPendingOperation(string operationName, IObjectHasId participant) => LayoutModel.StateManager.OperationStates[ContextSectionName].HasState(GetKey(operationName, participant));

        /// <summary>
        /// Get the context of a specified operation for a specific object
        /// </summary>
        /// <param name="operationName">The operation name</param>
        /// <param name="participant">The object for which to get operation context</param>
        /// <returns>Operation context or null if there is no pending operation for this object</returns>
        public static LayoutOperationContext? GetPendingOperation(string operationName, IObjectHasId participant) {
            var operationContexts = LayoutModel.StateManager.OperationStates[ContextSectionName];
            string key = GetKey(operationName, participant);

            if (operationContexts.HasState(key))
                return operationContexts.Get<LayoutOperationContext>(key);
            else
                return null;
        }

        /// <summary>
        /// Add another participant in for this operation
        /// </summary>
        /// <param name="participant">Another participant (e.g. block definintion) in this operation</param>
        public void AddPariticpant(IObjectHasId participant) {
            if (!HasPendingOperation(OperationName, participant))
                LayoutModel.StateManager.OperationStates[ContextSectionName].Set<LayoutOperationContext>(GetKey(OperationName, participant), this);
        }

        /// <summary>
        /// Remove a participant in an operation
        /// </summary>
        /// <param name="participant">The participant to remove</param>
        public void RemoveParticipant(IObjectHasId participant) {
            if (HasPendingOperation(OperationName, participant))
                LayoutModel.StateManager.OperationStates[ContextSectionName].Remove(GetKey(OperationName, participant));
        }

        #endregion
    }

    /// <summary>
    /// Extend layout event with 
    /// </summary>
    static public class LayoutEventOperationContextExtender {
        private const string elementName = "OperationContext";

        /// <summary>
        /// Set the operation context for which an event is invoked
        /// </summary>
        /// <param name="context">The operation context</param>
        /// <returns>The event</returns>
        public static LayoutEvent SetOperationContext(this LayoutEvent theEvent, LayoutOperationContext context) {
            theEvent.SetOption(elementName, "Key", context.Key);
            return theEvent;
        }

        /// <summary>
        /// Copy the operation context of another event to this event
        /// </summary>
        /// <param name="otherEvent">The other event</param>
        /// <returns>The event</returns>
        public static LayoutEvent CopyOperationContext(this LayoutEvent theEvent, LayoutEvent otherEvent) {
            theEvent.CopyOptions(otherEvent, elementName);
            return theEvent;
        }

        /// <summary>
        /// Get the operation context under which an event is invoked
        /// </summary>
        /// <returns>Operation context or null if this event was not invoked under any operation context</returns>
        public static LayoutOperationContext? GetOperationContext(this LayoutEvent theEvent) {
            var key = theEvent.GetOption(elementName: elementName, optionName: "Key");

            return key != null
                ? LayoutModel.StateManager.OperationStates[LayoutOperationContext.ContextSectionName].Get<LayoutOperationContext>(key)
                : null;
        }

        /// <summary>
        /// Return cancellation token that should be used to indicate cancellation of the operation
        /// </summary>
        /// <returns>Cancelation token</returns>
        public static CancellationToken GetCancellationToken(this LayoutEvent theEvent) {
            var context = GetOperationContext(theEvent);

            if (context != null)
                return context.CancelationToken;
            else
                return System.Threading.CancellationToken.None;
        }
    }

    #endregion

    #region Locomotive programming progress state

    public abstract class ProgrammingState : IOperationState {
        private ILayoutActionContainer? programmingActions;

        public ILayoutActionContainer ProgrammingActions { get => Ensure.NotNull<ILayoutActionContainer>(programmingActions, "ProgrammingActions"); set => programmingActions = value; }
        public LayoutBlockDefinitionComponent? ProgrammingLocation { get; set; }
    }

    public class LocomotiveProgrammingState : ProgrammingState {
        public LocomotiveInfo Locomotive { get; }
        public LayoutBlockDefinitionComponent? PlacementLocation { get; protected set; }

        public LocomotiveProgrammingState(LocomotiveInfo locomotive) {
            this.Locomotive = locomotive;
        }
    }

    public class PlacedLocomotiveProgrammingState : LocomotiveProgrammingState {
        public XmlElement PlacedElement { get; }

        public PlacedLocomotiveProgrammingState(XmlElement placedElement, LocomotiveInfo locomotive, LayoutBlockDefinitionComponent placementLocation) : base(locomotive) {
            this.PlacedElement = placedElement;
            this.PlacementLocation = placementLocation;
        }
    }

    public class ControlModuleProgrammingState : ProgrammingState {
        public ControlModule ControlModule { get; }
        public IModelComponentCanProgramLocomotives Programmer { get; }

        public ControlModuleProgrammingState(ControlModule controlModule, IModelComponentCanProgramLocomotives programmer) {
            this.ControlModule = controlModule;
            this.Programmer = programmer;
        }
    }

    #endregion

    #region Trains State

    public enum TrainPart {
        Locomotive, Car, LastCar, None
    };

    public class TrainStateInfo : TrainCommonInfo, IEqualityComparer<TrainStateInfo> {
        // WARNING: Do not add local variables, as this class is stateless, and a new instance is created for the each request
        // for the same train ID.

        // The following variables are used to accelerate the conversion between speed in speed steps and logical speed
        private double trainToLogicalSpeedFactor;
        private TripPlanAssignmentInfo? trip;

        private enum SpeedConversionState {
            NotSet,             // Conversion factor was not calculated
            NotNeeded,          // The number of speed steps for this train is the same as the number of steps for logical speed
            Set                 // The conversion factor was calculated
        };

        private SpeedConversionState speedConversionState = SpeedConversionState.NotSet;

        public TrainStateInfo(XmlElement element)
            : base(element) {
        }

        #region Properties

        /// <summary>
        /// The collection of track blocks in which the train is currently located on the track
        /// </summary>
        public IList<TrainLocationInfo> Locations {
            get {
                List<TrainLocationInfo> trainLocations = new List<TrainLocationInfo>(LocationsElement.ChildNodes.Count);

                foreach (XmlElement locationElement in LocationsElement)
                    trainLocations.Add(new TrainLocationInfo(this, locationElement));

                return trainLocations.AsReadOnly();
            }
        }

        public XmlElement LocationsElement {
            get {
                XmlElement locationsElement = Element["Locations"];

                if (locationsElement == null) {
                    locationsElement = Element.OwnerDocument.CreateElement("Locations");
                    Element.AppendChild(locationsElement);
                }

                return locationsElement;
            }
        }

        /// <summary>
        /// Return the location element for the frontmost train location (usually the locomotive)
        /// </summary>
        public TrainLocationInfo? LocomotiveLocation {
            get {
                if (LocationsElement.HasChildNodes)
                    return new TrainLocationInfo(this, (XmlElement)LocationsElement.FirstChild);
                else
                    return null;
            }
        }

        /// <summary>
        /// The block on which the frontmost train element is located (usually it will be a locomotive)
        /// </summary>
        public LayoutBlock? LocomotiveBlock {
            get {
                var locomotiveLocation = LocomotiveLocation;

                if (locomotiveLocation != null)
                    return LayoutModel.Blocks[locomotiveLocation.BlockId];
                else
                    return null;
            }
        }

        public TrainLocationInfo? LastCarLocation {
            get {
                if (LocationsElement.HasChildNodes)
                    return new TrainLocationInfo(this, (XmlElement)LocationsElement.LastChild);
                else
                    return null;
            }
        }

        public LayoutBlock? LastCarBlock {
            get {
                var lastCarLocation = LastCarLocation;

                if (lastCarLocation != null)
                    return LayoutModel.Blocks[lastCarLocation.BlockId];
                else
                    return null;
            }
        }

        public override int SpeedLimit {
            get => base.SpeedLimit;

            set {
                base.SpeedLimit = value;

                RefreshSpeedLimit();
            }
        }

        public override int TargetSpeed {
            get => base.TargetSpeed;

            set {
                base.TargetSpeed = value;
                EventManager.Event(new LayoutEvent("train-target-speed-changed", this, value));
            }
        }

        private const string A_CurrentSpeedLimit = "CurrentSpeedLimit";

        public int CurrentSpeedLimit {
            get => (int?)AttributeValue(A_CurrentSpeedLimit) ?? 0;
            set => SetAttribute(A_CurrentSpeedLimit, value);
        }

        private const string A_CurrentSlowDownSpeed = "CurrentSlowDownSpeed";

        public int CurrentSlowdownSpeed {
            get => (int?)AttributeValue(A_CurrentSlowDownSpeed) ?? 0;
            set => SetAttribute(A_CurrentSlowDownSpeed, value);
        }

        private const string A__RequestedSpeedLimit = "_RequestedSpeedLimit";

        protected int RequestedSpeedLimit {
            get => (int?)AttributeValue(A__RequestedSpeedLimit) ?? 0;
            set => SetAttribute(A__RequestedSpeedLimit, value);
        }

        private const string A_MotionDirection = "MotionDirection";

        public LocomotiveOrientation MotionDirection {
            get => AttributeValue(A_MotionDirection).Enum<LocomotiveOrientation>() ?? LocomotiveOrientation.Forward;
            set => SetAttribute(A_MotionDirection, value);
        }

        /// <summary>
        /// Get or set the train speed and direction. The speed is given in logical speed units (0 to Model.LogicalSpeedSteps).
        /// Negative value of speed indicated a backward motion, positive number indicates forward motion.
        /// </summary>
        public int Speed {
            get {
                if (speedConversionState == SpeedConversionState.NotSet) {
                    if (LayoutModel.Instance.LogicalSpeedSteps == this.SpeedSteps)
                        speedConversionState = SpeedConversionState.NotNeeded;
                    else {
                        trainToLogicalSpeedFactor = (double)LayoutModel.Instance.LogicalSpeedSteps / (double)this.SpeedSteps;
                        speedConversionState = SpeedConversionState.Set;
                    }
                }

                if (speedConversionState == SpeedConversionState.NotNeeded)
                    return SpeedInSteps;
                else
                    return (int)Math.Round(SpeedInSteps * trainToLogicalSpeedFactor);
            }

            set {
                if (NotManaged)
                    throw new LocomotiveNotManagedException(Locomotives[0].Locomotive);
                int currentSpeed = Speed;

                if (value != currentSpeed) {
                    if (value > currentSpeed)
                        ChangeSpeed(value, AccelerationRamp);
                    else
                        ChangeSpeed(value, DecelerationRamp);
                }
            }
        }

        private const string A_FinalSpeed = "FinalSpeed";

        /// <summary>
        /// The speed to which the train is set. After acceleration or deceleration is done, Speed will be equal to FinalSpeed
        /// </summary>
        public int FinalSpeed {
            get => (int?)AttributeValue(A_FinalSpeed) ?? 0;
            set => SetAttribute(A_FinalSpeed, value);
        }

        private const string A_Speed = "Speed";

        /// <summary>
        /// Get or set the train speed and direction. The speed is given speed steps unit (0..this.SpeedSteps)
        /// Negative value of speed indicated a backward motion, positive number indicates forward motion.
        /// </summary>
        public int SpeedInSteps {
            get => (int?)AttributeValue(A_Speed) ?? 0;

            set {
                if (NotManaged)
                    throw new LocomotiveNotManagedException(Locomotives[0].Locomotive);

                LocomotiveOrientation direction = MotionDirection;

                if ((value > 0 && direction == LocomotiveOrientation.Backward) || (value < 0 && direction == LocomotiveOrientation.Forward))
                    EventManager.Event(new LayoutEvent("reverse-train-motion-direction-request", this));

                if (value > 0)
                    MotionDirection = LocomotiveOrientation.Forward;
                else if (value < 0)
                    MotionDirection = LocomotiveOrientation.Backward;

                EventManager.Event(new LayoutEvent("set-train-speed-request", this, value));
            }
        }

        /// <summary>
        /// Search for a motion ramp of a given role. First check if the block with the locomotive
        /// overrides the default, then check if the train overrides the default, and if neither
        /// override the default, return the default ramp of this type
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        protected MotionRampInfo SearchRamp(string role) {
            MotionRampInfo? ramp = null;

            Debug.Assert(LocomotiveBlock != null);

            if (LocomotiveBlock.BlockDefinintion != null)
                ramp = LayoutStateManager.GetRamp(LocomotiveBlock.BlockDefinintion.Element, role);

            if (ramp == null)
                ramp = LayoutStateManager.GetRamp(Element, role);

            if (ramp == null)
                ramp = LayoutModel.StateManager.GetDefaultRamp(role);

            Debug.Assert(ramp != null);
            return ramp;
        }

        /// <summary>
        /// Return the motion ramp that should be used to accelerate the train
        /// </summary>
        public MotionRampInfo AccelerationRamp => SearchRamp("Acceleration");

        /// <summary>
        /// Return the motion ramp that should be used to decelerate the train
        /// </summary>
        public MotionRampInfo DecelerationRamp => SearchRamp("Deceleration");

        /// <summary>
        /// Return the ramp that should be used to slow down the train (in preparation to stop)
        /// </summary>
        public MotionRampInfo SlowdownRamp => SearchRamp("SlowDown");

        /// <summary>
        /// Return the ramp that should be used to stop the train
        /// </summary>
        public MotionRampInfo StopRamp => SearchRamp("Stop");

        private const string A_Lights = "Lights";

        /// <summary>
        /// Get or set the locomotive light state
        /// </summary>
        public bool Lights {
            get => (bool?)AttributeValue(A_Lights) ?? false;

            set {
                if (NotManaged)
                    throw new LocomotiveNotManagedException(Locomotives[0].Locomotive);
                EventManager.Event(new LayoutEvent("set-train-lights-request", this, value));
            }
        }

        /// <summary>
        /// Return human readable status
        /// </summary>
        public string StatusText {
            get {
                if (NotManaged) {
                    string location = "in layout";

                    if (LocomotiveBlock?.BlockDefinintion != null) {
                        if (LocomotiveBlock.BlockDefinintion.NameProvider.Element != null)
                            location = "in " + LocomotiveBlock.BlockDefinintion.NameProvider.Name;
                    }

                    return "Not managed - " + location;
                }
                else {
                    string motion;
                    string location = "";

                    if (Speed == 0)
                        motion = "Standing";
                    else if (Speed > 0)
                        motion = "Forward at " + Speed;
                    else
                        motion = "Backward at " + -Speed;

                    if (LocomotiveBlock != null && LocomotiveBlock.BlockDefinintion != null) {
                        if (LocomotiveBlock.BlockDefinintion.NameProvider.Element != null)
                            location = " in " + LocomotiveBlock.BlockDefinintion.NameProvider.Name;
                    }

                    return motion + location;
                }
            }
        }

        private const string A_InTrip = "InTrip";

        /// <summary>
        /// Return the trip that controls this trin, or null if this train is not currently
        /// controled by a trip
        /// </summary>
        public TripPlanAssignmentInfo? Trip {
            get {
                if (HasAttribute(A_InTrip)) {
                    if (trip == null) {
                        trip = (TripPlanAssignmentInfo?)EventManager.Event(new LayoutEvent("get-train-active-trip", this));
                        if (trip == null)
                            Element.RemoveAttribute(A_InTrip);
                    }
                    return trip;
                }
                else
                    return null;
            }

            set {
                if (value == null) {
                    Element.RemoveAttribute(A_InTrip);
                    trip = null;
                }
                else {
                    Element.SetAttribute(A_InTrip, "true");
                    trip = value;
                }
            }
        }

        /// <summary>
        /// The power that is this train is powered with
        /// </summary>
        public ILayoutPower? Power => LocomotiveBlock?.BlockDefinintion.Power;

        /// <summary>
        /// The command station that is controlling this train
        /// </summary>
        public IModelComponentHasPowerOutlets? CommandStation {
            get {
                if (Power == null || Power.PowerOriginComponentId == Guid.Empty) {
                    Debug.Assert(Power != null && Power.Type == LayoutPowerType.Disconnected);
                    return null;        // Power inlet is connected to thin air.
                }

                return Power.PowerOriginComponent;
            }
        }

        private static bool HasPendingOperation(IObjectHasId participant) {
            if (LayoutOperationContext.HasPendingOperation("TrainPlacement", participant))
                return true;
            if (LayoutOperationContext.HasPendingOperation("TrainRemoval", participant))
                return true;
            return false;
        }

        /// <summary>
        /// Return true if train is on track. This is usally the case, except for trains that are
        /// in the middle of being placed on the track
        /// </summary>
        public bool OnTrack {
            get {
                if (LocomotiveBlock == null)
                    return false;
                if (HasPendingOperation(this))
                    return false;
                if ((from trainLoco in Locomotives select trainLoco.Locomotive).Any(l => HasPendingOperation(l)))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// True if train is powered (connected to digital command station)
        /// </summary>
        public bool IsPowered {
            get {
                if (Power == null)
                    return false;
                else
                    return Power.Type == LayoutPowerType.Digital;
            }
        }

        #endregion

        #region Operations

        #region Train Locomotives Management

        #region Add Locomotive

        public override CanPlaceTrainResult AddLocomotive(LocomotiveInfo loco, LocomotiveOrientation orientation, LayoutBlock? block, bool validateAddress) {
            CanPlaceTrainResult result;

            if (block != null)
                // Verify that indeed the locomotive can be added (exception is thrown if not)
                result = EventManager.Event<XmlElement, LayoutBlockDefinitionComponent, CanPlaceTrainResult>("can-locomotive-be-placed-on-track", loco.Element, block.BlockDefinintion)!;
            else
                result = new CanPlaceTrainResult() { CanBeResolved = true, Status = CanPlaceTrainStatus.CanPlaceTrain };

            if (result.Status == CanPlaceTrainStatus.CanPlaceTrain) {
                if (validateAddress) {
                    if (LocomotiveBlock == null)
                        result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent("is-locomotive-address-valid", loco.Element, block, "<Orientation Value='" + orientation.ToString() + "' />"))!;
                    else
                        result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent("is-locomotive-address-valid", loco.Element, this, "<Orientation Value='" + orientation.ToString() + "' />"))!;
                }

                if (LocomotiveBlock != null)
                    block = LocomotiveBlock;
            }

            if (result.Status == CanPlaceTrainStatus.CanPlaceTrain) {
                result = base.AddLocomotive(loco, orientation, block, validateAddress);

                if (result.Status == CanPlaceTrainStatus.CanPlaceTrain) {
                    TrainLocomotiveInfo trainLocomotive = result.TrainLocomotive;

                    LayoutModel.StateManager.Trains.IdToTrainStateElement[loco.Id] = Element;
                    EventManager.Event(new LayoutEvent("locomotive-added-to-train", this, trainLocomotive));
                }
            }

            return result;
        }

        public void AddTrainFromCollection(TrainInCollectionInfo trainInCollection, LayoutBlock block, bool validateAddress) {
            CopyFrom(trainInCollection, block, validateAddress);
        }

        public void AddLocomotiveCollectionElement(XmlElement collectionElement, LayoutBlock block, bool validateAddress) {
            if (collectionElement.Name == "Locomotive")
                AddLocomotive(new LocomotiveInfo(collectionElement), LocomotiveOrientation.Forward, block, validateAddress);
            else if (collectionElement.Name == "Train") // TODO: Check this code
                AddTrainFromCollection(new TrainInCollectionInfo(collectionElement), block, validateAddress);
            else
                Debug.Assert(false, "Invalid locomotive collection element");
        }

        #endregion

        public override void RemoveLocomotive(TrainLocomotiveInfo removedTrainLoco) {
            EventManager.Event(new LayoutEvent("removing-locomotive-from-train", removedTrainLoco, this));

            LayoutModel.StateManager.Trains.IdToTrainStateElement.Remove(removedTrainLoco.Locomotive.Id);
            LayoutModel.StateManager.Trains.IdToTrainStateElement.Remove(removedTrainLoco.CollectionElementId);

            base.RemoveLocomotive(removedTrainLoco);

            EventManager.Event(new LayoutEvent("locomotive-removed-from-train", this));
        }

        #endregion

        #region Train location management

        public TrainLocationInfo? LocationOfBlock(LayoutBlock block) {
            foreach (TrainLocationInfo trainLocation in Locations)
                if (trainLocation.BlockId == block.Id)
                    return trainLocation;

            return null;
        }

        public TrainLocationInfo PlaceInBlock(LayoutBlock block, LayoutComponentConnectionPoint front) {
            TrainLocationInfo trainLocation = new TrainLocationInfo(this, Element.OwnerDocument.CreateElement("Block"));
            XmlElement locationsElement = LocationsElement;

            trainLocation.BlockId = block.Id;
            trainLocation.DisplayFront = front;
            locationsElement.InsertBefore(trainLocation.Element, locationsElement.FirstChild);
            block.AddTrain(trainLocation);

            Redraw();

            return trainLocation;
        }

        /// <summary>
        /// This method is called when the train "enters" a new block
        /// </summary>
        /// <param name="trainPart">which train part is in the block. Can be either Locomotive or LastCar</param>
        /// <param name="block">The block</param>
        /// <param name="blockEdge">An optional track contact that was crossed when the train entered the block (or null)</param>
        /// <returns>The new train location object</returns>
        public TrainLocationInfo EnterBlock(TrainPart trainPart, LayoutBlock block, LayoutBlockEdgeBase blockEdge, string? eventName) {
            if (trainPart != TrainPart.Locomotive && trainPart != TrainPart.LastCar)
                throw new ArgumentException("Invalid train part - can be only Locomotive or LastCar", nameof(trainPart));

            TrainLocationInfo trainLocation = new TrainLocationInfo(this, Element.OwnerDocument.CreateElement("Block"));
            XmlElement locationsElement = LocationsElement;

            trainLocation.BlockId = block.Id;

            if (blockEdge != null)
                trainLocation.BlockEdgeId = blockEdge.Id;

            if (trainPart == TrainPart.Locomotive)
                locationsElement.InsertBefore(trainLocation.Element, locationsElement.FirstChild);
            else
                locationsElement.InsertAfter(trainLocation.Element, locationsElement.LastChild);

            block.AddTrain(trainLocation);

            if (eventName != null)
                EventManager.Event(new LayoutEvent(eventName, this, block));

            Redraw();

            return trainLocation;
        }

        /// <summary>
        /// This method is called when the train leaves a block
        /// </summary>
        /// <param name="trainLocation">The train locoation info object that encupsolte the block</param>
        [LayoutEventDef("train-leave-block", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(LayoutBlock))]
        public void LeaveBlock(LayoutBlock block, bool generateEvent) {
            var trainLocation = LocationOfBlock(block);

            if (trainLocation == null)
                throw new ArgumentException("Train is not in the given block", nameof(block));

            block.RemoveTrain(trainLocation);
            LocationsElement.RemoveChild(trainLocation.Element);
            if (generateEvent)
                EventManager.Event(new LayoutEvent("train-leave-block", this, block));
            Redraw();
        }

        public void LeaveBlock(LayoutBlock block) {
            LeaveBlock(block, true);
        }

        public override void Redraw() {
            foreach (TrainLocationInfo trainLocation in Locations) {
                if (trainLocation.Block.BlockDefinintion != null)
                    trainLocation.Block.BlockDefinintion.Redraw();
            }
        }

        public override void EraseImage() {
            foreach (TrainLocationInfo trainLocation in Locations) {
                if (trainLocation.Block.BlockDefinintion != null)
                    trainLocation.Block.BlockDefinintion.EraseImage();
            }
        }

        #endregion

        #region Value settings

        /// <summary>
        /// Change the train state speed and direction. This method should not be called directly. It is
        /// called by the locomotive manager event handler for speed and direction related events
        /// </summary>
        /// <param name="speedInSteps"></param>
        /// <param name="direction"></param>
        [LayoutEventDef("train-speed-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(int))]
        public void SetSpeedValue(int speedInSteps) {
            if (speedInSteps != this.SpeedInSteps) {
                SetAttribute(A_Speed, speedInSteps);
                EventManager.Event(new LayoutEvent("train-speed-changed", this, speedInSteps));
            }
        }

        /// <summary>
        /// Change the train state lights on/off setting. This method should not be called directly. It is
        /// called by the locomotive manager event manager for light state related events.
        /// </summary>
        /// <param name="lights"></param>
        [LayoutEventDef("train-lights-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(bool))]
        public void SetLightsValue(bool lights) {
            if (lights != Lights) {
                SetAttribute(A_Lights, lights);
                EventManager.Event(new LayoutEvent("train-lights-changed", this, lights));
            }
        }

        private const string A_Name = "Name";
        private const string A_LocomotiveId = "LocomotiveID";

        #endregion

        #region Locomotive special function management

        /// <summary>
        /// Find the current state for a given function for a given locomotive.
        /// </summary>
        /// <param name="functionName">The function name</param>
        /// <param name="locoIndex">The locomotive index</param>
        /// <param name="create">If true, a new state element will be created</param>
        /// <returns></returns>
        protected LocomotiveFunctionInfo? FindFunctionState(String functionName, Guid locomotiveId, bool create) {
            XmlElement resultElement;

            if (locomotiveId == Guid.Empty)
                resultElement = (XmlElement)FunctionStatesElement.SelectSingleNode("FunctionState[@Name='" + functionName + "']");
            else
                resultElement = (XmlElement)FunctionStatesElement.SelectSingleNode("FunctionState[@Name='" + functionName + "' and @LocomotiveID='" + locomotiveId + "']");

            if (resultElement == null && create) {
                resultElement = FunctionStatesElement.OwnerDocument.CreateElement("FunctionState");

                resultElement.SetAttribute(A_Name, functionName);
                if (locomotiveId != Guid.Empty)
                    resultElement.SetAttribute(A_LocomotiveId, locomotiveId);

                FunctionStatesElement.AppendChild(resultElement);
            }

            if (resultElement != null)
                return new LocomotiveFunctionInfo(resultElement);
            else
                return null;
        }

        protected LocomotiveFunctionInfo? FindFunctionState(int functionNumber, Guid locomotiveId) {
            XmlElement resultElement = locomotiveId == Guid.Empty
                ? (XmlElement)FunctionStatesElement.SelectSingleNode($"FunctionState[@Number='{functionNumber}']")
                : (XmlElement)FunctionStatesElement.SelectSingleNode($"FunctionState[@Number='{functionNumber}' and @LocomotiveID='{locomotiveId}']");
            return resultElement != null ? new LocomotiveFunctionInfo(resultElement) : null;
        }

        private const string E_FunctionStates = "FunctionStates";

        public XmlElement FunctionStatesElement {
            get {
                XmlElement functionStates = Element[E_FunctionStates];

                if (functionStates == null) {
                    functionStates = Element.OwnerDocument.CreateElement(E_FunctionStates);

                    Element.AppendChild(functionStates);
                }
                return functionStates;
            }
        }

        private string getFunctionXml(String functionName, Guid locomotiveId) {
            if (locomotiveId != Guid.Empty)
                return $"<Function Name='{functionName}' LocomotiveID='{locomotiveId}' />";
            else
                return $"<Function Name='{functionName}' />";
        }

        [LayoutEventDef("locomotive-function-state-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(int))]
        public void SetLocomotiveFunctionStateValue(String functionName, Guid locomotiveId, bool state) {
            var function = FindFunctionState(functionName, locomotiveId, true);

            if (function != null) {
                function.State = state;
                EventManager.Event(new LayoutEvent("locomotive-function-state-changed", this,
                    state, getFunctionXml(functionName, locomotiveId)));
            }
        }

        [LayoutEventDef("locomotive-function-state-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(int))]
        public void SetLocomotiveFunctionStateValue(int functionNumber, Guid locomotiveId, bool state) {
            var function = FindFunctionState(functionNumber, locomotiveId);

            if (function != null) {
                function.State = state;

                EventManager.Event(new LayoutEvent("locomotive-function-state-changed", this,
                    state, getFunctionXml(function.Name, locomotiveId)));
            }
        }

        /// <summary>
        /// Set the state of a given locomotive function
        /// </summary>
        /// <param name="functionName">The function name</param>
        /// <param name="locoIndex">which locomotive (relevant if locomotive set), -1 set the state for all locomotives in the set</param>
        /// <param name="state">The state (true = on, false = off)</param>
        public void SetLocomotiveFunctionState(String functionName, Guid locomotiveId, bool state) {
            EventManager.Event(new LayoutEvent("set-locomotive-function-state-request", this,
                state, getFunctionXml(functionName, locomotiveId)));
        }

        /// <summary>
        /// Trigger a given locomotive function
        /// </summary>
        /// <param name="functionName">The function name</param>
        /// <param name="locoIndex">which locomotive (relevant if locomotive set), -1 set the state for all locomotives in the set</param>
        public void TriggerLocomotiveFunction(String functionName, Guid locomotiveId) {
            EventManager.Event(new LayoutEvent("trigger-locomotive-function-request", sender: this,
                xmlDocument: getFunctionXml(functionName, locomotiveId)));
        }

        private const string A_State = "State";

        /// <summary>
        /// Get the state of a given locomotive function
        /// </summary>
        /// <param name="functionName">The function name</param>
        /// <param name="locoIndex">which locomotive (relevant if locomotive set)</param>
        /// <param name="defaultValue">Value to return if no state was defined</param>
        /// <returns>True = state is ON, false = state is OFF</returns>
        public bool GetFunctionState(String functionName, Guid locomotiveId, bool defaultValue = false) {
            var function = FindFunctionState(functionName, locomotiveId, false);

            if (function == null || !function.Element.HasAttribute(A_State))
                return defaultValue;
            else
                return function.State;
        }

        /// <summary>
        /// Get the state of a given locomotive function
        /// </summary>
        /// <param name="functionName">The function number</param>
        /// <param name="locoIndex">which locomotive (relevant if locomotive set)</param>
        /// <param name="defaultValue">Value to return if no state was defined</param>
        /// <returns>True = state is ON, false = state is OFF</returns>
        public bool GetFunctionState(int functionNumber, Guid locomotiveId, bool defaultValue = false) {
            var function = FindFunctionState(functionNumber, locomotiveId);

            if (function == null || !function.Element.HasAttribute(A_State))
                return defaultValue;
            else
                return function.State;
        }

        /// <summary>
        /// Return true if locomotive has any active function
        /// </summary>
        public bool HasActiveFunction(Guid locomotiveId) {
            XmlNodeList elements;

            if (locomotiveId == Guid.Empty)
                elements = FunctionStatesElement.ChildNodes;
            else
                elements = FunctionStatesElement.SelectNodes($"FunctionState[@LocomotiveID='{locomotiveId}']");

            foreach (XmlElement functionStateElement in elements) {
                if((bool?)functionStateElement.AttributeValue(A_State) ?? false)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Return true if locomotive has any active function
        /// </summary>
        public bool HasActiveFunction(TrainLocomotiveInfo trainLoco) => HasActiveFunction(trainLoco.LocomotiveId);

        /// <summary>
        /// Return true if locomotive has any active function
        /// </summary>
        public bool HasActiveFunction(LocomotiveInfo locomotive) => HasActiveFunction(locomotive.Id);

        /// <summary>
        /// return true if train has any active function
        /// </summary>
        public bool HasActiveFunction() => HasActiveFunction(Guid.Empty);

        #endregion

        #region Speed Limit & Speed

        [LayoutEventDef("train-speed-limit-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(int))]
        [LayoutEventDef("train-slow-down-speed-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo), InfoType = typeof(int))]
        public void RequestSpeedLimit(int requestedSpeedLimit) {
            if (requestedSpeedLimit < 0)
                requestedSpeedLimit = RequestedSpeedLimit;
            else
                RequestedSpeedLimit = requestedSpeedLimit;

            int newCurrentSpeedLimit = LayoutModel.StateManager.DefaultDriverParameters.SpeedLimit;
            int locomotiveSpeedLimit = LocomotivesSpeedLimit;

            if (locomotiveSpeedLimit > 0 && (newCurrentSpeedLimit == 0 || locomotiveSpeedLimit < newCurrentSpeedLimit))
                newCurrentSpeedLimit = locomotiveSpeedLimit;

            int trainSpeedLimit = SpeedLimit;

            if (trainSpeedLimit > 0 && (newCurrentSpeedLimit == 0 || trainSpeedLimit < newCurrentSpeedLimit))
                newCurrentSpeedLimit = trainSpeedLimit;

            if (requestedSpeedLimit > 0 && (newCurrentSpeedLimit == 0 || requestedSpeedLimit < newCurrentSpeedLimit))
                newCurrentSpeedLimit = requestedSpeedLimit;

            foreach (TrainLocationInfo trainLocation in Locations) {
                if (trainLocation.Block.BlockDefinintion != null) {
                    int blockSpeedLimit = trainLocation.Block.BlockDefinintion.Info.SpeedLimit;

                    if (blockSpeedLimit != 0 && (newCurrentSpeedLimit == 0 || blockSpeedLimit < newCurrentSpeedLimit))
                        newCurrentSpeedLimit = blockSpeedLimit;
                }
            }

            if (newCurrentSpeedLimit != CurrentSpeedLimit) {
                CurrentSpeedLimit = newCurrentSpeedLimit;
                EventManager.Event(new LayoutEvent("train-speed-limit-changed", this, newCurrentSpeedLimit));
            }

            // Calculate slow down speed
            int newCurrentSlowDownSpeed = LayoutModel.StateManager.DefaultDriverParameters.SlowdownSpeed;
            int trainSlowDownSpeed = SlowdownSpeed;

            if (trainSlowDownSpeed > 0 && (newCurrentSlowDownSpeed == 0 || trainSlowDownSpeed < CurrentSpeedLimit))
                newCurrentSlowDownSpeed = trainSlowDownSpeed;

            foreach (TrainLocationInfo trainLocation in Locations) {
                if (trainLocation.Block.BlockDefinintion != null) {
                    int blockSlowDownSpeed = trainLocation.Block.BlockDefinintion.Info.SlowdownSpeed;

                    if (blockSlowDownSpeed != 0 && (newCurrentSlowDownSpeed == 0 || blockSlowDownSpeed < CurrentSpeedLimit))
                        newCurrentSlowDownSpeed = blockSlowDownSpeed;
                }
            }

            if (newCurrentSlowDownSpeed != CurrentSlowdownSpeed) {
                CurrentSlowdownSpeed = newCurrentSlowDownSpeed;
                EventManager.Event(new LayoutEvent("train-slow-down-speed-changed", this, newCurrentSlowDownSpeed));
            }
        }

        public override void RefreshSpeedLimit() {
            RequestSpeedLimit(-1);
        }

        public void ChangeSpeed(int requestedSpeed, MotionRampInfo ramp) {
            EventManager.Event(new LayoutEvent("train-speed-change-request", this, new TrainSpeedChangeParameters(requestedSpeed, ramp),
                null));
        }

        private const string A_LastCrossedBlockEdge = "LastCrossedBlockEdge";
        private const string A_LastBlockEdgeCrossingTime = "LastBlockEdgeCrossingTime";
        private const string A_LastBlockEdgeCrossingSpeed = "LastBlockEdgeCrossingSpeed";
        private const string A_StoppedOnBlockCrossing = "StoppedOnBlockCrossing";

        #endregion

        #endregion

        public LayoutBlockEdgeBase? LastCrossedBlockEdge {
            get {
                if (Element.HasAttribute(A_LastCrossedBlockEdge)) {
                    var blockEdgeId = (Guid)AttributeValue(A_LastCrossedBlockEdge);

                    return LayoutModel.Component<LayoutBlockEdgeBase>(blockEdgeId, LayoutModel.ActivePhases);
                }
                return null;
            }

            set {
                if (value == null) {
                    Element.RemoveAttribute(A_LastCrossedBlockEdge);
                    Element.RemoveAttribute(A_LastBlockEdgeCrossingTime);
                }
                else {
                    SetAttribute(A_LastCrossedBlockEdge, value.Id);
                    SetAttribute(A_LastBlockEdgeCrossingTime, DateTime.Now.Ticks);
                }
            }
        }

        public Int64 LastBlockEdgeCrossingTime {
            get => (Int64?)Element.AttributeValue(A_LastBlockEdgeCrossingTime) ?? 0;
        }

        public bool IsLastBlockEdgeCrossingSpeedKnown => Element.HasAttribute(A_LastBlockEdgeCrossingSpeed);

        public int LastBlockEdgeCrossingSpeed {
            get => (int?)AttributeValue(A_LastBlockEdgeCrossingSpeed) ?? 0;

            set => SetAttribute(A_LastBlockEdgeCrossingSpeed, value);
        }

        public bool TrainStoppedOnBlockEdgeCrossing {
            get => (bool?)AttributeValue(A_StoppedOnBlockCrossing) ?? false;

            set => SetAttribute(A_StoppedOnBlockCrossing, value, removeIf: false);
        }

        #region IEqualityComparer<TrainStateInfo> Members

        public bool Equals(TrainStateInfo x, TrainStateInfo y) => x.Id == y.Id;

        public int GetHashCode(TrainStateInfo obj) => obj.Id.GetHashCode();

        #endregion
    }

    /// <summary>
    /// Wrapper for element providing information on train location
    /// </summary>
    public class TrainLocationInfo : LayoutInfo {
        private const string A_BlockId = "BlockID";
        private const string A_BlockEdgeId = "BlockEdgeID";
        private const string A_BlockEdgeCrossingTime = "BlockEdgeCrossingTime";
        private const string E_Locations = "Locations";
        private const string A_DisplayFront = "DisplayFront";

        internal TrainLocationInfo(TrainStateInfo trainState, XmlElement element)
            : base(element) {
            this.Train = trainState;
        }

        public TrainStateInfo Train { get; }

        /// <summary>
        /// The BlockID of the block in which the train is located
        /// </summary>
        public Guid BlockId {
            get => (Guid)AttributeValue(A_BlockId);
            set => SetAttribute(A_BlockId, value);
        }

        /// <summary>
        /// The block in which the train is located
        /// </summary>
        public LayoutBlock Block {
            get {
                if (!LayoutModel.Blocks.TryGetValue(BlockId, out LayoutBlock block))
                    throw new LayoutException($"Location of train {Train.Name} could not found - (track was removed?)");
                return block;
            }

            set => BlockId = value.Id;
        }

        /// <summary>
        /// The ID of the track contact that the train had crossed in order to enter this block
        /// </summary>m 
        public Guid BlockEdgeId {
            get => (Guid?)Element.AttributeValue(A_BlockEdgeId) ?? Guid.Empty;

            set {
                if (value != Guid.Empty) {
                    SetAttribute(A_BlockEdgeId, value);
                    SetAttribute(A_BlockEdgeCrossingTime, $"{DateTime.Now.Ticks}");
                }
                else {
                    Element.RemoveAttribute(A_BlockEdgeId);
                    Element.RemoveAttribute(A_BlockEdgeCrossingTime);
                }
            }
        }

        public LayoutBlockEdgeBase? BlockEdge {
            get => LayoutModel.Component<LayoutBlockEdgeBase>(BlockEdgeId, LayoutModel.ActivePhases);

            set {
                if (value == null)
                    BlockEdgeId = Guid.Empty;
                else
                    BlockEdgeId = value.Id;
            }
        }

        /// <summary>
        /// The time in which the train had crossed the track contact which caused it to enter this block
        /// </summary>
        public long BlockEdgeCrossingTime => (Int64)AttributeValue(A_BlockEdgeCrossingTime) ;

        public XmlElement LocomotiveStateElement => (XmlElement)Element.ParentNode.ParentNode;

        public TrainPart TrainPart {
            get {
                XmlElement locationsElement = (XmlElement)LocomotiveStateElement[E_Locations];

                if (Element == locationsElement.FirstChild)
                    return TrainPart.Locomotive;
                else if (Element == locationsElement.LastChild)
                    return TrainPart.LastCar;
                else {
                    foreach (XmlElement locationElement in locationsElement)
                        if (locationElement == Element)
                            return TrainPart.Car;
                    return TrainPart.None;
                }
            }
        }

        public bool IsDisplayFrontKnown => Element.HasAttribute(A_DisplayFront);

        public void SetDisplayFrontToUnknown() {
            Element.RemoveAttribute(A_DisplayFront);
        }

        public LayoutComponentConnectionPoint DisplayFront {
            get => LayoutComponentConnectionPoint.Parse(GetAttribute(A_DisplayFront));

            set => SetAttribute(A_DisplayFront, value);
        }
    }

    public class TrainsStateInfo : LayoutStateInfoBase, IEnumerable<TrainStateInfo> {
        private const string E_Locomotive = "Locomotive";
        private const string E_Train = "Train";
        private const string A_Id = "ID";

        internal TrainsStateInfo(XmlElement trainsStateElement)
            : base(trainsStateElement) {
        }

        #region Accessors

        /// <summary>
        /// Return a list of all locomotive state that are on a given block
        /// </summary>
        public IList<TrainStateInfo> this[LayoutBlock block] {
            get {
                List<TrainStateInfo> result;
                XmlNodeList trainLocationList = Element.SelectNodes("TrainState/Locations/Block[@BlockID='" + block.Id.ToString() + "']");

                result = new List<TrainStateInfo>(trainLocationList.Count);

                foreach (XmlElement trainLocationElement in trainLocationList) {
                    var trainStateElement = (XmlElement)trainLocationElement.ParentNode.ParentNode;
                    var train = new TrainStateInfo(trainStateElement);

                    if (train.OnTrack)
                        result.Add(new TrainStateInfo(trainStateElement));
                }

                return result.AsReadOnly();
            }
        }

        /// <summary>
        /// Return a locomotive state object for a locomotive of a given ID.
        /// </summary>
        public TrainStateInfo? this[Guid id] {
            get {
                if (IdToTrainStateElement.TryGetValue(id, out XmlElement trainStateElement))
                    return new TrainStateInfo(trainStateElement);
                return null;
            }
        }

        public TrainStateInfo? this[XmlElement element] {
            get {
                if (element.Name == E_Locomotive) {
                    LocomotiveInfo loco = new LocomotiveInfo(element);

                    return this[loco.Id];
                }
                else if (element.Name == E_Train) {
                    TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(element);

                    return this[trainInCollection.Id];
                }
                else
                    throw new ArgumentException("Invalid XML element (not Locomotive or Train)");
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Remove all state information
        /// </summary>
        public void Clear() {
            Element.RemoveAll();
            IdToTrainStateElement.Clear();
        }

        /// <summary>
        /// Add a new train. The new train state is returned.
        /// </summary>
        /// <returns>The newly added train</returns>
        public TrainStateInfo Add(Guid trainId) {
            XmlElement trainStateElement = Element.OwnerDocument.CreateElement("TrainState");

            if (trainId == Guid.Empty)
                trainId = Guid.NewGuid();

            trainStateElement.SetAttribute(A_Id, trainId);
            Element.AppendChild(trainStateElement);

            TrainStateInfo trainState = new TrainStateInfo(trainStateElement);

            IdToTrainStateElement.Add(trainState.Id, trainState.Element);

            return trainState;
        }

        public TrainStateInfo Add() => Add(Guid.Empty);

        internal Dictionary<Guid, XmlElement> IdToTrainStateElement { get; } = new Dictionary<Guid, XmlElement>();

        public void RemoveTrainState(TrainStateInfo trainState) {
            RemoveTrainFromTrack(trainState);

            foreach (TrainLocomotiveInfo trainLoco in trainState.Locomotives)
                trainState.RemoveLocomotive(trainLoco);

            Element.RemoveChild(trainState.Element);
            IdToTrainStateElement.Remove(trainState.Id);
        }

        public void RemoveTrainFromTrack(TrainStateInfo trainState) {
            foreach (TrainLocationInfo trainLocation in trainState.Locations)
                trainState.LeaveBlock(trainLocation.Block, generateEvent: false);

            LayoutModel.StateManager.Components.RemoveForTrain(trainState.Id, "TrainPassing");
        }

        [LayoutEventDef("train-removed", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
        public void Remove(TrainStateInfo trainState) {
            EventManager.Event(new LayoutEvent<TrainStateInfo>("train-is-removed", trainState));
            RemoveTrainState(trainState);
            EventManager.Event(new LayoutEvent("train-removed", trainState));
        }

        /// <summary>
        /// Rebuild the ID to train state map. This method is called after loading the layout state from persistent store
        /// </summary>
        public void RebuildIdMap() {
            IdToTrainStateElement.Clear();

            foreach (XmlElement trainStateElement in Element) {
                TrainStateInfo trainState = new TrainStateInfo(trainStateElement);

                IdToTrainStateElement.Add(trainState.Id, trainStateElement);

                foreach (TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
                    IdToTrainStateElement.Add(trainLoco.Locomotive.Id, trainStateElement);

                    if (!IdToTrainStateElement.ContainsKey(trainLoco.CollectionElementId))
                        IdToTrainStateElement.Add(trainLoco.CollectionElementId, trainStateElement);
                }
            }
        }

        #endregion

        #region IEnumerable<TrainStateInfo> Members

        public IEnumerator<TrainStateInfo> GetEnumerator() {
            foreach (XmlElement trainStateElement in Element)
                yield return new TrainStateInfo(trainStateElement);
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion
    }

    #endregion

    #region Components State

    public class ComponentsStateInfo : LayoutStateInfoBase {
        private const string E_ComponentState = "ComponentState";
        private const string A_Id = "ID";
        private readonly Dictionary<Guid, XmlElement> _idToComponentStateElement = new Dictionary<Guid, XmlElement>();

        public ComponentsStateInfo(XmlElement element)
            : base(element) {
        }

        #region Accessors

        /// <summary>
        /// Get complete runtime state element for a given model component
        /// </summary>
        /// <param name="componentId">The component ID</param>
        /// <returns>The component runtime state element</returns>
        private XmlElement GetComponentStateElement(Guid componentId) {
            if (!_idToComponentStateElement.TryGetValue(componentId, out XmlElement componentStateElement)) {
                componentStateElement = Element.OwnerDocument.CreateElement(E_ComponentState);

                componentStateElement.SetAttribute(A_Id, componentId);
                Element.AppendChild(componentStateElement);

                _idToComponentStateElement.Add(componentId, componentStateElement);
            }

            return componentStateElement;
        }

        /// <summary>
        /// Get the runtime state element for a component and topic
        /// </summary>
        /// <param name="componentId">The component name</param>
        /// <param name="topicName">The topic name</param>
        /// <returns>The component topic runtime state XML element</returns>
        public XmlElementWrapper StateOf(Guid componentId, string topicName, bool create = false) {
            XmlElement componentStateElement = GetComponentStateElement(componentId);
            XmlElement componentStateTopicElement = componentStateElement[topicName];

            if (create && componentStateTopicElement == null) {
                componentStateTopicElement = Element.OwnerDocument.CreateElement(topicName);

                componentStateElement.AppendChild(componentStateTopicElement);
            }

            return new XmlElementWrapper(componentStateTopicElement);
        }

        public XmlElementWrapper StateOf(IModelComponentHasId component, string topicName, bool create = false) => StateOf(component.Id, topicName, create);

        #endregion

        #region Properties

        public IDictionary<Guid, XmlElement> IdToComponentStateElement => _idToComponentStateElement;

        #endregion

        #region Operations

        public bool Contains(Guid componentId) => _idToComponentStateElement.ContainsKey(componentId);

        public bool Contains(ModelComponent component) => Contains(component.Id);

        public bool Contains(Guid componentId, string topicName) {
            XmlElement stateElement = GetComponentStateElement(componentId);

            if (stateElement != null)
                return stateElement[topicName] != null;
            return false;
        }

        public bool Contains(ModelComponent component, string topicName) => Contains(component.Id, topicName);

        public void Remove(Guid componentId) {
            if (Contains(componentId)) {
                XmlElement componentStateElement = GetComponentStateElement(componentId);

                Debug.Assert(componentStateElement != null);

                Element.RemoveChild(componentStateElement);
                _idToComponentStateElement.Remove(componentId);
            }
        }

        public void Remove(ModelComponent component) {
            Remove(component.Id);
        }

        public void Remove(Guid componentId, string topicName) {
            if (Contains(componentId, topicName)) {
                XmlElement componentStateElement = GetComponentStateElement(componentId);
                XmlElement componentStateTopicElement = componentStateElement[topicName];

                Debug.Assert(componentStateElement != null);
                Debug.Assert(componentStateTopicElement != null);

                componentStateElement.RemoveChild(componentStateTopicElement);

                if (componentStateElement.ChildNodes.Count == 0)
                    Remove(componentId);
            }
        }

        public void Remove(ModelComponent component, string topicName) {
            Remove(component.Id, topicName);
        }

        /// <summary>
        /// Remove all topics of a given name that relates to a given train.
        /// </summary>
        /// <remarks>This is useful to remove all resources for a particular train when this
        /// train is removed.
        /// </remarks>
        /// <param name="trainID">The train ID</param>
        /// <param name="topicName">The topic name</param>
        public void RemoveForTrain(Guid trainId, string topicName) {
            XmlNodeList elementsToRemove = Element.SelectNodes($"ComponentState/{topicName}[@TrainID='{trainId}']");
            List<Guid> removeList = new List<Guid>();

            foreach (XmlElement componentStateTopicElement in elementsToRemove) {
                var componentStateElement = (XmlElement)componentStateTopicElement.ParentNode;
                var componentID = (Guid)componentStateElement.AttributeValue(A_Id);

                removeList.Add(componentID);
            }

            foreach (Guid componentId in removeList)
                Remove(componentId, topicName);
        }

        public void Clear() {
            _idToComponentStateElement.Clear();
            Element.RemoveAll();
        }

        /// <summary>
        /// Remove state of components with no topics
        /// </summary>
        public void RemoveEmpty() {
            var removeList = (from XmlElement e in this.Element where e.ChildNodes.Count == 0 select e).ToList();

            foreach(var componentStateElement in removeList)
                Remove((Guid)componentStateElement.AttributeValue(A_Id));
        }

        #endregion
    }

    #endregion

    #region Track Contact State

    public class TrackContactPassingStateInfo : LayoutStateTopicBase {
        private const string A_TrainId = "TrainID";
        private const string A_FromBlockID = "FromBlockID";
        private const string A_FromBlockTriggerCount = "FromBlockTriggerCount";
        private const string A_ToBlockID = "ToBlockID";
        private const string A_ToBlockTriggerCount = "ToBlockTriggerCount";
        private const string A_Direction = "Direction";

        public TrackContactPassingStateInfo(XmlElement element)
            : base(element) {
        }

        public Guid TrackContactId => ComponentId;

        public LayoutTrackContactComponent TrackContact => Ensure.NotNull<LayoutTrackContactComponent>(LayoutModel.Component<LayoutTrackContactComponent>(ComponentId, LayoutModel.ActivePhases), "TrackContact");

        public Guid TrainId {
            get => (Guid)AttributeValue(A_TrainId);
            set => SetAttribute(A_TrainId, value);
        }

        public TrainStateInfo? Train {
            get => LayoutModel.StateManager.Trains[TrainId];

            set {
                Debug.Assert(value != null);
                TrainId = value.Id;
            }
        }

        public Guid FromBlockId {
            get => (Guid)AttributeValue(A_FromBlockID);
            set => SetAttribute(A_FromBlockID, value);
        }

        public LayoutBlock FromBlock {
            get => LayoutModel.Blocks[FromBlockId];
            set => FromBlockId = value.Id;
        }

        public int FromBlockTriggerCount {
            get => (int)AttributeValue(A_FromBlockTriggerCount);
            set => SetAttribute(A_FromBlockTriggerCount, value);
        }

        public Guid ToBlockId {
            get => (Guid)AttributeValue(A_ToBlockID);
            set => SetAttribute(A_ToBlockID, value);
        }

        public LayoutBlock ToBlock {
            get => LayoutModel.Blocks[ToBlockId];

            set => ToBlockId = value.Id;
        }

        public int ToBlockTriggerCount {
            get => (int)AttributeValue(A_ToBlockTriggerCount);
            set => SetAttribute(A_ToBlockTriggerCount, value);
        }

        public LocomotiveOrientation Direction {
            get => AttributeValue(A_Direction).Enum<LocomotiveOrientation>() ?? throw new ArgumentNullException(A_Direction);
            set => SetAttribute(A_Direction, value);
        }

        public void Remove() {
            LayoutModel.StateManager.Components.Remove(ComponentId, "TrainPassing");
        }
    }

    #endregion

    #region Manual Dispatch Region States

    #region Block ID collection

    public class BlockIdCollection : XmlCollection<Guid>, ICollection<Guid> {
        private const string E_Block = "Block";
        private const string A_BlockId = "BlockID";

        internal BlockIdCollection(ManualDispatchRegionInfo manualDispatchRegion)
            : base(manualDispatchRegion.BlocksElement) {
        }

        protected override XmlElement CreateElement(Guid item) {
            XmlElement itemElement = Element.OwnerDocument.CreateElement(E_Block);

            itemElement.SetAttribute(A_BlockId, item);
            return itemElement;
        }

        protected override Guid FromElement(XmlElement itemElement) => (Guid)itemElement.AttributeValue(A_BlockId);

        public override void Add(Guid blockId) {
            if (Contains(blockId))
                throw new LayoutException(blockId, "This block is already in the manual dispatch region");

            base.Add(blockId);
        }
    }

    #endregion

    public class ManualDispatchRegionInfo : LayoutStateInfoBase, IObjectHasId {
        private const string E_Blocks = "Blocks";
        private const string A_Active = "Active";
        private const string A_Name = "Name";
        private BlockIdCollection? blockIdCollection;

        public ManualDispatchRegionInfo(XmlElement element)
            : base(element) {
        }

        public XmlElement BlocksElement {
            get {
                XmlElement blocksElement = Element[E_Blocks];

                if (blocksElement == null) {
                    blocksElement = Element.OwnerDocument.CreateElement(E_Blocks);
                    Element.AppendChild(blocksElement);
                }

                return blocksElement;
            }
        }

        public string Name {
            get => GetOptionalAttribute(A_Name) ?? "";
            set => SetAttribute(A_Name, value);
        }

        public bool Active {
            get => (bool?)AttributeValue(A_Active) ?? false;

            set {
                if (value && Active == false)
                    EventManager.Event(new LayoutEvent("request-manual-dispatch-lock", this));
                else if (value == false && Active) {
                    EventManager.Event(new LayoutEvent("free-manual-dispatch-lock", this, false));
                    SetActive(false);
                }
            }
        }

        public string NameAndStatus {
            get {
                if (Active)
                    return Name + " (Active)";
                else
                    return Name;
            }
        }

        /// <summary>
        /// Set whether this manual dispatch region is active or not. Should not be called directly
        /// </summary>
        /// <param name="active"></param>
        public void SetActive(bool active) {
            SetAttribute(A_Active, active);
            EventManager.Event(new LayoutEvent("manual-dispatch-region-activation-changed", this, active));
        }

        public BlockIdCollection BlockIdList {
            get {
                return blockIdCollection ?? (blockIdCollection = new BlockIdCollection(this));
            }
        }

        public LayoutSelection Selection {
            get {
                LayoutSelection selection = new LayoutSelection();

                foreach (Guid blockID in BlockIdList) {
                    LayoutBlock block = LayoutModel.Blocks[blockID];

                    selection.Add(block.GetSelection());
                }

                return selection;
            }
        }

        public bool CheckIntegrity(LayoutModuleBase mb) {
            List<Guid> removeList = new List<Guid>();

            foreach (Guid blockID in BlockIdList) {
                if (LayoutModel.Blocks[blockID] == null) {
                    LayoutModuleBase.Warning("Block in manual dispatch region " + Name + " not found - removed from region");
                    removeList.Add(blockID);
                }
            }

            foreach (Guid blockID in removeList)
                BlockIdList.Remove(blockID);

            return removeList.Count == 0;
        }
    }

    public class AllLayoutManualDispatchRegion : ManualDispatchRegionInfo {
        public AllLayoutManualDispatchRegion() : base(CreateElement()) {
            foreach (LayoutBlock block in LayoutModel.Blocks)
                BlockIdList.Add(block.Id);
        }

        private static XmlElement CreateElement() {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            var element = doc.CreateElement("ManualDispatchRegion");
            doc.AppendChild(element);

            return element;
        }
    }

    public class ManualDispatchRegionCollection : XmlIndexedCollection<ManualDispatchRegionInfo, Guid> {
        public ManualDispatchRegionCollection(XmlElement manualDispatchRegionsElement)
            : base(manualDispatchRegionsElement) {
        }

        protected override Guid GetItemKey(ManualDispatchRegionInfo item) => item.Id;

        protected override ManualDispatchRegionInfo FromElement(XmlElement itemElement) => new ManualDispatchRegionInfo(itemElement);

        public ManualDispatchRegionInfo Create() {
            XmlElement manualDispatchRegionElement = Element.OwnerDocument.CreateElement("ManualDispatchRegion");
            ManualDispatchRegionInfo createdManualDispatchRegion = new ManualDispatchRegionInfo(manualDispatchRegionElement);

            Add(createdManualDispatchRegion);
            return createdManualDispatchRegion;
        }

        public ManualDispatchRegionInfo Create(string name) {
            ManualDispatchRegionInfo createdManualDispatchRegion = Create();

            createdManualDispatchRegion.Name = name;
            return createdManualDispatchRegion;
        }

        public bool CheckIntegrity(LayoutModuleBase moduleBase) {
            bool ok = true;
            List<ManualDispatchRegionInfo> removeList = new List<ManualDispatchRegionInfo>();

            foreach (ManualDispatchRegionInfo manualDispatchRegion in this) {
                if (!manualDispatchRegion.CheckIntegrity(moduleBase)) {
                    ok = false;
                    if (manualDispatchRegion.BlockIdList.Count == 0) {
                        LayoutModuleBase.Warning("Manual dispatch region " + manualDispatchRegion.Name + " became empty - removing region");
                        removeList.Add(manualDispatchRegion);
                    }
                }
            }

            foreach (ManualDispatchRegionInfo manualDispatchRegion in removeList)
                Remove(manualDispatchRegion);

            return ok;
        }
    }

    #endregion

    #region Layout Verification options

    public class LayoutVerificationOptions : LayoutStateInfoBase {
        private const string A_IgnoreNotConnectedFeedbacks = "IgnoreNotConnectedFeedbacks";
        private const string A_IgnoreNotPowerComponents = "IgnoreNotPowerComponents";

        public LayoutVerificationOptions(XmlElement element)
            : base(element) {
        }

        /// <summary>
        /// Ignore feedback (track contacts, block occupancy detectors) components that are not connected
        /// </summary>
        public bool IgnoreNotConnectedFeedbacks {
            get => (bool?)AttributeValue(A_IgnoreNotConnectedFeedbacks) ?? false;

            set => SetAttribute(A_IgnoreNotConnectedFeedbacks, value);
        }

        /// <summary>
        /// Ignore not connected power components
        /// </summary>
        public bool IgnoreNotConnectedPowerComponents {
            get => (bool?)AttributeValue(A_IgnoreNotPowerComponents) ?? false;

            set => SetAttribute(A_IgnoreNotPowerComponents, value);
        }
    }

    #endregion

    public enum TrainTrackingInManualDispatchRegion {
        None,
        Normal,
    }

    public class TrainTrackingOptions : LayoutStateInfoBase {
        private const string A_InManualDispatchRegion = "InManualDispatchRegion";
        private const string A_ProximitySensorSensitivityDelay = "ProximitySensorSensitivity";
        private const int ProxmitySensorSensitivityDefault = 400;

        public TrainTrackingOptions(XmlElement element) : base(element) {
        }

        public TrainTrackingInManualDispatchRegion TrainTrackingInManualDispatchRegion {
            get => AttributeValue(A_InManualDispatchRegion).Enum<TrainTrackingInManualDispatchRegion>() ?? TrainTrackingInManualDispatchRegion.Normal;
            set => SetAttribute(A_InManualDispatchRegion, value);
        }

        /// <summary>
        /// Time (in milliseconds) in which proxmity sensor need to remain in the same state for the state to acknoledge
        /// </summary>
        public int ProximitySensorSensitivityDelay {
            get => (int?)AttributeValue(A_ProximitySensorSensitivityDelay) ?? ProxmitySensorSensitivityDefault;
            set {
                SetAttribute(A_ProximitySensorSensitivityDelay, value, removeIf: ProxmitySensorSensitivityDefault);
                EventManager.Event("proximity-sensitivity-delay-changed");
            }
        }
    }

    #region Train tracking options
    #endregion

    #region Policy

    public class LayoutPolicyInfo : LayoutXmlWrapper, IObjectHasId {
        private const string E_Policy = "Policy";
        private const string E_EventScript = "EventScript";
        private const string A_Global = "Global";
        private const string A_Name = "Name";
        private const string A_Scope = "Scope";
        private const string A_ShowInMenu = "ShowInMenu";
        private const string A_Apply = "Apply";

        public LayoutPolicyInfo(XmlElement element)
            : base(element) {
        }

        public LayoutPolicyInfo(XmlElement parent, string name, XmlElement eventScriptElement, string scope, bool globalPolicy, bool apply) {
            Element = parent.OwnerDocument.CreateElement(E_Policy);

            if (name != null)
                Name = name;

            if (eventScriptElement != null)
                EventScriptElement = eventScriptElement;

            Scope = scope;
            Apply = apply;

            SetAttribute(A_Global, globalPolicy);
        }

        public LayoutEventScript? ActiveScript => (LayoutEventScript?)EventManager.Event(new LayoutEvent("get-active-event-script", Id));

        public bool IsActive => ActiveScript != null;

        public string Name {
            get => GetAttribute(A_Name);

            set => SetAttribute(A_Name, value);
        }

        public string Scope {
            get => GetOptionalAttribute(A_Scope) ?? "TripPlan";

            set => SetAttribute(A_Scope, value);
        }

        /// <summary>
        /// return whether this policy is global (available in all layouts) or not-global (available only in the
        /// current layout)
        /// </summary>
        public bool GlobalPolicy {
            get => (bool?)AttributeValue(A_Global) ?? false;

            set => SetAttribute(A_Global, value);
        }

        public bool ShowInMenu {
            get => (bool?)AttributeValue(A_ShowInMenu) ?? false;

            set => SetAttribute(A_ShowInMenu, value);
        }

        public XmlElement? EventScriptElement {
            get {
                XmlElement eventScriptElement = Element[E_EventScript];

                if (eventScriptElement != null) {
                    Debug.Assert(eventScriptElement.ChildNodes.Count == 1);

                    return (XmlElement)eventScriptElement.ChildNodes[0];
                }

                return null;
            }

            set {
                Debug.Assert(value != null);

                Ensure.NotNull<XmlElement>(value, "EventScriptElement");

                XmlElement eventScriptElement = Element[E_EventScript];
                XmlElement theScriptElement;

                if (eventScriptElement == null) {
                    eventScriptElement = Element.OwnerDocument.CreateElement(E_EventScript);
                    Element.AppendChild(eventScriptElement);
                }

                if (value.OwnerDocument != eventScriptElement.OwnerDocument)
                    theScriptElement = (XmlElement)eventScriptElement.OwnerDocument.ImportNode(value, true);
                else
                    theScriptElement = value;

                eventScriptElement.RemoveAll();
                eventScriptElement.AppendChild(theScriptElement);
            }
        }

        /// <summary>
        /// Get text representation for the policy
        /// </summary>
        public string Text {
            get {
                if (EventScriptElement == null)
                    return "(Policy not set)";
                return (string)EventManager.Event(new LayoutEvent("get-event-script-description", EventScriptElement))!;
            }
        }

        public bool Apply {
            get => (bool?)AttributeValue(A_Apply) ?? false;
            set => SetAttribute(A_Apply, value);
        }
    }

    public class LayoutPoliciesCollection : ICollection<LayoutPolicyInfo> {
        private readonly List<LayoutPolicyInfo> policies = new List<LayoutPolicyInfo>();
        private readonly XmlElement globalPoliciesElement;
        private readonly XmlElement? policiesElement;

        public LayoutPoliciesCollection(XmlElement globalPoliciesElement, XmlElement? policiesElement, string? scope) {
            this.globalPoliciesElement = globalPoliciesElement;
            this.policiesElement = policiesElement;

            string filter;

            if (scope != null)
                filter = "Policy[@Scope='" + scope + "']";
            else
                filter = "*";

            if (globalPoliciesElement != null) {
                foreach (XmlElement policyElement in globalPoliciesElement.SelectNodes(filter)) {
                    LayoutPolicyInfo policy = new LayoutPolicyInfo(policyElement) {
                        GlobalPolicy = true
                    };
                    policies.Add(policy);
                }
            }

            if (policiesElement != null) {
                foreach (XmlElement policyElement in policiesElement.SelectNodes(filter)) {
                    LayoutPolicyInfo policy = new LayoutPolicyInfo(policyElement) {
                        GlobalPolicy = false
                    };
                    policies.Add(policy);
                }
            }
        }

        public void Update(LayoutPolicyInfo policy) {
            Remove(policy);

            if (policy.GlobalPolicy && policy.Element.OwnerDocument != globalPoliciesElement.OwnerDocument)
                policy.Element = (XmlElement)globalPoliciesElement.OwnerDocument.ImportNode(policy.Element, true);
            else if (!policy.GlobalPolicy && policiesElement != null && policy.Element.OwnerDocument != policiesElement.OwnerDocument)
                policy.Element = (XmlElement)policiesElement.OwnerDocument.ImportNode(policy.Element, true);

            Add(policy);

            if (policy.Element.OwnerDocument == globalPoliciesElement.OwnerDocument)
                LayoutModel.WriteModelXmlInfo();

            EventManager.Event(new LayoutEvent("policy-added-to-policies-collection", this, policy));
        }

        public bool Remove(Guid policyId) {
            var policyToRemove = this[policyId];

            if (policyToRemove != null) {
                bool save = (policyToRemove.Element.OwnerDocument == globalPoliciesElement.OwnerDocument);

                EventManager.Event(new LayoutEvent("policy-removed-from-policies-collection", this, policyToRemove));
                policies.Remove(policyToRemove);

                EventManager.Event(new LayoutEvent("policy-removed", policyToRemove, this));
                policyToRemove.Element.ParentNode.RemoveChild(policyToRemove.Element);

                if (save)
                    LayoutModel.WriteModelXmlInfo();

                return true;
            }
            else
                return false;
        }

        public LayoutPolicyInfo? this[Guid policyID] {
            get {
                foreach (LayoutPolicyInfo policy in policies)
                    if (policy.Id == policyID)
                        return policy;
                return null;
            }
        }

        public LayoutPolicyInfo? this[string name] {
            get {
                foreach (LayoutPolicyInfo policy in policies)
                    if (policy.Name == name)
                        return policy;
                return null;
            }
        }

        public IDictionary<Guid, LayoutPolicyInfo> IdToPolicyMap {
            get {
                IDictionary<Guid, LayoutPolicyInfo> map = new Dictionary<Guid, LayoutPolicyInfo>();

                foreach (LayoutPolicyInfo policy in policies)
                    map.Add(policy.Id, policy);

                return map;
            }
        }

        #region ICollection<LayoutPolicyInfo> Members

        public void Add(LayoutPolicyInfo policy) {
            if (policy.GlobalPolicy)
                globalPoliciesElement.AppendChild(policy.Element);
            else if(policiesElement != null)
                policiesElement.AppendChild(policy.Element);

            policies.Add(policy);
            EventManager.Event(new LayoutEvent("policy-added", policy, this));
        }

        public void Clear() {
            foreach (LayoutPolicyInfo policy in policies) {
                EventManager.Event(new LayoutEvent("policy-deleted", policy, this));
                if (policy.GlobalPolicy)
                    globalPoliciesElement.RemoveChild(policy.Element);
                else if(policiesElement != null)
                    policiesElement.RemoveChild(policy.Element);
            }

            policies.Clear();
        }

        public bool Contains(LayoutPolicyInfo item) => this[item.Id] != null;

        public bool Contains(string name) {
            foreach (LayoutPolicyInfo policy in this)
                if (policy.Name == name)
                    return true;
            return false;
        }

        public void CopyTo(LayoutPolicyInfo[] array, int arrayIndex) {
            foreach (LayoutPolicyInfo policy in policies)
                array[arrayIndex++] = policy;
        }

        public int Count => policies.Count;

        public bool IsReadOnly => false;

        public bool Remove(LayoutPolicyInfo policy) => Remove(policy.Id);

        #endregion

        #region IEnumerable<LayoutPolicyInfo> Members

        public IEnumerator<LayoutPolicyInfo> GetEnumerator() => policies.GetEnumerator();

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    public class LayoutPolicyType {
        public string DisplayName;
        public string ScopeName;
        public LayoutPoliciesCollection Policies;

        public LayoutPolicyType(string n, string s, LayoutPoliciesCollection p) {
            this.DisplayName = n;
            this.ScopeName = s;
            this.Policies = p;
        }
    }

    #endregion

    public class LayoutStateManager : LayoutObject {
        private const string A_AllLayoutManualDispatchRegion = "AllLayoutManualDispatchRegion";
        private TrainsStateInfo? trains;
        private ComponentsStateInfo? componentsState;
        private TripPlanCatalogInfo? tripPlansCatalog;
        private ManualDispatchRegionCollection? manualDispatchRegions;
        private DefaultDriverParametersInfo? defaultDriverParameters;
        private LayoutPoliciesCollection? tripPlanPolicies;
        private LayoutPoliciesCollection? blockInfoPolicies;
        private LayoutPoliciesCollection? layoutPolicies;
        private LayoutPoliciesCollection? rideStartPolicies;
        private LayoutPoliciesCollection? driverInstructionsPolicies;
        private AllLayoutManualDispatchRegion? allLayoutManualDispatch;
        private LayoutVerificationOptions? layoutVerificationOptions;
        private TrainTrackingOptions? trainTrackingOptions;

        private List<LayoutPolicyType>? policyTypes;
        private OperationStates? operationStates;

        public LayoutStateManager() : base("<LayoutState><Trains /><Components /><TrackContacts /><TripPlansCatalog /></LayoutState>") {
            initialize();
        }

        private void initialize() {
            trains = new TrainsStateInfo(getElement("Trains"));
            componentsState = new ComponentsStateInfo(getElement("Components"));
            tripPlansCatalog = new TripPlanCatalogInfo(getElement("TripPlansCatalog"));
            manualDispatchRegions = new ManualDispatchRegionCollection(getElement("ManualDispatchRegions"));
            defaultDriverParameters = new DefaultDriverParametersInfo(getElement("DefaultDriverParameters"));
            layoutVerificationOptions = new LayoutVerificationOptions(getElement("LayoutVerificiationOptions"));
            trainTrackingOptions = new TrainTrackingOptions(getElement("TrainTracking"));

            tripPlanPolicies = new LayoutPoliciesCollection(GlobalPoliciesElement, LayoutPoliciesElement, "TripPlan");
            blockInfoPolicies = new LayoutPoliciesCollection(GlobalPoliciesElement, LayoutPoliciesElement, "BlockInfo");
            layoutPolicies = new LayoutPoliciesCollection(GlobalPoliciesElement, LayoutPoliciesElement, "Global");
            rideStartPolicies = new LayoutPoliciesCollection(GlobalPoliciesElement, LayoutPoliciesElement, "RideStart");
            driverInstructionsPolicies = new LayoutPoliciesCollection(GlobalPoliciesElement, LayoutPoliciesElement, "DriverInstructions");
            operationStates = null;
        }

        private XmlElement getElement(string name) {
            XmlElement element = Element[name];

            if (element == null) {
                element = Element.OwnerDocument.CreateElement(name);
                Element.AppendChild(element);
            }

            return element;
        }

        public XmlElement LayoutPoliciesElement => getElement("Policies");

        public XmlElement GlobalPoliciesElement => LayoutModel.Instance.GlobalPoliciesElement;

        public TrainsStateInfo Trains => trains!;

        public ComponentsStateInfo Components => componentsState!;

        public TripPlanCatalogInfo TripPlansCatalog => tripPlansCatalog!;

        public LayoutVerificationOptions VerificationOptions => layoutVerificationOptions!;

        public ManualDispatchRegionCollection ManualDispatchRegions => manualDispatchRegions!;

        public LayoutPoliciesCollection TripPlanPolicies => tripPlanPolicies!;

        public LayoutPoliciesCollection Policies(string? scope) => new LayoutPoliciesCollection(GlobalPoliciesElement, LayoutPoliciesElement, scope);

        public LayoutPoliciesCollection BlockInfoPolicies => blockInfoPolicies!;

        public LayoutPoliciesCollection LayoutPolicies => layoutPolicies!;

        public LayoutPoliciesCollection RideStartPolicies => rideStartPolicies!;

        public LayoutPoliciesCollection DriverInstructionsPolicies => driverInstructionsPolicies!;

        public DefaultDriverParametersInfo DefaultDriverParameters => defaultDriverParameters!;

        public TrainTrackingOptions TrainTrackingOptions => trainTrackingOptions!;

        /// <summary>
        /// Given an element and a ramp role, return this ramp (if can be found)
        /// </summary>
        /// <param name="element"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static MotionRampInfo? GetRamp(XmlElement element, string role) {
            XmlElement rampElement = (XmlElement)element.SelectSingleNode("Ramp[@Role='" + role + "']");

            if (rampElement == null)
                return null;
            else
                return new MotionRampInfo(rampElement);
        }

        /// <summary>
        /// Get the default ramp for a given role
        /// </summary>
        /// <param name="role">Accelerate, Decelrate, Slowdown, Stop</param>
        /// <returns>The ramp to be used</returns>
        public MotionRampInfo? GetDefaultRamp(string role) => GetRamp(DefaultDriverParameters.Element, role);

        public MotionRampInfo? DefaultAccelerationRamp => GetDefaultRamp("Acceleration");

        public MotionRampInfo? DefaultDecelerationRamp => GetDefaultRamp("Deceleration");

        public MotionRampInfo? DefaultSlowdownRamp => GetDefaultRamp("SlowDown");

        public MotionRampInfo? DefaultStopRamp => GetDefaultRamp("Stop");

        public List<LayoutPolicyType> PolicyTypes {
            get {
                return policyTypes ?? (policyTypes = new List<LayoutPolicyType>(new LayoutPolicyType[] {
                        new LayoutPolicyType("Global scripts", "Global", LayoutModel.StateManager.LayoutPolicies),
                        new LayoutPolicyType("Block scripts", "BlockInfo", LayoutModel.StateManager.BlockInfoPolicies),
                        new LayoutPolicyType("Trip plan scripts", "TripPlan", LayoutModel.StateManager.TripPlanPolicies),
                        new LayoutPolicyType("Before Trip Section Start scripts", "RideStart", LayoutModel.StateManager.RideStartPolicies),
                        new LayoutPolicyType("Driver Instructions scrtips", "DriverInstructions", LayoutModel.StateManager.DriverInstructionsPolicies)
                    }));
            }
        }

        public OperationStates OperationStates {
            get {
                return operationStates ?? (operationStates = new OperationStates());
            }
        }

        /// <summary>
        /// Remove all state information
        /// </summary>
        public void Clear() {
            Trains.Clear();
            Components.Clear();
            TripPlansCatalog.Clear();
            ManualDispatchRegions.Clear();
            operationStates = null;
        }

        public void Save() {
            Components.RemoveEmpty();
            XmlDocument.Save(LayoutController.LayoutRuntimeStateFilename);
        }

        public void Load() {
            try {
                XmlDocument.Load(LayoutController.LayoutRuntimeStateFilename);
                initialize();
            }
            catch (IOException) {
                if (Element == null)
                    XmlDocument.LoadXml("<LayoutState><Trains /><Components /><TrackContacts /><TripPlansCatalog /></LayoutState>");
                initialize();
            }
        }

        public bool AllLayoutManualDispatch {
            get => (bool?)Element.AttributeValue(A_AllLayoutManualDispatchRegion) ?? false;

            set {
                bool active = false;

                if (value && allLayoutManualDispatch == null) {
                    allLayoutManualDispatch = new AllLayoutManualDispatchRegion();

                    try {
                        allLayoutManualDispatch.Active = true;
                        Element.SetAttribute(A_AllLayoutManualDispatchRegion, true);
                        active = true;
                    }
                    catch (LayoutException) {
                        allLayoutManualDispatch = null;
                        EventManager.Event(new LayoutEvent("all-layout-manual-dispatch-mode-status-changed", this, active));
                        throw;
                    }
                }
                else if (value == false && allLayoutManualDispatch != null) {
                    allLayoutManualDispatch.Active = false;
                    allLayoutManualDispatch = null;
                    Element.RemoveAttribute(A_AllLayoutManualDispatchRegion);
                }

                EventManager.Event(new LayoutEvent("all-layout-manual-dispatch-mode-status-changed", this, active));
            }
        }

        [LayoutEvent("exit-operation-mode")]
        private void exitOperationMode(LayoutEvent e) {
            Save();
            allLayoutManualDispatch = null;
        }
    }
}
