
/// Definition of interfaces
/// 

using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

#nullable enable
namespace LayoutManager {
    #region Interfaces for various services

    public interface ILayoutController {
        // Properties
        ILayoutFrameWindow ActiveFrameWindow { get; set; }

        string LayoutFilename { get; }
        string LayoutRuntimeStateFilename { get; }
        string LayoutDisplayStateFilename { get; }
        string LayoutRoutingFilename { get; }
        string CommonDataFolderName { get; }

        LayoutModuleManager ModuleManager { get; }

        bool IsOperationMode { get; }
        bool TrainsAnalysisPhase { get; }
        bool IsDesignTimeActivation { get; }

        PreviewRouteManager PreviewRouteManager { get; }
        LayoutSelection UserSelection { get; }
        OperationModeParameters? OperationModeSettings { get; }
        ILayoutCommandManager CommandManager { get; }

        // Operations

        /// <summary>
        /// Execute command and add it to the undo stack
        /// </summary>
        /// <param name="command"></param>
        void Do(ILayoutCommand command);

        /// <summary>
        /// Mark layout as modified
        /// </summary>
        void LayoutModified();

        LayoutModelArea AddArea(string areaName);

        bool EnterOperationModeRequest(OperationModeParameters settings);
        Task ExitOperationModeRequest();
        Task EnterDesignModeRequest();
        bool BeginDesignTimeActivation();
        void EndDesignTimeActivation();
    }

    public interface ILayoutFrameWindow : IWin32Window, IObjectHasId {
    }

    public static class LayoutController {
        private static ILayoutController? _instance;

        // Properties
        public static ILayoutController Instance {
            get => _instance ?? throw new NullReferenceException("Getting LayoutController.Instance before it has been set");
            set { _instance = value; }
        }

        public static string LayoutFilename => Instance.LayoutFilename;
        public static string LayoutRuntimeStateFilename => Instance.LayoutRuntimeStateFilename;
        public static string LayoutRoutingFilename => Instance.LayoutRoutingFilename;
        public static string CommonDataFolderName => Instance.CommonDataFolderName;

        public static bool IsOperationMode => Instance.IsOperationMode;
        public static bool IsDesignMode => !IsOperationMode;
        public static bool TrainsAnalysisPhase => Instance.TrainsAnalysisPhase;
        public static bool IsDesignTimeActivation => Instance.IsDesignTimeActivation;
        public static bool IsOperationSimulationMode => IsOperationMode && OperationModeSettings != null && OperationModeSettings.Simulation;

        public static PreviewRouteManager PreviewRouteManager => Instance.PreviewRouteManager;
        public static ILayoutSelectionManager SelectionManager => (ILayoutSelectionManager)Instance;
        public static LayoutSelection UserSelection => Instance.UserSelection;
        public static OperationModeParameters? OptionalOperationModeSettings => Instance.OperationModeSettings;
        public static OperationModeParameters OperationModeSettings => Ensure.NotNull<OperationModeParameters>(Instance.OperationModeSettings);
        public static ILayoutCommandManager CommandManager => Instance.CommandManager;
        public static LayoutModuleManager ModuleManager => Instance.ModuleManager;

        /// <summary>
        /// Currently active frame window
        /// </summary>
        public static ILayoutFrameWindow ActiveFrameWindow {
            get { return Instance.ActiveFrameWindow; }
            set { Instance.ActiveFrameWindow = value; }
        }

        // Operations
        public static void Do(ILayoutCommand command) { Instance.Do(command); }
        public static void LayoutModified() { Instance.LayoutModified(); }
        public static LayoutModelArea AddArea(string areaName) => Instance.AddArea(areaName);
    }

    /// <summary>
    /// This interface defines methods that are implemented by the controller and
    /// used by the selection class and the view to show/hide selections
    /// </summary>
    public interface ILayoutSelectionManager {
        /// <summary>
        /// Cause a selection to displayed on views controlled by the controller
        /// </summary>
        /// <param name="selection">Selection to be shown</param>
        /// <param name="zOrder">Can be either LayoutSelection.ZOrderTopMost or LayoutSelection.ZOrderBottom</param>
        void DisplaySelection(LayoutSelection selection, int zOrder);

        /// <summary>
        /// Stop showing a selection
        /// </summary>
        /// <param name="selection">The selection that will no longer be displayed</param>
        void HideSelection(LayoutSelection selection);

        /// <summary>
        /// An array of all selections which are currently displayed
        /// </summary>
        IList<LayoutSelection> DisplayedSelections {
            get;
        }
    }

    public interface ILayoutTopologyServices {
        /// <summary>
        /// Find the TrackEdge (a combination of a track component and a connection point) that connects
        /// to a given track edge. First looking for track in the same layer. If one cannot be found, look
        /// to see if there is one in another layer. The function will issue a warning if more than one
        /// track could connect to the given track
        /// </summary>
        /// <param name="trackEdge">The track edge for which a track connection is looked for</param>
        /// <param name="phase">Find only spot that are in this phase</param>
        TrackEdge FindTrackConnectingAt(TrackEdge trackEdge, LayoutPhase phase, bool trackLinks);

        /// <summary>
        /// Find the TrackEdge (a combination of a track component and a connection point) that connects
        /// to a given track edge. First looking for track in the same layer. If one cannot be found, look
        /// to see if there is one in another layer. The function will issue a warning if more than one
        /// track could connect to the given track. This function follows track links
        /// </summary>
        /// <param name="edge">The track edge for which a track connection is looked for</param>
        TrackEdge FindTrackConnectingAt(TrackEdge edge);

        /// <summary>
        /// Find all track and connection points that connects to the given track. First, all connection points
        /// that connect to the given connection point are found. Then tracks that connect to those connection
        /// points are looked for.
        /// </summary>
        /// <param name="edge">The track/connection point combination to look from</param>
        /// <param name="type">The type of connection</param>
        /// <returns>An array of all track that connect to the current track</returns>
        TrackEdge[] FindConnectingTracks(TrackEdge edge, LayoutComponentConnectionType type);
    }

    public interface ILayoutLockManagerServices {
        /// <summary>
        /// Return whether this block has other pending locks on it
        /// </summary>
        /// <param name="blockID"></param>
        /// <returns></returns>
        bool HasPendingLocks(Guid blockId, Guid ofOwnerOtherThen);
    }

    public interface IRoutePlanningServices {
        /// <summary>
        /// Find the best route to a model component
        /// </summary>
        /// <param name="sourceComponent">The source component</param>
        /// <param name="front">The connection point to which the train front points to</param>
        /// <param name="direction">The direction in which the train will move</param>
        /// <param name="destinationComponent">The destination component</param>
        /// <param name="routeOwner">The train/manual dispatch region for which the route is calculated</param>
        /// <returns>BestRoute object describing the best route and its quality</returns>
        BestRoute FindBestRoute(ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, ModelComponent destinationComponent, Guid routeOwner, bool trainStopping);

        /// <summary>
        /// Find the best route to a given destination (which may be a "smart destination")
        /// </summary>
        /// <param name="sourceComponent">The source component</param>
        /// <param name="front">The connection point to which the train front points to</param>
        /// <param name="direction">The direction in which the train will move</param>
        /// <param name="destination">The destination</param>
        /// <param name="routeOwner">The train/manual dispatch region for which the route is calculated</param>
        /// <returns>BestRoute object describing the best route and its quality</returns>
        BestRoute FindBestRoute(ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, TripPlanDestinationInfo destination, Guid routeOwner, bool trainStopping);
    }

    #endregion

    #region Object property interfaces (mostly UI related)

    public class CreateTrainSettings {
        public string? TrainName { get; set; }
        public LayoutComponentConnectionPoint? Front { get; set; }
        public bool ValidateAddress { get; set; } = true;
        public TrainLength? Length { get; set; }
    }

    public class IsLocomotiveAddressValidSettings {
        public LocomotiveOrientation Orientation { get; set; } = LocomotiveOrientation.Forward;
        public int? LocomotiveAddress { get; set; }

    }

    /// <summary>
    /// This interface is implemented by any object that is capable of being locked by the lock
    /// manager. For example, a block object
    /// </summary>
    public interface ILayoutLockResource : IModelComponentHasId {
        /// <summary>
        /// Return true if resource is ready to be locked.
        /// </summary>
        /// <returns></returns>
        bool IsResourceReady();

        /// <summary>
        /// Make sure that the resource ready to be locked. 
        /// </summary>
        /// <remarks>
        /// For example, if the resource is some kind of a gate,
        /// then it is ready to be locked when the gate is open. If the function returns false, it is
        /// expected that the resource will generate a Dispatch.Call.LayoutLockResourceReady when it becomes ready
        /// (in our example, calling IsResourceReady when the gate is closed will initiate the opening
        /// of this gate, and when the gate is open, the "layout-lock-resource-ready" event is sent.
        /// </remarks>
        void MakeResourceReady();

        /// <summary>
        /// The resource is no longer needed, for example, if the resource is a gate, the gate may be closed.
        /// </summary>
        void FreeResource();
    }

    /// <summary>
    /// The object has name provider
    /// </summary>
    public interface IObjectHasName {
        LayoutTextInfo NameProvider {
            get;
        }
    }

    static class DispatcherFilter {
        static private bool MyNameFilter(string? filterValue, object? targetObject, object? parameterValue) {
            if (filterValue != null)
                throw new DispatchFilterException("Unexpected Value in DispatchFilter attribute");

            if (targetObject == null || parameterValue == null)
                return false;

            var parameterNameValue = parameterValue switch {
                string s => s,
                IObjectHasName name => name.NameProvider.Name,
                _ => throw new DispatchFilterException("parameter is not object with name")
            };

            if (targetObject is IObjectHasName targetWithName)
                return parameterNameValue == targetWithName.NameProvider.Name;
            else
                throw new DispatchFilterException("Target object is not an object with Id");
        }

        [DispatchTarget]
        public static void AddDispatcherFilters() {
            Dispatch.AddCustomParameterFilter("IsMyName", MyNameFilter);
        }


    }
    /// <summary>
    /// This object can provide address information
    /// </summary>
    public interface IObjectHasAddress : IObjectHasId {
        LayoutAddressInfo AddressProvider {
            get;
        }
    }

    /// <summary>
    /// Implemented by forms that are properties of components
    /// </summary>
    public interface ILayoutComponentPropertiesDialog {
        LayoutXmlInfo XmlInfo {
            get;
        }

        DialogResult ShowDialog();
    }

    /// <summary>
    /// Implement by modeless forms that can receive components (for example trip plan
    /// editor which allows the user to add way points).
    /// </summary>
    public interface IModelComponentReceiverDialog {
        string DialogName(IModelComponent component);

        void AddComponent(IModelComponent component);
    }

    /// <summary>
    /// Implemented by the trip plan editor
    /// </summary>
    public interface ITripPlanEditorDialog {
        string DialogName {
            get;
        }

        void AddWayPoint(LayoutBlockDefinitionComponent blockDefinition);

        void AddWayPoint(TripPlanDestinationInfo destinationInfo);
    }

    public interface IControlConnectionPointDestinationReceiverDialog {
        string DialogName(ControlConnectionPointDestination connectionDestination);

        /// <summary>
        /// The bus that the module of control connection point (the exact module was not yet determined)
        /// </summary>
        /// <value>ControlBus</value>
        ControlBus Bus { get; }

        /// <summary>
        /// To where the connection should be done
        /// </summary>
        /// <param name="connectionDestination"></param>
        void AddControlConnectionPointDestination(ControlConnectionPointDestination connectionDestination);
    }

    /// <summary>
    /// This interface should be defined by objects that are drawing helpers (painters)
    /// </summary>
    public interface ILayoutComponentPainter {
        void Draw(Graphics g);
    }

    /// <summary>
    /// Encapsulate the visual properties of a selection.
    /// </summary>
    public interface ILayoutSelectionLook {
        /// <summary>
        /// The selection visual indicator color
        /// </summary>
        Color Color {
            get;
        }
    }

    /// <summary>
    /// This object functions as a command station emulator
    /// </summary>
    public interface ILayoutCommandStationEmulator : IDisposable {
    }

    #endregion

    #region Trip Route planning related interfaces

    /// <summary>
    /// Describe one possible route from an origin to a destination.
    /// </summary>
    public interface ITripRoute {
        /// <summary>
        /// Return the number switch states in the route.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Return an array with switch states. For each multi-path component (e.g. turnout), along the route
        /// the corresponding element provide the switch state that this component should be set to for arriving
        /// from the origin to the destination
        /// </summary>
        IList<int> SwitchStates {
            get;
        }

        /// <summary>
        /// Get a selection containing all components in the route
        /// </summary>
        LayoutSelection Selection {
            get;
        }

        /// <summary>
        /// Return track edges along the route
        /// </summary>
        IList<TrackEdge> TrackEdges {
            get;
        }

        /// <summary>
        /// Get the blocks in the route
        /// </summary>
        IList<LayoutBlock> Blocks {
            get;
        }

        /// <summary>
        /// Return the connection point that will contain the train front when it will
        /// arrive to the destination
        /// </summary>
        LayoutComponentConnectionPoint DestinationFront {
            get;
        }

        /// <summary>
        /// The track edge in which the route starts
        /// </summary>
        TrackEdge SourceEdge {
            get;
        }

        /// <summary>
        /// The track edge in which the route ends
        /// </summary>
        TrackEdge DestinationEdge {
            get;
        }

        /// <summary>
        /// The origin track
        /// </summary>
        LayoutTrackComponent SourceTrack { get; }

        /// <summary>
        /// The connection point pointed by the train front at the origin
        /// </summary>
        LayoutComponentConnectionPoint SourceFront { get; }

        /// <summary>
        /// The destination track
        /// </summary>
        LayoutTrackComponent? DestinationTrack { get; }

        /// <summary>
        /// The direction in which the train has to move from the origin (source)
        /// </summary>
        LocomotiveOrientation Direction { get; }

        /// <summary>
        /// The train/manual dispatch region for which the route was calculated
        /// </summary>
        Guid RouteOwner { get; }
    }

    public interface ITripRouteValidationResult {
        bool CanBeFixed { get; }

        IList<ITripRouteValidationAction> Actions { get; }
    }

    public interface ITripRouteValidationAction {
        /// <summary>
        /// Get the way point index for which this action applies
        /// </summary>
        int WaypointIndex { get; }

        /// <summary>
        /// Get description of the action to be applied
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Apply the action on the trip plan
        /// </summary>
        /// <param name="tripPlan"></param>
        void Apply(TripPlanInfo tripPlan);
    }

    #endregion

    #region Model Commands interfaces

    /// <summary>
    /// Base class for all layout commands. Basically a command encapsulate an operation
    /// (in most cases on the model). The command has enough information so it can
    /// undo the command
    /// </summary>
    public interface ILayoutCommand {
        /// <summary>
        /// Preform the operation of this command 
        /// </summary>
        void Do();

        /// <summary>
        /// Undo the operation done by this command
        /// </summary>
        void Undo();

        /// <summary>
        /// return a display description for the command. Used in Redo ... and
        /// Undo ... menu entries
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// True if this command returned the model to its persistent state
        /// </summary>
        bool IsCheckpoint {
            get;
        }
    }

    /// <summary>
    /// This interface describes a command that contains other commands. All the
    /// contained commands are done or undone in one operation
    /// </summary>
    public interface ILayoutCompoundCommand : ILayoutCommand {
        /// <summary>
        /// Add a command to the compound command
        /// </summary>
        /// <param name="command">The command to be added</param>
        void Add(ILayoutCommand command);

        /// <summary>
        /// The number of commands in the compound command
        /// </summary>
        int Count {
            get;
        }
    }

    public interface ILayoutCommandManager {
        int ChangeLevel { get; }

        /// <summary>
        /// Execute a command and add it to the command stack
        /// </summary>
        /// <param name="command">The command object to be carried out</param>
        void Do(ILayoutCommand command);

        /// <summary>
        /// Undo the "last" command
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo the last undone command
        /// </summary>
        void Redo();

        /// <summary>
        /// Return true if there is a command that can be undone
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Return true if there is a command that can be redone
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Return the command name that can be undone
        /// </summary>
        string UndoCommandName { get; }

        /// <summary>
        /// Return the command name that can be redone
        /// </summary>
        string RedoCommandName { get; }
    }

    #endregion

    #region Layout power related interfaces
    /// <summary>
    /// Enumeration of power type
    /// </summary>
    public enum LayoutPowerType {
        Disconnected,       // No power
        Digital,            // Digital DCC power
        Analog,             // Analog power
        Programmer,         // DCC programmer
    }

    [Flags]
    public enum DigitalPowerFormats {
        NRMA = 0x01,
        Motorola = 0x02,

        None = 0x00,
        All = NRMA | Motorola,
    }

    /// <summary>
    /// Represent power connected to tracks.
    /// </summary>
    public interface ILayoutPower {
        LayoutPowerType Type {
            get;
        }

        DigitalPowerFormats DigitalFormats {
            get;
        }

        string Name {
            get;
        }

        /// <summary>
        /// This ID of the component supplying the power
        /// </summary>
        Guid PowerOriginComponentId {
            get;
        }

        /// <summary>
        /// The component generating this power
        /// </summary>
        IModelComponentHasPowerOutlets PowerOriginComponent {
            get;
        }
    }

    /// <summary>
    /// A component that can provide power (for example a command station) implement this interface for each power outlet
    /// </summary>
    public interface ILayoutPowerOutlet {
        IModelComponentHasPowerOutlets OutletComponent { get; }

        /// <summary>
        /// The description of this power source. Please note that a component may generate more than one power source.
        /// For example, a command station could generate both digital track power and programming track power.
        /// </summary>
        string OutletDescription {
            get;
        }

        /// <summary>
        /// The current power that this power source generates
        /// </summary>
        ILayoutPower Power {
            get;
        }

        ILayoutPower? OptionalPower { get; }

        /// <summary>
        /// The powers that can be obtained from this outlet. For example, The outlet of a power selector component can contain
        /// each of the power type that are connected to its inlet
        /// </summary>
        IEnumerable<ILayoutPower> ObtainablePowers { get; }

        /// <summary>
        /// Select power of a given type
        /// </summary>
        /// <param name="powerType">The power type to select</param>
        /// <returns>True of power was successfully selected</returns>
        bool SelectPower(LayoutPowerType powerType, IList<SwitchingCommand> switchingCommands);
    }

    public interface ILayoutPowerInlet : IObjectHasXml {
        /// <summary>
        /// The component Id to which this inlet is connected. If this inlet is not connected then get/set ID to Guid.Empty
        /// </summary>
        Guid OutletComponentId { get; set; }

        /// <summary>
        /// The component to which this inlet is connected (null if inlet is not connected)
        /// </summary>
        IModelComponentHasPowerOutlets? OutletComponent { get; set; }

        /// <summary>
        /// A component with power outlets may have more than 1 outlet. This property contains the outlet index to which this inlet is connected
        /// </summary>
        int OutletIndex { get; set; }

        /// <summary>
        /// The power outlet to which this inlet is connected
        /// </summary>
        ILayoutPowerOutlet ConnectedOutlet { get; }

        /// <summary>
        /// Is this inlet connected
        /// </summary>
        bool IsConnected { get; }
    }

    #endregion

    #region Script Editor related interfaces

    /// <summary>
    /// Functionality implemented by the event script editor
    /// </summary>
    public interface IEventScriptEditor {
        Form Form { get; }

        LayoutBlockDefinitionComponent BlockDefinition { get; }
    }

    public interface IEventScriptEditorSite : IEventScriptEditor {
        void EditingDone();

        void EditingCancelled();
    }

    public interface IEventScriptEditorMenuEntry {
        void SetEventScriptEditor(IEventScriptEditor eventScriptEditor);
    }

    #endregion

    #region Drawing region related interfaces

    /// <summary>
    /// Implement by object referenced by Region.Info field to be notify when the region is clicked in editing mode.
    /// </summary>
    public interface ILayoutDrawingRegionInfoEditingOnClick {
        bool OnRegionClick();
    }

    /// <summary>
    /// Implement by object referenced by Region.Info field to be notify when the region is right clicked in editing mode.
    /// </summary>
    public interface ILayoutDrawingRegionInfoEditingOnRightClick {
        bool OnRegionRightClick();
    }

    /// <summary>
    /// Implement by object referenced by Region.Info field to be notify when the region is clicked in operation mode.
    /// </summary>
    public interface ILayoutDrawingRegionInfoOperationOnClick {
        bool OnRegionClick();
    }

    /// <summary>
    /// Implement by object referenced by Region.Info field to be notify when the region is right clicked in operation mode.
    /// </summary>
    public interface ILayoutDrawingRegionInfoOperationOnRightClick {
        bool OnRegionRightClick();
    }

    #endregion
}


