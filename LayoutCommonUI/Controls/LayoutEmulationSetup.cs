using LayoutManager.Components;
using MethodDispatcher;
using System.Xml;


namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LayoutEmulationSetup.
    /// </summary>
    public partial class LayoutEmulationSetup : UserControl {
        private XmlElement? element;

        public LayoutEmulationSetup() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public XmlElement? Element {
            get {
                return element;
            }

            set {
                if (value != null) {
                    element = value;
                    Initialize();
                }
            }
        }

        public void Initialize() {
            var emulationServices = Dispatch.Call.GetLayoutEmulationServices();

            if (emulationServices != null) {
                int tickTime = (int?)Element.AttributeValue(LayoutCommandStationComponent.A_EmulationTickTime) ?? 200;
                textBoxEmulationTickTime.Text = tickTime.ToString();
                checkBoxEmulateTrainMotion.Checked = (bool?)Element.AttributeValue(LayoutCommandStationComponent.A_EmulateTrainMotion) ?? true;
                checkBoxAnimateTrainMotion.Checked = (bool?)Element.AttributeValue(LayoutCommandStationComponent.A_AnimateTrainMotion) ?? false;
            }
            else
                groupBoxEmulationServices.Visible = false;

            UpdateControls();
        }

        private void UpdateControls() {
            checkBoxAnimateTrainMotion.Enabled = checkBoxEmulateTrainMotion.Checked;
            labelEmulationTickTime.Enabled = checkBoxEmulateTrainMotion.Checked;
            textBoxEmulationTickTime.Enabled = checkBoxEmulateTrainMotion.Checked;
        }

        public bool ValidateInput() {
            try {
                if (checkBoxEmulateTrainMotion.Checked)
                    int.Parse(textBoxEmulationTickTime.Text);
            }
            catch (FormatException) {
                MessageBox.Show(this, "Invalid emulation tick time value", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxEmulationTickTime.Focus();
                return false;
            }

            return true;
        }

        public bool Commit() {
            if (!ValidateInput())
                return false;

            Element?.SetAttributeValue(LayoutCommandStationComponent.A_EmulationTickTime, int.Parse(textBoxEmulationTickTime.Text));
            Element?.SetAttributeValue(LayoutCommandStationComponent.A_EmulateTrainMotion, checkBoxEmulateTrainMotion.Checked);
            Element?.SetAttributeValue(LayoutCommandStationComponent.A_AnimateTrainMotion, checkBoxAnimateTrainMotion.Checked);

            return true;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }


        private void CheckBoxEmulateTrainMotion_CheckedChanged(object? sender, EventArgs e) {
            UpdateControls();
        }
    }
}
