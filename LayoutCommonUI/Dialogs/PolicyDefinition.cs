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
	public class PolicyDefinition : Form {
		private Label label1;
		private TextBox textBoxName;
		private LayoutManager.CommonUI.Controls.EventScriptEditor eventScriptEditor;
		private Button buttonOK;
		private Button buttonCancel;
		private CheckBox checkBoxApply;
		private CheckBox checkBoxGlobalPolicy;
		private CheckBox checkBoxShowInMenu;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		XmlElement			scriptElement;
		LayoutPolicyInfo	policy;

		public PolicyDefinition(LayoutPolicyInfo policy)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.policy = policy;

			XmlDocument		scriptDocument = LayoutXmlInfo.XmlImplementation.CreateDocument();
			
			if(policy.EventScriptElement == null)
				scriptElement = scriptDocument.CreateElement("Sequence");
			else
				scriptElement = (XmlElement)scriptDocument.ImportNode(policy.EventScriptElement, true);

			scriptDocument.AppendChild(scriptElement);

			textBoxName.Text = policy.Name;
			checkBoxApply.Checked = policy.Apply;
			checkBoxGlobalPolicy.Checked = policy.GlobalPolicy;

			if(policy.Scope != "Global" && policy.Scope != "TripPlan")
				checkBoxApply.Visible = false;

			if(policy.Scope == "Global")
				checkBoxShowInMenu.Checked = policy.ShowInMenu;
			else
				checkBoxShowInMenu.Visible = false;

			eventScriptEditor.EventScriptElement = scriptElement;
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.checkBoxApply = new System.Windows.Forms.CheckBox();
            this.eventScriptEditor = new LayoutManager.CommonUI.Controls.EventScriptEditor();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkBoxGlobalPolicy = new System.Windows.Forms.CheckBox();
            this.checkBoxShowInMenu = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(51, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(205, 26);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(759, 38);
            this.textBoxName.TabIndex = 1;
            // 
            // checkBoxApply
            // 
            this.checkBoxApply.Location = new System.Drawing.Point(70, 86);
            this.checkBoxApply.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.checkBoxApply.Name = "checkBoxApply";
            this.checkBoxApply.Size = new System.Drawing.Size(333, 41);
            this.checkBoxApply.TabIndex = 2;
            this.checkBoxApply.Text = "Apply this policy";
            // 
            // eventScriptEditor
            // 
            this.eventScriptEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventScriptEditor.BlockDefinition = null;
            this.eventScriptEditor.EventScriptElement = null;
            this.eventScriptEditor.Location = new System.Drawing.Point(0, 210);
            this.eventScriptEditor.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.eventScriptEditor.Name = "eventScriptEditor";
            this.eventScriptEditor.Size = new System.Drawing.Size(1075, 553);
            this.eventScriptEditor.TabIndex = 5;
            this.eventScriptEditor.ViewOnly = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(666, 782);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(179, 55);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(870, 782);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(179, 55);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // checkBoxGlobalPolicy
            // 
            this.checkBoxGlobalPolicy.Location = new System.Drawing.Point(70, 126);
            this.checkBoxGlobalPolicy.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.checkBoxGlobalPolicy.Name = "checkBoxGlobalPolicy";
            this.checkBoxGlobalPolicy.Size = new System.Drawing.Size(794, 43);
            this.checkBoxGlobalPolicy.TabIndex = 3;
            this.checkBoxGlobalPolicy.Text = "This policy is available in all layouts";
            // 
            // checkBoxShowInMenu
            // 
            this.checkBoxShowInMenu.Location = new System.Drawing.Point(70, 172);
            this.checkBoxShowInMenu.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.checkBoxShowInMenu.Name = "checkBoxShowInMenu";
            this.checkBoxShowInMenu.Size = new System.Drawing.Size(800, 38);
            this.checkBoxShowInMenu.TabIndex = 4;
            this.checkBoxShowInMenu.Text = "Show policy in \"Tools\" menu";
            // 
            // PolicyDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(1075, 854);
            this.ControlBox = false;
            this.Controls.Add(this.checkBoxShowInMenu);
            this.Controls.Add(this.checkBoxGlobalPolicy);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.eventScriptEditor);
            this.Controls.Add(this.checkBoxApply);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.Name = "PolicyDefinition";
            this.ShowInTaskbar = false;
            this.Text = "Policy Definition";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(textBoxName.Text.Trim() == "") {
				MessageBox.Show(this, "You must provide name for the policy", "Missing Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxName.Focus();
				return;
			}

			if(!eventScriptEditor.ValidateScript(checkBoxGlobalPolicy.Checked)) {
				eventScriptEditor.Focus();
				return;
			}

			policy.Name = textBoxName.Text;
			policy.Apply = checkBoxApply.Checked;
			policy.EventScriptElement = eventScriptEditor.EventScriptElement;
			policy.GlobalPolicy = checkBoxGlobalPolicy.Checked;
			policy.ShowInMenu = checkBoxShowInMenu.Checked;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
