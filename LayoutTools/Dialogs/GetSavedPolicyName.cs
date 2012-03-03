using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
	public partial class GetSavedPolicyName : Form {
		public GetSavedPolicyName(LayoutPoliciesCollection policies) {
			InitializeComponent();

			foreach(LayoutPolicyInfo policy in policies)
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

		private void buttonOK_Click(object sender, EventArgs e) {
			if(string.IsNullOrEmpty(Name)) {
				MessageBox.Show("Please enter name for the saved definition");
				comboBoxName.Focus();
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}