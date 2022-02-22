using LayoutManager;
using LayoutManager.CommonUI;
using MethodDispatcher;
using System;
using System.ComponentModel;

namespace LayoutLGB {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("LGB MTS Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {
        #region Implementation of ILayoutModuleSetup

        public LayoutModule? Module { set; get; }

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

        [LayoutEvent("model-component-placement-request", SenderType = typeof(MTScentralStation))]
        private void PlaceTrackContactRequest(LayoutEvent e) {
            var component = Ensure.NotNull<MTScentralStation>(e.Sender);
            var csProperties = new Dialogs.CentralStationProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                e.Info = true;      // Place component
            }
            else
                e.Info = false;     // Do not place component
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] MTScentralStation _) => true;

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] MTScentralStation component, MenuOrMenuItem menu) {
            menu.Items.Add(new MTScentralStationMenuItemProperties(component));
        }

        #region MTS Central Station Menu Item Classes

        private class MTScentralStationMenuItemProperties : LayoutMenuItem {
            private readonly MTScentralStation component;

            internal MTScentralStationMenuItemProperties(MTScentralStation component) {
                this.component = component;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                var csProperties = new Dialogs.CentralStationProperties(component);

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
