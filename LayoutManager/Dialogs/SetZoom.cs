using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for SetZoom.
    /// </summary>
    public class SetZoom : Form {
        private Label label1;
        private TextBox textBoxZoomFactor;
        private Label label2;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public SetZoom() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public int ZoomFactor {
            set {
                textBoxZoomFactor.Text = value.ToString();
            }

            get {
                return System.Int32.Parse(textBoxZoomFactor.Text);
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
            this.label1 = new Label();
            this.textBoxZoomFactor = new TextBox();
            this.label2 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 9);
            this.label1.Name = "label1";
            this.label1.TabIndex = 0;
            this.label1.Text = "Set zoom factor to: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxZoomFactor
            // 
            this.textBoxZoomFactor.Location = new System.Drawing.Point(112, 12);
            this.textBoxZoomFactor.Name = "textBoxZoomFactor";
            this.textBoxZoomFactor.Size = new System.Drawing.Size(40, 20);
            this.textBoxZoomFactor.TabIndex = 1;
            this.textBoxZoomFactor.Text = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(152, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "%";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(176, 8);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(64, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(176, 40);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            // 
            // SetZoom
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(250, 72);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonCancel,
                                                                          this.buttonOK,
                                                                          this.label2,
                                                                          this.textBoxZoomFactor,
                                                                          this.label1});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SetZoom";
            this.ShowInTaskbar = false;
            this.Text = "Set Zoom";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            try {
                int zoomFactor = zoomFactor = System.Int32.Parse(textBoxZoomFactor.Text);

                if (zoomFactor == 0) {
                    MessageBox.Show(this, "Zoom factor cannot be zero", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxZoomFactor.Focus();
                    DialogResult = DialogResult.None;
                    return;
                }
            }
            catch (Exception) {
                MessageBox.Show(this, "Invalid zoom factor", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxZoomFactor.Focus();
                DialogResult = DialogResult.None;
                return;
            }

            this.Close();
        }
    }
}
