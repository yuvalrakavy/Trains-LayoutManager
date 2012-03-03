using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for DestinationEditor.
	/// </summary>
	public class OriginEditor : System.Windows.Forms.Form, IModelComponentReceiverComponent, IControlSupportViewOnly
	{
		private System.Windows.Forms.ImageList imageListButttons;
		private System.Windows.Forms.GroupBox groupBoxLocations;
		private System.Windows.Forms.ListBox listBoxLocations;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAddLocation;
		private System.Windows.Forms.Button buttonRemoveLocation;
		private System.Windows.Forms.Button buttonOk;
		private System.ComponentModel.IContainer components;

		private void endOfDesignerVariables() { }

		LayoutEventManager		eventManager;
		string					title;
		TripPlanInfo			tripPlan;
		DialogEditing			changeToViewonly = null;
		LayoutSelection			originSelection = null;
		LayoutSelection			selectedBlockInfoSelection = null;

		public OriginEditor(LayoutEventManager eventManager, TripPlanInfo tripPlan, string title)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.title = title;
			this.Text = title;
			this.tripPlan = tripPlan;
			this.eventManager = eventManager;

			originSelection = new LayoutSelection(eventManager);
			selectedBlockInfoSelection = new LayoutSelection(eventManager);

			foreach(TripPlanOriginInfo origin in tripPlan.Origins) {
				LayoutBlock	block = Model.Blocks[origin.BlockID];

				if(block != null) {
					listBoxLocations.Items.Add(new OriginItem(block.BlockInfo, origin.Front));
					originSelection.Add(block.BlockInfo);
				}
			}

			eventManager.AddObjectSubscriptions(this);

			originSelection.Display(new LayoutSelectionLook(Color.GreenYellow));
			selectedBlockInfoSelection.Display(new LayoutSelectionLook(Color.OrangeRed));
		}

		LayoutModel Model {
			get {
				return (LayoutModel)eventManager.Event(new LayoutEvent(this, "get-model"));
			}
		}

		private void updateButtons(object sender, EventArgs e) {
			selectedBlockInfoSelection.Clear();

			foreach(OriginItem originItem in listBoxLocations.SelectedItems)
				selectedBlockInfoSelection.Add(originItem.BlockInfo);

			if(listBoxLocations.SelectedItem != null)
				buttonRemoveLocation.Enabled = true;
			else
				buttonRemoveLocation.Enabled = false;
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

		private bool hasOrigin(LayoutBlockInfoComponent blockInfo) {
			foreach(OriginItem originItem in listBoxLocations.Items) {
				if(blockInfo.ID == originItem.BlockInfo.ID)
					return true;
			}
			
			return false;
		}

		[LayoutEvent("query-edit-origin-dialog")]
		private void queryEditOriginDialog(LayoutEvent e) {
			LayoutBlockInfoComponent	blockInfo = (LayoutBlockInfoComponent)e.Sender;
			ArrayList					dialogs = (ArrayList)e.Info;

			if(Enabled && !ViewOnly && !hasOrigin(blockInfo))
				dialogs.Add((IModelComponentReceiverComponent)this);
		}

		// Implementation of IModelComponentReceiverComponent

		public string DialogName {
			get {
				return this.Text;
			}
		}

		public void AddComponent(ModelComponent component) {
			LayoutBlockInfoComponent	blockInfo = (LayoutBlockInfoComponent)component;

			try {
				object	oFront = eventManager.Event(new LayoutEvent(blockInfo, "recommend-origin-front-request", null, tripPlan));

				if(oFront == null)
					oFront = eventManager.Event(new LayoutEvent(blockInfo, "get-locomotive-front", null, ""));

				if(oFront != null) {
					LayoutComponentConnectionPoint	front = (LayoutComponentConnectionPoint)oFront;
			
					listBoxLocations.Items.Add(new OriginItem(blockInfo, front));
					originSelection.Add(blockInfo);
					updateButtons(null, null);
				}
			} catch(LayoutException ex) {
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public bool ViewOnly {
			get {
				return changeToViewonly != null;
			}

			set {
				if(value && !ViewOnly) {
					Size	newSize = new Size(listBoxLocations.Width, buttonAddLocation.Bottom - listBoxLocations.Top);

					changeToViewonly = new DialogEditing(this,
						new DialogEditingCommandBase[] {
														   new DialogEditingRemoveControl(buttonAddLocation),
														   new DialogEditingRemoveControl(buttonOk),
														   new DialogEditingRemoveControl(buttonRemoveLocation),
														   new DialogEditingChangeText(buttonCancel, "Close"),
														   new DialogEditingResizeControl(listBoxLocations, newSize)
													   } );
					changeToViewonly.Do();
					changeToViewonly.ViewOnly = true;
					this.ControlBox = false;
					this.ShowInTaskbar = false;
				}
				else if(!value && ViewOnly) {
					changeToViewonly.Undo();
					changeToViewonly.ViewOnly = false;
					changeToViewonly = null;
					this.ControlBox = true;
					this.ShowInTaskbar = true;
				}
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(OriginEditor));
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxLocations = new System.Windows.Forms.GroupBox();
			this.buttonAddLocation = new System.Windows.Forms.Button();
			this.buttonRemoveLocation = new System.Windows.Forms.Button();
			this.listBoxLocations = new System.Windows.Forms.ListBox();
			this.imageListButttons = new System.Windows.Forms.ImageList(this.components);
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBoxLocations.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(150, 276);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(56, 20);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// groupBoxLocations
			// 
			this.groupBoxLocations.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.groupBoxLocations.Controls.AddRange(new System.Windows.Forms.Control[] {
																							this.buttonAddLocation,
																							this.buttonRemoveLocation,
																							this.listBoxLocations});
			this.groupBoxLocations.Location = new System.Drawing.Point(8, 13);
			this.groupBoxLocations.Name = "groupBoxLocations";
			this.groupBoxLocations.Size = new System.Drawing.Size(198, 259);
			this.groupBoxLocations.TabIndex = 4;
			this.groupBoxLocations.TabStop = false;
			this.groupBoxLocations.Text = "Locations:";
			// 
			// buttonAddLocation
			// 
			this.buttonAddLocation.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAddLocation.Location = new System.Drawing.Point(8, 232);
			this.buttonAddLocation.Name = "buttonAddLocation";
			this.buttonAddLocation.Size = new System.Drawing.Size(56, 20);
			this.buttonAddLocation.TabIndex = 1;
			this.buttonAddLocation.Text = "&Add";
			this.buttonAddLocation.Click += new System.EventHandler(this.buttonAddLocation_Click);
			// 
			// buttonRemoveLocation
			// 
			this.buttonRemoveLocation.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRemoveLocation.Location = new System.Drawing.Point(70, 232);
			this.buttonRemoveLocation.Name = "buttonRemoveLocation";
			this.buttonRemoveLocation.Size = new System.Drawing.Size(56, 20);
			this.buttonRemoveLocation.TabIndex = 2;
			this.buttonRemoveLocation.Text = "&Remove";
			this.buttonRemoveLocation.Click += new System.EventHandler(this.buttonRemoveLocation_Click);
			// 
			// listBoxLocations
			// 
			this.listBoxLocations.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listBoxLocations.DisplayMember = "Name";
			this.listBoxLocations.IntegralHeight = false;
			this.listBoxLocations.Location = new System.Drawing.Point(8, 17);
			this.listBoxLocations.Name = "listBoxLocations";
			this.listBoxLocations.Size = new System.Drawing.Size(184, 210);
			this.listBoxLocations.TabIndex = 0;
			this.listBoxLocations.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
			// 
			// imageListButttons
			// 
			this.imageListButttons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListButttons.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListButttons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButttons.ImageStream")));
			this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonOk.Location = new System.Drawing.Point(86, 276);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(56, 20);
			this.buttonOk.TabIndex = 6;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// OriginEditor
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(214, 301);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBoxLocations,
																		  this.buttonCancel,
																		  this.buttonOk});
			this.Name = "OriginEditor";
			this.Text = "OriginEditor";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.DestinationEditor_Closing);
			this.groupBoxLocations.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(listBoxLocations.Items.Count == 0) {
				MessageBox.Show(this, "Destination must contain at least one location", "Invalid destination", MessageBoxButtons.OK, MessageBoxIcon.Error);
				listBoxLocations.Focus();
				return;
			}

			tripPlan.Origins.Clear();

			foreach(OriginItem originItem in listBoxLocations.Items)
				tripPlan.Origins.Add(originItem.BlockInfo.Block.ID, originItem.Front);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void buttonRemoveLocation_Click(object sender, System.EventArgs e) {
			OriginItem	selected = (OriginItem)listBoxLocations.SelectedItem;

			if(selected != null) {
				listBoxLocations.Items.Remove(selected);
				originSelection.Remove(selected.BlockInfo);
				updateButtons(null, null);
			}
		}

		private void buttonAddLocation_Click(object sender, System.EventArgs e) {
			ContextMenu	menu = new ContextMenu();

			if(Model.StateManager.TripPlansCatalog.Destinations.Count > 0) {
				MenuItem	savedDestinations = new MenuItem("Common Destinations");

				foreach(TripPlanDestinationInfo destination in Model.StateManager.TripPlansCatalog.Destinations)
					savedDestinations.MenuItems.Add(new AddSavedDestinationMenuItem(this, destination));

				menu.MenuItems.Add(savedDestinations);
				menu.MenuItems.Add("-");
			}

			eventManager.Event(new LayoutEvent(
				new CommonUI.BuildComponentsMenuOptions(typeof(LayoutBlockInfoComponent),
					"*[@SuggestForDestination='true']", 
					new CommonUI.BuildComponentsMenuComponentFilter(this.addMenuFilter), 
					new EventHandler(onAddComponentMenuItemClick)
				),
				"build-components-menu", null, menu));

			if(menu.MenuItems.Count > 0)
				menu.Show(groupBoxLocations, new Point(buttonAddLocation.Left, buttonAddLocation.Bottom));
		}

		class AddSavedDestinationMenuItem : MenuItem {
			OriginEditor			originEditor;
			TripPlanDestinationInfo	destination;

			public AddSavedDestinationMenuItem(OriginEditor originEditor, TripPlanDestinationInfo destination) {
				this.originEditor = originEditor;;
				this.destination = destination;

				this.Text = destination.Name;
			}

			protected override void OnClick(EventArgs e) {
				foreach(Guid blockID in destination.BlockIDs) {
					LayoutBlock	block = originEditor.Model.Blocks[blockID];

					if(block != null) {
						LayoutBlockInfoComponent	blockInfo = block.BlockInfo;

						if(!originEditor.listBoxLocations.Items.Contains(blockInfo))
							originEditor.AddComponent(blockInfo);
					}
				}
			}
		}


		private bool addMenuFilter(ModelComponent component) {
			foreach(ModelComponent c in listBoxLocations.Items)
				if(c.ID == component.ID)
					return false;

			return true;
		}

		private void onAddComponentMenuItemClick(object sender, EventArgs e) {
			CommonUI.ModelComponentMenuItemBase	item = (CommonUI.ModelComponentMenuItemBase)sender;

			AddComponent(item.Component);
		}

		private void DestinationEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			originSelection.Hide();
			originSelection.Clear();

			selectedBlockInfoSelection.Hide();
			selectedBlockInfoSelection.Clear();

			eventManager.Subscriptions.RemoveObjectSubscriptions(this);
			Owner = null;
		}

		class OriginItem {
			LayoutBlockInfoComponent		blockInfo;
			LayoutComponentConnectionPoint	front;

			public OriginItem(LayoutBlockInfoComponent blockInfo, LayoutComponentConnectionPoint front) {
				this.blockInfo = blockInfo;
				this.front = front;
			}

			public LayoutBlockInfoComponent BlockInfo {
				get {
					return blockInfo;
				}
			}

			public LayoutComponentConnectionPoint Front {
				get {
					return front;
				}
			}

			public override string ToString() {
				if(blockInfo.Name != null && blockInfo.Name != "")
					return blockInfo.Name;
				return "Block";
			}
		}
	}
}
