using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.Linq;

using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

#pragma warning disable IDE0051
namespace LayoutManager.Dialogs {
    /// <summary>
    /// Base form for the locomotive & locomotive type properties
    /// </summary>
    public class LocomotiveBasePropertiesForm : Form {
        readonly Dictionary<string, Control> nameToControlMap = new Dictionary<string, Control>();
        protected XmlElement element;
        CommonUI.Controls.ImageGetter imageGetter;

        ListView listViewFunctions;
        AttributesEditor attributesEditor;

        public LocomotiveBasePropertiesForm() {
        }

        private void addToNameToControlMap(Dictionary<string, Control> h, Control c) {
            if (c.Name != null && c.Name != "")
                h.Add(c.Name, c);

            foreach (Control cc in c.Controls)
                addToNameToControlMap(h, cc);
        }

        protected void BuildControlNameMap() {
            addToNameToControlMap(nameToControlMap, this);
        }

        protected LocomotiveCatalogInfo Catalog => LayoutModel.LocomotiveCatalog;

        protected void InitializeControls(XmlElement element, XmlElement storesElement) {
            LocomotiveTypeInfo locoType = new LocomotiveTypeInfo(element);

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

                List<DecoderTypeInfo> decoderTypes = new List<DecoderTypeInfo>();

                EventManager.Event(new LayoutEvent("enum-decoder-types", decoderTypes));

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
            GetSpeedLimit("textBoxSpeedLimit", "SpeedLimit");
            GetGuage();

            if (imageGetter.ImageModified) {
                var _ = new LocomotiveTypeInfo(element) {
                    Image = imageGetter.HasImage ? imageGetter.Image : null
                };
            }

            CheckBox checkBoxHasLights = (CheckBox)nameToControlMap["checkBoxHasLights"];
            ComboBox comboBoxStore = (ComboBox)nameToControlMap["comboBoxStore"];

            element["Functions"].SetAttribute("Light", XmlConvert.ToString(checkBoxHasLights.Checked));

            element.SetAttribute("Store", comboBoxStore.SelectedIndex);

            attributesEditor.Commit();

            return true;
        }

        #region Utility methods

        protected void SetImage() {
            LocomotiveTypeInfo locoType = new LocomotiveTypeInfo(element);

            if (locoType.Image != null)
                imageGetter.Image = locoType.Image;
        }

        protected void SetCheckbox(String controlName, XmlElement e, string a, bool defaultValue) {
            CheckBox checkbox = (CheckBox)nameToControlMap[controlName];

            if (e.HasAttribute(a))
                checkbox.Checked = XmlConvert.ToBoolean(a);
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
                c.NeutralValue = XmlConvert.ToDouble(element.GetAttribute(a));
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
            int limit = 0;

            if (element.HasAttribute("SpeedLimit"))
                limit = XmlConvert.ToInt32(element.GetAttribute("SpeedLimit"));

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

        protected string GetRadioValue(Enum e, string a) {
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
                element.SetAttribute(a, XmlConvert.ToString(c.NeutralValue));
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
                element.SetAttribute(a, XmlConvert.ToString(limit));
        }

        protected void GetGuage() {
            TrackGuageSelector trackGuageSelector = (TrackGuageSelector)nameToControlMap["trackGuageSelector"];

            element.SetAttribute("Guage", trackGuageSelector.Value);
        }

        #endregion

        protected void SetStore(XmlElement storesElement) {
            ComboBox comboBoxStore = (ComboBox)nameToControlMap["comboBoxStore"];

            foreach (XmlElement storeElement in storesElement)
                comboBoxStore.Items.Add(storeElement.GetAttribute("Name"));

            comboBoxStore.SelectedIndex = XmlConvert.ToInt32(element.GetAttribute("Store"));
        }

        protected void SetFunctions() {
            CheckBox checkBoxHasLights = (CheckBox)nameToControlMap["checkBoxHasLights"];

            XmlElement functionsElement = element["Functions"];

            if (functionsElement == null) {
                functionsElement = element.OwnerDocument.CreateElement("Functions");
                element.AppendChild(functionsElement);
            }

            if (functionsElement.HasAttribute("Light"))
                checkBoxHasLights.Checked = XmlConvert.ToBoolean(functionsElement.GetAttribute("Light"));
            else
                checkBoxHasLights.Checked = true;

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

        protected LocomotiveKind CurrentKind => (LocomotiveKind)Enum.Parse(typeof(LocomotiveKind), GetRadioValue(new LocomotiveKind(), "Kind"));

        protected LocomotiveOrigin CurrentOrigin => (LocomotiveOrigin)Enum.Parse(typeof(LocomotiveOrigin), GetRadioValue(new LocomotiveOrigin(), "Origin"));

#if DEBUG
        void Dump() {
            foreach (String controlName in nameToControlMap.Keys)
                Debug.WriteLine(controlName);
        }
#endif

        #region Default event handler implementations

        protected void ButtonFunctionAdd_Click(object sender, System.EventArgs e) {
            XmlElement functionElement = element.OwnerDocument.CreateElement("Function");
            FunctionItem item = new FunctionItem(functionElement);

            // Allocate default function number
            int functionNumber = 0;
            bool functionNumberUsed;
            XmlElement functionsElement = element["Functions"];

            do {
                functionNumber++;
                functionNumberUsed = false;

                foreach (XmlElement f in functionsElement) {
                    if (XmlConvert.ToInt32(f.GetAttribute("Number")) == functionNumber) {
                        functionNumberUsed = true;
                        break;
                    }
                }
            } while (functionNumberUsed);

            functionElement.SetAttribute("Number", XmlConvert.ToString(functionNumber));

            if (item.Edit(this, Catalog, functionsElement) == DialogResult.OK) {
                functionsElement.AppendChild(item.FunctionElement);
                listViewFunctions.Items.Add(item);
            }

            item.Selected = true;
            UpdateButtons();
        }

        protected void ButtonFunctionEdit_Click(object sender, System.EventArgs e) {
            if (listViewFunctions.SelectedItems.Count > 0) {
                FunctionItem selected = (FunctionItem)listViewFunctions.SelectedItems[0];
                XmlElement functionsElement = element["Functions"];

                selected.Edit(this, Catalog, functionsElement);
            }
        }

        protected void ButtonFunctionRemove_Click(object sender, System.EventArgs e) {
            if (listViewFunctions.SelectedItems.Count > 0) {
                FunctionItem selected = (FunctionItem)listViewFunctions.SelectedItems[0];

                selected.FunctionElement.ParentNode.RemoveChild(selected.FunctionElement);
                listViewFunctions.Items.Remove(selected);
                UpdateButtons();
            }
        }

        protected void TrackGuageSelector_SelectedIndexChanged(object sender, System.EventArgs e) {
            InitDecoderTypeComboBox();
        }


        protected void ListViewFunctions_SelectedIndexChanged(object sender, System.EventArgs e) {
            UpdateButtons();
        }

        protected void OnUpdateImage(object sender, System.EventArgs e) {
            imageGetter.DefaultImage = Catalog.GetStandardImage(CurrentKind, CurrentOrigin);
        }

        protected void ButtonCopyFrom_Click(object sender, System.EventArgs e) {
            LocomotiveCatalogInfo catalog = Catalog;

            catalog.Load();
            Dialogs.LocomotiveFunctionsCopyFrom copyFromDialog = new Dialogs.LocomotiveFunctionsCopyFrom(catalog);

            if (copyFromDialog.ShowDialog(this) == DialogResult.OK) {
                XmlElement functionsElement = element["Functions"];
                XmlElement copyFunctionsElement = copyFromDialog.SelectedLocomotiveType.Element["Functions"];

                functionsElement.RemoveAll();
                listViewFunctions.Items.Clear();

                if (copyFunctionsElement != null) {
                    foreach (XmlElement f in copyFunctionsElement) {
                        XmlElement fCopy;

                        if (f.OwnerDocument == functionsElement.OwnerDocument)
                            fCopy = (XmlElement)f.CloneNode(true);
                        else
                            fCopy = (XmlElement)functionsElement.OwnerDocument.ImportNode(f, true);

                        functionsElement.AppendChild(fCopy);
                        listViewFunctions.Items.Add(new FunctionItem(fCopy));
                    }

                    if (copyFunctionsElement.HasAttribute("Light"))
                        functionsElement.SetAttribute("Light", copyFunctionsElement.GetAttribute("Light"));
                }
            }

            catalog.Unload();
        }

        #endregion

    }

    /// <summary>
    /// Represent an item in the function list
    /// </summary>
    class FunctionItem : ListViewItem {
        readonly XmlElement functionElement;

        public FunctionItem(XmlElement functionElement) {
            this.functionElement = functionElement;

            LocomotiveFunctionInfo function = new LocomotiveFunctionInfo(functionElement);

            this.Text = function.Number.ToString();
            this.SubItems.Add(function.Type == LocomotiveFunctionType.OnOff ? "On/Off" : function.Type.ToString());
            this.SubItems.Add(function.Name);
            this.SubItems.Add(function.Description);
        }

        public XmlElement FunctionElement => functionElement;

        public void Update() {
            LocomotiveFunctionInfo function = new LocomotiveFunctionInfo(functionElement);

            this.Text = function.Number.ToString();
            this.SubItems[1].Text = function.Type == LocomotiveFunctionType.OnOff ? "On/Off" : function.Type.ToString();
            this.SubItems[2].Text = function.Name;
            this.SubItems[3].Text = function.Description;
        }

        public DialogResult Edit(Control parent, LocomotiveCatalogInfo catalog, XmlElement functionsElement) {
            Dialogs.LocomotiveFunction locoFunction = new Dialogs.LocomotiveFunction(catalog, functionsElement, functionElement);

            DialogResult r = locoFunction.ShowDialog(parent);
            Update();

            return r;
        }
    }

    class DecoderTypeItem {
        readonly DecoderTypeInfo decoderType;

        public DecoderTypeItem(DecoderTypeInfo decoderType) {
            this.decoderType = decoderType;
        }

        public DecoderTypeInfo DecoderType => this.decoderType;

        public override string ToString() => DecoderType.Manufacturer + " " + DecoderType.Name + (DecoderType.Description != null ? " (" + DecoderType.Description + ")" : "");
    }
}

