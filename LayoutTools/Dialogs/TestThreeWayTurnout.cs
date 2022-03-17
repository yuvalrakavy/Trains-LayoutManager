using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    public partial class TestThreeWayTurnout : Form {
        private readonly LayoutThreeWayTurnoutComponent turnout;
        private int _state = -1;
        private readonly Guid frameWindowId;

        public TestThreeWayTurnout(Guid frameWindowId, LayoutThreeWayTurnoutComponent turnout) {
            InitializeComponent();

            this.frameWindowId = frameWindowId;
            this.turnout = turnout;

            radioButtonBottom.Checked = false;
            radioButtonTop.Checked = false;
            radioButtonLeft.Checked = false;
            radioButtonRight.Checked = false;
            State = -1;

            switch (turnout.Tip) {
                case LayoutComponentConnectionPoint.T: radioButtonTop.Visible = false; break;
                case LayoutComponentConnectionPoint.B: radioButtonBottom.Visible = false; break;
                case LayoutComponentConnectionPoint.L: radioButtonLeft.Visible = false; break;
                case LayoutComponentConnectionPoint.R: radioButtonRight.Visible = false; break;
            }

            UpdateButtons();
        }

        private void UpdateButtons() {
            bool fullyConnected = turnout.FullyConnected;

            radioButtonTop.Enabled = fullyConnected;
            radioButtonRight.Enabled = fullyConnected;
            radioButtonBottom.Enabled = fullyConnected;
            radioButtonLeft.Enabled = fullyConnected;

            buttonConnect.Enabled = !fullyConnected;
            buttonDisconnect.Enabled = turnout.IsConnected;

            buttonPassed.Enabled = fullyConnected;
            buttonSwap.Enabled = fullyConnected;
            panelIllustration.Enabled = fullyConnected;

            if (!fullyConnected)
                State = -1;
        }

        protected int State {
            get {
                return _state;
            }

            set {
                if (turnout.FullyConnected) {
                    _state = value;

                    if (_state >= 0) {
                        ControlConnectionPointReference? refRight = new(Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlRight"]));
                        ControlConnectionPointReference refLeft = new(Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlLeft"]));
                        int stateRight = 0;
                        int stateLeft = 0;

                        if (_state == 1)
                            stateRight = 1;
                        else if (_state == 2)
                            stateLeft = 1;

                        var commandStation = Dispatch.Call.GetCommandStation(refRight);

                        Dispatch.Call.ChangeTrackComponentStateCommand(commandStation, refRight, stateRight);
                        Dispatch.Call.ChangeTrackComponentStateCommand(commandStation, refLeft, stateLeft);
                    }
                }
                else
                    _state = -1;

                RadioButton[][] checkRadioButtonTable = new RadioButton[][] {
                    new RadioButton[] { radioButtonBottom, radioButtonLeft, radioButtonRight }, // Tip = top
                    new RadioButton[] { radioButtonLeft, radioButtonTop, radioButtonBottom },   // Tip = right
                    new RadioButton[] { radioButtonTop, radioButtonRight, radioButtonLeft },    // Tip = bottom
                    new RadioButton[] { radioButtonRight, radioButtonBottom, radioButtonTop }   // Tip = left
                };

                int tipIndex = 0;

                switch (turnout.Tip) {
                    case LayoutComponentConnectionPoint.T: tipIndex = 0; break;
                    case LayoutComponentConnectionPoint.R: tipIndex = 1; break;
                    case LayoutComponentConnectionPoint.B: tipIndex = 2; break;
                    case LayoutComponentConnectionPoint.L: tipIndex = 3; break;
                }

                RadioButton? checkedRadioButton = null;

                if (_state >= 0)
                    checkedRadioButton = checkRadioButtonTable[tipIndex][_state];

                foreach (RadioButton r in new RadioButton[] { radioButtonTop, radioButtonRight, radioButtonBottom, radioButtonLeft })
                    r.Checked = r == checkedRadioButton;

                panelIllustration.Invalidate();
            }
        }

        private void PanelIllustration_Paint(object? sender, PaintEventArgs e) {
            LayoutThreeWayTurnoutPainter painter = new(new Size(64, 64), turnout.Tip, _state);

            if (!panelIllustration.Enabled) {
                painter.TrackColor = Color.Gray;
                painter.SegmentColors = new Color[] { Color.Gray, Color.Gray, Color.Gray };
            }

            painter.Paint(e.Graphics);
        }

        [DispatchTarget]
        private bool QueryTestLayoutObject() => true;


        [DispatchTarget]
        private void OnComponentConnectedToControlModule(IModelComponentConnectToControl component, ControlConnectionPoint connetionPoint) {
            UpdateButtons();
        }

        [DispatchTarget]
        private void OnComponentDisconnectedFromControlModule(ModelComponent component, ControlConnectionPoint connectionPoint) {
            UpdateButtons();
        }

        private void TestThreeWayTurnout_FormClosed(object? sender, FormClosedEventArgs e) {
            LayoutController.Instance.EndDesignTimeActivation();
            Dispatch.Call.DeselectControlObjects(frameWindowId);
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void TestThreeWayTurnout_Load(object? sender, EventArgs e) {
            Dispatch.AddObjectInstanceDispatcherTargets(this);
            EventManager.AddObjectSubscriptions(this);
        }

        private void RadioButtonTop_Clicked(object? sender, EventArgs e) {
            switch (turnout.Tip) {
                case LayoutComponentConnectionPoint.B: State = 0; break;
                case LayoutComponentConnectionPoint.R: State = 1; break;
                case LayoutComponentConnectionPoint.L: State = 2; break;
            }
        }

        private void RadioButtonRight_Clicked(object? sender, EventArgs e) {
            switch (turnout.Tip) {
                case LayoutComponentConnectionPoint.L: State = 0; break;
                case LayoutComponentConnectionPoint.B: State = 1; break;
                case LayoutComponentConnectionPoint.T: State = 2; break;
            }
        }

        private void RadioButtonBottom_Clicked(object? sender, EventArgs e) {
            switch (turnout.Tip) {
                case LayoutComponentConnectionPoint.T: State = 0; break;
                case LayoutComponentConnectionPoint.R: State = 2; break;
                case LayoutComponentConnectionPoint.L: State = 1; break;
            }
        }

        private void RadioButtonLeft_Clicked(object? sender, EventArgs e) {
            switch (turnout.Tip) {
                case LayoutComponentConnectionPoint.R: State = 0; break;
                case LayoutComponentConnectionPoint.B: State = 2; break;
                case LayoutComponentConnectionPoint.T: State = 1; break;
            }
        }

        private void ButtonPassed_Click(object? sender, EventArgs e) {
            var connectionPoints = Ensure.NotNull<IList<ControlConnectionPoint>>(LayoutModel.ControlManager.ConnectionPoints[turnout.Id]);

            foreach (ControlConnectionPoint cp in connectionPoints) {
                if (cp.UserActionRequired) {
                    SetControlUserActionRequiredCommand setCommand = new(cp, false);

                    LayoutController.Do(setCommand);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonFailed_Click(object? sender, EventArgs e) {
            var connectionPoints = Ensure.NotNull<IList<ControlConnectionPoint>>(LayoutModel.ControlManager.ConnectionPoints[turnout.Id]);

            foreach (ControlConnectionPoint cp in connectionPoints) {
                SetControlUserActionRequiredCommand setCommand = new(cp, true);

                LayoutController.Do(setCommand);
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonDisconnect_Click(object? sender, EventArgs e) {
            var m = new ContextMenuStrip();
            var connectionPoints = Ensure.NotNull<IList<ControlConnectionPoint>>(LayoutModel.ControlManager.ConnectionPoints[turnout.Id]);

            foreach (var cp in connectionPoints) {
                ControlConnectionPoint connectionPoint = cp;

                m.Items.Add($"{cp.DisplayName} ({cp.Module.ConnectionPoints.GetLabel(cp.Index, true)})", null,
                    (_, _) => {
                        DisconnectComponentFromConnectionPointCommand disconnectCommand = new(connectionPoint);

                        LayoutController.Do(disconnectCommand);
                    });
            }

            m.Show(buttonDisconnect.Parent, new Point(buttonDisconnect.Left, buttonDisconnect.Bottom));
        }

        private void ButtonConnect_Click(object? sender, EventArgs e) {
            var m = new ContextMenuStrip();

            foreach (ModelComponentControlConnectionDescription connectionDescription in turnout.ControlConnectionDescriptions) {
                if (LayoutModel.ControlManager.ConnectionPoints[turnout.Id, connectionDescription.Name] == null) {
                    ModelComponentControlConnectionDescription desc = connectionDescription;

                    m.Items.Add(connectionDescription.DisplayName, null,
                        (_, _) => {
                            ControlConnectionPointDestination destination = new(turnout, desc);
                            Dispatch.Call.RequestComponentToControlConnect(destination);
                        });
                }
            }

            m.Show(buttonConnect.Parent, new Point(buttonConnect.Left, buttonConnect.Bottom));
        }

        private void ButtonSwap_Click(object? sender, EventArgs e) {
            LayoutCompoundCommand swapCommand = new("Swap three way turnouts controls");
            ControlConnectionPointReference refRight = new(Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlRight"]));
            ControlConnectionPointReference refLeft = new(Ensure.NotNull<ControlConnectionPoint>(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlLeft"]));
            string rightName = refRight.ConnectionPoint.Name ?? String.Empty;
            string rightDisplayName = refRight.ConnectionPoint.DisplayName;
            string leftName = refLeft.ConnectionPoint.Name ?? String.Empty;
            string leftDisplayName = refLeft.ConnectionPoint.DisplayName;

            swapCommand.Add(new DisconnectComponentFromConnectionPointCommand(refRight.ConnectionPoint));
            swapCommand.Add(new DisconnectComponentFromConnectionPointCommand(refLeft.ConnectionPoint));
            swapCommand.Add(new ConnectComponentToControlConnectionPointCommand(refRight.Module, refRight.Index, turnout, leftName, leftDisplayName));
            swapCommand.Add(new ConnectComponentToControlConnectionPointCommand(refLeft.Module, refLeft.Index, turnout, rightName, rightDisplayName));

            LayoutController.Do(swapCommand);
        }
    }
}
