﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Gui.Wizard {
    /// <summary>
    /// Summary description for UserControl1.
    /// </summary>
    partial class InfoContainer : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoContainer));
            this.picImage = new PictureBox();
            this.lblTitle = new Label();
            this.SuspendLayout();
            // 
            // picImage
            // 
            this.picImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.picImage.Image = (System.Drawing.Image)resources.GetObject("picImage.Image");
            this.picImage.Location = new System.Drawing.Point(0, 0);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(164, 388);
            this.picImage.TabIndex = 0;
            this.picImage.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
            this.lblTitle.Location = new System.Drawing.Point(172, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(304, 48);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "Welcome to the / Completing the <Title> Wizard";
            // 
            // InfoContainer
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.picImage);
            this.Name = "InfoContainer";
            this.Size = new System.Drawing.Size(480, 388);
            this.Load += new EventHandler(this.InfoContainer_Load);
            this.ResumeLayout(false);
        }
        #endregion
        private PictureBox picImage;
        private Label lblTitle;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
