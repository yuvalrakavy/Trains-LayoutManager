using LayoutManager.CommonUI.Controls;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for AddLocomotiveToTrain.
    /// </summary>
    public partial class AddLocomotiveToTrain : Form {

        private readonly TrainCommonInfo train;

        public AddLocomotiveToTrain(TrainCommonInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;

            radioButtonOrientationForward.Checked = true;

            UpdateLocomotiveList();
            UpdateButtons(null, EventArgs.Empty);
        }

        public LocomotiveOrientation Orientation => radioButtonOrientationForward.Checked ? LocomotiveOrientation.Forward : LocomotiveOrientation.Backward;

        public LocomotiveInfo Locomotive => (LocomotiveInfo)listBoxLocomotives.SelectedItem;

        private void UpdateButtons(Object? sender, EventArgs e) {
            buttonAdd.Enabled = listBoxLocomotives.SelectedItem != null;
        }

        private void UpdateLocomotiveList(object? sender, EventArgs e) {
            UpdateLocomotiveList();
        }

        private bool IsValidLoco(LocomotiveInfo loco, LocomotiveOrientation orientation) {
            foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                if (trainLocomotive.LocomotiveId == loco.Id)
                    return false;

                if (trainLocomotive.Locomotive.Guage != loco.Guage)
                    return false;

                if (loco.Kind == LocomotiveKind.SoundUnit)
                    continue;

                if (loco.AddressProvider.OptionalElement != null && trainLocomotive.Locomotive.AddressProvider.OptionalElement != null &&
                    loco.AddressProvider.Unit == trainLocomotive.Locomotive.AddressProvider.Unit && trainLocomotive.Orientation != orientation)
                    return false;
            }

            return true;
        }

        private void UpdateLocomotiveList() {
            IDictionary inList = new HybridDictionary();
            LocomotiveOrientation orientation;

            if (radioButtonOrientationBackward.Checked)
                orientation = LocomotiveOrientation.Backward;
            else
                orientation = LocomotiveOrientation.Forward;

            ArrayList removeList = new();

            foreach (LocomotiveInfo loco in listBoxLocomotives.Items) {
                // Check if loco is valid
                bool retainInList = true;

                inList.Add(loco.Id, loco);

                if (train is TrainStateInfo) {
                    var result = Dispatch.Call.IsLocomotiveAddressValid(loco.Element, train, new IsLocomotiveAddressValidSettings { Orientation = orientation });

                    if (!result.CanBeResolved)
                        retainInList = false;
                }
                else {
                    if (!IsValidLoco(loco, orientation))
                        retainInList = false;
                }

                if (!retainInList)
                    removeList.Add(loco);
            }

            foreach (LocomotiveInfo loco in removeList)
                listBoxLocomotives.Items.Remove(loco);

            // Now pass on the collection and check each locomotive that was not in the list
            foreach (XmlElement collectionElement in LayoutModel.LocomotiveCollection.CollectionElement.SelectNodes("Locomotive")!) {
                bool validLoco = true;
                LocomotiveInfo loco = new(collectionElement);

                if (!inList.Contains(loco.Id)) {
                    if (train is TrainStateInfo) {
                        var result = Dispatch.Call.CanLocomotiveBePlacedOnTrack(loco.Element);

                        if (result.Status == CanPlaceTrainStatus.CanPlaceTrain)
                            result = Dispatch.Call.IsLocomotiveAddressValid(loco.Element, train, new IsLocomotiveAddressValidSettings { Orientation = orientation });

                        if (result.Status != CanPlaceTrainStatus.CanPlaceTrain)
                            validLoco = false;
                    }
                    else {
                        if (!IsValidLoco(loco, orientation))
                            validLoco = false;
                    }

                    if (validLoco)
                        listBoxLocomotives.Items.Add(loco);
                }
            }
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

        private void ButtonAdd_Click(object? sender, System.EventArgs e) {
            if (listBoxLocomotives.SelectedIndex == -1) {
                MessageBox.Show(this, "You did not select a locomotive to add", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void ListBoxLocomotives_DrawItem(object? sender, System.Windows.Forms.DrawItemEventArgs e) {
            int xText;
            float yText;
            LocomotiveInfo loco = (LocomotiveInfo)listBoxLocomotives.Items[e.Index];

            e.DrawBackground();

            GraphicsState gs = e.Graphics.Save();

            e.Graphics.TranslateTransform(e.Bounds.Left, e.Bounds.Top);

            using (LocomotiveImagePainter locoPainter = new(LayoutModel.LocomotiveCatalog)) {
                locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
            }

            yText = 2;
            xText = 55;

            using (Brush textBrush = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.WindowText)) {
                SizeF textSize;

                using (Font titleFont = new("Arial", 8, FontStyle.Bold)) {
                    textSize = e.Graphics.MeasureString(loco.DisplayName, titleFont);
                    e.Graphics.DrawString(loco.DisplayName, titleFont, textBrush, new PointF(xText, yText));
                    yText += textSize.Height;
                }

                if (loco.TypeName != null) {
                    using Font typeFont = new("Arial", 7, FontStyle.Regular);
                    string typeText = " (" + loco.TypeName + ")";
                    SizeF typeSize = e.Graphics.MeasureString(typeText, typeFont);

                    e.Graphics.DrawString(loco.TypeName, typeFont, textBrush, new PointF(xText, yText));
                }
            }

            e.Graphics.Restore(gs);

            using (Pen p = new(Color.Black, 2.0F))
                e.Graphics.DrawLine(p, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Bottom);

            e.DrawFocusRectangle();
        }
    }
}
