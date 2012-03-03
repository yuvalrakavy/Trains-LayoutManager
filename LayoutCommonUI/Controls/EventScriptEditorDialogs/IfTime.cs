using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs
{
	/// <summary>
	/// Summary description for IfTime.
	/// </summary>
	public class IfTime : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.MenuItem menuItemSeconds;
		private System.Windows.Forms.MenuItem menuItemMinutes;
		private System.Windows.Forms.MenuItem menuItemHours;
		private System.Windows.Forms.MenuItem menuItemDayOfWeek;
		private System.Windows.Forms.ContextMenu contextMenuAdd;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		XmlDocument			workingDoc;
		XmlElement			element;
		XmlElement			inElement;

		public IfTime(XmlElement inElement)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.inElement = inElement;

			workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
			element = (XmlElement)workingDoc.ImportNode(inElement, true);
			workingDoc.AppendChild(element);

			int		i = 0;
			foreach(string timeConstraintName in new string[] { "Seconds", "Minutes", "Hours", "DayOfWeek" }) {
				IIfTimeNode[]	nodes = (IIfTimeNode[])EventManager.Event(new LayoutEvent(element, "parse-if-time-element", null, timeConstraintName));

				if(nodes.Length > 0)
					treeView.Nodes.Add(new TimeSectionTreeNode(new string[] {"Seconds", "Minutes", "Hours", "Day of week"}[i],
						new Type[] {typeof(SecondsTreeNode), typeof(MinutesTreeNode), typeof(HoursTreeNode), typeof(DayOfWeekTreeNode) }[i], nodes));
				i++;
			}
		}

		private TimeSectionTreeNode findTimeSection(Type treeNodeType) {
			foreach(TimeSectionTreeNode timeSection in treeView.Nodes)
				if(treeNodeType.IsInstanceOfType(timeSection.Nodes[0]))
					return timeSection;
			return null;
		}

		private void insert(string nodeElementName, string timeSectionTitle, Type treeNodeType) {
			XmlElement	nodeElement = element.OwnerDocument.CreateElement(nodeElementName);
			IIfTimeNode	node = (IIfTimeNode)EventManager.Event(new LayoutEvent(nodeElement, "allocate-if-time-node"));

			node.Value = 0;
			TreeNodeBase	treeNode = (TreeNodeBase)Activator.CreateInstance(treeNodeType, new object[] { node });

			if(treeNode.Edit()) {
				TimeSectionTreeNode	timeSection = findTimeSection(treeNodeType);

				element.AppendChild(nodeElement);
				treeNode.Text = node.Description;

				if(timeSection == null) {
					timeSection = new TimeSectionTreeNode(timeSectionTitle);
					treeView.Nodes.Add(timeSection);
				}

				timeSection.Nodes.Add(treeNode);
				timeSection.ExpandAll();
			}
		}

		private void updateButtons() {
			if(treeView.SelectedNode is TimeSectionTreeNode)
				buttonEdit.Enabled = false;
			else if(treeView.SelectedNode is TreeNodeBase)
				buttonEdit.Enabled = true;

			buttonDelete.Enabled = treeView.SelectedNode != null;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeView = new System.Windows.Forms.TreeView();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.contextMenuAdd = new System.Windows.Forms.ContextMenu();
			this.menuItemSeconds = new System.Windows.Forms.MenuItem();
			this.menuItemMinutes = new System.Windows.Forms.MenuItem();
			this.menuItemHours = new System.Windows.Forms.MenuItem();
			this.menuItemDayOfWeek = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.treeView.ImageIndex = -1;
			this.treeView.Location = new System.Drawing.Point(8, 8);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(200, 200);
			this.treeView.TabIndex = 0;
			this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAdd.Location = new System.Drawing.Point(8, 212);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(61, 19);
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEdit.Location = new System.Drawing.Point(72, 212);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(61, 19);
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "&Edit...";
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDelete.Location = new System.Drawing.Point(136, 212);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(61, 19);
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(73, 250);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(64, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(145, 250);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(64, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// contextMenuAdd
			// 
			this.contextMenuAdd.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuItemSeconds,
																						   this.menuItemMinutes,
																						   this.menuItemHours,
																						   this.menuItemDayOfWeek});
			// 
			// menuItemSeconds
			// 
			this.menuItemSeconds.Index = 0;
			this.menuItemSeconds.Text = "Seconds...";
			this.menuItemSeconds.Click += new System.EventHandler(this.menuItemSeconds_Click);
			// 
			// menuItemMinutes
			// 
			this.menuItemMinutes.Index = 1;
			this.menuItemMinutes.Text = "Minutes...";
			this.menuItemMinutes.Click += new System.EventHandler(this.menuItemMinutes_Click);
			// 
			// menuItemHours
			// 
			this.menuItemHours.Index = 2;
			this.menuItemHours.Text = "Hours...";
			this.menuItemHours.Click += new System.EventHandler(this.menuItemHours_Click);
			// 
			// menuItemDayOfWeek
			// 
			this.menuItemDayOfWeek.Index = 3;
			this.menuItemDayOfWeek.Text = "Day of week...";
			this.menuItemDayOfWeek.Click += new System.EventHandler(this.menuItemDayOfWeek_Click);
			// 
			// IfTime
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(216, 278);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.treeView);
			this.Controls.Add(this.buttonEdit);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.buttonCancel);
			this.Name = "IfTime";
			this.ShowInTaskbar = false;
			this.Text = "If (Time)";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e) {
			contextMenuAdd.Show(buttonAdd.Parent, new Point(buttonAdd.Left, buttonAdd.Bottom));
		}

		private void menuItemSeconds_Click(object sender, System.EventArgs e) {
			insert("Seconds", "Seconds", typeof(SecondsTreeNode));
		}

		private void menuItemMinutes_Click(object sender, System.EventArgs e) {
			insert("Minutes", "Minutes", typeof(MinutesTreeNode));
		}

		private void menuItemHours_Click(object sender, System.EventArgs e) {
			insert("Hours", "Hours", typeof(HoursTreeNode));
		}

		private void menuItemDayOfWeek_Click(object sender, System.EventArgs e) {
			insert("DayOfWeek", "Day of week", typeof(DayOfWeekTreeNode));
		}

		private void buttonOK_Click(object sender, System.EventArgs e) {
			inElement.RemoveAll();

			foreach(XmlElement nodeElement in element) {
				XmlElement	inNodeElement = (XmlElement)inElement.OwnerDocument.ImportNode(nodeElement, true);

				inElement.AppendChild(inNodeElement);
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void buttonEdit_Click(object sender, System.EventArgs e) {
			TreeNodeBase	selected = treeView.SelectedNode as TreeNodeBase;

			if(selected != null) {
				if(selected.Edit())
					selected.Text = selected.Node.Description;
			}
		}

		private void buttonDelete_Click(object sender, System.EventArgs e) {
			TimeSectionTreeNode	timeSection;

			if(treeView.SelectedNode is TimeSectionTreeNode) {
				timeSection = (TimeSectionTreeNode)treeView.SelectedNode;

				foreach(TreeNodeBase treeNode in timeSection.Nodes)
					element.RemoveChild(treeNode.Element);
				timeSection.Nodes.Clear();
			}
			else if(treeView.SelectedNode is TreeNodeBase) {
				TreeNodeBase	treeNode = (TreeNodeBase)treeView.SelectedNode;

				timeSection = (TimeSectionTreeNode)treeNode.Parent;
				element.RemoveChild(treeNode.Node.Element);
				timeSection.Nodes.Remove(treeNode);
			}
			else
				throw new ApplicationException("Unexpected tree node type");

			if(timeSection.Nodes.Count == 0)
				treeView.Nodes.Remove(timeSection);
		}

		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			updateButtons();
		}

		private void treeView_DoubleClick(object sender, System.EventArgs e) {
			buttonEdit.PerformClick();
		}

		class TimeSectionTreeNode : TreeNode {
			public TimeSectionTreeNode(string title, Type treeNodeClass, IIfTimeNode[] nodes) {
				Text = title;

				if(nodes != null) {
					foreach(IIfTimeNode node in nodes) {
						TreeNode	treeNode = (TreeNode)Activator.CreateInstance(treeNodeClass, new object[] { node });

						Nodes.Add(treeNode);
					}
				}

				ExpandAll();
			}

			public TimeSectionTreeNode(string title) {
				Text = title;
			}
		}

		abstract class TreeNodeBase : TreeNode, IObjectHasXml {
			IIfTimeNode		node;

			public TreeNodeBase(IIfTimeNode node) {
				this.node = node;
				Text = node.Description;
			}

			public XmlElement Element {
				get {
					return node.Element;
				}
			}

			public IIfTimeNode Node {
				get {
					return node;
				}
			}

			abstract public bool Edit();
		}

		class SecondsTreeNode : TreeNodeBase {
			public SecondsTreeNode(IIfTimeNode node) : base(node) {
			}

			public override bool Edit() {
				IfTimeNumericNode	d = new IfTimeNumericNode("Seconds", Node, 0, 59);

				if(d.ShowDialog() == DialogResult.OK)
					return true;
				return false;
			}

		}

		class MinutesTreeNode : TreeNodeBase {
			public MinutesTreeNode(IIfTimeNode node) : base(node) {
			}

			public override bool Edit() {
				IfTimeNumericNode	d = new IfTimeNumericNode("Minutes", Node, 0, 59);

				if(d.ShowDialog() == DialogResult.OK)
					return true;
				return false;
			}

		}

		class HoursTreeNode : TreeNodeBase {
			public HoursTreeNode(IIfTimeNode node) : base(node) {
			}

			public override bool Edit() {
				IfTimeNumericNode	d = new IfTimeNumericNode("Hours", Node, 0, 23);

				if(d.ShowDialog() == DialogResult.OK)
					return true;
				return false;
			}
		}

		class DayOfWeekTreeNode : TreeNodeBase {
			public DayOfWeekTreeNode(IIfTimeNode node) : base(node) {
			}

			public override bool Edit() {
				IfTimeDayOfWeekNode	d = new IfTimeDayOfWeekNode("Day Of Week", Node);

				if(d.ShowDialog() == DialogResult.OK)
					return true;
				return false;
			}
		}
	}
}
