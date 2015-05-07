#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
	partial class FindComponents : Form {
		LayoutModelArea activeArea;

		public FindComponents(LayoutModelArea area) {
			InitializeComponent();

			this.activeArea = area;
		}

        public LayoutModelArea ActiveArea => activeArea;

        private bool isMatch(string text, string findWhat) {
			if(checkBoxExactMatch.Checked)
				return text.ToLower() == findWhat;
			else
				return text.ToLower().Contains(findWhat);
		}

		private void searchArea(LayoutModelArea area, LayoutSelection results) {
			string findWhat = textBoxFind.Text.ToLower();

			foreach(LayoutModelSpotComponentCollection spot in area.Grid.Values) {
				foreach(ModelComponent component in spot) {
					bool found = false;

					if(checkBoxScopeNames.Checked) {
						IModelComponentHasName namedComponent = component as IModelComponentHasName;

						if(namedComponent != null && isMatch(namedComponent.NameProvider.Name, findWhat))
							found = true;
					}

					if(!found && checkBoxScopeAttributes.Checked) {
						IObjectHasAttributes attributedComponent = component as IObjectHasAttributes;

						if(attributedComponent != null && attributedComponent.HasAttributes) {
							foreach(AttributeInfo attribute in attributedComponent.Attributes) {
								if(attribute.Name.Contains(findWhat) || isMatch(attribute.ValueAsString, findWhat)) {
									found = true;
									break;
								}
							}
						}
					}

					if(!found && checkBoxScopeAddresses.Checked) {
						IModelComponentConnectToControl connectedComponent = component as IModelComponentConnectToControl;

						if(connectedComponent != null && connectedComponent.IsConnected) {
							IList<ControlConnectionPoint> connections = LayoutModel.ControlManager.ConnectionPoints[connectedComponent];

							foreach(ControlConnectionPoint connection in connections) {
								if(isMatch(connection.Module.ModuleType.GetConnectionPointAddressText(connection.Module.Address, connection.Index, true), findWhat)) {
									found = true;
									break;
								}
							}
						}
					}

					if(found)
						results.Add(component);
				}
			}
		}

		private void buttonSearch_Click(object sender, EventArgs e) {
			LayoutSelection results = new LayoutSelection();

			if(checkBoxLimitToActiveArea.Checked)
				searchArea(ActiveArea, results);
			else
				foreach(LayoutModelArea area in LayoutModel.Areas)
					searchArea(area, results);

			if(results.Count == 0)
				MessageBox.Show(this, "No components found", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
			else {
				if(checkBoxSelectResults.Checked) {
					LayoutController.UserSelection.Clear();
					LayoutController.UserSelection.Add(results);
				}
				else {
					EventManager.Event(new LayoutEvent(results, "add-message", null, "Components that matched '" + textBoxFind.Text + "'"));
					EventManager.Event(new LayoutEvent(this, "show-messages"));
				}

				Close();
			}
		}
	}
}