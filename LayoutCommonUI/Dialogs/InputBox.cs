using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Dialogs {
    [Flags]
    public enum InputBoxValidationOptions {
        AllowEmpty, IntegerNumber
    }

    /// <summary>
    /// Summary description for InputBox.
    /// </summary>
    public class InputBox : Form {
        private Label labelPrompt;
        private TextBox textBox;
        private Button buttonOK;
        private Button buttonCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private InputBoxValidationOptions validationOptions = 0;

        public InputBox(String caption, String prompt) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Text = caption;
            this.labelPrompt.Text = prompt;
        }

        public String Input {
            get {
                return textBox.Text;
            }

            set {
                textBox.Text = value;
            }
        }

        public InputBoxValidationOptions ValidationOptions {
            get {
                return validationOptions;
            }

            set {
                validationOptions = value;
            }
        }

        public static String Show(String caption, String prompt, InputBoxValidationOptions validationOptions) {
            InputBox inputBox = new InputBox(caption, prompt) {
                ValidationOptions = validationOptions
            };
            if (inputBox.ShowDialog() == DialogResult.OK)
                return inputBox.Input;
            else
                return null;
        }

        public static string Show(String caption, String prompt) => InputBox.Show(caption, prompt, 0);

        public static string Show(String prompt) => InputBox.Show("", prompt);

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
            this.buttonCancel = new Button();
            this.textBox = new TextBox();
            this.labelPrompt = new Label();
            this.buttonOK = new Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(216, 69);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(8, 40);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(280, 20);
            this.textBox.TabIndex = 1;
            this.textBox.Text = "";
            // 
            // labelPrompt
            // 
            this.labelPrompt.Location = new System.Drawing.Point(8, 16);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(280, 16);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Prompt";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(136, 69);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // InputBox
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(300, 98);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonCancel,
                                                                          this.buttonOK,
                                                                          this.textBox,
                                                                          this.labelPrompt});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InputBox";
            this.ShowInTaskbar = false;
            this.Text = "InputBox";
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if ((validationOptions & InputBoxValidationOptions.AllowEmpty) == 0 && textBox.Text.Trim() == "") {
                MessageBox.Show(this, "Value cannot be empty", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox.Focus();
                return;
            }

            if ((validationOptions & InputBoxValidationOptions.IntegerNumber) != 0) {
                try {
                    int n = int.Parse(textBox.Text);
                }
                catch (FormatException) {
                    MessageBox.Show(this, "Value is not a valid number", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Focus();
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }
    }
}
