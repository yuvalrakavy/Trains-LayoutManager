using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for AddLocomotiveToTrain.
    /// </summary>
    public class AddLocomotiveToTrain : Form {
        private ListBox listBoxLocomotives;
        private GroupBox groupBox1;
        private RadioButton radioButtonOrientationForward;
        private RadioButton radioButtonOrientationBackward;
        private Button buttonAdd;
        private Button buttonCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

#pragma warning disable IDE0051 // Remove unused private members
        private void endOfDesignerVariables() { }
#pragma warning restore IDE0051 // Remove unused private members

        readonly TrainCommonInfo train;

        public AddLocomotiveToTrain(TrainCommonInfo train) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.train = train;

            radioButtonOrientationForward.Checked = true;

            updateLocomotiveList();
            updateButtons(null, null);
        }

        public LocomotiveOrientation Orientation {
            get {
                if (radioButtonOrientationForward.Checked)
                    return LocomotiveOrientation.Forward;
                else
                    return LocomotiveOrientation.Backward;
            }
        }

        public LocomotiveInfo Locomotive => (LocomotiveInfo)listBoxLocomotives.SelectedItem;

        private void updateButtons(Object sender, EventArgs e) {
            buttonAdd.Enabled = listBoxLocomotives.SelectedItem != null;
        }

        private void updateLocomotiveList(Object sender, EventArgs e) {
            updateLocomotiveList();
        }

        private bool isValidLoco(LocomotiveInfo loco, LocomotiveOrientation orientation) {
            foreach (TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
                if (trainLocomotive.LocomotiveId == loco.Id)
                    return false;

                if (trainLocomotive.Locomotive.Guage != loco.Guage)
                    return false;

                if (loco.Kind == LocomotiveKind.SoundUnit)
                    continue;

                if (loco.AddressProvider.Element != null && trainLocomotive.Locomotive.AddressProvider.Element != null &&
                    loco.AddressProvider.Unit == trainLocomotive.Locomotive.AddressProvider.Unit && trainLocomotive.Orientation != orientation)
                    return false;
            }

            return true;
        }

        private void updateLocomotiveList() {
            IDictionary inList = new HybridDictionary();
            LocomotiveOrientation orientation;

            if (radioButtonOrientationBackward.Checked)
                orientation = LocomotiveOrientation.Backward;
            else
                orientation = LocomotiveOrientation.Forward;

            ArrayList removeList = new ArrayList();

            foreach (LocomotiveInfo loco in listBoxLocomotives.Items) {
                // Check if loco is valid
                bool retainInList = true;

                inList.Add(loco.Id, loco);

                if (train is TrainStateInfo) {
                    CanPlaceTrainResult result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent("is-locomotive-address-valid", loco.Element, train, "<Orientation Value='" + orientation.ToString() + "' />"));

                    if (!result.CanBeResolved)
                        retainInList = false;
                }
                else {
                    if (!isValidLoco(loco, orientation))
                        retainInList = false;
                }

                if (!retainInList)
                    removeList.Add(loco);
            }

            foreach (LocomotiveInfo loco in removeList)
                listBoxLocomotives.Items.Remove(loco);

            // Now pass on the collection and check each locomotive that was not in the list
            foreach (XmlElement collectionElement in LayoutModel.LocomotiveCollection.CollectionElement.SelectNodes("Locomotive")) {
                bool validLoco = true;
                LocomotiveInfo loco = new LocomotiveInfo(collectionElement);

                if (!inList.Contains(loco.Id)) {
                    if (train is TrainStateInfo) {
                        var result = EventManager.Event<XmlElement, object, CanPlaceTrainResult>("can-locomotive-be-placed-on-track", loco.Element);

                        if (result.Status == CanPlaceTrainStatus.CanPlaceTrain)
                            result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent(
                                "is-locomotive-address-valid", loco.Element, train, xmlDocument: "<Orientation Value='" + orientation.ToString() + "' />"));

                        if (result.Status != CanPlaceTrainStatus.CanPlaceTrain)
                            validLoco = false;
                    }
                    else {
                        if (!isValidLoco(loco, orientation))
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBoxLocomotives = new ListBox();
            this.groupBox1 = new GroupBox();
            this.radioButtonOrientationForward = new RadioButton();
            this.radioButtonOrientationBackward = new RadioButton();
            this.buttonAdd = new Button();
            this.buttonCancel = new Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxLocomotives
            // 
            this.listBoxLocomotives.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.listBoxLocomotives.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBoxLocomotives.ItemHeight = 46;
            this.listBoxLocomotives.Location = new System.Drawing.Point(8, 8);
            this.listBoxLocomotives.Name = "listBoxLocomotives";
            this.listBoxLocomotives.Size = new System.Drawing.Size(152, 234);
            this.listBoxLocomotives.TabIndex = 0;
            this.listBoxLocomotives.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxLocomotives_DrawItem);
            this.listBoxLocomotives.SelectedIndexChanged += new System.EventHandler(this.updateButtons);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.radioButtonOrientationForward,
                                                                                    this.radioButtonOrientationBackward});
            this.groupBox1.Location = new System.Drawing.Point(166, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(104, 56);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orientation";
            // 
            // radioButtonOrientationForward
            // 
            this.radioButtonOrientationForward.Location = new System.Drawing.Point(8, 15);
            this.radioButtonOrientationForward.Name = "radioButtonOrientationForward";
            this.radioButtonOrientationForward.Size = new System.Drawing.Size(80, 16);
            this.radioButtonOrientationForward.TabIndex = 0;
            this.radioButtonOrientationForward.Text = "Forward";
            this.radioButtonOrientationForward.CheckedChanged += new System.EventHandler(this.updateLocomotiveList);
            // 
            // radioButtonOrientationBackward
            // 
            this.radioButtonOrientationBackward.Location = new System.Drawing.Point(8, 35);
            this.radioButtonOrientationBackward.Name = "radioButtonOrientationBackward";
            this.radioButtonOrientationBackward.Size = new System.Drawing.Size(80, 16);
            this.radioButtonOrientationBackward.TabIndex = 1;
            this.radioButtonOrientationBackward.Text = "Backward";
            this.radioButtonOrientationBackward.CheckedChanged += new System.EventHandler(this.updateLocomotiveList);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonAdd.Location = new System.Drawing.Point(195, 192);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(195, 224);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "&Cancel";
            // 
            // AddLocomotiveToTrain
            // 
            this.AcceptButton = this.buttonAdd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(276, 266);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonAdd,
                                                                          this.groupBox1,
                                                                          this.listBoxLocomotives,
                                                                          this.buttonCancel});
            this.Name = "AddLocomotiveToTrain";
            this.ShowInTaskbar = false;
            this.Text = "Add Locomotive";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            if (listBoxLocomotives.SelectedIndex == -1) {
                MessageBox.Show(this, "You did not select a locomotive to add", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void listBoxLocomotives_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
            int xText;
            float yText;
            LocomotiveInfo loco = (LocomotiveInfo)listBoxLocomotives.Items[e.Index];

            e.DrawBackground();

            GraphicsState gs = e.Graphics.Save();

            e.Graphics.TranslateTransform(e.Bounds.Left, e.Bounds.Top);

            using (LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(LayoutModel.LocomotiveCatalog)) {
                locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
            };

            yText = 2;
            xText = 55;

            using (Brush textBrush = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.WindowText)) {
                SizeF textSize;

                using (Font titleFont = new Font("Arial", 8, FontStyle.Bold)) {
                    textSize = e.Graphics.MeasureString(loco.DisplayName, titleFont);
                    e.Graphics.DrawString(loco.DisplayName, titleFont, textBrush, new PointF(xText, yText));
                    yText += textSize.Height;
                }

                if (loco.TypeName != null) {
                    using (Font typeFont = new Font("Arial", 7, FontStyle.Regular)) {
                        string typeText = " (" + loco.TypeName + ")";
                        SizeF typeSize = e.Graphics.MeasureString(typeText, typeFont);

                        e.Graphics.DrawString(loco.TypeName, typeFont, textBrush, new PointF(xText, yText));
                    }
                }
            }

            e.Graphics.Restore(gs);

            using (Pen p = new Pen(Color.Black, 2.0F))
                e.Graphics.DrawLine(p, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Bottom);

            e.DrawFocusRectangle();
        }
    }
}
