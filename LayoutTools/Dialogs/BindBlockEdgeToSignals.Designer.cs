using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Collections.Generic;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BindBlockEdgeToSignals.
    /// </summary>
    partial class BindBlockEdgeToSignals : Form, IModelComponentReceiverDialog {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.listBoxSignals = new ListBox();
            this.buttonRemove = new Button();
            this.buttonCancel = new Button();
            this.buttonOk = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 56);
            this.label1.TabIndex = 0;
            this.label1.Text = "Right click, and select \"Link signal\" on the signal components that should reflec" +
                "t the signalling state of this track contact:";
            // 
            // listBoxSignals
            // 
            this.listBoxSignals.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listBoxSignals.Location = new System.Drawing.Point(12, 72);
            this.listBoxSignals.Name = "listBoxSignals";
            this.listBoxSignals.Size = new System.Drawing.Size(170, 108);
            this.listBoxSignals.TabIndex = 1;
            this.listBoxSignals.SelectedIndexChanged += this.updateButtons;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRemove.Location = new System.Drawing.Point(12, 186);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(56, 21);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.buttonRemove_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.Location = new System.Drawing.Point(128, 216);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.buttonCancel_Click;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonOk.Location = new System.Drawing.Point(64, 216);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(56, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += this.buttonOk_Click;
            // 
            // BindBlockEdgeToSignals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(192, 246);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonRemove,
                                                                          this.listBoxSignals,
                                                                          this.label1,
                                                                          this.buttonCancel,
                                                                          this.buttonOk});
            this.Name = "BindBlockEdgeToSignals";
            this.ShowInTaskbar = false;
            this.Text = "Link Signals to Track Contact";
            this.Closing += this.BindBlockEdgeToSignals_Closing;
            this.Closed += this.BindBlockEdgeToSignals_Closed;
            this.ResumeLayout(false);
        }
        #endregion
        private Label label1;
        private ListBox listBoxSignals;
        private Button buttonRemove;
        private Button buttonCancel;
        private Button buttonOk;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
