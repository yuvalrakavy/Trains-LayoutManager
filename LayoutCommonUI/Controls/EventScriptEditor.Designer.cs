namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for EventScriptEditor.
    /// </summary>
    partial class EventScriptEditor : System.Windows.Forms.UserControl, IControlSupportViewOnly, IEventScriptEditor {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventScriptEditor));
            this.treeViewConditions = new System.Windows.Forms.TreeView();
            this.imageListConditionTree = new System.Windows.Forms.ImageList(this.components);
            this.buttonAddCondition = new System.Windows.Forms.Button();
            this.buttonEditCondition = new System.Windows.Forms.Button();
            this.buttonDeleteCondition = new System.Windows.Forms.Button();
            this.buttonMoveConditionDown = new System.Windows.Forms.Button();
            this.imageListButttons = new System.Windows.Forms.ImageList(this.components);
            this.buttonMoveConditionUp = new System.Windows.Forms.Button();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.buttonInsert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeViewConditions
            // 
            this.treeViewConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewConditions.HideSelection = false;
            this.treeViewConditions.ImageIndex = 0;
            this.treeViewConditions.ImageList = this.imageListConditionTree;
            this.treeViewConditions.Location = new System.Drawing.Point(8, 8);
            this.treeViewConditions.Name = "treeViewConditions";
            this.treeViewConditions.SelectedImageIndex = 0;
            this.treeViewConditions.Size = new System.Drawing.Size(386, 290);
            this.treeViewConditions.TabIndex = 0;
            // 
            // imageListConditionTree
            // 
            this.imageListConditionTree.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListConditionTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListConditionTree.ImageStream")));
            this.imageListConditionTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListConditionTree.Images.SetKeyName(0, "");
            this.imageListConditionTree.Images.SetKeyName(1, "");
            this.imageListConditionTree.Images.SetKeyName(2, "");
            this.imageListConditionTree.Images.SetKeyName(3, "");
            this.imageListConditionTree.Images.SetKeyName(4, "");
            this.imageListConditionTree.Images.SetKeyName(5, "");
            this.imageListConditionTree.Images.SetKeyName(6, "");
            // 
            // buttonAddCondition
            // 
            this.buttonAddCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddCondition.Location = new System.Drawing.Point(8, 305);
            this.buttonAddCondition.Name = "buttonAddCondition";
            this.buttonAddCondition.Size = new System.Drawing.Size(54, 21);
            this.buttonAddCondition.TabIndex = 1;
            this.buttonAddCondition.Text = "&Add";
            // 
            // buttonEditCondition
            // 
            this.buttonEditCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEditCondition.Location = new System.Drawing.Point(128, 305);
            this.buttonEditCondition.Name = "buttonEditCondition";
            this.buttonEditCondition.Size = new System.Drawing.Size(54, 21);
            this.buttonEditCondition.TabIndex = 3;
            this.buttonEditCondition.Text = "&Edit";
            // 
            // buttonDeleteCondition
            // 
            this.buttonDeleteCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteCondition.Location = new System.Drawing.Point(248, 305);
            this.buttonDeleteCondition.Name = "buttonDeleteCondition";
            this.buttonDeleteCondition.Size = new System.Drawing.Size(54, 21);
            this.buttonDeleteCondition.TabIndex = 5;
            this.buttonDeleteCondition.Text = "&Delete";
            // 
            // buttonMoveConditionDown
            // 
            this.buttonMoveConditionDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveConditionDown.ImageIndex = 0;
            this.buttonMoveConditionDown.ImageList = this.imageListButttons;
            this.buttonMoveConditionDown.Location = new System.Drawing.Point(398, 32);
            this.buttonMoveConditionDown.Name = "buttonMoveConditionDown";
            this.buttonMoveConditionDown.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveConditionDown.TabIndex = 7;
            // 
            // imageListButttons
            // 
            this.imageListButttons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListButttons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButttons.ImageStream")));
            this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListButttons.Images.SetKeyName(0, "");
            this.imageListButttons.Images.SetKeyName(1, "");
            // 
            // buttonMoveConditionUp
            // 
            this.buttonMoveConditionUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMoveConditionUp.ImageIndex = 1;
            this.buttonMoveConditionUp.ImageList = this.imageListButttons;
            this.buttonMoveConditionUp.Location = new System.Drawing.Point(398, 8);
            this.buttonMoveConditionUp.Name = "buttonMoveConditionUp";
            this.buttonMoveConditionUp.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveConditionUp.TabIndex = 6;
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOptions.Location = new System.Drawing.Point(188, 305);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(54, 21);
            this.buttonOptions.TabIndex = 4;
            this.buttonOptions.Text = "&Options";
            // 
            // buttonInsert
            // 
            this.buttonInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonInsert.Location = new System.Drawing.Point(68, 305);
            this.buttonInsert.Name = "buttonInsert";
            this.buttonInsert.Size = new System.Drawing.Size(54, 21);
            this.buttonInsert.TabIndex = 2;
            this.buttonInsert.Text = "&Insert";
            // 
            // EventScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.buttonMoveConditionUp);
            this.Controls.Add(this.buttonAddCondition);
            this.Controls.Add(this.treeViewConditions);
            this.Controls.Add(this.buttonEditCondition);
            this.Controls.Add(this.buttonDeleteCondition);
            this.Controls.Add(this.buttonMoveConditionDown);
            this.Controls.Add(this.buttonOptions);
            this.Controls.Add(this.buttonInsert);
            this.Name = "EventScriptEditor";
            this.Size = new System.Drawing.Size(426, 330);
            this.ResumeLayout(false);

        }
        #endregion

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
        private System.ComponentModel.IContainer components;

    }
}

