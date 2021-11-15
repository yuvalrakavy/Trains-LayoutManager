using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackContactProperties.
    /// </summary>
    public partial class SignalProperties : Form, ILayoutComponentPropertiesDialog {
        public SignalProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.XmlInfo = new LayoutXmlInfo(component);

            LayoutSignalComponentInfo info = new(XmlInfo.Element);

            switch (info.SignalType) {
                case LayoutSignalType.Distance:
                    radioButtonSignalTypeDistance.Checked = true;
                    break;

                case LayoutSignalType.Lights:
                    radioButtonSignalTypeLights.Checked = true;
                    break;

                case LayoutSignalType.Semaphore:
                    radioButtonSignalTypeSemaphore.Checked = true;
                    break;
            }

            checkBoxReverseLogic.Checked = info.ReverseLogic;
        }

        public LayoutXmlInfo XmlInfo { get; }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            LayoutSignalComponentInfo info = new(XmlInfo.Element);

            if (radioButtonSignalTypeDistance.Checked)
                info.SignalType = LayoutSignalType.Distance;
            else if (radioButtonSignalTypeLights.Checked)
                info.SignalType = LayoutSignalType.Lights;
            else if (radioButtonSignalTypeSemaphore.Checked)
                info.SignalType = LayoutSignalType.Semaphore;

            info.ReverseLogic = checkBoxReverseLogic.Checked;

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
