using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ImageProperties.
    /// </summary>
    public class ImageProperties : Form, ILayoutComponentPropertiesDialog {
        private Label label1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private GroupBox groupBox4;
        private Button buttonOK;
        private Button buttonCancel;
        private TextBox textBoxImageFile;
        private Button buttonBrowse;
        private RadioButton radioButtonWidthOriginal;
        private RadioButton radioButtonWidthSet;
        private NumericUpDown numericUpDownWidth;
        private NumericUpDown numericUpDownHeight;
        private RadioButton radioButtonHeightSet;
        private RadioButton radioButtonHeightOriginal;
        private NumericUpDown numericUpDownHorizontalOffset;
        private NumericUpDown numericUpDownVerticalOffset;
        private RadioButton radioButtonEffectStretch;
        private RadioButton radioButtonEffectTile;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuHorizontalUnits;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuVerticalUnits;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuOrigin;
        private Label label6;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuVerticalAlignment;
        private Label label7;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuHorizontalAlignment;
        private Label label8;
        private LayoutManager.CommonUI.Controls.LinkMenu linkMenuRotateFlip;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void EndOfDesignerVariables() { }

        public ImageProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.XmlInfo = new LayoutXmlInfo(component);

            LayoutImageInfo image = new LayoutImageInfo(XmlInfo.Element);

            textBoxImageFile.Text = image.ImageFile;

            Size imageSize = image.Size;

            if (imageSize.Width < 0)
                radioButtonWidthOriginal.Checked = true;
            else {
                radioButtonWidthSet.Checked = true;
                numericUpDownWidth.Value = imageSize.Width;
            }

            linkMenuHorizontalUnits.SelectedIndex = (int)image.WidthSizeUnit;

            if (imageSize.Height < 0)
                radioButtonHeightOriginal.Checked = true;
            else {
                radioButtonHeightSet.Checked = true;
                numericUpDownHeight.Value = imageSize.Height;
            }

            linkMenuVerticalUnits.SelectedIndex = (int)image.HeightSizeUnit;

            Size offset = image.Offset;

            numericUpDownHorizontalOffset.Value = offset.Width;
            numericUpDownVerticalOffset.Value = offset.Height;
            linkMenuOrigin.SelectedIndex = (int)image.OriginMethod;
            linkMenuHorizontalAlignment.SelectedIndex = (int)image.HorizontalAlignment;
            linkMenuVerticalAlignment.SelectedIndex = (int)image.VerticalAlignment;

            if (image.FillEffect == LayoutImageInfo.ImageFillEffect.Stretch)
                radioButtonEffectStretch.Checked = true;
            else if (image.FillEffect == LayoutImageInfo.ImageFillEffect.Tile)
                radioButtonEffectTile.Checked = true;

            String[] rotateFlipEffects = Enum.GetNames(typeof(RotateFlipType));

            linkMenuRotateFlip.Options = rotateFlipEffects;

            for (int i = 0; i < rotateFlipEffects.Length; i++)
                if (rotateFlipEffects[i] == image.RotateFlipEffect.ToString()) {
                    linkMenuRotateFlip.SelectedIndex = i;
                    break;
                }

            updateButtons();
        }

        public LayoutXmlInfo XmlInfo { get; }

        private void updateButtons() {
            radioButtonHeightOriginal.Text = "Maintain original image aspect ratio";
            radioButtonWidthOriginal.Text = "Maintain original image aspect ratio";

            if (radioButtonHeightOriginal.Checked && radioButtonWidthOriginal.Checked) {
                radioButtonEffectStretch.Enabled = false;
                radioButtonEffectTile.Enabled = false;
            }
            else {
                radioButtonEffectStretch.Enabled = true;
                radioButtonEffectTile.Enabled = true;
            }

            if (radioButtonHeightOriginal.Checked && radioButtonWidthOriginal.Checked) {
                radioButtonHeightOriginal.Text = "Original image height";
                radioButtonWidthOriginal.Text = "Original image width";
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
            this.label1 = new Label();
            this.textBoxImageFile = new TextBox();
            this.buttonBrowse = new Button();
            this.groupBox1 = new GroupBox();
            this.linkMenuHorizontalUnits = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.numericUpDownWidth = new NumericUpDown();
            this.radioButtonWidthSet = new RadioButton();
            this.radioButtonWidthOriginal = new RadioButton();
            this.groupBox2 = new GroupBox();
            this.linkMenuVerticalUnits = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.numericUpDownHeight = new NumericUpDown();
            this.radioButtonHeightSet = new RadioButton();
            this.radioButtonHeightOriginal = new RadioButton();
            this.groupBox3 = new GroupBox();
            this.linkMenuVerticalAlignment = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.label6 = new Label();
            this.label5 = new Label();
            this.numericUpDownVerticalOffset = new NumericUpDown();
            this.label3 = new Label();
            this.numericUpDownHorizontalOffset = new NumericUpDown();
            this.label2 = new Label();
            this.linkMenuOrigin = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.label4 = new Label();
            this.label7 = new Label();
            this.linkMenuHorizontalAlignment = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.groupBox4 = new GroupBox();
            this.radioButtonEffectTile = new RadioButton();
            this.radioButtonEffectStretch = new RadioButton();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.label8 = new Label();
            this.linkMenuRotateFlip = new LayoutManager.CommonUI.Controls.LinkMenu();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownWidth).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownHeight).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownVerticalOffset).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownHorizontalOffset).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image file:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxImageFile
            // 
            this.textBoxImageFile.Location = new System.Drawing.Point(64, 14);
            this.textBoxImageFile.Name = "textBoxImageFile";
            this.textBoxImageFile.Size = new System.Drawing.Size(192, 20);
            this.textBoxImageFile.TabIndex = 1;
            this.textBoxImageFile.Text = "";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(263, 14);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(64, 20);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "&Browse...";
            this.buttonBrowse.Click += this.buttonBrowse_Click;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.linkMenuHorizontalUnits,
                                                                                    this.numericUpDownWidth,
                                                                                    this.radioButtonWidthSet,
                                                                                    this.radioButtonWidthOriginal});
            this.groupBox1.Location = new System.Drawing.Point(6, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 72);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Width:";
            // 
            // linkMenuHorizontalUnits
            // 
            this.linkMenuHorizontalUnits.Location = new System.Drawing.Point(176, 40);
            this.linkMenuHorizontalUnits.Name = "linkMenuHorizontalUnits";
            this.linkMenuHorizontalUnits.Options = new string[] {
                                                                    "pixels",
                                                                    "grid units"};
            this.linkMenuHorizontalUnits.SelectedIndex = 0;
            this.linkMenuHorizontalUnits.Size = new System.Drawing.Size(100, 20);
            this.linkMenuHorizontalUnits.TabIndex = 3;
            this.linkMenuHorizontalUnits.TabStop = true;
            this.linkMenuHorizontalUnits.Text = "pixels";
            this.linkMenuHorizontalUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuHorizontalUnits.Click += this.linkMenuHorizontalUnits_Click;
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Location = new System.Drawing.Point(104, 40);
            this.numericUpDownWidth.Maximum = new System.Decimal(new int[] {
                                                                               10000,
                                                                               0,
                                                                               0,
                                                                               0});
            this.numericUpDownWidth.Minimum = new System.Decimal(new int[] {
                                                                               1,
                                                                               0,
                                                                               0,
                                                                               0});
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(64, 20);
            this.numericUpDownWidth.TabIndex = 2;
            this.numericUpDownWidth.Value = new System.Decimal(new int[] {
                                                                             1,
                                                                             0,
                                                                             0,
                                                                             0});
            this.numericUpDownWidth.Enter += this.numericUpDownWidth_Enter;
            // 
            // radioButtonWidthSet
            // 
            this.radioButtonWidthSet.Location = new System.Drawing.Point(9, 40);
            this.radioButtonWidthSet.Name = "radioButtonWidthSet";
            this.radioButtonWidthSet.Size = new System.Drawing.Size(89, 16);
            this.radioButtonWidthSet.TabIndex = 1;
            this.radioButtonWidthSet.Text = "Set width to:";
            this.radioButtonWidthSet.CheckedChanged += this.onUpdateButtons;
            // 
            // radioButtonWidthOriginal
            // 
            this.radioButtonWidthOriginal.Location = new System.Drawing.Point(9, 16);
            this.radioButtonWidthOriginal.Name = "radioButtonWidthOriginal";
            this.radioButtonWidthOriginal.Size = new System.Drawing.Size(287, 16);
            this.radioButtonWidthOriginal.TabIndex = 0;
            this.radioButtonWidthOriginal.Text = "Keep original width";
            this.radioButtonWidthOriginal.CheckedChanged += this.onUpdateButtons;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.linkMenuVerticalUnits,
                                                                                    this.numericUpDownHeight,
                                                                                    this.radioButtonHeightSet,
                                                                                    this.radioButtonHeightOriginal});
            this.groupBox2.Location = new System.Drawing.Point(6, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(320, 72);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Height:";
            // 
            // linkMenuVerticalUnits
            // 
            this.linkMenuVerticalUnits.Location = new System.Drawing.Point(176, 40);
            this.linkMenuVerticalUnits.Name = "linkMenuVerticalUnits";
            this.linkMenuVerticalUnits.Options = new string[] {
                                                                  "pixels",
                                                                  "grid units"};
            this.linkMenuVerticalUnits.SelectedIndex = 0;
            this.linkMenuVerticalUnits.Size = new System.Drawing.Size(100, 20);
            this.linkMenuVerticalUnits.TabIndex = 4;
            this.linkMenuVerticalUnits.TabStop = true;
            this.linkMenuVerticalUnits.Text = "pixels";
            this.linkMenuVerticalUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkMenuVerticalUnits.Click += this.linkMenuVerticalUnits_Click;
            // 
            // numericUpDownHeight
            // 
            this.numericUpDownHeight.Location = new System.Drawing.Point(104, 40);
            this.numericUpDownHeight.Maximum = new System.Decimal(new int[] {
                                                                                10000,
                                                                                0,
                                                                                0,
                                                                                0});
            this.numericUpDownHeight.Minimum = new System.Decimal(new int[] {
                                                                                1,
                                                                                0,
                                                                                0,
                                                                                0});
            this.numericUpDownHeight.Name = "numericUpDownHeight";
            this.numericUpDownHeight.Size = new System.Drawing.Size(64, 20);
            this.numericUpDownHeight.TabIndex = 2;
            this.numericUpDownHeight.Value = new System.Decimal(new int[] {
                                                                              1,
                                                                              0,
                                                                              0,
                                                                              0});
            this.numericUpDownHeight.Enter += this.numericUpDownHeight_Enter;
            // 
            // radioButtonHeightSet
            // 
            this.radioButtonHeightSet.Location = new System.Drawing.Point(9, 40);
            this.radioButtonHeightSet.Name = "radioButtonHeightSet";
            this.radioButtonHeightSet.Size = new System.Drawing.Size(89, 16);
            this.radioButtonHeightSet.TabIndex = 1;
            this.radioButtonHeightSet.Text = "Set height to:";
            this.radioButtonHeightSet.CheckedChanged += this.onUpdateButtons;
            // 
            // radioButtonHeightOriginal
            // 
            this.radioButtonHeightOriginal.Location = new System.Drawing.Point(9, 16);
            this.radioButtonHeightOriginal.Name = "radioButtonHeightOriginal";
            this.radioButtonHeightOriginal.Size = new System.Drawing.Size(295, 16);
            this.radioButtonHeightOriginal.TabIndex = 0;
            this.radioButtonHeightOriginal.Text = "Keep original height";
            this.radioButtonHeightOriginal.CheckedChanged += this.onUpdateButtons;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.linkMenuVerticalAlignment,
                                                                                    this.label6,
                                                                                    this.label5,
                                                                                    this.numericUpDownVerticalOffset,
                                                                                    this.label3,
                                                                                    this.numericUpDownHorizontalOffset,
                                                                                    this.label2,
                                                                                    this.linkMenuOrigin,
                                                                                    this.label4,
                                                                                    this.label7,
                                                                                    this.linkMenuHorizontalAlignment});
            this.groupBox3.Location = new System.Drawing.Point(6, 192);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(320, 80);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Position:";
            // 
            // linkMenuVerticalAlignment
            // 
            this.linkMenuVerticalAlignment.BackColor = System.Drawing.SystemColors.Control;
            this.linkMenuVerticalAlignment.Location = new System.Drawing.Point(120, 58);
            this.linkMenuVerticalAlignment.Name = "linkMenuVerticalAlignment";
            this.linkMenuVerticalAlignment.Options = new string[] {
                                                                      "top",
                                                                      "middle",
                                                                      "bottom"};
            this.linkMenuVerticalAlignment.SelectedIndex = 0;
            this.linkMenuVerticalAlignment.Size = new System.Drawing.Size(48, 16);
            this.linkMenuVerticalAlignment.TabIndex = 9;
            this.linkMenuVerticalAlignment.TabStop = true;
            this.linkMenuVerticalAlignment.Text = "top";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 16);
            this.label6.TabIndex = 8;
            this.label6.Text = "This is the position of";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(160, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(152, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "of image component position";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownVerticalOffset
            // 
            this.numericUpDownVerticalOffset.Location = new System.Drawing.Point(256, 14);
            this.numericUpDownVerticalOffset.Maximum = new System.Decimal(new int[] {
                                                                                        1000,
                                                                                        0,
                                                                                        0,
                                                                                        0});
            this.numericUpDownVerticalOffset.Name = "numericUpDownVerticalOffset";
            this.numericUpDownVerticalOffset.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownVerticalOffset.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(160, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Vertical offset of: ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numericUpDownHorizontalOffset
            // 
            this.numericUpDownHorizontalOffset.Location = new System.Drawing.Point(112, 14);
            this.numericUpDownHorizontalOffset.Maximum = new System.Decimal(new int[] {
                                                                                          1000,
                                                                                          0,
                                                                                          0,
                                                                                          0});
            this.numericUpDownHorizontalOffset.Name = "numericUpDownHorizontalOffset";
            this.numericUpDownHorizontalOffset.Size = new System.Drawing.Size(48, 20);
            this.numericUpDownHorizontalOffset.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Horizontal offset of: ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkMenuOrigin
            // 
            this.linkMenuOrigin.Location = new System.Drawing.Point(96, 38);
            this.linkMenuOrigin.Name = "linkMenuOrigin";
            this.linkMenuOrigin.Options = new string[] {
                                                           "center",
                                                           "top-left"};
            this.linkMenuOrigin.SelectedIndex = 0;
            this.linkMenuOrigin.Size = new System.Drawing.Size(72, 16);
            this.linkMenuOrigin.TabIndex = 7;
            this.linkMenuOrigin.TabStop = true;
            this.linkMenuOrigin.Text = "center";
            this.linkMenuOrigin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "Pixels relative to: ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(234, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 16);
            this.label7.TabIndex = 8;
            this.label7.Text = "of the image.";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkMenuHorizontalAlignment
            // 
            this.linkMenuHorizontalAlignment.BackColor = System.Drawing.SystemColors.Control;
            this.linkMenuHorizontalAlignment.Location = new System.Drawing.Point(177, 58);
            this.linkMenuHorizontalAlignment.Name = "linkMenuHorizontalAlignment";
            this.linkMenuHorizontalAlignment.Options = new string[] {
                                                                        "left",
                                                                        "center",
                                                                        "right"};
            this.linkMenuHorizontalAlignment.SelectedIndex = 0;
            this.linkMenuHorizontalAlignment.Size = new System.Drawing.Size(48, 16);
            this.linkMenuHorizontalAlignment.TabIndex = 9;
            this.linkMenuHorizontalAlignment.TabStop = true;
            this.linkMenuHorizontalAlignment.Text = "left";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.linkMenuRotateFlip,
                                                                                    this.label8,
                                                                                    this.radioButtonEffectTile,
                                                                                    this.radioButtonEffectStretch});
            this.groupBox4.Location = new System.Drawing.Point(6, 272);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(320, 64);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Effect:";
            // 
            // radioButtonEffectTile
            // 
            this.radioButtonEffectTile.Location = new System.Drawing.Point(9, 40);
            this.radioButtonEffectTile.Name = "radioButtonEffectTile";
            this.radioButtonEffectTile.Size = new System.Drawing.Size(128, 16);
            this.radioButtonEffectTile.TabIndex = 1;
            this.radioButtonEffectTile.Text = "Tile image";
            // 
            // radioButtonEffectStretch
            // 
            this.radioButtonEffectStretch.Location = new System.Drawing.Point(9, 16);
            this.radioButtonEffectStretch.Name = "radioButtonEffectStretch";
            this.radioButtonEffectStretch.Size = new System.Drawing.Size(128, 16);
            this.radioButtonEffectStretch.TabIndex = 0;
            this.radioButtonEffectStretch.Text = "Stretch image";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(170, 344);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(250, 344);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(168, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(120, 16);
            this.label8.TabIndex = 2;
            this.label8.Text = "Rotate && Flip Effect:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkMenuRotateFlip
            // 
            this.linkMenuRotateFlip.Location = new System.Drawing.Point(168, 40);
            this.linkMenuRotateFlip.Name = "linkMenuRotateFlip";
            this.linkMenuRotateFlip.Options = new string[0];
            this.linkMenuRotateFlip.SelectedIndex = -1;
            this.linkMenuRotateFlip.Size = new System.Drawing.Size(120, 16);
            this.linkMenuRotateFlip.TabIndex = 3;
            this.linkMenuRotateFlip.TabStop = true;
            this.linkMenuRotateFlip.Text = "linkMenu1";
            // 
            // ImageProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(330, 378);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.buttonCancel,
                                                                          this.groupBox4,
                                                                          this.groupBox3,
                                                                          this.groupBox2,
                                                                          this.groupBox1,
                                                                          this.buttonBrowse,
                                                                          this.textBoxImageFile,
                                                                          this.label1});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ImageProperties";
            this.ShowInTaskbar = false;
            this.Text = "Image Properties";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownWidth).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownHeight).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownVerticalOffset).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownHorizontalOffset).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (textBoxImageFile.Text.Trim() == "") {
                MessageBox.Show(this, "You must specify image file name", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxImageFile.Focus();
                return;
            }

            LayoutImageInfo image = new LayoutImageInfo(XmlInfo.Element) {
                ImageFile = textBoxImageFile.Text
            };

            Size imageSize = new Size(radioButtonWidthOriginal.Checked ? -1 : (int)numericUpDownWidth.Value,
                radioButtonHeightOriginal.Checked ? -1 : (int)numericUpDownHeight.Value);

            image.Size = imageSize;

            if (radioButtonWidthSet.Checked)
                image.WidthSizeUnit = (LayoutImageInfo.ImageSizeUnit)linkMenuHorizontalUnits.SelectedIndex;

            if (radioButtonHeightSet.Checked)
                image.HeightSizeUnit = (LayoutImageInfo.ImageSizeUnit)linkMenuVerticalUnits.SelectedIndex;

            image.OriginMethod = (LayoutImageInfo.ImageOriginMethod)linkMenuOrigin.SelectedIndex;
            image.HorizontalAlignment = (LayoutImageInfo.ImageHorizontalAlignment)linkMenuHorizontalAlignment.SelectedIndex;
            image.VerticalAlignment = (LayoutImageInfo.ImageVerticalAlignment)linkMenuVerticalAlignment.SelectedIndex;

            if (radioButtonEffectStretch.Checked)
                image.FillEffect = LayoutImageInfo.ImageFillEffect.Stretch;
            else if (radioButtonEffectTile.Checked)
                image.FillEffect = LayoutImageInfo.ImageFillEffect.Tile;

            image.RotateFlipEffect = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), Enum.GetNames(typeof(RotateFlipType))[linkMenuRotateFlip.SelectedIndex]);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void onUpdateButtons(object sender, System.EventArgs e) {
            updateButtons();
        }

        private void numericUpDownWidth_Enter(object sender, System.EventArgs e) {
            radioButtonWidthSet.Checked = true;
            updateButtons();
        }

        private void linkMenuVerticalUnits_Click(object sender, System.EventArgs e) {
            radioButtonHeightSet.Checked = true;
            updateButtons();
        }

        private void linkMenuHorizontalUnits_Click(object sender, System.EventArgs e) {
            radioButtonWidthSet.Checked = true;
            updateButtons();
        }

        private void numericUpDownHeight_Enter(object sender, System.EventArgs e) {
            radioButtonHeightSet.Checked = true;
            updateButtons();
        }

        private void buttonBrowse_Click(object sender, System.EventArgs e) {
            OpenFileDialog of = new OpenFileDialog {
                AddExtension = true,
                CheckFileExists = true,
                FileName = textBoxImageFile.Text,
                Filter = "Image files (*.jpg,*.bmp,*.gif)|*.jpg;*.bmp;*.gif|All files (*.*)|*.*"
            };

            if (of.ShowDialog(this) == DialogResult.OK) {
                string layoutDirectory = Path.GetDirectoryName(LayoutController.LayoutFilename);

                if (layoutDirectory != null && of.FileName.StartsWith(layoutDirectory))
                    textBoxImageFile.Text = of.FileName.Substring(layoutDirectory.Length + 1);
                else
                    textBoxImageFile.Text = of.FileName;
            }
        }
    }
}
