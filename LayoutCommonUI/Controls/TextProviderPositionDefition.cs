using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TextProviderPositionDefinition.
    /// </summary>
    public partial class TextProviderPositionDefinition : UserControl {
        private LayoutXmlInfo? xmlInfo;

        public LayoutXmlInfo XmlInfo { get => Ensure.NotNull<LayoutXmlInfo>(OptionalXmlInfo); set => OptionalXmlInfo = value; }

        public LayoutXmlInfo? OptionalXmlInfo {
            get {
                return xmlInfo;
            }

            set {
                xmlInfo = value;

                var positions = Ensure.NotNull<XmlElement>(LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Positions"));

                layoutInfosComboBoxPositions.InfoType = typeof(LayoutPositionInfo);
                layoutInfosComboBoxPositions.InfoContainer = positions;
            }
        }

        public string CustomPositionElementName { get; set; } = "Position";

        public LayoutPositionInfo PositionProvider {
            get {
                if (radioButtonStandardPosition.Checked)
                    return (LayoutPositionInfo)layoutInfosComboBoxPositions.SelectedItem;
                else {
                    LayoutPositionInfo positionProvider = GetCustomPosition();

                    positionDefinition1.Get(positionProvider);
                    return positionProvider;
                }
            }

            set {
                if (value.OptionalElement != null && !value.FoundInComponentDocument) {
                    radioButtonStandardPosition.Checked = true;
                    layoutInfosComboBoxPositions.SelectedItem = value;
                }
                else
                    radioButtonCustomPosition.Checked = true;

                UpdateDependencies();
            }
        }

        private void UpdateDependencies() {
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

        private LayoutPositionInfo GetCustomPosition() {
            LayoutPositionInfo positionProvider = new(XmlInfo.DocumentElement, CustomPositionElementName);

            // If font element did not exist, create it
            if (positionProvider.OptionalElement == null)
                positionProvider.Element = LayoutInfo.CreateProviderElement(XmlInfo.DocumentElement, CustomPositionElementName, null);

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


        private void RadioButtonStandardPosition_CheckedChanged(object? sender, EventArgs e) {
            UpdateDependencies();
        }

        private void RadioButtonCustomPosition_CheckedChanged(object? sender, EventArgs e) {
            LayoutPositionInfo positionProvider = GetCustomPosition();

            positionDefinition1.Set(positionProvider);
            UpdateDependencies();
        }
    }
}
