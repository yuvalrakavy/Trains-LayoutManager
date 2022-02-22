using LayoutManager.CommonUI;
using LayoutManager.Model;
using System;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TripPlanWayPointStartCondition.
    /// </summary>
    public partial class TripPlanWayPointDriverInstructions : Form, IControlSupportViewOnly {
        private readonly TripPlanWaypointInfo wayPoint;
        private readonly XmlElement driverInstructionsElement;
        private readonly XmlDocument tempDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
        private DialogEditing? changeToViewonly = null;

        public TripPlanWayPointDriverInstructions(TripPlanWaypointInfo wayPoint) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.wayPoint = wayPoint;

            if (wayPoint.DriverInstructions == null)
                driverInstructionsElement = tempDoc.CreateElement("Sequence");
            else
                driverInstructionsElement = (XmlElement)tempDoc.ImportNode(wayPoint.DriverInstructions, true);

            tempDoc.AppendChild(driverInstructionsElement);
            eventScriptEditor.EventScriptElement = driverInstructionsElement;

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
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

        public bool ViewOnly {
            get {
                return changeToViewonly != null;
            }

            set {
                if (value && !ViewOnly) {
                    changeToViewonly = new DialogEditing(this,
                        new DialogEditingCommandBase[] {
                                                           new DialogEditingRemoveControl(buttonOk),
                                                           new DialogEditingChangeText(buttonCancel, "Close"),
                    });
                    changeToViewonly.Do();
                    changeToViewonly.ViewOnly = true;
                }
                else if (!value && ViewOnly) {
                    if (changeToViewonly != null) {
                        changeToViewonly.Undo();
                        changeToViewonly.ViewOnly = false;
                        changeToViewonly = null;
                    }
                }
            }
        }

        private void ButtonOk_Click(object? sender, System.EventArgs e) {
            if (!eventScriptEditor.ValidateScript() || eventScriptEditor.EventScriptElement == null) {
                eventScriptEditor.Focus();
                return;
            }

            wayPoint.DriverInstructions = (XmlElement)wayPoint.Element.OwnerDocument.ImportNode(eventScriptEditor.EventScriptElement, true);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonSave_Click(object? sender, EventArgs e) {
            Dialogs.GetSavedPolicyName d = new(LayoutModel.StateManager.DriverInstructionsPolicies);

            if (d.ShowDialog(this) == DialogResult.OK) {
                bool doSave = true;
                bool overwrite = false;

                if (!eventScriptEditor.ValidateScript(d.IsGlobalPolicy)) {
                    eventScriptEditor.Focus();
                    return;
                }

                if (LayoutModel.StateManager.DriverInstructionsPolicies.Contains(d.PolicyName)) {
                    if (MessageBox.Show("A definition with this name already exists, override?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        doSave = false;
                    else
                        overwrite = true;
                }

                if (doSave && eventScriptEditor.EventScriptElement != null) {
                    LayoutPolicyInfo policy;

                    if (overwrite) {
                        policy = Ensure.NotNull<LayoutPolicyInfo>(LayoutModel.StateManager.DriverInstructionsPolicies[d.PolicyName]);
                        policy.EventScriptElement = eventScriptEditor.EventScriptElement;
                        policy.GlobalPolicy = d.IsGlobalPolicy;
                    }
                    else {
                        XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                        workingDoc.AppendChild(workingDoc.CreateElement("Policies"));

                        policy = new LayoutPolicyInfo(workingDoc.DocumentElement!, d.PolicyName, eventScriptEditor.EventScriptElement, "DriverInstructions", d.IsGlobalPolicy, false);
                    }

                    LayoutModel.StateManager.DriverInstructionsPolicies.Update(policy);
                }
            }
        }

        private void RadioButtonDriverInstructions_Click(object? sender, System.EventArgs e) {
            eventScriptEditor.Focus();
            UpdateButtons(null, EventArgs.Empty);
        }
    }
}
