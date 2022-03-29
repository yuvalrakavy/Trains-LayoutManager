﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// List box showing a tree-like representation of owner draw
    /// items. Each item is based on XML element. Each tree node is basically an XPath
    /// query that represent the items (leafs) shown when the node is expanded.
    /// A node may contains sub-nodes. You should derive specialized controls from this
    /// control in order to customize for the specific item.
    /// </summary>
    public partial class XmlQueryList : UserControl {
        private XmlElement? containerElement;
        private XPathNavigator? containerNavigator;
        private readonly List<XmlQueryListLayout> layouts = new();
        private XmlQueryListLayout? currentLayout;
        private int canUpdateNesting;

        public XmlQueryList() {
            InitializeComponent();
            this.CreateQueryItem = () => new XmlQueryListItem();

            Root = CreateQueryItem();
            Root.List = this;
            Root.Expand();

        }

        #region public properties/methods

        /// <summary>
        /// The XML element that contains all the elements in the list. Each query
        /// node in the list represents an XPath select query that selects elements
        /// from this container. You must set this property when the control
        /// is initialized.
        /// </summary>
        [Browsable(false)]
        public XmlElement? ContainerElement {
            get {
                return containerElement;
            }

            set {
                bool needToUpdate = false;

                if (containerElement != null)
                    needToUpdate = true;

                containerElement = value;
                if (containerElement != null) {
                    containerNavigator = containerElement.CreateNavigator();
                    containerElement.OwnerDocument.NodeInserted += this.OnContainerDocumentChanged;
                    containerElement.OwnerDocument.NodeRemoved += this.OnContainerDocumentChanged;

                    if (needToUpdate) {
                        UpdateList();
                        if (CurrentListLayout != null)
                            ApplyLayout(CurrentListLayout);
                    }
                }
            }
        }

        /// <summary>
        /// Create an object that represents an item in the list. The object must implement
        /// the IXmlQueryListboxItem interface
        /// </summary>
        /// <param name="queryItem">The query under which this item is created</param>
        /// <param name="itemElement">The XML element representing the item</param>
        /// <returns>The new item object</returns>
        public Func<XmlQueryListItem, XmlElement, IXmlQueryListItem>? CreateItem;

        /// <summary>
        /// Return the XPathNavigator of the container element
        /// </summary>
        public XPathNavigator? ContainerNavigator => containerNavigator;

        /// <summary>
        /// Create a new query node item. You should override this method if you define
        /// your own customized version of query node items
        /// </summary>
        /// <returns>The new query item object</returns>
        public Func<XmlQueryListItem> CreateQueryItem;

        public XPathNodeIterator? ProcessQuery(XPathExpression expression) => ContainerNavigator?.Select(expression);

        [Browsable(false)]
        public ListBox ListBox => this.listBox;

        /// <summary>
        /// Default double click behavior. If the selected item is a query item, toggle
        /// its expansion state
        /// </summary>
        /// <param name="e"></param>
        private void OnDoubleClick(object? sender, EventArgs e) {
            if (listBox.SelectedItem is XmlQueryListItem selected) {
                if (selected.IsExpanded)
                    selected.Collapse();
                else
                    selected.Expand();

                listBox.Invalidate();
            }
        }

        [Browsable(false)]
        public XmlQueryListLayout? CurrentListLayout {
            get {
                return currentLayout;
            }

            set {
                currentLayout = value;
                if (currentLayout != null)
                    ApplyLayout(currentLayout);
            }
        }

        [Browsable(false)]
        public int CurrentListLayoutIndex {
            get {
                return currentLayout != null ? layouts.IndexOf(currentLayout) : -1; 
            }

            set {
                if (value >= 0)
                    CurrentListLayout = (XmlQueryListLayout?)layouts[value];
            }
        }

        /// <summary>
        /// Add a query node to the list root. If query is null, this node will not
        /// include any items, it may still include other query nodes.
        /// </summary>
        /// <param name="name">The name of this node</param>
        /// <param name="query">The XPath query (may be null)</param>
        /// <returns>
        /// A reference to the query item. This reference can be used for
        /// adding sub-nodes.
        /// </returns>
        public XmlQueryListItem? AddQuery(String name, string? query) {
            XmlQueryListItem? q = null;

            if (Root != null) {
                q = Root.Add(name, query);
                if (DefaultSortField != null)
                    q.SortField = DefaultSortField;
                UpdateList();
            }

            return q;
        }

        [Browsable(false)]
        public XmlQueryListItem? Root { get; } = null;

        public ImageList ArrowsImageList => imageListArrows;

        public string? DefaultSortField { get; set; }

        /// <summary>
        /// A reference to the selected item. It will be null if no item is selected
        /// or the selected item is a query node. You can tell apart the two cases
        /// by checking whether this.SelectedItem is null (no item is selected) or not
        /// (in which case, the selected item is a query node).
        /// </summary>
        public Object? SelectedXmlItem => listBox.SelectedItem is not XmlQueryListItem ? listBox.SelectedItem : null;

        /// <summary>
        /// Check if the currently selected tree node is expanded or not
        /// </summary>
        /// <returns>True if it is expand, false otherwise</returns>
        /// <exception cref="ApplicationException">
        /// Thrown if the currently selected item is not a query tree node
        /// </exception>
        public bool IsSelectedExpanded() {
            if (listBox.SelectedItem != null) {
                if (listBox.SelectedItem is XmlQueryListItem item)
                    return item.IsExpanded;
                else
                    throw new ApplicationException("Selected item is not a query item");
            }
            return false;
        }

        public void DisableUpdate() {
            canUpdateNesting++;
        }

        public void EnableUpdate() {
            if (--canUpdateNesting == 0)
                UpdateList();
        }

        /// <summary>
        /// Update the list to reflect changes in items or in the container items.
        /// You should call this method after performing such updates (for example,
        /// adding items or removing items)
        /// </summary>
        public void UpdateList() {
            object? selectedBookmark = null;
            object? parentBookmark = null;

            if (listBox.SelectedItem != null)
                selectedBookmark = ((IXmlQueryListItem)listBox.SelectedItem).Bookmark;

            for (int i = listBox.SelectedIndex - 1; i >= 0; i--) {
                if (listBox.Items[i] is XmlQueryListItem item) {
                    parentBookmark = item.Bookmark;
                    break;
                }
            }

            listBox.BeginUpdate();
            listBox.Items.Clear();
            Root?.Expand();

            // Try to locate the selected item based on the bookmarks
            int parentIndex = LocateBookmark(0, parentBookmark);
            int selectIndex = -1;

            if (parentIndex >= 0)
                selectIndex = LocateBookmark(parentIndex, selectedBookmark);

            if (selectIndex < 0)
                selectIndex = LocateBookmark(0, selectedBookmark);

            if (selectIndex >= 0)
                listBox.SelectedIndex = selectIndex;

            listBox.EndUpdate();
        }

        /// <summary>
        /// Toggle the expansion state of the current selected query node.
        /// </summary>
        /// <exception cref="ApplicationException">
        /// If selected node is not query node
        /// </exception>
        public void ToggleSelectedExpansion() {
            if (listBox.SelectedItem is not XmlQueryListItem q)
                throw new ApplicationException("Selected item must be a query node item");

            if (q.IsExpanded)
                q.Collapse();
            else
                q.Expand();
        }

#endregion

        #region Internal methods

        /// <summary>
        /// Locate an item with a given bookmark
        /// </summary>
        /// <param name="index">Where to start searching from</param>
        /// <param name="bookmark">The bookmark</param>
        /// <returns>The index of an item with the bookmark or -1 if no item found</returns>
        private int LocateBookmark(int index, object? bookmark) {
            if (bookmark != null) {
                for (int i = index; i < listBox.Items.Count; i++)
                    if (((IXmlQueryListItem)listBox.Items[i]).IsBookmarkEqual(bookmark))
                        return i;
            }

            return -1;
        }

        /// <summary>
        /// Dispatch the owner draw message to the items
        /// </summary>
        /// <param name="e"></param>
        private void OnDrawItem(object? sender, DrawItemEventArgs e) {
            if (!DesignMode && e.Index >= 0) {
                IXmlQueryListItem item = (IXmlQueryListItem)listBox.Items[e.Index];

                item.Draw(e);
            }
        }

        /// <summary>
        /// Dispatch the measure item message to the items
        /// </summary>
        /// <param name="e"></param>
        private void OnMeasureItem(object? sender, MeasureItemEventArgs e) {
            if (!DesignMode && e.Index >= 0) {
                IXmlQueryListItem item = (IXmlQueryListItem)listBox.Items[e.Index];

                item.Measure(e);
            }
        }

        private void OnContainerDocumentChanged(object? sender, XmlNodeChangedEventArgs e) {
            if (canUpdateNesting == 0 && e.Node != null) {
                if (e.Node.NodeType == XmlNodeType.Element && (e.NewParent == (XmlNode?)containerElement || e.OldParent == (XmlNode?)containerElement))
                    UpdateList();
                else {
                    for (XmlNode? p = e.NewParent; p != null; p = p.ParentNode) {
                        if (p == (XmlNode?)containerElement) {
                            Invalidate();
                            break;
                        }
                    }
                }
            }
        }

        #endregion

        #region ListLayout methods and classes

        public void AddLayout(XmlQueryListLayout layout) {
            layouts.Add(layout);
        }

        public XmlQueryListLayout[] Layouts => (XmlQueryListLayout[])layouts.ToArray();

        protected void ApplyLayout(XmlQueryListLayout layout) {
            Root?.ClearItems();
            layout.ApplyLayout(this);
            UpdateList();
        }

        protected void ApplyLayout(int iLayout) {
            ApplyLayout(Layouts[iLayout]);
        }

        public void AddLayoutMenuItems(MenuOrMenuItem m) {
            foreach (XmlQueryListLayout layout in Layouts)
                m.Items.Add(new XmlLayoutMenuItem(this, layout));
        }


        private class XmlLayoutMenuItem : LayoutMenuItem {
            private readonly XmlQueryList list;
            private readonly XmlQueryListLayout layout;

            public XmlLayoutMenuItem(XmlQueryList list, XmlQueryListLayout layout) {
                this.list = list;
                this.layout = layout;

                this.Text = layout.LayoutName;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                list.CurrentListLayout = layout;
            }
        }

        #endregion

    }

    /// <summary>
    /// An interface that must be implemented by a class that represents an item in
    /// a XmlQueryListbox
    /// </summary>
    public interface IXmlQueryListItem {
        /// <summary>
        /// Draw the item.
        /// </summary>
        /// <param name="e">The event arguments for OnOwnerDraw event</param>
        void Draw(DrawItemEventArgs e);

        /// <summary>
        /// Measure (return) the drawn item size
        /// </summary>
        /// <param name="e">The event arguments for OnMeasureItem event</param>
        void Measure(MeasureItemEventArgs e);

        /// <summary>
        /// A bookmark that represent the item. If two items have the same bookmark
        /// then they should refer to the same data. Since class that represents Xml
        /// element are created and deleted when the list is collapsed/expanded, instances
        /// of item classes cannot be used for comparison. A common implementation for
        /// this property is to return a reference to the item's underlying XML element object.
        /// Bookmarks are used for example, to relocate the selected element after updating the
        /// list.
        /// </summary>
        object Bookmark { get; }

        /// <summary>
        /// Check if a given bookmark is the same as the current instance bookmark
        /// </summary>
        /// <param name="bookmark">The bookmark to check</param>
        /// <returns>True if the bookmark is for this instance, false otherwise</returns>
        bool IsBookmarkEqual(object bookmark);
    }

    public interface IXmlQueryListBoxXmlElementItem : IXmlQueryListItem {
        /// <summary>
        /// Return the XML element represented by the item
        /// </summary>
        XmlElement Element {
            get;
        }
    }

    /// <summary>
    /// Base class for XmlQueryListbox layout. A layout is a set of query items that
    /// define what and how items are shown in the list box.
    /// </summary>
    abstract public class XmlQueryListLayout {
        /// <summary>
        /// Apply the layout on a given list
        /// </summary>
        /// <param name="list">The list on which the layout should be applied</param>
        public abstract void ApplyLayout(XmlQueryList list);

        /// <summary>
        /// Return a display name for the layout. For example: "Locomotive by Origin"
        /// </summary>
        public abstract string LayoutName {
            get;
        }
    }

#region Default implementation of Query node item

    public class XmlQueryListItem : IXmlQueryListItem {
        protected int level;
        protected bool expanded = false;
        protected XmlQueryList? list;
        protected string? query = null;
        protected XPathExpression? compiledQuery = null;
        protected List<XmlQueryListItem> items = new();
        protected XmlQueryListItem? parent = null;
        protected string? name = null;
        protected string? sortField = null;

        /// <summary>
        /// Add a sub-query. The added query will not have any items. It may have
        /// sub-queries
        /// </summary>
        /// <param name="name">The sub-query name</param>
        /// <returns>The added sub-query object</returns>
        public XmlQueryListItem Add(String name) {
            XmlQueryListItem q = List.CreateQueryItem();

            q.Initialize(List, name, null, level + 1, this);
            items.Add(q);
            return q;
        }

        public string Query {
            get {
                return Ensure.NotNull<string>(query, nameof(query));
            }

            set {
                query = value;
                if (query != null)
                    this.compiledQuery = List.ContainerNavigator?.Compile(query);
            }
        }

        /// <summary>
        /// Add a subquery.
        /// </summary>
        /// <param name="name">The query item name</param>
        /// <param name="query">
        /// XPath query defining the items to display when query node is expanded
        /// </param>
        /// <returns>The added query item object</returns>
        public XmlQueryListItem Add(String name, string? query) {
            XmlQueryListItem q = List.CreateQueryItem();

            q.Initialize(List, name, query, level + 1, this);

            if (List.DefaultSortField != null)
                q.SortField = List.DefaultSortField;

            items.Add(q);
            return q;
        }

        /// <summary>
        /// Array with all the sub-queries
        /// </summary>
        public List<XmlQueryListItem> Items => items;

        /// <summary>
        /// Remove all sub-queries
        /// </summary>
        public void ClearItems() {
            items.Clear();
        }

        /// <summary>
        /// Is current query item expanded?
        /// </summary>
        public bool IsExpanded => expanded;

        /// <summary>
        /// Current query item nesting level
        /// </summary>
        public int Level => level;

        public string? SortField {
            get {
                return sortField;
            }

            set {
                sortField = value;
                if (compiledQuery != null && sortField != null)
                    compiledQuery.AddSort(sortField, XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
            }
        }

        /// <summary>
        /// The XmlQueryListBox owning this query item
        /// </summary>
        public XmlQueryList List {
            get {
                return Ensure.NotNull<XmlQueryList>(list, nameof(list));
            }

            set {
                list = value;
            }
        }

        /// <summary>
        /// Expand the current query node showing all subqueries nodes and
        /// all items that are the result of applying the XPath query on the
        /// container XML element.
        /// </summary>
        /// <returns></returns>
        public int Expand() {
            var l = List.ListBox;
            int insertion = l.Items.IndexOf(this) + 1;

            l.BeginUpdate();

            foreach (XmlQueryListItem item in items) {
                l.Items.Insert(insertion++, item);

                if (item.IsExpanded)
                    insertion = item.Expand();
            }

            if (query != null && List.ContainerElement != null) {
                var resultIterator = List.ProcessQuery(Ensure.NotNull<XPathExpression>(compiledQuery));

                if (resultIterator != null) {
                    while (resultIterator.MoveNext()) {
                        var element = (XmlElement?)((IHasXmlNode?)resultIterator.Current)?.GetNode();

                        if (element != null) {
                            var item = List.CreateItem?.Invoke(this, element);

                            if (item != null)
                                l.Items.Insert(insertion++, item);
                        }
                    }
                }
            }

            expanded = true;
            l.EndUpdate();

            return insertion;
        }

        /// <summary>
        /// Collapse the query item.
        /// </summary>
        public void Collapse() {
            var l = List.ListBox;

            int index = l.Items.IndexOf(this) + 1;

            l.BeginUpdate();

            while (index < l.Items.Count && (l.Items[index] is not XmlQueryListItem || ((XmlQueryListItem)l.Items[index]).Level > level))
                l.Items.RemoveAt(index);

            l.EndUpdate();

            expanded = false;
        }

        /// <summary>
        /// Initialize the query item
        /// </summary>
        /// <param name="list">The list to which this item belongs to</param>
        /// <param name="name">The item name</param>
        /// <param name="query">The XPath query for getting items to be displayed</param>
        /// <param name="level">The nesting level of this item</param>
        /// <param name="parent">The query item under which this item is inserted</param>
        protected void Initialize(XmlQueryList list, string name, string? query, int level, XmlQueryListItem parent) {
            this.list = list;
            this.name = name;
            this.query = query;
            this.level = level;
            this.parent = parent;

            if (query != null && list.ContainerNavigator != null)
                this.compiledQuery = list.ContainerNavigator.Compile(query);
        }

        //
        // Default drawing code for query items.
        //

        public virtual void Measure(MeasureItemEventArgs e) {
            e.ItemHeight = 28;
        }

        public virtual void DrawLevelLines(DrawItemEventArgs e) {
            using Pen p = new(Brushes.DarkCyan, 2);
            for (int l = 0; l < level; l++) {
                int x = e.Bounds.Left + 11 + (16 * l);

                e.Graphics.DrawLine(p, x, e.Bounds.Top, x, e.Bounds.Bottom);
            }
        }

        public virtual void DrawExpandCollapse(DrawItemEventArgs e) {
            int x = e.Bounds.Left + 11 + (16 * (level - 1));
            int y = e.Bounds.Top + ((e.Bounds.Height - 16) / 2) - 2;

            List.ArrowsImageList.Draw(e.Graphics, x - 8, y + 1, IsExpanded ? 1 : 0);
        }

        public virtual void Draw(DrawItemEventArgs e) {
            if ((e.State & DrawItemState.Selected) != 0)
                e.DrawBackground();
            else
                e.Graphics.FillRectangle(Brushes.Cyan, e.Bounds);

            DrawLevelLines(e);
            DrawExpandCollapse(e);

            using Brush textBrush = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.WindowText);
            using Font font = new("Arial", 10, FontStyle.Bold);
            SizeF textSize = e.Graphics.MeasureString(name, font);

            StringFormat format = new();
            int leftMargin = e.Bounds.Left + 4 + (16 * level) + 1;
            Rectangle textRect = new(new Point(leftMargin, e.Bounds.Top), new Size(e.Bounds.Width - leftMargin, e.Bounds.Height));

            format.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(name, font, textBrush, textRect, format);
        }

        public object Bookmark => this;

        public bool IsBookmarkEqual(object bookmark) => bookmark == this.Bookmark;
    }

#endregion

}