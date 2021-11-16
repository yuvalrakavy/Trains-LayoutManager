using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StandardPositionProperties.
    /// </summary>
    public class StandardPositionProperties : Form {
        private Label label1;
        private TextBox textBoxTitle;
        private CommonUI.Controls.PositionDefinition positionDefinition;
        private Button buttonOK;
        private Button buttonCancel;
        private Label label2;
        private TextBox textBoxRef;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public StandardPositionProperties(LayoutPositionInfo positionProvider) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            positionDefinition.Set(positionProvider);
            textBoxTitle.Text = positionProvider.GetAttribute("Title");
            textBoxRef.Text = positionProvider.Ref;
        }

        public void Get(LayoutPositionInfo positionProvider) {
            positionDefinition.Get(positionProvider);
            positionProvider.SetAttributeValue("Title", textBoxTitle.Text);
            if (textBoxRef.Text.Trim() == "")
                positionProvider.Ref = null;
            else
                positionProvider.Ref = textBoxRef.Text;
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
            this.buttonOK = new Button();
            this.label1 = new Label();
            this.buttonCancel = new Button();
            this.positionDefinition = new CommonUI.Controls.PositionDefinition();
            this.textBoxTitle = new TextBox();
            this.label2 = new Label();
            this.textBoxRef = new TextBox();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(128, 176);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // label1
            // 
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 176);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            // 
            // positionDefinition
            // 
            this.positionDefinition.Location = new System.Drawing.Point(8, 64);
            this.positionDefinition.Name = "positionDefinition";
            this.positionDefinition.Size = new System.Drawing.Size(272, 104);
            this.positionDefinition.TabIndex = 2;
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(48, 8);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(216, 20);
            this.textBoxTitle.TabIndex = 1;
            this.textBoxTitle.Text = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Ref:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxRef
            // 
            this.textBoxRef.Location = new System.Drawing.Point(48, 33);
            this.textBoxRef.Name = "textBoxRef";
            this.textBoxRef.Size = new System.Drawing.Size(216, 20);
            this.textBoxRef.TabIndex = 6;
            this.textBoxRef.Text = "";
            // 
            // StandardPositionProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(296, 212);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxRef);
            this.Controls.Add(this.textBoxTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.positionDefinition);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StandardPositionProperties";
            this.ShowInTaskbar = false;
            this.Text = "Position Properties";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object? sender, System.EventArgs e) {
            if (textBoxTitle.Text.Trim() == "") {
                MessageBox.Show(this, "You must specify title", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxTitle.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
