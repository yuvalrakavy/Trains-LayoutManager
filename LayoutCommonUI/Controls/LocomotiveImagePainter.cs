using System;
using System.Drawing;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Draw locomotive image
    /// </summary>
    public class LocomotiveImagePainter : IDisposable {
        private LocomotiveCatalogInfo catalog = null;
        private Pen framePen = new Pen(Color.Black, 1.0F);

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
                return catalog ?? (catalog = LayoutModel.LocomotiveCatalog);
            }
        }

        public XmlElement LocomotiveElement {
            set {
                LocomotiveTypeInfo locoType = new LocomotiveTypeInfo(value);
                Image = locoType.Image;

                if (Image == null)
                    Image = Catalog.GetStandardImage(locoType.Kind, locoType.Origin);
            }
        }

        public Image Image { get; set; } = null;

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

        public Size FrameSize { get; set; } = new Size(100, 50);

        public int FrameMargin { get; set; } = 2;

        public bool FlipImage { get; set; } = false;

        public Point Origin { get; set; } = new Point(0, 0);

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
                double aspectRatio = (double)Image.Width / (double)Image.Height;
                int w = FrameSize.Width - (2 * FrameMargin);
                int h = FrameSize.Height - (2 * FrameMargin);

                if (FlipImage)
                    this.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);

                if (w / aspectRatio > h)
                    drawnImageSize = new SizeF((float)(h * aspectRatio), h);
                else
                    drawnImageSize = new SizeF(w, (float)(w / aspectRatio));

                drawnImageRectangle = new RectangleF(new PointF(
                    Origin.X + ((FrameSize.Width - drawnImageSize.Width) / 2.0F),
                    Origin.Y + ((FrameSize.Height - drawnImageSize.Height) / 2.0F)),
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
