using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LayoutManager.ControlComponents.Dialogs {
    /// <summary>
    /// Summary description for LGBbusDIPswitchSetting.
    /// </summary>
	public partial class KxxDIPswitchSetting : Form, IMarklinControlModuleSettingDialog {
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

        public KxxDIPswitchSetting(string moduleName, int address, bool userActionRequiredFlag) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            diPswitchSettings.Value = KxxDipSwitchSettings[(address - 1) / 4];

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
