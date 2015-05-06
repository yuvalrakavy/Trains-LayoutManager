using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Summary description for TestLocoPainter.
	/// </summary>
	public class TestLocoPainter : Form {
		private GroupBox groupBox1;
		private RadioButton radioButtonFrontT;
		private RadioButton radioButtonFrontB;
		private RadioButton radioButtonFrontR;
		private RadioButton radioButtonFrontL;
		private GroupBox groupBox2;
		private RadioButton radioButtonDirectionForward;
		private RadioButton radioButtonDirectionBackward;
		private GroupBox groupBox3;
		private RadioButton radioButtonOrientationForward;
		private RadioButton radioButtonOrientationBackward;
		private Label label1;
		private TextBox textBoxLabel;
		private Button buttonClose;
		private Panel panel;
		private Label label2;
		private NumericUpDown numericUpDownSpeed;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		LayoutManager.View.LocomotivePainter	locoPainter = new LayoutManager.View.LocomotivePainter();

		public TestLocoPainter()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			radioButtonDirectionForward.Checked = true;
			radioButtonFrontR.Checked = true;
			radioButtonOrientationForward.Checked = true;
		}

		RadioButton findRadioButton(Control where, String name) {
			if(where.Controls.Count == 0)
				return null;

			foreach(Control c in where.Controls) {
				if(c is RadioButton && c.Name == name)
					return (RadioButton)c;

				Control	result = findRadioButton(c, name);
				if(result != null)
					return (RadioButton)result;
			}

			return null;
		}

		object GetRadio(String genericName, Type enumType) {
			String[]	valueNames = Enum.GetNames(enumType);
			Array		values = Enum.GetValues(enumType);

			for(int i = 0; i < valueNames.Length; i++) {
				RadioButton		r = findRadioButton(this, genericName + valueNames[i]);

				if(r.Checked)
					return values.GetValue(i);
			}

			throw new ApplicationException("No checked radio control found in " + genericName);
		}

		protected void UpdateLocoPainter(Object sender, EventArgs e) {
			locoPainter.DrawFront = true;
			locoPainter.Front = (LayoutComponentConnectionPoint)GetRadio("radioButtonFront", typeof(LayoutComponentConnectionPoint));
			locoPainter.Orientation = (LocomotiveOrientation)GetRadio("radioButtonOrientation", typeof(LocomotiveOrientation));
			locoPainter.Speed = (int)numericUpDownSpeed.Value;
			locoPainter.Label = textBoxLabel.Text;

			panel.Invalidate();
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
			this.groupBox1 = new GroupBox();
			this.radioButtonFrontT = new RadioButton();
			this.radioButtonFrontB = new RadioButton();
			this.radioButtonFrontR = new RadioButton();
			this.radioButtonFrontL = new RadioButton();
			this.groupBox2 = new GroupBox();
			this.radioButtonDirectionForward = new RadioButton();
			this.radioButtonDirectionBackward = new RadioButton();
			this.groupBox3 = new GroupBox();
			this.radioButtonOrientationForward = new RadioButton();
			this.radioButtonOrientationBackward = new RadioButton();
			this.label1 = new Label();
			this.textBoxLabel = new TextBox();
			this.buttonClose = new Button();
			this.panel = new Panel();
			this.label2 = new Label();
			this.numericUpDownSpeed = new NumericUpDown();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpeed)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.radioButtonFrontT,
																					this.radioButtonFrontB,
																					this.radioButtonFrontR,
																					this.radioButtonFrontL});
			this.groupBox1.Location = new System.Drawing.Point(8, 152);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(136, 112);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Front";
			// 
			// radioButtonFrontT
			// 
			this.radioButtonFrontT.Location = new System.Drawing.Point(8, 16);
			this.radioButtonFrontT.Name = "radioButtonFrontT";
			this.radioButtonFrontT.Size = new System.Drawing.Size(104, 16);
			this.radioButtonFrontT.TabIndex = 0;
			this.radioButtonFrontT.Text = "Top";
			this.radioButtonFrontT.CheckedChanged += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// radioButtonFrontB
			// 
			this.radioButtonFrontB.Location = new System.Drawing.Point(8, 40);
			this.radioButtonFrontB.Name = "radioButtonFrontB";
			this.radioButtonFrontB.Size = new System.Drawing.Size(104, 16);
			this.radioButtonFrontB.TabIndex = 1;
			this.radioButtonFrontB.Text = "Bottom";
			this.radioButtonFrontB.Click += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// radioButtonFrontR
			// 
			this.radioButtonFrontR.Location = new System.Drawing.Point(8, 64);
			this.radioButtonFrontR.Name = "radioButtonFrontR";
			this.radioButtonFrontR.Size = new System.Drawing.Size(104, 16);
			this.radioButtonFrontR.TabIndex = 2;
			this.radioButtonFrontR.Text = "Right";
			this.radioButtonFrontR.Click += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// radioButtonFrontL
			// 
			this.radioButtonFrontL.Location = new System.Drawing.Point(8, 88);
			this.radioButtonFrontL.Name = "radioButtonFrontL";
			this.radioButtonFrontL.Size = new System.Drawing.Size(104, 16);
			this.radioButtonFrontL.TabIndex = 3;
			this.radioButtonFrontL.Text = "Left";
			this.radioButtonFrontL.Click += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.radioButtonDirectionForward,
																					this.radioButtonDirectionBackward});
			this.groupBox2.Location = new System.Drawing.Point(152, 152);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(112, 64);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Direction";
			// 
			// radioButtonDirectionForward
			// 
			this.radioButtonDirectionForward.Location = new System.Drawing.Point(8, 16);
			this.radioButtonDirectionForward.Name = "radioButtonDirectionForward";
			this.radioButtonDirectionForward.Size = new System.Drawing.Size(88, 16);
			this.radioButtonDirectionForward.TabIndex = 0;
			this.radioButtonDirectionForward.Text = "Forward";
			this.radioButtonDirectionForward.Click += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// radioButtonDirectionBackward
			// 
			this.radioButtonDirectionBackward.Location = new System.Drawing.Point(8, 40);
			this.radioButtonDirectionBackward.Name = "radioButtonDirectionBackward";
			this.radioButtonDirectionBackward.Size = new System.Drawing.Size(88, 16);
			this.radioButtonDirectionBackward.TabIndex = 1;
			this.radioButtonDirectionBackward.Text = "Backward";
			this.radioButtonDirectionBackward.Click += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.radioButtonOrientationForward,
																					this.radioButtonOrientationBackward});
			this.groupBox3.Location = new System.Drawing.Point(272, 152);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(112, 64);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Orientation";
			// 
			// radioButtonOrientationForward
			// 
			this.radioButtonOrientationForward.Location = new System.Drawing.Point(8, 16);
			this.radioButtonOrientationForward.Name = "radioButtonOrientationForward";
			this.radioButtonOrientationForward.Size = new System.Drawing.Size(88, 16);
			this.radioButtonOrientationForward.TabIndex = 0;
			this.radioButtonOrientationForward.Text = "Normal";
			this.radioButtonOrientationForward.Click += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// radioButtonOrientationBackward
			// 
			this.radioButtonOrientationBackward.Location = new System.Drawing.Point(8, 40);
			this.radioButtonOrientationBackward.Name = "radioButtonOrientationBackward";
			this.radioButtonOrientationBackward.Size = new System.Drawing.Size(88, 16);
			this.radioButtonOrientationBackward.TabIndex = 1;
			this.radioButtonOrientationBackward.Text = "Reverse";
			this.radioButtonOrientationBackward.Click += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(152, 222);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 24);
			this.label1.TabIndex = 4;
			this.label1.Text = "Label:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxLabel
			// 
			this.textBoxLabel.Location = new System.Drawing.Point(200, 224);
			this.textBoxLabel.Name = "textBoxLabel";
			this.textBoxLabel.Size = new System.Drawing.Size(184, 20);
			this.textBoxLabel.TabIndex = 5;
			this.textBoxLabel.Text = "";
			this.textBoxLabel.TextChanged += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// buttonClose
			// 
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(304, 256);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 8;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// panel
			// 
			this.panel.Location = new System.Drawing.Point(8, 16);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(376, 128);
			this.panel.TabIndex = 0;
			this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(152, 248);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 24);
			this.label2.TabIndex = 6;
			this.label2.Text = "Speed:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownSpeed
			// 
			this.numericUpDownSpeed.Location = new System.Drawing.Point(200, 250);
			this.numericUpDownSpeed.Name = "numericUpDownSpeed";
			this.numericUpDownSpeed.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownSpeed.TabIndex = 7;
			this.numericUpDownSpeed.TextChanged += new System.EventHandler(this.UpdateLocoPainter);
			// 
			// TestLocoPainter
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(392, 286);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.numericUpDownSpeed,
																		  this.panel,
																		  this.buttonClose,
																		  this.textBoxLabel,
																		  this.label1,
																		  this.groupBox2,
																		  this.groupBox1,
																		  this.groupBox3,
																		  this.label2});
			this.Name = "TestLocoPainter";
			this.Text = "Test Loco Painter";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpeed)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void panel_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			e.Graphics.TranslateTransform(panel.Width / 2, panel.Height / 2);
			locoPainter.Draw(e.Graphics);
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}
	}
}
