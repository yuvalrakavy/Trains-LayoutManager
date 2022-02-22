using LayoutManager.Components;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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

        private void ListBoxProgrammingLocations_SelectedIndexChanged(object? sender, EventArgs e) {
            LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)listBoxProgrammingLocations.SelectedItem;

            if (blockDefinition != null)
                Dispatch.Call.EnsureComponentVisible(LayoutController.ActiveFrameWindow.Id, blockDefinition, true);
        }
    }
}
