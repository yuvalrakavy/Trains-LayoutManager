using System.ComponentModel;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for IfTimeNumericNode.
    /// </summary>
    public class IfTimeNumericNode : Form {
        private RadioButton radioButtonValue;
        private RadioButton radioButtonRange;
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private NumericUpDown numericUpDownValue;
        private NumericUpDown numericUpDownFrom;
        private NumericUpDown numericUpDownTo;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void endOfDesignerVariables() { }

        readonly IIfTimeNode node;

        public IfTimeNumericNode(string title, IIfTimeNode node, int minValue, int maxValue) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.node = node;

            Text = title;

            numericUpDownFrom.Minimum = minValue;
            numericUpDownFrom.Maximum = maxValue;
            numericUpDownTo.Minimum = minValue;
            numericUpDownTo.Maximum = maxValue;
            numericUpDownValue.Minimum = minValue;
            numericUpDownValue.Maximum = maxValue;

            if (node.IsRange) {
                radioButtonRange.Checked = true;
                numericUpDownFrom.Value = node.From;
                numericUpDownTo.Value = node.To;
                numericUpDownFrom.Focus();
            }
            else {
                radioButtonValue.Checked = true;
                numericUpDownValue.Value = node.Value;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonValue = new RadioButton();
            this.numericUpDownValue = new NumericUpDown();
            this.radioButtonRange = new RadioButton();
            this.numericUpDownFrom = new NumericUpDown();
            this.label1 = new Label();
            this.numericUpDownTo = new NumericUpDown();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTo)).BeginInit();
            this.SuspendLayout();
            // 
            // radioButtonValue
            // 
            this.radioButtonValue.Location = new System.Drawing.Point(8, 16);
            this.radioButtonValue.Name = "radioButtonValue";
            this.radioButtonValue.Size = new System.Drawing.Size(56, 24);
            this.radioButtonValue.TabIndex = 0;
            this.radioButtonValue.Text = "Value:";
            // 
            // numericUpDownValue
            // 
            this.numericUpDownValue.Location = new System.Drawing.Point(64, 18);
            this.numericUpDownValue.Name = "numericUpDownValue";
            this.numericUpDownValue.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownValue.TabIndex = 1;
            this.numericUpDownValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownValue_KeyDown);
            this.numericUpDownValue.ValueChanged += new System.EventHandler(this.numericUpDownValue_ValueChanged);
            // 
            // radioButtonRange
            // 
            this.radioButtonRange.Location = new System.Drawing.Point(8, 48);
            this.radioButtonRange.Name = "radioButtonRange";
            this.radioButtonRange.Size = new System.Drawing.Size(56, 24);
            this.radioButtonRange.TabIndex = 2;
            this.radioButtonRange.Text = "From:";
            // 
            // numericUpDownFrom
            // 
            this.numericUpDownFrom.Location = new System.Drawing.Point(64, 50);
            this.numericUpDownFrom.Name = "numericUpDownFrom";
            this.numericUpDownFrom.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownFrom.TabIndex = 3;
            this.numericUpDownFrom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownFromOrTo_KeyDown);
            this.numericUpDownFrom.ValueChanged += new System.EventHandler(this.numericUpDownFromOrTo_ValueChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(117, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "To:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownTo
            // 
            this.numericUpDownTo.Location = new System.Drawing.Point(145, 50);
            this.numericUpDownTo.Name = "numericUpDownTo";
            this.numericUpDownTo.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownTo.TabIndex = 5;
            this.numericUpDownTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numericUpDownFromOrTo_KeyDown);
            this.numericUpDownTo.ValueChanged += new System.EventHandler(this.numericUpDownFromOrTo_ValueChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(58, 83);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(138, 83);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // IfTimeNumericNode
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(216, 110);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButtonRange);
            this.Controls.Add(this.numericUpDownValue);
            this.Controls.Add(this.radioButtonValue);
            this.Controls.Add(this.numericUpDownFrom);
            this.Controls.Add(this.numericUpDownTo);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "IfTimeNumericNode";
            this.ShowInTaskbar = false;
            this.Text = "IfTimeNumericNode";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTo)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void numericUpDownValue_ValueChanged(object sender, System.EventArgs e) {
            radioButtonValue.Checked = true;
        }

        private void numericUpDownFromOrTo_ValueChanged(object sender, System.EventArgs e) {
            radioButtonRange.Checked = true;
        }

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (radioButtonRange.Checked) {
                if (numericUpDownFrom.Value > numericUpDownTo.Value) {
                    MessageBox.Show(this, "Invalid range, \"from\" is larger than \"to\"", "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    numericUpDownFrom.Focus();
                    return;
                }

                node.From = (int)numericUpDownFrom.Value;
                node.To = (int)numericUpDownTo.Value;
            }
            else
                node.Value = (int)numericUpDownValue.Value;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void numericUpDownValue_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            radioButtonValue.Checked = true;
        }

        private void numericUpDownFromOrTo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            radioButtonRange.Checked = true;
        }
    }
}
