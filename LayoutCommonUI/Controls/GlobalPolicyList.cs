namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for GlobalPolicyList.
    /// </summary>
    public partial class GlobalPolicyList : PolicyList {

        public GlobalPolicyList() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ShowIfRunning = true;
        }

        protected override void UpdateButtons(object? sender, EventArgs e) {
            PolicyItem? selected = GetSelection();

            if (selected == null || !LayoutController.IsOperationMode) {
                buttonStartStop.Text = "Activate";
                buttonStartStop.Enabled = false;
            }
            else {
                if (selected.Policy.IsActive)
                    buttonStartStop.Text = "Deactivate";
                else
                    buttonStartStop.Text = "Activate";

                buttonStartStop.Enabled = true;
            }
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


        private void ButtonStartStop_Click(object? sender, EventArgs e) {
            var selected = GetSelection();

            if (selected != null) {
                var runningScript = (LayoutEventScript?)EventManager.Event(new LayoutEvent("get-active-event-script", selected.Policy.Id));

                if (runningScript != null)
                    runningScript.Dispose();
                else {
                    runningScript = EventManager.EventScript("Layout policy " + selected.Policy.Name, selected.Policy.EventScriptElement, new Guid[] { }, null);
                    runningScript.Id = selected.Policy.Id;

                    runningScript.Reset();
                }

                selected.Update();
                UpdateButtons(null, EventArgs.Empty);
            }
        }
    }
}
