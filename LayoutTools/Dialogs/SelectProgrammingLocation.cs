using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
	public partial class SelectProgrammingLocation : Form {
		public SelectProgrammingLocation(IEnumerable<LayoutBlockDefinitionComponent> programmingLocations) {
			InitializeComponent();

			foreach(var programmingLocation in programmingLocations)
				listBoxProgrammingLocations.Items.Add(programmingLocation);

			if(listBoxProgrammingLocations.Items.Count > 0)
				listBoxProgrammingLocations.SelectedIndex = 0;
		}

		public LayoutBlockDefinitionComponent SelectedProgrammingLocation {
			get {
				return (LayoutBlockDefinitionComponent)listBoxProgrammingLocations.SelectedItem;
			}
		}

		private void listBoxProgrammingLocations_SelectedIndexChanged(object sender, EventArgs e) {
			LayoutBlockDefinitionComponent blockDefinition = (LayoutBlockDefinitionComponent)listBoxProgrammingLocations.SelectedItem;

			if(blockDefinition != null)
				EventManager.Event(new LayoutEvent(blockDefinition, "ensure-component-visible", null, true).SetFrameWindow(LayoutController.ActiveFrameWindow));
		}
	}
}
