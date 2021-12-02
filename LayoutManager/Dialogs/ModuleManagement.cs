using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

namespace LayoutManager.Dialogs {

    /// <summary>
    /// Implement the module management dialog
    /// </summary>
    public partial class ModuleManagement : Form {
        private bool needToSaveState = false;

        public ModuleManagement() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (LayoutAssembly layoutAssembly in LayoutController.ModuleManager.LayoutAssemblies)
                treeViewAssemblies.Nodes.Add(new AssemblyNode(layoutAssembly));

            UpdateButtons();
        }

        private void UpdateButtons() {
            TreeNode selected = treeViewAssemblies.SelectedNode;

            if (selected != null) {
                if (selected is AssemblyNode) {
                    LayoutAssembly la = ((AssemblyNode)selected).LayoutAssembly;

                    if (la.SaveAssemblyReference)
                        buttonChangeStatus.Text = "&Temporary";
                    else
                        buttonChangeStatus.Text = "&Save";

                    if (la.Origin != LayoutAssembly.AssemblyOrigin.UserLoaded) {
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
                    LayoutModule module = ((ModuleNode)selected).Module;

                    buttonRemove.Visible = false;

                    if (module.Enabled)
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
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonAdd_Click(object? sender, EventArgs e) {
            if (openFileDialogAssembly.ShowDialog() == DialogResult.OK) {
                try {
                    LayoutAssembly layoutAssembly = new(openFileDialogAssembly.FileName);

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

        private void TreeViewAssemblies_AfterSelect(object? sender, TreeViewEventArgs e) {
            UpdateButtons();
        }

        private void ButtonChangeStatus_Click(object? sender, EventArgs e) {
            TreeNode selected = treeViewAssemblies.SelectedNode;

            if (selected != null) {
                if (selected is AssemblyNode) {
                    ((AssemblyNode)selected).ToggleSaveState();
                    needToSaveState = true;
                }
                else
                    ((ModuleNode)selected).ToggleEnableState();

                UpdateButtons();
            }
        }

        private void ButtonRemove_Click(object? sender, EventArgs e) {
            TreeNode selected = treeViewAssemblies.SelectedNode;

            if (selected != null) {
                if (selected is AssemblyNode) {
                    LayoutController.ModuleManager.LayoutAssemblies.Remove(((AssemblyNode)selected).LayoutAssembly);
                    selected.Remove();
                    needToSaveState = true;
                }
            }

            UpdateButtons();
        }

        private void ModuleManagement_Closed(object? sender, EventArgs e) {
            if (needToSaveState)
                LayoutController.ModuleManager.SaveState();
        }
    }

    internal class AssemblyNode : TreeNode {
        const int Image_SaveModuleReference = 0;
        const int Image_TemporaryModuleReference = 1;

        internal AssemblyNode(LayoutAssembly layoutAssembly) {
            this.LayoutAssembly = layoutAssembly;

            SetName();

            foreach (LayoutModule module in layoutAssembly.LayoutModules)
                Nodes.Add(new ModuleNode(module));
        }

        private void SetName() {
            this.Text = Path.GetFileName(LayoutAssembly.AssemblyFilename);

            if (LayoutAssembly.Origin == LayoutAssembly.AssemblyOrigin.BuiltIn) {
                this.Text += " (Builtin)";
                this.ImageIndex = Image_SaveModuleReference;
                this.SelectedImageIndex = Image_SaveModuleReference;
            }
            else if (LayoutAssembly.Origin == LayoutAssembly.AssemblyOrigin.InModulesDirectory) {
                this.Text += " (Modules directory)";
                this.ImageIndex = Image_SaveModuleReference;
                this.SelectedImageIndex = Image_SaveModuleReference;
            }
            else if (!LayoutAssembly.SaveAssemblyReference) {
                this.Text += " (Temporary)";
                this.ImageIndex = Image_TemporaryModuleReference;
                this.SelectedImageIndex = Image_TemporaryModuleReference;
            }
            else {
                this.ImageIndex = Image_SaveModuleReference;
                this.SelectedImageIndex = Image_SaveModuleReference;
            }
        }

        internal LayoutAssembly LayoutAssembly { get; }

        internal void ToggleSaveState() {
            LayoutAssembly.SaveAssemblyReference = !LayoutAssembly.SaveAssemblyReference;
            SetName();
        }
    }

    internal class ModuleNode : TreeNode {
        const int Image_EnabledModule = 2;
        const int Image_DisbaledModule = 3;

        internal ModuleNode(LayoutModule module) {
            this.Module = module;

            SetName();
        }

        private void SetName() {
            this.Text = Module.ModuleName;

            if (!Module.Enabled) {
                Text += " (Disabled)";
                this.ImageIndex = Image_DisbaledModule;
                this.SelectedImageIndex = Image_DisbaledModule;
            }
            else {
                this.ImageIndex = Image_EnabledModule;
                this.SelectedImageIndex = Image_EnabledModule;
            }
        }

        internal LayoutModule Module { get; }

        internal void ToggleEnableState() {
            Module.Enabled = !Module.Enabled;
            SetName();
        }
    }
}
