using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for XmlInfoEditor.
    /// </summary>
    public partial class XmlInfoEditor : UserControl {

        private LayoutXmlInfo? xmlInfo = null;

        public XmlInfoEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call

        }

        public LayoutXmlInfo? XmlInfo {
            get {
                return xmlInfo;
            }

            set {
                xmlInfo = value;
                if (xmlInfo != null) {
                    AddNode(treeViewElements.Nodes, xmlInfo.DocumentElement);
                    treeViewElements.ExpandAll();
                }
            }
        }

        private void AddNode(TreeNodeCollection nodes, XmlNode xmlNode) {
            if (xmlNode != null) {
                ElementTreeNode treeNode = new(xmlNode);

                nodes.Add(treeNode);
                foreach (XmlNode xmlChild in xmlNode.ChildNodes)
                    AddNode(treeNode.Nodes, xmlChild);
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

       
        private void ButtonNewElement_Click(object? sender, EventArgs e) {
            contextMenuAddElement.Show(this, new Point(buttonNewElement.Bounds.Left, buttonNewElement.Bounds.Bottom));
        }

        private void ButtonEditElement_Click(object? sender, EventArgs e) {
            ElementTreeNode treeNode = (ElementTreeNode)treeViewElements.SelectedNode;

            if (treeNode != null) {
                var newName = Dialogs.InputBox.Show("Edit node", treeNode.IsTextNode ? "Enter node text" : "Enter node name");

                if (newName != null)
                    treeNode.Text = newName;
            }
        }

        private void MenuItemAddElement_Click(object? sender, EventArgs e) {
            ElementTreeNode selected = (ElementTreeNode)treeViewElements.SelectedNode;

            if (selected != null) {
                var name = Dialogs.InputBox.Show("Add Element", "Enter new element name:");
                XmlElement? element;

                if (name != null && (element = selected.XmlNode.OwnerDocument?.CreateElement(name)) != null) {
                    selected.XmlNode.AppendChild(element);
                    selected.Nodes.Add(new ElementTreeNode(element));
                    selected.Expand();
                }
            }
        }

        private void MenuItemText_Click(object? sender, EventArgs e) {
            ElementTreeNode selected = (ElementTreeNode)treeViewElements.SelectedNode;

            if (selected != null) {
                var text = Dialogs.InputBox.Show("Add Text", "Enter new text:");

                if (text != null) {
                    var textElement = selected.XmlNode.OwnerDocument?.CreateTextNode(text);

                    if (textElement != null) {
                        selected.XmlNode.AppendChild(textElement);
                        selected.Nodes.Add(new ElementTreeNode(textElement));
                        selected.Expand();
                    }
                }
            }
        }

        private void ButtonDeleteElement_Click(object? sender, EventArgs e) {
            ElementTreeNode selected = (ElementTreeNode)treeViewElements.SelectedNode;

            if (selected != null) {
                if (selected.Parent != null) {
                    ElementTreeNode parent = (ElementTreeNode)selected.Parent;

                    parent.XmlNode.RemoveChild(selected.XmlNode);
                    parent.Nodes.Remove(selected);
                }
            }
        }

        private void TreeViewElements_AfterSelect(object? sender, TreeViewEventArgs e) {
            ElementTreeNode selected = (ElementTreeNode)treeViewElements.SelectedNode;

            System.Diagnostics.Debug.WriteLine("Selection changed");

            listViewAttributes.Items.Clear();

            if (selected != null && !selected.IsTextNode) {
                foreach (XmlAttribute a in selected.XmlNode.Attributes!)
                    listViewAttributes.Items.Add(new AttributeItem(a));
            }
        }

        private void ButtonNewAttribute_Click(object? sender, EventArgs e) {
            if (SelectedNode != null && SelectedNode.NodeType != XmlNodeType.Text) {
                XmlInfoEditorForms.AttributeInfo attributeInfo = new();

                if (attributeInfo.ShowDialog(this) == DialogResult.OK && xmlInfo != null) {
                    XmlAttribute a = xmlInfo.XmlDocument.CreateAttribute(attributeInfo.AttributeName);

                    a.Value = attributeInfo.AttributeValue;

                    SelectedNode.Attributes?.Append(a);
                    listViewAttributes.Items.Add(new AttributeItem(a));
                }
            }
        }

        private void ButtonEditAttribute_Click(object? sender, EventArgs e) {
            if (listViewAttributes.SelectedItems.Count > 0) {
                AttributeItem selected = (AttributeItem)listViewAttributes.SelectedItems[0];
                XmlInfoEditorForms.AttributeInfo attributeInfo = new(selected.XmlAttribute);

                if (attributeInfo.ShowDialog(this) == DialogResult.OK) {
                    selected.AttributeName = attributeInfo.AttributeName;
                    selected.AttributeValue = attributeInfo.AttributeValue;
                }
            }
        }

        private void ButtonDeleteAttribute_Click(object? sender, EventArgs e) {
            if (listViewAttributes.SelectedItems.Count > 0) {
                AttributeItem selected = (AttributeItem)listViewAttributes.SelectedItems[0];

                SelectedNode?.Attributes?.Remove(selected.XmlAttribute);
                listViewAttributes.Items.Remove(selected);
            }
        }

        private XmlNode? SelectedNode {
            get {
                ElementTreeNode selected = (ElementTreeNode)treeViewElements.SelectedNode;

                return selected?.XmlNode;
            }
        }

        private class ElementTreeNode : TreeNode {
            private XmlNode node;

            internal ElementTreeNode(XmlNode node) {
                this.node = node;

                FormatNodeText();
            }

            public XmlNode XmlNode => node;

            public new string Text {
                get {
                    return node.NodeType == XmlNodeType.Text ? node.Value ?? String.Empty : node.Name;
                }

                set {
                    if (node.NodeType == XmlNodeType.Text)
                        node.Value = value;
                    else
                        RenameNode(value);

                    FormatNodeText();
                }
            }

            public bool IsTextNode => node.NodeType == XmlNodeType.Text;

            public void FormatNodeText() {
                if (node.NodeType == XmlNodeType.Text)
                    base.Text = "\"" + node.Value + "\"";
                else
                    base.Text = "<" + node.Name + ">";
            }

            /// <summary>
            /// Rename the node to a new name
            /// </summary>
            /// <remarks>
            /// Renaming a node is a bit tricky. The XmlNode name property is read only, so renaming
            /// boils down to creating a new node (with the new name), appending to it all the old node's
            /// children and attributes.
            /// </remarks>
            /// <param name="newName"></param>
            private void RenameNode(String newName) {
                XmlElement newElement = node.OwnerDocument!.CreateElement(newName);

                foreach (XmlNode n in node.ChildNodes) {
                    node.RemoveChild(n);
                    newElement.AppendChild(n);
                }

                // Copy attributes
                foreach (XmlAttribute a in node.Attributes!)
                    newElement.Attributes.Append(a);

                node.ParentNode?.ReplaceChild(newElement, node);
                node = newElement;
            }
        }

        private class AttributeItem : ListViewItem {
            private XmlAttribute attribute;

            internal AttributeItem(XmlAttribute attribute) {
                this.attribute = attribute;

                this.Text = attribute.Name;
                this.SubItems.Add(attribute.Value);
            }

            public XmlAttribute XmlAttribute => attribute;

            public string AttributeName {
                get {
                    return attribute.Name;
                }

                set {
                    if (attribute.Name != value) {
                        XmlAttribute newAttribute = attribute.OwnerDocument.CreateAttribute(value);
                        var parent = attribute.ParentNode;

                        newAttribute.Value = attribute.Value;
                        parent?.Attributes?.Remove(attribute);
                        parent?.Attributes?.Append(newAttribute);
                        attribute = newAttribute;

                        this.Text = attribute.Name;
                    }
                }
            }

            public string AttributeValue {
                get {
                    return attribute.Value;
                }

                set {
                    attribute.Value = value;
                    this.SubItems[1].Text = value;
                }
            }
        }
    }
}
