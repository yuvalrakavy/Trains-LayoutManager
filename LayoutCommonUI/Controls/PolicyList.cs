using System.Xml;
using LayoutManager.Model;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Controls {
    public interface IPolicyListCustomizer {
        bool IsPolicyChecked(LayoutPolicyInfo policy);

        void SetPolicyChecked(LayoutPolicyInfo policy, bool checkValue);
    }

    /// <summary>
    /// Summary description for PolicyList.
    /// </summary>
    public partial class PolicyList : UserControl, IPolicyListCustomizer, IControlSupportViewOnly {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        private readonly ListViewStringColumnsSorter sorter;
        private int updateOnMouseUp = -1;
        private bool editingPolicy = false;

        public PolicyList() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            sorter = new ListViewStringColumnsSorter(listViewPolicies);
            Customizer = this;
        }

        public string Scope { get; set; } = "TripPlan";

        public LayoutPoliciesCollection Policies { get => Ensure.NotNull<LayoutPoliciesCollection>(OptionalPolicies); set => OptionalPolicies = value; }

        public LayoutPoliciesCollection? OptionalPolicies { get; set; }

        public IPolicyListCustomizer? Customizer { get; set; }

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

            UpdateButtons(null, EventArgs.Empty);
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        protected PolicyItem? GetSelection() {
            return listViewPolicies.SelectedItems.Count == 0 ? null : (PolicyItem)listViewPolicies.SelectedItems[0];
        }

        protected virtual void UpdateButtons(object? sender, EventArgs e) {
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

        private void ListViewPolicies_ItemCheck(object? sender, ItemCheckEventArgs e) {
            if (!ViewOnly) {
                PolicyItem policyItem = (PolicyItem)listViewPolicies.Items[e.Index];

                if (e.CurrentValue == CheckState.Checked)
                    Customizer?.SetPolicyChecked(policyItem.Policy, false);
                else
                    Customizer?.SetPolicyChecked(policyItem.Policy, true);
            }
            else
                updateOnMouseUp = e.Index;
        }

        private void ButtonEdit_Click(object? sender, EventArgs e) {
            var selectedItem = GetSelection();

            if (selectedItem != null) {
                Dialogs.PolicyDefinition d = new(selectedItem.Policy);

                editingPolicy = true;
                new SemiModalDialog(this.FindForm(), d, new SemiModalDialogClosedHandler(OnCloseEditPolicy), selectedItem).ShowDialog();
            }
        }

        private void OnCloseEditPolicy(Form dialog, object? info) {
            if (dialog.DialogResult == DialogResult.OK) {
                var selectedItem = Ensure.NotNull<PolicyItem>(info);

                Policies.Update(selectedItem.Policy);
                selectedItem.Update();
                UpdateButtons(null, EventArgs.Empty);
            }

            editingPolicy = false;
        }

        private void ButtonNew_Click(object? sender, EventArgs e) {
            XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            workingDoc.AppendChild(workingDoc.CreateElement("Policies"));

            LayoutPolicyInfo policy = new(workingDoc.DocumentElement!, null, null, Scope, false, false);
            Dialogs.PolicyDefinition d = new(policy);

            new SemiModalDialog(this.FindForm(), d, new SemiModalDialogClosedHandler(OnAddDone), policy).ShowDialog();
        }

        private void OnAddDone(Form dialog, object? info) {
            if (dialog.DialogResult == DialogResult.OK) {
                var policy = Ensure.NotNull<LayoutPolicyInfo>(info);

                Policies.Update(policy);
            }
        }

        [LayoutEvent("policy-added")]
        protected void PolicyAdded(LayoutEvent e) {
            if (!editingPolicy) {
                var policy = Ensure.NotNull<LayoutPolicyInfo>(e.Sender);
                var policies = Ensure.NotNull<LayoutPoliciesCollection>(e.Info);

                if (policies == this.Policies) {
                    PolicyItem policyItem = new(this, policy);

                    listViewPolicies.Items.Add(policyItem);
                    policyItem.Update();
                    policyItem.Selected = true;

                    UpdateButtons(null, EventArgs.Empty);
                }
            }
        }

        [LayoutEvent("policy-updated")]
        private void PolicyUpdated(LayoutEvent e) {
            var policy = Ensure.NotNull<LayoutPolicyInfo>(e.Sender);
            var policies = Ensure.NotNull<LayoutPoliciesCollection>(e.Info);

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
        protected virtual void UpdateList(LayoutEvent e) {
            foreach (PolicyItem policyItem in listViewPolicies.Items)
                policyItem.Update();

            UpdateButtons(null, EventArgs.Empty);
        }

        private void ButtonRemove_Click(object? sender, EventArgs e) {
            var selectedItem = GetSelection();

            if (selectedItem != null)
                Policies.Remove(selectedItem.Policy);
        }

        [LayoutEvent("policy-removed")]
        protected void PolicyRemoved(LayoutEvent e) {
            if (!editingPolicy) {
                var policy = Ensure.NotNull<LayoutPolicyInfo>(e.Sender);
                var policies = Ensure.NotNull<LayoutPoliciesCollection>(e.Info);

                if (policies == this.Policies) {
                    PolicyItem? policyItem = null;

                    foreach (PolicyItem p in listViewPolicies.Items)
                        if (p.Policy.Id == policy.Id) {
                            policyItem = p;
                            break;
                        }

                    if (policyItem != null) {
                        listViewPolicies.Items.Remove(policyItem);
                        UpdateButtons(null, EventArgs.Empty);
                    }
                }
            }
        }

        private void ListViewPolicies_MouseUp(object? sender, MouseEventArgs e) {
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
