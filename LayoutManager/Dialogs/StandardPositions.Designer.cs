using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StandardPositions.
    /// </summary>
    partial class StandardPositions : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBoxPositions = new ListBox();
            this.buttonNew = new Button();
            this.buttonDelete = new Button();
            this.buttonEdit = new Button();
            this.buttonClose = new Button();
            this.SuspendLayout();
            // 
            // listBoxPositions
            // 
            this.listBoxPositions.Location = new System.Drawing.Point(8, 16);
            this.listBoxPositions.Name = "listBoxPositions";
            this.listBoxPositions.Size = new System.Drawing.Size(248, 238);
            this.listBoxPositions.TabIndex = 0;
            this.listBoxPositions.DoubleClick += new EventHandler(this.ButtonEdit_Click);
            // 
            // buttonNew
            // 
            this.buttonNew.Location = new System.Drawing.Point(16, 264);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.TabIndex = 1;
            this.buttonNew.Text = "&New";
            this.buttonNew.Click += new EventHandler(this.ButtonNew_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(176, 264);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += new EventHandler(this.ButtonDelete_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(96, 264);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += new EventHandler(this.ButtonEdit_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(176, 296);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // StandardPositions
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 346);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonClose,
                                                                          this.buttonDelete,
                                                                          this.buttonEdit,
                                                                          this.buttonNew,
                                                                          this.listBoxPositions});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StandardPositions";
            this.ShowInTaskbar = false;
            this.Text = "Standard Positions";
            this.ResumeLayout(false);
        }
        #endregion

        private ListBox listBoxPositions;
        private Button buttonNew;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonClose;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

