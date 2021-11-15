using System.Xml;
using System.Diagnostics;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for MotionRampSelector.
    /// </summary>
    public partial class MotionRampSelector : UserControl, IObjectHasXml {
        private XmlElement? element = null;
        private MotionRampInfo? ramp = null;

        public MotionRampSelector() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public string? Role { get; set; }

        public XmlElement Element {
            get => Ensure.NotNull<XmlElement>(OptionalElement, "Element");
            set => OptionalElement = value;
        }

        public XmlElement? OptionalElement {
            get {
                return element;
            }

            set {
                element = value;

                if (element != null) {
                    Debug.Assert(Role != null);

                    Initialize();
                }
            }
        }

        public string Title {
            get {
                return groupBoxRampSelector.Text;
            }

            set {
                groupBoxRampSelector.Text = value;
            }
        }

        public bool ValidateValues() {
            if (checkBoxOverrideDefault.Checked && ramp == null) {
                MessageBox.Show(this, "You have not defined an alternative acceleration profile. " +
                    "Either define a profile (press the 'Override Default' button), or use the default (uncheck the check-mark)", "Invalid Value",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonOverrideDefault.Focus();
                return false;
            }

            return true;
        }

        public bool Commit() {
            if (!ValidateValues())
                return false;

            var rampElement = (XmlElement?)Element.SelectSingleNode("Ramp[@Role='" + Role + "']");

            if (rampElement != null)
                Element.RemoveChild(rampElement);

            if (checkBoxOverrideDefault.Checked && ramp != null) {
                ramp.Role = Role;

                rampElement = (XmlElement)Element.OwnerDocument.ImportNode(ramp.Element, true);
                Element.AppendChild(rampElement);
            }

            return true;
        }

        private void Initialize() {
            var rampElement = (XmlElement?)Element.SelectSingleNode("Ramp[@Role='" + Role + "']");

            if (rampElement == null)
                checkBoxOverrideDefault.Checked = false;
            else {
                checkBoxOverrideDefault.Checked = true;

                XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                rampElement = (XmlElement)doc.ImportNode(rampElement, true);
                doc.AppendChild(rampElement);

                ramp = new MotionRampInfo(rampElement);
                labelRampDescription.Text = ramp.Description;
            }
        }

        private void ButtonOverrideDefault_Click(object? sender, EventArgs e) {
            ramp = new MotionRampInfo(MotionRampType.RateFixed, LayoutModel.Instance.LogicalSpeedSteps / 2);

            var d = new Dialogs.EditMotionRamp(ramp);

            if (d.ShowDialog(this) == DialogResult.OK) {
                checkBoxOverrideDefault.Checked = true;
                labelRampDescription.Text = ramp.Description;
            }
        }

        private void CheckBoxOverrideDefault_Click(object? sender, EventArgs e) {
            if (!checkBoxOverrideDefault.Checked)
                labelRampDescription.Text = "";
            else {
                if (ramp != null)
                    labelRampDescription.Text = ramp.Description;
            }
        }
    }
}
