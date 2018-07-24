using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.Dialogs {
    public partial class TestThreeWayTurnout : Form {
        LayoutThreeWayTurnoutComponent turnout;
        int _state = -1;
		Guid frameWindowId;

        public TestThreeWayTurnout(Guid frameWindowId, LayoutThreeWayTurnoutComponent turnout) {
            InitializeComponent();

			this.frameWindowId = frameWindowId;
            this.turnout = turnout;

            radioButtonBottom.Checked = false;
            radioButtonTop.Checked = false;
            radioButtonLeft.Checked = false;
            radioButtonRight.Checked = false;
            State = -1;

            switch(turnout.Tip) {
                case LayoutComponentConnectionPoint.T: radioButtonTop.Visible = false; break;
                case LayoutComponentConnectionPoint.B: radioButtonBottom.Visible = false; break;
                case LayoutComponentConnectionPoint.L: radioButtonLeft.Visible = false; break;
                case LayoutComponentConnectionPoint.R: radioButtonRight.Visible = false; break;
            }

            updateButtons();
        }

        private void updateButtons() {
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

            if(!fullyConnected)
                State = -1;
        }

        protected int State {
            get {
                return _state;
            }

            set {
                if(turnout.FullyConnected) {
                    _state = value;

                    if(_state >= 0) {
                        ControlConnectionPointReference refRight = new ControlConnectionPointReference(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlRight"]);
                        ControlConnectionPointReference refLeft = new ControlConnectionPointReference(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlLeft"]);
                        int stateRight = 0;
                        int stateLeft = 0;


                        if(_state == 1)
                            stateRight = 1;
                        else if(_state == 2)
                            stateLeft = 1;

						EventManager.AsyncEvent(new LayoutEvent<ControlConnectionPointReference, int>("change-track-component-state-command", refRight, stateRight).SetCommandStation(refRight));
						EventManager.AsyncEvent(new LayoutEvent<ControlConnectionPointReference, int>("change-track-component-state-command", refLeft, stateLeft).SetCommandStation(refLeft));
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

                switch(turnout.Tip) {
                    case LayoutComponentConnectionPoint.T: tipIndex = 0; break;
                    case LayoutComponentConnectionPoint.R: tipIndex = 1; break;
                    case LayoutComponentConnectionPoint.B: tipIndex = 2; break;
                    case LayoutComponentConnectionPoint.L: tipIndex = 3; break;
                }

                RadioButton checkedRadioButton = null;

                if(_state >= 0)
                    checkedRadioButton = checkRadioButtonTable[tipIndex][_state];

                foreach(RadioButton r in new RadioButton[] { radioButtonTop, radioButtonRight, radioButtonBottom, radioButtonLeft })
                    r.Checked = (r == checkedRadioButton);

                panelIllustration.Invalidate();
            }
        }

        private void panelIllustration_Paint(object sender, PaintEventArgs e) {
            LayoutThreeWayTurnoutPainter painter = new LayoutThreeWayTurnoutPainter(new Size(64, 64), turnout.Tip, _state);

            if(!panelIllustration.Enabled) {
                painter.TrackColor = Color.Gray;
                painter.SegmentColors = new Color[] { Color.Gray, Color.Gray, Color.Gray };
            }

            painter.Paint(e.Graphics);
        }

        [LayoutEvent("query-test-layout-object")]
        private void queryTestLayoutObject(LayoutEvent e) {
            e.Info = this;
        }

        [LayoutEvent("component-connected-to-control-module")]
        [LayoutEvent("component-disconnected-from-control-module")]
        private void componentConnectedOrDisconnectedToControlModule(LayoutEvent e) {
            updateButtons();
        }

        private void TestThreeWayTurnout_FormClosed(object sender, FormClosedEventArgs e) {
			LayoutController.Instance.EndDesignTimeActivation();
            EventManager.Event(new LayoutEvent(this, "deselect-control-objects").SetFrameWindow(frameWindowId));
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void TestThreeWayTurnout_Load(object sender, EventArgs e) {
            EventManager.AddObjectSubscriptions(this);
        }

        private void radioButtonTop_Clicked(object sender, EventArgs e) {
            switch(turnout.Tip) {
                case LayoutComponentConnectionPoint.B: State = 0; break;
                case LayoutComponentConnectionPoint.R: State = 1; break;
                case LayoutComponentConnectionPoint.L: State = 2; break;
            }
        }

        private void radioButtonRight_Clicked(object sender, EventArgs e) {
            switch(turnout.Tip) {
                case LayoutComponentConnectionPoint.L: State = 0; break;
                case LayoutComponentConnectionPoint.B: State = 1; break;
                case LayoutComponentConnectionPoint.T: State = 2; break;
            }
        }

        private void radioButtonBottom_Clicked(object sender, EventArgs e) {
            switch(turnout.Tip) {
                case LayoutComponentConnectionPoint.T: State = 0; break;
                case LayoutComponentConnectionPoint.R: State = 2; break;
                case LayoutComponentConnectionPoint.L: State = 1; break;
            }

        }

        private void radioButtonLeft_Clicked(object sender, EventArgs e) {
            switch(turnout.Tip) {
                case LayoutComponentConnectionPoint.R: State = 0; break;
                case LayoutComponentConnectionPoint.B: State = 2; break;
                case LayoutComponentConnectionPoint.T: State = 1; break;
            }
        }

        private void buttonPassed_Click(object sender, EventArgs e) {
			IList<ControlConnectionPoint> connectionPoints = LayoutModel.ControlManager.ConnectionPoints[turnout.Id];

            foreach(ControlConnectionPoint cp in connectionPoints) {
                if(cp.UserActionRequired) {
                    SetControlUserActionRequiredCommand setCommand = new SetControlUserActionRequiredCommand(cp, false);

                    LayoutController.Do(setCommand);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonFailed_Click(object sender, EventArgs e) {
			IList<ControlConnectionPoint> connectionPoints = LayoutModel.ControlManager.ConnectionPoints[turnout.Id];

            foreach(ControlConnectionPoint cp in connectionPoints) {
                SetControlUserActionRequiredCommand setCommand = new SetControlUserActionRequiredCommand(cp, true);

                LayoutController.Do(setCommand);
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e) {
            ContextMenu m = new ContextMenu();

			foreach(ControlConnectionPoint cp in LayoutModel.ControlManager.ConnectionPoints[turnout.Id]) {
                ControlConnectionPoint connectionPoint = cp;

                m.MenuItems.Add(cp.DisplayName + " (" + cp.Module.ConnectionPoints.GetLabel(cp.Index, true) + ")",
                    delegate(object s, EventArgs ev) {
                        DisconnectComponentFromConnectionPointCommand disconnectCommand = new DisconnectComponentFromConnectionPointCommand(connectionPoint);

                        LayoutController.Do(disconnectCommand);
                    });
            }

            m.Show(buttonDisconnect.Parent, new Point(buttonDisconnect.Left, buttonDisconnect.Bottom));
        }

        private void buttonConnect_Click(object sender, EventArgs e) {
            ContextMenu m = new ContextMenu();

            foreach(ModelComponentControlConnectionDescription connectionDescription in turnout.ControlConnectionDescriptions) {
                if(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, connectionDescription.Name] == null) {
                    ModelComponentControlConnectionDescription desc = connectionDescription;

                    m.MenuItems.Add(connectionDescription.DisplayName,
                        delegate(object s, EventArgs ev) {

                            ControlConnectionPointDestination destination = new ControlConnectionPointDestination(turnout, desc);
                            EventManager.Event(new LayoutEvent(destination, "request-component-to-control-connect"));
                        });
                }
            }

            m.Show(buttonConnect.Parent, new Point(buttonConnect.Left, buttonConnect.Bottom));
        }

        private void buttonSwap_Click(object sender, EventArgs e) {
            LayoutCompoundCommand swapCommand = new LayoutCompoundCommand("Swap three way turnouts controls");
			ControlConnectionPointReference refRight = new ControlConnectionPointReference(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlRight"]);
			ControlConnectionPointReference refLeft = new ControlConnectionPointReference(LayoutModel.ControlManager.ConnectionPoints[turnout.Id, "ControlLeft"]);
            string rightName = refRight.ConnectionPoint.Name;
            string rightDisplayName = refRight.ConnectionPoint.DisplayName;
            string leftName = refLeft.ConnectionPoint.Name;
            string leftDisplayName = refLeft.ConnectionPoint.DisplayName;

            swapCommand.Add(new DisconnectComponentFromConnectionPointCommand(refRight.ConnectionPoint));
            swapCommand.Add(new DisconnectComponentFromConnectionPointCommand(refLeft.ConnectionPoint));
            swapCommand.Add(new ConnectComponentToControlConnectionPointCommand(refRight.Module, refRight.Index, turnout, leftName, leftDisplayName));
            swapCommand.Add(new ConnectComponentToControlConnectionPointCommand(refLeft.Module, refLeft.Index, turnout, rightName, rightDisplayName));

            LayoutController.Do(swapCommand);
        }
    }
}
