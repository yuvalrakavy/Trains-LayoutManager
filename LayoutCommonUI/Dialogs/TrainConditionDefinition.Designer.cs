namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for PolicyDefinition.
    /// </summary>
    partial class TrainConditionDefinition : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.eventScriptEditor = new LayoutManager.CommonUI.Controls.EventScriptEditor();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.radioButtonNoCondition = new RadioButton();
            this.radioButtonCondition = new RadioButton();
            this.linkMenuConditionScope = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.SuspendLayout();
            // 
            // eventScriptEditor
            // 
            this.eventScriptEditor.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.eventScriptEditor.EventScriptElement = null;
            this.eventScriptEditor.Location = new System.Drawing.Point(16, 53);
            this.eventScriptEditor.Name = "eventScriptEditor";
            this.eventScriptEditor.Size = new System.Drawing.Size(338, 243);
            this.eventScriptEditor.TabIndex = 3;
            this.eventScriptEditor.ViewOnly = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonOK.Location = new System.Drawing.Point(232, 306);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(56, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(296, 306);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // radioButtonNoCondition
            // 
            this.radioButtonNoCondition.Location = new System.Drawing.Point(8, 8);
            this.radioButtonNoCondition.Name = "radioButtonNoCondition";
            this.radioButtonNoCondition.TabIndex = 6;
            this.radioButtonNoCondition.Text = "No condition";
            this.radioButtonNoCondition.CheckedChanged += new EventHandler(this.RadioButtonNoCondition_CheckedChanged);
            // 
            // radioButtonCondition
            // 
            this.radioButtonCondition.Location = new System.Drawing.Point(8, 32);
            this.radioButtonCondition.Name = "radioButtonCondition";
            this.radioButtonCondition.Size = new System.Drawing.Size(16, 24);
            this.radioButtonCondition.TabIndex = 7;
            this.radioButtonCondition.CheckedChanged += new EventHandler(this.RadioButtonCondition_CheckedChanged);
            // 
            // linkMenuConditionScope
            // 
            this.linkMenuConditionScope.Location = new System.Drawing.Point(25, 36);
            this.linkMenuConditionScope.Name = "linkMenuConditionScope";
            this.linkMenuConditionScope.Options = new string[] {
                                                                   "Allow only trains for which the following is True",
                                                                   "Do not allow trains for which the following is True"};
            this.linkMenuConditionScope.SelectedIndex = -1;
            this.linkMenuConditionScope.Size = new System.Drawing.Size(256, 16);
            this.linkMenuConditionScope.TabIndex = 8;
            this.linkMenuConditionScope.TabStop = true;
            this.linkMenuConditionScope.Text = "Allow only trains for which the following is True";
            // 
            // TrainConditionDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(360, 334);
            this.ControlBox = false;
            this.Controls.Add(this.linkMenuConditionScope);
            this.Controls.Add(this.radioButtonCondition);
            this.Controls.Add(this.radioButtonNoCondition);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.eventScriptEditor);
            this.Controls.Add(this.buttonCancel);
            this.Name = "TrainConditionDefinition";
            this.ShowInTaskbar = false;
            this.Text = "Train Condition Definition";
            this.ResumeLayout(false);
        }
        #endregion
        private LayoutManager.CommonUI.Controls.EventScriptEditor eventScriptEditor;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonNoCondition;
        private RadioButton radioButtonCondition;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuConditionScope;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}