#region Using directives

using LayoutManager.CommonUI;
using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace LayoutManager.Tools.Dialogs {
    internal partial class TestLayoutObject : Form {
        private const string A_ReverseLogic = "ReverseLogic";
        private ControlConnectionPointReference? connectionPointRef;
        private IModelComponentConnectToControl? component;
        private LayoutDelayedEvent? toggleEvent = null;
        private readonly Guid frameWindowId;

        private int state = -1;

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
            var connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component.Id];

            if (connectionPoints == null || connectionPoints.Count == 0)
                throw new ArgumentException("Component " + component.FullDescription + " is not connected to control module");

            List<ControlConnectionPoint> validConnectionPoints = new();

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

            if (connectionPointRef != null) {
                if (component == null)
                    Text = $"{connectionPointRef.Module.ConnectionPoints.GetLabel(connectionPointRef.Index, true)} on {connectionPointRef.Module.Bus.Name}";
                else {
                    Text = $"{connectionPointRef.ConnectionPoint.DisplayName} at {connectionPointRef.Module.ModuleType.GetConnectionPointAddressText(connectionPointRef.Module.ModuleType, connectionPointRef.Module.Address, connectionPointRef.Index, true)}";
                    Dispatch.Call.ShowControlConnectionPoint(frameWindowId, connectionPointRef);
                }
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

                    if (connectionPointRef != null)
                        Dispatch.Call.ChangeTrackComponentStateCommand(Dispatch.Call.GetCommandStation(connectionPointRef), connectionPointRef, stateToSend);

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

        private void StopToggle() {
            if (checkBoxToggle.Checked) {
                if (toggleEvent != null) {
                    toggleEvent.Cancel();
                    toggleEvent = null;
                }

                checkBoxToggle.Checked = false;
            }
        }

        private void TestLayoutObject_Load(object? sender, EventArgs e) {
            Dispatch.AddObjectInstanceDispatcherTargets(this);
            EventManager.AddObjectSubscriptions(this);
        }

        private void TestLayoutObject_FormClosed(object? sender, FormClosedEventArgs e) {
            LayoutController.Instance.EndDesignTimeActivation();
            Dispatch.Call.DeselectControlObjects(frameWindowId);
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void RadioButtonOn_Click(object? sender, EventArgs e) {
            StopToggle();
            State = 1;
        }

        private void RadioButtonOff_Click(object? sender, EventArgs e) {
            StopToggle();
            State = 0;
        }

        private void CheckBoxToggle_CheckedChanged(object? sender, EventArgs e) {
            if (checkBoxToggle.Checked) {
                if (toggleEvent == null) {
                    toggleEvent = EventManager.DelayedEvent((int)numericUpDownToggleTime.Value * 1000, new LayoutEvent("test-layout-object-toggle", this));
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
        private void TestLayoutObjectToggle(LayoutEvent e) {
            if (e.Sender == this) {
                State = 1 - State;

                if (checkBoxToggle.Checked)
                    toggleEvent = EventManager.DelayedEvent((int)numericUpDownToggleTime.Value * 1000, new LayoutEvent("test-layout-object-toggle", this));
            }
        }

        [DispatchTarget]
        private bool QueryTestLayoutObject() => true;

        private void PanelIllustration_Paint(object? sender, PaintEventArgs e) {
            Size componentSize = new(58, 58);

            if (component is LayoutTurnoutTrackComponent turnout) {
                LayoutComponentConnectionPoint switchPosition = LayoutComponentConnectionPoint.Empty;

                if (state == 0)
                    switchPosition = turnout.Straight;
                else if (state == 1)
                    switchPosition = turnout.Branch;

                e.Graphics.TranslateTransform(2, 2);

                LayoutTurnoutTrackPainter painter = new(componentSize, turnout.Tip, turnout.Straight, turnout.Branch, switchPosition) {
                    TrackWidth = 6
                };
                painter.Paint(e.Graphics);
            }
            else if (component is LayoutSignalComponent signal) {
                Graphics g = e.Graphics;

                if (signal.Info.SignalType == LayoutSignalType.Lights) {
                    int topMargin = 5;
                    int lightSize = 14;
                    Size signalFrameSize = new(30, 46);
                    Rectangle signalFrame = new(new Point((panelIllustration.Width - signalFrameSize.Width) / 2, topMargin), signalFrameSize);
                    Rectangle redLightBbox = new(new Point((panelIllustration.Width - lightSize) / 2, topMargin + (signalFrameSize.Height / 4) - (lightSize / 2)), new Size(lightSize, lightSize));
                    Rectangle greenLightBbox = new(new Point((panelIllustration.Width - lightSize) / 2, topMargin + (3 * signalFrameSize.Height / 4) - (lightSize / 2)), new Size(lightSize, lightSize));

                    using (Pen pollPen = new(Color.Black, 6))
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

                    using (Pen pollPen = new(Color.Black, 8))
                        g.DrawLine(pollPen, new Point(panelIllustration.Width / 2, pollTopMargin), new Point(panelIllustration.Width / 2, panelIllustration.Height));

                    g.TranslateTransform(panelIllustration.Width / 2, semaphoreJoin);

                    if (state == 0)
                        g.RotateTransform(-45);

                    Rectangle semaphoreRect = new(new Point(-12, 0), new Size(34, 6));
                    g.FillRectangle(Brushes.Brown, semaphoreRect);
                    g.DrawRectangle(Pens.Black, semaphoreRect);
                }
            }
            else if (component is LayoutDoubleSlipTrackComponent doubleSlip) {
                LayoutDoubleSlipPainter painter = new(componentSize, doubleSlip.DiagonalIndex, state);

                e.Graphics.TranslateTransform(2, 2);

                painter.TrackWidth = 6;
                painter.Paint(e.Graphics);
            }
        }

        private void CheckBoxReverseLogic_CheckedChanged(object? sender, EventArgs e) {
            if (component != null) {
                ModelComponent theComponent = (ModelComponent)component;
                LayoutXmlInfo xmlInfo = new(theComponent);

                xmlInfo.DocumentElement.SetAttributeValue(A_ReverseLogic, checkBoxReverseLogic.Checked);

                LayoutModifyComponentDocumentCommand modifyDocCommand = new(theComponent, xmlInfo);

                LayoutController.Do(modifyDocCommand);
                State = -1;     // Force user to select command
                panelIllustration.Invalidate();
            }
        }

        private void ButtonPassed_Click(object? sender, EventArgs e) {
            if (connectionPointRef != null && connectionPointRef.IsDefined && connectionPointRef.ConnectionPoint.UserActionRequired) {
                SetControlUserActionRequiredCommand setCommand = new(connectionPointRef.ConnectionPoint, false);

                LayoutController.Do(setCommand);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonFailed_Click(object? sender, EventArgs e) {
            if (connectionPointRef != null) {
                SetControlUserActionRequiredCommand setCommand = new(connectionPointRef.ConnectionPoint, true);

                LayoutController.Do(setCommand);
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonConnect_Click(object? sender, EventArgs e) {
            if (connectionPointRef == null)
                return;

            if (component == null) {
                CommandStationInputEvent csEvent;

                if (connectionPointRef.Module.Bus.BusType.AddressingMethod == ControlAddressingMethod.DirectConnectionPointAddressing)
                    csEvent = new CommandStationInputEvent((ModelComponent)connectionPointRef.Module.Bus.BusProvider, connectionPointRef.Module.Bus, connectionPointRef.Module.Address + connectionPointRef.Index);
                else
                    csEvent = new CommandStationInputEvent((ModelComponent)connectionPointRef.Module.Bus.BusProvider, connectionPointRef.Module.Bus, connectionPointRef.Module.Address, connectionPointRef.Index, 0);

                PickComponentToConnectToAddress pickDialog = new(csEvent);

                new SemiModalDialog(this, pickDialog, (dialog, info) => {
                    if (pickDialog.DialogResult == DialogResult.OK) {
                        ControlConnectionPoint result = Ensure.NotNull<ControlConnectionPoint>(EventManager.Event(new LayoutEvent("connect-component-to-control-module-address-request", pickDialog.ConnectionDestination, csEvent)));

                        Dispatch.Call.ShowControlConnectionPoint(frameWindowId, connectionPointRef);
                        this.component = result.Component;
                        Initialize();
                        panelIllustration.Invalidate();
                    }
                }, null).ShowDialog();
            }
            else {
                DisconnectComponentFromConnectionPointCommand disconnectCommand = new(connectionPointRef.ConnectionPoint);

                LayoutController.Do(disconnectCommand);
                component = null;
                Initialize();
            }
        }
    }
}
