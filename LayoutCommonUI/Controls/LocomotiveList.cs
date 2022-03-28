using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System.Xml;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for LocomotiveList.
    /// </summary>
    public class LocomotiveList {
        private const string A_OnTrack = "OnTrack";
        private const string A_Reason = "Reason";
        private const string A_CanPlaceOnTrack = "CanPlaceOnTrack";
        private const string A_Id = "ID";
        private LocomotiveCatalogInfo? catalog = null;
        private bool operationMode = false;
        private Rectangle dragSourceRect = Rectangle.Empty;
        private IXmlQueryListBoxXmlElementItem? draggedItem = null;
        private readonly XmlQueryList xmlQueryList;

        public LocomotiveList(XmlQueryList xmlQueryList) {
            this.xmlQueryList = xmlQueryList;
        }

        public void Initialize() {
            Dispatch.AddObjectInstanceDispatcherTargets(this);

            xmlQueryList.AddLayout(new ListLayoutSimple());
            xmlQueryList.AddLayout(new ListLayoutByType());
            xmlQueryList.AddLayout(new ListLayoutByOrigin());
            xmlQueryList.AddLayout(new ListLayoutByStorage());

            xmlQueryList.CreateItem = (queryItem, itemElement) => new LocomotiveItem(this, queryItem, itemElement);

            var listBox = xmlQueryList.ListBox;
            listBox.MouseDown += OnMouseDown;
            listBox.MouseUp += OnMouseUp;
            listBox.MouseMove += OnMouseMove;
        }

        public bool ShowOnlyLocomotives { set; get; } = false;


        public XmlElement? SelectedXmlElement => xmlQueryList.SelectedXmlItem != null ? ((IXmlQueryListBoxXmlElementItem)xmlQueryList.SelectedXmlItem).Element : null;

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
                        var result = Dispatch.Call.CanLocomotiveBePlaced(element, blockDefinition);

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
            if (xmlQueryList.ContainerElement != null) {
                Dispatch.Call.DisableLocomotiveListUpdate();

                foreach (XmlElement element in xmlQueryList.ContainerElement)
                    UpdateElementState(element);

                Dispatch.Call.EnableLocomotiveListUpdate();
            }
        }

        protected void InvalidateElement(XmlElement element) {
            var listBox = xmlQueryList.ListBox;

            for (int i = 0; i < xmlQueryList.ListBox.Items.Count; i++) {
                if (listBox.Items[i] is IXmlQueryListBoxXmlElementItem item && item.Element == element) {
                    listBox.Invalidate(listBox.GetItemRectangle(i));
                    break;
                }
            }
        }

        protected void InvalidateTrainState(TrainStateInfo trainState) {
            foreach (TrainLocomotiveInfo trainLoco in trainState.Locomotives) {
                XmlElement? element = LayoutModel.LocomotiveCollection[trainLoco.CollectionElementId];

                if (element != null)
                    InvalidateElement(element);
            }
        }

        #region Drag/Drop support

        private void OnMouseDown(object? sender, MouseEventArgs e) {
            var listBox = xmlQueryList.ListBox;

            int index = listBox.IndexFromPoint(new Point(e.X, e.Y));

            // Bug - listBix.IndexFromPoint return 0xffff and not 32 bits -1...
            if (index != ListBox.NoMatches && index < listBox.Items.Count) {
                draggedItem = listBox.Items[index] as IXmlQueryListBoxXmlElementItem;

                if (draggedItem != null) {
                    Size dragAreaSize = SystemInformation.DragSize;

                    dragSourceRect = new Rectangle(new Point(e.X - (dragAreaSize.Width / 2), e.Y - (dragAreaSize.Height / 2)), dragAreaSize);
                }
            }
        }

        private void OnMouseUp(object? sender, MouseEventArgs e) {
            dragSourceRect = Rectangle.Empty;
            draggedItem = null;
        }

        private void OnMouseMove(object? sender, MouseEventArgs e) {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left && dragSourceRect != Rectangle.Empty) {
                if (!dragSourceRect.Contains(e.X, e.Y)) {
                    // Drag operation has begun
                    if (draggedItem != null)
                        xmlQueryList.ListBox.DoDragDrop(draggedItem.Element, DragDropEffects.Link);
                }
            }
        }

        #endregion

        [DispatchTarget(Order = 1000)]
        private void OnEnteredOperationMode(OperationModeParameters settings) {
            operationMode = true;
            UpdateElements();
            xmlQueryList.ListBox.Invalidate();
        }

        [DispatchTarget]
        private void OnEnteredDesignMode() {
            operationMode = false;
            xmlQueryList.ListBox.Invalidate();
        }

        [DispatchTarget]
        private void OnTrainPlacedOnTrack(TrainStateInfo train) {
            UpdateElements();
            xmlQueryList.ListBox.Invalidate();
        }

        [DispatchTarget]
        private void OnLocomotiveRemovedFromTrain(TrainStateInfo train) {
            UpdateElements();
            xmlQueryList.ListBox.Invalidate();
        }

        [DispatchTarget]
        private void OnTrainSpeedChanged(TrainStateInfo train, int speed) {
            InvalidateTrainState(train);
        }

        [DispatchTarget]
        private void OnTrainEnteredBlock(TrainStateInfo train, LayoutBlock block) {
            InvalidateTrainState(train);
        }

        [DispatchTarget]
        private void OnTrainSavedInCollection(TrainInCollectionInfo trainInCollection) {
            UpdateElements();
            xmlQueryList.ListBox.Invalidate();
        }

        [DispatchTarget]
        private void OnLocomotiveAddressChanged(LocomotiveInfo locomotive, int address) {
            UpdateElements();
            xmlQueryList.ListBox.Invalidate();
        }

        #region Item classes

        private class LocomotiveItem : IXmlQueryListBoxXmlElementItem {
            private readonly LocomotiveList list;

            public LocomotiveItem(LocomotiveList list, XmlQueryListItem queryItem, XmlElement element) {
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

        private class ListLayoutSimple : XmlQueryListLayout {
            public override string LayoutName => "Simple";

            public override void ApplyLayout(XmlQueryList list) {
                list.AddQuery("Locomotives", "Locomotive")?.Expand();
                list.AddQuery("Trains", "Train")?.Expand();
            }
        }

        private class ListLayoutByOrigin : XmlQueryListLayout {
            public override string LayoutName => "Locomotive origin";

            public override void ApplyLayout(XmlQueryList list) {
                list.AddQuery("European", "Locomotive[@Origin='Europe']");
                list.AddQuery("American", "Locomotive[@Origin='US']");
                list.AddQuery("Trains", "Train");
            }
        }

        private class ListLayoutByType : XmlQueryListLayout {
            public override string LayoutName => "Locomotive type";

            public override void ApplyLayout(XmlQueryList list) {
                list.AddQuery("Steam", "Locomotive[@Kind='Steam']");
                list.AddQuery("Diesel", "Locomotive[@Kind='Diesel']");
                list.AddQuery("Electric", "Locomotive[@Kind='Electric']");
                list.AddQuery("Sound Units", "Locomotive[@Kind='SoundUnit']");
                list.AddQuery("Trains", "Train");
            }
        }

        private class ListLayoutByStorage : XmlQueryListLayout {
            public override string LayoutName => "By locomotive storage file";

            public override void ApplyLayout(XmlQueryList list) {
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
