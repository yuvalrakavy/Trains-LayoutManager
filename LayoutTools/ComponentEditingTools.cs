using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.View;

namespace LayoutManager.Tools {
    public class PlacementInfo {
        readonly LayoutModelArea area;
        Point location;

        public PlacementInfo(LayoutEvent e) {
            XmlElement placementInfoElement = e.XmlInfo.DocumentElement["PlacementInfo"];

            area = LayoutModel.Areas[XmlConvert.ToGuid(placementInfoElement.GetAttribute("AreaID"))];
            location = new Point(XmlConvert.ToInt32(placementInfoElement.GetAttribute("X")), XmlConvert.ToInt32(placementInfoElement.GetAttribute("Y")));
        }

        public PlacementInfo(ModelComponent component) {
            area = component.Spot.Area;
            location = component.Location;
        }

        public LayoutModelArea Area => area;

        public Point Location => location;

        public LayoutModelSpotComponentCollection Spot => Area.Grid[Location];

        public LayoutTrackComponent Track => Spot.Track;
    }

    /// <summary>
    /// Summary description for ComponentTools.
    /// </summary>
    [LayoutModule("Components Editing Tools", UserControl = false)]
    public class ComponentEditingTools : System.ComponentModel.Component, ILayoutModuleSetup {
        public static String ComponentEditingToolsVersion = "1.0";

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        #region Constructors

        public ComponentEditingTools(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentEditingTools() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #endregion

        [LayoutEvent("query-can-remove-model-component", SenderType = typeof(LayoutTrackComponent))]
        void QueryCanRemoveTrackComponent(LayoutEvent e) {
            e.Info = false;     // Tracks cannot be removed by itself
        }

        [Flags]
        enum PropertyDialogOptions {
            None = 0,
            AlsoOperationalMode = 0x0001,
            Modeless = 0x0002,
            NotOpenOnPlacement = 0x0004,        // Do not open when placeing new component
        }

        class PropertiesDialogMapEntry {
            internal Type componentType;
            internal Type propertiesDialogType;
            internal PropertyDialogOptions options = PropertyDialogOptions.None;

            internal PropertiesDialogMapEntry(Type componentType, Type propertiesDialogType, PropertyDialogOptions options) {
                this.componentType = componentType;
                this.propertiesDialogType = propertiesDialogType;
                this.options = options;
            }

            internal PropertiesDialogMapEntry(Type componentType, Type propertiesDialogType) {
                this.componentType = componentType;
                this.propertiesDialogType = propertiesDialogType;
                options = PropertyDialogOptions.None;
            }
        }

        /// <summary>
        /// Map between component type and the component properites dialog. If no special treatment is needed,
        /// all you need to do in order to implement showing a property sheet when the component is placed, and
        /// adding a properites context menu item is to add the appropriate entry to this map.
        /// </summary>
        readonly PropertiesDialogMapEntry[] propertiesDialogMap =
            new PropertiesDialogMapEntry[] {
                                               new PropertiesDialogMapEntry(typeof(LayoutTextComponent), typeof(Dialogs.TextProperties), PropertyDialogOptions.AlsoOperationalMode),
                                               new PropertiesDialogMapEntry(typeof(LayoutTrackContactComponent), typeof(Dialogs.TrackContactProperties), PropertyDialogOptions.NotOpenOnPlacement),
                                               new PropertiesDialogMapEntry(typeof(LayoutTurnoutTrackComponent), typeof(Dialogs.TurnoutProperties), PropertyDialogOptions.AlsoOperationalMode|PropertyDialogOptions.NotOpenOnPlacement),
                                               new PropertiesDialogMapEntry(typeof(LayoutDoubleSlipTrackComponent), typeof(Dialogs.TurnoutProperties), PropertyDialogOptions.AlsoOperationalMode|PropertyDialogOptions.NotOpenOnPlacement),
                                               new PropertiesDialogMapEntry(typeof(LayoutTrackPowerConnectorComponent), typeof(Dialogs.TrackPowerConnectorProperties)),
                                               new PropertiesDialogMapEntry(typeof(LayoutBlockDefinitionComponent), typeof(Dialogs.BlockInfoProperties), PropertyDialogOptions.AlsoOperationalMode),
                                               new PropertiesDialogMapEntry(typeof(LayoutImageComponent), typeof(Dialogs.ImageProperties), PropertyDialogOptions.AlsoOperationalMode),
                                               new PropertiesDialogMapEntry(typeof(LayoutSignalComponent), typeof(Dialogs.SignalProperties)),
                                               new PropertiesDialogMapEntry(typeof(LayoutControlModuleLocationComponent), typeof(Dialogs.ControlModuleLocationProperties)),
                                               new PropertiesDialogMapEntry(typeof(LayoutGateComponent), typeof(Dialogs.GateProperties)),
                                               new PropertiesDialogMapEntry(typeof(LayoutPowerSelectorComponent), typeof(Dialogs.PowerSelectorProperties)),
        };

        #region Generic event handlers for component that appear in the 'propertiesDialogMap'

        [LayoutEvent("model-component-placement-request")]
        void PlaceComponent(LayoutEvent e) {
            ModelComponent component = (ModelComponent)e.Sender;

            foreach (PropertiesDialogMapEntry entry in propertiesDialogMap) {
                if (entry.componentType.IsInstanceOfType(component) && (entry.options & PropertyDialogOptions.NotOpenOnPlacement) == 0) {
                    ConstructorInfo constructor = entry.propertiesDialogType.GetConstructor(new Type[] { typeof(ModelComponent), typeof(PlacementInfo) });
                    ILayoutComponentPropertiesDialog dialog;

                    if (constructor != null) {
                        dialog = (ILayoutComponentPropertiesDialog)entry.propertiesDialogType.Assembly.CreateInstance(
                            entry.propertiesDialogType.FullName, false,
                            BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, new Object[] { component, new PlacementInfo(e) }, null, new Object[] { });
                    }
                    else {
                        constructor = entry.propertiesDialogType.GetConstructor(new Type[] { typeof(ModelComponent) });

                        if (constructor != null) {
                            dialog = (ILayoutComponentPropertiesDialog)entry.propertiesDialogType.Assembly.CreateInstance(
                                entry.propertiesDialogType.FullName, false,
                                BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, new Object[] { component }, null, new Object[] { });
                        }
                        else
                            throw new ArgumentException("Invalid dialog class constructor (should be cons(LayoutModel, ModelComponent, [XmlElement]))");
                    }

                    if (dialog.ShowDialog() == DialogResult.OK) {
                        component.XmlInfo.XmlDocument = dialog.XmlInfo.XmlDocument;
                        e.Info = true;      // Place component
                    }
                    else
                        e.Info = false;     // Do not place component

                    break;
                }
            }
        }

        [LayoutEvent("add-component-editing-context-menu-entries")]
        [LayoutEvent("add-component-operation-context-menu-entries", Order = 500)]
        void AddContextMenuPropertiesEntry(LayoutEvent e) {
            ModelComponent component = (ModelComponent)e.Sender;

            foreach (PropertiesDialogMapEntry entry in propertiesDialogMap) {
                if (entry.componentType.IsInstanceOfType(component)) {
                    if (!LayoutController.IsOperationMode || (entry.options & PropertyDialogOptions.AlsoOperationalMode) != 0) {
                        Menu menu = (Menu)e.Info;

                        menu.MenuItems.Add(new MenuItemProperties(component, entry.propertiesDialogType));
                        break;
                    }
                }
            }
        }

        [LayoutEvent("query-component-editing-context-menu")]
        [LayoutEvent("query-component-operation-context-menu")]
        void QueryTrackContactMenu(LayoutEvent e) {
            ModelComponent component = (ModelComponent)e.Sender;

            foreach (PropertiesDialogMapEntry entry in propertiesDialogMap) {
                if (entry.componentType.IsInstanceOfType(component)) {
                    if (!LayoutController.IsOperationMode || (entry.options & PropertyDialogOptions.AlsoOperationalMode) != 0) {
                        e.Info = e.Sender;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Testing component

        private bool canTestComponent(ModelComponent component) {

            if (component is IModelComponentConnectToControl connectableComponent && connectableComponent.FullyConnected) {
                IList<ControlConnectionPoint> connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component.Id];

                if (connectionPoints != null) {
                    foreach (ControlConnectionPoint ccp in connectionPoints) {
                        if (ccp.Usage == ControlConnectionPointUsage.Output)
                            return true;
                    }
                }
            }

            return false;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", Order = 50, SenderType = typeof(IModelComponentConnectToControl))]
        private void AddTestComponentMenuEntry(LayoutEvent e) {
            ModelComponent component = (ModelComponent)e.Sender;
            Menu m = (Menu)e.Info;

            if (canTestComponent(component)) {
                MenuItem item = new MenuItem("&Test", delegate (object sender, EventArgs ea) {
                    EventManager.Event(new LayoutEvent("test-layout-object-request", component).SetFrameWindow(e));
                });

                if (EventManager.Event(new LayoutEvent("query-test-layout-object", this).SetFrameWindow(e)) != null)
                    item.Enabled = false;
                m.MenuItems.Add(item);
            }
        }

        [LayoutEvent("query-component-editing-context-menu")]
        private void queryTestComponentMenuEntry(LayoutEvent e) {
            ModelComponent component = (ModelComponent)e.Sender;

            if (canTestComponent(component))
                e.Info = e.Sender;
        }

        [LayoutEvent("get-test-component-dialog", SenderType = typeof(LayoutThreeWayTurnoutComponent))]
        private void getThreeWayTurnoutTestDialog(LayoutEvent e) {
            LayoutThreeWayTurnoutComponent component = (LayoutThreeWayTurnoutComponent)e.Sender;

            if (component != null)
                e.Info = new Dialogs.TestThreeWayTurnout(e.GetFrameWindowId(), component);
        }

        [LayoutEvent("test-layout-object-request")]
        private void testLayoutObjectRequest(LayoutEvent e) {
            object testObject = e.Sender;

            try {
                if (LayoutController.Instance.BeginDesignTimeActivation()) {
                    Form testDialog = null;

                    if (testDialog == null) {
                        if (testObject is IModelComponentConnectToControl) {
                            testDialog = (Form)EventManager.Event(new LayoutEvent("get-test-component-dialog", testObject).SetFrameWindow(e));
                            if (testDialog == null)
                                testDialog = new LayoutManager.Tools.Dialogs.TestLayoutObject(e.GetFrameWindowId(), (IModelComponentConnectToControl)testObject);
                        }
                        else if (testObject is ControlConnectionPoint controlCp) {
                            testDialog = (Form)EventManager.Event(new LayoutEvent("get-test-component-dialog", controlCp.Component).SetFrameWindow(e));
                            if (testDialog == null)
                                testDialog = new LayoutManager.Tools.Dialogs.TestLayoutObject(e.GetFrameWindowId(), new ControlConnectionPointReference((ControlConnectionPoint)testObject));
                        }
                        else if (testObject is ControlConnectionPointReference controlCpRef) {
                            testDialog = (Form)EventManager.Event(new LayoutEvent("get-test-component-dialog", controlCpRef.ConnectionPoint.Component).SetFrameWindow(e));

                            if (testDialog == null)
                                testDialog = new LayoutManager.Tools.Dialogs.TestLayoutObject(e.GetFrameWindowId(), (ControlConnectionPointReference)testObject);
                        }
                        else
                            throw new ArgumentException("Cannot invoke test layout object dialog on this object");
                    }

                    if (testDialog != null)
                        testDialog.Show(LayoutController.ActiveFrameWindow);
                }
                else
                    throw new LayoutException("Command station does not support powering the layout while in design mode");
            }
            catch (Exception ex) {
                MessageBox.Show(LayoutController.ActiveFrameWindow, "Error when activating layout: " + ex.Message, "Error in layout activation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion

        #region Track Link Component

        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(LayoutTrackLinkComponent))]
        [LayoutEvent("add-editing-details-window-sections", SenderType = typeof(LayoutTrackLinkComponent))]
        private void addTrackLinkSection(LayoutEvent e) {
            LayoutTrackLinkComponent trackLinkComponent = (LayoutTrackLinkComponent)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;
            LayoutTrackLinkTextInfo textProvider = new LayoutTrackLinkTextInfo(trackLinkComponent, "Name");

            container.AddText("Track link: " + textProvider.Name);

            if (trackLinkComponent.LinkedComponent != null) {
                LayoutTrackLinkComponent linkedComponent = trackLinkComponent.LinkedComponent;
                LayoutTrackLinkTextInfo linkedTextProvider = new LayoutTrackLinkTextInfo(linkedComponent, "Name");

                container.AddText("Linked to: " + linkedTextProvider.Name);
            }
            else
                container.AddText("Not yet linked");
        }

        [LayoutEvent("model-component-placement-request", SenderType = typeof(LayoutTrackLinkComponent))]
        void PlaceTrackLinkRequest(LayoutEvent e) {
            LayoutTrackLinkComponent component = (LayoutTrackLinkComponent)e.Sender;
            PlacementInfo placementProvider = new PlacementInfo(e);
            LayoutModelArea area = placementProvider.Area;
            Dialogs.TrackLinkProperties trackLinkProperties = new Dialogs.TrackLinkProperties(area, component);

            if (trackLinkProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = trackLinkProperties.XmlInfo.XmlDocument;

                LayoutCompoundCommand addCommand = new LayoutCompoundCommand("Add track-link");

                addCommand.Add(new LayoutComponentPlacmentCommand(area, placementProvider.Location, component, "Add track link", area.Phase(placementProvider.Location)));

                if (trackLinkProperties.TrackLink != null) {
                    LayoutTrackLinkComponent destinationTrackLinkComponent = trackLinkProperties.TrackLink.ResolveLink();

                    if (destinationTrackLinkComponent.Link != null)
                        addCommand.Add(new LayoutComponentUnlinkCommand(destinationTrackLinkComponent));

                    addCommand.Add(new LayoutComponentLinkCommand(component, trackLinkProperties.TrackLink));
                }

                LayoutController.Do(addCommand);

            }

            e.Info = false;         // Do not place component, it is already placed
        }

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(LayoutTrackLinkComponent))]
        void QueryTrackLinkContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(LayoutTrackLinkComponent))]
        void AddTrackLinkConextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            LayoutTrackLinkComponent component = (LayoutTrackLinkComponent)e.Sender;

            menu.MenuItems.Add(new TrackLinkMenuItemProperties(component));
        }

        [LayoutEvent("add-component-editing-context-menu-top-entries", SenderType = typeof(LayoutTrackLinkComponent))]
        [LayoutEvent("add-component-operation-context-menu-top-entries", SenderType = typeof(LayoutTrackLinkComponent))]
        void AddTrackLinkTopContextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            LayoutTrackLinkComponent component = (LayoutTrackLinkComponent)e.Sender;

            MenuItem item = new TrackLinkMenuItemViewLinkedComponent(e.GetFrameWindowId(), component) {
                DefaultItem = true
            };

            menu.MenuItems.Add(item);
        }

        [LayoutEvent("add-component-editing-context-menu-top-entries", SenderType = typeof(LayoutTrackPowerConnectorComponent))]
        void addPowerConnectorContextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            var powerConnectorcomponent = (LayoutTrackPowerConnectorComponent)e.Sender;

            menu.MenuItems.Add("Assign as block resource",
                (sender, ea) => {
                    LayoutPhase checkPhases = LayoutPhase.None;
                    LayoutCompoundCommand command = null;

                    switch (powerConnectorcomponent.Spot.Phase) {
                        case LayoutPhase.Operational: checkPhases = LayoutPhase.Operational; break;
                        case LayoutPhase.Construction: checkPhases = LayoutPhase.Operational | LayoutPhase.Construction; break;
                        case LayoutPhase.Planned: checkPhases = LayoutPhase.All; break;
                    }

                    if ((bool)EventManager.Event(new LayoutEvent("check-layout", this, true, null).SetPhases(checkPhases))) {
                        EventManager.Event(
                            new LayoutEvent<LayoutTrackPowerConnectorComponent, Action<LayoutBlockDefinitionComponent>>("add-power-connector-as-resource", powerConnectorcomponent,
                                blockDefinition => {
                                    LayoutXmlInfo xmlInfo = new LayoutXmlInfo(blockDefinition);
                                    var newBlockDefinitionInfo = new LayoutBlockDefinitionComponentInfo(blockDefinition, xmlInfo.Element);
                                    command = command ?? new LayoutCompoundCommand("Assign power connector as resource");

                                    newBlockDefinitionInfo.Resources.Add(powerConnectorcomponent.Id);
                                    command.Add(new LayoutModifyComponentDocumentCommand(blockDefinition, xmlInfo));
                                }
                            )
                        );

                        if (command != null)
                            LayoutController.Do(command);
                    }
                }
            );
        }

        [LayoutEvent("query-editing-default-action", SenderType = typeof(LayoutTrackLinkComponent))]
        private void trackLinkQueryEditingDefaultAction(LayoutEvent e) {
            e.Info = true;
        }

        [LayoutEvent("default-action-command", SenderType = typeof(LayoutTrackLinkComponent))]
        [LayoutEvent("editing-default-action-command", SenderType = typeof(LayoutTrackLinkComponent))]
        private void trackLinkDefaultAction(LayoutEvent e) {
            LayoutTrackLinkComponent trackLink = (LayoutTrackLinkComponent)e.Sender;

            ModelComponent linkedComponent = trackLink.LinkedComponent;

            if (linkedComponent != null)
                EventManager.Event(new LayoutEvent("ensure-component-visible", linkedComponent, true, null).SetFrameWindow(e));
        }

        #region Track Link Menu item classes

        class TrackLinkMenuItemProperties : MenuItem {
            readonly LayoutTrackLinkComponent trackLinkComponent;

            internal TrackLinkMenuItemProperties(LayoutTrackLinkComponent trackLinkComponent) {
                this.trackLinkComponent = trackLinkComponent;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                Dialogs.TrackLinkProperties trackLinkProperties = new Dialogs.TrackLinkProperties(trackLinkComponent.Spot.Area, trackLinkComponent);

                if (trackLinkProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    LayoutCompoundCommand editProperties = new LayoutCompoundCommand("edit " + trackLinkComponent.ToString() + " properties");

                    LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                        new LayoutModifyComponentDocumentCommand(trackLinkComponent, trackLinkProperties.XmlInfo);

                    editProperties.Add(modifyComponentDocumentCommand);

                    // If this component is to be linked
                    if (trackLinkProperties.TrackLink != null) {
                        // Unlink this component if it is already linked
                        if (trackLinkComponent.Link != null) {
                            LayoutComponentUnlinkCommand unlinkCommand = new LayoutComponentUnlinkCommand(trackLinkComponent);

                            editProperties.Add(unlinkCommand);
                        }

                        LayoutTrackLinkComponent destinationTrackLinkComponent = trackLinkProperties.TrackLink.ResolveLink();

                        // Unlink destination component if it is linked
                        if (destinationTrackLinkComponent.Link != null) {
                            LayoutComponentUnlinkCommand unlinkCommand = new LayoutComponentUnlinkCommand(destinationTrackLinkComponent);

                            editProperties.Add(unlinkCommand);
                        }

                        // Link to the destination component
                        LayoutComponentLinkCommand linkCommand = new LayoutComponentLinkCommand(trackLinkComponent, trackLinkProperties.TrackLink);

                        editProperties.Add(linkCommand);
                    }
                    else {
                        // If component was linked, remove the link
                        if (trackLinkComponent.Link != null) {
                            LayoutComponentUnlinkCommand unlinkCommand = new LayoutComponentUnlinkCommand(trackLinkComponent);

                            editProperties.Add(unlinkCommand);
                        }
                    }

                    LayoutController.Do(editProperties);
                }
            }
        }

        class TrackLinkMenuItemViewLinkedComponent : MenuItem {
            internal TrackLinkMenuItemViewLinkedComponent(Guid frameWindowId, LayoutTrackLinkComponent trackLinkComponent)
                : base("&Show other link", (s, ea) => {
                    EventManager.Event(new LayoutEvent("ensure-component-visible", trackLinkComponent.LinkedComponent, true, null).SetFrameWindow(frameWindowId));
                }) {

                if (trackLinkComponent.LinkedComponent == null)
                    this.Enabled = false;
            }
        }

        #endregion

        #endregion

        #region Block Edge component

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(LayoutBlockEdgeBase))]
        void QueryBlockEdgeContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(LayoutBlockEdgeBase))]
        void AddBlockEdgeConextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            LayoutBlockEdgeBase component = (LayoutBlockEdgeBase)e.Sender;
            ArrayList myBindDialog = new ArrayList();
            ArrayList allBindDialog = new ArrayList();

            EventManager.Event(new LayoutEvent("query-bind-signals-dialogs", component, myBindDialog, null));
            EventManager.Event(new LayoutEvent("query-bind-signals-dialogs", null, allBindDialog, null));

            MenuItem item = new BlockEdgeBindSignalMenuItem(component);

            if (myBindDialog.Count == 0 && allBindDialog.Count > 0)
                item.Enabled = false;       // Another track contact bind dialog is open

            menu.MenuItems.Add(item);
        }

        public class BlockEdgeBindSignalMenuItem : MenuItem {
            readonly LayoutBlockEdgeBase blockEdge;

            public BlockEdgeBindSignalMenuItem(LayoutBlockEdgeBase blockEdge) {
                this.blockEdge = blockEdge;

                Text = "&Link Signals";
            }

            protected override void OnClick(EventArgs e) {
                ArrayList myBindDialog = new ArrayList();

                EventManager.Event(new LayoutEvent("query-bind-signals-dialogs", blockEdge, myBindDialog, null));

                if (myBindDialog.Count > 0) {
                    Form f = (Form)myBindDialog[0];

                    f.Activate();
                }
                else {
                    Dialogs.BindBlockEdgeToSignals dialog = new Dialogs.BindBlockEdgeToSignals(blockEdge);

                    dialog.Show();
                }
            }
        }

        #endregion

        #region Signal component

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(LayoutSignalComponent))]
        void QuerySignalContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(LayoutSignalComponent))]
        void AddSignalConextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            LayoutSignalComponent component = (LayoutSignalComponent)e.Sender;
            ArrayList allBindDialog = new ArrayList();

            EventManager.Event(new LayoutEvent("query-bind-signals-dialogs", null, allBindDialog, null));

            if (allBindDialog.Count > 0) {
                MenuItem item = new SignalBindSignalMenuItem((IModelComponentReceiverDialog)allBindDialog[0], component);
                menu.MenuItems.Add(item);
            }
        }

        public class SignalBindSignalMenuItem : MenuItem {
            readonly LayoutSignalComponent signal;
            readonly IModelComponentReceiverDialog dialog;

            public SignalBindSignalMenuItem(IModelComponentReceiverDialog dialog, LayoutSignalComponent signal) {
                this.dialog = dialog;
                this.signal = signal;

                Text = "&Link Signal";
            }

            protected override void OnClick(EventArgs e) {
                dialog.AddComponent(signal);
            }
        }

        #endregion


        #region Shift components

        #region Base class

        class ShiftLimit {
            public int From;
            public int To;

            public ShiftLimit(int v1, int v2) {
                From = Math.Min(v1, v2);
                To = Math.Max(v1, v2);
            }

            public bool Contains(int v) => From <= v && v <= To;
        }

        class ShiftComponentsMenuEntryBase : MenuItem {
            readonly LayoutHitTestResult hitTestResult = null;

            protected enum OperationType {
                ShiftAreaComponents, MoveSelection
            };

            readonly OperationType operation = OperationType.ShiftAreaComponents;
            readonly string directionText;
            ChangeCoordinate changeWhat;
            CompareValue.Order sortOrder;
            int delta;
            bool fillGaps = true;

            protected ShiftComponentsMenuEntryBase(OperationType operation, LayoutHitTestResult hitTestResult, string directionText, ChangeCoordinate changeWhat, CompareValue.Order sortOrder) {
                this.operation = operation;
                this.hitTestResult = hitTestResult;
                this.directionText = directionText;
                this.changeWhat = changeWhat;
                this.sortOrder = sortOrder;

                Text = directionText;
            }

            protected ShiftComponentsMenuEntryBase(OperationType operation, LayoutHitTestResult hitTestResult, string text) {
                this.operation = operation;
                this.hitTestResult = hitTestResult;
                this.changeWhat = ChangeCoordinate.Prompt;
                this.sortOrder = CompareValue.Order.Ascending;
                this.Text = text;
            }

            protected OperationType Operation => operation;

            protected ChangeCoordinate ChangeWhat {
                get {
                    return changeWhat;
                }

                set {
                    changeWhat = value;
                }
            }

            protected CompareValue.Order SortOrder {
                get {
                    return sortOrder;
                }

                set {
                    sortOrder = value;
                }
            }

            protected string OperationText {
                get {
                    if (Operation == OperationType.MoveSelection)
                        return "move selection " + directionText;
                    else
                        return "shift components " + directionText;
                }
            }

            public LayoutHitTestResult HitTestResult => hitTestResult;

            public int Delta {
                get {
                    return delta;
                }

                protected set {
                    delta = value;
                }
            }

            public bool FillGaps {
                get {
                    return fillGaps;
                }

                set {
                    fillGaps = value;
                }
            }

            protected class CompareValue : IComparer<int> {
                public enum Order {
                    Ascending, Decending
                }

                readonly Order compareOrder;

                public CompareValue(Order compareOrder) {
                    this.compareOrder = compareOrder;
                }

                #region IComparer<int> Members

                public int Compare(int v1, int v2) {
                    if (compareOrder == Order.Ascending)
                        return v1 - v2;
                    else
                        return v2 - v1;
                }

                #endregion
            }

            class ZorderSorter : IComparer {
                public int Compare(Object o1, Object o2) => ((ModelComponent)o2).ZOrder - ((ModelComponent)o1).ZOrder;
            }


            protected enum ChangeCoordinate {
                Prompt, ChangeX, ChangeY
            }

            protected LayoutCompoundCommand DoShift(SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>> majorAxis, string commandName, ChangeCoordinate chageWhat) {
                LayoutCompoundCommand shiftCommand = new LayoutCompoundCommand(commandName);

                foreach (SortedList<int, LayoutModelSpotComponentCollection> minorAxis in majorAxis.Values) {
                    foreach (LayoutModelSpotComponentCollection spot in minorAxis.Values) {
                        Point newSpotLocation;

                        if (chageWhat == ChangeCoordinate.ChangeX)
                            newSpotLocation = new Point(spot.Location.X + Delta, spot.Location.Y);
                        else
                            newSpotLocation = new Point(spot.Location.X, spot.Location.Y + Delta);

                        LayoutModelSpotComponentCollection existingSpot = spot.Area[newSpotLocation, LayoutPhase.All];

                        if (existingSpot != null) {
                            bool deleteIt = true;

                            // Check this spot was moved, if so, do not delete it because it will not be there when command is
                            // executed
                            foreach (LayoutCommand command in shiftCommand) {

                                if (command is LayoutMoveModelSpotCommand moveSpotCommand && moveSpotCommand.Spot == existingSpot) {
                                    deleteIt = false;
                                    break;
                                }
                            }

                            if (deleteIt) {
                                ModelComponent[] components = new ModelComponent[existingSpot.Count];

                                existingSpot.CopyTo(components, 0);
                                Array.Sort(components, new ZorderSorter());     // Sort, so the first delete components will be those with higher Z order

                                for (int i = components.Length - 1; i >= 0; i--) {
                                    EventManager.Event(new LayoutEvent("prepare-for-component-remove-command", components[i], shiftCommand, null));
                                    shiftCommand.Add(new LayoutComponentRemovalCommand(components[i], "Delete"));
                                }
                            }
                        }

                        shiftCommand.Add(new LayoutMoveModelSpotCommand(spot, newSpotLocation));
                    }
                }

                return shiftCommand;
            }

            protected SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>> GetSelectionSpotList(LayoutSelection selection, ChangeCoordinate majorAxisIs, CompareValue.Order sortOrder) {
                SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>> majorAxis = new SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>>(new CompareValue(CompareValue.Order.Ascending));

                foreach (ModelComponent component in selection) {
                    if (component.Spot.Area == HitTestResult.Area) {
                        int majorAxisIndex;
                        int minorAxisIndex;

                        if (majorAxisIs == ChangeCoordinate.ChangeY) {
                            majorAxisIndex = component.Location.X;
                            minorAxisIndex = component.Location.Y;
                        }
                        else {
                            majorAxisIndex = component.Location.Y;
                            minorAxisIndex = component.Location.X;
                        }


                        if (!majorAxis.TryGetValue(majorAxisIndex, out SortedList<int, LayoutModelSpotComponentCollection> minorAxis)) {
                            minorAxis = new SortedList<int, LayoutModelSpotComponentCollection>(new CompareValue(sortOrder));

                            majorAxis.Add(majorAxisIndex, minorAxis);
                        }

                        if (!minorAxis.ContainsKey(minorAxisIndex))
                            minorAxis.Add(minorAxisIndex, component.Spot);
                    }
                }

                return majorAxis;
            }

            /// <summary>
            /// Return a selection containing all components that should be moved
            /// </summary>
            /// <returns>Selection containing components to be moved</returns>
            private LayoutSelection GetAreaSelection() {
                LayoutSelection selection = new LayoutSelection();
                int threashold = (ChangeWhat == ChangeCoordinate.ChangeX) ? HitTestResult.ModelLocation.X : HitTestResult.ModelLocation.Y;

                foreach (LayoutModelSpotComponentCollection spot in HitTestResult.Area.Grid.Values) {
                    int v = (ChangeWhat == ChangeCoordinate.ChangeX) ? spot.Location.X : spot.Location.Y;

                    if ((Delta > 0 && v >= threashold) || (Delta < 0 && v <= threashold))
                        selection.Add(spot.Components);
                }

                return selection;
            }

            protected void FillHorizontalGap(IDictionary<int, int> boundries, LayoutCompoundCommand shiftCommand) {
                LayoutModelArea area = HitTestResult.Area;

                foreach (KeyValuePair<int, int> boundryPair in boundries) {
                    int x = boundryPair.Key;
                    int y = boundryPair.Value;
                    int movedSpotY, fixedSpotY;
                    LayoutModelSpotComponentCollection movedSpot;

                    if (Delta < 0) {            // move up
                        movedSpotY = y;
                        fixedSpotY = y + 1;
                    }
                    else {  // move down
                        movedSpotY = y;
                        fixedSpotY = y - 1;
                    }

                    movedSpot = area.Grid[new Point(x, movedSpotY)];

                    if (area.Grid.TryGetValue(new Point(movedSpot.Location.X, fixedSpotY), out LayoutModelSpotComponentCollection fixedSpot)) {
                        // Check that the track layer of the top spot has a B connection point and that the 
                        // track layer of the bottom spot has a T connection point. If this is the case,
                        // add a vertical straight track to connect the two.
                        LayoutTrackComponent movedTrack = movedSpot.Track;
                        LayoutTrackComponent fixedTrack = fixedSpot.Track;

                        if (movedTrack != null && fixedTrack != null) {
                            if (movedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.B) && fixedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.T)) {
                                int inc = Delta < 0 ? -1 : 1;
                                int count = Math.Abs(Delta);

                                for (int i = 0; i < count; i++) {
                                    LayoutTrackComponent newTrack = new LayoutStraightTrackComponent(LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B);

                                    shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(movedSpot.Location.X, y), newTrack, "track", movedSpot.Phase));


                                    if (movedTrack.TrackBackground is LayoutBridgeComponent bridge) {
                                        bridge = new LayoutBridgeComponent();
                                        shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(movedSpot.Location.X, y), bridge, "bridge", movedSpot.Phase));
                                    }


                                    if (movedTrack.TrackBackground is LayoutTunnelComponent tunnel) {
                                        tunnel = new LayoutTunnelComponent();
                                        shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(movedSpot.Location.X, y), tunnel, "tunnel", movedSpot.Phase));
                                    }

                                    y += inc;
                                }
                            }
                        }
                    }
                }
            }

            protected void FillVerticalGap(IDictionary<int, int> boundries, LayoutCompoundCommand shiftCommand) {
                LayoutModelArea area = HitTestResult.Area;

                foreach (KeyValuePair<int, int> boundryPair in boundries) {
                    int y = boundryPair.Key;
                    int x = boundryPair.Value;
                    int movedSpotX, fixedSpotX;
                    LayoutModelSpotComponentCollection movedSpot;

                    if (Delta < 0) {            // move left
                        movedSpotX = x;
                        fixedSpotX = x + 1;
                    }
                    else {  // move right
                        movedSpotX = x;
                        fixedSpotX = x - 1;
                    }

                    movedSpot = area.Grid[new Point(movedSpotX, y)];


                    if (area.Grid.TryGetValue(new Point(fixedSpotX, movedSpot.Location.Y), out LayoutModelSpotComponentCollection fixedSpot)) {
                        // Check that the track layer of the moved spot has a R connection point and that the 
                        // track layer of the fixed spot has a L connection point. If this is the case,
                        // add a horizontal straight track to connect the two.
                        LayoutTrackComponent movedTrack = movedSpot.Track;
                        LayoutTrackComponent fixedTrack = fixedSpot.Track;

                        if (movedTrack != null && fixedTrack != null) {
                            if (movedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.R) && fixedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.L)) {
                                int inc = Delta < 0 ? -1 : 1;
                                int count = Math.Abs(Delta);

                                for (int i = 0; i < count; i++) {
                                    LayoutTrackComponent newTrack = new LayoutStraightTrackComponent(LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R);

                                    shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(x, movedSpot.Location.Y), newTrack, "track", movedSpot.Phase));


                                    if (movedTrack.TrackBackground is LayoutBridgeComponent bridge) {
                                        bridge = new LayoutBridgeComponent();
                                        shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(x, movedSpot.Location.Y), bridge, "bridge", movedSpot.Phase));
                                    }


                                    if (movedTrack.TrackBackground is LayoutTunnelComponent tunnel) {
                                        tunnel = new LayoutTunnelComponent();
                                        shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(x, movedSpot.Location.Y), tunnel, "tunnel", movedSpot.Phase));
                                    }

                                    x += inc;
                                }
                            }
                        }
                    }
                }
            }

            private IDictionary<int, int> FindBoundries(LayoutSelection selection) {
                Dictionary<int, int> boundries = new Dictionary<int, int>();

                foreach (ModelComponent component in selection) {
                    int i = (ChangeWhat == ChangeCoordinate.ChangeY) ? component.Spot.Location.X : component.Location.Y;
                    int v = (ChangeWhat == ChangeCoordinate.ChangeY) ? component.Spot.Location.Y : component.Location.X;

                    if (!boundries.TryGetValue(i, out int boundryValue))
                        boundries[i] = v;
                    else if ((Delta < 0 && v > boundryValue) || (Delta > 0 && v < boundryValue)) {
                        boundries.Remove(i);
                        boundries[i] = v;
                    }
                }

                return boundries;
            }

            public void DoOperation(int moveBy, bool fillGap) {
                Delta = SortOrder == CompareValue.Order.Ascending ? -moveBy : moveBy;

                LayoutSelection selection = (Operation == OperationType.ShiftAreaComponents) ? GetAreaSelection() : LayoutController.UserSelection;
                SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>> majorAxis = GetSelectionSpotList(selection, changeWhat, sortOrder);
                LayoutCompoundCommand shiftCommand = DoShift(majorAxis, OperationText, changeWhat);

                if (FillGaps) {
                    IDictionary<int, int> bounderies = FindBoundries(selection);

                    if (changeWhat == ChangeCoordinate.ChangeY)
                        FillHorizontalGap(bounderies, shiftCommand);
                    else
                        FillVerticalGap(bounderies, shiftCommand);
                }

                shiftCommand.Add(new RedrawLayoutModelAreaCommand(HitTestResult.Area));
                LayoutController.Do(shiftCommand);
            }

            protected override void OnClick(EventArgs e) {
                if (changeWhat == ChangeCoordinate.Prompt) {
                    Dialogs.ShiftOrMove d = new LayoutManager.Tools.Dialogs.ShiftOrMove {
                        Text = (Operation == OperationType.MoveSelection) ? "Move Selection" : "Shift Components"
                    };

                    if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.OK) {
                        if (d.DeltaX != 0) {
                            ChangeWhat = ChangeCoordinate.ChangeX;
                            Delta = Math.Abs(d.DeltaX);
                            SortOrder = d.DeltaX < 0 ? CompareValue.Order.Ascending : CompareValue.Order.Decending;
                        }
                        else {
                            ChangeWhat = ChangeCoordinate.ChangeY;
                            Delta = Math.Abs(d.DeltaY);
                            SortOrder = d.DeltaY < 0 ? CompareValue.Order.Ascending : CompareValue.Order.Decending;
                        }

                        DoOperation(Delta, true);
                    }
                }
                else
                    DoOperation(1, true);
            }

        }

        #endregion

        #region Shift command menu entries

        class ShiftComponentsUpMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsUpMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "up", ChangeCoordinate.ChangeY, CompareValue.Order.Ascending) {
            }
        }

        class ShiftComponentsDownMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsDownMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "down", ChangeCoordinate.ChangeY, CompareValue.Order.Decending) {
            }
        }

        class ShiftComponentsLeftMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsLeftMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "left", ChangeCoordinate.ChangeX, CompareValue.Order.Ascending) {
            }
        }

        class ShiftComponentsRightMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsRightMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "right", ChangeCoordinate.ChangeX, CompareValue.Order.Decending) {
            }
        }

        class ShiftComponentsByMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsByMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "By...") {
            }
        }

        class MoveSelectionUpMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionUpMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "up", ChangeCoordinate.ChangeY, CompareValue.Order.Ascending) {
            }
        }

        class MoveSelectionDownMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionDownMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "down", ChangeCoordinate.ChangeY, CompareValue.Order.Decending) {
            }
        }

        class MoveSelectionLeftMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionLeftMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "left", ChangeCoordinate.ChangeX, CompareValue.Order.Ascending) {
            }
        }

        class MoveSelectionRightMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionRightMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "right", ChangeCoordinate.ChangeX, CompareValue.Order.Decending) {
            }
        }

        class MoveComponentsByMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveComponentsByMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "By...") {
            }
        }

        #endregion

        public class ShiftComponentsMenuEntry : MenuItem {
            public ShiftComponentsMenuEntry(LayoutHitTestResult hitTestResult) {
                this.Text = "&Shift Components";

                MenuItems.Add(new ShiftComponentsByMenuEntry(hitTestResult));
                MenuItems.Add("-");
                MenuItems.Add(new ShiftComponentsUpMenuEntry(hitTestResult));
                MenuItems.Add(new ShiftComponentsDownMenuEntry(hitTestResult));
                MenuItems.Add(new ShiftComponentsLeftMenuEntry(hitTestResult));
                MenuItems.Add(new ShiftComponentsRightMenuEntry(hitTestResult));
            }
        }

        public class MoveSelectionMenuEntry : MenuItem {
            public MoveSelectionMenuEntry(LayoutHitTestResult hitTestResult) {
                this.Text = "&Move selection";

                MenuItems.Add(new MoveComponentsByMenuEntry(hitTestResult));
                MenuItems.Add("-");
                MenuItems.Add(new MoveSelectionUpMenuEntry(hitTestResult));
                MenuItems.Add(new MoveSelectionDownMenuEntry(hitTestResult));
                MenuItems.Add(new MoveSelectionLeftMenuEntry(hitTestResult));
                MenuItems.Add(new MoveSelectionRightMenuEntry(hitTestResult));
            }
        }

        [LayoutEvent("add-editing-empty-spot-context-menu-entries", Order = 100)]
        [LayoutEvent("add-component-editing-context-menu-common-entries", Order = 100)]
        private void addShiftComponentMenuEntry(LayoutEvent e) {
            LayoutHitTestResult hitTestResult = (LayoutHitTestResult)e.Sender;
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add(new ShiftComponentsMenuEntry(hitTestResult));
        }

        [LayoutEvent("add-editing-selection-menu-entries", Order = 100)]
        private void addSelectionShiftComponentMenuEntries(LayoutEvent e) {
            LayoutHitTestResult hitTestResult = (LayoutHitTestResult)e.Sender;
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add(new MoveSelectionMenuEntry(hitTestResult));
        }

        #endregion

        #region Count Components

        [LayoutEvent("add-editing-selection-menu-entries", Order = 101)]
        private void addCountComponentsMenuEntry(LayoutEvent e) {
            LayoutHitTestResult hitTestResult = (LayoutHitTestResult)e.Sender;
            Menu menu = (Menu)e.Info;

            menu.MenuItems.Add("Count Components", (sender, ea) => {
                Dictionary<string, int[]> counts = new Dictionary<string, int[]>();
                var connectedComponents = from component in LayoutController.UserSelection.Components where component is IModelComponentConnectToControl select (IModelComponentConnectToControl)component;

                foreach (var component in connectedComponents) {
                    foreach (var connectionDescription in component.ControlConnectionDescriptions) {
                        foreach (var type in connectionDescription.ConnectionTypes.Split(',')) {
                            if (!counts.ContainsKey(type))
                                counts.Add(type, new int[2]);
                            counts[type][component.FullyConnected ? 0 : 1]++;
                        }
                    }
                }

                new Dialogs.CountComponents(counts).ShowDialog();
            });
        }

        #endregion

        #region Module Location Component

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(LayoutControlModuleLocationComponent))]
        void QueryControlModuleLocationContextMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(LayoutControlModuleLocationComponent))]
        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(LayoutControlModuleLocationComponent))]
        void AddControlModuleLocationConextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            LayoutControlModuleLocationComponent component = (LayoutControlModuleLocationComponent)e.Sender;

            menu.MenuItems.Add(new ShowControlModuleLocationMenuItem(component));
        }

        class ShowControlModuleLocationMenuItem : MenuItem {
            readonly LayoutControlModuleLocationComponent component;

            public ShowControlModuleLocationMenuItem(LayoutControlModuleLocationComponent component) {
                this.component = component;

                Text = "&Show Control Modules";
                DefaultItem = true;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                EventManager.Event(new LayoutEvent("show-control-modules-location", component));
            }
        }

        [LayoutEvent("query-editing-default-action", SenderType = typeof(LayoutControlModuleLocationComponent))]
        private void controlModuleLocationQueryEditingDefaultAction(LayoutEvent e) {
            e.Info = true;
        }

        [LayoutEvent("default-action-command", SenderType = typeof(LayoutControlModuleLocationComponent))]
        [LayoutEvent("editing-default-action-command", SenderType = typeof(LayoutControlModuleLocationComponent))]
        private void controlModuleLocationDefaultAction(LayoutEvent e) {
            LayoutControlModuleLocationComponent component = (LayoutControlModuleLocationComponent)e.Sender;

            EventManager.Event(new LayoutEvent("show-control-modules-location", component));
        }

        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new Container();
        }
        #endregion

        #region Component independt handing of Details window

        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(IModelComponentHasName), Order = 10)]
        [LayoutEvent("add-editing-details-window-sections", SenderType = typeof(IModelComponentHasName), Order = 10)]
        private void addComponentNameSection(LayoutEvent e) {
            IModelComponentHasName component = (IModelComponentHasName)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;

            string name = component.NameProvider.Name;

            if (!string.IsNullOrEmpty(name))
                container.AddVerticalSection(new PopupWindowTextSection(new Font("Arial", 8, FontStyle.Bold), name));
        }

        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(IModelComponentConnectToControl), Order = 100)]
        [LayoutEvent("add-editing-details-window-sections", SenderType = typeof(IModelComponentConnectToControl), Order = 100)]
        private void addComponentControlConnectionSection(LayoutEvent e) {
            IModelComponentConnectToControl component = (IModelComponentConnectToControl)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;

            if (component is IModelComponentIsMultiPath) {
                PopupWindowContainerSection infoContainer = container.CreateContainer();

                addControlledComponentDetails(component, infoContainer);

                container.AddHorizontalSection(new PopupWindowViewZoomSection((View.LayoutView)container.Parent, component.Location, new Size(32, 32)));
                container.AddHorizontalSection(infoContainer);
            }
            else
                addControlledComponentDetails(component, container);
        }

        private void addControlledComponentDetails(IModelComponentConnectToControl component, PopupWindowContainerSection container) {
            IList<ControlConnectionPoint> connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component.Id];

            if (connectionPoints != null) {
                foreach (ControlConnectionPoint connectionPoint in connectionPoints)
                    container.AddVerticalSection(new PopupWindowTextSection(connectionPoint.DisplayName + ": " + connectionPoint.Module.ModuleType.GetConnectionPointAddressText(connectionPoint.Module.Address, connectionPoint.Index, true)));
            }

            if (!component.FullyConnected)
                container.AddVerticalSection(new PopupWindowTextSection("This component needs to be connected"));
        }

        #endregion

        #region Block Definition details popup window handler

        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(LayoutBlockDefinitionComponent), Order = 200)]
        [LayoutEvent("add-editing-details-window-sections", SenderType = typeof(LayoutBlockDefinitionComponent), Order = 200)]
        private void addBlockDefinitionSection(LayoutEvent e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;
            LayoutBlockDefinitionComponentInfo info = blockDefinition.Info;

            if (blockDefinition.Attributes.Count > 0)
                container.AddVerticalSection(new PopupWindowAttributesSection("Block attributes: ", blockDefinition));

            if (info.TrainLengthLimit != TrainLength.VeryLong)
                container.AddText("Only trains which are not longer than " + info.TrainLengthLimit.ToDisplayString(true) + " can stop in this block");

            if (info.SpeedLimit > 0)
                container.AddText("Speed limit: " + info.SpeedLimit);

            if (info.SlowdownSpeed > 0)
                container.AddText("Slow down speed: " + info.SlowdownSpeed);

            if (info.IsSlowdownRegion)
                container.AddText("Part of a slow down region");

            if (!info.TrainPassCondition.IsConditionEmpty)
                container.AddText((info.TrainPassCondition.ConditionScope == TripPlanTrainConditionScope.AllowIfTrue ? "Allow Passing " : "Disallow Passing ") + info.TrainPassCondition.GetDescription());

            if (!info.TrainStopCondition.IsConditionEmpty)
                container.AddText((info.TrainStopCondition.ConditionScope == TripPlanTrainConditionScope.AllowIfTrue ? "Allow Stopping " : "Disallow Stopping ") + info.TrainStopCondition.GetDescription());

            if (info.IsTripSectionBoundry(0) && info.IsTripSectionBoundry(1))
                container.AddText("Trip section boundry");
            else {
                LayoutComponentConnectionPoint boundryFrom = LayoutComponentConnectionPoint.Empty;

                if (info.IsTripSectionBoundry(0))
                    boundryFrom = blockDefinition.Track.ConnectionPoints[0];
                else if (info.IsTripSectionBoundry(1))
                    boundryFrom = blockDefinition.Track.ConnectionPoints[1];

                if (boundryFrom != LayoutComponentConnectionPoint.Empty) {
                    string from = "unknown";

                    switch (boundryFrom) {
                        case LayoutComponentConnectionPoint.T: from = "top"; break;
                        case LayoutComponentConnectionPoint.B: from = "bottom"; break;
                        case LayoutComponentConnectionPoint.L: from = "left"; break;
                        case LayoutComponentConnectionPoint.R: from = "right"; break;
                    }

                    container.AddText("Trip section boundry for trains coming from " + from);
                }
            }

            if (info.Length != 100)
                container.AddText("Block length: " + info.Length);

            if (info.Resources.Count > 0) {
                string resourceList = "";

                foreach (ResourceInfo resourceInfo in info.Resources) {
                    if (resourceList.Length > 0)
                        resourceList += ", ";

                    IModelComponentLayoutLockResource resource = resourceInfo.GetResource(LayoutPhase.All);
                    resourceList += resource.ToString() + ": " + resource.NameProvider.Name;
                }

                container.AddText("Requires locks on: " + resourceList);
            }

            if (info.Policies.Count > 0)
                container.AddVerticalSection(new PopupWindowPoliciesSection("Activate on train entry:", info.Policies, LayoutModel.StateManager.BlockInfoPolicies));
        }

        #endregion

        #region Text component popup window handler

        [LayoutEvent("add-operation-details-window-sections", SenderType = typeof(LayoutTextComponent))]
        [LayoutEvent("add-editing-details-window-sections", SenderType = typeof(LayoutTextComponent))]
        private void addTextComponentSection(LayoutEvent e) {
            LayoutTextComponent textComponent = (LayoutTextComponent)e.Sender;
            PopupWindowContainerSection container = (PopupWindowContainerSection)e.Info;

            container.AddText(textComponent.TextProvider.Name);
        }

        #endregion

    }
}
