using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LayoutManager.Components;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetNoSpaceSelection.
    /// </summary>
    public class GetNoSpaceSelection : Form {
		private Label label2;
		private Label label3;
		private Label labelTitle;
		private RadioButton radioButtonUseModuleLocation;
		private ComboBox comboBoxModuleLocation;
		private Button buttonOK;
		private Button buttonCancel;
		private RadioButton radioButtonAddNewModule;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public GetNoSpaceSelection(LayoutControlModuleLocationComponent noSpaceLocation, IEnumerable<LayoutControlModuleLocationComponent> otherModuleLocations)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			labelTitle.Text = Regex.Replace(labelTitle.Text, "NAME", noSpaceLocation.Name);

			foreach(LayoutControlModuleLocationComponent moduleLocation in otherModuleLocations)
				if(moduleLocation == null)
					comboBoxModuleLocation.Items.Add(new Item("(Not in any location)"));
				else
					comboBoxModuleLocation.Items.Add(moduleLocation.Name);

			if(comboBoxModuleLocation.Items.Count > 0)
				comboBoxModuleLocation.SelectedIndex = 0;

			radioButtonAddNewModule.Checked = true;

			updateButtons();
		}

		private void updateButtons() {
			comboBoxModuleLocation.Enabled = !radioButtonAddNewModule.Checked;
		}

        public bool AddNewModule => radioButtonAddNewModule.Checked;

        public LayoutControlModuleLocationComponent ModuleLocation {
			get {
				if(comboBoxModuleLocation.SelectedItem == null)
					return null;
				return ((Item)comboBoxModuleLocation.SelectedItem).ModuleLocation;
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
			this.labelTitle = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.radioButtonUseModuleLocation = new RadioButton();
			this.comboBoxModuleLocation = new ComboBox();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.radioButtonAddNewModule = new RadioButton();
			this.SuspendLayout();
			// 
			// labelTitle
			// 
			this.labelTitle.Location = new System.Drawing.Point(16, 8);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(256, 48);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "There is no free control connection point in the selected control module location" +
				" (NAME), to which the component can be connected.";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(256, 32);
			this.label2.TabIndex = 0;
			this.label2.Text = "However there are free control connection points in other areas. What would you l" +
				"ike to do:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 95);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(256, 24);
			this.label3.TabIndex = 0;
			this.label3.Text = "What would you like to do:";
			// 
			// radioButtonUseModuleLocation
			// 
			this.radioButtonUseModuleLocation.Location = new System.Drawing.Point(16, 138);
			this.radioButtonUseModuleLocation.Name = "radioButtonUseModuleLocation";
			this.radioButtonUseModuleLocation.Size = new System.Drawing.Size(256, 32);
			this.radioButtonUseModuleLocation.TabIndex = 1;
			this.radioButtonUseModuleLocation.Text = "Connect the component to a control module in control module location:";
			this.radioButtonUseModuleLocation.CheckedChanged += new System.EventHandler(this.radioButtonUseModuleLocation_CheckedChanged);
			// 
			// comboBoxModuleLocation
			// 
			this.comboBoxModuleLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxModuleLocation.Location = new System.Drawing.Point(32, 173);
			this.comboBoxModuleLocation.Name = "comboBoxModuleLocation";
			this.comboBoxModuleLocation.Size = new System.Drawing.Size(176, 21);
			this.comboBoxModuleLocation.Sorted = true;
			this.comboBoxModuleLocation.TabIndex = 2;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(128, 208);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "Continue";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(208, 208);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			// 
			// radioButtonAddNewModule
			// 
			this.radioButtonAddNewModule.Location = new System.Drawing.Point(16, 115);
			this.radioButtonAddNewModule.Name = "radioButtonAddNewModule";
			this.radioButtonAddNewModule.Size = new System.Drawing.Size(152, 24);
			this.radioButtonAddNewModule.TabIndex = 9;
			this.radioButtonAddNewModule.Text = "Add new control module";
			this.radioButtonAddNewModule.CheckedChanged += new System.EventHandler(this.radioButtonAddNewModule_CheckedChanged);
			// 
			// GetNoSpaceSelection
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(292, 238);
			this.ControlBox = false;
			this.Controls.Add(this.radioButtonAddNewModule);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.comboBoxModuleLocation);
			this.Controls.Add(this.radioButtonUseModuleLocation);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GetNoSpaceSelection";
			this.ShowInTaskbar = false;
			this.Text = "No Free Connection Point";
			this.ResumeLayout(false);

		}
		#endregion

		private void radioButtonAddNewModule_CheckedChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void radioButtonUseModuleLocation_CheckedChanged(object sender, System.EventArgs e) {
			updateButtons();
		}

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
		}

		class Item {
			LayoutControlModuleLocationComponent	moduleLocation;
			string									text;

			public Item(LayoutControlModuleLocationComponent moduleLocation) {
				this.moduleLocation = moduleLocation;
				text = moduleLocation.Name;
			}

			public Item(string text) {
				this.moduleLocation = null;
				this.text = text;
			}

            public LayoutControlModuleLocationComponent ModuleLocation => moduleLocation;

            public override string ToString() => text;
        }
	}
}
