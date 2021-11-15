using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for MotionRampDefinition.
    /// </summary>
    partial class MotionRampDefinition : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSHowInTrainControllerDialog = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.motionRampEditor = new CommonUI.Controls.MotionRampEditor();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(10, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 57);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(125, 42);
            this.textBoxName.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(472, 39);
            this.textBoxName.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxSHowInTrainControllerDialog);
            this.groupBox1.Location = new System.Drawing.Point(18, 354);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.groupBox1.Size = new System.Drawing.Size(541, 138);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Usage:";
            // 
            // checkBoxSHowInTrainControllerDialog
            // 
            this.checkBoxSHowInTrainControllerDialog.Location = new System.Drawing.Point(21, 39);
            this.checkBoxSHowInTrainControllerDialog.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.checkBoxSHowInTrainControllerDialog.Name = "checkBoxSHowInTrainControllerDialog";
            this.checkBoxSHowInTrainControllerDialog.Size = new System.Drawing.Size(502, 79);
            this.checkBoxSHowInTrainControllerDialog.TabIndex = 0;
            this.checkBoxSHowInTrainControllerDialog.Text = "Show in train speed control dialog";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(582, 352);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(177, 57);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(582, 431);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(177, 57);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // MotionRampDefinition
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(770, 507);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "MotionRampDefinition";
            this.Text = "Acceleration/Deceleration Profile";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private Label label1;
        private TextBox textBoxName;
        private LayoutManager.CommonUI.Controls.MotionRampEditor motionRampEditor;
        private GroupBox groupBox1;
        private CheckBox checkBoxSHowInTrainControllerDialog;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

