using System.Xml;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfTime.
    /// </summary>
    public partial class IfTime : Form {

        /// <summary>
        /// Required designer variable.
        /// </summary>

        private readonly XmlDocument workingDoc;
        private readonly XmlElement element;
        private readonly XmlElement inElement;

        public IfTime(XmlElement inElement) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.inElement = inElement;

            workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            element = (XmlElement)workingDoc.ImportNode(inElement, true);
            workingDoc.AppendChild(element);

            int i = 0;
            foreach (string timeConstraintName in new string[] { "Seconds", "Minutes", "Hours", "DayOfWeek" }) {
                var nodes = Dispatch.Call.ParseIfTimeElement(element, timeConstraintName);

                if (nodes.Length > 0)
                    treeView.Nodes.Add(new TimeSectionTreeNode(new string[] { "Seconds", "Minutes", "Hours", "Day of week" }[i],
                        new Type[] { typeof(SecondsTreeNode), typeof(MinutesTreeNode), typeof(HoursTreeNode), typeof(DayOfWeekTreeNode) }[i], nodes));
                i++;
            }
        }

        private TimeSectionTreeNode? FindTimeSection(Type treeNodeType) {
            foreach (TimeSectionTreeNode timeSection in treeView.Nodes)
                if (treeNodeType.IsInstanceOfType(timeSection.Nodes[0]))
                    return timeSection;
            return null;
        }

        private void Insert(string nodeElementName, string timeSectionTitle, Type treeNodeType) {
            XmlElement nodeElement = element.OwnerDocument.CreateElement(nodeElementName);
            var node = Dispatch.Call.AllocateIfTimeNode(nodeElement);

            node.Value = 0;
            TreeNodeBase treeNode = Ensure.NotNull<TreeNodeBase>(Activator.CreateInstance(treeNodeType, new object[] { node }));

            if (treeNode.Edit()) {
                var timeSection = FindTimeSection(treeNodeType);

                element.AppendChild(nodeElement);
                treeNode.Text = node.Description;

                if (timeSection == null) {
                    timeSection = new TimeSectionTreeNode(timeSectionTitle);
                    treeView.Nodes.Add(timeSection);
                }

                timeSection.Nodes.Add(treeNode);
                timeSection.ExpandAll();
            }
        }

        private void UpdateButtons() {
            if (treeView.SelectedNode is TimeSectionTreeNode)
                buttonEdit.Enabled = false;
            else if (treeView.SelectedNode is TreeNodeBase)
                buttonEdit.Enabled = true;

            buttonDelete.Enabled = treeView.SelectedNode != null;
        }


        private void ButtonAdd_Click(object? sender, EventArgs e) {
            contextMenuAdd.Show(buttonAdd.Parent, new Point(buttonAdd.Left, buttonAdd.Bottom));
        }

        private void MenuItemSeconds_Click(object? sender, EventArgs e) {
            Insert("Seconds", "Seconds", typeof(SecondsTreeNode));
        }

        private void MenuItemMinutes_Click(object? sender, EventArgs e) {
            Insert("Minutes", "Minutes", typeof(MinutesTreeNode));
        }

        private void MenuItemHours_Click(object? sender, EventArgs e) {
            Insert("Hours", "Hours", typeof(HoursTreeNode));
        }

        private void MenuItemDayOfWeek_Click(object? sender, EventArgs e) {
            Insert("DayOfWeek", "Day of week", typeof(DayOfWeekTreeNode));
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            inElement.RemoveAll();

            foreach (XmlElement nodeElement in element) {
                XmlElement inNodeElement = (XmlElement)inElement.OwnerDocument.ImportNode(nodeElement, true);

                inElement.AppendChild(inNodeElement);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonEdit_Click(object? sender, EventArgs e) {
            if (treeView.SelectedNode is TreeNodeBase selected) {
                if (selected.Edit())
                    selected.Text = selected.Node.Description;
            }
        }

        private void ButtonDelete_Click(object? sender, EventArgs e) {
            TimeSectionTreeNode timeSection;

            if (treeView.SelectedNode is TimeSectionTreeNode timeSectionNode) {
                timeSection = timeSectionNode;

                foreach (TreeNodeBase treeNode in timeSection.Nodes)
                    element.RemoveChild(treeNode.Element);
                timeSection.Nodes.Clear();
            }
            else if (treeView.SelectedNode is TreeNodeBase treeNode) {
                timeSection = (TimeSectionTreeNode)treeNode.Parent;
                element.RemoveChild(treeNode.Node.Element);
                timeSection.Nodes.Remove(treeNode);
            }
            else
                throw new ApplicationException("Unexpected tree node type");

            if (timeSection.Nodes.Count == 0)
                treeView.Nodes.Remove(timeSection);
        }

        private void TreeView_AfterSelect(object? sender, TreeViewEventArgs e) {
            UpdateButtons();
        }

        private void TreeView_DoubleClick(object? sender, EventArgs e) {
            buttonEdit.PerformClick();
        }

        private class TimeSectionTreeNode : TreeNode {
            public TimeSectionTreeNode(string title, Type treeNodeClass, IIfTimeNode[] nodes) {
                Text = title;

                if (nodes != null) {
                    foreach (IIfTimeNode node in nodes) {
                        TreeNode treeNode = Ensure.NotNull<TreeNode>(Activator.CreateInstance(treeNodeClass, new object[] { node }));

                        Nodes.Add(treeNode);
                    }
                }

                ExpandAll();
            }

            public TimeSectionTreeNode(string title) {
                Text = title;
            }
        }

        private abstract class TreeNodeBase : TreeNode, IObjectHasXml {
            protected TreeNodeBase(IIfTimeNode node) {
                this.Node = node;
                Text = node.Description;
            }

            public XmlElement Element => Node.Element;
            public XmlElement? OptionalElement => Element;

            public IIfTimeNode Node { get; }

            abstract public bool Edit();
        }

        private class SecondsTreeNode : TreeNodeBase {
            public SecondsTreeNode(IIfTimeNode node) : base(node) {
            }

            public override bool Edit() {
                var d = new IfTimeNumericNode("Seconds", Node, 0, 59);

                return d.ShowDialog() == DialogResult.OK;
            }
        }

        private class MinutesTreeNode : TreeNodeBase {
            public MinutesTreeNode(IIfTimeNode node) : base(node) {
            }

            public override bool Edit() {
                var d = new IfTimeNumericNode("Minutes", Node, 0, 59);

                return d.ShowDialog() == DialogResult.OK;
            }
        }

        private class HoursTreeNode : TreeNodeBase {
            public HoursTreeNode(IIfTimeNode node) : base(node) {
            }

            public override bool Edit() {
                var d = new IfTimeNumericNode("Hours", Node, 0, 23);

                return d.ShowDialog() == DialogResult.OK;
            }
        }

        private class DayOfWeekTreeNode : TreeNodeBase {
            public DayOfWeekTreeNode(IIfTimeNode node) : base(node) {
            }

            public override bool Edit() {
                var d = new IfTimeDayOfWeekNode("Day Of Week", Node);

                return d.ShowDialog() == DialogResult.OK;
            }
        }
    }
}
