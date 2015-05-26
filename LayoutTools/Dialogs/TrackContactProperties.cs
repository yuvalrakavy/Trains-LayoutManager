using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;
using System.Xml;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TrackContactProperties.
	/// </summary>
	public class TrackContactProperties : Form, ILayoutComponentPropertiesDialog
	{
		private TabPage tabPageAddress;
		private TabControl tabControl;
		private Button buttonOK;
		private Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;

		private TabPage tabPageAttributes;
		private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
        private Label label1;
        private CheckBox checkBoxEmergencyContact;
        LayoutXmlInfo xmlInfo;

		public TrackContactProperties(ModelComponent component) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.xmlInfo = new LayoutXmlInfo(component);

			nameDefinition.XmlInfo = xmlInfo;
			nameDefinition.IsOptional = true;
			nameDefinition.Component = component;

            checkBoxEmergencyContact.Checked = IsEmergencyContact;

			attributesEditor.AttributesSource = typeof(LayoutTrackContactComponent);
			attributesEditor.AttributesOwner = new AttributesOwner(xmlInfo.Element);
		}

        bool IsEmergencyContact {
            get { return xmlInfo.Element.HasAttribute(LayoutTrackContactComponent.EmergencyContactAttribute) ? XmlConvert.ToBoolean(xmlInfo.Element.GetAttribute(LayoutTrackContactComponent.EmergencyContactAttribute)) : false; }

            set {
                if (value == false)
                    XmlInfo.Element.RemoveAttribute(LayoutTrackContactComponent.EmergencyContactAttribute);
                else
                    XmlInfo.Element.SetAttribute(LayoutTrackContactComponent.EmergencyContactAttribute, XmlConvert.ToString(value));
            }
        }

        public LayoutXmlInfo XmlInfo => xmlInfo;

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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageAddress = new System.Windows.Forms.TabPage();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.tabPageAttributes = new System.Windows.Forms.TabPage();
            this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
            this.checkBoxEmergencyContact = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageAddress.SuspendLayout();
            this.tabPageAttributes.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 272);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(120, 272);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageAddress);
            this.tabControl.Controls.Add(this.tabPageAttributes);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(290, 264);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl.TabIndex = 0;
            // 
            // tabPageAddress
            // 
            this.tabPageAddress.Controls.Add(this.label1);
            this.tabPageAddress.Controls.Add(this.checkBoxEmergencyContact);
            this.tabPageAddress.Controls.Add(this.nameDefinition);
            this.tabPageAddress.Location = new System.Drawing.Point(4, 22);
            this.tabPageAddress.Name = "tabPageAddress";
            this.tabPageAddress.Size = new System.Drawing.Size(282, 238);
            this.tabPageAddress.TabIndex = 0;
            this.tabPageAddress.Text = "General";
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(8, 8);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(264, 64);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // tabPageAttributes
            // 
            this.tabPageAttributes.Controls.Add(this.attributesEditor);
            this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttributes.Name = "tabPageAttributes";
            this.tabPageAttributes.Size = new System.Drawing.Size(282, 238);
            this.tabPageAttributes.TabIndex = 2;
            this.tabPageAttributes.Text = "Attributes";
            // 
            // attributesEditor
            // 
            this.attributesEditor.AttributesOwner = null;
            this.attributesEditor.AttributesSource = null;
            this.attributesEditor.Location = new System.Drawing.Point(0, 8);
            this.attributesEditor.Name = "attributesEditor";
            this.attributesEditor.Size = new System.Drawing.Size(280, 224);
            this.attributesEditor.TabIndex = 0;
            this.attributesEditor.ViewOnly = false;
            // 
            // checkBoxEmergencyContact
            // 
            this.checkBoxEmergencyContact.AutoSize = true;
            this.checkBoxEmergencyContact.Location = new System.Drawing.Point(8, 78);
            this.checkBoxEmergencyContact.Name = "checkBoxEmergencyContact";
            this.checkBoxEmergencyContact.Size = new System.Drawing.Size(118, 17);
            this.checkBoxEmergencyContact.TabIndex = 1;
            this.checkBoxEmergencyContact.Text = "Emergency contact";
            this.checkBoxEmergencyContact.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = " (triggering this contact  will power off the whole layout)";
            // 
            // TrackContactProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(290, 306);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrackContactProperties";
            this.ShowInTaskbar = false;
            this.Text = "Track Contact Properties";
            this.tabControl.ResumeLayout(false);
            this.tabPageAddress.ResumeLayout(false);
            this.tabPageAddress.PerformLayout();
            this.tabPageAttributes.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(nameDefinition.Commit() && attributesEditor.Commit()) {
				this.DialogResult = DialogResult.OK;
				Close();
			}

            IsEmergencyContact = checkBoxEmergencyContact.Checked;
		}
	}
}
