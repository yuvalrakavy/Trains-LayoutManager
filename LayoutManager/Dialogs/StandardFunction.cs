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
	/// Dialog box for creating/editing locomotive function template
	/// </summary>
	public class StandardFunction : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxFunctionType;
		private System.Windows.Forms.TextBox textBoxFunctionName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		XmlElement	functionInfoElement;

		public StandardFunction(XmlElement functionInfoElement)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.functionInfoElement = functionInfoElement;

			if(functionInfoElement.HasAttribute("Type"))
				comboBoxFunctionType.SelectedIndex = (int)Enum.Parse(typeof(LocomotiveFunctionType), functionInfoElement.GetAttribute("Type"));
			else
				comboBoxFunctionType.SelectedIndex = 0;

			if(functionInfoElement.HasAttribute("Name"))
				textBoxFunctionName.Text = functionInfoElement.GetAttribute("Name");

			if(functionInfoElement.HasAttribute("Description"))
				textBoxDescription.Text = functionInfoElement.GetAttribute("Description");
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxFunctionType = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxFunctionName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(23, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxFunctionType
			// 
			this.comboBoxFunctionType.Items.AddRange(new object[] {
																	  "Trigger",
																	  "On/Off"});
			this.comboBoxFunctionType.Location = new System.Drawing.Point(80, 7);
			this.comboBoxFunctionType.Name = "comboBoxFunctionType";
			this.comboBoxFunctionType.Size = new System.Drawing.Size(78, 21);
			this.comboBoxFunctionType.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(23, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Name:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxFunctionName
			// 
			this.textBoxFunctionName.Location = new System.Drawing.Point(80, 38);
			this.textBoxFunctionName.Name = "textBoxFunctionName";
			this.textBoxFunctionName.Size = new System.Drawing.Size(136, 20);
			this.textBoxFunctionName.TabIndex = 2;
			this.textBoxFunctionName.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(65, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Description:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Location = new System.Drawing.Point(80, 69);
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.Size = new System.Drawing.Size(168, 20);
			this.textBoxDescription.TabIndex = 2;
			this.textBoxDescription.Text = "";
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(224, 6);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(64, 23);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(224, 35);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(64, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			// 
			// StandardFunction
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(292, 96);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonOk,
																		  this.textBoxFunctionName,
																		  this.comboBoxFunctionType,
																		  this.label1,
																		  this.label2,
																		  this.label3,
																		  this.textBoxDescription,
																		  this.buttonCancel});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "StandardFunction";
			this.ShowInTaskbar = false;
			this.Text = "Locomotive Function Template";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(textBoxFunctionName.Text.Trim() == "") {
				MessageBox.Show(this, "Missing value", "You have to specifiy function name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxFunctionName.Focus();
				return;
			}

			functionInfoElement.SetAttribute("Type", ((LocomotiveFunctionType)comboBoxFunctionType.SelectedIndex).ToString());
			functionInfoElement.SetAttribute("Name", textBoxFunctionName.Text);
			functionInfoElement.SetAttribute("Description", textBoxDescription.Text);

			DialogResult = DialogResult.OK;
		}
	}
}
