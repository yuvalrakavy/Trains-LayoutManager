using LayoutManager.Model;
using System;
using System.ComponentModel;
using System.Windows.Forms;

#nullable enable
#pragma warning disable IDE0069
namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for SetControlModuleLabel.
    /// </summary>
    public class SetControlModuleLabel : Form {
        private Label label1;
        private TextBox textBoxLabel;
        private Button buttonOK;
        private Button buttonCancel;

        private readonly ControlModule module;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container? components = null;

        #nullable disable
        public SetControlModuleLabel(ControlModule module) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.module = module;

            if (module.Label == null)
                textBoxLabel.Text = "";
            else
                textBoxLabel.Text = module.Label;
        }
        #nullable enable

        public string? Label => string.IsNullOrWhiteSpace(textBoxLabel.Text) ? null : textBoxLabel.Text;

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
            this.textBoxLabel = new TextBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter a label to attach to the control module:";
            // 
            // textBoxLabel
            // 
            this.textBoxLabel.Location = new System.Drawing.Point(24, 40);
            this.textBoxLabel.Name = "textBoxLabel";
            this.textBoxLabel.Size = new System.Drawing.Size(240, 20);
            this.textBoxLabel.TabIndex = 1;
            this.textBoxLabel.Text = "";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(109, 72);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(189, 72);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // SetControlModuleLabel
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(272, 102);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetControlModuleLabel";
            this.ShowInTaskbar = false;
            this.Text = "Set Control Module Label";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            var validationError = (string?)EventManager.Event(new LayoutEvent("validate-control-module-label", module, null).SetOption("ModuleTypeName", module.ModuleTypeName).SetOption("Label", textBoxLabel.Text));

            if(validationError != null) {
                MessageBox.Show(validationError, "Invalid label", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxLabel.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
