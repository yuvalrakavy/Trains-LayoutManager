using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TripPlanList.
    /// </summary>
    partial class TripPlanList : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripPlanList));
            this.buttonChangeIcon = new Button();
            this.buttonDelete = new Button();
            this.listViewTripPlans = new ListView();
            this.buttonRename = new Button();
            this.buttonDuplicate = new Button();
            this.imageListState = new ImageList(this.components);
            this.SuspendLayout();
            // 
            // buttonChangeIcon
            // 
            this.buttonChangeIcon.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonChangeIcon.Location = new System.Drawing.Point(158, 169);
            this.buttonChangeIcon.Name = "buttonChangeIcon";
            this.buttonChangeIcon.Size = new System.Drawing.Size(93, 20);
            this.buttonChangeIcon.TabIndex = 8;
            this.buttonChangeIcon.Text = "Change Icon...";
            this.buttonChangeIcon.Click += this.ButtonChangeIcon_Click;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonDelete.Location = new System.Drawing.Point(4, 169);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(71, 20);
            this.buttonDelete.TabIndex = 6;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += this.ButtonRemove_Click;
            // 
            // listViewTripPlans
            // 
            this.listViewTripPlans.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.listViewTripPlans.HideSelection = false;
            this.listViewTripPlans.LabelEdit = true;
            this.listViewTripPlans.Location = new System.Drawing.Point(4, 4);
            this.listViewTripPlans.Name = "listViewTripPlans";
            this.listViewTripPlans.Size = new System.Drawing.Size(527, 160);
            this.listViewTripPlans.TabIndex = 5;
            this.listViewTripPlans.AfterLabelEdit += this.ListViewTripPlans_AfterLabelEdit;
            this.listViewTripPlans.SelectedIndexChanged += this.ListViewTripPlans_SelectedIndexChanged;
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRename.Location = new System.Drawing.Point(81, 169);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(71, 20);
            this.buttonRename.TabIndex = 7;
            this.buttonRename.Text = "&Rename";
            this.buttonRename.Click += this.ButtonRename_Click;
            // 
            // buttonDuplicate
            // 
            this.buttonDuplicate.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonDuplicate.Location = new System.Drawing.Point(256, 169);
            this.buttonDuplicate.Name = "buttonDuplicate";
            this.buttonDuplicate.Size = new System.Drawing.Size(64, 20);
            this.buttonDuplicate.TabIndex = 9;
            this.buttonDuplicate.Text = "D&uplicate";
            this.buttonDuplicate.Click += this.ButtonDuplicate_Click;
            // 
            // imageListState
            // 
            this.imageListState.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListState.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListState.ImageStream");
            this.imageListState.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // TripPlanList
            // 
            this.Controls.Add(this.buttonChangeIcon);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.listViewTripPlans);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.buttonDuplicate);
            this.Name = "TripPlanList";
            this.Size = new System.Drawing.Size(536, 192);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ResumeLayout(false);
        }
        #endregion

        private const string A_TripPlanID = "TripPlanID";
        private const string A_ShouldReverse = "ShouldReverse";
        private Button buttonChangeIcon;
        private Button buttonDelete;
        private ListView listViewTripPlans;
        private Button buttonRename;
        private Button buttonDuplicate;
        private ImageList imageListState;
        private IContainer components;


        private bool initialized = false;
        private XmlElement applicableTripPlansElement = null;
        private TripPlanInfo tripPlanToSelect = null;
        private TripPlanIconListInfo tripPlanIconList = null;
    }
}

