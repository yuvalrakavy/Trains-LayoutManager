using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Components;
using LayoutManager.Model;

namespace MarklinDigital.Dialogs {
    /// <summary>
    /// Summary description for MarklinDigitalProperties.
    /// </summary>
    public class MarklinDigitalProperties : Form {
        private const string A_FeedbackPolling = "FeedbackPolling";
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private ComboBox comboBoxPort;
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private Label label2;
        private NumericUpDown numericUpDownFeedbackPolling;
        private Label label3;
        private LayoutManager.CommonUI.Controls.LayoutEmulationSetup layoutEmulationSetup;
        private Button buttonCOMsettings;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private readonly MarklinDigitalCentralStation component;

        public MarklinDigitalProperties(MarklinDigitalCentralStation component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.component = component;
            this.XmlInfo = new LayoutXmlInfo(component);

            nameDefinition.XmlInfo = this.XmlInfo;

            layoutEmulationSetup.Element = XmlInfo.DocumentElement;

            comboBoxPort.Text = XmlInfo.DocumentElement.GetAttribute(LayoutIOServices.A_Port);
            if (XmlInfo.DocumentElement.HasAttribute(A_FeedbackPolling))
                numericUpDownFeedbackPolling.Value = (int)XmlInfo.DocumentElement.AttributeValue(A_FeedbackPolling);

            updateButtons(null, null);
        }

        public LayoutXmlInfo XmlInfo { get; }

        private void updateButtons(object sender, EventArgs e) {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.comboBoxPort = new ComboBox();
            this.label1 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.label2 = new Label();
            this.numericUpDownFeedbackPolling = new NumericUpDown();
            this.label3 = new Label();
            this.layoutEmulationSetup = new LayoutManager.CommonUI.Controls.LayoutEmulationSetup();
            this.buttonCOMsettings = new Button();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownFeedbackPolling).BeginInit();
            this.SuspendLayout();
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(8, 8);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(280, 64);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
            this.comboBoxPort.Location = new System.Drawing.Point(58, 88);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(104, 21);
            this.comboBoxPort.TabIndex = 1;
            this.comboBoxPort.Text = "COM1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(192, 248);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(272, 248);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(199, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Poll feedback decoders approximately: ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownFeedbackPolling
            // 
            this.numericUpDownFeedbackPolling.Location = new System.Drawing.Point(212, 115);
            this.numericUpDownFeedbackPolling.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownFeedbackPolling.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownFeedbackPolling.Name = "numericUpDownFeedbackPolling";
            this.numericUpDownFeedbackPolling.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownFeedbackPolling.TabIndex = 9;
            this.numericUpDownFeedbackPolling.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(259, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "times per second";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // layoutEmulationSetup
            // 
            this.layoutEmulationSetup.Element = null;
            this.layoutEmulationSetup.Location = new System.Drawing.Point(8, 152);
            this.layoutEmulationSetup.Name = "layoutEmulationSetup";
            this.layoutEmulationSetup.Size = new System.Drawing.Size(338, 89);
            this.layoutEmulationSetup.TabIndex = 10;
            // 
            // buttonCOMsettings
            // 
            this.buttonCOMsettings.Location = new System.Drawing.Point(169, 89);
            this.buttonCOMsettings.Name = "buttonCOMsettings";
            this.buttonCOMsettings.Size = new System.Drawing.Size(75, 21);
            this.buttonCOMsettings.TabIndex = 11;
            this.buttonCOMsettings.Text = "Settings...";
            this.buttonCOMsettings.Click += this.buttonCOMsettings_Click;
            // 
            // MarklinDigitalProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(354, 280);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCOMsettings);
            this.Controls.Add(this.layoutEmulationSetup);
            this.Controls.Add(this.numericUpDownFeedbackPolling);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxPort);
            this.Controls.Add(this.nameDefinition);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MarklinDigitalProperties";
            this.Text = "Marklin Digital Interface Properties";
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownFeedbackPolling).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            // Validate

            if (nameDefinition.Commit()) {
                LayoutTextInfo myName = new LayoutTextInfo(XmlInfo.DocumentElement, "Name");
                IEnumerable<IModelComponentIsCommandStation> commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All);

                foreach (IModelComponentIsCommandStation otherCommandStation in commandStations) {
                    if (otherCommandStation.NameProvider.Name == myName.Name && otherCommandStation.Id != component.Id) {
                        MessageBox.Show(this, "The name " + myName.Text + " is already used", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        nameDefinition.Focus();
                        return;
                    }
                }
            }
            else
                return;

            if (!layoutEmulationSetup.ValidateInput())
                return;

            // Commit

            XmlInfo.DocumentElement.SetAttribute(LayoutIOServices.A_Port, comboBoxPort.Text);
            XmlInfo.DocumentElement.SetAttributeValue(A_FeedbackPolling, numericUpDownFeedbackPolling.Value);
            layoutEmulationSetup.Commit();

            DialogResult = DialogResult.OK;
        }

        private void buttonCOMsettings_Click(object sender, EventArgs e) {
            string modeString = XmlInfo.DocumentElement[LayoutIOServices.E_ModeString].InnerText;

            LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters d = new LayoutManager.CommonUI.Dialogs.SerialInterfaceParameters(modeString);

            if (d.ShowDialog(this) == DialogResult.OK)
                XmlInfo.DocumentElement[LayoutIOServices.E_ModeString].InnerText = d.ModeString;
        }
    }
}
