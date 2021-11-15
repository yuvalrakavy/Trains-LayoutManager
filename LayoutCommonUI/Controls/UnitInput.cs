namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for UnitInput.
    /// </summary>
    public partial class UnitInput : UserControl {

        private readonly List<Unit> units = new();
        private Unit? currentUnit = null;
        private bool unitDefinitionUpdated = false;

        public UnitInput() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        public double NeutralValue {
            get {
                if (!unitDefinitionUpdated)
                    throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
                return currentUnit?.ToNeutralValue(UnitValue) ?? UnitValue;
            }

            set {
                if (!unitDefinitionUpdated)
                    throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
                textBoxValue.Text = (currentUnit?.ToUnitValue(value) ?? UnitValue).ToString();
            }
        }

        public double UnitValue {
            get {
                if (!unitDefinitionUpdated)
                    throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
                else
                    return IsEmpty ? 0.0 : Double.Parse(textBoxValue.Text);
            }

            set {
                if (!unitDefinitionUpdated)
                    throw new ApplicationException("No units are defined or you did not called UnitDefinitionDone()");
                else
                    textBoxValue.Text = value.ToString();
            }
        }

        public bool IsEmpty {
            get => textBoxValue.Text.Trim() == "";

            set {
                if (value)
                    textBoxValue.Text = "";
            }
        }

        public void DefineUnit(string unitName, double factor, double offset) {
            units.Add(new Unit(unitName, factor, offset, units.Count));
            unitDefinitionUpdated = false;
        }

        public void UnitDefinitionDone() {
            string[] unitNames = new string[units.Count];

            for (int i = 0; i < units.Count; i++)
                unitNames[i] = ((Unit)units[i]).UnitName;

            linkMenuUnits.Options = unitNames;
            unitDefinitionUpdated = true;
        }

        private void SelectUnit(Unit u) {
            double v = 0;

            if (!IsEmpty)
                v = NeutralValue;

            currentUnit = u;
            linkMenuUnits.SelectedIndex = currentUnit.Index;

            if (!IsEmpty)
                NeutralValue = v;
        }

        public void SelectUnit(String unitName) {
            bool found = false;

            foreach (Unit u in units)
                if (u.UnitName == unitName) {
                    SelectUnit(u);
                    found = true;
                    break;
                }

            if (!found)
                throw new ArgumentException("Invalid unit name");
        }

        public void SelectUnit(int unitIndex) {
            if (unitIndex < 0 || unitIndex >= units.Count)
                throw new ArgumentException("Invalid unit index", nameof(unitIndex));

            SelectUnit((Unit)units[unitIndex]);
        }

        public bool ReadOnly {
            set => textBoxValue.ReadOnly = value;
            get => textBoxValue.ReadOnly;
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

        private void LinkMenuUnits_ValueChanged(object? sender, EventArgs e) {
            SelectUnit(linkMenuUnits.SelectedIndex);
        }

        private class Unit {
            private readonly double factor;
            private readonly double offset;

            /// <summary>
            /// Declare a new unit
            /// </summary>
            /// <param name="unitName">The unit name</param>
            /// <param name="factor">factor for converting value in unit to neutral value</param>
            /// <param name="offset">offset fro converting value in unit to neutral value</param>
            public Unit(string unitName, double factor, double offset, int index) {
                this.UnitName = unitName;
                this.factor = factor;
                this.offset = offset;
                this.Index = index;
            }

            public double ToNeutralValue(double valueInUnit) => (valueInUnit * factor) + offset;

            public double ToUnitValue(double neutralValue) => (neutralValue - offset) / factor;

            public string UnitName { get; }

            public int Index { get; }
        }
    }
}
