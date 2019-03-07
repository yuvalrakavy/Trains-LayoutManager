#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

#endregion


namespace LayoutManager.Tools.Dialogs {

    #pragma warning disable IDE0051, IDE0060
    partial class TestLayoutObject : Form {
        ControlConnectionPointReference connectionPointRef;
        IModelComponentConnectToControl component;
        LayoutDelayedEvent toggleEvent = null;
        Guid frameWindowId;

        int state = -1;

        public TestLayoutObject(Guid frameWindowId, ControlConnectionPointReference connectionPointRef) {
            InitializeComponent();

            this.frameWindowId = frameWindowId;
            this.connectionPointRef = connectionPointRef;

            if (connectionPointRef.IsConnected)
                component = connectionPointRef.ConnectionPoint.Component;

            Initialize();
        }

        public TestLayoutObject(Guid frameWindowId, IModelComponentConnectToControl component) {
            InitializeComponent();

            this.frameWindowId = frameWindowId;
            UseComponent(component);

            Initialize();
        }

        protected void UseComponent(IModelComponentConnectToControl component) {
            IList<ControlConnectionPoint> connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component.Id];

            if (connectionPoints == null || connectionPoints.Count == 0)
                throw new ArgumentException("Component " + component.FullDescription + " is not connected to control module");

            List<ControlConnectionPoint> validConnectionPoints = new List<ControlConnectionPoint>();

            foreach (ControlConnectionPoint ccp in connectionPoints) {
                if (ccp.Usage == ControlConnectionPointUsage.Output)
                    validConnectionPoints.Add(ccp);
            }

            if (validConnectionPoints.Count == 0)
                throw new LayoutException(component, "This component is not connected to any output control module");

            connectionPointRef = new ControlConnectionPointReference(validConnectionPoints[0]);
            this.component = component;
        }

        private void Initialize() {

            if (component is IModelComponentHasReverseLogic componentWithReverseLogic) {
                checkBoxReverseLogic.Enabled = true;
                checkBoxReverseLogic.Checked = componentWithReverseLogic.ReverseLogic;
            }
            else
                checkBoxReverseLogic.Enabled = false;

            if (component == null)
                buttonConnect.Text = "&Connect";
            else
                buttonConnect.Text = "&Disconnect";

            if (component == null)
                Text = "Testing " + connectionPointRef.Module.ConnectionPoints.GetLabel(connectionPointRef.Index, true) +
                    " on " + connectionPointRef.Module.Bus.Name;
            else {
                Text = "Testing " + connectionPointRef.ConnectionPoint.DisplayName + " at " +
                connectionPointRef.Module.ModuleType.GetConnectionPointAddressText(connectionPointRef.Module.Address, connectionPointRef.Index, true);
                EventManager.Event(new LayoutEvent(connectionPointRef, "show-control-connection-point").SetFrameWindow(frameWindowId));
            }
        }

        protected int State {
            get {
                return state;
            }

            set {
                state = value;

                if (state >= 0) {
                    int stateToSend = state;

                    if (checkBoxReverseLogic.Enabled && checkBoxReverseLogic.Checked)
                        stateToSend = 1 - stateToSend;

                    EventManager.AsyncEvent(new LayoutEventInfoValueType<ControlConnectionPointReference, int>("change-track-component-state-command", connectionPointRef, stateToSend).SetCommandStation(connectionPointRef));

                    if (state == 0)
                        radioButtonOff.Checked = true;
                    else
                        radioButtonOn.Checked = true;

                }
                else {
                    radioButtonOff.Checked = false;
                    radioButtonOn.Checked = false;
                }

                panelIllustration.Invalidate();
            }
        }

        private void stopToggle() {
            if (checkBoxToggle.Checked) {
                if (toggleEvent != null) {
                    toggleEvent.Cancel();
                    toggleEvent = null;
                }

                checkBoxToggle.Checked = false;
            }
        }

        private void TestLayoutObject_Load(object sender, EventArgs e) {
            EventManager.AddObjectSubscriptions(this);
        }

        private void TestLayoutObject_FormClosed(object sender, FormClosedEventArgs e) {
            LayoutController.Instance.EndDesignTimeActivation();
            EventManager.Event(new LayoutEvent(this, "deselect-control-objects").SetFrameWindow(frameWindowId));
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void radioButtonOn_Click(object sender, EventArgs e) {
            stopToggle();
            State = 1;
        }

        private void radioButtonOff_Click(object sender, EventArgs e) {
            stopToggle();
            State = 0;
        }

        private void checkBoxToggle_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxToggle.Checked) {
                if (toggleEvent == null) {
                    toggleEvent = EventManager.DelayedEvent((int)numericUpDownToggleTime.Value * 1000, new LayoutEvent(this, "test-layout-object-toggle"));
                    State = 1 - State;
                }
            }
            else {
                if (toggleEvent != null) {
                    toggleEvent.Cancel();
                    toggleEvent = null;
                }
            }
        }

        [LayoutEvent("test-layout-object-toggle")]
        private void testLayoutObjectToggle(LayoutEvent e) {
            if (e.Sender == this) {
                State = 1 - State;

                if (checkBoxToggle.Checked)
                    toggleEvent = EventManager.DelayedEvent((int)numericUpDownToggleTime.Value * 1000, new LayoutEvent(this, "test-layout-object-toggle"));
            }
        }

        [LayoutEvent("query-test-layout-object")]
        private void queryTestLayoutObject(LayoutEvent e) {
            e.Info = this;
        }

        private void panelIllustration_Paint(object sender, PaintEventArgs e) {
            Size componentSize = new Size(58, 58);

            if (component is LayoutTurnoutTrackComponent turnout) {
                LayoutComponentConnectionPoint switchPosition = LayoutComponentConnectionPoint.Empty;

                if (state == 0)
                    switchPosition = turnout.Straight;
                else if (state == 1)
                    switchPosition = turnout.Branch;

                e.Graphics.TranslateTransform(2, 2);

                LayoutTurnoutTrackPainter painter = new LayoutTurnoutTrackPainter(componentSize, turnout.Tip, turnout.Straight, turnout.Branch, switchPosition) {
                    TrackWidth = 6
                };
                painter.Paint(e.Graphics);
            }
            else if (component is LayoutSignalComponent) {
                Graphics g = e.Graphics;
                LayoutSignalComponent signal = (LayoutSignalComponent)component;

                if (signal.Info.SignalType == LayoutSignalType.Lights) {
                    int topMargin = 5;
                    int lightSize = 14;
                    Size signalFrameSize = new Size(30, 46);
                    Rectangle signalFrame = new Rectangle(new Point((panelIllustration.Width - signalFrameSize.Width) / 2, topMargin), signalFrameSize);
                    Rectangle redLightBbox = new Rectangle(new Point((panelIllustration.Width - lightSize) / 2, topMargin + signalFrameSize.Height / 4 - lightSize / 2), new Size(lightSize, lightSize));
                    Rectangle greenLightBbox = new Rectangle(new Point((panelIllustration.Width - lightSize) / 2, topMargin + 3 * signalFrameSize.Height / 4 - lightSize / 2), new Size(lightSize, lightSize));

                    using (Pen pollPen = new Pen(Color.Black, 6))
                        g.DrawLine(pollPen, new Point(panelIllustration.Width / 2, topMargin + signalFrameSize.Height), new Point(panelIllustration.Width / 2, panelIllustration.Height));

                    g.FillRectangle(Brushes.LightGray, signalFrame);
                    g.DrawRectangle(Pens.Black, signalFrame);

                    if (state == 1)
                        g.FillEllipse(Brushes.Red, redLightBbox);
                    g.DrawEllipse(Pens.Black, redLightBbox);

                    if (state == 0)
                        g.FillEllipse(Brushes.LightGreen, greenLightBbox);
                    g.DrawEllipse(Pens.Black, greenLightBbox);
                }
                else {
                    int pollTopMargin = 14;
                    int semaphoreJoin = 24;

                    using (Pen pollPen = new Pen(Color.Black, 8))
                        g.DrawLine(pollPen, new Point(panelIllustration.Width / 2, pollTopMargin), new Point(panelIllustration.Width / 2, panelIllustration.Height));

                    g.TranslateTransform(panelIllustration.Width / 2, semaphoreJoin);

                    if (state == 0)
                        g.RotateTransform(-45);

                    Rectangle semaphoreRect = new Rectangle(new Point(-12, 0), new Size(34, 6));
                    g.FillRectangle(Brushes.Brown, semaphoreRect);
                    g.DrawRectangle(Pens.Black, semaphoreRect);
                }
            }
            else if (component is LayoutDoubleSlipTrackComponent doubleSlip) {
                LayoutDoubleSlipPainter painter = new LayoutDoubleSlipPainter(componentSize, doubleSlip.DiagonalIndex, state);

                e.Graphics.TranslateTransform(2, 2);

                painter.TrackWidth = 6;
                painter.Paint(e.Graphics);
            }
        }

        private void checkBoxReverseLogic_CheckedChanged(object sender, EventArgs e) {
            ModelComponent theComponent = (ModelComponent)component;
            LayoutXmlInfo xmlInfo = new LayoutXmlInfo(theComponent);

            xmlInfo.DocumentElement.SetAttribute("ReverseLogic", XmlConvert.ToString(checkBoxReverseLogic.Checked));

            LayoutModifyComponentDocumentCommand modifyDocCommand = new LayoutModifyComponentDocumentCommand(theComponent, xmlInfo);

            LayoutController.Do(modifyDocCommand);
            State = -1;     // Force user to select command
            panelIllustration.Invalidate();
        }

        private void buttonPassed_Click(object sender, EventArgs e) {
            if (connectionPointRef.IsDefined && connectionPointRef.ConnectionPoint.UserActionRequired == true) {
                SetControlUserActionRequiredCommand setCommand = new SetControlUserActionRequiredCommand(connectionPointRef.ConnectionPoint, false);

                LayoutController.Do(setCommand);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonFailed_Click(object sender, EventArgs e) {
            SetControlUserActionRequiredCommand setCommand = new SetControlUserActionRequiredCommand(connectionPointRef.ConnectionPoint, true);

            LayoutController.Do(setCommand);
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonConnect_Click(object sender, EventArgs e) {
            if (component == null) {
                CommandStationInputEvent csEvent;

                if (connectionPointRef.Module.Bus.BusType.AddressingMethod == ControlAddressingMethod.DirectConnectionPointAddressing)
                    csEvent = new CommandStationInputEvent((ModelComponent)connectionPointRef.Module.Bus.BusProvider, connectionPointRef.Module.Bus, connectionPointRef.Module.Address + connectionPointRef.Index);
                else
                    csEvent = new CommandStationInputEvent((ModelComponent)connectionPointRef.Module.Bus.BusProvider, connectionPointRef.Module.Bus, connectionPointRef.Module.Address, connectionPointRef.Index, 0);

                PickComponentToConnectToAddress pickDialog = new PickComponentToConnectToAddress(csEvent);

                new SemiModalDialog(this, pickDialog, delegate (Form dialog, object info) {
                    if (pickDialog.DialogResult == DialogResult.OK) {
                        ControlConnectionPoint result = (ControlConnectionPoint)EventManager.Event(new LayoutEvent(pickDialog.ConnectionDestination, "connect-component-to-control-module-address-request", null, csEvent));

                        EventManager.Event(new LayoutEvent(connectionPointRef, "show-control-connection-point").SetFrameWindow(frameWindowId));
                        this.component = result.Component;
                        Initialize();
                        panelIllustration.Invalidate();
                    }
                }, null).ShowDialog();
            }
            else {
                DisconnectComponentFromConnectionPointCommand disconnectCommand = new DisconnectComponentFromConnectionPointCommand(connectionPointRef.ConnectionPoint);

                LayoutController.Do(disconnectCommand);
                component = null;
                Initialize();
            }
        }
    }
}
