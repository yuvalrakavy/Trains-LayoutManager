using System;
using System.Drawing;
using System.Windows.Forms;
using LayoutManager.Model;
using System.Xml;
using System.IO;

namespace LayoutManager.Dialogs {
    public partial class ImportPolicies : Form {
        readonly Font boldFont;

        public ImportPolicies() {
            InitializeComponent();

            boldFont = new Font(listViewScripts.Font, FontStyle.Bold | listViewScripts.Font.Style);

            UpdateButtons();
        }

        void UpdateButtons() {
            buttonViewScript.Enabled = listViewScripts.SelectedItems.Count > 0;
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                textBoxFilename.Text = openFileDialog.FileName;
        }

        private void wizardPageSetFilename_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e) {
            try {
                XmlDocument importedScriptsDoc = new XmlDocument();

                importedScriptsDoc.Load(textBoxFilename.Text);

                foreach (XmlElement policyElement in importedScriptsDoc.DocumentElement)
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

        class PolicyItem : ListViewItem {
            readonly LayoutPolicyInfo policy;

            public PolicyItem(ListView listViewScripts, Font boldFont, XmlElement policyElement) {
                policy = new LayoutPolicyInfo(policyElement);

                ListViewGroup group = listViewScripts.Groups[policy.Scope];
                LayoutPolicyType policyType = LayoutModel.StateManager.PolicyTypes.Find(delegate (LayoutPolicyType pt) { return pt.ScopeName == policy.Scope; });

                if (policyType != null) {

                    if (group == null) {
                        group = new ListViewGroup(policy.Scope, policyType.DisplayName);
                        listViewScripts.Groups.Add(group);
                    }

                    group.Items.Add(this);

                    if (policyType.Policies[policy.Id] != null || policyType.Policies[policy.Name] != null)
                        Font = boldFont;

                    SubItems.Add("");
                    Update();
                    Checked = true;
                }
            }

            public void Update() {
                Text = policy.Name;
                SubItems[1].Text = policy.Text;
            }

            public LayoutPolicyInfo Policy => policy;
        }

        private void ImportPolicies_FormClosed(object sender, FormClosedEventArgs e) {
            boldFont.Dispose();
        }

        private void wizardPageSelectScripts_CloseFromNext(object sender, Gui.Wizard.PageEventArgs e) {
            foreach (PolicyItem policyItem in listViewScripts.Items) {
                if (policyItem.Checked) {
                    LayoutPolicyInfo policy;
                    LayoutPolicyType policyType = LayoutModel.StateManager.PolicyTypes.Find(delegate (LayoutPolicyType pt) { return pt.ScopeName == policyItem.Group.Name; });

                    policy = policyType.Policies[policyItem.Policy.Id];
                    if (policy == null)
                        policy = policyType.Policies[policyItem.Policy.Name];

                    if (policy != null) {
                        policy.Name = policyItem.Policy.Name;
                        policy.Apply = policyItem.Policy.Apply;
                        policy.EventScriptElement = policyItem.Policy.EventScriptElement;
                        policy.GlobalPolicy = policyItem.Policy.GlobalPolicy;
                        policy.ShowInMenu = policyItem.Policy.ShowInMenu;

                        EventManager.Event(new LayoutEvent("policy-updated", policy, policyType.Policies, null));
                    }
                    else {
                        policy = policyItem.Policy;
                        policyType.Policies.Update(policy);
                    }
                }
            }
        }

        private void buttonViewScript_Click(object sender, EventArgs e) {
            if (listViewScripts.SelectedItems.Count > 0) {
                PolicyItem policyItem = (PolicyItem)listViewScripts.SelectedItems[0];

                LayoutManager.CommonUI.Dialogs.PolicyDefinition d = new LayoutManager.CommonUI.Dialogs.PolicyDefinition(policyItem.Policy);

                d.ShowDialog(this);
                policyItem.Update();
            }
        }

        private void listViewScripts_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateButtons();
        }

    }
}