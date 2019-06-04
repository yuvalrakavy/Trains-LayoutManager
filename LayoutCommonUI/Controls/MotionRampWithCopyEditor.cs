using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for MotionRampWithCopyEditor.
    /// </summary>
    public class MotionRampWithCopyEditor : MotionRampEditor {
        private Button buttonUsrExistingRamp;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public MotionRampWithCopyEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonUsrExistingRamp = new Button();
            this.SuspendLayout();
            // 
            // buttonUsrExistingRamp
            // 
            this.buttonUsrExistingRamp.Location = new System.Drawing.Point(8, 99);
            this.buttonUsrExistingRamp.Name = "buttonUsrExistingRamp";
            this.buttonUsrExistingRamp.Size = new System.Drawing.Size(112, 22);
            this.buttonUsrExistingRamp.TabIndex = 1;
            this.buttonUsrExistingRamp.Text = "Use existing profile";
            this.buttonUsrExistingRamp.Click += this.buttonUsrExistingRamp_Click;
            // 
            // MotionRampWithCopyEditor
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonUsrExistingRamp});
            this.Name = "MotionRampWithCopyEditor";
            this.Size = new System.Drawing.Size(232, 128);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonUsrExistingRamp_Click(object sender, System.EventArgs e) {
            ContextMenu menu = new ContextMenu();

            foreach (MotionRampInfo ramp in LayoutModel.Instance.Ramps)
                menu.MenuItems.Add(new UseRampMenuItem(this, ramp));

            menu.Show(this, new Point(buttonUsrExistingRamp.Left, buttonUsrExistingRamp.Bottom));
        }

        private class UseRampMenuItem : MenuItem {
            private readonly MotionRampWithCopyEditor rampEditor;
            private readonly MotionRampInfo ramp;

            public UseRampMenuItem(MotionRampWithCopyEditor rampEditor, MotionRampInfo ramp) {
                this.rampEditor = rampEditor;
                this.ramp = ramp;

                Text = ramp.Description;
            }

            protected override void OnClick(EventArgs e) {
                rampEditor.CopyFrom(ramp);
            }
        }
    }
}
