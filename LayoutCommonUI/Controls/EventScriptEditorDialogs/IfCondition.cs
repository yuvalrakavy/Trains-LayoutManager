using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs
{
	public interface IIfConditionDialogCustomizer {

		string[]	OperatorNames { get; }
		string[]	OperatorDescriptions { get; }
		Type[]		AllowedTypes { get; }
		bool		ValueIsBoolean { get; }

		string		Title { get; }
	}

	/// <summary>
	/// Summary description for IfCondition.
	/// </summary>
	public class IfCondition : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private LayoutManager.CommonUI.Controls.Operand operand1;
		private System.Windows.Forms.GroupBox groupBox2;
		private LayoutManager.CommonUI.Controls.Operand operand2;
		private System.Windows.Forms.ComboBox comboBoxOperation;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		IIfConditionDialogCustomizer	customizer;

		public IfCondition(XmlElement conditionElement, IIfConditionDialogCustomizer customizer)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.customizer = customizer;
			Text = customizer.Title;

			operand1.Element = conditionElement;
			operand1.Suffix = "1";
			operand1.DefaultAccess = "Property";
			operand1.AllowedTypes = customizer.AllowedTypes;
			operand1.ValueIsBoolean = customizer.ValueIsBoolean;

			operand2.Element = conditionElement;
			operand2.Suffix = "2";
			operand2.DefaultAccess = "Value";
			operand2.AllowedTypes = customizer.AllowedTypes;
			operand2.ValueIsBoolean = customizer.ValueIsBoolean;

			operand1.Initialize();
			operand2.Initialize();

			foreach(string operatorDescription in customizer.OperatorDescriptions)
				comboBoxOperation.Items.Add(operatorDescription);
			comboBoxOperation.SelectedIndex = 0;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.operand1 = new LayoutManager.CommonUI.Controls.Operand();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.operand2 = new LayoutManager.CommonUI.Controls.Operand();
			this.comboBoxOperation = new System.Windows.Forms.ComboBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.operand1});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 128);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// operand1
			// 
			this.operand1.AllowedTypes = null;
			this.operand1.DefaultAccess = "Property";
			this.operand1.Element = null;
			this.operand1.Location = new System.Drawing.Point(8, 24);
			this.operand1.Name = "operand1";
			this.operand1.Size = new System.Drawing.Size(184, 96);
			this.operand1.Suffix = "";
			this.operand1.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.operand2});
			this.groupBox2.Location = new System.Drawing.Point(312, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 128);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			// 
			// operand2
			// 
			this.operand2.AllowedTypes = null;
			this.operand2.DefaultAccess = "Property";
			this.operand2.Element = null;
			this.operand2.Location = new System.Drawing.Point(8, 24);
			this.operand2.Name = "operand2";
			this.operand2.Size = new System.Drawing.Size(184, 96);
			this.operand2.Suffix = "";
			this.operand2.TabIndex = 0;
			// 
			// comboBoxOperation
			// 
			this.comboBoxOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOperation.Location = new System.Drawing.Point(216, 64);
			this.comboBoxOperation.Name = "comboBoxOperation";
			this.comboBoxOperation.Size = new System.Drawing.Size(88, 21);
			this.comboBoxOperation.TabIndex = 1;
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(352, 144);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(437, 144);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// IfCondition
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(520, 174);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonOk,
																		  this.comboBoxOperation,
																		  this.groupBox1,
																		  this.groupBox2,
																		  this.buttonCancel});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "IfCondition";
			this.ShowInTaskbar = false;
			this.Text = "If Condition";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(comboBoxOperation.SelectedIndex < 0) {
				MessageBox.Show(this, "You must selection operation", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxOperation.Focus();
				return;
			}

			if(!operand1.ValidateInput() || !operand2.ValidateInput())
				return;

			operand1.Commit();
			operand2.Commit();

			operand1.Element.SetAttribute("Operation", customizer.OperatorNames[comboBoxOperation.SelectedIndex]);
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
