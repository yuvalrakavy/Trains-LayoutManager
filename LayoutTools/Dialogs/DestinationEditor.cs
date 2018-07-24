using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for DestinationEditor.
    /// </summary>
    public class DestinationEditor : Form, IModelComponentReceiverDialog, IControlSupportViewOnly
	{
		private Label label1;
		private Label label2;
		private GroupBox groupBoxSelection;
		private ImageList imageListButttons;
		private CommonUI.TextBoxWithViewOnly textBoxName;
		private GroupBox groupBoxLocations;
		private CommonUI.RadioButtonWithViewOnly radioButtonSelectionListOrder;
		private CommonUI.RadioButtonWithViewOnly radioButtonSelectionRandom;
		private Button buttonMoveLocationDown;
		private Button buttonMoveLocationUp;
		private Button buttonCancel;
		private Button buttonAddLocation;
		private Button buttonRemoveLocation;
		private Button buttonOk;
		private Button buttonSave;
		private ListView listViewLocations;
		private ColumnHeader columnHeaderLocation;
		private ColumnHeader columnHeaderCondition;
		private Button buttonCondition;
		private IContainer components;

		private void endOfDesignerVariables() { }

		TripPlanDestinationInfo	inDestination;
		string					title;
		TripPlanDestinationInfo	destination;
		DialogEditing			changeToViewonly = null;
		LayoutSelection			destinationSelection = null;
		LayoutSelection			selectedBlockInfoSelection = null;

		public DestinationEditor(TripPlanDestinationInfo inDestination, string title)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.title = title;
			this.Text = title;
			this.inDestination = inDestination;
			
			XmlDocument	tempDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
			tempDoc.AppendChild(tempDoc.ImportNode(inDestination.Element, true));
			destination = new TripPlanDestinationInfo(tempDoc.DocumentElement);

			if(destination.HasName)
				textBoxName.Text = destination.Name;

			if(destination.SelectionMethod == TripPlanLocationSelectionMethod.ListOrder)
				radioButtonSelectionListOrder.Checked = true;
			else
				radioButtonSelectionRandom.Checked = true;

			foreach(TripPlanDestinationEntryInfo entry in destination.Entries)
				listViewLocations.Items.Add(new LocationItem(entry));

			EventManager.AddObjectSubscriptions(this);

			destinationSelection = new LayoutSelection();
			selectedBlockInfoSelection = new LayoutSelection();

			destinationSelection.Add(destination.Selection);

			destinationSelection.Display(new LayoutSelectionLook(Color.Orange));
			selectedBlockInfoSelection.Display(new LayoutSelectionLook(Color.OrangeRed));
		}


		private void updateButtons(object sender, EventArgs e) {
			selectedBlockInfoSelection.Clear();

			foreach(LocationItem item in listViewLocations.SelectedItems) {
				LayoutBlockDefinitionComponent	blockInfo = item.BlockDefinition;

				if(blockInfo != null)
					selectedBlockInfoSelection.Add(blockInfo);
			}

			if(listViewLocations.SelectedItems.Count > 0) {
				buttonRemoveLocation.Enabled = true;

				int	selectedIndex = listViewLocations.SelectedIndices[0];

				buttonMoveLocationUp.Enabled = (selectedIndex > 0);
				buttonMoveLocationDown.Enabled = (selectedIndex < listViewLocations.Items.Count - 1);
				buttonCondition.Enabled = true;
			}
			else {
				buttonRemoveLocation.Enabled = false;
				buttonCondition.Enabled = false;

				buttonMoveLocationUp.Enabled = false;
				buttonMoveLocationDown.Enabled = false;
			}

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

		[LayoutEvent("query-edit-destination-dialog")]
		private void queryEditDestinationDialog(LayoutEvent e0) {
			var e = (LayoutEvent<LayoutBlockDefinitionComponent, List<IModelComponentReceiverDialog>>)e0;
			var blockDefinition = e.Sender;
			var dialogs = e.Info;
			bool locationInList = false;

			foreach(LocationItem item in listViewLocations.Items)
				if(item.Entry.BlockId == blockDefinition.Block.Id) {
					locationInList = true;
					break;
				}

			if(Enabled && !ViewOnly && !locationInList)
				dialogs.Add(this);
		}

        // Implementation of IModelComponentReceiverComponent

        public string DialogName(IModelComponent component) => this.Text;

        public void AddComponent(IModelComponent component) {
			LayoutBlockDefinitionComponent	blockInfo = (LayoutBlockDefinitionComponent)component;

			TripPlanDestinationEntryInfo	newEntry = destination.Add(blockInfo);
			listViewLocations.Items.Add(new LocationItem(newEntry));
			destinationSelection.Add(blockInfo);
			updateButtons(null, null);
		}

		public bool ViewOnly {
			get {
				return changeToViewonly != null;
			}

			set {
				if(value && !ViewOnly) {
					Size	newSize = new Size(buttonMoveLocationUp.Right - listViewLocations.Left, buttonAddLocation.Bottom - listViewLocations.Top);

					changeToViewonly = new DialogEditing(this,
						new DialogEditingCommandBase[] {
														   new DialogEditingRemoveControl(buttonAddLocation),
														   new DialogEditingRemoveControl(buttonMoveLocationDown),
														   new DialogEditingRemoveControl(buttonMoveLocationUp),
														   new DialogEditingRemoveControl(buttonCondition),
														   new DialogEditingRemoveControl(buttonOk),
														   new DialogEditingRemoveControl(buttonRemoveLocation),
														   new DialogEditingChangeText(buttonCancel, "Close"),
														   new DialogEditingResizeControl(listViewLocations, newSize)
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

		public bool HideSaveButton {
			get {
				return buttonSave.Visible;
			}

			set {
				buttonSave.Visible = false;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DestinationEditor));
			this.buttonCancel = new Button();
			this.label1 = new Label();
			this.textBoxName = new LayoutManager.CommonUI.TextBoxWithViewOnly();
			this.groupBoxLocations = new GroupBox();
			this.buttonCondition = new Button();
			this.listViewLocations = new ListView();
			this.columnHeaderLocation = new ColumnHeader();
			this.columnHeaderCondition = new ColumnHeader();
			this.buttonAddLocation = new Button();
			this.buttonRemoveLocation = new Button();
			this.buttonMoveLocationUp = new Button();
			this.imageListButttons = new ImageList(this.components);
			this.buttonMoveLocationDown = new Button();
			this.label2 = new Label();
			this.groupBoxSelection = new GroupBox();
			this.radioButtonSelectionListOrder = new LayoutManager.CommonUI.RadioButtonWithViewOnly();
			this.radioButtonSelectionRandom = new LayoutManager.CommonUI.RadioButtonWithViewOnly();
			this.buttonOk = new Button();
			this.buttonSave = new Button();
			this.groupBoxLocations.SuspendLayout();
			this.groupBoxSelection.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(232, 276);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(56, 20);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(42, 6);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(188, 20);
			this.textBoxName.TabIndex = 1;
			this.textBoxName.ViewOnly = false;
			// 
			// groupBoxLocations
			// 
			this.groupBoxLocations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxLocations.Controls.Add(this.buttonCondition);
			this.groupBoxLocations.Controls.Add(this.listViewLocations);
			this.groupBoxLocations.Controls.Add(this.buttonAddLocation);
			this.groupBoxLocations.Controls.Add(this.buttonRemoveLocation);
			this.groupBoxLocations.Controls.Add(this.buttonMoveLocationUp);
			this.groupBoxLocations.Controls.Add(this.buttonMoveLocationDown);
			this.groupBoxLocations.Location = new System.Drawing.Point(8, 112);
			this.groupBoxLocations.Name = "groupBoxLocations";
			this.groupBoxLocations.Size = new System.Drawing.Size(280, 160);
			this.groupBoxLocations.TabIndex = 4;
			this.groupBoxLocations.TabStop = false;
			this.groupBoxLocations.Text = "Locations:";
			// 
			// buttonCondition
			// 
			this.buttonCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCondition.Location = new System.Drawing.Point(168, 133);
			this.buttonCondition.Name = "buttonCondition";
			this.buttonCondition.Size = new System.Drawing.Size(73, 20);
			this.buttonCondition.TabIndex = 3;
			this.buttonCondition.Text = "Condition...";
			this.buttonCondition.Click += new System.EventHandler(this.buttonCondition_Click);
			// 
			// listViewLocations
			// 
			this.listViewLocations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewLocations.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderLocation,
            this.columnHeaderCondition});
			this.listViewLocations.FullRowSelect = true;
			this.listViewLocations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewLocations.HideSelection = false;
			this.listViewLocations.Location = new System.Drawing.Point(8, 16);
			this.listViewLocations.MultiSelect = false;
			this.listViewLocations.Name = "listViewLocations";
			this.listViewLocations.Size = new System.Drawing.Size(240, 112);
			this.listViewLocations.TabIndex = 0;
			this.listViewLocations.UseCompatibleStateImageBehavior = false;
			this.listViewLocations.View = System.Windows.Forms.View.Details;
			this.listViewLocations.SelectedIndexChanged += new System.EventHandler(this.listViewLocations_SelectedIndexChanged);
			// 
			// columnHeaderLocation
			// 
			this.columnHeaderLocation.Text = "Location";
			this.columnHeaderLocation.Width = 78;
			// 
			// columnHeaderCondition
			// 
			this.columnHeaderCondition.Text = "Condition";
			this.columnHeaderCondition.Width = 154;
			// 
			// buttonAddLocation
			// 
			this.buttonAddLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAddLocation.Location = new System.Drawing.Point(8, 133);
			this.buttonAddLocation.Name = "buttonAddLocation";
			this.buttonAddLocation.Size = new System.Drawing.Size(73, 20);
			this.buttonAddLocation.TabIndex = 1;
			this.buttonAddLocation.Text = "&Add";
			this.buttonAddLocation.Click += new System.EventHandler(this.buttonAddLocation_Click);
			// 
			// buttonRemoveLocation
			// 
			this.buttonRemoveLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRemoveLocation.Location = new System.Drawing.Point(88, 133);
			this.buttonRemoveLocation.Name = "buttonRemoveLocation";
			this.buttonRemoveLocation.Size = new System.Drawing.Size(73, 20);
			this.buttonRemoveLocation.TabIndex = 2;
			this.buttonRemoveLocation.Text = "&Remove";
			this.buttonRemoveLocation.Click += new System.EventHandler(this.buttonRemoveLocation_Click);
			// 
			// buttonMoveLocationUp
			// 
			this.buttonMoveLocationUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonMoveLocationUp.ImageIndex = 1;
			this.buttonMoveLocationUp.ImageList = this.imageListButttons;
			this.buttonMoveLocationUp.Location = new System.Drawing.Point(251, 17);
			this.buttonMoveLocationUp.Name = "buttonMoveLocationUp";
			this.buttonMoveLocationUp.Size = new System.Drawing.Size(24, 20);
			this.buttonMoveLocationUp.TabIndex = 4;
			this.buttonMoveLocationUp.Click += new System.EventHandler(this.buttonMoveLocationUp_Click);
			// 
			// imageListButttons
			// 
			this.imageListButttons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButttons.ImageStream")));
			this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListButttons.Images.SetKeyName(0, "");
			this.imageListButttons.Images.SetKeyName(1, "");
			// 
			// buttonMoveLocationDown
			// 
			this.buttonMoveLocationDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonMoveLocationDown.ImageIndex = 0;
			this.buttonMoveLocationDown.ImageList = this.imageListButttons;
			this.buttonMoveLocationDown.Location = new System.Drawing.Point(251, 40);
			this.buttonMoveLocationDown.Name = "buttonMoveLocationDown";
			this.buttonMoveLocationDown.Size = new System.Drawing.Size(24, 20);
			this.buttonMoveLocationDown.TabIndex = 5;
			this.buttonMoveLocationDown.Click += new System.EventHandler(this.buttonMoveLocationDown_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(184, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "(Leave empty to use default name)";
			// 
			// groupBoxSelection
			// 
			this.groupBoxSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelection.Controls.Add(this.radioButtonSelectionListOrder);
			this.groupBoxSelection.Controls.Add(this.radioButtonSelectionRandom);
			this.groupBoxSelection.Location = new System.Drawing.Point(8, 48);
			this.groupBoxSelection.Name = "groupBoxSelection";
			this.groupBoxSelection.Size = new System.Drawing.Size(280, 56);
			this.groupBoxSelection.TabIndex = 3;
			this.groupBoxSelection.TabStop = false;
			this.groupBoxSelection.Text = "Selection:";
			// 
			// radioButtonSelectionListOrder
			// 
			this.radioButtonSelectionListOrder.Location = new System.Drawing.Point(8, 16);
			this.radioButtonSelectionListOrder.Name = "radioButtonSelectionListOrder";
			this.radioButtonSelectionListOrder.Size = new System.Drawing.Size(168, 16);
			this.radioButtonSelectionListOrder.TabIndex = 0;
			this.radioButtonSelectionListOrder.Text = "Priority based on order in list";
			this.radioButtonSelectionListOrder.ViewOnly = false;
			// 
			// radioButtonSelectionRandom
			// 
			this.radioButtonSelectionRandom.AutoSize = true;
			this.radioButtonSelectionRandom.Location = new System.Drawing.Point(8, 34);
			this.radioButtonSelectionRandom.Name = "radioButtonSelectionRandom";
			this.radioButtonSelectionRandom.Size = new System.Drawing.Size(228, 17);
			this.radioButtonSelectionRandom.TabIndex = 1;
			this.radioButtonSelectionRandom.Text = "Best fit between train and destination lenth ";
			this.radioButtonSelectionRandom.ViewOnly = false;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.Location = new System.Drawing.Point(168, 276);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(56, 20);
			this.buttonOk.TabIndex = 6;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonSave
			// 
			this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonSave.Location = new System.Drawing.Point(8, 276);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(56, 20);
			this.buttonSave.TabIndex = 5;
			this.buttonSave.Text = "Save...";
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// DestinationEditor
			// 
			this.AcceptButton = this.buttonOk;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(296, 301);
			this.Controls.Add(this.groupBoxSelection);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBoxLocations);
			this.Controls.Add(this.textBoxName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonSave);
			this.Name = "DestinationEditor";
			this.Text = "DestinationEditor";
			this.Closed += new System.EventHandler(this.DestinationEditor_Closed);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.DestinationEditor_Closing);
			this.groupBoxLocations.ResumeLayout(false);
			this.groupBoxSelection.ResumeLayout(false);
			this.groupBoxSelection.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(listViewLocations.Items.Count == 0) {
				MessageBox.Show(this, "Destination must contain at least one location", "Invalid destination", MessageBoxButtons.OK, MessageBoxIcon.Error);
				listViewLocations.Focus();
				return;
			}

			if(textBoxName.Text.Trim() == "")
				destination.HasName = false;
			else
				destination.Name = textBoxName.Text;

			if(radioButtonSelectionListOrder.Checked)
				destination.SelectionMethod = TripPlanLocationSelectionMethod.ListOrder;
			else
				destination.SelectionMethod = TripPlanLocationSelectionMethod.Random;

			destination.Clear();
			foreach(LocationItem item in listViewLocations.Items)
				destination.Add(item.Entry);

			XmlElement	inParent = (XmlElement)inDestination.Element.ParentNode;

			inParent.RemoveChild(inDestination.Element);
			inParent.AppendChild(inParent.OwnerDocument.ImportNode(destination.Element, true));

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void DestinationEditor_Closed(object sender, System.EventArgs e) {
			destinationSelection.Hide();
			destinationSelection.Clear();

			selectedBlockInfoSelection.Hide();
			selectedBlockInfoSelection.Clear();

			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
		}

		private void buttonMoveLocationUp_Click(object sender, System.EventArgs e) {
			if(listViewLocations.SelectedItems.Count > 0) {
				LocationItem	selected = (LocationItem)listViewLocations.SelectedItems[0];
				int				selectedIndex = listViewLocations.SelectedIndices[0];

				if(selectedIndex > 0) {
					listViewLocations.Items.Remove(selected);
					listViewLocations.Items.Insert(selectedIndex-1, selected);
					selected.Selected = true;
				}
			}
		}

		private void buttonMoveLocationDown_Click(object sender, System.EventArgs e) {
			if(listViewLocations.SelectedItems.Count > 0) {
				LocationItem	selected = (LocationItem)listViewLocations.SelectedItems[0];
				int				selectedIndex = listViewLocations.SelectedIndices[0];

				if(selectedIndex < listViewLocations.Items.Count - 1) {
					listViewLocations.Items.Remove(selected);
					listViewLocations.Items.Insert(selectedIndex+1, selected);
					selected.Selected = true;
				}
			}
		}

		private void buttonRemoveLocation_Click(object sender, System.EventArgs e) {
			if(listViewLocations.SelectedItems.Count > 0) {
				LocationItem	selected = (LocationItem)listViewLocations.SelectedItems[0];

				listViewLocations.Items.Remove(selected);
				destinationSelection.Remove(selected.BlockDefinition);
				updateButtons(null, null);
			}
		}

		private void buttonAddLocation_Click(object sender, System.EventArgs e) {
			ContextMenu	menu = new ContextMenu();

			new BuildComponentsMenu<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases, "*[@SuggestForDestination='true']",
					new CommonUI.BuildComponentsMenuComponentFilter<LayoutBlockDefinitionComponent>(this.addMenuFilter),
					delegate(object s, EventArgs ea) { AddComponent(((ModelComponentMenuItemBase<LayoutBlockDefinitionComponent>)s).Component); }).AddComponentMenuItems(menu);

			if(menu.MenuItems.Count > 0)
				menu.Show(groupBoxLocations, new Point(buttonAddLocation.Left, buttonAddLocation.Bottom));
		}

		private bool addMenuFilter(LayoutBlockDefinitionComponent component) {
			foreach(LocationItem item in listViewLocations.Items)
				if(item.BlockDefinition.Id == component.Id)
					return false;

			return true;
		}

		private void buttonSave_Click(object sender, System.EventArgs e) {
			CommonUI.Dialogs.InputBox	inBox = new CommonUI.Dialogs.InputBox("Save Destination", "Enter destination name:");

			inBox.Input = textBoxName.Text;

			if(inBox.ShowDialog(this) == DialogResult.OK) {
				bool				doSave = true;
				string				name = inBox.Input;
				TripPlanCatalogInfo	tripPlanCatalog = LayoutModel.StateManager.TripPlansCatalog;

				TripPlanDestinationInfo	existingDestination = tripPlanCatalog.Destinations[name];

				if(existingDestination != null) {
					if(MessageBox.Show(this, "A destination with name already exists, would you like to replace it?", 
						"Destination already exist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
						doSave = false;
					else
						tripPlanCatalog.Destinations.Remove(existingDestination);
				}

				if(doSave) {
					XmlElement				catalogDestinationElement = LayoutModel.StateManager.TripPlansCatalog.Element.OwnerDocument.CreateElement("Destination");
					TripPlanDestinationInfo	catalogDestination = new TripPlanDestinationInfo(catalogDestinationElement);

					foreach(LocationItem item in listViewLocations.Items) {
						XmlElement	catalogDestinationEntryElement = (XmlElement)catalogDestinationElement.OwnerDocument.ImportNode(item.Entry.Element, true);

						catalogDestination.Add(new TripPlanDestinationEntryInfo(catalogDestinationEntryElement));
					}

					catalogDestination.Name = name;

					if(radioButtonSelectionListOrder.Checked)
						catalogDestination.SelectionMethod = TripPlanLocationSelectionMethod.ListOrder;
					else
						catalogDestination.SelectionMethod = TripPlanLocationSelectionMethod.Random;

					LayoutModel.StateManager.TripPlansCatalog.Destinations.Add(catalogDestination);
				}
			}
		}

		private void DestinationEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			Owner = null;
		}

		private void buttonCondition_Click(object sender, System.EventArgs e) {
			if(listViewLocations.SelectedItems.Count > 0) {
				LocationItem	selected = (LocationItem)listViewLocations.SelectedItems[0];

				selected.Edit(this);
			}
		}

		private void listViewLocations_SelectedIndexChanged(object sender, System.EventArgs e) {
			updateButtons(null, null);
		}

		class LocationItem : ListViewItem {
			TripPlanDestinationEntryInfo	entry;

			public LocationItem(TripPlanDestinationEntryInfo entry) {
				this.entry = entry;

				SubItems.Add("");
				Update();
			}

			public void Update() {
				LayoutBlock	block = LayoutModel.Blocks[entry.BlockId];

				if(block != null)
					SubItems[0].Text = block.BlockDefinintion.Name;
				else
					SubItems[0].Text = "*UNKNOWN*";

				SubItems[1].Text = entry.GetDescription();
			}

            public TripPlanDestinationEntryInfo Entry => entry;

            public LayoutBlockDefinitionComponent BlockDefinition {
				get {
					LayoutBlock	block = LayoutModel.Blocks[entry.BlockId];

					if(block != null)
						return block.BlockDefinintion;
					return null;
				}
			}

			public bool Edit(IWin32Window owner) {
				CommonUI.Dialogs.TrainConditionDefinition	d = new CommonUI.Dialogs.TrainConditionDefinition(BlockDefinition, entry);

				if(d.ShowDialog(owner) == DialogResult.OK) {
					XmlElement	parentElement = (XmlElement)entry.Element.ParentNode;
					XmlElement	newEntryElement = (XmlElement)entry.Element.OwnerDocument.ImportNode(d.TrainCondition.Element, true);

					parentElement.ReplaceChild(newEntryElement, entry.Element);
					entry = new TripPlanDestinationEntryInfo(newEntryElement);

					Update();
					return true;
				}
				else
					return false;
			}
		}
	}
}
