#region Using directives

using System;
using System.Windows.Forms;

using MethodDispatcher;

#endregion

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    internal partial class FindComponents : Form {
        private readonly LayoutModelArea activeArea;

        public FindComponents(LayoutModelArea area) {
            InitializeComponent();

            this.activeArea = area;
        }

        public LayoutModelArea ActiveArea => activeArea;

        private bool IsMatch(string text, string findWhat) {
            return checkBoxExactMatch.Checked ? text.ToLower() == findWhat : text.ToLower().Contains(findWhat);
        }

        private void SearchArea(LayoutModelArea area, LayoutSelection results) {
            string findWhat = textBoxFind.Text.ToLower();

            foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                foreach (ModelComponent component in spot) {
                    bool found = false;

                    if (checkBoxScopeNames.Checked) {
                        if (component is IModelComponentHasName namedComponent && IsMatch(namedComponent.NameProvider.Name, findWhat))
                            found = true;
                    }

                    if (!found && checkBoxScopeAttributes.Checked) {
                        if (component is IObjectHasAttributes attributedComponent && attributedComponent.HasAttributes) {
                            foreach (AttributeInfo attribute in attributedComponent.Attributes) {
                                if (attribute.Name.Contains(findWhat) || IsMatch(attribute.ValueAsString, findWhat)) {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!found && checkBoxScopeAddresses.Checked) {
                        if (component is IModelComponentConnectToControl connectedComponent && connectedComponent.IsConnected) {
                            var connections = LayoutModel.ControlManager.ConnectionPoints[connectedComponent];

                            if (connections != null) {
                                foreach (ControlConnectionPoint connection in connections) {
                                    if (IsMatch(connection.Module.ModuleType.GetConnectionPointAddressText(connection.Module.ModuleType, connection.Module.Address, connection.Index, true), findWhat)) {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (found)
                        results.Add(component);
                }
            }
        }

        private void ButtonSearch_Click(object? sender, EventArgs e) {
            LayoutSelection results = new();

            if (checkBoxLimitToActiveArea.Checked)
                SearchArea(ActiveArea, results);
            else
                foreach (LayoutModelArea area in LayoutModel.Areas)
                    SearchArea(area, results);

            if (results.Count == 0)
                MessageBox.Show(this, "No components found", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else {
                if (checkBoxSelectResults.Checked) {
                    LayoutController.UserSelection.Clear();
                    LayoutController.UserSelection.Add(results);
                }
                else {
                    Dispatch.Call.AddMessage($"Components that matched '{textBoxFind.Text}'", results);
                    Dispatch.Call.ShowMessages();
                }

                Close();
            }
        }
    }
}