namespace LayoutManager.Dialogs {
	partial class LearnLayout {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LearnLayout));
			this.listViewEvents = new System.Windows.Forms.ListView();
			this.columnHeaderAddress = new System.Windows.Forms.ColumnHeader("");
			this.columnHeaderBus = new System.Windows.Forms.ColumnHeader("");
			this.columnHeaderState = new System.Windows.Forms.ColumnHeader("");
			this.columnHeaderStatus = new System.Windows.Forms.ColumnHeader("");
			this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
			this.buttonAction = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonRemoveAll = new System.Windows.Forms.Button();
			this.buttonClose = new System.Windows.Forms.Button();
			this.checkBoxEnableSound = new System.Windows.Forms.CheckBox();
			this.checkBoxOnlyShowNotConnected = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
// 
// listViewEvents
// 
			this.listViewEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderAddress,
            this.columnHeaderBus,
            this.columnHeaderState,
            this.columnHeaderStatus});
			this.listViewEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewEvents.HideSelection = false;
			this.listViewEvents.Location = new System.Drawing.Point(13, 13);
			this.listViewEvents.MultiSelect = false;
			this.listViewEvents.Name = "listViewEvents";
			this.listViewEvents.Size = new System.Drawing.Size(504, 235);
			this.listViewEvents.SmallImageList = this.imageListStatus;
			this.listViewEvents.TabIndex = 0;
			this.listViewEvents.View = System.Windows.Forms.View.Details;
			this.listViewEvents.SelectedIndexChanged += new System.EventHandler(this.ListViewEvents_SelectedIndexChanged);
			this.listViewEvents.DoubleClick += new System.EventHandler(this.ListViewEvents_DoubleClick);
// 
// columnHeaderAddress
// 
			this.columnHeaderAddress.Text = "Address";
			this.columnHeaderAddress.Width = 77;
// 
// columnHeaderBus
// 
			this.columnHeaderBus.Text = "Connection";
			this.columnHeaderBus.Width = 88;
// 
// columnHeaderState
// 
			this.columnHeaderState.Text = "State";
			this.columnHeaderState.Width = 48;
// 
// columnHeaderStatus
// 
			this.columnHeaderStatus.Text = "Status";
			this.columnHeaderStatus.Width = 168;
// 
// imageListStatus
// 
			this.imageListStatus.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
			this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListStatus.Images.SetKeyName(0, "LearnLayoutConnected.ICO");
			this.imageListStatus.Images.SetKeyName(1, "LearnLayoutNotConnected.ICO");
			this.imageListStatus.Images.SetKeyName(2, "LearnLayoutNoControlModule.ICO");
// 
// buttonAction
// 
			this.buttonAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonAction.Location = new System.Drawing.Point(13, 255);
			this.buttonAction.Name = "buttonAction";
			this.buttonAction.TabIndex = 1;
			this.buttonAction.Text = "&Connect";
			this.buttonAction.Click += new System.EventHandler(this.ButtonAction_Click);
// 
// buttonRemove
// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonRemove.Location = new System.Drawing.Point(95, 255);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.TabIndex = 2;
			this.buttonRemove.Text = "&Remove";
			this.buttonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
// 
// buttonRemoveAll
// 
			this.buttonRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemoveAll.Location = new System.Drawing.Point(442, 255);
			this.buttonRemoveAll.Name = "buttonRemoveAll";
			this.buttonRemoveAll.TabIndex = 3;
			this.buttonRemoveAll.Text = "Remove &All";
			this.buttonRemoveAll.Click += new System.EventHandler(this.ButtonRemoveAll_Click);
// 
// buttonClose
// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(442, 306);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "&Close";
			this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
// 
// checkBoxEnableSound
// 
			this.checkBoxEnableSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxEnableSound.AutoSize = true;
			this.checkBoxEnableSound.Location = new System.Drawing.Point(13, 312);
			this.checkBoxEnableSound.Name = "checkBoxEnableSound";
			this.checkBoxEnableSound.Size = new System.Drawing.Size(150, 17);
			this.checkBoxEnableSound.TabIndex = 5;
			this.checkBoxEnableSound.Text = "Play sound upon new input";
			this.checkBoxEnableSound.CheckedChanged += new System.EventHandler(this.CheckBoxEnableSound_CheckedChanged);
// 
// checkBoxOnlyShowNotConnected
// 
			this.checkBoxOnlyShowNotConnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxOnlyShowNotConnected.AutoSize = true;
			this.checkBoxOnlyShowNotConnected.Location = new System.Drawing.Point(13, 288);
			this.checkBoxOnlyShowNotConnected.Name = "checkBoxOnlyShowNotConnected";
			this.checkBoxOnlyShowNotConnected.Size = new System.Drawing.Size(256, 17);
			this.checkBoxOnlyShowNotConnected.TabIndex = 6;
			this.checkBoxOnlyShowNotConnected.Text = "Only show input from non-connected components";
// 
// LearnLayout
// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(529, 337);
			this.Controls.Add(this.checkBoxOnlyShowNotConnected);
			this.Controls.Add(this.checkBoxEnableSound);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.buttonRemoveAll);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonAction);
			this.Controls.Add(this.listViewEvents);
			this.Name = "LearnLayout";
			this.Text = "Learn Layout";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LearnLayout_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewEvents;
		private System.Windows.Forms.ColumnHeader columnHeaderStatus;
		private System.Windows.Forms.ColumnHeader columnHeaderAddress;
		private System.Windows.Forms.ImageList imageListStatus;
		private System.Windows.Forms.Button buttonAction;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonRemoveAll;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.ColumnHeader columnHeaderBus;
		private System.Windows.Forms.CheckBox checkBoxEnableSound;
		private System.Windows.Forms.ColumnHeader columnHeaderState;
		private System.Windows.Forms.CheckBox checkBoxOnlyShowNotConnected;
	}
}