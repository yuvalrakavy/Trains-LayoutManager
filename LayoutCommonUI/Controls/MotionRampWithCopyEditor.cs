using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for MotionRampWithCopyEditor.
    /// </summary>
    public partial class MotionRampWithCopyEditor : MotionRampEditor {

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


        private void ButtonUsrExistingRamp_Click(object? sender, EventArgs e) {
            var menu = new ContextMenuStrip();

            foreach (MotionRampInfo ramp in LayoutModel.Instance.Ramps)
                menu.Items.Add(new UseRampMenuItem(this, ramp));

            menu.Show(this, new Point(buttonUsrExistingRamp.Left, buttonUsrExistingRamp.Bottom));
        }

        private class UseRampMenuItem : LayoutMenuItem {
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
