using LayoutManager;
using LayoutManager.CommonUI;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.ComponentModel;

namespace MarklinDigital {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Marklin Digital Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>
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

        [DispatchTarget]
        bool RequestModelComponentPlacement([DispatchFilter] MarklinDigitalCentralStation component, PlacementInfo placement) {
            var csProperties = new Dialogs.MarklinDigitalProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                return true;      // Place component
            }
            else
                return false;     // Do not place component
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] MarklinDigitalCentralStation _) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] MarklinDigitalCentralStation component, MenuOrMenuItem menu) {
            menu.Items.Add(new MarklinDigitalMenuItemProperties(component));
        }

        #region Markin Digital Menu Item Classes

        private class MarklinDigitalMenuItemProperties : LayoutMenuItem {
            private readonly MarklinDigitalCentralStation component;

            internal MarklinDigitalMenuItemProperties(MarklinDigitalCentralStation component) {
                this.component = component;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                var csProperties = new Dialogs.MarklinDigitalProperties(component);

                if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    var modifyComponentDocumentCommand =
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
        }
        #endregion
    }
}
