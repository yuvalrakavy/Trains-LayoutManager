using System;
using System.Windows.Forms;
using System.Xml;
using MethodDispatcher;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveType.
    /// </summary>
    public partial class LocomotiveTypeProperties : LocomotiveBasePropertiesForm {
        private readonly LocomotiveTypeInfo? inLocoType;
        private readonly LocomotiveTypeInfo? locoType;
        private readonly LocomotiveCatalogInfo? catalog;

        public LocomotiveTypeProperties(LocomotiveTypeInfo inLocoType) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.inLocoType = inLocoType;
            this.catalog = Catalog;
            lengthInput.Initialize();

            // Make a copy for editing
            locoType = new LocomotiveTypeInfo((XmlElement)inLocoType.Element.CloneNode(true));

            InitializeControls(locoType.Element, Ensure.NotNull<XmlElement>(catalog.Element["Stores"]));

            textBoxName.Text = locoType.TypeName;

            checkBoxHasBuiltinDecoder.Checked = locoType.DecoderTypeName != null;
            comboBoxDecoderType.Enabled = checkBoxHasBuiltinDecoder.Checked;

            UpdateButtons();
        }

        public LocomotiveTypeProperties() {
        }

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

        private void ButtonOk_Click(object? sender, EventArgs e) {
            if (locoType == null || inLocoType == null)
                return;

            if (textBoxName.Text.Trim() == "") {
                tabControl1.SelectedTab = tabPageGeneral;
                MessageBox.Show(this, "You must enter a name", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (checkBoxHasBuiltinDecoder.Checked && comboBoxDecoderType.SelectedItem == null) {
                tabControl1.SelectedTab = tabPageDecoder;
                MessageBox.Show(this, "You have indicated that loco has built in decoder, but you have not specified its type", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxDecoderType.Focus();
                return;
            }

            if (!GetLocomotiveTypeFields())
                return;

            locoType.TypeName = textBoxName.Text;

            if (checkBoxHasBuiltinDecoder.Checked)
                locoType.DecoderTypeName = ((DecoderTypeItem)comboBoxDecoderType.SelectedItem).DecoderType.TypeName;
            else
                locoType.DecoderTypeName = "Generic-DCC";

            if (inLocoType.Element.ParentNode != null) {
                // Replace the input with copy made for editing
                inLocoType.Element.ParentNode.ReplaceChild(locoType.Element, inLocoType.Element);
            }

            inLocoType.Element = locoType.Element;

            Dispatch.Notification.OnLocomotiveTypeUpdated(locoType);
            DialogResult = DialogResult.OK;
        }

        private void LengthInput1_Load(object? sender, EventArgs e) {
        }

        private void CheckBoxHasBuiltinDecoder_CheckedChanged(object? sender, EventArgs e) {
            comboBoxDecoderType.Enabled = checkBoxHasBuiltinDecoder.Checked;
        }

        private void ButtonFunctionAdd_Click_1(object? sender, EventArgs e) {
            ButtonFunctionAdd_Click(sender, e);
        }

        private void ButtonFunctionEdit_Click_1(object? sender, EventArgs e) {
            ButtonFunctionEdit_Click(sender, e);
        }

        private void ButtonFunctionRemove_Click_1(object? sender, EventArgs e) {
            ButtonFunctionRemove_Click(sender, e);
        }

        private void ButtonCopyFrom_Click_1(object? sender, EventArgs e) {
            ButtonCopyFrom_Click(sender, e);
        }

        private void ListViewFunctions_SelectedIndexChanged_1(object? sender, EventArgs e) {
            ListViewFunctions_SelectedIndexChanged(sender, e);
        }

        private void TrackGuageSelector_SelectedIndexChanged_1(object? sender, EventArgs e) {
            TrackGuageSelector_SelectedIndexChanged(sender, e);
        }
    }
}
