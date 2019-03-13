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
        public TripPlanTrainConditionInfo(XmlElement element) : base(element) {
        }

        public TripPlanTrainConditionInfo(XmlElement parentElement, string elementName) {
            Element = parentElement[elementName];

            if (Element == null) {
                Element = parentElement.OwnerDocument.CreateElement(elementName);
                parentElement.AppendChild(Element);
            }
        }

        public TripPlanTrainConditionScope ConditionScope {
            get {
                if (HasAttribute("Scope"))
                    return (TripPlanTrainConditionScope)Enum.Parse(typeof(TripPlanTrainConditionScope), GetAttribute("Scope"));
                return TripPlanTrainConditionScope.AllowIfTrue;
            }

            set {
                SetAttribute("Scope", value.ToString());
            }
        }

        public bool IsConditionEmpty => Element.ChildNodes.Count == 0;

        public XmlElement ConditionElement => Element;

        public XmlElement ConditionBodyElement {
            get {
                if (Element.ChildNodes.Count > 0)
                    return (XmlElement)Element.ChildNodes[0];
                else
                    return null;
            }
        }

        public virtual string GetDescription() {
            if (IsConditionEmpty)
                return "";
            else {
                LayoutConditionScript conditionScript = new LayoutConditionScript("Train Condition", ConditionElement);

                return conditionScript.Description;
            }
        }
    }

    public class TripPlanDestinationEntryInfo : TripPlanTrainConditionInfo {
        public TripPlanDestinationEntryInfo(XmlElement element) : base(element) {
        }

        public TripPlanDestinationEntryInfo(XmlElement parentElement, string name) : base(parentElement, name) {
        }

        public TripPlanDestinationEntryInfo(XmlElement parentElement, Guid blockID) : base(parentElement, "Block") {
            BlockId = blockID;
        }

        public Guid BlockId {
            get {
                return XmlConvert.ToGuid(GetAttribute("BlockID"));
            }

            set {
                SetAttribute("BlockID", XmlConvert.ToString(value));
            }
        }

        public override string GetDescription() {
            if (IsConditionEmpty)
                return "(Allow all trains)";
            else {
                string condition = base.GetDescription();

                if (ConditionScope == TripPlanTrainConditionScope.AllowIfTrue)
                    return "Allow " + condition;
                else
                    return "Disallow " + condition;
            }
        }
    }

    /// <summary>
    /// Wrapper for way point destination element, or for stored destinations.
    /// </summary>
    public class TripPlanDestinationInfo : LayoutInfo, IObjectHasId {
        List<TripPlanDestinationEntryInfo> entries;

        public TripPlanDestinationInfo(XmlElement element) : base(element) {
        }

        public TripPlanDestinationInfo(TripPlanWaypointInfo waypoint, LayoutBlockDefinitionComponent blockInfo) {
            Element = waypoint.Element.OwnerDocument.CreateElement("Destination");
            waypoint.Element.AppendChild(Element);

            Add(blockInfo);
        }

        public TripPlanDestinationEntryInfo Add(LayoutBlockDefinitionComponent blockInfo) {
            XmlElement entryElement = Element.OwnerDocument.CreateElement("Block");

            Element.AppendChild(entryElement);
            TripPlanDestinationEntryInfo entry = new TripPlanDestinationEntryInfo(entryElement) {
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
                TripPlanDestinationEntryInfo entry = new TripPlanDestinationEntryInfo(destinationEntryElement);

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
                List<Guid> blockIdList = new List<Guid>(Entries.Count);

                entries.ForEach(delegate (TripPlanDestinationEntryInfo entry) { blockIdList.Add(entry.BlockId); });
                return blockIdList;
            }
        }

        public IList<LayoutBlock> Blocks {
            get {
                List<LayoutBlock> blocks = new List<LayoutBlock>(Entries.Count);

                entries.ForEach(delegate (TripPlanDestinationEntryInfo entry) { blocks.Add(LayoutModel.Blocks[entry.BlockId]); });
                return blocks;
            }
        }

        public IList<LayoutBlockDefinitionComponent> BlockInfoList {
            get {
                List<LayoutBlockDefinitionComponent> blockInfos = new List<LayoutBlockDefinitionComponent>(Entries.Count);

                entries.ForEach(delegate (TripPlanDestinationEntryInfo entry) {

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
                LayoutSelection selection = new LayoutSelection();

                selection.Add<LayoutBlockDefinitionComponent>(BlockInfoList);
                return selection;
            }
        }

        public string Name {
            get {
                if (Element.HasAttribute("Name"))
                    return Element.GetAttribute("Name");
                else {
                    if (Count > 0) {
                        if (BlockInfoList.Count > 0) {
                            LayoutBlockDefinitionComponent blockInfo = BlockInfoList[0]; ;

                            if (LayoutModel.Areas.Count > 1)
                                return blockInfo.Spot.Area.Name + ": " + blockInfo.Name;
                            else
                                return blockInfo.Name;
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
                    Element.SetAttribute("Name", value);
            }
        }

        public bool HasName {
            get {
                if (Element.HasAttribute("Name"))
                    return true;
                return false;
            }

            set {
                if (value == false)
                    Element.RemoveAttribute("Name");
            }
        }

        public void Flush() {
            entries = null;
        }

        public TripPlanLocationSelectionMethod SelectionMethod {
            get {
                if (Element.HasAttribute("SelectionMethod"))
                    return (TripPlanLocationSelectionMethod)Enum.Parse(typeof(TripPlanLocationSelectionMethod),
                        Element.GetAttribute("SelectionMethod"));
                else
                    return TripPlanLocationSelectionMethod.ListOrder;
            }

            set {
                Element.SetAttribute("SelectionMethod", value.ToString());
            }
        }
    }

    public class TripPlanWaypointInfo : LayoutInfo {
        public TripPlanWaypointInfo(XmlElement element) : base(element) {
        }

        public TripPlanWaypointInfo(TripPlanInfo tripPlan, LayoutBlockDefinitionComponent blockInfo) {
            Element = tripPlan.Element.OwnerDocument.CreateElement("WayPoint");

            // This added new destination to the destination list
            new TripPlanDestinationInfo(this, blockInfo);
        }

        public TripPlanWaypointInfo(TripPlanInfo tripPlan) {
            Element = tripPlan.Element.OwnerDocument.CreateElement("WayPoint");
        }

        public TripPlanDestinationInfo Destination {
            get {
                XmlElement destinationElement = Element["Destination"];

                if (destinationElement == null) {
                    destinationElement = Element.OwnerDocument.CreateElement("Destination");
                    Element.AppendChild(destinationElement);
                }

                return new TripPlanDestinationInfo(destinationElement);
            }

            set {
                XmlElement destinationElement = Element["Destination"];

                if (destinationElement != null)
                    Element.RemoveChild(destinationElement);

                destinationElement = (XmlElement)value.Element.CloneNode(true);
                Element.AppendChild(destinationElement);
            }
        }

        public string Name => Destination.Name;

        public XmlElement StartCondition {
            get {
                XmlElement startConditionElement = Element["StartCondition"];

                if (startConditionElement != null && startConditionElement.ChildNodes.Count > 0)
                    return (XmlElement)startConditionElement.ChildNodes[0];
                return null;
            }

            set {
                XmlElement startConditionElement = Element["StartCondition"];

                if (value == null) {
                    if (startConditionElement != null)
                        Element.RemoveChild(startConditionElement); // No start condition
                }
                else {
                    if (startConditionElement == null) {
                        startConditionElement = Element.OwnerDocument.CreateElement("StartCondition");
                        Element.AppendChild(startConditionElement);
                    }

                    startConditionElement.RemoveAll();
                    startConditionElement.AppendChild(value);
                }
            }
        }

        public TripPlanInfo TripPlan {
            get {
                TripPlanInfo tripPlan = new TripPlanInfo((XmlElement)Element.ParentNode.ParentNode);

                return tripPlan;
            }
        }

        public bool TrainStopping {
            get {
                bool lastWaypoint = (XmlElement)TripPlan.WaypointsElement.LastChild == Element;

                return (StartCondition != null && !StartCondition.IsEmpty) || (lastWaypoint && !TripPlan.IsCircular);
            }
        }

        public string StartConditionDescription {
            get {
                if (StartCondition == null)
                    return "At once";
                else
                    return (string)EventManager.Event(new LayoutEvent("get-event-script-description", StartCondition));
            }
        }

        public XmlElement DriverInstructions {
            get {
                XmlElement driverInstructionsElement = Element["DriverInstructions"];

                if (driverInstructionsElement != null && driverInstructionsElement.ChildNodes.Count > 0)
                    return (XmlElement)driverInstructionsElement.ChildNodes[0];
                return null;
            }

            set {
                XmlElement driverInstructionsElement = Element["DriverInstructions"];

                if (value == null) {
                    if (driverInstructionsElement != null)
                        Element.RemoveChild(driverInstructionsElement); // No driver instructions
                }
                else {
                    if (driverInstructionsElement == null) {
                        driverInstructionsElement = Element.OwnerDocument.CreateElement("DriverInstructions");
                        Element.AppendChild(driverInstructionsElement);
                    }

                    driverInstructionsElement.RemoveAll();
                    driverInstructionsElement.AppendChild(value);
                }
            }
        }

        public string DriverInstructionsDescription {
            get {
                if (DriverInstructions == null)
                    return "";
                else
                    return (string)EventManager.Event(new LayoutEvent("get-event-script-description", DriverInstructions));
            }
        }

        public LocomotiveOrientation Direction {
            get {
                if (Element.HasAttribute("Direction"))
                    return (LocomotiveOrientation)Enum.Parse(typeof(LocomotiveOrientation), Element.GetAttribute("Direction"));
                else
                    return LocomotiveOrientation.Forward;
            }

            set {
                Element.SetAttribute("Direction", value.ToString());
            }
        }
    }

    public class TripPlanInfo : LayoutInfo, IObjectHasId, IObjectHasAttributes {
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
                List<TripPlanWaypointInfo> wayPoints = new List<TripPlanWaypointInfo>(Count);

                foreach (XmlElement wayPointElement in WaypointsElement)
                    wayPoints.Add(new TripPlanWaypointInfo(wayPointElement));

                return wayPoints.AsReadOnly();
            }
        }

        public bool IsCircular {
            get {
                return XmlConvert.ToBoolean(GetAttribute("IsCircular", "false"));
            }

            set {
                SetAttribute("IsCircular", value, removeIf: false);
            }
        }

        public string Name {
            get {
                return GetAttribute("Name", null);
            }

            set {
                SetAttribute("Name", value, removeIf: null);
            }
        }

        public Guid IconId {
            get {
                if (HasAttribute("IconID"))
                    return XmlConvert.ToGuid(GetAttribute("IconID"));
                else
                    return Guid.Empty;
            }

            set {
                SetAttribute("IconID", XmlConvert.ToString(value));
            }
        }

        public bool FromCatalog {
            get {
                return XmlConvert.ToBoolean(GetAttribute("FromCatalog", "false"));
            }

            set {
                SetAttribute("FromCatalog", value, removeIf: false);
            }
        }

        public LayoutSelection Selection {
            get {
                LayoutSelection selection = new LayoutSelection();

                foreach (TripPlanWaypointInfo wayPoint in Waypoints)
                    selection.Add(wayPoint.Destination.Selection);

                return selection;
            }
        }

        public LayoutPolicyIdCollection Policies => new LayoutPolicyIdCollection(Element);

        /// <summary>
        /// Add a new way point to the trip plan.
        /// </summary>
        /// <param name="blockInfo">Create a destination containing only this block</param>
        public TripPlanWaypointInfo Add(LayoutBlockDefinitionComponent blockInfo) {
            TripPlanWaypointInfo wayPoint = new TripPlanWaypointInfo(this, blockInfo);

            WaypointsElement.AppendChild(wayPoint.Element);
            return wayPoint;
        }

        public TripPlanWaypointInfo Add(TripPlanDestinationInfo destination) {
            TripPlanWaypointInfo wayPoint = new TripPlanWaypointInfo(this) {
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
        TrainStateInfo train;
        int currentWaypointIndex;

        public TripPlanAssignmentInfo(TripPlanInfo tripPlan, TrainStateInfo train) {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            doc.LoadXml("<TripPlanAssignment />");
            Element = doc.DocumentElement;

            Element.AppendChild(doc.ImportNode(tripPlan.Element, true));
            Element.SetAttribute("TrainID", XmlConvert.ToString(train.Id));
            Status = TripStatus.NotSubmitted;
        }

        public TripPlanAssignmentInfo(XmlElement element) : base(element) {
        }

        public TripPlanInfo TripPlan => new TripPlanInfo(Element["TripPlan"]);

        public Guid TrainId => XmlConvert.ToGuid(Element.GetAttribute("TrainID"));

        public TrainStateInfo Train {
            get {
                if (train == null)
                    train = LayoutModel.StateManager.Trains[TrainId];

                return train;
            }
        }

        public TripStatus Status {
            get {
                return (TripStatus)Enum.Parse(typeof(TripStatus), GetAttribute("Status", "NotSubmitted"));
            }

            [LayoutEventDef("trip-status-changed", Role = LayoutEventRole.Notification, SenderType = typeof(TripPlanAssignmentInfo), InfoType = typeof(TripStatus))]
            set {
                TripStatus oldStatus = Status;

                SetAttribute("Status", value.ToString());
                EventManager.Event(new LayoutEvent("trip-status-changed", this, oldStatus));
                Train.Redraw();
            }
        }

        public int CurrentWaypointIndex {
            get {
                return currentWaypointIndex;
            }

            set {
                currentWaypointIndex = value;
            }
        }

        public TripPlanWaypointInfo CurrentWaypoint {
            get {
                if (CurrentWaypointIndex >= 0 && TripPlan.Waypoints.Count > 0)
                    return TripPlan.Waypoints[CurrentWaypointIndex];
                return null;
            }
        }

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

        protected override TripPlanDestinationInfo FromElement(XmlElement itemElement) => new TripPlanDestinationInfo(itemElement);

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

        protected override TripPlanInfo FromElement(XmlElement itemElement) => new TripPlanInfo(itemElement);

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
        readonly TripPlanCollection tripPlans;
        readonly DestinationCollection destinations;

        public TripPlanCatalogInfo(XmlElement element) : base(element) {
            tripPlans = new TripPlanCollection(this);
            destinations = new DestinationCollection(this);
        }

        public XmlElement TripPlansElement {
            get {
                XmlElement tripPlansElement = Element["TripPlans"];

                if (tripPlansElement == null) {
                    tripPlansElement = Element.OwnerDocument.CreateElement("TripPlans");
                    Element.AppendChild(tripPlansElement);
                }

                return tripPlansElement;
            }
        }

        public XmlElement DestinationsElement {
            get {
                XmlElement destinationsElement = Element["Destinations"];

                if (destinationsElement == null) {
                    destinationsElement = Element.OwnerDocument.CreateElement("Destinations");
                    Element.AppendChild(destinationsElement);
                }

                return destinationsElement;
            }
        }

        public TripPlanCollection TripPlans => tripPlans;

        public DestinationCollection Destinations => destinations;

        public void Clear() {
            tripPlans.Clear();
            destinations.Clear();
        }

        #region Integrity checking

        private class IntegrityContext {
            TripPlanInfo tripPlan;
            TripPlanWaypointInfo waypoint;
            TripPlanDestinationInfo destination;

            public void Message(string message) {
                string s = "";

                if (tripPlan != null)
                    s = "Trip plan '" + tripPlan.Name + "'";

                if (waypoint != null)
                    s += ", waypoint '" + waypoint.Destination.Name + "'";
                else if (destination != null)
                    s = "Destination '" + destination.Name + "'";

                if (!string.IsNullOrEmpty(s))
                    s += ": ";

                s += message;
                LayoutModuleBase.Message(s);
            }

            public TripPlanInfo TripPlan {
                get {
                    return tripPlan;
                }

                set {
                    tripPlan = value;
                }
            }

            public TripPlanWaypointInfo Waypoint {
                get {
                    return waypoint;
                }

                set {
                    waypoint = value;

                    if (waypoint == null)
                        destination = null;
                    else
                        destination = value.Destination;
                }
            }

            public TripPlanDestinationInfo Destination {
                get {
                    return destination;
                }

                set {
                    destination = value;
                }
            }
        }

        /// <summary>
        /// Check that each location indeed exist in the model.
        /// </summary>
        /// <returns>true - destination ok, false - destination became empty, should be deleted</returns>
        private bool checkDestinationIntegrity(IntegrityContext ic, TripPlanDestinationInfo destination, LayoutPhase phase) {
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

            if (destination.Count == 0)
                return false;
            return true;
        }

        public void CheckIntegrity(LayoutModuleBase mb, LayoutPhase phase) {
            IntegrityContext ic = new IntegrityContext();
            ArrayList tripPlanRemoveList = new ArrayList();

            foreach (TripPlanInfo tripPlan in TripPlans) {
                List<TripPlanWaypointInfo> wayPointRemoveList = new List<TripPlanWaypointInfo>();

                ic.TripPlan = tripPlan;

                foreach (TripPlanWaypointInfo wayPoint in tripPlan.Waypoints) {
                    ic.Waypoint = wayPoint;

                    if (!checkDestinationIntegrity(ic, wayPoint.Destination, phase)) {
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

            ArrayList destinationRemoveList = new ArrayList();

            foreach (TripPlanDestinationInfo destination in Destinations) {
                ic.Destination = destination;

                if (!checkDestinationIntegrity(ic, destination, phase)) {
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
        Guid _routeOwner;
        readonly TripPlanDestinationInfo _destination;
        readonly ModelComponent _source;
        readonly LayoutComponentConnectionPoint _front;
        LocomotiveOrientation _direction;
        readonly bool _trainStopping;

        public TripBestRouteRequest(Guid routeOwner, TripPlanDestinationInfo destination, ModelComponent source, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, bool trainStopping) {
            _routeOwner = routeOwner;
            _destination = destination;
            _source = source;
            _front = front;
            _direction = direction;
            _trainStopping = trainStopping;
        }

        public Guid RouteOwner => _routeOwner;

        public TripPlanDestinationInfo Destination => _destination;

        public ModelComponent Source => _source;

        public LayoutComponentConnectionPoint Front => _front;

        public LocomotiveOrientation Direction {
            get {
                return _direction;
            }

            set {
                _direction = value;
            }
        }

        public bool TrainStopping => _trainStopping;
    }

    public class TripBestRouteResult {
        readonly ITripRoute _bestRoute;
        readonly RouteQuality _quality;
        bool _shouldReverse;

        public TripBestRouteResult(ITripRoute bestRoute, RouteQuality quality, bool shouldReverse) {
            _bestRoute = bestRoute;
            _quality = quality;
            _shouldReverse = shouldReverse;
        }

        public ITripRoute BestRoute => _bestRoute;

        public RouteQuality Quality => _quality;

        public bool ShouldReverse {
            get {
                return _shouldReverse;
            }

            set {
                _shouldReverse = value;
            }
        }
    }

    #endregion

    #region Class representing image list (list of icons)

    public class TripPlanIconListInfo : LayoutInfo {
        ImageList largeIconImageList;
        Dictionary<Guid, int> iconIdMap;
        bool iconListModified;

        public TripPlanIconListInfo(XmlElement element) : base(element) {
        }

        public TripPlanIconListInfo(XmlElement parent, string elementName) : base(parent, elementName) {
        }

        protected void CreateIconIdMap() {
            if (iconIdMap == null) {
                iconIdMap = new Dictionary<Guid, int>(Element.ChildNodes.Count);

                int iconIndex = 0;
                foreach (XmlElement iconElement in Element)
                    iconIdMap.Add(XmlConvert.ToGuid(iconElement.GetAttribute("ID")), iconIndex++);
            }
        }

        public IDictionary<Guid, int> IConIdMap {
            get {
                CreateIconIdMap();
                return iconIdMap;
            }
        }

        private ImageList createImageList(int width, int height) {
            ImageList imageList = new ImageList {
                ImageSize = new Size(height, width),
                ColorDepth = ColorDepth.Depth24Bit
            };

            foreach (XmlElement iconElement in Element) {
                Icon icon;

                using (MemoryStream s = new MemoryStream(Convert.FromBase64String(iconElement.InnerText))) {
                    icon = new Icon(s);
                }

                imageList.Images.Add(icon);
            }

            return imageList;
        }

        public ImageList LargeIconImageList {
            get {
                if (largeIconImageList == null)
                    largeIconImageList = createImageList(32, 32);
                return largeIconImageList;
            }
        }

        /// <summary>
        /// Given iconID returns its icon index
        /// </summary>
        public int this[Guid iconId] {
            get {

                CreateIconIdMap();
                if (iconIdMap.TryGetValue(iconId, out int iconIndex))
                    return iconIndex;
                return 0;
            }
        }

        /// <summary>
        /// Given an iconIndex, return its icon ID
        /// </summary>
        public Guid this[int iconIndex] => XmlConvert.ToGuid(((XmlElement)Element.ChildNodes[iconIndex]).GetAttribute("ID"));

        /// <summary>
        /// Add Icon
        /// </summary>
        /// <param name="icon"></param>
        /// <returns>The new Icon ID</returns>
        public Guid Add(Icon icon) {
            XmlElement iconElement = Element.OwnerDocument.CreateElement("Icon");
            Guid iconID = Guid.NewGuid();

            iconElement.SetAttribute("ID", XmlConvert.ToString(iconID));

            using (MemoryStream s = new MemoryStream()) {
                icon.Save(s);

                iconElement.InnerText = Convert.ToBase64String(s.GetBuffer());
            }

            Element.AppendChild(iconElement);
            iconIdMap = null;               // force rebuilding the icon ID to index map

            // Add to the image list(s)
            LargeIconImageList.Images.Add(icon);
            iconListModified = true;

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
            iconListModified = true;
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
        public bool IconListModified {
            get {
                return iconListModified;
            }

            set {
                iconListModified = value;
            }
        }
    }
    #endregion

}
