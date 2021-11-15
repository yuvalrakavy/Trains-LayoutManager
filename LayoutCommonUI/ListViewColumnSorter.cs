using System.Collections;

namespace LayoutManager.CommonUI {
    /// <summary>
    /// Use this to add functionallity of sorting columns by clicking on their headers
    /// To use simply declare: sorter = ListViewStringColumnSorter(theListToSort);
    /// </summary>
    public class ListViewStringColumnsSorter : IComparer {
        private enum SortOrder { Ascending, Decending };

        private readonly ListView listView;
        private readonly SortOrder[] orders;
        private SortOrder sortOrder = SortOrder.Ascending;
        private int sortedColumn = 0;

        public ListViewStringColumnsSorter(ListView listView) {
            this.listView = listView;
            orders = new SortOrder[listView.Columns.Count];

            for (int i = 0; i < orders.Length; i++)
                orders[i] = SortOrder.Ascending;

            listView.ListViewItemSorter = this;
            listView.ColumnClick += this.ListView_ColumnClick;
        }

        private void ListView_ColumnClick(object? sender, ColumnClickEventArgs e) {
            if (sortedColumn == e.Column)
                orders[sortedColumn] = (sortOrder == SortOrder.Ascending) ? SortOrder.Decending : SortOrder.Ascending;

            sortedColumn = e.Column;
            sortOrder = orders[sortedColumn];
            listView.Sort();
        }

        public virtual int Compare(object? oItem1, object? oItem2) {
            var item1 = Ensure.NotNull<ListViewItem>(oItem1, nameof(oItem1));
            var item2 = Ensure.NotNull<ListViewItem>(oItem2, nameof(oItem2));

            int result = String.Compare(item1.SubItems[sortedColumn].Text, item2.SubItems[sortedColumn].Text);

            if (sortOrder == SortOrder.Decending)
                result *= -1;

            return result;
        }
    }
}
