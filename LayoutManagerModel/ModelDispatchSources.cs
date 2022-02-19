using System;
using System.Xml;
using Microsoft.Win32.SafeHandles;

using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;

namespace LayoutManager {
    static public class ModelDispatchSources {
        public static void Initialize() { }

        [DispatchSource]
        public static System.IO.FileStream OpenSerialCommunicationDeviceRequest(this Dispatcher d, XmlElement setupElement) {
            return d[nameof(OpenSerialCommunicationDeviceRequest)].Call<System.IO.FileStream>(setupElement);
        }

        [DispatchSource]
        public static SafeFileHandle CreateNamedPipeRequest(this Dispatcher d, string pipeName, bool overlapppedIo) {
            return d[nameof(CreateNamedPipeRequest)].Call<SafeFileHandle>(pipeName, overlapppedIo);
        }

        [DispatchSource]
        public static System.IO.FileStream WaitNamedPipeClientToConnectRequest(this Dispatcher d, SafeFileHandle handle, bool overlappedIo) {
            return d[nameof(WaitNamedPipeClientToConnectRequest)].Call<System.IO.FileStream>(handle, overlappedIo);
        }

        [DispatchSource]
        public static void DisconnectNamedPipeRequest(this Dispatcher d, object handle) {
            d[nameof(DisconnectNamedPipeRequest)].CallVoid(handle);
        }

        [DispatchSource]
        public static LayoutComponentConnectionPoint? GetLocomotiveFront(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, object trainNameObject) {
            return d[nameof(GetLocomotiveFront)].CallNullable<LayoutComponentConnectionPoint>(blockDefinition, trainNameObject);
        }

        [DispatchSource]
        public static System.IO.FileStream WaitNamedPipeRequest(this Dispatcher d, string pipeName, bool overlappedIo) {
            return d[nameof(WaitNamedPipeRequest)].Call<System.IO.FileStream>(pipeName, overlappedIo);
        }

        [DispatchSource]
        public static LayoutComponentConnectionPoint? GetWaypointFront(this Dispatcher d, LayoutStraightTrackComponent track) {
            return d[nameof(GetWaypointFront)].CallNullable<LayoutComponentConnectionPoint?>(track);
        }

        [DispatchSource]
        public static TrainFrontAndLength? GetTrainFrontAndLength(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, XmlElement collectionElement) {
            return d[nameof(GetTrainFrontAndLength)].CallNullable<TrainFrontAndLength?>(blockDefinition, collectionElement);
        }

        [DispatchSource]
        public static ILayoutTopologyServices GetTopologyServices(this MethodDispatcher.Dispatcher d) {
            return d[nameof(GetTopologyServices)].Call<ILayoutTopologyServices>();
        }

        [DispatchSource]
        public static void RequestManualDispatchLock(this Dispatcher d, ManualDispatchRegionInfo manualDispatchRegion) {
            d[nameof(RequestManualDispatchLock)].CallVoid(manualDispatchRegion);
        }

        [DispatchSource]
        public static void FreeManualDispatchLock(this Dispatcher d, ManualDispatchRegionInfo manualDispatchRegion) {
            d[nameof(FreeManualDispatchLock)].CallVoid(manualDispatchRegion);
        }

        [DispatchSource]
        public static bool RequestLayoutLock(this Dispatcher d, LayoutLockRequest lockRequest) {
            return d[nameof(RequestLayoutLock)].Call<bool>(lockRequest);
        }

        [DispatchSource]
        public static Task<bool> RequestLayoutLockAsync(this Dispatcher d, LayoutLockRequest lockRequest) {
            return d[nameof(RequestLayoutLockAsync)].Call<Task<bool>>(lockRequest);
        }

        [DispatchSource]
        public static Task<TrainStateInfo> PlaceTrainRequest(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinnition, XmlElement collectionElement, CreateTrainSettings options, LayoutOperationContext operationContext) {
            return d[nameof(PlaceTrainRequest)].Call<Task<TrainStateInfo>>(blockDefinnition, collectionElement, options, operationContext);
        }

        [DispatchSource]
        public static TrainStateInfo CreateTrain(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, XmlElement collectionElement, CreateTrainSettings options) {
            return d[nameof(CreateTrain)].Call<TrainStateInfo>(blockDefinition, collectionElement, options);
        }

        [DispatchSource]
        public static void FreeLayoutLock(this Dispatcher d, object lockedObject) {
            d[nameof(FreeLayoutLock)].CallVoid(lockedObject);
        }

        [DispatchSource]
        public static void FreeOwnedLayoutLocks(this Dispatcher d, Guid ownerId, bool releasePending) {
            d[nameof(FreeOwnedLayoutLocks)].CallVoid(ownerId, releasePending);
        }

        [DispatchSource]
        public static void CancelLayoutLock(this Dispatcher d, LayoutLockRequest lockRequest) {
            d[nameof(CancelLayoutLock)].CallVoid(lockRequest);
        }

        [DispatchSource]
        public static ILayoutLockManagerServices GetLayoutLockManagerServices(this Dispatcher d) {
            return d[nameof(GetLayoutLockManagerServices)].Call<ILayoutLockManagerServices>();
        }

        [DispatchSource]
        public static void DumpLocks(this Dispatcher d) {
            d[nameof(DumpLocks)].CallVoid();
        }

        [DispatchSource]
        public static void LayoutLockResourceReady(this Dispatcher d, ILayoutLockResource resource) {
            d[nameof(LayoutLockResourceReady)].CallVoid(resource);
        }

        public static void OnLayoutLockGranted(this Dispatcher d, LayoutLockRequest lockRequest) {
            d[nameof(OnLayoutLockGranted)].CallVoid(lockRequest);
        }

        public static void OnLayoutLockReleased(this Dispatcher d, Guid resourceId) {
            d[nameof(OnLayoutLockReleased)].CallVoid(resourceId);
        }

        [DispatchSource]
        public static bool RebuildLayoutState(this Dispatcher d, LayoutPhase phase) {
            return d[nameof(RebuildLayoutState)].CallBoolFunctions(invokeUntil: false, invokeAll: true, phase);
        }

        [DispatchSource]
        public static void OnTrainCreated(this Dispatcher d, TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            d[nameof(OnTrainCreated)].CallVoid(train, blockDefinition);
        }

        [DispatchSource]
        public static void OnTrainPlacedOnTrack(this Dispatcher d, TrainStateInfo train) {
            d[nameof(OnTrainPlacedOnTrack)].CallVoid(train);
        }

        [DispatchSource]
        public static Task<TrainStateInfo> ValidateAndPlaceTrainRequest(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, XmlElement placedElement, CreateTrainSettings options, LayoutOperationContext operationContext) {
            return d[nameof(ValidateAndPlaceTrainRequest)].Call<Task<TrainStateInfo>>(blockDefinition, placedElement, options, operationContext);
        }

        [DispatchSource]
        public static CanPlaceTrainResult CanLocomotiveBePlaced(this Dispatcher d, XmlElement placeableElement, LayoutBlockDefinitionComponent blockDefinition) {
            return d[nameof(CanLocomotiveBePlaced)].Call<CanPlaceTrainResult>(placeableElement, blockDefinition);
        }

        [DispatchSource]
        public static CanPlaceTrainResult CanLocomotiveBePlacedOnTrack(this Dispatcher d, XmlElement placeableElement, LayoutBlockDefinitionComponent? blockDefinition = null) {
            return d[nameof(CanLocomotiveBePlacedOnTrack)].Call<CanPlaceTrainResult>(placeableElement, blockDefinition);
        }

        [DispatchSource]
        public static CanPlaceTrainResult IsLocomotiveAddressValid(this Dispatcher d, XmlElement placeableElement, object powerObject, IsLocomotiveAddressValidSettings settings) {
            return d[nameof(IsLocomotiveAddressValid)].Call<CanPlaceTrainResult>(placeableElement, powerObject, settings);
        }

        [DispatchSource]
        public static OnTrackLocomotiveAddressMap GetOnTrackLocomotiveAddressMap(this Dispatcher d, object commandStationObject) {
            return d[nameof(GetOnTrackLocomotiveAddressMap)].Call<OnTrackLocomotiveAddressMap>(commandStationObject);
        }

        [DispatchSource]
        public static int? AllocateLocomotiveAddress(this Dispatcher d, LocomotiveInfo locomotive) {
            return d[nameof(AllocateLocomotiveAddress)].Call<int?>(locomotive);
        }

        [DispatchSource]
        public static TrainStateInfo ExtractTrainState(this Dispatcher d, object trainStateObject) {
            return d[nameof(ExtractTrainState)].Call<TrainStateInfo>(trainStateObject);
        }

        [DispatchSource]
        public static Task RemoveFromTrackRequest(this Dispatcher d, TrainStateInfo train, string balloonText, LayoutOperationContext operationContext) {
            return d[nameof(RemoveFromTrackRequest)].Call<Task>(train, balloonText, operationContext);
        }

        [DispatchSource]
        public static Task<TrainStateInfo?> PlacedLocomotiveAddressReprogramming(this Dispatcher d, PlacedLocomotiveProgrammingState programmingState, CreateTrainSettings settings, LayoutOperationContext context) {
            return d[nameof(PlacedLocomotiveAddressReprogramming)].Call<Task<TrainStateInfo?>>(programmingState, settings, context);
        }

        [DispatchSource]
        public static Task<TrainStateInfo?> ProgramLocomotive(this Dispatcher d, LocomotiveProgrammingState programmingState, CreateTrainSettings settins, LayoutOperationContext operationContext) {
            return d[nameof(ProgramLocomotive)].Call<Task<TrainStateInfo?>>(programmingState, settins, operationContext);
        }

        [DispatchSource]
        public static LayoutBlockDefinitionComponent? GetProgrammingLocation(this Dispatcher d) {
            return d[nameof(GetProgrammingLocation)].Call<LayoutBlockDefinitionComponent?>();
        }

        [DispatchSource]
        public static bool QueryAction(this Dispatcher d, IHasDecoder target, string actionName) {
            return d[nameof(QueryAction)].CallBoolFunctions(invokeUntil: true, invokeAll: false, target, actionName);
        }

        [DispatchSource]
        public static LayoutAction? GetAction(this Dispatcher d, XmlElement actionElement, object owner) {
            return d[nameof(GetAction)].CallUntilNotNull<LayoutAction>(actionElement, owner);
        }

        [DispatchSource]
        public static Task SetPower(this Dispatcher d, object powerRegionObject, object powerObject, LayoutOperationContext operationContext) {
            return d[nameof(SetPower)].Call<Task>(powerRegionObject, powerObject, operationContext);
        }

        [DispatchSource]
        public static void RequestAutoExtendTrain(this Dispatcher d, TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            d[nameof(RequestAutoExtendTrain)].CallVoid(train, blockDefinition);
        }

        static DispatchSource? ds_GetCommandStation = null;
        [DispatchSource]
        public static IModelComponentHasNameAndId GetCommandStation(this Dispatcher d, object commandStationObject) {
            ds_GetCommandStation ??= d[nameof(GetCommandStation)];
            return ds_GetCommandStation.Call<IModelComponentHasNameAndId>(commandStationObject);
        }

        [DispatchSource]
        public static void SetLocomotiveSpeedRequest(this Dispatcher d, TrainStateInfo train, int speed) {
            d[nameof(SetLocomotiveSpeedRequest)].CallVoid(train, speed);
        }

        [DispatchSource]
        public static void LocomotiveMotionCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, int speed) {
            d[nameof(LocomotiveMotionCommand)].CallVoid(commandStation, locomotive, speed);
        }

        [DispatchSource]
        public static void ReverseTrainMotionDirectionRequest(this Dispatcher d, object trainObject) {
            d[nameof(ReverseTrainMotionDirectionRequest)].CallVoid(trainObject);
        }

        [DispatchSource]
        public static void LocomotiveReverseMotionDirectionCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, LocomotiveInfo loco) {
            d[nameof(LocomotiveReverseMotionDirectionCommand)].CallVoid(commandStation, loco);
        }

        [DispatchSource]
        public static void RelocateTrainRequest(this Dispatcher d, TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition, LayoutComponentConnectionPoint? front = null) {
            d[nameof(RelocateTrainRequest)].CallVoid(train, blockDefinition, front);
        }

        [DispatchSource]
        public static void OnTrainRelocated(this Dispatcher d, TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            d[nameof(OnTrainRelocated)].CallVoid(train, blockDefinition);
        }

        [DispatchSource]
        public static void OnLocomotiveMotion(this Dispatcher d, IModelComponentIsCommandStation commandStation, int speed, int unit) {
            d[nameof(OnLocomotiveMotion)].CallVoid(commandStation, speed, unit);
        }

        [DispatchSource]
        public static void SetLocomotiveLightsRequest(this Dispatcher d, TrainStateInfo train, bool lights) {
            d[nameof(SetLocomotiveSpeedRequest)].CallVoid(train, lights);
        }

        [DispatchSource]
        public static void SetLocomotiveLightCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, bool lightsOn) {
            d[nameof(SetLocomotiveLightCommand)].CallVoid(commandStation, locomotive, lightsOn);
        }

        [DispatchSource]
        public static void OnLocomotiveLightsToggled(this Dispatcher d, IModelComponentHasNameAndId commandStation, int unit) {
            d[nameof(OnLocomotiveLightsToggled)].CallVoid(commandStation, unit);
        }

        [DispatchSource]
        public static void OnLocomotiveLightsChanged(this Dispatcher d, IModelComponentHasNameAndId commandStation, int unit, bool lights) {
            d[nameof(OnLocomotiveLightsChanged)].CallVoid(commandStation, unit, lights);
        }

        [DispatchSource]
        public static void SetLocomotiveFunctionStateRequest(this Dispatcher d, TrainStateInfo train, Guid locomotiveId, string functionName, bool functionState) {
            d[nameof(SetLocomotiveFunctionStateRequest)].CallVoid(train, locomotiveId, functionName, functionState);
        }

        [DispatchSource]
        public static void SetLocomotiveFunctionStateCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, string functionName, bool functionState) {
            d[nameof(SetLocomotiveFunctionStateCommand)].CallVoid(commandStation, locomotive, functionName, functionState);
        }

        [DispatchSource]
        public static void TriggerLocomotiveFunctionStateRequest(this Dispatcher d, TrainStateInfo train, Guid locomotiveId, string functionName) {
            d[nameof(TriggerLocomotiveFunctionStateRequest)].CallVoid(train, locomotiveId, functionName);
        }

        [DispatchSource]
        public static void TriggerLocomotiveFunctionStateCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, LocomotiveInfo locomotive, string functionName, bool functionState) {
            d[nameof(TriggerLocomotiveFunctionStateCommand)].CallVoid(commandStation, locomotive, functionName, functionState);
        }

        [DispatchSource]
        public static void OnLocomotiveFunctionStateChanged(this Dispatcher d, IModelComponentHasNameAndId commandStation, int unit, int functionNumber, bool functionState) {
            d[nameof(OnLocomotiveFunctionStateChanged)].CallVoid(commandStation, unit, functionNumber, functionState);
        }

        static DispatchSource? ds_TriggerLocomotiveFunctionNumberCommand = null;
        [DispatchSource]
        public static void TriggerLocomotiveFunctionNumberCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, LocomotiveInfo loco, int functionNumber, bool functionState) {
            ds_TriggerLocomotiveFunctionNumberCommand ??= d[nameof(TriggerLocomotiveFunctionNumberCommand)];
            ds_TriggerLocomotiveFunctionNumberCommand.CallVoid(commandStation, loco, functionNumber, functionState);
        }


        static DispatchSource? ds_ChangeSignalStateCommand = null;
        [DispatchSource]
        public static void ChangeSignalStateCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, ControlConnectionPointReference connectionPointRef, LayoutSignalState state) {
            ds_ChangeSignalStateCommand ??= d[nameof(ChangeSignalStateCommand)];
            ds_ChangeSignalStateCommand.CallVoid(commandStation, connectionPointRef, state);
        }

        static DispatchSource? ds_OnSignalStateRemoved = null;
        [DispatchSource]
        public static void OnSignalStateRemoved(this Dispatcher d, LayoutBlockEdgeBase blockEdge) {
            ds_OnSignalStateRemoved ??= d[nameof(OnSignalStateRemoved)];
            ds_OnSignalStateRemoved.CallVoid(blockEdge);
        }

        [DispatchSource]
        public static void OnTrainControllerActivated(this Dispatcher d, TrainStateInfo train) {
            d[nameof(OnTrainControllerActivated)].CallVoid(train);
        }

        [DispatchSource]
        public static void OnTrainControllerDeactivated(this Dispatcher d, TrainStateInfo train) {
            d[nameof(OnTrainControllerDeactivated)].CallVoid(train);
        }

        [DispatchSource]
        public static void OnLocomotiveControllerActivated(this Dispatcher d, TrainStateInfo train, TrainLocomotiveInfo trainLoco) {
            d[nameof(OnLocomotiveControllerActivated)].CallVoid(train, trainLoco);
        }

        [DispatchSource]
        public static void OnLocomotiveControllerDeactivated(this Dispatcher d, TrainStateInfo train, TrainLocomotiveInfo trainLoco) {
            d[nameof(OnLocomotiveControllerDeactivated)].CallVoid(train, trainLoco);
        }

        [DispatchSource]
        public static void AddCommandStationLocomotiveBusToAddressMap(this Dispatcher d, IModelComponentHasNameAndId commandStation, LocomotiveAddressMap addressMap) {
            d[nameof(AddCommandStationLocomotiveBusToAddressMap)].CallVoid(commandStation, addressMap);
        }

        [DispatchSource]
        public static Task<LayoutActionFailure?> DoCommandStationActions(this Dispatcher d, IModelComponentCanProgramLocomotives commandStation, ILayoutActionContainer actions, bool usePOM) {
            return d[nameof(DoCommandStationActions)].Call<Task<LayoutActionFailure?>>(commandStation, actions, usePOM);
        }

        [DispatchSource]
        public static void OnLocomotiveConfigurationChanged(this Dispatcher d, LocomotiveInfo locomotive) {
            d[nameof(OnLocomotiveConfigurationChanged)].CallVoid(locomotive);
        }


        static DispatchSource? ds_OnPrepareEnterOperationMode = null;
        [DispatchSource]
        public static void OnPrepareEnterOperationMode(this Dispatcher d, OperationModeParameters operationModeParameters) {
            ds_OnPrepareEnterOperationMode ??= d[nameof(OnPrepareEnterOperationMode)];
            ds_OnPrepareEnterOperationMode.CallVoid(operationModeParameters);
        }

        [DispatchSource]
        public static void OnEnteredOperationMode(this Dispatcher d, OperationModeParameters settings) {
            d[nameof(OnEnteredOperationMode)].CallVoid(settings);
        }

        [DispatchSource]
        public static void OnExitOperationMode(this Dispatcher d) {
            d[nameof(OnExitOperationMode)].CallVoid();
        }

        [DispatchSource]
        public static bool VerifyComponentStateTopic(this Dispatcher d, XmlElement componentStateElement) {
            return d[nameof(VerifyComponentStateTopic)].CallBoolFunctions(invokeUntil: false, invokeAll: false, componentStateElement);
        }

        [DispatchSource]
        public static void ClearLayoutState(this Dispatcher d) {
            d[nameof(ClearLayoutState)].CallVoid();
        }

        // ComponentManager

        [DispatchSource]
        public static void OnControlConnectionPointStateChanged(this Dispatcher d, ControlConnectionPointReference connectionPointRef, int state) {
            d[nameof(OnControlConnectionPointStateChanged)].CallVoid(connectionPointRef, state);
        }

        [DispatchSource]
        public static void OnTrackContactTriggered(this Dispatcher d, LayoutTrackContactComponent component) {
            d[nameof(OnTrackContactTriggered)].CallVoid(component);
        }

        [DispatchSource]
        public static void OnProximitySensorStateChanged(this Dispatcher d, LayoutProximitySensorComponent proximitySensor, bool state) {
            d[nameof(OnProximitySensorStateChanged)].CallVoid(proximitySensor, state);
        }

        [DispatchSource]
        public static void OnTrainDetectionStateChanged(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, bool state) {
            d[nameof(OnTrainDetectionStateChanged)].CallVoid(blockDefinition, state);
        }

        [DispatchSource]
        public static void OnTrackComponentStateChanged(this Dispatcher d, ControlConnectionPoint connectionPoint, int state) {
            d[nameof(OnTrackComponentStateChanged)].CallVoid(connectionPoint, state);
        }

        [DispatchSource]
        public static void OnSignalStateChanged(this Dispatcher d, LayoutSignalComponent signal, LayoutSignalState state) {
            d[nameof(OnSignalStateChanged)].CallVoid(signal, state);
        }

        [DispatchSource]
        public static void OnControlConnectionPointStateUnstable(this Dispatcher d, ControlConnectionPointReference connectionRef) {
            d[nameof(OnControlConnectionPointStateUnstable)].CallVoid(connectionRef);
        }

        [DispatchSource]
        public static Task SetTrackComponentsState(this Dispatcher d, List<SwitchingCommand> switchingCommands) {
            return d[nameof(SetTrackComponentsState)].Call<Task>(switchingCommands);
        }

        [DispatchSource]
        public static Task ChangeBatchOfTrackComponentStateCommand(this Dispatcher d, Guid commandStationId, List<SwitchingCommand> switchingCommands) {
            return d[nameof(ChangeBatchOfTrackComponentStateCommand)].Call<Task>(commandStationId, switchingCommands);
        }

        static DispatchSource? ds_ChangeTrackComponentStateCommand = null;
        public static Task ChangeTrackComponentStateCommand(this Dispatcher d, IModelComponentHasNameAndId commandStation, ControlConnectionPointReference controlPointRef, int state) {
            ds_ChangeTrackComponentStateCommand ??= d[nameof(ChangeTrackComponentStateCommand)];
            return ds_ChangeTrackComponentStateCommand.Call<Task>(commandStation, controlPointRef, state);
        }

        [DispatchSource]
        public static void OnProximitySensitivityDelayChanged(this Dispatcher d) {
            d[nameof(OnProximitySensitivityDelayChanged)].CallVoid();
        }

        [DispatchSource]
        public static void OnLogicalSignalStateChanged(this Dispatcher d, LayoutBlockEdgeBase blockEdge, LayoutSignalState state) {
            d[nameof(OnLogicalSignalStateChanged)].CallVoid(blockEdge, state);
        }

        [DispatchSource]
        public static Dictionary<Guid, LayoutBlockEdgeBase> GetLinkedSignalMap(this Dispatcher d) {
            return d[nameof(GetLinkedSignalMap)].Call<Dictionary<Guid, LayoutBlockEdgeBase>>();
        }

        [DispatchSource]
        public static void OnSignalComponentLinked(this Dispatcher d, LayoutBlockEdgeBase blockEdge, LayoutSignalComponent signalComponent) {
            d[nameof(OnSignalComponentLinked)].CallVoid(blockEdge, signalComponent);
        }

        [DispatchSource]
        public static void OnSignalComponentUnlinked(this Dispatcher d, LayoutSignalComponent signalComponent) {
            d[nameof(OnSignalComponentUnlinked)].CallVoid(signalComponent);
        }

        [DispatchSource]
        public static void OnRemovedFromModel(this Dispatcher d, ModelComponent component) {
            d[nameof(OnRemovedFromModel)].CallVoid(component);
        }

        [DispatchSource]
        public static void OnComponentConfigurationChanged(this Dispatcher d, ModelComponent component) {
            d[nameof(OnComponentConfigurationChanged)].CallVoid(component);
        }

        // LayoutBlocks

        [DispatchSource]
        public static bool CheckLayout(this Dispatcher d, LayoutPhase phase) {
            return d[nameof(CheckLayout)].CallBoolFunctions(invokeUntil: false, invokeAll: false, phase);
        }

        static DispatchSource? ds_OnTrainEnteredBlock = null;
        [DispatchSource]
        public static void OnTrainEnteredBlock(this Dispatcher d, TrainStateInfo train, LayoutBlock block) {
            ds_OnTrainEnteredBlock ??= d[nameof(OnTrainEnteredBlock)];
            ds_OnTrainEnteredBlock.CallVoid(train, block);
        }

        static DispatchSource? ds_OnTrainExtended = null;
        [DispatchSource]
        public static void OnTrainExtended(this Dispatcher d, TrainStateInfo train, LayoutBlock block) {
            ds_OnTrainExtended ??= d[nameof(OnTrainExtended)];
            ds_OnTrainExtended.CallVoid(train, block);
        }

        static DispatchSource? ds_isTrainInActiveTrip = null;
        [DispatchSource]
        public static bool IsTrainInActiveTrip(this Dispatcher d, TrainStateInfo train) {
            ds_isTrainInActiveTrip ??= d[nameof(IsTrainInActiveTrip)];
            return ds_isTrainInActiveTrip.Call<bool>(train);
        }

        // LayoutPower

        [DispatchSource]
        public static void OnPowerOutletChangedState(this Dispatcher d, ILayoutPowerOutlet powerOutlet, ILayoutPower? power) {
            d[nameof(OnPowerOutletChangedState)].CallVoid(powerOutlet, power);
        }

        [DispatchSource]
        public static void RegisterPowerConnectedLock(this Dispatcher d, LayoutTrackPowerConnectorComponent powerConnector) {
            d[nameof(RegisterPowerConnectedLock)].CallVoid(powerConnector);
        }

        [DispatchSource]
        public static void AddPowerConnectorAsResource(this Dispatcher d, LayoutTrackPowerConnectorComponent powerConnector, Action<LayoutBlockDefinitionComponent> addPowerConnectorAsResourceAction) {
            d[nameof(AddPowerConnectorAsResource)].CallVoid(powerConnector, addPowerConnectorAsResourceAction);
        }

        [DispatchSource]
        public static void OnTrainPowerChanged(this Dispatcher d, TrainStateInfo train, ILayoutPower power) {
            d[nameof(OnTrainPowerChanged)].CallVoid(train, power);
        }

        [DispatchSource]
        public static void OnLocomotivePowerChanged(this Dispatcher d, TrainLocomotiveInfo trainLoco, ILayoutPower power) {
            d[nameof(OnLocomotivePowerChanged)].CallVoid(trainLoco, power);
        }

        [DispatchSource]
        public static void ChangeTrainPower(this Dispatcher d, TrainStateInfo train, ILayoutPower power) {
            d[nameof(ChangeTrainPower)].CallVoid(train, power);
        }

        [DispatchSource]
        public static void OnCommandStationPowerOn(this Dispatcher d, IModelComponentIsCommandStation commandStation) {
            d[nameof(OnCommandStationPowerOn)].CallVoid(commandStation);
        }

        [DispatchSource]
        public static void OnTrainIsRemoved(this Dispatcher d, TrainStateInfo train) {
            d[nameof(OnTrainIsRemoved)].CallVoid(train);
        }

        [DispatchSource]
        public static void OnTrainWasRemoved(this Dispatcher d, TrainStateInfo train) {
            d[nameof(OnTrainWasRemoved)].CallVoid(train);
        }

        // LayoutValidator

        [DispatchSource]
        public static void PerformTrainsAnalysis(this Dispatcher d) {
            d[nameof(PerformTrainsAnalysis)].CallVoid();
        }

        // Locomotive Tracking

        static DispatchSource? ds_OnBlockEdgeSensorActive = null;
        [DispatchSource]
        public static void OnBlockEdgeSensorActive(this Dispatcher d, LayoutTriggerableBlockEdgeBase blockEdge) {
            ds_OnBlockEdgeSensorActive ??= d[nameof(OnBlockEdgeSensorActive)];
            ds_OnBlockEdgeSensorActive.CallVoid(blockEdge);
        }

        static DispatchSource? ds_OnBlockEdgeSensorNotActive = null;
        [DispatchSource]
        public static void OnBlockEdgeSensorNotActive(this Dispatcher d, LayoutTriggerableBlockEdgeBase blockEdge) {
            ds_OnBlockEdgeSensorNotActive ??= d[nameof(OnBlockEdgeSensorNotActive)];
            ds_OnBlockEdgeSensorNotActive.CallVoid(blockEdge);
        }

        [DispatchSource]
        public static void EmergencyStopRequest(this Dispatcher d, string reason, IModelComponentIsCommandStation? commandStation = null) {
            d[nameof(EmergencyStopRequest)].CallVoid(commandStation, reason);
        }

        static DispatchSource? ds_OnTrackedTrainCrossedBlockEdge = null;
        [DispatchSource]
        public static void OnTrackedTrainCrossedBlockEdge(this Dispatcher d, LocomotiveTrackingResult trackingResult) {
            ds_OnTrackedTrainCrossedBlockEdge ??= d[nameof(OnTrackedTrainCrossedBlockEdge)];
            ds_OnTrackedTrainCrossedBlockEdge.CallVoid(trackingResult);
        }

        static DispatchSource? ds_OnTrainCrossedBlockEdge = null;
        [DispatchSource]
        public static void OnTrainCrossedBlockEdge(this Dispatcher d, TrainStateInfo train, LayoutBlockEdgeBase blockEdge) {
            ds_OnTrainCrossedBlockEdge ??= d[nameof(OnTrainCrossedBlockEdge)];
            ds_OnTrainCrossedBlockEdge.CallVoid(train, blockEdge);
        }

        static DispatchSource? ds_OnTrainDetectionBlockWillBeOccupied = null;
        [DispatchSource]
        public static void OnTrainDetectionBlockWillBeOccupied(this Dispatcher d, LayoutOccupancyBlock occupancyBlock) {
            ds_OnTrainDetectionBlockWillBeOccupied ??= d[nameof(OnTrainDetectionBlockWillBeOccupied)];
            ds_OnTrainDetectionBlockWillBeOccupied.CallVoid(occupancyBlock);
        }

        static DispatchSource? ds_OnTrainDetectionBlockWillBeFree = null;
        [DispatchSource]
        public static void OnTrainDetectionBlockWillBeFree(this Dispatcher d, LayoutOccupancyBlock occupancyBlock) {
            ds_OnTrainDetectionBlockWillBeFree ??= d[nameof(OnTrainDetectionBlockWillBeFree)];
            ds_OnTrainDetectionBlockWillBeFree.CallVoid(occupancyBlock);
        }

        static DispatchSource? ds_OnOccupancyBlockEdgeCrossed = null;
        [DispatchSource]
        public static void OnOccupancyBlockEdgeCrossed(this Dispatcher d, LayoutBlockEdgeBase blockEdge, TrainStateInfo train) {
            ds_OnOccupancyBlockEdgeCrossed ??= d[nameof(OnOccupancyBlockEdgeCrossed)];
            ds_OnOccupancyBlockEdgeCrossed.CallVoid(blockEdge, train);
        }

        [DispatchSource]
        public static void AbortTrip(this Dispatcher d, TrainStateInfo train, bool emergency) {
            d[nameof(AbortTrip)].CallVoid(train, emergency);
        }

        static DispatchSource? ds_OnTrainDetectionBlockOccupied = null;
        [DispatchSource]
        public static void OnTrainDetectionBlockOccupied(this Dispatcher d, LayoutOccupancyBlock occupancyBlock) {
            ds_OnTrainDetectionBlockOccupied ??= d[nameof(OnTrainDetectionBlockOccupied)];
            ds_OnTrainDetectionBlockOccupied.CallVoid(occupancyBlock);
        }

        static DispatchSource? ds_OnTrainDetectionBlockFree = null;
        [DispatchSource]
        public static void OnTrainDetectionBlockFree(this Dispatcher d, LayoutOccupancyBlock occupancyBlock) {
            ds_OnTrainDetectionBlockFree ??= d[nameof(OnTrainDetectionBlockFree)];
            ds_OnTrainDetectionBlockFree.CallVoid(occupancyBlock);
        }

        static DispatchSource? ds_TrainLeavingBlockDetails = null;
        [DispatchSource]
        public static void TrainLeavingBlockDetails(this Dispatcher d, LayoutBlockEdgeBase blockEdge, TrainChangingBlock trainChangingBlock) {
            ds_TrainLeavingBlockDetails ??= d[nameof(TrainLeavingBlockDetails)];
            ds_TrainLeavingBlockDetails.CallVoid(blockEdge, trainChangingBlock);

        }

        // Motion manager

        [DispatchSource]
        public static void ChangeTrainSpeedRequest(this Dispatcher d, TrainStateInfo train, int speed, MotionRampInfo ramp) {
            d[nameof(ChangeTrainSpeedRequest)].CallVoid(train, speed, ramp);
        }

        [DispatchSource]
        public static void RemoveSpeedChangeState(this Dispatcher d, TrainStateInfo train) {
            d[nameof(RemoveSpeedChangeState)].CallVoid(train);
        }

        [DispatchSource]
        public static void OnNewLayoutDocument(this Dispatcher d, string filename) {
            d[nameof(OnNewLayoutDocument)].CallVoid(filename);
        }

        [DispatchSource]
        public static void ShowRoutingTableGenerationDialog(this Dispatcher d, int nTurnouts) {
            d[nameof(ShowRoutingTableGenerationDialog)].CallVoid(nTurnouts);
        }

        [DispatchSource]
        public static void AbortRoutingTableGenerationDialog(this Dispatcher d) {
            d[nameof(AbortRoutingTableGenerationDialog)].CallVoid();
        }

        [DispatchSource]
        public static void RoutingTableGenerationProgress(this Dispatcher d) {
            d[nameof(RoutingTableGenerationProgress)].CallVoid();
        }

        [DispatchSource]
        public static void RoutingTableGenerationDone(this Dispatcher d) {
            d[nameof(RoutingTableGenerationDone)].CallVoid();
        }

        static DispatchSource? ds_DispatcherRetryWaitingTrains = null;
        [DispatchSource]
        public static void DispatcherRetryWaitingTrains(this Dispatcher d, bool allTrains) {
            ds_DispatcherRetryWaitingTrains ??= d[nameof(DispatcherRetryWaitingTrains)];
            ds_DispatcherRetryWaitingTrains.CallVoid(allTrains);
        }

        static DispatchSource? ds_DriverStop = null;
        [DispatchSource]
        public static void DriverStop(this Dispatcher d, TrainStateInfo train) {
            ds_DriverStop ??= d[nameof(DriverStop)];
            ds_DriverStop.CallVoid(train);
        }


        static DispatchSource? ds_ExecuteTripPlan = null;
        [DispatchSource]
        public static bool ExecuteTripPlan(this Dispatcher d, TripPlanAssignmentInfo tripPlanAssignment) {
            ds_ExecuteTripPlan ??= d[nameof(ExecuteTripPlan)];
            return ds_ExecuteTripPlan.Call<bool>(tripPlanAssignment);
        }


        static DispatchSource? ds_DriverAssignment = null;
        [DispatchSource]
        public static bool DriverAssignment(this Dispatcher d, TrainStateInfo train) {
            ds_DriverAssignment ??= d[nameof(DriverAssignment)];
            return ds_DriverAssignment.Call<bool>(train);
        }


        static DispatchSource? ds_ShowLocomotiveController = null;
        [DispatchSource]
        public static void ShowLocomotiveController(this Dispatcher d, TrainStateInfo train) {
            ds_ShowLocomotiveController ??= d[nameof(ShowLocomotiveController)];
            ds_ShowLocomotiveController.CallVoid(train);
        }


        static DispatchSource? ds_ValidateTripPlanRoute = null;
        [DispatchSource]
        public static ITripRouteValidationResult ValidateTripPlanRoute(this Dispatcher d, TripPlanInfo tripPlan, TrainStateInfo train) {
            ds_ValidateTripPlanRoute ??= d[nameof(ValidateTripPlanRoute)];
            return ds_ValidateTripPlanRoute.Call<ITripRouteValidationResult>(tripPlan, train);
        }


        static DispatchSource? ds_OnTripAdded = null;
        [DispatchSource]
        public static void OnTripAdded(this Dispatcher d, TripPlanAssignmentInfo tripPlanAssignment) {
            ds_OnTripAdded ??= d[nameof(OnTripAdded)];
            ds_OnTripAdded.CallVoid(tripPlanAssignment);
        }


        static DispatchSource? ds_DriverEmergencyStop = null;
        [DispatchSource]
        public static void DriverEmergencyStop(this Dispatcher d, TrainStateInfo train) {
            ds_DriverEmergencyStop ??= d[nameof(DriverEmergencyStop)];
            ds_DriverEmergencyStop.CallVoid(train);
        }


        static DispatchSource? ds_OnTripDone = null;
        [DispatchSource]
        public static void OnTripDone(this Dispatcher d, TripPlanAssignmentInfo trip) {
            ds_OnTripDone ??= d[nameof(OnTripDone)];
            ds_OnTripDone.CallVoid(trip);
        }

        static DispatchSource? ds_OnTripAborted = null;
        [DispatchSource]
        public static void OnTripAborted(this Dispatcher d, TrainStateInfo train, TripPlanInfo trip) {
            ds_OnTripAborted ??= d[nameof(OnTripAborted)];
            ds_OnTripAborted.CallVoid(train, trip);
        }

        static DispatchSource? ds_SuspendTrip = null;
        [DispatchSource]
        public static void SuspendTrip(this Dispatcher d, TrainStateInfo train, bool emergency) {
            ds_SuspendTrip ??= d[nameof(SuspendTrip)];
            ds_SuspendTrip.CallVoid(train, emergency);
        }


        static DispatchSource? ds_ResumeTrip = null;
        [DispatchSource]
        public static void ResumeTrip(this Dispatcher d, TrainStateInfo train) {
            ds_ResumeTrip ??= d[nameof(ResumeTrip)];
            ds_ResumeTrip.CallVoid(train);
        }


        static DispatchSource? ds_ClearTrip = null;
        [DispatchSource]
        public static void ClearTrip(this Dispatcher d, TrainStateInfo train) {
            ds_ClearTrip ??= d[nameof(ClearTrip)];
            ds_ClearTrip.CallVoid(train);
        }

        static DispatchSource? ds_OnTripCleared = null;
        [DispatchSource]
        public static void OnTripCleared(this Dispatcher d, TrainStateInfo train, TripPlanInfo trip) {
            ds_OnTripCleared ??= d[nameof(OnTripCleared)];
            ds_OnTripCleared.CallVoid(train, trip);
        }

        static DispatchSource? ds_SuspendAllTrips = null;
        [DispatchSource]
        public static void SuspendAllTrips(this Dispatcher d) {
            ds_SuspendAllTrips ??= d[nameof(SuspendAllTrips)];
            ds_SuspendAllTrips.CallVoid();
        }

        static DispatchSource? ds_ResumeAllTrips = null;
        [DispatchSource]
        public static void ResumeAllTrips(this Dispatcher d) {
            ds_ResumeAllTrips ??= d[nameof(ResumeAllTrips)];
            ds_ResumeAllTrips.CallVoid();
        }


        static DispatchSource? ds_GetActiveTrip = null;
        [DispatchSource]
        public static TripPlanAssignmentInfo? GetActiveTrip(this Dispatcher d, TrainStateInfo train) {
            ds_GetActiveTrip ??= d[nameof(GetActiveTrip)];
            return ds_GetActiveTrip.Call<TripPlanAssignmentInfo?>(train);
        }


        static DispatchSource? ds_GetEditedTrip = null;
        [DispatchSource]
        public static TripPlanAssignmentInfo? GetEditedTrip(this Dispatcher d, TrainStateInfo train) {
            ds_GetEditedTrip ??= d[nameof(GetEditedTrip)];
            return ds_GetEditedTrip.Call<TripPlanAssignmentInfo?>(train);
        }


        static DispatchSource? ds_GetActiveTrips = null;
        [DispatchSource]
        public static IList<TripPlanAssignmentInfo> GetActiveTrips(this Dispatcher d) {
            ds_GetActiveTrips ??= d[nameof(GetActiveTrips)];
            return ds_GetActiveTrips.Call<IList<TripPlanAssignmentInfo>>();
        }

        static DispatchSource? ds_AreAllTripsSuspended = null;
        [DispatchSource]
        public static bool AreAllTripsSuspended(this Dispatcher d) {
            ds_AreAllTripsSuspended ??= d[nameof(AreAllTripsSuspended)];
            return ds_AreAllTripsSuspended.Call<bool>();
        }

        static DispatchSource? ds_IsAnyActiveTripPlan = null;
        [DispatchSource]
        public static bool IsAnyActiveTripPlan(this Dispatcher d) {
            ds_IsAnyActiveTripPlan ??= d[nameof(IsAnyActiveTripPlan)];
            return ds_IsAnyActiveTripPlan.Call<bool>();
        }

        static DispatchSource? ds_OnAllTripsSuspendedStatusChanged = null;
        [DispatchSource]
        public static void OnAllTripsSuspendedStatusChanged(this Dispatcher d, bool allSuspended) {
            ds_OnAllTripsSuspendedStatusChanged ??= d[nameof(OnAllTripsSuspendedStatusChanged)];
            ds_OnAllTripsSuspendedStatusChanged.CallVoid(allSuspended);
        }

        static DispatchSource? ds_OnTripStatusChanged = null;
        [DispatchSource]
        public static void OnTripStatusChanged(this Dispatcher d, TripPlanAssignmentInfo tripPlanAssignment, TripStatus oldStatus) {
            ds_OnTripStatusChanged ??= d[nameof(OnTripStatusChanged)];
            ds_OnTripStatusChanged.CallVoid(tripPlanAssignment, oldStatus);
        }

        static DispatchSource? ds_OnTrainSpeedChanged = null;
        [DispatchSource]
        public static void OnTrainSpeedChanged(this Dispatcher d, TrainStateInfo train, int speed) {
            ds_OnTrainSpeedChanged ??= d[nameof(OnTrainSpeedChanged)];
            ds_OnTrainSpeedChanged.CallVoid(train, speed);
        }

        static DispatchSource? ds_OnDriverTargetSpeedChanged = null;
        [DispatchSource]
        public static void OnDriverTargetSpeedChanged(this Dispatcher d, TrainStateInfo train) {
            ds_OnDriverTargetSpeedChanged ??= d[nameof(OnDriverTargetSpeedChanged)];
            ds_OnDriverTargetSpeedChanged.CallVoid(train);
        }

        static DispatchSource? ds_DriverPrepareStop = null;
        [DispatchSource]
        public static void DriverPrepareStop(this Dispatcher d, TrainStateInfo train) {
            ds_DriverPrepareStop ??= d[nameof(DriverPrepareStop)];
            ds_DriverPrepareStop.CallVoid(train);
        }

        static DispatchSource? ds_DriverUpdateSpeed = null;
        [DispatchSource]
        public static void DriverUpdateSpeed(this Dispatcher d, TrainStateInfo train) {
            ds_DriverUpdateSpeed ??= d[nameof(DriverUpdateSpeed)];
            ds_DriverUpdateSpeed.CallVoid(train);
        }

        static DispatchSource? ds_OnTrainLeaveBlock = null;
        [DispatchSource]
        public static void OnTrainLeaveBlock(this Dispatcher d, TrainStateInfo train, LayoutBlock block) {
            ds_OnTrainLeaveBlock ??= d[nameof(OnTrainLeaveBlock)];
            ds_OnTrainLeaveBlock.CallVoid(train, block);
        }

        static DispatchSource? ds_OnManualDispatchRegionActivationChanged = null;
        [DispatchSource]
        public static void OnManualDispatchRegionActivationChanged(this Dispatcher d, ManualDispatchRegionInfo manualDispatchRegion, bool active) {
            ds_OnManualDispatchRegionActivationChanged ??= d[nameof(OnManualDispatchRegionActivationChanged)];
            ds_OnManualDispatchRegionActivationChanged.CallVoid(manualDispatchRegion, active);
        }

        static DispatchSource? ds_DriverTrainGo = null;
        [DispatchSource]
        public static void DriverTrainGo(this Dispatcher d, TrainStateInfo train, LocomotiveOrientation direction) {
            ds_DriverTrainGo ??= d[nameof(DriverTrainGo)];
            ds_DriverTrainGo.CallVoid(train, direction);
        }

        static DispatchSource? ds_FindBestRouteRequest = null;
        [DispatchSource]
        public static TripBestRouteResult FindBestRouteRequest(this Dispatcher d, TripBestRouteRequest request) {
            ds_FindBestRouteRequest ??= d[nameof(FindBestRouteRequest)];
            return ds_FindBestRouteRequest.Call<TripBestRouteResult>(request);
        }

        static DispatchSource? ds_DispatcherSetSwitches = null;
        [DispatchSource]
        public static bool DispatcherSetSwitches(this Dispatcher d, Guid ownerId, ITripRoute route) {
            ds_DispatcherSetSwitches ??= d[nameof(DispatcherSetSwitches)];
            return ds_DispatcherSetSwitches.Call<bool>(ownerId, route);
        }

        static DispatchSource? ds_EnumTrainDrivers = null;
        [DispatchSource]
        public static void EnumTrainDrivers(this Dispatcher d, XmlElement driversElement) {
            ds_EnumTrainDrivers ??= d[nameof(EnumTrainDrivers)];
            ds_EnumTrainDrivers.CallVoid(driversElement);
        }

        static DispatchSource? ds_QueryDriverDialog = null;
        [DispatchSource]
        public static bool QueryDriverDialog(this Dispatcher d, XmlElement driverElement) {
            ds_QueryDriverDialog ??= d[nameof(QueryDriverDialog)];
            return ds_QueryDriverDialog.CallBoolFunctions(invokeUntil: true, invokeAll: false, driverElement);
        }

        static DispatchSource? ds_EditDriverSettings = null;
        [DispatchSource]
        public static void EditDriverSettings(this Dispatcher d, TrainCommonInfo train, XmlElement driverElement) {
            ds_EditDriverSettings ??= d[nameof(EditDriverSettings)];
            ds_EditDriverSettings.CallVoid(train, driverElement);
        }

        static DispatchSource? ds_GetTrainTargetSpeed = null;
        [DispatchSource]
        public static bool GetTrainTargetSpeed(this Dispatcher d, TrainCommonInfo train, System.Windows.Forms.IWin32Window? owner = null) {
            ds_GetTrainTargetSpeed ??= d[nameof(GetTrainTargetSpeed)];
            return ds_GetTrainTargetSpeed.Call<bool>(train, owner);
        }

        static DispatchSource? ds_GetLayoutEmulationServices = null;
        [DispatchSource]
        public static ILayoutEmulatorServices GetLayoutEmulationServices(this Dispatcher d) {
            ds_GetLayoutEmulationServices ??= d[nameof(GetLayoutEmulationServices)];
            return ds_GetLayoutEmulationServices.Call<ILayoutEmulatorServices>();
        }

        static DispatchSource? ds_InitializeLayoutEmulation = null;
        [DispatchSource]
        public static void InitializeLayoutEmulation(this Dispatcher d, bool emulateTrainMotion, int emulationTickTime) {
            ds_InitializeLayoutEmulation ??= d[nameof(InitializeLayoutEmulation)];
            ds_InitializeLayoutEmulation.CallVoid(emulateTrainMotion, emulationTickTime);
        }

        static DispatchSource? ds_ConnectPowerRequest = null;
        [DispatchSource]
        public static void ConnectPowerRequest(this Dispatcher d, IModelComponentHasNameAndId commandStation) {
            ds_ConnectPowerRequest ??= d[nameof(ConnectPowerRequest)];
            ds_ConnectPowerRequest.CallVoid(commandStation);
        }

        static DispatchSource? ds_DisconnectPowerRequest = null;
        [DispatchSource]
        public static void DisconnectPowerRequest(this Dispatcher d, IModelComponentHasNameAndId commandStation) {
            ds_DisconnectPowerRequest ??= d[nameof(DisconnectPowerRequest)];
            ds_DisconnectPowerRequest.CallVoid(commandStation);
        }

        static DispatchSource? ds_ResetLayoutEmulation = null;
        [DispatchSource]
        public static void ResetLayoutEmulation(this Dispatcher d) {
            ds_ResetLayoutEmulation ??= d[nameof(ResetLayoutEmulation)];
            ds_ResetLayoutEmulation.CallVoid();
        }

        static DispatchSource? ds_ExitOperationModeAsync = null;
        [DispatchSource]
        public static Task ExitOperationModeAsync(this Dispatcher d) {
            ds_ExitOperationModeAsync ??= d[nameof(ExitOperationModeAsync)];
            var tasks = ds_ExitOperationModeAsync.CallCollect<Task>();
            return Task.WhenAll(tasks);
        }

        static DispatchSource? ds_OnCommandStationPowerOff = null;
        [DispatchSource]
        public static void OnCommandStationPowerOff(this Dispatcher d, IModelComponentHasNameAndId commandStation) {
            ds_OnCommandStationPowerOff ??= d[nameof(OnCommandStationPowerOff)];
            ds_OnCommandStationPowerOff.CallVoid(commandStation);
        }

        static DispatchSource? ds_QueryPerformTrainsAnalysis = null;
        [DispatchSource]
        public static void QueryPerformTrainsAnalysis(this Dispatcher d, List<IModelComponentIsCommandStation> commandStations) {
            ds_QueryPerformTrainsAnalysis ??= d[nameof(QueryPerformTrainsAnalysis)];
            ds_QueryPerformTrainsAnalysis.CallVoid(commandStations);
        }

        static DispatchSource? ds_BeginDesignTimeLayoutActivation = null;
        [DispatchSource]
        public static bool BeginDesignTimeLayoutActivation(this Dispatcher d) {
            ds_BeginDesignTimeLayoutActivation ??= d[nameof(BeginDesignTimeLayoutActivation)];
            return ds_BeginDesignTimeLayoutActivation.CallBoolFunctions(invokeUntil: false, invokeAll: true);
        }

        static DispatchSource? ds_endDesignTimeLayoutActivation = null;
        [DispatchSource]
        public static bool EndDesignTimeLayoutActivation(this Dispatcher d) {
            ds_endDesignTimeLayoutActivation ??= d[nameof(EndDesignTimeLayoutActivation)];
            return ds_endDesignTimeLayoutActivation.CallBoolFunctions(invokeUntil: false, invokeAll: true);
        }

        static DispatchSource? ds_OnCommandStationTrainAnalysisPhaseDone = null;
        [DispatchSource]
        public static void OnCommandStationTrainAnalysisPhaseDone(this Dispatcher d) {
            ds_OnCommandStationTrainAnalysisPhaseDone ??= d[nameof(OnCommandStationTrainAnalysisPhaseDone)];
            ds_OnCommandStationTrainAnalysisPhaseDone.CallVoid();
        }

        static DispatchSource? ds_ProgramCvPomRequest = null;
        [DispatchSource]
        public static Task<LayoutActionResult> ProgramCvPomRequest(this Dispatcher d, IModelComponentHasNameAndId commandStation, DccProgrammingCV cv, DccDecoderTypeInfo decoderTypeInfo, int address) {
            ds_ProgramCvPomRequest ??= d[nameof(ProgramCvPomRequest)];
            return ds_ProgramCvPomRequest.Call<Task<LayoutActionResult>>(commandStation, cv, decoderTypeInfo, address);
        }

        static DispatchSource? ds_ProgramCvDirectRequest = null;
        [DispatchSource]
        public static Task<LayoutActionResult> ProgramCvDirectRequest(this Dispatcher d, IModelComponentHasNameAndId commandStation, DccProgrammingCV cv, DccDecoderTypeInfo decoderTypeInfo) {
            ds_ProgramCvDirectRequest ??= d[nameof(ProgramCvDirectRequest)];
            return ds_ProgramCvDirectRequest.Call<Task<LayoutActionResult>>(commandStation, cv, decoderTypeInfo);
        }

        static DispatchSource? ds_ProgramCvRegisterRequest = null;
        [DispatchSource]
        public static Task<LayoutActionResult> ProgramCvRegisterRequest(this Dispatcher d, IModelComponentHasNameAndId commandStation, DccProgrammingCV cv, DccDecoderTypeInfo decoderTypeInfo) {
            ds_ProgramCvRegisterRequest ??= d[nameof(ProgramCvRegisterRequest)];
            return ds_ProgramCvRegisterRequest.Call<Task<LayoutActionResult>>(commandStation, cv, decoderTypeInfo);
        }

        static DispatchSource? ds_GetCommandStationSetFunctionNumberSupport = null;
        [DispatchSource]
        public static CommandStationSetFunctionNumberSupportInfo? GetCommandStationSetFunctionNumberSupport(this Dispatcher d, IModelComponentHasNameAndId commandStation) {
            ds_GetCommandStationSetFunctionNumberSupport ??= d[nameof(GetCommandStationSetFunctionNumberSupport)];
            return ds_GetCommandStationSetFunctionNumberSupport.CallNullable<CommandStationSetFunctionNumberSupportInfo>(commandStation);
        }

        static DispatchSource? ds_GetCommandStationCapabilities = null;
        [DispatchSource]
        public static XmlElement GetCommandStationCapabilities(this Dispatcher d, IModelComponentHasNameAndId commandStation) {
            ds_GetCommandStationCapabilities ??= d[nameof(GetCommandStationCapabilities)];
            return ds_GetCommandStationCapabilities.Call<XmlElement>(commandStation);
        }

        static DispatchSource? ds_GetImageFromCache = null;
        [DispatchSource]
        public static Image GetImageFromCache(this Dispatcher d, object requestor, string imageFilename, RotateFlipType effect) {
            ds_GetImageFromCache ??= d[nameof(GetImageFromCache)];
            return ds_GetImageFromCache.Call<Image>(requestor, imageFilename, effect);
        }

        static DispatchSource? ds_RemoveImageFromCache = null;
        [DispatchSource]
        public static void RemoveImageFromCache(this Dispatcher d, string imageFilename, RotateFlipType effect) {
            ds_RemoveImageFromCache ??= d[nameof(RemoveImageFromCache)];
            ds_RemoveImageFromCache.CallVoid(imageFilename, effect);
        }

        static DispatchSource? ds_ClearImageCache = null;
        [DispatchSource]
        public static void ClearImageCache(this Dispatcher d) {
            ds_ClearImageCache ??= d[nameof(ClearImageCache)];
            ds_ClearImageCache.CallVoid();
        }

        static DispatchSource? ds_FreeResources = null;
        [DispatchSource]
        public static void FreeResources(this Dispatcher d) {
            ds_FreeResources ??= d[nameof(FreeResources)];
            ds_FreeResources.CallVoid();
        }

        static DispatchSource? ds_OpenGateRequest = null;
        [DispatchSource]
        public static void OpenGateRequest(this Dispatcher d, LayoutGateComponent gate) {
            ds_OpenGateRequest ??= d[nameof(OpenGateRequest)];
            ds_OpenGateRequest.CallVoid(gate);
        }

        static DispatchSource? ds_CloseGateRequest = null;
        [DispatchSource]
        public static void CloseGateRequest(this Dispatcher d, LayoutGateComponent gate) {
            ds_CloseGateRequest ??= d[nameof(CloseGateRequest)];
            ds_CloseGateRequest.CallVoid(gate);
        }

        static DispatchSource? ds_OnGateIsOpen = null;
        [DispatchSource]
        public static void OnGateIsOpen(this Dispatcher d, LayoutGateComponent gate) {
            ds_OnGateIsOpen ??= d[nameof(OnGateIsOpen)];
            ds_OnGateIsOpen.CallVoid(gate);
        }

        static DispatchSource? ds_OnGateIsClosed = null;
        [DispatchSource]
        public static void OnGateIsClosed(this Dispatcher d, LayoutGateComponent gate) {
            ds_OnGateIsClosed ??= d[nameof(OnGateIsClosed)];
            ds_OnGateIsClosed.CallVoid(gate);
        }

        static DispatchSource? ds_OnGateOpenTimeout = null;
        [DispatchSource]
        public static void OnGateOpenTimeout(this Dispatcher d, LayoutGateComponent gate) {
            ds_OnGateOpenTimeout ??= d[nameof(OnGateOpenTimeout)];
            ds_OnGateOpenTimeout.CallVoid(gate);
        }

        static DispatchSource? ds_OnGateCloseTimeout = null;
        [DispatchSource]
        public static void OnGateCloseTimeout(this Dispatcher d, LayoutGateComponent gate) {
            ds_OnGateCloseTimeout ??= d[nameof(OnGateCloseTimeout)];
            ds_OnGateCloseTimeout.CallVoid(gate);
        }

        // FrameWindow

        static DispatchSource? ds_EnsureComponentVisible = null;

        [DispatchSource]
        public static void EnsureComponentVisible(this Dispatcher d, Guid frameWindowId, ModelComponent component, bool markComponent = false) {
            ds_EnsureComponentVisible ??= d[nameof(EnsureComponentVisible)];
            ds_EnsureComponentVisible.CallVoid(frameWindowId, component, markComponent);
        }

        static DispatchSource? ds_ShowMarker = null;
        [DispatchSource]
        public static void ShowMarker(this Dispatcher d, Guid frameWindowId, object markedObject) {
            ds_ShowMarker ??= d[nameof(ShowMarker)];
            ds_ShowMarker.CallVoid(frameWindowId, markedObject);
        }

        static DispatchSource? ds_DeselectControlObjects = null;
        [DispatchSource]
        public static void DeselectControlObjects(this Dispatcher d, Guid frameWindowId) {
            ds_DeselectControlObjects ??= d[nameof(DeselectControlObjects)];
            ds_DeselectControlObjects.CallVoid(frameWindowId);
        }

        static DispatchSource? ds_ShowControlModule = null;
        [DispatchSource]
        public static void ShowControlModule(this Dispatcher d, Guid frameWindowId, ControlModuleReference controlModule) {
            ds_ShowControlModule ??= d[nameof(ShowControlModule)];
            ds_ShowControlModule.CallVoid(frameWindowId, controlModule);
        }

        static DispatchSource? ds_ShowControlConnectionPoint = null;
        [DispatchSource]
        public static void ShowControlConnectionPoint(this Dispatcher d, Guid frameWindowId, ControlConnectionPointReference connectionPointReference) {
            ds_ShowControlConnectionPoint ??= d[nameof(ShowControlConnectionPoint)];
            ds_ShowControlConnectionPoint.CallVoid(frameWindowId, connectionPointReference);
        }

        [DispatchSource]
        public static void ShowMessages(this Dispatcher d, Guid frameWindowId) {
            d[nameof(ShowMessages)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void HideMessages(this Dispatcher d, Guid frameWindowId) {
            d[nameof(HideMessages)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnMessagesShown(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnMessagesShown)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnMessagesHidden(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnMessagesHidden)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void ShowTripsMonitor(this Dispatcher d, Guid frameWindowId) {
            d[nameof(ShowTripsMonitor)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void HideTripsMonitor(this Dispatcher d, Guid frameWindowId) {
            d[nameof(HideTripsMonitor)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnTripsMonitorShown(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnTripsMonitorShown)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnTripsMonitorHidden(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnTripsMonitorHidden)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void ShowLocomotives(this Dispatcher d, Guid frameWindowId) {
            d[nameof(ShowLocomotives)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void HideLocomotives(this Dispatcher d, Guid frameWindowId) {
            d[nameof(HideLocomotives)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnLocomotivesShown(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnLocomotivesShown)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnLocomotivesHidden(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnLocomotivesHidden)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void ShowLayoutControl(this Dispatcher d, Guid frameWindowId) {
            d[nameof(ShowLayoutControl)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void HideLayoutControl(this Dispatcher d, Guid frameWindowId) {
            d[nameof(HideLayoutControl)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnLayoutControlShown(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnLayoutControlShown)].CallVoid(frameWindowId);
        }

        [DispatchSource]
        public static void OnLayoutControlHidden(this Dispatcher d, Guid frameWindowId) {
            d[nameof(OnLayoutControlHidden)].CallVoid(frameWindowId);
        }
    }
}
