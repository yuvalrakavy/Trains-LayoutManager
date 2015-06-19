using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TripPlanWayPointStartCondition.
	/// </summary>
	public class TripPlanWayPointDriverInstructions : Form, IControlSupportViewOnly
	{
		private LayoutManager.CommonUI.Controls.EventScriptEditor eventScriptEditor;
		private Button buttonOk;
		private Button buttonCancel;
		private Button buttonSave;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		private void endOfDesignerVariables() { }

		TripPlanWaypointInfo	wayPoint;
		XmlElement				driverInstructionsElement;
		XmlDocument				tempDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
		DialogEditing			changeToViewonly = null;

		public TripPlanWayPointDriverInstructions(TripPlanWaypointInfo wayPoint) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.wayPoint = wayPoint;

			if(wayPoint.DriverInstructions == null)
				driverInstructionsElement = tempDoc.CreateElement("Sequence");
			else
				driverInstructionsElement = (XmlElement)tempDoc.ImportNode(wayPoint.DriverInstructions, true);

			tempDoc.AppendChild(driverInstructionsElement);
			eventScriptEditor.EventScriptElement = driverInstructionsElement;

			updateButtons(null, null);
		}

		private void updateButtons(object sender, EventArgs e) {
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

		public bool ViewOnly {
			get {
				return changeToViewonly != null;
			}

			set {
				if(value == true && !ViewOnly) {
					changeToViewonly = new DialogEditing(this,
						new DialogEditingCommandBase[] {
														   new DialogEditingRemoveControl(buttonOk),
														   new DialogEditingChangeText(buttonCancel, "Close"),
					} );
					changeToViewonly.Do();
					changeToViewonly.ViewOnly = true;
				}
				else if(value == false && ViewOnly) {
					changeToViewonly.Undo();
					changeToViewonly.ViewOnly = false;
					changeToViewonly = null;
				}
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.eventScriptEditor = new LayoutManager.CommonUI.Controls.EventScriptEditor();
			this.buttonOk = new Button();
			this.buttonCancel = new Button();
			this.buttonSave = new Button();
			this.SuspendLayout();
			// 
			// eventScriptEditor
			// 
			this.eventScriptEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.eventScriptEditor.BlockDefinition = null;
			this.eventScriptEditor.EventScriptElement = null;
			this.eventScriptEditor.Location = new System.Drawing.Point(16, 12);
			this.eventScriptEditor.Name = "eventScriptEditor";
			this.eventScriptEditor.Size = new System.Drawing.Size(336, 213);
			this.eventScriptEditor.TabIndex = 2;
			this.eventScriptEditor.ViewOnly = false;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.Location = new System.Drawing.Point(220, 238);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(56, 21);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(284, 238);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(56, 21);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonSave
			// 
			this.buttonSave.Location = new System.Drawing.Point(16, 238);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(56, 21);
			this.buttonSave.TabIndex = 5;
			this.buttonSave.Text = "Save...";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			// 
			// TripPlanWayPointDriverInstructions
			// 
			this.AcceptButton = this.buttonOk;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(360, 262);
			this.ControlBox = false;
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.eventScriptEditor);
			this.Controls.Add(this.buttonCancel);
			this.Name = "TripPlanWayPointDriverInstructions";
			this.ShowInTaskbar = false;
			this.Text = "Waypoint Driver Instructions";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOk_Click(object sender, System.EventArgs e) {
			if(!eventScriptEditor.ValidateScript()) {
				eventScriptEditor.Focus();
				return;
			}

			wayPoint.DriverInstructions = (XmlElement)wayPoint.Element.OwnerDocument.ImportNode(eventScriptEditor.EventScriptElement, true);

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonSave_Click(object sender, EventArgs e) {
			Dialogs.GetSavedPolicyName d = new GetSavedPolicyName(LayoutModel.StateManager.DriverInstructionsPolicies);

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

				if(doSave) {
					LayoutPolicyInfo policy;

					if(overwrite) {
						policy = LayoutModel.StateManager.DriverInstructionsPolicies[d.PolicyName];
						policy.EventScriptElement = eventScriptEditor.EventScriptElement;
						policy.GlobalPolicy = d.IsGlobalPolicy;
					}
					else {
						XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

						workingDoc.AppendChild(workingDoc.CreateElement("Policies"));

						policy = new LayoutPolicyInfo(workingDoc.DocumentElement, d.PolicyName, eventScriptEditor.EventScriptElement, "DriverInstructions", d.IsGlobalPolicy, false);
					}

					LayoutModel.StateManager.DriverInstructionsPolicies.Update(policy);
				}
			}
		}

		private void radioButtonDriverInstructions_Click(object sender, System.EventArgs e) {
			eventScriptEditor.Focus();
			updateButtons(null, null);
		}
	}
}
