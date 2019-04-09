using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for AdvancedBlockInfoProperties.
    /// </summary>
    public class AdvancedBlockInfoProperties : Form {
        private GroupBox groupBox1;
        private CheckBox checkBoxWaitable;
        private CheckBox checkBoxOverrideWaitable;
        private Button buttonCancel;
        private Button buttonOK;
        private CheckBox checkBoxSlowDownRegion;
        private GroupBox groupBox2;
        private Panel panelTripSectonBoundry;
        private CheckBox checkBoxFromRight;
        private CheckBox checkBoxFromLeft;
        private CheckBox checkBoxFromBottom;
        private CheckBox checkBoxFromTop;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void EndOfDesignerVariables() { }

        private readonly LayoutBlockDefinitionComponentInfo info;
        private readonly PlacementInfo placementInfo;

        public AdvancedBlockInfoProperties(LayoutBlockDefinitionComponentInfo info, PlacementInfo placementInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.info = info;
            this.placementInfo = placementInfo;

            checkBoxOverrideWaitable.Checked = !info.UseDefaultCanTrainWait;
            checkBoxWaitable.Enabled = checkBoxOverrideWaitable.Checked;
            if (checkBoxOverrideWaitable.Checked)
                checkBoxWaitable.Checked = info.CanTrainWait;
            checkBoxSlowDownRegion.Checked = info.IsSlowdownRegion;

            if (LayoutStraightTrackComponent.IsVertical(placementInfo.Track)) {
                checkBoxFromLeft.Visible = false;
                checkBoxFromRight.Visible = false;

                if (info.IsTripSectionBoundry(0)) {
                    if (placementInfo.Track.ConnectionPoints[0] == LayoutComponentConnectionPoint.T)
                        checkBoxFromTop.Checked = true;
                    else
                        checkBoxFromBottom.Checked = true;
                }

                if (info.IsTripSectionBoundry(1)) {
                    if (placementInfo.Track.ConnectionPoints[1] == LayoutComponentConnectionPoint.T)
                        checkBoxFromTop.Checked = true;
                    else
                        checkBoxFromBottom.Checked = true;
                }
            }
            else {
                checkBoxFromTop.Visible = false;
                checkBoxFromBottom.Visible = false;

                if (info.IsTripSectionBoundry(0)) {
                    if (placementInfo.Track.ConnectionPoints[0] == LayoutComponentConnectionPoint.L)
                        checkBoxFromLeft.Checked = true;
                    else
                        checkBoxFromRight.Checked = true;
                }

                if (info.IsTripSectionBoundry(1)) {
                    if (placementInfo.Track.ConnectionPoints[1] == LayoutComponentConnectionPoint.L)
                        checkBoxFromLeft.Checked = true;
                    else
                        checkBoxFromRight.Checked = true;
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new GroupBox();
            this.checkBoxWaitable = new CheckBox();
            this.checkBoxOverrideWaitable = new CheckBox();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.checkBoxSlowDownRegion = new CheckBox();
            this.groupBox2 = new GroupBox();
            this.checkBoxFromLeft = new CheckBox();
            this.checkBoxFromBottom = new CheckBox();
            this.checkBoxFromTop = new CheckBox();
            this.checkBoxFromRight = new CheckBox();
            this.panelTripSectonBoundry = new Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxWaitable);
            this.groupBox1.Controls.Add(this.checkBoxOverrideWaitable);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(251, 64);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Can train wait in this block:";
            // 
            // checkBoxWaitable
            // 
            this.checkBoxWaitable.Location = new System.Drawing.Point(24, 36);
            this.checkBoxWaitable.Name = "checkBoxWaitable";
            this.checkBoxWaitable.Size = new System.Drawing.Size(200, 24);
            this.checkBoxWaitable.TabIndex = 1;
            this.checkBoxWaitable.Text = "Train is allowed to wait in this block";
            // 
            // checkBoxOverrideWaitable
            // 
            this.checkBoxOverrideWaitable.Location = new System.Drawing.Point(7, 16);
            this.checkBoxOverrideWaitable.Name = "checkBoxOverrideWaitable";
            this.checkBoxOverrideWaitable.Size = new System.Drawing.Size(225, 24);
            this.checkBoxOverrideWaitable.TabIndex = 0;
            this.checkBoxOverrideWaitable.Text = "Override default value";
            this.checkBoxOverrideWaitable.CheckedChanged += new System.EventHandler(this.checkBoxOverrideWaitable_CheckedChanged);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(231, 275);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(151, 275);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // checkBoxSlowDownRegion
            // 
            this.checkBoxSlowDownRegion.Location = new System.Drawing.Point(8, 222);
            this.checkBoxSlowDownRegion.Name = "checkBoxSlowDownRegion";
            this.checkBoxSlowDownRegion.Size = new System.Drawing.Size(248, 16);
            this.checkBoxSlowDownRegion.TabIndex = 6;
            this.checkBoxSlowDownRegion.Text = "Slow down in this block before stopping";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxFromLeft);
            this.groupBox2.Controls.Add(this.checkBoxFromBottom);
            this.groupBox2.Controls.Add(this.checkBoxFromTop);
            this.groupBox2.Controls.Add(this.checkBoxFromRight);
            this.groupBox2.Controls.Add(this.panelTripSectonBoundry);
            this.groupBox2.Location = new System.Drawing.Point(8, 78);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(251, 138);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trip section boundry for trains coming from:";
            // 
            // checkBoxFromLeft
            // 
            this.checkBoxFromLeft.AutoSize = true;
            this.checkBoxFromLeft.Location = new System.Drawing.Point(69, 66);
            this.checkBoxFromLeft.Name = "checkBoxFromLeft";
            this.checkBoxFromLeft.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFromLeft.TabIndex = 4;
            // 
            // checkBoxFromBottom
            // 
            this.checkBoxFromBottom.AutoSize = true;
            this.checkBoxFromBottom.Location = new System.Drawing.Point(117, 114);
            this.checkBoxFromBottom.Name = "checkBoxFromBottom";
            this.checkBoxFromBottom.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFromBottom.TabIndex = 3;
            // 
            // checkBoxFromTop
            // 
            this.checkBoxFromTop.AutoSize = true;
            this.checkBoxFromTop.Location = new System.Drawing.Point(117, 19);
            this.checkBoxFromTop.Name = "checkBoxFromTop";
            this.checkBoxFromTop.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFromTop.TabIndex = 2;
            // 
            // checkBoxFromRight
            // 
            this.checkBoxFromRight.AutoSize = true;
            this.checkBoxFromRight.Location = new System.Drawing.Point(166, 66);
            this.checkBoxFromRight.Name = "checkBoxFromRight";
            this.checkBoxFromRight.Size = new System.Drawing.Size(15, 14);
            this.checkBoxFromRight.TabIndex = 1;
            // 
            // panelTripSectonBoundry
            // 
            this.panelTripSectonBoundry.Location = new System.Drawing.Point(89, 38);
            this.panelTripSectonBoundry.Name = "panelTripSectonBoundry";
            this.panelTripSectonBoundry.Size = new System.Drawing.Size(70, 70);
            this.panelTripSectonBoundry.TabIndex = 0;
            this.panelTripSectonBoundry.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTripSectonBoundry_Paint);
            // 
            // AdvancedBlockInfoProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(319, 307);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.checkBoxSlowDownRegion);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AdvancedBlockInfoProperties";
            this.Text = "Advanced Block Properties";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            info.UseDefaultCanTrainWait = !checkBoxOverrideWaitable.Checked;
            if (!info.UseDefaultCanTrainWait)
                info.CanTrainWait = checkBoxWaitable.Checked;

            info.IsSlowdownRegion = checkBoxSlowDownRegion.Checked;

            switch (placementInfo.Track.ConnectionPoints[0]) {
                case LayoutComponentConnectionPoint.T: info.SetTripSectionBoundry(0, checkBoxFromTop.Checked); break;
                case LayoutComponentConnectionPoint.B: info.SetTripSectionBoundry(0, checkBoxFromBottom.Checked); break;
                case LayoutComponentConnectionPoint.L: info.SetTripSectionBoundry(0, checkBoxFromLeft.Checked); break;
                case LayoutComponentConnectionPoint.R: info.SetTripSectionBoundry(0, checkBoxFromRight.Checked); break;
            }

            switch (placementInfo.Track.ConnectionPoints[1]) {
                case LayoutComponentConnectionPoint.T: info.SetTripSectionBoundry(1, checkBoxFromTop.Checked); break;
                case LayoutComponentConnectionPoint.B: info.SetTripSectionBoundry(1, checkBoxFromBottom.Checked); break;
                case LayoutComponentConnectionPoint.L: info.SetTripSectionBoundry(1, checkBoxFromLeft.Checked); break;
                case LayoutComponentConnectionPoint.R: info.SetTripSectionBoundry(1, checkBoxFromRight.Checked); break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void checkBoxOverrideWaitable_CheckedChanged(object sender, System.EventArgs e) {
            checkBoxWaitable.Enabled = checkBoxOverrideWaitable.Checked;
        }

        private void panelTripSectonBoundry_Paint(object sender, PaintEventArgs e) {
            e.Graphics.FillRectangle(Brushes.White, 3, 3, 64, 64);
            e.Graphics.DrawRectangle(Pens.Black, 3, 3, 64, 64);
            e.Graphics.TranslateTransform(3, 3);

            LayoutStraightTrackPainter painter = new LayoutStraightTrackPainter(new Size(64, 64), placementInfo.Track.ConnectionPoints);

            painter.Paint(e.Graphics);
        }
    }
}
