using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ImageProperties.
    /// </summary>
    partial class ImageProperties : Form, ILayoutComponentPropertiesDialog {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxImageFile = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numericUpDownWidth = new System.Windows.Forms.NumericUpDown();
            this.radioButtonWidthSet = new System.Windows.Forms.RadioButton();
            this.radioButtonWidthOriginal = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownHeight = new System.Windows.Forms.NumericUpDown();
            this.radioButtonHeightSet = new System.Windows.Forms.RadioButton();
            this.radioButtonHeightOriginal = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownVerticalOffset = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownHorizontalOffset = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.radioButtonEffectTile = new System.Windows.Forms.RadioButton();
            this.radioButtonEffectStretch = new System.Windows.Forms.RadioButton();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.linkMenuHorizontalAlignment = new CommonUI.Controls.LinkMenu();
            this.linkMenuVerticalAlignment = new CommonUI.Controls.LinkMenu();
            this.linkMenuRotateFlip = new CommonUI.Controls.LinkMenu();
            this.linkMenuOrigin = new CommonUI.Controls.LinkMenu();
            this.linkMenuVerticalUnits = new CommonUI.Controls.LinkMenu();
            this.linkMenuHorizontalUnits = new CommonUI.Controls.LinkMenu();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVerticalOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHorizontalOffset)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image file:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxImageFile
            // 
            this.textBoxImageFile.Location = new System.Drawing.Point(166, 34);
            this.textBoxImageFile.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxImageFile.Name = "textBoxImageFile";
            this.textBoxImageFile.Size = new System.Drawing.Size(493, 39);
            this.textBoxImageFile.TabIndex = 1;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(675, 29);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(166, 49);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "&Browse...";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numericUpDownWidth);
            this.groupBox1.Controls.Add(this.radioButtonWidthSet);
            this.groupBox1.Controls.Add(this.radioButtonWidthOriginal);
            this.groupBox1.Location = new System.Drawing.Point(16, 118);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Size = new System.Drawing.Size(832, 177);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Width:";
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Location = new System.Drawing.Point(270, 98);
            this.numericUpDownWidth.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.numericUpDownWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(166, 39);
            this.numericUpDownWidth.TabIndex = 2;
            this.numericUpDownWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonWidthSet
            // 
            this.radioButtonWidthSet.Location = new System.Drawing.Point(23, 98);
            this.radioButtonWidthSet.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonWidthSet.Name = "radioButtonWidthSet";
            this.radioButtonWidthSet.Size = new System.Drawing.Size(231, 39);
            this.radioButtonWidthSet.TabIndex = 1;
            this.radioButtonWidthSet.Text = "Set width to:";
            // 
            // radioButtonWidthOriginal
            // 
            this.radioButtonWidthOriginal.Location = new System.Drawing.Point(23, 39);
            this.radioButtonWidthOriginal.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonWidthOriginal.Name = "radioButtonWidthOriginal";
            this.radioButtonWidthOriginal.Size = new System.Drawing.Size(746, 39);
            this.radioButtonWidthOriginal.TabIndex = 0;
            this.radioButtonWidthOriginal.Text = "Keep original width";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericUpDownHeight);
            this.groupBox2.Controls.Add(this.radioButtonHeightSet);
            this.groupBox2.Controls.Add(this.radioButtonHeightOriginal);
            this.groupBox2.Location = new System.Drawing.Point(16, 295);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox2.Size = new System.Drawing.Size(832, 177);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Height:";
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Location = new System.Drawing.Point(270, 98);
            this.numericUpDownHeight.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.numericUpDownHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(166, 39);
            this.numericUpDownHeight.TabIndex = 2;
            this.numericUpDownHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radioButtonHeightSet
            // 
            this.radioButtonHeightSet.Location = new System.Drawing.Point(23, 98);
            this.radioButtonHeightSet.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonHeightSet.Name = "radioButtonHeightSet";
            this.radioButtonHeightSet.Size = new System.Drawing.Size(231, 39);
            this.radioButtonHeightSet.TabIndex = 1;
            this.radioButtonHeightSet.Text = "Set height to:";
            // 
            // radioButtonHeightOriginal
            // 
            this.radioButtonHeightOriginal.Location = new System.Drawing.Point(23, 39);
            this.radioButtonHeightOriginal.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonHeightOriginal.Name = "radioButtonHeightOriginal";
            this.radioButtonHeightOriginal.Size = new System.Drawing.Size(767, 39);
            this.radioButtonHeightOriginal.TabIndex = 0;
            this.radioButtonHeightOriginal.Text = "Keep original height";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.numericUpDownVerticalOffset);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownHorizontalOffset);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(16, 473);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox3.Size = new System.Drawing.Size(832, 197);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Position:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(21, 143);
            this.label6.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(291, 39);
            this.label6.TabIndex = 8;
            this.label6.Text = "This is the position of";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(416, 94);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(395, 39);
            this.label5.TabIndex = 6;
            this.label5.Text = "of image component position";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownVerticalOffset
            // 
            this.numericUpDownVerticalOffset.Location = new System.Drawing.Point(666, 34);
            this.numericUpDownVerticalOffset.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.numericUpDownVerticalOffset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownVerticalOffset.Name = "numericUpDownVerticalOffset";
            this.numericUpDownVerticalOffset.Size = new System.Drawing.Size(125, 39);
            this.numericUpDownVerticalOffset.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(416, 39);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(250, 39);
            this.label3.TabIndex = 2;
            this.label3.Text = "Vertical offset of: ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownHorizontalOffset
            // 
            this.numericUpDownHorizontalOffset.Location = new System.Drawing.Point(291, 34);
            this.numericUpDownHorizontalOffset.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.numericUpDownHorizontalOffset.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownHorizontalOffset.Name = "numericUpDownHorizontalOffset";
            this.numericUpDownHorizontalOffset.Size = new System.Drawing.Size(125, 39);
            this.numericUpDownHorizontalOffset.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(21, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(270, 39);
            this.label2.TabIndex = 0;
            this.label2.Text = "Horizontal offset of: ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(21, 94);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(234, 39);
            this.label4.TabIndex = 4;
            this.label4.Text = "Pixels relative to: ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(608, 143);
            this.label7.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(187, 39);
            this.label7.TabIndex = 8;
            this.label7.Text = "of the image.";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.radioButtonEffectTile);
            this.groupBox4.Controls.Add(this.radioButtonEffectStretch);
            this.groupBox4.Location = new System.Drawing.Point(16, 670);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox4.Size = new System.Drawing.Size(832, 158);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Effect:";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(437, 39);
            this.label8.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(312, 39);
            this.label8.TabIndex = 2;
            this.label8.Text = "Rotate && Flip Effect:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radioButtonEffectTile
            // 
            this.radioButtonEffectTile.Location = new System.Drawing.Point(23, 98);
            this.radioButtonEffectTile.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonEffectTile.Name = "radioButtonEffectTile";
            this.radioButtonEffectTile.Size = new System.Drawing.Size(333, 39);
            this.radioButtonEffectTile.TabIndex = 1;
            this.radioButtonEffectTile.Text = "Tile image";
            // 
            // radioButtonEffectStretch
            // 
            this.radioButtonEffectStretch.Location = new System.Drawing.Point(23, 39);
            this.radioButtonEffectStretch.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonEffectStretch.Name = "radioButtonEffectStretch";
            this.radioButtonEffectStretch.Size = new System.Drawing.Size(333, 39);
            this.radioButtonEffectStretch.TabIndex = 0;
            this.radioButtonEffectStretch.Text = "Stretch image";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(442, 847);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(195, 57);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "OK";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(650, 847);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(195, 57);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            // 
            // ImageProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(858, 930);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.textBoxImageFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "ImageProperties";
            this.ShowInTaskbar = false;
            this.Text = "Image Properties";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHeight)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownVerticalOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHorizontalOffset)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private Label label1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private GroupBox groupBox4;
        private Button buttonOK;
        private Button buttonCancel;
        private TextBox textBoxImageFile;
        private Button buttonBrowse;
        private RadioButton radioButtonWidthOriginal;
        private RadioButton radioButtonWidthSet;
        private NumericUpDown numericUpDownWidth;
        private NumericUpDown numericUpDownHeight;
        private RadioButton radioButtonHeightSet;
        private RadioButton radioButtonHeightOriginal;
        private NumericUpDown numericUpDownHorizontalOffset;
        private NumericUpDown numericUpDownVerticalOffset;
        private RadioButton radioButtonEffectStretch;
        private RadioButton radioButtonEffectTile;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuHorizontalUnits;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuVerticalUnits;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuOrigin;
        private Label label6;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuVerticalAlignment;
        private Label label7;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuHorizontalAlignment;
        private Label label8;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuRotateFlip;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}
