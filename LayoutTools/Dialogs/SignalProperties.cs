using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrackContactProperties.
    /// </summary>
    public class SignalProperties : Form, ILayoutComponentPropertiesDialog
	{
		private TabPage tabPageAddress;
		private TabControl tabControl;
		private Button buttonOK;
		private Button buttonCancel;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private GroupBox groupBox1;
		private RadioButton radioButtonSignalTypeLights;
		private RadioButton radioButtonSignalTypeSemaphore;
		private RadioButton radioButtonSignalTypeDistance;
		private LayoutManager.CommonUI.Controls.PictureBoxWithTransparency pictureBoxWithTransparency1;
		private LayoutManager.CommonUI.Controls.PictureBoxWithTransparency pictureBoxWithTransparency2;
		private LayoutManager.CommonUI.Controls.PictureBoxWithTransparency pictureBoxWithTransparency3;
		private CheckBox checkBoxReverseLogic;
		LayoutXmlInfo	xmlInfo;

		public SignalProperties(ModelComponent component) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.xmlInfo = new LayoutXmlInfo(component);

			LayoutSignalComponentInfo	info = new LayoutSignalComponentInfo(xmlInfo.Element);

			switch(info.SignalType) {
				case LayoutSignalType.Distance:
					radioButtonSignalTypeDistance.Checked = true;
					break;

				case LayoutSignalType.Lights:
					radioButtonSignalTypeLights.Checked = true;
					break;

				case LayoutSignalType.Semaphore:
					radioButtonSignalTypeSemaphore.Checked = true;
					break;
			}

			checkBoxReverseLogic.Checked = info.ReverseLogic;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignalProperties));
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.tabControl = new TabControl();
			this.tabPageAddress = new TabPage();
			this.groupBox1 = new GroupBox();
			this.radioButtonSignalTypeLights = new RadioButton();
			this.radioButtonSignalTypeSemaphore = new RadioButton();
			this.radioButtonSignalTypeDistance = new RadioButton();
			this.checkBoxReverseLogic = new CheckBox();
			this.pictureBoxWithTransparency1 = new LayoutManager.CommonUI.Controls.PictureBoxWithTransparency();
			this.pictureBoxWithTransparency2 = new LayoutManager.CommonUI.Controls.PictureBoxWithTransparency();
			this.pictureBoxWithTransparency3 = new LayoutManager.CommonUI.Controls.PictureBoxWithTransparency();
			this.tabControl.SuspendLayout();
			this.tabPageAddress.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithTransparency1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithTransparency2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithTransparency3)).BeginInit();
			this.SuspendLayout();
// 
// buttonCancel
// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(208, 272);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
// 
// buttonOK
// 
			this.buttonOK.Location = new System.Drawing.Point(120, 272);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
// 
// tabControl
// 
			this.tabControl.Controls.Add(this.tabPageAddress);
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
			this.tabPageAddress.Controls.Add(this.checkBoxReverseLogic);
			this.tabPageAddress.Controls.Add(this.groupBox1);
			this.tabPageAddress.Location = new System.Drawing.Point(4, 22);
			this.tabPageAddress.Name = "tabPageAddress";
			this.tabPageAddress.Size = new System.Drawing.Size(282, 238);
			this.tabPageAddress.TabIndex = 0;
			this.tabPageAddress.Text = "General";
// 
// groupBox1
// 
			this.groupBox1.Controls.Add(this.pictureBoxWithTransparency1);
			this.groupBox1.Controls.Add(this.radioButtonSignalTypeLights);
			this.groupBox1.Controls.Add(this.radioButtonSignalTypeSemaphore);
			this.groupBox1.Controls.Add(this.radioButtonSignalTypeDistance);
			this.groupBox1.Controls.Add(this.pictureBoxWithTransparency2);
			this.groupBox1.Controls.Add(this.pictureBoxWithTransparency3);
			this.groupBox1.Location = new System.Drawing.Point(9, 13);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(256, 112);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Signal Type:";
// 
// radioButtonSignalTypeLights
// 
			this.radioButtonSignalTypeLights.Location = new System.Drawing.Point(56, 16);
			this.radioButtonSignalTypeLights.Name = "radioButtonSignalTypeLights";
			this.radioButtonSignalTypeLights.TabIndex = 0;
			this.radioButtonSignalTypeLights.Text = "Lights";
// 
// radioButtonSignalTypeSemaphore
// 
			this.radioButtonSignalTypeSemaphore.Location = new System.Drawing.Point(56, 48);
			this.radioButtonSignalTypeSemaphore.Name = "radioButtonSignalTypeSemaphore";
			this.radioButtonSignalTypeSemaphore.TabIndex = 0;
			this.radioButtonSignalTypeSemaphore.Text = "Semaphore";
// 
// radioButtonSignalTypeDistance
// 
			this.radioButtonSignalTypeDistance.Location = new System.Drawing.Point(56, 80);
			this.radioButtonSignalTypeDistance.Name = "radioButtonSignalTypeDistance";
			this.radioButtonSignalTypeDistance.TabIndex = 0;
			this.radioButtonSignalTypeDistance.Text = "Distance signal";
// 
// checkBoxReverseLogic
// 
			this.checkBoxReverseLogic.AutoSize = true;
			this.checkBoxReverseLogic.Location = new System.Drawing.Point(26, 132);
			this.checkBoxReverseLogic.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
			this.checkBoxReverseLogic.Name = "checkBoxReverseLogic";
			this.checkBoxReverseLogic.Size = new System.Drawing.Size(87, 17);
			this.checkBoxReverseLogic.TabIndex = 3;
			this.checkBoxReverseLogic.Text = "Reverse logic";
// 
// pictureBoxWithTransparency1
// 
			this.pictureBoxWithTransparency1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxWithTransparency1.Image")));
			this.pictureBoxWithTransparency1.Location = new System.Drawing.Point(17, 21);
			this.pictureBoxWithTransparency1.Name = "pictureBoxWithTransparency1";
			this.pictureBoxWithTransparency1.Size = new System.Drawing.Size(23, 19);
			this.pictureBoxWithTransparency1.TabIndex = 1;
			this.pictureBoxWithTransparency1.TabStop = false;
			this.pictureBoxWithTransparency1.TransparentColor = System.Drawing.Color.Cyan;
// 
// pictureBoxWithTransparency2
// 
			this.pictureBoxWithTransparency2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxWithTransparency2.Image")));
			this.pictureBoxWithTransparency2.Location = new System.Drawing.Point(17, 48);
			this.pictureBoxWithTransparency2.Name = "pictureBoxWithTransparency2";
			this.pictureBoxWithTransparency2.Size = new System.Drawing.Size(23, 19);
			this.pictureBoxWithTransparency2.TabIndex = 1;
			this.pictureBoxWithTransparency2.TabStop = false;
			this.pictureBoxWithTransparency2.TransparentColor = System.Drawing.Color.Cyan;
// 
// pictureBoxWithTransparency3
// 
			this.pictureBoxWithTransparency3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxWithTransparency3.Image")));
			this.pictureBoxWithTransparency3.Location = new System.Drawing.Point(17, 80);
			this.pictureBoxWithTransparency3.Name = "pictureBoxWithTransparency3";
			this.pictureBoxWithTransparency3.Size = new System.Drawing.Size(23, 19);
			this.pictureBoxWithTransparency3.TabIndex = 1;
			this.pictureBoxWithTransparency3.TabStop = false;
			this.pictureBoxWithTransparency3.TransparentColor = System.Drawing.Color.Cyan;
// 
// SignalProperties
// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.ClientSize = new System.Drawing.Size(290, 306);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SignalProperties";
			this.ShowInTaskbar = false;
			this.Text = "Signal Properties";
			this.tabControl.ResumeLayout(false);
			this.tabPageAddress.ResumeLayout(false);
			this.tabPageAddress.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithTransparency1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithTransparency2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxWithTransparency3)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			LayoutSignalComponentInfo	info = new LayoutSignalComponentInfo(xmlInfo.Element);

			if(radioButtonSignalTypeDistance.Checked)
				info.SignalType = LayoutSignalType.Distance;
			else if(radioButtonSignalTypeLights.Checked)
				info.SignalType = LayoutSignalType.Lights;
			else if(radioButtonSignalTypeSemaphore.Checked)
				info.SignalType = LayoutSignalType.Semaphore;

			info.ReverseLogic = checkBoxReverseLogic.Checked;

			this.DialogResult = DialogResult.OK;
			Close();
		}
	}
}
