using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using LayoutManager;
using LayoutManager.Model;

using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LayoutManager.View {
	public interface IPopupWindowSection {

		/// <summary>
		/// Get the size of this section
		/// </summary>
		/// <param name="g">Graphics with parameters that will be used to paint the section</param>
		/// <returns>The section size in pixels</returns>
		Size GetSize(Graphics g);

		/// <summary>
		/// Paint the section
		/// </summary>
		/// <param name="g">Graphics to use for painting to section (0, 0) is section top left corner</param>
		void Paint(Graphics g);
	}

	public class PopupWindowContainerSection : IPopupWindowSection {

		#region SectionEntry definition

		struct SectionEntry {
			IPopupWindowSection section;
			Point origin;

			public SectionEntry(Point origin, IPopupWindowSection section) {
				this.origin = origin;
				this.section = section;
			}

            public Size GetSize(Graphics g) => section.GetSize(g);

            public void Paint(Graphics g) {
				GraphicsState gs = g.Save();

				g.TranslateTransform(origin.X, origin.Y);
				section.Paint(g);
				g.Restore(gs);
			}

            public Point Origin => origin;
        };

		#endregion

		List<SectionEntry> sectionEntries = new List<SectionEntry>();
		Point insertionPoint = new Point(0, 0);
		int verticalHeight = 0;
		Size innerMargins = new Size(0, 0);
		Size outerMargins = new Size(3, 3);
		Pen borderPen;

		public PopupWindowContainerSection() {
			this.Parent = null;
		}

		public PopupWindowContainerSection(Control parent) {
			this.Parent = parent;
		}

		#region Properties

		/// <summary>
		/// The parent control on which this popup window will be displayed
		/// </summary>
		public Control Parent { get; private set; }

        /// <summary>
        ///  Number of sections in the container
        /// </summary>
        public int Count => sectionEntries.Count;

        /// <summary>
        /// Margins around the sections in the container, this margin is "before" an optional container 
        /// border line
        /// </summary>
        public Size OuterMargins {
			get {
				return outerMargins;
			}

			set {
				outerMargins = value;
			}
		}

		/// <summary>
		/// Margins around the sections in the container, this margin is inside an optional container
		/// border line
		/// </summary>
		public Size InnerMargins {
			get {
				return innerMargins;
			}

			set {
				innerMargins = value;
			}
		}

		/// <summary>
		/// Return the container size
		/// </summary>
		public Size Size {
			get {
				using(Graphics g = Parent.CreateGraphics())
					return GetSize(g);
			}
		}

		/// <summary>
		/// Pen used to draw an optional border around the sections contained in this container
		/// </summary>
		public Pen BorderPen {
			get {
				return borderPen;
			}

			set {
				borderPen = value;
			}
		}

		#endregion

		#region Operations

		/// <summary>
		/// Add section to the container. The section is added below the previous sections
		/// </summary>
		/// <param name="section">The section to add</param>
		public void AddVerticalSection(Control parent, IPopupWindowSection section) {
			Size sectionSize;

			using(Graphics g = parent.CreateGraphics())
				sectionSize = section.GetSize(g);

			insertionPoint = new Point(0, insertionPoint.Y + verticalHeight);
			verticalHeight = 0;

			sectionEntries.Add(new SectionEntry(insertionPoint, section));
			insertionPoint = new Point(0, insertionPoint.Y + sectionSize.Height);
		}

		public void AddVerticalSection(IPopupWindowSection section) {
			AddVerticalSection(this.Parent, section);
		}

		/// <summary>
		/// Add section to the container. The section is added to the right of the previouslly added sections
		/// </summary>
		/// <param name="section">The section to add</param>
		public void AddHorizontalSection(Control parent, IPopupWindowSection section) {
			sectionEntries.Add(new SectionEntry(insertionPoint, section));

			Size sectionSize;

			using(Graphics g = parent.CreateGraphics())
				sectionSize = section.GetSize(g);

			insertionPoint = new Point(insertionPoint.X + sectionSize.Width, insertionPoint.Y);
			if(sectionSize.Height > verticalHeight)
				verticalHeight = sectionSize.Height;
		}

		public void AddHorizontalSection(IPopupWindowSection section) {
			AddHorizontalSection(this.Parent, section);
		}

		public void AddText(Control parent, string text) {
			AddVerticalSection(parent, new PopupWindowTextSection(text));
		}

		/// <summary>
		/// Add text section (shortcut for AddVertcialSection(new DetailsPopupTextSection(text)))
		/// </summary>
		/// <param name="text">The text to add</param>
		public void AddText(string text) {
			AddVerticalSection(new PopupWindowTextSection(text));
		}

        /// <summary>
        /// Create a new container section for the same view as the current container
        /// </summary>
        /// <returns>A new container section</returns>
        public PopupWindowContainerSection CreateContainer() => new PopupWindowContainerSection(this.Parent);

        #endregion

        #region IDetailsPopWindowSection Members

        public Size GetSize(Graphics g) {
			Size totalSize = new Size(0, 0);

			foreach(SectionEntry entry in sectionEntries) {
				Size sectionSize = entry.GetSize(g);
				Size s = new Size(entry.Origin.X + sectionSize.Width, entry.Origin.Y + sectionSize.Height);

				if(s.Width > totalSize.Width)
					totalSize.Width = s.Width;
				if(s.Height > totalSize.Height)
					totalSize.Height = s.Height;
			}

			return new Size(totalSize.Width + 2 * innerMargins.Width + 2 * outerMargins.Width, totalSize.Height + 2 * innerMargins.Height + 2 * outerMargins.Height);
		}

		public void Paint(Graphics g) {
			GraphicsState gs = g.Save();

			Size mySize = GetSize(g);

			if(borderPen != null)
				g.DrawRectangle(borderPen, outerMargins.Width, outerMargins.Height, mySize.Width - 2 * outerMargins.Width, mySize.Height - 2 * outerMargins.Height);

			g.TranslateTransform(outerMargins.Width + innerMargins.Width, outerMargins.Height + innerMargins.Height);

			foreach(SectionEntry entry in sectionEntries)
				entry.Paint(g);

			g.Restore(gs);
		}

		#endregion
	}

	public class PopupWindowTextSection : IPopupWindowSection, IDisposable {
		string text;
		Font font;
		Brush brush;

		public PopupWindowTextSection() {
		}

		public PopupWindowTextSection(string text) {
			this.text = text;
		}

		public PopupWindowTextSection(Font font, string text) {
			this.font = font;
			this.text = text;
		}

		#region Properties

		public string Text {
			get {
				return text;
			}

			set {
				text = value;
			}
		}

		public Font Font {
			get {
				return font;
			}

			set {
				font = value;
			}
		}

		public Brush Brush {
			get {
				return brush ?? Brushes.Black;
			}

			set {
				brush = value;
			}
		}

		#endregion

		#region IDetailsPopWindowSection Members

		public Size GetSize(Graphics g) {
			if(font == null)
				font = new Font("Arial", 8);

			return g.MeasureString(text, font).ToSize();
		}

		public void Paint(Graphics g) {
			g.DrawString(text, font, this.Brush, new Point(0, 0));
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			if(font != null)
				font.Dispose();
			if(brush != null)
				brush.Dispose();
		}

		#endregion
	}

	public class PopupWindowImageSection : IPopupWindowSection {
		Image image;
		Size size = Size.Empty;

		public PopupWindowImageSection(Image image) {
			this.image = image;
		}

		public PopupWindowImageSection(Image image, Size size) {
			this.image = image;
			this.size = size;
		}

		#region IDetailsPopWindowSection Members

		public Size GetSize(Graphics g) {
			if(size == Size.Empty)
				return image.Size;
			else
				return size;
		}

		public void Paint(Graphics g) {
			if(size == Size.Empty)
				g.DrawImage(image, new Point(0, 0));
			else
				g.DrawImage(image, new Rectangle(new Point(0, 0), size));
		}

		#endregion
	}

	public class PopupWindowAttributesSection : PopupWindowTextSection {

		public PopupWindowAttributesSection(string prefix, IObjectHasAttributes objectWithAttributes) {
			string list = "";

			foreach(AttributeInfo a in objectWithAttributes.Attributes) {
				if(list.Length > 0)
					list += ", ";

				list += a.Name + "=" + a.ValueAsString;
			}

			Text = prefix + " " + list;
		}
	}

	public class PopupWindowPoliciesSection : PopupWindowTextSection {

		public PopupWindowPoliciesSection(string prefix, LayoutPolicyIdCollection policyIds, LayoutPoliciesCollection policies) {
			string list = "";

			foreach(Guid policyId in policyIds) {
				LayoutPolicyInfo policy = policies[policyId];

				if(policy != null) {
					if(list.Length > 0)
						list += ", ";

					list += policy.Name;
				}
			}

			Text = prefix + " " + list;
		}
	}

	public class PopupWindowViewZoomSection : IPopupWindowSection {
		LayoutView view;
		Point ml;
		Size drawingSize;

		public PopupWindowViewZoomSection(LayoutView view, Point ml, Size drawingSize) {
			this.view = view;
			this.ml = ml;
			this.drawingSize = drawingSize;
		}

        #region IDetailsPopWindowSection Members

        public Size GetSize(Graphics g) => drawingSize;

        public void Paint(Graphics g) {
			Rectangle drawingRect = new Rectangle(new Point(0, 0), drawingSize);

			view.Draw(g, drawingRect, new Rectangle(ml, new Size(1, 1)), 0, false);
			g.DrawRectangle(Pens.Black, drawingRect);
		}

		#endregion
	}

}
