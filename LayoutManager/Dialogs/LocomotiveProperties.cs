using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using MethodDispatcher;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveType.
    /// </summary>
    public partial class LocomotiveProperties : LocomotiveBasePropertiesForm {
        private readonly LocomotiveInfo inLoco;
        private readonly LocomotiveInfo loco;

        public LocomotiveProperties(LocomotiveInfo inLoco) : base() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.inLoco = inLoco;

            // Make a copy for editing
            loco = new LocomotiveInfo((XmlElement)inLoco.Element.CloneNode(true));

            textBoxName.Text = loco.Name;

            lengthInput.Initialize();
            InitializeControls(loco.Element, Ensure.NotNull<XmlElement>(LocomotiveCollection.Element["Stores"]));

            if (loco.TypeName != "")
                linkLabelTypeName.Text = loco.TypeName;
            else
                linkLabelTypeName.Enabled = false;

            checkBoxLinkedToType.Checked = loco.LinkedToType;
            checkBoxTriggerTrackContact.Checked = loco.CanTriggerTrackContact;

            textBoxCollectionID.Text = loco.CollectionId;

            UpdateButtons();
        }

        protected LocomotiveCollectionInfo LocomotiveCollection => LayoutModel.LocomotiveCollection;

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
            if (textBoxName.Text.Trim() == "") {
                tabControl1.SelectedTab = tabPageGeneral;
                MessageBox.Show(this, "You must enter a name", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (comboBoxDecoderType.SelectedItem == null) {
                tabControl1.SelectedTab = tabPageDecoder;
                MessageBox.Show(this, "You must specify the type of decoder that is install in this locomotive", "Missing decoder type", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxDecoderType.Focus();
                return;
            }

            if (!GetLocomotiveTypeFields())
                return;

            loco.DecoderTypeName = ((DecoderTypeItem)comboBoxDecoderType.SelectedItem).DecoderType.TypeName;

            loco.Name = textBoxName.Text;
            loco.CollectionId = textBoxCollectionID.Text;

            loco.LinkedToType = checkBoxLinkedToType.Checked;
            loco.CanTriggerTrackContact = checkBoxTriggerTrackContact.Checked;

            if (inLoco.Element.ParentNode != null) {
                // Replace the input with copy made for editing
                inLoco.Element.ParentNode.ReplaceChild(loco.Element, inLoco.Element);
            }

            inLoco.Element = loco.Element;

            Dispatch.Notification.OnLocomotiveConfigurationChanged(inLoco);
            DialogResult = DialogResult.OK;
        }

        private void ButtonSelectType_Click(object? sender, EventArgs e) {
            SelectLocomotiveType selectType = new();

            if (selectType.ShowDialog(this) == DialogResult.OK) {
                if (selectType.SelectedLocomotiveType != null) {
                    loco.UpdateFromLocomotiveType(selectType.SelectedLocomotiveType);
                    SetLocomotiveTypeFields();
                    linkLabelTypeName.Text = loco.TypeName;
                    linkLabelTypeName.Enabled = true;
                }
            }

            selectType.Dispose();
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
