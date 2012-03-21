using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.Linq;

using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.Dialogs {
	/// <summary>
	/// Base form for the locomotive & locomotive type properties
	/// </summary>
	public class LocomotiveBasePropertiesForm : System.Windows.Forms.Form {
		Dictionary<string, Control> nameToControlMap = new Dictionary<string, Control>();
		protected XmlElement		element;
		protected bool				hasImage = false;
		protected bool				imageModified = false;

		ListView					listViewFunctions;
		PictureBox					pictureBoxImage;
		AttributesEditor			attributesEditor;

		public LocomotiveBasePropertiesForm() {
		}

		private void addToNameToControlMap(Dictionary<string, Control> h, Control c) {
			if(c.Name != null && c.Name != "")
				h.Add(c.Name, c);

			foreach(Control cc in c.Controls)
				addToNameToControlMap(h, cc);
		}

		protected void BuildControlNameMap() {
			addToNameToControlMap(nameToControlMap, this);
		}

		protected LocomotiveCatalogInfo Catalog {
			get {
				return LayoutModel.LocomotiveCatalog;
			}
		}

		protected void InitializeControls(XmlElement element, XmlElement storesElement) {
			this.element = element;

			BuildControlNameMap();

			listViewFunctions = (ListView)nameToControlMap["listViewFunctions"];
			pictureBoxImage = (PictureBox)nameToControlMap["pictureBoxImage"];

			if(storesElement != null)
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
			SetSpeedLimit("textBoxSpeedLimit", "SpeedLimit");

			SetImage();
			SetFunctions();
			SetGuage();

			InitDecoderTypeComboBox();

			if(element.HasAttribute("DecoderType"))
				SetDecoderType("comboBoxDecoderType", element.GetAttribute("DecoderType"));
			

		}

		private void InitDecoderTypeComboBox() {
			ComboBox comboBoxDecoderType = (ComboBox)nameToControlMap["comboBoxDecoderType"];

			if(comboBoxDecoderType != null) {
				TrackGuageSelector trackGuageSelector = (TrackGuageSelector)nameToControlMap["trackGuageSelector"];

				List<DecoderTypeInfo> decoderTypes = new List<DecoderTypeInfo>();

				EventManager.Event(new LayoutEvent(decoderTypes, "enum-decoder-types"));

				comboBoxDecoderType.Items.Clear();

				foreach(var validDecoderType in from decoderType in decoderTypes where (decoderType.TrackGuages & trackGuageSelector.Value) != 0 orderby decoderType.TypeName select decoderType)
					comboBoxDecoderType.Items.Add(new DecoderTypeItem(validDecoderType));
			}
		}

		protected bool GetLocomotiveTypeFields() {
			if(!ValidateSpeedLimit("textBoxSpeedLimit"))
				return false;

			GetRadio("Kind", new LocomotiveKind());
			GetRadio("Origin", new LocomotiveOrigin());
			GetLength("lengthInput", "Length");
			GetSpeedLimit("textBoxSpeedLimit", "SpeedLimit");
			GetGuage();

			if(imageModified) {
				LocomotiveTypeInfo	locoType = new LocomotiveTypeInfo(element);
				locoType.Image = hasImage ? pictureBoxImage.Image : null;
			}

			CheckBox	checkBoxHasLights = (CheckBox)nameToControlMap["checkBoxHasLights"];
			ComboBox	comboBoxStore = (ComboBox)nameToControlMap["comboBoxStore"];
			LengthInput	length = (LengthInput)nameToControlMap["lengthInput"];

			element["Functions"].SetAttribute("Light", XmlConvert.ToString(checkBoxHasLights.Checked));

			element.SetAttribute("Store", XmlConvert.ToString(comboBoxStore.SelectedIndex));

			attributesEditor.Commit();

			return true;
		}

		#region Utility methods

		protected void SetImage() {
			LocomotiveTypeInfo	locoType = new LocomotiveTypeInfo(element);
			PictureBox				pictureBoxImage = (PictureBox)nameToControlMap["pictureBoxImage"];

			if(locoType.Image != null) {
				pictureBoxImage.Image = locoType.Image;
				hasImage = true;
			}
			else {
				pictureBoxImage.Image = Catalog.GetStandardImage(locoType.Kind, locoType.Origin);
				hasImage = false;
			}
		}

		protected void SetCheckbox(String controlName, XmlElement e, String a, bool defaultValue) {
			CheckBox	checkbox = (CheckBox)nameToControlMap[controlName];

			if(e.HasAttribute(a))
				checkbox.Checked = XmlConvert.ToBoolean(a);
			else
				checkbox.Checked = defaultValue;
		}

		protected void SetCheckbox(String controlName, String a) {
			SetCheckbox(controlName, element, a, false);
		}


		protected void SetRadio(XmlElement e, String a, String defaultValue) {
			String	v = defaultValue;

			if(e.HasAttribute(a))
				v = e.GetAttribute(a);

			String	controlName = "radioButton" + a + v;
			((RadioButton)nameToControlMap[controlName]).Checked = true;
		}

		protected void SetRadio(String a, String defaultValue) {
			SetRadio(element, a, defaultValue);
		}

		protected void SetLength(String controlName, String a) {
			LengthInput	c = (LengthInput)nameToControlMap[controlName];

			if(element.HasAttribute(a))
				c.NeutralValue = XmlConvert.ToDouble(element.GetAttribute(a));
			else
				c.IsEmpty = true;
		}

		protected void SetGuage() {
			TrackGuageSelector trackGuageSelector = (TrackGuageSelector)nameToControlMap["trackGuageSelector"];

			trackGuageSelector.Init();

			if(element.HasAttribute("Guage"))
				trackGuageSelector.Value = (TrackGauges)Enum.Parse(typeof(TrackGauges), element.GetAttribute("Guage"));
			else
				trackGuageSelector.Value = TrackGauges.HO;
		}

		protected void SetSpeedLimit(String controlName, string a) {
			TextBox	textBoxSpeedLimit = (TextBox)nameToControlMap[controlName];
			int		limit = 0;

			if(element.HasAttribute("SpeedLimit"))
				limit = XmlConvert.ToInt32(element.GetAttribute("SpeedLimit"));

			if(limit == 0)
				textBoxSpeedLimit.Text = "";
			else
				textBoxSpeedLimit.Text = limit.ToString();
		}

		protected void SetDecoderType(string controlName, string decoderTypeName) {
			ComboBox comboBoxDecoderType = (ComboBox)nameToControlMap[controlName];

			if(comboBoxDecoderType != null) {
				foreach(DecoderTypeItem item in comboBoxDecoderType.Items) {
					if(item.DecoderType.TypeName == decoderTypeName) {
						comboBoxDecoderType.SelectedItem = item;
						break;
					}
				}
			}
		}

		protected String GetRadioValue(Enum e, String a) {
			String[]	names = Enum.GetNames(e.GetType());

			foreach(String n in names) {
				String	controlName = "radioButton" + a + n;
				RadioButton		rb = (RadioButton)nameToControlMap[controlName];

				if(rb != null && rb.Checked)
					return n;
			}

			return null;
		}

		protected void GetRadio(XmlElement element, String a, Enum e) {
			element.SetAttribute(a, GetRadioValue(e, a));
		}

		protected void GetRadio(String a, Enum e) {
			GetRadio(element, a, e);
		}

		protected void GetLength(String controlName, String a) {
			LengthInput	c = (LengthInput)nameToControlMap[controlName];

			if(!c.IsEmpty)
				element.SetAttribute(a, XmlConvert.ToString(c.NeutralValue));
		}

		protected bool ValidateSpeedLimit(string controlName) {
			TextBox	textBoxSpeedLimit = (TextBox)nameToControlMap[controlName];

			if(textBoxSpeedLimit.Text.Trim() != "") {
				try {
					int.Parse(textBoxSpeedLimit.Text);
				} catch(FormatException) {
					MessageBox.Show(this, "Invalid speed limit", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
					textBoxSpeedLimit.Focus();
					return false;
				}

			}
			return true;
		}

		protected void GetSpeedLimit(string controlName, string a) {
			TextBox	textBoxSpeedLimit = (TextBox)nameToControlMap[controlName];
			int		limit = 0;

			if(textBoxSpeedLimit.Text.Trim() != "")
				limit = int.Parse(textBoxSpeedLimit.Text);


			if(limit == 0)
				element.RemoveAttribute(a);
			else
				element.SetAttribute(a, XmlConvert.ToString(limit));
		}

		protected void GetGuage() {
			TrackGuageSelector trackGuageSelector = (TrackGuageSelector)nameToControlMap["trackGuageSelector"];

			element.SetAttribute("Guage", trackGuageSelector.Value.ToString());
		}

		#endregion

		protected void SetStore(XmlElement storesElement) {
			ComboBox	comboBoxStore = (ComboBox)nameToControlMap["comboBoxStore"];

			foreach(XmlElement storeElement in storesElement)
				comboBoxStore.Items.Add(storeElement.GetAttribute("Name"));

			comboBoxStore.SelectedIndex = XmlConvert.ToInt32(element.GetAttribute("Store"));
		}

		protected void SetFunctions() {
			CheckBox	checkBoxHasLights = (CheckBox)nameToControlMap["checkBoxHasLights"];

			XmlElement	functionsElement = element["Functions"];

			if(functionsElement == null) {
				functionsElement = element.OwnerDocument.CreateElement("Functions");
				element.AppendChild(functionsElement);
			}

			if(functionsElement.HasAttribute("Light"))
				checkBoxHasLights.Checked = XmlConvert.ToBoolean(functionsElement.GetAttribute("Light"));
			else
				checkBoxHasLights.Checked = true;

			listViewFunctions.Items.Clear();

			foreach(XmlElement functionElement in functionsElement)
				listViewFunctions.Items.Add(new FunctionItem(functionElement));
		}

		protected virtual void UpdateButtons() {
			Button	buttonFunctionEdit = (Button)nameToControlMap["buttonFunctionEdit"];
			Button	buttonFunctionRemove = (Button)nameToControlMap["buttonFunctionRemove"];

			if(listViewFunctions.SelectedItems.Count > 0) {
				buttonFunctionEdit.Enabled = true;
				buttonFunctionRemove.Enabled = true;
			}
			else {
				buttonFunctionEdit.Enabled = false;
				buttonFunctionRemove.Enabled = false;
			}

			
		}

		protected LocomotiveKind CurrentKind {
			get {
				return (LocomotiveKind)Enum.Parse(typeof(LocomotiveKind), GetRadioValue(new LocomotiveKind(), "Kind"));
			}
		}

		protected LocomotiveOrigin CurrentOrigin {
			get {
				return (LocomotiveOrigin)Enum.Parse(typeof(LocomotiveOrigin), GetRadioValue(new LocomotiveOrigin(), "Origin"));
			}
		}

#if DEBUG
		void Dump() {
			foreach(String controlName in nameToControlMap.Keys)
				Debug.WriteLine(controlName);
		}
#endif

		#region Default event handler implementations

		protected void buttonImageSet_Click(object sender, System.EventArgs e) {
			FileDialog		fileDialog = new OpenFileDialog();

			fileDialog.AddExtension = true;
			fileDialog.CheckFileExists = true;
			fileDialog.Filter = "Image files (*.jpg,*.bmp)|*.jpg;*.bmp|All files|*.*";

			if(fileDialog.ShowDialog(this) == DialogResult.OK) {
				try {
					Image	image = Image.FromFile(fileDialog.FileName);

					pictureBoxImage.Image = image;
					hasImage = true;
					imageModified = true;
				} catch(Exception ex) {
					MessageBox.Show(this, "Error loading image: " + ex.Message, "Image load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		protected void buttonImageClear_Click(object sender, System.EventArgs e) {
			pictureBoxImage.Image = Catalog.GetStandardImage(CurrentKind, CurrentOrigin);
			hasImage = false;
			imageModified = true;
		}

		protected void buttonFunctionAdd_Click(object sender, System.EventArgs e) {
			XmlElement		functionElement = element.OwnerDocument.CreateElement("Function");
			FunctionItem	item = new FunctionItem(functionElement);

			// Allocate default function number
			int			functionNumber = 0;
			bool		functionNumberUsed;
			XmlElement	functionsElement = element["Functions"];

			do {
				functionNumber++;
				functionNumberUsed = false;

				foreach(XmlElement f in functionsElement) {
					if(XmlConvert.ToInt32(f.GetAttribute("Number")) == functionNumber) {
						functionNumberUsed = true;
						break;
					}
				}
			} while(functionNumberUsed);

			functionElement.SetAttribute("Number", XmlConvert.ToString(functionNumber));

			if(item.Edit(this, Catalog, functionsElement) == DialogResult.OK) {
				functionsElement.AppendChild(item.FunctionElement);
				listViewFunctions.Items.Add(item);
			}

			item.Selected = true;
			UpdateButtons();
		}

		protected void buttonFunctionEdit_Click(object sender, System.EventArgs e) {
			if(listViewFunctions.SelectedItems.Count > 0) {
				FunctionItem	selected = (FunctionItem)listViewFunctions.SelectedItems[0];
				XmlElement		functionsElement = element["Functions"];

				selected.Edit(this, Catalog, functionsElement);
			}
		}

		protected void buttonFunctionRemove_Click(object sender, System.EventArgs e) {
			if(listViewFunctions.SelectedItems.Count > 0) {
				FunctionItem	selected = (FunctionItem)listViewFunctions.SelectedItems[0];

				selected.FunctionElement.ParentNode.RemoveChild(selected.FunctionElement);
				listViewFunctions.Items.Remove(selected);
				UpdateButtons();
			}
		}

		protected void listViewFunctions_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateButtons();
		}

		protected void OnUpdateImage(object sender, System.EventArgs e) {
			if(!hasImage)
				pictureBoxImage.Image = Catalog.GetStandardImage(CurrentKind, CurrentOrigin);
		}

		protected void buttonCopyFrom_Click(object sender, System.EventArgs e) {
			LocomotiveCatalogInfo	catalog = Catalog;

			catalog.Load();
			Dialogs.LocomotiveFunctionsCopyFrom	copyFromDialog = new Dialogs.LocomotiveFunctionsCopyFrom(catalog);

			if(copyFromDialog.ShowDialog(this) == DialogResult.OK) {
				XmlElement	functionsElement = element["Functions"];
				XmlElement	copyFunctionsElement = copyFromDialog.SelectedLocomotiveType.Element["Functions"];

				functionsElement.RemoveAll();
				listViewFunctions.Items.Clear();

				if(copyFunctionsElement != null) {
					foreach(XmlElement f in copyFunctionsElement) {
						XmlElement	fCopy;
						
						if(f.OwnerDocument == functionsElement.OwnerDocument)
							fCopy = (XmlElement)f.CloneNode(true);
						else
							fCopy = (XmlElement)functionsElement.OwnerDocument.ImportNode(f, true);

						functionsElement.AppendChild(fCopy);
						listViewFunctions.Items.Add(new FunctionItem(fCopy));
					}

					if(copyFunctionsElement.HasAttribute("Light"))
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
		XmlElement	functionElement;

		public FunctionItem(XmlElement functionElement) {
			this.functionElement = functionElement;

			LocomotiveFunctionInfo	function = new LocomotiveFunctionInfo(functionElement);

			this.Text = function.Number.ToString();
			this.SubItems.Add(function.Type == LocomotiveFunctionType.OnOff ? "On/Off" : function.Type.ToString());
			this.SubItems.Add(function.Name);
			this.SubItems.Add(function.Description);
		}

		public XmlElement FunctionElement {
			get {
				return functionElement;
			}
		}

		public void Update() {
			LocomotiveFunctionInfo	function = new LocomotiveFunctionInfo(functionElement);

			this.Text = function.Number.ToString();
			this.SubItems[1].Text = function.Type == LocomotiveFunctionType.OnOff ? "On/Off" : function.Type.ToString();
			this.SubItems[2].Text = function.Name;
			this.SubItems[3].Text = function.Description;
		}

		public DialogResult Edit(Control parent, LocomotiveCatalogInfo catalog, XmlElement functionsElement) {
			Dialogs.LocomotiveFunction	locoFunction = new Dialogs.LocomotiveFunction(catalog, functionsElement, functionElement);

			DialogResult	r = locoFunction.ShowDialog(parent);
			Update();

			return r;
		}
	}

	class DecoderTypeItem {
		DecoderTypeInfo decoderType;

		public DecoderTypeItem(DecoderTypeInfo decoderType) {
			this.decoderType = decoderType;
		}

		public DecoderTypeInfo DecoderType {
			get { return this.decoderType; }
		}

		public override string ToString() {
			return DecoderType.Manufacturer + " " + DecoderType.Name + (DecoderType.Description != null ? " (" + DecoderType.Description + ")" : "");
		}
	}
}

