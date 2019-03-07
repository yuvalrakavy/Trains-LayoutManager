using System;
using System.Drawing;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Draw locomotive image
    /// </summary>
    public class LocomotiveImagePainter : IDisposable {
        LocomotiveCatalogInfo catalog = null;
        Image image = null;
        bool flipImage = false;
        Pen framePen = new Pen(Color.Black, 1.0F);
        int frameMargin = 2;
        Size frameSize = new Size(100, 50);
        Point origin = new Point(0, 0);

        public LocomotiveImagePainter() {
        }

        public LocomotiveImagePainter(LocomotiveCatalogInfo catalog) {
            this.Catalog = catalog;
        }

        public LocomotiveImagePainter(XmlElement element) {
            this.LocomotiveElement = element;
        }

        public LocomotiveImagePainter(LocomotiveCatalogInfo catalog, XmlElement element) {
            this.Catalog = catalog;
            this.LocomotiveElement = element;
        }

        public LocomotiveCatalogInfo Catalog {
            set {
                catalog = value;
            }

            get {
                if (catalog == null)
                    catalog = LayoutModel.LocomotiveCatalog;

                return catalog;
            }
        }

        public XmlElement LocomotiveElement {
            set {
                LocomotiveTypeInfo locoType = new LocomotiveTypeInfo(value);
                image = locoType.Image;

                if (image == null)
                    image = Catalog.GetStandardImage(locoType.Kind, locoType.Origin);
            }
        }

        public Image Image {
            get {
                return image;
            }

            set {
                image = value;
            }
        }

        public Pen FramePen {
            get {
                return framePen;
            }

            set {
                if (framePen != null)
                    framePen.Dispose();
                framePen = value;
            }
        }

        public Size FrameSize {
            get {
                return frameSize;
            }

            set {
                frameSize = value;
            }
        }

        public int FrameMargin {
            get {
                return frameMargin;
            }

            set {
                frameMargin = value;
            }
        }

        public bool FlipImage {
            get {
                return flipImage;
            }

            set {
                flipImage = value;
            }
        }

        public Point Origin {
            get {
                return origin;
            }

            set {
                origin = value;
            }
        }

        public void Dispose() {
            if (framePen != null)
                framePen.Dispose();
        }

        public void Draw(Graphics g) {
            if (FramePen != null)
                g.DrawRectangle(FramePen, new Rectangle(Origin, FrameSize));

            if (Image != null) {
                SizeF drawnImageSize;
                RectangleF drawnImageRectangle;
                double aspectRatio = (double)image.Width / (double)image.Height;
                int w = FrameSize.Width - 2 * FrameMargin;
                int h = FrameSize.Height - 2 * FrameMargin;

                if (FlipImage)
                    this.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);

                if (w / aspectRatio > h)
                    drawnImageSize = new SizeF((float)(h * aspectRatio), h);
                else
                    drawnImageSize = new SizeF(w, (float)(w / aspectRatio));

                drawnImageRectangle = new RectangleF(new PointF(
                    origin.X + (FrameSize.Width - drawnImageSize.Width) / 2.0F,
                    origin.Y + (FrameSize.Height - drawnImageSize.Height) / 2.0F),
                    drawnImageSize);

                g.DrawImage(this.Image, drawnImageRectangle);
            }
        }

        public void Draw(Graphics g, Image image) {
            this.Image = image;
            Draw(g);
        }

        public void Draw(Graphics g, XmlElement element) {
            this.LocomotiveElement = element;
            Draw(g);
        }

        public void Draw(Graphics g, LocomotiveTypeInfo locoType) {
            this.LocomotiveElement = locoType.Element;
            Draw(g);
        }

        public void Draw(Graphics g, Point origin, Size frameSize, XmlElement element) {
            this.LocomotiveElement = element;
            this.Origin = origin;
            this.FrameSize = frameSize;
            Draw(g);
        }
    }
}
