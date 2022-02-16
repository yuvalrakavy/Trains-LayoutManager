using System.Xml;
using MethodDispatcher;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TrainDriverComboBox.
    /// </summary>
    public partial class TrainDriverComboBox : UserControl {

        private TrainCommonInfo? train;

        public TrainCommonInfo Train { get => Ensure.NotNull<TrainCommonInfo>(OptionalTrain); set => OptionalTrain = value; }

        public TrainCommonInfo? OptionalTrain {
            set {
                train = value;

                if (train != null)
                    Initialize();
            }

            get {
                return train;
            }
        }

        public bool ValidateInput() {
            if (comboBoxDrivers.SelectedItem == null) {
                MessageBox.Show(this, "No valid driver is selected", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxDrivers.Focus();
                return false;
            }

            return true;
        }

        public bool Commit() {
            if (!ValidateInput())
                return false;

            DriverItem selected = (DriverItem)comboBoxDrivers.SelectedItem;

            Train.DriverElement = selected.DriverElement;
            return true;
        }

        public TrainDriverComboBox() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private void Initialize() {
            XmlDocument driversDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            // enum the possible drivers, and build a menu
            driversDoc.LoadXml("<Drivers />");
            Dispatch.Call.EnumTrainDrivers(driversDoc.DocumentElement!);

            foreach (XmlElement driverElement in driversDoc.DocumentElement!)
                comboBoxDrivers.Items.Add(new DriverItem(driverElement));

            foreach (DriverItem driverItem in comboBoxDrivers.Items)
                if (driverItem.Type == Train.Driver.Type) {
                    comboBoxDrivers.SelectedItem = driverItem;
                    break;
                }

            SetDriverSettingButtonState();
        }

        private bool IsSelectedHasSettings() {
            DriverItem selected = (DriverItem)comboBoxDrivers.SelectedItem;

            return selected != null && Dispatch.Call.QueryDriverDialog(selected.DriverElement);
        }

        private void SetDriverSettingButtonState() {
            var selected = (DriverItem)comboBoxDrivers.SelectedItem;

            if (selected != null)
                buttonDriverSettings.Enabled = IsSelectedHasSettings();
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

        private void ComboBoxDrivers_SelectedIndexChanged(object? sender, EventArgs e) {
            SetDriverSettingButtonState();

            if (IsSelectedHasSettings())
                buttonDriverSettings.PerformClick();
        }

        private void ButtonDriverSettings_Click(object? sender, EventArgs e) {
            DriverItem selected = (DriverItem)comboBoxDrivers.SelectedItem;

            if (selected != null && train != null)
                Dispatch.Call.EditDriverSettings(train, selected.DriverElement);
        }

        private class DriverItem {
            public DriverItem(XmlElement driverElement) {
                this.DriverElement = driverElement;
            }

            public XmlElement DriverElement { get; }

            public override string ToString() => DriverElement.GetAttribute("TypeName");

            public string Type => DriverElement.GetAttribute("Type");
        }
    }
}
