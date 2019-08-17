using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    public interface IPolicyListCustomizer {
        bool IsPolicyChecked(LayoutPolicyInfo policy);

        void SetPolicyChecked(LayoutPolicyInfo policy, bool checkValue);
    }

    /// <summary>
    /// Summary description for PolicyList.
    /// </summary>
    public class PolicyList : System.Windows.Forms.UserControl, IPolicyListCustomizer, IControlSupportViewOnly {
        private ListView listViewPolicies;
        private ColumnHeader columnHeaderName;
        private Button buttonNew;
        private Button buttonRemove;
        private Button buttonEdit;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }
        private readonly CommonUI.ListViewStringColumnsSorter sorter;
        private int updateOnMouseUp = -1;
        private bool editingPolicy = false;

        public PolicyList() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            sorter = new CommonUI.ListViewStringColumnsSorter(listViewPolicies);
            Customizer = this;
        }

        public string Scope { get; set; } = "TripPlan";

        public LayoutPoliciesCollection Policies { get; set; }

        public IPolicyListCustomizer Customizer { get; set; }

        public bool ViewOnly { get; set; } = false;

        public bool ShowIfRunning { get; set; } = false;

        public bool ShowPolicyDefinition { get; set; } = false;

        public virtual void Initialize() {
            listViewPolicies.Items.Clear();

            if (ShowIfRunning)
                listViewPolicies.Columns.Add("Status", 70, HorizontalAlignment.Left);

            if (ShowPolicyDefinition)
                listViewPolicies.Columns.Add("Policy", listViewPolicies.Width - 200, HorizontalAlignment.Left);

            if (!ShowIfRunning && !ShowPolicyDefinition)
                listViewPolicies.Columns[0].Width = listViewPolicies.Width;
            else if (ShowIfRunning)
                listViewPolicies.Columns[0].Width = listViewPolicies.Width - 74;
            else if (ShowPolicyDefinition)
                listViewPolicies.Columns[0].Width = listViewPolicies.Width - 204;

            foreach (LayoutPolicyInfo policy in Policies)
                listViewPolicies.Items.Add(new PolicyItem(this, policy));

            if (Customizer == null)
                listViewPolicies.CheckBoxes = false;

            UpdateButtons(null, null);
            EventManager.AddObjectSubscriptions(this);
        }

        protected PolicyItem GetSelection() {
            return listViewPolicies.SelectedItems.Count == 0 ? null : (PolicyItem)listViewPolicies.SelectedItems[0];
        }

        protected virtual void UpdateButtons(object sender, EventArgs e) {
            if (GetSelection() == null) {
                buttonEdit.Enabled = false;
                buttonRemove.Enabled = false;
            }
            else {
                buttonEdit.Enabled = true;
                buttonRemove.Enabled = true;
            }
        }

        // Default implementation of IPolicyListCustomizer
        public bool IsPolicyChecked(LayoutPolicyInfo policy) => policy.Apply;

        public void SetPolicyChecked(LayoutPolicyInfo policy, bool checkValue) {
            policy.Apply = checkValue;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                EventManager.Subscriptions.RemoveObjectSubscriptions(this);

                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listViewPolicies = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.buttonNew = new Button();
            this.buttonRemove = new Button();
            this.buttonEdit = new Button();
            this.SuspendLayout();
            // 
            // listViewPolicies
            // 
            this.listViewPolicies.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.listViewPolicies.CheckBoxes = true;
            this.listViewPolicies.Columns.AddRange(new ColumnHeader[] {
                                                                                               this.columnHeaderName});
            this.listViewPolicies.FullRowSelect = true;
            this.listViewPolicies.GridLines = true;
            this.listViewPolicies.HideSelection = false;
            this.listViewPolicies.Location = new System.Drawing.Point(8, 8);
            this.listViewPolicies.MultiSelect = false;
            this.listViewPolicies.Name = "listViewPolicies";
            this.listViewPolicies.Size = new System.Drawing.Size(368, 112);
            this.listViewPolicies.TabIndex = 0;
            this.listViewPolicies.View = System.Windows.Forms.View.Details;
            this.listViewPolicies.MouseUp += this.listViewPolicies_MouseUp;
            this.listViewPolicies.SelectedIndexChanged += this.UpdateButtons;
            this.listViewPolicies.ItemCheck += this.listViewPolicies_ItemCheck;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 92;
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonNew.Location = new System.Drawing.Point(8, 128);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(72, 23);
            this.buttonNew.TabIndex = 1;
            this.buttonNew.Text = "&New...";
            this.buttonNew.Click += this.buttonNew_Click;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRemove.Location = new System.Drawing.Point(168, 128);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(72, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.buttonRemove_Click;
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEdit.Location = new System.Drawing.Point(88, 128);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(72, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += this.buttonEdit_Click;
            // 
            // PolicyList
            // 
            this.Controls.Add(this.buttonNew);
            this.Controls.Add(this.listViewPolicies);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonEdit);
            this.Name = "PolicyList";
            this.Size = new System.Drawing.Size(384, 160);
            this.ResumeLayout(false);
        }
        #endregion

        private void listViewPolicies_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e) {
            if (!ViewOnly) {
                PolicyItem policyItem = (PolicyItem)listViewPolicies.Items[e.Index];

                if (e.CurrentValue == CheckState.Checked)
                    Customizer.SetPolicyChecked(policyItem.Policy, false);
                else
                    Customizer.SetPolicyChecked(policyItem.Policy, true);
            }
            else
                updateOnMouseUp = e.Index;
        }

        private void buttonEdit_Click(object sender, System.EventArgs e) {
            PolicyItem selectedItem = GetSelection();

            if (selectedItem != null) {
                Dialogs.PolicyDefinition d = new Dialogs.PolicyDefinition(selectedItem.Policy);

                editingPolicy = true;
                new SemiModalDialog(this.FindForm(), d, new SemiModalDialogClosedHandler(onCloseEditPolicy), selectedItem).ShowDialog();
            }
        }

        private void onCloseEditPolicy(Form dialog, object info) {
            if (dialog.DialogResult == DialogResult.OK) {
                PolicyItem selectedItem = (PolicyItem)info;

                Policies.Update(selectedItem.Policy);
                selectedItem.Update();
                UpdateButtons(null, null);
            }

            editingPolicy = false;
        }

        private void buttonNew_Click(object sender, System.EventArgs e) {
            XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            workingDoc.AppendChild(workingDoc.CreateElement("Policies"));

            LayoutPolicyInfo policy = new LayoutPolicyInfo(workingDoc.DocumentElement, null, null, Scope, false, false);
            Dialogs.PolicyDefinition d = new Dialogs.PolicyDefinition(policy);

            new SemiModalDialog(this.FindForm(), d, new SemiModalDialogClosedHandler(onAddDone), policy).ShowDialog();
        }

        private void onAddDone(Form dialog, object info) {
            if (dialog.DialogResult == DialogResult.OK) {
                LayoutPolicyInfo policy = (LayoutPolicyInfo)info;

                Policies.Update(policy);
            }
        }

        [LayoutEvent("policy-added")]
        protected void PolicyAdded(LayoutEvent e) {
            if (!editingPolicy) {
                LayoutPolicyInfo policy = (LayoutPolicyInfo)e.Sender;
                LayoutPoliciesCollection policies = (LayoutPoliciesCollection)e.Info;

                if (policies == this.Policies) {
                    PolicyItem policyItem = new PolicyItem(this, policy);

                    listViewPolicies.Items.Add(policyItem);
                    policyItem.Update();
                    policyItem.Selected = true;

                    UpdateButtons(null, null);
                }
            }
        }

        [LayoutEvent("policy-updated")]
        private void policyUpdated(LayoutEvent e) {
            LayoutPolicyInfo policy = (LayoutPolicyInfo)e.Sender;
            LayoutPoliciesCollection policies = (LayoutPoliciesCollection)e.Info;

            if (policies == this.Policies) {
                foreach (PolicyItem policyItem in listViewPolicies.Items)
                    if (policyItem.Policy.Id == policy.Id) {
                        policyItem.Update();
                        break;
                    }
            }
        }

        [LayoutEvent("event-script-reset", Order = 100)]
        [LayoutEvent("event-script-terminated", Order = 100)]
        [LayoutEvent("event-script-dispose", Order = 100)]
        protected virtual void updateList(LayoutEvent e) {
            foreach (PolicyItem policyItem in listViewPolicies.Items)
                policyItem.Update();

            UpdateButtons(null, null);
        }

        private void buttonRemove_Click(object sender, System.EventArgs e) {
            PolicyItem selectedItem = GetSelection();

            if (selectedItem != null)
                Policies.Remove(selectedItem.Policy);
        }

        [LayoutEvent("policy-removed")]
        protected void PolicyRemoved(LayoutEvent e) {
            if (!editingPolicy) {
                LayoutPolicyInfo policy = (LayoutPolicyInfo)e.Sender;
                LayoutPoliciesCollection policies = (LayoutPoliciesCollection)e.Info;

                if (policies == this.Policies) {
                    PolicyItem policyItem = null;

                    foreach (PolicyItem p in listViewPolicies.Items)
                        if (p.Policy.Id == policy.Id) {
                            policyItem = p;
                            break;
                        }

                    if (policyItem != null) {
                        listViewPolicies.Items.Remove(policyItem);
                        UpdateButtons(null, null);
                    }
                }
            }
        }

        private void listViewPolicies_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (updateOnMouseUp >= 0) {
                ((PolicyItem)listViewPolicies.Items[updateOnMouseUp]).Update();
                updateOnMouseUp = -1;
            }
        }

        public class PolicyItem : ListViewItem {
            private readonly PolicyList policyList;

            public PolicyItem(PolicyList policyList, LayoutPolicyInfo policy) {
                this.policyList = policyList;
                this.Policy = policy;

                if (policyList.ShowIfRunning)
                    this.SubItems.Add(" ");
                this.SubItems.Add(" ");
                Update();
            }

            public void Update() {
                Text = Policy.Name;

                if (policyList.ShowIfRunning) {
                    if (Policy.IsActive)
                        SubItems[1].Text = "Active";
                    else
                        SubItems[1].Text = "Not active";

                    SubItems[2].Text = Policy.Text;
                }
                else
                    SubItems[1].Text = Policy.Text;

                if (policyList.Customizer != null)
                    this.Checked = policyList.Customizer.IsPolicyChecked(Policy);
            }

            public LayoutPolicyInfo Policy { get; }
        }
    }
}
