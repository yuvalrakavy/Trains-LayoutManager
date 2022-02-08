using System.Windows.Forms;
using System.Xml;
using MethodDispatcher;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanCatalog.
    /// </summary>
    public partial class TripPlanCatalog : Form {

        private TrainStateInfo train;
        private readonly XmlDocument workingDoc;
        const string E_ApplicableTripPlans = "ApplicableTripPlans";

        public TripPlanCatalog(TrainStateInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Owner = LayoutController.ActiveFrameWindow as Form;

            tripPlanViewer.ViewOnly = true;

            workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            XmlElement rootElement = workingDoc.CreateElement("WorkingDocument");
            XmlElement applicableTripPlansElement = workingDoc.CreateElement(E_ApplicableTripPlans);

            workingDoc.AppendChild(rootElement);
            rootElement.AppendChild(applicableTripPlansElement);

            tripPlanViewer.Initialize();
            tripPlanList.Initialize();

            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);

            this.train = train;
            SetTrain(train);
        }

        private void SetTrain(TrainStateInfo train) {
            var applicableTripPlansElement = Ensure.NotNull<XmlElement>(workingDoc.DocumentElement![E_ApplicableTripPlans]);

            applicableTripPlansElement.RemoveAll();

            this.train = train;
            tripPlanViewer.Train = train;
            tripPlanViewer.OptionalTripPlan = null;

            Text = "Trip-plans for " + train.DisplayName;

            trainDriverComboBox.Train = train;

            EventManager.Event(new LayoutEvent("get-applicable-trip-plans-request", train, applicableTripPlansElement).SetOption("CalculatePenalty", false));

            tripPlanList.ApplicableTripPlansElement = applicableTripPlansElement;
            UpdateButtons();
        }

        [LayoutEvent("show-saved-trip-plans-for-train")]
        private void ShowSavedTripPlansForTrain(LayoutEvent e) {
            var aTrain = Ensure.NotNull<TrainStateInfo>(e.Sender);

            SetTrain(aTrain);
            Activate();
            e.Info = true;
        }

        private void UpdateButtons() {
            if (tripPlanList.SelectedTripPlan != null) {
                buttonEdit.Enabled = true;
                buttonGo.Enabled = true;        // TODO: Chnage to tripPlanViewer.IsTripPlanValid
            }
            else {
                buttonEdit.Enabled = false;
                buttonGo.Enabled = false;       // TODO: Chnage to tripPlanViewer.IsTripPlanValid
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
            }
            base.Dispose(disposing);
        }

        [LayoutEvent("train-is-removed")]
        private void TrainRemoved(LayoutEvent e) {
            var removedTrain = Ensure.NotNull<TrainStateInfo>(e.Sender);

            if (train != null && removedTrain.Id == train.Id)
                Close();
        }

        [LayoutEvent("exit-operation-mode")]
        private void ExitOperationMode(LayoutEvent e) {
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TripPlanCatalog_Closed(object? sender, System.EventArgs e) {
            tripPlanViewer.Dispose();
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonEdit_Click(object? sender, System.EventArgs e) {
            if (tripPlanList.SelectedTripPlan != null) {
                if (!trainDriverComboBox.ValidateInput())
                    return;

                trainDriverComboBox.Commit();

                XmlDocument tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                tripPlanDoc.AppendChild(tripPlanDoc.ImportNode(tripPlanList.SelectedTripPlan.Element, true));

                TripPlanInfo tripPlan = new(tripPlanDoc.DocumentElement);

                if (tripPlanList.ShouldReverseSelectedTripPlan)
                    tripPlan.Reverse();

                Dialogs.TripPlanEditor tripPlanEditor = new(tripPlan, train);

                tripPlan.FromCatalog = true;
                tripPlanEditor.Show();

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ButtonGo_Click(object? sender, System.EventArgs e) {
            if (tripPlanList.SelectedTripPlan != null) {
                if (!trainDriverComboBox.ValidateInput())
                    return;

                trainDriverComboBox.Commit();

                XmlDocument tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                tripPlanDoc.AppendChild(tripPlanDoc.ImportNode(tripPlanList.SelectedTripPlan.Element, true));

                TripPlanInfo tripPlan = new(tripPlanDoc.DocumentElement);

                if (tripPlanList.ShouldReverseSelectedTripPlan)
                    tripPlan.Reverse();

                TripPlanAssignmentInfo tripAssignment = new(tripPlan, train);

                EventManager.Event(new LayoutEvent("execute-trip-plan", tripAssignment));
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ListViewTripPlans_DoubleClick(object? sender, System.EventArgs e) {
            buttonGo.PerformClick();
        }

        private void TripPlanCatalog_Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }

        private void TripPlanList_SelectedTripPlanChanged(object? sender, System.EventArgs e) {
            if (tripPlanList.SelectedTripPlan != null) {
                TripPlanInfo tripPlan = tripPlanList.SelectedTripPlan;

                if (tripPlanList.ShouldReverseSelectedTripPlan) {
                    XmlDocument tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                    tripPlanDoc.AppendChild(tripPlanDoc.ImportNode(tripPlanList.SelectedTripPlan.Element, true));

                    tripPlan = new TripPlanInfo(tripPlanDoc.DocumentElement);
                    tripPlan.Reverse();
                }

                tripPlanViewer.TripPlanName = tripPlan.Name;
                tripPlanViewer.TripPlan = tripPlan;
            }
            else
                tripPlanViewer.OptionalTripPlan = null;

            UpdateButtons();
        }

        private void TripPlanCatalog_FormClosing(object? sender, FormClosingEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }
    }
}
