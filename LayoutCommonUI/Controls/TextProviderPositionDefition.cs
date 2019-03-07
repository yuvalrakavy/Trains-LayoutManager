using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TextProviderPositionDefinition.
    /// </summary>
    public class TextProviderPositionDefinition : System.Windows.Forms.UserControl {
        private GroupBox groupBoxPosition;
        private RadioButton radioButtonCustomPosition;
        private RadioButton radioButtonStandardPosition;
        private LayoutManager.CommonUI.Controls.PositionDefinition positionDefinition1;
        private LayoutManager.CommonUI.Controls.LayoutInfosComboBox layoutInfosComboBoxPositions;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        String customPositionElementName = "Position";
        LayoutXmlInfo xmlInfo;

        public LayoutXmlInfo XmlInfo {
            get {
                return xmlInfo;
            }

            set {
                xmlInfo = value;

                XmlElement positions = (XmlElement)LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Positions");

                layoutInfosComboBoxPositions.InfoType = typeof(LayoutPositionInfo);
                layoutInfosComboBoxPositions.InfoContainer = positions;
            }
        }

        public String CustomPositionElementName {
            get {
                return customPositionElementName;
            }

            set {
                customPositionElementName = value;
            }
        }

        public LayoutPositionInfo PositionProvider {
            get {
                if (radioButtonStandardPosition.Checked)
                    return (LayoutPositionInfo)layoutInfosComboBoxPositions.SelectedItem;
                else {
                    LayoutPositionInfo positionProvider = getCustomPosition();

                    positionDefinition1.Get(positionProvider);
                    return positionProvider;
                }
            }

            set {
                if (value.Element != null && !value.FoundInComponentDocument) {
                    radioButtonStandardPosition.Checked = true;
                    layoutInfosComboBoxPositions.SelectedItem = value;
                }
                else
                    radioButtonCustomPosition.Checked = true;

                updateDependencies();
            }
        }

        void updateDependencies() {
            if (radioButtonStandardPosition.Checked) {
                layoutInfosComboBoxPositions.Enabled = true;
                positionDefinition1.Enabled = false;
            }
            else {
                layoutInfosComboBoxPositions.Enabled = false;
                positionDefinition1.Enabled = true;
            }
        }

        public TextProviderPositionDefinition() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call

        }

        LayoutPositionInfo getCustomPosition() {
            LayoutPositionInfo positionProvider = new LayoutPositionInfo(xmlInfo.DocumentElement, customPositionElementName);

            // If font element did not exist, create it
            if (positionProvider.Element == null)
                positionProvider.Element = LayoutInfo.CreateProviderElement(xmlInfo.DocumentElement, customPositionElementName, null);

            return positionProvider;
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
            this.layoutInfosComboBoxPositions = new LayoutManager.CommonUI.Controls.LayoutInfosComboBox();
            this.positionDefinition1 = new LayoutManager.CommonUI.Controls.PositionDefinition();
            this.groupBoxPosition = new GroupBox();
            this.radioButtonCustomPosition = new RadioButton();
            this.radioButtonStandardPosition = new RadioButton();
            this.groupBoxPosition.SuspendLayout();
            this.SuspendLayout();
            // 
            // layoutInfosComboBoxPositions
            // 
            this.layoutInfosComboBoxPositions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.layoutInfosComboBoxPositions.DropDownWidth = 176;
            this.layoutInfosComboBoxPositions.InfoContainer = null;
            this.layoutInfosComboBoxPositions.Location = new System.Drawing.Point(128, 18);
            this.layoutInfosComboBoxPositions.Name = "layoutInfosComboBoxPositions";
            this.layoutInfosComboBoxPositions.SelectedItem = null;
            this.layoutInfosComboBoxPositions.Size = new System.Drawing.Size(168, 21);
            this.layoutInfosComboBoxPositions.TabIndex = 8;
            // 
            // positionDefinition1
            // 
            this.positionDefinition1.Location = new System.Drawing.Point(24, 56);
            this.positionDefinition1.Name = "positionDefinition1";
            this.positionDefinition1.Size = new System.Drawing.Size(272, 104);
            this.positionDefinition1.TabIndex = 7;
            // 
            // groupBoxPosition
            // 
            this.groupBoxPosition.Controls.Add(this.layoutInfosComboBoxPositions);
            this.groupBoxPosition.Controls.Add(this.radioButtonCustomPosition);
            this.groupBoxPosition.Controls.Add(this.radioButtonStandardPosition);
            this.groupBoxPosition.Controls.Add(this.positionDefinition1);
            this.groupBoxPosition.Location = new System.Drawing.Point(8, 8);
            this.groupBoxPosition.Name = "groupBoxPosition";
            this.groupBoxPosition.Size = new System.Drawing.Size(304, 162);
            this.groupBoxPosition.TabIndex = 2;
            this.groupBoxPosition.TabStop = false;
            this.groupBoxPosition.Text = "Position:";
            // 
            // radioButtonCustomPosition
            // 
            this.radioButtonCustomPosition.Location = new System.Drawing.Point(12, 40);
            this.radioButtonCustomPosition.Name = "radioButtonCustomPosition";
            this.radioButtonCustomPosition.Size = new System.Drawing.Size(132, 16);
            this.radioButtonCustomPosition.TabIndex = 3;
            this.radioButtonCustomPosition.Text = "Custom Position:";
            this.radioButtonCustomPosition.CheckedChanged += new System.EventHandler(this.radioButtonCustomPosition_CheckedChanged);
            // 
            // radioButtonStandardPosition
            // 
            this.radioButtonStandardPosition.Location = new System.Drawing.Point(12, 20);
            this.radioButtonStandardPosition.Name = "radioButtonStandardPosition";
            this.radioButtonStandardPosition.Size = new System.Drawing.Size(116, 16);
            this.radioButtonStandardPosition.TabIndex = 3;
            this.radioButtonStandardPosition.Text = "Standard Position:";
            this.radioButtonStandardPosition.CheckedChanged += new System.EventHandler(this.radioButtonStandardPosition_CheckedChanged);
            // 
            // TextProviderPositionDefinition
            // 
            this.Controls.Add(this.groupBoxPosition);
            this.Name = "TextProviderPositionDefinition";
            this.Size = new System.Drawing.Size(320, 171);
            this.groupBoxPosition.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void radioButtonStandardPosition_CheckedChanged(object sender, System.EventArgs e) {
            updateDependencies();
        }

        private void radioButtonCustomPosition_CheckedChanged(object sender, System.EventArgs e) {
            LayoutPositionInfo positionProvider = getCustomPosition();

            positionDefinition1.Set(positionProvider);
            updateDependencies();
        }
    }
}
