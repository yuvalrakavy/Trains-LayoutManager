namespace LayoutManager.Tools.Dialogs {
    partial class TestThreeWayTurnout {
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
            this.components = new System.ComponentModel.Container();
            this.panelIllustration = new System.Windows.Forms.Panel();
            this.radioButtonBottom = new System.Windows.Forms.RadioButton();
            this.radioButtonTop = new System.Windows.Forms.RadioButton();
            this.radioButtonRight = new System.Windows.Forms.RadioButton();
            this.radioButtonLeft = new System.Windows.Forms.RadioButton();
            this.buttonPassed = new System.Windows.Forms.Button();
            this.buttonFailed = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.buttonSwap = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // panelIllustration
            // 
            this.panelIllustration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelIllustration.Location = new System.Drawing.Point(30, 30);
            this.panelIllustration.Name = "panelIllustration";
            this.panelIllustration.Size = new System.Drawing.Size(64, 64);
            this.panelIllustration.TabIndex = 0;
            this.panelIllustration.Paint += new System.Windows.Forms.PaintEventHandler(this.panelIllustration_Paint);
            // 
            // radioButtonBottom
            // 
            this.radioButtonBottom.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonBottom.Location = new System.Drawing.Point(46, 99);
            this.radioButtonBottom.Name = "radioButtonBottom";
            this.radioButtonBottom.Size = new System.Drawing.Size(32, 16);
            this.radioButtonBottom.TabIndex = 5;
            this.radioButtonBottom.TabStop = true;
            this.radioButtonBottom.Text = " ";
            this.radioButtonBottom.UseVisualStyleBackColor = true;
            this.radioButtonBottom.Click += new System.EventHandler(this.radioButtonBottom_Clicked);
            // 
            // radioButtonTop
            // 
            this.radioButtonTop.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonTop.Location = new System.Drawing.Point(46, 8);
            this.radioButtonTop.Name = "radioButtonTop";
            this.radioButtonTop.Size = new System.Drawing.Size(32, 16);
            this.radioButtonTop.TabIndex = 3;
            this.radioButtonTop.TabStop = true;
            this.radioButtonTop.Text = " ";
            this.radioButtonTop.UseVisualStyleBackColor = true;
            this.radioButtonTop.Click += new System.EventHandler(this.radioButtonTop_Clicked);
            // 
            // radioButtonRight
            // 
            this.radioButtonRight.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonRight.Location = new System.Drawing.Point(100, 46);
            this.radioButtonRight.Name = "radioButtonRight";
            this.radioButtonRight.Size = new System.Drawing.Size(16, 32);
            this.radioButtonRight.TabIndex = 4;
            this.radioButtonRight.TabStop = true;
            this.radioButtonRight.Text = " ";
            this.radioButtonRight.UseVisualStyleBackColor = true;
            this.radioButtonRight.Click += new System.EventHandler(this.radioButtonRight_Clicked);
            // 
            // radioButtonLeft
            // 
            this.radioButtonLeft.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButtonLeft.Location = new System.Drawing.Point(8, 46);
            this.radioButtonLeft.Name = "radioButtonLeft";
            this.radioButtonLeft.Size = new System.Drawing.Size(16, 32);
            this.radioButtonLeft.TabIndex = 6;
            this.radioButtonLeft.TabStop = true;
            this.radioButtonLeft.Text = " ";
            this.radioButtonLeft.UseVisualStyleBackColor = true;
            this.radioButtonLeft.Click += new System.EventHandler(this.radioButtonLeft_Clicked);
            // 
            // buttonPassed
            // 
            this.buttonPassed.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonPassed.Location = new System.Drawing.Point(125, 113);
            this.buttonPassed.Name = "buttonPassed";
            this.buttonPassed.Size = new System.Drawing.Size(69, 23);
            this.buttonPassed.TabIndex = 1;
            this.buttonPassed.Text = "&Passed";
            this.buttonPassed.Click += new System.EventHandler(this.buttonPassed_Click);
            // 
            // buttonFailed
            // 
            this.buttonFailed.Location = new System.Drawing.Point(125, 140);
            this.buttonFailed.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.buttonFailed.Name = "buttonFailed";
            this.buttonFailed.Size = new System.Drawing.Size(69, 23);
            this.buttonFailed.TabIndex = 2;
            this.buttonFailed.Text = "&Failed";
            this.buttonFailed.Click += new System.EventHandler(this.buttonFailed_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(125, 12);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(69, 23);
            this.buttonConnect.TabIndex = 7;
            this.buttonConnect.Text = "&Connect";
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(125, 41);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(69, 23);
            this.buttonDisconnect.TabIndex = 8;
            this.buttonDisconnect.Text = "&Disconnect";
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // buttonSwap
            // 
            this.buttonSwap.Location = new System.Drawing.Point(125, 70);
            this.buttonSwap.Name = "buttonSwap";
            this.buttonSwap.Size = new System.Drawing.Size(69, 23);
            this.buttonSwap.TabIndex = 9;
            this.buttonSwap.Text = "&Swap";
            this.toolTips.SetToolTip(this.buttonSwap, "Swap between Straight/Right and Straight/Left");
            this.buttonSwap.Click += new System.EventHandler(this.buttonSwap_Click);
            // 
            // TestThreeWayTurnout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(206, 175);
            this.Controls.Add(this.buttonSwap);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.buttonPassed);
            this.Controls.Add(this.buttonFailed);
            this.Controls.Add(this.radioButtonLeft);
            this.Controls.Add(this.radioButtonRight);
            this.Controls.Add(this.radioButtonTop);
            this.Controls.Add(this.radioButtonBottom);
            this.Controls.Add(this.panelIllustration);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TestThreeWayTurnout";
            this.Text = "Test 3 Way Turnout";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TestThreeWayTurnout_FormClosed);
            this.Load += new System.EventHandler(this.TestThreeWayTurnout_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelIllustration;
        private System.Windows.Forms.RadioButton radioButtonBottom;
        private System.Windows.Forms.RadioButton radioButtonTop;
        private System.Windows.Forms.RadioButton radioButtonRight;
        private System.Windows.Forms.RadioButton radioButtonLeft;
        private System.Windows.Forms.Button buttonPassed;
        private System.Windows.Forms.Button buttonFailed;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Button buttonSwap;
        private System.Windows.Forms.ToolTip toolTips;
    }
}