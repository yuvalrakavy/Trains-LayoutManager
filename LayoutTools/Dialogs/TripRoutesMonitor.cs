using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace LayoutManager.Tools.Dialogs
{
	/// <summary>
	/// Summary description for TripRoutesMonitor.
	/// </summary>
	public class TripRoutesMonitor : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonShowOrigin;
		private System.Windows.Forms.Button buttonShowDestination;
		private System.Windows.Forms.NumericUpDown numericUpDownRoute;
		private System.Windows.Forms.Label labelRouteCount;
		private System.Windows.Forms.Button buttonClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		LayoutEventManager	eventManager;
		ITripRoutes			tripRoutes;
		LayoutSelection		activeSelection = null;
		int					activeRouteIndex = -1;

		public TripRoutesMonitor(LayoutEventManager eventManager, ITripRoutes tripRoutes)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.eventManager = eventManager;
			this.tripRoutes = tripRoutes;

			labelRouteCount.Text = "of " + tripRoutes.Count + " possible routes";
			numericUpDownRoute.Minimum = 1;
			numericUpDownRoute.Maximum = tripRoutes.Count;
			numericUpDownRoute.Value = 1;

			updateRouteSelection(null, new EventArgs());
		}

		private void updateRouteSelection(object sender, EventArgs e) {
			if(activeSelection != null) {
				activeSelection.Hide();
				activeSelection = null;
			}

			activeRouteIndex = (int)numericUpDownRoute.Value - 1;

			activeSelection = tripRoutes.Routes[activeRouteIndex].Selection;
			activeSelection.Display(new LayoutSelectionLook(Color.Green));
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonShowOrigin = new System.Windows.Forms.Button();
			this.buttonShowDestination = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDownRoute = new System.Windows.Forms.NumericUpDown();
			this.labelRouteCount = new System.Windows.Forms.Label();
			this.buttonClose = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoute)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.buttonShowOrigin,
																					this.buttonShowDestination});
			this.groupBox1.Location = new System.Drawing.Point(8, 80);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(184, 48);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Show:";
			// 
			// buttonShowOrigin
			// 
			this.buttonShowOrigin.Location = new System.Drawing.Point(8, 16);
			this.buttonShowOrigin.Name = "buttonShowOrigin";
			this.buttonShowOrigin.TabIndex = 0;
			this.buttonShowOrigin.Text = "&Origin";
			this.buttonShowOrigin.Click += new System.EventHandler(this.buttonShowOrigin_Click);
			// 
			// buttonShowDestination
			// 
			this.buttonShowDestination.Location = new System.Drawing.Point(96, 16);
			this.buttonShowDestination.Name = "buttonShowDestination";
			this.buttonShowDestination.TabIndex = 1;
			this.buttonShowDestination.Text = "&Destination";
			this.buttonShowDestination.Click += new System.EventHandler(this.buttonShowDestination_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Route: ";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numericUpDownRoute
			// 
			this.numericUpDownRoute.Location = new System.Drawing.Point(80, 9);
			this.numericUpDownRoute.Name = "numericUpDownRoute";
			this.numericUpDownRoute.Size = new System.Drawing.Size(40, 20);
			this.numericUpDownRoute.TabIndex = 1;
			this.numericUpDownRoute.ValueChanged += new System.EventHandler(this.updateRouteSelection);
			// 
			// labelRouteCount
			// 
			this.labelRouteCount.Location = new System.Drawing.Point(128, 8);
			this.labelRouteCount.Name = "labelRouteCount";
			this.labelRouteCount.Size = new System.Drawing.Size(120, 23);
			this.labelRouteCount.TabIndex = 2;
			this.labelRouteCount.Text = "of XX possible routes";
			this.labelRouteCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonClose
			// 
			this.buttonClose.Location = new System.Drawing.Point(208, 96);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.TabIndex = 4;
			this.buttonClose.Text = "Close";
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// TripRoutesMonitor
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 134);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonClose,
																		  this.numericUpDownRoute,
																		  this.label1,
																		  this.groupBox1,
																		  this.labelRouteCount});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "TripRoutesMonitor";
			this.Text = "Trip Routes";
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRoute)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonClose_Click(object sender, System.EventArgs e) {
			if(activeSelection != null) {
				activeSelection.Hide();
				activeSelection = null;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonShowOrigin_Click(object sender, System.EventArgs e) {
			eventManager.Event(new LayoutEvent(tripRoutes.Routes[activeRouteIndex].SourceTrack, "ensure-component-visible", null, true));
		}

		private void buttonShowDestination_Click(object sender, System.EventArgs e) {
			eventManager.Event(new LayoutEvent(tripRoutes.Routes[activeRouteIndex].DestinationTrack, "ensure-component-visible", null, true));
		}
	}
}
