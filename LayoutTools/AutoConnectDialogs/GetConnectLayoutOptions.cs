using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Linq;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Tools.AutoConnectDialogs
{
	/// <summary>
	/// Summary description for GetConnectLayoutOptions.
	/// </summary>
	public class GetConnectLayoutOptions : Form {
		private Label label1;
		private RadioButton radioButtonConnectNotConnected;
		private RadioButton radioButtonConnectAllLayout;
		private Label label2;
		private Button buttonOK;
		private Button buttonCancel;
		private GroupBox groupBoxWarning;
		private CheckBox checkBoxSetUserActionRequired;
		private GroupBox groupBox1;
		private RadioButton radioButtonScopeOperational;
		private RadioButton radioButtonScopNotPlanned;
		private RadioButton radioButtonScopeAll;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private CheckBox checkBoxSetProgrammingRequired;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public GetConnectLayoutOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			radioButtonConnectNotConnected.Checked = true;
			checkBoxSetUserActionRequired.Checked = true;
			radioButtonConnectAllLayout.Checked = true;
			checkBoxSetProgrammingRequired.Checked = true;
			checkBoxSetUserActionRequired.Checked = true;

			updateButtons();
		}

		public bool ConnectAllLayout {
			get {
				return radioButtonConnectAllLayout.Checked;
			}
		}

		public bool SetUserActionRequired {
			get {
				return checkBoxSetUserActionRequired.Checked;
			}
		}

		public bool SetProgrammingRequired {
			get {
				return checkBoxSetProgrammingRequired.Checked;
			}
		}

		public LayoutPhase Phase {
			get {
				if(radioButtonScopeAll.Checked)
					return LayoutPhase.All;
				if(radioButtonScopNotPlanned.Checked)
					return LayoutPhase.NotPlanned;
				else if(radioButtonScopeOperational.Checked)
					return LayoutPhase.Operational;
				else
					throw new ApplicationException("Cannot figure connect scope");
			}
		}

		private void updateButtons() {
			if(radioButtonScopeOperational.Checked) { 
				var anyOperationalSpots = LayoutModel.Components<IModelComponentConnectToControl>(Phase).Select(c => c.Spot).Where(s => s.Phase == LayoutPhase.Operational).Any();

				groupBoxWarning.Visible = radioButtonConnectAllLayout.Checked && anyOperationalSpots;
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
			this.radioButtonConnectNotConnected = new RadioButton();
			this.radioButtonConnectAllLayout = new RadioButton();
			this.groupBoxWarning = new GroupBox();
			this.label2 = new Label();
			this.buttonOK = new Button();
			this.buttonCancel = new Button();
			this.checkBoxSetUserActionRequired = new CheckBox();
			this.groupBox1 = new GroupBox();
			this.radioButtonScopeOperational = new RadioButton();
			this.radioButtonScopNotPlanned = new RadioButton();
			this.radioButtonScopeAll = new RadioButton();
			this.groupBox2 = new GroupBox();
			this.groupBox3 = new GroupBox();
			this.checkBoxSetProgrammingRequired = new CheckBox();
			this.groupBoxWarning.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(240, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please choose one from the following options:";
			// 
			// radioButtonConnectNotConnected
			// 
			this.radioButtonConnectNotConnected.Location = new System.Drawing.Point(11, 13);
			this.radioButtonConnectNotConnected.Name = "radioButtonConnectNotConnected";
			this.radioButtonConnectNotConnected.Size = new System.Drawing.Size(272, 24);
			this.radioButtonConnectNotConnected.TabIndex = 0;
			this.radioButtonConnectNotConnected.Text = "Connect component which are not yet connected";
			this.radioButtonConnectNotConnected.CheckedChanged += new System.EventHandler(this.radioButtonConnectNotConnected_CheckedChanged);
			// 
			// radioButtonConnectAllLayout
			// 
			this.radioButtonConnectAllLayout.Location = new System.Drawing.Point(11, 34);
			this.radioButtonConnectAllLayout.Name = "radioButtonConnectAllLayout";
			this.radioButtonConnectAllLayout.Size = new System.Drawing.Size(184, 24);
			this.radioButtonConnectAllLayout.TabIndex = 1;
			this.radioButtonConnectAllLayout.Text = "Reconnect all components";
			this.radioButtonConnectAllLayout.CheckedChanged += new System.EventHandler(this.radioButtonConnectAllLayout_CheckedChanged);
			// 
			// groupBoxWarning
			// 
			this.groupBoxWarning.BackColor = System.Drawing.Color.Firebrick;
			this.groupBoxWarning.Controls.Add(this.label2);
			this.groupBoxWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.groupBoxWarning.ForeColor = System.Drawing.Color.Yellow;
			this.groupBoxWarning.Location = new System.Drawing.Point(23, 55);
			this.groupBoxWarning.Name = "groupBoxWarning";
			this.groupBoxWarning.Size = new System.Drawing.Size(265, 80);
			this.groupBoxWarning.TabIndex = 2;
			this.groupBoxWarning.TabStop = false;
			this.groupBoxWarning.Text = "Warning:";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Firebrick;
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.ForeColor = System.Drawing.Color.Gold;
			this.label2.Location = new System.Drawing.Point(3, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(259, 61);
			this.label2.TabIndex = 0;
			this.label2.Text = "Choosing this option will overwrite the existing layout connections. If this is a" +
				"n existing layout, you will probably have to rewire the whole layout!";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(191, 385);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "Continue";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(272, 385);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			// 
			// checkBoxSetUserActionRequired
			// 
			this.checkBoxSetUserActionRequired.Location = new System.Drawing.Point(11, 19);
			this.checkBoxSetUserActionRequired.Name = "checkBoxSetUserActionRequired";
			this.checkBoxSetUserActionRequired.Size = new System.Drawing.Size(288, 24);
			this.checkBoxSetUserActionRequired.TabIndex = 0;
			this.checkBoxSetUserActionRequired.Text = "Set \"User Action Required\" flag";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonScopeOperational);
			this.groupBox1.Controls.Add(this.radioButtonScopNotPlanned);
			this.groupBox1.Controls.Add(this.radioButtonScopeAll);
			this.groupBox1.Location = new System.Drawing.Point(12, 35);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(335, 100);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Scope:";
			// 
			// radioButtonScopeOperational
			// 
			this.radioButtonScopeOperational.AutoSize = true;
			this.radioButtonScopeOperational.Location = new System.Drawing.Point(11, 19);
			this.radioButtonScopeOperational.Name = "radioButtonScopeOperational";
			this.radioButtonScopeOperational.Size = new System.Drawing.Size(168, 17);
			this.radioButtonScopeOperational.TabIndex = 0;
			this.radioButtonScopeOperational.TabStop = true;
			this.radioButtonScopeOperational.Text = "Only \'Operational\' components";
			this.radioButtonScopeOperational.UseVisualStyleBackColor = true;
			this.radioButtonScopeOperational.CheckedChanged += new System.EventHandler(this.radioButtonScope_CheckedChanged);
			// 
			// radioButtonScopNotPlanned
			// 
			this.radioButtonScopNotPlanned.AutoSize = true;
			this.radioButtonScopNotPlanned.Location = new System.Drawing.Point(11, 42);
			this.radioButtonScopNotPlanned.Name = "radioButtonScopNotPlanned";
			this.radioButtonScopNotPlanned.Size = new System.Drawing.Size(260, 17);
			this.radioButtonScopNotPlanned.TabIndex = 1;
			this.radioButtonScopNotPlanned.TabStop = true;
			this.radioButtonScopNotPlanned.Text = "Only \'Operational\' or \'In construction\' components ";
			this.radioButtonScopNotPlanned.UseVisualStyleBackColor = true;
			this.radioButtonScopNotPlanned.CheckedChanged += new System.EventHandler(this.radioButtonScope_CheckedChanged);
			// 
			// radioButtonScopeAll
			// 
			this.radioButtonScopeAll.AutoSize = true;
			this.radioButtonScopeAll.Location = new System.Drawing.Point(11, 65);
			this.radioButtonScopeAll.Name = "radioButtonScopeAll";
			this.radioButtonScopeAll.Size = new System.Drawing.Size(309, 17);
			this.radioButtonScopeAll.TabIndex = 2;
			this.radioButtonScopeAll.TabStop = true;
			this.radioButtonScopeAll.Text = "All components (\'Panned\', \'In construction\' and \'Operational\')";
			this.radioButtonScopeAll.UseVisualStyleBackColor = true;
			this.radioButtonScopeAll.CheckedChanged += new System.EventHandler(this.radioButtonScope_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioButtonConnectNotConnected);
			this.groupBox2.Controls.Add(this.radioButtonConnectAllLayout);
			this.groupBox2.Controls.Add(this.groupBoxWarning);
			this.groupBox2.Location = new System.Drawing.Point(12, 141);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(335, 145);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Which components to connect:";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.checkBoxSetUserActionRequired);
			this.groupBox3.Controls.Add(this.checkBoxSetProgrammingRequired);
			this.groupBox3.Location = new System.Drawing.Point(12, 292);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(335, 79);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "For new connections:";
			// 
			// checkBoxSetProgrammingRequired
			// 
			this.checkBoxSetProgrammingRequired.AutoSize = true;
			this.checkBoxSetProgrammingRequired.Location = new System.Drawing.Point(11, 49);
			this.checkBoxSetProgrammingRequired.Name = "checkBoxSetProgrammingRequired";
			this.checkBoxSetProgrammingRequired.Size = new System.Drawing.Size(182, 17);
			this.checkBoxSetProgrammingRequired.TabIndex = 1;
			this.checkBoxSetProgrammingRequired.Text = "Set \"Programming Required\" flag";
			this.checkBoxSetProgrammingRequired.UseVisualStyleBackColor = true;
			// 
			// GetConnectLayoutOptions
			// 
			this.AcceptButton = this.buttonOK;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(359, 420);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GetConnectLayoutOptions";
			this.ShowInTaskbar = false;
			this.Text = "Connect Layout";
			this.groupBoxWarning.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void radioButtonConnectNotConnected_CheckedChanged(object sender, System.EventArgs e) {
			checkBoxSetUserActionRequired.Checked = true;
			updateButtons();
		}

		private void radioButtonConnectAllLayout_CheckedChanged(object sender, System.EventArgs e) {
			checkBoxSetUserActionRequired.Checked = false;
			updateButtons();
		}

		private void buttonOK_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
		}

		private void radioButtonScope_CheckedChanged(object sender, EventArgs e) {
			updateButtons();
		}
	}
}
