using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for SaveTripPlan.
    /// </summary>
    public class SaveTripPlan : Form {
        private Label label1;
        private TextBox textBoxName;
        private LayoutManager.CommonUI.Controls.SelectTripPlanIcon selectTripPlanIcon;
        private Label label2;
        private Button buttonSave;
        private Button buttonCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public SaveTripPlan() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            selectTripPlanIcon.IconList = LayoutModel.Instance.TripPlanIconList;
            selectTripPlanIcon.Initialize();
        }

        public string TripPlanName {
            get {
                return textBoxName.Text;
            }

            set {
                textBoxName.Text = value;
            }
        }

        public Guid IconID => selectTripPlanIcon.SelectedID;

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
            this.textBoxName = new TextBox();
            this.selectTripPlanIcon = new LayoutManager.CommonUI.Controls.SelectTripPlanIcon();
            this.label2 = new Label();
            this.buttonSave = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Trip plan name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(96, 16);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(184, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Text = "";
            // 
            // selectTripPlanIcon
            // 
            this.selectTripPlanIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.selectTripPlanIcon.IconList = null;
            this.selectTripPlanIcon.Location = new System.Drawing.Point(8, 64);
            this.selectTripPlanIcon.Name = "selectTripPlanIcon";
            this.selectTripPlanIcon.SelectedID = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.selectTripPlanIcon.SelectedIndex = -1;
            this.selectTripPlanIcon.Size = new System.Drawing.Size(272, 86);
            this.selectTripPlanIcon.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Trip-plan Icon:";
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(120, 160);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.TabIndex = 4;
            this.buttonSave.Text = "Save";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(205, 160);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // SaveTripPlan
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(292, 190);
            this.ControlBox = false;
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.selectTripPlanIcon);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Name = "SaveTripPlan";
            this.ShowInTaskbar = false;
            this.Text = "Save Trip-Plan";
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonSave_Click(object sender, System.EventArgs e) {
            if (textBoxName.Text.Trim() == "") {
                MessageBox.Show(this, "Please specify name for the saved trip-plan", "Missing Trip-plan Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (selectTripPlanIcon.IconList.LargeIconImageList.Images.Count > 0 && selectTripPlanIcon.SelectedIndex < 0) {
                MessageBox.Show(this, "Please select an icon for the save trip-plan", "No Trip-plan Icon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                selectTripPlanIcon.Focus();
                return;
            }

            if (selectTripPlanIcon.IconList.IconListModified)
                LayoutModel.WriteModelXmlInfo();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            if (selectTripPlanIcon.IconList.IconListModified)
                LayoutModel.WriteModelXmlInfo();
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
