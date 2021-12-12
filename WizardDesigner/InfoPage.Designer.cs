using System.ComponentModel;
using System.Windows.Forms;

namespace Gui.Wizard {
    /// <summary>
    /// An inherited <see cref="InfoContainer"/> that contains a <see cref="Label"/> 
    /// with the description of the page.
    /// </summary>
    partial class InfoPage : InfoContainer {
        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lblDescription = new Label();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            this.lblDescription.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.lblDescription.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
            this.lblDescription.Location = new System.Drawing.Point(172, 56);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(304, 328);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "This wizard enables you to...";
            // 
            // InfoPage
            // 
            this.Controls.Add(this.lblDescription);
            this.Name = "InfoPage";
            this.Controls.SetChildIndex(this.lblDescription, 0);
            this.ResumeLayout(false);
        }
        #endregion
        private Label lblDescription;
        private readonly IContainer components = null;
    }
}

