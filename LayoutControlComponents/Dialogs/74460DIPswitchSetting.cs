using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LayoutManager.ControlComponents.Dialogs {
    /// <summary>
    /// Summary description for LGBbusDIPswitchSetting.
    /// </summary>
	public partial class TurnoutDecoderDIPswitchSetting : Form, IMarklinControlModuleSettingDialog {
        public TurnoutDecoderDIPswitchSetting(string moduleName, int address, bool userActionRequiredFlag) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            diPswitchSettings.Value = KxxDIPswitchSetting.KxxDipSwitchSettings[(address - 1) / 4] | ((((address - 1) & 3) ^ 0x3) << 8);

            if (userActionRequiredFlag)
                checkBoxClearUserActionFlag.Checked = true;
            else
                checkBoxClearUserActionFlag.Visible = false;

            Text = Regex.Replace(Text, "MODULENAME", moduleName);
        }

        public bool ClearUserActionRequiredFlag => checkBoxClearUserActionFlag.Checked;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
