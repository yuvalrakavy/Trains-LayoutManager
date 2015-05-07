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
	/// Summary description for LocomotiveFunction.
	/// </summary>
	public class LocomotiveFunction : Form {
		private Label label1;
		private Label label2;
		private NumericUpDown numericUpDownFunctionNumber;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private Button buttonOk;
		private Button buttonCancel;
		private ComboBox comboBoxFunctionName;
		private Label label3;
		private Label label4;
		private TextBox textBoxFunctionDescription;
		private ComboBox comboBoxFunctionType;

		private void endOfDesignerVariables() { }

		LocomotiveCatalogInfo	catalog;
		XmlElement					functionsElement;
		XmlElement					functionElement;
		bool						typeChanged = false;
		bool						descriptionChanged = false;

		public LocomotiveFunction(LocomotiveCatalogInfo catalog, XmlElement functionsElement, XmlElement functionElement)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.catalog = catalog;
			this.functionsElement = functionsElement;
			this.functionElement = functionElement;

			LocomotiveFunctionInfo	function = new LocomotiveFunctionInfo(functionElement);

			numericUpDownFunctionNumber.Value = function.Number;
			comboBoxFunctionName.Text = function.Name;
			textBoxFunctionDescription.Text = function.Description;
			comboBoxFunctionType.SelectedIndex = (int)function.Type;

			foreach(XmlElement f in catalog.LocomotiveFunctionNames)
				comboBoxFunctionName.Items.Add(f.GetAttribute("Name"));

			typeChanged = false;
			descriptionChanged = false;
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
			this.numericUpDownFunctionNumber = new NumericUpDown();
			this.label2 = new Label();
			this.buttonOk = new Button();
			this.buttonCancel = new Button();
			this.comboBoxFunctionName = new ComboBox();
			this.label3 = new Label();
			this.comboBoxFunctionType = new ComboBox();
			this.label4 = new Label();
			this.textBoxFunctionDescription = new TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownFunctionNumber)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Function number:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownFunctionNumber
			// 
			this.numericUpDownFunctionNumber.Location = new System.Drawing.Point(112, 17);
			this.numericUpDownFunctionNumber.Name = "numericUpDownFunctionNumber";
			this.numericUpDownFunctionNumber.Size = new System.Drawing.Size(40, 20);
			this.numericUpDownFunctionNumber.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(112, 112);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.TabIndex = 8;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(192, 112);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			// 
			// comboBoxFunctionName
			// 
			this.comboBoxFunctionName.Location = new System.Drawing.Point(112, 48);
			this.comboBoxFunctionName.Name = "comboBoxFunctionName";
			this.comboBoxFunctionName.Size = new System.Drawing.Size(160, 21);
			this.comboBoxFunctionName.TabIndex = 5;
			this.comboBoxFunctionName.SelectionChangeCommitted += new System.EventHandler(this.comboBoxFunctionName_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(156, 19);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 16);
			this.label3.TabIndex = 2;
			this.label3.Text = "Type:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxFunctionType
			// 
			this.comboBoxFunctionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFunctionType.Items.AddRange(new object[] {
																	  "Trigger",
																	  "On/Off"});
			this.comboBoxFunctionType.Location = new System.Drawing.Point(194, 16);
			this.comboBoxFunctionType.Name = "comboBoxFunctionType";
			this.comboBoxFunctionType.Size = new System.Drawing.Size(78, 21);
			this.comboBoxFunctionType.TabIndex = 3;
			this.comboBoxFunctionType.SelectedIndexChanged += new System.EventHandler(this.comboBoxFunctionType_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 16);
			this.label4.TabIndex = 6;
			this.label4.Text = "Description:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxFunctionDescription
			// 
			this.textBoxFunctionDescription.Location = new System.Drawing.Point(80, 80);
			this.textBoxFunctionDescription.Name = "textBoxFunctionDescription";
			this.textBoxFunctionDescription.Size = new System.Drawing.Size(192, 20);
			this.textBoxFunctionDescription.TabIndex = 7;
			this.textBoxFunctionDescription.Text = "";
			this.textBoxFunctionDescription.TextChanged += new System.EventHandler(this.textBoxFunctionDescription_TextChanged);
			// 
			// LocomotiveFunction
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(274, 144);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.textBoxFunctionDescription,
																		  this.comboBoxFunctionType,
																		  this.comboBoxFunctionName,
																		  this.buttonOk,
																		  this.numericUpDownFunctionNumber,
																		  this.label1,
																		  this.label2,
																		  this.buttonCancel,
																		  this.label3,
																		  this.label4});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LocomotiveFunction";
			this.ShowInTaskbar = false;
			this.Text = "Locomotive Function";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownFunctionNumber)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e) {
			foreach(XmlElement fElement in functionsElement) {
				LocomotiveFunctionInfo	f = new LocomotiveFunctionInfo(fElement);

				if(fElement != functionElement && f.Number == numericUpDownFunctionNumber.Value) {
					MessageBox.Show(this, "Function number " + numericUpDownFunctionNumber.Value + " is already used", "Duplicate function number",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					numericUpDownFunctionNumber.Focus();
					return;
				}

				if(fElement != functionElement && f.Name == comboBoxFunctionName.Text) {
					MessageBox.Show(this, "Function name '" + comboBoxFunctionName.Text + "' is already used", "Duplicate function name",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					comboBoxFunctionName.Focus();
					return;
				}
			}

			LocomotiveFunctionInfo	function = new LocomotiveFunctionInfo(functionElement);

			function.Number = (int)numericUpDownFunctionNumber.Value;
			function.Type = (LocomotiveFunctionType)comboBoxFunctionType.SelectedIndex;
			function.Name = comboBoxFunctionName.Text;
			function.Description = textBoxFunctionDescription.Text;

			if(comboBoxFunctionName.FindStringExact(comboBoxFunctionName.Text) < 0) {
				XmlElement	element = catalog.Element.OwnerDocument.CreateElement("Function");

				element.SetAttribute("Name", comboBoxFunctionName.Text);
				element.SetAttribute("Description", textBoxFunctionDescription.Text);
				element.SetAttribute("Type", ((LocomotiveFunctionType)comboBoxFunctionType.SelectedIndex).ToString());
				catalog.LocomotiveFunctionNames.AppendChild(element);
			}

			DialogResult = DialogResult.OK;
		}

		private void comboBoxFunctionType_SelectedIndexChanged(object sender, System.EventArgs e) {
			typeChanged = true;
		}

		private void textBoxFunctionDescription_TextChanged(object sender, System.EventArgs e) {
			descriptionChanged = true;
		}

		private void comboBoxFunctionName_SelectionChangeCommitted(object sender, System.EventArgs e) {
			XmlElement	functionInfoElement = (XmlElement)catalog.LocomotiveFunctionNames.GetElementsByTagName("Function")[comboBoxFunctionName.SelectedIndex];

			if(!typeChanged) {
				comboBoxFunctionType.SelectedIndex = (int)Enum.Parse(typeof(LocomotiveFunctionType), functionInfoElement.GetAttribute("Type"));
				typeChanged = false;
			}

			if(!descriptionChanged) {
				textBoxFunctionDescription.Text = functionInfoElement.GetAttribute("Description");
				descriptionChanged = false;
			}
		}
	}
}
