using LayoutManager;
using LayoutManager.CommonUI;
using MethodDispatcher;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for TraceManager.
    /// </summary>
    public partial class TraceManager : Form {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        private readonly ListViewStringColumnsSorter sorter;

        public TraceManager() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            FillList();

            if (File.Exists(DefaultTraceFlagsFilename))
                Restore(DefaultTraceFlagsFilename);

            sorter = new ListViewStringColumnsSorter(listViewSwitches);

            SetRadioButtonLayout();
        }

        private void FillList() {
            Cursor previousCursor = Cursor;

            Cursor = Cursors.WaitCursor;

            listViewSwitches.Items.Clear();
            listViewSwitches.BeginUpdate();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies) {
                Type[] assemblyTypes = assembly.GetTypes();

                foreach (Type assemblyType in assemblyTypes) {
                    LookForSwitchObjects(assemblyType, radioButtonApplicationSwitches.Checked ? typeof(ILayoutSwitch) : typeof(Switch));
                    Application.DoEvents();
                }
            }

            Cursor = previousCursor;

            listViewSwitches.EndUpdate();
        }

        private void LookForSwitchObjects(Type type, Type switchType) {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (FieldInfo field in fields) {
                if ((field.FieldType.GetInterface(switchType.Name) != null || field.FieldType.IsSubclassOf(switchType)) && field.IsStatic) {
                    var theSwitch = (Switch?)field.GetValue(null);

                    if (theSwitch != null)
                        listViewSwitches.Items.Add(new SwitchItem(theSwitch));
                }
            }

            Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);

            foreach (Type nestedType in nestedTypes)
                LookForSwitchObjects(nestedType, switchType);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SetRadioButtonLayout() {
            var selectedSwitch = GetSelected();

            foreach (Control c in groupBoxSwitchValue.Controls) {
                if (c is RadioButton button)
                    button.Visible = false;
            }

            if (selectedSwitch is TraceSwitch) {
                foreach (RadioButton rb in new RadioButton[] { radioButtonTraceNone, radioButtonTraceError, radioButtonWarnings, radioButtonTraceInfo, radioButtonTraceVerbose })
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

        private Switch? GetSelected() {
            return listViewSwitches.SelectedItems.Count > 0 ? ((SwitchItem?)listViewSwitches.SelectedItems[0])?.Switch : null;
        }

        private void UpdateSelected() {
            var selection = listViewSwitches.SelectedItems[0] as SwitchItem;

            selection?.Update();
        }

        private void ListViewSwitches_SelectedIndexChanged(object? sender, System.EventArgs e) {
            var selectedSwitch = GetSelected();

            SetRadioButtonLayout();

            if (selectedSwitch != null) {
                if (selectedSwitch is TraceSwitch traceSwitch) {
                    switch (traceSwitch.Level) {
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
                else if (selectedSwitch is BooleanSwitch booleanSwitch) {
                    if (booleanSwitch.Enabled)
                        radioButtonBooleanOn.Checked = true;
                    else
                        radioButtonBooleanOff.Checked = true;
                }
            }
        }

        private void RadioButtonTraceNone_CheckedChanged(object? sender, System.EventArgs e) {
            if (GetSelected() is TraceSwitch selectedSwitch)
                selectedSwitch.Level = TraceLevel.Off;

            UpdateSelected();
        }

        private void RadioButtonTraceError_CheckedChanged(object? sender, System.EventArgs e) {
            if (GetSelected() is TraceSwitch selectedSwitch)
                selectedSwitch.Level = TraceLevel.Error;
            UpdateSelected();
        }

        private void RadioButtonWarnings_CheckedChanged(object? sender, System.EventArgs e) {
            if (GetSelected() is TraceSwitch selectedSwitch)
                selectedSwitch.Level = TraceLevel.Warning;
            UpdateSelected();
        }

        private void RadioButtonTraceInfo_CheckedChanged(object? sender, System.EventArgs e) {
            if (GetSelected() is TraceSwitch selectedSwitch)
                selectedSwitch.Level = TraceLevel.Info;
            UpdateSelected();
        }

        private void RadioButtonTraceVerbose_CheckedChanged(object? sender, System.EventArgs e) {
            if (GetSelected() is TraceSwitch selectedSwitch)
                selectedSwitch.Level = TraceLevel.Verbose;
            UpdateSelected();
        }

        private void RadioButtonBooleanOff_CheckedChanged(object? sender, System.EventArgs e) {
            if (GetSelected() is BooleanSwitch selectedSwitch)
                selectedSwitch.Enabled = false;
            UpdateSelected();
        }

        private void RadioButtonBooleanOn_CheckedChanged(object? sender, System.EventArgs e) {
            if (GetSelected() is BooleanSwitch selectedSwitch)
                selectedSwitch.Enabled = true;
            UpdateSelected();
        }

        private void SwitchTypeChanged(object? sender, EventArgs e) {
            FillList();
        }

        private void ClearAll() {
            foreach(SwitchItem item in listViewSwitches.Items) {
                if (item.Switch is TraceSwitch traceSwitch)
                    traceSwitch.Level = TraceLevel.Off;
                else if (item.Switch is BooleanSwitch booleanSwitch)
                    booleanSwitch.Enabled = false;
                item.Update();
            }

            ListViewSwitches_SelectedIndexChanged(listViewSwitches, EventArgs.Empty);
        }

        private void Restore(XmlElement statesElement) {
            ClearAll();

            foreach (XmlElement stateElement in statesElement) {
                var name = stateElement.GetAttribute("Name");

                if(name != null) {
                    var item = listViewSwitches.Items.Find(name, false).FirstOrDefault() as SwitchItem;

                    if(item != null) {
                        if(item.Switch is TraceSwitch traceSwitch) {
                            traceSwitch.Level = stateElement.GetAttribute("Value") switch {
                                "Error" => TraceLevel.Error,
                                "Warning" => TraceLevel.Warning,
                                "Info" => TraceLevel.Info,
                                "Verbose" => TraceLevel.Verbose,
                                _ => TraceLevel.Off,
                            };
                        }
                        else if(item.Switch is BooleanSwitch booleanSwitch)
                            booleanSwitch.Enabled = true;
                    }
                }
            }
        }

        private void Restore(string filename) {
            XmlDocument doc = new();

            doc.Load(filename);
            if (doc.DocumentElement != null)
                Restore(doc.DocumentElement);
        }

        private string DefaultTraceFlagsFilename => Path.ChangeExtension(LayoutController.LayoutRuntimeStateFilename, ".traceFlags");

        private void Save(string filename) {
            XmlDocument doc = new();

            doc.LoadXml("<TraceFlags />");
            foreach (SwitchItem item in listViewSwitches.Items)
                item.AddState(doc.DocumentElement!);

            doc.Save(filename);
        }

        private class SwitchItem : ListViewItem {
            public SwitchItem(Switch theSwitch) {
                this.Switch = theSwitch;
                this.Name = theSwitch.DisplayName;
                Text = theSwitch.DisplayName;
                SubItems.Add(theSwitch.Description);
                SubItems.Add("");
                Update();
            }

            public void Update() {
                if (Switch is TraceSwitch traceSwitch) {
                    SubItems[2].Text =traceSwitch.Level switch {
                        TraceLevel.Off => "",
                        TraceLevel.Error => "Errors",
                        TraceLevel.Warning => "Warnings",
                        TraceLevel.Info => "Info",
                        TraceLevel.Verbose => "Verbose",
                        _ => "Other - " + traceSwitch.Level,
                    };
                }
                else if (Switch is BooleanSwitch booleanSwitch) {
                    if (booleanSwitch.Enabled)
                        SubItems[2].Text = "On";
                    else
                        SubItems[2].Text = "";
                }
            }

            public void AddState(XmlElement statesElement) {
                if (Switch is TraceSwitch traceSwitch) {
                    if (traceSwitch.Level != TraceLevel.Off) {
                        var stateElement = statesElement.OwnerDocument.CreateElement("State");

                        stateElement.SetAttribute("Name", Switch.DisplayName);

                        stateElement.SetAttribute("Value",
                            traceSwitch.Level switch {
                                TraceLevel.Off => "Off",
                                TraceLevel.Error => "Error",
                                TraceLevel.Warning => "Warning",
                                TraceLevel.Info => "Info",
                                TraceLevel.Verbose => "Verbose",
                                _ => "Unknown",
                            }
                        );

                        statesElement.AppendChild(stateElement);
                    }
                }
                else if(Switch is BooleanSwitch booleanSwitch && booleanSwitch.Enabled) {
                    var stateElement = statesElement.OwnerDocument.CreateElement("State");

                    stateElement.SetAttribute("Name", Switch.DisplayName);
                    stateElement.SetAttribute("Value", "Enabled");
                    statesElement.AppendChild(stateElement);
                }
            }

            public Switch Switch { get; }
        }

        private void ButtonClose_Click(object sender, EventArgs e) {
            Save(DefaultTraceFlagsFilename);
            Close();
        }

        private void ButtonResetAll_Click(object sender, EventArgs e) {
            ClearAll();
        }

        private void ButtonSave_Click(object sender, EventArgs e) {
            try {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    Save(saveFileDialog.FileName);
            } catch(Exception ex) {
                MessageBox.Show($"Save failed: {ex.Message}");
            }
        }

        private void ButtonLoad_Click(object sender, EventArgs e) {
            try {
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                    Restore(openFileDialog.FileName);
            } catch(Exception ex) {
                MessageBox.Show($"Load failed: {ex.Message}");
            }
        }
    }

    [LayoutModule("Trace Manager")]
    internal class TraceManagerModule : LayoutModuleBase {

        [DispatchTarget]
        private void ToolsMenuOpenRequest(MenuOrMenuItem toolsMenu) {
            toolsMenu.Items.Add("&Trace switches", null, new EventHandler(this.OnSetTraceSwitches));
        }

        private void OnSetTraceSwitches(object? sender, EventArgs e) {
            TraceManager traceManager = new TraceManager();

            traceManager.ShowDialog();
        }
    }
}
