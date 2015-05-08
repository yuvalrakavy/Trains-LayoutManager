using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Dialogs
{
	[Flags]
	public enum InputBoxValidationOptions {
		AllowEmpty, IntegerNumber
	}

	/// <summary>
	/// Summary description for InputBox.
	/// </summary>
	public class InputBox : Form {
		private Label labelPrompt;
		private TextBox textBox;
		private Button buttonOK;
		private Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private InputBoxValidationOptions	validationOptions = 0;

		public InputBox(String caption, String prompt)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Text = caption;
			this.labelPrompt.Text = prompt;
		}

		public String Input {
			get {
				return textBox.Text;
			}

			set {
				textBox.Text = value;
			}
		}

		public InputBoxValidationOptions ValidationOptions {
			get {
				return validationOptions;
			}

			set {
				validationOptions = value;
			}
		}

		public static String Show(String caption, String prompt, InputBoxValidationOptions validationOptions) {
			InputBox	inputBox = new InputBox(caption, prompt);

			inputBox.ValidationOptions = validationOptions;
			if(inputBox.ShowDialog() == DialogResult.OK)
				return inputBox.Input;
			else
				return null;
		}

        public static String Show(String caption, String prompt) => InputBox.Show(caption, prompt, 0);

        public static String Show(String prompt) => InputBox.Show("", prompt);

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
            this.textBox = new System.Windows.Forms.TextBox();
            this.labelPrompt = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(691, 165);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(240, 55);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(26, 95);
            this.textBox.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(887, 38);
            this.textBox.TabIndex = 1;
            // 
            // labelPrompt
            // 
            this.labelPrompt.Location = new System.Drawing.Point(26, 38);
            this.labelPrompt.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(896, 38);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Prompt";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(435, 165);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(240, 55);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // InputBox
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(960, 234);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.labelPrompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.Name = "InputBox";
            this.ShowInTaskbar = false;
            this.Text = "InputBox";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if((validationOptions & InputBoxValidationOptions.AllowEmpty) == 0 && textBox.Text.Trim() == "") {
				MessageBox.Show(this, "Value cannot be empty", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBox.Focus();
				return;
			}

			if((validationOptions & InputBoxValidationOptions.IntegerNumber) != 0) {
				try {
					int	n = int.Parse(textBox.Text);
				}
				catch(FormatException) {
					MessageBox.Show(this, "Value is not a valid number", "Input error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBox.Focus();
					return;
				}
			}

			DialogResult = DialogResult.OK;
		}
	}
}
