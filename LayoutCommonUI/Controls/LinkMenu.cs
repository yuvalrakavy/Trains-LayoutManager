namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LinkMenu.
    /// </summary>
    public class LinkMenu : LinkLabel {
        private readonly ContextMenuStrip menu = new();
        private int selectedIndex = -1;
        public event EventHandler? ValueChanged;

        public LinkMenu() {
        }

        public String[] Options {
            get {
                String[] values = new String[menu.Items.Count];

                for (int i = 0; i < values.Length; i++)
                    values[i] = menu.Items[i].Text;

                return values;
            }

            set {
                menu.Items.Clear();

                if (value != null) {
                    foreach (String text in value)
                        menu.Items.Add(new LinkMenuItem(this, text));
                }
            }
        }

        public int SelectedIndex {
            get {
                return selectedIndex;
            }

            set {
                if (value >= 0) {
                    if (value >= menu.Items.Count)
                        throw new ArgumentException("Invalid selected index");
                    selectedIndex = value;
                    Text = menu.Items[selectedIndex].Text;
                }
            }
        }

        protected ToolStripMenuItem SelectedItem {
            set {
                SelectedIndex = menu.Items.IndexOf(value);
            }
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            menu.Show(Parent, new Point(Left, Bottom));
        }

        private class LinkMenuItem : LayoutMenuItem {
            private readonly LinkMenu linkMenu;

            public LinkMenuItem(LinkMenu linkMenu, string text) {
                this.linkMenu = linkMenu;
                this.Text = text;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);
                linkMenu.SelectedItem = this;

                linkMenu.ValueChanged?.Invoke(linkMenu, EventArgs.Empty);
            }
        }
    }
}
