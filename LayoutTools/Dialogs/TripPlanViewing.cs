using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanViewing.
    /// </summary>
    public class TripPlanViewing : Form {
        private LayoutManager.Tools.Controls.TripPlanEditor tripPlanEditor;
        private Button buttonSave;
        private Button buttonClose;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        readonly TripPlanInfo tripPlan;

        public TripPlanViewing(TrainStateInfo train, TripPlanInfo tripPlan, int trainTargetWaypointIndex) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.tripPlan = tripPlan;

            tripPlanEditor.Initialize();
            tripPlanEditor.ViewOnly = true;
            tripPlanEditor.Train = train;
            tripPlanEditor.TrainTargetWaypoint = trainTargetWaypointIndex;
            tripPlanEditor.TripPlan = tripPlan;

            Text = "Trip plan for " + train.DisplayName;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripPlanViewing));
            this.tripPlanEditor = new LayoutManager.Tools.Controls.TripPlanEditor();
            this.buttonSave = new Button();
            this.buttonClose = new Button();
            this.SuspendLayout();
            // 
            // tripPlanEditor
            // 
            this.tripPlanEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tripPlanEditor.EnablePreview = true;
            this.tripPlanEditor.Front = ((LayoutManager.Model.LayoutComponentConnectionPoint)(resources.GetObject("tripPlanEditor.Front")));
            this.tripPlanEditor.Location = new System.Drawing.Point(3, 8);
            this.tripPlanEditor.LocomotiveBlock = null;
            this.tripPlanEditor.Name = "tripPlanEditor";
            this.tripPlanEditor.Size = new System.Drawing.Size(475, 272);
            this.tripPlanEditor.TabIndex = 0;
            this.tripPlanEditor.Train = null;
            this.tripPlanEditor.TrainTargetWaypoint = -1;
            this.tripPlanEditor.TripPlan = null;
            this.tripPlanEditor.TripPlanName = null;
            this.tripPlanEditor.ViewOnly = false;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.Location = new System.Drawing.Point(3, 286);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "&Save...";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(403, 286);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // TripPlanViewing
            // 
            this.ClientSize = new System.Drawing.Size(480, 310);
            this.ControlBox = false;
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tripPlanEditor);
            this.Name = "TripPlanViewing";
            this.ShowInTaskbar = false;
            this.Text = "TripPlanViewing";
            this.Closed += new System.EventHandler(this.TripPlanViewing_Closed);
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonClose_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void TripPlanViewing_Closed(object sender, System.EventArgs e) {
            Dispose(true);
        }

        private void buttonSave_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("save-trip-plan", tripPlan, this, null));
        }
    }
}
