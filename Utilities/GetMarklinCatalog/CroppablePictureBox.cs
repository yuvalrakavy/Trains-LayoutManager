using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

namespace GetMarklinCatalog
{
	/// <summary>
	/// Summary description for CroppablePictureBox.
	/// </summary>
	public class CroppablePictureBox : PictureBox
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		Rectangle	croppingRectangle = new Rectangle(0, 0, 100, 100);
		Cursor		saveCursor = null;
		MovedHandle	movedHandle = MovedHandle.None;
		Point		moveReferecePoint;
		Rectangle	movedCroppingRectangle;
		bool		eraseCroppingRectangle = false;

		[Flags]
		enum MovedHandle {
			None = 0,
			Right = 0x0001,
			Left  = 0x0002,
			Top   = 0x0004,
			Bottom= 0x0008,
			Panning=0x0010,
		}

		public event EventHandler CroppingRectangleChanged = null;

		public CroppablePictureBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		public Rectangle CroppingRectangle {
			get {
				return croppingRectangle;
			}

			set {
				croppingRectangle = value;
				Invalidate();
			}
		}

		private void drawSizeHandle(Graphics g, int x, int y) {
			g.FillRectangle(Brushes.Black, new Rectangle(x-2, y-2, 4, 4));
		}

		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);

			if(Image != null && !eraseCroppingRectangle) {
				Bitmap	bm = (Bitmap)Image;

				Rectangle	outside = croppingRectangle;

				e.Graphics.DrawRectangle(Pens.Black, croppingRectangle);
				drawSizeHandle(e.Graphics, croppingRectangle.Left, croppingRectangle.Top);
				drawSizeHandle(e.Graphics, croppingRectangle.Left, croppingRectangle.Bottom);
				drawSizeHandle(e.Graphics, croppingRectangle.Right, croppingRectangle.Top);
				drawSizeHandle(e.Graphics, croppingRectangle.Right, croppingRectangle.Bottom);

				drawSizeHandle(e.Graphics, croppingRectangle.Left, (croppingRectangle.Bottom + croppingRectangle.Top) / 2);
				drawSizeHandle(e.Graphics, (croppingRectangle.Right + croppingRectangle.Left) / 2, croppingRectangle.Top);
				drawSizeHandle(e.Graphics, croppingRectangle.Right, (croppingRectangle.Bottom + croppingRectangle.Top) / 2);
				drawSizeHandle(e.Graphics, (croppingRectangle.Right + croppingRectangle.Left) / 2, croppingRectangle.Bottom);
			}
		}

		bool IsOnHandle(Point pt, MovedHandle h) {

			if(h == MovedHandle.Panning) {
				Rectangle	inside = croppingRectangle;

				inside.Inflate(-4, -4);
				if(inside.Contains(pt) && h == MovedHandle.Panning)
					return true;
			}

			if((h & MovedHandle.Left) != 0) {
				if(Math.Abs(pt.X - croppingRectangle.Left) > 2)
					return false;

				if((h & MovedHandle.Top) != 0 && Math.Abs(pt.Y - croppingRectangle.Top) <= 2)
					return true;
				else if((h & MovedHandle.Bottom) != 0 && Math.Abs(pt.Y - croppingRectangle.Bottom) <= 2)
					return true;
				else if(Math.Abs(pt.Y - (croppingRectangle.Bottom + croppingRectangle.Top) / 2) <= 2)
					return true;

				return false;
			}

			if((h & MovedHandle.Right) != 0) {
				if(Math.Abs(pt.X - croppingRectangle.Right) > 2)
					return false;

				if((h & MovedHandle.Top) != 0 && Math.Abs(pt.Y - croppingRectangle.Top) <= 2)
					return true;
				else if((h & MovedHandle.Bottom) != 0 && Math.Abs(pt.Y - croppingRectangle.Bottom) <= 2)
					return true;
				else if(Math.Abs(pt.Y - (croppingRectangle.Bottom + croppingRectangle.Top) / 2) <= 2)
					return true;
			}

			if((h & MovedHandle.Top) != 0) {
				if(Math.Abs(pt.Y - croppingRectangle.Top) > 2)
					return false;

				if(Math.Abs(pt.X - (croppingRectangle.Right + croppingRectangle.Left) / 2) <= 2)
					return true;
			}


			if((h & MovedHandle.Bottom) != 0) {
				if(Math.Abs(pt.Y - croppingRectangle.Bottom) > 2)
					return false;

				if(Math.Abs(pt.X - (croppingRectangle.Right + croppingRectangle.Left) / 2) <= 2)
					return true;
			}

			return false;
		}

		MovedHandle HitTest(Point pt) {
			if(IsOnHandle(pt, MovedHandle.Bottom))
				return MovedHandle.Bottom;
			if(IsOnHandle(pt, MovedHandle.Top))
				return MovedHandle.Top;
			if(IsOnHandle(pt, MovedHandle.Left))
				return MovedHandle.Left;
			if(IsOnHandle(pt, MovedHandle.Right))
				return MovedHandle.Right;

			if(IsOnHandle(pt, (MovedHandle.Bottom|MovedHandle.Left)))
				return MovedHandle.Bottom|MovedHandle.Left;
			if(IsOnHandle(pt, MovedHandle.Top|MovedHandle.Left))
				return MovedHandle.Top|MovedHandle.Left;
			if(IsOnHandle(pt, MovedHandle.Right|MovedHandle.Top))
				return MovedHandle.Right|MovedHandle.Top;
			if(IsOnHandle(pt, MovedHandle.Right|MovedHandle.Bottom))
				return MovedHandle.Right|MovedHandle.Bottom;

			if(IsOnHandle(pt, MovedHandle.Panning))
				return MovedHandle.Panning;

			return MovedHandle.None;
		}

		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);

			saveCursor = Cursor.Current;
		}

		protected override void OnMouseLeave(EventArgs e) {
			Cursor.Current = saveCursor;

			base.OnMouseLeave(e);
		}

		private void invalidateCroppingRectangle() {
			Invalidate(new Rectangle(croppingRectangle.Left - 3, croppingRectangle.Top - 3, croppingRectangle.Width + 6, 6));
			Invalidate(new Rectangle(croppingRectangle.Left - 3, croppingRectangle.Bottom - 3, croppingRectangle.Width + 6, 6));
			Invalidate(new Rectangle(croppingRectangle.Left - 3, croppingRectangle.Top - 3, 6, croppingRectangle.Height + 6));
			Invalidate(new Rectangle(croppingRectangle.Right- 3, croppingRectangle.Top - 3, 6, croppingRectangle.Height + 6));
		}


		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);

			MovedHandle	h;

			if(movedHandle != MovedHandle.None)
				h = movedHandle;
			else
				h = HitTest(new Point(e.X, e.Y));

			switch(h) {

				case MovedHandle.Bottom:					Cursor.Current = Cursors.SizeNS;	break;
				case MovedHandle.Top:						Cursor.Current = Cursors.SizeNS;	break;
				case MovedHandle.Left:						Cursor.Current = Cursors.SizeWE;	break;
				case MovedHandle.Right:						Cursor.Current = Cursors.SizeWE;	break;

				case MovedHandle.Top|MovedHandle.Left:		Cursor.Current = Cursors.SizeNWSE;	break;
				case MovedHandle.Bottom|MovedHandle.Right:	Cursor.Current = Cursors.SizeNWSE;	break;
				case MovedHandle.Top|MovedHandle.Right:		Cursor.Current = Cursors.SizeNESW;	break;
				case MovedHandle.Bottom|MovedHandle.Left:	Cursor.Current = Cursors.SizeNESW;	break;

				case MovedHandle.Panning:					Cursor.Current = Cursors.Hand;		break;

				default:									Cursor.Current = Cursors.No;		break;
			}

			if(movedHandle != MovedHandle.None) {
				eraseCroppingRectangle = true;
				invalidateCroppingRectangle();
				Update();
				eraseCroppingRectangle = false;

				if(movedHandle == MovedHandle.Panning) {
					Rectangle	newRect = movedCroppingRectangle;

					newRect.Offset(e.X - moveReferecePoint.X, e.Y - moveReferecePoint.Y);
					if(newRect.Top >= 0 && newRect.Bottom < Height && newRect.Left >= 0 && newRect.Right < Width)
						croppingRectangle = newRect;
				}
				else {
					// Calculate the new rectangle
					int		t = croppingRectangle.Top;
					int		b = croppingRectangle.Bottom;
					int		l = croppingRectangle.Left;
					int		r = croppingRectangle.Right;

					if((movedHandle & MovedHandle.Left) != 0 && e.X >= 0)
						l = e.X;
					else if((movedHandle & MovedHandle.Right) != 0 && e.X < Width)
						r = e.X;

					if((movedHandle & MovedHandle.Top) != 0 && e.Y >= 0)
						t = e.Y;
					else if((movedHandle & MovedHandle.Bottom) != 0 && e.Y < Height)
						b = e.Y;

					croppingRectangle = Rectangle.FromLTRB(l, t, r, b);
				}

				invalidateCroppingRectangle();

				if(CroppingRectangleChanged != null)
					CroppingRectangleChanged(this, null);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			MovedHandle	h = HitTest(new Point(e.X, e.Y));

			if(h != MovedHandle.None) {
				this.Capture = true;
				movedHandle = h;
				moveReferecePoint = new Point(e.X, e.Y);
				movedCroppingRectangle = croppingRectangle;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			if(movedHandle != MovedHandle.None) {
				this.Capture = false;
				movedHandle = MovedHandle.None;
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}
