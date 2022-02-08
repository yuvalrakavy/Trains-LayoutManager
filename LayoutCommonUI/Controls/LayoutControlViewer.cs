using LayoutManager.Model;
using LayoutManager.Components;
using System.Diagnostics;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LayoutControlViewer.
    /// </summary>
    public partial class LayoutControlViewer : UserControl {
        private readonly float[] zooms = new float[] { 0.2F, 0.5F, 0.75F, 1.0F, 1.5F, 2.0F, 4.0F };
        private int zoomIndex = 3;
        private readonly string notInLocationText = "(Not in any location)";

        public LayoutControlViewer() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }

        internal void EnsureModuleVisible(ControlModule module) {
            if (((Item)comboBoxBusProvider.SelectedItem).Id != Guid.Empty && module.Bus.BusProviderId != ((Item)comboBoxBusProvider.SelectedItem).Id)
                ViewBusProvider(module.Bus.BusProviderId);

            if (((Item)comboBoxBus.SelectedItem).Value != null && ((Item)comboBoxBus.SelectedItem).Value != module.Bus.BusType.BusTypeName)
                ViewBusType(module.Bus.BusType.BusTypeName);

            if (comboBoxLocation.SelectedIndex != 0 && ((Item)comboBoxLocation.SelectedItem).Id != module.LocationId)
                ViewLocation(module.LocationId ?? Guid.Empty);
        }

        public void EnsureVisible(ControlConnectionPointReference connectionPointRef, bool select) {
            EnsureModuleVisible(connectionPointRef.Module);
            layoutControlBusViewer.EnsureVisible(connectionPointRef, select);
        }

        public void EnsureVisible(ControlModuleReference moduleRef, bool select) {
            EnsureModuleVisible(moduleRef.Module);
            layoutControlBusViewer.EnsureVisible(moduleRef, select);
        }

        public void DeselectAll() {
            layoutControlBusViewer.DeselectAll();
        }

        public void ViewBusProvider(Guid busProviderId) {
            foreach (Item item in comboBoxBusProvider.Items) {
                if (item.Id == busProviderId) {
                    comboBoxBusProvider.SelectedItem = item;
                    break;
                }
            }
        }

        public void ViewBusType(string busTypeName) {
            foreach (Item item in comboBoxBus.Items)
                if (item.Value == busTypeName) {
                    comboBoxBus.SelectedItem = item;
                    break;
                }
        }

        public void ViewLocation(Guid locationID) {
            foreach (Item item in comboBoxLocation.Items)
                if (item.Id == locationID) {
                    comboBoxLocation.SelectedItem = item;
                    break;
                }
        }

        public void Initialize() {
            if(DesignMode)
                throw new ApplicationException("Initialize called");

            layoutControlBusViewer.Initialize();
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);

            UpdateSelectors();
        }

        private void UpdateSelectors() {
            Guid idSelected = Guid.Empty;

            if (comboBoxBusProvider.Items.Count > 0)
                idSelected = ((Item)comboBoxBusProvider.SelectedItem).Id;

            comboBoxBusProvider.Items.Clear();
            comboBoxBusProvider.Items.Add(new Item("(All)", Guid.Empty));

            var busProviders = LayoutModel.Components<IModelComponentIsBusProvider>(LayoutModel.ActivePhases);

            foreach (var busProvider in busProviders)
                comboBoxBusProvider.Items.Add(new Item(busProvider.NameProvider.Name, busProvider.Id));
            comboBoxBusProvider.SelectedIndex = 0;

            UpdateBusSelector();

            ViewBusProvider(idSelected);

            idSelected = Guid.Empty;
            if (comboBoxLocation.Items.Count > 0 && comboBoxLocation.SelectedItem != null)
                idSelected = ((Item)comboBoxLocation.SelectedItem).Id;

            comboBoxLocation.Items.Clear();
            comboBoxLocation.Items.Add(new Item("(All)", Guid.Empty));
            comboBoxLocation.Items.Add(new Item(notInLocationText, Guid.Empty));

            foreach (LayoutControlModuleLocationComponent controlModuleLocation in LayoutModel.Components<LayoutControlModuleLocationComponent>(LayoutModel.ActivePhases))
                comboBoxLocation.Items.Add(new Item(controlModuleLocation.Name, controlModuleLocation.Id));

            foreach (Item item in comboBoxLocation.Items)
                if (item.Id == idSelected) {
                    comboBoxLocation.SelectedItem = item;
                    break;
                }
        }

        [LayoutEvent("model-loadded")]
        [LayoutEvent("control-buses-added")]
        [LayoutEvent("control-buses-removed")]
        [LayoutEvent("component-configuration-changed", SenderType = typeof(IModelComponentIsBusProvider))]
        [LayoutEvent("removed-from-model", SenderType = typeof(LayoutControlModuleLocationComponent))]
        [LayoutEvent("added-to-model", SenderType = typeof(LayoutControlModuleLocationComponent))]
        private void DoUpdateSelectors(LayoutEvent e) {
            UpdateSelectors();
            layoutControlBusViewer.Recalc();
        }

        [LayoutEvent("show-control-modules-location")]
        private void ShowControlModulesLocation(LayoutEvent e) {
            var controlModuleLocationId = e.Sender switch {
                LayoutControlModuleLocationComponent component => component.Id,
                Guid guid => guid,
                _ => throw new ArgumentException("Invalid sender for show-control-modules-location"),
            };

            foreach (Item item in comboBoxLocation.Items)
                if (item.Id == controlModuleLocationId) {
                    comboBoxLocation.SelectedItem = item;
                    break;
                }

            EventManager.Event(new LayoutEvent("show-layout-control", this));
        }

        private void UpdateBusSelector() {
            Item selectedBusProvider = (Item)comboBoxBusProvider.SelectedItem;

            string? selectedBusType = null;

            if (comboBoxBus.SelectedIndex >= 0)
                selectedBusType = ((Item)comboBoxBus.SelectedItem).Value;

            comboBoxBus.Items.Clear();
            comboBoxBus.Items.Add(new Item("(All)", null));

            if (selectedBusProvider.Id == Guid.Empty) {
                var busTypes = new Dictionary<string, object?>();

                foreach (var busProvider in LayoutModel.Components<IModelComponentIsBusProvider>(LayoutModel.ActivePhases)) {
                    IEnumerable<ControlBus> buses = LayoutModel.ControlManager.Buses.Buses(busProvider);

                    foreach (ControlBus bus in buses) {
                        if (!busTypes.ContainsKey(bus.BusTypeName)) {
                            comboBoxBus.Items.Add(new Item(bus.BusType.Name, bus.BusTypeName));
                            busTypes.Add(bus.BusTypeName, null);
                        }
                    }
                }
            }
            else {
                var busProvider = LayoutModel.Component<IModelComponentIsBusProvider>(selectedBusProvider.Id, LayoutModel.ActivePhases);

                if (busProvider != null) {
                    foreach (ControlBus bus in LayoutModel.ControlManager.Buses.Buses(busProvider))
                        comboBoxBus.Items.Add(new Item(bus.BusType.Name, bus.BusTypeName));
                }
                else
                    Debug.Assert(false);
            }

            bool selectionFound = false;

            foreach (Item item in comboBoxBus.Items)
                if (item.Value == selectedBusType) {
                    comboBoxBus.SelectedItem = item;
                    selectionFound = true;
                    break;
                }

            if (!selectionFound)
                comboBoxBus.SelectedIndex = 0;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                EventManager.Subscriptions.RemoveObjectSubscriptions(this);
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void ButtonClose_Click(object? sender, EventArgs e) {
            EventManager.Event(new LayoutEvent("hide-layout-control", this));
        }

        private void ComboBoxBusProvider_SelectedIndexChanged(object? sender, EventArgs e) {
            Item selected = (Item)comboBoxBusProvider.SelectedItem;

            layoutControlBusViewer.BusProviderId = selected.Id;
            UpdateBusSelector();
        }

        private void ComboBoxBus_SelectedIndexChanged(object? sender, EventArgs e) {
            Item selected = (Item)comboBoxBus.SelectedItem;

            layoutControlBusViewer.BusTypeName = selected.Value;
        }

        private void ComboBoxLocation_SelectedIndexChanged(object? sender, EventArgs e) {
            Item selected = (Item)comboBoxLocation.SelectedItem;

            if (selected.Id == Guid.Empty && selected.Text == notInLocationText)
                layoutControlBusViewer.ShowOnlyNotInLocation = true;
            else
                layoutControlBusViewer.ShowOnlyNotInLocation = false;

            layoutControlBusViewer.ModuleLocationID = selected.Id;
        }

        private void ButtonZoomOut_Click(object? sender, EventArgs e) {
            if (zoomIndex < zooms.Length - 1)
                layoutControlBusViewer.Zoom = zooms[++zoomIndex];
        }

        private void ButtonZoomIn_Click(object? sender, EventArgs e) {
            if (zoomIndex > 0)
                layoutControlBusViewer.Zoom = zooms[--zoomIndex];
        }

        private struct Item {
            public string Text;
            public Guid Id;
            public string? Value;

            public Item(string text, Guid id) {
                this.Text = text;
                this.Id = id;
                this.Value = null;
            }

            public Item(string text, string? theValue) {
                this.Text = text;
                this.Id = Guid.Empty;
                this.Value = theValue;
            }

            public override string ToString() => Text;
        }
    }
}
