using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for PolicyDefinition.
    /// </summary>
    public class TrainConditionDefinition : Form {
        private LayoutManager.CommonUI.Controls.EventScriptEditor eventScriptEditor;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonNoCondition;
        private RadioButton radioButtonCondition;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuConditionScope;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        private readonly TripPlanTrainConditionInfo trainCondition;

        public TrainConditionDefinition(LayoutBlockDefinitionComponent blockDefinition, TripPlanTrainConditionInfo inTrainCondition) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            XmlDocument scriptDocument = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement conditionBodyElement = null;

            trainCondition = new TripPlanTrainConditionInfo((XmlElement)scriptDocument.ImportNode(inTrainCondition.Element, true));

            scriptDocument.AppendChild(trainCondition.Element);

            if (!trainCondition.IsConditionEmpty)
                conditionBodyElement = trainCondition.ConditionBodyElement;

            if (!trainCondition.IsConditionEmpty) {
                radioButtonCondition.Checked = true;
                eventScriptEditor.Enabled = true;
            }
            else {
                radioButtonNoCondition.Checked = true;
                conditionBodyElement = scriptDocument.CreateElement("And");
                trainCondition.Element.AppendChild(conditionBodyElement);
                eventScriptEditor.Enabled = false;
            }

            eventScriptEditor.EventScriptElement = conditionBodyElement;
            eventScriptEditor.BlockDefinition = blockDefinition;

            if (trainCondition.ConditionScope == TripPlanTrainConditionScope.AllowIfTrue)
                linkMenuConditionScope.SelectedIndex = 0;
            else
                linkMenuConditionScope.SelectedIndex = 1;
        }

        public TripPlanTrainConditionInfo TrainCondition {
            get {
                if (radioButtonNoCondition.Checked) {
                    if (!trainCondition.IsConditionEmpty)
                        trainCondition.Element.RemoveChild(trainCondition.ConditionBodyElement);
                }

                return trainCondition;
            }
        }

        public bool NoCondition => radioButtonNoCondition.Checked;

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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.eventScriptEditor = new LayoutManager.CommonUI.Controls.EventScriptEditor();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.radioButtonNoCondition = new RadioButton();
            this.radioButtonCondition = new RadioButton();
            this.linkMenuConditionScope = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.SuspendLayout();
            // 
            // eventScriptEditor
            // 
            this.eventScriptEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.eventScriptEditor.EventScriptElement = null;
            this.eventScriptEditor.Location = new System.Drawing.Point(16, 53);
            this.eventScriptEditor.Name = "eventScriptEditor";
            this.eventScriptEditor.Size = new System.Drawing.Size(338, 243);
            this.eventScriptEditor.TabIndex = 3;
            this.eventScriptEditor.ViewOnly = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(232, 306);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(56, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(296, 306);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.buttonCancel_Click;
            // 
            // radioButtonNoCondition
            // 
            this.radioButtonNoCondition.Location = new System.Drawing.Point(8, 8);
            this.radioButtonNoCondition.Name = "radioButtonNoCondition";
            this.radioButtonNoCondition.TabIndex = 6;
            this.radioButtonNoCondition.Text = "No condition";
            this.radioButtonNoCondition.CheckedChanged += this.radioButtonNoCondition_CheckedChanged;
            // 
            // radioButtonCondition
            // 
            this.radioButtonCondition.Location = new System.Drawing.Point(8, 32);
            this.radioButtonCondition.Name = "radioButtonCondition";
            this.radioButtonCondition.Size = new System.Drawing.Size(16, 24);
            this.radioButtonCondition.TabIndex = 7;
            this.radioButtonCondition.CheckedChanged += this.radioButtonCondition_CheckedChanged;
            // 
            // linkMenuConditionScope
            // 
            this.linkMenuConditionScope.Location = new System.Drawing.Point(25, 36);
            this.linkMenuConditionScope.Name = "linkMenuConditionScope";
            this.linkMenuConditionScope.Options = new string[] {
                                                                   "Allow only trains for which the following is True",
                                                                   "Do not allow trains for which the following is True"};
            this.linkMenuConditionScope.SelectedIndex = -1;
            this.linkMenuConditionScope.Size = new System.Drawing.Size(256, 16);
            this.linkMenuConditionScope.TabIndex = 8;
            this.linkMenuConditionScope.TabStop = true;
            this.linkMenuConditionScope.Text = "Allow only trains for which the following is True";
            // 
            // TrainConditionDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(360, 334);
            this.ControlBox = false;
            this.Controls.Add(this.linkMenuConditionScope);
            this.Controls.Add(this.radioButtonCondition);
            this.Controls.Add(this.radioButtonNoCondition);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.eventScriptEditor);
            this.Controls.Add(this.buttonCancel);
            this.Name = "TrainConditionDefinition";
            this.ShowInTaskbar = false;
            this.Text = "Train Condition Definition";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (radioButtonCondition.Checked) {
                if (!eventScriptEditor.ValidateScript()) {
                    eventScriptEditor.Focus();
                    return;
                }
            }

            if (linkMenuConditionScope.SelectedIndex == 0)
                trainCondition.ConditionScope = TripPlanTrainConditionScope.AllowIfTrue;
            else
                trainCondition.ConditionScope = TripPlanTrainConditionScope.DisallowIfTrue;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void radioButtonNoCondition_CheckedChanged(object sender, System.EventArgs e) {
            eventScriptEditor.Enabled = false;
            linkMenuConditionScope.Enabled = false;
        }

        private void radioButtonCondition_CheckedChanged(object sender, System.EventArgs e) {
            eventScriptEditor.Enabled = true;
            linkMenuConditionScope.Enabled = true;
        }
    }
}
