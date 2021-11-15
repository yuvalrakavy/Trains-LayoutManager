using LayoutManager.CommonUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutManager.Tools.Controls {
    partial class TripPlanEditor {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TripPlanEditor));
            this.menuItemGoHumanDriver = new LayoutMenuItem();
            this.contextMenuEdit = new ContextMenuStrip();
            this.menuItemEditDestination = new LayoutMenuItem();
            this.menuItemEditDirection = new LayoutMenuItem();
            this.menuItemEditDirectionForward = new LayoutMenuItem();
            this.menuItemEditDirectionBackward = new LayoutMenuItem();
            this.menuItemEditStartCondition = new LayoutMenuItem();
            this.menuItemEditDriverInstructions = new LayoutMenuItem();
            this.menuItemGoAutoDriver = new LayoutMenuItem();
            this.imageListButttons = new ImageList(this.components);
            this.checkBoxTripPlanCircular = new LayoutManager.CommonUI.CheckBoxWithViewOnly();
            this.groupBoxWayPoints = new GroupBox();
            this.buttonWayPointMoveDown = new Button();
            this.buttonWayPointMoveUp = new Button();
            this.buttonAddWaypoint = new Button();
            this.buttonEditWaypoint = new Button();
            this.buttonRemoveWaypoint = new Button();
            this.listViewWayPoints = new ListView();
            this.columnHeaderName = new ColumnHeader();
            this.columnHeaderDirection = new ColumnHeader();
            this.columnHeaderStartCondition = new ColumnHeader();
            this.columnHeaderDriverInstructions = new ColumnHeader();
            this.imageListWayPointStatus = new ImageList(this.components);
            this.tabControl1 = new TabControl();
            this.tabPageTrip = new TabPage();
            this.tabPagePolicies = new TabPage();
            this.policyList = new LayoutManager.CommonUI.Controls.PolicyList();
            this.tabPageAttributes = new TabPage();
            this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
            this.groupBoxWayPoints.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageTrip.SuspendLayout();
            this.tabPagePolicies.SuspendLayout();
            this.tabPageAttributes.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuItemGoHumanDriver
            // 
            this.menuItemGoHumanDriver.Text = "&Human driver";
            // 
            // contextMenuEdit
            // 
            this.contextMenuEdit.Items.AddRange(new ToolStripItem[] {
            this.menuItemEditDestination,
            this.menuItemEditDirection,
            this.menuItemEditStartCondition,
            this.menuItemEditDriverInstructions});
            // 
            // menuItemEditDestination
            // 
            this.menuItemEditDestination.Text = "&Destination...";
            this.menuItemEditDestination.Click += this.MenuItemEditDestination_Click;
            // 
            // menuItemEditDirection
            // 
            this.menuItemEditDirection.DropDownItems.AddRange(new ToolStripItem[] {
            this.menuItemEditDirectionForward,
            this.menuItemEditDirectionBackward});
            this.menuItemEditDirection.Text = "D&irection";
            // 
            // menuItemEditDirectionForward
            // 
            this.menuItemEditDirectionForward.Text = "&Forward";
            this.menuItemEditDirectionForward.Click += this.MenuItemEditDirectionForward_Click;
            // 
            // menuItemEditDirectionBackward
            // 
            this.menuItemEditDirectionBackward.Text = "&Backward";
            this.menuItemEditDirectionBackward.Click += this.MenuItemEditDirectionBackward_Click;
            // 
            // menuItemEditStartCondition
            // 
            this.menuItemEditStartCondition.Text = "Start &Condition";
            this.menuItemEditStartCondition.Click += this.MenuItemEditStartCondition_Click;
            this.menuItemEditStartCondition.DropDownOpening += this.MenuItemEditStartCondition_Popup;
            // 
            // menuItemEditDriverInstructions
            // 
            this.menuItemEditDriverInstructions.Text = "Driver &Instructions";
            this.menuItemEditDriverInstructions.Click += this.MenuItemEditDriverInstructions_Click;
            // 
            // menuItemGoAutoDriver
            // 
            this.menuItemGoAutoDriver.Text = "&Auto driver";
            // 
            // imageListButttons
            // 
            this.imageListButttons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListButttons.ImageStream");
            this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListButttons.Images.SetKeyName(0, "");
            this.imageListButttons.Images.SetKeyName(1, "");
            // 
            // checkBoxTripPlanCircular
            // 
            this.checkBoxTripPlanCircular.Location = new System.Drawing.Point(8, 8);
            this.checkBoxTripPlanCircular.Name = "checkBoxTripPlanCircular";
            this.checkBoxTripPlanCircular.Size = new System.Drawing.Size(128, 16);
            this.checkBoxTripPlanCircular.TabIndex = 2;
            this.checkBoxTripPlanCircular.Text = "Trip plan is circular";
            this.checkBoxTripPlanCircular.ViewOnly = false;
            this.checkBoxTripPlanCircular.Click += this.UpdateButtons;
            // 
            // groupBoxWayPoints
            // 
            this.groupBoxWayPoints.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.groupBoxWayPoints.Controls.Add(this.buttonWayPointMoveDown);
            this.groupBoxWayPoints.Controls.Add(this.buttonWayPointMoveUp);
            this.groupBoxWayPoints.Controls.Add(this.buttonAddWaypoint);
            this.groupBoxWayPoints.Controls.Add(this.buttonEditWaypoint);
            this.groupBoxWayPoints.Controls.Add(this.buttonRemoveWaypoint);
            this.groupBoxWayPoints.Controls.Add(this.listViewWayPoints);
            this.groupBoxWayPoints.Location = new System.Drawing.Point(8, 32);
            this.groupBoxWayPoints.Name = "groupBoxWayPoints";
            this.groupBoxWayPoints.Size = new System.Drawing.Size(312, 158);
            this.groupBoxWayPoints.TabIndex = 5;
            this.groupBoxWayPoints.TabStop = false;
            this.groupBoxWayPoints.Text = "Way points:";
            // 
            // buttonWayPointMoveDown
            // 
            this.buttonWayPointMoveDown.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonWayPointMoveDown.ImageIndex = 0;
            this.buttonWayPointMoveDown.ImageList = this.imageListButttons;
            this.buttonWayPointMoveDown.Location = new System.Drawing.Point(251, 133);
            this.buttonWayPointMoveDown.Name = "buttonWayPointMoveDown";
            this.buttonWayPointMoveDown.Size = new System.Drawing.Size(24, 20);
            this.buttonWayPointMoveDown.TabIndex = 4;
            this.buttonWayPointMoveDown.Click += this.ButtonWayPointMoveDown_Click;
            // 
            // buttonWayPointMoveUp
            // 
            this.buttonWayPointMoveUp.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonWayPointMoveUp.ImageIndex = 1;
            this.buttonWayPointMoveUp.ImageList = this.imageListButttons;
            this.buttonWayPointMoveUp.Location = new System.Drawing.Point(280, 133);
            this.buttonWayPointMoveUp.Name = "buttonWayPointMoveUp";
            this.buttonWayPointMoveUp.Size = new System.Drawing.Size(24, 20);
            this.buttonWayPointMoveUp.TabIndex = 5;
            this.buttonWayPointMoveUp.Click += this.ButtonWayPointMoveUp_Click;
            // 
            // buttonAddWaypoint
            // 
            this.buttonAddWaypoint.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonAddWaypoint.Location = new System.Drawing.Point(8, 133);
            this.buttonAddWaypoint.Name = "buttonAddWaypoint";
            this.buttonAddWaypoint.Size = new System.Drawing.Size(56, 20);
            this.buttonAddWaypoint.TabIndex = 1;
            this.buttonAddWaypoint.Text = "&Add";
            this.buttonAddWaypoint.Click += this.ButtonAddWaypoint_Click;
            // 
            // buttonEditWaypoint
            // 
            this.buttonEditWaypoint.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEditWaypoint.Location = new System.Drawing.Point(72, 133);
            this.buttonEditWaypoint.Name = "buttonEditWaypoint";
            this.buttonEditWaypoint.Size = new System.Drawing.Size(56, 20);
            this.buttonEditWaypoint.TabIndex = 2;
            this.buttonEditWaypoint.Text = "&Edit";
            this.buttonEditWaypoint.Click += this.ButtonEditWaypoint_Click;
            // 
            // buttonRemoveWaypoint
            // 
            this.buttonRemoveWaypoint.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRemoveWaypoint.Location = new System.Drawing.Point(136, 133);
            this.buttonRemoveWaypoint.Name = "buttonRemoveWaypoint";
            this.buttonRemoveWaypoint.Size = new System.Drawing.Size(56, 20);
            this.buttonRemoveWaypoint.TabIndex = 3;
            this.buttonRemoveWaypoint.Text = "&Remove";
            this.buttonRemoveWaypoint.Click += this.ButtonRemoveWaypoint_Click;
            // 
            // listViewWayPoints
            // 
            this.listViewWayPoints.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.listViewWayPoints.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderDirection,
            this.columnHeaderStartCondition,
            this.columnHeaderDriverInstructions});
            this.listViewWayPoints.FullRowSelect = true;
            this.listViewWayPoints.GridLines = true;
            this.listViewWayPoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewWayPoints.HideSelection = false;
            this.listViewWayPoints.Location = new System.Drawing.Point(8, 24);
            this.listViewWayPoints.MultiSelect = false;
            this.listViewWayPoints.Name = "listViewWayPoints";
            this.listViewWayPoints.Size = new System.Drawing.Size(296, 104);
            this.listViewWayPoints.SmallImageList = this.imageListWayPointStatus;
            this.listViewWayPoints.TabIndex = 0;
            this.listViewWayPoints.UseCompatibleStateImageBehavior = false;
            this.listViewWayPoints.View = System.Windows.Forms.View.Details;
            this.listViewWayPoints.DoubleClick += this.ListViewWayPoints_DoubleClick;
            this.listViewWayPoints.SelectedIndexChanged += this.ListViewWayPoints_SelectedIndexChanged;
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Destination";
            this.columnHeaderName.Width = 100;
            // 
            // columnHeaderDirection
            // 
            this.columnHeaderDirection.Text = "Direction";
            // 
            // columnHeaderStartCondition
            // 
            this.columnHeaderStartCondition.Text = "Starting condition";
            this.columnHeaderStartCondition.Width = 120;
            // 
            // columnHeaderDriverInstructions
            // 
            this.columnHeaderDriverInstructions.Text = "Driver Instructions";
            this.columnHeaderDriverInstructions.Width = 120;
            // 
            // imageListWayPointStatus
            // 
            this.imageListWayPointStatus.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListWayPointStatus.ImageStream");
            this.imageListWayPointStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListWayPointStatus.Images.SetKeyName(0, "");
            this.imageListWayPointStatus.Images.SetKeyName(1, "");
            this.imageListWayPointStatus.Images.SetKeyName(2, "");
            this.imageListWayPointStatus.Images.SetKeyName(3, "");
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageTrip);
            this.tabControl1.Controls.Add(this.tabPagePolicies);
            this.tabControl1.Controls.Add(this.tabPageAttributes);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(336, 224);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPageTrip
            // 
            this.tabPageTrip.Controls.Add(this.checkBoxTripPlanCircular);
            this.tabPageTrip.Controls.Add(this.groupBoxWayPoints);
            this.tabPageTrip.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrip.Name = "tabPageTrip";
            this.tabPageTrip.Size = new System.Drawing.Size(328, 198);
            this.tabPageTrip.TabIndex = 0;
            this.tabPageTrip.Text = "Route";
            // 
            // tabPagePolicies
            // 
            this.tabPagePolicies.Controls.Add(this.policyList);
            this.tabPagePolicies.Location = new System.Drawing.Point(4, 22);
            this.tabPagePolicies.Name = "tabPagePolicies";
            this.tabPagePolicies.Size = new System.Drawing.Size(328, 198);
            this.tabPagePolicies.TabIndex = 1;
            this.tabPagePolicies.Text = "Actions";
            // 
            // policyList
            // 
            this.policyList.Customizer = this.policyList;
            this.policyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.policyList.Location = new System.Drawing.Point(0, 0);
            this.policyList.Name = "policyList";
            this.policyList.Policies = null;
            this.policyList.Scope = "TripPlan";
            this.policyList.ShowIfRunning = false;
            this.policyList.ShowPolicyDefinition = false;
            this.policyList.Size = new System.Drawing.Size(328, 198);
            this.policyList.TabIndex = 0;
            this.policyList.ViewOnly = false;
            // 
            // tabPageAttributes
            // 
            this.tabPageAttributes.Controls.Add(this.attributesEditor);
            this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttributes.Name = "tabPageAttributes";
            this.tabPageAttributes.Size = new System.Drawing.Size(328, 198);
            this.tabPageAttributes.TabIndex = 2;
            this.tabPageAttributes.Text = "Attributes";
            // 
            // attributesEditor
            // 
            this.attributesEditor.AttributesOwner = null;
            this.attributesEditor.AttributesSource = null;
            this.attributesEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attributesEditor.Location = new System.Drawing.Point(0, 0);
            this.attributesEditor.Name = "attributesEditor";
            this.attributesEditor.Size = new System.Drawing.Size(328, 198);
            this.attributesEditor.TabIndex = 0;
            this.attributesEditor.ViewOnly = false;
            // 
            // TripPlanEditor
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "TripPlanEditor";
            this.Size = new System.Drawing.Size(336, 224);
            this.groupBoxWayPoints.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageTrip.ResumeLayout(false);
            this.tabPagePolicies.ResumeLayout(false);
            this.tabPageAttributes.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private LayoutMenuItem menuItemGoHumanDriver;
        private ContextMenuStrip contextMenuEdit;
        private LayoutMenuItem menuItemEditDestination;
        private LayoutMenuItem menuItemEditDirection;
        private LayoutMenuItem menuItemEditDirectionForward;
        private LayoutMenuItem menuItemEditDirectionBackward;
        private LayoutMenuItem menuItemEditStartCondition;
        private LayoutMenuItem menuItemEditDriverInstructions;
        private LayoutMenuItem menuItemGoAutoDriver;
        private ImageList imageListButttons;
        private CommonUI.CheckBoxWithViewOnly checkBoxTripPlanCircular;
        private GroupBox groupBoxWayPoints;
        private Button buttonWayPointMoveDown;
        private Button buttonWayPointMoveUp;
        private Button buttonAddWaypoint;
        private Button buttonEditWaypoint;
        private Button buttonRemoveWaypoint;
        private ListView listViewWayPoints;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderDirection;
        private ColumnHeader columnHeaderStartCondition;
        private TabControl tabControl1;
        private TabPage tabPageTrip;
        private TabPage tabPagePolicies;
        private TabPage tabPageAttributes;
        private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
        private LayoutManager.CommonUI.Controls.PolicyList policyList;
        private ColumnHeader columnHeaderDriverInstructions;
        private ImageList imageListWayPointStatus;
        private System.ComponentModel.IContainer components;
    }
}
