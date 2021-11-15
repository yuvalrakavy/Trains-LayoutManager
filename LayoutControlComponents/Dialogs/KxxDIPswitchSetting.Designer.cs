using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LayoutManager.ControlComponents.Dialogs {
    /// <summary>
    /// Summary description for LGBbusDIPswitchSetting.
    /// </summary>
	partial class KxxDIPswitchSetting : Form, IMarklinControlModuleSettingDialog {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
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
            this.diPswitchSettings.Value = (long)1;
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
            this.AutoScaleMode = AutoScaleMode.Font;
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
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private LayoutManager.CommonUI.Controls.DIPswitch diPswitchSettings;
        private Label label2;
        private CheckBox checkBoxClearUserActionFlag;
        private Button buttonClose;
    }
}

