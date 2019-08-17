using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for SelectTripPlanIcon.
    /// </summary>
    public class SelectTripPlanIcon : System.Windows.Forms.UserControl {
        private NoBackgroundErasePanel panelIcons;
        private Button buttonAdd;
        private Button buttonDelete;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.HScrollBar hScrollBar;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }
        private int selected = -1;
        private int leftIconIndex = 0;
        private bool initialized = false;
        private Bitmap frameBuffer = null;

        #region Exposed Properties

        public TripPlanIconListInfo IconList { get; set; } = null;

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
            setScrollBar();
            buttonDelete.Enabled = false;
        }

        #endregion

        #region Internal methods

        private Graphics getFrameBufferGraphics(Graphics g, Rectangle clipBounds) {
            if (frameBuffer == null || clipBounds.Width > frameBuffer.Width || clipBounds.Height > frameBuffer.Height) {
                if (frameBuffer != null)
                    frameBuffer.Dispose();

                frameBuffer = new Bitmap(clipBounds.Width, clipBounds.Height, g);
            }

            return Graphics.FromImage(frameBuffer);
        }

        private void setScrollBar() {
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panelIcons = new LayoutManager.CommonUI.Controls.SelectTripPlanIcon.NoBackgroundErasePanel();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.buttonAdd = new Button();
            this.buttonDelete = new Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panelIcons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelIcons
            // 
            this.panelIcons.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.panelIcons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelIcons.Controls.Add(this.hScrollBar);
            this.panelIcons.Location = new System.Drawing.Point(2, 2);
            this.panelIcons.Name = "panelIcons";
            this.panelIcons.Size = new System.Drawing.Size(244, 60);
            this.panelIcons.TabIndex = 0;
            this.panelIcons.Resize += this.panelIcons_Resize;
            this.panelIcons.Paint += this.panelIcons_Paint;
            this.panelIcons.MouseDown += this.panelIcons_MouseDown;
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 46);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(242, 12);
            this.hScrollBar.TabIndex = 0;
            this.hScrollBar.Scroll += this.hScrollBar_Scroll;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(4, 66);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(53, 17);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add...";
            this.buttonAdd.Click += this.buttonAdd_Click;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(61, 66);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(53, 17);
            this.buttonDelete.TabIndex = 1;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.Click += this.buttonDelete_Click;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "ico";
            this.openFileDialog.Filter = "Icon files|*.ico|All files|*.*";
            this.openFileDialog.Multiselect = true;
            // 
            // SelectTripPlanIcon
            // 
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.panelIcons);
            this.Controls.Add(this.buttonDelete);
            this.Name = "SelectTripPlanIcon";
            this.Size = new System.Drawing.Size(248, 86);
            this.panelIcons.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void panelIcons_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
            Rectangle clipBounds = Rectangle.Ceiling(e.Graphics.VisibleClipBounds);

            using (Graphics g = getFrameBufferGraphics(e.Graphics, clipBounds)) {
                g.TranslateTransform(-clipBounds.Left, -clipBounds.Top);

                g.FillRectangle(SystemBrushes.Window, clipBounds);

                if (initialized) {
                    int x = 0;

                    for (int iconIndex = leftIconIndex; iconIndex < IconList.LargeIconImageList.Images.Count; iconIndex++) {
                        g.DrawImage(IconList.LargeIconImageList.Images[iconIndex], x + 6, 6);

                        if (iconIndex == selected) {
                            using Pen p = new Pen(Color.Red, 2);
                            g.DrawRectangle(p, x + 2, 2, 32 + 8, 32 + 8);
                        }

                        x += 36;
                    }
                }
            }

            e.Graphics.DrawImage(frameBuffer, clipBounds.Location);
        }

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
                foreach (string filename in openFileDialog.FileNames) {
                    try {
                        Icon icon = new Icon(filename);

                        IconList.Add(icon);
                    }
                    catch (Exception ex) {
                        MessageBox.Show(this, "Error: " + ex.Message, "Error opening Icon file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                setScrollBar();
                panelIcons.Invalidate();
            }
        }

        private void panelIcons_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
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

        private void hScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) {
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

        private void panelIcons_Resize(object sender, System.EventArgs e) {
            if (initialized)
                setScrollBar();
        }

        private void buttonDelete_Click(object sender, System.EventArgs e) {
            int count = IconList.LargeIconImageList.Images.Count;
            bool doit = true;

            if (count == 1)
                doit = MessageBox.Show(this, "Erasing the last icon is not advisable, are you sure you want to do this?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            if (doit) {
                if (SelectedIndex != -1) {
                    IconList.Remove(SelectedIndex);
                    if (SelectedIndex >= count - 1)
                        SelectedIndex = count - 2;

                    setScrollBar();
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
