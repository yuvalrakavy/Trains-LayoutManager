using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for ExecuteRandomTripPlan.
    /// </summary>
    public class ExecuteRandomTripPlan : Form {
		private TextBox textBoxCondition;
		private Button buttonConditionEdit;
		private Label label1;
		private CheckBox checkBoxCircularMayBeSelected;
		private GroupBox groupBox1;
		private RadioButton radioButtonRevresedNotSelected;
		private RadioButton radioButtonReversedMayBeSelected;
		private RadioButton radioButtonReversedSelected;
		private Button buttonCancel;
		private Button buttonOK;
		private Label label2;
		private ComboBox comboBoxSymbol;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		XmlElement			element;

		XmlElement			newFilterElement = null;
		bool				conditionEdited = false;

		public ExecuteRandomTripPlan(XmlElement element)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.element = element;

			if(element.HasAttribute("Symbol"))
				comboBoxSymbol.Text = element.GetAttribute("Symbol");

			if(element.HasAttribute("SelectCircularTripPlans"))
				checkBoxCircularMayBeSelected.Checked = XmlConvert.ToBoolean(element.GetAttribute("SelectCircularTripPlans"));

			if(element.HasAttribute("ReversedTripPlanSelection")) {
				switch(element.GetAttribute("ReversedTripPlanSelection")) {

					case "Yes":
						radioButtonReversedSelected.Checked = true;
						break;

					case "No":
						radioButtonRevresedNotSelected.Checked = true;
						break;

					case "IfNoAlternative":
					default:
						radioButtonReversedMayBeSelected.Checked = true;
						break;
				}
			}
			else
				radioButtonReversedMayBeSelected.Checked = true;

			updateConditionDescription(element["Filter"]);
		}

		private void updateConditionDescription(XmlElement conditionElement) {
			if(conditionElement == null || conditionElement.ChildNodes.Count < 1)
				textBoxCondition.Text = "<No condition>";
			else
				textBoxCondition.Text = (string)EventManager.Event(new LayoutEvent(conditionElement.ChildNodes[0], "get-event-script-description"));
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
			this.textBoxCondition = new TextBox();
			this.buttonConditionEdit = new Button();
			this.label1 = new Label();
			this.checkBoxCircularMayBeSelected = new CheckBox();
			this.groupBox1 = new GroupBox();
			this.radioButtonRevresedNotSelected = new RadioButton();
			this.radioButtonReversedMayBeSelected = new RadioButton();
			this.radioButtonReversedSelected = new RadioButton();
			this.buttonCancel = new Button();
			this.buttonOK = new Button();
			this.label2 = new Label();
			this.comboBoxSymbol = new ComboBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxCondition
			// 
			this.textBoxCondition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxCondition.Location = new System.Drawing.Point(8, 60);
			this.textBoxCondition.Multiline = true;
			this.textBoxCondition.Name = "textBoxCondition";
			this.textBoxCondition.ReadOnly = true;
			this.textBoxCondition.Size = new System.Drawing.Size(416, 56);
			this.textBoxCondition.TabIndex = 3;
			this.textBoxCondition.Text = "";
			// 
			// buttonConditionEdit
			// 
			this.buttonConditionEdit.Location = new System.Drawing.Point(432, 93);
			this.buttonConditionEdit.Name = "buttonConditionEdit";
			this.buttonConditionEdit.TabIndex = 4;
			this.buttonConditionEdit.Text = "&Edit...";
			this.buttonConditionEdit.Click += new System.EventHandler(this.buttonConditionEdit_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(336, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Consider only trip-plans for which the following condition is true:";
			// 
			// checkBoxCircularMayBeSelected
			// 
			this.checkBoxCircularMayBeSelected.Location = new System.Drawing.Point(8, 124);
			this.checkBoxCircularMayBeSelected.Name = "checkBoxCircularMayBeSelected";
			this.checkBoxCircularMayBeSelected.Size = new System.Drawing.Size(224, 24);
			this.checkBoxCircularMayBeSelected.TabIndex = 5;
			this.checkBoxCircularMayBeSelected.Text = "Circular trip-plans may be selected";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonRevresedNotSelected);
			this.groupBox1.Controls.Add(this.radioButtonReversedMayBeSelected);
			this.groupBox1.Controls.Add(this.radioButtonReversedSelected);
			this.groupBox1.Location = new System.Drawing.Point(8, 156);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(416, 100);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Reversed Trip plans";
			// 
			// radioButtonRevresedNotSelected
			// 
			this.radioButtonRevresedNotSelected.Location = new System.Drawing.Point(8, 16);
			this.radioButtonRevresedNotSelected.Name = "radioButtonRevresedNotSelected";
			this.radioButtonRevresedNotSelected.Size = new System.Drawing.Size(168, 24);
			this.radioButtonRevresedNotSelected.TabIndex = 0;
			this.radioButtonRevresedNotSelected.Text = "May not be selected";
			// 
			// radioButtonReversedMayBeSelected
			// 
			this.radioButtonReversedMayBeSelected.Location = new System.Drawing.Point(8, 40);
			this.radioButtonReversedMayBeSelected.Name = "radioButtonReversedMayBeSelected";
			this.radioButtonReversedMayBeSelected.Size = new System.Drawing.Size(328, 24);
			this.radioButtonReversedMayBeSelected.TabIndex = 1;
			this.radioButtonReversedMayBeSelected.Text = "May be selected, only if there are no non-reversed trip-plans";
			// 
			// radioButtonReversedSelected
			// 
			this.radioButtonReversedSelected.Location = new System.Drawing.Point(8, 64);
			this.radioButtonReversedSelected.Name = "radioButtonReversedSelected";
			this.radioButtonReversedSelected.Size = new System.Drawing.Size(328, 24);
			this.radioButtonReversedSelected.TabIndex = 2;
			this.radioButtonReversedSelected.Text = "May be selected";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(432, 233);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(432, 202);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(232, 23);
			this.label2.TabIndex = 0;
			this.label2.Text = "Execute random trip-plan for train defined by:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboBoxSymbol
			// 
			this.comboBoxSymbol.Items.AddRange(new object[] {
																"Train",
																"Script:Train"});
			this.comboBoxSymbol.Location = new System.Drawing.Point(248, 17);
			this.comboBoxSymbol.Name = "comboBoxSymbol";
			this.comboBoxSymbol.Size = new System.Drawing.Size(176, 21);
			this.comboBoxSymbol.TabIndex = 1;
			this.comboBoxSymbol.Text = "Train";
			// 
			// ExecuteRandomTripPlan
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(512, 270);
			this.ControlBox = false;
			this.Controls.Add(this.comboBoxSymbol);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkBoxCircularMayBeSelected);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxCondition);
			this.Controls.Add(this.buttonConditionEdit);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "ExecuteRandomTripPlan";
			this.ShowInTaskbar = false;
			this.Text = "Execute Random Trip-plan";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e) {
			if(comboBoxSymbol.Text.Trim() == null) {
				MessageBox.Show(this, "You did not provide symbol name", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
				comboBoxSymbol.Focus();
				return;
			}

			if(conditionEdited) {
				XmlElement	filterElement = element["Filter"];

				if(filterElement != null)
					element.RemoveChild(filterElement);

				if(newFilterElement != null) {
					filterElement = (XmlElement)element.OwnerDocument.ImportNode(newFilterElement, true);

					element.AppendChild(filterElement);
				}
			}

			element.SetAttribute("Symbol", comboBoxSymbol.Text);
			element.SetAttribute("SelectCircularTripPlans", XmlConvert.ToString(checkBoxCircularMayBeSelected.Checked));

			string	v;

			if(radioButtonReversedSelected.Checked)
				v = "Yes";
			else if(radioButtonRevresedNotSelected.Checked)
				v = "No";
			else
				v = "IfNoAlternative";

			element.SetAttribute("ReversedTripPlanSelection", v);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonConditionEdit_Click(object sender, System.EventArgs e) {
			XmlElement	filterElement;
			
			if(newFilterElement == null) {
				filterElement = element["Filter"];

				if(filterElement == null) {
					XmlDocument		dummyDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
					
					filterElement = dummyDoc.CreateElement("Filter");
					dummyDoc.AppendChild(filterElement);
				}
			}
			else
				filterElement = newFilterElement;

			LayoutManager.CommonUI.Dialogs.ConditionDefinition	d = new LayoutManager.CommonUI.Dialogs.ConditionDefinition(filterElement);

			if(d.ShowDialog(this) == DialogResult.OK) {
				if(d.NoCondition)
					newFilterElement = null;
				else
					newFilterElement = d.ConditionElement;

				updateConditionDescription(newFilterElement);
				conditionEdited = true;
			}
		}
	}
}
