using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Drop list box showing all providers in a given container (used to show lists
    /// of standard fonts, standard positions etc.)
    /// </summary>
    public class LayoutInfosComboBox : ComboBox {
		XmlElement	container = null;
		Type		infoType = null;

		public LayoutInfosComboBox()
		{
			this.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		public XmlElement InfoContainer {
			set {
				container = value;
				
				if(container != null)
					fillComboBox();
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
				if(value != null) {
					if(value.IsSubclassOf(typeof(LayoutInfo)))
						infoType = value;
					else
						throw new ArgumentException("Invalid type (not subclass of LayoutInfoType)");
				}
			}
		}

		public new LayoutInfo SelectedItem {
			set {
				foreach(LayoutInfo info in this.Items)
					if(info.Id == value.Id) {
						base.SelectedItem = info;
						break;
					}
			}

			get {
				return (LayoutInfo)base.SelectedItem;
			}
		}

		protected void fillComboBox() {
			foreach(XmlElement element in container) {
				LayoutInfo	info = (LayoutInfo)Activator.CreateInstance(infoType);

				info.Element = element;
				this.Items.Add(info);
			}
		}
	}

	/// <summary>
	/// Show values of an enumeration
	/// </summary>
	public class EnumComboBox : ComboBox {
		Type	enumType;

		public EnumComboBox() {
			this.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		public Type EnumType {
			set {
				this.enumType = value;

				String[]	names = Enum.GetNames(value);

				foreach(String n in names)
					Items.Add(n);
			}
		}

		public new int SelectedItem {
			set {
				int[]	values = (int[] )Enum.GetValues(enumType);

				for(int i = 0; i < values.Length; i++)
					if(values[i] == value) {
						base.SelectedIndex = i;
						break;
					}
			}

			get {
				return (int)Enum.Parse(enumType, (String)base.SelectedItem, false);
			}
		}
	}

	public class LayoutPositionInfoPreview : Control {
		LayoutDrawingSide			side = LayoutDrawingSide.Bottom;
		LayoutDrawingAnchorPoint	alignment = LayoutDrawingAnchorPoint.Center;
		int							distance = 0;
		Size						areaGridSize = new Size(32, 32);
		Size						previewGridSize = new Size(16, 16);
		Size						rectSize = new Size(30, 10);

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
				int		w = (value == 0) ? 30 : value * previewGridSize.Width / areaGridSize.Width;

				rectSize = new Size(w, 10);
				Invalidate();
			}
		}


		private void drawBackground(Graphics g) {
			int		x = 0;

			while(x < ClientSize.Width) {
				int		y = 0;

				while(y < ClientSize.Height) {
					if(Enabled)
						g.FillRectangle(Brushes.WhiteSmoke, x, y, previewGridSize.Width, previewGridSize.Height);
					else {
						using(Brush b = new SolidBrush(Parent.BackColor))
							g.FillRectangle(b, x, y, previewGridSize.Width, previewGridSize.Height);
					}

					g.DrawLine(Pens.DarkGray, x+previewGridSize.Width, y, x+previewGridSize.Width, y+previewGridSize.Height);
					y += previewGridSize.Height;

					g.DrawLine(Pens.DarkGray, x, y, x+previewGridSize.Width, y);
					y++;
				}

				x += previewGridSize.Width+1;
			}
		}

		private void drawFrame(Graphics g) {
			g.DrawRectangle(Pens.Black, 0, 0, ClientRectangle.Width-1, ClientRectangle.Height-1);
		}

		private Rectangle drawSampleComponent(Graphics g) {
			Point		ml = new Point((ClientSize.Width / 2) / (previewGridSize.Width+1), (ClientSize.Height / 2) / (previewGridSize.Height+1));
			Rectangle	r = new Rectangle(ml.X*previewGridSize.Width + ml.X-1, ml.Y*previewGridSize.Height + ml.Y-1, 
				previewGridSize.Width+1, previewGridSize.Height+1);

			using(Pen p = new Pen(Brushes.Black, 2)) {
				g.DrawRectangle(p, r);
			}

			return r;
		}

		private float getAlignedValue(float v, float d) {
			switch(alignment) {
				case LayoutDrawingAnchorPoint.Center:
					return v - (d / 2.0f);

				case LayoutDrawingAnchorPoint.Left:
					return v;

				case LayoutDrawingAnchorPoint.Right:
					return v - d;

				default:
					throw new ArgumentException("Invalid Anchor point value");
			}
		}

		private void drawPositionedRect(Graphics g, Rectangle rectComponent) {
			int		d = distance * previewGridSize.Width / areaGridSize.Width;
			RectangleF	rcRegion;
			PointF		origin = new PointF(rectComponent.Left + previewGridSize.Width/2, rectComponent.Top + previewGridSize.Height/2);

			switch(side) {
				case LayoutDrawingSide.Top:
					rcRegion = new RectangleF(
						new PointF(getAlignedValue(origin.X, rectSize.Width), 
						origin.Y - rectSize.Height - d), rectSize);
					break;

				case LayoutDrawingSide.Bottom:
					rcRegion = new RectangleF(
						new PointF(getAlignedValue(origin.X, rectSize.Width), 
						origin.Y + d), rectSize);
					break;

				case LayoutDrawingSide.Left:
					rcRegion = new RectangleF(
						new PointF(origin.X - rectSize.Width - d,
						getAlignedValue(origin.Y, rectSize.Height)), rectSize);
					break;

				case LayoutDrawingSide.Right:
					rcRegion = new RectangleF(new PointF(origin.X + d,
						getAlignedValue(origin.Y, rectSize.Height)), rectSize);
					break;

				case LayoutDrawingSide.Center:
					rcRegion = new RectangleF(new PointF(origin.X - rectSize.Width / 2, origin.Y - rectSize.Height / 2),
						rectSize);
					break;

				default:
					throw new ArgumentException("Invalid LayoutDrawingSide value");
			}

			using(Pen p = new Pen(Brushes.BlueViolet, 2)) {
				g.DrawRectangle(p, Rectangle.Ceiling(rcRegion));
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) {
		}

		protected override void OnPaint(PaintEventArgs e) {
			drawBackground(e.Graphics);
			drawFrame(e.Graphics);
			Rectangle rectSize = drawSampleComponent(e.Graphics);

			if(Enabled)
				drawPositionedRect(e.Graphics, rectSize);
		}
	}
}
