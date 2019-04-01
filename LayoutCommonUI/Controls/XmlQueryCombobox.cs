using System;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for XmlQueryCombobox.
    /// </summary>
    public class XmlQueryCombobox : ComboBox {
        XmlElement container;
        string query = "*[contains(Name, '<TEXT>')]";
        string extract = "string(Name)";

        public XmlQueryCombobox() {
        }

        public XmlElement ContainerElement {
            set {
                container = value;
            }

            get {
                return container;
            }
        }

        public string Query {
            get {
                return query;
            }

            set {
                query = value;
            }
        }

        public string Extract {
            get {
                return extract;
            }

            set {
                extract = value;
            }
        }

        public XmlElement SelectedElement {
            get {
                XmlNodeList l = getElements();

                if (l.Count > 0)
                    return (XmlElement)l[0];
                return null;
            }
        }

        public bool IsTextAmbiguous {
            get {
                XmlNodeList l = getElements();

                if (l.Count > 1)
                    return true;
                return false;
            }
        }

        private XmlNodeList getElements() {
            string q = "*";

            if (Text.Trim() != "")
                q = Regex.Replace(query, "<TEXT>", Text);


            return container.SelectNodes(q);
        }

        protected override void OnDropDown(EventArgs e) {
            base.OnDropDown(e);

            string ext = extract;
            if (ext == null)
                ext = "string(Name)";

            Items.Clear();
            foreach (XmlElement element in getElements())
                Items.Add(element.CreateNavigator().Evaluate(ext));
        }
    }
}
