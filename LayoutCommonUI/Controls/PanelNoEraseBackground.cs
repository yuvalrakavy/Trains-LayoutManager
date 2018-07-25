using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls {
    public partial class PanelNoEraseBackground : Panel {
        public PanelNoEraseBackground() {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            // Do not call base to prevent erasing the background
        }
    }
}
