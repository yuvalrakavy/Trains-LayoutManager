using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for TestLocoPainter.
    /// </summary>
    public partial class TestLocoPainter : Form {
        private readonly View.LocomotivePainter locoPainter = new();

        public TestLocoPainter() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            radioButtonDirectionForward.Checked = true;
            radioButtonFrontR.Checked = true;
            radioButtonOrientationForward.Checked = true;
        }

        private RadioButton? FindRadioButton(Control where, string name) {
            if (where.Controls.Count == 0)
                return null;

            foreach (Control c in where.Controls) {
                if (c is RadioButton button && c.Name == name)
                    return button;

                var result = FindRadioButton(c, name);
                if (result != null)
                    return (RadioButton)result;
            }

            return null;
        }

        private object? GetRadio(String genericName, Type enumType) {
            String[] valueNames = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);

            for (int i = 0; i < valueNames.Length; i++) {
                var r = FindRadioButton(this, genericName + valueNames[i]);

                if (r != null && r.Checked)
                    return values.GetValue(i);
            }

            throw new ApplicationException("No checked radio control found in " + genericName);
        }

        protected void UpdateLocoPainter(object? sender, EventArgs e) {
            locoPainter.DrawFront = true;
            locoPainter.Front = (LayoutComponentConnectionPoint?)GetRadio("radioButtonFront", typeof(LayoutComponentConnectionPoint)) ?? LayoutComponentConnectionPoint.Empty;
            locoPainter.Orientation = (LocomotiveOrientation?)GetRadio("radioButtonOrientation", typeof(LocomotiveOrientation)) ?? LocomotiveOrientation.Unknown;
            locoPainter.Speed = (int)numericUpDownSpeed.Value;
            locoPainter.Label = textBoxLabel.Text;

            panel.Invalidate();
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

        private void Panel_Paint(object? sender, PaintEventArgs e) {
            e.Graphics.TranslateTransform(panel.Width / 2, panel.Height / 2);
            locoPainter.Draw(e.Graphics);
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            this.Close();
        }
    }
}
