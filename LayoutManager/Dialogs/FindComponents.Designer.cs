namespace LayoutManager.Dialogs {
	partial class FindComponents {
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxFind = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkBoxScopeAttributes = new System.Windows.Forms.CheckBox();
			this.checkBoxScopeAddresses = new System.Windows.Forms.CheckBox();
			this.checkBoxScopeNames = new System.Windows.Forms.CheckBox();
			this.buttonSearch = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxSelectResults = new System.Windows.Forms.CheckBox();
			this.checkBoxLimitToActiveArea = new System.Windows.Forms.CheckBox();
			this.checkBoxExactMatch = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find:";
			// 
			// textBoxFind
			// 
			this.textBoxFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxFind.Location = new System.Drawing.Point(49, 10);
			this.textBoxFind.Name = "textBoxFind";
			this.textBoxFind.Size = new System.Drawing.Size(231, 20);
			this.textBoxFind.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkBoxScopeAttributes);
			this.groupBox1.Controls.Add(this.checkBoxScopeAddresses);
			this.groupBox1.Controls.Add(this.checkBoxScopeNames);
			this.groupBox1.Location = new System.Drawing.Point(13, 37);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(143, 100);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search scope:";
			// 
			// checkBoxScopeAttributes
			// 
			this.checkBoxScopeAttributes.AutoSize = true;
			this.checkBoxScopeAttributes.Checked = true;
			this.checkBoxScopeAttributes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxScopeAttributes.Location = new System.Drawing.Point(7, 68);
			this.checkBoxScopeAttributes.Name = "checkBoxScopeAttributes";
			this.checkBoxScopeAttributes.Size = new System.Drawing.Size(70, 17);
			this.checkBoxScopeAttributes.TabIndex = 2;
			this.checkBoxScopeAttributes.Text = "Attributes";
			// 
			// checkBoxScopeAddresses
			// 
			this.checkBoxScopeAddresses.AutoSize = true;
			this.checkBoxScopeAddresses.Checked = true;
			this.checkBoxScopeAddresses.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxScopeAddresses.Location = new System.Drawing.Point(7, 44);
			this.checkBoxScopeAddresses.Name = "checkBoxScopeAddresses";
			this.checkBoxScopeAddresses.Size = new System.Drawing.Size(75, 17);
			this.checkBoxScopeAddresses.TabIndex = 1;
			this.checkBoxScopeAddresses.Text = "Addresses";
			// 
			// checkBoxScopeNames
			// 
			this.checkBoxScopeNames.AutoSize = true;
			this.checkBoxScopeNames.Checked = true;
			this.checkBoxScopeNames.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxScopeNames.Location = new System.Drawing.Point(7, 20);
			this.checkBoxScopeNames.Name = "checkBoxScopeNames";
			this.checkBoxScopeNames.Size = new System.Drawing.Size(59, 17);
			this.checkBoxScopeNames.TabIndex = 0;
			this.checkBoxScopeNames.Text = "Names";
			// 
			// buttonSearch
			// 
			this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSearch.Location = new System.Drawing.Point(205, 37);
			this.buttonSearch.Name = "buttonSearch";
			this.buttonSearch.Size = new System.Drawing.Size(75, 23);
			this.buttonSearch.TabIndex = 6;
			this.buttonSearch.Text = "Search";
			this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(205, 67);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			// 
			// checkBoxSelectResults
			// 
			this.checkBoxSelectResults.AutoSize = true;
			this.checkBoxSelectResults.Location = new System.Drawing.Point(13, 192);
			this.checkBoxSelectResults.Name = "checkBoxSelectResults";
			this.checkBoxSelectResults.Size = new System.Drawing.Size(147, 17);
			this.checkBoxSelectResults.TabIndex = 5;
			this.checkBoxSelectResults.Text = "Select found components";
			// 
			// checkBoxLimitToActiveArea
			// 
			this.checkBoxLimitToActiveArea.AutoSize = true;
			this.checkBoxLimitToActiveArea.Checked = true;
			this.checkBoxLimitToActiveArea.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxLimitToActiveArea.Location = new System.Drawing.Point(13, 168);
			this.checkBoxLimitToActiveArea.Name = "checkBoxLimitToActiveArea";
			this.checkBoxLimitToActiveArea.Size = new System.Drawing.Size(149, 17);
			this.checkBoxLimitToActiveArea.TabIndex = 4;
			this.checkBoxLimitToActiveArea.Text = "Search only in active area";
			// 
			// checkBoxExactMatch
			// 
			this.checkBoxExactMatch.AutoSize = true;
			this.checkBoxExactMatch.Location = new System.Drawing.Point(13, 144);
			this.checkBoxExactMatch.Name = "checkBoxExactMatch";
			this.checkBoxExactMatch.Size = new System.Drawing.Size(85, 17);
			this.checkBoxExactMatch.TabIndex = 3;
			this.checkBoxExactMatch.Text = "Exact match";
			// 
			// FindComponents
			// 
			this.AcceptButton = this.buttonSearch;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(292, 213);
			this.ControlBox = false;
			this.Controls.Add(this.checkBoxExactMatch);
			this.Controls.Add(this.checkBoxLimitToActiveArea);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSearch);
			this.Controls.Add(this.checkBoxSelectResults);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textBoxFind);
			this.Controls.Add(this.label1);
			this.Name = "FindComponents";
			this.ShowInTaskbar = false;
			this.Text = "Find Components";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxFind;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxScopeNames;
		private System.Windows.Forms.CheckBox checkBoxScopeAddresses;
		private System.Windows.Forms.CheckBox checkBoxScopeAttributes;
		private System.Windows.Forms.Button buttonSearch;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxSelectResults;
		private System.Windows.Forms.CheckBox checkBoxLimitToActiveArea;
		private System.Windows.Forms.CheckBox checkBoxExactMatch;
	}
}