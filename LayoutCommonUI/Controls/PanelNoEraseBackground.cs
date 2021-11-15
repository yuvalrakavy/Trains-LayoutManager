namespace LayoutManager.CommonUI.Controls {
    public partial class PanelNoEraseBackground : Panel {
        public PanelNoEraseBackground() {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            // Do not call base to prevent erasing the background
        }
    }
}
