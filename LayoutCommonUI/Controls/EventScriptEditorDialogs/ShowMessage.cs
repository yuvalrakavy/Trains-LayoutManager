using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for ShowMessage.
    /// </summary>
    public partial class ShowMessage : Form {
        private readonly XmlElement element;

        public ShowMessage(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            if (element.HasAttribute("Message"))
                textBoxMessage.Text = element.GetAttribute("Message");

            if (element.HasAttribute("MessageType")) {
                switch (element.GetAttribute("MessageType")) {
                    case "Message":
                        radioButtonMessage.Checked = true;
                        break;

                    case "Warning":
                        radioButtonWanrning.Checked = true;
                        break;

                    case "Error":
                        radioButtonError.Checked = true;
                        break;
                }
            }
            else
                radioButtonMessage.Checked = true;
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
            element.SetAttribute("Message", textBoxMessage.Text);

            if (radioButtonMessage.Checked)
                element.SetAttribute("MessageType", "Message");
            else if (radioButtonWanrning.Checked)
                element.SetAttribute("MessageType", "Warning");
            else if (radioButtonError.Checked)
                element.SetAttribute("MessageType", "Error");

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
