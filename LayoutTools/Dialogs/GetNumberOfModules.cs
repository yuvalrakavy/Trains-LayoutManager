using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for GetNumberOfModules.
	/// </summary>
	public class GetNumberOfModules : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelQuestion;
		private System.Windows.Forms.NumericUpDown numericUpDownModuleCount;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GetNumberOfModules(string moduleName, int maxModules)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			labelQuestion.Text = Regex.Replace(labelQuestion.Text, "MODULENAME", moduleName);
			Text = Regex.Replace(Text, "MODULENAME", moduleName);

			numericUpDownModuleCount.Maximum = maxModules;
		}

		public int Count {
			get {
				return (int)numericUpDownModuleCount.Value;
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
			this.labelQuestion = new System.Windows.Forms.Label();
			this.numericUpDownModuleCount = new System.Windows.Forms.NumericUpDown();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleCount)).BeginInit();
			this.SuspendLayout();
			// 
			// labelQuestion
			// 
			this.labelQuestion.Location = new System.Drawing.Point(8, 8);
			this.labelQuestion.Name = "labelQuestion";
			this.labelQuestion.Size = new System.Drawing.Size(296, 20);
			this.labelQuestion.TabIndex = 0;
			this.labelQuestion.Text = "How many MODULENAME would you like to add:";
			this.labelQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownModuleCount
			// 
			this.numericUpDownModuleCount.Location = new System.Drawing.Point(304, 9);
			this.numericUpDownModuleCount.Minimum = new System.Decimal(new int[] {
																					 1,
																					 0,
																					 0,
																					 0});
			this.numericUpDownModuleCount.Name = "numericUpDownModuleCount";
			this.numericUpDownModuleCount.Size = new System.Drawing.Size(57, 20);
			this.numericUpDownModuleCount.TabIndex = 1;
			this.numericUpDownModuleCount.Value = new System.Decimal(new int[] {
																				   1,
																				   0,
																				   0,
																				   0});
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(208, 40);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(286, 40);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			// 
			// GetNumberOfModules
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(368, 70);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.numericUpDownModuleCount);
			this.Controls.Add(this.labelQuestion);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GetNumberOfModules";
			this.Text = "Add MODULENAME";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleCount)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
		}
	}
}
