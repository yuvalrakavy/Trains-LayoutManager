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
	/// Summary description for MotionRampDefinition.
	/// </summary>
	public class MotionRampDefinition : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxName;
		private LayoutManager.CommonUI.Controls.MotionRampEditor motionRampEditor;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxSHowInTrainControllerDialog;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		void endOfDesignerVariables() {}

		MotionRampInfo		ramp;

		public MotionRampDefinition(MotionRampInfo ramp)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.ramp = ramp;

			textBoxName.Text = ramp.Name;
			checkBoxSHowInTrainControllerDialog.Checked = ramp.UseInTrainControllerDialog;
			
			motionRampEditor.Ramp = ramp;
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
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.motionRampEditor = new LayoutManager.CommonUI.Controls.MotionRampEditor();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBoxSHowInTrainControllerDialog = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(48, 17);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(184, 20);
			this.textBoxName.TabIndex = 1;
			this.textBoxName.Text = "";
			// 
			// motionRampEditor
			// 
			this.motionRampEditor.Location = new System.Drawing.Point(-1, 48);
			this.motionRampEditor.Name = "motionRampEditor";
			this.motionRampEditor.Ramp = null;
			this.motionRampEditor.Size = new System.Drawing.Size(218, 98);
			this.motionRampEditor.TabIndex = 2;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.checkBoxSHowInTrainControllerDialog});
			this.groupBox1.Location = new System.Drawing.Point(7, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(208, 56);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Usage:";
			// 
			// checkBoxSHowInTrainControllerDialog
			// 
			this.checkBoxSHowInTrainControllerDialog.Location = new System.Drawing.Point(8, 16);
			this.checkBoxSHowInTrainControllerDialog.Name = "checkBoxSHowInTrainControllerDialog";
			this.checkBoxSHowInTrainControllerDialog.Size = new System.Drawing.Size(193, 32);
			this.checkBoxSHowInTrainControllerDialog.TabIndex = 0;
			this.checkBoxSHowInTrainControllerDialog.Text = "Show in train speed control dialog";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(224, 143);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(68, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(224, 175);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(68, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// MotionRampDefinition
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(296, 206);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonOK,
																		  this.groupBox1,
																		  this.motionRampEditor,
																		  this.textBoxName,
																		  this.label1,
																		  this.buttonCancel});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "MotionRampDefinition";
			this.Text = "Acceleration/Deceleration Profile";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(textBoxName.Text.Trim() == "") {
				MessageBox.Show(this, "You must specify name", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxName.Focus();
				return;
			}

			if(!motionRampEditor.ValidateValues())
				return;

			ramp.Name = textBoxName.Text;
			ramp.UseInTrainControllerDialog = checkBoxSHowInTrainControllerDialog.Checked;
			motionRampEditor.Commit();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
