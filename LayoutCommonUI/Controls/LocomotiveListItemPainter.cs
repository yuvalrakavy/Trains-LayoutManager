using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    public static class LocomotiveListItemPainter {
        private const string A_OnTrack = "OnTrack";
        private const string A_CanPlaceOnTrack = "CanPlaceOnTrack";

        public static void Measure(MeasureItemEventArgs e, XmlElement element) {
            if (element.Name == "Train")
                e.ItemHeight = 48;
            else
                e.ItemHeight = 46;
        }

        public static void Draw(DrawItemEventArgs e, XmlElement element, LocomotiveCatalogInfo catalog, bool annotateForOperationMode) {
            bool onTrack = false;
            CanPlaceTrainResolveMethod canPlaceOnTrack = CanPlaceTrainResolveMethod.Resolved;

            if (annotateForOperationMode) {
                onTrack = (bool)element.AttributeValue(A_OnTrack);
                if (!onTrack) {
                    if (!Enum.TryParse<CanPlaceTrainResolveMethod>(element.GetAttribute(A_CanPlaceOnTrack), out canPlaceOnTrack))
                        canPlaceOnTrack = CanPlaceTrainResolveMethod.NotPossible;
                }
            }

            if (!annotateForOperationMode || (e.State & DrawItemState.Selected) != 0)
                e.DrawBackground();
            else {
                if (onTrack)
                    e.Graphics.FillRectangle(Brushes.Yellow, e.Bounds);
                else {
                    e.DrawBackground();
                }
            }

            e.DrawFocusRectangle();

            GraphicsState gs = e.Graphics.Save();
            Color textColor;

            e.Graphics.TranslateTransform(e.Bounds.Left, e.Bounds.Top);

            if ((e.State & DrawItemState.Selected) != 0)
                textColor = SystemColors.HighlightText;
            else {
                if (!annotateForOperationMode || onTrack)
                    textColor = SystemColors.WindowText;
                else {
                    switch (canPlaceOnTrack) {
                        case CanPlaceTrainResolveMethod.NotPossible:
                            textColor = SystemColors.GrayText;
                            break;

                        case CanPlaceTrainResolveMethod.ReprogramAddress:
                            textColor = Color.Blue;
                            break;

                        default:
                            textColor = SystemColors.WindowText;
                            break;
                    }
                }
            }

            using Brush textBrush = new SolidBrush(textColor);
            // Draw the locomotive image and name. There are two possible layouts one
            // for locomotive and one for locomotive set
            int xText;
            float yText;
            string name;
            string typeName = null;

            if (element.Name == "Locomotive") {
                LocomotiveInfo loco = new LocomotiveInfo(element);

                using (LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(catalog)) {
                    locoPainter.FlipImage = true;   //DEBUG
                    locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
                }

                yText = 2;
                xText = 55;
                name = loco.DisplayName;

                typeName = loco.TypeName;
            }
            else if (element.Name == "Train") {
                TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(element);

                using (LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(catalog)) {
                    int x = 2;

                    locoPainter.FrameSize = new Size(28, 20);

                    foreach (TrainLocomotiveInfo trainLocomotive in trainInCollection.Locomotives) {
                        locoPainter.LocomotiveElement = trainLocomotive.Locomotive.Element;
                        locoPainter.FlipImage = (trainLocomotive.Orientation == LocomotiveOrientation.Backward);
                        locoPainter.Origin = new Point(x, 2);
                        locoPainter.Draw(e.Graphics);

                        x += locoPainter.FrameSize.Width + 2;
                    }

                    using Font f = new Font("Arial", 8);
                    e.Graphics.DrawString("(" + trainInCollection.Length.ToDisplayString(true) + ")", f, textBrush, new Point(x, 4));
                }

                xText = 2;
                yText = 22;
                name = trainInCollection.DisplayName;
            }
            else
                throw new ApplicationException("Invalid element");

            SizeF textSize;

            using (Font titleFont = new Font("Arial", 8, FontStyle.Bold)) {
                textSize = e.Graphics.MeasureString(name, titleFont);
                e.Graphics.DrawString(name, titleFont, textBrush, new PointF(xText, yText));
            }

            if (typeName != null) {
                using Font typeFont = new Font("Arial", 7, FontStyle.Regular);
                string typeText = " (" + typeName + ")";
                SizeF typeSize = e.Graphics.MeasureString(typeText, typeFont);

                e.Graphics.DrawString(typeText, typeFont, textBrush,
                    new PointF(xText + textSize.Width, yText + textSize.Height - typeSize.Height));
            }

            yText += textSize.Height;

            if (annotateForOperationMode) {
                string status;
                TrainStateInfo train = LayoutModel.StateManager.Trains[element];

                if (onTrack && train != null)
                    status = train.StatusText;
                else {
                    if (canPlaceOnTrack == CanPlaceTrainResolveMethod.Resolved)
                        status = "Can be placed on track";
                    else
                        status = element.GetAttribute("Reason");
                }

                using (Font typeFont = new Font("Arial", 6.5F, FontStyle.Regular)) {
                    textSize = e.Graphics.MeasureString(status, typeFont);
                    e.Graphics.DrawString(status, typeFont, textBrush, new PointF(xText, yText));
                }

                yText += textSize.Height;
            }

            e.Graphics.Restore(gs);

            using Pen p = new Pen(Color.Black, 2.0F);
            e.Graphics.DrawLine(p, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Bottom);
        }
    }
}