using System;
using System.Xml;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using LayoutManager.Components;
using System.Collections.Generic;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Model {
    /// <summary>
    /// How to select a location from destination with more than one location
    /// </summary>
    public enum TripPlanLocationSelectionMethod {
        ListOrder, Random
    };

    public enum TripPlanTrainConditionScope {
        AllowIfTrue, DisallowIfTrue
    }

    public class TripPlanTrainConditionInfo : LayoutXmlWrapper {
        private const string A_Scope = "Scope";

        public TripPlanTrainConditionInfo(XmlElement element) : base(element) {
        }

        public TripPlanTrainConditionInfo(XmlElement parentElement, string elementName) {
            OptionalElement = parentElement[elementName];

            if (OptionalElement == null) {
                Element = parentElement.OwnerDocument.CreateElement(elementName);
                parentElement.AppendChild(Element);
            }
        }

        public TripPlanTrainConditionScope ConditionScope {
            get => AttributeValue(A_Scope).Enum<TripPlanTrainConditionScope>() ?? TripPlanTrainConditionScope.AllowIfTrue;
            set => SetAttributeValue(A_Scope, value);
        }

        public bool IsConditionEmpty => Element.ChildNodes.Count == 0;

        public XmlElement ConditionElement => Element;

        public XmlElement ConditionBodyElement => Element.ChildNodes.Count > 0 ? (XmlElement)Element.ChildNodes[0] : null;

        public virtual string GetDescription() {
            if (IsConditionEmpty)
                return "";
            else {
                LayoutConditionScript conditionScript = new("Train Condition", ConditionElement);

                return conditionScript.Description;
            }
        }
    }

    public class TripPlanDestinationEntryInfo : TripPlanTrainConditionInfo {
        private const string A_BlockId = "BlockID";

        public TripPlanDestinationEntryInfo(XmlElement element) : base(element) {
        }

        public TripPlanDestinationEntryInfo(XmlElement parentElement, string name) : base(parentElement, name) {
        }

        public TripPlanDestinationEntryInfo(XmlElement parentElement, Guid blockID) : base(parentElement, "Block") {
            BlockId = blockID;
        }

        public Guid BlockId {
            get => (Guid)AttributeValue(A_BlockId);
            set => SetAttributeValue(A_BlockId, value);
        }

        public override string GetDescription() {
            if (IsConditionEmpty)
                return "(Allow all trains)";
            else {
                string condition = base.GetDescription();

                return ConditionScope == TripPlanTrainConditionScope.AllowIfTrue ? "Allow " + condition : "Disallow " + condition;
            }
        }
    }

    /// <summary>
    /// Wrapper for way point destination element, or for stored destinations.
    /// </summary>
    public class TripPlanDestinationInfo : LayoutInfo, IObjectHasId {
        private const string A_Name = "Name";
        private const string A_SelectionMethod = "SelectionMethod";
        private const string E_Destination = "Destination";
        private const string E_Block = "Block";
        private List<TripPlanDestinationEntryInfo> entries;

        public TripPlanDestinationInfo(XmlElement element) : base(element) {
        }

        public TripPlanDestinationInfo(TripPlanWaypointInfo waypoint, LayoutBlockDefinitionComponent blockInfo) {
            Element = waypoint.Element.OwnerDocument.CreateElement(E_Destination);
            waypoint.Element.AppendChild(Element);

            Add(blockInfo);
        }

        public TripPlanDestinationEntryInfo Add(LayoutBlockDefinitionComponent blockInfo) {
            XmlElement entryElement = Element.OwnerDocument.CreateElement(E_Block);

            Element.AppendChild(entryElement);
            TripPlanDestinationEntryInfo entry = new(entryElement) {
                BlockId = blockInfo.Block.Id
            };
            Flush();

            return entry;
        }

        public TripPlanDestinationEntryInfo Add(TripPlanDestinationEntryInfo entry) {
            Element.AppendChild(entry.Element);

            return entry;
        }

        public void Remove(Guid blockId) {
            foreach (XmlElement destinationEntryElement in Element) {
                TripPlanDestinationEntryInfo entry = new(destinationEntryElement);

                if (entry.BlockId == blockId) {
                    Element.RemoveChild(entry.Element);
                    Flush();
                    break;
                }
            }
        }

        public void Clear() {
            Element.InnerXml = "";
            Flush();
        }

        public int Count => Element.ChildNodes.Count;

        public IList<TripPlanDestinationEntryInfo> Entries {
            get {
                if (entries == null) {
                    entries = new List<TripPlanDestinationEntryInfo>(Count);

                    foreach (XmlElement destinationEntryElement in Element)
                        entries.Add(new TripPlanDestinationEntryInfo(destinationEntryElement));
                }

                return entries;
            }
        }

        public IList<Guid> BlockIdList {
            get {
                List<Guid> blockIdList = new(Entries.Count);

                entries.ForEach((TripPlanDestinationEntryInfo entry) => blockIdList.Add(entry.BlockId));
                return blockIdList;
            }
        }

        public IList<LayoutBlock> Blocks {
            get {
                List<LayoutBlock> blocks = new(Entries.Count);

                entries.ForEach((TripPlanDestinationEntryInfo entry) => blocks.Add(LayoutModel.Blocks[entry.BlockId]));
                return blocks;
            }
        }

        public IList<LayoutBlockDefinitionComponent> BlockInfoList {
            get {
                List<LayoutBlockDefinitionComponent> blockInfos = new(Entries.Count);

                entries.ForEach((TripPlanDestinationEntryInfo entry) => {
                    if (LayoutModel.Blocks.TryGetValue(entry.BlockId, out LayoutBlock block)) {
                        if (block.BlockDefinintion != null)
                            blockInfos.Add(block.BlockDefinintion);
                    }
                });

                return blockInfos;
            }
        }

        public LayoutSelection Selection {
            get {
                LayoutSelection selection = new();

                selection.Add<LayoutBlockDefinitionComponent>(BlockInfoList);
                return selection;
            }
        }

        public string Name {
            get {
                if (Element.HasAttribute(A_Name))
                    return Element.GetAttribute(A_Name);
                else {
                    if (Count > 0) {
                        if (BlockInfoList.Count > 0) {
                            LayoutBlockDefinitionComponent blockInfo = BlockInfoList[0];

                            return LayoutModel.Areas.Count > 1 ? blockInfo.Spot.Area.Name + ": " + blockInfo.Name : blockInfo.Name;
                        }
                        else
                            return "Unknown block";
                    }
                    else
                        return "";
                }
            }

            set {
                if (Count == 0 || value != BlockInfoList[0].Name)
                    Element.SetAttribute(A_Name, value);
            }
        }

        public bool HasName {
            get {
                return Element.HasAttribute(A_Name);
            }

            set {
                if (!value)
                    Element.RemoveAttribute(A_Name);
            }
        }

        public void Flush() {
            entries = null;
        }

        public TripPlanLocationSelectionMethod SelectionMethod {
            get => AttributeValue(A_SelectionMethod).Enum<TripPlanLocationSelectionMethod>() ?? TripPlanLocationSelectionMethod.ListOrder;
            set => Element.SetAttribute(A_SelectionMethod, value.ToString());
        }
    }

    public class TripPlanWaypointInfo : LayoutInfo {
        private const string E_WayPoint = "WayPoint";
        private const string E_Destination = "Destination";
        private const string E_StartCondition = "StartCondition";
        private const string E_DriverInstructions = "DriverInstructions";
        private const string A_Direction = "Direction";

        public TripPlanWaypointInfo(XmlElement element) : base(element) {
        }

        public TripPlanWaypointInfo(TripPlanInfo tripPlan, LayoutBlockDefinitionComponent blockInfo) {
            Element = tripPlan.Element.OwnerDocument.CreateElement(E_WayPoint);

            // This added new destination to the destination list
            _ = new TripPlanDestinationInfo(this, blockInfo);
        }

        public TripPlanWaypointInfo(TripPlanInfo tripPlan) {
            Element = tripPlan.Element.OwnerDocument.CreateElement(E_WayPoint);
        }

        public TripPlanDestinationInfo Destination {
            get {
                XmlElement destinationElement = Element[E_Destination];

                if (destinationElement == null) {
                    destinationElement = Element.OwnerDocument.CreateElement(E_Destination);
                    Element.AppendChild(destinationElement);
                }

                return new TripPlanDestinationInfo(destinationElement);
            }

            set {
                XmlElement destinationElement = Element[E_Destination];

                if (destinationElement != null)
                    Element.RemoveChild(destinationElement);

                destinationElement = (XmlElement)value.Element.CloneNode(true);
                Element.AppendChild(destinationElement);
            }
        }

        public string Name => Destination.Name;

        public XmlElement StartCondition {
            get {
                XmlElement startConditionElement = Element[E_StartCondition];

                return startConditionElement != null && startConditionElement.ChildNodes.Count > 0
                    ? (XmlElement)startConditionElement.ChildNodes[0]
                    : null;
            }

            set {
                XmlElement startConditionElement = Element[E_StartCondition];

                if (value == null) {
                    if (startConditionElement != null)
                        Element.RemoveChild(startConditionElement); // No start condition
                }
                else {
                    if (startConditionElement == null) {
                        startConditionElement = Element.OwnerDocument.CreateElement(E_StartCondition);
                        Element.AppendChild(startConditionElement);
                    }

                    startConditionElement.RemoveAll();
                    startConditionElement.AppendChild(value);
                }
            }
        }

        public TripPlanInfo TripPlan {
            get {
                TripPlanInfo tripPlan = new((XmlElement)Element.ParentNode.ParentNode);

                return tripPlan;
            }
        }

        public bool TrainStopping {
            get {
                bool lastWaypoint = (XmlElement)TripPlan.WaypointsElement.LastChild == Element;

                return (StartCondition != null && !StartCondition.IsEmpty) || (lastWaypoint && !TripPlan.IsCircular);
            }
        }

        public string StartConditionDescription => StartCondition == null
                    ? "At once"
                    : (string)EventManager.Event(new LayoutEvent("get-event-script-description", StartCondition));

        public XmlElement DriverInstructions {
            get {
                XmlElement driverInstructionsElement = Element[E_DriverInstructions];

                return driverInstructionsElement != null && driverInstructionsElement.ChildNodes.Count > 0
                    ? (XmlElement)driverInstructionsElement.ChildNodes[0]
                    : null;
            }

            set {
                XmlElement driverInstructionsElement = Element[E_DriverInstructions];

                if (value == null) {
                    if (driverInstructionsElement != null)
                        Element.RemoveChild(driverInstructionsElement); // No driver instructions
                }
                else {
                    if (driverInstructionsElement == null) {
                        driverInstructionsElement = Element.OwnerDocument.CreateElement(E_DriverInstructions);
                        Element.AppendChild(driverInstructionsElement);
                    }

                    driverInstructionsElement.RemoveAll();
                    driverInstructionsElement.AppendChild(value);
                }
            }
        }

        public string DriverInstructionsDescription => DriverInstructions == null
                    ? ""
                    : (string)EventManager.Event(new LayoutEvent("get-event-script-description", DriverInstructions));

        public LocomotiveOrientation Direction {
            get => AttributeValue(A_Direction).Enum<LocomotiveOrientation>() ?? LocomotiveOrientation.Forward;
            set => Element.SetAttribute(A_Direction, value.ToString());
        }
    }

    public class TripPlanInfo : LayoutInfo, IObjectHasId, IObjectHasAttributes {
        private const string A_IsCircular = "IsCircular";
        private const string A_Name = "Name";
        private const string A_IconId = "IconID";
        private const string A_FromCatalog = "FromCatalog";

        public TripPlanInfo(XmlElement element) : base(element) {
        }

        public TripPlanInfo() {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            doc.LoadXml("<TripPlan><WayPoints /></TripPlan>");
            Element = doc.DocumentElement;

            // Enable the policies which are on by default
            foreach (LayoutPolicyInfo policy in LayoutModel.StateManager.TripPlanPolicies)
                if (policy.Apply)
                    Policies.Add(policy.Id);
        }

        public bool HasAttributes => new AttributesOwner(Element).HasAttributes;

        public AttributesInfo Attributes => new AttributesOwner(Element).Attributes;

        public XmlElement WaypointsElement {
            get {
                XmlElement waypointsElement = Element["WayPoints"];

                if (waypointsElement == null) {
                    waypointsElement = Element.OwnerDocument.CreateElement("WayPoints");
                    Element.AppendChild(waypointsElement);
                }

                return waypointsElement;
            }
        }

        /// <summary>
        /// The number of way points
        /// </summary>
        public int Count => WaypointsElement.ChildNodes.Count;

        /// <summary>
        /// The way points that are part of this trip plan
        /// </summary>
        public IList<TripPlanWaypointInfo> Waypoints {
            get {
                List<TripPlanWaypointInfo> wayPoints = new(Count);

                foreach (XmlElement wayPointElement in WaypointsElement)
                    wayPoints.Add(new TripPlanWaypointInfo(wayPointElement));

                return wayPoints.AsReadOnly();
            }
        }

        public bool IsCircular {
            get => (bool?)AttributeValue(A_IsCircular) ?? false;
            set => SetAttributeValue(A_IsCircular, value, removeIf: false);
        }

        public string Name {
            get => GetAttribute(A_Name);
            set => SetAttributValue(A_Name, value, removeIf: null);
        }

        public Guid IconId {
            get => (Guid?)AttributeValue(A_IconId) ?? Guid.Empty;
            set => SetAttributeValue(A_IconId, value);
        }

        public bool FromCatalog {
            get => (bool?)AttributeValue(A_FromCatalog) ?? false;
            set => SetAttributeValue(A_FromCatalog, value, removeIf: false);
        }

        public LayoutSelection Selection {
            get {
                LayoutSelection selection = new();

                foreach (TripPlanWaypointInfo wayPoint in Waypoints)
                    selection.Add(wayPoint.Destination.Selection);

                return selection;
            }
        }

        public LayoutPolicyIdCollection Policies => new(Element);

        /// <summary>
        /// Add a new way point to the trip plan.
        /// </summary>
        /// <param name="blockInfo">Create a destination containing only this block</param>
        public TripPlanWaypointInfo Add(LayoutBlockDefinitionComponent blockInfo) {
            TripPlanWaypointInfo wayPoint = new(this, blockInfo);

            WaypointsElement.AppendChild(wayPoint.Element);
            return wayPoint;
        }

        public TripPlanWaypointInfo Add(TripPlanDestinationInfo destination) {
            TripPlanWaypointInfo wayPoint = new(this) {
                Destination = destination
            };
            WaypointsElement.AppendChild(wayPoint.Element);
            return wayPoint;
        }

        public void Remove(TripPlanWaypointInfo waypoint) {
            WaypointsElement.RemoveChild(waypoint.Element);
        }

        public void Reverse() {
            foreach (TripPlanWaypointInfo wayPoint in Waypoints)
                wayPoint.Direction = wayPoint.Direction == LocomotiveOrientation.Forward ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward;
        }

        public void Dump() {
            foreach (TripPlanWaypointInfo wayPoint in Waypoints)
                Trace.WriteLine("   to " + wayPoint.Destination.Name + "  -" + (wayPoint.Direction == LocomotiveOrientation.Forward ? "Forward" : "Reverse") + "- " + (wayPoint.StartCondition != null ? wayPoint.StartConditionDescription : ""));
        }
    }

    public enum TripStatus {
        NotSubmitted, Go, PrepareStop, Stopped, WaitLock, WaitStartCondition, Suspended, Aborted, Done
    }

    /// <summary>
    /// Warpper to document describing assignment of trip plan to train and a train driver
    /// </summary>
    public class TripPlanAssignmentInfo : LayoutXmlWrapper {
        private const string E_TripPlan = "TripPlan";
        private const string A_TrainId = "TrainID";
        private const string A_Status = "Status";
        private TrainStateInfo train;

        public TripPlanAssignmentInfo(TripPlanInfo tripPlan, TrainStateInfo train) {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            doc.LoadXml("<TripPlanAssignment />");
            Element = doc.DocumentElement;

            Element.AppendChild(doc.ImportNode(tripPlan.Element, true));
            SetAttributeValue(A_TrainId, train.Id);
            Status = TripStatus.NotSubmitted;
        }

        public TripPlanAssignmentInfo(XmlElement element) : base(element) {
        }

        public TripPlanInfo TripPlan => new(Element[E_TripPlan]);

        public Guid TrainId => (Guid)AttributeValue(A_TrainId);

        public TrainStateInfo Train => train ??= LayoutModel.StateManager.Trains[TrainId];

        public TripStatus Status {
            get => AttributeValue(A_Status).Enum<TripStatus>() ?? TripStatus.NotSubmitted;

            [LayoutEventDef("trip-status-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TripPlanAssignmentInfo), InfoType = typeof(TripStatus))]
            set {
                TripStatus oldStatus = Status;

                SetAttributeValue(A_Status, value.ToString());
                EventManager.Event(new LayoutEvent("trip-status-changed", this, oldStatus));
                Train.Redraw();
            }
        }

        public int CurrentWaypointIndex { get; set; }

        public TripPlanWaypointInfo CurrentWaypoint => CurrentWaypointIndex >= 0 && TripPlan.Waypoints.Count > 0 ? TripPlan.Waypoints[CurrentWaypointIndex] : null;

        public bool CanBeCleared => Status == TripStatus.Done || Status == TripStatus.Aborted;

        public void Dump() {
            Trace.WriteLine("%%%%% Trip plan for train " + Train.DisplayName);
            TripPlan.Dump();
            Trace.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%");
        }
    }

    #region Trip Catalog Property type classses (DestinationCollection, TripPlanCollection)

    /// <summary>
    /// Smart destination collection
    /// </summary>
    public class DestinationCollection : XmlIndexedCollection<TripPlanDestinationInfo, Guid> {
        internal DestinationCollection(TripPlanCatalogInfo tripPlanCatalog) : base(tripPlanCatalog.DestinationsElement) {
        }

        protected override Guid GetItemKey(TripPlanDestinationInfo item) => item.Id;

        protected override XmlElement CreateElement(TripPlanDestinationInfo item) {
            throw new NotImplementedException();
        }

        protected override TripPlanDestinationInfo FromElement(XmlElement itemElement) => new(itemElement);

        public TripPlanDestinationInfo this[string name] {
            get {
                foreach (TripPlanDestinationInfo existingDestination in this)
                    if (existingDestination.Name == name)
                        return existingDestination;
                return null;
            }
        }
    }

    /// <summary>
    /// Trip plan in the catalog
    /// </summary>
    public class TripPlanCollection : XmlIndexedCollection<TripPlanInfo, Guid> {
        internal TripPlanCollection(TripPlanCatalogInfo tripPlanCatalog) : base(tripPlanCatalog.TripPlansElement) {
        }

        protected override Guid GetItemKey(TripPlanInfo item) => item.Id;

        protected override TripPlanInfo FromElement(XmlElement itemElement) => new(itemElement);

        public TripPlanInfo this[string name] {
            get {
                foreach (TripPlanInfo existingTripPlan in this)
                    if (existingTripPlan.Name == name)
                        return existingTripPlan;
                return null;
            }
        }
    }

    #endregion

    /// <summary>
    /// Represent the catalog of trip plans
    /// </summary>
    public class TripPlanCatalogInfo : LayoutInfo {
        private const string E_TripPlans = "TripPlans";
        private const string E_Destinations = "Destinations";

        public TripPlanCatalogInfo(XmlElement element) : base(element) {
            TripPlans = new TripPlanCollection(this);
            Destinations = new DestinationCollection(this);
        }

        public XmlElement TripPlansElement {
            get {
                XmlElement tripPlansElement = Element[E_TripPlans];

                if (tripPlansElement == null) {
                    tripPlansElement = Element.OwnerDocument.CreateElement(E_TripPlans);
                    Element.AppendChild(tripPlansElement);
                }

                return tripPlansElement;
            }
        }

        public XmlElement DestinationsElement {
            get {
                XmlElement destinationsElement = Element[E_Destinations];

                if (destinationsElement == null) {
                    destinationsElement = Element.OwnerDocument.CreateElement(E_Destinations);
                    Element.AppendChild(destinationsElement);
                }

                return destinationsElement;
            }
        }

        public TripPlanCollection TripPlans { get; }

        public DestinationCollection Destinations { get; }

        public void Clear() {
            TripPlans.Clear();
            Destinations.Clear();
        }

        #region Integrity checking

        private class IntegrityContext {
            private TripPlanWaypointInfo waypoint;

            public void Message(string message) {
                string s = "";

                if (TripPlan != null)
                    s = "Trip plan '" + TripPlan.Name + "'";

                if (waypoint != null)
                    s += ", waypoint '" + waypoint.Destination.Name + "'";
                else if (Destination != null)
                    s = "Destination '" + Destination.Name + "'";

                if (!string.IsNullOrEmpty(s))
                    s += ": ";

                s += message;
                LayoutModuleBase.Message(s);
            }

            public TripPlanInfo TripPlan { get; set; }

            public TripPlanWaypointInfo Waypoint {
                get => waypoint;

                set {
                    waypoint = value;

                    if (waypoint == null)
                        Destination = null;
                    else
                        Destination = value.Destination;
                }
            }

            public TripPlanDestinationInfo Destination { get; set; }
        }

        /// <summary>
        /// Check that each location indeed exist in the model.
        /// </summary>
        /// <returns>true - destination ok, false - destination became empty, should be deleted</returns>
        private static bool CheckDestinationIntegrity(IntegrityContext ic, TripPlanDestinationInfo destination, LayoutPhase phase) {
            List<Guid> removeList = null;

            foreach (Guid blockId in destination.BlockIdList)
                if (LayoutModel.Component<ModelComponent>(blockId, phase) == null) {
                    if (removeList == null)
                        removeList = new List<Guid>();

                    removeList.Add(blockId);
                }

            if (removeList != null) {
                foreach (Guid blockID in removeList) {
                    ic.Message("Trip plan catalog destination has a reference to non-existence block, removeing it");
                    destination.Remove(blockID);
                }
            }

            return destination.Count != 0;
        }

        public void CheckIntegrity(LayoutModuleBase mb, LayoutPhase phase) {
            IntegrityContext ic = new();
            ArrayList tripPlanRemoveList = new();

            foreach (TripPlanInfo tripPlan in TripPlans) {
                List<TripPlanWaypointInfo> wayPointRemoveList = new();

                ic.TripPlan = tripPlan;

                foreach (TripPlanWaypointInfo wayPoint in tripPlan.Waypoints) {
                    ic.Waypoint = wayPoint;

                    if (!CheckDestinationIntegrity(ic, wayPoint.Destination, phase)) {
                        ic.Message("All the locations (blocks) in this waypoint were removed, removing waypoint");
                        wayPointRemoveList.Add(wayPoint);
                    }

                    ic.Waypoint = null;
                }

                foreach (TripPlanWaypointInfo wayPoint in wayPointRemoveList)
                    tripPlan.Remove(wayPoint);

                if (tripPlan.Count == 0) {
                    ic.Message("All waypoint were removed, removing trip plan");
                    tripPlanRemoveList.Add(tripPlan);
                }

                ic.TripPlan = null;
            }

            foreach (TripPlanInfo tripPlan in tripPlanRemoveList)
                TripPlans.Remove(tripPlan);

            ArrayList destinationRemoveList = new();

            foreach (TripPlanDestinationInfo destination in Destinations) {
                ic.Destination = destination;

                if (!CheckDestinationIntegrity(ic, destination, phase)) {
                    ic.Message("All blocks in this destination are removed, removing destination");
                    destinationRemoveList.Add(destination);
                }

                ic.Destination = null;
            }

            foreach (TripPlanDestinationInfo destination in destinationRemoveList)
                Destinations.Remove(destination);
        }

        #endregion

    }

    #region Classes for find-best-route-request event

    public class TripBestRouteRequest {
        private readonly Guid _routeOwner;
        private readonly bool _trainStopping;

        public TripBestRouteRequest(Guid routeOwner, TripPlanDestinationInfo destination, ModelComponent source, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, bool trainStopping) {
            _routeOwner = routeOwner;
            Destination = destination;
            Source = source;
            Front = front;
            Direction = direction;
            _trainStopping = trainStopping;
        }

        public Guid RouteOwner => _routeOwner;

        public TripPlanDestinationInfo Destination { get; }

        public ModelComponent Source { get; }

        public LayoutComponentConnectionPoint Front { get; }

        public LocomotiveOrientation Direction { get; set; }

        public bool TrainStopping => _trainStopping;
    }

    public class TripBestRouteResult {
        public TripBestRouteResult(ITripRoute bestRoute, RouteQuality quality, bool shouldReverse) {
            BestRoute = bestRoute;
            Quality = quality;
            ShouldReverse = shouldReverse;
        }

        public ITripRoute BestRoute { get; }

        public RouteQuality Quality { get; }

        public bool ShouldReverse { get; set; }
    }

    #endregion

    #region Class representing image list (list of icons)

    public class TripPlanIconListInfo : LayoutInfo {
        private const string E_Icon = "Icon";
        private const string A_Id = "ID";
        private ImageList largeIconImageList;
        private Dictionary<Guid, int> iconIdMap;

        public TripPlanIconListInfo(XmlElement element) : base(element) {
        }

        public TripPlanIconListInfo(XmlElement parent, string elementName) : base(parent, elementName) {
        }

        protected void CreateIconIdMap() {
            if (iconIdMap == null) {
                iconIdMap = new Dictionary<Guid, int>(Element.ChildNodes.Count);

                int iconIndex = 0;
                foreach (XmlElement iconElement in Element)
                    iconIdMap.Add((Guid)iconElement.AttributeValue(A_Id), iconIndex++);
            }
        }

        public IDictionary<Guid, int> IConIdMap {
            get {
                CreateIconIdMap();
                return iconIdMap;
            }
        }

        private ImageList CreateImageList(int width, int height) {
            ImageList imageList = new() {
                ImageSize = new Size(height, width),
                ColorDepth = ColorDepth.Depth24Bit
            };

            foreach (XmlElement iconElement in Element) {
                Icon icon;

                using (MemoryStream s = new(Convert.FromBase64String(iconElement.InnerText))) {
                    icon = new Icon(s);
                }

                imageList.Images.Add(icon);
            }

            return imageList;
        }

        public ImageList LargeIconImageList => largeIconImageList ??= CreateImageList(32, 32);

        /// <summary>
        /// Given iconID returns its icon index
        /// </summary>
        public int this[Guid iconId] {
            get {
                CreateIconIdMap();
                return iconIdMap.TryGetValue(iconId, out int iconIndex) ? iconIndex : 0;
            }
        }

        /// <summary>
        /// Given an iconIndex, return its icon ID
        /// </summary>
        public Guid this[int iconIndex] => (Guid)((XmlElement)Element.ChildNodes[iconIndex]).AttributeValue(A_Id);

        /// <summary>
        /// Add Icon
        /// </summary>
        /// <param name="icon"></param>
        /// <returns>The new Icon ID</returns>
        public Guid Add(Icon icon) {
            XmlElement iconElement = Element.OwnerDocument.CreateElement(E_Icon);
            Guid iconID = Guid.NewGuid();

            iconElement.SetAttributeValue(A_Id, iconID);

            using (MemoryStream s = new()) {
                icon.Save(s);

                iconElement.InnerText = Convert.ToBase64String(s.GetBuffer());
            }

            Element.AppendChild(iconElement);
            iconIdMap = null;               // force rebuilding the icon ID to index map

            // Add to the image list(s)
            LargeIconImageList.Images.Add(icon);
            IconListModified = true;

            return iconID;
        }

        /// <summary>
        /// Remove an icon given its icon index
        /// </summary>
        /// <param name="iconIndex"></param>
        public void Remove(int iconIndex) {
            XmlElement iconElement = (XmlElement)Element.ChildNodes[iconIndex];

            Element.RemoveChild(iconElement);
            iconIdMap = null;               // force rebuilding the icon ID to index map

            // Remove from the image list(s)
            LargeIconImageList.Images.RemoveAt(iconIndex);
            IconListModified = true;
        }

        /// <summary>
        /// Remove an icon given its icon ID
        /// </summary>
        /// <param name="iconID"></param>
        public void Remove(Guid iconId) {
            Remove(this[iconId]);
        }

        /// <summary>
        /// return true if the icon list was modified and should be saved
        /// </summary>
        public bool IconListModified { get; set; }
    }
    #endregion

}
