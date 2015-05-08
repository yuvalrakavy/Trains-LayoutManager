using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Dialogs
{
	/// <summary>
	/// Summary description for EditMotionRamp.
	/// </summary>
	public class EditMotionRamp : Form {
		private LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor motionRampEditor;
		private Button buttonOK;
		private Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public EditMotionRamp(MotionRampInfo ramp)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			motionRampEditor.Ramp = ramp;
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
            this.motionRampEditor = new LayoutManager.CommonUI.Controls.MotionRampWithCopyEditor();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // motionRampEditor
            // 
            this.motionRampEditor.Location = new System.Drawing.Point(26, 19);
            this.motionRampEditor.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.motionRampEditor.Name = "motionRampEditor";
            this.motionRampEditor.Ramp = null;
            this.motionRampEditor.Size = new System.Drawing.Size(742, 305);
            this.motionRampEditor.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(768, 38);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(179, 55);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(768, 114);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(179, 55);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // EditMotionRamp
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(973, 339);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.motionRampEditor);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.Name = "EditMotionRamp";
            this.Text = "Edit Acceleration/Deceleration Profile";
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(!motionRampEditor.ValidateValues())
				return;

			motionRampEditor.Commit();
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
