namespace LayoutManager.Tools.Dialogs {
	partial class SelectTrainToPlace {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectTrainToPlace));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxSearch = new System.Windows.Forms.TextBox();
			this.listBoxSearchResult = new System.Windows.Forms.ListBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelSearchInstructions = new System.Windows.Forms.Label();
			this.labelNoMatch = new System.Windows.Forms.Label();
			this.locomotiveFront = new LayoutManager.CommonUI.Controls.LocomotiveFront();
			this.buttonNew = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.trainLengthDiagram = new LayoutManager.CommonUI.Controls.TrainLengthDiagram();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(44, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Search:";
			// 
			// textBoxSearch
			// 
			this.textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxSearch.Location = new System.Drawing.Point(15, 25);
			this.textBoxSearch.Name = "textBoxSearch";
			this.textBoxSearch.Size = new System.Drawing.Size(199, 20);
			this.textBoxSearch.TabIndex = 1;
			this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
			// 
			// listBoxSearchResult
			// 
			this.listBoxSearchResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxSearchResult.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.listBoxSearchResult.FormattingEnabled = true;
			this.listBoxSearchResult.IntegralHeight = false;
			this.listBoxSearchResult.Location = new System.Drawing.Point(15, 51);
			this.listBoxSearchResult.Name = "listBoxSearchResult";
			this.listBoxSearchResult.Size = new System.Drawing.Size(199, 171);
			this.listBoxSearchResult.TabIndex = 2;
			this.listBoxSearchResult.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxSearchResult_DrawItem);
			this.listBoxSearchResult.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxSearchResult_MeasureItem);
			this.listBoxSearchResult.SelectedIndexChanged += new System.EventHandler(this.listBoxSearchResult_SelectedIndexChanged);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(262, 228);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(343, 228);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// labelSearchInstructions
			// 
			this.labelSearchInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelSearchInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelSearchInstructions.Location = new System.Drawing.Point(12, 51);
			this.labelSearchInstructions.Name = "labelSearchInstructions";
			this.labelSearchInstructions.Size = new System.Drawing.Size(202, 110);
			this.labelSearchInstructions.TabIndex = 8;
			this.labelSearchInstructions.Text = "Seach a locomotive or train by entering any part of its name.\r\n\r\nYou may include " +
				"(address) or [collection ID].\r\n\r\nFor example (3) or [100]";
			this.labelSearchInstructions.Visible = false;
			// 
			// labelNoMatch
			// 
			this.labelNoMatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelNoMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.labelNoMatch.Location = new System.Drawing.Point(12, 152);
			this.labelNoMatch.Name = "labelNoMatch";
			this.labelNoMatch.Size = new System.Drawing.Size(202, 57);
			this.labelNoMatch.TabIndex = 9;
			this.labelNoMatch.Text = "No locomotive or train name matches  the text you have entered";
			this.labelNoMatch.Visible = false;
			// 
			// locomotiveFront
			// 
			this.locomotiveFront.ConnectionPoints = ((System.Collections.Generic.IList<LayoutManager.Model.LayoutComponentConnectionPoint>)(resources.GetObject("locomotiveFront.ConnectionPoints")));
			this.locomotiveFront.Front = ((LayoutManager.Model.LayoutComponentConnectionPoint)(resources.GetObject("locomotiveFront.Front")));
			this.locomotiveFront.Location = new System.Drawing.Point(47, 19);
			this.locomotiveFront.LocomotiveName = null;
			this.locomotiveFront.Name = "locomotiveFront";
			this.locomotiveFront.Size = new System.Drawing.Size(90, 90);
			this.locomotiveFront.TabIndex = 0;
			this.locomotiveFront.Text = "locomotiveFront3";
			// 
			// buttonNew
			// 
			this.buttonNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNew.Location = new System.Drawing.Point(15, 228);
			this.buttonNew.Name = "buttonNew";
			this.buttonNew.Size = new System.Drawing.Size(75, 23);
			this.buttonNew.TabIndex = 3;
			this.buttonNew.Text = "New";
			this.buttonNew.UseVisualStyleBackColor = true;
			this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.locomotiveFront);
			this.groupBox1.Location = new System.Drawing.Point(226, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(192, 124);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Set orientation:";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.trainLengthDiagram);
			this.groupBox2.Location = new System.Drawing.Point(226, 139);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(192, 83);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sef train length:";
			// 
			// trainLengthDiagram
			// 
			this.trainLengthDiagram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.trainLengthDiagram.Comparison = LayoutManager.Model.TrainLengthComparison.None;
			this.trainLengthDiagram.Location = new System.Drawing.Point(6, 19);
			this.trainLengthDiagram.Name = "trainLengthDiagram";
			this.trainLengthDiagram.Size = new System.Drawing.Size(180, 52);
			this.trainLengthDiagram.TabIndex = 0;
			// 
			// SelectTrainToPlace
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(427, 265);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonNew);
			this.Controls.Add(this.labelNoMatch);
			this.Controls.Add(this.labelSearchInstructions);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listBoxSearchResult);
			this.Controls.Add(this.textBoxSearch);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "SelectTrainToPlace";
			this.Text = "Place train or locomotive";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxSearch;
		private System.Windows.Forms.ListBox listBoxSearchResult;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private LayoutManager.CommonUI.Controls.LocomotiveFront locomotiveFront;
		private System.Windows.Forms.Label labelSearchInstructions;
		private System.Windows.Forms.Label labelNoMatch;
		private System.Windows.Forms.Button buttonNew;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private LayoutManager.CommonUI.Controls.TrainLengthDiagram trainLengthDiagram;
	}
}