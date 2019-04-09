using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCollectionStore.
    /// </summary>
    public class LocomotiveCollectionStore : Form {
        private Label label1;
        private Label label2;
        private TextBox textBoxName;
        private TextBox textBoxFile;
        private Button buttonBrowse;
        private Button buttonOk;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.label2 = new Label();
            this.textBoxName = new TextBox();
            this.textBoxFile = new TextBox();
            this.buttonBrowse = new Button();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Description:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Stored in file:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(80, 8);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(176, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Text = "";
            // 
            // textBoxFile
            // 
            this.textBoxFile.Location = new System.Drawing.Point(80, 32);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(176, 20);
            this.textBoxFile.TabIndex = 3;
            this.textBoxFile.Text = "";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(264, 32);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(64, 20);
            this.buttonBrowse.TabIndex = 4;
            this.buttonBrowse.Text = "&Browse...";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(176, 80);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.TabIndex = 5;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(256, 80);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            // 
            // LocomotiveCollectionStore
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(336, 110);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOk,
                                                                          this.buttonBrowse,
                                                                          this.textBoxName,
                                                                          this.label1,
                                                                          this.label2,
                                                                          this.textBoxFile,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LocomotiveCollectionStore";
            this.ShowInTaskbar = false;
            this.Text = "Locomotive Catalog Storage Definition";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOk_Click(object sender, System.EventArgs e) {
            storeElement.SetAttribute("Name", textBoxName.Text);
            storeElement.SetAttribute("File", textBoxFile.Text);

            DialogResult = DialogResult.OK;
        }

        private void buttonBrowse_Click(object sender, System.EventArgs e) {
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
