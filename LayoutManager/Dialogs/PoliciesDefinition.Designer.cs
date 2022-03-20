using System;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for PoliciesDefinition.
    /// </summary>
    partial class PoliciesDefinition : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonClose = new Button();
            this.tabControlStartCondition = new TabControl();
            this.tabPageGlobal = new TabPage();
            this.tabPageBlock = new TabPage();
            this.tabPageTripPlan = new TabPage();
            this.tabPageStartCondition = new TabPage();
            this.tabPageDriverInstructions = new TabPage();
            this.buttonExport = new Button();
            this.globalPolicyListGlobal = new CommonUI.Controls.GlobalPolicyList();
            this.policyListBlock = new CommonUI.Controls.PolicyList();
            this.policyListTripPlan = new CommonUI.Controls.PolicyList();
            this.policyListStartCondition = new CommonUI.Controls.PolicyList();
            this.policyListDriverInstructions = new CommonUI.Controls.PolicyList();
            this.buttonImport = new Button();
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
            this.buttonClose.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(312, 240);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // tabControlStartCondition
            // 
            this.tabControlStartCondition.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
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
            this.tabPageStartCondition.Padding = new Padding(3);
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
            this.tabPageDriverInstructions.Padding = new Padding(3);
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
            this.buttonExport.Click += new EventHandler(this.ButtonExport_Click);
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
            this.buttonImport.Click += new EventHandler(this.ButtonImport_Click);
            // 
            // PoliciesDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(400, 270);
            this.ControlBox = false;
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.tabControlStartCondition);
            this.Controls.Add(this.buttonClose);
            this.Name = "PoliciesDefinition";
            this.ShowInTaskbar = false;
            this.Text = "Script Definitions";
            this.Closed += new EventHandler(this.PoliciesDefinition_Closed);
            this.Closing += new CancelEventHandler(this.PoliciesDefinition_Closing);
            this.tabControlStartCondition.ResumeLayout(false);
            this.tabPageGlobal.ResumeLayout(false);
            this.tabPageBlock.ResumeLayout(false);
            this.tabPageTripPlan.ResumeLayout(false);
            this.tabPageStartCondition.ResumeLayout(false);
            this.tabPageDriverInstructions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private Button buttonClose;
        private TabControl tabControlStartCondition;
        private TabPage tabPageGlobal;
        private TabPage tabPageTripPlan;
        private TabPage tabPageBlock;
        private CommonUI.Controls.PolicyList policyListTripPlan;
        private CommonUI.Controls.PolicyList policyListBlock;
        private CommonUI.Controls.GlobalPolicyList globalPolicyListGlobal;
        private TabPage tabPageStartCondition;
        private TabPage tabPageDriverInstructions;
        private CommonUI.Controls.PolicyList policyListStartCondition;
        private CommonUI.Controls.PolicyList policyListDriverInstructions;
        private Button buttonExport;
        private Button buttonImport;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

