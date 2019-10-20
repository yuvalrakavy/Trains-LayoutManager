using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Diagnostics;

#nullable enable
#pragma warning disable IDE0051, IDE0069, IDE0060
namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LayoutControlViewer.
    /// </summary>
    public class LayoutControlViewer : System.Windows.Forms.UserControl {
        private ImageList imageListCloseButton;
        private Panel panel1;
        private Button buttonClose;
        private LayoutManager.CommonUI.LayoutControlBusViewer layoutControlBusViewer;
        private Label label1;
        private ComboBox comboBoxBusProvider;
        private Label label2;
        private ComboBox comboBoxBus;
        private Label label3;
        private ComboBox comboBoxLocation;
        private Button buttonZoomIn;
        private Button buttonZoomOut;
        private IContainer components;

        private void endOfDesignerVariables() { }

        private readonly float[] zooms = new float[] { 0.2F, 0.5F, 0.75F, 1.0F, 1.5F, 2.0F, 4.0F };
        private int zoomIndex = 3;
        private readonly string notInLocationText = "(Not in any location)";

        #nullable disable
        public LayoutControlViewer() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }
        #nullable enable

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
            layoutControlBusViewer.Initialize();
            EventManager.AddObjectSubscriptions(this);

            updateSelectors();
        }

        private void updateSelectors() {
            Guid idSelected = Guid.Empty;

            if (comboBoxBusProvider.Items.Count > 0)
                idSelected = ((Item)comboBoxBusProvider.SelectedItem).Id;

            comboBoxBusProvider.Items.Clear();
            comboBoxBusProvider.Items.Add(new Item("(All)", Guid.Empty));

            var busProviders = LayoutModel.Components<IModelComponentIsBusProvider>(LayoutModel.ActivePhases);

            foreach (var busProvider in busProviders)
                comboBoxBusProvider.Items.Add(new Item(busProvider.NameProvider.Name, busProvider.Id));
            comboBoxBusProvider.SelectedIndex = 0;

            updateBusSelector();

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
        private void doUpdateSelectors(LayoutEvent e) {
            updateSelectors();
            layoutControlBusViewer.Recalc();
        }

        [LayoutEvent("show-control-modules-location")]
        private void showControlModulesLocation(LayoutEvent e) {
            Guid controlModuleLocationId;

            if (e.Sender is LayoutControlModuleLocationComponent)
                controlModuleLocationId = ((LayoutControlModuleLocationComponent)e.Sender).Id;
            else if (e.Sender is Guid)
                controlModuleLocationId = (Guid)e.Sender;
            else
                throw new ArgumentException("Invalid sender for show-control-modules-location");

            foreach (Item item in comboBoxLocation.Items)
                if (item.Id == controlModuleLocationId) {
                    comboBoxLocation.SelectedItem = item;
                    break;
                }

            EventManager.Event(new LayoutEvent("show-layout-control", this));
        }

        private void updateBusSelector() {
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutControlViewer));
            this.imageListCloseButton = new ImageList(this.components);
            this.panel1 = new Panel();
            this.buttonClose = new Button();
            this.layoutControlBusViewer = new LayoutManager.CommonUI.LayoutControlBusViewer();
            this.label1 = new Label();
            this.comboBoxBusProvider = new ComboBox();
            this.label2 = new Label();
            this.comboBoxBus = new ComboBox();
            this.label3 = new Label();
            this.comboBoxLocation = new ComboBox();
            this.buttonZoomIn = new Button();
            this.buttonZoomOut = new Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListCloseButton
            // 
            this.imageListCloseButton.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListCloseButton.ImageStream");
            this.imageListCloseButton.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListCloseButton.Images.SetKeyName(0, "");
            this.imageListCloseButton.Images.SetKeyName(1, "");
            this.imageListCloseButton.Images.SetKeyName(2, "");
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.buttonClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 20);
            this.panel1.TabIndex = 9;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.ImageIndex = 0;
            this.buttonClose.ImageList = this.imageListCloseButton;
            this.buttonClose.Location = new System.Drawing.Point(182, 1);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(16, 16);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Click += this.buttonClose_Click;
            // 
            // layoutControlBusViewer
            // 
            this.layoutControlBusViewer.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right);
            this.layoutControlBusViewer.BusProviderId = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.layoutControlBusViewer.BusTypeName = null;
            this.layoutControlBusViewer.Location = new System.Drawing.Point(0, 168);
            this.layoutControlBusViewer.ModuleLocationID = new System.Guid("00000000-0000-0000-0000-000000000000");
            this.layoutControlBusViewer.Name = "layoutControlBusViewer";
            this.layoutControlBusViewer.ShowOnlyNotInLocation = false;
            this.layoutControlBusViewer.Size = new System.Drawing.Size(200, 523);
            this.layoutControlBusViewer.StartingPoint = (System.Drawing.PointF)resources.GetObject("layoutControlBusViewer.StartingPoint");
            this.layoutControlBusViewer.TabIndex = 8;
            this.layoutControlBusViewer.Text = "layoutControlBusViewer1";
            this.layoutControlBusViewer.Zoom = 1F;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Controller:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxBusProvider
            // 
            this.comboBoxBusProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBusProvider.Location = new System.Drawing.Point(8, 36);
            this.comboBoxBusProvider.Name = "comboBoxBusProvider";
            this.comboBoxBusProvider.Size = new System.Drawing.Size(144, 21);
            this.comboBoxBusProvider.TabIndex = 1;
            this.comboBoxBusProvider.SelectedIndexChanged += this.comboBoxBusProvider_SelectedIndexChanged;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Connection:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxBus
            // 
            this.comboBoxBus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBus.Location = new System.Drawing.Point(8, 74);
            this.comboBoxBus.Name = "comboBoxBus";
            this.comboBoxBus.Size = new System.Drawing.Size(144, 21);
            this.comboBoxBus.TabIndex = 3;
            this.comboBoxBus.SelectedIndexChanged += this.comboBoxBus_SelectedIndexChanged;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Control Modules Location:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxLocation
            // 
            this.comboBoxLocation.Location = new System.Drawing.Point(8, 112);
            this.comboBoxLocation.Name = "comboBoxLocation";
            this.comboBoxLocation.Size = new System.Drawing.Size(144, 21);
            this.comboBoxLocation.TabIndex = 5;
            this.comboBoxLocation.SelectedIndexChanged += this.comboBoxLocation_SelectedIndexChanged;
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonZoomIn.ImageIndex = 2;
            this.buttonZoomIn.ImageList = this.imageListCloseButton;
            this.buttonZoomIn.Location = new System.Drawing.Point(27, 141);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(16, 16);
            this.buttonZoomIn.TabIndex = 7;
            this.buttonZoomIn.UseVisualStyleBackColor = false;
            this.buttonZoomIn.Click += this.buttonZoomIn_Click;
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.BackColor = System.Drawing.SystemColors.ControlDark;
            this.buttonZoomOut.ImageIndex = 1;
            this.buttonZoomOut.ImageList = this.imageListCloseButton;
            this.buttonZoomOut.Location = new System.Drawing.Point(8, 141);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(16, 16);
            this.buttonZoomOut.TabIndex = 6;
            this.buttonZoomOut.UseVisualStyleBackColor = false;
            this.buttonZoomOut.Click += this.buttonZoomOut_Click;
            // 
            // LayoutControlViewer
            // 
            this.Controls.Add(this.buttonZoomOut);
            this.Controls.Add(this.buttonZoomIn);
            this.Controls.Add(this.comboBoxLocation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxBus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxBusProvider);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.layoutControlBusViewer);
            this.Controls.Add(this.panel1);
            this.Name = "LayoutControlViewer";
            this.Size = new System.Drawing.Size(200, 688);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonClose_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("hide-layout-control", this));
        }

        private void comboBoxBusProvider_SelectedIndexChanged(object sender, System.EventArgs e) {
            Item selected = (Item)comboBoxBusProvider.SelectedItem;

            layoutControlBusViewer.BusProviderId = selected.Id;
            updateBusSelector();
        }

        private void comboBoxBus_SelectedIndexChanged(object sender, System.EventArgs e) {
            Item selected = (Item)comboBoxBus.SelectedItem;

            layoutControlBusViewer.BusTypeName = selected.Value;
        }

        private void comboBoxLocation_SelectedIndexChanged(object sender, System.EventArgs e) {
            Item selected = (Item)comboBoxLocation.SelectedItem;

            if (selected.Id == Guid.Empty && selected.Text == notInLocationText)
                layoutControlBusViewer.ShowOnlyNotInLocation = true;
            else
                layoutControlBusViewer.ShowOnlyNotInLocation = false;

            layoutControlBusViewer.ModuleLocationID = selected.Id;
        }

        private void buttonZoomOut_Click(object sender, System.EventArgs e) {
            if (zoomIndex < zooms.Length - 1)
                layoutControlBusViewer.Zoom = zooms[++zoomIndex];
        }

        private void buttonZoomIn_Click(object sender, System.EventArgs e) {
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
