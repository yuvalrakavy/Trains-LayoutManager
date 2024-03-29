using LayoutManager;
using LayoutManager.CommonUI;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace NumatoController {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Numato Relay Controller Component Editing Tool", UserControl = false)]
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

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentTool() {
        }

        #endregion

        [DispatchTarget]
        bool RequestModelComponentPlacement([DispatchFilter] NumatoController component, PlacementInfo placement) {
            using var csProperties = new Dialogs.NumatoControllerProperties(component);

            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                return true;      // Place component
            }
            else
                return false;     // Do not place component
        }

        [DispatchTarget]
        void OnModelComponentPlacedNotification([DispatchFilter] NumatoController component, ILayoutCompoundCommand command, PlacementInfo placement) {
            var bus = component.RelayBus;
            var moduleType = $"{component.RelaysCount}_NumatoRelays";
            var addControlModuleCommand = new AddControlModuleCommand(bus, moduleType, Guid.Empty, 0);

            command.Add(addControlModuleCommand);
            command.Add(new SetControlUserActionRequiredCommand(addControlModuleCommand.AddedModule.Module, true));
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] NumatoController component) => true;

        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] NumatoController component, MenuOrMenuItem menu) {
            menu.Items.Add("&Properties", null, (s, ea) => {
                var d = new Dialogs.NumatoControllerProperties(component);

                if (d.ShowDialog() == DialogResult.OK)
                    LayoutController.Do(new LayoutModifyComponentDocumentCommand(component, d.XmlInfo));
            });
        }
    }
}
