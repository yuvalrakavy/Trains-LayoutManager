using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

#pragma warning disable IDE0069, IDE0067
namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BlockInfoProperties.
    /// </summary>
    public class BlockInfoProperties : Form, ILayoutComponentPropertiesDialog, CommonUI.Controls.IPolicyListCustomizer {
        private TabControl tabControl1;
        private Button buttonCancel;
        private Button buttonOK;
        private TabPage tabPageGeneral;
        private TabPage tabPageConditions;
        private TabPage tabPageAttributes;
        private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
        private TabPage tabPageDriver;
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private TabPage tabPagePolicies;
        private Button buttonAdvanced;
        private CheckBox checkBoxOccupancyDetectionBlock;
        private LayoutManager.CommonUI.Controls.PolicyList policyListBlockInfo;
        private CheckBox checkBoxSuggestAsDestination;
        private LayoutManager.CommonUI.Controls.DrivingParameters drivingParameters;
        private Button buttonResources;
        private GroupBox groupBox2;
        private Button buttonEditingTrainPassCondition;
        private TextBox textBoxTrainPassCondition;
        private Label labelTrainPassConditionScope;
        private GroupBox groupBox3;
        private Button buttonEditingTrainStopCondition;
        private TextBox textBoxTrainStopCondition;
        private Label labelTrainStopConditionScope;
        private Label label1;
        private Panel panel1;
        private LayoutManager.CommonUI.Controls.TrainLengthDiagram trainLengthDiagram;
        private CheckBox checkBoxSuggestForLocomotivePlacement;
        private CheckBox checkBoxSuggestForProgramming;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private readonly LayoutBlockDefinitionComponent blockDefinition;
        private static int nextBlockNumber = -1;
        private readonly PlacementInfo placementInfo;

        public BlockInfoProperties(ModelComponent component, PlacementInfo placementInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.blockDefinition = (LayoutBlockDefinitionComponent)component;
            this.XmlInfo = new LayoutXmlInfo(component);
            this.placementInfo = placementInfo;

            nameDefinition.XmlInfo = XmlInfo;
            nameDefinition.Component = component;

            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);

            checkBoxSuggestForLocomotivePlacement.Checked = info.SuggestForPlacement;
            checkBoxSuggestAsDestination.Checked = info.SuggestForDestination;
            checkBoxSuggestForProgramming.Checked = info.SuggestForProgramming;
            trainLengthDiagram.Length = info.TrainLengthLimit;

            updateTrainPassCondition();
            updateTrainStopCondition();

            drivingParameters.Element = XmlInfo.Element;

            attributesEditor.AttributesSource = typeof(LayoutBlockDefinitionComponent);
            attributesEditor.AttributesOwner = new AttributesOwner(XmlInfo.Element);

            checkBoxOccupancyDetectionBlock.Checked = info.IsOccupancyDetectionBlock;

            policyListBlockInfo.Customizer = this;
            policyListBlockInfo.Scope = "BlockInfo";
            policyListBlockInfo.Policies = LayoutModel.StateManager.BlockInfoPolicies;
            policyListBlockInfo.Initialize();
        }

        private void updateTrainPassCondition() {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);
            TripPlanTrainConditionInfo trainPassCondition = info.TrainPassCondition;

            if (trainPassCondition.IsConditionEmpty) {
                labelTrainPassConditionScope.Text = "All trains can pass";
                textBoxTrainPassCondition.Visible = false;
            }
            else {
                if (trainPassCondition.ConditionScope == TripPlanTrainConditionScope.AllowIfTrue)
                    labelTrainPassConditionScope.Text = "Passing is allowed only for trains which:";
                else
                    labelTrainPassConditionScope.Text = "Passing is not allow for trains which:";

                textBoxTrainPassCondition.Text = trainPassCondition.GetDescription();
                textBoxTrainPassCondition.Visible = true;
            }
        }

        private void updateTrainStopCondition() {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);
            TripPlanTrainConditionInfo trainStopCondition = info.TrainStopCondition;

            if (trainStopCondition.IsConditionEmpty) {
                labelTrainStopConditionScope.Text = "All trains can stop";
                textBoxTrainStopCondition.Visible = false;
            }
            else {
                if (trainStopCondition.ConditionScope == TripPlanTrainConditionScope.AllowIfTrue)
                    labelTrainStopConditionScope.Text = "Stopping is allowed only for trains which:";
                else
                    labelTrainStopConditionScope.Text = "Stopping is not allowed for trains which:";

                textBoxTrainStopCondition.Text = trainStopCondition.GetDescription();
                textBoxTrainStopCondition.Visible = true;
            }
        }

        public LayoutXmlInfo XmlInfo { get; }

        // Implementation of IPolicyListCustomizer

        public bool IsPolicyChecked(LayoutPolicyInfo policy) {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);

            return info.Policies.Contains(policy.Id);
        }

        public void SetPolicyChecked(LayoutPolicyInfo policy, bool checkValue) {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);

            if (checkValue)
                info.Policies.Add(policy.Id);
            else
                info.Policies.Remove(policy.Id);
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
            this.tabControl1 = new TabControl();
            this.tabPageGeneral = new TabPage();
            this.checkBoxSuggestForProgramming = new CheckBox();
            this.checkBoxSuggestForLocomotivePlacement = new CheckBox();
            this.panel1 = new Panel();
            this.checkBoxSuggestAsDestination = new CheckBox();
            this.trainLengthDiagram = new LayoutManager.CommonUI.Controls.TrainLengthDiagram();
            this.label1 = new Label();
            this.buttonResources = new Button();
            this.checkBoxOccupancyDetectionBlock = new CheckBox();
            this.buttonAdvanced = new Button();
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.tabPageDriver = new TabPage();
            this.drivingParameters = new LayoutManager.CommonUI.Controls.DrivingParameters();
            this.tabPageAttributes = new TabPage();
            this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
            this.tabPageConditions = new TabPage();
            this.groupBox3 = new GroupBox();
            this.buttonEditingTrainStopCondition = new Button();
            this.textBoxTrainStopCondition = new TextBox();
            this.labelTrainStopConditionScope = new Label();
            this.groupBox2 = new GroupBox();
            this.buttonEditingTrainPassCondition = new Button();
            this.textBoxTrainPassCondition = new TextBox();
            this.labelTrainPassConditionScope = new Label();
            this.tabPagePolicies = new TabPage();
            this.policyListBlockInfo = new LayoutManager.CommonUI.Controls.PolicyList();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageDriver.SuspendLayout();
            this.tabPageAttributes.SuspendLayout();
            this.tabPageConditions.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPagePolicies.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageDriver);
            this.tabControl1.Controls.Add(this.tabPageAttributes);
            this.tabControl1.Controls.Add(this.tabPageConditions);
            this.tabControl1.Controls.Add(this.tabPagePolicies);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(288, 296);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.checkBoxSuggestForProgramming);
            this.tabPageGeneral.Controls.Add(this.checkBoxSuggestForLocomotivePlacement);
            this.tabPageGeneral.Controls.Add(this.panel1);
            this.tabPageGeneral.Controls.Add(this.buttonResources);
            this.tabPageGeneral.Controls.Add(this.checkBoxOccupancyDetectionBlock);
            this.tabPageGeneral.Controls.Add(this.buttonAdvanced);
            this.tabPageGeneral.Controls.Add(this.nameDefinition);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(280, 270);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // checkBoxSuggestForProgramming
            // 
            this.checkBoxSuggestForProgramming.AutoSize = true;
            this.checkBoxSuggestForProgramming.Location = new System.Drawing.Point(6, 208);
            this.checkBoxSuggestForProgramming.Name = "checkBoxSuggestForProgramming";
            this.checkBoxSuggestForProgramming.Size = new System.Drawing.Size(266, 17);
            this.checkBoxSuggestForProgramming.TabIndex = 10;
            this.checkBoxSuggestForProgramming.Text = "Suggest block for decoder programming operations";
            this.checkBoxSuggestForProgramming.UseVisualStyleBackColor = true;
            // 
            // checkBoxSuggestForLocomotivePlacement
            // 
            this.checkBoxSuggestForLocomotivePlacement.AutoSize = true;
            this.checkBoxSuggestForLocomotivePlacement.Location = new System.Drawing.Point(6, 187);
            this.checkBoxSuggestForLocomotivePlacement.Name = "checkBoxSuggestForLocomotivePlacement";
            this.checkBoxSuggestForLocomotivePlacement.Size = new System.Drawing.Size(221, 17);
            this.checkBoxSuggestForLocomotivePlacement.TabIndex = 9;
            this.checkBoxSuggestForLocomotivePlacement.Text = "Suggest block for placing trains on tracks";
            this.checkBoxSuggestForLocomotivePlacement.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxSuggestAsDestination);
            this.panel1.Controls.Add(this.trainLengthDiagram);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(265, 104);
            this.panel1.TabIndex = 8;
            // 
            // checkBoxSuggestAsDestination
            // 
            this.checkBoxSuggestAsDestination.Location = new System.Drawing.Point(6, 4);
            this.checkBoxSuggestAsDestination.Name = "checkBoxSuggestAsDestination";
            this.checkBoxSuggestAsDestination.Size = new System.Drawing.Size(248, 17);
            this.checkBoxSuggestAsDestination.TabIndex = 0;
            this.checkBoxSuggestAsDestination.Text = "Suggest block as trip plan way point";
            // 
            // trainLengthDiagram
            // 
            this.trainLengthDiagram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.trainLengthDiagram.Comparison = LayoutManager.Model.TrainLengthComparison.None;
            this.trainLengthDiagram.Location = new System.Drawing.Point(25, 39);
            this.trainLengthDiagram.Name = "trainLengthDiagram";
            this.trainLengthDiagram.Size = new System.Drawing.Size(180, 52);
            this.trainLengthDiagram.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Longest train which is allowed to stop:";
            // 
            // buttonResources
            // 
            this.buttonResources.Location = new System.Drawing.Point(3, 235);
            this.buttonResources.Name = "buttonResources";
            this.buttonResources.Size = new System.Drawing.Size(75, 23);
            this.buttonResources.TabIndex = 5;
            this.buttonResources.Text = "&Resources...";
            this.buttonResources.UseVisualStyleBackColor = true;
            this.buttonResources.Click += this.buttonResources_Click;
            // 
            // checkBoxOccupancyDetectionBlock
            // 
            this.checkBoxOccupancyDetectionBlock.Location = new System.Drawing.Point(6, 59);
            this.checkBoxOccupancyDetectionBlock.Name = "checkBoxOccupancyDetectionBlock";
            this.checkBoxOccupancyDetectionBlock.Size = new System.Drawing.Size(248, 16);
            this.checkBoxOccupancyDetectionBlock.TabIndex = 1;
            this.checkBoxOccupancyDetectionBlock.Text = "Train occupancy detection block";
            // 
            // buttonAdvanced
            // 
            this.buttonAdvanced.Location = new System.Drawing.Point(84, 235);
            this.buttonAdvanced.Name = "buttonAdvanced";
            this.buttonAdvanced.Size = new System.Drawing.Size(75, 23);
            this.buttonAdvanced.TabIndex = 4;
            this.buttonAdvanced.Text = "&Advanced...";
            this.buttonAdvanced.Click += this.buttonAdvanced_Click;
            // 
            // nameDefinition
            // 
            this.nameDefinition.Component = null;
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(0, 0);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(280, 64);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // tabPageDriver
            // 
            this.tabPageDriver.Controls.Add(this.drivingParameters);
            this.tabPageDriver.Location = new System.Drawing.Point(4, 22);
            this.tabPageDriver.Name = "tabPageDriver";
            this.tabPageDriver.Size = new System.Drawing.Size(280, 270);
            this.tabPageDriver.TabIndex = 4;
            this.tabPageDriver.Text = "Driver";
            // 
            // drivingParameters
            // 
            this.drivingParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drivingParameters.Element = null;
            this.drivingParameters.Location = new System.Drawing.Point(0, 0);
            this.drivingParameters.Name = "drivingParameters";
            this.drivingParameters.Size = new System.Drawing.Size(280, 270);
            this.drivingParameters.TabIndex = 0;
            // 
            // tabPageAttributes
            // 
            this.tabPageAttributes.Controls.Add(this.attributesEditor);
            this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttributes.Name = "tabPageAttributes";
            this.tabPageAttributes.Size = new System.Drawing.Size(280, 270);
            this.tabPageAttributes.TabIndex = 3;
            this.tabPageAttributes.Text = "Attributes";
            // 
            // attributesEditor
            // 
            this.attributesEditor.AttributesOwner = null;
            this.attributesEditor.AttributesSource = null;
            this.attributesEditor.Location = new System.Drawing.Point(8, 8);
            this.attributesEditor.Name = "attributesEditor";
            this.attributesEditor.Size = new System.Drawing.Size(264, 256);
            this.attributesEditor.TabIndex = 0;
            this.attributesEditor.ViewOnly = false;
            // 
            // tabPageConditions
            // 
            this.tabPageConditions.Controls.Add(this.groupBox3);
            this.tabPageConditions.Controls.Add(this.groupBox2);
            this.tabPageConditions.Location = new System.Drawing.Point(4, 22);
            this.tabPageConditions.Name = "tabPageConditions";
            this.tabPageConditions.Size = new System.Drawing.Size(280, 270);
            this.tabPageConditions.TabIndex = 2;
            this.tabPageConditions.Text = "Conditions";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonEditingTrainStopCondition);
            this.groupBox3.Controls.Add(this.textBoxTrainStopCondition);
            this.groupBox3.Controls.Add(this.labelTrainStopConditionScope);
            this.groupBox3.Location = new System.Drawing.Point(8, 118);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 99);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Restriction on trains stopping in this block";
            // 
            // buttonEditingTrainStopCondition
            // 
            this.buttonEditingTrainStopCondition.Location = new System.Drawing.Point(6, 75);
            this.buttonEditingTrainStopCondition.Name = "buttonEditingTrainStopCondition";
            this.buttonEditingTrainStopCondition.Size = new System.Drawing.Size(75, 20);
            this.buttonEditingTrainStopCondition.TabIndex = 2;
            this.buttonEditingTrainStopCondition.Text = "&Edit...";
            this.buttonEditingTrainStopCondition.Click += this.buttonEditTrainStopCondition_Click;
            // 
            // textBoxTrainStopCondition
            // 
            this.textBoxTrainStopCondition.Location = new System.Drawing.Point(6, 31);
            this.textBoxTrainStopCondition.Multiline = true;
            this.textBoxTrainStopCondition.Name = "textBoxTrainStopCondition";
            this.textBoxTrainStopCondition.ReadOnly = true;
            this.textBoxTrainStopCondition.Size = new System.Drawing.Size(246, 40);
            this.textBoxTrainStopCondition.TabIndex = 1;
            // 
            // labelTrainStopConditionScope
            // 
            this.labelTrainStopConditionScope.Location = new System.Drawing.Point(6, 16);
            this.labelTrainStopConditionScope.Name = "labelTrainStopConditionScope";
            this.labelTrainStopConditionScope.Size = new System.Drawing.Size(213, 16);
            this.labelTrainStopConditionScope.TabIndex = 0;
            this.labelTrainStopConditionScope.Text = "CONDITION SCOPE";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonEditingTrainPassCondition);
            this.groupBox2.Controls.Add(this.textBoxTrainPassCondition);
            this.groupBox2.Controls.Add(this.labelTrainPassConditionScope);
            this.groupBox2.Location = new System.Drawing.Point(8, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 99);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Restriction on trains passing through this block";
            // 
            // buttonEditingTrainPassCondition
            // 
            this.buttonEditingTrainPassCondition.Location = new System.Drawing.Point(6, 75);
            this.buttonEditingTrainPassCondition.Name = "buttonEditingTrainPassCondition";
            this.buttonEditingTrainPassCondition.Size = new System.Drawing.Size(75, 20);
            this.buttonEditingTrainPassCondition.TabIndex = 2;
            this.buttonEditingTrainPassCondition.Text = "&Edit...";
            this.buttonEditingTrainPassCondition.Click += this.buttonEditTrainPassCondition_Click;
            // 
            // textBoxTrainPassCondition
            // 
            this.textBoxTrainPassCondition.Location = new System.Drawing.Point(6, 31);
            this.textBoxTrainPassCondition.Multiline = true;
            this.textBoxTrainPassCondition.Name = "textBoxTrainPassCondition";
            this.textBoxTrainPassCondition.ReadOnly = true;
            this.textBoxTrainPassCondition.Size = new System.Drawing.Size(246, 40);
            this.textBoxTrainPassCondition.TabIndex = 1;
            // 
            // labelTrainPassConditionScope
            // 
            this.labelTrainPassConditionScope.Location = new System.Drawing.Point(6, 16);
            this.labelTrainPassConditionScope.Name = "labelTrainPassConditionScope";
            this.labelTrainPassConditionScope.Size = new System.Drawing.Size(213, 16);
            this.labelTrainPassConditionScope.TabIndex = 0;
            this.labelTrainPassConditionScope.Text = "CONDITION SCOPE";
            // 
            // tabPagePolicies
            // 
            this.tabPagePolicies.Controls.Add(this.policyListBlockInfo);
            this.tabPagePolicies.Location = new System.Drawing.Point(4, 22);
            this.tabPagePolicies.Name = "tabPagePolicies";
            this.tabPagePolicies.Size = new System.Drawing.Size(280, 270);
            this.tabPagePolicies.TabIndex = 1;
            this.tabPagePolicies.Text = "Actions";
            // 
            // policyListBlockInfo
            // 
            this.policyListBlockInfo.Customizer = this.policyListBlockInfo;
            this.policyListBlockInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.policyListBlockInfo.Location = new System.Drawing.Point(0, 0);
            this.policyListBlockInfo.Name = "policyListBlockInfo";
            this.policyListBlockInfo.Policies = null;
            this.policyListBlockInfo.Scope = "TripPlan";
            this.policyListBlockInfo.ShowIfRunning = true;
            this.policyListBlockInfo.ShowPolicyDefinition = false;
            this.policyListBlockInfo.Size = new System.Drawing.Size(280, 270);
            this.policyListBlockInfo.TabIndex = 0;
            this.policyListBlockInfo.ViewOnly = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(209, 300);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(129, 300);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // BlockInfoProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(288, 327);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "BlockInfoProperties";
            this.ShowInTaskbar = false;
            this.Text = "Block Properties";
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPageDriver.ResumeLayout(false);
            this.tabPageAttributes.ResumeLayout(false);
            this.tabPageConditions.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPagePolicies.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (!checkBoxSuggestForLocomotivePlacement.Checked)
                nameDefinition.IsOptional = true;

            if (!drivingParameters.ValidateValues())
                return;

            if (nameDefinition.IsEmptyName) {
                if (checkBoxSuggestAsDestination.Checked || checkBoxSuggestForLocomotivePlacement.Checked) {
                    MessageBox.Show(this, "Block name must be provided if this block is to be suggested as destination or for locomotive/train placement",
                        "Missing Block Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    nameDefinition.Focus();
                    return;
                }
                else {
                    if (nextBlockNumber == -1) {
                        foreach (LayoutBlockDefinitionComponent otherBlockInfo in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutPhase.All)) {
                            if (otherBlockInfo.Name.StartsWith("Block #") && int.TryParse(otherBlockInfo.Name.Substring(7), out int blockNumber)) {
                                if (blockNumber > nextBlockNumber)
                                    nextBlockNumber = blockNumber + 1;
                            }
                        }

                        if (nextBlockNumber < 0)
                            nextBlockNumber = 1;
                    }
                    else
                        nextBlockNumber++;

                    nameDefinition.Set("Block #" + nextBlockNumber, false);
                }
            }
            else if (!nameDefinition.Commit())
                return;

            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element) {
                SuggestForPlacement = checkBoxSuggestForLocomotivePlacement.Checked,
                SuggestForDestination = checkBoxSuggestAsDestination.Checked,
                SuggestForProgramming = checkBoxSuggestForProgramming.Checked,
                TrainLengthLimit = trainLengthDiagram.Length,
                IsOccupancyDetectionBlock = checkBoxOccupancyDetectionBlock.Checked
            };

            if (LayoutController.IsOperationMode) {
                if (info.BlockDefinition != null && info.BlockDefinition.Block != null) {
                    foreach (TrainLocationInfo trainLocation in info.BlockDefinition.Block.Trains)
                        trainLocation.Train.RefreshSpeedLimit();
                }
            }

            drivingParameters.Commit();
            attributesEditor.Commit();

            DialogResult = DialogResult.OK;
        }

        private void buttonAdvanced_Click(object sender, System.EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);
            Dialogs.AdvancedBlockInfoProperties advancedBlockInfo = new Dialogs.AdvancedBlockInfoProperties(info, placementInfo);

            advancedBlockInfo.ShowDialog(this);
        }

        private void buttonEditTrainPassCondition_Click(object sender, System.EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);
            TripPlanTrainConditionInfo passCondition = info.TrainPassCondition;
            CommonUI.Dialogs.TrainConditionDefinition d = new CommonUI.Dialogs.TrainConditionDefinition(blockDefinition, passCondition);

            if (d.ShowDialog(this) == DialogResult.OK) {
                XmlElement newRoutingConditionElement = (XmlElement)info.Element.OwnerDocument.ImportNode(d.TrainCondition.Element, true);

                info.Element.ReplaceChild(newRoutingConditionElement, passCondition.Element);
                passCondition.Element = newRoutingConditionElement;

                updateTrainPassCondition();
            }
        }

        private void buttonEditTrainStopCondition_Click(object sender, System.EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);
            TripPlanTrainConditionInfo stopCondition = info.TrainStopCondition;
            CommonUI.Dialogs.TrainConditionDefinition d = new CommonUI.Dialogs.TrainConditionDefinition(blockDefinition, stopCondition);

            if (d.ShowDialog(this) == DialogResult.OK) {
                XmlElement newRoutingConditionElement = (XmlElement)info.Element.OwnerDocument.ImportNode(d.TrainCondition.Element, true);

                info.Element.ReplaceChild(newRoutingConditionElement, stopCondition.Element);
                stopCondition.Element = newRoutingConditionElement;

                updateTrainStopCondition();
            }
        }

        private void buttonResources_Click(object sender, EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new LayoutBlockDefinitionComponentInfo(blockDefinition, XmlInfo.Element);
            Dialogs.BlockDefinitionResources d = new BlockDefinitionResources(info);

            d.ShowDialog(this);
        }
    }
}
