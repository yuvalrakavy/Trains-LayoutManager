using System;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LinkMenu.
    /// </summary>
    public class LinkMenu : System.Windows.Forms.LinkLabel
	{
		ContextMenu					menu = new ContextMenu();
		int							selectedIndex = -1;
		public event EventHandler	ValueChanged;

		public LinkMenu()
		{
		}

		public String[] Options {
			get {
				String[]	values = new String[menu.MenuItems.Count];

				for(int i = 0; i < values.Length; i++)
					values[i] = menu.MenuItems[i].Text;

				return values;
			}

			set {
				menu.MenuItems.Clear();

				if(value != null) {
					foreach(String text in value)
						menu.MenuItems.Add(new LinkMenuItem(this, text));
				}
			}
		}

		public int SelectedIndex {
			get {
				return selectedIndex;
			}

			set {
				if(value >= 0) {
					if(value >= menu.MenuItems.Count)
						throw new ArgumentException("Invalid selected index");
					selectedIndex = value;
					Text = menu.MenuItems[selectedIndex].Text;
				}
			}
		}

		protected MenuItem SelectedItem {
			set {
				SelectedIndex = menu.MenuItems.IndexOf(value);

			}
		}

		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			menu.Show(Parent, new Point(Left, Bottom));
		}

		class LinkMenuItem : MenuItem {
			LinkMenu	linkMenu;

			public LinkMenuItem(LinkMenu linkMenu, String text) {
				this.linkMenu = linkMenu;
				this.Text = text;
			}

			protected override void OnClick(EventArgs e) {
				base.OnClick(e);
				linkMenu.SelectedItem = this;

				if(linkMenu.ValueChanged != null)
					linkMenu.ValueChanged(linkMenu, null);
			}
		}
	}
}
