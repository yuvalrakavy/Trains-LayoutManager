namespace TrainDetector.Dialogs {
    partial class SelectTrainDetectorModule {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.labelDetectedTrainDetector = new System.Windows.Forms.Label();
            this.radioButtonAddController = new System.Windows.Forms.RadioButton();
            this.radioButtonAssignToController = new System.Windows.Forms.RadioButton();
            this.listViewControllerModules = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSensorsCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelDetectedTrainDetector
            // 
            this.labelDetectedTrainDetector.AutoSize = true;
            this.labelDetectedTrainDetector.Location = new System.Drawing.Point(35, 36);
            this.labelDetectedTrainDetector.Name = "labelDetectedTrainDetector";
            this.labelDetectedTrainDetector.Size = new System.Drawing.Size(347, 25);
            this.labelDetectedTrainDetector.TabIndex = 0;
            this.labelDetectedTrainDetector.Text = "Train Detector controller was found";
            // 
            // radioButtonAddController
            // 
            this.radioButtonAddController.AutoSize = true;
            this.radioButtonAddController.Location = new System.Drawing.Point(40, 86);
            this.radioButtonAddController.Name = "radioButtonAddController";
            this.radioButtonAddController.Size = new System.Drawing.Size(268, 29);
            this.radioButtonAddController.TabIndex = 1;
            this.radioButtonAddController.TabStop = true;
            this.radioButtonAddController.Text = "Add as a new controller";
            this.radioButtonAddController.UseVisualStyleBackColor = true;
            // 
            // radioButtonAssignToController
            // 
            this.radioButtonAssignToController.AutoSize = true;
            this.radioButtonAssignToController.Location = new System.Drawing.Point(40, 129);
            this.radioButtonAssignToController.Name = "radioButtonAssignToController";
            this.radioButtonAssignToController.Size = new System.Drawing.Size(306, 29);
            this.radioButtonAssignToController.TabIndex = 2;
            this.radioButtonAssignToController.TabStop = true;
            this.radioButtonAssignToController.Text = "Use on of these controllers:";
            this.radioButtonAssignToController.UseVisualStyleBackColor = true;
            this.radioButtonAssignToController.CheckedChanged += new System.EventHandler(this.RadioButtonAssignToController_CheckedChanged);
            // 
            // listViewControllerModules
            // 
            this.listViewControllerModules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewControllerModules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderSensorsCount});
            this.listViewControllerModules.FullRowSelect = true;
            this.listViewControllerModules.GridLines = true;
            this.listViewControllerModules.HideSelection = false;
            this.listViewControllerModules.Location = new System.Drawing.Point(74, 180);
            this.listViewControllerModules.MultiSelect = false;
            this.listViewControllerModules.Name = "listViewControllerModules";
            this.listViewControllerModules.Size = new System.Drawing.Size(845, 251);
            this.listViewControllerModules.TabIndex = 3;
            this.listViewControllerModules.UseCompatibleStateImageBehavior = false;
            this.listViewControllerModules.View = System.Windows.Forms.View.Details;
            this.listViewControllerModules.SelectedIndexChanged += new System.EventHandler(this.ListViewControllerModules_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 194;
            // 
            // columnHeaderSensorsCount
            // 
            this.columnHeaderSensorsCount.Text = "Sensors";
            this.columnHeaderSensorsCount.Width = 116;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(586, 452);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(155, 50);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(764, 452);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(155, 50);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // SelectTrainDetectorModule
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(964, 540);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.listViewControllerModules);
            this.Controls.Add(this.radioButtonAssignToController);
            this.Controls.Add(this.radioButtonAddController);
            this.Controls.Add(this.labelDetectedTrainDetector);
            this.Name = "SelectTrainDetectorModule";
            this.ShowInTaskbar = false;
            this.Text = "Select Train Detector Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectTrainDetectorModule_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDetectedTrainDetector;
        private System.Windows.Forms.RadioButton radioButtonAddController;
        private System.Windows.Forms.RadioButton radioButtonAssignToController;
        private System.Windows.Forms.ListView listViewControllerModules;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderSensorsCount;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}