using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Summary description for StandardFunctions.
	/// </summary>
	public class StandardFunctions : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listViewFunctionInfo;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.ColumnHeader columnHeaderType;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private System.Windows.Forms.ColumnHeader columnHeaderDescription;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		XmlElement		commonFunctionsElement;

		public StandardFunctions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			commonFunctionsElement = LayoutModel.LocomotiveCatalog.LocomotiveFunctionNames;

			foreach(XmlElement f in commonFunctionsElement)
				listViewFunctionInfo.Items.Add(new StandardFunctionItem(f));

			updateButtons();
		}

		private void updateButtons() {
			if(listViewFunctionInfo.SelectedItems.Count > 0) {
				buttonEdit.Enabled = true;
				buttonDelete.Enabled = true;
			}
			else {
				buttonEdit.Enabled = false;
				buttonDelete.Enabled = false;
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
			this.listViewFunctionInfo = new System.Windows.Forms.ListView();
			this.buttonNew = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.columnHeaderType = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderDescription = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// listViewFunctionInfo
			// 
			this.listViewFunctionInfo.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listViewFunctionInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								   this.columnHeaderType,
																								   this.columnHeaderName,
																								   this.columnHeaderDescription});
			this.listViewFunctionInfo.FullRowSelect = true;
			this.listViewFunctionInfo.GridLines = true;
			this.listViewFunctionInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewFunctionInfo.HideSelection = false;
			this.listViewFunctionInfo.Location = new System.Drawing.Point(8, 8);
			this.listViewFunctionInfo.MultiSelect = false;
			this.listViewFunctionInfo.Name = "listViewFunctionInfo";
			this.listViewFunctionInfo.Size = new System.Drawing.Size(312, 176);
			this.listViewFunctionInfo.TabIndex = 0;
			this.listViewFunctionInfo.View = System.Windows.Forms.View.Details;
			this.listViewFunctionInfo.SelectedIndexChanged += new System.EventHandler(this.listViewFunctionInfo_SelectedIndexChanged);
			// 
			// buttonNew
			// 
			this.buttonNew.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonNew.Location = new System.Drawing.Point(8, 188);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(70, 23);
			this.buttonNew.TabIndex = 1;
			this.buttonNew.Text = "&New";
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonEdit.Location = new System.Drawing.Point(83, 188);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(70, 23);
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonDelete.Location = new System.Drawing.Point(159, 188);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(70, 23);
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.Location = new System.Drawing.Point(250, 188);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(70, 23);
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// columnHeaderType
			// 
			this.columnHeaderType.Text = "Type";
			this.columnHeaderType.Width = 50;
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Name";
			this.columnHeaderName.Width = 90;
			// 
			// columnHeaderDescription
			// 
			this.columnHeaderDescription.Text = "Description";
			this.columnHeaderDescription.Width = 168;
			// 
			// StandardFunctions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 222);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonClose,
																		  this.buttonNew,
																		  this.listViewFunctionInfo,
																		  this.buttonEdit,
																		  this.buttonDelete});
			this.MinimumSize = new System.Drawing.Size(336, 256);
			this.Name = "StandardFunctions";
			this.ShowInTaskbar = false;
			this.Text = "Common Locomotive Functions";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonNew_Click(object sender, System.EventArgs e) {
			XmlElement			f = commonFunctionsElement.OwnerDocument.CreateElement("Function");
			StandardFunction	d = new StandardFunction(f);

			if(d.ShowDialog(this) == DialogResult.OK) {
				commonFunctionsElement.AppendChild(f);
				listViewFunctionInfo.Items.Add(new StandardFunctionItem(f));
			}
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			if(listViewFunctionInfo.SelectedItems.Count > 0) {
				StandardFunctionItem	item = (StandardFunctionItem)listViewFunctionInfo.SelectedItems[0];

				item.Edit();
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e) {
			if(listViewFunctionInfo.SelectedItems.Count > 0) {
				StandardFunctionItem	item = (StandardFunctionItem)listViewFunctionInfo.SelectedItems[0];

				item.Element.ParentNode.RemoveChild(item.Element);
				listViewFunctionInfo.Items.Remove(item);
			}

			updateButtons();
		}

		private void listViewFunctionInfo_SelectedIndexChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}

		class StandardFunctionItem : ListViewItem {

			XmlElement	element;


			public StandardFunctionItem(XmlElement element) {
				this.element = element;

				this.Text = getTypeString();
				this.SubItems.Add(element.GetAttribute("Name"));
				this.SubItems.Add(element.GetAttribute("Description"));
			}

			public XmlElement Element {
				get {
					return element;
				}
			}

			public void Edit() {
				StandardFunction	d = new StandardFunction(element);

				if(d.ShowDialog(this.ListView.Parent) == DialogResult.OK) {
					this.Text = getTypeString();
					this.SubItems[1].Text = element.GetAttribute("Name");
					this.SubItems[2].Text = element.GetAttribute("Description");
				}
			}

			private String getTypeString() {
				if(element.HasAttribute("Type")) {
					LocomotiveFunctionType	type = (LocomotiveFunctionType)Enum.Parse(typeof(LocomotiveFunctionType), element.GetAttribute("Type"));

					if(type == LocomotiveFunctionType.OnOff)
						return "On/Off";
					else
						return type.ToString();
				}
				else
					return "";
			}
		}
	}
}
