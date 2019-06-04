using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.CommonUI.Controls.XmlInfoEditorForms {
    /// <summary>
    /// Summary description for AttributeInfo.
    /// </summary>
    public class AttributeInfo : Form {
        private Button buttonOK;
        private Button buttonCancel;
        private TextBox textBoxValue;
        private Label labelName;
        private TextBox textBoxName;
        private Label labelValue;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public AttributeInfo() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public AttributeInfo(XmlAttribute a) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            textBoxName.Text = a.Name;
            textBoxValue.Text = a.Value;
        }

        public string AttributeName => textBoxName.Text;

        public string AttributeValue => textBoxValue.Text;

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
            this.textBoxValue = new TextBox();
            this.labelName = new Label();
            this.textBoxName = new TextBox();
            this.labelValue = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(56, 40);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(184, 20);
            this.textBoxValue.TabIndex = 3;
            this.textBoxValue.Text = "";
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(8, 16);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(48, 16);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(56, 14);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(184, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Text = "";
            // 
            // labelValue
            // 
            this.labelValue.Location = new System.Drawing.Point(8, 42);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(48, 16);
            this.labelValue.TabIndex = 2;
            this.labelValue.Text = "Value:";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(80, 64);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(168, 64);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // AttributeInfo
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(260, 98);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.buttonCancel,
                                                                          this.textBoxValue,
                                                                          this.labelValue,
                                                                          this.textBoxName,
                                                                          this.labelName});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AttributeInfo";
            this.ShowInTaskbar = false;
            this.Text = "Attribute Information";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (textBoxName.Text.Trim() == "") {
                MessageBox.Show(this, "Attribute name must be entered", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
