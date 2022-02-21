using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using MethodDispatcher;

using LayoutManager.Model;
using LayoutManager.View;
using System.Collections.Generic;
using LayoutManager.CommonUI;

namespace LayoutManager {
    /// <summary>
    /// Base class for design tools (such as Selection, Track tools)
    /// </summary>
    public abstract class LayoutTool : Component {
        private Point selectionCorner = Point.Empty;
        private LayoutModelArea? selectionCornerArea = null;
        private LayoutSelectionWithUndo? _userSelection = null;

        public LayoutSelectionWithUndo UserSelection => _userSelection ??= (LayoutSelectionWithUndo)LayoutController.UserSelection;

        /// <summary>
        /// Called when the user clicks on the view while using this tool. The default
        /// implementation is to figure out what model component is clicked, and call
        /// the OnModelComponentClick handler
        /// </summary>
        public void LayoutView_ModelComponentClick(object? sender, LayoutViewEventArgs e) {
            if (e.MouseEventArgs == null)
                return;

            if ((e.MouseEventArgs.Button & MouseButtons.Right) != 0)
                ModelComponent_RightClick(e);
            else
                ModelComponent_Click(e);
        }

        /// <summary>
        /// Called to find out whether there is a component that will accept a drop of the dragged object
        /// </summary>
        /// <param name="sender">The view sending this event</param>
        /// <param name="e">EventArgs including: HitTest of which the drop should occur, DragEventArgs in which the allowed drop effect are returned</param>
        public void LayoutView_ModelComponentQueryDrop(object? sender, LayoutViewEventArgs e) {
            if (e.DragEventArgs == null)
                return;

            e.DragEventArgs.Effect = DragDropEffects.None;

            if (e.HitTestResult != null) {
                foreach (ModelComponent component in e.HitTestResult.Selection)
                    Dispatch.Call.QueryDrop(component, e.DragEventArgs);
            }
        }

        /// <summary>
        /// Called when something is dropped
        /// </summary>
        /// <param name="sender">The view sending this event</param>
        /// <param name="e">EventArgs including: Where the drop occurs and what type of operation is to take place</param>
        public void LayoutView_ModelComponentDrop(object? sender, LayoutViewEventArgs e) {
            if (e.HitTestResult == null || e.DragEventArgs == null)
                return;

            foreach (ModelComponent component in e.HitTestResult.Selection)
                Dispatch.Call.DoDrop(component, e.DragEventArgs);
        }

        /// <summary>
        /// Called when user is dragging something.
        /// </summary>
        /// <param name="sender">The event sender (the view in which the user initiated the dragging)</param>
        /// <param name="e">Arguments - including what is being dragged, and mouse events when dragging begun</param>
        public void LayoutView_ModelComponentDragged(object? sender, LayoutViewEventArgs e) {
            object? draggedObject = null;
            DragDropEffects allowedEffects = DragDropEffects.All;

            if (e.HitTestResult != null) {
                foreach (ModelComponent component in e.HitTestResult.Selection) {
                    draggedObject = Dispatch.Call.QueryDrag(component);

                    if(draggedObject != null)
                        break;
                }

                if (draggedObject != null) {
                    if (draggedObject is LayoutDraggedObject draggedObjectDetails) {
                        draggedObject = draggedObjectDetails.DraggedObject;
                        allowedEffects = draggedObjectDetails.AllowedEffects;
                    }

                    DragDropEffects dropEffect = e.HitTestResult.View.DoDragDrop(draggedObject, allowedEffects);
                    Dispatch.Notification.OnDragDone(draggedObject, dropEffect);
                }
            }
        }

        /// <summary>
        /// Called when the user left-click the mouse
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="hitTestResult">Mouse hit information</param>
        public virtual void ModelComponent_Click(LayoutViewEventArgs e) {
            bool applyDefaultAction;

            if (e.HitTestResult == null)
                return;

            applyDefaultAction = TestRegionClick(e.HitTestResult);

            if (applyDefaultAction) {
                // If control key is pressed, simply add this component to the selection
                // otherwise clear selection and show the context menu
                if ((Control.ModifierKeys & Keys.Control) != 0) {
                    // Toggle selection
                    if (LayoutController.UserSelection.ContainsAny(e.HitTestResult.Selection)) {
                        LayoutCompoundCommand deselectCommand = new("deselect component");

                        foreach (ModelComponent component in e.HitTestResult.Selection)
                            if (UserSelection.Contains(component))
                                UserSelection.Remove(component, deselectCommand);

                        LayoutController.Do(deselectCommand);
                    }
                    else {
                        LayoutCompoundCommand selectCommand = new("select component");

                        foreach (ModelComponent component in e.HitTestResult.Selection)
                            UserSelection.Add(component, selectCommand);

                        LayoutController.Do(selectCommand);
                    }
                }
                else {
                    UserSelection.Clear(null);
                    selectionCorner = Point.Empty;
                    selectionCornerArea = null;
                    DefaultAction(e.HitTestResult.View.Area, e.HitTestResult);
                }
            }
        }

        /// <summary>
        /// Called to execute the default click action
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="hitTestResult">Mouse hit information</param>
        protected abstract void DefaultAction(LayoutModelArea area, LayoutHitTestResult hitTestResult);

        protected bool TestRegionClick(LayoutHitTestResult hitTestResult) {
            bool applyDefaultAction = true;

            foreach (var region in hitTestResult.Regions)
                if (region.ClickHandler != null)
                    if (region.ClickHandler())
                        applyDefaultAction = false;

            return applyDefaultAction;
        }

        protected bool TestRegionRightClick(LayoutHitTestResult hitTestResult) {
            bool applyDefaultAction = true;

            foreach (var region in hitTestResult.Regions)
                if (region.RightClickHandler != null)
                    if (region.RightClickHandler())
                        applyDefaultAction = false;

            return applyDefaultAction;
        }

        /// <summary>
        /// Called when the user right click the mouse
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="hitTestResult">Mouse hit information</param>
        public virtual void ModelComponent_RightClick(LayoutViewEventArgs e) {
            if (e.HitTestResult == null)
                return;

            bool applyDefaultAction;
            var area = e.HitTestResult.View.Area;

            applyDefaultAction = TestRegionRightClick(e.HitTestResult);

            if (applyDefaultAction) {
                ContextMenuStrip? menu;
                bool hitSelectionShown = false;

                if ((Control.ModifierKeys & Keys.Control) != 0) {
                    if (selectionCorner != Point.Empty && selectionCornerArea == area) {
                        UserSelection.Add(LayoutPhase.All, area, e.HitTestResult.ModelLocation, selectionCorner);
                        selectionCorner = Point.Empty;
                    }
                    else {
                        selectionCorner = e.HitTestResult.ModelLocation;
                        selectionCornerArea = area;
                    }
                }
                else {
                    // Check if right clicked on a selected component if so, show selection
                    // right click menu (with operations such as Copy/Cut etc.
                    // otherwise, clear the selection and show a component right click menu
                    if (UserSelection.ContainsAny(e.HitTestResult.Selection))
                        menu = GetSelectionContextMenu(area, e.HitTestResult);
                    else {
                        UserSelection.Clear(null);
                        selectionCorner = Point.Empty;
                        selectionCornerArea = null;

                        e.HitTestResult.Selection.Display(new LayoutSelectionLook(Color.LightGray));
                        hitSelectionShown = true;

                        if (e.HitTestResult.Selection.Count == 0)
                            menu = GetEmptySpotMenu(area, e.HitTestResult);
                        else
                            menu = GetSpotContextMenu(area, e.HitTestResult);
                    }

                    if (menu != null)
                        menu.Show(e.HitTestResult.View, e.HitTestResult.ClientLocation);

                    if (hitSelectionShown)
                        e.HitTestResult.Selection.Hide();
                }
            }
        }

        /// <summary>
        /// Get a context menu for a selection. The default implementation returns no menu
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="hitTestResult">Mouse hit information</param>
        /// <returns>A menu to show, or null if no menu</returns>
        protected virtual ContextMenuStrip? GetSelectionContextMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            var contextMenu = new ContextMenuStrip();

            Dispatch.Call.AddSelectionMenuEntries(hitTestResult, new MenuOrMenuItem(contextMenu));
            return contextMenu.Items.Count == 0 ? null : contextMenu;
        }

        /// <summary>
        /// Get a context menu for empty regions.
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="hitTestResult">Mouse hit information</param>
        /// <returns>A menu to show, or null if no menu</returns>
        protected virtual ContextMenuStrip? GetEmptySpotMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            var contextMenu = new ContextMenuStrip();

            Dispatch.Call.AddComponentContextEmptySpotEntries(hitTestResult, new MenuOrMenuItem(contextMenu));

            return contextMenu.Items.Count == 0 ? null : contextMenu;
        }

        /// <summary>
        /// Show the context menu for a a spot.
        /// <</summary>
        /// <remarks>The context menu is composed from
        /// operations that are possible on all the components in that spot which
        /// are in the active layer.
        /// </remarks>
        /// <param name="area">The model area</param>
        /// <param name="hitTestResult">Information about the regions that contains the mouse hot-spot</param>
        protected ContextMenuStrip GetSpotContextMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            var contextMenu = new ContextMenuStrip();
            List<ModelComponent> components = new();

            // Add top level entries

            foreach (ModelComponent component in hitTestResult.Selection) {
                Dispatch.Call.AddContextMenuTopEntries(hitTestResult.FrameWindow.Id, component, new MenuOrMenuItem(contextMenu));

                if(!components.Contains(component) && Dispatch.Call.IncludeInComponentContextMenu(component))
                    components.Add(component);
            }

            if (contextMenu.Items.Count > 0 && components.Count > 0)       // At least one entry was added
                contextMenu.Items.Add(new ToolStripSeparator());

            if (components.Count <= 1) {
                // Only one component contribute context menu, add as part of the context menu
                foreach (ModelComponent component in hitTestResult.Selection) {
                    if (components.Contains(component))
                        Dispatch.Call.AddComponentContextMenuEntries(hitTestResult.FrameWindow.Id, component, new MenuOrMenuItem(contextMenu));

                    var componentCanBeRemoved = LayoutController.IsDesignMode && Dispatch.Call.QueryCanRemoveModelComponent(component);

                    if (componentCanBeRemoved && hitTestResult.Selection.Count > 1) {
                        if (contextMenu.Items.Count > 0)
                            contextMenu.Items.Add(new ToolStripSeparator());
                        contextMenu.Items.Add(new MenuItemDeleteComponent($"&Remove {component}", component));
                    }
                }
            }
            else {
                foreach (ModelComponent component in hitTestResult.Selection) {
                    var componentMenuItem = new LayoutMenuItem(component.ToString());

                    Dispatch.Call.AddComponentContextMenuEntries(hitTestResult.FrameWindow.Id, component, new MenuOrMenuItem(componentMenuItem));

                    var componentCanBeRemoved = LayoutController.IsDesignMode && Dispatch.Call.QueryCanRemoveModelComponent(component);

                    if (componentCanBeRemoved) {
                        if (componentMenuItem.DropDownItems.Count > 0)
                            componentMenuItem.DropDownItems.Add(new ToolStripSeparator());
                        componentMenuItem.DropDownItems.Add(new MenuItemDeleteComponent("&Remove", component));
                    }

                    if (componentMenuItem.DropDownItems.Count > 0)
                        contextMenu.Items.Add(componentMenuItem);
                }
            }

            Dispatch.Call.AddCommonContextMenuEntries(hitTestResult, new MenuOrMenuItem(contextMenu));

            return contextMenu;
        }
    }
}
