using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using LayoutManager.CommonUI;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ManualDispatchRegion.
    /// </summary>
    partial class ManualDispatchRegion : Form, IModelComponentReceiverDialog, IObjectHasXml {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBoxBlocks = new System.Windows.Forms.ListBox();
            this.buttonAddBlock = new System.Windows.Forms.Button();
            this.buttonRemoveBlock = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxBlocks
            // 
            this.listBoxBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxBlocks.DisplayMember = "Name";
            this.listBoxBlocks.ItemHeight = 32;
            this.listBoxBlocks.Location = new System.Drawing.Point(21, 79);
            this.listBoxBlocks.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.listBoxBlocks.Name = "listBoxBlocks";
            this.listBoxBlocks.Size = new System.Drawing.Size(555, 484);
            this.listBoxBlocks.TabIndex = 0;
            // 
            // buttonAddBlock
            // 
            this.buttonAddBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAddBlock.Location = new System.Drawing.Point(21, 591);
            this.buttonAddBlock.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonAddBlock.Name = "buttonAddBlock";
            this.buttonAddBlock.Size = new System.Drawing.Size(174, 54);
            this.buttonAddBlock.TabIndex = 1;
            this.buttonAddBlock.Text = "&Add";
            // 
            // buttonRemoveBlock
            // 
            this.buttonRemoveBlock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRemoveBlock.Location = new System.Drawing.Point(208, 591);
            this.buttonRemoveBlock.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonRemoveBlock.Name = "buttonRemoveBlock";
            this.buttonRemoveBlock.Size = new System.Drawing.Size(174, 54);
            this.buttonRemoveBlock.TabIndex = 2;
            this.buttonRemoveBlock.Text = "&Remove";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(229, 674);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(174, 54);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 39);
            this.label1.TabIndex = 4;
            this.label1.Text = "Region Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(229, 20);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(347, 39);
            this.textBoxName.TabIndex = 5;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(416, 674);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(174, 54);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // ManualDispatchRegion
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(603, 743);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonAddBlock);
            this.Controls.Add(this.listBoxBlocks);
            this.Controls.Add(this.buttonRemoveBlock);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "ManualDispatchRegion";
            this.ShowInTaskbar = false;
            this.Text = "Manual Dispatch Region";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private ListBox listBoxBlocks;
        private Button buttonAddBlock;
        private Button buttonRemoveBlock;
        private Label label1;
        private TextBox textBoxName;
        private Button buttonOk;
        private Button buttonCancel;

    }
}

