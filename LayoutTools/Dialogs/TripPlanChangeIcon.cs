using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanChangeIcon.
    /// </summary>
    public class TripPlanChangeIcon : Form {
        private Label label1;
        private Label labelTripPlanName;
        private LayoutManager.CommonUI.Controls.SelectTripPlanIcon selectTripPlanIcon;
        private Button ButtonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private readonly TripPlanInfo tripPlan;

        public TripPlanChangeIcon(TripPlanInfo tripPlan, TripPlanIconListInfo tripPlanIconList) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.tripPlan = tripPlan;

            labelTripPlanName.Text = tripPlan.Name;
            selectTripPlanIcon.IconList = tripPlanIconList;
            selectTripPlanIcon.SelectedID = tripPlan.IconId;
            selectTripPlanIcon.Initialize();
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
            this.label1 = new Label();
            this.labelTripPlanName = new Label();
            this.selectTripPlanIcon = new LayoutManager.CommonUI.Controls.SelectTripPlanIcon();
            this.ButtonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Change Icon for trip-plan:";
            // 
            // labelTripPlanName
            // 
            this.labelTripPlanName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTripPlanName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelTripPlanName.Location = new System.Drawing.Point(8, 40);
            this.labelTripPlanName.Name = "labelTripPlanName";
            this.labelTripPlanName.Size = new System.Drawing.Size(280, 24);
            this.labelTripPlanName.TabIndex = 1;
            this.labelTripPlanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // selectTripPlanIcon
            // 
            this.selectTripPlanIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.selectTripPlanIcon.IconList = null;
            this.selectTripPlanIcon.Location = new System.Drawing.Point(8, 72);
            this.selectTripPlanIcon.Name = "selectTripPlanIcon";
            this.selectTripPlanIcon.SelectedID = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.selectTripPlanIcon.SelectedIndex = -1;
            this.selectTripPlanIcon.Size = new System.Drawing.Size(280, 86);
            this.selectTripPlanIcon.TabIndex = 2;
            // 
            // ButtonOK
            // 
            this.ButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonOK.Location = new System.Drawing.Point(153, 168);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(64, 24);
            this.ButtonOK.TabIndex = 3;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(224, 168);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(64, 24);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // TripPlanChangeIcon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 198);
            this.ControlBox = false;
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.selectTripPlanIcon);
            this.Controls.Add(this.labelTripPlanName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Name = "TripPlanChangeIcon";
            this.ShowInTaskbar = false;
            this.Text = "Change Trip-plan Icon";
            this.ResumeLayout(false);
        }
        #endregion

        private void ButtonOK_Click(object sender, System.EventArgs e) {
            tripPlan.IconId = selectTripPlanIcon.SelectedID;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
