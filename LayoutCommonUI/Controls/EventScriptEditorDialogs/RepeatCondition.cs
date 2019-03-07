using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for RepeatCondition.
    /// </summary>
    public class RepeatCondition : Form {
        private Label label2;
        private NumericUpDown numericUpDownCount;
        private Button buttonOk;
        private Button buttonCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private RadioButton radioButtonRepeatForever;
        private RadioButton radioButtonRepeatCount;
        readonly XmlElement element;

        public RepeatCondition(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            int count = element.HasAttribute("Count") ? XmlConvert.ToInt32(element.GetAttribute("Count")) : -1;

            if (count >= 0) {
                radioButtonRepeatCount.Checked = true;
                numericUpDownCount.Value = XmlConvert.ToInt32(element.GetAttribute("Count"));
            }
            else {
                radioButtonRepeatForever.Checked = true;
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
            this.numericUpDownCount = new NumericUpDown();
            this.label2 = new Label();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.radioButtonRepeatForever = new RadioButton();
            this.radioButtonRepeatCount = new RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCount)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownCount
            // 
            this.numericUpDownCount.Location = new System.Drawing.Point(72, 31);
            this.numericUpDownCount.Maximum = new System.Decimal(new int[] {
                                                                               10000,
                                                                               0,
                                                                               0,
                                                                               0});
            this.numericUpDownCount.Minimum = new System.Decimal(new int[] {
                                                                               1,
                                                                               0,
                                                                               0,
                                                                               0});
            this.numericUpDownCount.Name = "numericUpDownCount";
            this.numericUpDownCount.Size = new System.Drawing.Size(40, 20);
            this.numericUpDownCount.TabIndex = 2;
            this.numericUpDownCount.Value = new System.Decimal(new int[] {
                                                                             1,
                                                                             0,
                                                                             0,
                                                                             0});
            this.numericUpDownCount.Enter += new System.EventHandler(this.numericUpDownCount_Enter);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(120, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "times";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(112, 72);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(64, 23);
            this.buttonOk.TabIndex = 4;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(184, 72);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // radioButtonRepeatForever
            // 
            this.radioButtonRepeatForever.Location = new System.Drawing.Point(16, 5);
            this.radioButtonRepeatForever.Name = "radioButtonRepeatForever";
            this.radioButtonRepeatForever.TabIndex = 0;
            this.radioButtonRepeatForever.Text = "Repeat forever";
            // 
            // radioButtonRepeatCount
            // 
            this.radioButtonRepeatCount.Location = new System.Drawing.Point(16, 29);
            this.radioButtonRepeatCount.Name = "radioButtonRepeatCount";
            this.radioButtonRepeatCount.Size = new System.Drawing.Size(64, 24);
            this.radioButtonRepeatCount.TabIndex = 1;
            this.radioButtonRepeatCount.Text = "Repeat";
            // 
            // RepeatCondition
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(256, 104);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.radioButtonRepeatForever,
                                                                          this.buttonOk,
                                                                          this.label2,
                                                                          this.numericUpDownCount,
                                                                          this.buttonCancel,
                                                                          this.radioButtonRepeatCount});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RepeatCondition";
            this.ShowInTaskbar = false;
            this.Text = "Repeat";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCount)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonOk_Click(object sender, System.EventArgs e) {
            if (radioButtonRepeatForever.Checked)
                element.SetAttribute("Count", XmlConvert.ToString(-1));
            else
                element.SetAttribute("Count", XmlConvert.ToString(numericUpDownCount.Value));
            DialogResult = DialogResult.OK;
        }

        private void numericUpDownCount_Enter(object sender, System.EventArgs e) {
            radioButtonRepeatCount.Checked = true;
        }

    }
}
