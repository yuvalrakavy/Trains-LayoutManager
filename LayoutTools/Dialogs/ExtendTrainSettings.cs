using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ExtendTrainSettings.
    /// </summary>
    public class ExtendTrainSettings : Form {
        private Label label1;
        private Label label3;
        private Label labelTrainInfo;
        private PictureBox pictureBox1;
        private NumericUpDown numericUpDownFrom;
        private Panel panel1;
        private NumericUpDown numericUpDownTo;
        private Panel panel2;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private readonly TrainStateInfo train;

        public ExtendTrainSettings(TrainStateInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;

            labelTrainInfo.Text = "The train '" + train.DisplayName + "' has " + train.TrackContactTriggerCount +
                " track contact triggers (track magnets).";

            numericUpDownTo.Maximum = train.TrackContactTriggerCount - 1;
            numericUpDownTo.Minimum = 1;

            numericUpDownFrom.Maximum = train.TrackContactTriggerCount - 1;
            numericUpDownFrom.Minimum = 1;

            numericUpDownTo.Value = 1;
            numericUpDownFrom.Value = train.TrackContactTriggerCount - 1;
        }

        public int FromCount => (int)numericUpDownFrom.Value;

        public int ToCount => (int)numericUpDownTo.Value;

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExtendTrainSettings));
            this.label1 = new Label();
            this.labelTrainInfo = new Label();
            this.label3 = new Label();
            this.pictureBox1 = new PictureBox();
            this.numericUpDownFrom = new NumericUpDown();
            this.panel1 = new Panel();
            this.numericUpDownTo = new NumericUpDown();
            this.panel2 = new Panel();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 56);
            this.label1.TabIndex = 0;
            this.label1.Text = "How many track contact triggers (track magnet) are on the other side of the track" +
                " contact component:";
            // 
            // labelTrainInfo
            // 
            this.labelTrainInfo.Location = new System.Drawing.Point(8, 16);
            this.labelTrainInfo.Name = "labelTrainInfo";
            this.labelTrainInfo.Size = new System.Drawing.Size(264, 32);
            this.labelTrainInfo.TabIndex = 1;
            this.labelTrainInfo.Text = "Train setting info";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(200, 56);
            this.label3.TabIndex = 0;
            this.label3.Text = "How many track contact triggers (track magnet) are in the same side of the track " +
                "contact component as the block to which you extend the train to:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = ((System.Drawing.Bitmap)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(168, 184);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(104, 72);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // numericUpDownFrom
            // 
            this.numericUpDownFrom.Location = new System.Drawing.Point(221, 84);
            this.numericUpDownFrom.Name = "numericUpDownFrom";
            this.numericUpDownFrom.Size = new System.Drawing.Size(56, 20);
            this.numericUpDownFrom.TabIndex = 3;
            this.numericUpDownFrom.ValueChanged += this.numericUpDownFrom_ValueChanged;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(248, 104);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 80);
            this.panel1.TabIndex = 4;
            // 
            // numericUpDownTo
            // 
            this.numericUpDownTo.Location = new System.Drawing.Point(176, 148);
            this.numericUpDownTo.Name = "numericUpDownTo";
            this.numericUpDownTo.Size = new System.Drawing.Size(56, 20);
            this.numericUpDownTo.TabIndex = 3;
            this.numericUpDownTo.ValueChanged += this.numericUpDownTo_ValueChanged;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(200, 171);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(2, 13);
            this.panel2.TabIndex = 4;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(8, 232);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(64, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(80, 232);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // ExtendTrainSettings
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.panel1,
                                                                          this.numericUpDownFrom,
                                                                          this.pictureBox1,
                                                                          this.labelTrainInfo,
                                                                          this.label1,
                                                                          this.label3,
                                                                          this.numericUpDownTo,
                                                                          this.panel2,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExtendTrainSettings";
            this.ShowInTaskbar = false;
            this.Text = "Extend Train Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTo)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private void numericUpDownFrom_ValueChanged(object sender, System.EventArgs e) {
            numericUpDownTo.Value = train.TrackContactTriggerCount - numericUpDownFrom.Value;
        }

        private void numericUpDownTo_ValueChanged(object sender, System.EventArgs e) {
            numericUpDownFrom.Value = train.TrackContactTriggerCount - numericUpDownTo.Value;
        }

        private void buttonOK_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}
