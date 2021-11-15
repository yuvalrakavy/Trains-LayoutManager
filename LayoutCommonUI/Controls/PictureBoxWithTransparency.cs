using System.ComponentModel;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for PictureBoxWithTransparency.
    /// </summary>
    public class PictureBoxWithTransparency : PictureBox {
        private Color transparentColor = Color.Empty;
        private bool isTransparent = false;

        public PictureBoxWithTransparency() {
        }

        [Category("Appearance")]
        public Color TransparentColor {
            get {
                return transparentColor;
            }

            set {
                transparentColor = value;
                isTransparent = false;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pe) {
            if (!isTransparent && Image != null) {
                Bitmap bm = (Bitmap)Image;

                bm.MakeTransparent(transparentColor);
                isTransparent = true;
            }

            // Calling the base class OnPaint
            base.OnPaint(pe);
        }
    }
}
