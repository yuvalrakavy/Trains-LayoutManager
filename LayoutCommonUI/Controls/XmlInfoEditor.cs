using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for XmlInfoEditor.
	/// </summary>
	public class XmlInfoEditor : System.Windows.Forms.UserControl {
		private TreeView treeViewElements;
		private Button buttonNewElement;
		private Button buttonEditElement;
		private Button buttonDeleteElement;
		private ImageList imageListButtons;
		private ListView listViewAttributes;
		private Button buttonNewAttribute;
		private Button buttonEditAttribute;
		private Button buttonDeleteAttribute;
		private ColumnHeader columnHeaderAttributeName;
		private ColumnHeader columnHeaderAttributeValue;
		private Label labelElementAttributes;
		private System.Windows.Forms.ToolTip toolTips;
		private ContextMenu contextMenuAddElement;
		private MenuItem menuItemAddElement;
		private MenuItem menuItemText;
		private IContainer components;

		LayoutXmlInfo	xmlInfo = null;

		public XmlInfoEditor() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		public LayoutXmlInfo XmlInfo {
			get {
				return xmlInfo;
			}

			set {
				xmlInfo = value;
				if(xmlInfo != null)
					initialize();
			}
		}

		private void initialize() {
			addNode(treeViewElements.Nodes, xmlInfo.DocumentElement);
			treeViewElements.ExpandAll();
		}

		private void addNode(TreeNodeCollection nodes, XmlNode xmlNode) {
			if(xmlNode != null) {
				ElementTreeNode	treeNode = new ElementTreeNode(xmlNode);

				nodes.Add(treeNode);
				foreach(XmlNode xmlChild in xmlNode.ChildNodes)
					addNode(treeNode.Nodes, xmlChild);
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XmlInfoEditor));
			this.buttonDeleteElement = new Button();
			this.imageListButtons = new ImageList(this.components);
			this.columnHeaderAttributeValue = new ColumnHeader();
			this.listViewAttributes = new ListView();
			this.columnHeaderAttributeName = new ColumnHeader();
			this.toolTips = new System.Windows.Forms.ToolTip(this.components);
			this.buttonEditAttribute = new Button();
			this.buttonNewElement = new Button();
			this.buttonEditElement = new Button();
			this.buttonNewAttribute = new Button();
			this.buttonDeleteAttribute = new Button();
			this.treeViewElements = new TreeView();
			this.menuItemText = new MenuItem();
			this.contextMenuAddElement = new ContextMenu();
			this.menuItemAddElement = new MenuItem();
			this.labelElementAttributes = new Label();
			this.SuspendLayout();
			// 
			// buttonDeleteElement
			// 
			this.buttonDeleteElement.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonDeleteElement.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonDeleteElement.Image")));
			this.buttonDeleteElement.ImageIndex = 2;
			this.buttonDeleteElement.ImageList = this.imageListButtons;
			this.buttonDeleteElement.Location = new System.Drawing.Point(64, 180);
			this.buttonDeleteElement.Name = "buttonDeleteElement";
			this.buttonDeleteElement.Size = new System.Drawing.Size(24, 24);
			this.buttonDeleteElement.TabIndex = 3;
			this.toolTips.SetToolTip(this.buttonDeleteElement, "Delete element");
			this.buttonDeleteElement.Click += new System.EventHandler(this.buttonDeleteElement_Click);
			// 
			// imageListButtons
			// 
			this.imageListButtons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListButtons.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListButtons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButtons.ImageStream")));
			this.imageListButtons.TransparentColor = System.Drawing.Color.Silver;
			// 
			// columnHeaderAttributeValue
			// 
			this.columnHeaderAttributeValue.Text = "Value";
			this.columnHeaderAttributeValue.Width = 55;
			// 
			// listViewAttributes
			// 
			this.listViewAttributes.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.listViewAttributes.Columns.AddRange(new ColumnHeader[] {
																								 this.columnHeaderAttributeName,
																								 this.columnHeaderAttributeValue});
			this.listViewAttributes.FullRowSelect = true;
			this.listViewAttributes.GridLines = true;
			this.listViewAttributes.Location = new System.Drawing.Point(128, 36);
			this.listViewAttributes.Name = "listViewAttributes";
			this.listViewAttributes.Size = new System.Drawing.Size(110, 136);
			this.listViewAttributes.TabIndex = 5;
			this.listViewAttributes.View = System.Windows.Forms.View.Details;
			this.listViewAttributes.DoubleClick += new System.EventHandler(this.buttonEditAttribute_Click);
			// 
			// columnHeaderAttributeName
			// 
			this.columnHeaderAttributeName.Text = "Attribute";
			this.columnHeaderAttributeName.Width = 55;
			// 
			// buttonEditAttribute
			// 
			this.buttonEditAttribute.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonEditAttribute.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonEditAttribute.Image")));
			this.buttonEditAttribute.ImageIndex = 1;
			this.buttonEditAttribute.ImageList = this.imageListButtons;
			this.buttonEditAttribute.Location = new System.Drawing.Point(160, 180);
			this.buttonEditAttribute.Name = "buttonEditAttribute";
			this.buttonEditAttribute.Size = new System.Drawing.Size(24, 24);
			this.buttonEditAttribute.TabIndex = 7;
			this.toolTips.SetToolTip(this.buttonEditAttribute, "Edit attribute value");
			this.buttonEditAttribute.Click += new System.EventHandler(this.buttonEditAttribute_Click);
			// 
			// buttonNewElement
			// 
			this.buttonNewElement.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonNewElement.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonNewElement.Image")));
			this.buttonNewElement.ImageIndex = 0;
			this.buttonNewElement.ImageList = this.imageListButtons;
			this.buttonNewElement.Location = new System.Drawing.Point(8, 180);
			this.buttonNewElement.Name = "buttonNewElement";
			this.buttonNewElement.Size = new System.Drawing.Size(24, 24);
			this.buttonNewElement.TabIndex = 1;
			this.toolTips.SetToolTip(this.buttonNewElement, "Add element");
			this.buttonNewElement.Click += new System.EventHandler(this.buttonNewElement_Click);
			// 
			// buttonEditElement
			// 
			this.buttonEditElement.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonEditElement.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonEditElement.Image")));
			this.buttonEditElement.ImageIndex = 1;
			this.buttonEditElement.ImageList = this.imageListButtons;
			this.buttonEditElement.Location = new System.Drawing.Point(36, 180);
			this.buttonEditElement.Name = "buttonEditElement";
			this.buttonEditElement.Size = new System.Drawing.Size(24, 24);
			this.buttonEditElement.TabIndex = 2;
			this.toolTips.SetToolTip(this.buttonEditElement, "Edit element value");
			this.buttonEditElement.Click += new System.EventHandler(this.buttonEditElement_Click);
			// 
			// buttonNewAttribute
			// 
			this.buttonNewAttribute.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonNewAttribute.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonNewAttribute.Image")));
			this.buttonNewAttribute.ImageIndex = 0;
			this.buttonNewAttribute.ImageList = this.imageListButtons;
			this.buttonNewAttribute.Location = new System.Drawing.Point(132, 180);
			this.buttonNewAttribute.Name = "buttonNewAttribute";
			this.buttonNewAttribute.Size = new System.Drawing.Size(24, 24);
			this.buttonNewAttribute.TabIndex = 6;
			this.toolTips.SetToolTip(this.buttonNewAttribute, "Add new attribute");
			this.buttonNewAttribute.Click += new System.EventHandler(this.buttonNewAttribute_Click);
			// 
			// buttonDeleteAttribute
			// 
			this.buttonDeleteAttribute.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonDeleteAttribute.Image = ((System.Drawing.Bitmap)(resources.GetObject("buttonDeleteAttribute.Image")));
			this.buttonDeleteAttribute.ImageIndex = 2;
			this.buttonDeleteAttribute.ImageList = this.imageListButtons;
			this.buttonDeleteAttribute.Location = new System.Drawing.Point(188, 180);
			this.buttonDeleteAttribute.Name = "buttonDeleteAttribute";
			this.buttonDeleteAttribute.Size = new System.Drawing.Size(24, 24);
			this.buttonDeleteAttribute.TabIndex = 8;
			this.toolTips.SetToolTip(this.buttonDeleteAttribute, "Delete attribute");
			this.buttonDeleteAttribute.Click += new System.EventHandler(this.buttonDeleteAttribute_Click);
			// 
			// treeViewElements
			// 
			this.treeViewElements.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left);
			this.treeViewElements.ImageIndex = -1;
			this.treeViewElements.Location = new System.Drawing.Point(8, 8);
			this.treeViewElements.Name = "treeViewElements";
			this.treeViewElements.SelectedImageIndex = -1;
			this.treeViewElements.Size = new System.Drawing.Size(116, 164);
			this.treeViewElements.TabIndex = 0;
			this.treeViewElements.DoubleClick += new System.EventHandler(this.buttonEditElement_Click);
			this.treeViewElements.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewElements_AfterSelect);
			// 
			// menuItemText
			// 
			this.menuItemText.Index = 1;
			this.menuItemText.Text = "Text";
			this.menuItemText.Click += new System.EventHandler(this.menuItemText_Click);
			// 
			// contextMenuAddElement
			// 
			this.contextMenuAddElement.MenuItems.AddRange(new MenuItem[] {
																								  this.menuItemAddElement,
																								  this.menuItemText});
			// 
			// menuItemAddElement
			// 
			this.menuItemAddElement.Index = 0;
			this.menuItemAddElement.Text = "Element";
			this.menuItemAddElement.Click += new System.EventHandler(this.menuItemAddElement_Click);
			// 
			// labelElementAttributes
			// 
			this.labelElementAttributes.Location = new System.Drawing.Point(128, 16);
			this.labelElementAttributes.Name = "labelElementAttributes";
			this.labelElementAttributes.Size = new System.Drawing.Size(112, 16);
			this.labelElementAttributes.TabIndex = 4;
			this.labelElementAttributes.Text = "Element Attributes:";
			// 
			// XmlInfoEditor
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.labelElementAttributes,
																		  this.buttonNewAttribute,
																		  this.buttonEditAttribute,
																		  this.buttonDeleteAttribute,
																		  this.listViewAttributes,
																		  this.buttonNewElement,
																		  this.treeViewElements,
																		  this.buttonEditElement,
																		  this.buttonDeleteElement});
			this.Name = "XmlInfoEditor";
			this.Size = new System.Drawing.Size(248, 212);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonNewElement_Click(object sender, System.EventArgs e) {
			contextMenuAddElement.Show(this, new Point(buttonNewElement.Bounds.Left, buttonNewElement.Bounds.Bottom));
		}

		private void buttonEditElement_Click(object sender, System.EventArgs e) {
			ElementTreeNode	treeNode = (ElementTreeNode)treeViewElements.SelectedNode;

			if(treeNode != null) {
				String	newName = Dialogs.InputBox.Show("Edit node", treeNode.IsTextNode ? "Enter node text" : "Enter node name");
				if(newName != null)
					treeNode.Text = newName;
			}
		}

		private void menuItemAddElement_Click(object sender, System.EventArgs e) {
			ElementTreeNode	selected = (ElementTreeNode)treeViewElements.SelectedNode;

			if(selected != null) {
				String	name = Dialogs.InputBox.Show("Add Element", "Enter new element name:");

				if(name != null) {
					XmlElement	element = selected.XmlNode.OwnerDocument.CreateElement(name);

					selected.XmlNode.AppendChild(element);
					selected.Nodes.Add(new ElementTreeNode(element));
					selected.Expand();
				}
			}
		}

		private void menuItemText_Click(object sender, System.EventArgs e) {
			ElementTreeNode	selected = (ElementTreeNode)treeViewElements.SelectedNode;

			if(selected != null) {
				String	text = Dialogs.InputBox.Show("Add Text", "Enter new text:");

				if(text != null) {
					XmlText	textElement = selected.XmlNode.OwnerDocument.CreateTextNode(text);

					selected.XmlNode.AppendChild(textElement);
					selected.Nodes.Add(new ElementTreeNode(textElement));
					selected.Expand();
				}
			}
		}

		private void buttonDeleteElement_Click(object sender, System.EventArgs e) {
			ElementTreeNode	selected = (ElementTreeNode)treeViewElements.SelectedNode;

			if(selected != null) {
				if(selected.Parent != null) {
					ElementTreeNode	parent = (ElementTreeNode)selected.Parent;

					parent.XmlNode.RemoveChild(selected.XmlNode);
					parent.Nodes.Remove(selected);
				}
			}
		}

		private void treeViewElements_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			ElementTreeNode	selected = (ElementTreeNode)treeViewElements.SelectedNode;

			System.Diagnostics.Debug.WriteLine("Selection changed");

			listViewAttributes.Items.Clear();

			if(selected != null && !selected.IsTextNode) {
				foreach(XmlAttribute a in selected.XmlNode.Attributes)
					listViewAttributes.Items.Add(new AttributeItem(a));
			}
		}

		private void buttonNewAttribute_Click(object sender, System.EventArgs e) {
			if(SelectedNode != null && SelectedNode.NodeType != XmlNodeType.Text) {
				XmlInfoEditorForms.AttributeInfo	attributeInfo = new XmlInfoEditorForms.AttributeInfo();

				if(attributeInfo.ShowDialog(this) == DialogResult.OK) {
					XmlAttribute	a = xmlInfo.XmlDocument.CreateAttribute(attributeInfo.AttributeName);

					a.Value = attributeInfo.AttributeValue;

					SelectedNode.Attributes.Append(a);
					listViewAttributes.Items.Add(new AttributeItem(a));
				}
			}
		}

		private void buttonEditAttribute_Click(object sender, System.EventArgs e) {
			if(listViewAttributes.SelectedItems.Count > 0) {
				AttributeItem						selected = (AttributeItem)listViewAttributes.SelectedItems[0];
				XmlInfoEditorForms.AttributeInfo	attributeInfo = new XmlInfoEditorForms.AttributeInfo(selected.XmlAttribute);

				if(attributeInfo.ShowDialog(this) == DialogResult.OK) {
					selected.AttributeName = attributeInfo.AttributeName;
					selected.AttributeValue = attributeInfo.AttributeValue;
				}
			}
		}

		private void buttonDeleteAttribute_Click(object sender, System.EventArgs e) {
			if(listViewAttributes.SelectedItems.Count > 0) {
				AttributeItem						selected = (AttributeItem)listViewAttributes.SelectedItems[0];

				SelectedNode.Attributes.Remove(selected.XmlAttribute);
				listViewAttributes.Items.Remove(selected);
			}
		}

		XmlNode SelectedNode {
			get {
				ElementTreeNode	selected = (ElementTreeNode)treeViewElements.SelectedNode;

				if(selected != null)
					return selected.XmlNode;
				else
					return null;
			}
		}

		class ElementTreeNode : TreeNode {
			XmlNode		node;

			internal ElementTreeNode(XmlNode node) {
				this.node = node;

				formatNodeText();
			}

			public XmlNode XmlNode {
				get {
					return node;
				}
			}

			public new String Text {
				get {
					if(node.NodeType == XmlNodeType.Text)
						return node.Value;
					else
						return node.Name;
				}

				set {
					if(node.NodeType == XmlNodeType.Text)
						node.Value = value;
					else
						renameNode(value);

					formatNodeText();
				}
			}

			public bool IsTextNode {
				get {
					return node.NodeType == XmlNodeType.Text;
				}
			}

			public void formatNodeText() {
				if(node.NodeType == XmlNodeType.Text)
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
			private void renameNode(String newName) {
				XmlElement	newElement = node.OwnerDocument.CreateElement(newName);

				foreach(XmlNode n in node.ChildNodes) {
					node.RemoveChild(n);
					newElement.AppendChild(n);
				}

				// Copy attributes
				foreach(XmlAttribute a in node.Attributes)
					newElement.Attributes.Append(a);

				node.ParentNode.ReplaceChild(newElement, node);
				node = newElement;
			}
		}

		class AttributeItem : ListViewItem {

			XmlAttribute	attribute;

			internal AttributeItem(XmlAttribute attribute) {
				this.attribute = attribute;

				this.Text = attribute.Name;
				this.SubItems.Add(attribute.Value);
			}

			public XmlAttribute XmlAttribute {
				get {
					return attribute;
				}
			}

			public String AttributeName {
				get {
					return attribute.Name;
				}

				set {
					if(attribute.Name != value) {
						XmlAttribute	newAttribute = attribute.OwnerDocument.CreateAttribute(value);
						XmlNode			parent = attribute.ParentNode;

						newAttribute.Value = attribute.Value;
						parent.Attributes.Remove(attribute);
						parent.Attributes.Append(newAttribute);
						attribute = newAttribute;

						this.Text = attribute.Name;
					}
				}
			}

			public String AttributeValue {
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
