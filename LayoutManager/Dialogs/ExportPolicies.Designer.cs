namespace LayoutManager.Dialogs {
	partial class ExportPolicies {
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxFilename = new System.Windows.Forms.TextBox();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.treeViewScripts = new System.Windows.Forms.TreeView();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.treeViewExportedScripts = new System.Windows.Forms.TreeView();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.label4 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Export scripts to:";
			// 
			// textBoxFilename
			// 
			this.textBoxFilename.Location = new System.Drawing.Point(103, 6);
			this.textBoxFilename.Name = "textBoxFilename";
			this.textBoxFilename.Size = new System.Drawing.Size(498, 20);
			this.textBoxFilename.TabIndex = 1;
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Location = new System.Drawing.Point(607, 4);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
			this.buttonBrowse.TabIndex = 2;
			this.buttonBrowse.Text = "&Browse...";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.treeViewScripts, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.buttonAdd, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.buttonRemove, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.treeViewExportedScripts, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 55);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.36364F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.63636F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(667, 276);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// treeViewScripts
			// 
			this.treeViewScripts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewScripts.Location = new System.Drawing.Point(3, 23);
			this.treeViewScripts.Name = "treeViewScripts";
			this.tableLayoutPanel1.SetRowSpan(this.treeViewScripts, 4);
			this.treeViewScripts.Size = new System.Drawing.Size(287, 250);
			this.treeViewScripts.TabIndex = 1;
			this.treeViewScripts.DoubleClick += new System.EventHandler(this.TreeViewScripts_DoubleClick);
			this.treeViewScripts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewScriptsAndExportedScripts_AfterSelect);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdd.Location = new System.Drawing.Point(296, 116);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(74, 21);
			this.buttonAdd.TabIndex = 3;
			this.buttonAdd.Text = ">>";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.Location = new System.Drawing.Point(296, 143);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(74, 21);
			this.buttonRemove.TabIndex = 4;
			this.buttonRemove.Text = "<<";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.ButtonRemove_Click);
			// 
			// treeViewExportedScripts
			// 
			this.treeViewExportedScripts.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewExportedScripts.Location = new System.Drawing.Point(376, 23);
			this.treeViewExportedScripts.Name = "treeViewExportedScripts";
			this.tableLayoutPanel1.SetRowSpan(this.treeViewExportedScripts, 4);
			this.treeViewExportedScripts.Size = new System.Drawing.Size(288, 250);
			this.treeViewExportedScripts.TabIndex = 2;
			this.treeViewExportedScripts.DoubleClick += new System.EventHandler(this.TreeViewExportedScripts_DoubleClick);
			this.treeViewExportedScripts.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewScriptsAndExportedScripts_AfterSelect);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Available Definitions:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(376, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Exported definitions";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(526, 337);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(607, 337);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "LayoutScripts";
			this.saveFileDialog.Filter = "Layout Scripts|*.LayoutScripts|All files|*.*";
			this.saveFileDialog.Title = "Export scripts to";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 29);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(364, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Note: Only scripts that are marked \'Available for all layouts\' can be exported.";
			// 
			// ExportPolicies
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(694, 369);
			this.ControlBox = false;
			this.Controls.Add(this.label4);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.buttonBrowse);
			this.Controls.Add(this.textBoxFilename);
			this.Controls.Add(this.label1);
			this.Name = "ExportPolicies";
			this.ShowInTaskbar = false;
			this.Text = "Export Script Definitions";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxFilename;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TreeView treeViewScripts;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.TreeView treeViewExportedScripts;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label label4;
	}
}