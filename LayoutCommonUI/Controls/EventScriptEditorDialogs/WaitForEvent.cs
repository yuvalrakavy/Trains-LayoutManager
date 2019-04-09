using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

#nullable enable
namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for WaitForEvent.
    /// </summary>
    public class WaitForEvent : Form, IObjectHasXml {
        private const string A_Name = "Name";
        private const string A_LimitToScope = "LimitToScope";
        private Label label1;
        private ComboBox comboBoxEvent;
        private Button buttonOK;
        private Button buttonCancel;
        private CheckBox checkBoxLimitedToScope;

        #pragma warning disable nullable
        public WaitForEvent(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.Element = element;

            comboBoxEvent.Text = (string?)element.AttributeValue(A_Name) ?? "";
            checkBoxLimitedToScope.Checked = (bool?)element.AttributeValue(A_LimitToScope) ?? true;

            // Populate the combobox with possible event that have notification role
            foreach (LayoutEventDefAttribute eventDef in EventManager.Instance.GetEventDefinitions(LayoutEventRole.Notification))
                comboBoxEvent.Items.Add(eventDef.Name);
        }
#pragma warning restore nullable

        public XmlElement Element { get; }
        public XmlElement? OptionalElement => Element;

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxEvent = new ComboBox();
            this.checkBoxLimitedToScope = new CheckBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wait for event: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxEvent
            // 
            this.comboBoxEvent.Location = new System.Drawing.Point(96, 17);
            this.comboBoxEvent.Name = "comboBoxEvent";
            this.comboBoxEvent.Size = new System.Drawing.Size(216, 21);
            this.comboBoxEvent.Sorted = true;
            this.comboBoxEvent.TabIndex = 1;
            // 
            // checkBoxLimitedToScope
            // 
            this.checkBoxLimitedToScope.Location = new System.Drawing.Point(8, 48);
            this.checkBoxLimitedToScope.Name = "checkBoxLimitedToScope";
            this.checkBoxLimitedToScope.Size = new System.Drawing.Size(248, 24);
            this.checkBoxLimitedToScope.TabIndex = 2;
            this.checkBoxLimitedToScope.Text = "Limit only to events sent by current scope";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(152, 80);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(232, 80);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // WaitForEvent
            // 
            this.AcceptButton = this.buttonCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(320, 110);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.checkBoxLimitedToScope,
                                                                          this.comboBoxEvent,
                                                                          this.label1,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WaitForEvent";
            this.ShowInTaskbar = false;
            this.Text = "Wait For Event";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (comboBoxEvent.Text.Trim() == "") {
                MessageBox.Show(this, "You must specify event name", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxEvent.Focus();
                return;
            }

            Element.SetAttribute(A_Name, comboBoxEvent.Text);
            Element.SetAttribute(A_LimitToScope, checkBoxLimitedToScope.Checked, removeIf: true);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
