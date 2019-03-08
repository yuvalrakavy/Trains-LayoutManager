using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using LayoutManager.Model;
using LayoutManager.View;
using System.Collections.Generic;

namespace LayoutManager {
    /// <summary>
    /// Base class for design tools (such as Selection, Track tools)
    /// </summary>
    public abstract class LayoutTool : Component {
        Point selectionCorner = Point.Empty;
        LayoutModelArea selectionCornerArea = null;
        LayoutSelectionWithUndo _userSelection = null;

        public LayoutSelectionWithUndo UserSelection {
            get {
                if (_userSelection == null)
                    _userSelection = (LayoutSelectionWithUndo)LayoutController.UserSelection;
                return _userSelection;
            }
        }

        /// <summary>
        /// Called when the user clicks on the view while using this tool. The default
        /// implementation is to figure out what model component is clicked, and call
        /// the OnModelComponentClick handler
        /// </summary>
        public void LayoutView_ModelComponentClick(object sender, LayoutViewEventArgs e) {
            if ((e.MouseEventArgs.Button & MouseButtons.Right) != 0)
                ModelComponent_RightClick(e);
            else
                ModelComponent_Click(e);
        }

        /// <summary>
        /// Called to find out whether there is a component that will accept a drop of the dragged object
        /// </summary>
        /// <param name="sender">The view sending this event</param>
        /// <param name="e">EventArgs including: HitTest of which the drop should occure, DragEventArgs in which the allowed drop effect are returned</param>
        public void LayoutView_ModelComponentQueryDrop(object sender, LayoutViewEventArgs e) {
            e.DragEventArgs.Effect = DragDropEffects.None;

            if (e.HitTestResult != null) {
                foreach (ModelComponent component in e.HitTestResult.Selection)
                    EventManager.Event(new LayoutEvent(QueryDropEventName, component, e.DragEventArgs, null).SetFrameWindow(e.HitTestResult.FrameWindow));
            }
        }

        /// <summary>
        /// Called when something is dropped
        /// </summary>
        /// <param name="sender">The view sending this event</param>
        /// <param name="e">EventArgs including: Where the drop occurs and what type of operation is to take place</param>
        public void LayoutView_ModelComponentDrop(object sender, LayoutViewEventArgs e) {
            foreach (ModelComponent component in e.HitTestResult.Selection)
                EventManager.Event(new LayoutEvent(DropEventName, component, e.DragEventArgs, null).SetFrameWindow(e.HitTestResult.FrameWindow));
        }

        /// <summary>
        /// Called when user is dragging something.
        /// </summary>
        /// <param name="sender">The event sender (the view in which the user initiated the dragging)</param>
        /// <param name="e">Arguments - including what is being dragged, and mouse events when dragging begun</param>
        public void LayoutView_ModelComponentDragged(object sender, LayoutViewEventArgs e) {
            object draggedObject = null;
            DragDropEffects allowedEffects = DragDropEffects.All;

            if (e.HitTestResult != null) {
                foreach (ModelComponent component in e.HitTestResult.Selection) {
                    object info = EventManager.Event(new LayoutEvent(QueryDragEventName, component, e, null).SetFrameWindow(e.HitTestResult.FrameWindow));

                    if (info != e) {
                        draggedObject = info;
                        break;
                    }
                }

                if (draggedObject != null) {

                    if (draggedObject is LayoutDraggedObject draggedObjectDetails) {
                        draggedObject = draggedObjectDetails.DraggedObject;
                        allowedEffects = draggedObjectDetails.AllowedEffects;
                    }

                    DragDropEffects dropEffect = e.HitTestResult.View.DoDragDrop(draggedObject, allowedEffects);

                    EventManager.Event(new LayoutEvent(DragDoneEventName, draggedObject, dropEffect, null).SetFrameWindow(e.HitTestResult.FrameWindow));
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

            applyDefaultAction = TestRegionClick(e.HitTestResult);

            if (applyDefaultAction) {
                // If control key is pressed, simply add this component to the selection
                // otherwise clear selection and show the context menu
                if ((Control.ModifierKeys & Keys.Control) != 0) {
                    // Toggle selection
                    if (LayoutController.UserSelection.ContainsAny(e.HitTestResult.Selection)) {
                        LayoutCompoundCommand deselectCommand = new LayoutCompoundCommand("deselect component");

                        foreach (ModelComponent component in e.HitTestResult.Selection)
                            if (UserSelection.Contains(component))
                                UserSelection.Remove(component, deselectCommand);

                        LayoutController.Do(deselectCommand);
                    }
                    else {
                        LayoutCompoundCommand selectCommand = new LayoutCompoundCommand("select component");

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
            bool applyDefaultAction;
            LayoutModelArea area = e.HitTestResult.View.Area;

            applyDefaultAction = TestRegionRightClick(e.HitTestResult);

            if (applyDefaultAction) {
                ContextMenu menu;
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
        protected virtual ContextMenu GetSelectionContextMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            ContextMenu menu = new ContextMenu();

            EventManager.Event(new LayoutEvent(ComponentContextMenuAddSelectionEntriesEventName, hitTestResult, menu, null).SetFrameWindow(hitTestResult.FrameWindow));

            if (menu.MenuItems.Count == 0)
                return null;
            else
                return menu;
        }

        /// <summary>
        /// Get a context menu for empty regions.
        /// </summary>
        /// <param name="area">The area</param>
        /// <param name="hitTestResult">Mouse hit information</param>
        /// <returns>A menu to show, or null if no menu</returns>
        protected virtual ContextMenu GetEmptySpotMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            ContextMenu menu = new ContextMenu();

            EventManager.Event(new LayoutEvent(ComponentContextMenuAddEmptySpotEntriesEventName, hitTestResult, menu, null).SetFrameWindow(hitTestResult.FrameWindow));

            if (menu.MenuItems.Count == 0)
                return null;
            else
                return menu;
        }

        protected abstract string ComponentContextMenuAddEmptySpotEntriesEventName {
            get;
        }

        /// <summary>
        /// The event to send for adding top level to component context menu
        /// </summary>
        protected abstract string ComponentContextMenuAddTopEntriesEventName {
            get;
        }

        /// <summary>
        /// The event to send for checking if a component has an associated context menu
        /// </summary>
        protected abstract string ComponentContextMenuQueryEventName {
            get;
        }

        /// <summary>
        /// Event to send to check if a component can be removed (without removing all components
        /// in its grid location)
        /// </summary>
        protected abstract string ComponentContextMenuQueryCanRemoveEventName {
            get;
        }

        /// <summary>
        /// Event to send for adding context menu entries
        /// </summary>
        protected abstract string ComponentContextMenuAddEntriesEventName {
            get;
        }

        /// <summary>
        /// Event to send to instruct component to add entries in the bottom of the context menu
        /// </summary>
        protected abstract string ComponentContextMenuAddBottomEntriesEventName {
            get;
        }

        /// <summary>
        /// Event sent to add context menu entries which are common to all components in the spot
        /// </summary>
        protected abstract string ComponentContextMenuAddCommonEntriesEventName {
            get;
        }

        /// <summary>
        /// Event sent to allow component to set its context menu name (This name is used if the spot
        /// contains more than one component, in this case the actions for each component are grouped
        /// as a submenu having the returned name)
        /// </summary>
        protected abstract string ComponentContextMenuQueryNameEventName {
            get;
        }

        protected abstract string ComponentContextMenuAddSelectionEntriesEventName {
            get;
        }

        /// <summary>
        /// Event used to query whether the component on which dragging was initiated has anything to drag
        /// </summary>
        protected abstract string QueryDragEventName {
            get;
        }

        /// <summary>
        /// Event used when dragging was done
        /// </summary>
        protected abstract string DragDoneEventName {
            get;
        }

        protected abstract string QueryDropEventName {
            get;
        }

        protected abstract string DropEventName {
            get;
        }

        /// <summary>
        /// Show the context menu for a a spot.
        /// <</summary>
        /// <remarks>The context menu is composed from
        /// operations that are possible on all the components in that spot which
        /// are in the active layer.
        /// </remarks>
        /// <param name="area">The model area</param>
        /// <param name="hitTestResult">Information about the regions that contains the mouse hotspot</param>
        protected ContextMenu GetSpotContextMenu(LayoutModelArea area, LayoutHitTestResult hitTestResult) {
            ContextMenu menu = new ContextMenu();
            List<ModelComponent> components = new List<ModelComponent>();

            // Add top level entries

            foreach (ModelComponent component in hitTestResult.Selection) {
                if (ComponentContextMenuAddTopEntriesEventName != null)
                    EventManager.Event(new LayoutEvent(ComponentContextMenuAddTopEntriesEventName, component, menu, null));

                object queryResult = EventManager.Event(new LayoutEvent(ComponentContextMenuQueryEventName, component, false, null).SetFrameWindow(hitTestResult.FrameWindow));

                if (queryResult is bool) {
                    if ((bool)queryResult && !components.Contains(component))
                        components.Add(component);
                }
                else if (queryResult is ModelComponent && !components.Contains(component))
                    components.Add((ModelComponent)queryResult);
            }

            if (menu.MenuItems.Count > 0 && components.Count > 0)       // At least one entry was added
                menu.MenuItems.Add("-");

            if (components.Count <= 1) {
                // Only one component contribute context menu, add as part of the context menu
                foreach (ModelComponent component in hitTestResult.Selection) {
                    if (components.Contains(component))
                        EventManager.Event(new LayoutEvent(ComponentContextMenuAddEntriesEventName, component, menu, null).SetFrameWindow(hitTestResult.FrameWindow));

                    bool componentCanBeRemoved = false;

                    if (ComponentContextMenuQueryCanRemoveEventName != null)
                        componentCanBeRemoved = (bool)EventManager.Event(
                            new LayoutEvent(ComponentContextMenuQueryCanRemoveEventName, component, true, null).SetFrameWindow(hitTestResult.FrameWindow));

                    if (componentCanBeRemoved && hitTestResult.Selection.Count > 1) {
                        if (menu.MenuItems.Count > 0)
                            menu.MenuItems.Add("-");
                        menu.MenuItems.Add(new MenuItemDeleteComponent("&Remove " + component.ToString(), component));
                    }
                }
            }
            else {
                foreach (ModelComponent component in hitTestResult.Selection) {
                    String componentMenuName;

                    if (ComponentContextMenuQueryNameEventName != null)
                        componentMenuName = (String)EventManager.Event(
                            new LayoutEvent(ComponentContextMenuQueryNameEventName, sender: component, component.ToString()).SetFrameWindow(hitTestResult.FrameWindow));
                    else
                        componentMenuName = component.ToString();

                    MenuItem componentContextMenu = new MenuItem(componentMenuName);

                    EventManager.Event(new LayoutEvent(ComponentContextMenuAddEntriesEventName, component, componentContextMenu,
                        null).SetFrameWindow(hitTestResult.FrameWindow));

                    bool componentCanBeRemoved = false;

                    if (ComponentContextMenuQueryCanRemoveEventName != null)
                        componentCanBeRemoved = (bool)EventManager.Event(
                            new LayoutEvent(ComponentContextMenuQueryCanRemoveEventName, component, true, null).SetFrameWindow(hitTestResult.FrameWindow));

                    if (componentCanBeRemoved) {
                        if (componentContextMenu.MenuItems.Count > 0)
                            componentContextMenu.MenuItems.Add("-");
                        componentContextMenu.MenuItems.Add(new MenuItemDeleteComponent("&Remove", component));
                    }

                    if (componentContextMenu.MenuItems.Count > 0)
                        menu.MenuItems.Add(componentContextMenu);
                }
            }

            if (ComponentContextMenuAddBottomEntriesEventName != null) {
                foreach (ModelComponent component in hitTestResult.Selection)
                    EventManager.Event(new LayoutEvent(ComponentContextMenuAddBottomEntriesEventName, component, menu, null).SetFrameWindow(hitTestResult.FrameWindow));
            }

            if (ComponentContextMenuAddCommonEntriesEventName != null)
                EventManager.Event(
                    new LayoutEvent(ComponentContextMenuAddCommonEntriesEventName, hitTestResult, menu, null).SetFrameWindow(hitTestResult.FrameWindow));

            return menu;
        }
    }
}
