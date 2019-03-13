using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackLinkForm.
    /// </summary>
    public class TrackLinkProperties : Form {
        private LayoutManager.CommonUI.Controls.TrackLinkTree trackLinkTree;
        private RadioButton radioButtonNotLinked;
        private RadioButton radioButtonLinked;
        private Button buttonOK;
        private Button buttonCancel;
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        readonly LayoutXmlInfo xmlInfo;

        public TrackLinkProperties(LayoutModelArea area, LayoutTrackLinkComponent trackLinkComponent) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Set initial fields
            xmlInfo = new LayoutXmlInfo(trackLinkComponent);
            trackLinkTree.ThisComponentLink = new LayoutTrackLink(area.AreaGuid, trackLinkComponent.TrackLinkGuid);

            nameDefinition.DefaultIsVisible = true;
            nameDefinition.XmlInfo = xmlInfo;
            nameDefinition.Component = trackLinkComponent;

            if (trackLinkComponent.Link != null) {
                radioButtonLinked.Checked = true;

                trackLinkTree.SelectedTrackLink = trackLinkComponent.Link;
            }
            else
                radioButtonNotLinked.Checked = true;

            updateDependencies();
        }

        /// <summary>
        /// Make sure that all dependencies between controls are maintained
        /// </summary>
        private void updateDependencies() {
            trackLinkTree.Enabled = radioButtonLinked.Checked;
        }

        public LayoutXmlInfo XmlInfo => xmlInfo;

        public LayoutTrackLink TrackLink {
            get {
                if (radioButtonLinked.Checked)
                    return trackLinkTree.SelectedTrackLink;
                return null;
            }
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
            this.radioButtonNotLinked = new RadioButton();
            this.radioButtonLinked = new RadioButton();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.trackLinkTree = new LayoutManager.CommonUI.Controls.TrackLinkTree();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.SuspendLayout();
            // 
            // radioButtonNotLinked
            // 
            this.radioButtonNotLinked.Location = new System.Drawing.Point(32, 64);
            this.radioButtonNotLinked.Name = "radioButtonNotLinked";
            this.radioButtonNotLinked.TabIndex = 4;
            this.radioButtonNotLinked.Text = "Not linked";
            this.radioButtonNotLinked.CheckedChanged += new System.EventHandler(this.radioButtonNotLinked_CheckedChanged);
            // 
            // radioButtonLinked
            // 
            this.radioButtonLinked.Location = new System.Drawing.Point(32, 88);
            this.radioButtonLinked.Name = "radioButtonLinked";
            this.radioButtonLinked.TabIndex = 5;
            this.radioButtonLinked.Text = "Linked to:";
            this.radioButtonLinked.CheckedChanged += new System.EventHandler(this.radioButtonLinked_CheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 248);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(128, 248);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // trackLinkTree
            // 
            this.trackLinkTree.Location = new System.Drawing.Point(48, 112);
            this.trackLinkTree.Name = "trackLinkTree";
            this.trackLinkTree.SelectedTrackLink = null;
            this.trackLinkTree.Size = new System.Drawing.Size(232, 128);
            this.trackLinkTree.TabIndex = 6;
            // 
            // nameDefinition
            // 
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(9, 4);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(280, 64);
            this.nameDefinition.TabIndex = 9;
            this.nameDefinition.XmlInfo = null;
            // 
            // TrackLinkProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(308, 310);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.nameDefinition,
                                                                          this.buttonCancel,
                                                                          this.buttonOK,
                                                                          this.radioButtonLinked,
                                                                          this.radioButtonNotLinked,
                                                                          this.trackLinkTree});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrackLinkProperties";
            this.ShowInTaskbar = false;
            this.Text = "Track Link Properties";
            this.ResumeLayout(false);

        }
        #endregion

        private void checkBoxVisible_CheckedChanged(object sender, System.EventArgs e) {
            updateDependencies();
        }

        private void radioButtonNotLinked_CheckedChanged(object sender, System.EventArgs e) {
            updateDependencies();
        }

        private void radioButtonLinked_CheckedChanged(object sender, System.EventArgs e) {
            updateDependencies();
        }

        private void buttonOK_Click(object sender, System.EventArgs e) {
            // Validate the dialog
            if (!nameDefinition.Commit())
                return;

            if (radioButtonLinked.Checked) {
                if (trackLinkTree.SelectedTrackLink == null) {
                    MessageBox.Show(this, "You must select a valid track-link", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    trackLinkTree.Focus();
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
