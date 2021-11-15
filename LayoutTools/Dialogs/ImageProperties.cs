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
    public partial class ImageProperties : Form, ILayoutComponentPropertiesDialog {

        public ImageProperties(ModelComponent component) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.XmlInfo = new LayoutXmlInfo(component);

            LayoutImageInfo image = new(XmlInfo.Element);

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

            UpdateButtons();
        }

        public LayoutXmlInfo XmlInfo { get; }

        private void UpdateButtons() {
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

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (textBoxImageFile.Text.Trim() == "") {
                MessageBox.Show(this, "You must specify image file name", "Missing input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxImageFile.Focus();
                return;
            }

            LayoutImageInfo image = new(XmlInfo.Element) {
                ImageFile = textBoxImageFile.Text
            };

            Size imageSize = new(radioButtonWidthOriginal.Checked ? -1 : (int)numericUpDownWidth.Value,
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

        private void OnUpdateButtons(object? sender, System.EventArgs e) {
            UpdateButtons();
        }

        private void NumericUpDownWidth_Enter(object? sender, System.EventArgs e) {
            radioButtonWidthSet.Checked = true;
            UpdateButtons();
        }

        private void LinkMenuVerticalUnits_Click(object? sender, System.EventArgs e) {
            radioButtonHeightSet.Checked = true;
            UpdateButtons();
        }

        private void LinkMenuHorizontalUnits_Click(object? sender, System.EventArgs e) {
            radioButtonWidthSet.Checked = true;
            UpdateButtons();
        }

        private void NumericUpDownHeight_Enter(object? sender, System.EventArgs e) {
            radioButtonHeightSet.Checked = true;
            UpdateButtons();
        }

        private void ButtonBrowse_Click(object? sender, System.EventArgs e) {
            OpenFileDialog of = new() {
                AddExtension = true,
                CheckFileExists = true,
                FileName = textBoxImageFile.Text,
                Filter = "Image files (*.jpg,*.bmp,*.gif)|*.jpg;*.bmp;*.gif|All files (*.*)|*.*"
            };

            if (of.ShowDialog(this) == DialogResult.OK) {
                string? layoutDirectory = Path.GetDirectoryName(LayoutController.LayoutFilename);

                if (layoutDirectory != null && of.FileName.StartsWith(layoutDirectory))
                    textBoxImageFile.Text = of.FileName[(layoutDirectory.Length + 1)..];
                else
                    textBoxImageFile.Text = of.FileName;
            }
        }
    }
}
