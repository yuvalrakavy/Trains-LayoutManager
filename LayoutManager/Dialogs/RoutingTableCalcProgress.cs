using System.ComponentModel;
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

        [LayoutEvent("routing-table-generation-progress")]
        private void RoutingTableGenerationProgress(LayoutEvent e) {
            progressBar.PerformStep();
        }

        [LayoutEvent("routing-table-generation-done")]
        private void RoutingTableGenerationDone(LayoutEvent e) {
            Close();
        }

        private void ButtonAbort_Click(object? sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("abort-routing-table-generation", this));
        }
    }
}
