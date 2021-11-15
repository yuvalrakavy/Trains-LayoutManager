namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LengthInput.
    /// </summary>
    public class LengthInput : UnitInput {
        public LengthInput() {
            DefineUnit("cm", 1.0, 0.0);
            DefineUnit("inch", 2.54, 0.0);
            UnitDefinitionDone();

            SelectUnit(0);
        }

        public void Initialize() {
            var defaultLengthUnit = (String?)EventManager.Event(new LayoutEvent("get-default-length-unit", Parent));

            if (defaultLengthUnit != null)
                SelectUnit(defaultLengthUnit);
        }
    }
}
