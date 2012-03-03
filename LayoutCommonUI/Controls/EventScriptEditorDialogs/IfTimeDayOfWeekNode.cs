using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs
{
	/// <summary>
	/// Summary description for IfTimeNumericNode.
	/// </summary>
	public class IfTimeDayOfWeekNode : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RadioButton radioButtonValue;
		private System.Windows.Forms.RadioButton radioButtonRange;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ComboBox comboBoxFrom;
		private System.Windows.Forms.ComboBox comboBoxTo;
		private System.Windows.Forms.ComboBox comboBoxValue;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		IIfTimeNode		node;

		public IfTimeDayOfWeekNode(string title, IIfTimeNode node)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.node = node;

			Text = title;

			if(node.IsRange) {
				radioButtonRange.Checked = true;
				comboBoxFrom.SelectedIndex = node.From;
				comboBoxTo.SelectedIndex = node.To;
			}
			else {
				radioButtonValue.Checked = true;
				comboBoxValue.SelectedIndex = node.Value;
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
			this.radioButtonValue = new System.Windows.Forms.RadioButton();
			this.radioButtonRange = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.comboBoxValue = new System.Windows.Forms.ComboBox();
			this.comboBoxFrom = new System.Windows.Forms.ComboBox();
			this.comboBoxTo = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// radioButtonValue
			// 
			this.radioButtonValue.Location = new System.Drawing.Point(8, 16);
			this.radioButtonValue.Name = "radioButtonValue";
			this.radioButtonValue.Size = new System.Drawing.Size(56, 24);
			this.radioButtonValue.TabIndex = 0;
			this.radioButtonValue.Text = "Value:";
			// 
			// radioButtonRange
			// 
			this.radioButtonRange.Location = new System.Drawing.Point(8, 48);
			this.radioButtonRange.Name = "radioButtonRange";
			this.radioButtonRange.Size = new System.Drawing.Size(56, 24);
			this.radioButtonRange.TabIndex = 2;
			this.radioButtonRange.Text = "From:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(130, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(24, 23);
			this.label1.TabIndex = 4;
			this.label1.Text = "To:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(61, 83);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(143, 83);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// comboBoxValue
			// 
			this.comboBoxValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxValue.Items.AddRange(new object[] {
															   "Sun",
															   "Mon",
															   "Tue",
															   "Wed",
															   "Thu",
															   "Fri",
															   "Sat"});
			this.comboBoxValue.Location = new System.Drawing.Point(64, 18);
			this.comboBoxValue.Name = "comboBoxValue";
			this.comboBoxValue.Size = new System.Drawing.Size(64, 21);
			this.comboBoxValue.TabIndex = 8;
			this.comboBoxValue.DropDown += new System.EventHandler(this.comboBoxValue_Changed);
			this.comboBoxValue.SelectedIndexChanged += new System.EventHandler(this.comboBoxValue_Changed);
			// 
			// comboBoxFrom
			// 
			this.comboBoxFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFrom.Items.AddRange(new object[] {
															  "Sun",
															  "Mon",
															  "Tue",
															  "Wed",
															  "Thu",
															  "Fri",
															  "Sat"});
			this.comboBoxFrom.Location = new System.Drawing.Point(64, 50);
			this.comboBoxFrom.Name = "comboBoxFrom";
			this.comboBoxFrom.Size = new System.Drawing.Size(64, 21);
			this.comboBoxFrom.TabIndex = 8;
			this.comboBoxFrom.SelectedIndexChanged += new System.EventHandler(this.comboBoxFromOrTo_Changed);
			// 
			// comboBoxTo
			// 
			this.comboBoxTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTo.Items.AddRange(new object[] {
															"Sun",
															"Mon",
															"Tue",
															"Wed",
															"Thu",
															"Fri",
															"Sat"});
			this.comboBoxTo.Location = new System.Drawing.Point(154, 50);
			this.comboBoxTo.Name = "comboBoxTo";
			this.comboBoxTo.Size = new System.Drawing.Size(64, 21);
			this.comboBoxTo.TabIndex = 8;
			this.comboBoxTo.SelectedIndexChanged += new System.EventHandler(this.comboBoxFromOrTo_Changed);
			// 
			// IfTimeDayOfWeekNode
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(226, 110);
			this.ControlBox = false;
			this.Controls.Add(this.comboBoxValue);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.radioButtonRange);
			this.Controls.Add(this.radioButtonValue);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.comboBoxFrom);
			this.Controls.Add(this.comboBoxTo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "IfTimeDayOfWeekNode";
			this.ShowInTaskbar = false;
			this.Text = "IfDayOfWeekNode";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(radioButtonRange.Checked) {
				if(comboBoxFrom.SelectedIndex > comboBoxTo.SelectedIndex) {
					MessageBox.Show(this, "Invalid range, \"from\" is larger than \"to\"", "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
					comboBoxFrom.Focus();
					return;
				}

				node.From = comboBoxFrom.SelectedIndex;
				node.To = comboBoxTo.SelectedIndex;
				comboBoxFrom.Focus();
			}
			else
				node.Value = comboBoxValue.SelectedIndex;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void comboBoxValue_Changed(object sender, System.EventArgs e) {
			radioButtonValue.Checked = true;
		}

		private void comboBoxFromOrTo_Changed(object sender, System.EventArgs e) {
			radioButtonRange.Checked = true;
		
		}
	}
}
