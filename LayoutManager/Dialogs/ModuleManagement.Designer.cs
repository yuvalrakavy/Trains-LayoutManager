using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Implement the module management dialog
    /// </summary>
    partial class ModuleManagement : Form {
        private Button buttonClose;
        private Button buttonAdd;
        private IContainer components;
        private OpenFileDialog openFileDialogAssembly;
        private Button buttonChangeStatus;
        private TreeView treeViewAssemblies;
        private Button buttonRemove;
        private ImageList imageListTree;
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModuleManagement));
            this.buttonRemove = new Button();
            this.buttonChangeStatus = new Button();
            this.treeViewAssemblies = new TreeView();
            this.imageListTree = new ImageList(this.components);
            this.buttonClose = new Button();
            this.buttonAdd = new Button();
            this.openFileDialogAssembly = new OpenFileDialog();
            this.SuspendLayout();
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(96, 256);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.TabIndex = 5;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.ButtonRemove_Click;
            // 
            // buttonChangeStatus
            // 
            this.buttonChangeStatus.Location = new System.Drawing.Point(8, 256);
            this.buttonChangeStatus.Name = "buttonChangeStatus";
            this.buttonChangeStatus.Size = new System.Drawing.Size(80, 23);
            this.buttonChangeStatus.TabIndex = 1;
            this.buttonChangeStatus.Text = "Op";
            this.buttonChangeStatus.Click += this.ButtonChangeStatus_Click;
            // 
            // treeViewAssemblies
            // 
            this.treeViewAssemblies.HideSelection = false;
            this.treeViewAssemblies.ImageList = this.imageListTree;
            this.treeViewAssemblies.Location = new System.Drawing.Point(8, 8);
            this.treeViewAssemblies.Name = "treeViewAssemblies";
            this.treeViewAssemblies.Size = new System.Drawing.Size(232, 240);
            this.treeViewAssemblies.TabIndex = 4;
            this.treeViewAssemblies.AfterSelect += this.TreeViewAssemblies_AfterSelect;
            // 
            // imageListTree
            // 
            this.imageListTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListTree.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListTree.ImageStream = (ImageListStreamer)resources.GetObject("imageListTree.ImageStream");
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
            this.buttonAdd.Click += this.ButtonAdd_Click;
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
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(346, 290);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
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
            this.Closed += this.ModuleManagement_Closed;
            this.ResumeLayout(false);
        }
        #endregion

    }
}

