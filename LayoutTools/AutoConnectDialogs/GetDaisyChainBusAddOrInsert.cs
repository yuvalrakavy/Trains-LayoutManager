using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LayoutManager.Components;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetDaisyChainBusAddOrInsert.
    /// </summary>
    public class GetDaisyChainBusAddOrInsert : Form {
        private RadioButton radioButtonInsert;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonAdd;
        private Label labelTitle;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public GetDaisyChainBusAddOrInsert(LayoutControlModuleLocationComponent moduleLocation) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            labelTitle.Text = Regex.Replace(labelTitle.Text, "LOCATION", moduleLocation.Name);

            radioButtonInsert.Checked = true;
        }

        public bool InsertModule => radioButtonInsert.Checked;

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
            this.labelTitle = new Label();
            this.radioButtonInsert = new RadioButton();
            this.radioButtonAdd = new RadioButton();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Location = new System.Drawing.Point(8, 8);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(264, 56);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "A new module is being added to a \"daisy\" chain bus. The last module in the chain " +
                "is not located in this control module location (LOCATION). What would you like t" +
                "o do:";
            // 
            // radioButtonInsert
            // 
            this.radioButtonInsert.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioButtonInsert.Location = new System.Drawing.Point(13, 72);
            this.radioButtonInsert.Name = "radioButtonInsert";
            this.radioButtonInsert.Size = new System.Drawing.Size(264, 56);
            this.radioButtonInsert.TabIndex = 1;
            this.radioButtonInsert.Text = "Insert the new module in the middle of the chain, so it is connected to a control" +
                " module already that is located already in this control module location";
            this.radioButtonInsert.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // radioButtonAdd
            // 
            this.radioButtonAdd.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.radioButtonAdd.Location = new System.Drawing.Point(13, 136);
            this.radioButtonAdd.Name = "radioButtonAdd";
            this.radioButtonAdd.Size = new System.Drawing.Size(267, 32);
            this.radioButtonAdd.TabIndex = 2;
            this.radioButtonAdd.Text = "Connect the new control module to the end of the chain";
            this.radioButtonAdd.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(128, 176);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "Continue";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 176);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            // 
            // GetDaisyChainBusAddOrInsert
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(288, 206);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.radioButtonAdd);
            this.Controls.Add(this.radioButtonInsert);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GetDaisyChainBusAddOrInsert";
            this.ShowInTaskbar = false;
            this.Text = "Add Module to Daisy Chain Bus";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}
