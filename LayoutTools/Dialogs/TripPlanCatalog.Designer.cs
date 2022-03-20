using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanCatalog.
    /// </summary>
    partial class TripPlanCatalog : Form {
        private Panel panelTripPlans;
        private Splitter splitter;
        private GroupBox groupBoxTripPlanPreview;
        private LayoutManager.Tools.Controls.TripPlanEditor tripPlanViewer;
        private Button buttonCancel;
        private Button buttonGo;
        private Button buttonEdit;
        private LayoutManager.CommonUI.Controls.TrainDriverComboBox trainDriverComboBox;
        private Label label1;
        private LayoutManager.CommonUI.Controls.TripPlanList tripPlanList;
        private Panel panelTripPlanPreview;
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripPlanCatalog));
            this.panelTripPlans = new Panel();
            this.tripPlanList = new LayoutManager.CommonUI.Controls.TripPlanList();
            this.splitter = new Splitter();
            this.panelTripPlanPreview = new Panel();
            this.label1 = new Label();
            this.trainDriverComboBox = new LayoutManager.CommonUI.Controls.TrainDriverComboBox();
            this.buttonCancel = new Button();
            this.groupBoxTripPlanPreview = new GroupBox();
            this.tripPlanViewer = new LayoutManager.Tools.Controls.TripPlanEditor();
            this.buttonGo = new Button();
            this.buttonEdit = new Button();
            this.panelTripPlans.SuspendLayout();
            this.panelTripPlanPreview.SuspendLayout();
            this.groupBoxTripPlanPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTripPlans
            // 
            this.panelTripPlans.Controls.Add(this.tripPlanList);
            this.panelTripPlans.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTripPlans.Location = new System.Drawing.Point(0, 0);
            this.panelTripPlans.Name = "panelTripPlans";
            this.panelTripPlans.Size = new System.Drawing.Size(504, 192);
            this.panelTripPlans.TabIndex = 0;
            // 
            // tripPlanList
            // 
            this.tripPlanList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tripPlanList.Location = new System.Drawing.Point(0, 0);
            this.tripPlanList.Name = "tripPlanList";
            this.tripPlanList.SelectedTripPlan = null;
            this.tripPlanList.Size = new System.Drawing.Size(504, 192);
            this.tripPlanList.TabIndex = 0;
            this.tripPlanList.SelectedTripPlanChanged += new EventHandler(this.TripPlanList_SelectedTripPlanChanged);
            // 
            // splitter
            // 
            this.splitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter.Location = new System.Drawing.Point(0, 192);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(504, 4);
            this.splitter.TabIndex = 1;
            this.splitter.TabStop = false;
            // 
            // panelTripPlanPreview
            // 
            this.panelTripPlanPreview.Controls.Add(this.label1);
            this.panelTripPlanPreview.Controls.Add(this.trainDriverComboBox);
            this.panelTripPlanPreview.Controls.Add(this.buttonCancel);
            this.panelTripPlanPreview.Controls.Add(this.groupBoxTripPlanPreview);
            this.panelTripPlanPreview.Controls.Add(this.buttonGo);
            this.panelTripPlanPreview.Controls.Add(this.buttonEdit);
            this.panelTripPlanPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTripPlanPreview.Location = new System.Drawing.Point(0, 196);
            this.panelTripPlanPreview.Name = "panelTripPlanPreview";
            this.panelTripPlanPreview.Size = new System.Drawing.Size(504, 266);
            this.panelTripPlanPreview.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.label1.Location = new System.Drawing.Point(8, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Train driven:";
            // 
            // trainDriverComboBox
            // 
            this.trainDriverComboBox.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.trainDriverComboBox.Location = new System.Drawing.Point(80, 239);
            this.trainDriverComboBox.Name = "trainDriverComboBox";
            this.trainDriverComboBox.Size = new System.Drawing.Size(192, 24);
            this.trainDriverComboBox.TabIndex = 1;
            this.trainDriverComboBox.Train = null;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(432, 240);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 22);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // groupBoxTripPlanPreview
            // 
            this.groupBoxTripPlanPreview.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.groupBoxTripPlanPreview.Controls.Add(this.tripPlanViewer);
            this.groupBoxTripPlanPreview.Location = new System.Drawing.Point(4, 8);
            this.groupBoxTripPlanPreview.Name = "groupBoxTripPlanPreview";
            this.groupBoxTripPlanPreview.Size = new System.Drawing.Size(494, 223);
            this.groupBoxTripPlanPreview.TabIndex = 0;
            this.groupBoxTripPlanPreview.TabStop = false;
            this.groupBoxTripPlanPreview.Text = "Selected Trip-plan:";
            // 
            // tripPlanViewer
            // 
            this.tripPlanViewer.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.tripPlanViewer.EnablePreview = true;
            this.tripPlanViewer.Front = (LayoutManager.Model.LayoutComponentConnectionPoint)resources.GetObject("tripPlanViewer.Front");
            this.tripPlanViewer.Location = new System.Drawing.Point(8, 16);
            this.tripPlanViewer.LocomotiveBlock = null;
            this.tripPlanViewer.Name = "tripPlanViewer";
            this.tripPlanViewer.Size = new System.Drawing.Size(480, 199);
            this.tripPlanViewer.TabIndex = 0;
            this.tripPlanViewer.Train = null;
            this.tripPlanViewer.TrainTargetWaypoint = -1;
            this.tripPlanViewer.TripPlan = null;
            this.tripPlanViewer.TripPlanName = null;
            this.tripPlanViewer.ViewOnly = true;
            // 
            // buttonGo
            // 
            this.buttonGo.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonGo.Location = new System.Drawing.Point(360, 240);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(64, 22);
            this.buttonGo.TabIndex = 3;
            this.buttonGo.Text = "Go!";
            this.buttonGo.Click += new EventHandler(this.ButtonGo_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonEdit.Location = new System.Drawing.Point(288, 240);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(64, 22);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "Edit..";
            this.buttonEdit.Click += new EventHandler(this.ButtonEdit_Click);
            // 
            // TripPlanCatalog
            // 
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.AcceptButton = this.buttonGo;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(504, 462);
            this.Controls.Add(this.panelTripPlanPreview);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.panelTripPlans);
            this.Name = "TripPlanCatalog";
            this.Text = "Saved Trip-plans";
            this.Closed += new EventHandler(this.TripPlanCatalog_Closed);
            this.Closing += new CancelEventHandler(this.TripPlanCatalog_Closing);
            this.FormClosing += new FormClosingEventHandler(this.TripPlanCatalog_FormClosing);
            this.panelTripPlans.ResumeLayout(false);
            this.panelTripPlanPreview.ResumeLayout(false);
            this.groupBoxTripPlanPreview.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

    }
}

