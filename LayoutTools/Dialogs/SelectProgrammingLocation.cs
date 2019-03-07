using System;
using System.Collections.Generic;
using System.Windows.Forms;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    public partial class SelectProgrammingLocation : Form {
        public SelectProgrammingLocation(IEnumerable<LayoutBlockDefinitionComponent> programmingLocations) {
            InitializeComponent();

            foreach (var programmingLocation in programmingLocations)
                listBoxProgrammingLocations.Items.Add(programmingLocation);

            if (listBoxProgrammingLocations.Items.Count > 0)
                listBoxProgrammingLocations.SelectedIndex = 0;
        }

        public LayoutBlockDefinitionComponent SelectedProgrammingLocation => (LayoutBlockDefinitionComponent)listBoxProgrammingLocations.SelectedItem;

        private void listBoxProgrammingLocations_SelectedIndexChanged(object sender, EventArgs e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)listBoxProgrammingLocations.SelectedItem;

            if (blockDefinition != null)
                EventManager.Event(new LayoutEvent(blockDefinition, "ensure-component-visible", null, true).SetFrameWindow(LayoutController.ActiveFrameWindow));
        }
    }
}
