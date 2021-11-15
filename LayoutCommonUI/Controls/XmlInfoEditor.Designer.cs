namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for XmlInfoEditor.
    /// </summary>
    partial class XmlInfoEditor : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlInfoEditor));
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
            this.menuItemText = new ToolStripMenuItem();
            this.contextMenuAddElement = new ContextMenuStrip();
            this.menuItemAddElement = new ToolStripMenuItem();
            this.labelElementAttributes = new Label();
            this.SuspendLayout();
            // 
            // buttonDeleteElement
            // 
            this.buttonDeleteElement.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonDeleteElement.Image = (System.Drawing.Bitmap)resources.GetObject("buttonDeleteElement.Image");
            this.buttonDeleteElement.ImageIndex = 2;
            this.buttonDeleteElement.ImageList = this.imageListButtons;
            this.buttonDeleteElement.Location = new System.Drawing.Point(64, 180);
            this.buttonDeleteElement.Name = "buttonDeleteElement";
            this.buttonDeleteElement.Size = new System.Drawing.Size(24, 24);
            this.buttonDeleteElement.TabIndex = 3;
            this.toolTips.SetToolTip(this.buttonDeleteElement, "Delete element");
            this.buttonDeleteElement.Click += this.ButtonDeleteElement_Click;
            // 
            // imageListButtons
            // 
            this.imageListButtons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListButtons.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListButtons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListButtons.ImageStream");
            this.imageListButtons.TransparentColor = System.Drawing.Color.Silver;
            // 
            // columnHeaderAttributeValue
            // 
            this.columnHeaderAttributeValue.Text = "Value";
            this.columnHeaderAttributeValue.Width = 55;
            // 
            // listViewAttributes
            // 
            this.listViewAttributes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
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
            this.listViewAttributes.DoubleClick += this.ButtonEditAttribute_Click;
            // 
            // columnHeaderAttributeName
            // 
            this.columnHeaderAttributeName.Text = "Attribute";
            this.columnHeaderAttributeName.Width = 55;
            // 
            // buttonEditAttribute
            // 
            this.buttonEditAttribute.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEditAttribute.Image = (System.Drawing.Bitmap)resources.GetObject("buttonEditAttribute.Image");
            this.buttonEditAttribute.ImageIndex = 1;
            this.buttonEditAttribute.ImageList = this.imageListButtons;
            this.buttonEditAttribute.Location = new System.Drawing.Point(160, 180);
            this.buttonEditAttribute.Name = "buttonEditAttribute";
            this.buttonEditAttribute.Size = new System.Drawing.Size(24, 24);
            this.buttonEditAttribute.TabIndex = 7;
            this.toolTips.SetToolTip(this.buttonEditAttribute, "Edit attribute value");
            this.buttonEditAttribute.Click += this.ButtonEditAttribute_Click;
            // 
            // buttonNewElement
            // 
            this.buttonNewElement.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonNewElement.Image = (System.Drawing.Bitmap)resources.GetObject("buttonNewElement.Image");
            this.buttonNewElement.ImageIndex = 0;
            this.buttonNewElement.ImageList = this.imageListButtons;
            this.buttonNewElement.Location = new System.Drawing.Point(8, 180);
            this.buttonNewElement.Name = "buttonNewElement";
            this.buttonNewElement.Size = new System.Drawing.Size(24, 24);
            this.buttonNewElement.TabIndex = 1;
            this.toolTips.SetToolTip(this.buttonNewElement, "Add element");
            this.buttonNewElement.Click += this.ButtonNewElement_Click;
            // 
            // buttonEditElement
            // 
            this.buttonEditElement.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEditElement.Image = (System.Drawing.Bitmap)resources.GetObject("buttonEditElement.Image");
            this.buttonEditElement.ImageIndex = 1;
            this.buttonEditElement.ImageList = this.imageListButtons;
            this.buttonEditElement.Location = new System.Drawing.Point(36, 180);
            this.buttonEditElement.Name = "buttonEditElement";
            this.buttonEditElement.Size = new System.Drawing.Size(24, 24);
            this.buttonEditElement.TabIndex = 2;
            this.toolTips.SetToolTip(this.buttonEditElement, "Edit element value");
            this.buttonEditElement.Click += this.ButtonEditElement_Click;
            // 
            // buttonNewAttribute
            // 
            this.buttonNewAttribute.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonNewAttribute.Image = (System.Drawing.Bitmap)resources.GetObject("buttonNewAttribute.Image");
            this.buttonNewAttribute.ImageIndex = 0;
            this.buttonNewAttribute.ImageList = this.imageListButtons;
            this.buttonNewAttribute.Location = new System.Drawing.Point(132, 180);
            this.buttonNewAttribute.Name = "buttonNewAttribute";
            this.buttonNewAttribute.Size = new System.Drawing.Size(24, 24);
            this.buttonNewAttribute.TabIndex = 6;
            this.toolTips.SetToolTip(this.buttonNewAttribute, "Add new attribute");
            this.buttonNewAttribute.Click += this.ButtonNewAttribute_Click;
            // 
            // buttonDeleteAttribute
            // 
            this.buttonDeleteAttribute.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonDeleteAttribute.Image = (System.Drawing.Bitmap)resources.GetObject("buttonDeleteAttribute.Image");
            this.buttonDeleteAttribute.ImageIndex = 2;
            this.buttonDeleteAttribute.ImageList = this.imageListButtons;
            this.buttonDeleteAttribute.Location = new System.Drawing.Point(188, 180);
            this.buttonDeleteAttribute.Name = "buttonDeleteAttribute";
            this.buttonDeleteAttribute.Size = new System.Drawing.Size(24, 24);
            this.buttonDeleteAttribute.TabIndex = 8;
            this.toolTips.SetToolTip(this.buttonDeleteAttribute, "Delete attribute");
            this.buttonDeleteAttribute.Click += this.ButtonDeleteAttribute_Click;
            // 
            // treeViewElements
            // 
            this.treeViewElements.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left;
            this.treeViewElements.ImageIndex = -1;
            this.treeViewElements.Location = new System.Drawing.Point(8, 8);
            this.treeViewElements.Name = "treeViewElements";
            this.treeViewElements.SelectedImageIndex = -1;
            this.treeViewElements.Size = new System.Drawing.Size(116, 164);
            this.treeViewElements.TabIndex = 0;
            this.treeViewElements.DoubleClick += this.ButtonEditElement_Click;
            this.treeViewElements.AfterSelect += this.TreeViewElements_AfterSelect;
            // 
            // menuItemText
            // 
            this.menuItemText.Text = "Text";
            this.menuItemText.Click += this.MenuItemText_Click;
            // 
            // contextMenuAddElement
            // 
            this.contextMenuAddElement.Items.AddRange(new ToolStripMenuItem[] {
                                                                                                  this.menuItemAddElement,
                                                                                                  this.menuItemText});
            // 
            // menuItemAddElement
            // 
            this.menuItemAddElement.Text = "Element";
            this.menuItemAddElement.Click += this.MenuItemAddElement_Click;
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
        private ContextMenuStrip contextMenuAddElement;
        private ToolStripMenuItem menuItemAddElement;
        private ToolStripMenuItem menuItemText;
        private System.ComponentModel.IContainer components;
    }
}
