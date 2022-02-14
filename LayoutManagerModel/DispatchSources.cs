using System;
using System.Xml;
using Microsoft.Win32.SafeHandles;

using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LayoutManager {
    static public class ModelDispatchSources {
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
            return d[nameof(GetAction)].CallUtilNotNull<LayoutAction>(actionElement, owner);
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
        public static void OnTrainSpeedChanged(this Dispatcher d, TrainStateInfo train, int speed) {
            d[nameof(OnTrainSpeedChanged)].CallVoid(train, speed);
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
    }
}
