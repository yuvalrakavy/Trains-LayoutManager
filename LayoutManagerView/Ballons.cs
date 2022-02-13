using System.Windows.Forms;

namespace LayoutManager.View {
    public class Balloon {
        public PopupWindowContainerSection Content { get; }

        public Balloon(Control? parent = null) {
            Content = new PopupWindowContainerSection(parent);
        }
    }
}
