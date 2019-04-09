using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace MarklinDigital {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Marklin Digital Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

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

        [LayoutEvent("model-component-placement-request", SenderType = typeof(MarklinDigitalCentralStation))]
        private void PlaceTrackContactRequest(LayoutEvent e) {
            MarklinDigitalCentralStation component = (MarklinDigitalCentralStation)e.Sender;
            Dialogs.MarklinDigitalProperties csProperties = new Dialogs.MarklinDigitalProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                e.Info = true;      // Place component
            }
            else
                e.Info = false;     // Do not place component
        }

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(MarklinDigitalCentralStation))]
        private void QueryMarklinDigitalMenu(LayoutEvent e) {
            e.Info = true;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(MarklinDigitalCentralStation))]
        private void AddMarklinDigitalContextMenuEntries(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            MarklinDigitalCentralStation component = (MarklinDigitalCentralStation)e.Sender;

            menu.MenuItems.Add(new MarklinDigitalMenuItemProperties(component));
        }

        #region Markin Digital Menu Item Classes

        private class MarklinDigitalMenuItemProperties : MenuItem {
            private readonly MarklinDigitalCentralStation component;

            internal MarklinDigitalMenuItemProperties(MarklinDigitalCentralStation component) {
                this.component = component;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                Dialogs.MarklinDigitalProperties csProperties = new Dialogs.MarklinDigitalProperties(component);

                if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                        new LayoutModifyComponentDocumentCommand(component, csProperties.XmlInfo);

                    LayoutController.Do(modifyComponentDocumentCommand);
                }
            }
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
    }
}
