using System;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for XmlQueryCombobox.
    /// </summary>
    public class XmlQueryCombobox : ComboBox {
        public XmlQueryCombobox() {
        }

        public XmlElement ContainerElement { set; get; }

        public string Query { get; set; } = "*[contains(Name, '<TEXT>')]";

        public string Extract { get; set; } = "string(Name)";

        public XmlElement SelectedElement {
            get {
                XmlNodeList l = getElements();

                return l.Count > 0 ? (XmlElement)l[0] : null;
            }
        }

        public bool IsTextAmbiguous {
            get {
                XmlNodeList l = getElements();

                return l.Count > 1;
            }
        }

        private XmlNodeList getElements() {
            string q = "*";

            if (Text.Trim() != "")
                q = Regex.Replace(Query, "<TEXT>", Text);

            return ContainerElement.SelectNodes(q);
        }

        protected override void OnDropDown(EventArgs e) {
            base.OnDropDown(e);

            string ext = Extract;
            if (ext == null)
                ext = "string(Name)";

            Items.Clear();
            foreach (XmlElement element in getElements())
                Items.Add(element.CreateNavigator().Evaluate(ext));
        }
    }
}
