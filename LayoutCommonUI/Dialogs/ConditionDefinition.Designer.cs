namespace LayoutManager.CommonUI.Dialogs {
    /// <summary>
    /// Summary description for PolicyDefinition.
    /// </summary>
    partial class ConditionDefinition : Form {
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
            this.buttonOK.Click += this.ButtonOK_Click;
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
            this.buttonCancel.Click += this.ButtonCancel_Click;
            // 
            // radioButtonNoCondition
            // 
            this.radioButtonNoCondition.Location = new System.Drawing.Point(8, 8);
            this.radioButtonNoCondition.Name = "radioButtonNoCondition";
            this.radioButtonNoCondition.TabIndex = 6;
            this.radioButtonNoCondition.Text = "No condition";
            this.radioButtonNoCondition.CheckedChanged += this.RadioButtonNoCondition_CheckedChanged;
            // 
            // radioButtonCondition
            // 
            this.radioButtonCondition.Location = new System.Drawing.Point(8, 32);
            this.radioButtonCondition.Name = "radioButtonCondition";
            this.radioButtonCondition.Size = new System.Drawing.Size(176, 24);
            this.radioButtonCondition.TabIndex = 7;
            this.radioButtonCondition.Text = "Use the following condition:";
            this.radioButtonCondition.CheckedChanged += this.RadioButtonCondition_CheckedChanged;
            // 
            // ConditionDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(360, 334);
            this.ControlBox = false;
            this.Controls.Add(this.radioButtonCondition);
            this.Controls.Add(this.radioButtonNoCondition);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.eventScriptEditor);
            this.Controls.Add(this.buttonCancel);
            this.Name = "ConditionDefinition";
            this.ShowInTaskbar = false;
            this.Text = "Condition Definition";
            this.ResumeLayout(false);
        }
        #endregion
        private LayoutManager.CommonUI.Controls.EventScriptEditor eventScriptEditor;
        private Button buttonOK;
        private Button buttonCancel;
        private RadioButton radioButtonNoCondition;
        private RadioButton radioButtonCondition;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}