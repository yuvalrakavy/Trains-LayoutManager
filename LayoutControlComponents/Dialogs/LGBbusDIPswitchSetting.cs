using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using LayoutManager.CommonUI;

namespace LayoutManager.ControlComponents.Dialogs
{
	/// <summary>
	/// Summary description for LGBbusDIPswitchSetting.
	/// </summary>
	public class LGBbusDIPswitchSetting : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		LayoutManager.CommonUI.Controls.DIPswitch	diPswitchSettings;
		System.Windows.Forms.Label	label1;
		System.Windows.Forms.Label	label2;
		System.Windows.Forms.Label	label3;
		System.Windows.Forms.Label	label4;
		LayoutManager.CommonUI.Controls.DIPswitch	diPswitch2;
		System.Windows.Forms.Label	label5;
		System.Windows.Forms.Label	label6;
		System.Windows.Forms.Label	label7;
		System.Windows.Forms.Label	label8;
		System.Windows.Forms.Label	label9;
		System.Windows.Forms.Label	label10;
		System.Windows.Forms.CheckBox	checkBoxClearUserActionFlag;
		System.Windows.Forms.Button	buttonClose;

		public LGBbusDIPswitchSetting(string moduleName, int address, bool userActionRequiredFlag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			diPswitchSettings.Value = 0x80 | (address - 1) / 4;

			if(userActionRequiredFlag)
				checkBoxClearUserActionFlag.Checked = true;
			else
				checkBoxClearUserActionFlag.Visible = false;

			Text = Regex.Replace(Text, "MODULENAME", moduleName);
		}

		public bool ClearUserActionRequiredFlag {
			get {
				return checkBoxClearUserActionFlag.Checked;
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
			this.diPswitchSettings = new LayoutManager.CommonUI.Controls.DIPswitch();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.diPswitch2 = new LayoutManager.CommonUI.Controls.DIPswitch();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.checkBoxClearUserActionFlag = new System.Windows.Forms.CheckBox();
			this.buttonClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// diPswitchSettings
			// 
			this.diPswitchSettings.Location = new System.Drawing.Point(48, 74);
			this.diPswitchSettings.LSBonRight = false;
			this.diPswitchSettings.Name = "diPswitchSettings";
			this.diPswitchSettings.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.diPswitchSettings.Size = new System.Drawing.Size(152, 40);
			this.diPswitchSettings.SwitchCount = 8;
			this.diPswitchSettings.SwitchCountBase = 1;
			this.diPswitchSettings.TabIndex = 0;
			this.diPswitchSettings.Text = "diPswitch1";
			this.diPswitchSettings.Value = ((long)(1));
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(224, 32);
			this.label1.TabIndex = 1;
			this.label1.Text = "Disconnect the phone style connector (LGB Bus connector)";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(48, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(224, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "Set the DIP switches as shown below:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(48, 120);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(224, 32);
			this.label3.TabIndex = 1;
			this.label3.Text = "Connect the module to the Centeral Station. The LED will flash rapidly.";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(48, 152);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(224, 32);
			this.label4.TabIndex = 1;
			this.label4.Text = "Disconnect cable from centeral station and set all DIP switches to 0:";
			// 
			// diPswitch2
			// 
			this.diPswitch2.Location = new System.Drawing.Point(48, 184);
			this.diPswitch2.LSBonRight = false;
			this.diPswitch2.Name = "diPswitch2";
			this.diPswitch2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.diPswitch2.Size = new System.Drawing.Size(152, 40);
			this.diPswitch2.SwitchCount = 8;
			this.diPswitch2.SwitchCountBase = 1;
			this.diPswitch2.TabIndex = 0;
			this.diPswitch2.Text = "diPswitch1";
			this.diPswitch2.Value = ((long)(0));
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(48, 232);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(224, 24);
			this.label5.TabIndex = 1;
			this.label5.Text = "Reconnect cable to the centeral station";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label6.Location = new System.Drawing.Point(16, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(24, 23);
			this.label6.TabIndex = 2;
			this.label6.Text = "1";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label7.Location = new System.Drawing.Point(16, 56);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(24, 23);
			this.label7.TabIndex = 3;
			this.label7.Text = "2";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label8.Location = new System.Drawing.Point(16, 120);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(24, 23);
			this.label8.TabIndex = 4;
			this.label8.Text = "3";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label9.Location = new System.Drawing.Point(16, 152);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(24, 23);
			this.label9.TabIndex = 5;
			this.label9.Text = "4";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label10.Location = new System.Drawing.Point(16, 232);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(24, 23);
			this.label10.TabIndex = 6;
			this.label10.Text = "5";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// checkBoxClearUserActionFlag
			// 
			this.checkBoxClearUserActionFlag.Location = new System.Drawing.Point(8, 264);
			this.checkBoxClearUserActionFlag.Name = "checkBoxClearUserActionFlag";
			this.checkBoxClearUserActionFlag.Size = new System.Drawing.Size(192, 24);
			this.checkBoxClearUserActionFlag.TabIndex = 7;
			this.checkBoxClearUserActionFlag.Text = "Clear \"User Action Required\" flag";
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(208, 264);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 0;
			this.buttonClose.Text = "Close";
			// 
			// LGBbusDIPswitchSetting
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 294);
			this.ControlBox = false;
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.checkBoxClearUserActionFlag);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.diPswitchSettings);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.diPswitch2);
			this.Controls.Add(this.label5);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LGBbusDIPswitchSetting";
			this.ShowInTaskbar = false;
			this.Text = "MODULENAME Settings";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
