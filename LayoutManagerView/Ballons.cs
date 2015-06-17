using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutManager;
using System.Drawing.Drawing2D;
using LayoutManager.Model;

namespace LayoutManager.View {

	public class Ballon {
		public PopupWindowContainerSection Content { get; }

        public Ballon(Control parent = null) {
			Content = new PopupWindowContainerSection(parent);
		}
	}
}
