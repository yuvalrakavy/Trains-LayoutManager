using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutManager.CommonUI.Controls {
    public partial class ImageGetter : UserControl {
        public ImageGetter() {
            InitializeComponent();
        }

        private Image _defaultImage = null;

        public bool HasImage {
            get;
            private set;
        } = false;

        public bool ImageModified { get; private set; } = false;

        public Image Image {
            set {
                if (value != null) {
                    pictureBoxImage.Image = value;
                    HasImage = true;
                }
                else {
                    pictureBoxImage.Image = DefaultImage;
                    HasImage = false;
                }
            }

            get => pictureBoxImage.Image;
        }

        public Image DefaultImage {
            set {
                _defaultImage = value;
                if(!HasImage)
                    pictureBoxImage.Image = _defaultImage;
            }

            get => _defaultImage;
        }

        public Size RequiredImageSize { get; set; } = new Size(46, 34);

        private void buttonSet_Click(object sender, EventArgs e) {
            FileDialog fileDialog = new OpenFileDialog {
                AddExtension = true,
                CheckFileExists = true,
                Filter = "Image files (*.jpg,*.bmp)|*.jpg;*.bmp|All files|*.*"
            };

            if (fileDialog.ShowDialog(this) == DialogResult.OK) {
                try {
                    Image image = Image.FromFile(fileDialog.FileName);

                    var trimDialog = new Dialogs.ImageTrim(image, this.RequiredImageSize);

                    if(trimDialog.ShowDialog() == DialogResult.OK) {
                        this.Image = trimDialog.Image;
                        this.ImageModified = true;
                    }
                } catch (Exception ex) {
                    MessageBox.Show(this, "Error loading image: " + ex.Message, "Image load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e) {
            if (pictureBoxImage.Image != null)
                this.ImageModified = true;
            pictureBoxImage.Image = DefaultImage;
        }
    }
}
