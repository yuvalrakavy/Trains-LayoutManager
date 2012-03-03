using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Tools.AutoConnectDialogs
{
	/// <summary>
	/// Summary description for GetCommandStation.
	/// </summary>
	public class GetCommandStation : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ComboBox comboBoxCommandStations;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GetCommandStation(IList<IModelComponentIsCommandStation> commandStations)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			comboBoxCommandStations.DisplayMember = "Name";

			foreach(IModelComponentIsCommandStation commandStation in commandStations)
				comboBoxCommandStations.Items.Add(new Item(commandStation));
		
			if(comboBoxCommandStations.Items.Count > 0)
				comboBoxCommandStations.SelectedIndex = 0;
		}

		public IModelComponentIsCommandStation CommandStation {
			get {
				if(comboBoxCommandStations.SelectedItem == null)
					return null;
				return ((Item)comboBoxCommandStations.SelectedItem).CommandStation;
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
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxCommandStations = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(232, 48);
			this.label1.TabIndex = 0;
			this.label1.Text = "This layout has more than one command station. Please select the command station " +
				"to which you wish to connect the component:";
			// 
			// comboBoxCommandStations
			// 
			this.comboBoxCommandStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCommandStations.Location = new System.Drawing.Point(8, 64);
			this.comboBoxCommandStations.Name = "comboBoxCommandStations";
			this.comboBoxCommandStations.Size = new System.Drawing.Size(168, 21);
			this.comboBoxCommandStations.TabIndex = 1;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(88, 96);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "Continue";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(168, 96);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			// 
			// GetCommandStation
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(248, 126);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.comboBoxCommandStations);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GetCommandStation";
			this.ShowInTaskbar = false;
			this.Text = "Select Command Station";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
		}

		class Item {
			IModelComponentIsCommandStation	commandStation;

			public Item(IModelComponentIsCommandStation commandStation) {
				this.commandStation = commandStation;
			}

			public IModelComponentIsCommandStation CommandStation {
				get {
					return commandStation;
				}
			}

			public override string ToString() {
				return commandStation.NameProvider.Name.ToString();
			}
		}
	}
}
