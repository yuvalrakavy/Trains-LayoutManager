using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace LayoutManager.View {

    [LayoutModule("Image Cache Manager", UserControl = false)]
    class ImageCacheManager : LayoutModuleBase {
        readonly Dictionary<string, Image> imageHashtable = new Dictionary<string, Image>();
        string layoutFileDirectory;

        String LayoutFileDirectory {
            get {
                if (layoutFileDirectory == null)
                    layoutFileDirectory = Path.GetDirectoryName(LayoutController.LayoutFilename);
                return layoutFileDirectory;
            }
        }

        private String getImageFilename(LayoutEvent e) {
            String imageFilename = (String)e.Info;

            if (!Path.IsPathRooted(imageFilename) && LayoutFileDirectory != null)
                imageFilename = Path.Combine(LayoutFileDirectory, imageFilename);
            return imageFilename;
        }

        private String getEffectName(LayoutEvent e) => e.GetOption("Type", "Effect");

        private String getImageCacheKey(LayoutEvent e) => getEffectName(e) + "|" + getImageFilename(e);


        [LayoutEvent("get-image-from-cache")]
        private void getImageFromCache(LayoutEvent e) {
            String imageCacheKey = getImageCacheKey(e);

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
            String imageCacheKey = getImageCacheKey(e);
            Image image = (Image)imageHashtable[imageCacheKey];

            if (imageHashtable.TryGetValue(imageCacheKey, out image)) {
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
            EventManager.Event(new LayoutEvent(this, "clear-image-cache"));
        }

        [LayoutEvent("free-resources")]
        private void mainWindowMinimizedOrDeactivated(LayoutEvent e) {
            EventManager.Event(new LayoutEvent(this, "clear-image-cache"));
        }
    }
}
