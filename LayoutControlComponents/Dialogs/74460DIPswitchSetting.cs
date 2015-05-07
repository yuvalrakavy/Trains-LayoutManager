using System.Text.RegularExpressions;

namespace LayoutManager.ControlComponents.Dialogs {
    /// <summary>
    /// Summary description for LGBbusDIPswitchSetting.
    /// </summary>
    public class TurnoutDecoderDIPswitchSetting : System.Windows.Forms.Form, IMarklinControlModuleSettingDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		LayoutManager.CommonUI.Controls.DIPswitch	diPswitchSettings;
		System.Windows.Forms.Label	label2;
		System.Windows.Forms.CheckBox	checkBoxClearUserActionFlag;
		System.Windows.Forms.Button	buttonClose;

		public TurnoutDecoderDIPswitchSetting(string moduleName, int address, bool userActionRequiredFlag)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			diPswitchSettings.Value = KxxDIPswitchSetting.KxxDipSwitchSettings[(address - 1) / 4] | ((((address - 1)& 3) ^ 0x3) << 8);

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
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxClearUserActionFlag = new System.Windows.Forms.CheckBox();
			this.buttonClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// diPswitchSettings
			// 
			this.diPswitchSettings.Location = new System.Drawing.Point(16, 24);
			this.diPswitchSettings.LSBonRight = false;
			this.diPswitchSettings.Name = "diPswitchSettings";
			this.diPswitchSettings.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.diPswitchSettings.Size = new System.Drawing.Size(184, 40);
			this.diPswitchSettings.SwitchCount = 10;
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
			// TurnoutDecoderDIPswitchSetting
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
			this.Name = "TurnoutDecoderDIPswitchSetting";
			this.ShowInTaskbar = false;
			this.Text = "MODULENAME Settings";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
