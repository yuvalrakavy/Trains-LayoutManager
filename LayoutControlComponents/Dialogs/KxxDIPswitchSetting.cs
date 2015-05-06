using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace LayoutManager.ControlComponents.Dialogs
{
	/// <summary>
	/// Summary description for LGBbusDIPswitchSetting.
	/// </summary>
	public class KxxDIPswitchSetting : Form, IMarklinControlModuleSettingDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		LayoutManager.CommonUI.Controls.DIPswitch	diPswitchSettings;
        Label label2;
        CheckBox checkBoxClearUserActionFlag;
        Button buttonClose;

		public static readonly int[] KxxDipSwitchSettings = new int[] {
																0x56, 0x54, 0x59, 0x5a, 0x58, 0x51, 0x52, 0x50,
																0x65, 0x66, 0x64, 0x69, 0x6a, 0x68, 0x61, 0x62,
																0x60, 0x45, 0x46, 0x44, 0x49, 0x4a, 0x48, 0x41,
																0x42, 0x40, 0x95, 0x96, 0x94, 0x99, 0x9a, 0x98,
																0x91, 0x92, 0x90, 0xa5, 0xa6, 0xa4, 0xa9, 0xaa,
																0xa8, 0xa1, 0xa2, 0xa0, 0x85, 0x86, 0x84, 0x89,
																0x8a, 0x88, 0x81, 0x82, 0x80, 0x15, 0x16, 0x14,
																0x19, 0x1a, 0x18, 0x11, 0x12, 0x10, 0x25, 0x26,
		};

		public KxxDIPswitchSetting(string moduleName, int address, bool userActionRequiredFlag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			diPswitchSettings.Value = KxxDipSwitchSettings[(address - 1) / 4];

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
			this.label2 = new Label();
			this.checkBoxClearUserActionFlag = new CheckBox();
			this.buttonClose = new Button();
			this.SuspendLayout();
			// 
			// diPswitchSettings
			// 
			this.diPswitchSettings.Location = new System.Drawing.Point(16, 24);
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
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(224, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "Set the DIP switches as shown below:";
			// 
			// checkBoxClearUserActionFlag
			// 
			this.checkBoxClearUserActionFlag.Location = new System.Drawing.Point(8, 80);
			this.checkBoxClearUserActionFlag.Name = "checkBoxClearUserActionFlag";
			this.checkBoxClearUserActionFlag.Size = new System.Drawing.Size(192, 24);
			this.checkBoxClearUserActionFlag.TabIndex = 7;
			this.checkBoxClearUserActionFlag.Text = "Clear \"User Action Required\" flag";
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(208, 80);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 0;
			this.buttonClose.Text = "Close";
			// 
			// KxxDIPswitchSetting
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 112);
			this.ControlBox = false;
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.checkBoxClearUserActionFlag);
			this.Controls.Add(this.diPswitchSettings);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "KxxDIPswitchSetting";
			this.ShowInTaskbar = false;
			this.Text = "MODULENAME Settings";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
