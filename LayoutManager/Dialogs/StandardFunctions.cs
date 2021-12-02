using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StandardFunctions.
    /// </summary>
    public partial class StandardFunctions : Form {
        private readonly XmlElement commonFunctionsElement;

        public StandardFunctions() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            commonFunctionsElement = LayoutModel.LocomotiveCatalog.LocomotiveFunctionNames;

            foreach (XmlElement f in commonFunctionsElement)
                listViewFunctionInfo.Items.Add(new StandardFunctionItem(f));

            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listViewFunctionInfo.SelectedItems.Count > 0) {
                buttonEdit.Enabled = true;
                buttonDelete.Enabled = true;
            }
            else {
                buttonEdit.Enabled = false;
                buttonDelete.Enabled = false;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonNew_Click(object? sender, EventArgs e) {
            XmlElement f = commonFunctionsElement.OwnerDocument.CreateElement("Function");
            StandardFunction d = new(f);

            if (d.ShowDialog(this) == DialogResult.OK) {
                commonFunctionsElement.AppendChild(f);
                listViewFunctionInfo.Items.Add(new StandardFunctionItem(f));
            }
        }

        private void ButtonEdit_Click(object? sender, EventArgs e) {
            if (listViewFunctionInfo.SelectedItems.Count > 0) {
                StandardFunctionItem item = (StandardFunctionItem)listViewFunctionInfo.SelectedItems[0];

                item.Edit();
            }
        }

        private void ButtonDelete_Click(object? sender, EventArgs e) {
            if (listViewFunctionInfo.SelectedItems.Count > 0) {
                StandardFunctionItem item = (StandardFunctionItem)listViewFunctionInfo.SelectedItems[0];

                item.Element.ParentNode?.RemoveChild(item.Element);
                listViewFunctionInfo.Items.Remove(item);
            }

            UpdateButtons();
        }

        private void ListViewFunctionInfo_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            this.Close();
        }

        private class StandardFunctionItem : ListViewItem {
            public StandardFunctionItem(XmlElement element) {
                this.Element = element;

                this.Text = GetTypeString();
                this.SubItems.Add(element.GetAttribute("Name"));
                this.SubItems.Add(element.GetAttribute("Description"));
            }

            public XmlElement Element { get; }

            public void Edit() {
                StandardFunction d = new(Element);

                if (d.ShowDialog(this.ListView.Parent) == DialogResult.OK) {
                    this.Text = GetTypeString();
                    this.SubItems[1].Text = Element.GetAttribute("Name");
                    this.SubItems[2].Text = Element.GetAttribute("Description");
                }
            }

            private string GetTypeString() {
                if (Element.HasAttribute("Type")) {
                    LocomotiveFunctionType type = (LocomotiveFunctionType)Enum.Parse(typeof(LocomotiveFunctionType), Element.GetAttribute("Type"));

                    return type == LocomotiveFunctionType.OnOff ? "On/Off" : type.ToString();
                }
                else
                    return "";
            }
        }
    }
}
