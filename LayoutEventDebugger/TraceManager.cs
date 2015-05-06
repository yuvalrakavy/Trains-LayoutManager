using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using LayoutManager;
using LayoutManager.CommonUI;

namespace LayoutEventDebugger
{
	/// <summary>
	/// Summary description for TraceManager.
	/// </summary>
	public class TraceManager : Form {
		private RadioButton radioButtonTraceNone;
		private RadioButton radioButtonTraceError;
		private RadioButton radioButtonTraceInfo;
		private ListView listViewSwitches;
		private RadioButton radioButtonTraceVerbose;
		private ColumnHeader columnHeaderSwtchName;
		private ColumnHeader columnHeaderSwitchSetting;
		private Button buttonClose;
		private RadioButton radioButtonWarnings;
		private RadioButton radioButtonBooleanOff;
		private RadioButton radioButtonBooleanOn;
		private GroupBox groupBoxSwitchValue;
		private ColumnHeader columnHeaderDescription;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;
		private GroupBox groupBox1;
		private RadioButton radioButtonAllSwitches;
		private RadioButton radioButtonApplicationSwitches;

		ListViewStringColumnsSorter	sorter;

		public TraceManager()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			fillList();
			sorter = new ListViewStringColumnsSorter(listViewSwitches);

			setRadioButtonLayout();
		}

		private void fillList() {
			Cursor previousCursor = Cursor;

			Cursor = Cursors.WaitCursor;

			listViewSwitches.Items.Clear();
			listViewSwitches.BeginUpdate();

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach(Assembly assembly in assemblies) {
				Type[] assemblyTypes = assembly.GetTypes();

				foreach(Type assemblyType in assemblyTypes) {
					lookForSwitchObjects(assemblyType, radioButtonApplicationSwitches.Checked ? typeof(ILayoutSwitch) : typeof(Switch));
					Application.DoEvents();
				}
			}

			Cursor = previousCursor;


			listViewSwitches.EndUpdate();
		}

		private void lookForSwitchObjects(Type type, Type switchType) {
			FieldInfo[]	fields = type.GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static);

			foreach(FieldInfo field in fields) {
				if((field.FieldType.GetInterface(switchType.Name) != null || field.FieldType.IsSubclassOf(switchType)) && field.IsStatic) {
					Switch	theSwitch = (Switch)field.GetValue(null);

					if(theSwitch != null)
						listViewSwitches.Items.Add(new SwitchItem(theSwitch));
				}
			}

			Type[]	nestedTypes = type.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic);

			foreach(Type nestedType in nestedTypes)
				lookForSwitchObjects(nestedType, switchType);

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
            this.listViewSwitches = new ListView();
            this.columnHeaderSwtchName = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderDescription = ((ColumnHeader)(new ColumnHeader()));
            this.columnHeaderSwitchSetting = ((ColumnHeader)(new ColumnHeader()));
            this.groupBoxSwitchValue = new GroupBox();
            this.radioButtonBooleanOff = new RadioButton();
            this.radioButtonTraceNone = new RadioButton();
            this.radioButtonTraceError = new RadioButton();
            this.radioButtonTraceInfo = new RadioButton();
            this.radioButtonTraceVerbose = new RadioButton();
            this.radioButtonWarnings = new RadioButton();
            this.radioButtonBooleanOn = new RadioButton();
            this.buttonClose = new Button();
            this.groupBox1 = new GroupBox();
            this.radioButtonAllSwitches = new RadioButton();
            this.radioButtonApplicationSwitches = new RadioButton();
            this.groupBoxSwitchValue.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewSwitches
            // 
            this.listViewSwitches.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewSwitches.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderSwtchName,
            this.columnHeaderDescription,
            this.columnHeaderSwitchSetting});
            this.listViewSwitches.FullRowSelect = true;
            this.listViewSwitches.HideSelection = false;
            this.listViewSwitches.Location = new System.Drawing.Point(8, 8);
            this.listViewSwitches.MultiSelect = false;
            this.listViewSwitches.Name = "listViewSwitches";
            this.listViewSwitches.Size = new System.Drawing.Size(532, 160);
            this.listViewSwitches.TabIndex = 0;
            this.listViewSwitches.UseCompatibleStateImageBehavior = false;
            this.listViewSwitches.View = System.Windows.Forms.View.Details;
            this.listViewSwitches.SelectedIndexChanged += new System.EventHandler(this.listViewSwitches_SelectedIndexChanged);
            // 
            // columnHeaderSwtchName
            // 
            this.columnHeaderSwtchName.Text = "Name";
            this.columnHeaderSwtchName.Width = 173;
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "Description";
            this.columnHeaderDescription.Width = 238;
            // 
            // columnHeaderSwitchSetting
            // 
            this.columnHeaderSwitchSetting.Text = "Setting";
            this.columnHeaderSwitchSetting.Width = 111;
            // 
            // groupBoxSwitchValue
            // 
            this.groupBoxSwitchValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonBooleanOff);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceNone);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceError);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceInfo);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonTraceVerbose);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonWarnings);
            this.groupBoxSwitchValue.Controls.Add(this.radioButtonBooleanOn);
            this.groupBoxSwitchValue.Location = new System.Drawing.Point(8, 176);
            this.groupBoxSwitchValue.Name = "groupBoxSwitchValue";
            this.groupBoxSwitchValue.Size = new System.Drawing.Size(160, 120);
            this.groupBoxSwitchValue.TabIndex = 1;
            this.groupBoxSwitchValue.TabStop = false;
            this.groupBoxSwitchValue.Text = "Set selected switch to:";
            // 
            // radioButtonBooleanOff
            // 
            this.radioButtonBooleanOff.Location = new System.Drawing.Point(88, 16);
            this.radioButtonBooleanOff.Name = "radioButtonBooleanOff";
            this.radioButtonBooleanOff.Size = new System.Drawing.Size(104, 16);
            this.radioButtonBooleanOff.TabIndex = 4;
            this.radioButtonBooleanOff.Text = "Off";
            this.radioButtonBooleanOff.CheckedChanged += new System.EventHandler(this.radioButtonBooleanOff_CheckedChanged);
            // 
            // radioButtonTraceNone
            // 
            this.radioButtonTraceNone.Location = new System.Drawing.Point(8, 18);
            this.radioButtonTraceNone.Name = "radioButtonTraceNone";
            this.radioButtonTraceNone.Size = new System.Drawing.Size(104, 16);
            this.radioButtonTraceNone.TabIndex = 0;
            this.radioButtonTraceNone.Text = "None";
            this.radioButtonTraceNone.CheckedChanged += new System.EventHandler(this.radioButtonTraceNone_CheckedChanged);
            // 
            // radioButtonTraceError
            // 
            this.radioButtonTraceError.Location = new System.Drawing.Point(8, 37);
            this.radioButtonTraceError.Name = "radioButtonTraceError";
            this.radioButtonTraceError.Size = new System.Drawing.Size(104, 16);
            this.radioButtonTraceError.TabIndex = 1;
            this.radioButtonTraceError.Text = "Errors";
            this.radioButtonTraceError.CheckedChanged += new System.EventHandler(this.radioButtonTraceError_CheckedChanged);
            // 
            // radioButtonTraceInfo
            // 
            this.radioButtonTraceInfo.Location = new System.Drawing.Point(8, 75);
            this.radioButtonTraceInfo.Name = "radioButtonTraceInfo";
            this.radioButtonTraceInfo.Size = new System.Drawing.Size(104, 16);
            this.radioButtonTraceInfo.TabIndex = 2;
            this.radioButtonTraceInfo.Text = "Info";
            this.radioButtonTraceInfo.CheckedChanged += new System.EventHandler(this.radioButtonTraceInfo_CheckedChanged);
            // 
            // radioButtonTraceVerbose
            // 
            this.radioButtonTraceVerbose.Location = new System.Drawing.Point(8, 94);
            this.radioButtonTraceVerbose.Name = "radioButtonTraceVerbose";
            this.radioButtonTraceVerbose.Size = new System.Drawing.Size(104, 16);
            this.radioButtonTraceVerbose.TabIndex = 3;
            this.radioButtonTraceVerbose.Text = "Verbose";
            this.radioButtonTraceVerbose.CheckedChanged += new System.EventHandler(this.radioButtonTraceVerbose_CheckedChanged);
            // 
            // radioButtonWarnings
            // 
            this.radioButtonWarnings.Location = new System.Drawing.Point(8, 56);
            this.radioButtonWarnings.Name = "radioButtonWarnings";
            this.radioButtonWarnings.Size = new System.Drawing.Size(104, 16);
            this.radioButtonWarnings.TabIndex = 1;
            this.radioButtonWarnings.Text = "Warnings";
            this.radioButtonWarnings.CheckedChanged += new System.EventHandler(this.radioButtonWarnings_CheckedChanged);
            // 
            // radioButtonBooleanOn
            // 
            this.radioButtonBooleanOn.Location = new System.Drawing.Point(88, 40);
            this.radioButtonBooleanOn.Name = "radioButtonBooleanOn";
            this.radioButtonBooleanOn.Size = new System.Drawing.Size(104, 16);
            this.radioButtonBooleanOn.TabIndex = 4;
            this.radioButtonBooleanOn.Text = "On";
            this.radioButtonBooleanOn.CheckedChanged += new System.EventHandler(this.radioButtonBooleanOn_CheckedChanged);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(468, 272);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.radioButtonAllSwitches);
            this.groupBox1.Controls.Add(this.radioButtonApplicationSwitches);
            this.groupBox1.Location = new System.Drawing.Point(183, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 56);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show:";
            // 
            // radioButtonAllSwitches
            // 
            this.radioButtonAllSwitches.AutoSize = true;
            this.radioButtonAllSwitches.Location = new System.Drawing.Point(6, 36);
            this.radioButtonAllSwitches.Name = "radioButtonAllSwitches";
            this.radioButtonAllSwitches.Size = new System.Drawing.Size(80, 17);
            this.radioButtonAllSwitches.TabIndex = 1;
            this.radioButtonAllSwitches.Text = "All switches";
            this.radioButtonAllSwitches.UseVisualStyleBackColor = true;
            this.radioButtonAllSwitches.CheckedChanged += new System.EventHandler(this.switchTypeChanged);
            // 
            // radioButtonApplicationSwitches
            // 
            this.radioButtonApplicationSwitches.AutoSize = true;
            this.radioButtonApplicationSwitches.Checked = true;
            this.radioButtonApplicationSwitches.Location = new System.Drawing.Point(6, 15);
            this.radioButtonApplicationSwitches.Name = "radioButtonApplicationSwitches";
            this.radioButtonApplicationSwitches.Size = new System.Drawing.Size(163, 17);
            this.radioButtonApplicationSwitches.TabIndex = 0;
            this.radioButtonApplicationSwitches.TabStop = true;
            this.radioButtonApplicationSwitches.Text = "Only this application switches";
            this.radioButtonApplicationSwitches.UseVisualStyleBackColor = true;
            this.radioButtonApplicationSwitches.CheckedChanged += new System.EventHandler(this.switchTypeChanged);
            // 
            // TraceManager
            // 
            this.AcceptButton = this.buttonClose;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(552, 302);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.groupBoxSwitchValue);
            this.Controls.Add(this.listViewSwitches);
            this.Name = "TraceManager";
            this.ShowInTaskbar = false;
            this.Text = "Trace Manager";
            this.groupBoxSwitchValue.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void setRadioButtonLayout() {
			Switch	selectedSwitch = getSelected();

			foreach(Control c in groupBoxSwitchValue.Controls) {
				if(c is RadioButton)
					((RadioButton)c).Visible = false;
			}

			if(selectedSwitch is TraceSwitch) {
				foreach(RadioButton rb in new RadioButton[] { radioButtonTraceNone, radioButtonTraceError, radioButtonWarnings, radioButtonTraceInfo, radioButtonTraceVerbose })
					rb.Visible = true;
			}
			else {
				SuspendLayout();
				radioButtonBooleanOff.Location = radioButtonTraceNone.Location;
				radioButtonBooleanOff.Visible = true;
				radioButtonBooleanOn.Location = radioButtonTraceError.Location;
				radioButtonBooleanOn.Visible = true;
				ResumeLayout();
			}
		}


		private Switch getSelected() {
			if(listViewSwitches.SelectedItems.Count > 0)
				return ((SwitchItem)listViewSwitches.SelectedItems[0]).Switch;
			return null;
		}

        private void updateSelected() {
            var selection = listViewSwitches.SelectedItems[0] as SwitchItem;

            selection.Update();
        }

		private void listViewSwitches_SelectedIndexChanged(object sender, System.EventArgs e) {
			Switch	selectedSwitch = getSelected();

			setRadioButtonLayout();

			if(selectedSwitch != null) {
				if(selectedSwitch is TraceSwitch) {
					switch(((TraceSwitch)selectedSwitch).Level) {

						case TraceLevel.Error:
							radioButtonTraceError.Checked = true;
							break;

						case TraceLevel.Warning:
							radioButtonWarnings.Checked = true;
							break;

						case TraceLevel.Info:
							radioButtonTraceInfo.Checked = true;
							break;

						case TraceLevel.Verbose:
							radioButtonTraceVerbose.Checked = true;
							break;

						default:
							radioButtonTraceNone.Checked = true;
							break;
					}
				}
				else if(selectedSwitch is BooleanSwitch) {
					if(((BooleanSwitch)selectedSwitch).Enabled)
						radioButtonBooleanOn.Checked = true;
					else
						radioButtonBooleanOff.Checked = true;
				}
			}
		}

		private void radioButtonTraceNone_CheckedChanged(object sender, System.EventArgs e) {
			TraceSwitch	selectedSwitch = getSelected() as TraceSwitch;

			if(selectedSwitch != null)
				selectedSwitch.Level = TraceLevel.Off;

            updateSelected();
		}

		private void radioButtonTraceError_CheckedChanged(object sender, System.EventArgs e) {
			TraceSwitch	selectedSwitch = getSelected() as TraceSwitch;

			if(selectedSwitch != null)
				selectedSwitch.Level = TraceLevel.Error;
            updateSelected();
        }

		private void radioButtonWarnings_CheckedChanged(object sender, System.EventArgs e) {
			TraceSwitch	selectedSwitch = getSelected() as TraceSwitch;

			if(selectedSwitch != null)
				selectedSwitch.Level = TraceLevel.Warning;
            updateSelected();
        }

		private void radioButtonTraceInfo_CheckedChanged(object sender, System.EventArgs e) {
			TraceSwitch	selectedSwitch = getSelected() as TraceSwitch;

			if(selectedSwitch != null)
				selectedSwitch.Level = TraceLevel.Info;
            updateSelected();
        }

		private void radioButtonTraceVerbose_CheckedChanged(object sender, System.EventArgs e) {
			TraceSwitch	selectedSwitch = getSelected() as TraceSwitch;

			if(selectedSwitch != null)
				selectedSwitch.Level = TraceLevel.Verbose;
            updateSelected();
        }

		private void radioButtonBooleanOff_CheckedChanged(object sender, System.EventArgs e) {
			BooleanSwitch	selectedSwitch = getSelected() as BooleanSwitch;

			if(selectedSwitch != null)
				selectedSwitch.Enabled = false;
            updateSelected();
        }

		private void radioButtonBooleanOn_CheckedChanged(object sender, System.EventArgs e) {
			BooleanSwitch	selectedSwitch = getSelected() as BooleanSwitch;

			if(selectedSwitch != null)
				selectedSwitch.Enabled = true;
            updateSelected();
        }

		private void switchTypeChanged(object sender, EventArgs e) {
			fillList();
		}

		class SwitchItem : ListViewItem {
			Switch			theSwitch;

			public SwitchItem(Switch theSwitch) {
				this.theSwitch = theSwitch;
				Text = theSwitch.DisplayName;
				SubItems.Add(theSwitch.Description);
				SubItems.Add("");
				Update();
			}

			public void Update() {
				if(theSwitch is TraceSwitch) {
					switch(((TraceSwitch)theSwitch).Level) {

						case TraceLevel.Off:
							SubItems[2].Text = "";
							break;

						case TraceLevel.Error:
							SubItems[2].Text = "Errors";
							break;

						case TraceLevel.Warning:
							SubItems[2].Text = "Warnings";
							break;

						case TraceLevel.Info:
							SubItems[2].Text = "Info";
							break;

						case TraceLevel.Verbose:
							SubItems[2].Text = "Verbose";
							break;

						default:
							SubItems[2].Text = "Other - " + ((TraceSwitch)theSwitch).Level;
							break;
					}
				}
				else if(theSwitch is BooleanSwitch) {
					if(((BooleanSwitch)theSwitch).Enabled)
						SubItems[2].Text = "On";
					else
						SubItems[2].Text = "";
				}
			}

			public Switch Switch {
				get {
					return theSwitch;
				}
			}
		}
	}

	[LayoutModule("Trace Manager")]
	class TraceManagerModule : LayoutModuleBase {

		[LayoutEvent("tools-menu-open-request")]
		private void OnToolsMenuOpenRequest(LayoutEvent e) {
			Menu toolsMenu = (Menu)e.Info;

			toolsMenu.MenuItems.Add("&Trace switches", new EventHandler(this.onSetTraceSwitches));
		}

		private void onSetTraceSwitches(object sender, EventArgs e) {
			TraceManager	traceManager = new TraceManager();

			traceManager.ShowDialog();
		}
	}
}
