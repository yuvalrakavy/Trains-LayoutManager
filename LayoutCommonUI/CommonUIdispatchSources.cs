
using System.Xml;
using LayoutManager.CommonUI;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Components;
using LayoutManager.Model;
using LayoutManager.View;
using MethodDispatcher;

namespace LayoutManager {
    public static class CommonUIDispatchSources {
        public static void Initialize() { }

        static DispatchSource? ds_AddControlModuleEditingContextMenuEntries = null;
        [DispatchSource]
        public static void AddControlModuleEditingContextMenuEntries(this Dispatcher d, DrawControlBase module, MenuOrMenuItem menu) {
            ds_AddControlModuleEditingContextMenuEntries ??= d[nameof(AddControlModuleEditingContextMenuEntries)];
            ds_AddControlModuleEditingContextMenuEntries.CallVoid(module, menu);
        }

        static DispatchSource? ds_AddControlModuleOperationContextMenuEntries = null;
        [DispatchSource]
        public static void AddControlModuleOperationContextMenuEntries(this Dispatcher d, DrawControlBase module, MenuOrMenuItem menu) {
            ds_AddControlModuleOperationContextMenuEntries ??= d[nameof(AddControlModuleOperationContextMenuEntries)];
            ds_AddControlModuleOperationContextMenuEntries.CallVoid(module, menu);
        }

        static DispatchSource? ds_ControlModuleDefaultAction = null;
        [DispatchSource]
        public static void ControlModuleDefaultAction(this Dispatcher d, DrawControlBase module) {
            ds_ControlModuleDefaultAction ??= d[nameof(ControlModuleDefaultAction)];
            ds_ControlModuleDefaultAction.CallVoid(module);
        }

        static DispatchSource? ds_QueryTestLayoutObject = null;
        [DispatchSource]
        public static bool QueryTestLayoutObject(this Dispatcher d) {
            ds_QueryTestLayoutObject ??= d[nameof(QueryTestLayoutObject)];
            return ds_QueryTestLayoutObject.CallBoolFunctions(invokeUntil: true, invokeAll: false);
        }

        static DispatchSource? ds_TestLayoutObjectRequest = null;
        [DispatchSource]
        public static void TestLayoutObjectRequest(this Dispatcher d, Guid frameWindowId, object testedObject) {
            ds_TestLayoutObjectRequest ??= d[nameof(TestLayoutObjectRequest)];
            ds_TestLayoutObjectRequest.CallVoid(frameWindowId, testedObject);
        }

        static DispatchSource? ds_GetTestComponentDialog = null;
        [DispatchSource]
        public static Form? GetTestComponentDialog(this Dispatcher d, Guid frameWindowId, object testedObject) {
            ds_GetTestComponentDialog ??= d[nameof(GetTestComponentDialog)];
            return ds_GetTestComponentDialog.CallUntilNotNull<Form>(frameWindowId, testedObject);
        }

        static DispatchSource? ds_AddControlEditingContextMenuEntries = null;
        [DispatchSource]
        public static void AddControlEditingContextMenuEntries(this Dispatcher d, DrawControlBase module) {
            ds_AddControlEditingContextMenuEntries ??= d[nameof(AddControlEditingContextMenuEntries)];
            ds_AddControlEditingContextMenuEntries.CallVoid(module);
        }

        static DispatchSource? ds_AddContextMenuTopEntries = null;
        [DispatchSource]
        public static void AddContextMenuTopEntries(this Dispatcher d, Guid frameWindowId, ModelComponent component, MenuOrMenuItem menu) {
            ds_AddContextMenuTopEntries ??= d[nameof(AddContextMenuTopEntries)];
            ds_AddContextMenuTopEntries.CallVoid(frameWindowId, component, menu);
        }

        static DispatchSource? ds_AddComponentContextMenuEntries = null;
        [DispatchSource]
        public static void AddComponentContextMenuEntries(this Dispatcher d, Guid frameWindowId, ModelComponent component, MenuOrMenuItem menu) {
            ds_AddComponentContextMenuEntries ??= d[nameof(AddComponentContextMenuEntries)];
            ds_AddComponentContextMenuEntries.CallVoid(frameWindowId, component, menu);
        }

        static DispatchSource? ds_AddCommonContextMenuEntries = null;
        [DispatchSource]
        public static void AddCommonContextMenuEntries(this Dispatcher d, LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            ds_AddCommonContextMenuEntries ??= d[nameof(AddCommonContextMenuEntries)];
            ds_AddCommonContextMenuEntries.CallVoid(hitTestResult, menu);
        }

        static DispatchSource? ds_AddComponentInModuleLocationContextMenuEntries = null;
        [DispatchSource]
        public static void AddComponentInModuleLocationContextMenuEntries(this Dispatcher d, Guid frameWindowId, ModelComponent component, Guid moduleLocationId, MenuOrMenuItem menu) {
            ds_AddComponentInModuleLocationContextMenuEntries ??= d[nameof(AddComponentInModuleLocationContextMenuEntries)];
            ds_AddComponentInModuleLocationContextMenuEntries.CallVoid(frameWindowId, component, moduleLocationId, menu);
        }

        static DispatchSource? ds_ToolsMenuOpenRequest = null;
        [DispatchSource]
        public static void ToolsMenuOpenRequest(this Dispatcher d, MenuOrMenuItem menu) {
            ds_ToolsMenuOpenRequest ??= d[nameof(ToolsMenuOpenRequest)];
            ds_ToolsMenuOpenRequest.CallVoid(menu);
        }

        static DispatchSource? ds_AddSelectionMenuEntries = null;
        [DispatchSource]
        public static void AddSelectionMenuEntries(this Dispatcher d, LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            ds_AddSelectionMenuEntries ??= d[nameof(AddSelectionMenuEntries)];
            ds_AddSelectionMenuEntries.CallVoid(hitTestResult, menu);
        }

        static DispatchSource? ds_AddComponentContextEmptySpotEntries = null;
        [DispatchSource]
        public static void AddComponentContextEmptySpotEntries(this Dispatcher d, LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            ds_AddComponentContextEmptySpotEntries ??= d[nameof(AddComponentContextEmptySpotEntries)];
            ds_AddComponentContextEmptySpotEntries.CallVoid(hitTestResult, menu);
        }

        static DispatchSource? ds_IncludeInComponentContextMenu = null;
        [DispatchSource]
        public static bool IncludeInComponentContextMenu(this Dispatcher d, ModelComponent component) {
            ds_IncludeInComponentContextMenu ??= d[nameof(IncludeInComponentContextMenu)];
            return ds_IncludeInComponentContextMenu.CallBoolFunctions(invokeUntil: true, invokeAll: false, component);
        }

        static DispatchSource? ds_GetEventScriptEditorTreeNode = null;
        [DispatchSource]
        public static LayoutEventScriptEditorTreeNode GetEventScriptEditorTreeNode(this Dispatcher d, XmlElement element) {
            ds_GetEventScriptEditorTreeNode ??= d[nameof(GetEventScriptEditorTreeNode)];
            return ds_GetEventScriptEditorTreeNode.Call<LayoutEventScriptEditorTreeNode>(element);
        }

        static DispatchSource? ds_EditEventScriptElement = null;
        [DispatchSource]
        public static void EditEventScriptElement(this Dispatcher d, XmlElement element, IEventScriptEditorSite site) {
            ds_EditEventScriptElement ??= d[nameof(EditEventScriptElement)];
            ds_EditEventScriptElement.CallVoid(element, site);
        }

        static DispatchSource? ds_GetEventScriptEditorEventContainerMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditorEventContainerMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditorEventContainerMenu ??= d[nameof(GetEventScriptEditorEventContainerMenu)];
            ds_GetEventScriptEditorEventContainerMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_GetEventScriptEditorInsertEventConatinerMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditorInsertEventConatinerMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditorInsertEventConatinerMenu ??= d[nameof(GetEventScriptEditorInsertEventConatinerMenu)];
            ds_GetEventScriptEditorInsertEventConatinerMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_GetEventScriptEditorEventMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditorEventMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditorEventMenu ??= d[nameof(GetEventScriptEditorEventMenu)];
            ds_GetEventScriptEditorEventMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_GetEventScriptEditorEventSectionMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditorEventSectionMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditorEventSectionMenu ??= d[nameof(GetEventScriptEditorEventSectionMenu)];
            ds_GetEventScriptEditorEventSectionMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_GetEventScriptEditorRandomChoiceMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditorRandomChoiceMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditorRandomChoiceMenu ??= d[nameof(GetEventScriptEditorRandomChoiceMenu)];
            ds_GetEventScriptEditorRandomChoiceMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_GetEventScriptEditorConditionSectionMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditorConditionSectionMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditorConditionSectionMenu ??= d[nameof(GetEventScriptEditorConditionSectionMenu)];
            ds_GetEventScriptEditorConditionSectionMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_GetEventScriptEditorInsertConditionContainerMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditorInsertConditionContainerMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditorInsertConditionContainerMenu ??= d[nameof(GetEventScriptEditorInsertConditionContainerMenu)];
            ds_GetEventScriptEditorInsertConditionContainerMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_GetEventScriptEditoActionsSectionMenu = null;
        [DispatchSource]
        public static void GetEventScriptEditoActionsSectionMenu(this Dispatcher d, LayoutEventScriptEditorTreeNode parentNode, bool hasBlockDefinition, MenuOrMenuItem menu) {
            ds_GetEventScriptEditoActionsSectionMenu ??= d[nameof(GetEventScriptEditoActionsSectionMenu)];
            ds_GetEventScriptEditoActionsSectionMenu.CallVoid(parentNode, hasBlockDefinition, menu);
        }

        static DispatchSource? ds_AddContextSymbolsAndTypes = null;
        [DispatchSource]
        public static void AddContextSymbolsAndTypes(this Dispatcher d, Dictionary<string, Type> nameToTypeMap) {
            ds_AddContextSymbolsAndTypes ??= d[nameof(AddContextSymbolsAndTypes)];
            ds_AddContextSymbolsAndTypes.CallVoid(nameToTypeMap);
        }

        static DispatchSource? ds_GetObjectAttributes = null;
        [DispatchSource]
        public static void GetObjectAttributes(this Dispatcher d, List<AttributesInfo> attributeList, Type attribueType) {
            ds_GetObjectAttributes ??= d[nameof(GetObjectAttributes)];
            ds_GetObjectAttributes.CallVoid(attribueType, attributeList);
        }

        static DispatchSource? ds_ShowControlModuleLocation = null;
        [DispatchSource]
        public static void ShowControlModuleLocation(this Dispatcher d, LayoutControlModuleLocationComponent component) {
            ds_ShowControlModuleLocation ??= d[nameof(ShowControlModuleLocation)];
            ds_ShowControlModuleLocation.CallVoid(component);
        }

        static DispatchSource? ds_GetFrameWindowId = null;
        [DispatchSource]
        public static Guid GetFrameWindowId(this Dispatcher d, Control control) {
            ds_GetFrameWindowId ??= d[nameof(GetFrameWindowId)];
            return ds_GetFrameWindowId.Call<Guid>(control);
        }


        static DispatchSource? ds_GetDefaultLengthUnit = null;
        [DispatchSource]
        public static string? GetDefaultLengthUnit(this Dispatcher d) {
            ds_GetDefaultLengthUnit ??= d[nameof(GetDefaultLengthUnit)];
            return ds_GetDefaultLengthUnit.Call<string?>();
        }

        static DispatchSource? ds_DisableLocomotiveListUpdate = null;
        [DispatchSource]
        public static void DisableLocomotiveListUpdate(this Dispatcher d) {
            ds_DisableLocomotiveListUpdate ??= d[nameof(DisableLocomotiveListUpdate)];
            ds_DisableLocomotiveListUpdate.CallVoid();
        }

        static DispatchSource? ds_EnableLocomotiveListUpdate = null;
        [DispatchSource]
        public static void EnableLocomotiveListUpdate(this Dispatcher d) {
            ds_EnableLocomotiveListUpdate ??= d[nameof(EnableLocomotiveListUpdate)];
            ds_EnableLocomotiveListUpdate.CallVoid();
        }

        static DispatchSource? ds_OnEnteredDesignMode = null;
        [DispatchSource]
        public static void OnEnteredDesignMode(this Dispatcher d) {
            ds_OnEnteredDesignMode ??= d[nameof(OnEnteredDesignMode)];
            ds_OnEnteredDesignMode.CallVoid();
        }

        static DispatchSource? ds_OnTrainSavedInCollection = null;
        [DispatchSource]
        public static void OnTrainSavedInCollection(this Dispatcher d, TrainInCollectionInfo trainInCollection) {
            ds_OnTrainSavedInCollection ??= d[nameof(OnTrainSavedInCollection)];
            ds_OnTrainSavedInCollection.CallVoid(trainInCollection);
        }

        static DispatchSource? ds_OnPolicyUpdated = null;
        [DispatchSource]
        public static void OnPolicyUpdated(this Dispatcher d, LayoutPolicyInfo policy, LayoutPoliciesCollection policiesCollection) {
            ds_OnPolicyUpdated ??= d[nameof(OnPolicyUpdated)];
            ds_OnPolicyUpdated.CallVoid(policy, policiesCollection);
        }

        static DispatchSource? ds_GetEventScriptOperatorName = null;
        [DispatchSource]
        public static string GetEventScriptOperatorName(this Dispatcher d, XmlElement element) {
            ds_GetEventScriptOperatorName ??= d[nameof(GetEventScriptOperatorName)];
            return ds_GetEventScriptOperatorName.Call<string>(element);
        }

        static DispatchSource? ds_GetConnectionPointTypeImage = null;
        [DispatchSource]
        public static Image? GetConnectionPointTypeImage(this Dispatcher d, string connectionPointType, bool topRow) {
            ds_GetConnectionPointTypeImage ??= d[nameof(GetConnectionPointTypeImage)];
            return ds_GetConnectionPointTypeImage.Call<Image?>(connectionPointType, topRow);
        }

        static DispatchSource? ds_GetConnectionPointComponentImage = null;
        [DispatchSource]
        public static Image? GetConnectionPointComponentImage(this Dispatcher d, IModelComponentConnectToControl component, ControlModule module, bool topRow, int index) {
            ds_GetConnectionPointComponentImage ??= d[nameof(GetConnectionPointComponentImage)];
            return ds_GetConnectionPointComponentImage.Call<Image?>(component, module, topRow, index);
        }

        static DispatchSource? ds_GetComponentToControlConnect = null;
        [DispatchSource]
        public static ControlConnectionPointDestination? GetComponentToControlConnect(this Dispatcher d) {
            ds_GetComponentToControlConnect ??= d[nameof(GetComponentToControlConnect)];
            return ds_GetComponentToControlConnect.CallNullable<ControlConnectionPointDestination>();
        }

        static DispatchSource? ds_GetContextSymbolInfoType = null;
        [DispatchSource]
        public static Type? GetContextSymbolInfoType(this Dispatcher d, Type symbolType) {
            ds_GetContextSymbolInfoType ??= d[nameof(GetContextSymbolInfoType)];
            return ds_GetContextSymbolInfoType.Call<Type?>(symbolType);
        }

        static DispatchSource? ds_DefaultActionCommand = null;
        [DispatchSource]
        public static void DefaultActionCommand(this Dispatcher d, ModelComponent component, LayoutHitTestResult hitTestResult) {
            ds_DefaultActionCommand ??= d[nameof(DefaultActionCommand)];
            ds_DefaultActionCommand.CallVoid(component, hitTestResult);
        }

        static DispatchSource? ds_QueryBindSignalsDialogs = null;
        [DispatchSource]
        public static void QueryBindSignalsDialogs(this Dispatcher d, LayoutBlockEdgeBase? blockEdge, List<IModelComponentReceiverDialog> dialogs) {
            ds_QueryBindSignalsDialogs ??= d[nameof(QueryBindSignalsDialogs)];
            ds_QueryBindSignalsDialogs.CallVoid(blockEdge, dialogs);        
        }

        static DispatchSource? ds_PrepareForComponentRemoveCommand = null;
        [DispatchSource]
        public static void PrepareForComponentRemoveCommand(this Dispatcher d, ModelComponent component, ILayoutCompoundCommand command) {
            ds_PrepareForComponentRemoveCommand ??= d[nameof(PrepareForComponentRemoveCommand)];
            ds_PrepareForComponentRemoveCommand.CallVoid(component, command);
        }

        static DispatchSource? ds_GetCommandStationNotificationDialog = null;
        [DispatchSource]
        public static Form? GetCommandStationNotificationDialog(this Dispatcher d, IModelComponentIsCommandStation commandStation) {
            ds_GetCommandStationNotificationDialog ??= d[nameof(GetCommandStationNotificationDialog)];
            return ds_GetCommandStationNotificationDialog.CallUntilNotNull<Form>(commandStation);
        }

        static DispatchSource? ds_UpdateCommandStationNotificationDialog = null;
        [DispatchSource]
        public static void UpdateCommandStationNotificationDialog(this Dispatcher d, IModelComponentIsCommandStation commandStation, string reason) {
            ds_UpdateCommandStationNotificationDialog ??= d[nameof(UpdateCommandStationNotificationDialog)];
            ds_UpdateCommandStationNotificationDialog.CallVoid(commandStation, reason);
        }

        static DispatchSource? ds_CancelEmergencyStopRequest = null;
        [DispatchSource]
        public static void CancelEmergencyStopRequest(this Dispatcher d, IModelComponentIsCommandStation? commandStation) {
            ds_CancelEmergencyStopRequest ??= d[nameof(CancelEmergencyStopRequest)];
            ds_CancelEmergencyStopRequest.CallVoid(commandStation);
        }

        static DispatchSource? ds_GetComponentPropertiesMenuName = null;
        [DispatchSource]
        public static string? GetComponentPropertiesMenuName(this Dispatcher d, ModelComponent component) {
            ds_GetComponentPropertiesMenuName ??= d[nameof(GetComponentPropertiesMenuName)];
            return ds_GetComponentPropertiesMenuName.CallUntilNotNull<string>(component);
        }

        static DispatchSource? ds_QueryEditTripPlanDialog = null;
        [DispatchSource]
        public static void QueryEditTripPlanDialog(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, List<ITripPlanEditorDialog> dialogs) {
            ds_QueryEditTripPlanDialog ??= d[nameof(QueryEditTripPlanDialog)];
            ds_QueryEditTripPlanDialog.CallVoid(blockDefinition, dialogs);
        }

        static DispatchSource? ds_QueryExecuteTripPlanDialog = null;
        [DispatchSource]
        public static void QueryExecuteTripPlanDialog(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, List<IModelComponentReceiverDialog> dialogs) {
            ds_QueryExecuteTripPlanDialog ??= d[nameof(QueryExecuteTripPlanDialog)];
            ds_QueryExecuteTripPlanDialog.CallVoid(blockDefinition, dialogs);
        }

        static DispatchSource? ds_QueryEditDestinationDialog = null;
        [DispatchSource]
        public static void QueryEditDestinationDialog(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, List<IModelComponentReceiverDialog> dialogs) {
            ds_QueryEditDestinationDialog ??= d[nameof(QueryEditDestinationDialog)];
            ds_QueryEditDestinationDialog.CallVoid(blockDefinition, dialogs);
        }

        static DispatchSource? ds_QueryEditManualDispatchRegionDialog = null;
        [DispatchSource]
        public static void QueryEditManualDispatchRegionDialog(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, List<IModelComponentReceiverDialog> dialogs) {
            ds_QueryEditManualDispatchRegionDialog ??= d[nameof(QueryEditManualDispatchRegionDialog)];
            ds_QueryEditManualDispatchRegionDialog.CallVoid(blockDefinition, dialogs);
        }

        static DispatchSource? ds_GetExtendTrainMenu = null;
        [DispatchSource]
        public static LayoutMenuItem? GetExtendTrainMenu(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition) {
            ds_GetExtendTrainMenu ??= d[nameof(GetExtendTrainMenu)];
            return ds_GetExtendTrainMenu.Call<LayoutMenuItem?>(blockDefinition);
        }

        static DispatchSource? ds_AddToTrainOperationMenu = null;
        [DispatchSource]
        public static void AddToTrainOperationMenu(this Dispatcher d, TrainStateInfo train, MenuOrMenuItem menu) {
            ds_AddToTrainOperationMenu ??= d[nameof(AddToTrainOperationMenu)];
            ds_AddToTrainOperationMenu.CallVoid(train, menu);
        }

        static DispatchSource? ds_GetSmartDestinationList = null;
        [DispatchSource]
        public static void GetSmartDestinationList(this Dispatcher d, LayoutBlockDefinitionComponent blockDefinition, List<TripPlanDestinationInfo> destinations) {
            ds_GetSmartDestinationList ??= d[nameof(GetSmartDestinationList)];
            ds_GetSmartDestinationList.CallVoid(blockDefinition, destinations);
        }

        static DispatchSource? ds_AddTrainDetailsWindowSection = null;
        [DispatchSource]
        public static void AddTrainDetailsWindowSection(this Dispatcher d, TrainStateInfo train, PopupWindowContainerSection container) {
            ds_AddTrainDetailsWindowSection ??= d[nameof(AddTrainDetailsWindowSection)];
            ds_AddTrainDetailsWindowSection.CallVoid(train, container);
        }

        static DispatchSource? ds_QueryEditTripPlanForTrain = null;
        [DispatchSource]
        public static Form? QueryEditTripPlanForTrain(this Dispatcher d, TrainStateInfo train) {
            ds_QueryEditTripPlanForTrain ??= d[nameof(QueryEditTripPlanForTrain)];
            return ds_QueryEditTripPlanForTrain.CallUntilNotNull<Form>(train);
        }

        static DispatchSource? ds_ShowSavedTripPlansForTrain = null;
        [DispatchSource]
        public static bool ShowSavedTripPlansForTrain(this Dispatcher d, TrainStateInfo train) {
            ds_ShowSavedTripPlansForTrain ??= d[nameof(ShowSavedTripPlansForTrain)];
            return ds_ShowSavedTripPlansForTrain.CallBoolFunctions(invokeUntil: true, invokeAll: false, train);
        }

        static DispatchSource? ds_SaveTripPlan = null;
        [DispatchSource]
        public static void SaveTripPlan(this Dispatcher d, TripPlanInfo tripPlan, Form? parentForm) {
            ds_SaveTripPlan ??= d[nameof(SaveTripPlan)];
            ds_SaveTripPlan.CallVoid(tripPlan, parentForm);
        }

        static DispatchSource? ds_ActivateLocomotiveController = null;
        [DispatchSource]
        public static Form? ActivateLocomotiveController(this Dispatcher d, TrainStateInfo train) {
            ds_ActivateLocomotiveController ??= d[nameof(ActivateLocomotiveController)];
            return ds_ActivateLocomotiveController.CallUntilNotNull<Form>(train);
        }

        static DispatchSource? ds_OnTrainConfigurationChanged = null;
        [DispatchSource]
        public static void OnTrainConfigurationChanged(this Dispatcher d, TrainCommonInfo train) {
            ds_OnTrainConfigurationChanged ??= d[nameof(OnTrainConfigurationChanged)];
            ds_OnTrainConfigurationChanged.CallVoid(train);
        }

        static DispatchSource? ds_AddLocomotiveControllerFunctionMenuEntries = null;
        [DispatchSource]
        public static void AddLocomotiveControllerFunctionMenuEntries(this Dispatcher d, TrainStateInfo train, MenuOrMenuItem menu) {
            ds_AddLocomotiveControllerFunctionMenuEntries ??= d[nameof(AddLocomotiveControllerFunctionMenuEntries)];
            ds_AddLocomotiveControllerFunctionMenuEntries.CallVoid(train, menu);
        }

        static DispatchSource? ds_EditTrainProperties = null;
        [DispatchSource]
        public static void EditTrainProperties(this Dispatcher d, TrainStateInfo train) {
            ds_EditTrainProperties ??= d[nameof(EditTrainProperties)];
            ds_EditTrainProperties.CallVoid(train);
        }

        static DispatchSource? ds_QueryLearnLayoutPickComponentDialog = null;
        [DispatchSource]
        public static void QueryLearnLayoutPickComponentDialog(this Dispatcher d, IModelComponentConnectToControl component, List<IControlConnectionPointDestinationReceiverDialog> dialogs) {
            ds_QueryLearnLayoutPickComponentDialog ??= d[nameof(QueryLearnLayoutPickComponentDialog)];
            ds_QueryLearnLayoutPickComponentDialog.CallVoid(component, dialogs);
        }

        static DispatchSource? ds_AddNewLocomotiveToCollection = null;
        [DispatchSource]
        public static void AddNewLocomotiveToCollection(this Dispatcher d) {
            ds_AddNewLocomotiveToCollection ??= d[nameof(AddNewLocomotiveToCollection)];
            ds_AddNewLocomotiveToCollection.CallVoid();
        }

        static DispatchSource? ds_AddNewTrainToCollection = null;
        [DispatchSource]
        public static void AddNewTrainToCollection(this Dispatcher d) {
            ds_AddNewTrainToCollection ??= d[nameof(AddNewTrainToCollection)];
            ds_AddNewTrainToCollection.CallVoid();
        }

        static DispatchSource? ds_ConnectComponentToControlModuleAddress = null;
        [DispatchSource]
        public static ControlConnectionPoint? ConnectComponentToControlModuleAddress(this Dispatcher d, ControlConnectionPointDestination connectionDestination, CommandStationInputEvent csEvent) {
            ds_ConnectComponentToControlModuleAddress ??= d[nameof(ConnectComponentToControlModuleAddress)];
            return ds_ConnectComponentToControlModuleAddress.Call<ControlConnectionPoint?>(connectionDestination, csEvent);
        }

        static DispatchSource? ds_EditLocomotiveProperties = null;
        [DispatchSource]
        public static void EditLocomotiveProperties(this Dispatcher d, LocomotiveInfo locomotive) {
            ds_EditLocomotiveProperties ??= d[nameof(EditLocomotiveProperties)];
            ds_EditLocomotiveProperties.CallVoid(locomotive);
        }


        static DispatchSource? ds_ClickToAddTrainDetector = null;
        [DispatchSource]
        public static void ClickToAddTrainDetector(this Dispatcher d, DrawControlClickToAddModule drawObject) {
            ds_ClickToAddTrainDetector ??= d[nameof(ClickToAddTrainDetector)];
            ds_ClickToAddTrainDetector.CallVoid(drawObject);
        }

        static DispatchSource? ds_ClickToAddControlModule = null;
        [DispatchSource]
        public static void ClickToAddControlModule(this Dispatcher d, DrawControlClickToAddModule drawObject) {
            ds_ClickToAddControlModule ??= d[nameof(ClickToAddControlModule)];
            ds_ClickToAddControlModule.CallVoid(drawObject);
        }

        [DispatchSource]
        public static void ClickToAdd(this Dispatcher d, string clickToAddHandlerName, DrawControlClickToAddModule drawObject) {
            d[clickToAddHandlerName].CallVoid(drawObject);
        }

        static DispatchSource? ds_AddLocomotiveCollectionContextMenuEntries = null;
        [DispatchSource]
        public static void AddLocomotiveCollectionContextMenuEntries(this Dispatcher d, XmlElement placedElement, MenuOrMenuItem m) {
            ds_AddLocomotiveCollectionContextMenuEntries ??= d[nameof(AddLocomotiveCollectionContextMenuEntries)];
            ds_AddLocomotiveCollectionContextMenuEntries.CallVoid(placedElement, m);
        }

        static DispatchSource? ds_EditActionSettings = null;
        [DispatchSource]
        public static bool? EditActionSettings(this Dispatcher d, ControlModule module, ILayoutAction action) {
            ds_EditActionSettings ??= d[nameof(EditActionSettings)];
            return ds_EditActionSettings.Call<bool?>(module, action);
        }

    }
}

