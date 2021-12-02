using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for LocomotiveCatalogStandardImages.
    /// </summary>
    public partial class LocomotiveCatalogStandardImages : Form {
        private class ImageEntry {
            public PictureBox picBox;
            public Button setButton;
            public Button clearButton;

            public ImageEntry(PictureBox picBox, Button setButton, Button clearButton) {
                this.picBox = picBox;
                this.setButton = setButton;
                this.clearButton = clearButton;
            }
        }

        private readonly LocomotiveCatalogInfo catalog;
        private readonly ImageEntry[,] imageTable;
        private readonly LocomotiveOrigin[] origins = new LocomotiveOrigin[] { LocomotiveOrigin.Europe, LocomotiveOrigin.US };
        private readonly LocomotiveKind[] kinds = new LocomotiveKind[] { LocomotiveKind.Steam, LocomotiveKind.Diesel, LocomotiveKind.Electric, LocomotiveKind.SoundUnit };
        private readonly bool[,] modified;

        public LocomotiveCatalogStandardImages(LocomotiveCatalogInfo catalog) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.catalog = catalog;

            imageTable = new ImageEntry[,] { {
                new ImageEntry(pictureBoxES, buttonSetES, buttonClearES),
                new ImageEntry(pictureBoxED, buttonSetED, buttonClearED),
                new ImageEntry(pictureBoxEE, buttonSetEE, buttonClearEE),
                new ImageEntry(pictureBoxESU,buttonSetESU,buttonClearESU),
             }, {
                new ImageEntry(pictureBoxUS, buttonSetUS, buttonClearUS),
                new ImageEntry(pictureBoxUD, buttonSetUD, buttonClearUD),
                new ImageEntry(pictureBoxUE, buttonSetUE, buttonClearUE),
                new ImageEntry(pictureBoxUSU,buttonSetUSU,buttonClearUSU),
            } };

            modified = new bool[origins.Length, kinds.Length];

            for (int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
                for (int iKind = 0; iKind < kinds.Length; iKind++) {
                    ImageEntry entry = imageTable[iOrigin, iKind];

                    entry.picBox.Image = catalog.GetStandardImage(kinds[iKind], origins[iOrigin]);
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

        protected void buttonSet_click(object? sender, EventArgs e) {
            for (int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
                for (int iKind = 0; iKind < kinds.Length; iKind++) {
                    ImageEntry entry = imageTable[iOrigin, iKind];

                    if (sender == entry.setButton) {
                        FileDialog fileDialog = new OpenFileDialog {
                            AddExtension = true,
                            CheckFileExists = true,
                            Filter = "Image files (*.jpg,*.bmp,*.gif)|*.jpg;*.bmp;*.gif|All files|*.*"
                        };

                        if (fileDialog.ShowDialog(this) == DialogResult.OK) {
                            try {
                                Image image = Image.FromFile(fileDialog.FileName);

                                entry.picBox.Image = image;
                                modified[iOrigin, iKind] = true;
                            }
                            catch (Exception ex) {
                                MessageBox.Show(this, "Error loading image: " + ex.Message, "Image load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;
                    }
                }
            }
        }

        protected void buttonClear_click(object? sender, EventArgs e) {
            for (int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
                for (int iKind = 0; iKind < kinds.Length; iKind++) {
                    ImageEntry entry = imageTable[iOrigin, iKind];

                    if (sender == entry.clearButton) {
                        entry.picBox.Image = null;
                        modified[iOrigin, iKind] = true;
                        break;
                    }
                }
            }
        }

        private void buttonOK_Click(object? sender, EventArgs e) {
            for (int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
                for (int iKind = 0; iKind < kinds.Length; iKind++) {
                    ImageEntry entry = imageTable[iOrigin, iKind];

                    if (modified[iOrigin, iKind])
                        catalog.SetStandardImage(kinds[iKind], origins[iOrigin], entry.picBox.Image);
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
