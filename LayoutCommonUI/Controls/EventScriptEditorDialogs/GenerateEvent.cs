using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for GenerateEvent.
    /// </summary>
    public partial class GenerateEvent : Form {
        private readonly XmlElement element;
        private readonly XmlElement optionsElement;

        public GenerateEvent(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            LayoutEventDefAttribute[] requestEventDefs = EventManager.Instance.GetEventDefinitions(LayoutEventRole.Request);

            foreach (LayoutEventDefAttribute eventDef in requestEventDefs)
                comboBoxEventName.Items.Add(eventDef.Name);

            if (element.HasAttribute("EventName"))
                comboBoxEventName.Text = element.GetAttribute("EventName");

            generateEventArgumentSender.OptionalElement = element;
            generateEventArgumentSender.Prefix = "Sender";

            generateEventArgumentInfo.OptionalElement = element;
            generateEventArgumentInfo.Prefix = "Info";

            generateEventArgumentSender.Initialize();
            generateEventArgumentInfo.Initialize();

            XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            XmlElement? optonsElement = element["Options"];

            if (optonsElement != null)
                workingDoc.AppendChild(workingDoc.ImportNode(optonsElement, true));
            else
                workingDoc.AppendChild(workingDoc.CreateElement("Options"));

            optionsElement = workingDoc.DocumentElement!;

            foreach (XmlElement optionElement in optionsElement)
                listViewOptions.Items.Add(new OptionItem(optionElement));

            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listViewOptions.SelectedItems.Count > 0) {
                buttonEditOption.Enabled = true;
                buttonDeleteOption.Enabled = true;
            }
            else {
                buttonEditOption.Enabled = false;
                buttonDeleteOption.Enabled = false;
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


        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (comboBoxEventName.Text.Trim() == "") {
                MessageBox.Show(this, "The name of the event to be generated was not provided", "Missing event name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxEventName.Focus();
                return;
            }

            if (!generateEventArgumentSender.ValidateInput())
                return;

            if (!generateEventArgumentInfo.ValidateInput())
                return;

            element.SetAttribute("EventName", comboBoxEventName.Text);

            generateEventArgumentSender.Commit();
            generateEventArgumentInfo.Commit();

            XmlElement? existingOptionsElement = element["Options"];

            if (this.optionsElement.ChildNodes.Count > 0) {
                if (existingOptionsElement != null)
                    element.ReplaceChild(element.OwnerDocument.ImportNode(this.optionsElement, true), existingOptionsElement);
                else
                    element.AppendChild(element.OwnerDocument.ImportNode(this.optionsElement, true));
            }
            else if (existingOptionsElement != null)
                element.RemoveChild(existingOptionsElement);

            DialogResult = DialogResult.OK;
        }

        private void ButtonAddOption_Click(object? sender, EventArgs e) {
            XmlElement optionElement = optionsElement.OwnerDocument.CreateElement("Option");
            GenerateEventOption d = new(optionElement, optionsElement);

            if (d.ShowDialog(this) == DialogResult.OK) {
                listViewOptions.Items.Add(new OptionItem(optionElement));

                optionsElement.AppendChild(optionElement);
            }
        }

        private void ButtonEditOption_Click(object? sender, EventArgs e) {
            if (listViewOptions.SelectedItems.Count > 0) {
                OptionItem selected = (OptionItem)listViewOptions.SelectedItems[0];
                GenerateEventOption d = new(selected.Element, optionsElement);

                if (d.ShowDialog(this) == DialogResult.OK)
                    selected.Update();
            }
        }

        private void ButtonDeleteOption_Click(object? sender, EventArgs e) {
            if (listViewOptions.SelectedItems.Count > 0) {
                OptionItem selected = (OptionItem)listViewOptions.SelectedItems[0];

                optionsElement.RemoveChild(selected.Element);
                listViewOptions.Items.Remove(selected);

                UpdateButtons();
            }
        }

        private void ListViewOptions_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        private void ListViewOptions_DoubleClick(object? sender, EventArgs e) {
            buttonEditOption.PerformClick();
        }

        private class OptionItem : ListViewItem {
            public OptionItem(XmlElement optionElement) {
                this.Element = optionElement;
                SubItems.Add("");
                Update();
            }

            public void Update() {
                Text = Element.GetAttribute("Name");
                SubItems[1].Text = LayoutEventScriptEditorTreeNode.GetOperandDescription(Element, "Option");
            }

            public XmlElement Element { get; }
        }
    }
}
