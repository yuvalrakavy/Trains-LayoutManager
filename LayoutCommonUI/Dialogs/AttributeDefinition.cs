using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for AttributeDefinition.
    /// </summary>
    public class AttributeDefinition : Form {
		private Label label1;
		private GroupBox groupBox1;
		private Label label2;
		private ComboBox comboBoxName;
		private RadioButton radioButtonTypeString;
		private RadioButton radioButtonTypeNumber;
		private RadioButton radioButtonTypeBoolean;
		private Panel panelTextValue;
		private TextBox textBoxValue;
		private GroupBox groupBoxBooleanValue;
		private RadioButton radioButtonValueTrue;
		private RadioButton radioButtonValueFalse;
		private Button buttonCancel;
		private Button buttonOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		Controls.ICheckIfNameUsed	checkName;
		string						originalAttributeName;
		Type						attributesSource;
		IDictionary					attributesMap = new HybridDictionary();
		ArrayList					attributesList = null;

		public AttributeDefinition(Type attributeSource, Controls.ICheckIfNameUsed checkName, string attributeName, object attributeValue)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.checkName = checkName;
			this.originalAttributeName = attributeName;
			this.attributesSource = attributeSource;

			if(attributeName != null)
				comboBoxName.Text = attributeName;

			if(attributeValue != null) {
				if(attributeValue is bool) {
					radioButtonTypeBoolean.Checked = true;

					if((bool)attributeValue)
						radioButtonValueTrue.Checked = true;
					else
						radioButtonValueFalse.Checked = true;
				}
				else if(attributeValue is int) {
					radioButtonTypeNumber.Checked = true;
					textBoxValue.Text= attributeValue.ToString();
				}
				else if(attributeValue is string) {
					radioButtonTypeString.Checked = true;
					textBoxValue.Text = (string)attributeValue;
				}
				else
					throw new ArgumentException("Attribute value has invalid type: " + attributeValue.GetType().Name);
			}
			else {
				radioButtonTypeString.Checked = true;
				radioButtonValueTrue.Checked = true;
			}

			updateButtons(null, null);
		}

        #region Properties

        public string AttributeName => comboBoxName.Text;

        public object Value {
			get {
				if(radioButtonTypeBoolean.Checked)
					return (bool)radioButtonValueTrue.Checked;
				else if(radioButtonTypeNumber.Checked)
					return int.Parse(textBoxValue.Text);
				else
					return (string)textBoxValue.Text;
			}
		}

		#endregion

		private void updateButtons(object sender, EventArgs e) {
			if(radioButtonTypeBoolean.Checked) {
				panelTextValue.Visible = false;
				groupBoxBooleanValue.Visible = true;
			}
			else {
				panelTextValue.Visible = true;
				groupBoxBooleanValue.Visible = false;
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
			this.label1 = new Label();
			this.comboBoxName = new ComboBox();
			this.groupBox1 = new GroupBox();
			this.radioButtonTypeString = new RadioButton();
			this.radioButtonTypeNumber = new RadioButton();
			this.radioButtonTypeBoolean = new RadioButton();
			this.panelTextValue = new Panel();
			this.textBoxValue = new TextBox();
			this.label2 = new Label();
			this.groupBoxBooleanValue = new GroupBox();
			this.radioButtonValueTrue = new RadioButton();
			this.radioButtonValueFalse = new RadioButton();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.groupBox1.SuspendLayout();
			this.panelTextValue.SuspendLayout();
			this.groupBoxBooleanValue.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxName
			// 
			this.comboBoxName.Location = new System.Drawing.Point(72, 17);
			this.comboBoxName.Name = "comboBoxName";
			this.comboBoxName.Size = new System.Drawing.Size(160, 21);
			this.comboBoxName.Sorted = true;
			this.comboBoxName.TabIndex = 1;
			this.comboBoxName.DropDown += new System.EventHandler(this.comboBoxName_DropDown);
			this.comboBoxName.SelectionChangeCommitted += new System.EventHandler(this.comboBoxName_SelectionChangeCommitted);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.radioButtonTypeString,
																					this.radioButtonTypeNumber,
																					this.radioButtonTypeBoolean});
			this.groupBox1.Location = new System.Drawing.Point(22, 56);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(160, 80);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Type:";
			// 
			// radioButtonTypeString
			// 
			this.radioButtonTypeString.Location = new System.Drawing.Point(16, 16);
			this.radioButtonTypeString.Name = "radioButtonTypeString";
			this.radioButtonTypeString.Size = new System.Drawing.Size(104, 20);
			this.radioButtonTypeString.TabIndex = 0;
			this.radioButtonTypeString.Text = "Text (string)";
			this.radioButtonTypeString.CheckedChanged += new System.EventHandler(this.updateButtons);
			// 
			// radioButtonTypeNumber
			// 
			this.radioButtonTypeNumber.Location = new System.Drawing.Point(16, 36);
			this.radioButtonTypeNumber.Name = "radioButtonTypeNumber";
			this.radioButtonTypeNumber.Size = new System.Drawing.Size(112, 20);
			this.radioButtonTypeNumber.TabIndex = 0;
			this.radioButtonTypeNumber.Text = "Number (integer)";
			this.radioButtonTypeNumber.CheckedChanged += new System.EventHandler(this.updateButtons);
			// 
			// radioButtonTypeBoolean
			// 
			this.radioButtonTypeBoolean.Location = new System.Drawing.Point(16, 56);
			this.radioButtonTypeBoolean.Name = "radioButtonTypeBoolean";
			this.radioButtonTypeBoolean.Size = new System.Drawing.Size(136, 20);
			this.radioButtonTypeBoolean.TabIndex = 0;
			this.radioButtonTypeBoolean.Text = "Boolean (True/False)";
			this.radioButtonTypeBoolean.CheckedChanged += new System.EventHandler(this.updateButtons);
			// 
			// panelTextValue
			// 
			this.panelTextValue.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.textBoxValue,
																						 this.label2});
			this.panelTextValue.Location = new System.Drawing.Point(22, 143);
			this.panelTextValue.Name = "panelTextValue";
			this.panelTextValue.Size = new System.Drawing.Size(202, 32);
			this.panelTextValue.TabIndex = 3;
			// 
			// textBoxValue
			// 
			this.textBoxValue.Location = new System.Drawing.Point(56, 6);
			this.textBoxValue.Name = "textBoxValue";
			this.textBoxValue.Size = new System.Drawing.Size(136, 20);
			this.textBoxValue.TabIndex = 2;
			this.textBoxValue.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 5);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Value:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxBooleanValue
			// 
			this.groupBoxBooleanValue.Controls.AddRange(new System.Windows.Forms.Control[] {
																							   this.radioButtonValueTrue,
																							   this.radioButtonValueFalse});
			this.groupBoxBooleanValue.Location = new System.Drawing.Point(22, 143);
			this.groupBoxBooleanValue.Name = "groupBoxBooleanValue";
			this.groupBoxBooleanValue.Size = new System.Drawing.Size(152, 56);
			this.groupBoxBooleanValue.TabIndex = 4;
			this.groupBoxBooleanValue.TabStop = false;
			this.groupBoxBooleanValue.Text = "Value:";
			this.groupBoxBooleanValue.Visible = false;
			// 
			// radioButtonValueTrue
			// 
			this.radioButtonValueTrue.Location = new System.Drawing.Point(8, 12);
			this.radioButtonValueTrue.Name = "radioButtonValueTrue";
			this.radioButtonValueTrue.Size = new System.Drawing.Size(64, 20);
			this.radioButtonValueTrue.TabIndex = 1;
			this.radioButtonValueTrue.Text = "True";
			// 
			// radioButtonValueFalse
			// 
			this.radioButtonValueFalse.Location = new System.Drawing.Point(8, 32);
			this.radioButtonValueFalse.Name = "radioButtonValueFalse";
			this.radioButtonValueFalse.Size = new System.Drawing.Size(64, 20);
			this.radioButtonValueFalse.TabIndex = 1;
			this.radioButtonValueFalse.Text = "False";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(232, 176);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(56, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(232, 144);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(56, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// AttributeDefinition
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(296, 206);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBoxBooleanValue,
																		  this.panelTextValue,
																		  this.groupBox1,
																		  this.comboBoxName,
																		  this.label1,
																		  this.buttonCancel,
																		  this.buttonOK});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AttributeDefinition";
			this.Text = "Attribute Definition";
			this.groupBox1.ResumeLayout(false);
			this.panelTextValue.ResumeLayout(false);
			this.groupBoxBooleanValue.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(comboBoxName.Text.Trim() == "") {
				MessageBox.Show(this, "You must specify a name for the attribute", "Missing Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxName.Focus();
				return;
			}

			if(comboBoxName.Text != originalAttributeName && checkName.IsUsed(comboBoxName.Text)) {
				MessageBox.Show(this, "Another attribute already has that name", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxName.Focus();
				return;
			}

			if(radioButtonTypeNumber.Checked) {
				try {
					int.Parse(textBoxValue.Text);
				} catch(FormatException) {
					MessageBox.Show(this, "Value is not a valid number", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBoxValue.Focus();
					return;
				}
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void comboBoxName_DropDown(object sender, System.EventArgs e) {
			if(attributesList == null) {
				attributesList = new ArrayList();
				EventManager.Event(new LayoutEvent(attributesSource, "get-object-attributes", null, attributesList));
			}

			foreach(AttributesInfo attributes in attributesList) {
				foreach(AttributeInfo attribute in attributes) {
					if(!checkName.IsUsed(attribute.Name) && !attributesMap.Contains(attribute.Name)) {
						attributesMap.Add(attribute.Name, attribute);
						comboBoxName.Items.Add(attribute.Name);
					}
				}
			}
		}

		private void comboBoxName_SelectionChangeCommitted(object sender, System.EventArgs e) {
			AttributeInfo	attribute = (AttributeInfo)attributesMap[comboBoxName.SelectedItem];

			if(attribute != null) {
				switch(attribute.AttributeType) {
					case AttributeType.Boolean:
						radioButtonTypeBoolean.Checked = true;
						break;

					case AttributeType.Number:
						radioButtonTypeNumber.Checked = true;
						break;

					case AttributeType.String:
						radioButtonTypeString.Checked = true;
						break;
				}

				updateButtons(null, null);
			}
		}
	}
}
