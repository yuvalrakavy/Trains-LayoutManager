
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for PositionDefinition.
    /// </summary>
    public partial class PositionDefinition : UserControl {

        public PositionDefinition() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            enumComboBoxPosition.EnumType = typeof(LayoutDrawingSide);
            enumComboBoxAlignment.EnumType = typeof(LayoutDrawingAnchorPoint);
        }

        public void Set(LayoutPositionInfo positionProvider) {
            enumComboBoxPosition.SelectedItem = (int)positionProvider.Side;
            enumComboBoxAlignment.SelectedItem = (int)positionProvider.AnchorPoint;
            numericUpDownDistance.Value = positionProvider.Distance;
            numericUpDownWidth.Value = positionProvider.Width;
        }

        public void Get(LayoutPositionInfo positionProvider) {
            positionProvider.Side = (LayoutDrawingSide)enumComboBoxPosition.SelectedItem;
            positionProvider.AnchorPoint = (LayoutDrawingAnchorPoint)enumComboBoxAlignment.SelectedItem;
            positionProvider.Distance = (int)numericUpDownDistance.Value;
            positionProvider.Width = (int)numericUpDownWidth.Value;
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


        private void EnumComboBoxPosition_SelectedIndexChanged(object? sender, EventArgs e) {
            layoutPositionInfoPreview1.Side = (LayoutDrawingSide)enumComboBoxPosition.SelectedItem;
        }

        private void NumericUpDownDistance_ValueChanged(object? sender, EventArgs e) {
            layoutPositionInfoPreview1.Distance = (int)numericUpDownDistance.Value;
        }

        private void EnumComboBoxAlignment_SelectedIndexChanged(object? sender, EventArgs e) {
            layoutPositionInfoPreview1.Alignment = (LayoutDrawingAnchorPoint)enumComboBoxAlignment.SelectedItem;
        }

        private void NumericUpDownWidth_ValueChanged(object? sender, EventArgs e) {
            layoutPositionInfoPreview1.LayoutWidth = (int)numericUpDownWidth.Value;
        }
    }
}
