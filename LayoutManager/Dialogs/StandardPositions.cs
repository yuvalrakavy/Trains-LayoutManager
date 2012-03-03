using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Summary description for StandardPositions.
	/// </summary>
	public class StandardPositions : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox listBoxPositions;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StandardPositions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Fill the list box
			XmlNode		positions = LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Positions");

			foreach(XmlElement positionElement in positions)
				listBoxPositions.Items.Add(new LayoutPositionInfo(positionElement));
		}

		private bool edit(LayoutPositionInfo positionProvider) {
			Dialogs.StandardPositionProperties	positionProperties = new Dialogs.StandardPositionProperties(positionProvider);

			if(positionProperties.ShowDialog() == DialogResult.OK) {
				positionProperties.Get(positionProvider);
				return true;
			}

			return false;
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
			this.listBoxPositions = new System.Windows.Forms.ListBox();
			this.buttonNew = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listBoxPositions
			// 
			this.listBoxPositions.Location = new System.Drawing.Point(8, 16);
			this.listBoxPositions.Name = "listBoxPositions";
			this.listBoxPositions.Size = new System.Drawing.Size(248, 238);
			this.listBoxPositions.TabIndex = 0;
			this.listBoxPositions.DoubleClick += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonNew
			// 
			this.buttonNew.Location = new System.Drawing.Point(16, 264);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.TabIndex = 1;
			this.buttonNew.Text = "&New";
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Location = new System.Drawing.Point(176, 264);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Location = new System.Drawing.Point(96, 264);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Location = new System.Drawing.Point(176, 296);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "&Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// StandardPositions
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(270, 346);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonClose,
																		  this.buttonDelete,
																		  this.buttonEdit,
																		  this.buttonNew,
																		  this.listBoxPositions});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StandardPositions";
			this.ShowInTaskbar = false;
			this.Text = "Standard Positions";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			LayoutPositionInfo	positionProvider = (LayoutPositionInfo)listBoxPositions.SelectedItem;

			edit(positionProvider);
			listBoxPositions.Items[listBoxPositions.SelectedIndex] = positionProvider;
		}

		private void buttonNew_Click(object sender, System.EventArgs e) {
			XmlElement positionElement = LayoutInfo.CreateProviderElement(LayoutModel.Instance.XmlInfo, "Position", "Positions");
			LayoutPositionInfo	positionProvider = new LayoutPositionInfo(positionElement);

			if(edit(positionProvider))
				listBoxPositions.SelectedIndex = listBoxPositions.Items.Add(positionProvider);
		}

		private void buttonDelete_Click(object sender, System.EventArgs e) {
			if(listBoxPositions.SelectedItem != null) {
				if(MessageBox.Show("Do you really want to delete a style position definition. " +
					"This will cause problems if this position is still being used!", "Warning",
					MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK) {
					LayoutPositionInfo	positionProvider = (LayoutPositionInfo)listBoxPositions.SelectedItem;

					positionProvider.Element.ParentNode.RemoveChild(positionProvider.Element);
					listBoxPositions.Items.Remove(positionProvider);
				}
			}
		}
	}
}
