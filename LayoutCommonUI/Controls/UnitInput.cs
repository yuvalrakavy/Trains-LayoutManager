using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for UnitInput.
	/// </summary>
	public class UnitInput : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox textBoxValue;
		private LayoutManager.CommonUI.Controls.LinkMenu linkMenuUnits;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		ArrayList	units = new ArrayList();
		Unit		currentUnit = null;
		bool		unitDefinitionUpdated = false;

		public UnitInput()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		public double NeutralValue {
			get {
				if(!unitDefinitionUpdated)
					throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
				return currentUnit.ToNeutralValue(UnitValue);
			}

			set {
				if(!unitDefinitionUpdated)
					throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
				textBoxValue.Text = currentUnit.ToUnitValue(value).ToString();
			}
		}

		public double UnitValue {
			get {
				if(!unitDefinitionUpdated)
					throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
				else
					return IsEmpty ? 0.0 : Double.Parse(textBoxValue.Text);
			}

			set {
				if(!unitDefinitionUpdated)
					throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
				else
					textBoxValue.Text = value.ToString();
			}
		}

		public bool IsEmpty {
			get {
				return textBoxValue.Text.Trim() == "";
			}

			set {
				if(value == true)
					textBoxValue.Text = "";
			}
		}

		public void DefineUnit(string unitName, double factor, double offset) {
			units.Add(new Unit(unitName, factor, offset, units.Count));
			unitDefinitionUpdated = false;
   		}

		public void UnitDefinitionDone() {
			string[]	unitNames = new string[units.Count];

			for(int i = 0; i < units.Count; i++)
				unitNames[i] = ((Unit)units[i]).UnitName;

			linkMenuUnits.Options = unitNames;
			unitDefinitionUpdated = true;
		}

		void SelectUnit(Unit u) {
			double	v = 0;

			if(!IsEmpty)
				v = NeutralValue;

			currentUnit = u;
			linkMenuUnits.SelectedIndex = currentUnit.Index;

			if(!IsEmpty)
				NeutralValue = v;
		}

		public void SelectUnit(String unitName) {
			bool	found = false;

			foreach(Unit u in units)
				if(u.UnitName == unitName) {
					SelectUnit(u);
					found = true;
					break;
				}

			if(!found)
				throw new ArgumentException("Invalid unit name");
		}

		public void SelectUnit(int unitIndex) {
			if(unitIndex < 0 || unitIndex >= units.Count)
				throw new ArgumentException("Invalid unit index", "unitIndex");

			SelectUnit((Unit)units[unitIndex]);
		}

		public bool ReadOnly {
			set {
				textBoxValue.ReadOnly = value;
			}

			get {
				return textBoxValue.ReadOnly;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxValue = new System.Windows.Forms.TextBox();
			this.linkMenuUnits = new LayoutManager.CommonUI.Controls.LinkMenu();
			this.SuspendLayout();
			// 
			// textBoxValue
			// 
			this.textBoxValue.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBoxValue.Location = new System.Drawing.Point(7, 2);
			this.textBoxValue.Name = "textBoxValue";
			this.textBoxValue.Size = new System.Drawing.Size(48, 20);
			this.textBoxValue.TabIndex = 0;
			this.textBoxValue.Text = "";
			// 
			// linkMenuUnits
			// 
			this.linkMenuUnits.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.linkMenuUnits.Location = new System.Drawing.Point(60, 2);
			this.linkMenuUnits.Name = "linkMenuUnits";
			this.linkMenuUnits.Options = new string[0];
			this.linkMenuUnits.SelectedIndex = -1;
			this.linkMenuUnits.Size = new System.Drawing.Size(40, 20);
			this.linkMenuUnits.TabIndex = 1;
			this.linkMenuUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linkMenuUnits.ValueChanged += new System.EventHandler(this.linkMenuUnits_ValueChanged);
			// 
			// UnitInput
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.linkMenuUnits,
																		  this.textBoxValue});
			this.Name = "UnitInput";
			this.Size = new System.Drawing.Size(104, 24);
			this.ResumeLayout(false);

		}
		#endregion

		private void linkMenuUnits_ValueChanged(object sender, System.EventArgs e) {
			SelectUnit(linkMenuUnits.SelectedIndex);
		}

		class Unit {
			string	unitName;
			double	factor;
			double	offset;
			int		index;

			/// <summary>
			/// Declare a new unit
			/// </summary>
			/// <param name="unitName">The unit name</param>
			/// <param name="factor">factor for converting value in unit to neutral value</param>
			/// <param name="offset">offset fro converting value in unit to neutral value</param>
			public Unit(string unitName, double factor, double offset, int index) {
				this.unitName = unitName;
				this.factor = factor;
				this.offset = offset;
				this.index = index;
			}

			public double ToNeutralValue(double valueInUnit) {
				return valueInUnit * factor + offset;
			}

			public double ToUnitValue(double neutralValue) {
				return (neutralValue - offset) / factor;
			}

			public string UnitName {
				get {
					return unitName;
				}
			}

			public int Index {
				get {
					return index;
				}
			}
		}
	}
}
