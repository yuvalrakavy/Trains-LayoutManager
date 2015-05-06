using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs
{
	/// <summary>
	/// Summary description for GenerateEvent.
	/// </summary>
	public class GenerateEvent : Form {
		private Label label1;
		private ComboBox comboBoxEventName;
		private GroupBox groupBox1;
		private LayoutManager.CommonUI.Controls.GenerateEventArgument generateEventArgumentSender;
		private GroupBox groupBox2;
		private LayoutManager.CommonUI.Controls.GenerateEventArgument generateEventArgumentInfo;
		private GroupBox groupBox3;
		private ListView listViewOptions;
		private ColumnHeader columnHeaderOptionName;
		private ColumnHeader columnHeaderOptionValue;
		private Button buttonAddOption;
		private Button buttonEditOption;
		private Button buttonDeleteOption;
		private Button buttonOK;
		private Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		XmlElement			element;
		XmlElement			optionsElement;

		public GenerateEvent(XmlElement element)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.element = element;

			LayoutEventDefAttribute[]	requestEventDefs = EventManager.Instance.GetEventDefinitions(LayoutEventRole.Request);

			foreach(LayoutEventDefAttribute eventDef in requestEventDefs)
				comboBoxEventName.Items.Add(eventDef.Name);

			if(element.HasAttribute("EventName"))
				comboBoxEventName.Text = element.GetAttribute("EventName");

			generateEventArgumentSender.Element = element;
			generateEventArgumentSender.Prefix = "Sender";

			generateEventArgumentInfo.Element = element;
			generateEventArgumentInfo.Prefix = "Info";

			generateEventArgumentSender.Initialize();
			generateEventArgumentInfo.Initialize();

			XmlDocument	workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

			if(element["Options"] != null)
				workingDoc.AppendChild(workingDoc.ImportNode(element["Options"], true));
			else
				workingDoc.AppendChild(workingDoc.CreateElement("Options"));

			optionsElement = workingDoc.DocumentElement;

			foreach(XmlElement optionElement in optionsElement)
				listViewOptions.Items.Add(new OptionItem(optionElement));

			updateButtons();
		}

		private void updateButtons() {
			if(listViewOptions.SelectedItems.Count > 0) {
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
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new Label();
			this.comboBoxEventName = new ComboBox();
			this.groupBox1 = new GroupBox();
			this.generateEventArgumentSender = new LayoutManager.CommonUI.Controls.GenerateEventArgument();
			this.groupBox2 = new GroupBox();
			this.generateEventArgumentInfo = new LayoutManager.CommonUI.Controls.GenerateEventArgument();
			this.groupBox3 = new GroupBox();
			this.buttonDeleteOption = new Button();
			this.buttonEditOption = new Button();
			this.buttonAddOption = new Button();
			this.listViewOptions = new ListView();
			this.columnHeaderOptionName = new ColumnHeader();
			this.columnHeaderOptionValue = new ColumnHeader();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Event name:";
			// 
			// comboBoxEventName
			// 
			this.comboBoxEventName.Location = new System.Drawing.Point(88, 14);
			this.comboBoxEventName.Name = "comboBoxEventName";
			this.comboBoxEventName.Size = new System.Drawing.Size(288, 21);
			this.comboBoxEventName.Sorted = true;
			this.comboBoxEventName.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.generateEventArgumentSender);
			this.groupBox1.Location = new System.Drawing.Point(16, 56);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 184);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Parameter 1 (Sender)";
			// 
			// generateEventArgumentSender
			// 
			this.generateEventArgumentSender.Dock = System.Windows.Forms.DockStyle.Fill;
			this.generateEventArgumentSender.Element = null;
			this.generateEventArgumentSender.Location = new System.Drawing.Point(3, 16);
			this.generateEventArgumentSender.Name = "generateEventArgumentSender";
			this.generateEventArgumentSender.Prefix = "Arg";
			this.generateEventArgumentSender.Size = new System.Drawing.Size(266, 165);
			this.generateEventArgumentSender.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.generateEventArgumentInfo);
			this.groupBox2.Location = new System.Drawing.Point(296, 56);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(272, 184);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Parameter 2 (Info)";
			// 
			// generateEventArgumentInfo
			// 
			this.generateEventArgumentInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.generateEventArgumentInfo.Element = null;
			this.generateEventArgumentInfo.Location = new System.Drawing.Point(3, 16);
			this.generateEventArgumentInfo.Name = "generateEventArgumentInfo";
			this.generateEventArgumentInfo.Prefix = "Arg";
			this.generateEventArgumentInfo.Size = new System.Drawing.Size(266, 165);
			this.generateEventArgumentInfo.TabIndex = 0;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.buttonDeleteOption);
			this.groupBox3.Controls.Add(this.buttonEditOption);
			this.groupBox3.Controls.Add(this.buttonAddOption);
			this.groupBox3.Controls.Add(this.listViewOptions);
			this.groupBox3.Location = new System.Drawing.Point(16, 248);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(472, 160);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Event Options";
			// 
			// buttonDeleteOption
			// 
			this.buttonDeleteOption.Location = new System.Drawing.Point(168, 135);
			this.buttonDeleteOption.Name = "buttonDeleteOption";
			this.buttonDeleteOption.Size = new System.Drawing.Size(75, 20);
			this.buttonDeleteOption.TabIndex = 3;
			this.buttonDeleteOption.Text = "&Delete...";
			this.buttonDeleteOption.Click += new System.EventHandler(this.buttonDeleteOption_Click);
			// 
			// buttonEditOption
			// 
			this.buttonEditOption.Location = new System.Drawing.Point(88, 135);
			this.buttonEditOption.Name = "buttonEditOption";
			this.buttonEditOption.Size = new System.Drawing.Size(75, 20);
			this.buttonEditOption.TabIndex = 2;
			this.buttonEditOption.Text = "&Edit...";
			this.buttonEditOption.Click += new System.EventHandler(this.buttonEditOption_Click);
			// 
			// buttonAddOption
			// 
			this.buttonAddOption.Location = new System.Drawing.Point(8, 135);
			this.buttonAddOption.Name = "buttonAddOption";
			this.buttonAddOption.Size = new System.Drawing.Size(75, 20);
			this.buttonAddOption.TabIndex = 1;
			this.buttonAddOption.Text = "&Add...";
			this.buttonAddOption.Click += new System.EventHandler(this.buttonAddOption_Click);
			// 
			// listViewOptions
			// 
			this.listViewOptions.Columns.AddRange(new ColumnHeader[] {
																							  this.columnHeaderOptionName,
																							  this.columnHeaderOptionValue});
			this.listViewOptions.FullRowSelect = true;
			this.listViewOptions.Location = new System.Drawing.Point(8, 24);
			this.listViewOptions.MultiSelect = false;
			this.listViewOptions.Name = "listViewOptions";
			this.listViewOptions.Size = new System.Drawing.Size(456, 104);
			this.listViewOptions.TabIndex = 0;
			this.listViewOptions.View = System.Windows.Forms.View.Details;
			this.listViewOptions.DoubleClick += new System.EventHandler(this.listViewOptions_DoubleClick);
			this.listViewOptions.SelectedIndexChanged += new System.EventHandler(this.listViewOptions_SelectedIndexChanged);
			// 
			// columnHeaderOptionName
			// 
			this.columnHeaderOptionName.Text = "Option name";
			this.columnHeaderOptionName.Width = 150;
			// 
			// columnHeaderOptionValue
			// 
			this.columnHeaderOptionValue.Text = "Value";
			this.columnHeaderOptionValue.Width = 302;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(496, 352);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(496, 384);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			// 
			// GenerateEvent
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(576, 414);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.comboBoxEventName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GenerateEvent";
			this.ShowInTaskbar = false;
			this.Text = "Generate Event";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(comboBoxEventName.Text.Trim() == "") {
				MessageBox.Show(this, "The name of the event to be generated was not provided", "Missing event name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxEventName.Focus();
				return;
			}

			if(!generateEventArgumentSender.ValidateInput())
				return;

			if(!generateEventArgumentInfo.ValidateInput())
				return;

			element.SetAttribute("EventName", comboBoxEventName.Text);

			generateEventArgumentSender.Commit();
			generateEventArgumentInfo.Commit();

			if(optionsElement.ChildNodes.Count > 0) {
				if(element["Options"] != null)
					element.ReplaceChild(element.OwnerDocument.ImportNode(optionsElement, true), element["Options"]);
				else
					element.AppendChild(element.OwnerDocument.ImportNode(optionsElement, true));
			}
			else if(element["Options"] != null)
				element.RemoveChild(element["Options"]);

			DialogResult = DialogResult.OK;
		}

		private void buttonAddOption_Click(object sender, System.EventArgs e) {
			XmlElement			optionElement = optionsElement.OwnerDocument.CreateElement("Option");
			GenerateEventOption	d = new GenerateEventOption(optionElement, optionsElement);

			if(d.ShowDialog(this) == DialogResult.OK) {
				listViewOptions.Items.Add(new OptionItem(optionElement));

				optionsElement.AppendChild(optionElement);
			}
		}

		private void buttonEditOption_Click(object sender, System.EventArgs e) {
			if(listViewOptions.SelectedItems.Count > 0) {
				OptionItem			selected = (OptionItem)listViewOptions.SelectedItems[0];
				GenerateEventOption	d = new GenerateEventOption(selected.Element, optionsElement);

				if(d.ShowDialog(this) == DialogResult.OK)
					selected.Update();
			}
		}

		private void buttonDeleteOption_Click(object sender, System.EventArgs e) {
			if(listViewOptions.SelectedItems.Count > 0) {
				OptionItem			selected = (OptionItem)listViewOptions.SelectedItems[0];

				optionsElement.RemoveChild(selected.Element);
				listViewOptions.Items.Remove(selected);

				updateButtons();
			}
		}

		private void listViewOptions_SelectedIndexChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void listViewOptions_DoubleClick(object sender, System.EventArgs e) {
			buttonEditOption.PerformClick();
		}

		class OptionItem : ListViewItem {
			XmlElement	optionElement;

			public OptionItem(XmlElement optionElement) {
				this.optionElement = optionElement;
				SubItems.Add("");
				Update();
			}

			public void Update() {
				Text = optionElement.GetAttribute("Name");
				SubItems[1].Text = LayoutEventScriptEditorTreeNode.GetOperandDescription(optionElement, "Option");
			}

			public XmlElement Element {
				get {
					return optionElement;
				}
			}
		}
	}
}
