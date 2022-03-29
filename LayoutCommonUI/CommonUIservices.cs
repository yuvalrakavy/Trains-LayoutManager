using LayoutManager.Model;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.XPath;

namespace LayoutManager.CommonUI {
    public delegate bool BuildComponentsMenuComponentFilter<ComponentType>(ComponentType component);

    public class LayoutMenuItem : ToolStripMenuItem {
        bool _defaultItem = false;

        public bool DefaultItem {
            get => _defaultItem;
            set {
                _defaultItem = value;
                Font = new System.Drawing.Font(Font, _defaultItem ? (Font.Style | System.Drawing.FontStyle.Bold) : (Font.Style & ~System.Drawing.FontStyle.Bold));
            }
        }

        public LayoutMenuItem() { }
        public LayoutMenuItem(Image img) : base(img) { }
        public LayoutMenuItem(string text) : base(text) { }
        public LayoutMenuItem(string text, Image? img) : base(text, img) { }
        public LayoutMenuItem(string text, Image? img, EventHandler onClick) : base(text, img, onClick) { }
        public LayoutMenuItem(string text, Image? img, ToolStripItem[] items) : base(text, img, items) { }
        public LayoutMenuItem(string text, Image? img, EventHandler onClick, string name) : base(text, img, onClick, name) { }
        public LayoutMenuItem(string text, Image? img, EventHandler onClick, Keys shortcuts) : base(text, img, onClick, shortcuts) { }

        public void AddMeTo(object addTo) {
            switch (addTo) {
                case ToolStripDropDown menu:
                    menu.Items.Add(this);
                    break;
                case ToolStripMenuItem menuItem:
                    menuItem.DropDownItems.Add(this);
                    break;
                case MenuOrMenuItem menuOrItem:
                    menuOrItem.Items.Add(this);
                    break;
                default:
                    throw new ArgumentException("AddMeTo: menu item can be added to either menu or another menu item");
            }
        }

    }

    public static class ToolStripSeperatorExtension {
        public static void AddMeTo(this ToolStripSeparator toolStripSeparator, object addTo) {
            switch (addTo) {
                case ToolStripDropDown menu:
                    menu.Items.Add(toolStripSeparator);
                    break;
                case ToolStripMenuItem menuItem:
                    menuItem.DropDownItems.Add(toolStripSeparator);
                    break;
                case MenuOrMenuItem menuOrItem:
                    menuOrItem.Items.Add(toolStripSeparator);
                    break;
                default:
                    throw new ArgumentException("AddMeTo:  can be added to either menu or another menu item");
            }
        }
    }

    public struct MenuOrMenuItem {
        readonly object _menuOrMenuItem;

        public MenuOrMenuItem(object menuOrMenuItem) { _menuOrMenuItem = menuOrMenuItem; }

        public ToolStripItemCollection Items {
            get => _menuOrMenuItem switch {
                ToolStripDropDown menu => menu.Items,
                ToolStripMenuItem item => item.DropDownItems,
                _ => throw new ArgumentException("Invalid _menuOrMenuItem")
            };
        }
    }

    /// <summary>
    /// Add entries to a menu with names of components of a given type or that implement a given interface
    /// </summary>
    /// <typeparam name="ComponentType">The component type/interface</typeparam>
    /// <typeparam name="MenuItemType">The type of each menu item</typeparam>
    public class BuildComponentsMenu<ComponentType, MenuItemType>
    where ComponentType : IModelComponentHasName
    where MenuItemType : ModelComponentMenuItemBase<ComponentType>, new() {
        private readonly IEnumerable<ComponentType> components;
        private readonly string? xPathFilter;
        private readonly EventHandler clickHandler;
        private readonly BuildComponentsMenuComponentFilter<ComponentType>? filterHandler;

        /// <summary>
        /// Constrcuct a component menu builder
        /// </summary>
        /// <param name="components">The components to add to the menu (all must be of the given type or implement tha interface)</param>
        /// <param name="xPathFilter">Optional XPath filter, only components that match the filter are added</param>
        /// <param name="filterHandler">Optional filter callback function</param>
        /// <param name="clickHandler">Optiona menu item click handler</param>
        public BuildComponentsMenu(IEnumerable<ComponentType> components, string? xPathFilter, BuildComponentsMenuComponentFilter<ComponentType>? filterHandler, EventHandler clickHandler) {
            this.components = components;
            this.xPathFilter = xPathFilter;
            this.filterHandler = filterHandler;
            this.clickHandler = clickHandler;
        }

        /// <summary>
        /// Add the entries to the menu
        /// </summary>
        /// <param name="menu">The menu to which entries are to be added</param>
        public void AddComponentMenuItems(MenuOrMenuItem menu) {
            Dictionary<LayoutModelArea, List<ComponentType>> areas = new();

            foreach (ComponentType component in components) {
                if (IncludeComponent(component)) {
                    if (!areas.TryGetValue(component.Spot.Area, out List<ComponentType>? componentsForArea)) {
                        componentsForArea = new List<ComponentType>();
                        areas.Add(component.Spot.Area, componentsForArea);
                    }

                    componentsForArea.Add(component);
                }
            }

            if (areas.Count == 1) {
                List<ComponentType>? componentsInArea = null;

                foreach (List<ComponentType> a in areas.Values)
                    componentsInArea = a;

                Debug.Assert(componentsInArea != null);
                componentsInArea.Sort((ComponentType c1, ComponentType c2) => string.Compare(c1.NameProvider.Name, c2.NameProvider.Name));

                foreach (ComponentType component in componentsInArea)
                    menu.Items.Add(CreateMenuItem(component));
            }
            else if (areas.Count > 1) {
                List<LayoutModelArea> areasList = new();

                foreach (LayoutModelArea area in areas.Keys)
                    areasList.Add(area);

                areasList.Sort((LayoutModelArea a1, LayoutModelArea a2) => string.Compare(a1.Name, a2.Name));

                foreach (LayoutModelArea area in areasList) {
                    var areaMenuItem = new LayoutMenuItem(area.Name);
                    List<ComponentType> componentsInArea = areas[area];

                    componentsInArea.Sort((ComponentType c1, ComponentType c2) => string.Compare(c1.NameProvider.Name, c2.NameProvider.Name));

                    foreach (ComponentType component in componentsInArea)
                        areaMenuItem.DropDownItems.Add(CreateMenuItem(component));

                    menu.Items.Add(areaMenuItem);
                }
            }
        }

        private MenuItemType CreateMenuItem(ComponentType component) {
            MenuItemType item = new() {
                OptionalComponent = component,
                Text = component.NameProvider.Name,
                DisplayStyle = ToolStripItemDisplayStyle.Text,
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

            return xPathFilter == null || Ensure.NotNull<XPathNavigator>(component.Element.CreateNavigator()).Matches(xPathFilter);
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
        /// <param name="clickHandler">Optional menu item click handler</param>
        public BuildComponentsMenu(LayoutPhase phase, string xPathFilter, EventHandler clickHandler) :
            base(LayoutModel.Components<ComponentType>(phase), xPathFilter, null, clickHandler) {
        }

        /// <summary>
        /// Construct a component menu builder
        /// </summary>
        /// 
        /// <param name="clickHandler">Optional menu item click handler</param>
        public BuildComponentsMenu(LayoutPhase phase, EventHandler clickHandler) :
            base(LayoutModel.Components<ComponentType>(phase), null, null, clickHandler) {
        }
    }

    public class ModelComponentMenuItemBase<ComponentType> : ToolStripMenuItem where ComponentType : IModelComponentHasName {
        public ModelComponentMenuItemBase() {
        }

        public ModelComponentMenuItemBase(ComponentType component) {
            this.OptionalComponent = component;
            this.Text = component.NameProvider.Name;
        }

        public ComponentType Component { get => OptionalComponent ?? throw new NullReferenceException(); }

        public ComponentType? OptionalComponent { get; init; }
    }

    public delegate void SemiModalDialogClosedHandler(Form dialog, object? info);

    public class SemiModalDialog {
        private readonly Form parent;
        private readonly Form dialog;
        private readonly SemiModalDialogClosedHandler? onClose = null;
        private readonly object? info = null;
        private System.Windows.Forms.Timer? autoHideTimer = null;

        public SemiModalDialog(Form parent, Form dialog) {
            this.parent = parent;
            this.dialog = dialog;
        }

        public SemiModalDialog(Form parent, Form dialog, SemiModalDialogClosedHandler onClose, object? info) {
            this.parent = parent;
            this.dialog = dialog;
            this.onClose = onClose;
            this.info = info;
        }

        public void ShowDialog() {
            if (!parent.Modal) {
                dialog.Owner = parent;
                dialog.Closing += OnDialogClosing;
                dialog.Closed += OnDialogClosed;

                parent.Enabled = false;

                if (AutoHideParent) {
                    autoHideTimer = new System.Windows.Forms.Timer {
                        Interval = 750
                    };
                    autoHideTimer.Tick += OnAutoHideTimerTick;
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

        private void OnDialogClosing(object? sender, CancelEventArgs e) {
            dialog.Closing -= OnDialogClosing;

            if (autoHideTimer != null) {
                autoHideTimer.Dispose();
                autoHideTimer = null;
            }

            parent.Visible = true;
            parent.Enabled = true;
            dialog.Owner = null;
            parent.Activate();
        }

        private void OnDialogClosed(object? sender, EventArgs e) {
            dialog.Closed -= OnDialogClosed;
            onClose?.Invoke(dialog, info);
        }

        private void OnAutoHideTimerTick(object? sender, EventArgs e) {
            autoHideTimer?.Dispose();
            autoHideTimer = null;
            parent.Visible = false;
        }
    }
}