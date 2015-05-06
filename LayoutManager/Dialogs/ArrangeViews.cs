using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.View;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Summary description for ArrangeAreas.
	/// </summary>
	public class ArrangeViews : Form {
		private Button buttonMoveDown;
		private ImageList imageListButtons;
		private Button buttonMoveUp;
		private Button buttonClose;
		private Button buttonNew;
		private Button buttonDelete;
		private Button buttonRename;
		private IContainer components;
		private ListBox listBoxViews;

		LayoutFrameWindowAreaTabPage	areaPage;
		bool							rebuildTabs = false;
		LayoutFrameWindow				controller;

		public ArrangeViews(LayoutFrameWindow controller, LayoutFrameWindowAreaTabPage areaPage)
		{
			this.controller = controller;
			this.areaPage = areaPage;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			foreach(LayoutFrameWindowAreaViewTabPage viewPage in areaPage.TabViews.TabPages)
				listBoxViews.Items.Add(viewPage);

			updateButtons();
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

		private void updateButtons() {
			int		selectedViewIndex = listBoxViews.SelectedIndex;

			if(selectedViewIndex == -1) {
				buttonDelete.Enabled = false;
				buttonRename.Enabled = false;
				buttonMoveUp.Enabled = false;
				buttonMoveDown.Enabled = false;
			}
			else {
				buttonDelete.Enabled = true;
				buttonRename.Enabled = true;

				buttonMoveUp.Enabled = selectedViewIndex > 0;
				buttonMoveDown.Enabled = selectedViewIndex < listBoxViews.Items.Count - 1;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ArrangeViews));
			this.listBoxViews = new ListBox();
			this.buttonMoveDown = new Button();
			this.imageListButtons = new ImageList(this.components);
			this.buttonMoveUp = new Button();
			this.buttonClose = new Button();
			this.buttonNew = new Button();
			this.buttonDelete = new Button();
			this.buttonRename = new Button();
			this.SuspendLayout();
			// 
			// listBoxViews
			// 
			this.listBoxViews.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listBoxViews.DisplayMember = "Text";
			this.listBoxViews.Location = new System.Drawing.Point(8, 16);
			this.listBoxViews.Name = "listBoxViews";
			this.listBoxViews.Size = new System.Drawing.Size(216, 212);
			this.listBoxViews.TabIndex = 0;
			this.listBoxViews.SelectedIndexChanged += new System.EventHandler(this.listBoxViews_SelectedIndexChanged);
			// 
			// buttonMoveDown
			// 
			this.buttonMoveDown.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonMoveDown.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonMoveDown.Image")));
			this.buttonMoveDown.ImageIndex = 1;
			this.buttonMoveDown.ImageList = this.imageListButtons;
			this.buttonMoveDown.Location = new System.Drawing.Point(230, 48);
			this.buttonMoveDown.Name = "buttonMoveDown";
			this.buttonMoveDown.Size = new System.Drawing.Size(32, 23);
			this.buttonMoveDown.TabIndex = 2;
			this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
			// 
			// imageListButtons
			// 
			this.imageListButtons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListButtons.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListButtons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButtons.ImageStream")));
			this.imageListButtons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// buttonMoveUp
			// 
			this.buttonMoveUp.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonMoveUp.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonMoveUp.Image")));
			this.buttonMoveUp.ImageIndex = 0;
			this.buttonMoveUp.ImageList = this.imageListButtons;
			this.buttonMoveUp.Location = new System.Drawing.Point(230, 16);
			this.buttonMoveUp.Name = "buttonMoveUp";
			this.buttonMoveUp.Size = new System.Drawing.Size(32, 23);
			this.buttonMoveUp.TabIndex = 1;
			this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.Location = new System.Drawing.Point(152, 272);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(72, 23);
			this.buttonClose.TabIndex = 6;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// buttonNew
			// 
			this.buttonNew.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonNew.Location = new System.Drawing.Point(8, 240);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(72, 24);
			this.buttonNew.TabIndex = 3;
			this.buttonNew.Text = "New...";
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonDelete.Location = new System.Drawing.Point(80, 240);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(72, 24);
			this.buttonDelete.TabIndex = 4;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonRename
			// 
			this.buttonRename.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRename.Location = new System.Drawing.Point(152, 240);
			this.buttonRename.Name = "buttonRename";
			this.buttonRename.Size = new System.Drawing.Size(72, 24);
			this.buttonRename.TabIndex = 5;
			this.buttonRename.Text = "Rename...";
			this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
			// 
			// ArrangeViews
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(264, 302);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonRename,
																		  this.buttonDelete,
																		  this.buttonNew,
																		  this.buttonClose,
																		  this.buttonMoveUp,
																		  this.buttonMoveDown,
																		  this.listBoxViews});
			this.Name = "ArrangeViews";
			this.ShowInTaskbar = false;
			this.Text = "Arrange Views";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonMoveDown_Click(object sender, System.EventArgs e) {
			int		selectedViewIndex = listBoxViews.SelectedIndex;

			if(selectedViewIndex != -1 && selectedViewIndex < listBoxViews.Items.Count-1) {
				LayoutFrameWindowAreaViewTabPage	movedView = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

				listBoxViews.Items.Remove(movedView);
				listBoxViews.Items.Insert(selectedViewIndex+1, movedView);
				listBoxViews.SelectedItem = movedView;
				rebuildTabs = true;
			}
		}

		private void buttonMoveUp_Click(object sender, System.EventArgs e) {
			int		selectedViewIndex = listBoxViews.SelectedIndex;
		
			if(selectedViewIndex != -1 && selectedViewIndex > 0) {
				LayoutFrameWindowAreaViewTabPage	movedView = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

				listBoxViews.Items.Remove(movedView);
				listBoxViews.Items.Insert(selectedViewIndex-1, movedView);
				listBoxViews.SelectedItem = movedView;
				rebuildTabs = true;
			}
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			if(rebuildTabs) {
				areaPage.TabViews.TabPages.Clear();

				foreach(LayoutFrameWindowAreaViewTabPage viewPage in listBoxViews.Items)
					areaPage.TabViews.TabPages.Add(viewPage);
			}

			this.Close();
		}

		private void listBoxViews_SelectedIndexChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void buttonRename_Click(object sender, System.EventArgs e) {
			int		selectedIndex = listBoxViews.SelectedIndex;

			if(selectedIndex != -1) {
				LayoutFrameWindowAreaViewTabPage	selectedView = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

				Dialogs.GetViewName	getViewName = new Dialogs.GetViewName();

				if(getViewName.ShowDialog(this) == DialogResult.OK) {
					selectedView.Text = getViewName.ViewName;

					listBoxViews.Items[selectedIndex] = selectedView;
				}
			}
		}

		private void buttonNew_Click(object sender, System.EventArgs e) {
			LayoutView	newView = controller.AddNewView();

			foreach(LayoutFrameWindowAreaViewTabPage viewPage in areaPage.TabViews.TabPages) {
				if(viewPage.View == newView) {
					listBoxViews.Items.Add(viewPage);
					break;
				}
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e) {
			LayoutFrameWindowAreaViewTabPage	selectedViewPage = (LayoutFrameWindowAreaViewTabPage)listBoxViews.SelectedItem;

			if(selectedViewPage != null) {
				areaPage.TabViews.TabPages.Remove(selectedViewPage);
				listBoxViews.Items.Remove(selectedViewPage);
				updateButtons();
			}
		}
	}
}
