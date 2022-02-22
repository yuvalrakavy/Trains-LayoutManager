using LayoutManager;
using LayoutManager.CommonUI;
using MethodDispatcher;
using System;
using System.ComponentModel;

namespace DiMAX {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Massoth DiMAX Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        #region Implementation of ILayoutModuleSetup

        public LayoutModule? Module { set; get; }

        #endregion

        #region Constructors

        public ComponentTool(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentTool() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #endregion

        [LayoutEvent("model-component-placement-request", SenderType = typeof(DiMAXcommandStation))]
        private void PlaceTrackContactRequest(LayoutEvent e) {
            DiMAXcommandStation component = Ensure.NotNull<DiMAXcommandStation>(e.Sender);
            var csProperties = new Dialogs.DiMAXcommandlStationProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                e.Info = true;      // Place component
            }
            else
                e.Info = false;     // Do not place component
        }

        [DispatchTarget]
        private bool IncludeInComponentContextMenu([DispatchFilter] DiMAXcommandStation _) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries_DesignMode(Guid frameWindowId, [DispatchFilter] DiMAXcommandStation component, MenuOrMenuItem menu) {
            menu.Items.Add(new DiMAXcommandStationMenuItemProperties(component));
        }

        #region DiMAX Command Station Menu Item Classes

        private class DiMAXcommandStationMenuItemProperties : LayoutMenuItem {
            private readonly DiMAXcommandStation component;

            internal DiMAXcommandStationMenuItemProperties(DiMAXcommandStation component) {
                this.component = component;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                var csProperties = new Dialogs.DiMAXcommandlStationProperties(component);

                if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    var modifyComponentDocumentCommand =
                        new LayoutModifyComponentDocumentCommand(component, csProperties.XmlInfo);

                    LayoutController.Do(modifyComponentDocumentCommand);
                }
            }
        }

        #endregion

        [DispatchTarget]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries_OperationMode(Guid frameWindowId, [DispatchFilter] DiMAXcommandStation component, MenuOrMenuItem menu) {
            menu.Items.Add(new LayoutMenuItem("Test locomotive select", null, (_, _) => new Dialogs.TestLocoSelect(component).ShowDialog()));
            menu.Items.Add(new LayoutMenuItem("Test locomotive drive", null, (_, _) => new Dialogs.TestLocoDrive(component).ShowDialog()));
        }
    }
}
