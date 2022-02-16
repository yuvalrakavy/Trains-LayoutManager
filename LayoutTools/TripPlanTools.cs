using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using MethodDispatcher;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;
using LayoutManager.Logic;

namespace LayoutManager.Tools {
    public static class DispatchSources {
        [DispatchSource]
        public static ApplicableTripPlansData GetApplicableTripPlansRequest(this Dispatcher d, object? target, bool calculatePenalty, LayoutComponentConnectionPoint? front) {
            return d[nameof(GetApplicableTripPlansRequest)].Call<ApplicableTripPlansData>(target, calculatePenalty, front);
        }
    }

    public class ApplicableTripPlanData {
        public Guid TripPlanId { get; set; }
        public bool ShouldReverse { get; set; }
        public int? Penalty { get; set; }
        public RouteClearanceQuality? ClearanceQuality { get; set; }
    }

    public class ApplicableTripPlansData {
        private readonly bool staticGrade = false;
        private IRoutePlanningServices? _tripPlanningServices = null;

        public ApplicableTripPlansData(bool staticGrade = true) {
            this.staticGrade = staticGrade;
            TripPlans = new();
        }

        public IRoutePlanningServices TripPlanningServices => _tripPlanningServices ??= Dispatch.Call.GetRoutePlanningServices();
        public Guid LocomtiveBlockId { get; set; }
        public LayoutBlock? LocomotiveBlock { get; set; }
        public LayoutBlock? LastCarBlock { get; set; }
        public LayoutComponentConnectionPoint LastCarFront { get; set; }
        public Guid RouteOwner { get; set; } = Guid.Empty;
        public bool AllTripPlans { get; set; } = false;
        public bool CalculatePenalty { get; set; } = true;
        public LayoutComponentConnectionPoint LocomotiveFront { get; set; }
        public List<ApplicableTripPlanData> TripPlans { get; private set; }

        public TrainStateInfo Train {
            set {
                LocomotiveBlock = value.LocomotiveBlock;
                LocomotiveFront = value.LocomotiveLocation?.DisplayFront ?? LayoutComponentConnectionPoint.Empty;
                LastCarBlock = value.LastCarBlock;
                LastCarFront = value.LastCarLocation?.DisplayFront ?? LayoutComponentConnectionPoint.Empty;
                RouteOwner = value.Id;

                LocateRealTrainFront(value);
            }
        }

        // Verify that the blocks reachable from the locomotive front do not contain any part of the train. If they do, switch
        // between the last car and locomotive blocks. So at the end, the train can always move forward from the locomotive block
        // and backward from the last car block.
        protected void LocateRealTrainFront(TrainStateInfo train) {
            var locomotiveBlock = Ensure.NotNull<LayoutBlock>(LocomotiveBlock);
            var locomotiveBlockInfo = locomotiveBlock.BlockDefinintion;

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

                LocomotiveBlock = LastCarBlock;
                LastCarBlock = tBlock;

                LayoutComponentConnectionPoint tFront = LocomotiveFront;

                LocomotiveFront = LastCarFront;
                LastCarFront = tFront;
            }
        }

        private class Direction {
            private readonly bool shouldReverse;

            public Direction(bool shouldReverse) {
                this.shouldReverse = shouldReverse;
            }

            public LocomotiveOrientation Get(LocomotiveOrientation direction) {
                return shouldReverse
                    ? direction == LocomotiveOrientation.Forward ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward
                    : direction;
            }

            public LocomotiveOrientation Get(TripPlanWaypointInfo wayPoint) => Get(wayPoint.Direction);
        }

        protected bool IsTripPlanApplicable(TripPlanInfo tripPlan, bool shouldReverse) {
            var d = new Direction(shouldReverse);
            LayoutTrackComponent? sourceTrack = null;
            LayoutComponentConnectionPoint sourceFront = LayoutComponentConnectionPoint.Empty;

            for (int i = 0; i < tripPlan.Waypoints.Count; i++) {
                IList<TripPlanWaypointInfo> wayPoints = tripPlan.Waypoints;

                if (i == 0) {
                    var sourceBlock = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveBlock : LastCarBlock;

                    sourceTrack = sourceBlock?.BlockDefinintion.Track;
                    sourceFront = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveFront : LastCarFront;
                }

                if (sourceTrack == null)
                    return false;

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
            Direction d = new(shouldReverese);
            LayoutTrackComponent? sourceTrack = null;
            LayoutComponentConnectionPoint sourceFront = LayoutComponentConnectionPoint.Empty;
            TripBestRouteResult result;
            RouteQuality quality = new(RouteOwner);

            for (int i = 0; i < tripPlan.Waypoints.Count; i++) {
                TripBestRouteRequest request;
                IList<TripPlanWaypointInfo> wayPoints = tripPlan.Waypoints;

                if (i == 0) {
                    var sourceBlock = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveBlock : LastCarBlock;

                    if (sourceBlock == null)
                        throw new LayoutException("Cannot figure out source block");

                    sourceTrack = sourceBlock.BlockDefinintion.Track;
                    sourceFront = (d.Get(wayPoints[0]) == LocomotiveOrientation.Forward) ? LocomotiveFront : LastCarFront;
                }

                if (sourceTrack == null)
                    break;

                request = new TripBestRouteRequest(RouteOwner, wayPoints[i].Destination, sourceTrack, sourceFront, d.Get(wayPoints[i]), staticGrade);
                result = Dispatch.Call.FindBestRouteRequest(request);

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
                RouteQuality? quality = null;

                foreach (TripPlanInfo tripPlan in LayoutModel.StateManager.TripPlansCatalog.TripPlans) {
                    if (AllTripPlans || (quality = VerifyTripPlan(tripPlan, shouldReverese: false)).IsValidRoute) {
                        TripPlans.Add(new ApplicableTripPlanData {
                            TripPlanId = tripPlan.Id,
                            ShouldReverse = false,
                            Penalty = quality?.Penalty,
                            ClearanceQuality = quality?.ClearanceQuality,
                        });
                    }
                    else if ((quality = VerifyTripPlan(tripPlan, shouldReverese: true)).IsValidRoute) {
                        TripPlans.Add(new ApplicableTripPlanData {
                            TripPlanId = tripPlan.Id,
                            ShouldReverse = true,
                            Penalty = quality?.Penalty,
                            ClearanceQuality = quality?.ClearanceQuality,
                        });
                    }

                    Application.DoEvents();
                }
            }
            else {
                foreach (TripPlanInfo tripPlan in LayoutModel.StateManager.TripPlansCatalog.TripPlans) {
                    if (AllTripPlans || IsTripPlanApplicable(tripPlan, shouldReverse: false)) {
                        TripPlans.Add(new ApplicableTripPlanData {
                            TripPlanId = tripPlan.Id,
                            ShouldReverse = false
                        });
                    }
                    else if (IsTripPlanApplicable(tripPlan, shouldReverse: true)) {
                        TripPlans.Add(new ApplicableTripPlanData {
                            TripPlanId = tripPlan.Id,
                            ShouldReverse = true
                        });
                    }

                    Application.DoEvents();
                }

            }

            if (LocomotiveBlock != null)
                LocomtiveBlockId = LocomotiveBlock.Id;
        }
    }


    [LayoutModule("Trip Planning Tools", UserControl = false)]
    public class TripPlanningTools : ILayoutModuleSetup {
        /// <summary>
        /// Required designer variable.
        /// </summary>

        #region Save Trip plan

        [LayoutEvent("save-trip-plan")]
        private void SaveTripPlan(LayoutEvent e) {
            var tripPlan = Ensure.NotNull<TripPlanInfo>(e.Sender);
            var parentForm = Ensure.NotNull<Form>(e.Info);
            using Dialogs.SaveTripPlan saveTripPlan = new();

            Guid editedTripPlanID = Guid.Empty;

            if (tripPlan.FromCatalog) {
                editedTripPlanID = tripPlan.Id;
                saveTripPlan.TripPlanName = tripPlan.Name;
            }
            else
                saveTripPlan.TripPlanName = CreateDefaultName(tripPlan);

            if (saveTripPlan.ShowDialog(parentForm) == DialogResult.OK) {
                TripPlanCatalogInfo tripPlanCatalog = LayoutModel.StateManager.TripPlansCatalog;
                bool doSave = true;
                TripPlanInfo? existingTripPlan = tripPlanCatalog.TripPlans[saveTripPlan.TripPlanName];

                if (existingTripPlan != null) {
                    if (editedTripPlanID != existingTripPlan.Id && MessageBox.Show(parentForm, "A trip plan with name already exists, would you like to replace it?",
                        "Trip Plan already exist", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        doSave = false;
                    else
                        tripPlanCatalog.TripPlans.Remove(existingTripPlan);
                }

                if (doSave) {
                    XmlElement savedTripPlanElement = (XmlElement)tripPlanCatalog.Element.OwnerDocument.ImportNode(tripPlan.Element, true);
                    TripPlanInfo savedTripPlan = new(savedTripPlanElement) {
                        Name = saveTripPlan.TripPlanName,
                        IconId = saveTripPlan.IconID
                    };

                    if (tripPlanCatalog.TripPlans.ContainsKey(savedTripPlan.Id))
                        tripPlanCatalog.TripPlans.Remove(savedTripPlan.Id);

                    tripPlanCatalog.TripPlans.Add(savedTripPlan);
                }
            }
        }

        private string CreateDefaultName(TripPlanInfo tripPlan) {
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
        private void GetTrainTargetSpeed(LayoutEvent e) {
            var train = Ensure.NotNull<TrainStateInfo>(e.Sender);
            var owner = Ensure.NotNull<IWin32Window>(e.Info);

            var d = new Dialogs.GetTargetSpeed(train);

            e.Info = d.ShowDialog(owner) == DialogResult.OK ? true : (object)false;
        }

        #endregion

        #region Applicable Trip Plans

        #endregion

        [DispatchTarget]
        private ApplicableTripPlansData GetApplicableTripPlansRequest(object? target, bool calculatePenalty, LayoutComponentConnectionPoint? front) {
            switch (target) {
                case null: {
                        var applicableTripPlans = new ApplicableTripPlansData {
                            AllTripPlans = true,
                            CalculatePenalty = calculatePenalty,
                        };
                        applicableTripPlans.FindApplicableTripPlans();
                        return applicableTripPlans;
                    }

                case TrainStateInfo train: {
                        ApplicableTripPlansData applicableTripPlans = new() {
                            Train = train,
                            CalculatePenalty = calculatePenalty,
                        };
                        applicableTripPlans.FindApplicableTripPlans();
                        return applicableTripPlans;
                    }

                case LayoutBlock block: {
                        ApplicableTripPlansData applicableTripPlans = new();
                        front ??= Dispatch.Call.GetLocomotiveFront(block.BlockDefinintion, "");

                        if (front.HasValue) {
                            applicableTripPlans.LocomotiveBlock = block;
                            applicableTripPlans.LastCarBlock = block;
                            applicableTripPlans.LocomotiveFront = front.Value;
                            applicableTripPlans.LastCarFront = front.Value;
                            applicableTripPlans.CalculatePenalty = calculatePenalty;
                            applicableTripPlans.FindApplicableTripPlans();
                        }

                        return applicableTripPlans;
                    }

                case LayoutBlockDefinitionComponent blockDefinition:
                    return GetApplicableTripPlansRequest(blockDefinition.Block, calculatePenalty, front);

                default:
                    throw new LayoutException($"Invalid target type for {nameof(GetApplicableTripPlansRequest)}");
            }
        }

        #region Check if trip plan destination is free

        // Check if a block is a free destination. It is not free, if there is a train standing on it which
        // is not assigned to an active trip plan, or if there is a train that executes a trip plan in which
        // this block is the only destination.
        private bool IsBlockAfreeDestination(LayoutBlock block) {
            foreach (TrainLocationInfo trainLocation in block.Trains) {
                var tripPlanAssignment = Dispatch.Call.GetActiveTrip(trainLocation.Train) ?? Dispatch.Call.GetEditedTrip(trainLocation.Train);

                if (tripPlanAssignment == null)
                    return false;           // This block has a train that is not engaged in active trip plan, so it is not free
            }

            // Check all active trip plans and ensure that non of them is destined to this block
            var activeTrips = Dispatch.Call.GetActiveTrips();

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
        private void CheckTripPlanDestination(LayoutEvent e) {
            var tripPlan = Ensure.NotNull<TripPlanInfo>(e.Sender);

            if (tripPlan.IsCircular)
                e.Info = true;          // Trip plan is circular, so there is no "clear" destination (TODO: is that the right policy?)
            else {
                TripPlanWaypointInfo lastWaypoint = tripPlan.Waypoints[tripPlan.Waypoints.Count - 1];

                e.Info = false;

                // Check that at least one location in the last destination is available.
                foreach (LayoutBlock block in lastWaypoint.Destination.Blocks) {
                    if (IsBlockAfreeDestination(block)) {
                        e.Info = true;
                        break;
                    }
                }
            }
        }

        #endregion


        #region Trip planner debug code
        // DEBUG CODE

        private class TestTripInfo {
            public LayoutTrackComponent? OriginTrack { get; set; } = null;

            public LayoutComponentConnectionPoint OriginFront { get; set; }

            public LocomotiveOrientation Direction { get; set; } = LocomotiveOrientation.Forward;

            public LayoutTrackComponent? DestinationTrack { get; set; } = null;
        }

        private static readonly LayoutTraceSwitch traceShowTrackDescription = new("ShowTrackDescription", "Show track description menu");

        [LayoutEvent("add-component-operation-context-menu-entries", Order = 900, SenderType = typeof(LayoutTrackComponent))]
        private void AddTrackDescription(LayoutEvent e) {
            var menu = Ensure.ValueNotNull<MenuOrMenuItem>(e.Info);
            var track = Ensure.NotNull<LayoutTextComponent>(e.Sender);

            if (traceShowTrackDescription.TraceInfo)
                menu.Items.Add("(Track " + track.FullDescription + ")");
        }

        #endregion
    }
}
