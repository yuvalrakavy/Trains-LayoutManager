using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs {
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
			this.label1 = new Label();
			this.textBoxName = new TextBox();
			this.checkBoxApply = new CheckBox();
			this.eventScriptEditor = new LayoutManager.CommonUI.Controls.EventScriptEditor();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.checkBoxGlobalPolicy = new CheckBox();
			this.checkBoxShowInMenu = new CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxName
			// 
			this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxName.Location = new System.Drawing.Point(64, 11);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(240, 20);
			this.textBoxName.TabIndex = 1;
			this.textBoxName.Text = "";
			// 
			// checkBoxApply
			// 
			this.checkBoxApply.Location = new System.Drawing.Point(22, 36);
			this.checkBoxApply.Name = "checkBoxApply";
			this.checkBoxApply.Size = new System.Drawing.Size(104, 17);
			this.checkBoxApply.TabIndex = 2;
			this.checkBoxApply.Text = "Apply this policy";
			// 
			// eventScriptEditor
			// 
			this.eventScriptEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.eventScriptEditor.EventScriptElement = null;
			this.eventScriptEditor.Location = new System.Drawing.Point(0, 88);
			this.eventScriptEditor.Name = "eventScriptEditor";
			this.eventScriptEditor.Size = new System.Drawing.Size(336, 232);
			this.eventScriptEditor.TabIndex = 5;
			this.eventScriptEditor.ViewOnly = false;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(208, 328);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(56, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(272, 328);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(56, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// checkBoxGlobalPolicy
			// 
			this.checkBoxGlobalPolicy.Location = new System.Drawing.Point(22, 53);
			this.checkBoxGlobalPolicy.Name = "checkBoxGlobalPolicy";
			this.checkBoxGlobalPolicy.Size = new System.Drawing.Size(248, 18);
			this.checkBoxGlobalPolicy.TabIndex = 3;
			this.checkBoxGlobalPolicy.Text = "This policy is available in all layouts";
			// 
			// checkBoxShowInMenu
			// 
			this.checkBoxShowInMenu.Location = new System.Drawing.Point(22, 72);
			this.checkBoxShowInMenu.Name = "checkBoxShowInMenu";
			this.checkBoxShowInMenu.Size = new System.Drawing.Size(250, 16);
			this.checkBoxShowInMenu.TabIndex = 4;
			this.checkBoxShowInMenu.Text = "Show policy in \"Tools\" menu";
			// 
			// PolicyDefinition
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(336, 358);
			this.ControlBox = false;
			this.Controls.Add(this.checkBoxShowInMenu);
			this.Controls.Add(this.checkBoxGlobalPolicy);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.eventScriptEditor);
			this.Controls.Add(this.checkBoxApply);
			this.Controls.Add(this.textBoxName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Name = "PolicyDefinition";
			this.ShowInTaskbar = false;
			this.Text = "Policy Definition";
			this.ResumeLayout(false);

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
