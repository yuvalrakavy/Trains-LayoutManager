using LayoutManager.Model;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Dialogs {
    public partial class ImportPolicies : Form {
        private readonly Font boldFont;

        public ImportPolicies() {
            InitializeComponent();

            boldFont = new Font(listViewScripts.Font, FontStyle.Bold | listViewScripts.Font.Style);

            UpdateButtons();
        }

        private void UpdateButtons() {
            buttonViewScript.Enabled = listViewScripts.SelectedItems.Count > 0;
        }

        private void ButtonBrowse_Click(object? sender, EventArgs e) {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                textBoxFilename.Text = openFileDialog.FileName;
        }

        private void WizardPageSetFilename_CloseFromNext(object? sender, Gui.Wizard.PageEventArgs e) {
            try {
                XmlDocument importedScriptsDoc = new();

                importedScriptsDoc.Load(textBoxFilename.Text);

                foreach (XmlElement policyElement in importedScriptsDoc.DocumentElement!)
                    listViewScripts.Items.Add(new PolicyItem(listViewScripts, boldFont, policyElement));
            }
            catch (IOException ex) {
                MessageBox.Show(this, textBoxFilename.Text + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Page = wizardPageSetFilename;
            }
            catch (XmlException ex) {
                MessageBox.Show(this, textBoxFilename.Text + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Page = wizardPageSetFilename;
            }
        }

        private class PolicyItem : ListViewItem {
            public PolicyItem(ListView listViewScripts, Font boldFont, XmlElement policyElement) {
                Policy = new LayoutPolicyInfo(policyElement);

                ListViewGroup group = listViewScripts.Groups[Policy.Scope];
                var policyType = LayoutModel.StateManager.PolicyTypes.Find((LayoutPolicyType pt) => pt.ScopeName == Policy.Scope);

                if (policyType != null) {
                    if (group == null) {
                        group = new ListViewGroup(Policy.Scope, policyType.DisplayName);
                        listViewScripts.Groups.Add(group);
                    }

                    group.Items.Add(this);

                    if (policyType.Policies[Policy.Id] != null || policyType.Policies[Policy.Name] != null)
                        Font = boldFont;

                    SubItems.Add("");
                    Update();
                    Checked = true;
                }
            }

            public void Update() {
                Text = Policy.Name;
                SubItems[1].Text = Policy.Text;
            }

            public LayoutPolicyInfo Policy { get; }
        }

        private void ImportPolicies_FormClosed(object? sender, FormClosedEventArgs e) {
            boldFont.Dispose();
        }

        private void WizardPageSelectScripts_CloseFromNext(object? sender, Gui.Wizard.PageEventArgs e) {
            foreach (PolicyItem policyItem in listViewScripts.Items) {
                if (policyItem.Checked) {
                    LayoutPolicyInfo? policy;
                    var policyType = LayoutModel.StateManager.PolicyTypes.Find((LayoutPolicyType pt) => pt.ScopeName == policyItem.Group.Name);

                    if (policyType != null) {
                        policy = policyType.Policies[policyItem.Policy.Id];
                        if (policy == null)
                            policy = policyType.Policies[policyItem.Policy.Name];

                        if (policy != null) {
                            policy.Name = policyItem.Policy.Name;
                            policy.Apply = policyItem.Policy.Apply;
                            policy.EventScriptElement = policyItem.Policy.EventScriptElement;
                            policy.GlobalPolicy = policyItem.Policy.GlobalPolicy;
                            policy.ShowInMenu = policyItem.Policy.ShowInMenu;

                            EventManager.Event(new LayoutEvent("policy-updated", policy, policyType.Policies));
                        }
                        else {
                            policy = policyItem.Policy;
                            policyType.Policies.Update(policy);
                        }
                    }
                }
            }
        }

        private void ButtonViewScript_Click(object? sender, EventArgs e) {
            if (listViewScripts.SelectedItems.Count > 0) {
                PolicyItem policyItem = (PolicyItem)listViewScripts.SelectedItems[0];

                CommonUI.Dialogs.PolicyDefinition d = new(policyItem.Policy);

                d.ShowDialog(this);
                policyItem.Update();
            }
        }

        private void ListViewScripts_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }
    }
}