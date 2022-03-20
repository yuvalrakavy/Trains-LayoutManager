using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;
using System;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackLinkForm.
    /// </summary>
    partial class TrackLinkProperties : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonNotLinked = new RadioButton();
            this.radioButtonLinked = new RadioButton();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.trackLinkTree = new LayoutManager.CommonUI.Controls.TrackLinkTree();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.SuspendLayout();
            // 
            // radioButtonNotLinked
            // 
            this.radioButtonNotLinked.Location = new System.Drawing.Point(32, 64);
            this.radioButtonNotLinked.Name = "radioButtonNotLinked";
            this.radioButtonNotLinked.TabIndex = 4;
            this.radioButtonNotLinked.Text = "Not linked";
            this.radioButtonNotLinked.CheckedChanged += new EventHandler(this.RadioButtonNotLinked_CheckedChanged);
            // 
            // radioButtonLinked
            // 
            this.radioButtonLinked.Location = new System.Drawing.Point(32, 88);
            this.radioButtonLinked.Name = "radioButtonLinked";
            this.radioButtonLinked.TabIndex = 5;
            this.radioButtonLinked.Text = "Linked to:";
            this.radioButtonLinked.CheckedChanged += new EventHandler(this.RadioButtonLinked_CheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 248);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(128, 248);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // trackLinkTree
            // 
            this.trackLinkTree.Location = new System.Drawing.Point(48, 112);
            this.trackLinkTree.Name = "trackLinkTree";
            this.trackLinkTree.SelectedTrackLink = null;
            this.trackLinkTree.Size = new System.Drawing.Size(232, 128);
            this.trackLinkTree.TabIndex = 6;
            // 
            // nameDefinition
            // 
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(9, 4);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(280, 64);
            this.nameDefinition.TabIndex = 9;
            this.nameDefinition.XmlInfo = null;
            // 
            // TrackLinkProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(308, 310);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.nameDefinition,
                                                                          this.buttonCancel,
                                                                          this.buttonOK,
                                                                          this.radioButtonLinked,
                                                                          this.radioButtonNotLinked,
                                                                          this.trackLinkTree});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrackLinkProperties";
            this.ShowInTaskbar = false;
            this.Text = "Track Link Properties";
            this.ResumeLayout(false);
        }
        #endregion

        private LayoutManager.CommonUI.Controls.TrackLinkTree trackLinkTree;
        private RadioButton radioButtonNotLinked;
        private RadioButton radioButtonLinked;
        private Button buttonOK;
        private Button buttonCancel;
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

