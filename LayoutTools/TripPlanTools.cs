using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.Components;

#pragma warning disable IDE0051, IDE0060

namespace LayoutManager.Tools {

    [LayoutModule("Trip Planning Tools", UserControl = false)]
    public class TripPlanningTools : ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>

        #region Save Trip plan

        [LayoutEvent("save-trip-plan")]
        private void saveTripPlan(LayoutEvent e) {
            TripPlanInfo tripPlan = (TripPlanInfo)e.Sender;
            Form parentForm = (Form)e.Info;
            Dialogs.SaveTripPlan saveTripPlan = new Dialogs.SaveTripPlan();

            Guid editedTripPlanID = Guid.Empty;

            if (tripPlan.FromCatalog) {
                editedTripPlanID = tripPlan.Id;
                saveTripPlan.TripPlanName = tripPlan.Name;
            }
            else
                saveTripPlan.TripPlanName = createDefaultName(tripPlan);

            if (saveTripPlan.ShowDialog(parentForm) == DialogResult.OK) {
                TripPlanCatalogInfo tripPlanCatalog = LayoutModel.StateManager.TripPlansCatalog;
                bool doSave = true;
                TripPlanInfo existingTripPlan = tripPlanCatalog.TripPlans[saveTripPlan.TripPlanName];

                if (existingTripPlan != null) {
                    if (editedTripPlanID != existingTripPlan.Id && MessageBox.Show(parentForm, "A trip plan with name already exists, would you like to replace it?",
                        "Trip Plan already exist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        doSave = false;
                    else
                        tripPlanCatalog.TripPlans.Remove(existingTripPlan);
                }

                if (doSave) {
                    XmlElement savedTripPlanElement = (XmlElement)tripPlanCatalog.Element.OwnerDocument.ImportNode(tripPlan.Element, true);
                    TripPlanInfo savedTripPlan = new TripPlanInfo(savedTripPlanElement) {
                        Name = saveTripPlan.TripPlanName,
                        IconId = saveTripPlan.IconID
                    };

                    if (tripPlanCatalog.TripPlans.ContainsKey(savedTripPlan.Id))
                        tripPlanCatalog.TripPlans.Remove(savedTripPlan.Id);

                    tripPlanCatalog.TripPlans.Add(savedTripPlan);
                }
            }
        }

        private string createDefaultName(TripPlanInfo tripPlan) {
            int wayPointCount = tripPlan.Waypoints.Count;
            string name = "to " + tripPlan.Waypoints[wayPointCount - 1].Name;

            for (int i = 0; i < wayPointCount - 1; i++)
                name += (i == 0 ? " via " : ", ") + tripPlan.Waypoints[i].Name;

            if (tripPlan.IsCircular)
                name += " (circular)";

            return name;
        }

        #endregion

        #region Get train target speed

        [LayoutEvent("get-train-target-speed")]
        private void getTrainTargetSpeed(LayoutEvent e) {
            TrainCommonInfo train = (TrainStateInfo)e.Sender;
            IWin32Window owner = (IWin32Window)e.Info;

            Dialogs.GetTargetSpeed d = new Dialogs.GetTargetSpeed(train);

            if (d.ShowDialog(owner) == DialogResult.OK)
                e.Info = true;
            else
                e.Info = false;
        }

        #endregion

        #region Applicable Trip Plans

        class ApplicableTripPlansData : LayoutXmlWrapper {
            LayoutBlock locomotiveBlock;
            LayoutComponentConnectionPoint locomotiveFront;
            LayoutBlock lastCarBlock;
            LayoutComponentConnectionPoint lastCarFront;
            Guid routeOwner = Guid.Empty;
            bool allTripPlans = false;
            readonly bool staticGrade = false;
            bool calculatePenalty = true;
            IRoutePlanningServices _tripPlanningServices = null;

            public ApplicableTripPlansData(XmlElement element) : base(element) {
                staticGrade = XmlConvert.ToBoolean(GetAttribute("StaticGrade", "true"));
            }

            public IRoutePlanningServices TripPlanningServices {
                get {
                    if (_tripPlanningServices == null)
                        _tripPlanningServices = (IRoutePlanningServices)EventManager.Event(new LayoutEvent("get-route-planning-services", this));
                    return _tripPlanningServices;
                }
            }

            public LayoutBlock LocomotiveBlock {
                get {
                    return locomotiveBlock;
                }

                set {
                    locomotiveBlock = value;
                }
            }

            public LayoutComponentConnectionPoint LocomotiveFront {
                get {
                    return locomotiveFront;
                }

                set {
                    locomotiveFront = value;
                }
            }

            public LayoutBlock LastCarBlock {
                get {
                    return lastCarBlock;
                }

                set {
                    lastCarBlock = value;
                }
            }

            public LayoutComponentConnectionPoint LastCarFront {
                get {
                    return lastCarFront;
                }

                set {
                    lastCarFront = value;
                }
            }

            public Guid RouteOwner {
                get {
                    return routeOwner;
                }

                set {
                    routeOwner = value;
                }
            }

            public bool AllTripPlans {
                get {
                    return allTripPlans;
                }

                set {
                    allTripPlans = value;
                }
            }

            public bool CalculatePenalty {
                get {
                    return calculatePenalty;
                }

                set {
                    calculatePenalty = value;
                }
            }

            public TrainStateInfo Train {
                set {
                    LocomotiveBlock = value.LocomotiveBlock;
                    LocomotiveFront = value.LocomotiveLocation.DisplayFront;
                    LastCarBlock = value.LastCarBlock;
                    LastCarFront = value.LastCarLocation.DisplayFront;
                    RouteOwner = value.Id;

                    LocateRealTrainFront(value);
                }
            }

            // Verify that the blocks reachable from the locomotive front do not contain any part of the train. If they do, switch
            // between the last car and locomotive blocks. So at the end, the train can always move forward from the locomotive block
            // and backward from the last car block.
            protected void LocateRealTrainFront(TrainStateInfo train) {
                LayoutBlockDefinitionComponent locomotiveBlockInfo = locomotiveBlock.BlockDefinintion;

                LayoutBlockEdgeBase[] blockEdges = locomotiveBlockInfo.GetBlockEdges(locomotiveBlockInfo.GetConnectionPointIndex(LocomotiveFront));

                bool switchFrontAndLastCar = false;

                foreach (LayoutBlockEdgeBase blockEdge in blockEdges) {
                    LayoutBlock otherBlock = locomotiveBlock.OtherBlock(blockEdge);

                    if (train.LocationOfBlock(otherBlock) != null) {
                        switchFrontAndLastCar = true;
                        break;
                    }
                }

                if (switchFrontAndLastCar) {
                    LayoutBlock tBlock = locomotiveBlock;

                    locomotiveBlock = lastCarBlock;
                    lastCarBlock = tBlock;

                    LayoutComponentConnectionPoint tFront = locomotiveFront;

                    locomotiveFront = lastCarFront;
                    lastCarFront = tFront;
                }
            }

            class Direction {
                readonly bool shouldReverse;

                public Direction(bool shouldReverse) {
                    this.shouldReverse = shouldReverse;
                }

                public LocomotiveOrientation Get(LocomotiveOrientation direction) {
                    if (shouldReverse)
                        return direction == LocomotiveOrientation.Forward ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward;
                    else
                        return direction;
                }

                public LocomotiveOrientation Get(TripPlanWaypointInfo wayPoint) => Get(wayPoint.Direction);
            }

            protected bool IsTripPlanApplicable(TripPlanInfo tripPlan, bool shouldReverse) {
                Direction d = new Direction(shouldReverse);
                LayoutTrackComponent sourceTrack = null;
                LayoutComponentConnectionPoint sourceFront = LayoutComponentConnectionPoint.Empty;

                for (int i = 0; i < tripPlan.Waypoints.Count; i++) {
                    IList<TripPlanWaypointInfo> wayPoints = tripPlan.Waypoints;

                    if (i == 0) {
                        LayoutBlock sourceBlock = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveBlock : LastCarBlock;

                        sourceTrack = sourceBlock.BlockDefinintion.Track;
                        sourceFront = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveFront : LastCarFront;
                    }

                    BestRoute bestRoute = TripPlanningServices.FindBestRoute(sourceTrack, sourceFront, d.Get(wayPoints[i]), wayPoints[i].Destination, RouteOwner, wayPoints[i].TrainStopping);

                    if (!bestRoute.Quality.IsValidRoute)
                        return false;
                    else {
                        // Avoid selecting null routes
                        if (tripPlan.Waypoints.Count == 1 && bestRoute.TrackEdges.Count == 1)
                            return false;

                        sourceTrack = bestRoute.DestinationTrack;
                        sourceFront = bestRoute.DestinationFront;
                    }
                }

                return true;
            }

            protected RouteQuality VerifyTripPlan(TripPlanInfo tripPlan, bool shouldReverese) {
                Direction d = new Direction(shouldReverese);
                LayoutTrackComponent sourceTrack = null;
                LayoutComponentConnectionPoint sourceFront = LayoutComponentConnectionPoint.Empty;
                TripBestRouteResult result;
                RouteQuality quality = new RouteQuality(RouteOwner);

                for (int i = 0; i < tripPlan.Waypoints.Count; i++) {
                    TripBestRouteRequest request;
                    IList<TripPlanWaypointInfo> wayPoints = tripPlan.Waypoints;

                    if (i == 0) {
                        LayoutBlock sourceBlock = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveBlock : LastCarBlock;

                        sourceTrack = sourceBlock.BlockDefinintion.Track;
                        sourceFront = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveFront : LastCarFront;
                    }

                    request = new TripBestRouteRequest(RouteOwner, wayPoints[i].Destination, sourceTrack, sourceFront, d.Get(wayPoints[i]), staticGrade);
                    result = (TripBestRouteResult)EventManager.Event(new LayoutEvent("find-best-route-request", request));

                    if (result.BestRoute == null)
                        return result.Quality;
                    else {
                        // Avoid selecting null routes
                        if (tripPlan.Waypoints.Count == 1 && result.BestRoute.TrackEdges.Count == 1) {
                            quality.ClearanceQuality = RouteClearanceQuality.NoPath;
                            return quality;
                        }

                        sourceTrack = result.BestRoute.DestinationTrack;
                        sourceFront = result.BestRoute.DestinationFront;

                        quality.AddClearanceQuality(result.Quality.ClearanceQuality);
                        quality.Penalty += result.Quality.Penalty;
                    }
                }

                return quality;
            }

            public void FindApplicableTripPlans() {
                if (CalculatePenalty) {
                    RouteQuality quality = null;

                    foreach (TripPlanInfo tripPlan in LayoutModel.StateManager.TripPlansCatalog.TripPlans) {
                        if (allTripPlans || (quality = VerifyTripPlan(tripPlan, false)).IsValidRoute) {
                            XmlElement applicableTripPlanElement = Element.OwnerDocument.CreateElement("ApplicableTripPlan");

                            applicableTripPlanElement.SetAttribute("TripPlanID", XmlConvert.ToString(tripPlan.Id));
                            applicableTripPlanElement.SetAttribute("ShouldReverse", XmlConvert.ToString(false));

                            if (quality != null) {
                                applicableTripPlanElement.SetAttribute("Penalty", XmlConvert.ToString(quality.Penalty));
                                applicableTripPlanElement.SetAttribute("ClearanceQuality", quality.ClearanceQuality.ToString());
                            }

                            Element.AppendChild(applicableTripPlanElement);
                        }
                        else if ((quality = VerifyTripPlan(tripPlan, true)).IsValidRoute) {
                            XmlElement applicableTripPlanElement = Element.OwnerDocument.CreateElement("ApplicableTripPlan");

                            applicableTripPlanElement.SetAttribute("TripPlanID", XmlConvert.ToString(tripPlan.Id));
                            applicableTripPlanElement.SetAttribute("ShouldReverse", XmlConvert.ToString(true));
                            applicableTripPlanElement.SetAttribute("Penalty", XmlConvert.ToString(quality.Penalty));
                            applicableTripPlanElement.SetAttribute("ClearanceQuality", quality.ClearanceQuality.ToString());
                            Element.AppendChild(applicableTripPlanElement);
                        }

                        Application.DoEvents();
                    }

                    if (locomotiveBlock != null) {
                        Element.SetAttribute("LocomotiveBlockID", XmlConvert.ToString(locomotiveBlock.Id));
                        Element.SetAttribute("LocomotiveFront", LocomotiveFront.ToString());
                    }
                }
                else {
                    foreach (TripPlanInfo tripPlan in LayoutModel.StateManager.TripPlansCatalog.TripPlans) {
                        if (allTripPlans || IsTripPlanApplicable(tripPlan, false)) {
                            XmlElement applicableTripPlanElement = Element.OwnerDocument.CreateElement("ApplicableTripPlan");

                            applicableTripPlanElement.SetAttribute("TripPlanID", XmlConvert.ToString(tripPlan.Id));
                            applicableTripPlanElement.SetAttribute("ShouldReverse", XmlConvert.ToString(false));
                            Element.AppendChild(applicableTripPlanElement);
                        }
                        else if (IsTripPlanApplicable(tripPlan, true)) {
                            XmlElement applicableTripPlanElement = Element.OwnerDocument.CreateElement("ApplicableTripPlan");

                            applicableTripPlanElement.SetAttribute("TripPlanID", XmlConvert.ToString(tripPlan.Id));
                            applicableTripPlanElement.SetAttribute("ShouldReverse", XmlConvert.ToString(true));
                            Element.AppendChild(applicableTripPlanElement);
                        }

                        Application.DoEvents();
                    }

                    if (locomotiveBlock != null) {
                        Element.SetAttribute("LocomotiveBlockID", XmlConvert.ToString(locomotiveBlock.Id));
                        Element.SetAttribute("LocomotiveFront", LocomotiveFront.ToString());
                    }
                }
            }
        }

        [LayoutEvent("get-applicable-trip-plans-request")]
        private void getApplicableTripPlansRequest(LayoutEvent e) {
            XmlElement applicableTripPlansElement = (XmlElement)e.Info;

            if (e.Sender == null) {
                ApplicableTripPlansData applicableTripPlans = new ApplicableTripPlansData(applicableTripPlansElement) {
                    AllTripPlans = true,
                    CalculatePenalty = e.GetBoolOption("CalculatePenalty", true)
                };
                applicableTripPlans.FindApplicableTripPlans();
            }
            else if (e.Sender is TrainStateInfo train) {
                ApplicableTripPlansData applicableTripPlans = new ApplicableTripPlansData(applicableTripPlansElement) {
                    Train = train,
                    CalculatePenalty = e.GetBoolOption("CalculatePenalty", true)
                };
                applicableTripPlans.FindApplicableTripPlans();
            }
            else if (e.Sender is LayoutBlock block) {
                ApplicableTripPlansData applicableTripPlans = new ApplicableTripPlansData(applicableTripPlansElement);
                LayoutComponentConnectionPoint? front;

                if (e.HasOption("Front"))
                    front = LayoutComponentConnectionPoint.Parse(e.GetOption("Front"));
                else
                    front = EventManager.EventResultValueType<LayoutBlockDefinitionComponent, object, LayoutComponentConnectionPoint>("get-locomotive-front", block.BlockDefinintion, "");

                if (front.HasValue) {
                    applicableTripPlans.LocomotiveBlock = block;
                    applicableTripPlans.LastCarBlock = block;
                    applicableTripPlans.LocomotiveFront = front.Value;
                    applicableTripPlans.LastCarFront = front.Value;
                    applicableTripPlans.CalculatePenalty = e.GetBoolOption("CalculatePenalty", true);
                    applicableTripPlans.FindApplicableTripPlans();
                }
            }
            else if (e.Sender is LayoutBlockDefinitionComponent)
                e.Info = EventManager.Event(new LayoutEvent("get-applicable-trip-plans-request", ((LayoutBlockDefinitionComponent)e.Sender).Block, applicableTripPlansElement));
        }

        #endregion

        #region Check if trip plan destination is free

        // Check if a block is a free destination. It is not free, if there is a train standing on it which
        // is not assigned to an active trip plan, or if there is a train that executes a trip plan in which
        // this block is the only destination.
        private bool isBlockAfreeDestination(LayoutBlock block) {
            foreach (TrainLocationInfo trainLocation in block.Trains) {
                TripPlanAssignmentInfo tripPlanAssignment = (TripPlanAssignmentInfo)EventManager.Event(new LayoutEvent("get-train-active-trip", trainLocation.Train));

                if (tripPlanAssignment == null)
                    return false;           // This block has a train that is not engaged in active trip plan, so it is not free
            }

            // Check all active trip plans and ensure that non of them is destined to this block
            TripPlanAssignmentInfo[] activeTrips = (TripPlanAssignmentInfo[])EventManager.Event(new LayoutEvent("get-active-trips", this));

            foreach (TripPlanAssignmentInfo trip in activeTrips) {
                TripPlanInfo tripPlan = trip.TripPlan;

                if (!tripPlan.IsCircular) {
                    TripPlanWaypointInfo lastWayPoint = tripPlan.Waypoints[tripPlan.Waypoints.Count - 1];

                    if (lastWayPoint.Destination.BlockIdList.Count == 1 && lastWayPoint.Destination.BlockIdList[0] == block.Id)
                        return false;
                }
            }

            return true;
        }

        [LayoutEvent("check-trip-plan-destination-request")]
        private void checkTripPlanDestination(LayoutEvent e) {
            TripPlanInfo tripPlan = (TripPlanInfo)e.Sender;

            if (tripPlan.IsCircular)
                e.Info = true;          // Trip plan is circular, so there is no "clear" destination (TODO: is that the right policy?)
            else {
                TripPlanWaypointInfo lastWaypoint = tripPlan.Waypoints[tripPlan.Waypoints.Count - 1];

                e.Info = false;

                // Check that at least one location in the last destination is available.
                foreach (LayoutBlock block in lastWaypoint.Destination.Blocks) {
                    if (isBlockAfreeDestination(block)) {
                        e.Info = true;
                        break;
                    }
                }
            }
        }


        #endregion

        #region Trip planner debug code
        // DEBUG CODE

        class TestTripInfo {
            LayoutTrackComponent originTrack = null;
            LayoutComponentConnectionPoint originFront;
            LocomotiveOrientation direction = LocomotiveOrientation.Forward;
            LayoutTrackComponent destinationTrack = null;

            public LayoutTrackComponent OriginTrack {
                get {
                    return originTrack;
                }

                set {
                    originTrack = value;
                }
            }

            public LayoutComponentConnectionPoint OriginFront {
                get {
                    return originFront;
                }

                set {
                    originFront = value;
                }
            }

            public LocomotiveOrientation Direction {
                get {
                    return direction;
                }

                set {
                    direction = value;
                }
            }

            public LayoutTrackComponent DestinationTrack {
                get {
                    return destinationTrack;
                }

                set {
                    destinationTrack = value;
                }
            }
        }

        static readonly LayoutTraceSwitch traceShowTrackDescription = new LayoutTraceSwitch("ShowTrackDescription", "Show track description menu");

        [LayoutEvent("add-component-operation-context-menu-entries", Order = 900, SenderType = typeof(LayoutTrackComponent))]
        private void addTrackDescription(LayoutEvent e) {
            Menu menu = (Menu)e.Info;
            LayoutTrackComponent track = (LayoutTrackComponent)e.Sender;

            if (traceShowTrackDescription.TraceInfo)
                menu.MenuItems.Add("(Track " + track.FullDescription + ")");
        }

        #endregion

        #region Block Lock debug code

#if DEBUG_BLOCK_LOCK
		Dialogs.BlockLockTester	blockLockTester = null;

		[LayoutEvent("query-component-operation-context-menu", SenderType=typeof(LayoutBlockInfoComponent))]
		private void queryBlockInfoContextMenu(LayoutEvent e) {
			e.Info = true;
		}

		[LayoutEvent("add-component-operation-context-menu-entries", Order=1000, SenderType=typeof(LayoutBlockInfoComponent))]
		private void addAddToRequestMenu(LayoutEvent e) {
			Menu						menu = (Menu)e.Info;
			LayoutBlockInfoComponent	blockInfo = (LayoutBlockInfoComponent)e.Sender;

			menu.MenuItems.Add(new LayoutComponentMenuItem(blockInfo, "&Add to lock request", new EventHandler(this.addToLockRequest)));

			if(blockInfo.Block.IsLocked)
				menu.MenuItems.Add(new LayoutComponentMenuItem(blockInfo, "&Unlock block", new EventHandler(this.unlockBlock)));
		}

		private void addToLockRequest(object sender, EventArgs e) {
			LayoutBlockInfoComponent	blockInfo = (LayoutBlockInfoComponent)((LayoutComponentMenuItem)sender).Component;

			if(blockLockTester == null) {
				blockLockTester = new Dialogs.BlockLockTester(blockInfo);
				blockLockTester.Show();
			}
			else
				blockLockTester.AddBlockInfo(blockInfo);
		}

		private void unlockBlock(object sender, EventArgs e) {
			LayoutBlockInfoComponent	blockInfo = (LayoutBlockInfoComponent)((LayoutComponentMenuItem)sender).Component;

			blockInfo.EventManager.Event(new LayoutEvent(blockInfo.Block.ID, "free-layout-lock"));
		}

		[LayoutEvent("block-tester-dialog-closed")]
		private void blockTesterDialogClosed(LayoutEvent e) {
			blockLockTester = null;
		}
#endif

        #endregion
    }
}
