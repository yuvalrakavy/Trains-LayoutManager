namespace LayoutManager.CommonUI.Controls {
    public partial class ImageGetter : UserControl {
        public ImageGetter() {
            InitializeComponent();
        }

        private Image? _defaultImage = null;

        public bool HasImage {
            get;
            private set;
        } = false;

        public bool ImageModified { get; private set; } = false;

        public Image Image {
            set {
                if (value != null) {
                    pictureBoxImage.BackgroundImage = value;
                    HasImage = true;
                }
                else {
                    pictureBoxImage.BackgroundImage = DefaultImage;
                    HasImage = false;
                }
            }

            get => pictureBoxImage.BackgroundImage;
        }

        public Image? DefaultImage {
            set {
                _defaultImage = value;
                if (!HasImage)
                    pictureBoxImage.BackgroundImage = _defaultImage;
            }

            get => _defaultImage;
        }

        public Size RequiredImageSize { get; set; } = new Size(46, 34);

        private void ButtonSet_Click(object? sender, EventArgs e) {
            FileDialog fileDialog = new OpenFileDialog {
                AddExtension = true,
                CheckFileExists = true,
                Filter = "Image files (*.jpg,*.bmp)|*.jpg;*.bmp|All files|*.*"
            };

            if (fileDialog.ShowDialog(this) == DialogResult.OK) {
                try {
                    Image image = Image.FromFile(fileDialog.FileName);

                    var trimDialog = new Dialogs.ImageTrim(image, this.RequiredImageSize);

                    if (trimDialog.ShowDialog() == DialogResult.OK) {
                        this.pictureBoxImage.BackgroundImage = trimDialog.Image;
                        this.ImageModified = true;
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(this, "Error loading image: " + ex.Message, "Image load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ButtonClear_Click(object? sender, EventArgs e) {
            if (pictureBoxImage.Image != null)
                this.ImageModified = true;
            pictureBoxImage.BackgroundImage = DefaultImage;
        }

        private void ButtonPaste_Click(object? sender, EventArgs e) {
            if (Clipboard.ContainsImage()) {
                var image = Clipboard.GetImage();
                var trimDialog = new Dialogs.ImageTrim(image, this.RequiredImageSize);

                if (trimDialog.ShowDialog() == DialogResult.OK) {
                    pictureBoxImage.BackgroundImage = trimDialog.Image;
                    this.ImageModified = true;
                }
            }
            else
                MessageBox.Show("Clipboard does not contains a valid image", "Clipboard has no image", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
