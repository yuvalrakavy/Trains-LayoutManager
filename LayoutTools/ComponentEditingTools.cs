using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using LayoutManager.View;
using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

#nullable enable
namespace LayoutManager.Tools {
    /// <summary>
    /// Summary description for ComponentTools.
    /// </summary>
    [LayoutModule("Components Editing Tools", UserControl = false)]
    public class ComponentEditingTools : ILayoutModuleSetup {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "<Pending>")]
        public static string ComponentEditingToolsVersion = "1.0";

        /// <summary>
        /// Required designer variable.
        /// </summary>

        [DispatchTarget]
        private bool QueryCanRemoveModelComponent([DispatchFilter] LayoutTrackComponent component) => false;        // Track cannot be removed by itself

        [Flags]
        private enum PropertyDialogOptions {
            None = 0,
            AlsoOperationalMode = 0x0001,
            Modeless = 0x0002,
            NotOpenOnPlacement = 0x0004,        // Do not open when placing new component
        }

        private class PropertiesDialogMapEntry {
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
        /// Map between component type and the component properties dialog. If no special treatment is needed,
        /// all you need to do in order to implement showing a property sheet when the component is placed, and
        /// adding a properties context menu item is to add the appropriate entry to this map.
        /// </summary>
        private readonly PropertiesDialogMapEntry[] propertiesDialogMap =
            new PropertiesDialogMapEntry[] {
                                               new PropertiesDialogMapEntry(typeof(LayoutTextComponent), typeof(Dialogs.TextProperties), PropertyDialogOptions.AlsoOperationalMode),
                                               new PropertiesDialogMapEntry(typeof(LayoutTrackContactComponent), typeof(Dialogs.TriggerableBlockEdgeProperties), PropertyDialogOptions.NotOpenOnPlacement),
                                               new PropertiesDialogMapEntry(typeof(LayoutProximitySensorComponent), typeof(Dialogs.TriggerableBlockEdgeProperties), PropertyDialogOptions.NotOpenOnPlacement),
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

        [DispatchTarget]
        bool RequestModelComponentPlacement(ModelComponent component, PlacementInfo placement) {
            foreach (PropertiesDialogMapEntry entry in propertiesDialogMap) {
                if (entry.componentType.IsInstanceOfType(component) && (entry.options & PropertyDialogOptions.NotOpenOnPlacement) == 0) {
                    var constructor = entry.propertiesDialogType.GetConstructor(new Type[] { typeof(ModelComponent), typeof(PlacementInfo) });
                    ILayoutComponentPropertiesDialog? dialog;

                    if (constructor != null) {
                        dialog = (ILayoutComponentPropertiesDialog?)entry.propertiesDialogType.Assembly?.CreateInstance(
                            entry.propertiesDialogType.FullName!, false,
                            BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, new Object[] { component, placement }, null, Array.Empty<Object>());
                    }
                    else {
                        constructor = entry.propertiesDialogType.GetConstructor(new Type[] { typeof(ModelComponent) });

                        if (constructor != null) {
                            dialog = (ILayoutComponentPropertiesDialog?)entry.propertiesDialogType.Assembly.CreateInstance(
                                entry.propertiesDialogType.FullName!, false,
                                BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, new Object[] { component }, null, Array.Empty<Object>());
                        }
                        else
                            throw new ArgumentException("Invalid dialog class constructor (should be cons(LayoutModel, ModelComponent, [XmlElement]))");
                    }

                    if (dialog != null && dialog.ShowDialog() == DialogResult.OK) {
                        component.XmlInfo.XmlDocument = dialog.XmlInfo.XmlDocument;
                        return true;      // Place component
                    }
                    else
                        return false;     // Do not place component
                }
            }

            return true;        // No property dialog was found, so no dialog is canceled
        }

        [DispatchTarget(Order = 500)]
        private void AddComponentContextMenuEntries(Guid frameWindowId, ModelComponent component, MenuOrMenuItem menu) {
            foreach (PropertiesDialogMapEntry entry in propertiesDialogMap) {
                if (entry.componentType.IsInstanceOfType(component)) {
                    if (!LayoutController.IsOperationMode || (entry.options & PropertyDialogOptions.AlsoOperationalMode) != 0) {
                        menu.Items.Add(new MenuItemProperties(component, entry.propertiesDialogType));
                        break;
                    }
                }
            }
        }

        [DispatchTarget]
        private bool IncludeInComponentContextMenu(ModelComponent component) {
            foreach (PropertiesDialogMapEntry entry in propertiesDialogMap) {
                if (entry.componentType.IsInstanceOfType(component)) {
                    if (!LayoutController.IsOperationMode || (entry.options & PropertyDialogOptions.AlsoOperationalMode) != 0) {
                        return true;
                    }
                }
            }

            return false;
        }

#endregion

#region Testing component

        private static bool CanTestComponent(IModelComponentConnectToControl connectableComponent) {
            if (connectableComponent.FullyConnected) {
                var connectionPoints = LayoutModel.ControlManager.ConnectionPoints[connectableComponent.Id];

                if (connectionPoints != null) {
                    foreach (ControlConnectionPoint ccp in connectionPoints) {
                        if (ccp.Usage == ControlConnectionPointUsage.Output)
                            return true;
                    }
                }
            }

            return false;
        }

        [DispatchTarget(Order = 50)]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] IModelComponentConnectToControl component, MenuOrMenuItem menu) {
            if (CanTestComponent(component)) {
                var item = new LayoutMenuItem("&Test", null, (object? sender, EventArgs ea) => Dispatch.Call.TestLayoutObjectRequest(frameWindowId, component));

                if (Dispatch.Call.QueryTestLayoutObject())
                    item.Enabled = false;
                menu.Items.Add(item);
            }
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] IModelComponentConnectToControl component) => CanTestComponent(component);

        [DispatchTarget]
        private Form GetTestComponentDialog(Guid frameWindowId, [DispatchFilter] LayoutThreeWayTurnoutComponent component) {
            return new Dialogs.TestThreeWayTurnout(frameWindowId, component);
        }

        [DispatchTarget]
        private Form GetTestComponentDialog(Guid frameWindowId, [DispatchFilter] IModelComponentConnectToControl control) {
            return new LayoutManager.Tools.Dialogs.TestLayoutObject(frameWindowId, control);
        }

        [DispatchTarget]
        private Form GetTestComponentDialog(Guid frameWindowId, [DispatchFilter] ControlConnectionPoint connectionPoint) {
            return connectionPoint.Component != null && Dispatch.Call.GetTestComponentDialog(frameWindowId, connectionPoint.Component) is Form dialog
                ? dialog
                : new LayoutManager.Tools.Dialogs.TestLayoutObject(frameWindowId, new ControlConnectionPointReference(connectionPoint));
        }

        [DispatchTarget]
        private Form GetTestComponentDialog(Guid frameWindowId, [DispatchFilter] ControlConnectionPointReference connectionPointRef) {
            return connectionPointRef.ConnectionPoint.Component != null && Dispatch.Call.GetTestComponentDialog(frameWindowId, connectionPointRef.ConnectionPoint.Component) is Form dialog
                ? dialog
                : new LayoutManager.Tools.Dialogs.TestLayoutObject(frameWindowId, connectionPointRef);
        }

        [DispatchTarget]
        private void TestLayoutObjectRequest(Guid frameWindowId, object testObject) {
            try {
                if (LayoutController.Instance.BeginDesignTimeActivation()) {
                    Form? testDialog = Dispatch.Call.GetTestComponentDialog(frameWindowId, testObject);

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

        [DispatchTarget]
        private void AddDetailsWindowSections_TrackLink([DispatchFilter] LayoutTrackLinkComponent trackLinkComponent, PopupWindowContainerSection container) {
            var textProvider = new LayoutTrackLinkTextInfo(trackLinkComponent, "Name");

            container.AddText("Track link: " + textProvider.Name);

            if (trackLinkComponent.LinkedComponent != null) {
                LayoutTrackLinkComponent linkedComponent = trackLinkComponent.LinkedComponent;
                LayoutTrackLinkTextInfo linkedTextProvider = new(linkedComponent, "Name");

                container.AddText($"Linked to: {linkedTextProvider.Name}");
            }
            else
                container.AddText("Not yet linked");
        }

        [DispatchTarget]
        bool RequestModelComponentPlacement([DispatchFilter] LayoutTrackLinkComponent component, PlacementInfo placement) {
            var trackLinkProperties = new Dialogs.TrackLinkProperties(placement.Area, component);

            if (trackLinkProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = trackLinkProperties.XmlInfo.XmlDocument;

                LayoutCompoundCommand addCommand = new("Add track-link") {
                    new LayoutComponentPlacmentCommand(placement.Area, placement.Location, component, "Add track link", placement.Area.Phase(placement.Location))
                };

                if (trackLinkProperties.TrackLink != null) {
                    LayoutTrackLinkComponent destinationTrackLinkComponent = trackLinkProperties.TrackLink.ResolveLink();

                    if (destinationTrackLinkComponent.Link != null)
                        addCommand.Add(new LayoutComponentUnlinkCommand(destinationTrackLinkComponent));

                    addCommand.Add(new LayoutComponentLinkCommand(component, trackLinkProperties.TrackLink));
                }

                LayoutController.Do(addCommand);
            }

            return false;         // Do not place component, it is already placed
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutTrackLinkComponent component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutTrackLinkComponent component, MenuOrMenuItem menu) {
            menu.Items.Add(new TrackLinkMenuItemProperties(component));
        }

        [DispatchTarget]
        private void AddContextMenuTopEntries(Guid frameWindowId, [DispatchFilter] LayoutTrackLinkComponent component, MenuOrMenuItem menu) {
            var item = new TrackLinkMenuItemViewLinkedComponent(frameWindowId, component) {
                DefaultItem = true
            };

            menu.Items.Add(item);
        }

        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddContextMenuTopEntries(Guid frameWindowId, [DispatchFilter] LayoutTrackPowerConnectorComponent component, MenuOrMenuItem menu) {

            menu.Items.Add(new LayoutMenuItem("Assign as block resource", null,
                (sender, ea) => {
                    LayoutPhase checkPhases = LayoutPhase.None;
                    LayoutCompoundCommand? command = null;

                    switch (component.Spot.Phase) {
                        case LayoutPhase.Operational: checkPhases = LayoutPhase.Operational; break;
                        case LayoutPhase.Construction: checkPhases = LayoutPhase.Operational | LayoutPhase.Construction; break;
                        case LayoutPhase.Planned: checkPhases = LayoutPhase.All; break;
                    }

                    if(Dispatch.Call.CheckLayout(checkPhases)) {
                        Dispatch.Call.AddPowerConnectorAsResource(component,
                            blockDefinition => {
                                LayoutXmlInfo xmlInfo = new(blockDefinition);
                                var newBlockDefinitionInfo = new LayoutBlockDefinitionComponentInfo(blockDefinition, xmlInfo.Element);
                                command ??= new LayoutCompoundCommand("Assign power connector as resource");

                                newBlockDefinitionInfo.Resources.Add(component.Id);
                                command.Add(new LayoutModifyComponentDocumentCommand(blockDefinition, xmlInfo));
                            }
                        );

                        if (command != null)
                            LayoutController.Do(command);
                    }
                }
            ));
        }

        [DispatchTarget]
        private bool QueryEditingDefaultAction_TrackLink([DispatchFilter] LayoutTrackLinkComponent component) => true;

        [DispatchTarget]
        private void EditingDefaultActionCommand_TrackLink([DispatchFilter] LayoutTrackLinkComponent trackLink, LayoutHitTestResult hitTestResult) {
            var linkedComponent = trackLink.LinkedComponent;

            if (linkedComponent != null)
                Dispatch.Call.EnsureComponentVisible(hitTestResult.FrameWindow.Id, linkedComponent, true);
        }

        [DispatchTarget]
        void DefaultActionCommand_TrackLink([DispatchFilter] LayoutTrackLinkComponent component, LayoutHitTestResult hitTestResult) {
            var linkedComponent = component.LinkedComponent;

            if (linkedComponent != null)
                Dispatch.Call.EnsureComponentVisible(LayoutController.ActiveFrameWindow.Id, linkedComponent, true);
        }

#region Track Link Menu item classes

        private class TrackLinkMenuItemProperties : LayoutMenuItem {
            private readonly LayoutTrackLinkComponent trackLinkComponent;

            internal TrackLinkMenuItemProperties(LayoutTrackLinkComponent trackLinkComponent) {
                this.trackLinkComponent = trackLinkComponent;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                Dialogs.TrackLinkProperties trackLinkProperties = new(trackLinkComponent.Spot.Area, trackLinkComponent);

                if (trackLinkProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    LayoutCompoundCommand editProperties = new("edit " + trackLinkComponent.ToString() + " properties");

                    LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                        new(trackLinkComponent, trackLinkProperties.XmlInfo);

                    editProperties.Add(modifyComponentDocumentCommand);

                    // If this component is to be linked
                    if (trackLinkProperties.TrackLink != null) {
                        // Unlink this component if it is already linked
                        if (trackLinkComponent.Link != null) {
                            LayoutComponentUnlinkCommand unlinkCommand = new(trackLinkComponent);

                            editProperties.Add(unlinkCommand);
                        }

                        LayoutTrackLinkComponent destinationTrackLinkComponent = trackLinkProperties.TrackLink.ResolveLink();

                        // Unlink destination component if it is linked
                        if (destinationTrackLinkComponent.Link != null) {
                            LayoutComponentUnlinkCommand unlinkCommand = new(destinationTrackLinkComponent);

                            editProperties.Add(unlinkCommand);
                        }

                        // Link to the destination component
                        LayoutComponentLinkCommand linkCommand = new(trackLinkComponent, trackLinkProperties.TrackLink);

                        editProperties.Add(linkCommand);
                    }
                    else {
                        // If component was linked, remove the link
                        if (trackLinkComponent.Link != null) {
                            LayoutComponentUnlinkCommand unlinkCommand = new(trackLinkComponent);

                            editProperties.Add(unlinkCommand);
                        }
                    }

                    LayoutController.Do(editProperties);
                }
            }
        }

        private class TrackLinkMenuItemViewLinkedComponent : LayoutMenuItem {
            internal TrackLinkMenuItemViewLinkedComponent(Guid frameWindowId, LayoutTrackLinkComponent trackLinkComponent)
                : base("&Show other link", null, (s, ea) => {
                    if (trackLinkComponent.LinkedComponent != null)
                        Dispatch.Call.EnsureComponentVisible(frameWindowId, trackLinkComponent.LinkedComponent, true);
                }) {
                if (trackLinkComponent.LinkedComponent == null)
                    this.Enabled = false;
            }
        }

#endregion

#endregion

#region Block Edge component

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutBlockEdgeBase component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutBlockEdgeBase component, MenuOrMenuItem menu) {
            var myBindDialog = new List<IModelComponentReceiverDialog>();
            var allBindDialog = new List<IModelComponentReceiverDialog>();

            Dispatch.Call.QueryBindSignalsDialogs(component, myBindDialog);
            Dispatch.Call.QueryBindSignalsDialogs(null, allBindDialog);

            var item = new BlockEdgeBindSignalMenuItem(component);

            if (myBindDialog.Count == 0 && allBindDialog.Count > 0)
                item.Enabled = false;       // Another track contact bind dialog is open

            menu.Items.Add(item);
        }

        internal class BlockEdgeBindSignalMenuItem : LayoutMenuItem {
            private readonly LayoutBlockEdgeBase blockEdge;

            public BlockEdgeBindSignalMenuItem(LayoutBlockEdgeBase blockEdge) {
                this.blockEdge = blockEdge;

                Text = "&Link Signals";
            }

            protected override void OnClick(EventArgs e) {
                var myBindDialog = new List<IModelComponentReceiverDialog>();

                Dispatch.Call.QueryBindSignalsDialogs(blockEdge, myBindDialog);

                if (myBindDialog.Count > 0) {
                    Form f = (Form)myBindDialog[0];

                    f.Activate();
                }
                else {
                    Dialogs.BindBlockEdgeToSignals dialog = new(blockEdge);

                    dialog.Show();
                }
            }
        }

#endregion

#region Signal component

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutSignalComponent component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutSignalComponent component, MenuOrMenuItem menu) {
            var allBindDialog = new List<IModelComponentReceiverDialog>();

            Dispatch.Call.QueryBindSignalsDialogs(null, allBindDialog);

            if (allBindDialog.Count > 0) {
                var item = new SignalBindSignalMenuItem((IModelComponentReceiverDialog)allBindDialog[0], component);
                menu.Items.Add(item);
            }
        }

        internal class SignalBindSignalMenuItem : LayoutMenuItem {
            private readonly LayoutSignalComponent signal;
            private readonly IModelComponentReceiverDialog dialog;

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

        private class ShiftLimit {
            public int From;
            public int To;

            public ShiftLimit(int v1, int v2) {
                From = Math.Min(v1, v2);
                To = Math.Max(v1, v2);
            }

            public bool Contains(int v) => From <= v && v <= To;
        }

        private class ShiftComponentsMenuEntryBase : LayoutMenuItem {
            protected enum OperationType {
                ShiftAreaComponents, MoveSelection
            };

            private readonly string? directionText;

            protected ShiftComponentsMenuEntryBase(OperationType operation, LayoutHitTestResult hitTestResult, string directionText, ChangeCoordinate changeWhat, CompareValue.Order sortOrder) {
                this.Operation = operation;
                this.HitTestResult = hitTestResult;
                this.directionText = directionText;
                this.ChangeWhat = changeWhat;
                this.SortOrder = sortOrder;

                Text = directionText;
            }

            protected ShiftComponentsMenuEntryBase(OperationType operation, LayoutHitTestResult hitTestResult, string text) {
                this.Operation = operation;
                this.HitTestResult = hitTestResult;
                this.ChangeWhat = ChangeCoordinate.Prompt;
                this.SortOrder = CompareValue.Order.Ascending;
                this.Text = text;
            }

            protected OperationType Operation { get; } = OperationType.ShiftAreaComponents;

            protected ChangeCoordinate ChangeWhat { get; set; }

            protected CompareValue.Order SortOrder { get; set; }

            protected string OperationText => Operation == OperationType.MoveSelection ? "move selection " + (directionText ?? "??") : "shift components " + (directionText ?? "??");

            public LayoutHitTestResult HitTestResult { get; }

            public int Delta { get; protected set; }

            public bool FillGaps { get; set; } = true;

            protected class CompareValue : IComparer<int> {
                public enum Order {
                    Ascending, Decending
                }

                private readonly Order compareOrder;

                public CompareValue(Order compareOrder) {
                    this.compareOrder = compareOrder;
                }

#region IComparer<int> Members

                public int Compare(int x, int y) {
                    return compareOrder == Order.Ascending ? x - y : y - x;
                }

#endregion
            }

            private class ZorderSorter : IComparer {
                public int Compare(Object? x, Object? y) => (Ensure.NotNull<ModelComponent>(y)).ZOrder - (Ensure.NotNull<ModelComponent>(x)).ZOrder;
            }

            protected enum ChangeCoordinate {
                Prompt, ChangeX, ChangeY
            }

            protected LayoutCompoundCommand DoShift(SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>> majorAxis, string commandName, ChangeCoordinate chageWhat) {
                LayoutCompoundCommand shiftCommand = new(commandName);

                foreach (SortedList<int, LayoutModelSpotComponentCollection> minorAxis in majorAxis.Values) {
                    foreach (LayoutModelSpotComponentCollection spot in minorAxis.Values) {
                        Point newSpotLocation;

                        if (chageWhat == ChangeCoordinate.ChangeX)
                            newSpotLocation = new Point(spot.Location.X + Delta, spot.Location.Y);
                        else
                            newSpotLocation = new Point(spot.Location.X, spot.Location.Y + Delta);

                        var existingSpot = spot.Area[newSpotLocation, LayoutPhase.All];

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
                                    Dispatch.Call.PrepareForComponentRemoveCommand(components[i], shiftCommand);
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
                SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>> majorAxis = new(new CompareValue(CompareValue.Order.Ascending));

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

                        if (!majorAxis.TryGetValue(majorAxisIndex, out SortedList<int, LayoutModelSpotComponentCollection>? minorAxis)) {
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
                LayoutSelection selection = new();
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

                    if (area.Grid.TryGetValue(new Point(movedSpot.Location.X, fixedSpotY), out LayoutModelSpotComponentCollection? fixedSpot)) {
                        // Check that the track layer of the top spot has a B connection point and that the 
                        // track layer of the bottom spot has a T connection point. If this is the case,
                        // add a vertical straight track to connect the two.
                        var movedTrack = movedSpot.Track;
                        var fixedTrack = fixedSpot.Track;

                        if (movedTrack != null && fixedTrack != null) {
                            if (movedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.B) && fixedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.T)) {
                                int inc = Delta < 0 ? -1 : 1;
                                int count = Math.Abs(Delta);

                                for (int i = 0; i < count; i++) {
                                    LayoutTrackComponent newTrack = new LayoutStraightTrackComponent(LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B);

                                    shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(movedSpot.Location.X, y), newTrack, "track", movedSpot.Phase));

                                    if (movedTrack.TrackBackground is LayoutBridgeComponent _) {
                                        var bridge = new LayoutBridgeComponent();
                                        shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(movedSpot.Location.X, y), bridge, "bridge", movedSpot.Phase));
                                    }

                                    if (movedTrack.TrackBackground is LayoutTunnelComponent _) {
                                        var tunnel = new LayoutTunnelComponent();
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

                    if (area.Grid.TryGetValue(new Point(fixedSpotX, movedSpot.Location.Y), out LayoutModelSpotComponentCollection? fixedSpot)) {
                        // Check that the track layer of the moved spot has a R connection point and that the 
                        // track layer of the fixed spot has a L connection point. If this is the case,
                        // add a horizontal straight track to connect the two.
                        var movedTrack = movedSpot.Track;
                        var fixedTrack = fixedSpot.Track;

                        if (movedTrack != null && fixedTrack != null) {
                            if (movedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.R) && fixedTrack.HasConnectionPoint(LayoutComponentConnectionPoint.L)) {
                                int inc = Delta < 0 ? -1 : 1;
                                int count = Math.Abs(Delta);

                                for (int i = 0; i < count; i++) {
                                    LayoutTrackComponent newTrack = new LayoutStraightTrackComponent(LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R);

                                    shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(x, movedSpot.Location.Y), newTrack, "track", movedSpot.Phase));

                                    if (movedTrack.TrackBackground is LayoutBridgeComponent _) {
                                        var bridge = new LayoutBridgeComponent();
                                        shiftCommand.Add(new LayoutComponentPlacmentCommand(area, new Point(x, movedSpot.Location.Y), bridge, "bridge", movedSpot.Phase));
                                    }

                                    if (movedTrack.TrackBackground is LayoutTunnelComponent _) {
                                        var tunnel = new LayoutTunnelComponent();
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
                Dictionary<int, int> boundries = new();

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

            public void DoOperation(int moveBy, bool _) {
                Delta = SortOrder == CompareValue.Order.Ascending ? -moveBy : moveBy;

                LayoutSelection selection = (Operation == OperationType.ShiftAreaComponents) ? GetAreaSelection() : LayoutController.UserSelection;
                SortedList<int, SortedList<int, LayoutModelSpotComponentCollection>> majorAxis = GetSelectionSpotList(selection, ChangeWhat, SortOrder);
                LayoutCompoundCommand shiftCommand = DoShift(majorAxis, OperationText, ChangeWhat);

                if (FillGaps) {
                    IDictionary<int, int> bounderies = FindBoundries(selection);

                    if (ChangeWhat == ChangeCoordinate.ChangeY)
                        FillHorizontalGap(bounderies, shiftCommand);
                    else
                        FillVerticalGap(bounderies, shiftCommand);
                }

                shiftCommand.Add(new RedrawLayoutModelAreaCommand(HitTestResult.Area));
                LayoutController.Do(shiftCommand);
            }

            protected override void OnClick(EventArgs e) {
                if (ChangeWhat == ChangeCoordinate.Prompt) {
                    Dialogs.ShiftOrMove d = new() {
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

        private class ShiftComponentsUpMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsUpMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "up", ChangeCoordinate.ChangeY, CompareValue.Order.Ascending) {
            }
        }

        private class ShiftComponentsDownMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsDownMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "down", ChangeCoordinate.ChangeY, CompareValue.Order.Decending) {
            }
        }

        private class ShiftComponentsLeftMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsLeftMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "left", ChangeCoordinate.ChangeX, CompareValue.Order.Ascending) {
            }
        }

        private class ShiftComponentsRightMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsRightMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "right", ChangeCoordinate.ChangeX, CompareValue.Order.Decending) {
            }
        }

        private class ShiftComponentsByMenuEntry : ShiftComponentsMenuEntryBase {
            public ShiftComponentsByMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.ShiftAreaComponents, hitTestResult, "By...") {
            }
        }

        private class MoveSelectionUpMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionUpMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "up", ChangeCoordinate.ChangeY, CompareValue.Order.Ascending) {
            }
        }

        private class MoveSelectionDownMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionDownMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "down", ChangeCoordinate.ChangeY, CompareValue.Order.Decending) {
            }
        }

        private class MoveSelectionLeftMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionLeftMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "left", ChangeCoordinate.ChangeX, CompareValue.Order.Ascending) {
            }
        }

        private class MoveSelectionRightMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveSelectionRightMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "right", ChangeCoordinate.ChangeX, CompareValue.Order.Decending) {
            }
        }

        private class MoveComponentsByMenuEntry : ShiftComponentsMenuEntryBase {
            public MoveComponentsByMenuEntry(LayoutHitTestResult hitTestResult) :
                base(OperationType.MoveSelection, hitTestResult, "By...") {
            }
        }

#endregion

        internal class ShiftComponentsMenuEntry : LayoutMenuItem {
            public ShiftComponentsMenuEntry(LayoutHitTestResult hitTestResult) {
                this.Text = "&Shift Components";

                DropDownItems.Add(new ShiftComponentsByMenuEntry(hitTestResult));
                DropDownItems.Add(new ToolStripSeparator());
                DropDownItems.Add(new ShiftComponentsUpMenuEntry(hitTestResult));
                DropDownItems.Add(new ShiftComponentsDownMenuEntry(hitTestResult));
                DropDownItems.Add(new ShiftComponentsLeftMenuEntry(hitTestResult));
                DropDownItems.Add(new ShiftComponentsRightMenuEntry(hitTestResult));
            }
        }

        internal class MoveSelectionMenuEntry : LayoutMenuItem {
            public MoveSelectionMenuEntry(LayoutHitTestResult hitTestResult) {
                this.Text = "&Move selection";

                DropDownItems.Add(new MoveComponentsByMenuEntry(hitTestResult));
                DropDownItems.Add(new ToolStripSeparator());
                DropDownItems.Add(new MoveSelectionUpMenuEntry(hitTestResult));
                DropDownItems.Add(new MoveSelectionDownMenuEntry(hitTestResult));
                DropDownItems.Add(new MoveSelectionLeftMenuEntry(hitTestResult));
                DropDownItems.Add(new MoveSelectionRightMenuEntry(hitTestResult));
            }
        }

        [DispatchTarget(Order = 100)]
        [DispatchFilter("InDesignMode")]
        private void AddComponentContextEmptySpotEntries(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            menu.Items.Add(new ShiftComponentsMenuEntry(hitTestResult));
        }

        [DispatchTarget(Order =100)]
        [DispatchFilter("InDesignMode")]
        private void AddSelectionMenuEntries_ShiftComponents(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            menu.Items.Add(new ShiftComponentsMenuEntry(hitTestResult));
        }

        [DispatchTarget(Order = 100)]
        [DispatchFilter("InDesignMode")]
        private void AddSelectionMenuEntries_Move(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            menu.Items.Add(new MoveSelectionMenuEntry(hitTestResult));
        }

#endregion

#region Count Components

        [DispatchTarget(Order = 101)]
        [DispatchFilter("InDesignMode")]
        private void AddSelectionMenuEntries(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            menu.Items.Add("Count Components", null, (sender, ea) => {
                Dictionary<string, int[]> counts = new();
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

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] LayoutControlModuleLocationComponent component) => true;

        [DispatchTarget]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] LayoutControlModuleLocationComponent component, MenuOrMenuItem menu) {
            menu.Items.Add(new ShowControlModuleLocationMenuItem(component));
        }

        private class ShowControlModuleLocationMenuItem : LayoutMenuItem {
            private readonly LayoutControlModuleLocationComponent component;

            public ShowControlModuleLocationMenuItem(LayoutControlModuleLocationComponent component) {
                this.component = component;

                Text = "&Show Control Modules";
                DefaultItem = true;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                Dispatch.Call.ShowControlModuleLocation(component);
            }
        }

        [DispatchTarget]
        private bool QueryEditingDefaultAction_ModuleLocation([DispatchFilter] LayoutControlModuleLocationComponent component) => true;

        [DispatchTarget]
        private void EditingDefaultActionCommand_ControlModuleLocation([DispatchFilter] LayoutControlModuleLocationComponent component, LayoutHitTestResult hitTestResult) {
            Dispatch.Call.ShowControlModuleLocation(component);
        }

        [DispatchTarget]
        void DefaultActionCommand_ModuleLocation([DispatchFilter] LayoutControlModuleLocationComponent component, LayoutHitTestResult hitTestResult) {
            Dispatch.Call.ShowControlModuleLocation(component);
        }

#endregion

#region Component independent handing of Details window

        [DispatchTarget(Order = 10)]
        private void AddDetailsWindowSections_HasName([DispatchFilter] IModelComponentHasName component, PopupWindowContainerSection container) {
            string name = component.NameProvider.Name;

            if (!string.IsNullOrEmpty(name))
                container.AddVerticalSection(new PopupWindowTextSection(new Font("Arial", 8, FontStyle.Bold), name));
        }

        [DispatchTarget(Order = 100)]
        private void AddDetailsWindowSections_ConnectToControl([DispatchFilter] IModelComponentConnectToControl component, PopupWindowContainerSection container) {

            if (component is IModelComponentIsMultiPath) {
                PopupWindowContainerSection infoContainer = container.CreateContainer();

                AddControlledComponentDetails(component, infoContainer);

                container.AddHorizontalSection(new PopupWindowViewZoomSection((View.LayoutView)container.Parent!, component.Location, new Size(32, 32)));
                container.AddHorizontalSection(infoContainer);
            }
            else
                AddControlledComponentDetails(component, container);
        }

        private void AddControlledComponentDetails(IModelComponentConnectToControl component, PopupWindowContainerSection container) {
            var connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component.Id];

            if (connectionPoints != null) {
                foreach (ControlConnectionPoint connectionPoint in connectionPoints)
                    container.AddVerticalSection(new PopupWindowTextSection(connectionPoint.DisplayName + ": " +
                        connectionPoint.Module.ModuleType.GetConnectionPointAddressText(connectionPoint.Module.ModuleType, connectionPoint.Module.Address, connectionPoint.Index, true)));
            }

            if (!component.FullyConnected)
                container.AddVerticalSection(new PopupWindowTextSection("This component needs to be connected"));
        }

#endregion

#region Block Definition details pop-up window handler

        [DispatchTarget(Order = 200)]
        private void AddDetailsWindowSections_BlockDefinition([DispatchFilter] LayoutBlockDefinitionComponent blockDefinition, PopupWindowContainerSection container) {
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
                container.AddText("Trip section boundary");
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

                    container.AddText("Trip section boundary for trains coming from " + from);
                }
            }

            if (info.Length != 100)
                container.AddText("Block length: " + info.Length);

            if (info.Resources.Count > 0) {
                string resourceList = "";

                foreach (var resourceInfo in info.Resources) {
                    if (resourceList.Length > 0)
                        resourceList += ", ";

                    var resource = resourceInfo.GetResource(LayoutPhase.All);
                    if (resource != null)
                        resourceList += resource.ToString() + ": " + resource.NameProvider.Name;
                }

                container.AddText("Requires locks on: " + resourceList);
            }

            if (info.Policies.Count > 0)
                container.AddVerticalSection(new PopupWindowPoliciesSection("Activate on train entry:", info.Policies, LayoutModel.StateManager.BlockInfoPolicies));
        }

#endregion

#region Text component pop-up window handler

        [DispatchTarget]
        private void AddDetailsWindowSections_Text([DispatchFilter] LayoutTextComponent textComponent, PopupWindowContainerSection container) {
            container.AddText(textComponent.TextProvider.Name);
        }

#endregion

    }
}
