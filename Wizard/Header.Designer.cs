using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Gui.Wizard {
    /// <summary>
    /// Summary description for WizardHeader.
    /// </summary>
    [Designer("Gui.Wizard.HeaderDesigner", "WizardDesigner.dll")]
    partial class Header : UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Header));
            this.pnlDockPadding = new Panel();
            this.lblDescription = new Label();
            this.lblTitle = new Label();
            this.picIcon = new PictureBox();
            this.pnl3dDark = new Panel();
            this.pnl3dBright = new Panel();
            this.pnlDockPadding.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDockPadding
            // 
            this.pnlDockPadding.BackColor = System.Drawing.SystemColors.Window;
            this.pnlDockPadding.Controls.Add(this.lblDescription);
            this.pnlDockPadding.Controls.Add(this.lblTitle);
            this.pnlDockPadding.Controls.Add(this.picIcon);
            this.pnlDockPadding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDockPadding.DockPadding.Bottom = 4;
            this.pnlDockPadding.DockPadding.Left = 8;
            this.pnlDockPadding.DockPadding.Right = 4;
            this.pnlDockPadding.DockPadding.Top = 6;
            this.pnlDockPadding.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
            this.pnlDockPadding.Location = new System.Drawing.Point(0, 0);
            this.pnlDockPadding.Name = "pnlDockPadding";
            this.pnlDockPadding.Size = new System.Drawing.Size(324, 64);
            this.pnlDockPadding.TabIndex = 6;
            // 
            // lblDescription
            // 
            this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescription.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblDescription.Location = new System.Drawing.Point(8, 22);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(260, 38);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
            this.lblTitle.Location = new System.Drawing.Point(8, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(260, 16);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Title";
            // 
            // picIcon
            // 
            this.picIcon.Dock = System.Windows.Forms.DockStyle.Right;
            this.picIcon.Image = (System.Drawing.Image)resources.GetObject("picIcon.Image");
            this.picIcon.Location = new System.Drawing.Point(268, 6);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(52, 54);
            this.picIcon.TabIndex = 3;
            this.picIcon.TabStop = false;
            // 
            // pnl3dDark
            // 
            this.pnl3dDark.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnl3dDark.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl3dDark.Location = new System.Drawing.Point(0, 62);
            this.pnl3dDark.Name = "pnl3dDark";
            this.pnl3dDark.Size = new System.Drawing.Size(324, 1);
            this.pnl3dDark.TabIndex = 7;
            // 
            // pnl3dBright
            // 
            this.pnl3dBright.BackColor = System.Drawing.Color.White;
            this.pnl3dBright.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnl3dBright.Location = new System.Drawing.Point(0, 63);
            this.pnl3dBright.Name = "pnl3dBright";
            this.pnl3dBright.Size = new System.Drawing.Size(324, 1);
            this.pnl3dBright.TabIndex = 8;
            // 
            // Header
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CausesValidation = false;
            this.Controls.Add(this.pnl3dDark);
            this.Controls.Add(this.pnl3dBright);
            this.Controls.Add(this.pnlDockPadding);
            this.Name = "Header";
            this.Size = new System.Drawing.Size(324, 64);
            this.SizeChanged += this.Header_SizeChanged;
            this.pnlDockPadding.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private Panel pnlDockPadding;
        private Label lblDescription;
        private Label lblTitle;
        private PictureBox picIcon;
        private Panel pnl3dDark;
        private Panel pnl3dBright;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

