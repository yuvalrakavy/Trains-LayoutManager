using DigiFinder;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace NCDRelayController.Dialogs {
    public partial class DiscoverDigiDevicesDialog : Form {
        public DiscoverDigiDevicesDialog() {
            InitializeComponent();

            FindDevices();
        }

        public string? SelectedAddress { get; private set; }

        public async void FindDevices() {
            labelStatus.Text = "Searching the network for DIGI devices...";
            buttonSelect.Enabled = false;

            var devices = new ObservableCollection<DigiDevice>();

            devices.CollectionChanged += (s, ea) => {
                if (ea.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
                    this.Invoke((Action)(() => {
                        foreach (var d in ea.NewItems?.Cast<DigiDevice>() ?? Enumerable.Empty<DigiDevice>()) {
                            var item = new ListViewItem(new string[] { d.IpAddress?.ToString() ?? String.Empty, d.DeviceName ?? String.Empty });

                            listViewDevices.Items.Add(item);
                            if (listViewDevices.SelectedItems.Count == 0)
                                item.Selected = true;
                        }
                    }));
                }
            };

            var finder = new DigiFinder.DigiFinder();

            await finder.DiscoverDigiDevices(devices, timeout: 2000);

            if (listViewDevices.Items.Count == 0)
                labelStatus.Text = "No DIGI devices were found";
            else
                labelStatus.Text = "Select one of the following devices:";
        }

        private void ListViewDevices_SelectedIndexChanged(object? sender, EventArgs e) {
            if (listViewDevices.SelectedItems.Count > 0)
                buttonSelect.Enabled = true;
        }

        private void ListViewDevices_DoubleClick(object? sender, EventArgs e) {
            if (buttonSelect.Enabled)
                buttonSelect.PerformClick();
        }

        private void ButtonSelect_Click(object? sender, EventArgs e) {
            if (listViewDevices.SelectedItems.Count > 0)
                SelectedAddress = listViewDevices.SelectedItems[0].Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
