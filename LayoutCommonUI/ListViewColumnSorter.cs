using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LayoutManager.CommonUI {

	/// <summary>
	/// Use this to add functionallity of sorting columns by clicking on their headers
	/// To use simply declare: sorter = ListViewStringColumnSorter(theListToSort);
	/// </summary>
	public class ListViewStringColumnsSorter : IComparer {
		enum SortOrder { Ascending, Decending };

		ListView		listView;
		SortOrder[]		orders;
		SortOrder		sortOrder = SortOrder.Ascending;
		int				sortedColumn = 0;

		public ListViewStringColumnsSorter(ListView listView) {
			this.listView = listView;
			orders = new SortOrder[listView.Columns.Count];

			for(int i = 0; i < orders.Length; i++)
				orders[i] = SortOrder.Ascending;

			listView.ListViewItemSorter = this;
			listView.ColumnClick += new ColumnClickEventHandler(this.listView_ColumnClick);
		}

		private void listView_ColumnClick(object sender, ColumnClickEventArgs e) {
			if(sortedColumn == e.Column)
				orders[sortedColumn] = (sortOrder == SortOrder.Ascending) ? SortOrder.Decending : SortOrder.Ascending;

			sortedColumn = e.Column;
			sortOrder = orders[sortedColumn];
			listView.Sort();
		}

		public virtual int Compare(object oItem1, object oItem2) {
			ListViewItem	item1 = (ListViewItem)oItem1;
			ListViewItem	item2 = (ListViewItem)oItem2;

			int	result = String.Compare(item1.SubItems[sortedColumn].Text, item2.SubItems[sortedColumn].Text);

			if(sortOrder == SortOrder.Decending)
				result *= -1;

			return result;
		}
	}
}
