using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LocomotiveTypeList.
    /// </summary>
    public class LocomotiveTypeList : XmlQueryListbox {
        private LocomotiveCatalogInfo catalog = null;

        public LocomotiveTypeList() {
            if (!DesignMode) {
                AddLayout(new ListLayoutByOrigin());
                AddLayout(new ListLayoutByKind());
            }
        }

        public LocomotiveTypeInfo SelectedLocomotiveType => SelectedXmlItem != null ? ((LocoTypeItem)SelectedXmlItem).LocomotiveType : null;

        public override IXmlQueryListboxItem CreateItem(XmlQueryListbox.QueryItem queryItem, XmlElement itemElement) => new LocoTypeItem(this, queryItem, itemElement);

        public LocomotiveCatalogInfo Catalog => catalog ?? (catalog = LayoutModel.LocomotiveCatalog);

        public void Initialize() {
            AddLayout(new ListLayoutByStorage());
        }

        #region Item classes

        private class LocoTypeItem : IXmlQueryListboxItem {
            private readonly XmlElement locoTypeElement;
            private readonly LocomotiveTypeList list;
            private readonly XmlQueryListbox.QueryItem queryItem;

            public LocoTypeItem(LocomotiveTypeList list, XmlQueryListbox.QueryItem queryItem, XmlElement locoTypeElement) {
                this.list = list;
                this.queryItem = queryItem;
                this.locoTypeElement = locoTypeElement;
            }

            public LocomotiveTypeInfo LocomotiveType => new LocomotiveTypeInfo(locoTypeElement);

            public Object Bookmark => locoTypeElement.GetAttribute("ID");

            public bool IsBookmarkEqual(object bookmark) {
                return bookmark is String ? (String)bookmark == locoTypeElement.GetAttribute("ID") : false;
            }

            public void Measure(MeasureItemEventArgs e) {
                e.ItemHeight = 50 + 4 + 6;
            }

            public void Draw(DrawItemEventArgs e) {
                LocomotiveTypeInfo locomotiveType = LocomotiveType;
                Image image = locomotiveType.Image;

                if (image == null)
                    image = list.Catalog.GetStandardImage(locomotiveType.Kind, locomotiveType.Origin);

                // TODO: Draw background to indicate if this type is in the collection, and if it in the layout
                e.DrawBackground();
                e.DrawFocusRectangle();

                queryItem.DrawLevelLines(e);

                int leftMargin = e.Bounds.Left + 4 + (16 * queryItem.Level);

                Rectangle imageRect = new Rectangle(new Point(leftMargin, e.Bounds.Top + 4), new Size(100, 50));

                e.Graphics.FillRectangle(Brushes.White, imageRect);
                e.Graphics.DrawRectangle(Pens.Black, imageRect);

                if (image != null) {
                    Size imageSize = new Size(image.Width < 98 ? image.Width : 98,
                        image.Height < 48 ? image.Height : 48);
                    Point imageOrigin = new Point(imageRect.Left + ((100 - imageSize.Width) / 2), imageRect.Top + ((50 - imageSize.Height) / 2));

                    e.Graphics.DrawImage(image, new Rectangle(imageOrigin, imageSize));
                }

                float xText = imageRect.Right + 10;
                float yText = e.Bounds.Top + 4;

                using (Brush textBrush = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.WindowText)) {
                    SizeF titleSize;

                    using (Font titleFont = new Font("Arial", 8, FontStyle.Bold)) {
                        titleSize = e.Graphics.MeasureString(locomotiveType.TypeName, titleFont);

                        e.Graphics.DrawString(locomotiveType.TypeName, titleFont, textBrush, new PointF(xText, yText));
                    }

                    yText += titleSize.Height;
                }

                using Pen p = new Pen(Color.Black, 2.0F);
                e.Graphics.DrawLine(p, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Bottom);
            }
        }

        #endregion

        #region ListLayout classes

        private class ListLayoutByOrigin : ListLayout {
            public override string LayoutName => "Locomotive origin";

            public override void ApplyLayout(XmlQueryListbox list) {
                QueryItem q;

                q = list.AddQuery("European Locomotives", null);
                q.Add("Steam", "*[@Origin='Europe' and @Kind='Steam']");
                q.Add("Diesel", "*[@Origin='Europe' and @Kind='Diesel']");
                q.Add("Electric", "*[@Origin='Europe' and @Kind='Electric']");
                q.Add("Sound Unit", "*[@Origin='Europe' and @Kind='SoundUnit']");

                q = list.AddQuery("American Locomotives", null);
                q.Add("Steam", "*[@Origin='US' and @Kind='Steam']");
                q.Add("Diesel", "*[@Origin='US' and @Kind='Diesel']");
                q.Add("Electric", "*[@Origin='US' and @Kind='Electric']");
                q.Add("Sound Unit", "*[@Origin='US' and @Kind='SoundUnit']");
            }
        }

        private class ListLayoutByKind : ListLayout {
            public override string LayoutName => "Locomotive type";

            public override void ApplyLayout(XmlQueryListbox list) {
                QueryItem q;

                q = list.AddQuery("Steam Locomotives", null);
                q.Add("European Locomotives", "*[@Origin='Europe' and @Kind='Steam']");
                q.Add("American Locomotives", "*[@Origin='US' and @Kind='Steam']");

                q = list.AddQuery("Diesel Locomotives", null);
                q.Add("European Locomotives", "*[@Origin='Europe' and @Kind='Diesel']");
                q.Add("American Locomotives", "*[@Origin='US' and @Kind='Diesel']");

                q = list.AddQuery("Electric Locomotives", null);
                q.Add("European Locomotives", "*[@Origin='Europe' and @Kind='Electric']");
                q.Add("American Locomotives", "*[@Origin='US' and @Kind='Electric']");

                q = list.AddQuery("Sound units", null);
                q.Add("European Locomotives", "*[@Origin='Europe' and @Kind='SoundUnit']");
                q.Add("American Locomotives", "*[@Origin='US' and @Kind='SoundUnit']");
            }
        }

        private class ListLayoutByStorage : ListLayout {
            public override string LayoutName => "By locomotive type storage file";

            public override void ApplyLayout(XmlQueryListbox list) {
                LocomotiveCatalogInfo catalog = LayoutModel.LocomotiveCatalog;

                int iStore = 0;
                foreach (XmlElement storeElement in catalog.Element["Stores"]) {
                    LocomotiveStorageInfo store = new LocomotiveStorageInfo(storeElement);

                    list.AddQuery(store.StorageName, "*[@Store='" + iStore + "']");
                    iStore++;
                }
            }
        }
    }

    #endregion
}
