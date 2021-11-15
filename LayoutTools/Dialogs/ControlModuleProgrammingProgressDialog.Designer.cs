namespace LayoutManager.Tools.Dialogs {
	partial class ControlModuleProgrammingProgressDialog {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
			this.buttonNext = new System.Windows.Forms.Button();
			this.buttonCancelOrClose = new System.Windows.Forms.Button();
			this.panelConnectModule = new System.Windows.Forms.Panel();
			this.panelProgress = new System.Windows.Forms.Panel();
			this.listViewActions = new System.Windows.Forms.ListView();
			this.columnHeaderActionName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeaderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.labelConnect = new System.Windows.Forms.Label();
			this.panelConnectModule.SuspendLayout();
			this.panelProgress.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonNext
			// 
			this.buttonNext.Location = new System.Drawing.Point(263, 120);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(75, 23);
			this.buttonNext.TabIndex = 0;
			this.buttonNext.Text = "Next >";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.ButtonNext_Click);
			// 
			// buttonCancelOrClose
			// 
			this.buttonCancelOrClose.Location = new System.Drawing.Point(344, 120);
			this.buttonCancelOrClose.Name = "buttonCancelOrClose";
			this.buttonCancelOrClose.Size = new System.Drawing.Size(75, 23);
			this.buttonCancelOrClose.TabIndex = 1;
			this.buttonCancelOrClose.Text = "&Cancel";
			this.buttonCancelOrClose.UseVisualStyleBackColor = true;
			this.buttonCancelOrClose.Click += new System.EventHandler(this.ButtonCancelOrClose_Click);
			// 
			// panelConnectModule
			// 
			this.panelConnectModule.Controls.Add(this.labelConnect);
			this.panelConnectModule.Location = new System.Drawing.Point(12, 1);
			this.panelConnectModule.Name = "panelConnectModule";
			this.panelConnectModule.Size = new System.Drawing.Size(406, 119);
			this.panelConnectModule.TabIndex = 2;
			// 
			// panelProgress
			// 
			this.panelProgress.Controls.Add(this.listViewActions);
			this.panelProgress.Location = new System.Drawing.Point(0, 0);
			this.panelProgress.Name = "panelProgress";
			this.panelProgress.Size = new System.Drawing.Size(405, 118);
			this.panelProgress.TabIndex = 1;
			// 
			// listViewActions
			// 
			this.listViewActions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderActionName,
            this.columnHeaderStatus});
			this.listViewActions.FullRowSelect = true;
			this.listViewActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewActions.Location = new System.Drawing.Point(9, 8);
			this.listViewActions.Name = "listViewActions";
			this.listViewActions.Size = new System.Drawing.Size(400, 99);
			this.listViewActions.TabIndex = 1;
			this.listViewActions.UseCompatibleStateImageBehavior = false;
			this.listViewActions.View = System.Windows.Forms.View.Details;
			// 
			// columnHeaderActionName
			// 
			this.columnHeaderActionName.Text = "Action";
			this.columnHeaderActionName.Width = 300;
			// 
			// columnHeaderStatus
			// 
			this.columnHeaderStatus.Text = "Status";
			this.columnHeaderStatus.Width = 90;
			// 
			// labelConnect
			// 
			this.labelConnect.Location = new System.Drawing.Point(0, 25);
			this.labelConnect.Name = "labelConnect";
			this.labelConnect.Size = new System.Drawing.Size(400, 85);
			this.labelConnect.TabIndex = 0;
			this.labelConnect.Text = "Please connect {0} module to {1} programming track. Press Next when done.";
			// 
			// ControlModuleProgrammingProgressDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(428, 149);
			this.ControlBox = false;
			this.Controls.Add(this.panelProgress);
			this.Controls.Add(this.panelConnectModule);
			this.Controls.Add(this.buttonCancelOrClose);
			this.Controls.Add(this.buttonNext);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ControlModuleProgrammingProgressDialog";
			this.ShowInTaskbar = false;
			this.Text = "Control Module Programming";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlModuleProgrammingProgressDialog_FormClosing);
			this.panelConnectModule.ResumeLayout(false);
			this.panelProgress.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.Button buttonCancelOrClose;
		private System.Windows.Forms.Panel panelConnectModule;
		private System.Windows.Forms.Panel panelProgress;
		private System.Windows.Forms.ListView listViewActions;
		private System.Windows.Forms.ColumnHeader columnHeaderActionName;
		private System.Windows.Forms.ColumnHeader columnHeaderStatus;
		private System.Windows.Forms.Label labelConnect;
	}
}