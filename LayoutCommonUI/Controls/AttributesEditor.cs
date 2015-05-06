using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {

	public interface ICheckIfNameUsed {
		bool IsUsed(string name);
	}

	/// <summary>
	/// Summary description for AttributesEditor.
	/// </summary>
	public class AttributesEditor : System.Windows.Forms.UserControl, ICheckIfNameUsed, IControlSupportViewOnly {
		private ListView listViewAttributes;
		private Button buttonAdd;
		private Button buttonEdit;
		private Button buttonRemove;
		private ColumnHeader columnHeaderName;
		private ColumnHeader columnHeaderType;
		private ColumnHeader columnHeaderValue;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		void endOfDesignerVariables() { }

		IObjectHasAttributes		attributesOwner = null;
		Type						attributesSource = null;
		ListViewStringColumnsSorter	sorter;
		bool						viewOnly = false;

		public AttributesEditor() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			sorter = new ListViewStringColumnsSorter(listViewAttributes);
		}

		public Type AttributesSource {
			get {
				return attributesSource;
			}

			set {
				attributesSource = value;
			}
		}

		public IObjectHasAttributes AttributesOwner {
			get {
				return attributesOwner;
			}

			set {
				if(value != null) {
					attributesOwner = value;
					initialize();
				}
				else
					listViewAttributes.Items.Clear();
			}
		}

		public AttributesInfo Attributes {
			get {
				return attributesOwner.Attributes;
			}
		}

		public bool ViewOnly {
			get {
				return viewOnly;
			}

			set {
				if(value == true && viewOnly)
					throw new ArgumentException("Cannot change from view only to not view only");

				viewOnly = value;

				if(viewOnly) {
					buttonAdd.Visible = false;
					buttonEdit.Visible = false;
					buttonRemove.Visible = false;

					SuspendLayout();

					listViewAttributes.Size = new Size(listViewAttributes.Width, buttonAdd.Bottom - listViewAttributes.Top);

					ResumeLayout();
				}
			}
		}

		#region Operations

		public bool Commit() {
			if(listViewAttributes.Items.Count > 0 || attributesOwner.HasAttributes) {
				attributesOwner.Attributes.Clear();

				foreach(AttributeItem item in listViewAttributes.Items)
					attributesOwner.Attributes[item.AttributeName] = item.Value;
			}

			return true;
		}

		#endregion

		#region methods

		private void initialize() {
			listViewAttributes.Items.Clear();

			if(attributesOwner.HasAttributes) {
				foreach(AttributeInfo attribute in Attributes)
					listViewAttributes.Items.Add(new AttributeItem(attribute.Name, attribute.Value));
			}

			updateButtons(null, null);
		}

		private void updateButtons(object sender, EventArgs e) {
			if(listViewAttributes.SelectedItems.Count > 0) {
				buttonEdit.Enabled = true;
				buttonRemove.Enabled = true;
			}
			else {
				buttonEdit.Enabled = false;
				buttonRemove.Enabled = false;
			}
		}

		public bool IsUsed(string name) {
			foreach(AttributeItem item in listViewAttributes.Items)
				if(item.AttributeName == name)
					return true;
			return false;
		}

		AttributeItem getSelected() {
			if(listViewAttributes.SelectedItems.Count == 0)
				return null;
			else
				return (AttributeItem)listViewAttributes.SelectedItems[0];
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
			this.listViewAttributes = new ListView();
			this.columnHeaderName = new ColumnHeader();
			this.columnHeaderType = new ColumnHeader();
			this.columnHeaderValue = new ColumnHeader();
			this.buttonAdd = new Button();
			this.buttonEdit = new Button();
			this.buttonRemove = new Button();
			this.SuspendLayout();
			// 
			// listViewAttributes
			// 
			this.listViewAttributes.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listViewAttributes.Columns.AddRange(new ColumnHeader[] {
																								 this.columnHeaderName,
																								 this.columnHeaderType,
																								 this.columnHeaderValue});
			this.listViewAttributes.FullRowSelect = true;
			this.listViewAttributes.GridLines = true;
			this.listViewAttributes.Location = new System.Drawing.Point(8, 8);
			this.listViewAttributes.MultiSelect = false;
			this.listViewAttributes.Name = "listViewAttributes";
			this.listViewAttributes.Size = new System.Drawing.Size(240, 160);
			this.listViewAttributes.TabIndex = 0;
			this.listViewAttributes.View = System.Windows.Forms.View.Details;
			this.listViewAttributes.DoubleClick += new System.EventHandler(this.buttonEdit_Click);
			this.listViewAttributes.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Name";
			this.columnHeaderName.Width = 90;
			// 
			// columnHeaderType
			// 
			this.columnHeaderType.Text = "Type";
			this.columnHeaderType.Width = 56;
			// 
			// columnHeaderValue
			// 
			this.columnHeaderValue.Text = "Value";
			this.columnHeaderValue.Width = 90;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAdd.Location = new System.Drawing.Point(8, 176);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(56, 20);
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonEdit.Location = new System.Drawing.Point(72, 176);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(56, 20);
			this.buttonEdit.TabIndex = 1;
			this.buttonEdit.Text = "&Edit";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRemove.Location = new System.Drawing.Point(136, 176);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(56, 20);
			this.buttonRemove.TabIndex = 1;
			this.buttonRemove.Text = "&Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// AttributesEditor
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonAdd,
																		  this.listViewAttributes,
																		  this.buttonEdit,
																		  this.buttonRemove});
			this.Name = "AttributesEditor";
			this.Size = new System.Drawing.Size(256, 200);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e) {
			Dialogs.AttributeDefinition	d = new Dialogs.AttributeDefinition(attributesSource, this, null, null);

			if(d.ShowDialog(this) == DialogResult.OK) {
				listViewAttributes.Items.Add(new AttributeItem(d.AttributeName, d.Value));
				updateButtons(null, null);
			}
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			AttributeItem	selected = getSelected();

			if(selected != null && !viewOnly) {
				Dialogs.AttributeDefinition	d = new Dialogs.AttributeDefinition(attributesSource, this, selected.AttributeName, selected.Value);

				if(d.ShowDialog(this) == DialogResult.OK) {
					selected.AttributeName = d.AttributeName;
					selected.Value = d.Value;
				}
				updateButtons(null, null);
			}


		}

		private void buttonRemove_Click(object sender, System.EventArgs e) {
			AttributeItem	selected = getSelected();

			if(selected != null) {
				listViewAttributes.Items.Remove(selected);
				updateButtons(null, null);
			}
		}

		#region Item class

		class AttributeItem : ListViewItem {
			object	attributeValue;

			public AttributeItem(string name, object attributeValue) {
				Text = name;

				SubItems.Add("");
				SubItems.Add("");

				this.attributeValue = attributeValue;

				Update();
			}

			public string AttributeName {
				get {
					return Text;
				}

				set {
					Text = value;
				}
			}

			public object Value {
				get {
					return attributeValue;
				}

				set {
					attributeValue = value;
					Update();
				}
			}

			public void Update() {
				string	typeName;
				string	valueString;

				if(attributeValue is string) {
					typeName = "Text";
					valueString = (string)attributeValue;
				}
				else if(attributeValue is bool) {
					typeName = "Boolean";
					valueString = attributeValue.ToString();
				}
				else if(attributeValue is int) {
					typeName = "Number";
					valueString = attributeValue.ToString();
				}
				else
					throw new ApplicationException("Attribute value has non-supported type");

				SubItems[1].Text = typeName;
				SubItems[2].Text = valueString;
			}
		}

		#endregion
	}
}
