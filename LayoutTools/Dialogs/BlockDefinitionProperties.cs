using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BlockInfoProperties.
    /// </summary>
    public partial class BlockInfoProperties : Form, ILayoutComponentPropertiesDialog, CommonUI.Controls.IPolicyListCustomizer {
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

            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);

            checkBoxSuggestForLocomotivePlacement.Checked = info.SuggestForPlacement;
            checkBoxSuggestAsDestination.Checked = info.SuggestForDestination;
            checkBoxSuggestForProgramming.Checked = info.SuggestForProgramming;
            trainLengthDiagram.Length = info.TrainLengthLimit;

            UpdateTrainPassCondition();
            UpdateTrainStopCondition();

            drivingParameters.Element = XmlInfo.Element;

            attributesEditor.AttributesSource = typeof(LayoutBlockDefinitionComponent);
            attributesEditor.AttributesOwner = new AttributesOwner(XmlInfo.Element);

            checkBoxOccupancyDetectionBlock.Checked = info.IsOccupancyDetectionBlock;

            policyListBlockInfo.Customizer = this;
            policyListBlockInfo.Scope = "BlockInfo";
            policyListBlockInfo.Policies = LayoutModel.StateManager.BlockInfoPolicies;
            policyListBlockInfo.Initialize();
        }

        private void UpdateTrainPassCondition() {
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);
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

        private void UpdateTrainStopCondition() {
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);
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
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);

            return info.Policies.Contains(policy.Id);
        }

        public void SetPolicyChecked(LayoutPolicyInfo policy, bool checkValue) {
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);

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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
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
                            if (otherBlockInfo.Name.StartsWith("Block #") && int.TryParse(otherBlockInfo.Name.AsSpan(7), out int blockNumber)) {
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

            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element) {
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

        private void ButtonAdvanced_Click(object? sender, System.EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);
            Dialogs.AdvancedBlockInfoProperties advancedBlockInfo = new(info, placementInfo);

            advancedBlockInfo.ShowDialog(this);
        }

        private void ButtonEditTrainPassCondition_Click(object? sender, System.EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);
            TripPlanTrainConditionInfo passCondition = info.TrainPassCondition;
            CommonUI.Dialogs.TrainConditionDefinition d = new(blockDefinition, passCondition);

            if (d.ShowDialog(this) == DialogResult.OK) {
                XmlElement newRoutingConditionElement = (XmlElement)info.Element.OwnerDocument.ImportNode(d.TrainCondition.Element, true);

                info.Element.ReplaceChild(newRoutingConditionElement, passCondition.Element);
                passCondition.Element = newRoutingConditionElement;

                UpdateTrainPassCondition();
            }
        }

        private void ButtonEditTrainStopCondition_Click(object? sender, System.EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);
            TripPlanTrainConditionInfo stopCondition = info.TrainStopCondition;
            CommonUI.Dialogs.TrainConditionDefinition d = new(blockDefinition, stopCondition);

            if (d.ShowDialog(this) == DialogResult.OK) {
                XmlElement newRoutingConditionElement = (XmlElement)info.Element.OwnerDocument.ImportNode(d.TrainCondition.Element, true);

                info.Element.ReplaceChild(newRoutingConditionElement, stopCondition.Element);
                stopCondition.Element = newRoutingConditionElement;

                UpdateTrainStopCondition();
            }
        }

        private void ButtonResources_Click(object? sender, EventArgs e) {
            LayoutBlockDefinitionComponentInfo info = new(blockDefinition, XmlInfo.Element);
            Dialogs.BlockDefinitionResources d = new(info);

            d.ShowDialog(this);
        }
    }
}
