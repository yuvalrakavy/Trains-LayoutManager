using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Components;

#pragma warning disable IDE0069
#nullable enable
namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for EventScriptEditor.
    /// </summary>
    public class EventScriptEditor : System.Windows.Forms.UserControl, IControlSupportViewOnly, IEventScriptEditor {
        private TreeView treeViewConditions;
        private Button buttonAddCondition;
        private Button buttonEditCondition;
        private Button buttonDeleteCondition;
        private ImageList imageListButttons;
        private Button buttonMoveConditionDown;
        private Button buttonMoveConditionUp;
        private ImageList imageListConditionTree;
        private Button buttonOptions;
        private Button buttonInsert;
        private IContainer components;

        private DialogEditing? changeToViewonly;

#nullable disable
        public EventScriptEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call

        }
#nullable enable

        #region Public properties/Methods

        public XmlElement? EventScriptElement {
            get {
                if (treeViewConditions.Nodes.Count == 0)
                    return null;

                LayoutEventScriptEditorTreeNode root = (LayoutEventScriptEditorTreeNode)treeViewConditions.Nodes[0];

                return root.Element;
            }

            set {
                if (value != null)
                    Initialize(value);
            }
        }

        public Form Form => FindForm();

        public LayoutBlockDefinitionComponent BlockDefinition { get; set; }

        public bool ViewOnly {
            get {
                return changeToViewonly != null;
            }

            set {
                if (value && !ViewOnly) {
                    Size newSize = new Size(buttonMoveConditionDown.Right - treeViewConditions.Left, buttonAddCondition.Bottom - treeViewConditions.Top);

                    changeToViewonly = new DialogEditing(this,
                        new DialogEditingCommandBase[] {
                                                           new DialogEditingRemoveControl(buttonAddCondition),
                                                           new DialogEditingRemoveControl(buttonDeleteCondition),
                                                           new DialogEditingRemoveControl(buttonEditCondition),
                                                           new DialogEditingRemoveControl(buttonMoveConditionUp),
                                                           new DialogEditingRemoveControl(buttonMoveConditionDown),
                                                           new DialogEditingResizeControl(treeViewConditions, newSize),
                    });

                    changeToViewonly.Do();
                    changeToViewonly.ViewOnly = true;
                }
                else if (!value && ViewOnly) {
                    if (changeToViewonly != null) {
                        changeToViewonly.Undo();
                        changeToViewonly.ViewOnly = false;
                        changeToViewonly = null;
                    }
                }
            }
        }

        public bool ValidateScript() {
            LayoutEventScriptEditorTreeNode root = (LayoutEventScriptEditorTreeNode)treeViewConditions.Nodes[0];

            return ValidateScript(root, false);
        }

        public bool ValidateScript(bool globalPolicy) {
            LayoutEventScriptEditorTreeNode root = (LayoutEventScriptEditorTreeNode)treeViewConditions.Nodes[0];

            return ValidateScript(root, globalPolicy);
        }

        protected bool ValidateScript(LayoutEventScriptEditorTreeNode node, bool globalPolicy) {
            if (node.AddNodeEventName != null) {
                if (node.Nodes.Count < node.MinSubNodes) {
                    treeViewConditions.SelectedNode = node;
                    MessageBox.Show(this, "This node requires mandatory sub-nodes", "Missing sub-nodes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (globalPolicy && !node.SupportedInGlobalPolicy) {
                    treeViewConditions.SelectedNode = node;
                    MessageBox.Show(this, "This operation references to layout specific datat. Therefore it is not valid in a policy which is available to all layouts", "Illegal in all-layouts policy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                foreach (LayoutEventScriptEditorTreeNode subnode in node.Nodes)
                    if (!ValidateScript(subnode, globalPolicy))
                        return false;
            }

            return true;
        }

        #endregion

        protected void Initialize(XmlElement eventScriptElement) {
            var root = (LayoutEventScriptEditorTreeNode?)EventManager.Event(new LayoutEvent("get-event-script-editor-tree-node", eventScriptElement));

            if (root == null)
                throw new ApplicationException("No suitable event script tree node was found");

            treeViewConditions.Nodes.Add(root);
            root.UpdateNode();
            root.ExpandAll();
            treeViewConditions.SelectedNode = root;
            updateButtons();
        }

        private void updateButtons() {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected == null) {
                buttonAddCondition.Enabled = false;
                buttonEditCondition.Enabled = false;
                buttonDeleteCondition.Enabled = false;
            }
            else {
                buttonAddCondition.Enabled = (selected.AddNodeEventName != null) &&
                    (selected.MaxSubNodes < 0 || selected.Nodes.Count < selected.MaxSubNodes);
                buttonInsert.Enabled = selected.InsertNodeEventName != null;
                buttonEditCondition.Enabled = selected.NodeToEdit != null;

                if (selected.Parent != null)
                    buttonDeleteCondition.Enabled = selected.CanDeleteNode;
                else
                    buttonDeleteCondition.Enabled = false;

                if (selected.Parent != null && ((LayoutEventScriptEditorTreeNode)selected.Parent).CanReorder) {
                    buttonMoveConditionUp.Enabled = selected.Index > 0;
                    buttonMoveConditionDown.Enabled = selected.Index < selected.Parent.Nodes.Count - 1;
                }
                else {
                    buttonMoveConditionUp.Enabled = false;
                    buttonMoveConditionDown.Enabled = false;
                }

                buttonOptions.Enabled = selected.OptionsMenu != null;
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(EventScriptEditor));
            this.treeViewConditions = new TreeView();
            this.imageListConditionTree = new ImageList(this.components);
            this.buttonAddCondition = new Button();
            this.buttonEditCondition = new Button();
            this.buttonDeleteCondition = new Button();
            this.buttonMoveConditionDown = new Button();
            this.imageListButttons = new ImageList(this.components);
            this.buttonMoveConditionUp = new Button();
            this.buttonOptions = new Button();
            this.buttonInsert = new Button();
            this.SuspendLayout();
            // 
            // treeViewConditions
            // 
            this.treeViewConditions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.treeViewConditions.HideSelection = false;
            this.treeViewConditions.ImageList = this.imageListConditionTree;
            this.treeViewConditions.Location = new System.Drawing.Point(8, 8);
            this.treeViewConditions.Name = "treeViewConditions";
            this.treeViewConditions.Size = new System.Drawing.Size(296, 192);
            this.treeViewConditions.TabIndex = 0;
            this.treeViewConditions.DoubleClick += this.buttonEditEventScript_Click;
            this.treeViewConditions.AfterSelect += this.treeViewConditions_AfterSelect;
            // 
            // imageListConditionTree
            // 
            this.imageListConditionTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListConditionTree.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListConditionTree.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListConditionTree.ImageStream");
            this.imageListConditionTree.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonAddCondition
            // 
            this.buttonAddCondition.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAddCondition.Location = new System.Drawing.Point(8, 207);
            this.buttonAddCondition.Name = "buttonAddCondition";
            this.buttonAddCondition.Size = new System.Drawing.Size(54, 21);
            this.buttonAddCondition.TabIndex = 1;
            this.buttonAddCondition.Text = "&Add";
            this.buttonAddCondition.Click += this.buttonAddEventScript_Click;
            // 
            // buttonEditCondition
            // 
            this.buttonEditCondition.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEditCondition.Location = new System.Drawing.Point(128, 207);
            this.buttonEditCondition.Name = "buttonEditCondition";
            this.buttonEditCondition.Size = new System.Drawing.Size(54, 21);
            this.buttonEditCondition.TabIndex = 3;
            this.buttonEditCondition.Text = "&Edit";
            this.buttonEditCondition.Click += this.buttonEditEventScript_Click;
            // 
            // buttonDeleteCondition
            // 
            this.buttonDeleteCondition.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonDeleteCondition.Location = new System.Drawing.Point(248, 207);
            this.buttonDeleteCondition.Name = "buttonDeleteCondition";
            this.buttonDeleteCondition.Size = new System.Drawing.Size(54, 21);
            this.buttonDeleteCondition.TabIndex = 5;
            this.buttonDeleteCondition.Text = "&Delete";
            this.buttonDeleteCondition.Click += this.buttonDeleteEventScript_Click;
            // 
            // buttonMoveConditionDown
            // 
            this.buttonMoveConditionDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveConditionDown.Image = (System.Drawing.Bitmap)resources.GetObject("buttonMoveConditionDown.Image");
            this.buttonMoveConditionDown.ImageIndex = 0;
            this.buttonMoveConditionDown.ImageList = this.imageListButttons;
            this.buttonMoveConditionDown.Location = new System.Drawing.Point(308, 32);
            this.buttonMoveConditionDown.Name = "buttonMoveConditionDown";
            this.buttonMoveConditionDown.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveConditionDown.TabIndex = 7;
            this.buttonMoveConditionDown.Click += this.buttonMoveConditionDown_Click;
            // 
            // imageListButttons
            // 
            this.imageListButttons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListButttons.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListButttons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListButttons.ImageStream");
            this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonMoveConditionUp
            // 
            this.buttonMoveConditionUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveConditionUp.Image = (System.Drawing.Bitmap)resources.GetObject("buttonMoveConditionUp.Image");
            this.buttonMoveConditionUp.ImageIndex = 1;
            this.buttonMoveConditionUp.ImageList = this.imageListButttons;
            this.buttonMoveConditionUp.Location = new System.Drawing.Point(308, 8);
            this.buttonMoveConditionUp.Name = "buttonMoveConditionUp";
            this.buttonMoveConditionUp.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveConditionUp.TabIndex = 6;
            this.buttonMoveConditionUp.Click += this.buttonMoveConditionUp_Click;
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonOptions.Location = new System.Drawing.Point(188, 207);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(54, 21);
            this.buttonOptions.TabIndex = 4;
            this.buttonOptions.Text = "&Options";
            this.buttonOptions.Click += this.buttonOptions_Click;
            // 
            // buttonInsert
            // 
            this.buttonInsert.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonInsert.Location = new System.Drawing.Point(68, 207);
            this.buttonInsert.Name = "buttonInsert";
            this.buttonInsert.Size = new System.Drawing.Size(54, 21);
            this.buttonInsert.TabIndex = 2;
            this.buttonInsert.Text = "&Insert";
            this.buttonInsert.Click += this.buttonInsert_Click;
            // 
            // EventScriptEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonMoveConditionUp,
                                                                          this.buttonAddCondition,
                                                                          this.treeViewConditions,
                                                                          this.buttonEditCondition,
                                                                          this.buttonDeleteCondition,
                                                                          this.buttonMoveConditionDown,
                                                                          this.buttonOptions,
                                                                          this.buttonInsert});
            this.Name = "EventScriptEditor";
            this.Size = new System.Drawing.Size(336, 232);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonDeleteEventScript_Click(object sender, System.EventArgs e) {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected != null && selected.CanDeleteNode) {
                selected.Element.ParentNode.RemoveChild(selected.Element);
                if (selected.Parent != null)
                    treeViewConditions.SelectedNode = selected.Parent;
                selected.Remove();
                updateButtons();
            }
        }

        private void preProcessMenu(Menu.MenuItemCollection items) {
            foreach (MenuItem item in items) {
                if (item is IEventScriptEditorMenuEntry eventScriptEditorMenuEntry)
                    eventScriptEditorMenuEntry.SetEventScriptEditor(this);

                preProcessMenu(item.MenuItems);
            }
        }

        private void buttonAddEventScript_Click(object sender, System.EventArgs e) {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected != null && selected.AddNodeEventName != null) {
                ContextMenu menu = new ContextMenu();
                LayoutEvent addNodeEvent = new LayoutEvent(selected.AddNodeEventName, selected, menu);

                if (BlockDefinition != null)
                    addNodeEvent.SetOption("BlockDefinitionID", BlockDefinition.Id);

                EventManager.Event(addNodeEvent);
                preProcessMenu(menu.MenuItems);
                menu.Show(this, new Point(buttonAddCondition.Left, buttonAddCondition.Bottom));
            }
        }

        private void buttonInsert_Click(object sender, System.EventArgs e) {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected != null && selected.InsertNodeEventName != null) {
                ContextMenu menu = new ContextMenu();
                LayoutEvent insertNodeEvent = new LayoutEvent(selected.InsertNodeEventName, selected, menu);

                if (BlockDefinition != null)
                    insertNodeEvent.SetOption("BlockDefinitionID", BlockDefinition.Id);

                EventManager.Event(insertNodeEvent);
                preProcessMenu(menu.MenuItems);
                menu.Show(this, new Point(buttonInsert.Left, buttonInsert.Bottom));
            }
        }

        private void buttonEditEventScript_Click(object sender, System.EventArgs e) {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected != null && !ViewOnly && selected.NodeToEdit != null)
                EventManager.Event(new LayoutEvent("event-script-editor-edit-element", selected.NodeToEdit.Element, new EditSite(Form, BlockDefinition, selected)));
        }

        private class EditSite : IEventScriptEditorSite {
            private readonly LayoutEventScriptEditorTreeNode node;

            public EditSite(Form form, LayoutBlockDefinitionComponent blockDefinition, LayoutEventScriptEditorTreeNode node) {
                this.Form = form;
                this.node = node;
                this.BlockDefinition = blockDefinition;
            }

            public Form Form { get; }

            public LayoutBlockDefinitionComponent BlockDefinition { get; }

            public void EditingDone() {
                node.NodeToEdit?.UpdateNode();
            }

            public void EditingCancelled() {
            }
        }

        private void treeViewConditions_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
            updateButtons();
        }

        private void buttonMoveConditionUp_Click(object sender, System.EventArgs e) {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected != null && selected.Index > 0) {
                XmlNode previousElement = selected.Element.PreviousSibling;
                int index = selected.Index;
                TreeNode parent = selected.Parent;

                selected.Element.ParentNode.RemoveChild(selected.Element);
                previousElement.ParentNode.InsertBefore(selected.Element, previousElement);

                selected.Remove();
                parent.Nodes.Insert(index - 1, selected);
                treeViewConditions.SelectedNode = selected;
                selected.ExpandAll();
                updateButtons();
            }
        }

        private void buttonMoveConditionDown_Click(object sender, System.EventArgs e) {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected != null && selected.Index < selected.Parent.Nodes.Count - 1) {
                XmlNode nextElement = selected.Element.NextSibling;
                int index = selected.Index;
                TreeNode parent = selected.Parent;

                selected.Element.ParentNode.RemoveChild(selected.Element);
                nextElement.ParentNode.InsertAfter(selected.Element, nextElement);

                selected.Remove();
                parent.Nodes.Insert(index + 1, selected);
                treeViewConditions.SelectedNode = selected;
                selected.ExpandAll();
                updateButtons();
            }
        }

        private void buttonOptions_Click(object sender, System.EventArgs e) {
            LayoutEventScriptEditorTreeNode selected = (LayoutEventScriptEditorTreeNode)treeViewConditions.SelectedNode;

            if (selected.OptionsMenu != null)
                selected.OptionsMenu.Show(this, new Point(buttonOptions.Left, buttonOptions.Bottom));
        }
    }

    #region Menu Items

    #region Add item

    public class EventScriptEditorAddMenuItem : MenuItem, IEventScriptEditorMenuEntry {
        private readonly string eventScriptElementName;
        private readonly LayoutEventScriptEditorTreeNode parentNode;
        private IEventScriptEditor? _eventScriptEditor = null;

        public EventScriptEditorAddMenuItem(LayoutEvent e, string title, string eventScriptElementName) {
            Text = title;
            this.eventScriptElementName = eventScriptElementName;
            this.parentNode = Ensure.NotNull<LayoutEventScriptEditorTreeNode>(e.Sender, "parent");
        }

        public void SetEventScriptEditor(IEventScriptEditor eventScriptEditor) {
            _eventScriptEditor = eventScriptEditor;
        }

        protected override void OnClick(EventArgs e) {
            XmlElement eventScriptElement = parentNode.Element.OwnerDocument.CreateElement(eventScriptElementName);
            bool insertAsFirst = parentNode.ShouldInsertAsFirst(eventScriptElement);

            if (insertAsFirst)
                parentNode.Element.PrependChild(eventScriptElement);
            else
                parentNode.Element.AppendChild(eventScriptElement);

            var newNode = (LayoutEventScriptEditorTreeNode?)EventManager.Event(
                new LayoutEvent("get-event-script-editor-tree-node", eventScriptElement));

            if (newNode != null && _eventScriptEditor != null) {
                IEventScriptEditorSite site = new EventScriptEditorSite(_eventScriptEditor.Form, _eventScriptEditor.BlockDefinition, parentNode, newNode, eventScriptElement);

                if (newNode.NodeToEdit == null)
                    site.EditingDone();
                else
                    EventManager.Event(new LayoutEvent("event-script-editor-edit-element", eventScriptElement, site));
            }
        }

        private class EventScriptEditorSite : IEventScriptEditorSite {
            private readonly LayoutEventScriptEditorTreeNode parentNode;
            private readonly LayoutEventScriptEditorTreeNode newNode;
            private readonly XmlElement eventScriptElement;

            public EventScriptEditorSite(Form form, LayoutBlockDefinitionComponent blockDefinition, LayoutEventScriptEditorTreeNode parentNode, LayoutEventScriptEditorTreeNode newNode, XmlElement eventScriptElement) {
                this.Form = form;
                this.BlockDefinition = blockDefinition;
                this.parentNode = parentNode;
                this.newNode = newNode;
                this.eventScriptElement = eventScriptElement;
            }

            public Form Form { get; }

            public void EditingDone() {
                bool insertAsFirst = parentNode.ShouldInsertAsFirst(eventScriptElement);

                newNode.UpdateNode();
                if (insertAsFirst)
                    parentNode.Nodes.Insert(0, newNode);
                else
                    parentNode.Nodes.Add(newNode);

                parentNode.Expand();
                newNode.TreeView.SelectedNode = newNode;
            }

            public void EditingCancelled() {
                parentNode.Element.RemoveChild(eventScriptElement); // Editing was canceled
            }

            public LayoutBlockDefinitionComponent BlockDefinition { get; }
        }
    }

    #endregion

    #region Insert Event Container

    public class EventScriptEditorInsertEventContainerMenuItem : MenuItem, IEventScriptEditorMenuEntry {
        private readonly string eventScriptElementName;
        private readonly LayoutEventScriptEditorTreeNode node;
        private IEventScriptEditor? eventScriptEditor = null;

        public EventScriptEditorInsertEventContainerMenuItem(LayoutEvent e, string title, string eventScriptElementName) {
            node = Ensure.NotNull<LayoutEventScriptEditorTreeNode>(e.Sender, "node");
            Text = title;

            this.eventScriptElementName = eventScriptElementName;
        }

        public void SetEventScriptEditor(IEventScriptEditor eventScriptEditor) {
            this.eventScriptEditor = eventScriptEditor;
        }

        protected override void OnClick(EventArgs e) {
            XmlElement eventScriptElement = node.Element.OwnerDocument.CreateElement(eventScriptElementName);

            // eventScriptElement must be parented for XPath to work!!
            node.Element.AppendChild(eventScriptElement);

            var newNode = (LayoutEventScriptEditorTreeNode?)EventManager.Event(
                new LayoutEvent("get-event-script-editor-tree-node", eventScriptElement));

            if (newNode != null && eventScriptEditor != null) {
                IEventScriptEditorSite site = new EventScriptEditorSite(eventScriptEditor.Form, eventScriptEditor.BlockDefinition, node, newNode, eventScriptElement);

                if (newNode.NodeToEdit == null)
                    site.EditingDone();
                else
                    EventManager.Event(new LayoutEvent("event-script-editor-edit-element", eventScriptElement, site));
            }
        }

        private class EventScriptEditorSite : IEventScriptEditorSite {
            private readonly LayoutEventScriptEditorTreeNode node;
            private readonly LayoutEventScriptEditorTreeNode newNode;
            private readonly XmlElement eventScriptElement;

            public EventScriptEditorSite(Form form, LayoutBlockDefinitionComponent blockDefinition, LayoutEventScriptEditorTreeNode node, LayoutEventScriptEditorTreeNode newNode, XmlElement eventScriptElement) {
                this.Form = form;
                this.BlockDefinition = blockDefinition;
                this.node = node;
                this.newNode = newNode;
                this.eventScriptElement = eventScriptElement;
            }

            public Form Form { get; }

            public void EditingDone() {
                newNode.UpdateNode();

                node.Element.RemoveChild(eventScriptElement);

                // Insert XML
                if (node.Element.ParentNode is XmlDocument)
                    node.Element.OwnerDocument.ReplaceChild(newNode.Element, node.Element);
                else {
                    XmlElement nodeParentElement = (XmlElement)node.Element.ParentNode;

                    nodeParentElement.ReplaceChild(newNode.Element, node.Element);
                }

                XmlElement newNodeEventsSectionElement = newNode.Element["Events"] ?? newNode.Element;
                newNodeEventsSectionElement.AppendChild(node.Element);

                // Now do the same on the tree control
                int nodeIndex = node.Index;
                TreeNodeCollection parentNodes;

                if (node.Parent == null)
                    parentNodes = node.TreeView.Nodes;
                else
                    parentNodes = node.Parent.Nodes;

                node.Remove();      // Remove current node from the tree
                parentNodes.Insert(nodeIndex, newNode); // Replace the node with the new node

                LayoutEventScriptEditorTreeNode eventsSectionNode = newNode;

                foreach (LayoutEventScriptEditorTreeNode n in newNode.Nodes)
                    if (n.Element.Name == "Events") {
                        eventsSectionNode = n;
                        break;
                    }

                eventsSectionNode.Nodes.Add(node);

                newNode.ExpandAll();
                newNode.TreeView.SelectedNode = newNode;
            }

            public void EditingCancelled() {
                node.Element.RemoveChild(eventScriptElement);
            }

            public LayoutBlockDefinitionComponent BlockDefinition { get; }
        }
    }

    #endregion

    #region Insert condition container

    public class EventScriptEditorInsertConditionContainerMenuItem : MenuItem, IEventScriptEditorMenuEntry {
        private readonly string eventScriptElementName;
        private readonly LayoutEventScriptEditorTreeNode node;
        private IEventScriptEditor? eventScriptEditor = null;

        public EventScriptEditorInsertConditionContainerMenuItem(LayoutEvent e, string title, string eventScriptElementName) {
            node = Ensure.NotNull<LayoutEventScriptEditorTreeNode>(e.Sender, "node");
            Text = title;

            this.eventScriptElementName = eventScriptElementName;
        }

        public void SetEventScriptEditor(IEventScriptEditor eventScriptEditor) {
            this.eventScriptEditor = eventScriptEditor;
        }

        protected override void OnClick(EventArgs e) {
            XmlElement eventScriptElement = node.Element.OwnerDocument.CreateElement(eventScriptElementName);

            // eventScriptElement must be parented for XPath to work!!
            node.Element.AppendChild(eventScriptElement);

            var newNode = (LayoutEventScriptEditorTreeNode?)EventManager.Event(
                new LayoutEvent("get-event-script-editor-tree-node", eventScriptElement));

            if (newNode != null && eventScriptEditor != null) {
                IEventScriptEditorSite site = new EventScriptEditorSite(eventScriptEditor.Form, eventScriptEditor.BlockDefinition, node, newNode, eventScriptElement);

                if (newNode.NodeToEdit == null)
                    site.EditingDone();
                else
                    EventManager.Event(new LayoutEvent("event-script-editor-edit-element", eventScriptElement, site));
            }
        }

        private class EventScriptEditorSite : IEventScriptEditorSite {
            private readonly LayoutEventScriptEditorTreeNode node;
            private readonly LayoutEventScriptEditorTreeNode newNode;
            private readonly XmlElement eventScriptElement;

            public EventScriptEditorSite(Form form, LayoutBlockDefinitionComponent blockDefinition, LayoutEventScriptEditorTreeNode node, LayoutEventScriptEditorTreeNode newNode, XmlElement eventScriptElement) {
                this.Form = form;
                this.BlockDefinition = blockDefinition;
                this.node = node;
                this.newNode = newNode;
                this.eventScriptElement = eventScriptElement;
            }

            public Form Form { get; }

            public void EditingDone() {
                newNode.UpdateNode();

                node.Element.RemoveChild(eventScriptElement);

                // Insert XML
                XmlElement nodeParentElement = (XmlElement)node.Element.ParentNode;

                nodeParentElement.ReplaceChild(newNode.Element, node.Element);
                newNode.Element.AppendChild(node.Element);

                // Now do the same on the tree control
                int nodeIndex = node.Index;
                TreeNodeCollection parentNodes;

                if (node.Parent == null)
                    parentNodes = node.TreeView.Nodes;
                else
                    parentNodes = node.Parent.Nodes;

                node.Remove();      // Remove current node from the tree
                parentNodes.Insert(nodeIndex, newNode); // Replace the node with the new node
                newNode.Nodes.Add(node);

                newNode.ExpandAll();
                newNode.TreeView.SelectedNode = newNode;
            }

            public void EditingCancelled() {
                node.Element.RemoveChild(eventScriptElement);
            }

            public LayoutBlockDefinitionComponent BlockDefinition { get; }
        }
    }

    #endregion

    #endregion

    #region Base classes for tree node

    public abstract class LayoutEventScriptEditorTreeNode : TreeNode, IObjectHasXml {
        public const int IconEventSection = 0;
        public const int IconConditionSection = 1;
        public const int IconActionsSection = 2;
        public const int IconEvent = 3;
        public const int IconEventNotLimitedToScope = 4;
        public const int IconCondition = 5;
        public const int IconAction = 6;

        protected LayoutEventScriptEditorTreeNode(XmlElement eventScriptElement) {
            this.Element = eventScriptElement;
            this.Expand();
        }

        public XmlElement Element { get; }
        public XmlElement? OptionalElement => Element;

        static public string GetOperandDescription(XmlElement element, string suffix, Type? type) {
            var symbolAccess = element.GetAttribute($"Symbol{suffix}Access");

            if (symbolAccess == "Value") {
                var symbolValue = element.GetAttribute($"Value{suffix}");

                if (type == typeof(string))
                    return $"'{symbolValue}'";
                else return type == typeof(bool) ? bool.Parse(symbolValue) ? "True" : "False" : symbolValue;
            }
            else {
                var tagName = element.GetAttribute($"Name{suffix}");
                var symbolName = element.GetAttribute($"Symbol{suffix}");

                return symbolAccess == "Property" ? $"property {tagName} of {symbolName}" : $"attribute {tagName} of {symbolName}";
            }
        }

        static public string GetOperandDescription(XmlElement element, string suffix) {
            string typeName = element.GetAttribute("Type" + suffix);
            Type type;

            switch (typeName) {
                case "Boolean":
                    type = typeof(bool);
                    break;

                case "Integer":
                    type = typeof(int);
                    break;

                case "Double":
                    type = typeof(double);
                    break;

                case "String":
                default:
                    type = typeof(string);
                    break;
            }

            return GetOperandDescription(element, suffix, type);
        }

        protected void AddChildEventScriptTreeNodes(XmlElement element) {
            foreach (XmlElement childEventScriptElement in element) {
                var node = (LayoutEventScriptEditorTreeNode?)EventManager.Event(new LayoutEvent("get-event-script-editor-tree-node", childEventScriptElement));

                if (node == null)
                    throw new ApplicationException("No suitable condition tree node was found");

                Nodes.Add(node);
                node.UpdateNode();
            }
        }

        protected void AddChildEventScriptTreeNodes() {
            AddChildEventScriptTreeNodes(Element);
        }

        /// <summary>
        /// Update the node Text to reflect the node
        /// </summary>
        public virtual void UpdateNode() {
            this.ImageIndex = IconIndex;
            this.SelectedImageIndex = IconIndex;
            this.Text = $"{(((bool?)Element.AttributeValue("Optional") ?? false) ? "Optional: " : "")}{Description}";
        }

        protected abstract int IconIndex { get; }

        protected abstract string Description { get; }

        /// <summary>
        /// Optional options menu
        /// </summary>
        public virtual ContextMenu? OptionsMenu => null;

        /// <summary>
        /// The event name that is used for building the Add button menu. The default implementation returns
        /// null, so the add button is disabled
        /// </summary>
        public virtual string? AddNodeEventName => null;

        /// <summary>
        /// The event name that is used for building the insert button menu.
        /// </summary>
        public virtual string? InsertNodeEventName => null;

        /// <summary>
        /// The node to be edited when edit command is given. The default is to edit the current node
        /// </summary>
        public virtual LayoutEventScriptEditorTreeNode? NodeToEdit => this;

        /// <summary>
        /// Return true if the node can be deleted.
        /// </summary>
        public virtual bool CanDeleteNode => true;

        /// <summary>
        /// The maximum number of children conditions that can be added - default implementation - no limit
        /// </summary>
        public virtual int MaxSubNodes => -1;

        /// <summary>
        /// The minimum number of subconditions that can be added.
        /// </summary>
        public virtual int MinSubNodes => 1;

        public virtual bool CanReorder => false;

        public virtual bool CanHaveOptionalSubItems => false;

        public virtual bool ShouldInsertAsFirst(XmlElement elementToInsert) => false;

        public virtual bool SupportConditions => true;

        public virtual bool SupportActions => true;

        public virtual bool SupportedInGlobalPolicy => true;
    }

    public abstract class LayoutEventScriptEditorTreeNodeMayBeOptional : LayoutEventScriptEditorTreeNode {
        protected LayoutEventScriptEditorTreeNodeMayBeOptional(XmlElement eventElement) : base(eventElement) {
        }

        public override ContextMenu? OptionsMenu {
            get {
                ContextMenu? menu = null;

                if (Parent != null) {
                    LayoutEventScriptEditorTreeNode container = (LayoutEventScriptEditorTreeNode)Parent.Parent;

                    if (container != null && container.CanHaveOptionalSubItems) {
                        menu = new ContextMenu();
                        menu.MenuItems.Add(new MandatoryMenuItem(this));
                        menu.MenuItems.Add(new OptionalMenuItem(this));
                    }
                }

                return menu;
            }
        }

        #region menu item classes

        private class MandatoryMenuItem : MenuItem {
            private const string A_Optional = "Optional";
            private readonly LayoutEventScriptEditorTreeNode node;

            public MandatoryMenuItem(LayoutEventScriptEditorTreeNode node) {
                this.node = node;

                Text = "Not optional";
                RadioCheck = true;

                Checked = !((bool?)node.Element.AttributeValue(A_Optional) ?? false);
            }

            protected override void OnClick(EventArgs e) {
                node.Element.RemoveAttribute(A_Optional);
                node.UpdateNode();
            }
        }

        private class OptionalMenuItem : MenuItem {
            private const string A_Optional = "Optional";
            private readonly LayoutEventScriptEditorTreeNode node;

            public OptionalMenuItem(LayoutEventScriptEditorTreeNode node) {
                this.node = node;

                Text = "Optional";
                RadioCheck = true;

                Checked = (bool?)node.Element.AttributeValue(A_Optional) ?? false;
            }

            protected override void OnClick(EventArgs e) {
                node.Element.SetAttributeValue(A_Optional, true);
                node.UpdateNode();
            }
        }

        #endregion
    }

    public abstract class LayoutEventScriptEditorTreeNodeEventContainer : LayoutEventScriptEditorTreeNodeMayBeOptional {
        private const string E_Events = "Events";
        private const string A_LimitToScope = "LimitToScope";

        protected LayoutEventScriptEditorTreeNodeEventContainer(XmlElement eventContainerElement) : base(eventContainerElement) {
            if (eventContainerElement[E_Events] == null) {
                XmlElement eventsElement = eventContainerElement.OwnerDocument.CreateElement(E_Events);

                eventContainerElement.AppendChild(eventsElement);
            }
        }

        public override string AddNodeEventName => "get-event-script-editor-event-container-menu";

        public override string InsertNodeEventName => "get-event-script-editor-insert-event-container-menu";

        public override LayoutEventScriptEditorTreeNode? NodeToEdit => null;		// Composite rules can not be edited

        protected override int IconIndex => (!((bool?)Element.AttributeValue(A_LimitToScope) ?? false)) ? 4 : 3;
    }

    public abstract class LayoutEventScriptEditorTreeNodeEventSection : LayoutEventScriptEditorTreeNode {
        protected LayoutEventScriptEditorTreeNodeEventSection(XmlElement sectionElement) : base(sectionElement) {
            AddChildEventScriptTreeNodes();
        }

        public override LayoutEventScriptEditorTreeNode? NodeToEdit => null;     // Event section cannot be edited

        public override bool CanReorder => true;
    }

    public abstract class LayoutEventScriptEditorTreeNodeEvent : LayoutEventScriptEditorTreeNodeMayBeOptional {
        private const string A_LimitToScope = "LimitToScope";

        protected LayoutEventScriptEditorTreeNodeEvent(XmlElement eventElement) : base(eventElement) {
        }

        public override string AddNodeEventName => "get-event-script-editor-event-menu";

        public override string InsertNodeEventName => "get-event-script-editor-insert-event-container-menu";

        protected override int IconIndex => !((bool?)Element.AttributeValue(A_LimitToScope) ?? false) ? IconEventNotLimitedToScope : IconEvent;

        public override int MinSubNodes => 0;
    }

    public abstract class LayoutEventScriptEditorTreeNodeConditionContainer : LayoutEventScriptEditorTreeNode {
        protected LayoutEventScriptEditorTreeNodeConditionContainer(XmlElement conditionElement) : base(conditionElement) {
        }

        public override string AddNodeEventName => "get-event-script-editor-condition-section-menu";

        public override string InsertNodeEventName => "get-event-script-editor-insert-condition-container-menu";

        public override LayoutEventScriptEditorTreeNode? NodeToEdit => null;     // Condition container cannot be edited

        public override bool CanReorder => true;		// Conditions within the container can be reordered

        protected override int IconIndex => IconCondition;
    }

    public abstract class LayoutEventScriptEditorTreeNodeCondition : LayoutEventScriptEditorTreeNode {
        protected LayoutEventScriptEditorTreeNodeCondition(XmlElement conditionElement) : base(conditionElement) {
        }

        public override string InsertNodeEventName => "get-event-script-editor-insert-condition-container-menu";

        protected override int IconIndex => IconCondition;

        public override int MinSubNodes => 0;
    }

    public abstract class LayoutEventScriptEditorTreeNodeAction : LayoutEventScriptEditorTreeNode {
        protected LayoutEventScriptEditorTreeNodeAction(XmlElement actionElement) : base(actionElement) {
        }

        protected override int IconIndex => IconAction;
    }

    #endregion

    #region Classes for Events, Condition and Actions clauses for event container and events

    public class LayoutEventScriptEditorTreeNodeEventsSection : LayoutEventScriptEditorTreeNodeEventSection {
        public LayoutEventScriptEditorTreeNodeEventsSection(XmlElement eventsElement) : base(eventsElement) {
        }

        public override string AddNodeEventName => "get-event-script-editor-events-section-menu";

        protected override int IconIndex => IconEventSection;

        protected override string Description => "Events";
    }

    public class LayoutEventScriptEditorTreeNodeConditionSection : LayoutEventScriptEditorTreeNodeEventSection {
        public LayoutEventScriptEditorTreeNodeConditionSection(XmlElement conditionElement) : base(conditionElement) {
            Text = "Condition";
        }

        public override string AddNodeEventName => "get-event-script-editor-condition-section-menu";

        protected override int IconIndex => IconConditionSection;

        public override int MaxSubNodes => 1;

        protected override string Description => "Condition";
    }

    public class LayoutEventScriptEditorTreeNodeActionsSection : LayoutEventScriptEditorTreeNodeEventSection {
        public LayoutEventScriptEditorTreeNodeActionsSection(XmlElement actionsElement) : base(actionsElement) {
            Text = "Actions";
        }

        public override string AddNodeEventName => "get-event-script-editor-actions-section-menu";

        protected override int IconIndex => 2;

        protected override string Description => "Actions";
    }

    #endregion
}
