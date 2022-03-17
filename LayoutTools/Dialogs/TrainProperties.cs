using LayoutManager.CommonUI.Controls;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrainProperties.
    /// </summary>
    public partial class TrainProperties : Form, IObjectHasXml {

        private readonly TrainCommonInfo train;
        private readonly ArrayList locomotiveCancelList = new();
        private bool locomotiveEdited = false;

        public TrainProperties(TrainCommonInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;

            trainLengthDiagram.Length = train.Length;
            trainDriverComboBox.Train = train;

            foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives)
                locomotiveCancelList.Add(new TrainLocomotiveCancelInfo(trainLocomotive));

            SetTitleBar();
            textBoxName.Text = train.Name;

            checkBoxMagnetOnLastCar.Checked = train.LastCarTriggerBlockEdge;
            checkBoxLastCarDetected.Checked = train.LastCarDetectedByOccupancyBlock;

            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives)
                listViewLocomotives.Items.Add(new TrainLocomotiveItem(trainLoco));

            drivingParameters.Element = train.Element;

            attributesEditor.AttributesSource = typeof(TrainStateInfo);
            attributesEditor.AttributesOwner = train;

            UpdateControls();

            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        public XmlElement Element => train.Element;
        public XmlElement? OptionalElement => Element;

        private void UpdateControls(Object? sender, EventArgs e) {
            UpdateControls();
        }

        private void UpdateControls() {
            if (listViewLocomotives.SelectedItems.Count != 0) {
                buttonLocoEdit.Enabled = true;
                buttonLocoRemove.Enabled = true;
                buttonLocoMoveUp.Enabled = listViewLocomotives.SelectedIndices[0] > 0;
                buttonLocoMoveDown.Enabled = listViewLocomotives.SelectedIndices[0] < listViewLocomotives.Items.Count - 1;
            }
            else {
                buttonLocoEdit.Enabled = false;
                buttonLocoRemove.Enabled = false;
                buttonLocoMoveDown.Enabled = false;
                buttonLocoMoveUp.Enabled = false;
            }

            panelLocoImages.Invalidate();
        }

        private void SetTitleBar() {
            this.Text = "Train " + train.DisplayName + " Properties";
        }

        [DispatchTarget]
        private void OnTrainNameChanged([DispatchFilter("IsMyId")] TrainCommonInfo train) {
            SetTitleBar();
        }

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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            // Validate

            if (textBoxName.Text.Trim()?.Length == 0) {
                tabControl1.SelectedTab = tabPageGeneral;
                MessageBox.Show(this, "Train name cannot be empty", "Missing value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxName.Focus();
                return;
            }

            if (!trainDriverComboBox.ValidateInput()) {
                tabControl1.SelectedTab = tabPageGeneral;
                trainDriverComboBox.Focus();
            }

            if (!drivingParameters.ValidateValues())
                return;

            // Commit

            if (textBoxName.Text != train.Name)
                train.Name = textBoxName.Text;

            train.Length = trainLengthDiagram.Length;
            train.LastCarTriggerBlockEdge = checkBoxMagnetOnLastCar.Checked;
            train.LastCarDetectedByOccupancyBlock = checkBoxLastCarDetected.Checked;

            trainDriverComboBox.Commit();
            drivingParameters.Commit();
            attributesEditor.Commit();

            Dispatch.Notification.OnTrainConfigurationChanged(train);
            train.RefreshSpeedLimit();
            train.Redraw();

            DialogResult = DialogResult.OK;
        }

        private void TrainProperties_Closed(object? sender, System.EventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void ButtonCancel_Click(object? sender, System.EventArgs e) {
            if (locomotiveEdited) {
                try {
                    // Undo locomotive collection editing. Remove locomotives, and re-insert original list
                    foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives)
                        train.RemoveLocomotive(trainLocomotive);

                    foreach (TrainLocomotiveCancelInfo trainLocoCancelInfo in locomotiveCancelList) {
                        XmlElement? locoElement = LayoutModel.LocomotiveCollection[trainLocoCancelInfo.LocomotiveID];

                        if (locoElement != null) {
                            CanPlaceTrainResult result = train.AddLocomotive(new LocomotiveInfo(locoElement), trainLocoCancelInfo.Orientation, null, false);

                            if (result.Status != CanPlaceTrainStatus.CanPlaceTrain)
                                throw new LayoutException(result.ToString());
                        }
                    }
                }
                catch (LayoutException lex) {
                    lex.Report();
                }
            }
        }

        private void ButtonLocoEdit_Click(object? sender, System.EventArgs e) {
            contextMenuEditLocomotive.Show(tabPageLocomotives, new Point(buttonLocoEdit.Left, buttonLocoEdit.Bottom));
        }

        private void SetLocomotiveOrientation(LocomotiveOrientation orientation) {
            TrainLocomotiveItem selected = (TrainLocomotiveItem)listViewLocomotives.SelectedItems[0];
            TrainLocomotiveInfo selectedTrainLoco = selected.TrainLocomotive;

            // Verify that it is a valid setting
            foreach (TrainLocomotiveInfo trainLoco in train.Locomotives) {
                if (trainLoco.LocomotiveId != selectedTrainLoco.LocomotiveId) {
                    if (trainLoco.Locomotive.AddressProvider.Unit == selectedTrainLoco.Locomotive.AddressProvider.Unit &&
                        trainLoco.Orientation != orientation) {
                        MessageBox.Show(this, "You cannot change locomotive orientation. This train already has another locomotive with the same address. " +
                            "All locomotives with a given address must have the same orientation", "Cannot Change Orientation",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            selectedTrainLoco.Orientation = orientation;
            selected.Update();
            locomotiveEdited = true;
        }

        private void MenuItemLocoOrientationForward_Click(object? sender, System.EventArgs e) {
            SetLocomotiveOrientation(LocomotiveOrientation.Forward);
        }

        private void MenuItemLocoOrientationBackward_Click(object? sender, System.EventArgs e) {
            SetLocomotiveOrientation(LocomotiveOrientation.Backward);
        }

        private void MenuItemEditLocoDefinition_Click(object? sender, System.EventArgs e) {
            TrainLocomotiveItem selected = (TrainLocomotiveItem)listViewLocomotives.SelectedItems[0];
            TrainLocomotiveInfo trainLoco = selected.TrainLocomotive;

            Dispatch.Call.EditLocomotiveProperties(trainLoco.Locomotive);
            locomotiveEdited = true;
            train.FlushCachedValues();
            selected.Update();
            UpdateControls();
        }

        private void ButtonLocoRemove_Click(object? sender, System.EventArgs e) {
            TrainLocomotiveItem selected = (TrainLocomotiveItem)listViewLocomotives.SelectedItems[0];
            TrainLocomotiveInfo trainLoco = selected.TrainLocomotive;

            train.RemoveLocomotive(trainLoco);
            listViewLocomotives.Items.Remove(selected);
            locomotiveEdited = true;
        }

        private void ButtonLocoAdd_Click(object? sender, System.EventArgs e) {
            var addLoco = new Dialogs.AddLocomotiveToTrain(train);

            if (addLoco.ShowDialog() == DialogResult.OK) {
                CanPlaceTrainResult result = train.AddLocomotive(addLoco.Locomotive, addLoco.Orientation, null, true);

                if (result.Status == CanPlaceTrainStatus.CanPlaceTrain) {
                    TrainLocomotiveInfo trainLoco = result.TrainLocomotive;

                    listViewLocomotives.Items.Add(new TrainLocomotiveItem(trainLoco));
                    locomotiveEdited = true;
                    UpdateControls();
                }
            }
        }

        private void ButtonLocoMoveDown_Click(object? sender, System.EventArgs e) {
            int selectedIndex = listViewLocomotives.SelectedIndices[0];

            if (selectedIndex < listViewLocomotives.Items.Count - 1) {
                TrainLocomotiveItem selected = (TrainLocomotiveItem)listViewLocomotives.SelectedItems[0];
                TrainLocomotiveInfo trainLoco = selected.TrainLocomotive;

                var nextElement = trainLoco.Element.NextSibling;
                trainLoco.Element.ParentNode?.RemoveChild(trainLoco.Element);
                nextElement?.ParentNode?.InsertAfter(trainLoco.Element, nextElement);

                listViewLocomotives.Items.Remove(selected);
                listViewLocomotives.Items.Insert(selectedIndex + 1, selected);
                locomotiveEdited = true;
            }
        }

        private void ButtonLocoMoveUp_Click(object? sender, System.EventArgs e) {
            int selectedIndex = listViewLocomotives.SelectedIndices[0];

            if (selectedIndex > 0) {
                TrainLocomotiveItem selected = (TrainLocomotiveItem)listViewLocomotives.SelectedItems[0];
                TrainLocomotiveInfo trainLoco = selected.TrainLocomotive;

                var previousElement = trainLoco.Element.PreviousSibling;
                trainLoco.Element.ParentNode?.RemoveChild(trainLoco.Element);
                previousElement?.ParentNode?.InsertBefore(trainLoco.Element, previousElement);

                listViewLocomotives.Items.Remove(selected);
                listViewLocomotives.Items.Insert(selectedIndex - 1, selected);
                locomotiveEdited = true;
            }
        }

        private int ImageWidth => (panelLocoImages.Height - 8) * 50 / 36;

        private void PanelLocoImages_Paint(object? sender, System.Windows.Forms.PaintEventArgs e) {
            int x = 2;
            int w = ImageWidth;
            LocomotiveImagePainter locoPainter = new() {
                FrameSize = new Size(w, panelLocoImages.Height - 8)
            };

            foreach (TrainLocomotiveItem item in listViewLocomotives.Items) {
                TrainLocomotiveInfo trainLoco = item.TrainLocomotive;

                locoPainter.Origin = new Point(x, 2);
                locoPainter.LocomotiveElement = trainLoco.Locomotive.Element;

                locoPainter.FlipImage = trainLoco.Orientation == LocomotiveOrientation.Backward;

                if (item.Selected)
                    locoPainter.FramePen = new Pen(Color.Red, 2.0F);
                else
                    locoPainter.FramePen = new Pen(Color.Black);

                locoPainter.Draw(e.Graphics);
                x += w + 4;
            }

            locoPainter.Dispose();
        }

        private void PanelLocoImages_MouseDown(object? sender, System.Windows.Forms.MouseEventArgs e) {
            int iImage = (e.X - 2) / ImageWidth;

            if (iImage < listViewLocomotives.Items.Count)
                listViewLocomotives.Items[iImage].Selected = true;
        }

        private class TrainLocomotiveItem : ListViewItem {
            public TrainLocomotiveItem(TrainLocomotiveInfo trainLocomotive) {
                LocomotiveInfo loco = trainLocomotive.Locomotive;

                this.TrainLocomotive = trainLocomotive;

                Text = loco.DisplayName;
                SubItems.Add(loco.AddressProvider.Unit.ToString());
                SubItems.Add(trainLocomotive.Orientation.ToString());
                SubItems.Add(loco.TypeName);
            }

            public void Update() {
                LocomotiveInfo loco = TrainLocomotive.Locomotive;

                SubItems[0].Text = loco.DisplayName;
                SubItems[1].Text = loco.AddressProvider.Unit.ToString();
                SubItems[2].Text = TrainLocomotive.Orientation.ToString();
                SubItems[3].Text = loco.TypeName;
            }

            public TrainLocomotiveInfo TrainLocomotive { get; }
        }

        private class TrainLocomotiveCancelInfo {
            public Guid LocomotiveID;
            public LocomotiveOrientation Orientation;

            public TrainLocomotiveCancelInfo(TrainLocomotiveInfo trainLocomotive) {
                this.LocomotiveID = trainLocomotive.LocomotiveId;
                this.Orientation = trainLocomotive.Orientation;
            }
        }
    }
}
