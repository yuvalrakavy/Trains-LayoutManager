using System;
using System.Windows.Forms;
using LayoutManager.Model;
using System.Xml;

namespace LayoutManager.Dialogs {
    public partial class ExportPolicies : Form {
        public ExportPolicies() {
            InitializeComponent();

            foreach (LayoutPolicyType policyType in LayoutModel.StateManager.PolicyTypes) {
                TreeNode sectionNode = new PolicyTypeTreeNode(policyType);

                foreach (LayoutPolicyInfo policy in policyType.Policies)
                    if (policy.GlobalPolicy)
                        sectionNode.Nodes.Add(new PolicyTreeNode(policy));

                if (sectionNode.Nodes.Count > 0) {
                    treeViewScripts.Nodes.Add(sectionNode);
                    sectionNode.Expand();
                }
            }

            UpdateButtons(null, null);
        }

        void UpdateButtons(object sender, EventArgs e) {
            TreeNode selected = treeViewScripts.SelectedNode;

            buttonAdd.Enabled = selected != null;

            selected = treeViewExportedScripts.SelectedNode;
            buttonRemove.Enabled = selected != null;
        }

        private void treeViewScriptsAndExportedScripts_AfterSelect(object sender, TreeViewEventArgs e) {
            UpdateButtons(sender, e);
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                textBoxFilename.Text = saveFileDialog.FileName;
        }

        private void addScript(PolicyTreeNode policyNode) {
            PolicyTypeTreeNode policySectionNode = (PolicyTypeTreeNode)policyNode.Parent;
            PolicyTypeTreeNode theExportedSection = null;

            // Check if exported script contains this section
            foreach (PolicyTypeTreeNode exportedSection in treeViewExportedScripts.Nodes)
                if (policySectionNode.PolicyType.Policies == exportedSection.PolicyType.Policies) {
                    theExportedSection = exportedSection;
                    break;
                }

            if (theExportedSection == null) {
                theExportedSection = new PolicyTypeTreeNode(policySectionNode.PolicyType);
                treeViewExportedScripts.Nodes.Add(theExportedSection);
            }

            bool found = false;

            foreach (PolicyTreeNode p in theExportedSection.Nodes)
                if (p.Policy == policyNode.Policy) {
                    found = true;
                    break;
                }

            if (!found) {
                theExportedSection.Nodes.Add(new PolicyTreeNode(policyNode.Policy));
                theExportedSection.Expand();
                policyNode.Exported = true;
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e) {

            if (treeViewScripts.SelectedNode is PolicyTreeNode policyNode)
                addScript(policyNode);
            else {
                foreach (PolicyTreeNode p in treeViewScripts.SelectedNode.Nodes)
                    addScript(p);
            }
        }

        private void removeScript(PolicyTreeNode policyNode, bool removeEmptySection) {
            PolicyTypeTreeNode policySectionNode = (PolicyTypeTreeNode)policyNode.Parent;

            foreach (PolicyTypeTreeNode s in treeViewScripts.Nodes) {
                if (s.PolicyType == policySectionNode.PolicyType) {
                    foreach (PolicyTreeNode p in s.Nodes)
                        if (p.Policy == policyNode.Policy) {
                            p.Exported = false;
                            break;
                        }
                    break;
                }
            }

            policySectionNode.Nodes.Remove(policyNode);
            if (policySectionNode.Nodes.Count == 0 && removeEmptySection)
                treeViewExportedScripts.Nodes.Remove(policySectionNode);
        }


        private void buttonRemove_Click(object sender, EventArgs e) {

            if (treeViewExportedScripts.SelectedNode is PolicyTreeNode selectedPolicyNode)
                removeScript(selectedPolicyNode, true);
            else {
                PolicyTypeTreeNode policySectionNode = (PolicyTypeTreeNode)treeViewExportedScripts.SelectedNode;
                PolicyTreeNode[] nodes = new PolicyTreeNode[policySectionNode.Nodes.Count];

                policySectionNode.Nodes.CopyTo(nodes, 0);

                foreach (PolicyTreeNode p in nodes)
                    removeScript(p, false);

                treeViewExportedScripts.Nodes.Remove(policySectionNode);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(textBoxFilename.Text)) {
                MessageBox.Show(this, "Please provide a name for the file to which scripts are to be exported", "Missing filename", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxFilename.Focus();
                return;
            }

            if (treeViewExportedScripts.Nodes.Count == 0) {
                MessageBox.Show(this, "You did not specify any script to be exported", "No scripts to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                treeViewScripts.Focus();
                return;
            }

            XmlDocument doc = new XmlDocument();
            XmlElement exportedPoliciesElement = doc.CreateElement("ExportedScripts");

            doc.AppendChild(exportedPoliciesElement);

            foreach (PolicyTypeTreeNode policySectionNode in treeViewExportedScripts.Nodes) {
                foreach (PolicyTreeNode policyNode in policySectionNode.Nodes) {
                    XmlElement policyElement = (XmlElement)doc.ImportNode(policyNode.Policy.Element, true);

                    exportedPoliciesElement.AppendChild(policyElement);
                }
            }

            doc.Save(textBoxFilename.Text);

            MessageBox.Show(this, "Scripts were exported to: " + textBoxFilename.Text, "Script export done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void treeViewScripts_DoubleClick(object sender, EventArgs e) {
            buttonAdd.PerformClick();
        }

        private void treeViewExportedScripts_DoubleClick(object sender, EventArgs e) {
            buttonRemove.PerformClick();
        }
    }

    class PolicyTypeTreeNode : TreeNode {
        readonly LayoutPolicyType policyType;

        public PolicyTypeTreeNode(LayoutPolicyType policyType) {
            this.policyType = policyType;
            Text = policyType.DisplayName;
        }

        public LayoutPolicyType PolicyType => policyType;
    }

    class PolicyTreeNode : TreeNode {
        readonly LayoutPolicyInfo policy;
        bool exported;

        public PolicyTreeNode(LayoutPolicyInfo policy) {
            this.policy = policy;
            this.Text = policy.Name;
        }

        public LayoutPolicyInfo Policy => policy;

        public bool Exported {
            get {
                return exported;
            }

            set {
                exported = value;
            }
        }
    }
}
