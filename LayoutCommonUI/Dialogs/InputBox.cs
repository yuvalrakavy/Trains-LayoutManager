namespace LayoutManager.CommonUI.Dialogs {
    [Flags]
    public enum InputBoxValidationOptions {
        AllowEmpty, IntegerNumber
    }

    /// <summary>
    /// Summary description for InputBox.
    /// </summary>
    public partial class InputBox : Form {


        public InputBox(String caption, string prompt) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Text = caption;
            this.labelPrompt.Text = prompt;
        }

        public string Input {
            get {
                return textBox.Text;
            }

            set {
                textBox.Text = value;
            }
        }

        public InputBoxValidationOptions ValidationOptions { get; set; } = 0;

        public static string? Show(String caption, string prompt, InputBoxValidationOptions validationOptions) {
            InputBox inputBox = new(caption, prompt) {
                ValidationOptions = validationOptions
            };
            return inputBox.ShowDialog() == DialogResult.OK ? inputBox.Input : null;
        }

        public static string? Show(String caption, string prompt) => InputBox.Show(caption, prompt, 0);

        public static string? Show(String prompt) => InputBox.Show("", prompt);

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


        private void ButtonOK_Click(object? sender, EventArgs e) {
            if ((ValidationOptions & InputBoxValidationOptions.AllowEmpty) == 0 && textBox.Text.Trim() == "") {
                MessageBox.Show(this, "Value cannot be empty", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox.Focus();
                return;
            }

            if ((ValidationOptions & InputBoxValidationOptions.IntegerNumber) != 0) {
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
