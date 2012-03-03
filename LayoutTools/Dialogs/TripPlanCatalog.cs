using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TripPlanCatalog.
	/// </summary>
	public class TripPlanCatalog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panelTripPlans;
		private System.Windows.Forms.Splitter splitter;
		private System.Windows.Forms.GroupBox groupBoxTripPlanPreview;
		private LayoutManager.Tools.Controls.TripPlanEditor tripPlanViewer;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonGo;
		private System.Windows.Forms.Button buttonEdit;
		private LayoutManager.CommonUI.Controls.TrainDriverComboBox trainDriverComboBox;
		private System.Windows.Forms.Label label1;
		private LayoutManager.CommonUI.Controls.TripPlanList tripPlanList;
		private System.Windows.Forms.Panel panelTripPlanPreview;

		private void endOfDesignerVariables() { }

		TrainStateInfo			train;
		XmlDocument				workingDoc;
		TripPlanIconListInfo	tripPlanIconList = null;

		public TripPlanCatalog(TrainStateInfo train)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			Owner = LayoutController.ActiveFrameWindow as Form;
			
			tripPlanViewer.ViewOnly = true;

			tripPlanIconList = LayoutModel.Instance.TripPlanIconList;

			workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

			XmlElement	rootElement = workingDoc.CreateElement("WorkingDocument");
			XmlElement	applicableTripPlansElement = workingDoc.CreateElement("ApplicableTripPlans");

			workingDoc.AppendChild(rootElement);
			rootElement.AppendChild(applicableTripPlansElement);

			tripPlanViewer.Initialize();
			tripPlanList.Initialize();

			EventManager.AddObjectSubscriptions(this);
			setTrain(train);
		}

		private void setTrain(TrainStateInfo train) {
			XmlElement	applicableTripPlansElement = workingDoc.DocumentElement["ApplicableTripPlans"];

			applicableTripPlansElement.RemoveAll();

			this.train = train;
			tripPlanViewer.Train = train;
			tripPlanViewer.TripPlan = null;

			Text = "Trip-plans for " + train.DisplayName;

			trainDriverComboBox.Train = train;

			EventManager.Event(new LayoutEvent(train, "get-applicable-trip-plans-request", null, applicableTripPlansElement).SetOption("CalculatePenalty", false));

			tripPlanList.ApplicableTripPlansElement = applicableTripPlansElement;
			updateButtons();
		}

		[LayoutEvent("show-saved-trip-plans-for-train")]
		private void showSavedTripPlansForTrain(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;

			setTrain(train);
			Activate();
			e.Info = true;
		}

		private void updateButtons() {
			if(tripPlanList.SelectedTripPlan != null) {
				buttonEdit.Enabled = true;
				buttonGo.Enabled = true;		// TODO: Chnage to tripPlanViewer.IsTripPlanValid
			}
			else {
				buttonEdit.Enabled = false;
				buttonGo.Enabled = false;		// TODO: Chnage to tripPlanViewer.IsTripPlanValid
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
			}
			base.Dispose( disposing );
		}

		[LayoutEvent("train-is-removed")]
		private void trainRemoved(LayoutEvent e) {
			if(((TrainStateInfo)e.Sender).Id == train.Id)
				Close();
		}

		[LayoutEvent("exit-operation-mode")]
		private void exitOperationMode(LayoutEvent e) {
			Close();
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripPlanCatalog));
			this.panelTripPlans = new System.Windows.Forms.Panel();
			this.tripPlanList = new LayoutManager.CommonUI.Controls.TripPlanList();
			this.splitter = new System.Windows.Forms.Splitter();
			this.panelTripPlanPreview = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.trainDriverComboBox = new LayoutManager.CommonUI.Controls.TrainDriverComboBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxTripPlanPreview = new System.Windows.Forms.GroupBox();
			this.tripPlanViewer = new LayoutManager.Tools.Controls.TripPlanEditor();
			this.buttonGo = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
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
			this.tripPlanList.ApplicableTripPlansElement = null;
			this.tripPlanList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tripPlanList.Location = new System.Drawing.Point(0, 0);
			this.tripPlanList.Name = "tripPlanList";
			this.tripPlanList.SelectedTripPlan = null;
			this.tripPlanList.Size = new System.Drawing.Size(504, 192);
			this.tripPlanList.TabIndex = 0;
			this.tripPlanList.SelectedTripPlanChanged += new System.EventHandler(this.tripPlanList_SelectedTripPlanChanged);
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
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(8, 244);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 14);
			this.label1.TabIndex = 5;
			this.label1.Text = "Train driven:";
			// 
			// trainDriverComboBox
			// 
			this.trainDriverComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.trainDriverComboBox.Location = new System.Drawing.Point(80, 239);
			this.trainDriverComboBox.Name = "trainDriverComboBox";
			this.trainDriverComboBox.Size = new System.Drawing.Size(192, 24);
			this.trainDriverComboBox.TabIndex = 1;
			this.trainDriverComboBox.Train = null;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(432, 240);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(64, 22);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// groupBoxTripPlanPreview
			// 
			this.groupBoxTripPlanPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
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
			this.tripPlanViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tripPlanViewer.EnablePreview = true;
			this.tripPlanViewer.Front = ((LayoutManager.Model.LayoutComponentConnectionPoint)(resources.GetObject("tripPlanViewer.Front")));
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
			this.buttonGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonGo.Location = new System.Drawing.Point(360, 240);
			this.buttonGo.Name = "buttonGo";
			this.buttonGo.Size = new System.Drawing.Size(64, 22);
			this.buttonGo.TabIndex = 3;
			this.buttonGo.Text = "Go!";
			this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEdit.Location = new System.Drawing.Point(288, 240);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(64, 22);
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "Edit..";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// TripPlanCatalog
			// 
			this.AcceptButton = this.buttonGo;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(504, 462);
			this.Controls.Add(this.panelTripPlanPreview);
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.panelTripPlans);
			this.Name = "TripPlanCatalog";
			this.Text = "Saved Trip-plans";
			this.Closed += new System.EventHandler(this.TripPlanCatalog_Closed);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.TripPlanCatalog_Closing);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TripPlanCatalog_FormClosing);
			this.panelTripPlans.ResumeLayout(false);
			this.panelTripPlanPreview.ResumeLayout(false);
			this.groupBoxTripPlanPreview.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void TripPlanCatalog_Closed(object sender, System.EventArgs e) {
			tripPlanViewer.Dispose();
			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			if(tripPlanList.SelectedTripPlan != null) {
				if(!trainDriverComboBox.ValidateInput())
					return;

				trainDriverComboBox.Commit();

				XmlDocument		tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

				tripPlanDoc.AppendChild(tripPlanDoc.ImportNode(tripPlanList.SelectedTripPlan.Element, true));

				TripPlanInfo	tripPlan = new TripPlanInfo(tripPlanDoc.DocumentElement);

				if(tripPlanList.ShouldReverseSelectedTripPlan)
					tripPlan.Reverse();

				Dialogs.TripPlanEditor	tripPlanEditor = new Dialogs.TripPlanEditor(tripPlan, train);

				tripPlan.FromCatalog = true;
				tripPlanEditor.Show();

				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void buttonGo_Click(object sender, System.EventArgs e) {
			if(tripPlanList.SelectedTripPlan != null) {
				if(!trainDriverComboBox.ValidateInput())
					return;

				trainDriverComboBox.Commit();

				XmlDocument		tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

				tripPlanDoc.AppendChild(tripPlanDoc.ImportNode(tripPlanList.SelectedTripPlan.Element, true));

				TripPlanInfo	tripPlan = new TripPlanInfo(tripPlanDoc.DocumentElement);

				if(tripPlanList.ShouldReverseSelectedTripPlan)
					tripPlan.Reverse();

				TripPlanAssignmentInfo	tripAssignment = new TripPlanAssignmentInfo(tripPlan, train);

				EventManager.Event(new LayoutEvent(tripAssignment, "execute-trip-plan"));
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void listViewTripPlans_DoubleClick(object sender, System.EventArgs e) {
			buttonGo.PerformClick();
		}

		private void TripPlanCatalog_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(Owner != null)
				Owner.Activate();
		}

		private void tripPlanList_SelectedTripPlanChanged(object sender, System.EventArgs e) {
			if(tripPlanList.SelectedTripPlan != null) {
				TripPlanInfo	tripPlan = tripPlanList.SelectedTripPlan;

				if(tripPlanList.ShouldReverseSelectedTripPlan) {
					XmlDocument		tripPlanDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

					tripPlanDoc.AppendChild(tripPlanDoc.ImportNode(tripPlanList.SelectedTripPlan.Element, true));

					tripPlan = new TripPlanInfo(tripPlanDoc.DocumentElement);
					tripPlan.Reverse();
				}

				tripPlanViewer.TripPlanName = tripPlan.Name;
				tripPlanViewer.TripPlan = tripPlan;
			}
			else
				tripPlanViewer.TripPlan = null;

			updateButtons();
		}

		private void TripPlanCatalog_FormClosing(object sender, FormClosingEventArgs e) {
			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
		}
	}
}
