using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace GetMarklinCatalog
{
	/// <summary>
	/// Summary description for EditTranslations.
	/// </summary>
	public class EditTranslations : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listViewTranslations;
		private System.Windows.Forms.ColumnHeader columnHeaderFrom;
		private System.Windows.Forms.ColumnHeader columnHeaderTo;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		XmlElement	translationsElement;

		public EditTranslations(XmlElement translationsElement)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.translationsElement = translationsElement;

			foreach(XmlElement translateElement in translationsElement)
				listViewTranslations.Items.Add(new TranslateItem(translateElement));
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
			this.listViewTranslations = new System.Windows.Forms.ListView();
			this.columnHeaderFrom = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderTo = new System.Windows.Forms.ColumnHeader();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listViewTranslations
			// 
			this.listViewTranslations.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listViewTranslations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								   this.columnHeaderFrom,
																								   this.columnHeaderTo});
			this.listViewTranslations.FullRowSelect = true;
			this.listViewTranslations.GridLines = true;
			this.listViewTranslations.HideSelection = false;
			this.listViewTranslations.Location = new System.Drawing.Point(8, 8);
			this.listViewTranslations.MultiSelect = false;
			this.listViewTranslations.Name = "listViewTranslations";
			this.listViewTranslations.Size = new System.Drawing.Size(288, 280);
			this.listViewTranslations.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewTranslations.TabIndex = 0;
			this.listViewTranslations.View = System.Windows.Forms.View.Details;
			// 
			// columnHeaderFrom
			// 
			this.columnHeaderFrom.Text = "From";
			this.columnHeaderFrom.Width = 155;
			// 
			// columnHeaderTo
			// 
			this.columnHeaderTo.Text = "To";
			this.columnHeaderTo.Width = 121;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAdd.Location = new System.Drawing.Point(8, 296);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "Add...";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonEdit.Location = new System.Drawing.Point(88, 296);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.TabIndex = 1;
			this.buttonEdit.Text = "Edit...";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonDelete.Location = new System.Drawing.Point(168, 296);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.TabIndex = 1;
			this.buttonDelete.Text = "Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonClose.Location = new System.Drawing.Point(224, 328);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// EditTranslations
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(304, 358);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonAdd,
																		  this.listViewTranslations,
																		  this.buttonEdit,
																		  this.buttonDelete,
																		  this.buttonClose});
			this.Name = "EditTranslations";
			this.Text = "Edit Translations";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e) {
			XmlElement	translateElement = translationsElement.OwnerDocument.CreateElement("Translate");
			EditTranslation	d = new EditTranslation(translateElement);

			if(d.ShowDialog(this) == DialogResult.OK) {
				translationsElement.AppendChild(translateElement);
				listViewTranslations.Items.Add(new TranslateItem(translateElement));
			}
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			if(listViewTranslations.SelectedItems.Count > 0) {
				TranslateItem	selected = (TranslateItem)listViewTranslations.SelectedItems[0];
				EditTranslation	d = new EditTranslation(selected.Element);

				d.ShowDialog(this);
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e) {
			if(listViewTranslations.SelectedItems.Count > 0) {
				TranslateItem	selected = (TranslateItem)listViewTranslations.SelectedItems[0];

				translationsElement.RemoveChild(selected.Element);
				listViewTranslations.Items.Remove(selected);
			}
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		class TranslateItem : ListViewItem {

			XmlElement	translateElement;

			public TranslateItem(XmlElement translateElement) {
				this.translateElement = translateElement;

				SubItems.Add("");
				Update();
			}

			public void Update() {
				Text = translateElement.GetAttribute("From");
				SubItems[1].Text = translateElement.GetAttribute("To");
			}

			public XmlElement Element {
				get {
					return translateElement;
				}
			}
		}
	}
}
