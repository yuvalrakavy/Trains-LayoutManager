using System.Windows.Forms;

namespace LayoutManager.View {
    public class Ballon {
        public PopupWindowContainerSection Content { get; }

        public Ballon(Control? parent = null) {
            Content = new PopupWindowContainerSection(parent);
        }
    }
}
