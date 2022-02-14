using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;
using LayoutManager.CommonUI;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveController.
    /// </summary>
    public partial class LocomotiveController : Form, IObjectHasId, IObjectHasXml {

        private readonly TrainStateInfo train;

        public XmlElement Element => train.Element;
        public XmlElement? OptionalElement => Element;

        public Guid Id => train.Id;

        public LocomotiveController(TrainStateInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
            this.Owner = LayoutController.ActiveFrameWindow as Form;

            buttonLight.Enabled = train.HasLights;

            SetTitleBar();

            if (train.CurrentSpeedLimit != 0)
                labelSpeedLimit.Text = train.CurrentSpeedLimit.ToString();

            if (train.Lights)
                buttonLight.FlatStyle = FlatStyle.Flat;
            else
                buttonLight.FlatStyle = FlatStyle.Standard;

            Dispatch.Notification.OnTrainControllerActivated(train);
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            Dispatch.Notification.OnTrainControllerDeactivated(train);
        }

        private void SetTitleBar() {
            if (train.Locomotives.Count == 1) {
                LocomotiveInfo loco = train.Locomotives[0].Locomotive;

                if (loco.TypeName != "")
                    this.Text = $"{train.DisplayName} ({loco.TypeName})";
                else
                    this.Text = train.DisplayName;
            }
            else
                this.Text = train.DisplayName;
        }

        #region Layout Event Handlers

        [LayoutEvent("activate-locomotive-controller", IfSender = "*[@ID='`string(@ID)`']")]
        private void AcivateLocomotiveController(LayoutEvent e) {
            this.Activate();
            e.Info = this;
        }


        [DispatchTarget(Order = 10)]
        private void OnExitOperationMode() {
            this.Close();
        }

        [LayoutEvent("train-power-changed", IfSender = "*[@ID='`string(@ID)`']", Order = 10)]
        private void OnTrainPowerChanged(LayoutEvent e) {
            var _ = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");
            var power = Ensure.NotNull<ILayoutPower>(e.Info, "power");

            if (power.Type != LayoutPowerType.Digital)
                this.Close();
        }

        [DispatchTarget]
        private void OnTrainSpeedChanged([DispatchFilter(Type="IsMyId")] TrainStateInfo train, int speed) {
            panelInfo.Invalidate();
        }

        [LayoutEvent("train-speed-limit-changed", IfSender = "*[@ID='`string(@ID)`']")]
        private void TrainSpeedLimitChanged(LayoutEvent e) {
            int speedLimit = Ensure.ValueNotNull<int>(e.Info);

            if (speedLimit == 0)
                labelSpeedLimit.Text = "";
            else
                labelSpeedLimit.Text = speedLimit.ToString();
        }

        [DispatchTarget]
        private void OnTrainEnteredBlock([DispatchFilter(Type="IsMyId")] TrainStateInfo train, LayoutBlock block) {
            panelInfo.Invalidate();
        }

        [DispatchTarget]
        private void OnTrainCreated([DispatchFilter(Type="IsMyId")] TrainStateInfo train, LayoutBlockDefinitionComponent blockDefinition) {
            panelInfo.Invalidate();
        }

        [DispatchTarget]
        private void OnTrainExtended([DispatchFilter(Type = "IsMyId")] TrainStateInfo train, LayoutBlock block) {
            panelInfo.Invalidate();
        }

        [LayoutEvent("train-lights-changed", IfSender = "*[@ID='`string(@ID)`']")]
        private void LocomotiveLightsChanged(LayoutEvent e) {
            bool lights = Ensure.ValueNotNull<bool>(e.Info);

            if (lights)
                buttonLight.FlatStyle = FlatStyle.Flat;
            else
                buttonLight.FlatStyle = FlatStyle.Standard;
        }

        [LayoutEvent("train-configuration-changed", IfSender = "*[@ID='`string(@ID)`']")]
        private void TrainNameChanged(LayoutEvent e) {
            SetTitleBar();
        }

        [LayoutEvent("driver-train-go", IfSender = "*[@ID='`string(@ID)`']")]
        private void DriverTrainGo(LayoutEvent e) {
            var direction = Ensure.ValueNotNull<LocomotiveOrientation>(e.Info);

            labelDriverInstructions.BackColor = Color.Green;
            labelDriverInstructions.ForeColor = Color.Yellow;
            labelDriverInstructions.Font = new Font(labelDriverInstructions.Font, FontStyle.Bold);
            labelDriverInstructions.Text = "Go " + ((direction == LocomotiveOrientation.Forward) ? "Forward" : "Backward") + "!";
        }

        [LayoutEvent("driver-prepare-stop", IfSender = "*[@ID='`string(@ID)`']")]
        private void DriverTrainPrepareStop(LayoutEvent e) {
            labelDriverInstructions.BackColor = Color.Yellow;
            labelDriverInstructions.ForeColor = Color.Black;
            labelDriverInstructions.Font = new Font(labelDriverInstructions.Font, FontStyle.Regular);
            labelDriverInstructions.Text = "Get ready to STOP";
        }

        [LayoutEvent("driver-stop", IfSender = "*[@ID='`string(@ID)`']")]
        private void DriverTrainStop(LayoutEvent e) {
            labelDriverInstructions.BackColor = Color.Red;
            labelDriverInstructions.ForeColor = Color.Yellow;
            labelDriverInstructions.Font = new Font(labelDriverInstructions.Font, FontStyle.Bold);
            labelDriverInstructions.Text = "STOP!";
        }

        private static void AddLocomotiveFunctions(SortedList<string, List<LocomotiveInfo>> functions, LocomotiveInfo loco) {
            if (loco.Functions != null) {
                foreach (XmlElement functionElement in loco.Functions) {
                    var function = new LocomotiveFunctionInfo(functionElement);

                    if (!functions.TryGetValue(function.Name, out List<LocomotiveInfo>? locos)) {
                        locos = new List<LocomotiveInfo>();
                        functions.Add(function.Name, locos);
                    }

                    locos.Add(loco);
                }
            }
        }

        private void AddLocomotiveFunctionPresets(SortedList<string, List<LocomotiveInfo>> functions, LocomotiveInfo loco) {
            if (loco.Functions != null) {
                foreach (XmlElement functionElement in loco.Functions) {
                    LocomotiveFunctionInfo function = new(functionElement);

                    if (function.Type == LocomotiveFunctionType.OnOff) {
                        if (!functions.TryGetValue(function.Name, out List<LocomotiveInfo>? locos)) {
                            locos = new List<LocomotiveInfo>();
                            functions.Add(function.Name, locos);
                        }

                        locos.Add(loco);
                    }
                }
            }
        }

        [LayoutEvent("add-locomotive-controller-function-menu-entries", IfSender = "*[@ID='`string(@ID)`']")]
        private void AddLocomotiveControllerFunctionMenuEntries(LayoutEvent e) {
            var functions = new SortedList<string, List<LocomotiveInfo>>();
            var m = new MenuOrMenuItem(Ensure.NotNull<object>(e.Info));

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                AddLocomotiveFunctions(functions, trainLoco.Locomotive);

            // Add all available functions to the menu. If the same function is available on more than one
            // locomotive, it will be presented as a submenu with each locomotive, and a All option.
            if (functions.Count > 0) {
                if (new MenuOrMenuItem(m).Items.Count > 0)
                    m.Items.Add(new ToolStripSeparator());

                foreach (var d in functions) {
                    var locos = d.Value;
                    var functionName = d.Key;

                    Debug.Assert(locos.Count > 0, "locos array is empty");

                    if (locos.Count == 1) {
                        LocomotiveInfo loco = (LocomotiveInfo)locos[0];

                        m.Items.Add(new LocomotiveFunctionMenuItem(train, loco, functionName, true, train.Locomotives.Count > 1));
                    }
                    else {
                        var functionItem = new LayoutMenuItem(GetFunctionDescription((LocomotiveInfo)locos[0], functionName));
                        LocomotiveFunctionInfo? function = null;

                        foreach (LocomotiveInfo loco in locos) {
                            functionItem.DropDownItems.Add(new LocomotiveFunctionMenuItem(train, loco, functionName, false, true));
                            if (function == null)
                                function = loco.GetFunctionByName(functionName);
                        }

                        if (function != null) {
                            if (function.Type != LocomotiveFunctionType.OnOff) {
                                functionItem.DropDownItems.Add("-");
                                functionItem.DropDownItems.Add(new LocomotiveFunctionMenuItem(train, function));
                            }
                        }

                        m.Items.Add(functionItem);
                    }
                }

                functions.Clear();

                // Add presets
                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                    AddLocomotiveFunctionPresets(functions, trainLoco.Locomotive);

                if (functions.Count > 0) {
                    m.Items.Add(new ToolStripSeparator());

                    var presetItem = new LayoutMenuItem("Set function state");

                    foreach (var d in functions) {
                        var locos = d.Value;
                        var functionName = d.Key;

                        if (locos.Count == 1)
                            presetItem.DropDownItems.Add(new LocomotiveFunctionPresetMenuItem(train, locos[0], functionName, true, train.Locomotives.Count > 1));
                        else {
                            var functionItem = new LayoutMenuItem(GetFunctionDescription(locos[0], functionName));

                            foreach (var loco in locos)
                                functionItem.DropDownItems.Add(new LocomotiveFunctionPresetMenuItem(train, loco, functionName, false, true));

                            presetItem.DropDownItems.Add(functionItem);
                        }
                    }

                    m.Items.Add(presetItem);
                }
            }

            if (EventManager.Event(new LayoutEvent("get-command-station-set-function-number-support", train).SetCommandStation(train)) is CommandStationSetFunctionNumberSupportInfo functionNumberSupport &&
                functionNumberSupport.SetFunctionNumberSupport != SetFunctionNumberSupport.None) {
                if (m.Items.Count == 0)
                    AddTrainFunctionNumberItems(m, functionNumberSupport);
                else {
                    m.Items.Add(new ToolStripSeparator());

                    var functionNumberMenuItem = new LayoutMenuItem("Other functions...");

                    AddTrainFunctionNumberItems(new MenuOrMenuItem(functionNumberMenuItem), functionNumberSupport);
                    m.Items.Add(functionNumberMenuItem);
                }
            }
        }

        private void AddTrainFunctionNumberItems(MenuOrMenuItem m, CommandStationSetFunctionNumberSupportInfo functionNumberSupportInfo) {
            if (train.Locomotives.Count == 1)
                AddLocomotiveFunctionNumberItems(m, train.Locomotives[0].Locomotive, functionNumberSupportInfo);
            else
                foreach (var trainLoco in train.Locomotives) {
                    var locoFuncionNumbersMenuItem = new LayoutMenuItem(trainLoco.Name);

                    AddLocomotiveFunctionNumberItems(new MenuOrMenuItem(locoFuncionNumbersMenuItem), trainLoco.Locomotive, functionNumberSupportInfo);
                    m.Items.Add(locoFuncionNumbersMenuItem);
                }
        }

        private void AddLocomotiveFunctionNumberItems(MenuOrMenuItem m, LocomotiveInfo loco, CommandStationSetFunctionNumberSupportInfo functionNumberSupportInfo) {
            for (var functionNumber = functionNumberSupportInfo.MinFunctionNumber; functionNumber <= functionNumberSupportInfo.MaxFunctionNumber; functionNumber++)
                m.Items.Add(new LocomotiveFunctionNumberMenuItem(train, loco, functionNumber, functionNumberSupportInfo.SetFunctionNumberSupport == SetFunctionNumberSupport.FunctionNumberAndBooleanState));
        }

        protected static string GetFunctionDescription(LocomotiveInfo loco, string functionName) {
            var function = loco.GetFunctionByName(functionName);

            
            return function != null ? 
                (!string.IsNullOrEmpty(function.Description) ? function.Description : function.Name) :
                $"Function {functionName} not defined";
        }

        #region Menu Items

        private class LocomotiveFunctionMenuItem : LayoutMenuItem {
            private readonly LocomotiveFunctionInfo function;
            private readonly TrainStateInfo trainState;
            private readonly LocomotiveInfo? loco;
            private readonly Guid id;

            public LocomotiveFunctionMenuItem(TrainStateInfo trainState, LocomotiveInfo loco, string functionName, bool showFunctionName, bool addLocoName) {
                this.trainState = trainState;
                this.loco = loco;
                this.id = loco.Id;
                this.function = Ensure.NotNull<LocomotiveFunctionInfo>(loco.GetFunctionByName(functionName));

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
                    bool state = loco != null && trainState.GetFunctionState(function.Name, loco.Id, false);

                    state = !state;
                    trainState.SetLocomotiveFunctionState(function.Name, id, state);
                }
                else
                    trainState.TriggerLocomotiveFunction(function.Name, id);
            }
        }

        private class LocomotiveFunctionNumberMenuItem : LayoutMenuItem {
            public LocomotiveFunctionNumberMenuItem(TrainStateInfo train, LocomotiveInfo loco, int functionNumber, bool canSetBooleanState) {
                var function = loco.GetFunctionByNumber(functionNumber);

                this.Text = $"Function {functionNumber}{(function != null ? $" ({function})" : "")}";

                if (canSetBooleanState) {
                    this.DropDownItems.Add(new LayoutMenuItem("On", null, (sender, e) => EventManager.Event(new LayoutEvent("trigger-locomotive-function-number", loco, functionNumber).SetCommandStation(train).SetOption("FunctionState", true))));
                    this.DropDownItems.Add(new LayoutMenuItem("Off", null, (sender, e) => EventManager.Event(new LayoutEvent("trigger-locomotive-function-number", loco, functionNumber).SetCommandStation(train).SetOption("FunctionState", false))));
                }
                else
                    this.Click += (sender, e) => EventManager.Event(new LayoutEvent("trigger-locomotive-function-number", loco, functionNumber).SetCommandStation(train));
            }
        }

        private class LocomotiveFunctionPresetMenuItem : LayoutMenuItem {
            private readonly LocomotiveFunctionInfo function;
            private readonly TrainStateInfo trainState;
            private readonly LocomotiveInfo loco;

            public LocomotiveFunctionPresetMenuItem(TrainStateInfo trainState, LocomotiveInfo loco, string functionName, bool showFunctionName, bool addLocoName) {
                this.trainState = trainState;
                this.loco = loco;
                this.function = Ensure.NotNull<LocomotiveFunctionInfo>(loco.GetFunctionByName(functionName));

                if (showFunctionName)
                    this.Text = LocomotiveController.GetFunctionDescription(loco, functionName) + (addLocoName ? (" (" + loco.DisplayName + ")") : "");
                else
                    this.Text = loco.DisplayName;

                DropDownItems.Add("On", null, new EventHandler(this.MenuItemOn_Click));
                DropDownItems.Add("Off", null, new EventHandler(this.MenuItemOff_Click));
            }

            protected void MenuItemOn_Click(object? sender, EventArgs e) {
                trainState.SetLocomotiveFunctionStateValue(function.Name, loco.Id, true);
            }

            protected void MenuItemOff_Click(object? sender, EventArgs e) {
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

        private void PanelInfo_Paint(object? sender, System.Windows.Forms.PaintEventArgs e) {
            int xText;
            float yText;

            if (train.Locomotives.Count == 1) {
                LocomotiveInfo loco = train.Locomotives[0].Locomotive;

                using (LocomotiveImagePainter locoPainter = new(LayoutModel.LocomotiveCatalog)) {
                    locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
                }

                yText = 2;
                xText = 55;
            }
            else {
                using LocomotiveImagePainter locoPainter = new(LayoutModel.LocomotiveCatalog);
                int x = 2;

                locoPainter.FrameSize = new Size(28, 20);

                foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                    locoPainter.LocomotiveElement = trainLoco.Locomotive.Element;
                    locoPainter.FlipImage = trainLoco.Orientation == LocomotiveOrientation.Backward;
                    locoPainter.Origin = new Point(x, 2);
                    locoPainter.Draw(e.Graphics);

                    x += locoPainter.FrameSize.Width + 2;
                }

                xText = 2;
                yText = 24;
            }

            using Brush textBrush = new SolidBrush(SystemColors.WindowText);
            SizeF textSize;
            string status = train.StatusText;

            using (Font statusFont = new("Arial", 8, FontStyle.Regular)) {
                textSize = e.Graphics.MeasureString(status, statusFont);
                e.Graphics.DrawString(status, statusFont, textBrush, new PointF(xText, yText));
            }

            yText += textSize.Height;
        }

        private void LocomotiveController_Closed(object? sender, System.EventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonLocate_Click(object? sender, System.EventArgs e) {
            ModelComponent? locoLocation = null;

            if (train.LocomotiveBlock != null) {
                if (train.LocomotiveBlock.BlockDefinintion != null)
                    locoLocation = train.LocomotiveBlock.BlockDefinintion;
                else {
                    if (train.LocomotiveBlock.TrackEdges.Count > 0)
                        locoLocation = train.LocomotiveBlock.TrackEdges[0].Track;
                }

                EventManager.Event(new LayoutEvent("ensure-component-visible", locoLocation, true).SetFrameWindow(LayoutController.ActiveFrameWindow));
            }
        }

        private void ButtonStop_Click(object? sender, System.EventArgs e) {
            train.SpeedInSteps = 0;
        }

        private void ButtonBackward_Click(object? sender, System.EventArgs e) {
            if (train.Speed > -LayoutModel.Instance.LogicalSpeedSteps)
                train.Speed--;
        }

        private void ButtonForward_Click(object? sender, System.EventArgs e) {
            if (train.Speed < LayoutModel.Instance.LogicalSpeedSteps)
                train.Speed++;
        }

        private void ButtonLight_Click(object? sender, EventArgs e) {
            Dispatch.Call.SetLocomotiveLightsRequest(train, !train.Lights);
        }

        private void MenuLightsOn_Click(object? sender, EventArgs e) {
            train.SetLightsValue(true);
        }

        private void MenuItemLightsOff_Click(object? sender, EventArgs e) {
            train.SetLightsValue(false);
        }

        private void ButtonFunction_Click(object? sender, System.EventArgs e) {
            var functionMenu = new ContextMenuStrip();

            EventManager.Event(new LayoutEvent("add-locomotive-controller-function-menu-entries", train, functionMenu));

            if (functionMenu.Items.Count == 0) {
                var noFunctions = new LayoutMenuItem("No functions") {
                    Enabled = false
                };
                functionMenu.Items.Add(noFunctions);
            }

            functionMenu.Show(this, new Point(buttonFunction.Left, buttonFunction.Bottom));
        }

        private void ButtonProperties_Click(object? sender, System.EventArgs e) {
            EventManager.Event(new LayoutEvent("edit-train-properties", train));
        }

        private void PanelSpeedLimit_Paint(object? sender, System.Windows.Forms.PaintEventArgs e) {
            const int boarderSize = 3;

            e.Graphics.FillEllipse(Brushes.Red, 0, 0, panelSpeedLimit.Width, panelSpeedLimit.Height);
            e.Graphics.FillEllipse(Brushes.White, boarderSize, boarderSize, panelSpeedLimit.Width - (2 * boarderSize), panelSpeedLimit.Height - (2 * boarderSize));
        }

        #region Speed change menu

        private void AddAccelerationMenuEntries(object menuOrMenuItem, int speed) {
            foreach (MotionRampInfo ramp in LayoutModel.Instance.Ramps)
                new SpeedChangeMenuItem(train, speed, ramp).AddMeTo(menuOrMenuItem);
        }

        private MotionRampInfo GetDefaultRamp(int speed) {
            if (speed == 0)
                return LayoutModel.StateManager.DefaultStopRamp;
            else 
                return speed > train.Speed ? LayoutModel.StateManager.DefaultAccelerationRamp : LayoutModel.StateManager.DefaultDecelerationRamp;
        }

        private void AddSpeedMenuEntries(ToolStripDropDown menu, LocomotiveOrientation direction) {
            for (int i = 1; i <= LayoutModel.Instance.LogicalSpeedSteps; i++) {
                int speed = (direction == LocomotiveOrientation.Forward) ? i : -i;
                var speedItem = new LayoutMenuItem(i.ToString());

                AddAccelerationMenuEntries(speedItem, speed);

                if (speedItem.DropDownItems.Count == 0) {
                    speedItem.Click += (s, ea) => train.ChangeSpeed(speed, GetDefaultRamp(speed));
                }

                menu.Items.Add(speedItem);
            }
        }

        private void ButtonStopMenu_Click(object? sender, System.EventArgs e) {
            var menu = new ContextMenuStrip();

            AddAccelerationMenuEntries(menu, 0);

            if (menu.Items.Count > 0)
                menu.Show(this, new Point(buttonStopMenu.Left, buttonStopMenu.Bottom));
            else
                train.ChangeSpeed(0, GetDefaultRamp(0));
        }

        private void ButtonBackwardMenu_Click(object? sender, System.EventArgs e) {
            var menu = new ContextMenuStrip();

            AddSpeedMenuEntries(menu, LocomotiveOrientation.Backward);
            menu.Show(this, new Point(buttonBackwardMenu.Left, buttonBackwardMenu.Bottom));
        }

        private void ButtonForwardMenu_Click(object? sender, System.EventArgs e) {
            var menu = new ContextMenuStrip();

            AddSpeedMenuEntries(menu, LocomotiveOrientation.Forward);
            menu.Show(this, new Point(buttonForwardMenu.Left, buttonForwardMenu.Bottom));
        }

        private void LocomotiveController_Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }

        private class SpeedChangeMenuItem : LayoutMenuItem {
            private readonly MotionRampInfo ramp;
            private readonly int speed;
            private readonly TrainStateInfo train;

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
