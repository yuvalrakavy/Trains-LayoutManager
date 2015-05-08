using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.CommonUI.Dialogs
{
	/// <summary>
	/// Summary description for PolicyDefinition.
	/// </summary>
	public class TrainConditionDefinition : Form {
		private LayoutManager.CommonUI.Controls.EventScriptEditor eventScriptEditor;
		private Button buttonOK;
		private Button buttonCancel;
		private RadioButton radioButtonNoCondition;
		private RadioButton radioButtonCondition;
		private LayoutManager.CommonUI.Controls.LinkMenu linkMenuConditionScope;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		TripPlanTrainConditionInfo	trainCondition;

		public TrainConditionDefinition(LayoutBlockDefinitionComponent blockDefinition, TripPlanTrainConditionInfo inTrainCondition)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			XmlDocument		scriptDocument = LayoutXmlInfo.XmlImplementation.CreateDocument();
			XmlElement		conditionBodyElement = null;

			trainCondition = new TripPlanTrainConditionInfo((XmlElement)scriptDocument.ImportNode(inTrainCondition.Element, true));

			scriptDocument.AppendChild(trainCondition.Element);

			if(!trainCondition.IsConditionEmpty)
				conditionBodyElement = trainCondition.ConditionBodyElement;
			
			if(!trainCondition.IsConditionEmpty) {
				radioButtonCondition.Checked = true;
				eventScriptEditor.Enabled = true;
			}
			else {
				radioButtonNoCondition.Checked = true;
				conditionBodyElement = scriptDocument.CreateElement("And");
				trainCondition.Element.AppendChild(conditionBodyElement);
				eventScriptEditor.Enabled = false;
			}

			eventScriptEditor.EventScriptElement = conditionBodyElement;
			eventScriptEditor.BlockDefinition = blockDefinition;

			if(trainCondition.ConditionScope == TripPlanTrainConditionScope.AllowIfTrue)
				linkMenuConditionScope.SelectedIndex = 0;
			else
				linkMenuConditionScope.SelectedIndex = 1;
		}

		public TripPlanTrainConditionInfo TrainCondition {
			get {
				if(radioButtonNoCondition.Checked) {
					if(!trainCondition.IsConditionEmpty)
						trainCondition.Element.RemoveChild(trainCondition.ConditionBodyElement);
				}

				return trainCondition;
			}
		}

        public bool NoCondition => radioButtonNoCondition.Checked;

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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.radioButtonNoCondition = new System.Windows.Forms.RadioButton();
            this.radioButtonCondition = new System.Windows.Forms.RadioButton();
            this.linkMenuConditionScope = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.SuspendLayout();
            // 
            // eventScriptEditor
            // 
            this.eventScriptEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventScriptEditor.BlockDefinition = null;
            this.eventScriptEditor.EventScriptElement = null;
            this.eventScriptEditor.Location = new System.Drawing.Point(51, 126);
            this.eventScriptEditor.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.eventScriptEditor.Name = "eventScriptEditor";
            this.eventScriptEditor.Size = new System.Drawing.Size(1082, 579);
            this.eventScriptEditor.TabIndex = 3;
            this.eventScriptEditor.ViewOnly = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(742, 730);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(179, 55);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(947, 730);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(179, 55);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // radioButtonNoCondition
            // 
            this.radioButtonNoCondition.Location = new System.Drawing.Point(26, 19);
            this.radioButtonNoCondition.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.radioButtonNoCondition.Name = "radioButtonNoCondition";
            this.radioButtonNoCondition.Size = new System.Drawing.Size(333, 57);
            this.radioButtonNoCondition.TabIndex = 6;
            this.radioButtonNoCondition.Text = "No condition";
            this.radioButtonNoCondition.CheckedChanged += new System.EventHandler(this.radioButtonNoCondition_CheckedChanged);
            // 
            // radioButtonCondition
            // 
            this.radioButtonCondition.Location = new System.Drawing.Point(26, 76);
            this.radioButtonCondition.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.radioButtonCondition.Name = "radioButtonCondition";
            this.radioButtonCondition.Size = new System.Drawing.Size(51, 57);
            this.radioButtonCondition.TabIndex = 7;
            this.radioButtonCondition.CheckedChanged += new System.EventHandler(this.radioButtonCondition_CheckedChanged);
            // 
            // linkMenuConditionScope
            // 
            this.linkMenuConditionScope.Location = new System.Drawing.Point(80, 86);
            this.linkMenuConditionScope.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.linkMenuConditionScope.Name = "linkMenuConditionScope";
            this.linkMenuConditionScope.Options = new string[] {
        "Allow only trains for which the following is True",
        "Do not allow trains for which the following is True"};
            this.linkMenuConditionScope.SelectedIndex = -1;
            this.linkMenuConditionScope.Size = new System.Drawing.Size(819, 38);
            this.linkMenuConditionScope.TabIndex = 8;
            this.linkMenuConditionScope.TabStop = true;
            this.linkMenuConditionScope.Text = "Allow only trains for which the following is True";
            // 
            // TrainConditionDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(1152, 796);
            this.ControlBox = false;
            this.Controls.Add(this.linkMenuConditionScope);
            this.Controls.Add(this.radioButtonCondition);
            this.Controls.Add(this.radioButtonNoCondition);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.eventScriptEditor);
            this.Controls.Add(this.buttonCancel);
            this.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.Name = "TrainConditionDefinition";
            this.ShowInTaskbar = false;
            this.Text = "Train Condition Definition";
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

			if(linkMenuConditionScope.SelectedIndex == 0)
				trainCondition.ConditionScope = TripPlanTrainConditionScope.AllowIfTrue;
			else
				trainCondition.ConditionScope = TripPlanTrainConditionScope.DisallowIfTrue;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void radioButtonNoCondition_CheckedChanged(object sender, System.EventArgs e) {
			eventScriptEditor.Enabled = false;
			linkMenuConditionScope.Enabled = false;
		}

		private void radioButtonCondition_CheckedChanged(object sender, System.EventArgs e) {
			eventScriptEditor.Enabled = true;
			linkMenuConditionScope.Enabled = true;
		}
	}
}
