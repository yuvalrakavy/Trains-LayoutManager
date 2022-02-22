using LayoutManager.Components;
using LayoutManager.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    public partial class TrainArrivesFrom : Form {
        public TrainArrivesFrom(LayoutBlockDefinitionComponent blockDefinition, XmlElement element) {
            LayoutComponentConnectionPoint from;

            InitializeComponent();

            if (element.HasAttribute("From"))
                from = LayoutComponentConnectionPoint.Parse(element.GetAttribute("From"));
            else
                from = blockDefinition.Track.ConnectionPoints[0];

            if (LayoutTrackComponent.IsHorizontal(from)) {
                radioButtonBottom.Visible = false;
                radioButtonTop.Visible = false;

                if (from == LayoutComponentConnectionPoint.L)
                    radioButtonLeft.Checked = true;
                else
                    radioButtonRight.Checked = true;
            }
            else {
                radioButtonLeft.Visible = false;
                radioButtonRight.Visible = false;

                if (from == LayoutComponentConnectionPoint.T)
                    radioButtonTop.Checked = true;
                else
                    radioButtonBottom.Checked = true;
            }

            UpdateDisplay();
        }

        private void UpdateDisplay() {
            string t = "UNKNOWN";

            switch (From) {
                case LayoutComponentConnectionPoint.T: t = "top"; break;
                case LayoutComponentConnectionPoint.B: t = "bottom"; break;
                case LayoutComponentConnectionPoint.L: t = "left"; break;
                case LayoutComponentConnectionPoint.R: t = "right"; break;
            }

            panelArrow.Invalidate();
            labelCondition.Text = "Train arrives from " + t;
        }

        public LayoutComponentConnectionPoint From {
            get {
                if (radioButtonRight.Checked)
                    return LayoutComponentConnectionPoint.R;
                else if (radioButtonLeft.Checked)
                    return LayoutComponentConnectionPoint.L;
                else if (radioButtonTop.Checked)
                    return LayoutComponentConnectionPoint.T;
                else return radioButtonBottom.Checked ? (LayoutComponentConnectionPoint)LayoutComponentConnectionPoint.B : (LayoutComponentConnectionPoint)LayoutComponentConnectionPoint.Empty;
            }
        }

        private void RadioButton_CheckedChanged(object? sender, EventArgs e) {
            UpdateDisplay();
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void PanelArrow_Paint(object? sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            LayoutComponentConnectionPoint from = From;

            g.TranslateTransform(32, 32);       // To middle

            switch (from) {
                case LayoutComponentConnectionPoint.L: g.RotateTransform(180); break;
                case LayoutComponentConnectionPoint.T: g.RotateTransform(-90); break;
                case LayoutComponentConnectionPoint.B: g.RotateTransform(90); break;
            }

            using Brush b = new LinearGradientBrush(new Point(-8, 0), new Point(30, 0), Color.DarkBlue, Color.Cyan);
            Point[] arrow = new Point[] {
                    new Point(-8, 0), new Point(0, 10), new Point(0, 4), new Point(30, 4),
                    new Point(30, -4), new Point(0, -4), new Point(0, -10) };

            g.FillPolygon(b, arrow);
            g.DrawPolygon(Pens.Black, arrow);
        }

        private void PanelArrow_MouseClick(object? sender, MouseEventArgs e) {
            if (LayoutTrackComponent.IsVertical(From)) {
                if (e.Y > 32)
                    radioButtonBottom.PerformClick();
                else
                    radioButtonTop.PerformClick();
            }
            else {
                if (e.X > 32)
                    radioButtonRight.PerformClick();
                else
                    radioButtonLeft.PerformClick();
            }
        }
    }
}
