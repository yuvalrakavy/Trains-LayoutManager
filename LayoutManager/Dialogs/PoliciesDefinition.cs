using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Summary description for PoliciesDefinition.
	/// </summary>
	public class PoliciesDefinition : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.TabControl tabControlStartCondition;
		private System.Windows.Forms.TabPage tabPageGlobal;
		private System.Windows.Forms.TabPage tabPageTripPlan;
		private System.Windows.Forms.TabPage tabPageBlock;
		private LayoutManager.CommonUI.Controls.PolicyList policyListTripPlan;
		private LayoutManager.CommonUI.Controls.PolicyList policyListBlock;
		private LayoutManager.CommonUI.Controls.GlobalPolicyList globalPolicyListGlobal;
		private TabPage tabPageStartCondition;
		private TabPage tabPageDriverInstructions;
		private LayoutManager.CommonUI.Controls.PolicyList policyListStartCondition;
		private LayoutManager.CommonUI.Controls.PolicyList policyListDriverInstructions;
		private Button buttonExport;
		private Button buttonImport;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void endOfDesignerVariables() { }

		public PoliciesDefinition()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			EventManager.AddObjectSubscriptions(this);

			globalPolicyListGlobal.Scope = "Global";
			globalPolicyListGlobal.Policies = LayoutModel.StateManager.LayoutPolicies;

			policyListTripPlan.Scope = "TripPlan";
			policyListTripPlan.Policies = LayoutModel.StateManager.TripPlanPolicies;

			policyListBlock.Scope = "BlockInfo";
			policyListBlock.Policies = LayoutModel.StateManager.BlockInfoPolicies;
			policyListBlock.Customizer = null;

			policyListStartCondition.Scope = "RideStart";
			policyListStartCondition.Policies = LayoutModel.StateManager.RideStartPolicies;
			policyListStartCondition.Customizer = null;

			policyListDriverInstructions.Scope = "DriverInstructions";
			policyListDriverInstructions.Policies = LayoutModel.StateManager.DriverInstructionsPolicies;
			policyListDriverInstructions.Customizer = null;

			globalPolicyListGlobal.Initialize();
			policyListTripPlan.Initialize();
			policyListBlock.Initialize();
			policyListStartCondition.Initialize();
			policyListDriverInstructions.Initialize();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonClose = new System.Windows.Forms.Button();
			this.tabControlStartCondition = new System.Windows.Forms.TabControl();
			this.tabPageGlobal = new System.Windows.Forms.TabPage();
			this.tabPageBlock = new System.Windows.Forms.TabPage();
			this.tabPageTripPlan = new System.Windows.Forms.TabPage();
			this.tabPageStartCondition = new System.Windows.Forms.TabPage();
			this.tabPageDriverInstructions = new System.Windows.Forms.TabPage();
			this.buttonExport = new System.Windows.Forms.Button();
			this.globalPolicyListGlobal = new LayoutManager.CommonUI.Controls.GlobalPolicyList();
			this.policyListBlock = new LayoutManager.CommonUI.Controls.PolicyList();
			this.policyListTripPlan = new LayoutManager.CommonUI.Controls.PolicyList();
			this.policyListStartCondition = new LayoutManager.CommonUI.Controls.PolicyList();
			this.policyListDriverInstructions = new LayoutManager.CommonUI.Controls.PolicyList();
			this.buttonImport = new System.Windows.Forms.Button();
			this.tabControlStartCondition.SuspendLayout();
			this.tabPageGlobal.SuspendLayout();
			this.tabPageBlock.SuspendLayout();
			this.tabPageTripPlan.SuspendLayout();
			this.tabPageStartCondition.SuspendLayout();
			this.tabPageDriverInstructions.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonClose.Location = new System.Drawing.Point(312, 240);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 2;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// tabControlStartCondition
			// 
			this.tabControlStartCondition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlStartCondition.Controls.Add(this.tabPageGlobal);
			this.tabControlStartCondition.Controls.Add(this.tabPageBlock);
			this.tabControlStartCondition.Controls.Add(this.tabPageTripPlan);
			this.tabControlStartCondition.Controls.Add(this.tabPageStartCondition);
			this.tabControlStartCondition.Controls.Add(this.tabPageDriverInstructions);
			this.tabControlStartCondition.Location = new System.Drawing.Point(0, 0);
			this.tabControlStartCondition.Name = "tabControlStartCondition";
			this.tabControlStartCondition.SelectedIndex = 0;
			this.tabControlStartCondition.Size = new System.Drawing.Size(400, 232);
			this.tabControlStartCondition.TabIndex = 0;
			// 
			// tabPageGlobal
			// 
			this.tabPageGlobal.Controls.Add(this.globalPolicyListGlobal);
			this.tabPageGlobal.Location = new System.Drawing.Point(4, 22);
			this.tabPageGlobal.Name = "tabPageGlobal";
			this.tabPageGlobal.Size = new System.Drawing.Size(392, 206);
			this.tabPageGlobal.TabIndex = 0;
			this.tabPageGlobal.Text = "Global";
			this.tabPageGlobal.UseVisualStyleBackColor = true;
			// 
			// tabPageBlock
			// 
			this.tabPageBlock.Controls.Add(this.policyListBlock);
			this.tabPageBlock.Location = new System.Drawing.Point(4, 22);
			this.tabPageBlock.Name = "tabPageBlock";
			this.tabPageBlock.Size = new System.Drawing.Size(392, 206);
			this.tabPageBlock.TabIndex = 2;
			this.tabPageBlock.Text = "Block";
			this.tabPageBlock.UseVisualStyleBackColor = true;
			// 
			// tabPageTripPlan
			// 
			this.tabPageTripPlan.Controls.Add(this.policyListTripPlan);
			this.tabPageTripPlan.Location = new System.Drawing.Point(4, 22);
			this.tabPageTripPlan.Name = "tabPageTripPlan";
			this.tabPageTripPlan.Size = new System.Drawing.Size(392, 206);
			this.tabPageTripPlan.TabIndex = 1;
			this.tabPageTripPlan.Text = "Trip Plan (Train)";
			this.tabPageTripPlan.UseVisualStyleBackColor = true;
			// 
			// tabPageStartCondition
			// 
			this.tabPageStartCondition.Controls.Add(this.policyListStartCondition);
			this.tabPageStartCondition.Location = new System.Drawing.Point(4, 22);
			this.tabPageStartCondition.Name = "tabPageStartCondition";
			this.tabPageStartCondition.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageStartCondition.Size = new System.Drawing.Size(392, 206);
			this.tabPageStartCondition.TabIndex = 3;
			this.tabPageStartCondition.Text = "Start Condition";
			this.tabPageStartCondition.UseVisualStyleBackColor = true;
			// 
			// tabPageDriverInstructions
			// 
			this.tabPageDriverInstructions.Controls.Add(this.policyListDriverInstructions);
			this.tabPageDriverInstructions.Location = new System.Drawing.Point(4, 22);
			this.tabPageDriverInstructions.Name = "tabPageDriverInstructions";
			this.tabPageDriverInstructions.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDriverInstructions.Size = new System.Drawing.Size(392, 206);
			this.tabPageDriverInstructions.TabIndex = 4;
			this.tabPageDriverInstructions.Text = "Driver Instructions";
			this.tabPageDriverInstructions.UseVisualStyleBackColor = true;
			// 
			// buttonExport
			// 
			this.buttonExport.Location = new System.Drawing.Point(12, 240);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(75, 23);
			this.buttonExport.TabIndex = 1;
			this.buttonExport.Text = "Export...";
			this.buttonExport.UseVisualStyleBackColor = true;
			this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
			// 
			// globalPolicyListGlobal
			// 
			this.globalPolicyListGlobal.Customizer = this.globalPolicyListGlobal;
			this.globalPolicyListGlobal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.globalPolicyListGlobal.Location = new System.Drawing.Point(0, 0);
			this.globalPolicyListGlobal.Name = "globalPolicyListGlobal";
			this.globalPolicyListGlobal.Policies = null;
			this.globalPolicyListGlobal.Scope = "TripPlan";
			this.globalPolicyListGlobal.ShowIfRunning = true;
			this.globalPolicyListGlobal.ShowPolicyDefinition = false;
			this.globalPolicyListGlobal.Size = new System.Drawing.Size(392, 206);
			this.globalPolicyListGlobal.TabIndex = 0;
			this.globalPolicyListGlobal.ViewOnly = false;
			// 
			// policyListBlock
			// 
			this.policyListBlock.Customizer = this.policyListBlock;
			this.policyListBlock.Dock = System.Windows.Forms.DockStyle.Fill;
			this.policyListBlock.Location = new System.Drawing.Point(0, 0);
			this.policyListBlock.Name = "policyListBlock";
			this.policyListBlock.Policies = null;
			this.policyListBlock.Scope = "TripPlan";
			this.policyListBlock.ShowIfRunning = false;
			this.policyListBlock.ShowPolicyDefinition = false;
			this.policyListBlock.Size = new System.Drawing.Size(392, 206);
			this.policyListBlock.TabIndex = 0;
			this.policyListBlock.ViewOnly = false;
			// 
			// policyListTripPlan
			// 
			this.policyListTripPlan.Customizer = this.policyListTripPlan;
			this.policyListTripPlan.Dock = System.Windows.Forms.DockStyle.Fill;
			this.policyListTripPlan.Location = new System.Drawing.Point(0, 0);
			this.policyListTripPlan.Name = "policyListTripPlan";
			this.policyListTripPlan.Policies = null;
			this.policyListTripPlan.Scope = "TripPlan";
			this.policyListTripPlan.ShowIfRunning = false;
			this.policyListTripPlan.ShowPolicyDefinition = false;
			this.policyListTripPlan.Size = new System.Drawing.Size(392, 206);
			this.policyListTripPlan.TabIndex = 0;
			this.policyListTripPlan.ViewOnly = false;
			// 
			// policyListStartCondition
			// 
			this.policyListStartCondition.Customizer = this.policyListStartCondition;
			this.policyListStartCondition.Dock = System.Windows.Forms.DockStyle.Fill;
			this.policyListStartCondition.Location = new System.Drawing.Point(3, 3);
			this.policyListStartCondition.Name = "policyListStartCondition";
			this.policyListStartCondition.Policies = null;
			this.policyListStartCondition.Scope = "RideStart";
			this.policyListStartCondition.ShowIfRunning = false;
			this.policyListStartCondition.ShowPolicyDefinition = false;
			this.policyListStartCondition.Size = new System.Drawing.Size(386, 200);
			this.policyListStartCondition.TabIndex = 0;
			this.policyListStartCondition.ViewOnly = false;
			// 
			// policyListDriverInstructions
			// 
			this.policyListDriverInstructions.Customizer = this.policyListDriverInstructions;
			this.policyListDriverInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.policyListDriverInstructions.Location = new System.Drawing.Point(3, 3);
			this.policyListDriverInstructions.Name = "policyListDriverInstructions";
			this.policyListDriverInstructions.Policies = null;
			this.policyListDriverInstructions.Scope = "DriverInstructions";
			this.policyListDriverInstructions.ShowIfRunning = false;
			this.policyListDriverInstructions.ShowPolicyDefinition = false;
			this.policyListDriverInstructions.Size = new System.Drawing.Size(386, 200);
			this.policyListDriverInstructions.TabIndex = 0;
			this.policyListDriverInstructions.ViewOnly = false;
			// 
			// buttonImport
			// 
			this.buttonImport.Location = new System.Drawing.Point(93, 240);
			this.buttonImport.Name = "buttonImport";
			this.buttonImport.Size = new System.Drawing.Size(75, 23);
			this.buttonImport.TabIndex = 3;
			this.buttonImport.Text = "Import...";
			this.buttonImport.UseVisualStyleBackColor = true;
			this.buttonImport.Click += new System.EventHandler(this.buttonImport_Click);
			// 
			// PoliciesDefinition
			// 
			this.ClientSize = new System.Drawing.Size(400, 270);
			this.ControlBox = false;
			this.Controls.Add(this.buttonImport);
			this.Controls.Add(this.buttonExport);
			this.Controls.Add(this.tabControlStartCondition);
			this.Controls.Add(this.buttonClose);
			this.Name = "PoliciesDefinition";
			this.ShowInTaskbar = false;
			this.Text = "Script Definitions";
			this.Closed += new System.EventHandler(this.PoliciesDefinition_Closed);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PoliciesDefinition_Closing);
			this.tabControlStartCondition.ResumeLayout(false);
			this.tabPageGlobal.ResumeLayout(false);
			this.tabPageBlock.ResumeLayout(false);
			this.tabPageTripPlan.ResumeLayout(false);
			this.tabPageStartCondition.ResumeLayout(false);
			this.tabPageDriverInstructions.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		[LayoutEvent("query-policies-definition-dialog")]
		private void queryPoliciesDefinitionDialog(LayoutEvent e) {
			e.Info = this;
		}

		private void PoliciesDefinition_Closed(object sender, System.EventArgs e) {
			EventManager.Subscriptions.RemoveObjectSubscriptions(this);
			policyListTripPlan.Dispose();
		}

		private void PoliciesDefinition_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(Owner != null)
				Owner.Activate();
		}

		private void buttonClose_Click(object sender, System.EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonExport_Click(object sender, EventArgs e) {
			Dialogs.ExportPolicies d = new ExportPolicies();

			d.ShowDialog(this);
		}

		private void buttonImport_Click(object sender, EventArgs e) {
			Dialogs.ImportPolicies d = new ImportPolicies();

			d.ShowDialog(this);
		}
	}
}
