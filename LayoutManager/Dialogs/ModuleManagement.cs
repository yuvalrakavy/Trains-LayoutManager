using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Implement the module management dialog
	/// </summary>
	public class ModuleManagement : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Button buttonAdd;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.OpenFileDialog openFileDialogAssembly;
		private System.Windows.Forms.Button buttonChangeStatus;
		private System.Windows.Forms.TreeView treeViewAssemblies;
		private System.Windows.Forms.Button buttonRemove;

		// Images
		// 0 - Saved module reference
		// 1 - Temporary module reference
		// 2 - Disabled module
		// 3 - Enabled module
		private System.Windows.Forms.ImageList imageListTree;

		bool				needToSaveState = false;

		public ModuleManagement()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			foreach(LayoutAssembly layoutAssembly in LayoutController.ModuleManager.LayoutAssemblies)
				treeViewAssemblies.Nodes.Add(new AssemblyNode(layoutAssembly));

			updateButtons();
		}

		void updateButtons() {
			TreeNode	selected = treeViewAssemblies.SelectedNode;

			if(selected != null) {
				if(selected is AssemblyNode) {
					LayoutAssembly	la = ((AssemblyNode)selected).LayoutAssembly;

					if(la.SaveAssemblyReference)
						buttonChangeStatus.Text = "&Temporary";
					else
						buttonChangeStatus.Text = "&Save";

					if(la.Origin != LayoutAssembly.AssemblyOrigin.UserLoaded) {
						buttonRemove.Visible = false;
						buttonChangeStatus.Visible = false;
					}
					else {
						buttonChangeStatus.Visible = true;
						buttonChangeStatus.Enabled = true;

						buttonRemove.Visible = true;
					}
				}
				else {
					LayoutModule	module = ((ModuleNode)selected).Module;

					buttonRemove.Visible = false;

					if(module.Enabled)
						buttonChangeStatus.Text = "&Disable";
					else
						buttonChangeStatus.Text = "&Enable";

					buttonChangeStatus.Visible = true;
					buttonChangeStatus.Enabled = module.UserControl;
				}
			}
			else {
				buttonChangeStatus.Visible = false;
				buttonRemove.Visible = false;
			}
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ModuleManagement));
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonChangeStatus = new System.Windows.Forms.Button();
			this.treeViewAssemblies = new System.Windows.Forms.TreeView();
			this.imageListTree = new System.Windows.Forms.ImageList(this.components);
			this.buttonClose = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.openFileDialogAssembly = new System.Windows.Forms.OpenFileDialog();
			this.SuspendLayout();
			// 
			// buttonRemove
			// 
			this.buttonRemove.Location = new System.Drawing.Point(96, 256);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.TabIndex = 5;
			this.buttonRemove.Text = "&Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// buttonChangeStatus
			// 
			this.buttonChangeStatus.Location = new System.Drawing.Point(8, 256);
			this.buttonChangeStatus.Name = "buttonChangeStatus";
			this.buttonChangeStatus.Size = new System.Drawing.Size(80, 23);
			this.buttonChangeStatus.TabIndex = 1;
			this.buttonChangeStatus.Text = "Op";
			this.buttonChangeStatus.Click += new System.EventHandler(this.buttonChangeStatus_Click);
			// 
			// treeViewAssemblies
			// 
			this.treeViewAssemblies.HideSelection = false;
			this.treeViewAssemblies.ImageList = this.imageListTree;
			this.treeViewAssemblies.Location = new System.Drawing.Point(8, 8);
			this.treeViewAssemblies.Name = "treeViewAssemblies";
			this.treeViewAssemblies.Size = new System.Drawing.Size(232, 240);
			this.treeViewAssemblies.TabIndex = 4;
			this.treeViewAssemblies.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewAssemblies_AfterSelect);
			// 
			// imageListTree
			// 
			this.imageListTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListTree.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
			this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(248, 256);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 3;
			this.buttonClose.Text = "&Close";
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(248, 224);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.TabIndex = 2;
			this.buttonAdd.Text = "&Add...";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// openFileDialogAssembly
			// 
			this.openFileDialogAssembly.DefaultExt = "dll";
			this.openFileDialogAssembly.Filter = "DLL files|*.dll";
			this.openFileDialogAssembly.Title = "Select Layout Module Assembly file";
			// 
			// ModuleManagement
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(346, 290);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonRemove,
																		  this.treeViewAssemblies,
																		  this.buttonChangeStatus,
																		  this.buttonAdd,
																		  this.buttonClose});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "ModuleManagement";
			this.ShowInTaskbar = false;
			this.Text = "Module Management";
			this.Closed += new System.EventHandler(this.ModuleManagement_Closed);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonAdd_Click(object sender, System.EventArgs e) {
			if(openFileDialogAssembly.ShowDialog() == DialogResult.OK) {
				try {
					LayoutAssembly	layoutAssembly = new LayoutAssembly(openFileDialogAssembly.FileName);

					LayoutController.ModuleManager.LayoutAssemblies.Add(layoutAssembly);
					treeViewAssemblies.Nodes.Add(new AssemblyNode(layoutAssembly));
					needToSaveState = true;
				}
				catch (Exception ex) {
					MessageBox.Show(this, String.Format("Error while trying to add layout assembly:\n\n{0}", ex.Message), "Error adding assembly", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void treeViewAssemblies_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			updateButtons();
		}

		private void buttonChangeStatus_Click(object sender, System.EventArgs e) {
			TreeNode	selected = treeViewAssemblies.SelectedNode;

			if(selected != null) {
				if(selected is AssemblyNode) {
					((AssemblyNode)selected).ToggleSaveState();
					needToSaveState = true;
				}
				else
					((ModuleNode)selected).ToggleEnableState();

				updateButtons();
			}
		}

		private void buttonRemove_Click(object sender, System.EventArgs e) {
			TreeNode	selected = treeViewAssemblies.SelectedNode;

			if(selected != null) {
				if(selected is AssemblyNode) {
					LayoutController.ModuleManager.LayoutAssemblies.Remove(((AssemblyNode)selected).LayoutAssembly);
					selected.Remove();
					needToSaveState = true;
				}
			}

			updateButtons();
		}

		private void ModuleManagement_Closed(object sender, System.EventArgs e) {
			if(needToSaveState)
				LayoutController.ModuleManager.SaveState();
		}
	}

	class AssemblyNode : TreeNode {
		LayoutAssembly	layoutAssembly;

		internal AssemblyNode(LayoutAssembly layoutAssembly) {
			this.layoutAssembly = layoutAssembly;

			setName();

			foreach(LayoutModule module in layoutAssembly.LayoutModules)
				Nodes.Add(new ModuleNode(module));
		}

		void setName() {
			this.Text = Path.GetFileName(layoutAssembly.AssemblyFilename);

			if(layoutAssembly.Origin == LayoutAssembly.AssemblyOrigin.BuiltIn) {
				this.Text += " (Builtin)";
				this.ImageIndex = 0;
				this.SelectedImageIndex = 0;
			}
			else if(layoutAssembly.Origin == LayoutAssembly.AssemblyOrigin.InModulesDirectory) {
				this.Text += " (Modules directory)";
				this.ImageIndex = 0;
				this.SelectedImageIndex = 0;
			}
			else if(!layoutAssembly.SaveAssemblyReference) {
				this.Text += " (Temporary)";
				this.ImageIndex = 1;
				this.SelectedImageIndex = 1;
			}
			else {
				this.ImageIndex = 0;
				this.SelectedImageIndex = 0;
			}
		}

		internal LayoutAssembly LayoutAssembly {
			get {
				return layoutAssembly;
			}
		}

		internal void ToggleSaveState() {
			layoutAssembly.SaveAssemblyReference = !layoutAssembly.SaveAssemblyReference;
			setName();
		}
	}

	class ModuleNode : TreeNode {
		LayoutModule	module;

		internal ModuleNode(LayoutModule module) {
			this.module = module;

			setName();
		}

		private void setName() {
			this.Text = module.ModuleName;

			if(!module.Enabled) {
				Text += " (Disabled)";
				this.ImageIndex = 3;
				this.SelectedImageIndex = 3;
			}
			else {
				this.ImageIndex = 2;
				this.SelectedImageIndex = 2;
			}
		}

		internal LayoutModule Module {
			get {
				return module;
			}
		}

		internal void ToggleEnableState() {
			module.Enabled = !module.Enabled;
			setName();
		}
	}
}
