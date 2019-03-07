using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanEditor.
    /// </summary>
    public class TripPlanEditor : Form {
        private Button buttonSaveTripPlan;
        private Button buttonGo;
        private Button buttonCancel;
        private LayoutManager.Tools.Controls.TripPlanEditor tripPlanEditor1;
        private LayoutManager.CommonUI.Controls.TrainDriverComboBox trainDriverComboBox;
        private Label label1;
        private readonly IContainer components = null;

        private void endOfDesignerVariables() { }

        readonly TrainStateInfo train;

        public TripPlanEditor(TripPlanInfo tripPlan, TrainStateInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;

            tripPlanEditor1.Initialize();
            tripPlanEditor1.SetDialogName("Trip plan for " + train.DisplayName);
            tripPlanEditor1.TripPlanName = train.Name;
            tripPlanEditor1.Train = train;
            tripPlanEditor1.TripPlan = tripPlan;

            trainDriverComboBox.Train = train;

            this.Text = tripPlanEditor1.DialogName;

            UpdateButtons(null, null);

            EventManager.AddObjectSubscriptions(this);
            this.Owner = LayoutController.ActiveFrameWindow as Form;
        }

        protected void UpdateButtons(object sender, EventArgs e) {
            if (tripPlanEditor1.Count > 0) {
                buttonGo.Enabled = true;
                buttonSaveTripPlan.Enabled = true;
            }
            else {
                buttonGo.Enabled = false;
                buttonSaveTripPlan.Enabled = false;
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

        [LayoutEvent("train-is-removed")]
        private void trainRemoved(LayoutEvent e) {
            if (((TrainStateInfo)e.Sender).Id == train.Id)
                Close();
        }

        [LayoutEvent("query-edit-trip-plan-for-train")]
        private void queryEditTripPlanForTrain(LayoutEvent e) {
            if (((TrainStateInfo)e.Sender).Id == train.Id)
                e.Info = tripPlanEditor1.ActiveForm;
        }

        [LayoutEvent("exit-operation-mode")]
        private void exitOperationMode(LayoutEvent e) {
            Close();
        }

        public void SelectWayPoint(int wayPointIndex) {
            tripPlanEditor1.SelectWayPoint(wayPointIndex);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonSaveTripPlan = new Button();
            this.buttonGo = new Button();
            this.buttonCancel = new Button();
            this.tripPlanEditor1 = new LayoutManager.Tools.Controls.TripPlanEditor();
            this.trainDriverComboBox = new LayoutManager.CommonUI.Controls.TrainDriverComboBox();
            this.label1 = new Label();
            this.SuspendLayout();
            // 
            // buttonSaveTripPlan
            // 
            this.buttonSaveTripPlan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSaveTripPlan.Location = new System.Drawing.Point(8, 222);
            this.buttonSaveTripPlan.Name = "buttonSaveTripPlan";
            this.buttonSaveTripPlan.Size = new System.Drawing.Size(56, 20);
            this.buttonSaveTripPlan.TabIndex = 3;
            this.buttonSaveTripPlan.Text = "&Save";
            this.buttonSaveTripPlan.Click += new System.EventHandler(this.buttonSaveTripPlan_Click);
            // 
            // buttonGo
            // 
            this.buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGo.Location = new System.Drawing.Point(386, 222);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(32, 20);
            this.buttonGo.TabIndex = 4;
            this.buttonGo.Text = "&Go";
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(424, 222);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(48, 20);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tripPlanEditor1
            // 
            this.tripPlanEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tripPlanEditor1.EnablePreview = true;
            this.tripPlanEditor1.Location = new System.Drawing.Point(0, 0);
            this.tripPlanEditor1.LocomotiveBlock = null;
            this.tripPlanEditor1.Name = "tripPlanEditor1";
            this.tripPlanEditor1.Size = new System.Drawing.Size(480, 217);
            this.tripPlanEditor1.TabIndex = 6;
            this.tripPlanEditor1.Train = null;
            this.tripPlanEditor1.TrainTargetWaypoint = -1;
            this.tripPlanEditor1.TripPlan = null;
            this.tripPlanEditor1.TripPlanName = null;
            this.tripPlanEditor1.ViewOnly = false;
            this.tripPlanEditor1.WayPointCountChanged += new System.EventHandler(this.UpdateButtons);
            // 
            // trainDriverComboBox
            // 
            this.trainDriverComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trainDriverComboBox.Location = new System.Drawing.Point(153, 222);
            this.trainDriverComboBox.Name = "trainDriverComboBox";
            this.trainDriverComboBox.Size = new System.Drawing.Size(215, 20);
            this.trainDriverComboBox.TabIndex = 7;
            this.trainDriverComboBox.Train = null;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(73, 222);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Train driven:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TripPlanEditor
            // 
            this.AcceptButton = this.buttonGo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(480, 246);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trainDriverComboBox);
            this.Controls.Add(this.tripPlanEditor1);
            this.Controls.Add(this.buttonSaveTripPlan);
            this.Controls.Add(this.buttonGo);
            this.Controls.Add(this.buttonCancel);
            this.MinimumSize = new System.Drawing.Size(296, 184);
            this.Name = "TripPlanEditor";
            this.Text = "TripPlanEditor";
            this.Closed += new System.EventHandler(this.TripPlanEditor_Closed);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.TripPlanEditor_Closing);
            this.ResumeLayout(false);

        }
        #endregion

        private void TripPlanEditor_Closed(object sender, System.EventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            this.Close();
        }

        private void buttonGo_Click(object sender, System.EventArgs e) {
            TripPlanInfo tripPlan = tripPlanEditor1.TripPlan;

            if (!tripPlanEditor1.Check())
                return;

            if (!trainDriverComboBox.ValidateInput())
                return;

            ITripRouteValidationResult routeValidationResult = (ITripRouteValidationResult)EventManager.Event(
                new LayoutEvent(tripPlan, "validate-trip-plan-route", null, train));

            if (routeValidationResult.Actions.Count > 0) {
                Dialogs.TripPlanRouteValidationResult d = new Dialogs.TripPlanRouteValidationResult(tripPlan, routeValidationResult, this);

                if (d.ShowDialog(this) == DialogResult.OK) {
                    foreach (ITripRouteValidationAction action in routeValidationResult.Actions)
                        action.Apply(tripPlan);
                    tripPlanEditor1.UpdateAll();
                }

                return;
            }

            trainDriverComboBox.Commit();

            try {
                TripPlanAssignmentInfo tripPlanAssignment = new TripPlanAssignmentInfo(tripPlan, train);

                if ((bool)EventManager.Event(new LayoutEvent(tripPlanAssignment, "execute-trip-plan", null, this)))
                    Close();
            }
            catch (LayoutException ex) {
                ex.Report();
            }
        }

        private void buttonSaveTripPlan_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent(tripPlanEditor1.TripPlan, "save-trip-plan"));
        }

        private void TripPlanEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            tripPlanEditor1.Dispose();
            if (Owner != null)
                Owner.Activate();
        }
    }
}
