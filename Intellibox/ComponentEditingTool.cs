using LayoutManager;
using LayoutManager.CommonUI;
using MethodDispatcher;
using System;
using System.ComponentModel;

namespace Intellibox {
    /// <summary>
    /// Summary description for ComponentTool.
    /// </summary>
    [LayoutModule("Intellibox Component Editing Tool", UserControl = false)]
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

        [LayoutEvent("model-component-placement-request", SenderType = typeof(IntelliboxComponent))]
        private void PlaceTrackContactRequest(LayoutEvent e) {
            var component = Ensure.NotNull<IntelliboxComponent>(e.Sender);
            using Dialogs.CentralStationProperties csProperties = new(component);
            if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                component.XmlInfo.XmlDocument = csProperties.XmlInfo.XmlDocument;
                e.Info = true;      // Place component
            }
            else
                e.Info = false;		// Do not place component
        }

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] IntelliboxComponent _) => true;


        [DispatchTarget]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] IntelliboxComponent component, MenuOrMenuItem menu) {
            menu.Items.Add(new IntelliboxMenuItemProperties(component));
        }

        #region Intellibox Menu Item Classes

        private class IntelliboxMenuItemProperties : LayoutMenuItem {
            private readonly IntelliboxComponent component;

            internal IntelliboxMenuItemProperties(IntelliboxComponent component) {
                this.component = component;
                this.Text = "&Properties";
            }

            protected override void OnClick(EventArgs e) {
                Dialogs.CentralStationProperties csProperties = new(component);

                if (csProperties.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                        new(component, csProperties.XmlInfo);

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
