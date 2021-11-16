using System;
using System.Windows.Forms;

namespace Intellibox.Dialogs {
    public partial class SOdefinition : Form {
        public SOdefinition(SOinfo so) {
            InitializeComponent();
            SOinfoBindingSource.DataSource = so;
            //			SOinfoBindingSource.SuspendBinding();
        }

        private void ButtonOk_Click(object? sender, EventArgs e) {
            //			SOinfoBindingSource.ResumeBinding();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TextBoxNumber_Validated(object? sender, EventArgs e) {
            errorProvider.SetError(textBoxNumber, "");

            if (textBoxNumber.Text.Trim().Length == 0)
                errorProvider.SetError(textBoxNumber, "SO number must be specified");
            else if (!int.TryParse(textBoxNumber.Text, out int _))
                errorProvider.SetError(textBoxNumber, "SO number is not a valid number");
        }

        private void TextBoxValue_TextChanged(object? sender, EventArgs e) {
            errorProvider.SetError(textBoxValue, "");

            if (textBoxValue.Text.Trim().Length == 0)
                errorProvider.SetError(textBoxValue, "SO value must be specified");
            else if (!int.TryParse(textBoxValue.Text, out int _))
                errorProvider.SetError(textBoxValue, "SO value is not a valid number");
        }
    }
}