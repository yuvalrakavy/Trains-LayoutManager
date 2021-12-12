using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Gui.Wizard {
    /// <summary>
    /// A wizard is the control added to a form to provide a step by step functionality.
    /// It contains <see cref="WizardPage"/>s in the <see cref="Pages"/> collection, which
    /// are containers for other controls. Only one wizard page is shown at a time in the client
    /// are of the wizard.
    /// </summary>
    partial class Wizard : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.pnlButtons = new Panel();
            this.btnCancel = new Button();
            this.btnNext = new Button();
            this.btnBack = new Button();
            this.pnlButtonBright3d = new Panel();
            this.pnlButtonDark3d = new Panel();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnNext);
            this.pnlButtons.Controls.Add(this.btnBack);
            this.pnlButtons.Controls.Add(this.pnlButtonBright3d);
            this.pnlButtons.Controls.Add(this.pnlButtonDark3d);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtons.Location = new System.Drawing.Point(0, 224);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(444, 48);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(356, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += this.BtnCancel_Click;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Location = new System.Drawing.Point(272, 12);
            this.btnNext.Name = "btnNext";
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "&Next >";
            this.btnNext.Click += this.BtnNext_Click;
            this.btnNext.MouseDown += this.BtnNext_MouseDown;
            // 
            // btnBack
            // 
            this.btnBack.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBack.Location = new System.Drawing.Point(196, 12);
            this.btnBack.Name = "btnBack";
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "< &Back";
            this.btnBack.Click += this.BtnBack_Click;
            this.btnBack.MouseDown += this.BtnBack_MouseDown;
            // 
            // pnlButtonBright3d
            // 
            this.pnlButtonBright3d.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pnlButtonBright3d.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtonBright3d.Location = new System.Drawing.Point(0, 1);
            this.pnlButtonBright3d.Name = "pnlButtonBright3d";
            this.pnlButtonBright3d.Size = new System.Drawing.Size(444, 1);
            this.pnlButtonBright3d.TabIndex = 1;
            // 
            // pnlButtonDark3d
            // 
            this.pnlButtonDark3d.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pnlButtonDark3d.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtonDark3d.Location = new System.Drawing.Point(0, 0);
            this.pnlButtonDark3d.Name = "pnlButtonDark3d";
            this.pnlButtonDark3d.Size = new System.Drawing.Size(444, 1);
            this.pnlButtonDark3d.TabIndex = 2;
            // 
            // Wizard
            // 
            this.Controls.Add(this.pnlButtons);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
            this.Name = "Wizard";
            this.Size = new System.Drawing.Size(444, 272);
            this.Load += this.Wizard_Load;
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
        protected internal Panel pnlButtons;
        private Panel pnlButtonBright3d;
        private Panel pnlButtonDark3d;
        public Button btnBack;
        public Button btnNext;
        private Button btnCancel;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

