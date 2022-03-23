using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanRouteValidationResult.
    /// </summary>
    partial class TripPlanRouteValidationResult : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.labelDescription = new Label();
            this.listViewActions = new ListView();
            this.columnHeaderWayPointDestination = new ColumnHeader();
            this.columnHeaderAction = new ColumnHeader();
            this.buttonFixTripPlan = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.labelDescription.Location = new System.Drawing.Point(8, 8);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(388, 56);
            this.labelDescription.TabIndex = 0;
            this.labelDescription.Text = "The train cannot follow the trip plan because not all the needed routes are possi" +
                "ble. However, it is possible to fix the trip plan so it will become valid. The l" +
                "ist below describes the actions for fixing the trip plan:";
            // 
            // listViewActions
            // 
            this.listViewActions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listViewActions.Columns.AddRange(new ColumnHeader[] {
                                                                                              this.columnHeaderWayPointDestination,
                                                                                              this.columnHeaderAction});
            this.listViewActions.FullRowSelect = true;
            this.listViewActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewActions.HideSelection = false;
            this.listViewActions.Location = new System.Drawing.Point(8, 72);
            this.listViewActions.MultiSelect = false;
            this.listViewActions.Name = "listViewActions";
            this.listViewActions.Size = new System.Drawing.Size(384, 104);
            this.listViewActions.TabIndex = 1;
            this.listViewActions.View = System.Windows.Forms.View.Details;
            this.listViewActions.SelectedIndexChanged += new EventHandler(this.ListViewActions_SelectedIndexChanged);
            // 
            // columnHeaderWayPointDestination
            // 
            this.columnHeaderWayPointDestination.Text = "Way point Destination";
            this.columnHeaderWayPointDestination.Width = 128;
            // 
            // columnHeaderAction
            // 
            this.columnHeaderAction.Text = "Action";
            this.columnHeaderAction.Width = 250;
            // 
            // buttonFixTripPlan
            // 
            this.buttonFixTripPlan.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonFixTripPlan.Location = new System.Drawing.Point(220, 184);
            this.buttonFixTripPlan.Name = "buttonFixTripPlan";
            this.buttonFixTripPlan.Size = new System.Drawing.Size(80, 23);
            this.buttonFixTripPlan.TabIndex = 2;
            this.buttonFixTripPlan.Text = "Fix Trip Plan";
            this.buttonFixTripPlan.Click += new EventHandler(this.ButtonFixTripPlan_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(308, 184);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(80, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // TripPlanRouteValidationResult
            // 
            this.AcceptButton = this.buttonFixTripPlan;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(400, 214);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonFixTripPlan,
                                                                          this.listViewActions,
                                                                          this.labelDescription,
                                                                          this.buttonCancel});
            this.Name = "TripPlanRouteValidationResult";
            this.Text = "Trip Plan Validation Result";
            this.ResumeLayout(false);
        }
        #endregion

        private Label labelDescription;
        private ListView listViewActions;
        private ColumnHeader columnHeaderWayPointDestination;
        private ColumnHeader columnHeaderAction;
        private Button buttonFixTripPlan;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}

