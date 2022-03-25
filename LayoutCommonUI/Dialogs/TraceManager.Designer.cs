using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using LayoutManager;
using LayoutManager.CommonUI;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for TraceManager.
    /// </summary>
    partial class TraceManager : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listViewSwitches = new System.Windows.Forms.ListView();
            this.columnHeaderSwtchName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDescription = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderSwitchSetting = new System.Windows.Forms.ColumnHeader();
            this.groupBoxSwitchValue = new System.Windows.Forms.GroupBox();
            this.radioButtonBooleanOff = new System.Windows.Forms.RadioButton();
            this.radioButtonTraceNone = new System.Windows.Forms.RadioButton();
            this.radioButtonTraceError = new System.Windows.Forms.RadioButton();
            this.radioButtonTraceInfo = new System.Windows.Forms.RadioButton();
            this.radioButtonTraceVerbose = new System.Windows.Forms.RadioButton();
            this.radioButtonWarnings = new System.Windows.Forms.RadioButton();
            this.radioButtonBooleanOn = new System.Windows.Forms.RadioButton();
            this.buttonClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonAllSwitches = new System.Windows.Forms.RadioButton();
            this.radioButtonApplicationSwitches = new System.Windows.Forms.RadioButton();
            this.buttonResetAll = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxSwitchValue.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewSwitches
            // 
            this.listViewSwitches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSwitches.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderSwtchName,
            this.columnHeaderDescription,
            this.columnHeaderSwitchSetting});
            this.listViewSwitches.FullRowSelect = true;
            this.listViewSwitches.Location = new System.Drawing.Point(21, 20);
            this.listViewSwitches.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.listViewSwitches.MultiSelect = false;
            this.listViewSwitches.Name = "listViewSwitches";
            this.listViewSwitches.Size = new System.Drawing.Size(1377, 388);
            this.listViewSwitches.TabIndex = 0;
            this.listViewSwitches.UseCompatibleStateImageBehavior = false;
            this.listViewSwitches.View = System.Windows.Forms.View.Details;
            this.listViewSwitches.SelectedIndexChanged += new System.EventHandler(this.ListViewSwitches_SelectedIndexChanged);
            // 
            // columnHeaderSwtchName
            // 
            this.columnHeaderSwtchName.Text = "Name";
            this.columnHeaderSwtchName.Width = 173;
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "Description";
            this.columnHeaderDescription.Width = 238;
            // 
            // columnHeaderSwitchSetting
            // 
            this.columnHeaderSwitchSetting.Text = "Setting";
            this.columnHeaderSwitchSetting.Width = 111;
            // 
            // groupBoxSwitchValue
            // 
            this.groupBoxSwitchValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonBooleanOff);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceNone);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceError);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceInfo);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceVerbose);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonWarnings);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonBooleanOn);
            this.groupBoxSwitchValue.Location = new System.Drawing.Point(21, 433);
            this.groupBoxSwitchValue.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxSwitchValue.Name = "groupBoxSwitchValue";
            this.groupBoxSwitchValue.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBoxSwitchValue.Size = new System.Drawing.Size(416, 295);
            this.groupBoxSwitchValue.TabIndex = 1;
            this.groupBoxSwitchValue.TabStop = false;
            this.groupBoxSwitchValue.Text = "Set selected switch to:";
            // 
            // radioButtonBooleanOff
            // 
            this.radioButtonBooleanOff.Location = new System.Drawing.Point(229, 39);
            this.radioButtonBooleanOff.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonBooleanOff.Name = "radioButtonBooleanOff";
            this.radioButtonBooleanOff.Size = new System.Drawing.Size(270, 39);
            this.radioButtonBooleanOff.TabIndex = 4;
            this.radioButtonBooleanOff.Text = "Off";
            this.radioButtonBooleanOff.CheckedChanged += new System.EventHandler(this.RadioButtonBooleanOff_CheckedChanged);
            // 
            // radioButtonTraceNone
            // 
            this.radioButtonTraceNone.Location = new System.Drawing.Point(21, 44);
            this.radioButtonTraceNone.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonTraceNone.Name = "radioButtonTraceNone";
            this.radioButtonTraceNone.Size = new System.Drawing.Size(270, 39);
            this.radioButtonTraceNone.TabIndex = 0;
            this.radioButtonTraceNone.Text = "None";
            this.radioButtonTraceNone.CheckedChanged += new System.EventHandler(this.RadioButtonTraceNone_CheckedChanged);
            // 
            // radioButtonTraceError
            // 
            this.radioButtonTraceError.Location = new System.Drawing.Point(21, 91);
            this.radioButtonTraceError.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonTraceError.Name = "radioButtonTraceError";
            this.radioButtonTraceError.Size = new System.Drawing.Size(192, 39);
            this.radioButtonTraceError.TabIndex = 1;
            this.radioButtonTraceError.Text = "Errors";
            this.radioButtonTraceError.CheckedChanged += new System.EventHandler(this.RadioButtonTraceError_CheckedChanged);
            // 
            // radioButtonTraceInfo
            // 
            this.radioButtonTraceInfo.Location = new System.Drawing.Point(21, 185);
            this.radioButtonTraceInfo.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonTraceInfo.Name = "radioButtonTraceInfo";
            this.radioButtonTraceInfo.Size = new System.Drawing.Size(270, 39);
            this.radioButtonTraceInfo.TabIndex = 2;
            this.radioButtonTraceInfo.Text = "Info";
            this.radioButtonTraceInfo.CheckedChanged += new System.EventHandler(this.RadioButtonTraceInfo_CheckedChanged);
            // 
            // radioButtonTraceVerbose
            // 
            this.radioButtonTraceVerbose.Location = new System.Drawing.Point(21, 231);
            this.radioButtonTraceVerbose.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonTraceVerbose.Name = "radioButtonTraceVerbose";
            this.radioButtonTraceVerbose.Size = new System.Drawing.Size(270, 39);
            this.radioButtonTraceVerbose.TabIndex = 3;
            this.radioButtonTraceVerbose.Text = "Verbose";
            this.radioButtonTraceVerbose.CheckedChanged += new System.EventHandler(this.RadioButtonTraceVerbose_CheckedChanged);
            // 
            // radioButtonWarnings
            // 
            this.radioButtonWarnings.Location = new System.Drawing.Point(21, 138);
            this.radioButtonWarnings.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonWarnings.Name = "radioButtonWarnings";
            this.radioButtonWarnings.Size = new System.Drawing.Size(270, 39);
            this.radioButtonWarnings.TabIndex = 1;
            this.radioButtonWarnings.Text = "Warnings";
            this.radioButtonWarnings.CheckedChanged += new System.EventHandler(this.RadioButtonWarnings_CheckedChanged);
            // 
            // radioButtonBooleanOn
            // 
            this.radioButtonBooleanOn.Location = new System.Drawing.Point(229, 98);
            this.radioButtonBooleanOn.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonBooleanOn.Name = "radioButtonBooleanOn";
            this.radioButtonBooleanOn.Size = new System.Drawing.Size(270, 39);
            this.radioButtonBooleanOn.TabIndex = 4;
            this.radioButtonBooleanOn.Text = "On";
            this.radioButtonBooleanOn.CheckedChanged += new System.EventHandler(this.RadioButtonBooleanOn_CheckedChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(1217, 670);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(195, 57);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.radioButtonAllSwitches);
            this.groupBox1.Controls.Add(this.radioButtonApplicationSwitches);
            this.groupBox1.Location = new System.Drawing.Point(476, 433);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Size = new System.Drawing.Size(520, 138);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show:";
            // 
            // radioButtonAllSwitches
            // 
            this.radioButtonAllSwitches.AutoSize = true;
            this.radioButtonAllSwitches.Location = new System.Drawing.Point(16, 89);
            this.radioButtonAllSwitches.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonAllSwitches.Name = "radioButtonAllSwitches";
            this.radioButtonAllSwitches.Size = new System.Drawing.Size(168, 36);
            this.radioButtonAllSwitches.TabIndex = 1;
            this.radioButtonAllSwitches.Text = "All switches";
            this.radioButtonAllSwitches.UseVisualStyleBackColor = true;
            this.radioButtonAllSwitches.CheckedChanged += new System.EventHandler(this.SwitchTypeChanged);
            // 
            // radioButtonApplicationSwitches
            // 
            this.radioButtonApplicationSwitches.AutoSize = true;
            this.radioButtonApplicationSwitches.Checked = true;
            this.radioButtonApplicationSwitches.Location = new System.Drawing.Point(16, 37);
            this.radioButtonApplicationSwitches.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.radioButtonApplicationSwitches.Name = "radioButtonApplicationSwitches";
            this.radioButtonApplicationSwitches.Size = new System.Drawing.Size(360, 36);
            this.radioButtonApplicationSwitches.TabIndex = 0;
            this.radioButtonApplicationSwitches.TabStop = true;
            this.radioButtonApplicationSwitches.Text = "Only this application switches";
            this.radioButtonApplicationSwitches.UseVisualStyleBackColor = true;
            this.radioButtonApplicationSwitches.CheckedChanged += new System.EventHandler(this.SwitchTypeChanged);
            // 
            // buttonResetAll
            // 
            this.buttonResetAll.Location = new System.Drawing.Point(476, 664);
            this.buttonResetAll.Name = "buttonResetAll";
            this.buttonResetAll.Size = new System.Drawing.Size(195, 57);
            this.buttonResetAll.TabIndex = 5;
            this.buttonResetAll.Text = "Clear All";
            this.buttonResetAll.UseVisualStyleBackColor = true;
            this.buttonResetAll.Click += new System.EventHandler(this.ButtonResetAll_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(1217, 433);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(195, 57);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Save...";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(1217, 506);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(195, 57);
            this.buttonLoad.TabIndex = 7;
            this.buttonLoad.Text = "Load...";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.ButtonLoad_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "traceFlags";
            this.openFileDialog.Title = "Load trace configuration from";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "traceFlags";
            this.saveFileDialog.Title = "Save trace configuration to";
            // 
            // TraceManager
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(1435, 743);
            this.ControlBox = false;
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonResetAll);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBoxSwitchValue);
            this.Controls.Add(this.listViewSwitches);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "TraceManager";
            this.ShowInTaskbar = false;
            this.Text = "Trace Manager";
            this.groupBoxSwitchValue.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private RadioButton radioButtonTraceNone;
        private RadioButton radioButtonTraceError;
        private RadioButton radioButtonTraceInfo;
        private ListView listViewSwitches;
        private RadioButton radioButtonTraceVerbose;
        private ColumnHeader columnHeaderSwtchName;
        private ColumnHeader columnHeaderSwitchSetting;
        private Button buttonClose;
        private RadioButton radioButtonWarnings;
        private RadioButton radioButtonBooleanOff;
        private RadioButton radioButtonBooleanOn;
        private GroupBox groupBoxSwitchValue;
        private ColumnHeader columnHeaderDescription;
        private GroupBox groupBox1;
        private RadioButton radioButtonAllSwitches;
        private RadioButton radioButtonApplicationSwitches;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private Button buttonResetAll;
        private Button buttonSave;
        private Button buttonLoad;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
    }
}

