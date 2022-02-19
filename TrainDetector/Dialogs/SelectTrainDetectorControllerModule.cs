using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MethodDispatcher;
using LayoutManager;
using LayoutManager.Components;
using LayoutManager.ControlComponents;
using LayoutManager.Model;
using TrainDetector;

#nullable enable
namespace TrainDetector.Dialogs {
    public partial class SelectTrainDetectorModule : Form {
        readonly LayoutSelection connectedComponents = new LayoutSelection();

        public SelectTrainDetectorModule(DetectedTrainDetectorController detectedController, List<TrainDetectorControllerModule> modules) {
            InitializeComponent();

            labelDetectedTrainDetector.Text = $"Controller named '{detectedController.Name} at {detectedController.IpEndPoint.Address} with {detectedController.SensorsCount} {(detectedController.SensorsCount == 1 ? "sensor" : "sensors")} ";
            radioButtonAssignToController.Checked = true;

            foreach(var module in modules) {
                listViewControllerModules.Items.Add(new ModuleItem(module));
            }

        }

        public TrainDetectorControllerModule? Module {
            get => radioButtonAddController.Checked ? null : (listViewControllerModules.SelectedItems.Count == 0 ? null : ((ModuleItem)listViewControllerModules.SelectedItems[0]).Module);
        }

        private class ModuleItem : ListViewItem {  
            public TrainDetectorControllerModule Module { get; private set; }

            public ModuleItem(TrainDetectorControllerModule module) {
                this.Module = module;
                this.Text = module.Label;
                this.SubItems.Add(module.NumberOfConnectionPoints.ToString());
            }
        }

        private void RadioButtonAssignToController_CheckedChanged(object? sender, EventArgs e) {
            listViewControllerModules.Enabled = radioButtonAssignToController.Checked;
            listViewControllerModules.HideSelection = !radioButtonAssignToController.Checked;
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private void ListViewControllerModules_SelectedIndexChanged(object? sender, EventArgs e) {
            if (radioButtonAssignToController.Checked && listViewControllerModules.SelectedItems.Count > 0) {
                var module = ((ModuleItem)listViewControllerModules.SelectedItems[0]).Module;

                connectedComponents.Add(from cp in module.ConnectionPoints where cp.Component != null select (ModelComponent)cp.Component!);
                connectedComponents.Display(new LayoutSelectionLook(Color.DarkRed));
                var aComponent = connectedComponents.Components.FirstOrDefault();

                if (aComponent != null)
                    Dispatch.Call.EnsureComponentVisible(LayoutController.ActiveFrameWindow.Id, aComponent, true);
            }
            else {
                connectedComponents.Clear();
                connectedComponents.Hide();
            }
        }

        private void SelectTrainDetectorModule_FormClosing(object? sender, FormClosingEventArgs e) {
            connectedComponents.Hide();
            connectedComponents.Clear();
        }
    }
}
