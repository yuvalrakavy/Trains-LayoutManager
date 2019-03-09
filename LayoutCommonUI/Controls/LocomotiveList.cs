using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

#nullable enable
#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LocomotiveList.
    /// </summary>
    public class LocomotiveList : XmlQueryListbox {
        LocomotiveCatalogInfo? catalog = null;
        bool showOnlyLocomotives = false;
        bool operationMode = false;
        Rectangle dragSourceRect = Rectangle.Empty;
        IXmlQueryListBoxXmlElementItem? draggedItem = null;

        public LocomotiveList() {
            if (!DesignMode) {
                AddLayout(new ListLayoutSimple());
                AddLayout(new ListLayoutByType());
                AddLayout(new ListLayoutByOrigin());
            }
        }

        public void Initialize() {
            AddLayout(new ListLayoutByStorage());
            EventManager.AddObjectSubscriptions(this);
        }

        public bool ShowOnlyLocomotives {
            set {
                showOnlyLocomotives = value;
            }

            get {
                return showOnlyLocomotives;
            }
        }

        public override IXmlQueryListboxItem CreateItem(QueryItem query, XmlElement itemElement) => new LocomotiveItem(this, query, itemElement);

        public XmlElement? SelectedXmlElement {
            get {
                if (SelectedXmlItem != null)
                    return ((IXmlQueryListBoxXmlElementItem)SelectedXmlItem).Element;
                return null;
            }
        }

        protected LocomotiveCatalogInfo Catalog {
            get {
                if (catalog == null)
                    catalog = LayoutModel.LocomotiveCatalog;

                return catalog;
            }
        }

        protected bool OperationMode => operationMode;

        protected void UpdateElementState(XmlElement element) {
            Guid id = XmlConvert.ToGuid(element.GetAttribute("ID"));
            bool onTrack;

            TrainStateInfo trainState = LayoutModel.StateManager.Trains[id];

            if (trainState == null)
                onTrack = false;
            else {
                // TODO: This will not be correct if it would be possible to create on the fly trains (adding locos)
                if (trainState.Locomotives.Count > 1 && element.Name == "Locomotive")
                    onTrack = false;
                else
                    onTrack = true;
            }

            element.SetAttribute("OnTrack", XmlConvert.ToString(onTrack));

            if (!onTrack) {
                // Try to check if the locomotive can be placed on a block
                String reason = "No free blocks";
                ILayoutPower? power = null;
                CanPlaceTrainResolveMethod canPlaceOnTrack = CanPlaceTrainResolveMethod.NotPossible;

                foreach (LayoutBlockDefinitionComponent blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases)) {

                    if (!blockDefinition.Block.HasTrains && blockDefinition.Block.Power != power) {
                        power = blockDefinition.Block.Power;

                        bool lookForMoreBlocks = true;
                        var result = EventManager.Event<XmlElement, LayoutBlockDefinitionComponent, CanPlaceTrainResult>("can-locomotive-be-placed", element, blockDefinition)!;

                        canPlaceOnTrack = result.ResolveMethod;

                        switch (result.Status) {
                            case CanPlaceTrainStatus.TrainLocomotiveAlreadyUsed:
                                reason = "Member's address used by another locomotive";
                                lookForMoreBlocks = false;          // No chance of finding another better block...
                                break;

                            case CanPlaceTrainStatus.LocomotiveHasNoAddress:
                                reason = "No address is assigned";
                                lookForMoreBlocks = false;          // No chance of finding another better block...
                                break;

                            case CanPlaceTrainStatus.LocomotiveDuplicateAddress:
                                reason = "Members with same address";
                                break;

                            case CanPlaceTrainStatus.LocomotiveAddressAlreadyUsed:
                                reason = "Address already used by " + result.Train.DisplayName;
                                break;

                            default:
                                reason = result.ToString();
                                break;
                        }

                        if (canPlaceOnTrack == CanPlaceTrainResolveMethod.Resolved || !lookForMoreBlocks)
                            break;
                    }
                }

                if (canPlaceOnTrack == CanPlaceTrainResolveMethod.Resolved)
                    element.RemoveAttribute("Reason");
                else
                    element.SetAttribute("Reason", reason);

                element.SetAttribute("CanPlaceOnTrack", canPlaceOnTrack.ToString());
            }
        }

        protected void UpdateElements() {
            EventManager.Event(new LayoutEvent("disable-locomotive-list-update", this));
            foreach (XmlElement element in ContainerElement)
                UpdateElementState(element);
            EventManager.Event(new LayoutEvent("enable-locomotive-list-update", this));
        }

        protected void InvalidateElement(XmlElement element) {
            for (int i = 0; i < Items.Count; i++) {

                if (Items[i] is IXmlQueryListBoxXmlElementItem item && item.Element == element) {
                    Invalidate(GetItemRectangle(i));
                    break;
                }
            }
        }

        protected void InvalidateTrainState(TrainStateInfo trainState) {
            foreach (TrainLocomotiveInfo trainLoco in trainState.Locomotives)
                InvalidateElement(LayoutModel.LocomotiveCollection[trainLoco.CollectionElementId]);
        }

        #region Drag/Drop support

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            int index = IndexFromPoint(new Point(e.X, e.Y));

            if (index != ListBox.NoMatches) {
                draggedItem = Items[index] as IXmlQueryListBoxXmlElementItem;

                if (draggedItem != null) {
                    Size dragAreaSize = SystemInformation.DragSize;

                    dragSourceRect = new Rectangle(new Point(e.X - dragAreaSize.Width / 2, e.Y - dragAreaSize.Height / 2), dragAreaSize);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            dragSourceRect = Rectangle.Empty;
            draggedItem = null;
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && dragSourceRect != Rectangle.Empty) {
                if (!dragSourceRect.Contains(e.X, e.Y)) {
                    // Drag operation has begun
                    if (draggedItem != null)
                        DoDragDrop(draggedItem.Element, DragDropEffects.Link);
                }
            }
        }

        #endregion

        [LayoutEvent("enter-operation-mode", Order = 1000)]
        private void enterOperationMode(LayoutEvent e) {
            operationMode = true;
            UpdateElements();
            Invalidate();
        }

        [LayoutEvent("enter-design-mode")]
        private void enterDesignMode(LayoutEvent e) {
            operationMode = false;
            Invalidate();
        }

        [LayoutEvent("train-placed-on-track")]
        private void locomotivePlacedOnTrack(LayoutEvent e) {
            UpdateElements();
            Invalidate();
        }

        [LayoutEvent("locomotive-removed-from-train")]
        private void locomotiveRemovedFromTrack(LayoutEvent e) {
            UpdateElements();
            Invalidate();
        }

        [LayoutEvent("train-speed-changed")]
        [LayoutEvent("train-enter-block")]
        private void needToUpdateLocomotiveItem(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            InvalidateTrainState(train);
        }

        [LayoutEvent("train-saved-in-collection")]
        private void trainSavedInCollection(LayoutEvent e) {
            UpdateElements();
            Invalidate();
        }

        #region Item classes

        class LocomotiveItem : IXmlQueryListBoxXmlElementItem {
            readonly LocomotiveList list;
            readonly XmlElement element;

            public LocomotiveItem(LocomotiveList list, QueryItem queryItem, XmlElement element) {
                this.list = list;
                this.element = element;
            }

            public XmlElement Element => element;

            public void Measure(MeasureItemEventArgs e) {
                LocomotiveListItemPainter.Measure(e, element);
            }

            public void Draw(DrawItemEventArgs e) {
                if (list.OperationMode) {
                    if (!element.HasAttribute("OnTrack"))
                        list.UpdateElementState(element);
                }

                LocomotiveListItemPainter.Draw(e, element, list.Catalog, list.OperationMode);
            }

            public object Bookmark => new LocomotiveInfo(element).Id.ToString();

            public bool IsBookmarkEqual(object bookmark) {
                if (bookmark is String)
                    return (String)bookmark == element.GetAttribute("ID");
                return false;
            }
        }

        #endregion

        #region ListLayout classes

        class ListLayoutSimple : ListLayout {

            public override String LayoutName => "Simple";

            public override void ApplyLayout(XmlQueryListbox list) {
                list.AddQuery("Locomotives", "Locomotive").Expand();
                list.AddQuery("Trains", "Train").Expand();
            }
        }

        class ListLayoutByOrigin : ListLayout {

            public override String LayoutName => "Locomotive origin";

            public override void ApplyLayout(XmlQueryListbox list) {
                list.AddQuery("European", "Locomotive[@Origin='Europe']");
                list.AddQuery("American", "Locomotive[@Origin='US']");
                list.AddQuery("Trains", "Train");
            }
        }

        class ListLayoutByType : ListLayout {

            public override String LayoutName => "Locomotive type";

            public override void ApplyLayout(XmlQueryListbox list) {
                list.AddQuery("Steam", "Locomotive[@Kind='Steam']");
                list.AddQuery("Diesel", "Locomotive[@Kind='Diesel']");
                list.AddQuery("Electric", "Locomotive[@Kind='Electric']");
                list.AddQuery("Sound Units", "Locomotive[@Kind='SoundUnit']");
                list.AddQuery("Trains", "Train");
            }
        }

        class ListLayoutByStorage : ListLayout {
            public override String LayoutName => "By locomotive storage file";

            public override void ApplyLayout(XmlQueryListbox list) {
                LocomotiveCollectionInfo collection = LayoutModel.LocomotiveCollection;

                int iStore = 0;
                foreach (XmlElement storeElement in collection.Element["Stores"]) {
                    LocomotiveStorageInfo store = new LocomotiveStorageInfo(storeElement);
                    QueryItem q;

                    q = list.AddQuery(store.StorageName, null);
                    q.Add("Locomotives", "Locomotive[@Store='" + iStore + "']");
                    q.Add("Trains", "Train[@Store='" + iStore + "']");

                    iStore++;
                }
            }
        }

        #endregion
    }
}
