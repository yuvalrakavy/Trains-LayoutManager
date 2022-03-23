namespace LayoutManager.CommonUI.Controls.EventScriptEditorDialogs {
    /// <summary>
    /// Summary description for WaitForEvent.
    /// </summary>
    partial class WaitForEvent : Form, IObjectHasXml {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new Label();
            this.comboBoxEvent = new ComboBox();
            this.checkBoxLimitedToScope = new CheckBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Wait for event: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxEvent
            // 
            this.comboBoxEvent.Location = new System.Drawing.Point(96, 17);
            this.comboBoxEvent.Name = "comboBoxEvent";
            this.comboBoxEvent.Size = new System.Drawing.Size(216, 21);
            this.comboBoxEvent.Sorted = true;
            this.comboBoxEvent.TabIndex = 1;
            // 
            // checkBoxLimitedToScope
            // 
            this.checkBoxLimitedToScope.Location = new System.Drawing.Point(8, 48);
            this.checkBoxLimitedToScope.Name = "checkBoxLimitedToScope";
            this.checkBoxLimitedToScope.Size = new System.Drawing.Size(248, 24);
            this.checkBoxLimitedToScope.TabIndex = 2;
            this.checkBoxLimitedToScope.Text = "Limit only to events sent by current scope";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(152, 80);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(232, 80);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // WaitForEvent
            // 
            this.AcceptButton = this.buttonCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(320, 110);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.checkBoxLimitedToScope,
                                                                          this.comboBoxEvent,
                                                                          this.label1,
                                                                          this.buttonCancel});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "WaitForEvent";
            this.ShowInTaskbar = false;
            this.Text = "Wait For Event";
            this.ResumeLayout(false);
        }
        #endregion
        private Label label1;
        private ComboBox comboBoxEvent;
        private Button buttonOK;
        private Button buttonCancel;
        private CheckBox checkBoxLimitedToScope;

    }
}

