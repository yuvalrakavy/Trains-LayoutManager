using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.View;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for ArrangeAreas.
    /// </summary>
    partial class ArrangeViews : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArrangeViews));
            this.listBoxViews = new ListBox();
            this.buttonMoveDown = new Button();
            this.imageListButtons = new ImageList(this.components);
            this.buttonMoveUp = new Button();
            this.buttonClose = new Button();
            this.buttonNew = new Button();
            this.buttonDelete = new Button();
            this.buttonRename = new Button();
            this.SuspendLayout();
            // 
            // listBoxViews
            // 
            this.listBoxViews.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listBoxViews.DisplayMember = "Text";
            this.listBoxViews.Location = new System.Drawing.Point(8, 16);
            this.listBoxViews.Name = "listBoxViews";
            this.listBoxViews.Size = new System.Drawing.Size(216, 212);
            this.listBoxViews.TabIndex = 0;
            this.listBoxViews.SelectedIndexChanged += this.ListBoxViews_SelectedIndexChanged;
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveDown.Image = (System.Drawing.Bitmap)resources.GetObject("buttonMoveDown.Image");
            this.buttonMoveDown.ImageIndex = 1;
            this.buttonMoveDown.ImageList = this.imageListButtons;
            this.buttonMoveDown.Location = new System.Drawing.Point(230, 48);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(32, 23);
            this.buttonMoveDown.TabIndex = 2;
            this.buttonMoveDown.Click += this.ButtonMoveDown_Click;
            // 
            // imageListButtons
            // 
            this.imageListButtons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListButtons.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListButtons.ImageStream = (ImageListStreamer)resources.GetObject("imageListButtons.ImageStream");
            this.imageListButtons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveUp.Image = (System.Drawing.Bitmap)resources.GetObject("buttonMoveUp.Image");
            this.buttonMoveUp.ImageIndex = 0;
            this.buttonMoveUp.ImageList = this.imageListButtons;
            this.buttonMoveUp.Location = new System.Drawing.Point(230, 16);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(32, 23);
            this.buttonMoveUp.TabIndex = 1;
            this.buttonMoveUp.Click += this.ButtonMoveUp_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.Location = new System.Drawing.Point(152, 272);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(72, 23);
            this.buttonClose.TabIndex = 6;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // buttonNew
            // 
            this.buttonNew.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonNew.Location = new System.Drawing.Point(8, 240);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(72, 24);
            this.buttonNew.TabIndex = 3;
            this.buttonNew.Text = "New...";
            this.buttonNew.Click += this.ButtonNew_Click;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonDelete.Location = new System.Drawing.Point(80, 240);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(72, 24);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.ButtonDelete_Click;
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRename.Location = new System.Drawing.Point(152, 240);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(72, 24);
            this.buttonRename.TabIndex = 5;
            this.buttonRename.Text = "Rename...";
            this.buttonRename.Click += this.ButtonRename_Click;
            // 
            // ArrangeViews
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 302);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonRename,
                                                                          this.buttonDelete,
                                                                          this.buttonNew,
                                                                          this.buttonClose,
                                                                          this.buttonMoveUp,
                                                                          this.buttonMoveDown,
                                                                          this.listBoxViews});
            this.Name = "ArrangeViews";
            this.ShowInTaskbar = false;
            this.Text = "Arrange Views";
            this.ResumeLayout(false);
        }
        #endregion

        private Button buttonMoveDown;
        private ImageList imageListButtons;
        private Button buttonMoveUp;
        private Button buttonClose;
        private Button buttonNew;
        private Button buttonDelete;
        private Button buttonRename;
        private IContainer components;
        private ListBox listBoxViews;
    }
}

