﻿using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCatalogStandardImages.
    /// </summary>
    partial class LocomotiveCatalogStandardImages : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.groupBox2 = new GroupBox();
            this.pictureBoxES = new PictureBox();
            this.pictureBoxED = new PictureBox();
            this.pictureBoxEE = new PictureBox();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();
            this.buttonSetES = new Button();
            this.buttonClearES = new Button();
            this.buttonClearED = new Button();
            this.buttonSetED = new Button();
            this.buttonClearEE = new Button();
            this.buttonSetEE = new Button();
            this.buttonSetUS = new Button();
            this.label4 = new Label();
            this.pictureBoxUS = new PictureBox();
            this.pictureBoxUD = new PictureBox();
            this.pictureBoxUE = new PictureBox();
            this.label5 = new Label();
            this.label6 = new Label();
            this.buttonClearUS = new Button();
            this.buttonClearUD = new Button();
            this.buttonSetUD = new Button();
            this.buttonClearUE = new Button();
            this.buttonSetUE = new Button();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.buttonSetESU = new Button();
            this.label7 = new Label();
            this.pictureBoxESU = new PictureBox();
            this.buttonClearESU = new Button();
            this.buttonSetUSU = new Button();
            this.label8 = new Label();
            this.pictureBoxUSU = new PictureBox();
            this.buttonClearUSU = new Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new Control[] {
                                                                                    this.buttonSetES,
                                                                                    this.label1,
                                                                                    this.pictureBoxES,
                                                                                    this.pictureBoxED,
                                                                                    this.pictureBoxEE,
                                                                                    this.label2,
                                                                                    this.label3,
                                                                                    this.buttonClearES,
                                                                                    this.buttonClearED,
                                                                                    this.buttonSetED,
                                                                                    this.buttonClearEE,
                                                                                    this.buttonSetEE,
                                                                                    this.buttonSetESU,
                                                                                    this.label7,
                                                                                    this.pictureBoxESU,
                                                                                    this.buttonClearESU});
            this.groupBox1.Location = new Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(480, 112);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "European Locomotives:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.AddRange(new Control[] {
                                                                                    this.buttonSetUSU,
                                                                                    this.label8,
                                                                                    this.pictureBoxUSU,
                                                                                    this.buttonClearUSU,
                                                                                    this.buttonSetUS,
                                                                                    this.label4,
                                                                                    this.pictureBoxUS,
                                                                                    this.pictureBoxUD,
                                                                                    this.pictureBoxUE,
                                                                                    this.label5,
                                                                                    this.label6,
                                                                                    this.buttonClearUS,
                                                                                    this.buttonClearUD,
                                                                                    this.buttonSetUD,
                                                                                    this.buttonClearUE,
                                                                                    this.buttonSetUE});
            this.groupBox2.Location = new Point(8, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(480, 112);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "American Locomotives:";
            // 
            // pictureBoxES
            // 
            this.pictureBoxES.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxES.Location = new Point(8, 32);
            this.pictureBoxES.Name = "pictureBoxES";
            this.pictureBoxES.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxES.TabIndex = 0;
            this.pictureBoxES.TabStop = false;
            // 
            // pictureBoxED
            // 
            this.pictureBoxED.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxED.Location = new Point(128, 32);
            this.pictureBoxED.Name = "pictureBoxED";
            this.pictureBoxED.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxED.TabIndex = 0;
            this.pictureBoxED.TabStop = false;
            // 
            // pictureBoxEE
            // 
            this.pictureBoxEE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxEE.Location = new Point(248, 32);
            this.pictureBoxEE.Name = "pictureBoxEE";
            this.pictureBoxEE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxEE.TabIndex = 0;
            this.pictureBoxEE.TabStop = false;
            // 
            // label1
            // 
            this.label1.Location = new Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new Size(100, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Steam";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new Point(128, 16);
            this.label2.Name = "label2";
            this.label2.Size = new Size(100, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Diesel";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new Point(248, 16);
            this.label3.Name = "label3";
            this.label3.Size = new Size(100, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Electric";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonSetES
            // 
            this.buttonSetES.Location = new Point(8, 86);
            this.buttonSetES.Name = "buttonSetES";
            this.buttonSetES.Size = new Size(48, 19);
            this.buttonSetES.TabIndex = 2;
            this.buttonSetES.Text = "Set";
            this.buttonSetES.Click += new EventHandler(this.ButtonSet_click);
            // 
            // buttonClearES
            // 
            this.buttonClearES.Location = new Point(60, 86);
            this.buttonClearES.Name = "buttonClearES";
            this.buttonClearES.Size = new Size(48, 19);
            this.buttonClearES.TabIndex = 2;
            this.buttonClearES.Text = "Clear";
            this.buttonClearES.Click += new EventHandler(this.ButtonClear_click);
            // 
            // buttonClearED
            // 
            this.buttonClearED.Location = new Point(180, 86);
            this.buttonClearED.Name = "buttonClearED";
            this.buttonClearED.Size = new Size(48, 19);
            this.buttonClearED.TabIndex = 2;
            this.buttonClearED.Text = "Clear";
            this.buttonClearED.Click += new EventHandler(this.ButtonClear_click);
            // 
            // buttonSetED
            // 
            this.buttonSetED.Location = new Point(128, 86);
            this.buttonSetED.Name = "buttonSetED";
            this.buttonSetED.Size = new Size(48, 19);
            this.buttonSetED.TabIndex = 2;
            this.buttonSetED.Text = "Set";
            this.buttonSetED.Click += new EventHandler(this.ButtonSet_click);
            // 
            // buttonClearEE
            // 
            this.buttonClearEE.Location = new Point(300, 86);
            this.buttonClearEE.Name = "buttonClearEE";
            this.buttonClearEE.Size = new Size(48, 19);
            this.buttonClearEE.TabIndex = 2;
            this.buttonClearEE.Text = "Clear";
            this.buttonClearEE.Click += new EventHandler(this.ButtonClear_click);
            // 
            // buttonSetEE
            // 
            this.buttonSetEE.Location = new Point(248, 86);
            this.buttonSetEE.Name = "buttonSetEE";
            this.buttonSetEE.Size = new Size(48, 19);
            this.buttonSetEE.TabIndex = 2;
            this.buttonSetEE.Text = "Set";
            this.buttonSetEE.Click += new EventHandler(this.ButtonSet_click);
            // 
            // buttonSetUS
            // 
            this.buttonSetUS.Location = new Point(10, 84);
            this.buttonSetUS.Name = "buttonSetUS";
            this.buttonSetUS.Size = new Size(48, 19);
            this.buttonSetUS.TabIndex = 12;
            this.buttonSetUS.Text = "Set";
            this.buttonSetUS.Click += new EventHandler(this.ButtonSet_click);
            // 
            // label4
            // 
            this.label4.Location = new Point(10, 14);
            this.label4.Name = "label4";
            this.label4.Size = new Size(100, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "Steam";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxUS
            // 
            this.pictureBoxUS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxUS.Location = new Point(10, 30);
            this.pictureBoxUS.Name = "pictureBoxUS";
            this.pictureBoxUS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxUS.TabIndex = 5;
            this.pictureBoxUS.TabStop = false;
            // 
            // pictureBoxUD
            // 
            this.pictureBoxUD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxUD.Location = new Point(130, 30);
            this.pictureBoxUD.Name = "pictureBoxUD";
            this.pictureBoxUD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxUD.TabIndex = 3;
            this.pictureBoxUD.TabStop = false;
            // 
            // pictureBoxUE
            // 
            this.pictureBoxUE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxUE.Location = new Point(250, 30);
            this.pictureBoxUE.Name = "pictureBoxUE";
            this.pictureBoxUE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxUE.TabIndex = 4;
            this.pictureBoxUE.TabStop = false;
            // 
            // label5
            // 
            this.label5.Location = new Point(130, 14);
            this.label5.Name = "label5";
            this.label5.Size = new Size(100, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "Diesel";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Location = new Point(250, 14);
            this.label6.Name = "label6";
            this.label6.Size = new Size(100, 16);
            this.label6.TabIndex = 6;
            this.label6.Text = "Electric";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonClearUS
            // 
            this.buttonClearUS.Location = new Point(62, 84);
            this.buttonClearUS.Name = "buttonClearUS";
            this.buttonClearUS.Size = new Size(48, 19);
            this.buttonClearUS.TabIndex = 13;
            this.buttonClearUS.Text = "Clear";
            this.buttonClearUS.Click += new EventHandler(this.ButtonClear_click);
            // 
            // buttonClearUD
            // 
            this.buttonClearUD.Location = new Point(182, 84);
            this.buttonClearUD.Name = "buttonClearUD";
            this.buttonClearUD.Size = new Size(48, 19);
            this.buttonClearUD.TabIndex = 14;
            this.buttonClearUD.Text = "Clear";
            this.buttonClearUD.Click += new EventHandler(this.ButtonClear_click);
            // 
            // buttonSetUD
            // 
            this.buttonSetUD.Location = new Point(130, 84);
            this.buttonSetUD.Name = "buttonSetUD";
            this.buttonSetUD.Size = new Size(48, 19);
            this.buttonSetUD.TabIndex = 9;
            this.buttonSetUD.Text = "Set";
            this.buttonSetUD.Click += new EventHandler(this.ButtonSet_click);
            // 
            // buttonClearUE
            // 
            this.buttonClearUE.Location = new Point(302, 84);
            this.buttonClearUE.Name = "buttonClearUE";
            this.buttonClearUE.Size = new Size(48, 19);
            this.buttonClearUE.TabIndex = 10;
            this.buttonClearUE.Text = "Clear";
            this.buttonClearUE.Click += new EventHandler(this.ButtonClear_click);
            // 
            // buttonSetUE
            // 
            this.buttonSetUE.Location = new Point(250, 84);
            this.buttonSetUE.Name = "buttonSetUE";
            this.buttonSetUE.Size = new Size(48, 19);
            this.buttonSetUE.TabIndex = 11;
            this.buttonSetUE.Text = "Set";
            this.buttonSetUE.Click += new EventHandler(this.ButtonSet_click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new Point(331, 248);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new Point(413, 248);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonSetESU
            // 
            this.buttonSetESU.Location = new Point(368, 86);
            this.buttonSetESU.Name = "buttonSetESU";
            this.buttonSetESU.Size = new Size(48, 19);
            this.buttonSetESU.TabIndex = 2;
            this.buttonSetESU.Text = "Set";
            // 
            // label7
            // 
            this.label7.Location = new Point(368, 16);
            this.label7.Name = "label7";
            this.label7.Size = new Size(100, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "Sound Unit";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxESU
            // 
            this.pictureBoxESU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxESU.Location = new Point(368, 32);
            this.pictureBoxESU.Name = "pictureBoxESU";
            this.pictureBoxESU.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxESU.TabIndex = 0;
            this.pictureBoxESU.TabStop = false;
            // 
            // buttonClearESU
            // 
            this.buttonClearESU.Location = new Point(420, 86);
            this.buttonClearESU.Name = "buttonClearESU";
            this.buttonClearESU.Size = new Size(48, 19);
            this.buttonClearESU.TabIndex = 2;
            this.buttonClearESU.Text = "Clear";
            // 
            // buttonSetUSU
            // 
            this.buttonSetUSU.Location = new Point(368, 84);
            this.buttonSetUSU.Name = "buttonSetUSU";
            this.buttonSetUSU.Size = new Size(48, 19);
            this.buttonSetUSU.TabIndex = 18;
            this.buttonSetUSU.Text = "Set";
            // 
            // label8
            // 
            this.label8.Location = new Point(368, 14);
            this.label8.Name = "label8";
            this.label8.Size = new Size(100, 16);
            this.label8.TabIndex = 16;
            this.label8.Text = "Sound Unit";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxUSU
            // 
            this.pictureBoxUSU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxUSU.Location = new Point(368, 30);
            this.pictureBoxUSU.Name = "pictureBoxUSU";
            this.pictureBoxUSU.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxUSU.TabIndex = 15;
            this.pictureBoxUSU.TabStop = false;
            // 
            // buttonClearUSU
            // 
            this.buttonClearUSU.Location = new Point(420, 84);
            this.buttonClearUSU.Name = "buttonClearUSU";
            this.buttonClearUSU.Size = new Size(48, 19);
            this.buttonClearUSU.TabIndex = 17;
            this.buttonClearUSU.Text = "Clear";
            // 
            // LocomotiveCatalogStandardImages
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new Size(493, 278);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonOK,
                                                                          this.groupBox1,
                                                                          this.groupBox2,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LocomotiveCatalogStandardImages";
            this.ShowInTaskbar = false;
            this.Text = "Standard Locomotive Images";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Button buttonOK;
        private Button buttonCancel;
        private PictureBox pictureBoxES;
        private PictureBox pictureBoxED;
        private PictureBox pictureBoxEE;
        private Button buttonSetES;
        private Button buttonClearES;
        private Button buttonClearED;
        private Button buttonSetED;
        private Button buttonClearEE;
        private Button buttonSetEE;
        private Button buttonSetUS;
        private PictureBox pictureBoxUS;
        private PictureBox pictureBoxUD;
        private PictureBox pictureBoxUE;
        private Button buttonClearUS;
        private Button buttonClearUD;
        private Button buttonSetUD;
        private Button buttonClearUE;
        private Button buttonSetUE;
        private Label label7;
        private Button buttonSetESU;
        private PictureBox pictureBoxESU;
        private Button buttonClearESU;
        private Button buttonSetUSU;
        private Label label8;
        private PictureBox pictureBoxUSU;
        private Button buttonClearUSU;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

