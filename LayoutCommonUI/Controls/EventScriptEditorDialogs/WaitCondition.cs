using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs
{
	/// <summary>
	/// Summary description for WaitCondition.
	/// </summary>
	public class WaitCondition : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numericUpDownMinutes;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.NumericUpDown numericUpDownSeconds;
		private System.Windows.Forms.CheckBox checkBoxRadomWait;
		private System.Windows.Forms.NumericUpDown numericUpDownRandomSeconds;
		private System.Windows.Forms.CheckBox checkBoxErrorState;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		XmlElement	conditionElement;

		public WaitCondition(XmlElement conditionElement)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.conditionElement = conditionElement;

			if(conditionElement.HasAttribute("Minutes"))
				numericUpDownMinutes.Value = XmlConvert.ToDecimal(conditionElement.GetAttribute("Minutes"));
			if(conditionElement.HasAttribute("Seconds"))
				numericUpDownSeconds.Value = XmlConvert.ToDecimal(conditionElement.GetAttribute("Seconds"));

			if(conditionElement.HasAttribute("RandomSeconds")) {
				checkBoxRadomWait.Checked = true;
				numericUpDownRandomSeconds.Value = XmlConvert.ToDecimal(conditionElement.GetAttribute("RandomSeconds"));
			}
			else
				checkBoxRadomWait.Checked = false;

			if(conditionElement.HasAttribute("IsError"))
				checkBoxErrorState.Checked = XmlConvert.ToBoolean(conditionElement.GetAttribute("IsError"));
			else
				checkBoxErrorState.Checked = false;

			updateButtons(null, null);
		}

		private void updateButtons(object sender, EventArgs e) {
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
			this.numericUpDownMinutes = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.numericUpDownSeconds = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxRadomWait = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.numericUpDownRandomSeconds = new System.Windows.Forms.NumericUpDown();
			this.checkBoxErrorState = new System.Windows.Forms.CheckBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinutes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSeconds)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRandomSeconds)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Wait for";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownMinutes
			// 
			this.numericUpDownMinutes.Location = new System.Drawing.Point(64, 14);
			this.numericUpDownMinutes.Maximum = new System.Decimal(new int[] {
																				 1000,
																				 0,
																				 0,
																				 0});
			this.numericUpDownMinutes.Name = "numericUpDownMinutes";
			this.numericUpDownMinutes.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownMinutes.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(120, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "minutes and ";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numericUpDownSeconds
			// 
			this.numericUpDownSeconds.Location = new System.Drawing.Point(192, 14);
			this.numericUpDownSeconds.Maximum = new System.Decimal(new int[] {
																				 10000,
																				 0,
																				 0,
																				 0});
			this.numericUpDownSeconds.Name = "numericUpDownSeconds";
			this.numericUpDownSeconds.Size = new System.Drawing.Size(48, 20);
			this.numericUpDownSeconds.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(248, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(51, 16);
			this.label3.TabIndex = 4;
			this.label3.Text = "seconds";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkBoxRadomWait
			// 
			this.checkBoxRadomWait.Location = new System.Drawing.Point(15, 49);
			this.checkBoxRadomWait.Name = "checkBoxRadomWait";
			this.checkBoxRadomWait.Size = new System.Drawing.Size(217, 16);
			this.checkBoxRadomWait.TabIndex = 5;
			this.checkBoxRadomWait.Text = "then wait additional random time upto ";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(273, 49);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(51, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "seconds";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numericUpDownRandomSeconds
			// 
			this.numericUpDownRandomSeconds.Location = new System.Drawing.Point(223, 47);
			this.numericUpDownRandomSeconds.Maximum = new System.Decimal(new int[] {
																					   3600,
																					   0,
																					   0,
																					   0});
			this.numericUpDownRandomSeconds.Name = "numericUpDownRandomSeconds";
			this.numericUpDownRandomSeconds.Size = new System.Drawing.Size(43, 20);
			this.numericUpDownRandomSeconds.TabIndex = 6;
			this.numericUpDownRandomSeconds.Enter += new System.EventHandler(this.numericUpDownRandomSeconds_ValueChanged);
			// 
			// checkBoxErrorState
			// 
			this.checkBoxErrorState.Location = new System.Drawing.Point(15, 75);
			this.checkBoxErrorState.Name = "checkBoxErrorState";
			this.checkBoxErrorState.Size = new System.Drawing.Size(241, 16);
			this.checkBoxErrorState.TabIndex = 8;
			this.checkBoxErrorState.Text = "Occurence of this event indicates an error";
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(180, 106);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(64, 23);
			this.buttonOk.TabIndex = 9;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(252, 106);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(64, 23);
			this.buttonCancel.TabIndex = 10;
			this.buttonCancel.Text = "Cancel";
			// 
			// WaitCondition
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(320, 133);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonOk,
																		  this.buttonCancel,
																		  this.checkBoxErrorState,
																		  this.numericUpDownRandomSeconds,
																		  this.checkBoxRadomWait,
																		  this.label2,
																		  this.numericUpDownMinutes,
																		  this.label1,
																		  this.numericUpDownSeconds,
																		  this.label3,
																		  this.label4});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "WaitCondition";
			this.ShowInTaskbar = false;
			this.Text = "Wait";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinutes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSeconds)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRandomSeconds)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void numericUpDownRandomSeconds_ValueChanged(object sender, System.EventArgs e) {
			checkBoxRadomWait.Checked = true;
		}

		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(numericUpDownMinutes.Value != 0)
				conditionElement.SetAttribute("Minutes", XmlConvert.ToString(numericUpDownMinutes.Value));
			else
				conditionElement.RemoveAttribute("Minutes");

			if(numericUpDownSeconds.Value != 0)
				conditionElement.SetAttribute("Seconds", XmlConvert.ToString(numericUpDownSeconds.Value));
			else
				conditionElement.RemoveAttribute("Seconds");

			if(checkBoxRadomWait.Checked)
				conditionElement.SetAttribute("RandomSeconds", XmlConvert.ToString(numericUpDownRandomSeconds.Value));
			else
				conditionElement.RemoveAttribute("RandomSeconds");

			if(checkBoxErrorState.Checked)
				conditionElement.SetAttribute("IsError", XmlConvert.ToString(true));
			else
				conditionElement.RemoveAttribute("IsError");

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
