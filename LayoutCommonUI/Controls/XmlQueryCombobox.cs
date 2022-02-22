using System.Text.RegularExpressions;
using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for XmlQueryCombobox.
    /// </summary>
    public class XmlQueryCombobox : ComboBox {
        public XmlQueryCombobox() {
        }

        public XmlElement? ContainerElement { set; get; }

        public string Query { get; set; } = "*[contains(Name, '<TEXT>')]";

        public string Extract { get; set; } = "string(Name)";

        public XmlElement? SelectedElement {
            get {
                var l = GetElements();

                return l == null ? null : l.Count > 0 ? (XmlElement?)l[0] : null;
            }
        }

        public bool IsTextAmbiguous {
            get {
                var l = GetElements();

                return l != null && l.Count > 1;
            }
        }

        private XmlNodeList? GetElements() {
            string q = "*";

            if (Text.Trim() != "")
                q = Regex.Replace(Query, "<TEXT>", Text);

            return ContainerElement?.SelectNodes(q);
        }

        protected override void OnDropDown(EventArgs e) {
            base.OnDropDown(e);

            string ext = Extract;
            if (ext == null)
                ext = "string(Name)";

            Items.Clear();
            var elements = GetElements();

            if (elements != null) {
                foreach (XmlElement element in elements)
                    Items.Add(element.CreateNavigator()!.Evaluate(ext));
            }
        }
    }
}
