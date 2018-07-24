using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TripPlanList.
    /// </summary>
    public class TripPlanList : System.Windows.Forms.UserControl
	{
		private Button buttonChangeIcon;
		private Button buttonDelete;
		private ListView listViewTripPlans;
		private Button buttonRename;
		private Button buttonDuplicate;
		private ImageList imageListState;
		private IContainer components;

		private void endOfDesignerVariables() { }

		bool					initialized = false;
		XmlElement				applicableTripPlansElement = null;
		TripPlanInfo			tripPlanToSelect = null;
		TripPlanIconListInfo	tripPlanIconList  = null;

		public event EventHandler	SelectedTripPlanChanged = null;

        public TripPlanList()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		public XmlElement ApplicableTripPlansElement {
			get {
				return applicableTripPlansElement;
			}

			set {
				applicableTripPlansElement = value;
				if(initialized)
					fillList();
			}
		}

		public TripPlanInfo SelectedTripPlan {
			get {
				if(initialized) {
					if(listViewTripPlans.SelectedItems.Count > 0) {
						TripPlanItem	selected = (TripPlanItem)listViewTripPlans.SelectedItems[0];

						return selected.TripPlan;
					}

					return null;
				}
				else
					return tripPlanToSelect;
			}

			set {
				if(initialized) {
					foreach(TripPlanItem item in listViewTripPlans.Items)
						item.Selected = false;

					if(value != null) {
						foreach(TripPlanItem item in listViewTripPlans.Items) {
							if(item.TripPlan.Id == value.Id) {
								item.Selected = true;

								if(SelectedTripPlanChanged != null)
									SelectedTripPlanChanged(this, null);

								break;
							}
						}
					}
				}
				else
					tripPlanToSelect = value;
			}
		}

		public bool ShouldReverseSelectedTripPlan {
			get {
				if(initialized) {
					if(listViewTripPlans.SelectedItems.Count > 0) {
						TripPlanItem	selected = (TripPlanItem)listViewTripPlans.SelectedItems[0];

						return selected.Reversed;
					}
					else
						throw new ApplicationException("No trip plan is selected");
				}
				else
					throw new ApplicationException("You should first Initialize the control");
			}
		}

		public void Initialize() {
			tripPlanIconList = LayoutModel.Instance.TripPlanIconList;
			listViewTripPlans.LargeImageList = tripPlanIconList.LargeIconImageList;
			listViewTripPlans.StateImageList = imageListState;

			if(applicableTripPlansElement != null)
				fillList();

			initialized = true;

			if(tripPlanToSelect != null)
				SelectedTripPlan = tripPlanToSelect;
		}

		private void fillList() {
			listViewTripPlans.Items.Clear();

			foreach(XmlElement applicableTripPlanElement in applicableTripPlansElement) {
				TripPlanInfo	originalTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[XmlConvert.ToGuid(applicableTripPlanElement.GetAttribute("TripPlanID"))];
				bool			shouldReverse = XmlConvert.ToBoolean(applicableTripPlanElement.GetAttribute("ShouldReverse"));
				TripPlanInfo	tripPlan = originalTripPlan;

				listViewTripPlans.Items.Add(new TripPlanItem(tripPlan, shouldReverse, tripPlanIconList));
			}

			if(listViewTripPlans.Items.Count > 0)
				SelectedTripPlan = ((TripPlanItem)listViewTripPlans.Items[0]).TripPlan;

			updateButtons();
		}

		private void updateButtons() {
			if(listViewTripPlans.SelectedItems.Count > 0) {
				buttonDuplicate.Enabled = listViewTripPlans.SelectedItems.Count == 1;
				buttonRename.Enabled = listViewTripPlans.SelectedItems.Count == 1;
				buttonDelete.Enabled = true;
				buttonChangeIcon.Enabled = listViewTripPlans.SelectedItems.Count == 1;
			}
			else {
				buttonDuplicate.Enabled = false;
				buttonRename.Enabled = false;
				buttonDelete.Enabled = false;
				buttonChangeIcon.Enabled = false;
			}
		}

		private void updateAll() {
			foreach(TripPlanItem item in listViewTripPlans.Items)
				item.Update();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TripPlanList));
			this.buttonChangeIcon = new Button();
			this.buttonDelete = new Button();
			this.listViewTripPlans = new ListView();
			this.buttonRename = new Button();
			this.buttonDuplicate = new Button();
			this.imageListState = new ImageList(this.components);
			this.SuspendLayout();
			// 
			// buttonChangeIcon
			// 
			this.buttonChangeIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonChangeIcon.Location = new System.Drawing.Point(158, 169);
			this.buttonChangeIcon.Name = "buttonChangeIcon";
			this.buttonChangeIcon.Size = new System.Drawing.Size(93, 20);
			this.buttonChangeIcon.TabIndex = 8;
			this.buttonChangeIcon.Text = "Change Icon...";
			this.buttonChangeIcon.Click += new System.EventHandler(this.buttonChangeIcon_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDelete.Location = new System.Drawing.Point(4, 169);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(71, 20);
			this.buttonDelete.TabIndex = 6;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// listViewTripPlans
			// 
			this.listViewTripPlans.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewTripPlans.HideSelection = false;
			this.listViewTripPlans.LabelEdit = true;
			this.listViewTripPlans.Location = new System.Drawing.Point(4, 4);
			this.listViewTripPlans.Name = "listViewTripPlans";
			this.listViewTripPlans.Size = new System.Drawing.Size(527, 160);
			this.listViewTripPlans.TabIndex = 5;
			this.listViewTripPlans.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewTripPlans_AfterLabelEdit);
			this.listViewTripPlans.SelectedIndexChanged += new System.EventHandler(this.listViewTripPlans_SelectedIndexChanged);
			// 
			// buttonRename
			// 
			this.buttonRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRename.Location = new System.Drawing.Point(81, 169);
			this.buttonRename.Name = "buttonRename";
			this.buttonRename.Size = new System.Drawing.Size(71, 20);
			this.buttonRename.TabIndex = 7;
			this.buttonRename.Text = "&Rename";
			this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
			// 
			// buttonDuplicate
			// 
			this.buttonDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDuplicate.Location = new System.Drawing.Point(256, 169);
			this.buttonDuplicate.Name = "buttonDuplicate";
			this.buttonDuplicate.Size = new System.Drawing.Size(64, 20);
			this.buttonDuplicate.TabIndex = 9;
			this.buttonDuplicate.Text = "D&uplicate";
			this.buttonDuplicate.Click += new System.EventHandler(this.buttonDuplicate_Click);
			// 
			// imageListState
			// 
			this.imageListState.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListState.ImageStream")));
			this.imageListState.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// TripPlanList
			// 
			this.Controls.Add(this.buttonChangeIcon);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.listViewTripPlans);
			this.Controls.Add(this.buttonRename);
			this.Controls.Add(this.buttonDuplicate);
			this.Name = "TripPlanList";
			this.Size = new System.Drawing.Size(536, 192);
			this.ResumeLayout(false);

		}
		#endregion

		#region Event Handlers

		private void buttonRemove_Click(object sender, System.EventArgs e) {
			if(listViewTripPlans.SelectedItems.Count > 0) {
				foreach(TripPlanItem tripPlanItem in listViewTripPlans.SelectedItems) {
					LayoutModel.StateManager.TripPlansCatalog.TripPlans.Remove(tripPlanItem.TripPlan.Id);
					listViewTripPlans.Items.Remove(tripPlanItem);
				}
			}
		}

		private void listViewTripPlans_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(listViewTripPlans.SelectedItems.Count > 0) {
				TripPlanItem	tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];

				if(SelectedTripPlanChanged != null)
					SelectedTripPlanChanged(this, null);
			}
			else
				SelectedTripPlan = null;

			updateButtons();
		}

		private void buttonChangeIcon_Click(object sender, System.EventArgs e) {
			if(listViewTripPlans.SelectedItems.Count > 0) {
				TripPlanItem	tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];
				TripPlanInfo	tripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanItem.TripPlan.Id];	// Get real version (non-reversed)

				LayoutManager.Tools.Dialogs.TripPlanChangeIcon	d = new LayoutManager.Tools.Dialogs.TripPlanChangeIcon(tripPlan, tripPlanIconList);

				tripPlanIconList.IconListModified = false;

				if(d.ShowDialog(this) == DialogResult.OK)
					tripPlanItem.Update();

				tripPlanItem.TripPlan.IconId = tripPlan.IconId;		// Make sure that the reversed version has the same icon ID

				if(tripPlanIconList.IconListModified) {
					LayoutModel.WriteModelXmlInfo();
					updateAll();
				}
			}
		}

		private void buttonRename_Click(object sender, System.EventArgs e) {
			if(listViewTripPlans.SelectedItems.Count > 0) {
				TripPlanItem	tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];

				tripPlanItem.BeginEdit();
			}
		}

		private void listViewTripPlans_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e) {
			TripPlanItem	selected = (TripPlanItem)listViewTripPlans.Items[e.Item];
			TripPlanInfo	existingTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[e.Label];

			if(existingTripPlan != null && existingTripPlan.Id != selected.TripPlan.Id) {
				MessageBox.Show(this, "A trip plan with this name already exists", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				e.CancelEdit = true;
			}
			else {
				TripPlanInfo	tripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[selected.TripPlan.Id];

				tripPlan.Name = e.Label;
				selected.TripPlan.Name = e.Label;
				selected.Update();
			}
		}

		private void buttonDuplicate_Click(object sender, System.EventArgs e) {
			if(listViewTripPlans.SelectedItems.Count > 0) {
				TripPlanItem	tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];
				XmlElement		newTripPlanElement;

				if(tripPlanItem.TripPlan.Element.OwnerDocument != LayoutModel.StateManager.TripPlansCatalog.Element.OwnerDocument)
					newTripPlanElement = (XmlElement)LayoutModel.StateManager.TripPlansCatalog.Element.OwnerDocument.ImportNode(tripPlanItem.TripPlan.Element, true);
				else
					newTripPlanElement = (XmlElement)tripPlanItem.TripPlan.Element.CloneNode(true);

				TripPlanInfo	newTripPlan = new TripPlanInfo(newTripPlanElement);

				newTripPlan.Name = "Copy of " + newTripPlan.Name;
				LayoutModel.StateManager.TripPlansCatalog.TripPlans.Add(newTripPlan);
				listViewTripPlans.Items.Add(new TripPlanItem(newTripPlan, false, tripPlanIconList));
			}
		}

		#endregion

		#region TripPlanItem class

		class TripPlanItem : ListViewItem {
			TripPlanInfo			tripPlan;
			TripPlanIconListInfo	tripPlanIconList;
			bool					reversed;

			public TripPlanItem(TripPlanInfo tripPlan, bool reversed, TripPlanIconListInfo tripPlanIconList) {
				this.tripPlan = tripPlan;
				this.reversed = reversed;
				this.tripPlanIconList = tripPlanIconList;

				Text = tripPlan.Name;
				ImageIndex = tripPlanIconList[tripPlan.IconId];

				IList<TripPlanWaypointInfo>	wayPoints = tripPlan.Waypoints;
				SubItems.Add(wayPoints[wayPoints.Count-1].Destination.Name);

				if(reversed)
					StateImageIndex = 0;
			}

			public void Update() {
				Text = tripPlan.Name;
				ImageIndex = tripPlanIconList[tripPlan.IconId];
			}

            public TripPlanInfo TripPlan => tripPlan;

            public bool Reversed => reversed;
        }

		#endregion
	}
}
