using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LayoutEmulationSetup.
    /// </summary>
    public class LayoutEmulationSetup : System.Windows.Forms.UserControl {
        private GroupBox groupBoxEmulationServices;
        private CheckBox checkBoxAnimateTrainMotion;
        private TextBox textBoxEmulationTickTime;
        private Label labelEmulationTickTime;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        XmlElement element;

        public LayoutEmulationSetup() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public XmlElement Element {
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
            ILayoutEmulatorServices emulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent(this, "get-layout-emulation-services"));

            if (emulationServices != null) {
                int tickTime = 200;

                if (Element.HasAttribute("EmulationTickTime"))
                    tickTime = XmlConvert.ToInt32(Element.GetAttribute("EmulationTickTime"));

                textBoxEmulationTickTime.Text = tickTime.ToString();

                bool animateMotion = false;

                if (Element.HasAttribute("AnimateTrainMotion"))
                    animateMotion = XmlConvert.ToBoolean(Element.GetAttribute("AnimateTrainMotion"));

                checkBoxAnimateTrainMotion.Checked = animateMotion;
            }
            else
                groupBoxEmulationServices.Visible = false;
        }

        public bool ValidateInput() {
            try {
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

            Element.SetAttribute("EmulationTickTime", XmlConvert.ToString(int.Parse(textBoxEmulationTickTime.Text)));
            Element.SetAttribute("AnimateTrainMotion", XmlConvert.ToString(checkBoxAnimateTrainMotion.Checked));

            return true;
        }

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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBoxEmulationServices = new GroupBox();
            this.checkBoxAnimateTrainMotion = new CheckBox();
            this.textBoxEmulationTickTime = new TextBox();
            this.labelEmulationTickTime = new Label();
            this.groupBoxEmulationServices.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxEmulationServices
            // 
            this.groupBoxEmulationServices.Controls.Add(this.checkBoxAnimateTrainMotion);
            this.groupBoxEmulationServices.Controls.Add(this.textBoxEmulationTickTime);
            this.groupBoxEmulationServices.Controls.Add(this.labelEmulationTickTime);
            this.groupBoxEmulationServices.Location = new System.Drawing.Point(0, 0);
            this.groupBoxEmulationServices.Name = "groupBoxEmulationServices";
            this.groupBoxEmulationServices.Size = new System.Drawing.Size(336, 68);
            this.groupBoxEmulationServices.TabIndex = 11;
            this.groupBoxEmulationServices.TabStop = false;
            this.groupBoxEmulationServices.Text = "Simulation of layout operation";
            // 
            // checkBoxAnimateTrainMotion
            // 
            this.checkBoxAnimateTrainMotion.Location = new System.Drawing.Point(24, 41);
            this.checkBoxAnimateTrainMotion.Name = "checkBoxAnimateTrainMotion";
            this.checkBoxAnimateTrainMotion.Size = new System.Drawing.Size(144, 16);
            this.checkBoxAnimateTrainMotion.TabIndex = 3;
            this.checkBoxAnimateTrainMotion.Text = "Animate train motion";
            // 
            // textBoxEmulationTickTime
            // 
            this.textBoxEmulationTickTime.Location = new System.Drawing.Point(211, 17);
            this.textBoxEmulationTickTime.Name = "textBoxEmulationTickTime";
            this.textBoxEmulationTickTime.Size = new System.Drawing.Size(56, 20);
            this.textBoxEmulationTickTime.TabIndex = 2;
            // 
            // labelEmulationTickTime
            // 
            this.labelEmulationTickTime.AutoSize = true;
            this.labelEmulationTickTime.Location = new System.Drawing.Point(24, 20);
            this.labelEmulationTickTime.Name = "labelEmulationTickTime";
            this.labelEmulationTickTime.Size = new System.Drawing.Size(168, 13);
            this.labelEmulationTickTime.TabIndex = 1;
            this.labelEmulationTickTime.Text = "Simulation tick time (milliseconds): ";
            this.labelEmulationTickTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LayoutEmulationSetup
            // 
            this.Controls.Add(this.groupBoxEmulationServices);
            this.Name = "LayoutEmulationSetup";
            this.Size = new System.Drawing.Size(338, 76);
            this.groupBoxEmulationServices.ResumeLayout(false);
            this.groupBoxEmulationServices.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
