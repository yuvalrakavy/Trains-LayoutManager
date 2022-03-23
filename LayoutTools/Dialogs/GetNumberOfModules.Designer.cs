using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for GetNumberOfModules.
    /// </summary>
    partial class GetNumberOfModules : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.labelQuestion = new System.Windows.Forms.Label();
            this.numericUpDownModuleCount = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleCount)).BeginInit();
            this.SuspendLayout();
            // 
            // labelQuestion
            // 
            this.labelQuestion.Location = new System.Drawing.Point(21, 20);
            this.labelQuestion.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.labelQuestion.Name = "labelQuestion";
            this.labelQuestion.Size = new System.Drawing.Size(770, 49);
            this.labelQuestion.TabIndex = 0;
            this.labelQuestion.Text = "How many MODULENAME would you like to add:";
            this.labelQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownModuleCount
            // 
            this.numericUpDownModuleCount.Location = new System.Drawing.Point(790, 22);
            this.numericUpDownModuleCount.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.numericUpDownModuleCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownModuleCount.Name = "numericUpDownModuleCount";
            this.numericUpDownModuleCount.Size = new System.Drawing.Size(148, 39);
            this.numericUpDownModuleCount.TabIndex = 1;
            this.numericUpDownModuleCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(541, 98);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(195, 57);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(744, 98);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(195, 57);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // GetNumberOfModules
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(957, 172);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.numericUpDownModuleCount);
            this.Controls.Add(this.labelQuestion);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "GetNumberOfModules";
            this.Text = "Add MODULENAME";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownModuleCount)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        private Label labelQuestion;
        private NumericUpDown numericUpDownModuleCount;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

