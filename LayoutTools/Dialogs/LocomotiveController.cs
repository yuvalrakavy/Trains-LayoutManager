using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveController.
    /// </summary>
    [LayoutEventDef("train-controller-activated", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
    [LayoutEventDef("train-controller-deactivated", Role = LayoutEventRole.Notification, SenderType = typeof(TrainStateInfo))]
    public class LocomotiveController : Form, IObjectHasXml {
        private Panel panelInfo;
        private ImageList imageListMotionButtons;
        private Button buttonBackward;
        private Button buttonStop;
        private Button buttonForward;
        private Button buttonFunction;
        private Button buttonLight;
        private Button buttonLocate;
        private System.Windows.Forms.ToolTip toolTips;
        private ContextMenu contextMenuLights;
        private MenuItem menuItemLightsOn;
        private MenuItem menuItemLightsOff;
        private Button buttonProperties;
        private Label labelDriverInstructions;
        private Panel panelSpeedLimit;
        private Label labelSpeedLimit;
        private Button buttonBackwardMenu;
        private Button buttonStopMenu;
        private Button buttonForwardMenu;
        private IContainer components;

        readonly TrainStateInfo train;

        #pragma warning disable nullable
        public LocomotiveController(TrainStateInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;
            EventManager.AddObjectSubscriptions(this);
            this.Owner = LayoutController.ActiveFrameWindow as Form;

            buttonLight.Enabled = train.HasLights;

            setTitleBar();

            if (train.CurrentSpeedLimit != 0)
                labelSpeedLimit.Text = train.CurrentSpeedLimit.ToString();

            if (train.Lights)
                buttonLight.FlatStyle = FlatStyle.Flat;
            else
                buttonLight.FlatStyle = FlatStyle.Standard;

            EventManager.Event(new LayoutEvent<TrainStateInfo>("train-controller-activated", train));
        }
        #pragma warning restore nullable

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            EventManager.Event(new LayoutEvent<TrainStateInfo>("train-controller-deactivated", train));
        }

        private void setTitleBar() {
            if (train.Locomotives.Count == 1) {
                LocomotiveInfo loco = train.Locomotives[0].Locomotive;

                if (loco.TypeName != "")
                    this.Text = train.DisplayName + " (" + loco.TypeName + ")";
                else
                    this.Text = train.DisplayName;
            }
            else
                this.Text = train.DisplayName;
        }

        public XmlElement Element => train.Element;
        public XmlElement? OptionalElement => Element;

        #region Layout Event Handlers

        [LayoutEvent("activate-locomotive-controller", IfSender = "*[@ID='`string(@ID)`']")]
        private void acivateLocomotiveController(LayoutEvent e) {
            this.Activate();
            e.Info = this;
        }

        [LayoutEvent("exit-operation-mode", Order = 10)]
        private void trainRemoveFromTrack(LayoutEvent e) {
            this.Close();
        }

        [LayoutEvent("train-power-changed", IfSender = "*[@ID='`string(@ID)`']", Order = 10)]
        private void onTrainPowerChanged(LayoutEvent e0) {
            var e = (LayoutEvent<TrainStateInfo, ILayoutPower>)e0;

            if (e.Info.Type != LayoutPowerType.Digital)
                this.Close();
        }

        [LayoutEvent("train-speed-changed", IfSender = "*[@ID='`string(@ID)`']")]
        private void trainSpeedChanged(LayoutEvent e) {
            panelInfo.Invalidate();
        }

        [LayoutEvent("train-speed-limit-changed", IfSender = "*[@ID='`string(@ID)`']")]
        private void trainSpeedLimitChanged(LayoutEvent e) {
            int speedLimit = (int)e.Info;

            if (speedLimit == 0)
                labelSpeedLimit.Text = "";
            else
                labelSpeedLimit.Text = speedLimit.ToString();
        }

        [LayoutEvent("train-enter-block", IfSender = "*[@ID='`string(@ID)`']")]
        [LayoutEvent("train-created", IfSender = "*[@ID='`string(@ID)`']")]
        [LayoutEvent("train-extended", IfSender = "*[@ID='`string(@ID)`']")]
        private void locomotiveEnterBlock(LayoutEvent e) {
            panelInfo.Invalidate();
        }

        [LayoutEvent("train-lights-changed", IfSender = "*[@ID='`string(@ID)`']")]
        private void locomotiveLightsChanged(LayoutEvent e) {
            bool lights = (bool)e.Info;

            if (lights)
                buttonLight.FlatStyle = FlatStyle.Flat;
            else
                buttonLight.FlatStyle = FlatStyle.Standard;
        }

        [LayoutEvent("train-configuration-changed", IfSender = "*[@ID='`string(@ID)`']")]
        private void trainNameChanged(LayoutEvent e) {
            setTitleBar();
        }

        [LayoutEvent("driver-train-go", IfSender = "*[@ID='`string(@ID)`']")]
        private void driverTrainGo(LayoutEvent e) {
            LocomotiveOrientation direction = (LocomotiveOrientation)e.Info;

            labelDriverInstructions.BackColor = Color.Green;
            labelDriverInstructions.ForeColor = Color.Yellow;
            labelDriverInstructions.Font = new Font(labelDriverInstructions.Font, FontStyle.Bold);
            labelDriverInstructions.Text = "Go " + ((direction == LocomotiveOrientation.Forward) ? "Forward" : "Backward") + "!";
        }

        [LayoutEvent("driver-prepare-stop", IfSender = "*[@ID='`string(@ID)`']")]
        private void driverTrainPrepareStop(LayoutEvent e) {
            labelDriverInstructions.BackColor = Color.Yellow;
            labelDriverInstructions.ForeColor = Color.Black;
            labelDriverInstructions.Font = new Font(labelDriverInstructions.Font, FontStyle.Regular);
            labelDriverInstructions.Text = "Get ready to STOP";
        }

        [LayoutEvent("driver-stop", IfSender = "*[@ID='`string(@ID)`']")]
        private void driverTrainStop(LayoutEvent e) {
            labelDriverInstructions.BackColor = Color.Red;
            labelDriverInstructions.ForeColor = Color.Yellow;
            labelDriverInstructions.Font = new Font(labelDriverInstructions.Font, FontStyle.Bold);
            labelDriverInstructions.Text = "STOP!";
        }

        private void addLocomotiveFunctions(SortedList<string, List<LocomotiveInfo>> functions, LocomotiveInfo loco) {
            if (loco.Functions != null) {
                foreach (XmlElement functionElement in loco.Functions) {
                    LocomotiveFunctionInfo function = new LocomotiveFunctionInfo(functionElement);

                    if (!functions.TryGetValue(function.Name, out List<LocomotiveInfo> locos)) {
                        locos = new List<LocomotiveInfo>();
                        functions.Add(function.Name, locos);
                    }

                    locos.Add(loco);
                }
            }
        }

        private void addLocomotiveFunctionPresets(SortedList<string, List<LocomotiveInfo>> functions, LocomotiveInfo loco) {
            if (loco.Functions != null) {
                foreach (XmlElement functionElement in loco.Functions) {
                    LocomotiveFunctionInfo function = new LocomotiveFunctionInfo(functionElement);

                    if (function.Type == LocomotiveFunctionType.OnOff) {

                        if (!functions.TryGetValue(function.Name, out List<LocomotiveInfo> locos)) {
                            locos = new List<LocomotiveInfo>();
                            functions.Add(function.Name, locos);
                        }

                        locos.Add(loco);
                    }
                }
            }
        }

        [LayoutEvent("add-locomotive-controller-function-menu-entries", IfSender = "*[@ID='`string(@ID)`']")]
        private void addLocomotiveControllerFunctionMenuEntries(LayoutEvent e) {
            var functions = new SortedList<string, List<LocomotiveInfo>>();
            Menu m = (Menu)e.Info;

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                addLocomotiveFunctions(functions, trainLoco.Locomotive);

            // Add all available functions to the menu. If the same function is available on more than one
            // locomotive, it will be presented as a submenu with each locomotive, and a All option.
            if (functions.Count > 0) {
                if (m.MenuItems.Count > 0)
                    m.MenuItems.Add("-");

                foreach (var d in functions) {
                    var locos = d.Value;
                    var functionName = d.Key;

                    Debug.Assert(locos.Count > 0, "locos array is empty");

                    if (locos.Count == 1) {
                        LocomotiveInfo loco = (LocomotiveInfo)locos[0];

                        m.MenuItems.Add(new LocomotiveFunctionMenuItem(train, loco, functionName, true, train.Locomotives.Count > 1));
                    }
                    else {
                        MenuItem functionItem = new MenuItem(GetFunctionDescription((LocomotiveInfo)locos[0], functionName));
                        LocomotiveFunctionInfo function = null;

                        foreach (LocomotiveInfo loco in locos) {
                            functionItem.MenuItems.Add(new LocomotiveFunctionMenuItem(train, loco, functionName, false, true));
                            if (function == null)
                                function = loco.GetFunctionByName(functionName);
                        }

                        if (function.Type != LocomotiveFunctionType.OnOff) {
                            functionItem.MenuItems.Add("-");
                            functionItem.MenuItems.Add(new LocomotiveFunctionMenuItem(train, function));
                        }

                        m.MenuItems.Add(functionItem);
                    }
                }

                functions.Clear();

                // Add presets
                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                    addLocomotiveFunctionPresets(functions, trainLoco.Locomotive);

                if (functions.Count > 0) {
                    m.MenuItems.Add("-");

                    MenuItem presetItem = new MenuItem("Set function state");

                    foreach (var d in functions) {
                        var locos = d.Value;
                        var functionName = d.Key;

                        if (locos.Count == 1)
                            presetItem.MenuItems.Add(new LocomotiveFunctionPresetMenuItem(train, locos[0], functionName, true, train.Locomotives.Count > 1));
                        else {
                            MenuItem functionItem = new MenuItem(GetFunctionDescription(locos[0], functionName));

                            foreach (var loco in locos)
                                functionItem.MenuItems.Add(new LocomotiveFunctionPresetMenuItem(train, loco, functionName, false, true));

                            presetItem.MenuItems.Add(functionItem);
                        }
                    }

                    m.MenuItems.Add(presetItem);
                }
            }

            if (EventManager.Event(new LayoutEvent("get-command-station-set-function-number-support", train).SetCommandStation(train)) is CommandStationSetFunctionNumberSupportInfo functionNumberSupport &&
                functionNumberSupport.SetFunctionNumberSupport != SetFunctionNumberSupport.None) {

                if (m.MenuItems.Count == 0)
                    addTrainFunctionNumberItems(m, functionNumberSupport);
                else {
                    m.MenuItems.Add("-");

                    var functionNumberMenu = new MenuItem("Other functions...");

                    addTrainFunctionNumberItems(functionNumberMenu, functionNumberSupport);
                    m.MenuItems.Add(functionNumberMenu);
                }
            }
        }

        private void addTrainFunctionNumberItems(Menu m, CommandStationSetFunctionNumberSupportInfo functionNumberSupportInfo) {
            if (train.Locomotives.Count == 1)
                addLocomotiveFunctionNumberItems(m, train.Locomotives[0].Locomotive, functionNumberSupportInfo);
            else
                foreach (var trainLoco in train.Locomotives) {
                    MenuItem locoFuncionNumbersMenu = new MenuItem(trainLoco.Name);

                    addLocomotiveFunctionNumberItems(locoFuncionNumbersMenu, trainLoco.Locomotive, functionNumberSupportInfo);
                    m.MenuItems.Add(locoFuncionNumbersMenu);
                }
        }

        private void addLocomotiveFunctionNumberItems(Menu m, LocomotiveInfo loco, CommandStationSetFunctionNumberSupportInfo functionNumberSupportInfo) {
            for (var functionNumber = functionNumberSupportInfo.MinFunctionNumber; functionNumber <= functionNumberSupportInfo.MaxFunctionNumber; functionNumber++)
                m.MenuItems.Add(new LocomotiveFunctionNumberMenuItem(train, loco, functionNumber, functionNumberSupportInfo.SetFunctionNumberSupport == SetFunctionNumberSupport.FunctionNumberAndBooleanState));
        }

        protected static String GetFunctionDescription(LocomotiveInfo loco, String functionName) {
            LocomotiveFunctionInfo function = loco.GetFunctionByName(functionName);

            if (function.Description != null && function.Description != "")
                return function.Description;
            else
                return function.Name;
        }

        #region Menu Items

        class LocomotiveFunctionMenuItem : MenuItem {
            readonly LocomotiveFunctionInfo function;
            readonly TrainStateInfo trainState;
            readonly LocomotiveInfo loco;
            Guid id;

            public LocomotiveFunctionMenuItem(TrainStateInfo trainState, LocomotiveInfo loco, String functionName, bool showFunctionName, bool addLocoName) {
                this.trainState = trainState;
                this.loco = loco;
                this.id = loco.Id;
                this.function = loco.GetFunctionByName(functionName);

                if (showFunctionName)
                    this.Text = LocomotiveController.GetFunctionDescription(loco, functionName) + (addLocoName ? (" (" + loco.DisplayName + ")") : "");
                else
                    this.Text = loco.DisplayName;


                if (function.Type == LocomotiveFunctionType.OnOff) {
                    bool state = trainState.GetFunctionState(functionName, loco.Id, false);

                    if (state)
                        this.Checked = true;
                }
            }

            public LocomotiveFunctionMenuItem(TrainStateInfo trainState, LocomotiveFunctionInfo function) {
                this.trainState = trainState;
                this.loco = null;
                this.id = Guid.Empty;
                this.function = function;

                this.Text = "All Locomotives";
            }

            protected override void OnClick(EventArgs e) {
                if (function.Type == LocomotiveFunctionType.OnOff) {
                    bool state = trainState.GetFunctionState(function.Name, loco.Id, false);

                    state = !state;
                    trainState.SetLocomotiveFunctionState(function.Name, id, state);
                }
                else
                    trainState.TriggerLocomotiveFunction(function.Name, id);
            }
        }

        class LocomotiveFunctionNumberMenuItem : MenuItem {
            public LocomotiveFunctionNumberMenuItem(TrainStateInfo train, LocomotiveInfo loco, int functionNumber, bool canSetBooleanState) {
                var function = loco.GetFunctionByNumber(functionNumber);

                this.Text = $"Function {functionNumber}{(function != null ? $" ({function.ToString()})" : "")}";

                if (canSetBooleanState) {
                    this.MenuItems.Add(new MenuItem("On", (sender, e) => EventManager.Event(new LayoutEvent("trigger-locomotive-function-number", loco, functionNumber).SetCommandStation(train).SetOption("FunctionState", true))));
                    this.MenuItems.Add(new MenuItem("Off", (sender, e) => EventManager.Event(new LayoutEvent("trigger-locomotive-function-number", loco, functionNumber).SetCommandStation(train).SetOption("FunctionState", false))));
                }
                else
                    this.Click += (sender, e) => EventManager.Event(new LayoutEvent("trigger-locomotive-function-number", loco, functionNumber).SetCommandStation(train));
            }
        }

        class LocomotiveFunctionPresetMenuItem : MenuItem {
            readonly LocomotiveFunctionInfo function;
            readonly TrainStateInfo trainState;
            readonly LocomotiveInfo loco;

            public LocomotiveFunctionPresetMenuItem(TrainStateInfo trainState, LocomotiveInfo loco, String functionName, bool showFunctionName, bool addLocoName) {
                this.trainState = trainState;
                this.loco = loco;
                this.function = loco.GetFunctionByName(functionName);

                if (showFunctionName)
                    this.Text = LocomotiveController.GetFunctionDescription(loco, functionName) + (addLocoName ? (" (" + loco.DisplayName + ")") : "");
                else
                    this.Text = loco.DisplayName;

                MenuItems.Add("On", new EventHandler(this.MenuItemOn_Click));
                MenuItems.Add("Off", new EventHandler(this.MenuItemOff_Click));
            }

            protected void MenuItemOn_Click(object sender, EventArgs e) {
                trainState.SetLocomotiveFunctionStateValue(function.Name, loco.Id, true);
            }

            protected void MenuItemOff_Click(object sender, EventArgs e) {
                trainState.SetLocomotiveFunctionStateValue(function.Name, loco.Id, false);
            }
        }

        #endregion

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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocomotiveController));
            this.panelInfo = new System.Windows.Forms.Panel();
            this.imageListMotionButtons = new System.Windows.Forms.ImageList(this.components);
            this.buttonBackward = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonForward = new System.Windows.Forms.Button();
            this.buttonFunction = new System.Windows.Forms.Button();
            this.buttonLight = new System.Windows.Forms.Button();
            this.contextMenuLights = new System.Windows.Forms.ContextMenu();
            this.menuItemLightsOn = new System.Windows.Forms.MenuItem();
            this.menuItemLightsOff = new System.Windows.Forms.MenuItem();
            this.buttonLocate = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.buttonProperties = new System.Windows.Forms.Button();
            this.labelDriverInstructions = new System.Windows.Forms.Label();
            this.panelSpeedLimit = new System.Windows.Forms.Panel();
            this.labelSpeedLimit = new System.Windows.Forms.Label();
            this.buttonBackwardMenu = new System.Windows.Forms.Button();
            this.buttonStopMenu = new System.Windows.Forms.Button();
            this.buttonForwardMenu = new System.Windows.Forms.Button();
            this.panelSpeedLimit.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelInfo
            // 
            this.panelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInfo.Location = new System.Drawing.Point(3, 1);
            this.panelInfo.Name = "panelInfo";
            this.panelInfo.Size = new System.Drawing.Size(188, 56);
            this.panelInfo.TabIndex = 0;
            this.panelInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.panelInfo_Paint);
            // 
            // imageListMotionButtons
            // 
            this.imageListMotionButtons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMotionButtons.ImageStream")));
            this.imageListMotionButtons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMotionButtons.Images.SetKeyName(0, "");
            this.imageListMotionButtons.Images.SetKeyName(1, "");
            this.imageListMotionButtons.Images.SetKeyName(2, "");
            this.imageListMotionButtons.Images.SetKeyName(3, "");
            this.imageListMotionButtons.Images.SetKeyName(4, "");
            this.imageListMotionButtons.Images.SetKeyName(5, "");
            // 
            // buttonBackward
            // 
            this.buttonBackward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBackward.ImageIndex = 0;
            this.buttonBackward.ImageList = this.imageListMotionButtons;
            this.buttonBackward.Location = new System.Drawing.Point(5, 64);
            this.buttonBackward.Name = "buttonBackward";
            this.buttonBackward.Size = new System.Drawing.Size(32, 23);
            this.buttonBackward.TabIndex = 1;
            this.toolTips.SetToolTip(this.buttonBackward, "Backward");
            this.buttonBackward.Click += new System.EventHandler(this.buttonBackward_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStop.ImageIndex = 1;
            this.buttonStop.ImageList = this.imageListMotionButtons;
            this.buttonStop.Location = new System.Drawing.Point(40, 64);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(32, 23);
            this.buttonStop.TabIndex = 2;
            this.toolTips.SetToolTip(this.buttonStop, "Stop");
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonForward
            // 
            this.buttonForward.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonForward.ImageIndex = 2;
            this.buttonForward.ImageList = this.imageListMotionButtons;
            this.buttonForward.Location = new System.Drawing.Point(76, 64);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(32, 23);
            this.buttonForward.TabIndex = 3;
            this.toolTips.SetToolTip(this.buttonForward, "Forward");
            this.buttonForward.Click += new System.EventHandler(this.buttonForward_Click);
            // 
            // buttonFunction
            // 
            this.buttonFunction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFunction.Location = new System.Drawing.Point(165, 64);
            this.buttonFunction.Name = "buttonFunction";
            this.buttonFunction.Size = new System.Drawing.Size(56, 23);
            this.buttonFunction.TabIndex = 5;
            this.buttonFunction.Text = "Function";
            this.buttonFunction.Click += new System.EventHandler(this.buttonFunction_Click);
            // 
            // buttonLight
            // 
            this.buttonLight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLight.ContextMenu = this.contextMenuLights;
            this.buttonLight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLight.ImageIndex = 3;
            this.buttonLight.ImageList = this.imageListMotionButtons;
            this.buttonLight.Location = new System.Drawing.Point(133, 64);
            this.buttonLight.Name = "buttonLight";
            this.buttonLight.Size = new System.Drawing.Size(26, 23);
            this.buttonLight.TabIndex = 4;
            this.toolTips.SetToolTip(this.buttonLight, "Turn lights on/off");
            this.buttonLight.Click += new System.EventHandler(this.buttonLight_Click);
            // 
            // contextMenuLights
            // 
            this.contextMenuLights.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemLightsOn,
            this.menuItemLightsOff});
            // 
            // menuItemLightsOn
            // 
            this.menuItemLightsOn.Index = 0;
            this.menuItemLightsOn.Text = "Lights are ON";
            this.menuItemLightsOn.Click += new System.EventHandler(this.menuLightsOn_Click);
            // 
            // menuItemLightsOff
            // 
            this.menuItemLightsOff.Index = 1;
            this.menuItemLightsOff.Text = "Lights are OFF";
            this.menuItemLightsOff.Click += new System.EventHandler(this.menuItemLightsOff_Click);
            // 
            // buttonLocate
            // 
            this.buttonLocate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLocate.ImageIndex = 4;
            this.buttonLocate.ImageList = this.imageListMotionButtons;
            this.buttonLocate.Location = new System.Drawing.Point(193, 27);
            this.buttonLocate.Name = "buttonLocate";
            this.buttonLocate.Size = new System.Drawing.Size(26, 23);
            this.buttonLocate.TabIndex = 6;
            this.toolTips.SetToolTip(this.buttonLocate, "Locate locomotive");
            this.buttonLocate.Click += new System.EventHandler(this.buttonLocate_Click);
            // 
            // buttonProperties
            // 
            this.buttonProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonProperties.ImageIndex = 5;
            this.buttonProperties.ImageList = this.imageListMotionButtons;
            this.buttonProperties.Location = new System.Drawing.Point(193, 1);
            this.buttonProperties.Name = "buttonProperties";
            this.buttonProperties.Size = new System.Drawing.Size(26, 23);
            this.buttonProperties.TabIndex = 7;
            this.toolTips.SetToolTip(this.buttonProperties, "Edit train configuration");
            this.buttonProperties.Click += new System.EventHandler(this.buttonProperties_Click);
            // 
            // labelDriverInstructions
            // 
            this.labelDriverInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDriverInstructions.BackColor = System.Drawing.SystemColors.Control;
            this.labelDriverInstructions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelDriverInstructions.Location = new System.Drawing.Point(5, 104);
            this.labelDriverInstructions.Name = "labelDriverInstructions";
            this.labelDriverInstructions.Size = new System.Drawing.Size(173, 18);
            this.labelDriverInstructions.TabIndex = 8;
            this.labelDriverInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelSpeedLimit
            // 
            this.panelSpeedLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSpeedLimit.Controls.Add(this.labelSpeedLimit);
            this.panelSpeedLimit.Location = new System.Drawing.Point(185, 94);
            this.panelSpeedLimit.Name = "panelSpeedLimit";
            this.panelSpeedLimit.Size = new System.Drawing.Size(34, 34);
            this.panelSpeedLimit.TabIndex = 9;
            this.panelSpeedLimit.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSpeedLimit_Paint);
            // 
            // labelSpeedLimit
            // 
            this.labelSpeedLimit.BackColor = System.Drawing.Color.Transparent;
            this.labelSpeedLimit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.labelSpeedLimit.Location = new System.Drawing.Point(2, 7);
            this.labelSpeedLimit.Name = "labelSpeedLimit";
            this.labelSpeedLimit.Size = new System.Drawing.Size(30, 20);
            this.labelSpeedLimit.TabIndex = 0;
            this.labelSpeedLimit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonBackwardMenu
            // 
            this.buttonBackwardMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBackwardMenu.Location = new System.Drawing.Point(5, 87);
            this.buttonBackwardMenu.Name = "buttonBackwardMenu";
            this.buttonBackwardMenu.Size = new System.Drawing.Size(32, 10);
            this.buttonBackwardMenu.TabIndex = 10;
            this.buttonBackwardMenu.Click += new System.EventHandler(this.buttonBackwardMenu_Click);
            // 
            // buttonStopMenu
            // 
            this.buttonStopMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonStopMenu.Location = new System.Drawing.Point(40, 87);
            this.buttonStopMenu.Name = "buttonStopMenu";
            this.buttonStopMenu.Size = new System.Drawing.Size(32, 10);
            this.buttonStopMenu.TabIndex = 10;
            this.buttonStopMenu.Click += new System.EventHandler(this.buttonStopMenu_Click);
            // 
            // buttonForwardMenu
            // 
            this.buttonForwardMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonForwardMenu.Location = new System.Drawing.Point(76, 87);
            this.buttonForwardMenu.Name = "buttonForwardMenu";
            this.buttonForwardMenu.Size = new System.Drawing.Size(32, 10);
            this.buttonForwardMenu.TabIndex = 10;
            this.buttonForwardMenu.Click += new System.EventHandler(this.buttonForwardMenu_Click);
            // 
            // LocomotiveController
            // 
            this.ClientSize = new System.Drawing.Size(224, 130);
            this.Controls.Add(this.buttonBackwardMenu);
            this.Controls.Add(this.panelSpeedLimit);
            this.Controls.Add(this.labelDriverInstructions);
            this.Controls.Add(this.buttonFunction);
            this.Controls.Add(this.buttonBackward);
            this.Controls.Add(this.panelInfo);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonForward);
            this.Controls.Add(this.buttonLight);
            this.Controls.Add(this.buttonLocate);
            this.Controls.Add(this.buttonProperties);
            this.Controls.Add(this.buttonStopMenu);
            this.Controls.Add(this.buttonForwardMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimumSize = new System.Drawing.Size(220, 104);
            this.Name = "LocomotiveController";
            this.Text = "LocomotiveController";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.LocomotiveController_Closing);
            this.Closed += new System.EventHandler(this.LocomotiveController_Closed);
            this.panelSpeedLimit.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void panelInfo_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
            int xText;
            float yText;

            if (train.Locomotives.Count == 1) {
                LocomotiveInfo loco = train.Locomotives[0].Locomotive;

                using (LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(LayoutModel.LocomotiveCatalog)) {
                    locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
                };

                yText = 2;
                xText = 55;
            }
            else {
                using (LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(LayoutModel.LocomotiveCatalog)) {
                    int x = 2;

                    locoPainter.FrameSize = new Size(28, 20);

                    foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                        locoPainter.LocomotiveElement = trainLoco.Locomotive.Element;
                        locoPainter.FlipImage = (trainLoco.Orientation == LocomotiveOrientation.Backward);
                        locoPainter.Origin = new Point(x, 2);
                        locoPainter.Draw(e.Graphics);

                        x += locoPainter.FrameSize.Width + 2;
                    }

                    xText = 2;
                    yText = 24;
                }
            }

            using (Brush textBrush = new SolidBrush(SystemColors.WindowText)) {
                SizeF textSize;
                String status = train.StatusText;

                using (Font statusFont = new Font("Arial", 8, FontStyle.Regular)) {
                    textSize = e.Graphics.MeasureString(status, statusFont);
                    e.Graphics.DrawString(status, statusFont, textBrush, new PointF(xText, yText));
                }

                yText += textSize.Height;
            }
        }

        private void LocomotiveController_Closed(object sender, System.EventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void buttonLocate_Click(object sender, System.EventArgs e) {
            ModelComponent locoLocation = null;

            if (train.LocomotiveBlock.BlockDefinintion != null)
                locoLocation = train.LocomotiveBlock.BlockDefinintion;
            else {
                if (train.LocomotiveBlock.TrackEdges.Count > 0)
                    locoLocation = train.LocomotiveBlock.TrackEdges[0].Track;
            }

            EventManager.Event(new LayoutEvent("ensure-component-visible", locoLocation, true).SetFrameWindow(LayoutController.ActiveFrameWindow));
        }

        private void buttonStop_Click(object sender, System.EventArgs e) {
            train.SpeedInSteps = 0;
        }

        private void buttonBackward_Click(object sender, System.EventArgs e) {
            if (train.Speed > -LayoutModel.Instance.LogicalSpeedSteps)
                train.Speed--;
        }

        private void buttonForward_Click(object sender, System.EventArgs e) {
            if (train.Speed < LayoutModel.Instance.LogicalSpeedSteps)
                train.Speed++;
        }

        private void buttonLight_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("set-train-lights-request", train, !train.Lights));
        }

        private void menuLightsOn_Click(object sender, System.EventArgs e) {
            train.SetLightsValue(true);
        }

        private void menuItemLightsOff_Click(object sender, System.EventArgs e) {
            train.SetLightsValue(false);
        }

        private void buttonFunction_Click(object sender, System.EventArgs e) {
            ContextMenu functionMenu = new ContextMenu();

            EventManager.Event(new LayoutEvent("add-locomotive-controller-function-menu-entries", train, functionMenu));

            if (functionMenu.MenuItems.Count == 0) {
                MenuItem noFunctions = new MenuItem("No functions") {
                    Enabled = false
                };
                functionMenu.MenuItems.Add(noFunctions);
            }

            functionMenu.Show(this, new Point(buttonFunction.Left, buttonFunction.Bottom));
        }

        private void buttonProperties_Click(object sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("edit-train-properties", train));
        }

        private void panelSpeedLimit_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
            const int boarderSize = 3;

            e.Graphics.FillEllipse(Brushes.Red, 0, 0, panelSpeedLimit.Width, panelSpeedLimit.Height);
            e.Graphics.FillEllipse(Brushes.White, boarderSize, boarderSize, panelSpeedLimit.Width - 2 * boarderSize, panelSpeedLimit.Height - 2 * boarderSize);
        }

        #region Speed change menu

        private void addAccelerationMenuEntries(Menu menu, int speed) {
            foreach (MotionRampInfo ramp in LayoutModel.Instance.Ramps)
                menu.MenuItems.Add(new SpeedChangeMenuItem(train, speed, ramp));
        }

        private MotionRampInfo GetDefaultRamp(int speed) {
            if (speed == 0)
                return LayoutModel.StateManager.DefaultStopRamp;
            else if (speed > train.Speed)
                return LayoutModel.StateManager.DefaultAccelerationRamp;
            else
                return LayoutModel.StateManager.DefaultDecelerationRamp;
        }

        private void addSpeedMenuEntries(Menu menu, LocomotiveOrientation direction) {
            for (int i = 1; i <= LayoutModel.Instance.LogicalSpeedSteps; i++) {
                int speed = (direction == LocomotiveOrientation.Forward) ? i : -i;
                MenuItem speedItem = new MenuItem(i.ToString());

                addAccelerationMenuEntries(speedItem, speed);

                if (speedItem.MenuItems.Count == 0) {
                    speedItem.Click += (s, ea) => {
                        train.ChangeSpeed(speed, GetDefaultRamp(speed));
                    };
                }

                menu.MenuItems.Add(speedItem);
            }
        }

        private void buttonStopMenu_Click(object sender, System.EventArgs e) {
            ContextMenu menu = new ContextMenu();

            addAccelerationMenuEntries(menu, 0);

            if (menu.MenuItems.Count > 0)
                menu.Show(this, new Point(buttonStopMenu.Left, buttonStopMenu.Bottom));
            else
                train.ChangeSpeed(0, GetDefaultRamp(0));
        }

        private void buttonBackwardMenu_Click(object sender, System.EventArgs e) {
            ContextMenu menu = new ContextMenu();

            addSpeedMenuEntries(menu, LocomotiveOrientation.Backward);
            menu.Show(this, new Point(buttonBackwardMenu.Left, buttonBackwardMenu.Bottom));
        }

        private void buttonForwardMenu_Click(object sender, System.EventArgs e) {
            ContextMenu menu = new ContextMenu();

            addSpeedMenuEntries(menu, LocomotiveOrientation.Forward);
            menu.Show(this, new Point(buttonForwardMenu.Left, buttonForwardMenu.Bottom));
        }

        private void LocomotiveController_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }

        class SpeedChangeMenuItem : MenuItem {
            readonly MotionRampInfo ramp;
            readonly int speed;
            readonly TrainStateInfo train;

            public SpeedChangeMenuItem(TrainStateInfo train, int speed, MotionRampInfo ramp) {
                this.train = train;
                this.ramp = ramp;
                this.speed = speed;

                Text = ramp.Description;
            }

            protected override void OnClick(EventArgs e) {
                train.ChangeSpeed(speed, ramp);
            }
        }

        #endregion
    }
}
