using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for MotionRamps.
    /// </summary>
    partial class MotionRamps : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MotionRamps));
            this.listViewRamps = new ListView();
            this.buttonAdd = new Button();
            this.buttonEdit = new Button();
            this.buttonRemove = new Button();
            this.buttonMoveDown = new Button();
            this.imageListButttons = new ImageList(this.components);
            this.buttonMoveUp = new Button();
            this.buttonClose = new Button();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderUsage = new ColumnHeader();
            this.SuspendLayout();
            // 
            // listViewRamps
            // 
            this.listViewRamps.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listViewRamps.Columns.AddRange(new ColumnHeader[] {
                                                                                            this.columnHeaderName,
                                                                                            this.columnHeaderUsage});
            this.listViewRamps.FullRowSelect = true;
            this.listViewRamps.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewRamps.HideSelection = false;
            this.listViewRamps.Location = new System.Drawing.Point(8, 8);
            this.listViewRamps.MultiSelect = false;
            this.listViewRamps.Name = "listViewRamps";
            this.listViewRamps.Size = new System.Drawing.Size(288, 184);
            this.listViewRamps.TabIndex = 0;
            this.listViewRamps.View = System.Windows.Forms.View.Details;
            this.listViewRamps.SelectedIndexChanged += new EventHandler(this.UpdateButtons);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAdd.Location = new System.Drawing.Point(8, 200);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(67, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add...";
            this.buttonAdd.Click += new EventHandler(this.ButtonAdd_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonEdit.Location = new System.Drawing.Point(80, 200);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(67, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit...";
            this.buttonEdit.Click += new EventHandler(this.ButtonEdit_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRemove.Location = new System.Drawing.Point(152, 200);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(67, 23);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += new EventHandler(this.ButtonRemove_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveDown.Image = (System.Drawing.Bitmap)resources.GetObject("buttonMoveDown.Image");
            this.buttonMoveDown.ImageIndex = 0;
            this.buttonMoveDown.ImageList = this.imageListButttons;
            this.buttonMoveDown.Location = new System.Drawing.Point(304, 32);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveDown.TabIndex = 6;
            this.buttonMoveDown.Click += new EventHandler(this.ButtonMoveDown_Click);
            // 
            // imageListButttons
            // 
            this.imageListButttons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListButttons.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListButttons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListButttons.ImageStream");
            this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.buttonMoveUp.Image = (System.Drawing.Bitmap)resources.GetObject("buttonMoveUp.Image");
            this.buttonMoveUp.ImageIndex = 1;
            this.buttonMoveUp.ImageList = this.imageListButttons;
            this.buttonMoveUp.Location = new System.Drawing.Point(304, 8);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveUp.TabIndex = 5;
            this.buttonMoveUp.Click += new EventHandler(this.ButtonMoveUp_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.Location = new System.Drawing.Point(264, 200);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(67, 23);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 184;
            // 
            // columnHeaderUsage
            // 
            this.columnHeaderUsage.Text = "Usage";
            this.columnHeaderUsage.Width = 100;
            // 
            // MotionRamps
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(336, 230);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonMoveUp,
                                                                          this.buttonAdd,
                                                                          this.listViewRamps,
                                                                          this.buttonEdit,
                                                                          this.buttonRemove,
                                                                          this.buttonMoveDown,
                                                                          this.buttonClose});
            this.Name = "MotionRamps";
            this.Text = "Acceleration/Deceleration Profiles";
            this.ResumeLayout(false);
        }
        #endregion
        private ListView listViewRamps;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;
        private ImageList imageListButttons;
        private Button buttonClose;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderUsage;
        private Button buttonMoveDown;
        private Button buttonMoveUp;
        private IContainer components;
    }
}

