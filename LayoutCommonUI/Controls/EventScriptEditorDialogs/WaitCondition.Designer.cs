namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for WaitCondition.
    /// </summary>
    partial class WaitCondition : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.numericUpDownMinutes = new NumericUpDown();
            this.label2 = new Label();
            this.numericUpDownSeconds = new NumericUpDown();
            this.label3 = new Label();
            this.checkBoxRadomWait = new CheckBox();
            this.label4 = new Label();
            this.numericUpDownRandomSeconds = new NumericUpDown();
            this.checkBoxErrorState = new CheckBox();
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownMinutes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownSeconds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownRandomSeconds).BeginInit();
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
            this.numericUpDownRandomSeconds.Enter += this.NumericUpDownRandomSeconds_ValueChanged;
            // 
            // checkBoxErrorState
            // 
            this.checkBoxErrorState.Location = new System.Drawing.Point(15, 75);
            this.checkBoxErrorState.Name = "checkBoxErrorState";
            this.checkBoxErrorState.TabIndex = 8;
            this.checkBoxErrorState.Text = "Occurence of this event indicates an error";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(180, 106);
            this.checkBoxErrorState.Size = new System.Drawing.Size(241, 16);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(64, 23);
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += this.ButtonOk_Click;
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
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownMinutes).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownSeconds).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownRandomSeconds).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private NumericUpDown numericUpDownMinutes;
        private Button buttonOk;
        private Button buttonCancel;
        private NumericUpDown numericUpDownSeconds;
        private CheckBox checkBoxRadomWait;
        private NumericUpDown numericUpDownRandomSeconds;
        private CheckBox checkBoxErrorState;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}