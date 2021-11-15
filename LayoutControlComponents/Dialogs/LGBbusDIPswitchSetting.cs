using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LayoutManager.ControlComponents.Dialogs {
    /// <summary>
    /// Summary description for LGBbusDIPswitchSetting.
    /// </summary>
	public partial class LGBbusDIPswitchSetting : Form {
        public LGBbusDIPswitchSetting(string moduleName, int address, bool userActionRequiredFlag) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            diPswitchSettings.Value = 0x80 | (address - 1) / 4;

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
