using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LayoutManager.UIGadgets {
    /// <summary>
    /// Base class for ImageMenuItem or ImageMenuCategory
    /// </summary>
    public abstract class ImageMenuEntry {
        public string? Tooltip { get; set; }

        public Rectangle Bounds { get; set; }

        protected abstract void Paint(Graphics g);

        public void PaintEntry(Graphics g) {
            GraphicsState gs = g.Save();

            g.SetClip(Bounds);
            g.TranslateTransform(Bounds.Location.X, Bounds.Location.Y);
            Paint(g);
            g.Restore(gs);
        }
    }

    public abstract class ImageMenuItem : ImageMenuEntry {
    }

    public abstract class ImageMenuCategory : ImageMenuEntry {
        public ImageMenuItemCollection Items { get; } = new ImageMenuItemCollection();

        public ImageMenu? Menu { get; set; }

        public string? Name { get; set; }
    }

    /// <summary>
    /// A collection of all items in an image menu category
    /// </summary>
    public class ImageMenuItemCollection : List<ImageMenuItem> {
    }

    /// <summary>
    /// A collection of all items in an image menu category
    /// </summary>
    public class ImageMenuCategoryCollection : List<ImageMenuCategory> {
        public ImageMenuCategory? this[string categoryName] {
            get {
                foreach (ImageMenuCategory category in this)
                    if (category.Name == categoryName)
                        return category;

                return null;
            }
        }
    }

    /// <summary>
    /// This utility control displays shadow for a given control. It can be useful for
    /// popup controls such as the image menu
    /// </summary>
    public class Shadow : UserControl {
        private readonly Control shadowOf;

        /// <summary>
        /// Create the shadow for the provided control
        /// </summary>
        /// <param name="shadowOf"></param>
        public Shadow(Control shadowOf) {
            Rectangle r;

            this.Parent = shadowOf.Parent;
            this.shadowOf = shadowOf;

            r = shadowOf.Bounds;
            r.Offset(4, 4);
            this.Bounds = r;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.FromArgb(50, 0, 0, 0);
            this.Visible = true;

            shadowOf.Resize += ShadowOf_Resize;

            CreateControl();
        }

        /// <summary>
        /// Reside the shadow to reflect the new size of the window that this shadow belongs to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShadowOf_Resize(Object? sender, EventArgs e) {
            Rectangle r = shadowOf.Bounds;
            r.Offset(4, 4);
            this.Bounds = r;
        }

        /// <summary>
        /// Close the shadow window
        /// </summary>
        public void Close() {
            this.Dispose();
        }
    }

    /// <summary>
    /// Control that implement tooltip window for image menu objects
    /// </summary>
    internal class TipWindow : UserControl {
        private readonly Font tipFont = new("Arial", 8);

        internal TipWindow(Control parent) {
            this.Parent = parent;
            this.BackColor = Color.LightYellow;
            this.Visible = false;
            this.CreateControl();
            this.TipText = String.Empty;
        }

        /// <summary>
        /// The tooltip text to show
        /// </summary>
        internal string TipText { set; get; }

        /// <summary>
        /// Make sure that the tooltip text is not truncated
        /// </summary>
        private void AdjustLocation() {
            if (Right >= Parent.Right)
                Left = Parent.Right - Width - 5;
            if (Bottom >= Parent.Bottom)
                Top = Parent.Bottom - Height - 5;
        }

        /// <summary>
        /// Show the tooltip text at the given point
        /// </summary>
        /// <param name="p"></param>
        internal void ShowTip(Point p) {
            Graphics g = CreateGraphics();
            SizeF tipSize = g.MeasureString(TipText, tipFont);

            g.Dispose();

            this.Size = Size.Ceiling(tipSize);
            this.Location = p;
            AdjustLocation();
            this.BringToFront();
            this.Visible = true;
        }

        /// <summary>
        /// Hide the tooltip text
        /// </summary>
        internal void HideTip() {
            this.Visible = false;
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
            e.Graphics.DrawString(TipText, tipFont, Brushes.Black, new Point(0, 0));
        }
    }

    /// <summary>
    /// This class shows an horizontal menu of items that draw themself.
    /// </summary>
    public class ImageMenu : UserControl {
        private const int vMargin = 5;                  // Space from top and bottom of the menu
        private const int hMargin = 6;                  // Space from right and left of the menu
        private const int gap = 3;                      // Space between menu items
        private const int vGap = 3;                     // vertical gap (between categories)
        private const int itemsToCategoriesGap = 7;     // Space between items and categories
        private const int tipTime = 250;                // Show tooltip if no mouse motion in 250 milliseconds
        private Size itemSize = new(40, 40);
        private Size categorySize = new(18, 18);
        private ImageMenuEntry? hilightedEntry;
        private ImageMenuCategory? selectedCategory;
        private ImageMenuCategory? initialCategory;          // initial category to show
        private ImageMenuCategory? shiftKeyCategory;
        private ImageMenuCategory? beforeShiftCategory;
        private Shadow? shadow;
        private readonly bool categoriesVisible = true;
        private ImageMenuItem? resultItem;
        private TipWindow? tipWindow;
        private Timer? tipTimer;
        private ImageMenuEntry? tippedEntry;

        private enum MenuState {
            Open,               // Menu is open
            Closing,            // Menu is closing (mouse down outside the menu, mouse up should do nothing)
            Closed              // Menu is closed.
        };

        private MenuState menuState = new();

        /// <summary>
        /// Return a collection of items in the menu
        /// </summary>
        public ImageMenuCategoryCollection Categories { get; } = new ImageMenuCategoryCollection();

        /// <summary>
        /// Make sure that the menu is shown in such a way that it is not truncated.
        /// </summary>
        private void AdjustLocation() {
            if (Right >= Parent.Right)
                Left = Parent.Right - Width - 5;
            if (Bottom >= Parent.Bottom)
                Top = Parent.Bottom - Height - 5;
        }

        /// <summary>
        /// Adjust the size of the menu based on the number of items in the currently selected
        /// category
        /// </summary>
        private void AdjustSize(ImageMenuCategory selectedCategory) {
            int width = hMargin + (selectedCategory.Items.Count * (itemSize.Width + gap)) - gap + hMargin;
            int height = (vMargin * 2) + Math.Max(itemSize.Height, (categorySize.Height * 2) + gap);

            if (categoriesVisible)
                width += itemsToCategoriesGap + ((Categories.Count + 1) / 2 * (categorySize.Width + gap)) - gap;

            this.Size = new Size(width, height);
        }

        /// <summary>
        /// Select a new category. The menu is reformatted to show the items in the new category
        /// </summary>
        /// <param name="newCategory">New category to select</param>
        private void SelectCategory(ImageMenuCategory newCategory) {
            if (selectedCategory != newCategory) {
                // TODO: For animation effect, may want to display just the categories, and then expand
                // the menu with the new selection

                selectedCategory = newCategory;
                AdjustSize(selectedCategory);

                int x = hMargin;

                foreach (ImageMenuItem item in selectedCategory.Items) {
                    item.Bounds = new Rectangle(new Point(x, vMargin), itemSize);
                    x += itemSize.Width + gap;
                }

                x -= gap;       // Compensate for the one extra gap that added

                if (categoriesVisible) {
                    x += itemsToCategoriesGap;

                    for (int iCategory = 0; iCategory < Categories.Count; iCategory++) {
                        ImageMenuCategory category = Categories[iCategory];

                        category.Bounds = new Rectangle(new Point(x, (iCategory & 1) == 0 ? vMargin : vMargin + categorySize.Height + vGap),
                            categorySize);

                        if ((iCategory & 1) != 0)
                            x += categorySize.Width + gap;
                    }
                }

                Invalidate();
            }
        }

        /// <summary>
        /// The initial category to select
        /// </summary>
        public string? InitialCategoryName {
            get {
                return initialCategory?.Name;
            }

            set {
                initialCategory = null;

                foreach (ImageMenuCategory category in Categories)
                    if (category.Name == value) {
                        initialCategory = category;
                        break;
                    }
            }
        }

        public string? ShiftKeyCategory {
            get => shiftKeyCategory?.Name;

            set {
                shiftKeyCategory = null;

                foreach (ImageMenuCategory category in Categories)
                    if (category.Name == value) {
                        shiftKeyCategory = category;
                        break;
                    }
            }
        }

        /// <summary>
        /// The last category that was selected (it make sense to use this property after calling show)
        /// </summary>
        public ImageMenuCategory? SelectedCategory => selectedCategory;

        /// <summary>
        /// Show the menu. The menu is shown in a given point of a given parent control
        /// </summary>
        /// <param name="parent">The control on which the menu is shown</param>
        /// <param name="p">The position where the menu should be shown</param>
        /// <returns>The selected menu item or null if no item is selected.</returns>
        public ImageMenuItem? Show(Control parent, Point p) {
            this.Parent = parent;
            this.Location = p;      // TODO: Location should be much smarter (considering alighment) etc.

            if (Categories.Count == 0) {
                Debug.Fail("Image menu has no item categories");
                return null;
            }

            if (initialCategory == null)
                initialCategory = Categories[0];

            SelectCategory(initialCategory);
            AdjustLocation();

            this.Visible = true;

            shadow = new Shadow(this);
            tipWindow = new TipWindow(parent);
            tipTimer = new Timer {
                Interval = tipTime
            };
            tipTimer.Tick += TipTimer_tick;

            this.CreateControl();
            this.Capture = true;

            menuState = MenuState.Open;

            this.Focus();

            while (menuState != MenuState.Closed) {
                this.Focus();
                this.Capture = true;
                Application.DoEvents();
            }

            tipWindow.Dispose();
            tipWindow = null;
            tipTimer.Dispose();
            tipTimer = null;

            this.Dispose();
            return resultItem;
        }

        /// <summary>
        /// Start counting tooltip time from the begining. THis function is called when the mouse
        /// point is moved.
        /// </summary>
        private void ResetTipTimer() {
            if (tipWindow != null && tipTimer != null && !tipWindow.Visible) {
                tipTimer.Stop();
                tipTimer.Interval = tipTime;
                tipTimer.Start();
            }
        }

        /// <summary>
        /// Draw or erase a selection rectanle
        /// </summary>
        /// <param name="g">Graphics object to use</param>
        /// <param name="select">If true selection is drawn, if false it is erased</param>
        private void DrawSelection(Graphics g, bool select) {
            if (hilightedEntry != null) {
                Rectangle rectHighlighted = hilightedEntry.Bounds;
                Pen p = new(select ? Color.Red : this.BackColor);

                rectHighlighted.Inflate(new Size(2, 2));
                rectHighlighted.Width--;
                rectHighlighted.Height--;

                g.DrawRectangle(p, rectHighlighted);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.DrawLine(Pens.DarkGray, 0, 0, this.Width - 1, 0);
            e.Graphics.DrawLine(Pens.DarkGray, 0, 0, 0, this.Height - 1);
            e.Graphics.DrawLine(Pens.SlateGray, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(Pens.SlateGray, 0, this.Height - 1, this.Width - 1, this.Height - 1);

            if (selectedCategory != null) {
                foreach (ImageMenuItem item in selectedCategory.Items)
                    item.PaintEntry(e.Graphics);
            }

            if (categoriesVisible)
                foreach (ImageMenuCategory category in Categories)
                    category.PaintEntry(e.Graphics);

            // Highlight the selected category
            if (selectedCategory != null) {
                Rectangle r = selectedCategory.Bounds;

                r.Location += new Size(-1, -1);
                r.Size = new Size(r.Width + 1, r.Height + 1);

                e.Graphics.DrawRectangle(Pens.Blue, r);
            }

            DrawSelection(e.Graphics, true);
        }

        /// <summary>
        /// Find out which item is pointed by the mouse.
        /// </summary>
        /// <param name="p">The mouse point</param>
        /// <returns>The ImageMenuEntry under the mouse or null if none found</returns>
        private ImageMenuEntry? GetEntryAtPoint(Point p) {
            if (selectedCategory != null) {
                foreach (ImageMenuItem item in selectedCategory.Items)
                    if (item.Bounds.Contains(p))
                        return item;
            }

            foreach (ImageMenuCategory category in Categories)
                if (category.Bounds.Contains(p))
                    return category;

            return null;
        }

        /// <summary>
        /// Update the selected item (if needed)
        /// </summary>
        /// <param name="p">Mouse hit point</param>
        /// <param name="hideHighlightAllowed">What to do with the selection if the mouse if not on an item</param>
        private void UpdateSelection(Point p, bool hideHighlightAllowed) {
            var hitEntry = GetEntryAtPoint(p);

            // If mouse moved from the item for which there is an open tooltip, close the tooltip.
            if (tippedEntry != null && hitEntry != tippedEntry) {
                tippedEntry = null;
                tipWindow?.HideTip();
                ResetTipTimer();
            }

            if (hitEntry != null) {
                if (hitEntry != hilightedEntry) {
                    Graphics g = CreateGraphics();

                    DrawSelection(g, false);
                    hilightedEntry = hitEntry;
                    DrawSelection(g, true);

                    g.Dispose();
                }
            }
            else if (hideHighlightAllowed) {
                Graphics g = CreateGraphics();

                DrawSelection(g, false);
                hilightedEntry = null;
            }
        }

        private void SelectEntry(ImageMenuEntry newEntry) {
            Graphics g = CreateGraphics();

            DrawSelection(g, false);
            hilightedEntry = newEntry;
            DrawSelection(g, true);

            g.Dispose();
        }

        /// <summary>
        /// Called when the tooltip timer ticks. This will happend if the mouse did not move for a period of time
        /// If the mouse points to an item with a tooltip, show the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TipTimer_tick(Object? sender, EventArgs e) {
            var hitEntry = GetEntryAtPoint(PointToClient(Control.MousePosition));

            if (hitEntry != null && hitEntry.Tooltip != null) {
                tippedEntry = hitEntry;

                if (tipWindow != null) {
                    tipWindow.TipText = hitEntry.Tooltip;
                    tipWindow.ShowTip(Parent.PointToClient(Control.MousePosition) + new Size(5, 16));
                }

                tipTimer?.Stop();
            }
        }

        /// <summary>
        /// If the mouse is down outside the menu, the menu is hidden, and Show will
        /// return when the mouse is up (to avoid a click even on the outer form)
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e) {
            this.Capture = true;
            base.OnMouseDown(e);

            ResetTipTimer();

            if (!ClientRectangle.Contains(new Point(e.X, e.Y))) {
                menuState = MenuState.Closing;
                shadow?.Close();
                this.Visible = false;
            }
        }

        /// <summary>
        /// If the mouse is up, and it was down inside the menu, set the result to be
        /// the currently selected item and close the menu
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e) {
            this.Capture = true;
            base.OnMouseUp(e);

            tipWindow?.Hide();
            ResetTipTimer();

            if (menuState != MenuState.Closing) {
                var hitEntry = GetEntryAtPoint(new Point(e.X, e.Y));

                if (hitEntry != null) {
                    if (hitEntry is ImageMenuItem hitMenuItem) {
                        resultItem = hitMenuItem;
                        shadow?.Close();
                        this.Visible = false;
                        menuState = MenuState.Closed;
                    }
                    else {
                        if (hitEntry is ImageMenuCategory hitCategory) {
                            SelectCategory(hitCategory);
                            UpdateSelection(PointToClient(Control.MousePosition), true);
                        }
                    }
                }
            }
            else
                menuState = MenuState.Closed;
        }

        /// <summary>
        /// Move the selection to follow the mouse
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e) {
            this.Capture = true;
            base.OnMouseMove(e);

            ResetTipTimer();
            UpdateSelection(new Point(e.X, e.Y), false);
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            switch (e.KeyCode) {
                case Keys.ShiftKey:
                    if (shiftKeyCategory != null && SelectedCategory != shiftKeyCategory) {
                        beforeShiftCategory = SelectedCategory;
                        SelectCategory(shiftKeyCategory);
                    }
                    break;

                case Keys.Escape:
                    shadow?.Close();
                    this.Visible = false;
                    menuState = MenuState.Closed;
                    break;

                case Keys.Right: {
                        if (hilightedEntry is ImageMenuItem item && selectedCategory != null) {
                            int iItem = selectedCategory.Items.IndexOf(item);

                            if (++iItem >= selectedCategory.Items.Count)
                                iItem = 0;

                            SelectEntry(selectedCategory.Items[iItem]);
                        }
                        break;
                    }

                case Keys.Left: {
                        if (hilightedEntry is ImageMenuItem item && selectedCategory != null) {
                            int iItem = selectedCategory.Items.IndexOf(item);

                            if (iItem == 0)
                                iItem = selectedCategory.Items.Count - 1;
                            else
                                iItem--;

                            SelectEntry(selectedCategory.Items[iItem]);
                        }
                        break;
                    }

                case Keys.Space: {
                        if (hilightedEntry is ImageMenuItem item) {
                            resultItem = item;
                            shadow?.Close();
                            this.Visible = false;
                            menuState = MenuState.Closed;
                        }
                        break;
                    }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.ShiftKey) {
                if (beforeShiftCategory != null) {
                    SelectCategory(beforeShiftCategory);
                    beforeShiftCategory = null;
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            this.Capture = true;
            shadow?.Close();
            this.Visible = false;
            menuState = MenuState.Closed;
            base.OnMouseWheel(e);
        }

        protected override bool IsInputKey(Keys keyData) => true;
    }
}
