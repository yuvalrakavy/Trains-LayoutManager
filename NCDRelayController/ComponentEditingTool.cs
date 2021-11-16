using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.CommonUI;

namespace NCDRelayController {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("NCD Relay Controller Component Editing Tool", UserControl = false)]
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

        [LayoutEvent("model-component-placement-request", SenderType = typeof(NCDRelayController))]
        private void PlaceTrackContactRequest(LayoutEvent e) {
            var component = Ensure.NotNull<NCDRelayController>(e.Sender);
            var csProperties = new Dialogs.NCDRelayControllerProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                e.Info = true;      // Place component
            }
            else
                e.Info = false;     // Do not place component
        }

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(NCDRelayController))]
        private void QueryEditingMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(NCDRelayController))]
        private void AddEditingContextMenuEntries(LayoutEvent e) {
            var component = Ensure.NotNull<NCDRelayController>(e.Sender);
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);

            menu.Items.Add(new LayoutMenuItem("&Properties", null, (s, ea) => {
                var d = new Dialogs.NCDRelayControllerProperties(component);

                if (d.ShowDialog() == DialogResult.OK)
                    LayoutController.Do(new LayoutModifyComponentDocumentCommand(component, d.XmlInfo));
            }));
        }

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
