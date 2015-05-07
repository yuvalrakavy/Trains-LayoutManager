using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Tools;

namespace LayoutManager
{
	/// <summary>
	/// Summary description for LocomotivesViewer.
	/// </summary>
	public class LocomotivesViewer : System.Windows.Forms.UserControl
	{
		private ContextMenu contextMenuAdd;
		private MenuItem menuItemAddLocomotive;
		private IContainer components;
		private ContextMenu contextMenuOptions;
		private MenuItem menuItemArrange;
		private Button buttonAdd;
		private Button buttonEdit;
		private Button buttonDelete;
		private Button buttonOptions;
		private Panel panel1;
		private Button buttonClose;
		private ImageList imageListCloseButton;
		private ContextMenu contextMenuArrange;
		private LayoutManager.CommonUI.Controls.LocomotiveList locomotiveList;
		private MenuItem menuItemAddTrain;
		private MenuItem menuItemStorage;

		private void EndOfDesignerVariables() { }

		LocomotiveCollectionInfo	locomotiveCollection;
		bool						operationMode = false;


		public LocomotivesViewer()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		public LocomotiveCollectionInfo LocomotiveCollection {
			set {
				locomotiveCollection = value;
				if(locomotiveCollection != null)
					locomotiveList.ContainerElement = locomotiveCollection.CollectionElement;
				locomotiveList.CurrentListLayoutIndex = 0;
				updateButtons();
			}

			get {
				return locomotiveCollection;
			}
		}

		/// <summary>
		/// Check if an item can be deleted. In operation mode, an item can be deleted only if it is not on track (no state information)
		/// </summary>
		/// <param name="selectedElement"></param>
		/// <returns>True - item can be deleted</returns>
		private bool canDelete(XmlElement selectedElement) {
			bool	result = true;

			if(operationMode) {
				if(selectedElement.Name == "Locomotive") {
					LocomotiveInfo				loco = new LocomotiveInfo(selectedElement);

					result = LayoutModel.StateManager.Trains[loco.Id] == null;
				}
				else if(selectedElement.Name == "Train") {
					TrainInCollectionInfo	train = new TrainInCollectionInfo(selectedElement);

					result = LayoutModel.StateManager.Trains[train.Id] == null;
				}
			}

			return result;
		}

		private void updateButtons() {
			XmlElement	selectedElement = locomotiveList.SelectedXmlElement;

			if(selectedElement != null) {
				buttonEdit.Enabled = true;
				buttonDelete.Enabled = canDelete(selectedElement);
			}
			else {
				buttonEdit.Enabled = false;
				buttonDelete.Enabled = false;
			}
		}

		public void Initialize() {
			EventManager.AddObjectSubscriptions(this);
			locomotiveList.Initialize();
			locomotiveList.AddLayoutMenuItems(menuItemArrange);
			locomotiveList.AddLayoutMenuItems(contextMenuArrange);
		}

		[LayoutEvent("enter-design-mode")]
		private void enterDesignMode(LayoutEvent e) {
			operationMode = false;
			buttonOptions.Text = "&Options";
		}

		[LayoutEvent("enter-operation-mode")]
		private void enterOperationMode(LayoutEvent e) {
			operationMode = true;
			buttonOptions.Text = "&Arrange";
		}

		[LayoutEvent("train-created")]
		[LayoutEvent("train-is-removed")]
		private void eventThatUpdateButtons(LayoutEvent e) {
			updateButtons();
		}

		[LayoutEvent("disable-locomotive-list-update")]
		private void disableLocomotiveListUpdate(LayoutEvent e) {
			locomotiveList.DisableUpdate();
		}

		[LayoutEvent("enable-locomotive-list-update")]
		private void enableLocomotiveListUpdate(LayoutEvent e) {
			locomotiveList.EnableUpdate();
		}

		protected void SaveModelDocument() {
			LayoutModel.WriteModelXmlInfo();
		}

		[LayoutEvent("locomotive-set-updated-locomotive-definition")]
		private void locomotiveDefinitionUpdated(LayoutEvent e) {
			locomotiveCollection.Save();
		}

		[LayoutEvent("edit-locomotive-properties")]
		private void editLocomotiveProperties(LayoutEvent e) {
			LocomotiveInfo					loco = (LocomotiveInfo)e.Sender;
			Dialogs.LocomotiveProperties	locoProperties = new Dialogs.LocomotiveProperties(loco);

			if(locoProperties.ShowDialog(this) == DialogResult.OK)
				locomotiveCollection.Save();
		}

		[LayoutEvent("add-new-locomotive-to-collection")]
		private void addNewLocomotiveToCollection(LayoutEvent e) {
			XmlElement locoElement = locomotiveCollection.CollectionDocument.CreateElement("Locomotive");

			locoElement.SetAttribute("ID", XmlConvert.ToString(Guid.NewGuid()));
			locoElement.SetAttribute("Store", XmlConvert.ToString(locomotiveCollection.DefaultStore));

			LocomotiveInfo loco = new LocomotiveInfo(locoElement);
			Dialogs.LocomotiveProperties locoProperties = new Dialogs.LocomotiveProperties(loco);

			if(locoProperties.ShowDialog(this) == DialogResult.OK) {
				locomotiveCollection.CollectionElement.AppendChild(loco.Element);
				locomotiveCollection.Save();
			}
		}

		[LayoutEvent("add-new-train-to-collection")]
		private void addNewTrainToCollection(LayoutEvent e) {
			XmlElement trainInCollectionElement = locomotiveCollection.CollectionDocument.CreateElement("Train");
			TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(trainInCollectionElement);

			trainInCollection.Initialize();

			Tools.Dialogs.TrainProperties trainDialog = new Tools.Dialogs.TrainProperties(trainInCollection);

			if(trainDialog.ShowDialog(this) == DialogResult.OK) {
				locomotiveCollection.CollectionElement.AppendChild(trainInCollectionElement);
				locomotiveCollection.Save();
			}
		}

		[LayoutEvent("edit-locomotive-collection-item")]
		private void editLocomotiveCollectionItem(LayoutEvent e) {
			XmlElement	selectedElement = (XmlElement)e.Sender;

			if(selectedElement != null) {
				if(selectedElement.Name == "Locomotive") {
					LocomotiveInfo				loco = new LocomotiveInfo(selectedElement);
					Dialogs.LocomotiveProperties	locoProperties = new Dialogs.LocomotiveProperties(loco);

					if(locoProperties.ShowDialog(this) == DialogResult.OK)
						locomotiveCollection.Save();
				}
				else if(selectedElement.Name == "Train") {
					Tools.Dialogs.TrainProperties	trainDialog = new Tools.Dialogs.TrainProperties(new TrainInCollectionInfo(selectedElement));

					if(trainDialog.ShowDialog(this) == DialogResult.OK)
						locomotiveCollection.Save();
				}
			}
		}

		[LayoutEvent("delete-locomotive-collection-item")]
		private void deleteLocomotiveCollectionItem(LayoutEvent e) {
			XmlElement	selectedElement = (XmlElement)e.Sender;

			if(selectedElement != null && canDelete(selectedElement)) {
				selectedElement.ParentNode.RemoveChild(selectedElement);
				locomotiveCollection.EnsureReferentialIntegrity();
				locomotiveCollection.Save();
			}
		}

		[LayoutEvent("add-locomotive-collection-editing-context-menu-entries")]
		private void addLocomotiveCollectionEditingContextMenuEntries(LayoutEvent e) {
			XmlElement	clickedElement = (XmlElement)e.Sender;
			Menu		m = (Menu)e.Info;

			if(clickedElement != null) {
				m.MenuItems.Add(new EditLocomotiveCollectionItemPropertiesMenuItem(clickedElement, "&Properties..."));
				m.MenuItems.Add(new DeleteLocomotiveCollectionItemMenuItem(clickedElement, canDelete(clickedElement)));
			}
		}

		[LayoutEvent("add-locomotive-collection-operation-context-menu-entries", Order=100)]
		private void addLocomotiveCollectionOperationContextMenuEntries(LayoutEvent e) {
			XmlElement	clickedElement = (XmlElement)e.Sender;
			Menu		m = (Menu)e.Info;

			if(clickedElement != null) {
				if(m.MenuItems.Count > 0)
					m.MenuItems.Add("-");

				if(clickedElement.Name == "Locomotive")
					m.MenuItems.Add(new EditLocomotiveCollectionItemPropertiesMenuItem(clickedElement, "&Locomotive properties..."));
				m.MenuItems.Add(new DeleteLocomotiveCollectionItemMenuItem(clickedElement, canDelete(clickedElement)));
			}
		}

		#region Context menu items

		class EditLocomotiveCollectionItemPropertiesMenuItem : MenuItem {
			XmlElement			element;

			public EditLocomotiveCollectionItemPropertiesMenuItem(XmlElement element, string menuText) {
				this.element = element;
				Text = menuText;
			}

			protected override void OnClick(EventArgs e) {
				EventManager.Event(new LayoutEvent(element, "edit-locomotive-collection-item"));
			}
		}

		class DeleteLocomotiveCollectionItemMenuItem : MenuItem {
			XmlElement			element;

			public DeleteLocomotiveCollectionItemMenuItem (XmlElement element, bool canDelete) {
				this.element = element;
				Text = "&Delete";

				Enabled = canDelete;
			}

			protected override void OnClick(EventArgs e) {
				EventManager.Event(new LayoutEvent(element, "delete-locomotive-collection-item"));	
			}
		}

		#endregion

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LocomotivesViewer));
			this.contextMenuAdd = new ContextMenu();
			this.menuItemAddLocomotive = new MenuItem();
			this.menuItemAddTrain = new MenuItem();
			this.contextMenuOptions = new ContextMenu();
			this.menuItemArrange = new MenuItem();
			this.menuItemStorage = new MenuItem();
			this.buttonAdd = new Button();
			this.buttonEdit = new Button();
			this.buttonDelete = new Button();
			this.buttonOptions = new Button();
			this.panel1 = new Panel();
			this.buttonClose = new Button();
			this.imageListCloseButton = new ImageList(this.components);
			this.contextMenuArrange = new ContextMenu();
			this.locomotiveList = new LayoutManager.CommonUI.Controls.LocomotiveList();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuAdd
			// 
			this.contextMenuAdd.MenuItems.AddRange(new MenuItem[] {
																						   this.menuItemAddLocomotive,
																						   this.menuItemAddTrain});
			// 
			// menuItemAddLocomotive
			// 
			this.menuItemAddLocomotive.Index = 0;
			this.menuItemAddLocomotive.Text = "&Locomotive...";
			this.menuItemAddLocomotive.Click += new System.EventHandler(this.menuItemAddLocomotive_Click);
			// 
			// menuItemAddTrain
			// 
			this.menuItemAddTrain.Index = 1;
			this.menuItemAddTrain.Text = "&Train...";
			this.menuItemAddTrain.Click += new System.EventHandler(this.menuItemAddTrain_Click);
			// 
			// contextMenuOptions
			// 
			this.contextMenuOptions.MenuItems.AddRange(new MenuItem[] {
																							   this.menuItemArrange,
																							   this.menuItemStorage});
			// 
			// menuItemArrange
			// 
			this.menuItemArrange.Index = 0;
			this.menuItemArrange.Text = "Arrange by";
			// 
			// menuItemStorage
			// 
			this.menuItemStorage.Index = 1;
			this.menuItemStorage.Text = "Storage...";
			this.menuItemStorage.Click += new System.EventHandler(this.menuItemStorage_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAdd.Location = new System.Drawing.Point(12, 464);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(56, 20);
			this.buttonAdd.TabIndex = 5;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonEdit.Location = new System.Drawing.Point(76, 464);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(56, 20);
			this.buttonEdit.TabIndex = 6;
			this.buttonEdit.Text = "&Edit...";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonDelete.Location = new System.Drawing.Point(140, 464);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(56, 20);
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonOptions
			// 
			this.buttonOptions.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonOptions.Location = new System.Drawing.Point(134, 488);
			this.buttonOptions.Name = "buttonOptions";
			this.buttonOptions.Size = new System.Drawing.Size(62, 20);
			this.buttonOptions.TabIndex = 4;
			this.buttonOptions.Text = "&Options...";
			this.buttonOptions.Click += new System.EventHandler(this.buttonOptions_Click);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.buttonClose});
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(208, 20);
			this.panel1.TabIndex = 7;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonClose.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonClose.Image")));
			this.buttonClose.ImageIndex = 0;
			this.buttonClose.ImageList = this.imageListCloseButton;
			this.buttonClose.Location = new System.Drawing.Point(186, 1);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(16, 16);
			this.buttonClose.TabIndex = 0;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// imageListCloseButton
			// 
			this.imageListCloseButton.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListCloseButton.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListCloseButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCloseButton.ImageStream")));
			this.imageListCloseButton.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// locomotiveList
			// 
			this.locomotiveList.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.locomotiveList.ContainerElement = null;
			this.locomotiveList.CurrentListLayout = null;
			this.locomotiveList.CurrentListLayoutIndex = -1;
			this.locomotiveList.DefaultSortField = "Name";
			this.locomotiveList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.locomotiveList.Location = new System.Drawing.Point(4, 24);
			this.locomotiveList.Name = "locomotiveList";
			this.locomotiveList.ShowOnlyLocomotives = false;
			this.locomotiveList.Size = new System.Drawing.Size(196, 432);
			this.locomotiveList.TabIndex = 8;
			this.locomotiveList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.locomotiveList_MouseDown);
			this.locomotiveList.SelectedIndexChanged += new System.EventHandler(this.locomotiveList_SelectedIndexChanged);
			// 
			// LocomotivesViewer
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.locomotiveList,
																		  this.panel1,
																		  this.buttonAdd,
																		  this.buttonEdit,
																		  this.buttonDelete,
																		  this.buttonOptions});
			this.Name = "LocomotivesViewer";
			this.Size = new System.Drawing.Size(208, 512);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e) {
			contextMenuAdd.Show(this, new Point(buttonAdd.Left, buttonAdd.Bottom+2));
		}

		private void menuItemAddLocomotive_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "add-new-locomotive-to-collection"));
		}

		private void buttonOptions_Click(object sender, System.EventArgs e) {
			if(!operationMode)
				contextMenuOptions.Show(this, new Point(buttonOptions.Left, buttonOptions.Bottom));
			else
				contextMenuArrange.Show(this, new Point(buttonOptions.Left, buttonOptions.Bottom));
		}

		private void menuItemStorage_Click(object sender, System.EventArgs e) {
			Dialogs.LocomotiveCollectionStores	stores = new Dialogs.LocomotiveCollectionStores("Locomotive Collection", 
				locomotiveCollection.Element["Stores"], "Locomotive Collection",
				locomotiveCollection.DefaultStoreDirectory, ".LocomotiveCollection");

			stores.Text = "Locomotive Collection Storage";
			locomotiveCollection.Unload();

			stores.ShowDialog(this);
			locomotiveCollection.Load();
			locomotiveCollection.EnsureReferentialIntegrity();

			// Force to re-layout the list
			locomotiveList.ContainerElement = locomotiveCollection.CollectionElement;
			SaveModelDocument();
		}

		private void locomotiveList_SelectedIndexChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			XmlElement	selectedElement = locomotiveList.SelectedXmlElement;

			EventManager.Event(new LayoutEvent(selectedElement, "edit-locomotive-collection-item"));
		}

		private void buttonDelete_Click(object sender, System.EventArgs e) {
			XmlElement	selectedElement = locomotiveList.SelectedXmlElement;

			EventManager.Event(new LayoutEvent(selectedElement, "delete-locomotive-collection-item"));
		}

		private void menuItemAddTrain_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "add-new-train-to-collection"));
		}

		private void locomotiveList_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if((e.Button & MouseButtons.Right) != 0) {
				int		clickedIndex = locomotiveList.IndexFromPoint(e.X, e.Y);

				if(clickedIndex >= 0) {
					IXmlQueryListBoxXmlElementItem	clickedItem = locomotiveList.Items[clickedIndex] as IXmlQueryListBoxXmlElementItem;

					if(clickedItem != null) {
						ContextMenu		m = new ContextMenu();
					
						if(operationMode)
							EventManager.Event(new LayoutEvent(clickedItem.Element, "add-locomotive-collection-operation-context-menu-entries", null, m));
						else
							EventManager.Event(new LayoutEvent(clickedItem.Element, "add-locomotive-collection-editing-context-menu-entries", null, m));

						if(m.MenuItems.Count > 0) {
							Rectangle	itemRect = locomotiveList.GetItemRectangle(clickedIndex);
			
							m.Show(this, new Point(itemRect.Left, (itemRect.Top + itemRect.Bottom) / 2));
						}
					}
				}
			}
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			EventManager.Event(new LayoutEvent(this, "hide-locomotives"));
		}
	}
}
