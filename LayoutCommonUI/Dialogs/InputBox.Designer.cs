namespace LayoutManager.CommonUI.Dialogs {
    partial class InputBox : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonCancel = new Button();
            this.textBox = new TextBox();
            this.labelPrompt = new Label();
            this.buttonOK = new Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(216, 69);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(8, 40);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(280, 20);
            this.textBox.TabIndex = 1;
            this.textBox.Text = "";
            // 
            // labelPrompt
            // 
            this.labelPrompt.Location = new System.Drawing.Point(8, 16);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(280, 16);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "Prompt";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(136, 69);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // InputBox
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(300, 98);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonCancel,
                                                                          this.buttonOK,
                                                                          this.textBox,
                                                                          this.labelPrompt});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoSize = true;
            this.Name = "InputBox";
            this.ShowInTaskbar = false;
            this.Text = "InputBox";
            this.ResumeLayout(false);
        }
        #endregion

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;

        private Label labelPrompt;
        private TextBox textBox;
        private Button buttonOK;
        private Button buttonCancel;

    }
}

