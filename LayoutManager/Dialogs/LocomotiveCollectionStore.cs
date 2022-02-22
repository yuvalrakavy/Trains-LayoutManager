using System;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCollectionStore.
    /// </summary>
    public partial class LocomotiveCollectionStore : Form {
        private readonly XmlElement storeElement;
        private readonly string defaultExtension;
        private readonly string collectionDescription;
        private readonly string defaultDirectory;

        public LocomotiveCollectionStore(String storesName, XmlElement storeElement, string collectionDescription, string defaultDirectory, string defaultExtension) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.collectionDescription = collectionDescription;
            this.defaultDirectory = defaultDirectory;
            this.defaultExtension = defaultExtension;

            this.Text = storesName + " Storage Definition";
            this.storeElement = storeElement;
            textBoxName.Text = storeElement.GetAttribute("Name");
            textBoxFile.Text = storeElement.GetAttribute("File");
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
            storeElement.SetAttribute("Name", textBoxName.Text);
            storeElement.SetAttribute("File", textBoxFile.Text);

            DialogResult = DialogResult.OK;
        }

        private void ButtonBrowse_Click(object? sender, EventArgs e) {
            FileDialog fileDialog = new OpenFileDialog {
                FileName = textBoxFile.Text,
                InitialDirectory = defaultDirectory,
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                DefaultExt = defaultExtension,
                Filter = collectionDescription + "|*" + defaultExtension + "|All files (*.*)|*.*"
            };

            if (fileDialog.ShowDialog(this) == DialogResult.OK)
                textBoxFile.Text = LayoutAssembly.FilePathToValue(fileDialog.FileName, defaultDirectory);
        }
    }
}
