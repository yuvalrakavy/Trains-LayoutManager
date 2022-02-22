using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using MethodDispatcher;
using LayoutManager.CommonUI.Controls;
using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Base form for the locomotive & locomotive type properties
    /// </summary>
    public partial class LocomotiveBasePropertiesForm : Form {
        private const string E_Functions = "Functions";
        private const string A_Light = "Light";
        private const string A_Store = "Store";
        private const string A_Number = "Number";
        private const string A_SpeedLimit = "SpeedLimit";
        private const string A_Name = "Name";
        private readonly Dictionary<string, Control> nameToControlMap = new();
        protected XmlElement element;

        public LocomotiveBasePropertiesForm() {
            var doc = new XmlDocument();
            doc.LoadXml("<Root/>");
            element = doc.DocumentElement!;
        }

        private void AddToNameToControlMap(Dictionary<string, Control> h, Control c) {
            if (c.Name != null && c.Name != "")
                h.Add(c.Name, c);

            foreach (Control cc in c.Controls)
                AddToNameToControlMap(h, cc);
        }

        protected void BuildControlNameMap() {
            AddToNameToControlMap(nameToControlMap, this);
        }

        protected LocomotiveCatalogInfo Catalog => LayoutModel.LocomotiveCatalog;

        protected void InitializeControls(XmlElement element, XmlElement storesElement) {
            LocomotiveTypeInfo locoType = new(element);

            BuildControlNameMap();

            listViewFunctions = (ListView)nameToControlMap["listViewFunctions"];
            imageGetter = (ImageGetter)nameToControlMap["imageGetter"];

            this.element = element;
            imageGetter.DefaultImage = Catalog.GetStandardImage(locoType.Kind, locoType.Origin);

            if (storesElement != null)
                SetStore(storesElement);

            attributesEditor = (AttributesEditor)nameToControlMap["attributesEditor"];
            attributesEditor.AttributesSource = typeof(LocomotiveInfo);
            attributesEditor.AttributesOwner = new AttributesOwner(element);

            SetLocomotiveTypeFields();
        }

        protected void SetLocomotiveTypeFields() {
            SetRadio("Kind", "Steam");
            SetRadio("Origin", "Europe");
            SetLength("lengthInput", "Length");
            SetSpeedLimit("textBoxSpeedLimit");

            SetImage();
            SetFunctions();
            SetGuage();

            InitDecoderTypeComboBox();

            if (element.HasAttribute("DecoderType"))
                SetDecoderType("comboBoxDecoderType", element.GetAttribute("DecoderType"));
        }

        private void InitDecoderTypeComboBox() {
            ComboBox comboBoxDecoderType = (ComboBox)nameToControlMap["comboBoxDecoderType"];

            if (comboBoxDecoderType != null) {
                TrackGuageSelector trackGuageSelector = (TrackGuageSelector)nameToControlMap["trackGuageSelector"];
                var previousSelectionIndex = comboBoxDecoderType.SelectedIndex;

                var decoderTypes = Dispatch.Call.EnumDecoderTypes();

                comboBoxDecoderType.Items.Clear();

                foreach (var validDecoderType in from decoderType in decoderTypes where (decoderType.TrackGuages & trackGuageSelector.Value) != 0 orderby decoderType.TypeName select decoderType)
                    comboBoxDecoderType.Items.Add(new DecoderTypeItem(validDecoderType));

                if (previousSelectionIndex < comboBoxDecoderType.Items.Count)
                    comboBoxDecoderType.SelectedIndex = previousSelectionIndex;
            }
        }

        protected bool GetLocomotiveTypeFields() {
            if (!ValidateSpeedLimit("textBoxSpeedLimit"))
                return false;

            GetRadio("Kind", new LocomotiveKind());
            GetRadio("Origin", new LocomotiveOrigin());
            GetLength("lengthInput", "Length");
            GetSpeedLimit("textBoxSpeedLimit", A_SpeedLimit);
            GetGuage();

            if (imageGetter.ImageModified) {
                var _ = new LocomotiveTypeInfo(element) {
                    Image = imageGetter.HasImage ? imageGetter.Image : null
                };
            }

            CheckBox checkBoxHasLights = (CheckBox)nameToControlMap["checkBoxHasLights"];
            ComboBox comboBoxStore = (ComboBox)nameToControlMap["comboBoxStore"];

            element[E_Functions]?.SetAttributeValue(A_Light, checkBoxHasLights.Checked);
            element.SetAttributeValue(A_Store, comboBoxStore.SelectedIndex);

            attributesEditor.Commit();

            return true;
        }

        #region Utility methods

        protected void SetImage() {
            LocomotiveTypeInfo locoType = new(element);

            if (locoType.Image != null)
                imageGetter.Image = locoType.Image;
        }

        protected void SetCheckbox(String controlName, XmlElement e, string a, bool defaultValue) {
            CheckBox checkbox = (CheckBox)nameToControlMap[controlName];

            if (e.HasAttribute(a))
                checkbox.Checked = bool.Parse(a);
            else
                checkbox.Checked = defaultValue;
        }

        protected void SetCheckbox(String controlName, string a) {
            SetCheckbox(controlName, element, a, false);
        }

        protected void SetRadio(XmlElement e, string a, string defaultValue) {
            string v = defaultValue;

            if (e.HasAttribute(a))
                v = e.GetAttribute(a);

            string controlName = "radioButton" + a + v;
            ((RadioButton)nameToControlMap[controlName]).Checked = true;
        }

        protected void SetRadio(String a, string defaultValue) {
            SetRadio(element, a, defaultValue);
        }

        protected void SetLength(String controlName, string a) {
            LengthInput c = (LengthInput)nameToControlMap[controlName];

            if (element.HasAttribute(a))
                c.NeutralValue = (double)element.AttributeValue(a);
            else
                c.IsEmpty = true;
        }

        protected void SetGuage() {
            TrackGuageSelector trackGuageSelector = (TrackGuageSelector)nameToControlMap["trackGuageSelector"];

            trackGuageSelector.Init();
            trackGuageSelector.Value = new XmlElementWrapper(element).AttributeValue("Guage").Enum<TrackGauges>() ?? TrackGauges.HO;
        }

        protected void SetSpeedLimit(String controlName) {
            TextBox textBoxSpeedLimit = (TextBox)nameToControlMap[controlName];
            int limit = (int?)element.AttributeValue(A_SpeedLimit) ?? 0;

            if (limit == 0)
                textBoxSpeedLimit.Text = "";
            else
                textBoxSpeedLimit.Text = limit.ToString();
        }

        protected void SetDecoderType(string controlName, string decoderTypeName) {
            ComboBox comboBoxDecoderType = (ComboBox)nameToControlMap[controlName];

            if (comboBoxDecoderType != null) {
                foreach (DecoderTypeItem item in comboBoxDecoderType.Items) {
                    if (item.DecoderType.TypeName == decoderTypeName) {
                        comboBoxDecoderType.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        protected string? GetRadioValue(Enum e, string a) {
            String[] names = Enum.GetNames(e.GetType());

            foreach (String n in names) {
                string controlName = "radioButton" + a + n;
                RadioButton rb = (RadioButton)nameToControlMap[controlName];

                if (rb != null && rb.Checked)
                    return n;
            }

            return null;
        }

        protected void GetRadio(XmlElement element, string a, Enum e) {
            element.SetAttribute(a, GetRadioValue(e, a));
        }

        protected void GetRadio(String a, Enum e) {
            GetRadio(element, a, e);
        }

        protected void GetLength(String controlName, string a) {
            LengthInput c = (LengthInput)nameToControlMap[controlName];

            if (!c.IsEmpty)
                element.SetAttributeValue(a, c.NeutralValue);
        }

        protected bool ValidateSpeedLimit(string controlName) {
            TextBox textBoxSpeedLimit = (TextBox)nameToControlMap[controlName];

            if (textBoxSpeedLimit.Text.Trim() != "") {
                try {
                    int.Parse(textBoxSpeedLimit.Text);
                }
                catch (FormatException) {
                    MessageBox.Show(this, "Invalid speed limit", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxSpeedLimit.Focus();
                    return false;
                }
            }
            return true;
        }

        protected void GetSpeedLimit(string controlName, string a) {
            TextBox textBoxSpeedLimit = (TextBox)nameToControlMap[controlName];
            int limit = 0;

            if (textBoxSpeedLimit.Text.Trim() != "")
                limit = int.Parse(textBoxSpeedLimit.Text);

            if (limit == 0)
                element.RemoveAttribute(a);
            else
                element.SetAttributeValue(a, limit);
        }

        protected void GetGuage() {
            TrackGuageSelector trackGuageSelector = (TrackGuageSelector)nameToControlMap["trackGuageSelector"];

            element.SetAttributeValue("Guage", trackGuageSelector.Value);
        }

        #endregion

        protected void SetStore(XmlElement storesElement) {
            ComboBox comboBoxStore = (ComboBox)nameToControlMap["comboBoxStore"];

            foreach (XmlElement storeElement in storesElement)
                comboBoxStore.Items.Add(storeElement.GetAttribute(A_Name));

            comboBoxStore.SelectedIndex = (int)element.AttributeValue(A_Store);
        }

        protected void SetFunctions() {
            var checkBoxHasLights = (CheckBox)nameToControlMap["checkBoxHasLights"];
            var functionsElement = element[E_Functions];

            if (functionsElement == null) {
                functionsElement = element.OwnerDocument.CreateElement(E_Functions);
                element.AppendChild(functionsElement);
            }

            checkBoxHasLights.Checked = (bool?)functionsElement.AttributeValue(A_Light) ?? true;
            listViewFunctions.Items.Clear();

            foreach (XmlElement functionElement in functionsElement)
                listViewFunctions.Items.Add(new FunctionItem(functionElement));
        }

        protected virtual void UpdateButtons() {
            Button buttonFunctionEdit = (Button)nameToControlMap["buttonFunctionEdit"];
            Button buttonFunctionRemove = (Button)nameToControlMap["buttonFunctionRemove"];

            if (listViewFunctions.SelectedItems.Count > 0) {
                buttonFunctionEdit.Enabled = true;
                buttonFunctionRemove.Enabled = true;
            }
            else {
                buttonFunctionEdit.Enabled = false;
                buttonFunctionRemove.Enabled = false;
            }
        }

        protected LocomotiveKind CurrentKind => (LocomotiveKind)Enum.Parse(typeof(LocomotiveKind), GetRadioValue(new LocomotiveKind(), "Kind") ?? "Steam");

        protected LocomotiveOrigin CurrentOrigin => (LocomotiveOrigin)Enum.Parse(typeof(LocomotiveOrigin), GetRadioValue(new LocomotiveOrigin(), "Origin") ?? "Europe");

#if DEBUG
        private void Dump() {
            foreach (String controlName in nameToControlMap.Keys)
                Debug.WriteLine(controlName);
        }
#endif

        #region Default event handler implementations

        protected void ButtonFunctionAdd_Click(object? sender, EventArgs e) {
            XmlElement functionElement = element.OwnerDocument.CreateElement("Function");
            FunctionItem item = new(functionElement);

            // Allocate default function number
            int functionNumber = 0;
            bool functionNumberUsed;
            var functionsElement = element[E_Functions];

            if (functionsElement != null) {
                do {
                    functionNumber++;
                    functionNumberUsed = false;

                    foreach (XmlElement f in functionsElement) {
                        if ((int)f.AttributeValue(A_Number) == functionNumber) {
                            functionNumberUsed = true;
                            break;
                        }
                    }
                } while (functionNumberUsed);

                functionElement.SetAttributeValue(A_Number, functionNumber);

                if (item.Edit(this, Catalog, functionsElement) == DialogResult.OK) {
                    functionsElement.AppendChild(item.FunctionElement);
                    listViewFunctions.Items.Add(item);
                }

                item.Selected = true;
                UpdateButtons();
            }
        }

        protected void ButtonFunctionEdit_Click(object? sender, EventArgs e) {
            if (listViewFunctions.SelectedItems.Count > 0) {
                FunctionItem selected = (FunctionItem)listViewFunctions.SelectedItems[0];
                var functionsElement = element[E_Functions];

                if (functionsElement != null)
                    selected.Edit(this, Catalog, functionsElement);
            }
        }

        protected void ButtonFunctionRemove_Click(object? sender, EventArgs e) {
            if (listViewFunctions.SelectedItems.Count > 0) {
                FunctionItem selected = (FunctionItem)listViewFunctions.SelectedItems[0];

                selected.FunctionElement.ParentNode?.RemoveChild(selected.FunctionElement);
                listViewFunctions.Items.Remove(selected);
                UpdateButtons();
            }
        }

        protected void TrackGuageSelector_SelectedIndexChanged(object? sender, EventArgs e) {
            InitDecoderTypeComboBox();
        }

        protected void ListViewFunctions_SelectedIndexChanged(object? sender, EventArgs e) {
            UpdateButtons();
        }

        protected void OnUpdateImage(object? sender, EventArgs e) {
            imageGetter.DefaultImage = Catalog.GetStandardImage(CurrentKind, CurrentOrigin);
        }

        protected void ButtonCopyFrom_Click(object? sender, EventArgs e) {
            LocomotiveCatalogInfo catalog = Catalog;

            catalog.Load();
            LocomotiveFunctionsCopyFrom copyFromDialog = new(catalog);

            if (copyFromDialog.ShowDialog(this) == DialogResult.OK) {
                var functionsElement = element[E_Functions];
                var copyFunctionsElement = copyFromDialog.SelectedLocomotiveType?.Element[E_Functions];

                functionsElement?.RemoveAll();
                listViewFunctions.Items.Clear();

                if (copyFunctionsElement != null && functionsElement != null) {
                    foreach (XmlElement f in copyFunctionsElement) {
                        XmlElement fCopy;

                        if (f.OwnerDocument == functionsElement.OwnerDocument)
                            fCopy = (XmlElement)f.CloneNode(true);
                        else
                            fCopy = (XmlElement)functionsElement.OwnerDocument.ImportNode(f, true);

                        functionsElement.AppendChild(fCopy);
                        listViewFunctions.Items.Add(new FunctionItem(fCopy));
                    }

                    if (copyFunctionsElement.HasAttribute(A_Light))
                        functionsElement.SetAttribute(A_Light, copyFunctionsElement.GetAttribute(A_Light));
                }
            }

            catalog.Unload();
        }

        #endregion

    }

    /// <summary>
    /// Represent an item in the function list
    /// </summary>
    internal class FunctionItem : ListViewItem {
        public FunctionItem(XmlElement functionElement) {
            this.FunctionElement = functionElement;

            LocomotiveFunctionInfo function = new(functionElement);

            this.Text = function.Number.ToString();
            this.SubItems.Add(function.Type == LocomotiveFunctionType.OnOff ? "On/Off" : function.Type.ToString());
            this.SubItems.Add(function.Name);
            this.SubItems.Add(function.Description);
        }

        public XmlElement FunctionElement { get; }

        public void Update() {
            LocomotiveFunctionInfo function = new(FunctionElement);

            this.Text = function.Number.ToString();
            this.SubItems[1].Text = function.Type == LocomotiveFunctionType.OnOff ? "On/Off" : function.Type.ToString();
            this.SubItems[2].Text = function.Name;
            this.SubItems[3].Text = function.Description;
        }

        public DialogResult Edit(Control parent, LocomotiveCatalogInfo catalog, XmlElement functionsElement) {
            LocomotiveFunction locoFunction = new(catalog, functionsElement, FunctionElement);

            DialogResult r = locoFunction.ShowDialog(parent);
            Update();

            return r;
        }
    }

    internal class DecoderTypeItem {
        public DecoderTypeItem(DecoderTypeInfo decoderType) {
            this.DecoderType = decoderType;
        }

        public DecoderTypeInfo DecoderType { get; }

        public override string ToString() => DecoderType.Manufacturer + " " + DecoderType.Name + (DecoderType.Description != null ? " (" + DecoderType.Description + ")" : "");
    }
}

