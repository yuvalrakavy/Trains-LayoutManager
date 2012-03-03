using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for PictureBoxWithTransparency.
	/// </summary>
	public class PictureBoxWithTransparency : PictureBox
	{
		Color	transparentColor = Color.Empty;
		bool	isTransparent = false;

		public PictureBoxWithTransparency()
		{
		}

		[Category("Appearance")]
		public Color TransparentColor {
			get {
				return transparentColor;
			}

			set {
				transparentColor = value;
				isTransparent = false;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			if(!isTransparent && Image != null) {
				Bitmap		bm = (Bitmap)Image;
			
				bm.MakeTransparent(transparentColor);
				isTransparent = true;
			}

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}
	}
}
