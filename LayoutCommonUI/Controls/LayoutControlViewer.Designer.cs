namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LayoutControlViewer.
    /// </summary>
    partial class LayoutControlViewer : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutControlViewer));
            this.imageListCloseButton = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClose = new System.Windows.Forms.Button();
            this.layoutControlBusViewer = new LayoutManager.CommonUI.LayoutControlBusViewer();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxBusProvider = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxBus = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxLocation = new System.Windows.Forms.ComboBox();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListCloseButton
            // 
            this.imageListCloseButton.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListCloseButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCloseButton.ImageStream")));
            this.imageListCloseButton.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListCloseButton.Images.SetKeyName(0, "");
            this.imageListCloseButton.Images.SetKeyName(1, "");
            this.imageListCloseButton.Images.SetKeyName(2, "");
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(520, 49);
            this.panel1.TabIndex = 9;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.ImageIndex = 0;
            this.buttonClose.ImageList = this.imageListCloseButton;
            this.buttonClose.Location = new System.Drawing.Point(473, 2);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(42, 39);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // layoutControlBusViewer
            // 
            this.layoutControlBusViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.layoutControlBusViewer.BusProviderId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.layoutControlBusViewer.BusTypeName = null;
            this.layoutControlBusViewer.Location = new System.Drawing.Point(0, 414);
            this.layoutControlBusViewer.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.layoutControlBusViewer.ModuleLocationID = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.layoutControlBusViewer.Name = "layoutControlBusViewer";
            this.layoutControlBusViewer.ShowOnlyNotInLocation = false;
            this.layoutControlBusViewer.Size = new System.Drawing.Size(520, 1567);
            this.layoutControlBusViewer.StartingPoint = ((System.Drawing.PointF)(resources.GetObject("layoutControlBusViewer.StartingPoint")));
            this.layoutControlBusViewer.TabIndex = 8;
            this.layoutControlBusViewer.Zoom = 1F;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Controller:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxBusProvider
            // 
            this.comboBoxBusProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBusProvider.Location = new System.Drawing.Point(21, 89);
            this.comboBoxBusProvider.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBoxBusProvider.Name = "comboBoxBusProvider";
            this.comboBoxBusProvider.Size = new System.Drawing.Size(368, 40);
            this.comboBoxBusProvider.TabIndex = 1;
            this.comboBoxBusProvider.SelectedIndexChanged += new System.EventHandler(this.ComboBoxBusProvider_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(21, 145);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(260, 39);
            this.label2.TabIndex = 2;
            this.label2.Text = "Connection:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxBus
            // 
            this.comboBoxBus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBus.Location = new System.Drawing.Point(21, 182);
            this.comboBoxBus.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBoxBus.Name = "comboBoxBus";
            this.comboBoxBus.Size = new System.Drawing.Size(368, 40);
            this.comboBoxBus.TabIndex = 3;
            this.comboBoxBus.SelectedIndexChanged += new System.EventHandler(this.ComboBoxBus_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(21, 239);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(354, 39);
            this.label3.TabIndex = 4;
            this.label3.Text = "Control Modules Location:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxLocation
            // 
            this.comboBoxLocation.Location = new System.Drawing.Point(21, 276);
            this.comboBoxLocation.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.comboBoxLocation.Name = "comboBoxLocation";
            this.comboBoxLocation.Size = new System.Drawing.Size(368, 40);
            this.comboBoxLocation.TabIndex = 5;
            this.comboBoxLocation.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLocation_SelectedIndexChanged);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonZoomIn.ImageIndex = 2;
            this.buttonZoomIn.ImageList = this.imageListCloseButton;
            this.buttonZoomIn.Location = new System.Drawing.Point(70, 347);
            this.buttonZoomIn.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(42, 39);
            this.buttonZoomIn.TabIndex = 7;
            this.buttonZoomIn.UseVisualStyleBackColor = false;
            this.buttonZoomIn.Click += new System.EventHandler(this.ButtonZoomIn_Click);
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonZoomOut.ImageIndex = 1;
            this.buttonZoomOut.ImageList = this.imageListCloseButton;
            this.buttonZoomOut.Location = new System.Drawing.Point(21, 347);
            this.buttonZoomOut.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(42, 39);
            this.buttonZoomOut.TabIndex = 6;
            this.buttonZoomOut.UseVisualStyleBackColor = false;
            this.buttonZoomOut.Click += new System.EventHandler(this.ButtonZoomOut_Click);
            // 
            // LayoutControlViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.buttonZoomOut);
            this.Controls.Add(this.buttonZoomIn);
            this.Controls.Add(this.comboBoxLocation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxBus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxBusProvider);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.layoutControlBusViewer);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "LayoutControlViewer";
            this.Size = new System.Drawing.Size(520, 1974);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private ImageList imageListCloseButton;
        private Panel panel1;
        private Button buttonClose;
        private LayoutManager.CommonUI.LayoutControlBusViewer layoutControlBusViewer;
        private Label label1;
        private ComboBox comboBoxBusProvider;
        private Label label2;
        private ComboBox comboBoxBus;
        private Label label3;
        private ComboBox comboBoxLocation;
        private Button buttonZoomIn;
        private Button buttonZoomOut;
        private System.ComponentModel.IContainer components;


    }
}