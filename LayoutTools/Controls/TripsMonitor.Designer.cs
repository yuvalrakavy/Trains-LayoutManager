using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Tools.Controls {
    /// <summary>
    /// Summary description for TripsMonitor.
    /// </summary>
    partial class TripsMonitor : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripsMonitor));
            this.listViewTrips = new ListView();
            this.columnHeaderTrain = new ColumnHeader();
            this.columnHeaderDriver = new ColumnHeader();
            this.columnHeaderStatus = new ColumnHeader();
            this.columnHeaderFinalDestination = new ColumnHeader();
            this.columnHeaderState = new ColumnHeader();
            this.imageListTripState = new ImageList(this.components);
            this.buttonAbort = new Button();
            this.buttonClose = new Button();
            this.buttonView = new Button();
            this.buttonSave = new Button();
            this.buttonTalk = new Button();
            this.buttonOptions = new Button();
            this.buttonSpeed = new Button();
            this.buttonSuspend = new Button();
            this.SuspendLayout();
            // 
            // listViewTrips
            // 
            this.listViewTrips.AllowColumnReorder = true;
            this.listViewTrips.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listViewTrips.Columns.AddRange(new ColumnHeader[] {
                                                                                            this.columnHeaderTrain,
                                                                                            this.columnHeaderDriver,
                                                                                            this.columnHeaderStatus,
                                                                                            this.columnHeaderFinalDestination,
                                                                                            this.columnHeaderState});
            this.listViewTrips.FullRowSelect = true;
            this.listViewTrips.HideSelection = false;
            this.listViewTrips.Location = new System.Drawing.Point(8, 8);
            this.listViewTrips.MultiSelect = false;
            this.listViewTrips.Name = "listViewTrips";
            this.listViewTrips.Size = new System.Drawing.Size(656, 120);
            this.listViewTrips.SmallImageList = this.imageListTripState;
            this.listViewTrips.TabIndex = 0;
            this.listViewTrips.View = System.Windows.Forms.View.Details;
            this.listViewTrips.Click += new EventHandler(this.ListViewTrips_Click);
            this.listViewTrips.SelectedIndexChanged += new EventHandler(this.UpdateButtons);
            // 
            // columnHeaderTrain
            // 
            this.columnHeaderTrain.Text = "Train";
            this.columnHeaderTrain.Width = 85;
            // 
            // columnHeaderDriver
            // 
            this.columnHeaderDriver.Text = "Driver";
            this.columnHeaderDriver.Width = 107;
            // 
            // columnHeaderStatus
            // 
            this.columnHeaderStatus.Text = "Status";
            this.columnHeaderStatus.Width = 180;
            // 
            // columnHeaderFinalDestination
            // 
            this.columnHeaderFinalDestination.Text = "Destination";
            this.columnHeaderFinalDestination.Width = 160;
            // 
            // columnHeaderState
            // 
            this.columnHeaderState.Text = "State";
            this.columnHeaderState.Width = 120;
            // 
            // imageListTripState
            // 
            this.imageListTripState.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListTripState.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListTripState.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListTripState.ImageStream");
            this.imageListTripState.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonAbort
            // 
            this.buttonAbort.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonAbort.Location = new System.Drawing.Point(8, 136);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 19);
            this.buttonAbort.TabIndex = 1;
            this.buttonAbort.Text = "&Abort";
            this.buttonAbort.Click += new EventHandler(this.ButtonAbort_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.Location = new System.Drawing.Point(589, 136);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 19);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // buttonView
            // 
            this.buttonView.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonView.Location = new System.Drawing.Point(168, 136);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(75, 19);
            this.buttonView.TabIndex = 3;
            this.buttonView.Text = "&View...";
            this.buttonView.Click += new EventHandler(this.ButtonView_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonSave.Location = new System.Drawing.Point(248, 136);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 19);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "&Save...";
            this.buttonSave.Click += new EventHandler(this.ButtonSave_Click);
            // 
            // buttonTalk
            // 
            this.buttonTalk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonTalk.Location = new System.Drawing.Point(328, 136);
            this.buttonTalk.Name = "buttonTalk";
            this.buttonTalk.Size = new System.Drawing.Size(75, 19);
            this.buttonTalk.TabIndex = 5;
            this.buttonTalk.Text = "&Talk";
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonOptions.Location = new System.Drawing.Point(488, 136);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(75, 19);
            this.buttonOptions.TabIndex = 7;
            this.buttonOptions.Text = "&Options...";
            this.buttonOptions.Click += new EventHandler(this.ButtonOptions_Click);
            // 
            // buttonSpeed
            // 
            this.buttonSpeed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonSpeed.Location = new System.Drawing.Point(408, 136);
            this.buttonSpeed.Name = "buttonSpeed";
            this.buttonSpeed.Size = new System.Drawing.Size(75, 19);
            this.buttonSpeed.TabIndex = 6;
            this.buttonSpeed.Text = "S&peed...";
            this.buttonSpeed.Click += new EventHandler(this.ButtonSpeed_Click);
            // 
            // buttonSuspend
            // 
            this.buttonSuspend.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonSuspend.Location = new System.Drawing.Point(88, 136);
            this.buttonSuspend.Name = "buttonSuspend";
            this.buttonSuspend.Size = new System.Drawing.Size(75, 19);
            this.buttonSuspend.TabIndex = 2;
            this.buttonSuspend.Text = "&Suspend";
            this.buttonSuspend.Click += new EventHandler(this.ButtonSuspend_Click);
            // 
            // TripsMonitor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonTalk,
                                                                          this.buttonAbort,
                                                                          this.listViewTrips,
                                                                          this.buttonClose,
                                                                          this.buttonView,
                                                                          this.buttonSave,
                                                                          this.buttonOptions,
                                                                          this.buttonSpeed,
                                                                          this.buttonSuspend});
            this.Name = "TripsMonitor";
            this.Size = new System.Drawing.Size(672, 160);
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            //this.AutoSize = true;
            this.ResumeLayout(false);
        }
        #endregion

        private ListView listViewTrips;
        private Button buttonAbort;
        private ColumnHeader columnHeaderTrain;
        private ColumnHeader columnHeaderStatus;
        private ColumnHeader columnHeaderDriver;
        private ColumnHeader columnHeaderFinalDestination;
        private ColumnHeader columnHeaderState;
        private Button buttonClose;
        private Button buttonView;
        private Button buttonSave;
        private Button buttonTalk;
        private Button buttonOptions;
        private ImageList imageListTripState;
        private Button buttonSpeed;
        private Button buttonSuspend;
        private IContainer components;
    }
}
