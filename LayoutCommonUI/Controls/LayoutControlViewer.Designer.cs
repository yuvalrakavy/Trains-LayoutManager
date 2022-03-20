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
            this.imageListCloseButton = new ImageList(this.components);
            this.panel1 = new Panel();
            this.buttonClose = new Button();
            this.layoutControlBusViewer = new LayoutManager.CommonUI.LayoutControlBusViewer();
            this.label1 = new Label();
            this.comboBoxBusProvider = new ComboBox();
            this.label2 = new Label();
            this.comboBoxBus = new ComboBox();
            this.label3 = new Label();
            this.comboBoxLocation = new ComboBox();
            this.buttonZoomIn = new Button();
            this.buttonZoomOut = new Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListCloseButton
            // 
            this.imageListCloseButton.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListCloseButton.ImageStream");
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
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 20);
            this.panel1.TabIndex = 9;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.ImageIndex = 0;
            this.buttonClose.ImageList = this.imageListCloseButton;
            this.buttonClose.Location = new System.Drawing.Point(182, 1);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(16, 16);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // layoutControlBusViewer
            // 
            this.layoutControlBusViewer.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right);
            this.layoutControlBusViewer.BusProviderId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.layoutControlBusViewer.BusTypeName = null;
            this.layoutControlBusViewer.Location = new System.Drawing.Point(0, 168);
            this.layoutControlBusViewer.ModuleLocationID = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.layoutControlBusViewer.Name = "layoutControlBusViewer";
            this.layoutControlBusViewer.ShowOnlyNotInLocation = false;
            this.layoutControlBusViewer.Size = new System.Drawing.Size(200, 523);
            this.layoutControlBusViewer.StartingPoint = (System.Drawing.PointF)resources.GetObject("layoutControlBusViewer.StartingPoint");
            this.layoutControlBusViewer.TabIndex = 8;
            this.layoutControlBusViewer.Text = "layoutControlBusViewer1";
            this.layoutControlBusViewer.Zoom = 1F;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Controller:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxBusProvider
            // 
            this.comboBoxBusProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBusProvider.Location = new System.Drawing.Point(8, 36);
            this.comboBoxBusProvider.Name = "comboBoxBusProvider";
            this.comboBoxBusProvider.Size = new System.Drawing.Size(144, 21);
            this.comboBoxBusProvider.TabIndex = 1;
            this.comboBoxBusProvider.SelectedIndexChanged += new EventHandler(this.ComboBoxBusProvider_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Connection:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxBus
            // 
            this.comboBoxBus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBus.Location = new System.Drawing.Point(8, 74);
            this.comboBoxBus.Name = "comboBoxBus";
            this.comboBoxBus.Size = new System.Drawing.Size(144, 21);
            this.comboBoxBus.TabIndex = 3;
            this.comboBoxBus.SelectedIndexChanged += new EventHandler(this.ComboBoxBus_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Control Modules Location:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxLocation
            // 
            this.comboBoxLocation.Location = new System.Drawing.Point(8, 112);
            this.comboBoxLocation.Name = "comboBoxLocation";
            this.comboBoxLocation.Size = new System.Drawing.Size(144, 21);
            this.comboBoxLocation.TabIndex = 5;
            this.comboBoxLocation.SelectedIndexChanged += new EventHandler(this.ComboBoxLocation_SelectedIndexChanged);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonZoomIn.ImageIndex = 2;
            this.buttonZoomIn.ImageList = this.imageListCloseButton;
            this.buttonZoomIn.Location = new System.Drawing.Point(27, 141);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(16, 16);
            this.buttonZoomIn.TabIndex = 7;
            this.buttonZoomIn.UseVisualStyleBackColor = false;
            this.buttonZoomIn.Click += new EventHandler(this.ButtonZoomIn_Click);
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonZoomOut.ImageIndex = 1;
            this.buttonZoomOut.ImageList = this.imageListCloseButton;
            this.buttonZoomOut.Location = new System.Drawing.Point(8, 141);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(16, 16);
            this.buttonZoomOut.TabIndex = 6;
            this.buttonZoomOut.UseVisualStyleBackColor = false;
            this.buttonZoomOut.Click += new EventHandler(this.ButtonZoomOut_Click);
            // 
            // LayoutControlViewer
            // 
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
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.Name = "LayoutControlViewer";
            this.Size = new System.Drawing.Size(200, 688);
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