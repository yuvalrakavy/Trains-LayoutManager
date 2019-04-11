using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using LayoutManager.Model;

#nullable enable
namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for MotionRampSelector.
    /// </summary>
    public class MotionRampSelector : System.Windows.Forms.UserControl, IObjectHasXml {
        private GroupBox groupBoxRampSelector;
        private Label labelRampDescription;
        private Button buttonOverrideDefault;
        private CheckBox checkBoxOverrideDefault;

        /// <summary> 
        /// Required designer variable.
        /// </summary>

        private XmlElement? element = null;
        private MotionRampInfo? ramp = null;

        #pragma warning disable nullable
        public MotionRampSelector() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
#pragma warning restore nullable

        public string Role { get; set; }

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

                    initialize();
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

            XmlElement rampElement = (XmlElement)Element.SelectSingleNode("Ramp[@Role='" + Role + "']");

            if (rampElement != null)
                Element.RemoveChild(rampElement);

            if (checkBoxOverrideDefault.Checked) {
                ramp.Role = Role;

                rampElement = (XmlElement)Element.OwnerDocument.ImportNode(ramp.Element, true);
                Element.AppendChild(rampElement);
            }

            return true;
        }

        private void initialize() {
            XmlElement rampElement = (XmlElement)Element.SelectSingleNode("Ramp[@Role='" + Role + "']");

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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBoxRampSelector = new GroupBox();
            this.checkBoxOverrideDefault = new CheckBox();
            this.buttonOverrideDefault = new Button();
            this.labelRampDescription = new Label();
            this.groupBoxRampSelector.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxRampSelector
            // 
            this.groupBoxRampSelector.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                               this.checkBoxOverrideDefault,
                                                                                               this.buttonOverrideDefault,
                                                                                               this.labelRampDescription});
            this.groupBoxRampSelector.Location = new System.Drawing.Point(4, 0);
            this.groupBoxRampSelector.Name = "groupBoxRampSelector";
            this.groupBoxRampSelector.Size = new System.Drawing.Size(264, 41);
            this.groupBoxRampSelector.TabIndex = 0;
            this.groupBoxRampSelector.TabStop = false;
            this.groupBoxRampSelector.Text = "Acceleration Profile:";
            // 
            // checkBoxOverrideDefault
            // 
            this.checkBoxOverrideDefault.Location = new System.Drawing.Point(5, 18);
            this.checkBoxOverrideDefault.Name = "checkBoxOverrideDefault";
            this.checkBoxOverrideDefault.Size = new System.Drawing.Size(16, 16);
            this.checkBoxOverrideDefault.TabIndex = 4;
            this.checkBoxOverrideDefault.Click += new System.EventHandler(this.checkBoxOverrideDefault_Click);
            // 
            // buttonOverrideDefault
            // 
            this.buttonOverrideDefault.Location = new System.Drawing.Point(21, 16);
            this.buttonOverrideDefault.Name = "buttonOverrideDefault";
            this.buttonOverrideDefault.Size = new System.Drawing.Size(97, 20);
            this.buttonOverrideDefault.TabIndex = 3;
            this.buttonOverrideDefault.Text = "&Override Default";
            this.buttonOverrideDefault.Click += new System.EventHandler(this.buttonOverrideDefault_Click);
            // 
            // labelRampDescription
            // 
            this.labelRampDescription.Location = new System.Drawing.Point(124, 16);
            this.labelRampDescription.Name = "labelRampDescription";
            this.labelRampDescription.Size = new System.Drawing.Size(132, 16);
            this.labelRampDescription.TabIndex = 2;
            // 
            // MotionRampSelector
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.groupBoxRampSelector});
            this.Name = "MotionRampSelector";
            this.Size = new System.Drawing.Size(272, 41);
            this.groupBoxRampSelector.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOverrideDefault_Click(object sender, System.EventArgs e) {
            ramp = new MotionRampInfo(MotionRampType.RateFixed, LayoutModel.Instance.LogicalSpeedSteps / 2);

            Dialogs.EditMotionRamp d = new Dialogs.EditMotionRamp(ramp);

            if (d.ShowDialog(this) == DialogResult.OK) {
                checkBoxOverrideDefault.Checked = true;
                labelRampDescription.Text = ramp.Description;
            }
        }

        private void checkBoxOverrideDefault_Click(object sender, System.EventArgs e) {
            if (!checkBoxOverrideDefault.Checked)
                labelRampDescription.Text = "";
            else {
                if (ramp != null)
                    labelRampDescription.Text = ramp.Description;
            }
        }
    }
}
