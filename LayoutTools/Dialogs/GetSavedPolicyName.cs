using LayoutManager.Model;
using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class GetSavedPolicyName : Form {
        public GetSavedPolicyName(LayoutPoliciesCollection policies) {
            InitializeComponent();

            foreach (LayoutPolicyInfo policy in policies)
                comboBoxName.Items.Add(policy.Name);
        }

        public string PolicyName {
            get {
                return comboBoxName.Text;
            }

            set {
                comboBoxName.Text = value;
            }
        }

        public bool IsGlobalPolicy {
            get {
                return checkBoxGlobalPolicy.Checked;
            }

            set {
                checkBoxGlobalPolicy.Checked = true;
            }
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (string.IsNullOrEmpty(Name)) {
                MessageBox.Show("Please enter name for the saved definition");
                comboBoxName.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}