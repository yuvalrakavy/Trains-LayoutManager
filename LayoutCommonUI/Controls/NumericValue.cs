using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for NumericValue.
	/// </summary>
	public class NumericValue : System.Windows.Forms.UserControl {
		private LayoutManager.CommonUI.Controls.LinkMenu linkMenuOperation;
		private System.Windows.Forms.TextBox textBoxValue;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		XmlElement		element;

		public NumericValue() {
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		public XmlElement Element {
			get {
				return element;
			}

			set {
				element = value;

				if(element != null) {
					if(element.HasAttribute("Op") && element.GetAttribute("Op") == "Add")
						linkMenuOperation.SelectedIndex = 1;
					else
						linkMenuOperation.SelectedIndex = 0;

					if(element.HasAttribute("Value"))
						textBoxValue.Text = element.GetAttribute("Value");
				}
			}
		}

		public bool ValidateInput() {
			if(textBoxValue.Text.Trim() == "") {
				MessageBox.Show(this, "Missing value", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxValue.Focus();
				return false;
			}

			try {
				int.Parse(textBoxValue.Text);
			} catch(FormatException) {
				MessageBox.Show(this, "Invalid number", "Illegal Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxValue.Focus();
				return false;
			}

			return true;
		}

		public bool Commit() {
			if(!ValidateInput())
				return false;

			if(linkMenuOperation.SelectedIndex == 1)
				element.SetAttribute("Op", "Add");
			else
				element.SetAttribute("Op", "Set");

			element.SetAttribute("Value", textBoxValue.Text);

			return true;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.linkMenuOperation = new LayoutManager.CommonUI.Controls.LinkMenu();
			this.textBoxValue = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// linkMenuOperation
			// 
			this.linkMenuOperation.Location = new System.Drawing.Point(8, 2);
			this.linkMenuOperation.Name = "linkMenuOperation";
			this.linkMenuOperation.Options = new string[] {
															  "Set to",
															  "Add"};
			this.linkMenuOperation.SelectedIndex = 0;
			this.linkMenuOperation.Size = new System.Drawing.Size(48, 23);
			this.linkMenuOperation.TabIndex = 5;
			this.linkMenuOperation.TabStop = true;
			this.linkMenuOperation.Text = "Set to";
			this.linkMenuOperation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxValue
			// 
			this.textBoxValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxValue.Location = new System.Drawing.Point(48, 3);
			this.textBoxValue.Name = "textBoxValue";
			this.textBoxValue.Size = new System.Drawing.Size(111, 20);
			this.textBoxValue.TabIndex = 6;
			this.textBoxValue.Text = "";
			// 
			// NumericValue
			// 
			this.Controls.Add(this.textBoxValue);
			this.Controls.Add(this.linkMenuOperation);
			this.Name = "NumericValue";
			this.Size = new System.Drawing.Size(168, 27);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
