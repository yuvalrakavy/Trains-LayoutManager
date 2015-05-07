using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for XmlQueryCombobox.
	/// </summary>
	public class XmlQueryCombobox : ComboBox {
		XmlElement	container;
		String		query = "*[contains(Name, '<TEXT>')]";
		String		extract = "string(Name)";

		public XmlQueryCombobox()
		{
		}

		public XmlElement ContainerElement {
			set {
				container = value;
			}

			get {
				return container;
			}
		}

		public String Query {
			get {
				return query;
			}

			set {
				query = value;
			}
		}

		public String Extract {
			get {
				return extract;
			}

			set {
				extract = value;
			}
		}

		public XmlElement SelectedElement {
			get {
				XmlNodeList	l = getElements();

				if(l.Count > 0)
					return (XmlElement)l[0];
				return null;
			}
		}

		public bool IsTextAmbiguous {
			get {
				XmlNodeList	l = getElements();

				if(l.Count > 1)
					return true;
				return false;
			}
		}

		private XmlNodeList getElements() {
			String	q = "*";

			if(Text.Trim() != "")
				q = Regex.Replace(query, "<TEXT>", Text);


			return container.SelectNodes(q);
		}

		protected override void OnDropDown(EventArgs e) {
			base.OnDropDown(e);

			String	ext = extract;
			if(ext == null)
				ext = "string(Name)";

			Items.Clear();
			foreach(XmlElement element in getElements())
				Items.Add(element.CreateNavigator().Evaluate(ext));
		}
	}
}
