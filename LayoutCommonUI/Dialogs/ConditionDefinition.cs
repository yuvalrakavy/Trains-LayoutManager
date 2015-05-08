using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs
{
	/// <summary>
	/// Summary description for PolicyDefinition.
	/// </summary>
	public class ConditionDefinition : Form {
		private LayoutManager.CommonUI.Controls.EventScriptEditor eventScriptEditor;
		private Button buttonOK;
		private Button buttonCancel;
		private RadioButton radioButtonNoCondition;
		private RadioButton radioButtonCondition;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		XmlElement			conditionElement;

		public ConditionDefinition(XmlElement inConditionElement) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.conditionElement = inConditionElement;

			XmlDocument		scriptDocument = LayoutXmlInfo.XmlImplementation.CreateDocument();
			XmlElement		conditionBodyElement = null;

			conditionElement = (XmlElement)scriptDocument.ImportNode(inConditionElement, true);

			scriptDocument.AppendChild(conditionElement);

			if(conditionElement.ChildNodes.Count > 0)
				conditionBodyElement = (XmlElement)conditionElement.ChildNodes[0];
			
			if(conditionBodyElement != null) {
				radioButtonCondition.Checked = true;
				eventScriptEditor.Enabled = true;
			}
			else {
				radioButtonNoCondition.Checked = true;
				conditionBodyElement = scriptDocument.CreateElement("And");
				conditionElement.AppendChild(conditionBodyElement);
				eventScriptEditor.Enabled = false;
			}

			eventScriptEditor.EventScriptElement = conditionBodyElement;
		}

		public XmlElement ConditionElement {
			get {
				if(radioButtonNoCondition.Checked) {
					if(conditionElement.ChildNodes.Count > 0)
						conditionElement.RemoveChild(conditionElement.ChildNodes[0]);
				}

				return conditionElement;
			}
		}

		public bool NoCondition {
			get {
				return radioButtonNoCondition.Checked;
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
			this.eventScriptEditor = new LayoutManager.CommonUI.Controls.EventScriptEditor();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.radioButtonNoCondition = new RadioButton();
			this.radioButtonCondition = new RadioButton();
			this.SuspendLayout();
			// 
			// eventScriptEditor
			// 
			this.eventScriptEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.eventScriptEditor.EventScriptElement = null;
			this.eventScriptEditor.Location = new System.Drawing.Point(16, 53);
			this.eventScriptEditor.Name = "eventScriptEditor";
			this.eventScriptEditor.Size = new System.Drawing.Size(338, 243);
			this.eventScriptEditor.TabIndex = 3;
			this.eventScriptEditor.ViewOnly = false;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(232, 306);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(56, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(296, 306);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(56, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// radioButtonNoCondition
			// 
			this.radioButtonNoCondition.Location = new System.Drawing.Point(8, 8);
			this.radioButtonNoCondition.Name = "radioButtonNoCondition";
			this.radioButtonNoCondition.TabIndex = 6;
			this.radioButtonNoCondition.Text = "No condition";
			this.radioButtonNoCondition.CheckedChanged += new System.EventHandler(this.radioButtonNoCondition_CheckedChanged);
			// 
			// radioButtonCondition
			// 
			this.radioButtonCondition.Location = new System.Drawing.Point(8, 32);
			this.radioButtonCondition.Name = "radioButtonCondition";
			this.radioButtonCondition.Size = new System.Drawing.Size(176, 24);
			this.radioButtonCondition.TabIndex = 7;
			this.radioButtonCondition.Text = "Use the following condition:";
			this.radioButtonCondition.CheckedChanged += new System.EventHandler(this.radioButtonCondition_CheckedChanged);
			// 
			// ConditionDefinition
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(360, 334);
			this.ControlBox = false;
			this.Controls.Add(this.radioButtonCondition);
			this.Controls.Add(this.radioButtonNoCondition);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.eventScriptEditor);
			this.Controls.Add(this.buttonCancel);
			this.Name = "ConditionDefinition";
			this.ShowInTaskbar = false;
			this.Text = "Condition Definition";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(radioButtonCondition.Checked) {
				if(!eventScriptEditor.ValidateScript()) {
					eventScriptEditor.Focus();
					return;
				}
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void radioButtonNoCondition_CheckedChanged(object sender, System.EventArgs e) {
			eventScriptEditor.Enabled = false;
		}

		private void radioButtonCondition_CheckedChanged(object sender, System.EventArgs e) {
			eventScriptEditor.Enabled = true;
		}
	}
}
