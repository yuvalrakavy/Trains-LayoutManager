using System;
using System.Drawing;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    public partial class TrainLengthDiagram : UserControl {
        private TrainLength _trainLength = TrainLength.Standard;
        private TrainLengthComparison _comparison = TrainLengthComparison.None;

        private readonly int[] divMarks = new int[] {
            36, 37+27, 37+(2*27), 37+(3*27), 37+(4*27)
        };

        public TrainLengthDiagram() {
            InitializeComponent();

            buttonLonger.Visible = false;
            buttonNotLonger.Visible = false;
            FillMenu();

            linkMenuTrainLength.SelectedIndex = (int)TrainLength.Standard;
        }

        public event EventHandler TrainLengthChanged;
        public event EventHandler ComparisonChanged;

        public TrainLength Length {
            get {
                return _trainLength;
            }

            set {
                if (_trainLength != value) {
                    _trainLength = value;
                    Invalidate();

                    linkMenuTrainLength.SelectedIndex = (int)value;

                    TrainLengthChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public TrainLengthComparison Comparison {
            get {
                return _comparison;
            }

            set {
                if (_comparison != value) {
                    _comparison = value;

                    buttonNotLonger.Visible = value != TrainLengthComparison.None;
                    buttonLonger.Visible = value != TrainLengthComparison.None;

                    FillMenu();
                    linkMenuTrainLength.SelectedIndex = (int)_trainLength;

                    ComparisonChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void FillMenu() {
            if (Comparison == TrainLengthComparison.None)
                FillMenu(null);
            else if (Comparison == TrainLengthComparison.NotLonger)
                FillMenu("Not longer than");
            else
                FillMenu("Longer than");
        }

        private void FillMenu(string p) {
            string[] lengthEntries = new string[] {
                "Locomotive only", "Very short", "Short", "Standard", "Long", "Very long"
            };
            string[] entries = lengthEntries;

            if (p != null) {
                entries = new string[lengthEntries.Length];

                for (int i = 0; i < lengthEntries.Length; i++)
                    entries[i] = p + " " + lengthEntries[i].Substring(0, 1).ToLower() + lengthEntries[i].Substring(1);
            }

            linkMenuTrainLength.Options = entries;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            PaintTrain(g, 8);
            PaintDividers(g);
        }

        private void PaintTrain(Graphics g, int horizontalMargin) {
            int yTrain = 8;
            int xTrain = horizontalMargin;
            Rectangle loco = new Rectangle(xTrain, yTrain, 27, 16);
            int trainLength = 0;

            Brush locoBrush = Enabled ? Brushes.DarkGray : Brushes.LightGray;

            g.FillRectangle(locoBrush, loco);
            g.DrawRectangle(Pens.Black, loco);

            if (Length > TrainLength.LocomotiveOnly) {
                xTrain += loco.Width + 2;

                switch ((KnownTrainLength)Length) {
                    case KnownTrainLength.VeryShort: trainLength = 27; break;
                    case KnownTrainLength.Short: trainLength = 27 * 2; break;
                    case KnownTrainLength.Standard: trainLength = 27 * 3; break;
                    case KnownTrainLength.Long: trainLength = 27 * 4; break;
                    case KnownTrainLength.VeryLong: trainLength = 27 * 5; break;
                }

                Rectangle cars = new Rectangle(xTrain, yTrain, trainLength, 16);

                Brush carsBrush = Enabled ? Brushes.Wheat : Brushes.LightGray;

                g.FillRectangle(carsBrush, cars);
                g.DrawRectangle(Pens.Black, cars);
            }
        }

        private void PaintDividers(Graphics g) {
            using Pen p = new Pen(Color.Black) {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
            };

            foreach (int xDivider in divMarks)
                g.DrawLine(p, xDivider, 4, xDivider, 28);
        }

        private void buttonNotLonger_Click(object sender, EventArgs e) {
            Comparison = TrainLengthComparison.NotLonger;
        }

        private void buttonLonger_Click(object sender, EventArgs e) {
            Comparison = TrainLengthComparison.Longer;
        }

        private void linkMenuTrainLength_ValueChanged(object sender, EventArgs e) {
            Length = new TrainLength(linkMenuTrainLength.SelectedIndex);
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);

            if (e.Y < 32) {
                TrainLength l = TrainLength.VeryLong;

                for (int i = 0; i < divMarks.Length; i++)
                    if (e.X < divMarks[i]) {
                        l = new TrainLength(i);
                        break;
                    }

                Length = l;
            }
        }
    }
}
