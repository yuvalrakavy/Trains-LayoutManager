using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;
using MethodDispatcher;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LocomotiveList.
    /// </summary>
    public class LocomotiveList : XmlQueryListbox {
        private const string A_OnTrack = "OnTrack";
        private const string A_Reason = "Reason";
        private const string A_CanPlaceOnTrack = "CanPlaceOnTrack";
        private const string A_Id = "ID";
        private LocomotiveCatalogInfo? catalog = null;
        private bool operationMode = false;
        private Rectangle dragSourceRect = Rectangle.Empty;
        private IXmlQueryListBoxXmlElementItem? draggedItem = null;

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
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        public bool ShowOnlyLocomotives { set; get; } = false;

        public override IXmlQueryListboxItem CreateItem(QueryItem queryItem, XmlElement itemElement) => new LocomotiveItem(this, queryItem, itemElement);

        public XmlElement? SelectedXmlElement => SelectedXmlItem != null ? ((IXmlQueryListBoxXmlElementItem)SelectedXmlItem).Element : null;

        protected LocomotiveCatalogInfo Catalog => catalog ??= LayoutModel.LocomotiveCatalog;

        protected bool OperationMode => operationMode;

        protected static void UpdateElementState(XmlElement element) {
            var id = (Guid)element.AttributeValue(A_Id);
            bool onTrack;

            var trainState = LayoutModel.StateManager.Trains[id];

            if (trainState == null)
                onTrack = false;
            else {
                // TODO: This will not be correct if it would be possible to create on the fly trains (adding locos)
                onTrack = trainState.Locomotives.Count <= 1 || element.Name != "Locomotive";
            }

            element.SetAttributeValue(A_OnTrack, onTrack);

            if (!onTrack) {
                // Try to check if the locomotive can be placed on a block
                string reason = "No free blocks";
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
                    element.RemoveAttribute(A_Reason);
                else
                    element.SetAttribute(A_Reason, reason);

                element.SetAttributeValue(A_CanPlaceOnTrack, canPlaceOnTrack);
            }
        }

        protected void UpdateElements() {
            if (ContainerElement != null) {
                EventManager.Event(new LayoutEvent("disable-locomotive-list-update", this));
                foreach (XmlElement element in ContainerElement)
                    UpdateElementState(element);
                EventManager.Event(new LayoutEvent("enable-locomotive-list-update", this));
            }
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
            foreach (TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
                XmlElement? element = LayoutModel.LocomotiveCollection[trainLoco.CollectionElementId];

                if(element != null)
                    InvalidateElement(element);
            }
        }

        #region Drag/Drop support

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            int index = IndexFromPoint(new Point(e.X, e.Y));

            if (index != ListBox.NoMatches) {
                draggedItem = Items[index] as IXmlQueryListBoxXmlElementItem;

                if (draggedItem != null) {
                    Size dragAreaSize = SystemInformation.DragSize;

                    dragSourceRect = new Rectangle(new Point(e.X - (dragAreaSize.Width / 2), e.Y - (dragAreaSize.Height / 2)), dragAreaSize);
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
        private void EnterOperationMode(LayoutEvent e) {
            operationMode = true;
            UpdateElements();
            Invalidate();
        }

        [LayoutEvent("enter-design-mode")]
        private void EnterDesignMode(LayoutEvent e) {
            operationMode = false;
            Invalidate();
        }

        [LayoutEvent("train-placed-on-track")]
        private void LocomotivePlacedOnTrack(LayoutEvent e) {
            UpdateElements();
            Invalidate();
        }

        [LayoutEvent("locomotive-removed-from-train")]
        private void LocomotiveRemovedFromTrack(LayoutEvent e) {
            UpdateElements();
            Invalidate();
        }

        [LayoutEvent("train-speed-changed")]
        [LayoutEvent("train-enter-block")]
        private void NeedToUpdateLocomotiveItem(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender, "train");

            InvalidateTrainState(train);
        }

        [LayoutEvent("train-saved-in-collection")]
        private void TrainSavedInCollection(LayoutEvent e) {
            UpdateElements();
            Invalidate();
        }

        [LayoutEvent("locomotive-address-changed")]
        private void LocomotiveAddressChanged(LayoutEvent e0) {
            UpdateElements();
            Invalidate();
        }

        #region Item classes

        private class LocomotiveItem : IXmlQueryListBoxXmlElementItem {
            private readonly LocomotiveList list;

            public LocomotiveItem(LocomotiveList list, QueryItem queryItem, XmlElement element) {
                this.list = list;
                this.Element = element;
            }

            public XmlElement Element { get; }

            public void Measure(MeasureItemEventArgs e) {
                LocomotiveListItemPainter.Measure(e, Element);
            }

            public void Draw(DrawItemEventArgs e) {
                if (list.OperationMode) {
                    if (!Element.HasAttribute(A_OnTrack))
                        LocomotiveList.UpdateElementState(Element);
                }

                LocomotiveListItemPainter.Draw(e, Element, list.Catalog, list.OperationMode);
            }

            public object Bookmark => new LocomotiveInfo(Element).Id.ToString();

            public bool IsBookmarkEqual(object bookmark) => bookmark is string aString && aString == Element.GetAttribute(A_Id);
        }

        #endregion

        #region ListLayout classes

        private class ListLayoutSimple : ListLayout {
            public override string LayoutName => "Simple";

            public override void ApplyLayout(XmlQueryListbox list) {
                list.AddQuery("Locomotives", "Locomotive")?.Expand();
                list.AddQuery("Trains", "Train")?.Expand();
            }
        }

        private class ListLayoutByOrigin : ListLayout {
            public override string LayoutName => "Locomotive origin";

            public override void ApplyLayout(XmlQueryListbox list) {
                list.AddQuery("European", "Locomotive[@Origin='Europe']");
                list.AddQuery("American", "Locomotive[@Origin='US']");
                list.AddQuery("Trains", "Train");
            }
        }

        private class ListLayoutByType : ListLayout {
            public override string LayoutName => "Locomotive type";

            public override void ApplyLayout(XmlQueryListbox list) {
                list.AddQuery("Steam", "Locomotive[@Kind='Steam']");
                list.AddQuery("Diesel", "Locomotive[@Kind='Diesel']");
                list.AddQuery("Electric", "Locomotive[@Kind='Electric']");
                list.AddQuery("Sound Units", "Locomotive[@Kind='SoundUnit']");
                list.AddQuery("Trains", "Train");
            }
        }

        private class ListLayoutByStorage : ListLayout {
            public override string LayoutName => "By locomotive storage file";

            public override void ApplyLayout(XmlQueryListbox list) {
                LocomotiveCollectionInfo collection = LayoutModel.LocomotiveCollection;

                int iStore = 0;
                XmlElement? storeElements = collection.Element["Stores"];

                if (storeElements != null) {
                    foreach (XmlElement storeElement in storeElements) {
                        LocomotiveStorageInfo store = new(storeElement);
                        var q = list.AddQuery(store.StorageName, null);
                        
                        q?.Add("Locomotives", "Locomotive[@Store='" + iStore + "']");
                        q?.Add("Trains", "Train[@Store='" + iStore + "']");

                        iStore++;
                    }
                }
            }
        }

        #endregion
    }
}
