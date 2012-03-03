namespace LayoutManager.Tools.Dialogs {
	partial class BlockDefinitionResources {
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
			this.buttonResourceAdd = new System.Windows.Forms.Button();
			this.listBoxResources = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonResourceRemove = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonResourceAdd
			// 
			this.buttonResourceAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonResourceAdd.Location = new System.Drawing.Point(12, 236);
			this.buttonResourceAdd.Name = "buttonResourceAdd";
			this.buttonResourceAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonResourceAdd.TabIndex = 6;
			this.buttonResourceAdd.Text = "&Add";
			this.buttonResourceAdd.Click += new System.EventHandler(this.buttonResourceAdd_Click);
			// 
			// listBoxResources
			// 
			this.listBoxResources.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxResources.Location = new System.Drawing.Point(12, 44);
			this.listBoxResources.Name = "listBoxResources";
			this.listBoxResources.Size = new System.Drawing.Size(184, 186);
			this.listBoxResources.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(264, 40);
			this.label2.TabIndex = 3;
			this.label2.Text = "When this block is allocated to a train, the following components will be allocat" +
				"ed to the same train:";
			// 
			// buttonResourceRemove
			// 
			this.buttonResourceRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonResourceRemove.Location = new System.Drawing.Point(93, 237);
			this.buttonResourceRemove.Name = "buttonResourceRemove";
			this.buttonResourceRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonResourceRemove.TabIndex = 5;
			this.buttonResourceRemove.Text = "&Remove";
			this.buttonResourceRemove.Click += new System.EventHandler(this.buttonResourceRemove_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(205, 207);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(205, 237);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// BlockDefinitionResources
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 272);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonResourceAdd);
			this.Controls.Add(this.listBoxResources);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonResourceRemove);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "BlockDefinitionResources";
			this.ShowInTaskbar = false;
			this.Text = "Block Resources";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonResourceAdd;
		private System.Windows.Forms.ListBox listBoxResources;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonResourceRemove;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}