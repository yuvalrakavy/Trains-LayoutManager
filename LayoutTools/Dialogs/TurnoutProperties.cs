using LayoutManager.Components;
using LayoutManager.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MethodDispatcher;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackContactProperties.
    /// </summary>
    public partial class TurnoutProperties : Form, ILayoutComponentPropertiesDialog {
        private void FixLabel(Control c, string componentName) {
            c.Text = Regex.Replace(c.Text, "COMPONENT", componentName);
        }

        public TurnoutProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            string componentName = component is LayoutDoubleSlipTrackComponent ? "Double Slip" : "Turnout";
            var multiPathComponent = (IModelComponentIsMultiPath)component;

            FixLabel(this, componentName);
            FixLabel(checkBoxHasBuiltinDecoder, componentName);
            FixLabel(checkBoxReverseLogic, componentName);
            FixLabel(checkBoxHasFeedback, componentName);

            this.XmlInfo = new LayoutXmlInfo(component);

            checkBoxReverseLogic.Checked = (bool?)XmlInfo.Element.AttributeValue(A_ReverseLogic) ?? false;
            checkBoxHasFeedback.Checked = (bool?)XmlInfo.Element.AttributeValue(A_HasFeedback) ?? false;

            IEnumerable<IModelComponentIsCommandStation> commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All);

            foreach (IModelComponentIsCommandStation commandStation in commandStations) {
                foreach (ControlBus bus in LayoutModel.ControlManager.Buses.Buses(commandStation)) {
                    IList<string> moduleTypeNames = bus.BusType.GetConnectableControlModuleTypeNames(new ControlConnectionPointDestination(multiPathComponent, multiPathComponent.ControlConnectionDescriptions[0]));

                    if (moduleTypeNames.Count > 0) {
                        foreach (string moduleTypeName in moduleTypeNames) {
                            ControlModuleType moduleType = Dispatch.Call.GetControlModuleType(moduleTypeName);

                            if (moduleType.BuiltIn)
                                comboBoxBuiltinDecoders.Items.Add(new Item(moduleType));
                        }
                    }
                }
            }

            if (comboBoxBuiltinDecoders.Items.Count == 0) {
                checkBoxHasBuiltinDecoder.Enabled = false;
                comboBoxBuiltinDecoders.Visible = false;
            }
            else {
                comboBoxBuiltinDecoders.SelectedIndex = 0;

                if (XmlInfo.Element.HasAttribute(A_BuiltinDecoderTypeName)) {
                    string typeName = XmlInfo.Element.GetAttribute(A_BuiltinDecoderTypeName);

                    foreach (Item item in comboBoxBuiltinDecoders.Items)
                        if (item.ModuleType.TypeName == typeName) {
                            comboBoxBuiltinDecoders.SelectedItem = item;
                            checkBoxHasBuiltinDecoder.Checked = true;
                            break;
                        }
                }
            }

            UpdateButtons();
        }

        private void UpdateButtons() {
            comboBoxBuiltinDecoders.Enabled = checkBoxHasBuiltinDecoder.Checked;
        }

        public LayoutXmlInfo XmlInfo { get; }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            XmlInfo.DocumentElement.SetAttributeValue(A_ReverseLogic, checkBoxReverseLogic.Checked);
            XmlInfo.DocumentElement.SetAttributeValue(A_HasFeedback, checkBoxHasFeedback.Checked);

            if (checkBoxHasBuiltinDecoder.Checked)
                XmlInfo.Element.SetAttribute(A_BuiltinDecoderTypeName, ((Item)comboBoxBuiltinDecoders.SelectedItem).ModuleType.TypeName);
            else
                XmlInfo.Element.RemoveAttribute(A_BuiltinDecoderTypeName);

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void CheckBoxHasBuiltinDecoder_CheckedChanged(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private class Item {
            public Item(ControlModuleType moduleType) {
                this.ModuleType = moduleType;
            }

            public ControlModuleType ModuleType { get; }

            public override string ToString() => ModuleType.Name;
        }
    }
}
