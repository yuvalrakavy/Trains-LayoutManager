using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for AddLocomotiveToTrain.
    /// </summary>
    partial class AddLocomotiveToTrain : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBoxLocomotives = new ListBox();
            this.groupBox1 = new GroupBox();
            this.radioButtonOrientationForward = new RadioButton();
            this.radioButtonOrientationBackward = new RadioButton();
            this.buttonAdd = new Button();
            this.buttonCancel = new Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxLocomotives
            // 
            this.listBoxLocomotives.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listBoxLocomotives.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxLocomotives.ItemHeight = 46;
            this.listBoxLocomotives.Location = new System.Drawing.Point(8, 8);
            this.listBoxLocomotives.Name = "listBoxLocomotives";
            this.listBoxLocomotives.Size = new System.Drawing.Size(152, 234);
            this.listBoxLocomotives.TabIndex = 0;
            this.listBoxLocomotives.DrawItem += new DrawItemEventHandler(this.ListBoxLocomotives_DrawItem);
            this.listBoxLocomotives.SelectedIndexChanged += new EventHandler(this.UpdateButtons);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.radioButtonOrientationForward,
                                                                                    this.radioButtonOrientationBackward});
            this.groupBox1.Location = new System.Drawing.Point(166, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(104, 56);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orientation";
            // 
            // radioButtonOrientationForward
            // 
            this.radioButtonOrientationForward.Location = new System.Drawing.Point(8, 15);
            this.radioButtonOrientationForward.Name = "radioButtonOrientationForward";
            this.radioButtonOrientationForward.Size = new System.Drawing.Size(80, 16);
            this.radioButtonOrientationForward.TabIndex = 0;
            this.radioButtonOrientationForward.Text = "Forward";
            this.radioButtonOrientationForward.CheckedChanged += new EventHandler(this.UpdateLocomotiveList);
            // 
            // radioButtonOrientationBackward
            // 
            this.radioButtonOrientationBackward.Location = new System.Drawing.Point(8, 35);
            this.radioButtonOrientationBackward.Name = "radioButtonOrientationBackward";
            this.radioButtonOrientationBackward.Size = new System.Drawing.Size(80, 16);
            this.radioButtonOrientationBackward.TabIndex = 1;
            this.radioButtonOrientationBackward.Text = "Backward";
            this.radioButtonOrientationBackward.CheckedChanged += new EventHandler(this.UpdateLocomotiveList);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonAdd.Location = new System.Drawing.Point(195, 192);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new EventHandler(this.ButtonAdd_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(195, 224);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "&Cancel";
            // 
            // AddLocomotiveToTrain
            // 
            this.AcceptButton = this.buttonAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(276, 266);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonAdd,
                                                                          this.groupBox1,
                                                                          this.listBoxLocomotives,
                                                                          this.buttonCancel});
            this.Name = "AddLocomotiveToTrain";
            this.ShowInTaskbar = false;
            this.Text = "Add Locomotive";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private ListBox listBoxLocomotives;
        private GroupBox groupBox1;
        private RadioButton radioButtonOrientationForward;
        private RadioButton radioButtonOrientationBackward;
        private Button buttonAdd;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

