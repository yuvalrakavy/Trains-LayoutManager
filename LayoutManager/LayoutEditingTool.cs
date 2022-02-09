using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.UIGadgets;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager {
    /// <summary>
    /// This tool is used for editing the layout
    /// </summary>
    public class LayoutEditingTool : LayoutTool {
        private LayoutStraightTrackComponent? lastTrack = null;
        private string? lastCategoryName = null;

        public LayoutEditingTool() {
            InitializeComponent();
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        [LayoutEvent("add-editing-empty-spot-context-menu-entries", Order = 0)]
        private void AddEditingEmptySpotMenuEntries(LayoutEvent e) {
            var hitTestResult = Ensure.NotNull<LayoutHitTestResult>(e.Sender);
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);

            menu.Items.Add(new MenuItemPasteComponents(hitTestResult));
        }

        #region Implement properties for returning various context menu related event names

        protected override string ComponentContextMenuAddTopEntriesEventName => "add-component-editing-context-menu-top-entries";

        protected override string ComponentContextMenuQueryEventName => "query-component-editing-context-menu";

        protected override string ComponentContextMenuQueryCanRemoveEventName => "query-can-remove-model-component";

        protected override string ComponentContextMenuAddEntriesEventName => "add-component-editing-context-menu-entries";

        protected override string ComponentContextMenuAddBottomEntriesEventName => "add-component-editing-context-menu-bottom-entries";

        protected override string ComponentContextMenuAddCommonEntriesEventName => "add-component-editing-context-menu-common-entries";

        protected override string ComponentContextMenuQueryNameEventName => "query-component-editing-context-menu-name";

        protected override string ComponentContextMenuAddEmptySpotEntriesEventName => "add-editing-empty-spot-context-menu-entries";

        protected override string ComponentContextMenuAddSelectionEntriesEventName => "add-editing-selection-menu-entries";

        protected override string QueryDragEventName => "query-editing-drag";

        protected override string DragDoneEventName => "editing-drag-done";

        protected override string QueryDropEventName => "query-editing-drop";

        protected override string DropEventName => "do-editing-drop";

        #endregion

        [LayoutEvent("add-component-editing-context-menu-common-entries", Order = 0)]
        private void AddContextMenuCommonEntries(LayoutEvent e) {
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);
            var hitTestResult = Ensure.NotNull<LayoutHitTestResult>(e.Sender);

            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(new MenuItemDeleteAllComponents(hitTestResult));
        }

        [LayoutEvent("add-component-editing-context-menu-common-entries", Order = 200)]
        private void AddClipboardCommonEntries(LayoutEvent e) {
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);
            var hitTestResult = Ensure.NotNull<LayoutHitTestResult>(e.Sender);

            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new MenuItemCopyComponent(hitTestResult));
            menu.Items.Add(new MenuItemCutComponent(hitTestResult));
            menu.Items.Add(new MenuItemPasteComponents(hitTestResult));

            var spot = (from component in hitTestResult.Selection.Components select component.Spot).FirstOrDefault();

            if (spot != null) {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(GetSetPhaseMenuItem((phaneName, phase) => LayoutController.Do(new ChangePhaseCommand(spot, phase))));
            }
        }

        [LayoutEvent("add-editing-selection-menu-entries", Order = 0)]
        private void AddSelectionCommandsMenuEntries(LayoutEvent e) {
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);

            menu.Items.Add(new MenuItemDeleteSelection());
        }

        [LayoutEvent("add-editing-selection-menu-entries", Order = 1000)]
        private void AddSelectionClipboardMenuEntries(LayoutEvent e) {
            var hitTestResult = Ensure.NotNull<LayoutHitTestResult>(e.Sender);
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);

            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new MenuItemCopySelection(hitTestResult.ModelLocation));
            menu.Items.Add(new MenuItemCutSelection(hitTestResult.ModelLocation));
            menu.Items.Add(new MenuItemPasteComponents(hitTestResult));

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(GetSetPhaseMenuItem((phaseName, phase) => {
                var command = new LayoutCompoundCommand("Set selection phase to " + phaseName);

                foreach (var spot in (from component in LayoutController.UserSelection.Components select component.Spot).Distinct())
                    command.Add(new ChangePhaseCommand(spot, phase));

                LayoutController.Do(command);
            }));
        }

        protected override void DefaultAction(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            // Create the component
            Point ml = hitTestResult.ModelLocation;
            var spot = area[ml, LayoutPhase.All];
            LayoutTrackComponent? oldTrack = null;
            ModelComponent? component = null;
            bool showComponentsMenu = true;

            if (spot != null) {
                IList<ModelComponent> components = spot.Components;
                ModelComponent? componentWithDefaultEditingAction = null;

                oldTrack = spot.Track;

                foreach (ModelComponent c in components)
                    if (Ensure.ValueNotNull<bool>(EventManager.Event(new LayoutEvent("query-editing-default-action", c, (bool)false).SetFrameWindow(hitTestResult.FrameWindow)))) {
                        componentWithDefaultEditingAction = c;
                        break;
                    }

                if (componentWithDefaultEditingAction != null) {
                    showComponentsMenu = false;
                    EventManager.Event(new LayoutEvent("editing-default-action-command", componentWithDefaultEditingAction, hitTestResult).SetFrameWindow(hitTestResult.FrameWindow));
                }
            }

            if (showComponentsMenu) {
                if (oldTrack == null && (Control.ModifierKeys & Keys.Shift) == 0)
                    // There is not track in that spot. Check if using the last track
                    // will connect to one of its neighbors
                    component = CanUseLast(area, ml);

                if (component == null) {
                    ImageMenu m = new();
                    XmlDocument categories = new();

                    categories.LoadXml("<ComponentMenuCategories />");

                    EventManager.Event(new LayoutEvent("get-component-menu-categories", categories.DocumentElement,
                        oldTrack, null));

                    var childNodes = categories.DocumentElement?.ChildNodes;

                    if (childNodes != null) {
                        foreach (XmlElement categoryElement in childNodes) {
                            EventManager.Event(new LayoutEvent("get-component-menu-category-items", categoryElement,
                                oldTrack, null));

                            // If this category contains at least one item, create the category and add the items
                            if (categoryElement.ChildNodes.Count > 0)
                                m.Categories.Add(new LayoutImageMenuCategory(categoryElement));
                        }
                    }

                    if (lastCategoryName != null) {
                        if (lastCategoryName == "Tracks" && oldTrack is LayoutStraightTrackComponent && (Control.ModifierKeys & Keys.Shift) == 0)
                            m.InitialCategoryName = "ComposedTracks";
                        else
                            m.InitialCategoryName = lastCategoryName;
                    }

                    m.ShiftKeyCategory = "Tracks";

                    var s = (LayoutImageMenuItem?)m.Show(hitTestResult.View, hitTestResult.ClientLocation);

                    if (m.SelectedCategory != null)
                        lastCategoryName = m.SelectedCategory.Name ?? String.Empty;

                    if (s != null) {
                        component = s.CreateComponent(oldTrack);

                        if (component is LayoutStraightTrackComponent straightTrack)
                            lastTrack = straightTrack;
                    }
                }

                if (component != null) {
                    bool placeComponent;
                    var placementXml = $"<PlacementInfo AreaID='{area.AreaGuid}' X='{ml.X}' Y='{ml.Y}' />";

                    placeComponent = Ensure.ValueNotNull<bool>(EventManager.Event(new LayoutEvent("model-component-placement-request", component,
                        true, placementXml)));

                    if (placeComponent) {
                        var command = new LayoutCompoundCommand($"add {component}", true) {
                            new LayoutComponentPlacmentCommand(area, ml, component, $"add {component}", area.Phase(ml))
                        };

                        EventManager.Event(new LayoutEvent("model-component-post-placement-request", component, command, placementXml));
                        LayoutController.Do(command);
                    }
                }
            }
        }

        private LayoutMenuItem GetSetPhaseMenuItem(Action<string, LayoutPhase> doThis) {
            var m = new LayoutMenuItem("Change phase to");

            m.DropDownItems.Add("Operational", null, (s, ea) => doThis("Operational", LayoutPhase.Operational));
            m.DropDownItems.Add("In construction", null, (s, ea) => doThis("Construction", LayoutPhase.Construction));
            m.DropDownItems.Add("Planned", null, (s, ea) => doThis("Planned", LayoutPhase.Planned));

            return m;
        }

        /// <summary>
        /// Check if it make sense to use the last inserted component
        /// </summary>
        /// <param name="area">The model location</param>
        /// <param name="ml">Grid location</param>
        /// <returns>The component (if the last one could be used) or null</returns>
        private ModelComponent? CanUseLast(LayoutModelArea area, Point ml) {
            ModelComponent? result = null;

            if (lastTrack != null) {
                foreach (LayoutComponentConnectionPoint c in lastTrack.ConnectionPoints) {
                    ILayoutConnectableComponent? neighbor = null;
                    Point neighborLocation = ml + LayoutTrackComponent.GetConnectionOffset(c);
                    LayoutModelSpotComponentCollection? neighborSpot = area[neighborLocation, LayoutPhase.All];

                    if (neighborSpot != null)
                        neighbor = neighborSpot.Track as ILayoutConnectableComponent;

                    if (neighbor != null) {
                        LayoutComponentConnectionPoint neighborPoint = LayoutTrackComponent.GetPointConnectingTo(c);
                        LayoutComponentConnectionPoint[] neighborConnectsTo = neighbor.ConnectTo(neighborPoint, LayoutComponentConnectionType.Passage);

                        if (neighborConnectsTo != null) {
                            result = new LayoutStraightTrackComponent(lastTrack.ConnectionPoints[0], lastTrack.ConnectionPoints[1]);
                            break;
                        }
                    }
                }
            }

            return result;
        }

        [LayoutEvent("change-phase")]
        private void ChangePhase(LayoutEvent e0) {
            var e = (LayoutEventInfoValueType<LayoutSelection, LayoutPhase>)e0;
            LayoutSelection selection = e.Sender ?? LayoutController.UserSelection;
            LayoutPhase phase = e.Info;
            string phaseName = "**UNKNOWN**";

            switch (phase) {
                case LayoutPhase.Planned: phaseName = "Plan"; break;
                case LayoutPhase.Construction: phaseName = "Construction"; break;
                case LayoutPhase.Operational: phaseName = "Operational"; break;
            }

            LayoutCompoundCommand? command = null;

            if (selection.Count == 0) {
                if (MessageBox.Show("Set all components to '" + phaseName + "'", "Are you sure?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                    command = new LayoutCompoundCommand("Set all components to '" + phaseName + "'");

                    foreach (var area in LayoutModel.Areas) {
                        foreach (var spot in area.Grid.Values)
                            command.Add(new ChangePhaseCommand(spot, phase));
                    }
                }
            }
            else {
                command = new LayoutCompoundCommand("Set selected components to '" + phaseName + "'");

                foreach (var spot in (from component in selection.Components select component.Spot).Distinct())
                    command.Add(new ChangePhaseCommand(spot, phase));
            }

            if (command != null)
                LayoutController.Do(command);
        }

        private void InitializeComponent() {
        }
    }

    internal class LayoutImageMenuCategory : ImageMenuCategory {
        private readonly XmlElement categoryElement;

        internal LayoutImageMenuCategory(XmlElement categoryElement) {
            this.categoryElement = categoryElement;

            this.Tooltip = categoryElement.GetAttribute("Tooltip");
            this.Name = categoryElement.GetAttribute("Name");

            foreach (XmlElement itemElement in categoryElement.ChildNodes)
                this.Items.Add(new LayoutImageMenuItem(itemElement));
        }

        protected override void Paint(Graphics g) {
            EventManager.Event(new LayoutEvent("paint-image-menu-category", categoryElement, g));
        }
    }

    internal class LayoutImageMenuItem : ImageMenuItem {
        private readonly XmlElement itemElement;

        internal LayoutImageMenuItem(XmlElement itemElement) {
            this.itemElement = itemElement;

            this.Tooltip = itemElement.GetAttribute("Tooltip");
        }

        protected override void Paint(Graphics g) {
            EventManager.Event(new LayoutEvent("paint-image-menu-item", itemElement, g));
        }

        public ModelComponent? CreateComponent(ModelComponent? old) {
            var newComponent = (ModelComponent?)EventManager.Event(new LayoutEvent("create-model-component",
                itemElement, old, null));

            return newComponent != old ? newComponent : null;
        }
    }
}
