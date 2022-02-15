using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MethodDispatcher;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.View {
    [LayoutModule("Image Cache Manager", UserControl = false)]
    internal class ImageCacheManager : LayoutModuleBase {
        private readonly Dictionary<string, Image> imageHashtable = new();
        private string? layoutFileDirectory;

        private string LayoutFileDirectory => layoutFileDirectory ??= Path.GetDirectoryName(LayoutController.LayoutFilename)!;

        private string GetImageFilename(LayoutEvent e) {
            string imageFilename = Ensure.NotNull<String>(e.Info);

            if (!Path.IsPathRooted(imageFilename) && LayoutFileDirectory != null)
                imageFilename = Path.Combine(LayoutFileDirectory, imageFilename);
            return imageFilename;
        }

        private string GetEffectName(LayoutEvent e) => e.GetOption("Type", "Effect").ValidString();

        private string GetImageCacheKey(LayoutEvent e) => GetEffectName(e) + "|" + GetImageFilename(e);

        [LayoutEvent("get-image-from-cache")]
        private void GetImageFromCache(LayoutEvent e) {
            string imageCacheKey = GetImageCacheKey(e);

            if (!imageHashtable.TryGetValue(imageCacheKey, out Image? image)) {
                try {
                    image = Image.FromFile(GetImageFilename(e));
                }
                catch (Exception ex) {
                    throw new ImageLoadException(GetImageFilename(e), e.Sender, ex);
                }

                RotateFlipType effect = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), GetEffectName(e));

                image.RotateFlip(effect);
                imageHashtable[imageCacheKey] = image;
            }

            e.Info = image;
        }

        [LayoutEvent("remove-image-from-cache")]
        private void RemoveImageFromCache(LayoutEvent e) {
            string imageCacheKey = GetImageCacheKey(e);

            if (imageHashtable.TryGetValue(imageCacheKey, out var image)) {
                image.Dispose();
                imageHashtable.Remove(imageCacheKey);
            }
        }

        [LayoutEvent("clear-image-cache")]
        private void ClearImageCache(LayoutEvent e) {
            foreach (Image image in imageHashtable.Values)
                image.Dispose();
            imageHashtable.Clear();
        }

        [DispatchTarget]
        private void OnNewLayoutDocument(string filename) {
            layoutFileDirectory = null;     // Invalidate it
            EventManager.Event(new LayoutEvent("clear-image-cache", this));
        }

        [LayoutEvent("free-resources")]
        private void MainWindowMinimizedOrDeactivated(LayoutEvent e) {
            EventManager.Event(new LayoutEvent("clear-image-cache", this));
        }
    }
}
