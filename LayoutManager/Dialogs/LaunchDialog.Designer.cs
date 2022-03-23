namespace LayoutManager.Dialogs {
	partial class LaunchDialog {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LaunchDialog));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonNew = new System.Windows.Forms.Button();
			this.buttonOpen = new System.Windows.Forms.Button();
			this.buttonExit = new System.Windows.Forms.Button();
			this.linkLabelLastLayoutName = new System.Windows.Forms.LinkLabel();
			this.checkBoxResetToDefaultDisplayLayout = new System.Windows.Forms.CheckBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
			this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.pictureBox1.Location = new System.Drawing.Point(-2, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(566, 363);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.checkBoxResetToDefaultDisplayLayout);
			this.panel1.Controls.Add(this.buttonOpen);
			this.panel1.Controls.Add(this.linkLabelLastLayoutName);
			this.panel1.Location = new System.Drawing.Point(-2, 361);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(720, 62);
			this.panel1.TabIndex = 2;
			// 
			// buttonNew
			// 
			this.buttonNew.Location = new System.Drawing.Point(579, 12);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(122, 42);
			this.buttonNew.TabIndex = 3;
			this.buttonNew.Text = "&New...";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler(this.ButtonNew_Click);
			// 
			// buttonOpen
			// 
			this.buttonOpen.Location = new System.Drawing.Point(13, 8);
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new System.Drawing.Size(122, 42);
			this.buttonOpen.TabIndex = 3;
			this.buttonOpen.Text = "&Open...";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new System.EventHandler(this.ButtonOpen_Click);
			// 
			// buttonExit
			// 
			this.buttonExit.Location = new System.Drawing.Point(579, 304);
			this.buttonExit.Name = "buttonExit";
			this.buttonExit.Size = new System.Drawing.Size(122, 42);
			this.buttonExit.TabIndex = 3;
			this.buttonExit.Text = "&Exit";
			this.buttonExit.UseVisualStyleBackColor = true;
			this.buttonExit.Click += new System.EventHandler(this.ButtonExit_Click);
			// 
			// linkLabelLastLayoutName
			// 
			this.linkLabelLastLayoutName.AutoEllipsis = true;
			this.linkLabelLastLayoutName.Cursor = System.Windows.Forms.Cursors.Hand;
			this.linkLabelLastLayoutName.Location = new System.Drawing.Point(151, 8);
			this.linkLabelLastLayoutName.Name = "linkLabelLastLayoutName";
			this.linkLabelLastLayoutName.Size = new System.Drawing.Size(551, 23);
			this.linkLabelLastLayoutName.TabIndex = 2;
			this.linkLabelLastLayoutName.TabStop = true;
			this.linkLabelLastLayoutName.Text = "Last layout name";
			this.linkLabelLastLayoutName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelLastLayoutName_LinkClicked);
			// 
			// checkBoxResetToDefaultDisplayLayout
			// 
			this.checkBoxResetToDefaultDisplayLayout.AutoSize = true;
			this.checkBoxResetToDefaultDisplayLayout.Location = new System.Drawing.Point(154, 33);
			this.checkBoxResetToDefaultDisplayLayout.Name = "checkBoxResetToDefaultDisplayLayout";
			this.checkBoxResetToDefaultDisplayLayout.Size = new System.Drawing.Size(180, 17);
			this.checkBoxResetToDefaultDisplayLayout.TabIndex = 3;
			this.checkBoxResetToDefaultDisplayLayout.Text = "Reset display layout to its default";
			this.checkBoxResetToDefaultDisplayLayout.UseVisualStyleBackColor = true;
			// 
			// openFileDialog
			// 
			this.openFileDialog.DefaultExt = "Layout";
			this.openFileDialog.Filter = "Layout files|*.Layout|All files|*.*";
			this.openFileDialog.Title = "Select Layout file to use";
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "Layout";
			this.saveFileDialog.Filter = "Layout files|*.Layout|All files|*.*";
			// 
			// LaunchDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(713, 419);
			this.Controls.Add(this.buttonExit);
			this.Controls.Add(this.buttonNew);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "LaunchDialog";
			this.Text = "VillaRakavy - Model Train Layout Manager";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.Button buttonOpen;
		private System.Windows.Forms.Button buttonExit;
		private System.Windows.Forms.CheckBox checkBoxResetToDefaultDisplayLayout;
		private System.Windows.Forms.LinkLabel linkLabelLastLayoutName;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
	}
}