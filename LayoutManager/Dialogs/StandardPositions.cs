using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StandardPositions.
    /// </summary>
    public partial class StandardPositions : Form {

        public StandardPositions() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Fill the list box
            var positions = LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Positions");

            if (positions != null) {
                foreach (XmlElement positionElement in positions)
                    listBoxPositions.Items.Add(new LayoutPositionInfo(positionElement));
            }
        }

        private bool Edit(LayoutPositionInfo positionProvider) {
            StandardPositionProperties positionProperties = new(positionProvider);

            if (positionProperties.ShowDialog() == DialogResult.OK) {
                positionProperties.Get(positionProvider);
                return true;
            }

            return false;
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

        private void ButtonClose_Click(object? sender, System.EventArgs e) {
            this.Close();
        }

        private void ButtonEdit_Click(object? sender, System.EventArgs e) {
            LayoutPositionInfo positionProvider = (LayoutPositionInfo)listBoxPositions.SelectedItem;

            Edit(positionProvider);
            listBoxPositions.Items[listBoxPositions.SelectedIndex] = positionProvider;
        }

        private void ButtonNew_Click(object? sender, System.EventArgs e) {
            XmlElement positionElement = LayoutInfo.CreateProviderElement(LayoutModel.Instance.XmlInfo, "Position", "Positions");
            LayoutPositionInfo positionProvider = new(positionElement);

            if (Edit(positionProvider))
                listBoxPositions.SelectedIndex = listBoxPositions.Items.Add(positionProvider);
        }

        private void ButtonDelete_Click(object? sender, System.EventArgs e) {
            if (listBoxPositions.SelectedItem != null) {
                if (MessageBox.Show("Do you really want to delete a style position definition. " +
                    "This will cause problems if this position is still being used!", "Warning",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK) {
                    LayoutPositionInfo positionProvider = (LayoutPositionInfo)listBoxPositions.SelectedItem;

                    positionProvider.Element.ParentNode?.RemoveChild(positionProvider.Element);
                    listBoxPositions.Items.Remove(positionProvider);
                }
            }
        }
    }
}
