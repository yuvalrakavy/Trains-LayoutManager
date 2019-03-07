using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace DiMAX {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Massoth DiMAX Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        LayoutModule _module;

        #region Implementation of ILayoutModuleSetup

        public LayoutModule Module {
            set {
                _module = value;
            }

            get {
                return _module;
            }
        }

        #endregion

        #region Constructors

        public ComponentTool(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentTool() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #endregion

        [LayoutEvent("model-component-placement-request", SenderType = typeof(DiMAXcommandStation))]
        void PlaceTrackContactRequest(LayoutEvent e) {
            DiMAXcommandStation component = (DiMAXcommandStation)e.Sender;
            Dialogs.DiMAXcommandlStationProperties csProperties = new Dialogs.DiMAXcommandlStationProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                e.Info = true;      // Place component
            }
            else
                e.Info = false;     // Do not place component
        }

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(DiMAXcommandStation))]
        void QueryEditingMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(DiMAXcommandStation))]
        void AddEditingContextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            DiMAXcommandStation component = (DiMAXcommandStation)e.Sender;

            menu.MenuItems.Add(new DiMAXcommandStationMenuItemProperties(component));
        }

        #region DiMAX Command Station Menu Item Classes

        class DiMAXcommandStationMenuItemProperties : MenuItem {
            readonly DiMAXcommandStation component;

            internal DiMAXcommandStationMenuItemProperties(DiMAXcommandStation component) {
                this.component = component;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                Dialogs.DiMAXcommandlStationProperties csProperties = new Dialogs.DiMAXcommandlStationProperties(component);

                if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                        new LayoutModifyComponentDocumentCommand(component, csProperties.XmlInfo);

                    LayoutController.Do(modifyComponentDocumentCommand);
                }
            }
        }

        #endregion

        [LayoutEvent("query-component-operation-context-menu", SenderType = typeof(DiMAXcommandStation))]
        void QueryOperationalMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-operation-context-menu-entries", SenderType = typeof(DiMAXcommandStation))]
        void AddOperationContextMenuEntries(LayoutEvent e) {
            var commandStation = (DiMAXcommandStation)e.Sender;
            var m = (Menu)e.Info;

            m.MenuItems.Add(new MenuItem("Test locomotive select", (s, ea) => new Dialogs.TestLocoSelect(commandStation).ShowDialog()));
            m.MenuItems.Add(new MenuItem("Test locomotive drive", (s, ea) => new Dialogs.TestLocoDrive(commandStation).ShowDialog()));
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new Container();
        }
        #endregion
    }
}
