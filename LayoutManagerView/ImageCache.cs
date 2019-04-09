using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.View {
    [LayoutModule("Image Cache Manager", UserControl = false)]
    internal class ImageCacheManager : LayoutModuleBase {
        private readonly Dictionary<string, Image> imageHashtable = new Dictionary<string, Image>();
        private string layoutFileDirectory;

        private string LayoutFileDirectory {
            get {
                return layoutFileDirectory ?? (layoutFileDirectory = Path.GetDirectoryName(LayoutController.LayoutFilename));
            }
        }

        private string getImageFilename(LayoutEvent e) {
            string imageFilename = (String)e.Info;

            if (!Path.IsPathRooted(imageFilename) && LayoutFileDirectory != null)
                imageFilename = Path.Combine(LayoutFileDirectory, imageFilename);
            return imageFilename;
        }

        private string getEffectName(LayoutEvent e) => e.GetOption("Type", "Effect");

        private string getImageCacheKey(LayoutEvent e) => getEffectName(e) + "|" + getImageFilename(e);

        [LayoutEvent("get-image-from-cache")]
        private void getImageFromCache(LayoutEvent e) {
            string imageCacheKey = getImageCacheKey(e);

            if (!imageHashtable.TryGetValue(imageCacheKey, out Image image)) {
                try {
                    image = Image.FromFile(getImageFilename(e));
                }
                catch (Exception ex) {
                    throw new ImageLoadException(getImageFilename(e), e.Sender, ex);
                }

                RotateFlipType effect = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), getEffectName(e));

                image.RotateFlip(effect);
                imageHashtable[imageCacheKey] = image;
            }

            e.Info = image;
        }

        [LayoutEvent("remove-image-from-cache")]
        private void removeImageFromCache(LayoutEvent e) {
            string imageCacheKey = getImageCacheKey(e);

            if (imageHashtable.TryGetValue(imageCacheKey, out var image)) {
                image.Dispose();
                imageHashtable.Remove(imageCacheKey);
            }
        }

        [LayoutEvent("clear-image-cache")]
        private void clearImageCache(LayoutEvent e) {
            foreach (Image image in imageHashtable.Values)
                image.Dispose();
            imageHashtable.Clear();
        }

        [LayoutEvent("new-layout-document")]
        private void newLayoutDocument(LayoutEvent e) {
            layoutFileDirectory = null;     // Invalidate it
            EventManager.Event(new LayoutEvent("clear-image-cache", this));
        }

        [LayoutEvent("free-resources")]
        private void mainWindowMinimizedOrDeactivated(LayoutEvent e) {
            EventManager.Event(new LayoutEvent("clear-image-cache", this));
        }
    }
}
