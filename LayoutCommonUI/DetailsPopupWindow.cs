using System.Drawing;
using System.Windows.Forms;
using LayoutManager.View;

namespace LayoutManager.CommonUI {
    public partial class DetailsPopupWindow : Form {
        private const int VerticalOffset = 20;
        private readonly PopupWindowContainerSection sectionsContainer;

        public DetailsPopupWindow(LayoutHitTestResult hitTestResult, PopupWindowContainerSection sectionsContainer) {
            InitializeComponent();

            // For now size if not really calculated...
            this.sectionsContainer = sectionsContainer;
            Size s = sectionsContainer.Size;
            Size = new Size(s.Width + 4, s.Height + 4);
            int x, y;

            Point hitInScreenCoordinates = hitTestResult.View.PointToScreen(hitTestResult.ClientLocation);

            if (hitTestResult.ClientLocation.X - Width / 2 < 10)
                x = hitTestResult.View.PointToScreen(hitTestResult.View.Location).X + 10;
            else if (hitTestResult.ClientLocation.X + Width / 2 > hitTestResult.View.Width - 40)
                x = hitTestResult.View.PointToScreen(hitTestResult.View.Location).X + hitTestResult.View.Width - Width - 40;
            else
                x = hitInScreenCoordinates.X - Width / 2;

            if (hitTestResult.ClientLocation.Y + Height + VerticalOffset > hitTestResult.View.Height)
                y = hitInScreenCoordinates.Y - VerticalOffset - Height;
            else
                y = hitInScreenCoordinates.Y + VerticalOffset;

            Location = new Point(x, y);
        }

        protected override void OnPaint(PaintEventArgs e) {
            sectionsContainer.Paint(e.Graphics);
        }
    }
}
