using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.CommonUI {
    public delegate bool BuildComponentsMenuComponentFilter<ComponentType>(ComponentType component);

    /// <summary>
    /// Add entries to a menu with names of components of a given type or that implement a given interface
    /// </summary>
    /// <typeparam name="ComponentType">The component type/interface</typeparam>
    /// <typeparam name="MenuItemType">The type of each menu item</typeparam>
    public class BuildComponentsMenu<ComponentType, MenuItemType>
        where ComponentType : IModelComponentHasName
        where MenuItemType : ModelComponentMenuItemBase<ComponentType>, new() {
        private readonly IEnumerable<ComponentType> components;
        private readonly string xPathFilter;
        private readonly EventHandler clickHandler;
        private readonly BuildComponentsMenuComponentFilter<ComponentType> filterHandler;

        /// <summary>
        /// Constrcuct a component menu builder
        /// </summary>
        /// <param name="components">The components to add to the menu (all must be of the given type or implement tha interface)</param>
        /// <param name="xPathFilter">Optional XPath filter, only components that match the filter are added</param>
        /// <param name="filterHandler">Optional filter callback function</param>
        /// <param name="clickHandler">Optiona menu item click handler</param>
        public BuildComponentsMenu(IEnumerable<ComponentType> components, string xPathFilter, BuildComponentsMenuComponentFilter<ComponentType> filterHandler, EventHandler clickHandler) {
            this.components = components;
            this.xPathFilter = xPathFilter;
            this.filterHandler = filterHandler;
            this.clickHandler = clickHandler;
        }

        /// <summary>
        /// Add the entries to the menu
        /// </summary>
        /// <param name="menu">The menu to which entries are to be added</param>
        public void AddComponentMenuItems(Menu menu) {
            Dictionary<LayoutModelArea, List<ComponentType>> areas = new Dictionary<LayoutModelArea, List<ComponentType>>();

            foreach (ComponentType component in components) {
                if (IncludeComponent(component)) {
                    if (!areas.TryGetValue(component.Spot.Area, out List<ComponentType> componentsForArea)) {
                        componentsForArea = new List<ComponentType>();
                        areas.Add(component.Spot.Area, componentsForArea);
                    }

                    componentsForArea.Add(component);
                }
            }

            if (areas.Count == 1) {
                List<ComponentType> componentsInArea = null;

                foreach (List<ComponentType> a in areas.Values)
                    componentsInArea = a;

                Debug.Assert(componentsInArea != null);
                componentsInArea.Sort(delegate (ComponentType c1, ComponentType c2) { return string.Compare(c1.NameProvider.Name, c2.NameProvider.Name); });

                foreach (ComponentType component in componentsInArea)
                    menu.MenuItems.Add(CreateMenuItem(component));
            }
            else if (areas.Count > 1) {
                List<LayoutModelArea> areasList = new List<LayoutModelArea>();

                foreach (LayoutModelArea area in areas.Keys)
                    areasList.Add(area);

                areasList.Sort(delegate (LayoutModelArea a1, LayoutModelArea a2) { return string.Compare(a1.Name, a2.Name); });

                foreach (LayoutModelArea area in areasList) {
                    MenuItem areaMenuItem = new MenuItem(area.Name);
                    List<ComponentType> componentsInArea = areas[area];

                    componentsInArea.Sort(delegate (ComponentType c1, ComponentType c2) { return string.Compare(c1.NameProvider.Name, c2.NameProvider.Name); });

                    foreach (ComponentType component in componentsInArea)
                        areaMenuItem.MenuItems.Add(CreateMenuItem(component));

                    menu.MenuItems.Add(areaMenuItem);
                }
            }
        }

        private MenuItemType CreateMenuItem(ComponentType component) {
            MenuItemType item = new MenuItemType {
                Component = component,
                Text = component.NameProvider.Name
            };

            if (clickHandler != null)
                item.Click += clickHandler;

            return item;
        }

        private bool IncludeComponent(ComponentType component) {
            if (component.NameProvider.Name.Trim() == null)
                return false;

            if (filterHandler != null && !filterHandler(component))
                return false;

            if (xPathFilter != null && !component.Element.CreateNavigator().Matches(xPathFilter))
                return false;

            return true;
        }
    }

    public class BuildComponentsMenu<ComponentType> : BuildComponentsMenu<ComponentType, ModelComponentMenuItemBase<ComponentType>> where ComponentType : class, IModelComponentHasName, IModelComponentHasId {
        /// <summary>
        /// Construct a component menu builder
        /// </summary>
        /// 
        /// <param name="xPathFilter">Optional XPath filter, only components that match the filter are added</param>
        /// <param name="filterHandler">Optional filter callback function</param>
        /// <param name="clickHandler">Optiona menu item click handler</param>
        public BuildComponentsMenu(LayoutPhase phase, string xPathFilter, BuildComponentsMenuComponentFilter<ComponentType> filterHandler, EventHandler clickHandler) :
            base(LayoutModel.Components<ComponentType>(phase), xPathFilter, filterHandler, clickHandler) {
        }

        /// <summary>
        /// Construct a component menu builder
        /// </summary>
        /// 
        /// <param name="filterHandler">Optional filter callback function</param>
        /// <param name="clickHandler">Optiona menu item click handler</param>
        public BuildComponentsMenu(LayoutPhase phase, BuildComponentsMenuComponentFilter<ComponentType> filterHandler, EventHandler clickHandler) :
            base(LayoutModel.Components<ComponentType>(phase), null, filterHandler, clickHandler) {
        }

        /// <summary>
        /// Construct a component menu builder
        /// </summary>
        /// 
        /// <param name="xPathFilter">Optional XPath filter, only components that match the filter are added</param>
        /// <param name="clickHandler">Optiona menu item click handler</param>
        public BuildComponentsMenu(LayoutPhase phase, string xPathFilter, EventHandler clickHandler) :
            base(LayoutModel.Components<ComponentType>(phase), xPathFilter, null, clickHandler) {
        }

        /// <summary>
        /// Construct a component menu builder
        /// </summary>
        /// 
        /// <param name="clickHandler">Optiona menu item click handler</param>
        public BuildComponentsMenu(LayoutPhase phase, EventHandler clickHandler) :
            base(LayoutModel.Components<ComponentType>(phase), null, null, clickHandler) {
        }
    }

    public class ModelComponentMenuItemBase<ComponentType> : MenuItem where ComponentType : IModelComponentHasName {
        public ModelComponentMenuItemBase() {
        }

        public ModelComponentMenuItemBase(ComponentType component) {
            this.Component = component;
            this.Text = component.NameProvider.Name;
        }

        public ComponentType Component { get; set; }
    }

    public delegate void SemiModalDialogClosedHandler(Form dialog, object info);

    public class SemiModalDialog {
        private readonly Form parent;
        private readonly Form dialog;
        private readonly SemiModalDialogClosedHandler onClose = null;
        private readonly object info = null;
        private Timer autoHideTimer = null;

        public SemiModalDialog(Form parent, Form dialog) {
            this.parent = parent;
            this.dialog = dialog;
        }

        public SemiModalDialog(Form parent, Form dialog, SemiModalDialogClosedHandler onClose, object info) {
            this.parent = parent;
            this.dialog = dialog;
            this.onClose = onClose;
            this.info = info;
        }

        public void ShowDialog() {
            if (!parent.Modal) {
                dialog.Owner = parent;
                dialog.Closing += new CancelEventHandler(onDialogClosing);
                dialog.Closed += new EventHandler(onDialogClosed);

                parent.Enabled = false;

                if (AutoHideParent) {
                    autoHideTimer = new Timer {
                        Interval = 750
                    };
                    autoHideTimer.Tick += new EventHandler(onAutoHideTimerTick);
                    autoHideTimer.Start();
                }

                dialog.Show();
            }
            else {
                dialog.ShowDialog();
                onClose?.Invoke(dialog, info);
            }
        }

        public bool AutoHideParent { get; set; } = true;

        private void onDialogClosing(object sender, CancelEventArgs e) {
            dialog.Closing -= new CancelEventHandler(onDialogClosing);

            if (autoHideTimer != null) {
                autoHideTimer.Dispose();
                autoHideTimer = null;
            }

            parent.Visible = true;
            parent.Enabled = true;
            dialog.Owner = null;
            parent.Activate();
        }

        private void onDialogClosed(object sender, EventArgs e) {
            dialog.Closed -= new EventHandler(onDialogClosed);
            onClose?.Invoke(dialog, info);
        }

        private void onAutoHideTimerTick(object sender, EventArgs e) {
            autoHideTimer.Dispose();
            autoHideTimer = null;
            parent.Visible = false;
        }
    }
}