using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Drop list box showing all providers in a given container (used to show lists
    /// of standard fonts, standard positions etc.)
    /// </summary>
    public class LayoutInfosComboBox : ComboBox {
        private XmlElement? container = null;
        private Type? infoType = null;

        public LayoutInfosComboBox() {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public XmlElement? InfoContainer {
            set {
                container = value;

                if (container != null)
                    FillComboBox();
            }

            get {
                return container;
            }
        }

        public Type InfoType {
            get {
                return InfoType;
            }

            set {
                if (value != null) {
                    if (value.IsSubclassOf(typeof(LayoutInfo)))
                        infoType = value;
                    else
                        throw new ArgumentException("Invalid type (not subclass of LayoutInfoType)");
                }
            }
        }

        public new LayoutInfo SelectedItem {
            set {
                foreach (LayoutInfo info in this.Items)
                    if (info.Id == value.Id) {
                        base.SelectedItem = info;
                        break;
                    }
            }

            get {
                return (LayoutInfo)base.SelectedItem;
            }
        }

        protected void FillComboBox() {
            if (container != null && infoType != null) {
                foreach (XmlElement element in container) {
                    var info = (LayoutInfo?)Activator.CreateInstance(infoType);

                    if (info != null) {
                        info.Element = element;
                        this.Items.Add(info);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Show values of an enumeration
    /// </summary>
    public class EnumComboBox : ComboBox {
        private Type? enumType;

        public EnumComboBox() {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public Type EnumType {
            set {
                this.enumType = value;

                String[] names = Enum.GetNames(value);

                foreach (String n in names)
                    Items.Add(n);
            }
        }

        public new int SelectedItem {
            set {
                int[] values = (int[])Enum.GetValues(Ensure.NotNull<Type>(enumType));

                for (int i = 0; i < values.Length; i++)
                    if (values[i] == value) {
                        base.SelectedIndex = i;
                        break;
                    }
            }

            get {
                return (int)Enum.Parse(Ensure.NotNull<Type>(enumType), (String)base.SelectedItem, false);
            }
        }
    }

    public class LayoutPositionInfoPreview : Control {
        private LayoutDrawingSide side = LayoutDrawingSide.Bottom;
        private LayoutDrawingAnchorPoint alignment = LayoutDrawingAnchorPoint.Center;
        private int distance = 0;
        private Size areaGridSize = new(32, 32);
        private Size previewGridSize = new(16, 16);
        private Size rectSize = new(30, 10);

        public LayoutDrawingSide Side {
            set {
                side = value;
                Invalidate();
            }

            get {
                return side;
            }
        }

        public LayoutDrawingAnchorPoint Alignment {
            set {
                alignment = value;
                Invalidate();
            }

            get {
                return alignment;
            }
        }

        public int Distance {
            set {
                distance = value;
                Invalidate();
            }

            get {
                return distance;
            }
        }

        public int LayoutWidth {
            set {
                int w = (value == 0) ? 30 : value * previewGridSize.Width / areaGridSize.Width;

                rectSize = new Size(w, 10);
                Invalidate();
            }
        }

        private void DrawBackground(Graphics g) {
            int x = 0;

            while (x < ClientSize.Width) {
                int y = 0;

                while (y < ClientSize.Height) {
                    if (Enabled)
                        g.FillRectangle(Brushes.WhiteSmoke, x, y, previewGridSize.Width, previewGridSize.Height);
                    else {
                        using Brush b = new SolidBrush(Parent.BackColor);
                        g.FillRectangle(b, x, y, previewGridSize.Width, previewGridSize.Height);
                    }

                    g.DrawLine(Pens.DarkGray, x + previewGridSize.Width, y, x + previewGridSize.Width, y + previewGridSize.Height);
                    y += previewGridSize.Height;

                    g.DrawLine(Pens.DarkGray, x, y, x + previewGridSize.Width, y);
                    y++;
                }

                x += previewGridSize.Width + 1;
            }
        }

        private void DrawFrame(Graphics g) {
            g.DrawRectangle(Pens.Black, 0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
        }

        private Rectangle DrawSampleComponent(Graphics g) {
            Point ml = new(ClientSize.Width / 2 / (previewGridSize.Width + 1), ClientSize.Height / 2 / (previewGridSize.Height + 1));
            Rectangle r = new((ml.X * previewGridSize.Width) + ml.X - 1, (ml.Y * previewGridSize.Height) + ml.Y - 1,
                previewGridSize.Width + 1, previewGridSize.Height + 1);

            using (Pen p = new(Brushes.Black, 2)) {
                g.DrawRectangle(p, r);
            }

            return r;
        }

        private float GetAlignedValue(float v, float d) => alignment switch {
            LayoutDrawingAnchorPoint.Center => v - (d / 2.0f),
            LayoutDrawingAnchorPoint.Left => v,
            LayoutDrawingAnchorPoint.Right => v - d,
            _ => throw new ArgumentException("Invalid Anchor point value"),
        };

        private void DrawPositionedRect(Graphics g, Rectangle rectComponent) {
            int d = distance * previewGridSize.Width / areaGridSize.Width;
            PointF origin = new(rectComponent.Left + (previewGridSize.Width / 2), rectComponent.Top + (previewGridSize.Height / 2));
            var rcRegion = side switch {
                LayoutDrawingSide.Top => new RectangleF(
                                        new PointF(GetAlignedValue(origin.X, rectSize.Width),
                                        origin.Y - rectSize.Height - d), rectSize),
                LayoutDrawingSide.Bottom => new RectangleF(new PointF(GetAlignedValue(origin.X, rectSize.Width), origin.Y + d), rectSize),
                LayoutDrawingSide.Left => new RectangleF(new PointF(origin.X - rectSize.Width - d, GetAlignedValue(origin.Y, rectSize.Height)), rectSize),
                LayoutDrawingSide.Right => new RectangleF(new PointF(origin.X + d,GetAlignedValue(origin.Y, rectSize.Height)), rectSize),
                LayoutDrawingSide.Center => new RectangleF(new PointF(origin.X - (rectSize.Width / 2), origin.Y - (rectSize.Height / 2)),rectSize),
                _ => throw new ArgumentException("Invalid LayoutDrawingSide value"),
            };
            using Pen p = new(Brushes.BlueViolet, 2);
            g.DrawRectangle(p, Rectangle.Ceiling(rcRegion));
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
        }

        protected override void OnPaint(PaintEventArgs e) {
            DrawBackground(e.Graphics);
            DrawFrame(e.Graphics);
            Rectangle rectSize = DrawSampleComponent(e.Graphics);

            if (Enabled)
                DrawPositionedRect(e.Graphics, rectSize);
        }
    }
}
