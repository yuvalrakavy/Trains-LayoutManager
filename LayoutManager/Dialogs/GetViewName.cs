using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for GetAreasName.
    /// </summary>
    public class GetViewName : Form {
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private TextBox textBoxViewName;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public string ViewName {
            get {
                return textBoxViewName.Text;
            }

            set {
                textBoxViewName.Text = value;
            }
        }

        public GetViewName() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
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
            this.textBoxViewName = new TextBox();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(66, 32);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // textBoxViewName
            // 
            this.textBoxViewName.Location = new System.Drawing.Point(48, 8);
            this.textBoxViewName.Name = "textBoxViewName";
            this.textBoxViewName.Size = new System.Drawing.Size(232, 20);
            this.textBoxViewName.TabIndex = 1;
            this.textBoxViewName.Text = "";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(154, 32);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // GetViewName
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(298, 67);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonCancel,
                                                                          this.buttonOK,
                                                                          this.textBoxViewName,
                                                                          this.label1});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetViewName";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New view name";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object? sender, System.EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
