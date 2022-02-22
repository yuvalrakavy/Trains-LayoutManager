using LayoutManager.Model;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFunctionsCopyFrom.
    /// </summary>
    public partial class LocomotiveFunctionsCopyFrom : Form {
        private LocomotiveTypeInfo? selection = null;

        public LocomotiveFunctionsCopyFrom(LocomotiveCatalogInfo catalog) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            xmlQueryComboboxCopyFrom.ContainerElement = catalog.CollectionElement;
            xmlQueryComboboxCopyFrom.Query = "*[contains(TypeName, '<TEXT>')]";
            xmlQueryComboboxCopyFrom.Extract = "string(TypeName)";
        }

        public LocomotiveTypeInfo? SelectedLocomotiveType => selection;

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
            var selectedElement = xmlQueryComboboxCopyFrom.SelectedElement;

            if (selectedElement == null) {
                MessageBox.Show(this, "You did not select a locomotive type to copy functions from", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                xmlQueryComboboxCopyFrom.Focus();
                return;
            }

            if (xmlQueryComboboxCopyFrom.IsTextAmbiguous) {
                MessageBox.Show(this, "The value you entered specifies more than one locomotive type", "Ambiguous Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                xmlQueryComboboxCopyFrom.Focus();
                return;
            }

            selection = new LocomotiveTypeInfo(selectedElement);
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
