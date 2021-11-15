namespace LayoutManager.CommonUI.Controls {
    public interface ICheckIfNameUsed {
        bool IsUsed(string name);
    }

    /// <summary>
    /// Summary description for AttributesEditor.
    /// </summary>
    public partial class AttributesEditor : UserControl, ICheckIfNameUsed, IControlSupportViewOnly {
        private IObjectHasAttributes? attributesOwner = null;
        private readonly ListViewStringColumnsSorter sorter;
        private bool viewOnly = false;

        public AttributesEditor() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            sorter = new ListViewStringColumnsSorter(listViewAttributes);
        }

        public Type? AttributesSource { get; set; } = null;

        public IObjectHasAttributes? AttributesOwner {
            get {
                return Ensure.NotNull<IObjectHasAttributes>(attributesOwner);
            }

            set {
                if (value != null) {
                    attributesOwner = value;
                    Initialize();
                }
                else
                    listViewAttributes.Items.Clear();
            }
        }

        public AttributesInfo? Attributes => AttributesOwner?.Attributes;

        public bool ViewOnly {
            get {
                return viewOnly;
            }

            set {
                if (value && viewOnly)
                    throw new ArgumentException("Cannot change from view only to not view only");

                viewOnly = value;

                if (viewOnly) {
                    buttonAdd.Visible = false;
                    buttonEdit.Visible = false;
                    buttonRemove.Visible = false;

                    SuspendLayout();

                    listViewAttributes.Size = new Size(listViewAttributes.Width, buttonAdd.Bottom - listViewAttributes.Top);

                    ResumeLayout();
                }
            }
        }

        #region Operations

        public bool Commit() {
            if (AttributesOwner != null && (listViewAttributes.Items.Count > 0 || AttributesOwner.HasAttributes)) {
                var attOwner = AttributesOwner;

                attOwner.Attributes.Clear();

                foreach (AttributeItem item in listViewAttributes.Items)
                    attOwner.Attributes[item.AttributeName] = item.Value;
            }

            return true;
        }

        #endregion

        #region methods

        private void Initialize() {
            listViewAttributes.Items.Clear();

            if (AttributesOwner != null && AttributesOwner.HasAttributes && Attributes != null) {
                foreach (AttributeInfo attribute in Attributes)
                    listViewAttributes.Items.Add(new AttributeItem(attribute.Name, attribute.Value));
            }

            UpdateButtons(null, EventArgs.Empty);
        }

        private void UpdateButtons(object? sender, EventArgs e) {
            if (listViewAttributes.SelectedItems.Count > 0) {
                buttonEdit.Enabled = true;
                buttonRemove.Enabled = true;
            }
            else {
                buttonEdit.Enabled = false;
                buttonRemove.Enabled = false;
            }
        }

        public bool IsUsed(string name) {
            foreach (AttributeItem item in listViewAttributes.Items)
                if (item.AttributeName == name)
                    return true;
            return false;
        }

        private AttributeItem? GetSelected() {
            return listViewAttributes.SelectedItems.Count == 0 ? null : (AttributeItem)listViewAttributes.SelectedItems[0];
        }

        #endregion

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


        private void ButtonAdd_Click(object? sender, EventArgs e) {
            Dialogs.AttributeDefinition d = new(AttributesSource, this, null, null);

            if (d.ShowDialog(this) == DialogResult.OK) {
                listViewAttributes.Items.Add(new AttributeItem(d.AttributeName, d.Value));
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        private void ButtonEdit_Click(object? sender, EventArgs e) {
            var selected = GetSelected();

            if (selected != null && !viewOnly) {
                Dialogs.AttributeDefinition d = new(AttributesSource, this, selected.AttributeName, selected.Value);

                if (d.ShowDialog(this) == DialogResult.OK) {
                    selected.AttributeName = d.AttributeName;
                    selected.Value = d.Value;
                }
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        private void ButtonRemove_Click(object? sender, EventArgs e) {
            var selected = GetSelected();

            if (selected != null) {
                listViewAttributes.Items.Remove(selected);
                UpdateButtons(null, EventArgs.Empty);
            }
        }

        #region Item class

        private class AttributeItem : ListViewItem {
            private object attributeValue;

            public AttributeItem(string name, object attributeValue) {
                Text = name;

                SubItems.Add("");
                SubItems.Add("");

                this.attributeValue = attributeValue;

                Update();
            }

            public string AttributeName {
                get {
                    return Text;
                }

                set {
                    Text = value;
                }
            }

            public object Value {
                get {
                    return attributeValue;
                }

                set {
                    attributeValue = value;
                    Update();
                }
            }

            public void Update() {
                string typeName;
                string valueString;

                if (attributeValue is string aString) {
                    typeName = "Text";
                    valueString = aString;
                }
                else if (attributeValue is bool aBool) {
                    typeName = "Boolean";
                    valueString = aBool.ToString();
                }
                else if (attributeValue is int anInt) {
                    typeName = "Number";
                    valueString = anInt.ToString();
                }
                else
                    throw new ApplicationException("Attribute value has non-supported type");

                SubItems[1].Text = typeName;
                SubItems[2].Text = valueString;
            }
        }

        #endregion
    }
}
