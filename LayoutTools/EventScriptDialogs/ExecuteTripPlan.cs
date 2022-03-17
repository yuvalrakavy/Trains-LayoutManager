using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for ExecuteTripPlan.
    /// </summary>
    public partial class ExecuteTripPlan : Form, IModelComponentReceiverDialog {
        private readonly XmlElement element;

        private LayoutBlock? locomotiveBlock = null;
        private LayoutComponentConnectionPoint locomotiveFront = LayoutComponentConnectionPoint.Empty;

        public ExecuteTripPlan(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            tripPlanList.Initialize();

            tripPlanEditor.Initialize();
            tripPlanEditor.EnablePreview = false;

            EventManager.AddObjectSubscriptions(this);

            TripPlanInfo? selectedTripPlan = null;

            if (element.HasAttribute(A_TripPlanID)) {
                Guid tripPlanID = (Guid)element.AttributeValue(A_TripPlanID);

                selectedTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanID];
            }

            if (element.HasAttribute(A_TrainSymbol))
                comboBoxTrainSymbol.Text = element.GetAttribute(A_TrainSymbol);

            if (element.HasAttribute(A_LocomotiveBlockId)) {
                Guid locomotiveBlockID = (Guid)element.AttributeValue(A_LocomotiveBlockId);

                locomotiveBlock = LayoutModel.Blocks[locomotiveBlockID];
                locomotiveFront = element.AttributeValue(A_LocomotiveFront).Enum<LayoutComponentConnectionPoint>() ?? locomotiveBlock.BlockDefinintion.Track.ConnectionPoints[0];
            }

            FillList(locomotiveBlock, locomotiveFront);
            tripPlanList.SelectedTripPlan = selectedTripPlan;

            if (locomotiveBlock != null) {
                tripPlanEditor.LocomotiveBlock = locomotiveBlock;
                tripPlanEditor.Front = locomotiveFront;
                tripPlanEditor.EnablePreview = true;
            }

            if (selectedTripPlan != null)
                tripPlanEditor.TripPlan = selectedTripPlan;
        }

        private void FillList(LayoutBlock? block, LayoutComponentConnectionPoint front) {
            XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement applicableTripPlansElement = workingDoc.CreateElement("ApplicableTripPlans");

            workingDoc.AppendChild(applicableTripPlansElement);

            var applicableTripPlansData = Dispatch.Call.GetApplicableTripPlansRequest(block, true, front);

            if (applicableTripPlansData.LocomtiveBlockId != Guid.Empty) {
                locomotiveBlock = LayoutModel.Blocks[applicableTripPlansData.LocomtiveBlockId];
                locomotiveFront = applicableTripPlansData.LocomotiveFront;
            }

            tripPlanList.TripPlans = applicableTripPlansData.TripPlans;
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
        private void QueryExecuteTripPlanDialog(LayoutBlockDefinitionComponent blockDefinition, List<IModelComponentReceiverDialog> dialogs) {
            dialogs.Add(this);
        }

        private void ExecuteTripPlan_Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            var tripPlan = tripPlanList.SelectedTripPlan;

            if (tripPlan == null) {
                MessageBox.Show(this, "You did not select any trip-plan", "No Trip-plan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tripPlanList.Focus();
                return;
            }

            element.SetAttributeValue(A_TripPlanID, tripPlan.Id);
            element.SetAttributeValue(A_ShouldReverse, tripPlanList.ShouldReverseSelectedTripPlan);
            element.SetAttribute(A_TrainSymbol, comboBoxTrainSymbol.Text);

            if (locomotiveBlock != null) {
                element.SetAttributeValue(A_LocomotiveBlockId, locomotiveBlock.Id);
                element.SetAttributeValue(A_LocomotiveFront, locomotiveFront);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TripPlanList_SelectedTripPlanChanged(object? sender, System.EventArgs e) {
            tripPlanEditor.LocomotiveBlock = locomotiveBlock;
            tripPlanEditor.Front = locomotiveFront;

            var tripPlan = tripPlanList.SelectedTripPlan;

            if (tripPlan != null) {
                if (tripPlanList.ShouldReverseSelectedTripPlan) {
                    XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                    workingDoc.AppendChild((XmlElement)workingDoc.ImportNode(tripPlan.Element, true));
                    tripPlan = new TripPlanInfo(workingDoc.DocumentElement!);
                    tripPlan.Reverse();
                }

                tripPlanEditor.TripPlan = tripPlan;
            }

            tripPlanEditor.EnablePreview = locomotiveBlock != null;
        }

        #region IModelComponentReceiverComponent Members

        public string DialogName(IModelComponent component) => Text;

        public void AddComponent(IModelComponent component) {
            LayoutBlockDefinitionComponent blockInfo = (LayoutBlockDefinitionComponent)component;

            locomotiveBlock = blockInfo.Block;
            locomotiveFront = LayoutComponentConnectionPoint.Empty;
            FillList(locomotiveBlock, locomotiveFront);
        }

        #endregion

        private void TripPlanList_Load(object? sender, System.EventArgs e) {
        }
    }
}
