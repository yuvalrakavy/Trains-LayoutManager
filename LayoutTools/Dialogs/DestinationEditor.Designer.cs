using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for DestinationEditor.
    /// </summary>
    partial class DestinationEditor : Form, IModelComponentReceiverDialog, IControlSupportViewOnly {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new(typeof(DestinationEditor));
            this.buttonCancel = new Button();
            this.label1 = new Label();
            this.textBoxName = new LayoutManager.CommonUI.TextBoxWithViewOnly();
            this.groupBoxLocations = new GroupBox();
            this.buttonCondition = new Button();
            this.listViewLocations = new ListView();
            this.columnHeaderLocation = new ColumnHeader();
            this.columnHeaderCondition = new ColumnHeader();
            this.buttonAddLocation = new Button();
            this.buttonRemoveLocation = new Button();
            this.buttonMoveLocationUp = new Button();
            this.imageListButttons = new ImageList(this.components);
            this.buttonMoveLocationDown = new Button();
            this.label2 = new Label();
            this.groupBoxSelection = new GroupBox();
            this.radioButtonSelectionListOrder = new LayoutManager.CommonUI.RadioButtonWithViewOnly();
            this.radioButtonSelectionRandom = new LayoutManager.CommonUI.RadioButtonWithViewOnly();
            this.buttonOk = new Button();
            this.buttonSave = new Button();
            this.groupBoxLocations.SuspendLayout();
            this.groupBoxSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(232, 276);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 20);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.ButtonCancel_Click;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(42, 6);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(188, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.ViewOnly = false;
            // 
            // groupBoxLocations
            // 
            this.groupBoxLocations.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.groupBoxLocations.Controls.Add(this.buttonCondition);
            this.groupBoxLocations.Controls.Add(this.listViewLocations);
            this.groupBoxLocations.Controls.Add(this.buttonAddLocation);
            this.groupBoxLocations.Controls.Add(this.buttonRemoveLocation);
            this.groupBoxLocations.Controls.Add(this.buttonMoveLocationUp);
            this.groupBoxLocations.Controls.Add(this.buttonMoveLocationDown);
            this.groupBoxLocations.Location = new System.Drawing.Point(8, 112);
            this.groupBoxLocations.Name = "groupBoxLocations";
            this.groupBoxLocations.Size = new System.Drawing.Size(280, 160);
            this.groupBoxLocations.TabIndex = 4;
            this.groupBoxLocations.TabStop = false;
            this.groupBoxLocations.Text = "Locations:";
            // 
            // buttonCondition
            // 
            this.buttonCondition.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonCondition.Location = new System.Drawing.Point(168, 133);
            this.buttonCondition.Name = "buttonCondition";
            this.buttonCondition.Size = new System.Drawing.Size(73, 20);
            this.buttonCondition.TabIndex = 3;
            this.buttonCondition.Text = "Condition...";
            this.buttonCondition.Click += this.ButtonCondition_Click;
            // 
            // listViewLocations
            // 
            this.listViewLocations.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.listViewLocations.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderLocation,
            this.columnHeaderCondition});
            this.listViewLocations.FullRowSelect = true;
            this.listViewLocations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewLocations.HideSelection = false;
            this.listViewLocations.Location = new System.Drawing.Point(8, 16);
            this.listViewLocations.MultiSelect = false;
            this.listViewLocations.Name = "listViewLocations";
            this.listViewLocations.Size = new System.Drawing.Size(240, 112);
            this.listViewLocations.TabIndex = 0;
            this.listViewLocations.UseCompatibleStateImageBehavior = false;
            this.listViewLocations.View = System.Windows.Forms.View.Details;
            this.listViewLocations.SelectedIndexChanged += this.ListViewLocations_SelectedIndexChanged;
            // 
            // columnHeaderLocation
            // 
            this.columnHeaderLocation.Text = "Location";
            this.columnHeaderLocation.Width = 78;
            // 
            // columnHeaderCondition
            // 
            this.columnHeaderCondition.Text = "Condition";
            this.columnHeaderCondition.Width = 154;
            // 
            // buttonAddLocation
            // 
            this.buttonAddLocation.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonAddLocation.Location = new System.Drawing.Point(8, 133);
            this.buttonAddLocation.Name = "buttonAddLocation";
            this.buttonAddLocation.Size = new System.Drawing.Size(73, 20);
            this.buttonAddLocation.TabIndex = 1;
            this.buttonAddLocation.Text = "&Add";
            this.buttonAddLocation.Click += this.ButtonAddLocation_Click;
            // 
            // buttonRemoveLocation
            // 
            this.buttonRemoveLocation.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRemoveLocation.Location = new System.Drawing.Point(88, 133);
            this.buttonRemoveLocation.Name = "buttonRemoveLocation";
            this.buttonRemoveLocation.Size = new System.Drawing.Size(73, 20);
            this.buttonRemoveLocation.TabIndex = 2;
            this.buttonRemoveLocation.Text = "&Remove";
            this.buttonRemoveLocation.Click += this.ButtonRemoveLocation_Click;
            // 
            // buttonMoveLocationUp
            // 
            this.buttonMoveLocationUp.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonMoveLocationUp.ImageIndex = 1;
            this.buttonMoveLocationUp.ImageList = this.imageListButttons;
            this.buttonMoveLocationUp.Location = new System.Drawing.Point(251, 17);
            this.buttonMoveLocationUp.Name = "buttonMoveLocationUp";
            this.buttonMoveLocationUp.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveLocationUp.TabIndex = 4;
            this.buttonMoveLocationUp.Click += this.ButtonMoveLocationUp_Click;
            // 
            // imageListButttons
            // 
            this.imageListButttons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListButttons.ImageStream");
            this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListButttons.Images.SetKeyName(0, "");
            this.imageListButttons.Images.SetKeyName(1, "");
            // 
            // buttonMoveLocationDown
            // 
            this.buttonMoveLocationDown.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonMoveLocationDown.ImageIndex = 0;
            this.buttonMoveLocationDown.ImageList = this.imageListButttons;
            this.buttonMoveLocationDown.Location = new System.Drawing.Point(251, 40);
            this.buttonMoveLocationDown.Name = "buttonMoveLocationDown";
            this.buttonMoveLocationDown.Size = new System.Drawing.Size(24, 20);
            this.buttonMoveLocationDown.TabIndex = 5;
            this.buttonMoveLocationDown.Click += this.ButtonMoveLocationDown_Click;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "(Leave empty to use default name)";
            // 
            // groupBoxSelection
            // 
            this.groupBoxSelection.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.groupBoxSelection.Controls.Add(this.radioButtonSelectionListOrder);
            this.groupBoxSelection.Controls.Add(this.radioButtonSelectionRandom);
            this.groupBoxSelection.Location = new System.Drawing.Point(8, 48);
            this.groupBoxSelection.Name = "groupBoxSelection";
            this.groupBoxSelection.Size = new System.Drawing.Size(280, 56);
            this.groupBoxSelection.TabIndex = 3;
            this.groupBoxSelection.TabStop = false;
            this.groupBoxSelection.Text = "Selection:";
            // 
            // radioButtonSelectionListOrder
            // 
            this.radioButtonSelectionListOrder.Location = new System.Drawing.Point(8, 16);
            this.radioButtonSelectionListOrder.Name = "radioButtonSelectionListOrder";
            this.radioButtonSelectionListOrder.Size = new System.Drawing.Size(168, 16);
            this.radioButtonSelectionListOrder.TabIndex = 0;
            this.radioButtonSelectionListOrder.Text = "Priority based on order in list";
            this.radioButtonSelectionListOrder.ViewOnly = false;
            // 
            // radioButtonSelectionRandom
            // 
            this.radioButtonSelectionRandom.AutoSize = true;
            this.radioButtonSelectionRandom.Location = new System.Drawing.Point(8, 34);
            this.radioButtonSelectionRandom.Name = "radioButtonSelectionRandom";
            this.radioButtonSelectionRandom.Size = new System.Drawing.Size(228, 17);
            this.radioButtonSelectionRandom.TabIndex = 1;
            this.radioButtonSelectionRandom.Text = "Best fit between train and destination lenth ";
            this.radioButtonSelectionRandom.ViewOnly = false;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonOk.Location = new System.Drawing.Point(168, 276);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(56, 20);
            this.buttonOk.TabIndex = 6;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += this.ButtonOk_Click;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonSave.Location = new System.Drawing.Point(8, 276);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(56, 20);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save...";
            this.buttonSave.Click += this.ButtonSave_Click;
            // 
            // DestinationEditor
            // 
            this.AcceptButton = this.buttonOk;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(296, 301);
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxSelection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBoxLocations);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonSave);
            this.Name = "DestinationEditor";
            this.Text = "DestinationEditor";
            this.Closed += this.DestinationEditor_Closed;
            this.Closing += this.DestinationEditor_Closing;
            this.groupBoxLocations.ResumeLayout(false);
            this.groupBoxSelection.ResumeLayout(false);
            this.groupBoxSelection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private Label label1;
        private Label label2;
        private GroupBox groupBoxSelection;
        private ImageList imageListButttons;
        private CommonUI.TextBoxWithViewOnly textBoxName;
        private GroupBox groupBoxLocations;
        private CommonUI.RadioButtonWithViewOnly radioButtonSelectionListOrder;
        private CommonUI.RadioButtonWithViewOnly radioButtonSelectionRandom;
        private Button buttonMoveLocationDown;
        private Button buttonMoveLocationUp;
        private Button buttonCancel;
        private Button buttonAddLocation;
        private Button buttonRemoveLocation;
        private Button buttonOk;
        private Button buttonSave;
        private ListView listViewLocations;
        private ColumnHeader columnHeaderLocation;
        private ColumnHeader columnHeaderCondition;
        private Button buttonCondition;
        private IContainer components;
    }
}

