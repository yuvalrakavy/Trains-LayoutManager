namespace LayoutManager.Tools.Dialogs {
    partial class CountComponents {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            this.listViewCounts = new System.Windows.Forms.ListView();
            this.buttonClose = new System.Windows.Forms.Button();
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderConnectedCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderNotConnectedCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTotalCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listViewCounts
            // 
            this.listViewCounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderType,
            this.columnHeaderConnectedCount,
            this.columnHeaderNotConnectedCount,
            this.columnHeaderTotalCount});
            this.listViewCounts.GridLines = true;
            this.listViewCounts.Location = new System.Drawing.Point(12, 12);
            this.listViewCounts.Name = "listViewCounts";
            this.listViewCounts.Size = new System.Drawing.Size(349, 299);
            this.listViewCounts.TabIndex = 0;
            this.listViewCounts.UseCompatibleStateImageBehavior = false;
            this.listViewCounts.View = System.Windows.Forms.View.Details;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(286, 325);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Connection Type";
            this.columnHeaderType.Width = 110;
            // 
            // columnHeaderConnectedCount
            // 
            this.columnHeaderConnectedCount.Text = "Connected";
            this.columnHeaderConnectedCount.Width = 81;
            // 
            // columnHeaderNotConnectedCount
            // 
            this.columnHeaderNotConnectedCount.Text = "Not Connected";
            this.columnHeaderNotConnectedCount.Width = 96;
            // 
            // columnHeaderTotalCount
            // 
            this.columnHeaderTotalCount.Text = "Total";
            // 
            // CountComponents
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(373, 360);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewCounts);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CountComponents";
            this.Text = "Component Count";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewCounts;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderConnectedCount;
        private System.Windows.Forms.ColumnHeader columnHeaderNotConnectedCount;
        private System.Windows.Forms.ColumnHeader columnHeaderTotalCount;
        private System.Windows.Forms.Button buttonClose;
    }
}