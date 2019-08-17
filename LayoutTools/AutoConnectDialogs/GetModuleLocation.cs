using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Components;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetModuleLocation.
    /// </summary>
    public class GetModuleLocation : Form {
        private Label label1;
        private Label label2;
        private Label label3;
        private PictureBox pictureBox1;
        private Label label4;
        private ComboBox comboBoxModuleLocation;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public GetModuleLocation(IEnumerable<LayoutControlModuleLocationComponent> moduleLocations) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            comboBoxModuleLocation.Items.Add(new Item("(Any)"));

            foreach (LayoutControlModuleLocationComponent moduleLocation in moduleLocations)
                comboBoxModuleLocation.Items.Add(new Item(moduleLocation));

            if (comboBoxModuleLocation.Items.Count > 0)
                comboBoxModuleLocation.SelectedIndex = 0;
        }

        public LayoutControlModuleLocationComponent ModuleLocation {
            get {
                return comboBoxModuleLocation.SelectedItem == null ? null : ((Item)comboBoxModuleLocation.SelectedItem).ModuleLocation;
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GetModuleLocation));
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.comboBoxModuleLocation = new ComboBox();
            this.pictureBox1 = new PictureBox();
            this.label4 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "This area does not contain any control module location components (it is recommen" +
                "ded that you define one).";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(232, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "However, there are control module location components in other areas.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(232, 48);
            this.label3.TabIndex = 2;
            this.label3.Text = "Please select the control module location containing the control module to which " +
                "you would like to connect the component:";
            // 
            // comboBoxModuleLocation
            // 
            this.comboBoxModuleLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxModuleLocation.Location = new System.Drawing.Point(16, 144);
            this.comboBoxModuleLocation.Name = "comboBoxModuleLocation";
            this.comboBoxModuleLocation.Size = new System.Drawing.Size(232, 21);
            this.comboBoxModuleLocation.TabIndex = 3;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            this.pictureBox1.Location = new System.Drawing.Point(256, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 176);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(232, 40);
            this.label4.TabIndex = 4;
            this.label4.Text = "Select (Any) to use any module without restricting it to a specific control modul" +
                "e location.";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(136, 216);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "Continue";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(216, 216);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            // 
            // GetModuleLocation
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(296, 246);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.comboBoxModuleLocation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GetModuleLocation";
            this.ShowInTaskbar = false;
            this.Text = "Select Control Module Location";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private class Item {
            private readonly string text;

            public Item(LayoutControlModuleLocationComponent moduleLocation) {
                this.ModuleLocation = moduleLocation;
                text = moduleLocation.Name;
            }

            public Item(string text) {
                this.ModuleLocation = null;
                this.text = text;
            }

            public LayoutControlModuleLocationComponent ModuleLocation { get; }

            public override string ToString() => text;
        }
    }
}
