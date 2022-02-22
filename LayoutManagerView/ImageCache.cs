using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.View {
    [LayoutModule("Image Cache Manager", UserControl = false)]
    internal class ImageCacheManager : LayoutModuleBase {
        private readonly Dictionary<string, Image> imageHashtable = new();
        private string? layoutFileDirectory;

        private string LayoutFileDirectory => layoutFileDirectory ??= Path.GetDirectoryName(LayoutController.LayoutFilename)!;

        private string GetImageFilename(string imageFilename) {
            if (!Path.IsPathRooted(imageFilename) && LayoutFileDirectory != null)
                imageFilename = Path.Combine(LayoutFileDirectory, imageFilename);
            return imageFilename;
        }

        private string GetImageCacheKey(string imageFilename, RotateFlipType effect) => $"{effect}|{GetImageFilename(imageFilename)}";

        [DispatchTarget]
        private Image GetImageFromCache(object requestor, string imageFilename, RotateFlipType effect) {
            string imageCacheKey = GetImageCacheKey(imageFilename, effect);

            if (!imageHashtable.TryGetValue(imageCacheKey, out Image? image)) {
                try {
                    image = Image.FromFile(GetImageFilename(imageFilename));
                }
                catch (Exception ex) {
                    throw new ImageLoadException(GetImageFilename(imageFilename), requestor, ex);
                }

                image.RotateFlip(effect);
                imageHashtable[imageCacheKey] = image;
            }

            return image;
        }

        [DispatchTarget]
        private void RemoveImageFromCache(string imageFilename, RotateFlipType effect) {
            string imageCacheKey = GetImageCacheKey(imageFilename, effect);

            if (imageHashtable.TryGetValue(imageCacheKey, out var image)) {
                image.Dispose();
                imageHashtable.Remove(imageCacheKey);
            }
        }

        [DispatchTarget]
        private void ClearImageCache() {
            foreach (Image image in imageHashtable.Values)
                image.Dispose();
            imageHashtable.Clear();
        }

        [DispatchTarget]
        private void OnNewLayoutDocument(string filename) {
            layoutFileDirectory = null;     // Invalidate it
            Dispatch.Call.ClearImageCache();
        }

        [DispatchTarget]
        private void FreeResources() {
            Dispatch.Call.ClearImageCache();
        }
    }
}
