using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for PoliciesDefinition.
    /// </summary>
    public partial class PoliciesDefinition : Form {
        public PoliciesDefinition() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Dispatch.AddObjectInstanceDispatcherTargets(this);

            globalPolicyListGlobal.Scope = "Global";
            globalPolicyListGlobal.Policies = LayoutModel.StateManager.LayoutPolicies;

            policyListTripPlan.Scope = "TripPlan";
            policyListTripPlan.Policies = LayoutModel.StateManager.TripPlanPolicies;

            policyListBlock.Scope = "BlockInfo";
            policyListBlock.Policies = LayoutModel.StateManager.BlockInfoPolicies;
            policyListBlock.Customizer = null;

            policyListStartCondition.Scope = "RideStart";
            policyListStartCondition.Policies = LayoutModel.StateManager.RideStartPolicies;
            policyListStartCondition.Customizer = null;

            policyListDriverInstructions.Scope = "DriverInstructions";
            policyListDriverInstructions.Policies = LayoutModel.StateManager.DriverInstructionsPolicies;
            policyListDriverInstructions.Customizer = null;

            globalPolicyListGlobal.Initialize();
            policyListTripPlan.Initialize();
            policyListBlock.Initialize();
            policyListStartCondition.Initialize();
            policyListDriverInstructions.Initialize();
        }

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

        [DispatchTarget]
        private Form? QueryPoliciesDefinitionDialog() => this;

        private void PoliciesDefinition_Closed(object? sender, EventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
            policyListTripPlan.Dispose();
        }

        private void PoliciesDefinition_Closing(object? sender, CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonExport_Click(object? sender, EventArgs e) {
            ExportPolicies d = new();

            d.ShowDialog(this);
        }

        private void ButtonImport_Click(object? sender, EventArgs e) {
            ImportPolicies d = new();

            d.ShowDialog(this);
        }
    }
}
