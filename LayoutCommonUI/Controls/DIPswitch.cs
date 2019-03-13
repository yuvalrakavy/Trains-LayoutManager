using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for DIPswitch.
    /// </summary>
    public class DIPswitch : System.Windows.Forms.Control {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        int switchCount = 8;
        int switchCountBase = 1;
        long switchValue = 0;
        bool lsbOnRight = false;
        readonly int leftMargin = 20;
        readonly int topMargin = 8;
        readonly int bottomMargin = 14;
        int xUnit;

        public DIPswitch() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitComponent call
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        [Description("Number of DIP switches")]
        public int SwitchCount {
            get {
                return switchCount;
            }

            set {
                switchCount = value;

                Invalidate();
            }
        }

        [Description("The number to show for bit 0")]
        public int SwitchCountBase {
            get {
                return switchCountBase;
            }

            set {
                switchCountBase = value;
            }
        }

        public long Value {
            get {
                return switchValue;
            }

            set {
                switchValue = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Show LSB on right side
        /// </summary>
        [Description("If true, LSB (bit 0) is shown on the right")]
        public bool LSBonRight {
            get {
                return lsbOnRight;
            }

            set {
                lsbOnRight = value;
                Invalidate();
            }
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new Container();
        }
        #endregion

        private void drawSwitch(Graphics g, int iSwitch, int switchIndex) {
            int switchHeight = Height - topMargin - bottomMargin;
            Rectangle switchRect = new Rectangle(new Point(leftMargin + xUnit * 2 * iSwitch, topMargin), new Size(xUnit, switchHeight));
            bool thumbUp = (switchValue & (1 << switchIndex)) != 0;
            int thumbHeight = switchHeight / 3;
            int yThumb = thumbUp ? switchRect.Top + 1 : switchRect.Bottom - thumbHeight - 2;
            Rectangle thumbRect = new Rectangle(new Point(switchRect.Left + 1, yThumb), new Size(xUnit - 2, thumbHeight));

            g.FillRectangle(Brushes.Silver, switchRect);
            g.DrawLines(Pens.DarkGray, new Point[] { new Point(switchRect.Left, switchRect.Bottom), new Point(switchRect.Left, switchRect.Top), new Point(switchRect.Right, switchRect.Top) });
            g.DrawLines(Pens.LightGray, new Point[] { new Point(switchRect.Right, switchRect.Top + 1), new Point(switchRect.Right, switchRect.Bottom), new Point(switchRect.Left, switchRect.Bottom) });

            g.FillRectangle(Brushes.WhiteSmoke, thumbRect);
            g.DrawLines(Pens.White, new Point[] { new Point(thumbRect.Left, thumbRect.Bottom), new Point(thumbRect.Left, thumbRect.Top), new Point(thumbRect.Right, thumbRect.Top) });
            g.DrawLines(Pens.Gainsboro, new Point[] { new Point(thumbRect.Right, thumbRect.Top + 1), new Point(thumbRect.Right, thumbRect.Bottom), new Point(thumbRect.Left, thumbRect.Bottom) });

            using (Font labelFont = new Font("Arial", 6.5F)) {
                string label = ((int)(switchIndex + SwitchCountBase)).ToString();
                SizeF labelSize = g.MeasureString(label, labelFont);

                g.DrawString(label, labelFont, Brushes.White, new RectangleF(new PointF(switchRect.Left + (switchRect.Width - labelSize.Width) / 2, switchRect.Bottom + 2), labelSize));
            }
        }

        private void drawLabels(Graphics g) {
            using (Font labelFont = new Font("Arial", 6.5F)) {
                SizeF labelSize;

                labelSize = g.MeasureString("On", labelFont);
                g.DrawString("On", labelFont, Brushes.White, new RectangleF(new PointF(2, 2), labelSize));

                labelSize = g.MeasureString("Off", labelFont);
                g.DrawString("Off", labelFont, Brushes.White, new RectangleF(new PointF(2, Height - 2 - labelSize.Height), labelSize));
            }
        }


        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);

            xUnit = (Width - leftMargin) / (2 * switchCount);
            float ySwitch = Height - topMargin - bottomMargin;

            RectangleF clipBounds = pe.Graphics.VisibleClipBounds;
            Bitmap buffer = new Bitmap((int)Math.Ceiling(clipBounds.Width), (int)Math.Ceiling(clipBounds.Height), pe.Graphics);

            using (Graphics g = Graphics.FromImage(buffer)) {
                // Translate so the top-left of the clip region is on the top/left (0,0)
                // of the bitmap
                g.TranslateTransform(-clipBounds.Left, -clipBounds.Top);

                g.FillRectangle(Brushes.Red, ClientRectangle);

                int switchIndex = 0;

                if (lsbOnRight) {
                    for (int i = switchCount - 1; i >= 0; i--)
                        drawSwitch(g, i, switchIndex++);
                }
                else {
                    for (int i = 0; i < switchCount; i++)
                        drawSwitch(g, i, switchIndex++);
                }

                drawLabels(g);
            }

            // After the background image is created, draw it on the screen
            pe.Graphics.DrawImage(buffer, clipBounds.Location);
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);

            Invalidate();
        }
    }
}
