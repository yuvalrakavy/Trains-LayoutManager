using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for SelectTripPlanIcon.
    /// </summary>
    public partial class SelectTripPlanIcon : UserControl {
        private int selected = -1;
        private int leftIconIndex = 0;
        private bool initialized = false;
        private Bitmap? frameBuffer = null;

        #region Exposed Properties

        public TripPlanIconListInfo? OptionalIconList { get; set; } = null;

        public TripPlanIconListInfo IconList { get => Ensure.NotNull<TripPlanIconListInfo>(OptionalIconList); set => OptionalIconList = value; }

        public int SelectedIndex {
            get {
                return selected;
            }

            set {
                selected = value;
                if (initialized) {
                    panelIcons.Invalidate();
                    if (selected >= 0)
                        buttonDelete.Enabled = true;
                    else
                        buttonDelete.Enabled = false;
                }
            }
        }

        public Guid SelectedID {
            get {
                return selected >= 0 ? IconList[selected] : Guid.Empty;
            }

            set {
                if (value == Guid.Empty)
                    SelectedIndex = -1;
                else
                    SelectedIndex = IconList[value];
            }
        }

        #endregion

        #region Exposed operations

        public void Initialize() {
            initialized = true;
            panelIcons.Invalidate();
            SetScrollBar();
            buttonDelete.Enabled = false;
        }

        #endregion

        #region Internal methods

        private Graphics GetFrameBufferGraphics(Graphics g, Rectangle clipBounds) {
            if (frameBuffer == null || clipBounds.Width > frameBuffer.Width || clipBounds.Height > frameBuffer.Height) {
                if (frameBuffer != null)
                    frameBuffer.Dispose();

                frameBuffer = new Bitmap(clipBounds.Width, clipBounds.Height, g);
            }

            return Graphics.FromImage(frameBuffer);
        }

        private void SetScrollBar() {
            int maxDisplayedIconCount = Width / 36;
            int iconCount = IconList.LargeIconImageList.Images.Count;

            if (iconCount <= maxDisplayedIconCount)
                hScrollBar.Enabled = false;
            else {
                hScrollBar.Enabled = true;

                hScrollBar.Minimum = 0;
                hScrollBar.Maximum = iconCount;
                if (leftIconIndex > hScrollBar.Maximum)
                    leftIconIndex = hScrollBar.Maximum;
                hScrollBar.Value = leftIconIndex;
                hScrollBar.SmallChange = 1;
                hScrollBar.LargeChange = maxDisplayedIconCount;
            }
        }

        #endregion

        public SelectTripPlanIcon() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (frameBuffer != null)
                    frameBuffer.Dispose();

                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        private void PanelIcons_Paint(object? sender, PaintEventArgs e) {
            Rectangle clipBounds = Rectangle.Ceiling(e.Graphics.VisibleClipBounds);

            using (Graphics g = GetFrameBufferGraphics(e.Graphics, clipBounds)) {
                g.TranslateTransform(-clipBounds.Left, -clipBounds.Top);

                g.FillRectangle(SystemBrushes.Window, clipBounds);

                if (initialized) {
                    int x = 0;

                    for (int iconIndex = leftIconIndex; iconIndex < IconList.LargeIconImageList.Images.Count; iconIndex++) {
                        g.DrawImage(IconList.LargeIconImageList.Images[iconIndex], x + 6, 6);

                        if (iconIndex == selected) {
                            using Pen p = new(Color.Red, 2);
                            g.DrawRectangle(p, x + 2, 2, 32 + 8, 32 + 8);
                        }

                        x += 36;
                    }
                }
            }

            e.Graphics.DrawImage(frameBuffer!, clipBounds.Location);
        }

        private void ButtonAdd_Click(object? sender, EventArgs e) {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
                foreach (string filename in openFileDialog.FileNames) {
                    try {
                        Icon icon = new(filename);

                        IconList.Add(icon);
                    }
                    catch (Exception ex) {
                        MessageBox.Show(this, "Error: " + ex.Message, "Error opening Icon file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                SetScrollBar();
                panelIcons.Invalidate();
            }
        }

        private void PanelIcons_MouseDown(object? sender, MouseEventArgs e) {
            int newSelection = -1;
            int x = 0;

            for (int iconIndex = leftIconIndex; iconIndex < IconList.LargeIconImageList.Images.Count; iconIndex++) {
                if (new Rectangle(new Point(x, 0), new Size(36, 36)).Contains(e.X, e.Y)) {
                    newSelection = iconIndex;
                    break;
                }

                x += 36;
            }

            SelectedIndex = newSelection;
        }

        private void HScrollBar_Scroll(object? sender, ScrollEventArgs e) {
            int maxDisplayedIconCount = Width / 36;
            int oldValue = leftIconIndex;

            switch (e.Type) {
                case ScrollEventType.First:
                    leftIconIndex = 0;
                    break;

                case ScrollEventType.Last:
                    leftIconIndex = IconList.LargeIconImageList.Images.Count - maxDisplayedIconCount;
                    break;

                case ScrollEventType.SmallIncrement:
                    leftIconIndex += hScrollBar.SmallChange;
                    break;

                case ScrollEventType.LargeIncrement:
                    leftIconIndex += hScrollBar.LargeChange - 2;
                    break;

                case ScrollEventType.SmallDecrement:
                    leftIconIndex -= hScrollBar.SmallChange;
                    break;

                case ScrollEventType.LargeDecrement:
                    leftIconIndex -= hScrollBar.LargeChange - 2;
                    break;

                case ScrollEventType.ThumbTrack:
                    leftIconIndex = e.NewValue;
                    break;
            }

            if (leftIconIndex != oldValue) {
                int maxValue = IconList.LargeIconImageList.Images.Count - maxDisplayedIconCount;

                if (leftIconIndex < hScrollBar.Minimum)
                    leftIconIndex = hScrollBar.Minimum;
                else if (leftIconIndex > maxValue)
                    leftIconIndex = maxValue;
                panelIcons.Invalidate();
            }
        }

        private void PanelIcons_Resize(object? sender, EventArgs e) {
            if (initialized)
                SetScrollBar();
        }

        private void ButtonDelete_Click(object? sender, EventArgs e) {
            int count = IconList.LargeIconImageList.Images.Count;
            bool doit = true;

            if (count == 1)
                doit = MessageBox.Show(this, "Erasing the last icon is not advisable, are you sure you want to do this?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            if (doit) {
                if (SelectedIndex != -1) {
                    IconList.Remove(SelectedIndex);
                    if (SelectedIndex >= count - 1)
                        SelectedIndex = count - 2;

                    SetScrollBar();
                    panelIcons.Invalidate();
                }
            }
        }

        private class NoBackgroundErasePanel : Panel {
            protected override void OnPaintBackground(PaintEventArgs pevent) {
                // Avoid erasing background
            }
        }
    }
}
