using LayoutManager.Model;
using System;
using System.Windows.Forms;
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

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
            TreeNode selected = treeViewScripts.SelectedNode;

            buttonAdd.Enabled = selected != null;

            selected = treeViewExportedScripts.SelectedNode;
            buttonRemove.Enabled = selected != null;
        }

        private void TreeViewScriptsAndExportedScripts_AfterSelect(object? sender, TreeViewEventArgs e) {
            UpdateButtons(sender, e);
        }

        private void ButtonBrowse_Click(object? sender, EventArgs e) {
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                textBoxFilename.Text = saveFileDialog.FileName;
        }

        private void AddScript(PolicyTreeNode policyNode) {
            PolicyTypeTreeNode policySectionNode = (PolicyTypeTreeNode)policyNode.Parent;
            PolicyTypeTreeNode? theExportedSection = null;

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

        private void ButtonAdd_Click(object? sender, EventArgs e) {
            if (treeViewScripts.SelectedNode is PolicyTreeNode policyNode)
                AddScript(policyNode);
            else {
                foreach (PolicyTreeNode p in treeViewScripts.SelectedNode.Nodes)
                    AddScript(p);
            }
        }

        private void RemoveScript(PolicyTreeNode policyNode, bool removeEmptySection) {
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

        private void ButtonRemove_Click(object? sender, EventArgs e) {
            if (treeViewExportedScripts.SelectedNode is PolicyTreeNode selectedPolicyNode)
                RemoveScript(selectedPolicyNode, true);
            else {
                PolicyTypeTreeNode policySectionNode = (PolicyTypeTreeNode)treeViewExportedScripts.SelectedNode;
                PolicyTreeNode[] nodes = new PolicyTreeNode[policySectionNode.Nodes.Count];

                policySectionNode.Nodes.CopyTo(nodes, 0);

                foreach (PolicyTreeNode p in nodes)
                    RemoveScript(p, false);

                treeViewExportedScripts.Nodes.Remove(policySectionNode);
            }
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
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

            XmlDocument doc = new();
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

        private void TreeViewScripts_DoubleClick(object? sender, EventArgs e) {
            buttonAdd.PerformClick();
        }

        private void TreeViewExportedScripts_DoubleClick(object? sender, EventArgs e) {
            buttonRemove.PerformClick();
        }
    }

    internal class PolicyTypeTreeNode : TreeNode {
        public PolicyTypeTreeNode(LayoutPolicyType policyType) {
            this.PolicyType = policyType;
            Text = policyType.DisplayName;
        }

        public LayoutPolicyType PolicyType { get; }
    }

    internal class PolicyTreeNode : TreeNode {
        public PolicyTreeNode(LayoutPolicyInfo policy) {
            this.Policy = policy;
            this.Text = policy.Name;
        }

        public LayoutPolicyInfo Policy { get; }

        public bool Exported { get; set; }
    }
}
