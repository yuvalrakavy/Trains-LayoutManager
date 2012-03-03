using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for MotionRamps.
	/// </summary>
	public class MotionRamps : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listViewRamps;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.ImageList imageListButttons;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private System.Windows.Forms.ColumnHeader columnHeaderUsage;
		private System.Windows.Forms.Button buttonMoveDown;
		private System.Windows.Forms.Button buttonMoveUp;
		private System.ComponentModel.IContainer components;

		private void endOfDesignerVariables() { }

		MotionRampCollection	ramps;

		public MotionRamps(MotionRampCollection ramps)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.ramps = ramps;

			foreach(MotionRampInfo ramp in ramps)
				listViewRamps.Items.Add(new RampItem(ramp));

			updateButtons(null, null);
		}

		private void updateButtons(object sender, EventArgs e) {
			if(listViewRamps.SelectedItems.Count == 0) {
				buttonEdit.Enabled = false;
				buttonRemove.Enabled = false;
			}
			else {
				buttonEdit.Enabled = true;
				buttonRemove.Enabled = true;
			}

			if(listViewRamps.SelectedItems.Count > 0) {
				int	selectedItemIndex = listViewRamps.SelectedIndices[0];

				buttonMoveUp.Enabled = selectedItemIndex > 0;
				buttonMoveDown.Enabled = selectedItemIndex < listViewRamps.Items.Count - 1;
			}
			else {
				buttonMoveUp.Enabled = false;
				buttonMoveDown.Enabled = false;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MotionRamps));
			this.listViewRamps = new System.Windows.Forms.ListView();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonMoveDown = new System.Windows.Forms.Button();
			this.imageListButttons = new System.Windows.Forms.ImageList(this.components);
			this.buttonMoveUp = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderUsage = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// listViewRamps
			// 
			this.listViewRamps.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listViewRamps.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							this.columnHeaderName,
																							this.columnHeaderUsage});
			this.listViewRamps.FullRowSelect = true;
			this.listViewRamps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewRamps.HideSelection = false;
			this.listViewRamps.Location = new System.Drawing.Point(8, 8);
			this.listViewRamps.MultiSelect = false;
			this.listViewRamps.Name = "listViewRamps";
			this.listViewRamps.Size = new System.Drawing.Size(288, 184);
			this.listViewRamps.TabIndex = 0;
			this.listViewRamps.View = System.Windows.Forms.View.Details;
			this.listViewRamps.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAdd.Location = new System.Drawing.Point(8, 200);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(67, 23);
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "&Add...";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonEdit.Location = new System.Drawing.Point(80, 200);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(67, 23);
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "&Edit...";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRemove.Location = new System.Drawing.Point(152, 200);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(67, 23);
			this.buttonRemove.TabIndex = 3;
			this.buttonRemove.Text = "&Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// buttonMoveDown
			// 
			this.buttonMoveDown.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonMoveDown.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonMoveDown.Image")));
			this.buttonMoveDown.ImageIndex = 0;
			this.buttonMoveDown.ImageList = this.imageListButttons;
			this.buttonMoveDown.Location = new System.Drawing.Point(304, 32);
			this.buttonMoveDown.Name = "buttonMoveDown";
			this.buttonMoveDown.Size = new System.Drawing.Size(24, 20);
			this.buttonMoveDown.TabIndex = 6;
			this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
			// 
			// imageListButttons
			// 
			this.imageListButttons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListButttons.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListButttons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButttons.ImageStream")));
			this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// buttonMoveUp
			// 
			this.buttonMoveUp.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this.buttonMoveUp.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonMoveUp.Image")));
			this.buttonMoveUp.ImageIndex = 1;
			this.buttonMoveUp.ImageList = this.imageListButttons;
			this.buttonMoveUp.Location = new System.Drawing.Point(304, 8);
			this.buttonMoveUp.Name = "buttonMoveUp";
			this.buttonMoveUp.Size = new System.Drawing.Size(24, 20);
			this.buttonMoveUp.TabIndex = 5;
			this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.Location = new System.Drawing.Point(264, 200);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(67, 23);
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Name";
			this.columnHeaderName.Width = 184;
			// 
			// columnHeaderUsage
			// 
			this.columnHeaderUsage.Text = "Usage";
			this.columnHeaderUsage.Width = 100;
			// 
			// MotionRamps
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(336, 230);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonMoveUp,
																		  this.buttonAdd,
																		  this.listViewRamps,
																		  this.buttonEdit,
																		  this.buttonRemove,
																		  this.buttonMoveDown,
																		  this.buttonClose});
			this.Name = "MotionRamps";
			this.Text = "Acceleration/Deceleration Profiles";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e) {
			MotionRampInfo					ramp = new MotionRampInfo(MotionRampType.TimeFixed, 1500);
			Dialogs.MotionRampDefinition	motionRampDefinition = new Dialogs.MotionRampDefinition(ramp);

			if(motionRampDefinition.ShowDialog(this) == DialogResult.OK) {
				ramps.Add(ramp);
				listViewRamps.Items.Add(new RampItem(ramp));
				updateButtons(null, null);
			}
		}

		private RampItem getSelected() {
			if(listViewRamps.SelectedItems.Count == 0)
				return null;
			return (RampItem)listViewRamps.SelectedItems[0];
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			RampItem	selected = getSelected();

			if(selected != null) {
				Dialogs.MotionRampDefinition	motionRampDefinition = new Dialogs.MotionRampDefinition(selected.Ramp);

				if(motionRampDefinition.ShowDialog(this) == DialogResult.OK) {
					selected.Update();
					updateButtons(null, null);
				}
			}
		}

		private void buttonRemove_Click(object sender, System.EventArgs e) {
			RampItem	selected = getSelected();

			if(selected != null) {
				ramps.Remove(selected.Ramp);
				listViewRamps.Items.Remove(selected);
				updateButtons(null, null);
			}
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonMoveUp_Click(object sender, System.EventArgs e) {
			if(listViewRamps.SelectedItems.Count > 0) {
				int				selectedIndex = listViewRamps.SelectedIndices[0];

				if(selectedIndex > 0) {
					RampItem		selected = (RampItem)listViewRamps.SelectedItems[0];
					MotionRampInfo	selectedRamp = selected.Ramp;
					XmlNode			previousElement = selectedRamp.Element.PreviousSibling;

					selectedRamp.Element.ParentNode.RemoveChild(selectedRamp.Element);
					previousElement.ParentNode.InsertBefore(selectedRamp.Element, previousElement);

					listViewRamps.Items.Remove(selected);
					listViewRamps.Items.Insert(selectedIndex-1, selected);
				}
			}
		}

		private void buttonMoveDown_Click(object sender, System.EventArgs e) {
			if(listViewRamps.SelectedItems.Count > 0) {
				int				selectedIndex = listViewRamps.SelectedIndices[0];

				if(selectedIndex < listViewRamps.Items.Count - 1) {
					RampItem		selected = (RampItem)listViewRamps.SelectedItems[0];
					MotionRampInfo	selectedRamp= selected.Ramp;
					XmlNode			nextElement = selectedRamp.Element.NextSibling;

					selectedRamp.Element.ParentNode.RemoveChild(selectedRamp.Element);
					nextElement.ParentNode.InsertAfter(selectedRamp.Element, nextElement);

					listViewRamps.Items.Remove(selected);
					listViewRamps.Items.Insert(selectedIndex+1, selected);
				}
			}
		
		}

		class RampItem : ListViewItem {
			MotionRampInfo	ramp;

			public RampItem(MotionRampInfo ramp) {
				this.ramp = ramp;

				SubItems.Add("");
				Update();
			}

			public void Update() {
				Text = ramp.Description;

				if(ramp.UseInTrainControllerDialog)
					SubItems[1].Text = "Train Controller";
				else
					SubItems[1].Text = "";
			}

			public MotionRampInfo Ramp {
				get {
					return ramp;
				}
			}
		}
	}
}
