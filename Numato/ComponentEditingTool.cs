using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

#pragma warning disable IDE0051, IDE0052
namespace NumatoController {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Numato Relay Controller Component Editing Tool", UserControl = false)]
    public class ComponentTool : System.ComponentModel.Component, ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        #region Implementation of ILayoutModuleSetup

        public LayoutModule Module { set; get; }

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

        [LayoutEvent("model-component-placement-request", SenderType = typeof(NumatoController))]
        private void PlaceNumatoRelayController(LayoutEvent e) {
            var component = (NumatoController)e.Sender;
            using var csProperties = new Dialogs.NumatoControllerProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                e.Info = true;      // Place component
            }
            else
                e.Info = false;     // Do not place component
        }

        [LayoutEvent("model-component-post-placement-request", SenderType = typeof(NumatoController))]
        private void PlaceNumatoRelayControllerRelayModule(LayoutEvent e) {
            var component = (NumatoController)e.Sender;
            var command = (ILayoutCompoundCommand)e.Info;

            var bus = component.RelayBus;
            var moduleType = $"{component.RelaysCount}_NumatoRelays";
            var addControlModuleCommand = new AddControlModuleCommand(bus, moduleType, Guid.Empty, 0);

            command.Add(addControlModuleCommand);
            command.Add(new SetControlUserActionRequiredCommand(addControlModuleCommand.AddedModule.Module, true));
        }

        [LayoutEvent("query-component-editing-context-menu", SenderType = typeof(NumatoController))]
        private void QueryEditingMenu(LayoutEvent e) {
            e.Info = e.Sender;
        }

        [LayoutEvent("add-component-editing-context-menu-entries", SenderType = typeof(NumatoController))]
        private void AddEditingContextMenuEntries(LayoutEvent e) {
            var component = Ensure.NotNull<NumatoController>(e.Sender);
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);

            menu.Items.Add("&Properties", null, (s, ea) => {
                var d = new Dialogs.NumatoControllerProperties(component);

                if (d.ShowDialog() == DialogResult.OK)
                    LayoutController.Do(new LayoutModifyComponentDocumentCommand(component, d.XmlInfo));
            });
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
