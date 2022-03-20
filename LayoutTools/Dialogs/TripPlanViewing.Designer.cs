using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanViewing.
    /// </summary>
    partial class TripPlanViewing : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripPlanViewing));
            this.tripPlanEditor = new LayoutManager.Tools.Controls.TripPlanEditor();
            this.buttonSave = new Button();
            this.buttonClose = new Button();
            this.SuspendLayout();
            // 
            // tripPlanEditor
            // 
            this.tripPlanEditor.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.tripPlanEditor.EnablePreview = true;
            this.tripPlanEditor.Front = (LayoutManager.Model.LayoutComponentConnectionPoint)resources.GetObject("tripPlanEditor.Front");
            this.tripPlanEditor.Location = new System.Drawing.Point(3, 8);
            this.tripPlanEditor.LocomotiveBlock = null;
            this.tripPlanEditor.Name = "tripPlanEditor";
            this.tripPlanEditor.Size = new System.Drawing.Size(475, 272);
            this.tripPlanEditor.TabIndex = 0;
            this.tripPlanEditor.Train = null;
            this.tripPlanEditor.TrainTargetWaypoint = -1;
            this.tripPlanEditor.TripPlan = null;
            this.tripPlanEditor.TripPlanName = null;
            this.tripPlanEditor.ViewOnly = false;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonSave.Location = new System.Drawing.Point(3, 286);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "&Save...";
            this.buttonSave.Click += new EventHandler(this.ButtonSave_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(403, 286);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // TripPlanViewing
            // 
            this.ClientSize = new System.Drawing.Size(480, 310);
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tripPlanEditor);
            this.Name = "TripPlanViewing";
            this.ShowInTaskbar = false;
            this.Text = "TripPlanViewing";
            this.Closed += new EventHandler(this.TripPlanViewing_Closed);
            this.ResumeLayout(false);
        }
        #endregion

        private LayoutManager.Tools.Controls.TripPlanEditor tripPlanEditor;
        private Button buttonSave;
        private Button buttonClose;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

