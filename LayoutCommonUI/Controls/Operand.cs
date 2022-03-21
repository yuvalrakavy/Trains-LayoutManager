using System.Xml;

#nullable enable
namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for Operand.
    /// </summary>
    public partial class Operand : UserControl, IObjectHasXml {

        public Operand() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        #region Properties

        public string Suffix { get; set; } = "";

        public XmlElement Element { get => Ensure.NotNull<XmlElement>(OptionalElement); set => OptionalElement = value; }

        public XmlElement? OptionalElement { set; get; }

        public string DefaultAccess { get; set; } = "Property";

        public Type[]? AllowedTypes { get; set; }

        public bool ValueIsBoolean { get; set; }

        public bool ValueIsOnOff { set; get; }

        #endregion

        public void Initialize() {
            if (OptionalElement == null || AllowedTypes == null)
                throw new ArgumentException("Element or AllowedTypes not set");

            operandValueOf.OptionalElement = Element;
            operandValueOf.AllowedTypes = AllowedTypes;
            operandValueOf.Suffix = Suffix;
            operandValueOf.Initialize();

            if (ValueIsBoolean) {
                textBoxValue.Visible = false;
                linkMenu1Boolean.Visible = true;
                radioButtonValue.Text = "";
                linkMenu1Boolean.Location = textBoxValue.Location;

                if (ValueIsOnOff)
                    linkMenu1Boolean.Options = new string[] { "On", "Off" };
            }
            else
                linkMenu1Boolean.Visible = false;

            var symbolAccess = Element.GetAttribute($"Symbol{Suffix}Access");

            if (string.IsNullOrEmpty(symbolAccess))
                symbolAccess = DefaultAccess;

            if (symbolAccess == "Value") {
                radioButtonValue.Checked = true;

                if (Element.HasAttribute($"Value{Suffix}")) {
                    if (ValueIsBoolean)
                        linkMenu1Boolean.SelectedIndex = (bool)Element.AttributeValue($"Value{Suffix}") ? 0 : 1;
                    else
                        textBoxValue.Text = Element.GetAttribute($"Value{Suffix}");
                }
                else
                    textBoxValue.Text = "";

                linkMenu1Boolean.Text = linkMenu1Boolean.Options[linkMenu1Boolean.SelectedIndex];
                linkMenu1Boolean.Enabled = true;
            }
            else
                radioButtonPropertyOrAttribute.Checked = true;
        }

        public bool ValidateInput() {
            if (radioButtonPropertyOrAttribute.Checked) {
                if (!operandValueOf.ValidateInput())
                    return false;
            }

            return true;
        }

        public bool Commit() {
            var accessAttribute = $"Symbol{Suffix}Access";

            if (radioButtonValue.Checked) {
                Element.SetAttribute(accessAttribute, "Value");

                if (ValueIsBoolean) {
                    Element.SetAttributeValue($"Value{Suffix}", linkMenu1Boolean.SelectedIndex == 0);
                    Element.SetAttribute($"Type{Suffix}", "Booelan");
                }
                else {
                    Element.SetAttribute($"Value{Suffix}", textBoxValue.Text);
                    Element.SetAttribute($"Type{Suffix}", "String");
                }
            }
            else
                operandValueOf.Commit();

            return true;
        }


        private void RadioButtonValue_CheckedChanged(object? sender, EventArgs e) {
#if OLD
			linkMenu1Boolean.Enabled = true;
			textBoxValue.Enabled = true;
			comboBoxSymbol.Enabled = false;
			comboBoxTag.Enabled = false;
			linkMenuPropertyOrAttribute.Enabled = false;
#endif
        }

        private void TextBoxValue_TextChanged(object? sender, EventArgs e) {
            radioButtonValue.Checked = true;
        }

        private void OperandValueOf_ValueChanged(object? sender, EventArgs e) {
            radioButtonPropertyOrAttribute.Checked = true;
        }
    }
}
