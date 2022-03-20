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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModuleManagement));
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
            this.buttonRemove.Location = new System.Drawing.Point(250, 630);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(195, 57);
            this.buttonRemove.TabIndex = 5;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
            // 
            // buttonChangeStatus
            // 
            this.buttonChangeStatus.Location = new System.Drawing.Point(21, 630);
            this.buttonChangeStatus.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonChangeStatus.Name = "buttonChangeStatus";
            this.buttonChangeStatus.Size = new System.Drawing.Size(208, 57);
            this.buttonChangeStatus.TabIndex = 1;
            this.buttonChangeStatus.Text = "Op";
            this.buttonChangeStatus.Click += new System.EventHandler(this.ButtonChangeStatus_Click);
            // 
            // treeViewAssemblies
            // 
            this.treeViewAssemblies.HideSelection = false;
            this.treeViewAssemblies.ImageIndex = 0;
            this.treeViewAssemblies.ImageList = this.imageListTree;
            this.treeViewAssemblies.Location = new System.Drawing.Point(21, 20);
            this.treeViewAssemblies.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.treeViewAssemblies.Name = "treeViewAssemblies";
            this.treeViewAssemblies.SelectedImageIndex = 0;
            this.treeViewAssemblies.Size = new System.Drawing.Size(597, 585);
            this.treeViewAssemblies.TabIndex = 4;
            this.treeViewAssemblies.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewAssemblies_AfterSelect);
            // 
            // imageListTree
            // 
            this.imageListTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTree.ImageStream")));
            this.imageListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTree.Images.SetKeyName(0, "");
            this.imageListTree.Images.SetKeyName(1, "");
            this.imageListTree.Images.SetKeyName(2, "");
            this.imageListTree.Images.SetKeyName(3, "");
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(645, 630);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(195, 57);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "&Close";
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(645, 551);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(195, 57);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(900, 714);
            this.ControlBox = false;
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.treeViewAssemblies);
            this.Controls.Add(this.buttonChangeStatus);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MaximizeBox = false;
            this.Name = "ModuleManagement";
            this.ShowInTaskbar = false;
            this.Text = "Module Management";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModuleManagement_FormClosed);
            this.ResumeLayout(false);

        }
        #endregion

    }
}

