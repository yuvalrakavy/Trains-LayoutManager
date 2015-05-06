using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls
{
	/// <summary>
	/// Summary description for GlobalPolicyList.
	/// </summary>
	public class GlobalPolicyList : PolicyList
	{
		private Button buttonStartStop;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public GlobalPolicyList()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			ShowIfRunning = true;
   		}

		protected override void UpdateButtons(object sender, EventArgs e) {
			PolicyList.PolicyItem	selected = GetSelection();

			if(selected == null || !LayoutController.IsOperationMode) {
				buttonStartStop.Text = "Activate";
				buttonStartStop.Enabled = false;
			}
			else {
				if(selected.Policy.IsActive)
					buttonStartStop.Text = "Deactivate";
				else
					buttonStartStop.Text = "Activate";

				buttonStartStop.Enabled = true;
			}
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonStartStop = new Button();
			this.SuspendLayout();
			// 
			// buttonStartStop
			// 
			this.buttonStartStop.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonStartStop.Location = new System.Drawing.Point(248, 128);
			this.buttonStartStop.Name = "buttonStartStop";
			this.buttonStartStop.Size = new System.Drawing.Size(72, 23);
			this.buttonStartStop.TabIndex = 4;
			this.buttonStartStop.Text = "&Deactivate";
			this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
			// 
			// GlobalPolicyList
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonStartStop});
			this.Name = "GlobalPolicyList";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonStartStop_Click(object sender, System.EventArgs e) {
			PolicyList.PolicyItem	selected = GetSelection();

			if(selected != null) {
				LayoutEventScript	runningScript = (LayoutEventScript)EventManager.Event(new LayoutEvent(selected.Policy.Id, "get-active-event-script"));

				if(runningScript != null)
					runningScript.Dispose();
				else {
					runningScript = EventManager.EventScript("Layout policy " + selected.Policy.Name, selected.Policy.EventScriptElement, new Guid[] { }, null);
					runningScript.Id = selected.Policy.Id;

					runningScript.Reset();
				}

				selected.Update();
				UpdateButtons(null, null);
			}
		}
	}
}
