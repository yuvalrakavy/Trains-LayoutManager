﻿using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for TestControl.
    /// </summary>
    public partial class TestControl : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonClose = new Button();
            this.selectTripPlanIcon1 = new CommonUI.Controls.SelectTripPlanIcon();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(208, 232);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // selectTripPlanIcon1
            // 
            this.selectTripPlanIcon1.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.selectTripPlanIcon1.IconList = null;
            this.selectTripPlanIcon1.Location = new System.Drawing.Point(16, 16);
            this.selectTripPlanIcon1.Name = "selectTripPlanIcon1";
            this.selectTripPlanIcon1.SelectedID = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.selectTripPlanIcon1.SelectedIndex = -1;
            this.selectTripPlanIcon1.Size = new System.Drawing.Size(264, 86);
            this.selectTripPlanIcon1.TabIndex = 2;
            // 
            // TestControl
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.selectTripPlanIcon1);
            this.Controls.Add(this.buttonClose);
            this.Name = "TestControl";
            this.Text = "Test Some Control...";
            this.ResumeLayout(false);
        }
        #endregion

        private Button buttonClose;
        private CommonUI.Controls.SelectTripPlanIcon selectTripPlanIcon1;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
