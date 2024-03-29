using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using LayoutManager.UIGadgets;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager {
    /// <summary>
    /// This tool is used for editing the layout
    /// </summary>
    public class LayoutEditingTool : LayoutTool {
        private LayoutStraightTrackComponent? lastTrack = null;
        private string? lastCategoryName = null;

        public LayoutEditingTool() {
            InitializeComponent();
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        [DispatchTarget(Order = 0)]
        [DispatchFilter("InDesignMode")]
        private void AddComponentContextEmptySpotEntries(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            menu.Items.Add(new MenuItemPasteComponents(hitTestResult));
        }

        [DispatchTarget(Order = 0)]
        [DispatchFilter("InDesignMode")]
        private void AddCommonContextMenuEntries_DeleteAll(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(new MenuItemDeleteAllComponents(hitTestResult));
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private void AddCommonContextMenuEntries_Clipboard(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
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

        [DispatchTarget(Order = 0)]
        [DispatchFilter("InDesignMode")]
        private void AddSelectionMenuEntries_Delete(LayoutHitTestResult hitTestResults, MenuOrMenuItem menu) {
            menu.Items.Add(new MenuItemDeleteSelection());
        }

        [DispatchTarget(Order = 1000)]
        [DispatchFilter("InDesignMode")]
        private void AddSelectionMenuEntries_Clipboard(LayoutHitTestResult hitTestResult, MenuOrMenuItem menu) {
            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new MenuItemCopySelection(hitTestResult.ModelLocation));
            menu.Items.Add(new MenuItemCutSelection(hitTestResult.ModelLocation));
            menu.Items.Add(new MenuItemPasteComponents(hitTestResult));

            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(GetSetPhaseMenuItem((phaseName, phase) => {
                var command = new LayoutCompoundCommand($"Set selection phase to {phaseName}");

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
                    if (Dispatch.Call.QueryEditingDefaultAction(c)) {
                        componentWithDefaultEditingAction = c;
                        break;
                    }

                if (componentWithDefaultEditingAction != null) {
                    showComponentsMenu = false;
                    Dispatch.Call.EditingDefaultActionCommand(componentWithDefaultEditingAction, hitTestResult);
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

                    Dispatch.Call.GetComponentMenuCategories(categories.DocumentElement!, oldTrack);

                    var childNodes = categories.DocumentElement?.ChildNodes;

                    if (childNodes != null) {
                        foreach (XmlElement categoryElement in childNodes) {
                            Dispatch.Call.GetComponentMenuCategoryItems(oldTrack, categoryElement, categoryElement.GetAttribute("Name"));

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
                    var placement = new PlacementInfo(area, new Point(ml.X, ml.Y));

                    if (Dispatch.Call.RequestModelComponentPlacement(component, placement)) {
                        var command = new LayoutCompoundCommand($"add {component}", true) {
                            new LayoutComponentPlacmentCommand(area, ml, component, $"add {component}", area.Phase(ml))
                        };

                        Dispatch.Notification.OnModelComponentPlacedNotification(component, command, placement);
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

        [DispatchTarget]
        private void ChangePhase(LayoutSelection? inSelection, LayoutPhase phase) {
            LayoutSelection selection = inSelection ?? LayoutController.UserSelection;
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

        protected override void Paint(Graphics g) => Dispatch.Call.PaintImageMenuCategory(categoryElement, g);
    }

    internal class LayoutImageMenuItem : ImageMenuItem {
        private readonly XmlElement itemElement;

        internal LayoutImageMenuItem(XmlElement itemElement) {
            this.itemElement = itemElement;

            this.Tooltip = itemElement.GetAttribute("Tooltip");
        }

        protected override void Paint(Graphics g) {
            Dispatch.Call.PaintImageMenuItem(g, itemElement, itemElement.GetAttribute("Name"));
        }

        public ModelComponent? CreateComponent(ModelComponent? old) {
            var newComponent = Dispatch.Call.CreateModelComponent(itemElement, itemElement.GetAttribute("Name"));
            return newComponent != old ? newComponent : null;
        }
    }
}
