using MethodDispatcher;
using System.Windows.Forms;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for RoutingTableCalcProgress.
    /// </summary>
    public partial class RoutingTableCalcProgress : Form {
        public RoutingTableCalcProgress(int nTurnouts) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);

            progressBar.Maximum = nTurnouts;
            progressBar.Step = 1;
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

        private void RoutingTableCalcProgress_Closed(object? sender, System.EventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        [DispatchTarget]
        private void RoutingTableGenerationProgress() {
            progressBar.PerformStep();
        }

        [DispatchTarget]
        private void RoutingTableGenerationDone() {
            Close();
        }

        private void ButtonAbort_Click(object? sender, System.EventArgs e) {
            Dispatch.Call.AbortRoutingTableGenerationDialog();
        }
    }
}
