namespace LayoutManager.Tools.Dialogs {
	partial class SimulateCommandStationInputEvent {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxCommandStation = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxBus = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textBoxAddress = new System.Windows.Forms.TextBox();
			this.textBoxState = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.textBoxIndex = new System.Windows.Forms.TextBox();
			this.labelIndex = new System.Windows.Forms.Label();
			this.SuspendLayout();
// 
// label1
// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Command station:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
// 
// comboBoxCommandStation
// 
			this.comboBoxCommandStation.DisplayMember = "Name";
			this.comboBoxCommandStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCommandStation.FormattingEnabled = true;
			this.comboBoxCommandStation.Location = new System.Drawing.Point(116, 10);
			this.comboBoxCommandStation.Name = "comboBoxCommandStation";
			this.comboBoxCommandStation.Size = new System.Drawing.Size(168, 21);
			this.comboBoxCommandStation.TabIndex = 1;
			this.comboBoxCommandStation.SelectedIndexChanged += new System.EventHandler(this.comboBoxCommandStation_SelectedIndexChanged);
// 
// label2
// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(44, 41);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Connection:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
// 
// comboBoxBus
// 
			this.comboBoxBus.DisplayMember = "Name";
			this.comboBoxBus.FormattingEnabled = true;
			this.comboBoxBus.Location = new System.Drawing.Point(116, 38);
			this.comboBoxBus.Name = "comboBoxBus";
			this.comboBoxBus.Size = new System.Drawing.Size(100, 21);
			this.comboBoxBus.TabIndex = 3;
			this.comboBoxBus.SelectedIndexChanged += new System.EventHandler(this.comboBoxBus_SelectedIndexChanged);
// 
// label3
// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(60, 69);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Address:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
// 
// textBoxAddress
// 
			this.textBoxAddress.Location = new System.Drawing.Point(116, 66);
			this.textBoxAddress.Name = "textBoxAddress";
			this.textBoxAddress.Size = new System.Drawing.Size(54, 20);
			this.textBoxAddress.TabIndex = 5;
// 
// textBoxState
// 
			this.textBoxState.Location = new System.Drawing.Point(116, 93);
			this.textBoxState.Name = "textBoxState";
			this.textBoxState.Size = new System.Drawing.Size(44, 20);
			this.textBoxState.TabIndex = 9;
// 
// label4
// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(75, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 14);
			this.label4.TabIndex = 8;
			this.label4.Text = "State:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
// 
// buttonOK
// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(280, 63);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 10;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
// 
// buttonCancel
// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(280, 93);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 11;
			this.buttonCancel.Text = "Cancel";
// 
// textBoxIndex
// 
			this.textBoxIndex.Location = new System.Drawing.Point(219, 66);
			this.textBoxIndex.Name = "textBoxIndex";
			this.textBoxIndex.Size = new System.Drawing.Size(54, 20);
			this.textBoxIndex.TabIndex = 7;
// 
// labelIndex
// 
			this.labelIndex.AutoSize = true;
			this.labelIndex.Location = new System.Drawing.Point(178, 69);
			this.labelIndex.Name = "labelIndex";
			this.labelIndex.Size = new System.Drawing.Size(35, 14);
			this.labelIndex.TabIndex = 6;
			this.labelIndex.Text = "Index:";
			this.labelIndex.TextAlign = System.Drawing.ContentAlignment.TopRight;
// 
// SimulateCommandStationInputEvent
// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(367, 124);
			this.ControlBox = false;
			this.Controls.Add(this.textBoxIndex);
			this.Controls.Add(this.labelIndex);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.textBoxState);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBoxAddress);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBoxBus);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxCommandStation);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SimulateCommandStationInputEvent";
			this.Text = "Command Station Input";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxCommandStation;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxBus;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxAddress;
		private System.Windows.Forms.TextBox textBoxState;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxIndex;
		private System.Windows.Forms.Label labelIndex;
	}
}