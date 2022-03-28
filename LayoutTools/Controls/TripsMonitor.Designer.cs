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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripsMonitor));
            this.listViewTrips = new System.Windows.Forms.ListView();
            this.columnHeaderTrain = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDriver = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderStatus = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderFinalDestination = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderState = new System.Windows.Forms.ColumnHeader();
            this.imageListTripState = new System.Windows.Forms.ImageList(this.components);
            this.buttonAbort = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonView = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonTalk = new System.Windows.Forms.Button();
            this.buttonOptions = new System.Windows.Forms.Button();
            this.buttonSpeed = new System.Windows.Forms.Button();
            this.buttonSuspend = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanelLeft = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanelLeft.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewTrips
            // 
            this.listViewTrips.AllowColumnReorder = true;
            this.listViewTrips.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTrips.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderTrain,
            this.columnHeaderDriver,
            this.columnHeaderStatus,
            this.columnHeaderFinalDestination,
            this.columnHeaderState});
            this.listViewTrips.FullRowSelect = true;
            this.listViewTrips.Location = new System.Drawing.Point(8, 7);
            this.listViewTrips.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.listViewTrips.MultiSelect = false;
            this.listViewTrips.Name = "listViewTrips";
            this.listViewTrips.Size = new System.Drawing.Size(1731, 300);
            this.listViewTrips.SmallImageList = this.imageListTripState;
            this.listViewTrips.TabIndex = 0;
            this.listViewTrips.UseCompatibleStateImageBehavior = false;
            this.listViewTrips.View = System.Windows.Forms.View.Details;
            this.listViewTrips.SelectedIndexChanged += new System.EventHandler(this.UpdateButtons);
            this.listViewTrips.Click += new System.EventHandler(this.ListViewTrips_Click);
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
            this.imageListTripState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTripState.ImageStream")));
            this.imageListTripState.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListTripState.Images.SetKeyName(0, "");
            this.imageListTripState.Images.SetKeyName(1, "");
            this.imageListTripState.Images.SetKeyName(2, "");
            this.imageListTripState.Images.SetKeyName(3, "");
            this.imageListTripState.Images.SetKeyName(4, "");
            this.imageListTripState.Images.SetKeyName(5, "");
            this.imageListTripState.Images.SetKeyName(6, "");
            this.imageListTripState.Images.SetKeyName(7, "");
            this.imageListTripState.Images.SetKeyName(8, "");
            // 
            // buttonAbort
            // 
            this.buttonAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonAbort.Location = new System.Drawing.Point(8, 7);
            this.buttonAbort.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(195, 47);
            this.buttonAbort.TabIndex = 1;
            this.buttonAbort.Text = "&Abort";
            this.buttonAbort.Click += new System.EventHandler(this.ButtonAbort_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonClose.Location = new System.Drawing.Point(1485, 7);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(195, 47);
            this.buttonClose.TabIndex = 8;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // buttonView
            // 
            this.buttonView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonView.Location = new System.Drawing.Point(430, 7);
            this.buttonView.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(195, 47);
            this.buttonView.TabIndex = 3;
            this.buttonView.Text = "&View...";
            this.buttonView.Click += new System.EventHandler(this.ButtonView_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.Location = new System.Drawing.Point(641, 7);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(195, 47);
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "&Save...";
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // buttonTalk
            // 
            this.buttonTalk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTalk.Location = new System.Drawing.Point(852, 7);
            this.buttonTalk.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonTalk.Name = "buttonTalk";
            this.buttonTalk.Size = new System.Drawing.Size(195, 47);
            this.buttonTalk.TabIndex = 5;
            this.buttonTalk.Text = "&Talk";
            // 
            // buttonOptions
            // 
            this.buttonOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOptions.Location = new System.Drawing.Point(1274, 7);
            this.buttonOptions.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(195, 47);
            this.buttonOptions.TabIndex = 7;
            this.buttonOptions.Text = "&Options...";
            this.buttonOptions.Click += new System.EventHandler(this.ButtonOptions_Click);
            // 
            // buttonSpeed
            // 
            this.buttonSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSpeed.Location = new System.Drawing.Point(1063, 7);
            this.buttonSpeed.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonSpeed.Name = "buttonSpeed";
            this.buttonSpeed.Size = new System.Drawing.Size(195, 47);
            this.buttonSpeed.TabIndex = 6;
            this.buttonSpeed.Text = "S&peed...";
            this.buttonSpeed.Click += new System.EventHandler(this.ButtonSpeed_Click);
            // 
            // buttonSuspend
            // 
            this.buttonSuspend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSuspend.Location = new System.Drawing.Point(219, 7);
            this.buttonSuspend.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonSuspend.Name = "buttonSuspend";
            this.buttonSuspend.Size = new System.Drawing.Size(195, 47);
            this.buttonSuspend.TabIndex = 2;
            this.buttonSuspend.Text = "&Suspend";
            this.buttonSuspend.Click += new System.EventHandler(this.ButtonSuspend_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.listViewTrips, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1747, 394);
            this.tableLayoutPanel1.TabIndex = 9;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.TableLayoutPanel1_Paint);
            // 
            // flowLayoutPanelLeft
            // 
            this.flowLayoutPanelLeft.Controls.Add(this.buttonAbort);
            this.flowLayoutPanelLeft.Controls.Add(this.buttonSuspend);
            this.flowLayoutPanelLeft.Controls.Add(this.buttonView);
            this.flowLayoutPanelLeft.Controls.Add(this.buttonSave);
            this.flowLayoutPanelLeft.Controls.Add(this.buttonTalk);
            this.flowLayoutPanelLeft.Controls.Add(this.buttonSpeed);
            this.flowLayoutPanelLeft.Controls.Add(this.buttonOptions);
            this.flowLayoutPanelLeft.Controls.Add(this.buttonClose);
            this.flowLayoutPanelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelLeft.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelLeft.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanelLeft.Name = "flowLayoutPanelLeft";
            this.flowLayoutPanelLeft.Size = new System.Drawing.Size(1741, 74);
            this.flowLayoutPanelLeft.TabIndex = 10;
            this.flowLayoutPanelLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.FlowLayoutPanelLeft_Paint);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanelLeft);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 317);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1741, 74);
            this.panel1.TabIndex = 10;
            // 
            // TripsMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "TripsMonitor";
            this.Size = new System.Drawing.Size(1747, 394);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanelLeft.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
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
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private FlowLayoutPanel flowLayoutPanelLeft;
    }
}
