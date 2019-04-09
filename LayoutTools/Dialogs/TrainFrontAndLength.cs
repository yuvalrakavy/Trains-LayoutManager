using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveFront.
    /// </summary>
    public class TrainFrontAndLength : Form {
        private Button buttonOk;
        private Button buttonCancel;
        private Label labelBlockName;
        private LayoutManager.CommonUI.Controls.LocomotiveFront locomotiveFrontControl;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private LayoutManager.CommonUI.Controls.TrainLengthDiagram trainLengthDiagram;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public TrainFrontAndLength(LayoutBlockDefinitionComponent blockInfo, string name) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            locomotiveFrontControl.ConnectionPoints = blockInfo.Track.ConnectionPoints;
            locomotiveFrontControl.LocomotiveName = name;
            labelBlockName.Text = blockInfo.NameProvider.Name;
        }

        public TrainFrontAndLength(LayoutStraightTrackComponent track, string locoName, string trackName) {
            InitializeComponent();

            locomotiveFrontControl.ConnectionPoints = track.ConnectionPoints;
            locomotiveFrontControl.LocomotiveName = locoName;
            labelBlockName.Text = trackName;
        }

        public LayoutComponentConnectionPoint Front {
            get {
                return locomotiveFrontControl.Front;
            }

            set {
                locomotiveFrontControl.Front = value;
            }
        }

        public TrainLength Length {
            get {
                return trainLengthDiagram.Length;
            }

            set {
                trainLengthDiagram.Length = value;
            }
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainFrontAndLength));
            this.buttonOk = new Button();
            this.buttonCancel = new Button();
            this.labelBlockName = new Label();
            this.locomotiveFrontControl = new LayoutManager.CommonUI.Controls.LocomotiveFront();
            this.groupBox1 = new GroupBox();
            this.groupBox2 = new GroupBox();
            this.trainLengthDiagram = new LayoutManager.CommonUI.Controls.TrainLengthDiagram();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(209, 151);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(290, 151);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // labelBlockName
            // 
            this.labelBlockName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.labelBlockName.Location = new System.Drawing.Point(57, 185);
            this.labelBlockName.Name = "labelBlockName";
            this.labelBlockName.Size = new System.Drawing.Size(176, 16);
            this.labelBlockName.TabIndex = 3;
            this.labelBlockName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // locomotiveFrontControl
            // 
            this.locomotiveFrontControl.ConnectionPoints = ((System.Collections.Generic.IList<LayoutManager.Model.LayoutComponentConnectionPoint>)(resources.GetObject("locomotiveFrontControl.ConnectionPoints")));
            this.locomotiveFrontControl.Front = ((LayoutManager.Model.LayoutComponentConnectionPoint)(resources.GetObject("locomotiveFrontControl.Front")));
            this.locomotiveFrontControl.Location = new System.Drawing.Point(13, 19);
            this.locomotiveFrontControl.LocomotiveName = null;
            this.locomotiveFrontControl.Name = "locomotiveFrontControl";
            this.locomotiveFrontControl.Size = new System.Drawing.Size(117, 105);
            this.locomotiveFrontControl.TabIndex = 1;
            this.locomotiveFrontControl.Text = "locomotiveFront1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.locomotiveFrontControl);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(147, 133);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Locomotive direction:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.trainLengthDiagram);
            this.groupBox2.Location = new System.Drawing.Point(173, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(192, 133);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Train length:";
            // 
            // trainLengthDiagram
            // 
            this.trainLengthDiagram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.trainLengthDiagram.Comparison = LayoutManager.Model.TrainLengthComparison.None;
            this.trainLengthDiagram.Location = new System.Drawing.Point(6, 45);
            this.trainLengthDiagram.Name = "trainLengthDiagram";
            this.trainLengthDiagram.Size = new System.Drawing.Size(180, 52);
            this.trainLengthDiagram.TabIndex = 0;
            // 
            // TrainFrontAndLength
            // 
            this.AcceptButton = this.buttonOk;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(377, 179);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelBlockName);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TrainFrontAndLength";
            this.ShowInTaskbar = false;
            this.Text = "Set train orientation & length";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOk_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
