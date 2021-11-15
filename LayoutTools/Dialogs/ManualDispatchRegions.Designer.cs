using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.CommonUI;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ManualDispatchRegions.
    /// </summary>
    partial class ManualDispatchRegions : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBoxRegions = new System.Windows.Forms.ListBox();
            this.buttonNewRegion = new System.Windows.Forms.Button();
            this.buttonEditRegion = new System.Windows.Forms.Button();
            this.buttonDeleteRegion = new System.Windows.Forms.Button();
            this.buttonToggleRegionActivation = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxRegions
            // 
            this.listBoxRegions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxRegions.DisplayMember = "NameAndStatus";
            this.listBoxRegions.ItemHeight = 32;
            this.listBoxRegions.Location = new System.Drawing.Point(16, 20);
            this.listBoxRegions.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.listBoxRegions.Name = "listBoxRegions";
            this.listBoxRegions.Size = new System.Drawing.Size(602, 484);
            this.listBoxRegions.TabIndex = 0;
            // 
            // buttonNewRegion
            // 
            this.buttonNewRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonNewRegion.Location = new System.Drawing.Point(16, 532);
            this.buttonNewRegion.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonNewRegion.Name = "buttonNewRegion";
            this.buttonNewRegion.Size = new System.Drawing.Size(125, 44);
            this.buttonNewRegion.TabIndex = 1;
            this.buttonNewRegion.Text = "&New";
            // 
            // buttonEditRegion
            // 
            this.buttonEditRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEditRegion.Location = new System.Drawing.Point(151, 532);
            this.buttonEditRegion.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonEditRegion.Name = "buttonEditRegion";
            this.buttonEditRegion.Size = new System.Drawing.Size(125, 44);
            this.buttonEditRegion.TabIndex = 1;
            this.buttonEditRegion.Text = "&Edit";
            // 
            // buttonDeleteRegion
            // 
            this.buttonDeleteRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteRegion.Location = new System.Drawing.Point(289, 532);
            this.buttonDeleteRegion.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonDeleteRegion.Name = "buttonDeleteRegion";
            this.buttonDeleteRegion.Size = new System.Drawing.Size(125, 44);
            this.buttonDeleteRegion.TabIndex = 1;
            this.buttonDeleteRegion.Text = "&Delete";
            // 
            // buttonToggleRegionActivation
            // 
            this.buttonToggleRegionActivation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonToggleRegionActivation.Location = new System.Drawing.Point(426, 532);
            this.buttonToggleRegionActivation.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonToggleRegionActivation.Name = "buttonToggleRegionActivation";
            this.buttonToggleRegionActivation.Size = new System.Drawing.Size(198, 44);
            this.buttonToggleRegionActivation.TabIndex = 1;
            this.buttonToggleRegionActivation.Text = "&Activate";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(463, 603);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(166, 57);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            // 
            // ManualDispatchRegions
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 665);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonNewRegion);
            this.Controls.Add(this.listBoxRegions);
            this.Controls.Add(this.buttonEditRegion);
            this.Controls.Add(this.buttonDeleteRegion);
            this.Controls.Add(this.buttonToggleRegionActivation);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "ManualDispatchRegions";
            this.ShowInTaskbar = false;
            this.Text = "Manual Dispatch Regions";
            this.ResumeLayout(false);

        }
        #endregion
        private ListBox listBoxRegions;
        private Button buttonNewRegion;
        private Button buttonEditRegion;
        private Button buttonDeleteRegion;
        private Button buttonToggleRegionActivation;
        private Button buttonClose;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
