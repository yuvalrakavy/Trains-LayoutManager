using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic {
    /// <summary>
    /// Summary description for TripPlanner.
    /// </summary>
    [LayoutModule("Route Planner")]
    internal class RoutePlanner : LayoutModuleBase, IRoutePlanningServices {
        private static readonly LayoutTraceSwitch traceRoutePlanning = new("RoutePlanning", "Route Planning");
        private static readonly LayoutTraceSwitch traceRoutingTable = new("RoutingTable", "Routing table construction");

        private RoutingTable routingTable = new();
        private ModelTopology? routingTableTopology;
        private bool aborted;

        [LayoutEvent("get-route-planning-services")]
        private void GetRoutePlanningServers(LayoutEvent e) {
            e.Info = (IRoutePlanningServices)this;
        }

        #region Build routing table

        /// <summary>
        /// Build routing table. The routing table provide for each switching state of each split point (e.g. turnout)
        /// a list (actually a hashtable) of all possible split points that are can be reached by passing in this
        /// split point using the given switch state.
        /// </summary>
        [LayoutEvent("check-layout", Order = 400)]
        private void BuildRoutingTable(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();
            ModelTopology currentTopology = new(phase);

            if (routingTableTopology == null || !currentTopology.Equals(routingTableTopology)) {
                if (!CalculateRoutingTable(phase)) {
                    routingTableTopology = null;
                    routingTable.Clear();

                    e.Info = false;
                    e.ContinueProcessing = false;
                }
                else {
                    routingTableTopology = currentTopology;
                    e.Info = true;

                    // Save the result
                    WriteRoutingTable(routingTableTopology, routingTable);
                }
            }
            else
                e.Info = true;

#if DEBUG
            if ((bool)e.Info)
                TraceRouteTableInfo(routingTable);
#endif
        }

        [LayoutEvent("new-layout-document")]
        private void NewLayoutDocument(LayoutEvent e) {
            routingTableTopology = null;
            routingTable.Clear();

            if (System.IO.File.Exists(LayoutController.LayoutRoutingFilename)) {
                try {
                    XmlTextReader r = new(LayoutController.LayoutRoutingFilename);

                    Trace.WriteLine("-Loading routing information from " + LayoutController.LayoutRoutingFilename);

                    do {
                        r.Read();
                    } while (r.NodeType != XmlNodeType.Element);

                    r.Read();

                    do {
                        if (r.NodeType == XmlNodeType.Element) {
                            if (r.Name == "Topology")
                                routingTableTopology = new ModelTopology(r);
                            else if (r.Name == "RoutingTable")
                                routingTable = new RoutingTable(r);
                            else
                                throw new FileParseException("Invalid routing file element " + r.Name);
                        }

                        r.Read();
                    } while (r.NodeType != XmlNodeType.EndElement);

                    r.Close();
                }
                catch (System.IO.IOException) {
                }
            }
        }

        private bool CalculateRoutingTable(LayoutPhase phase) {
            IEnumerable<IModelComponentIsMultiPath> multiPathTracks = LayoutModel.Components<IModelComponentIsMultiPath>(phase);

            Thread generateRoutingTableThread = new(new ParameterizedThreadStart(GenerateRoutingTableTask));

            aborted = false;
            generateRoutingTableThread.Name = "Generate routing table";
            generateRoutingTableThread.Start(phase);

            EventManager.Event(new LayoutEvent("show-routing-table-generation-dialog", this, multiPathTracks.Count()));

            return !aborted;
        }

        private static void WriteRoutingTable(ModelTopology routingTableTopology, RoutingTable routingTable) {
            try {
                XmlTextWriter w = new(LayoutController.LayoutRoutingFilename, System.Text.Encoding.UTF8);

                w.WriteStartDocument();
                w.WriteStartElement("RoutingInformation");
                routingTableTopology.WriteXml(w);
                routingTable.WriteXml(w);
                w.WriteEndElement();
                w.WriteEndDocument();
                w.Close();
            }
            catch (System.IO.IOException) {
            }
        }

#if DEBUG
        private static void TraceRouteTableInfo(RoutingTable routingTable) {
            int totalPenalty = 0;
            int nEntries = 0;

            foreach (RoutingTableEntry entry in routingTable.Entries) {
                for (int i = 0; i < entry.SwitchStateCount; i++) {
                    Dictionary<TrackEdgeId, int> stateRouting = entry[i];

                    if (stateRouting != null) {
                        foreach (int penalty in stateRouting.Values) {
                            totalPenalty += penalty;
                            nEntries++;
                        }
                    }
                }
            }

            Trace.WriteLine("There are " + nEntries + " entries in the routing table, total penalty is: " + totalPenalty);
        }
#endif

        [LayoutEvent("abort-routing-table-generation")]
        private void AbortRoutingTableGeneration(LayoutEvent e) {
            aborted = true;
        }

        private void GenerateRoutingTableTask(object? param) {
            var phase = Ensure.ValueNotNull<LayoutPhase>(param);
            ILayoutInterThreadEventInvoker invoker = (ILayoutInterThreadEventInvoker)EventManager.Instance.InterThreadEventInvoker;
            IEnumerable<IModelComponentIsMultiPath> multiPathTracks = LayoutModel.Components<IModelComponentIsMultiPath>(phase);
            ModelTopology topology = new(phase);

            topology.Compile();
            routingTable.Clear();

            if (traceRoutePlanning.TraceInfo)
                Trace.WriteLine("Routing for " + multiPathTracks.Count() + " multi-path tracks (turnouts)");

            foreach (IModelComponentIsMultiPath turnout in multiPathTracks) {
                if (aborted)
                    break;

                foreach (LayoutComponentConnectionPoint cp in turnout.ConnectionPoints) {
                    if (turnout.IsSplitPoint(cp)) {
                        if (!routingTable.Contains(new TrackEdgeId(turnout.Id, cp))) {
                            LayoutComponentConnectionPoint[] connectedCps = turnout.ConnectTo(cp, LayoutComponentConnectionType.Passage);
                            RoutingTableEntry entry = new(connectedCps.Length);
                            TrackEdge splitEdge = new((LayoutTrackComponent)turnout, cp);

                            Trace.WriteLineIf(traceRoutingTable.TraceVerbose, "buildRoutingTable: create new routing entry for " + new TrackEdgeId(turnout.Id, cp).ToString());

                            for (int iState = 0; iState < connectedCps.Length; iState++) {
                                if (aborted)
                                    break;

                                GetSplitsInfo si = new(topology);
                                GetReachableSplits(si, new TrackEdgeId(splitEdge), iState, 0);

                                if (si.ReachableSplits.Count > 0) {
                                    entry[iState] = si.ReachableSplits;
                                    entry[iState].Remove(new TrackEdgeId(turnout.Id, cp));  //TODO: Check this
                                }
                            }

                            routingTable.Add(new TrackEdgeId(turnout.Id, cp), entry);
                        }
                    }
                }

                invoker.QueueEvent(new LayoutEvent("routing-table-generation-progress", this));
            }

            invoker.QueueEvent(new LayoutEvent("routing-table-generation-done", this));
        }

        private class GetSplitsInfo {
            public GetSplitsInfo(ModelTopology topology) {
                this.Topology = topology;
            }

            public Dictionary<TrackEdgeId, int> ReachableSplits { get; } = new Dictionary<TrackEdgeId, int>();

            public ModelTopology Topology { get; }
        }

        private void GetReachableSplits(GetSplitsInfo si, TrackEdgeId splitEdgeID, int switchState, int penalty) {
            ModelTopologyEntry splitEdgeEntry = si.Topology[splitEdgeID];

            if (splitEdgeEntry != null)
                GetReachableSplits(si, splitEdgeEntry, switchState, penalty);
        }

        private void GetReachableSplits(GetSplitsInfo si, ModelTopologyEntry splitEdgeEntry, int switchState, int penalty) {
            if (splitEdgeEntry.Visited)
                return;

            ModelTopologyConnectionEntry topologyConnectionEntry = splitEdgeEntry[switchState];

            if (topologyConnectionEntry == null)
                return;

            TrackEdgeId destination = topologyConnectionEntry.Destination;

            penalty += topologyConnectionEntry.Penalty;

            if (si.ReachableSplits.TryGetValue(destination, out int bestPenalty)) {
                if (penalty < bestPenalty) {
                    bestPenalty = penalty;
                    si.ReachableSplits[destination] = bestPenalty;  // Found a better path to this split point
                }
                else
                    return;
            }
            else
                si.ReachableSplits.Add(destination, penalty);

            ModelTopologyEntry? destinationEntry = topologyConnectionEntry.DestinationTopologyEntry;

            if (destinationEntry != null && !destinationEntry.Visited) {
                int switchStateCount = destinationEntry.SwitchingStateCount;
                splitEdgeEntry.Visited = true;

                for (int iState = 0; iState < switchStateCount; iState++)
                    GetReachableSplits(si, destinationEntry, iState, penalty);

                splitEdgeEntry.Visited = false;
            }
        }

        [LayoutEvent("dump-routing-table")]
        private void DumpRoutingTable(LayoutEvent e) {
            routingTable.Dump();
        }

        #endregion

        #region Figure out routing target

        private enum FirstBlockHandling {
            Ignore, AvoidIfNonWaitable, Processed
        }

        private class TargetScanEntry {
            public TrackEdge Edge;
            public RouteQuality Quality;
            public FirstBlockHandling FirstBlock;

            public TargetScanEntry(TrackEdge edge, FirstBlockHandling firstBlock, RouteQuality quality) {
                this.FirstBlock = firstBlock;
                this.Edge = edge;
                this.Quality = (RouteQuality)quality.Clone();
            }

            public void MoveTo(TrackEdge newEdge) {
                LayoutBlock newEdgeBlock = newEdge.Track.GetBlock(newEdge.ConnectionPoint);
                LayoutBlock edgeBlock = Edge.Track.GetBlock(Edge.ConnectionPoint);

                if (newEdgeBlock.Id != edgeBlock.Id) {
                    Quality.AddQuality(newEdgeBlock);

                    if (FirstBlock == FirstBlockHandling.AvoidIfNonWaitable) {
                        // If the destination block (edgeBlock) is non-occuapncy detection block and the block edge between
                        // the blocks is not a sensor, big hit on quality since train will not be detected when it reaches the
                        // destination
                        if (edgeBlock.OccupancyBlock == null && Edge.Track.TrackContactComponent == null)
                            Quality.AddClearanceQuality(RouteClearanceQuality.TrainWillNotBeDetected);

                        // If the block the train is coming from (newEdgeBlock...) is non waitable, give big hit on the quality
                        // since the train will not be able to stop there
                        if (!newEdgeBlock.BlockDefinintion.Info.CanTrainWait)
                            Quality.AddClearanceQuality(RouteClearanceQuality.DestinationWillBlockOtherTrains);

                        FirstBlock = FirstBlockHandling.Processed;
                    }
                }

                Edge = newEdge;
            }
        }

        private void AddTargets(IList<RouteTarget> targets, TrackEdge sourceEdge, TripPlanDestinationEntryInfo? destinationEntryInfo, LayoutTrackComponent targetTrack, Guid routeOwner, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, int initalPenalty, bool trainStopping) {
            Stack<TargetScanEntry> scanStack = new();

            if (targetTrack.ConnectTo(front, LayoutComponentConnectionType.Topology).Length == 0)
                throw new LayoutException(targetTrack, "Invalid target front: " + front.ToString() + " is not a connected");

            TrackEdge scanFrom = new(targetTrack, front);
            TargetScanEntry scanFromEntry = new(scanFrom, trainStopping ? FirstBlockHandling.AvoidIfNonWaitable : FirstBlockHandling.Ignore, new RouteQuality(routeOwner, scanFrom.Track.GetBlock(scanFrom.ConnectionPoint)));

            scanFromEntry.Quality.Penalty += initalPenalty;
            scanStack.Push(scanFromEntry);

            while (scanStack.Count > 0) {
                bool isTarget = false;

                TargetScanEntry entry = scanStack.Pop();

                if (entry.Edge.Track == sourceEdge.Track) {
                    // Got to the source, in this case the destination itself is a target
                    TrackEdge targetEdge = new TrackEdge(targetTrack, front).OtherSide;
                    RouteTarget newTarget = new(destinationEntryInfo, targetTrack, targetEdge, -1, targetEdge.ConnectionPoint, direction, entry.Quality);

                    targets.Clear();
                    targets.Add(newTarget);

                    break;      // direct path found!
                }
                else if (entry.Edge.Track is IModelComponentIsMultiPath switchingTrack) // Check if the edge is one possible target of a multi-path track (a nice name for turnout...)
                                                                                        // If it is, then it is a target, the trip planner can use the routing table to find a path
                                                                                        // to this turnout, and from there, there is only one path that leads to the target.
{
                    LayoutComponentConnectionPoint[] tipCps = entry.Edge.Track.ConnectTo(entry.Edge.ConnectionPoint, LayoutComponentConnectionType.Passage);

                    foreach (LayoutComponentConnectionPoint tipCp in tipCps) {
                        if (switchingTrack.IsSplitPoint(tipCp)) {
                            // It is a target!
                            isTarget = true;

                            RouteTarget newTarget = new(destinationEntryInfo, targetTrack, new TrackEdge(entry.Edge.Track, tipCp), entry.Edge.Track.GetSwitchState(tipCp, entry.Edge.ConnectionPoint), front, direction, entry.Quality);

                            if (!targets.Contains(newTarget))
                                targets.Add(newTarget);
                        }
                    }
                }

                if (!isTarget) {
                    TrackEdge edge = entry.Edge;

                    if (edge != TrackEdge.Empty) {
                        LayoutComponentConnectionPoint[] connectedCp = edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage);

                        for (int i = 0; i < connectedCp.Length; i++) {
                            TrackEdge connectedEdge = edge.Track.GetConnectedComponentEdge(connectedCp[i]);

                            if (i == connectedCp.Length - 1) {
                                if (connectedEdge != TrackEdge.Empty) {
                                    entry.MoveTo(connectedEdge);
                                    scanStack.Push(entry);
                                }
                            }
                            else
                                scanStack.Push(new TargetScanEntry(connectedEdge, entry.FirstBlock, entry.Quality));
                        }
                    }
                }
            }
        }

        private void AddTargets(BestRoute bestRoute, TrackEdge sourceEdge, TripPlanDestinationEntryInfo? destinationEntryInfo, ModelComponent destinationComponent, int initialPenatly, bool trainStopping) {
            LayoutTrackComponent destinationTrack = BestRoute.TrackOf(destinationComponent);

            AddTargets(
                targets: bestRoute.Targets,
                sourceEdge: sourceEdge,
                destinationEntryInfo: destinationEntryInfo,
                targetTrack: BestRoute.TrackOf(destinationComponent),
                routeOwner: bestRoute.RouteOwner,
                front: destinationTrack.ConnectionPoints[0],
                direction: bestRoute.Direction,
                initalPenalty: initialPenatly,
                trainStopping: trainStopping);

            AddTargets(
                targets: bestRoute.Targets,
                sourceEdge: sourceEdge,
                destinationEntryInfo: destinationEntryInfo,
                targetTrack: BestRoute.TrackOf(destinationComponent),
                routeOwner: bestRoute.RouteOwner,
                front: destinationTrack.ConnectionPoints[1],
                direction: bestRoute.Direction,
                initalPenalty: initialPenatly,
                trainStopping: trainStopping);
        }

        private enum SortTargetsResult {
            NoPath,                 // No path to any target
            FindRoute,              // Targets are sorted, look for route
        }

        private class TargetQualityComparer : IComparer<RouteTarget> {
            #region IComparer<RouteTarget> Members

            public int Compare(RouteTarget? x, RouteTarget? y) => Ensure.NotNull<RouteTarget>(x).Quality.CompareTo(Ensure.NotNull<RouteTarget>(y).Quality);

            public bool Equals(RouteTarget? x, RouteTarget? y) => Compare(x, y) == 0;

            public int GetHashCode(RouteTarget obj) => obj.GetHashCode();

            #endregion
        }

        private SortTargetsResult SortTargets(IList<RouteTarget> targets, ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction) {
            LayoutTrackComponent sourceTrack = TrackOf(sourceComponent);
            TrackEdge edge = new(sourceTrack,
                (direction == LocomotiveOrientation.Backward) ? front : sourceTrack.ConnectTo(front, LayoutComponentConnectionType.Passage)[0]);
            IModelComponentIsMultiPath? turnout = null;
            TrackEdgeDictionary scannedEdges = new();
            SortTargetsResult result = SortTargetsResult.FindRoute;
            int penaltyToTarget = 0;
            bool arrived = false;

            while (result == SortTargetsResult.FindRoute && !arrived) {
                foreach (RouteTarget target in targets) {
                    if (edge.Track == target.DestinationTrack) {
                        arrived = true;
                        target.Arrived = true;
                        if (penaltyToTarget < target.Quality.Penalty)
                            target.Quality.Penalty = penaltyToTarget;
                    }
                }

                if (!arrived) {
                    turnout = edge.Track as IModelComponentIsMultiPath;

                    if (turnout != null) {
                        if (scannedEdges.ContainsKey(edge))
                            result = SortTargetsResult.NoPath;      // Loop, could not get to split point
                        else {
                            scannedEdges.Add(edge, null);

                            if (turnout.IsSplitPoint(edge.ConnectionPoint))
                                break;
                        }
                    }

                    edge = edge.Track.GetConnectedComponentEdge(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);

                    if (edge == TrackEdge.Empty)
                        result = SortTargetsResult.NoPath;
                    else {
                        var blockDefinition = edge.Track.BlockDefinitionComponent;

                        if (blockDefinition != null)
                            penaltyToTarget += blockDefinition.Info.LengthInCM;
                    }
                }
            }

            if (arrived)
                result = SortTargetsResult.FindRoute;
            else if (result == SortTargetsResult.FindRoute) {
                // At this point edge (and turnout) are on split point. For each target check what is the route with the best penalty

                if (turnout != null)
                    Trace.WriteLineIf(traceRoutePlanning.TraceVerbose, " Sorting tagets relative to " + ((ModelComponent)turnout).FullDescription);

                foreach (RouteTarget target in targets) {
                    int minPenalty = -1;

                    RoutingTableEntry routingEntry = (RoutingTableEntry)routingTable[edge];

                    if (edge == target.TargetEdge || target.SwitchState < 0)
                        minPenalty = 0;
                    else {
                        foreach (Dictionary<TrackEdgeId, int> reachableNodes in routingEntry) {
                            if (reachableNodes != null && reachableNodes.TryGetValue(target.TargetEdgeId, out int penalty)) {
                                if (minPenalty == -1 || penalty < minPenalty)
                                    minPenalty = penalty;
                            }
                        }
                    }

                    if (minPenalty != -1)
                        target.Quality.Penalty += minPenalty;
                    else
                        target.Quality.ClearanceQuality = RouteClearanceQuality.NoPath;

                    Trace.WriteLineIf(traceRoutePlanning.TraceVerbose, " Sort for target " + target.TargetEdge.ToString() + " min Penalty is " + minPenalty + " Clearance quality: " + target.Quality.ClearanceQuality);
                }

                // Finally sort all the targets based on their quality
                ((List<RouteTarget>)targets).Sort(new TargetQualityComparer());
            }

            return result;
        }

        #endregion

        #region Trip route planning

        private LayoutTrackComponent TrackOf(ModelComponent component) {
            var track = component.Spot.Track;

            if (track == null)
                throw new LayoutException(component, "This component is not on track");
            return track;
        }

        #endregion

        #region Find best route

        public BestRoute FindBestRoute(ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, ModelComponent destinationComponent, Guid routeOwner, bool trainStopping) {
            BestRoute bestRoute = new(sourceComponent, front, direction, routeOwner);

            if (traceRoutePlanning.TraceInfo) {
                var train = LayoutModel.StateManager.Trains[routeOwner];

                Trace.Write("Find route to destination component ");
                if (train != null)
                    Trace.Write("from " + sourceComponent.FullDescription + " ");

                var blockInfo = BestRoute.TrackOf(destinationComponent).BlockDefinitionComponent;

                if (blockInfo != null)
                    Trace.WriteLine(" to " + blockInfo.Name + " direction " + direction.ToString());
            }

            AddTargets(
                bestRoute: bestRoute,
                sourceEdge: new TrackEdge((LayoutTrackComponent)sourceComponent, front),
                destinationEntryInfo: null,
                destinationComponent: destinationComponent,
                initialPenatly: 0,
                trainStopping: trainStopping);

            FindBestRouteToTargets(bestRoute, trainStopping);

            return bestRoute;
        }

        public BestRoute FindBestRoute(ModelComponent sourceComponent, LayoutComponentConnectionPoint front, LocomotiveOrientation direction, TripPlanDestinationInfo destination, Guid routeOwner, bool trainStopping) {
            const int penaltyForDestinationRank = 200;
            const int penaltyForLengthDifference = 100;
            const int penaltyForLengthLimitViolation = 100000;
            var train = LayoutModel.StateManager.Trains[routeOwner];
            BestRoute bestRoute = new(sourceComponent, front, direction, routeOwner);
            var sourceTrack = sourceComponent switch
            {
                LayoutBlockDefinitionComponent blockDefinition => blockDefinition.Track,
                LayoutTrackComponent track => track,
                _ => throw new ArgumentException("Invalid sourceComponent type (not track or block definition)")
            };

            int penaltyDelta = destination.SelectionMethod == TripPlanLocationSelectionMethod.ListOrder ? penaltyForDestinationRank : 0;
            int destinationRankPenalty = 0;

            foreach (TripPlanDestinationEntryInfo entry in destination.Entries) {
                LayoutBlock block = LayoutModel.Blocks[entry.BlockId];

                if (block != null) {
                    LayoutBlockDefinitionComponent blockDefinition = block.BlockDefinintion;

                    if (blockDefinition != null) {
                        int lengthDifferencePenalty = 0;

                        if (train != null) {
                            if (train.Length <= blockDefinition.Info.TrainLengthLimit) {
                                if (destination.SelectionMethod != TripPlanLocationSelectionMethod.ListOrder)
                                    lengthDifferencePenalty = ((int)blockDefinition.Info.TrainLengthLimit - (int)train.Length) * penaltyForLengthDifference;
                            }
                            else
                                lengthDifferencePenalty = penaltyForLengthLimitViolation;
                        }

                        if (traceRoutePlanning.TraceInfo) {
                            Trace.Write("Find route ");
                            if (train != null)
                                Trace.Write("from " + sourceComponent.FullDescription + " ");
                            Trace.WriteLine(" to " + block.Name + " direction " + direction.ToString() + ", front " + front.ToString() + ", train " + (trainStopping ? "stops" : "does not stop"));
                        }

                        AddTargets(bestRoute, new TrackEdge(sourceTrack, front), entry, blockDefinition.Track, destinationRankPenalty + lengthDifferencePenalty, trainStopping);
                        destinationRankPenalty += penaltyDelta;
                    }
                }
            }

            FindBestRouteToTargets(bestRoute, trainStopping);

            return bestRoute;
        }

        private void FindBestRouteToTargets(BestRoute bestRoute, bool trainStopping) {
            SortTargetsResult sortResult = SortTargets(bestRoute.Targets, bestRoute.SourceComponent, bestRoute.SourceFront, bestRoute.Direction);

            if (traceRoutePlanning.TraceInfo) {
                Trace.WriteLine("-Targets " + sortResult + ":");
                foreach (RouteTarget t in bestRoute.Targets)
                    Trace.WriteLine("  " + t.TargetEdge.ToString() + " switch state " + t.SwitchState + " quality " + t.Quality);
            }

            if (sortResult == SortTargetsResult.NoPath)
                bestRoute.Quality.ClearanceQuality = RouteClearanceQuality.NoPath;
            else {
                // Try to find a free route
                FindBestRouteToTargets(bestRoute, trainStopping, RouteClearanceQuality.Free);

                // Could not find free route, try to find a valid route
                if (bestRoute.DestinationTarget == null)
                    FindBestRouteToTargets(bestRoute, trainStopping, RouteClearanceQuality.ValidRoute);
            }
        }

        private void FindBestRouteToTargets(BestRoute bestRoute, bool trainStopping, RouteClearanceQuality qualityThreshold) {
            foreach (RouteTarget target in bestRoute.Targets) {
                if (target.Quality.IsBetterThan(qualityThreshold)) {
                    FindBestRouteToTarget(bestRoute, target, trainStopping, qualityThreshold);

                    if (bestRoute.DestinationTarget != null)
                        break;          // Route was found!
                }
                else
                    break;
            }

            if (bestRoute.DestinationTarget == null)
                bestRoute.Quality.ClearanceQuality = RouteClearanceQuality.NoPath;
        }

        private void FindBestRouteToTarget(BestRoute bestRoute, RouteTarget target, bool trainStopping, RouteClearanceQuality qualityThreshold) {
            TrackEdge edge = new(bestRoute.SourceTrack,
                (bestRoute.Direction == LocomotiveOrientation.Backward) ? bestRoute.SourceFront : bestRoute.SourceTrack.ConnectTo(bestRoute.SourceFront, LayoutComponentConnectionType.Passage)[0]);
            RouteLookupState? lookupState = new(bestRoute.RouteOwner, edge);
            RouteBacktrackManager backtrackManager = new();

            Trace.WriteLineIf(traceRoutePlanning.TraceVerbose, ">>> FindBestRoute to target " + target.TargetEdge.ToString());

            if (bestRoute.SourceTrack != target.DestinationTrack) {
                while (lookupState != null) {
                    bool deadEnd = false;

                    if (lookupState.Edge == TrackEdge.Empty ||
                        (lookupState.Edge.Track is IModelComponentIsMultiPath multiPathComponent &&
                        multiPathComponent.IsSplitPoint(lookupState.Edge.ConnectionPoint) && target.SwitchState < 0))
                        deadEnd = true;
                    else {
                        LayoutBlock newBlock = lookupState.Edge.Track.GetBlock(lookupState.Edge.ConnectionPoint);
                        bool isDestinationBlock = newBlock == target.DestinationTrack.GetBlock(target.DestinationTrack.ConnectionPoints[0]);

                        if (lookupState.Edge == target.TargetEdge)
                            lookupState.GotToTarget = true;

                        if (newBlock != lookupState.Block) {
                            lookupState.Quality.AddQuality(newBlock);

                            Trace.WriteLineIf(traceRoutePlanning.TraceVerbose, "Block " + newBlock.BlockDefinintion.FullDescription + " accumulated quality " + lookupState.Quality.ToString());

                            if (lookupState.Quality.IsWorseThen(qualityThreshold)) {
                                Trace.WriteLineIf(traceRoutePlanning.TraceInfo, "Block " + newBlock.BlockDefinintion.FullDescription + " worse than quality threshold - backtrack");
                                deadEnd = true;
                            }
                            else {
                                // Check block pass condition, if the train is not going to stop in this block
                                if ((!isDestinationBlock || !trainStopping) && !CheckCondition(bestRoute, lookupState, newBlock, "Block pass", newBlock.BlockDefinintion.Info.TrainPassCondition)) {
                                    deadEnd = true;
                                    lookupState.Quality.AddClearanceQuality(RouteClearanceQuality.ForbiddenByBlockPassCondition);
                                }

                                if (!deadEnd && isDestinationBlock) {
                                    if (target.DestinationEntryInfo != null && !CheckCondition(bestRoute, lookupState, newBlock, "Destination", target.DestinationEntryInfo)) {
                                        deadEnd = true;
                                        lookupState.Quality.AddClearanceQuality(RouteClearanceQuality.ForbiddenByDestinationCondition);
                                    }

                                    if (!deadEnd && trainStopping) {
                                        if (bestRoute.Train != null && bestRoute.Train.Length > newBlock.BlockDefinintion.Info.TrainLengthLimit) {
                                            deadEnd = true;
                                            lookupState.Quality.AddClearanceQuality(RouteClearanceQuality.ForbiddenByTrainTooLong);
                                        }

                                        if (!deadEnd && !CheckCondition(bestRoute, lookupState, newBlock, "Block stop", newBlock.BlockDefinintion.Info.TrainStopCondition)) {
                                            deadEnd = true;
                                            lookupState.Quality.AddClearanceQuality(RouteClearanceQuality.ForbiddenByBlockStopCondition);
                                        }
                                    }
                                }
                            }

                            lookupState.Block = newBlock;
                        }

                        if (!deadEnd && isDestinationBlock && lookupState.GotToTarget)
                            break;          // Train arrived to destination!
                    }

                    if (deadEnd) {
                        // If got to a deadend, but the quality is better than the previous deadend, save it, so in case of no route
                        // the quality will indicate the best quality that could be achieved
                        if (bestRoute.Quality.ClearanceQuality == RouteClearanceQuality.FreeNow || lookupState.Quality.IsBetterThan(bestRoute.Quality.ClearanceQuality))
                            bestRoute.Quality = lookupState.Quality;

                        lookupState = backtrackManager.LookupState;
                        if (lookupState != null)
                            Trace.WriteLineIf(traceRoutePlanning.TraceInfo, "  Backtracking to " + lookupState.Edge.ToString());
                    }
                    else {
                        if (lookupState.Edge.Track is IModelComponentIsMultiPath turnout) {
                            if (lookupState.Edge == target.TargetEdge) {
                                lookupState.SwitchStates.Add(target.SwitchState);
                                lookupState.Edge = lookupState.Edge.Track.GetConnectedComponentEdge(turnout.ConnectTo(lookupState.Edge.ConnectionPoint, target.SwitchState));
                            }
                            else if (turnout.IsMergePoint(lookupState.Edge.ConnectionPoint) && !turnout.IsSplitPoint(lookupState.Edge.ConnectionPoint)) {
                                int switchState = turnout.GetSwitchState(lookupState.Edge.ConnectionPoint,
                                    lookupState.Edge.Track.ConnectTo(lookupState.Edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);

                                lookupState.SwitchStates.Add(switchState);
                                lookupState.Edge = lookupState.Edge.Track.GetConnectedComponentEdge(lookupState.Edge.Track.ConnectTo(lookupState.Edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);
                            }
                            else {
                                if (backtrackManager.HasEntry(lookupState)) {
                                    lookupState = backtrackManager.LookupState;
                                    if (lookupState != null)
                                        Trace.WriteLineIf(traceRoutePlanning.TraceInfo, "  Backtracking to " + lookupState.Edge.ToString());
                                }
                                else {
                                    RouteOption[] routeOptions = GetRouteOptions(lookupState.Edge, target);

                                    if (routeOptions.Length > 1)
                                        backtrackManager.AddBacktrackEntry(lookupState, routeOptions);

                                    lookupState.SwitchStates.Add(routeOptions[0].SwitchState);
                                    lookupState.Edge = lookupState.Edge.Track.GetConnectedComponentEdge(turnout.ConnectTo(lookupState.Edge.ConnectionPoint, routeOptions[0].SwitchState));

                                    Trace.WriteLineIf(traceRoutePlanning.TraceVerbose, "Turnout " + ((ModelComponent)turnout).FullDescription + " selecting route option w/ switch state " + routeOptions[0].SwitchState + " penalty " + routeOptions[0].Penalty);
                                }
                            }
                        }
                        else
                            lookupState.Edge = lookupState.Edge.Track.GetConnectedComponentEdge(lookupState.Edge.OtherConnectionPoint);
                    }
                }
            }

            if (lookupState != null) {
                if (lookupState.Quality.IsValidRoute) {
                    bestRoute.DestinationTarget = target;

                    if (bestRoute.Direction == LocomotiveOrientation.Forward)
                        bestRoute.DestinationFront = target.DestinationEdge.ConnectionPoint;
                    else
                        bestRoute.DestinationFront = target.DestinationEdge.OtherConnectionPoint;

                    bestRoute.Quality = lookupState.Quality;
                    bestRoute.SetSwitchStates(lookupState.SwitchStates);

                    if (traceRoutePlanning.TraceInfo) {
                        Trace.Write("Found route: ");
                        if (bestRoute.SwitchStates != null) {
                            foreach (int switchState in bestRoute.SwitchStates)
                                Trace.Write(" " + switchState);
                            Trace.WriteLine("");
                        }
                        else
                            Trace.WriteLine("Route with empty (null) switch states list");
                    }
                }
                else
                    bestRoute.Quality.ClearanceQuality = lookupState.Quality.ClearanceQuality;
            }
        }

        private bool CheckCondition(BestRoute bestRoute, RouteLookupState lookupState, LayoutBlock newBlock, string conditionDescription, TripPlanTrainConditionInfo condition) {
            bool result = true;

            if (!condition.IsConditionEmpty) {
                LayoutConditionScript conditionScript = new(conditionDescription + " condition for " + newBlock.BlockDefinintion.Name, condition.ConditionElement);
                LayoutScriptContext context = conditionScript.ScriptContext;

                conditionScript.ScriptSubject = newBlock.BlockDefinintion;

                if (bestRoute.Train != null)
                    EventManager.Event(new LayoutEvent("set-script-context", bestRoute.Train, context));

                EventManager.Event(new LayoutEvent("set-script-context", newBlock, context));

                context["PreviousBlock"] = lookupState.Block;
                context["PreviousBlockInfo"] = lookupState.Block.BlockDefinintion;

                result = conditionScript.IsTrue;

                if (condition.ConditionScope == TripPlanTrainConditionScope.DisallowIfTrue)
                    result = !result;

                if (!result)
                    Trace.WriteLineIf(traceRoutePlanning.TraceInfo, conditionDescription + " condition failed " + lookupState.Edge.ToString());
            }

            return result;
        }

        private RouteOption[] GetRouteOptions(TrackEdge edge, RouteTarget target) {
            List<RouteOption> routeOptions = new();

            RoutingTableEntry routingTableEntry = routingTable[edge];

            for (int switchState = 0; switchState < routingTableEntry.SwitchStateCount; switchState++) {
                if (routingTableEntry[switchState] != null && routingTableEntry[switchState].TryGetValue(target.TargetEdgeId, out int penalty))
                    routeOptions.Add(new RouteOption(switchState, penalty));
            }

            Debug.Assert(routeOptions.Count > 0);

            if (routeOptions.Count > 1)
                routeOptions.Sort();

            if (traceRoutePlanning.TraceVerbose) {
                Trace.WriteLine("Route options for turnout: " + edge.ToString());

                foreach (RouteOption routeOption in routeOptions)
                    Trace.WriteLine("  Switch state " + routeOption.SwitchState + "  Penalty " + routeOption.Penalty);
            }

            return routeOptions.ToArray();
        }

        #region Helper classes

        private class RouteOption : IComparable {
            public int SwitchState;
            public int Penalty;

            public RouteOption(int switchState, int penalty) {
                this.SwitchState = switchState;
                this.Penalty = penalty;
            }

            public int CompareTo(object? oBacktrackOption) => Penalty - Ensure.NotNull<RouteOption>(oBacktrackOption).Penalty;
        }

        private class RouteLookupState : ICloneable {
            public RouteLookupState(Guid routeOwner, TrackEdge edge) {
                this.Edge = edge;
                this.Quality = new RouteQuality(routeOwner);
                this.Block = edge.Track.GetBlock(edge.ConnectionPoint);
                this.GotToTarget = false;
            }

            protected RouteLookupState(RouteLookupState lookupState) {
                this.Edge = lookupState.Edge;
                this.Block = lookupState.Block;
                this.Quality = (RouteQuality)lookupState.Quality.Clone();
                this.SwitchStates = new List<int>(lookupState.SwitchStates);
                this.GotToTarget = lookupState.GotToTarget;
            }

            public List<int> SwitchStates { get; } = new List<int>();

            public RouteQuality Quality { get; }

            public TrackEdge Edge { get; set; }

            public LayoutBlock Block { get; set; }

            public bool GotToTarget { get; set; }

            public object Clone() => new RouteLookupState(this);
        }

        private class RouteBacktrackEntry {
            private int nextOption;
            private readonly RouteLookupState lookupState;
            private readonly RouteOption[] routeOptions;

            public RouteBacktrackEntry(RouteLookupState lookupState, RouteOption[] routeOptions) {
                this.lookupState = (RouteLookupState)lookupState.Clone();
                this.routeOptions = routeOptions;
                nextOption = 1;
            }

            public int NextPenalty => routeOptions[nextOption].Penalty;

            public RouteLookupState? LookupState {
                get {
                    if (IsValid) {
                        RouteLookupState lookupState = (RouteLookupState)this.lookupState.Clone();
                        IModelComponentIsMultiPath turnout = (IModelComponentIsMultiPath)lookupState.Edge.Track;

                        lookupState.SwitchStates.Add(routeOptions[nextOption].SwitchState);
                        lookupState.Edge = lookupState.Edge.Track.GetConnectedComponentEdge(turnout.ConnectTo(lookupState.Edge.ConnectionPoint, routeOptions[nextOption].SwitchState));
                        nextOption++;

                        return lookupState;
                    }
                    else
                        return null;
                }
            }

            public bool IsValid => nextOption < routeOptions.Length;
        }

        private class RouteBacktrackManager {
            private readonly Dictionary<TrackEdge, RouteBacktrackEntry> backtrackEntries = new();

            public void AddBacktrackEntry(RouteLookupState lookupState, RouteOption[] routeOptions) {
                if (!backtrackEntries.ContainsKey(lookupState.Edge))
                    backtrackEntries.Add(lookupState.Edge, new RouteBacktrackEntry(lookupState, routeOptions));
            }

            public bool HasEntry(RouteLookupState lookupState) => backtrackEntries.ContainsKey(lookupState.Edge);

            public RouteBacktrackEntry? BestBacktrackEntry {
                get {
                    RouteBacktrackEntry? bestEntry = null;

                    foreach (KeyValuePair<TrackEdge, RouteBacktrackEntry> d in backtrackEntries) {
                        RouteBacktrackEntry backtrackEntry = d.Value;

                        if (backtrackEntry.IsValid) {
                            if (bestEntry == null || backtrackEntry.NextPenalty < bestEntry.NextPenalty)
                                bestEntry = backtrackEntry;
                        }
                    }

                    return bestEntry;
                }
            }

            public RouteLookupState? LookupState {
                get {
                    var bestEntry = BestBacktrackEntry;

                    return bestEntry?.LookupState;
                }
            }
        }

        #endregion

        #endregion

        [LayoutEvent("rebuild-layout-state", Order = 1100)]
        private void RebuildLayoutState(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();
            Debug.WriteLine("Rebuild trip plan catalog");

            LayoutModel.StateManager.TripPlansCatalog.CheckIntegrity(this, phase);
            e.Info = true;
        }

        #region Data structures

        private class RoutingTable {
            private const string E_RoutingTable = "RoutingTable";
            private const string E_Entry = "Entry";
            private const string A_Id = "ID";
            private const string A_Cp = "Cp";
            private const string A_States = "States";
            private const string E_State = "State";
            private const string A_Index = "Index";
            private const string E_Route = "Route";
            private const string A_DestID = "DestID";
            private const string A_DestCp = "DestCp";
            private const string A_Penalty = "Penalty";
            private readonly Dictionary<TrackEdgeId, RoutingTableEntry> routingTable = new();

            public RoutingTable() {
            }

            /// <summary>
            /// Load routing table from saved XML file
            /// </summary>
            /// <remarks>Reader current element is "RoutingTable"</remarks>
            /// <param name="r"></param>
            public RoutingTable(XmlReader r) {
                ConvertableString GetAttribute(string name) => new(r.GetAttribute(name), $"Attribute {name}");

                if (!r.IsEmptyElement) {
                    r.Read();

                    do {
                        if (r.NodeType == XmlNodeType.Element && r.Name == E_Entry) {
                            var nStates = (int)GetAttribute(A_States);
                            var id = (Guid)GetAttribute(A_Id);
                            var cp = GetAttribute(A_Cp).ToComponentConnectionPoint();

                            RoutingTableEntry entry = new(nStates);

                            if (!r.IsEmptyElement) {
                                r.Read();

                                do {
                                    if (r.NodeType == XmlNodeType.Element && r.Name == E_State) {
                                        var index = (int)GetAttribute(A_Index);

                                        if (!r.IsEmptyElement) {
                                            entry[index] = new Dictionary<TrackEdgeId, int>();

                                            r.Read();

                                            do {
                                                if (r.NodeType == XmlNodeType.Element && r.Name == E_Route) {
                                                    var destinationId = (Guid)GetAttribute(A_DestID);
                                                    var destinationCp = GetAttribute(A_DestCp).ToComponentConnectionPoint();
                                                    var penalty = (int)GetAttribute(A_Penalty);

                                                    entry[index].Add(new TrackEdgeId(destinationId, destinationCp), penalty);
                                                }

                                                r.Read();
                                            } while (r.NodeType != XmlNodeType.EndElement);
                                        }
                                    }

                                    r.Read();
                                } while (r.NodeType != XmlNodeType.EndElement);
                            }

                            Add(new TrackEdgeId(id, cp), entry);
                        }

                        r.Read();
                    } while (r.NodeType != XmlNodeType.EndElement);
                }
            }

            public RoutingTableEntry this[TrackEdgeId edgeID] {
                get {
                    return routingTable[edgeID];
                }

                set {
                    routingTable[edgeID] = value;
                }
            }

            public RoutingTableEntry this[TrackEdge edge] {
                get {
                    return this[new TrackEdgeId(edge)];
                }

                set {
                    this[new TrackEdgeId(edge)] = value;
                }
            }

            public ICollection<RoutingTableEntry> Entries => routingTable.Values;

            public void Clear() {
                routingTable.Clear();
            }

            public bool Contains(TrackEdgeId edgeID) => routingTable.ContainsKey(edgeID);

            public bool Contains(TrackEdge edge) => Contains(new TrackEdgeId(edge));

            public void Add(TrackEdgeId keyEdgeID, RoutingTableEntry entry) {
                routingTable.Add(keyEdgeID, entry);
            }

            public void WriteXml(XmlWriter w) {
                w.WriteStartElement(E_RoutingTable);

                foreach (KeyValuePair<TrackEdgeId, RoutingTableEntry> d in routingTable) {
                    TrackEdgeId keyEdgeID = d.Key;
                    RoutingTableEntry entry = d.Value;

                    w.WriteStartElement(E_Entry);
                    w.WriteAttributeString(A_Id, keyEdgeID.TrackId);
                    w.WriteAttributeString(A_Cp, keyEdgeID.ConnectionPoint.ToString());
                    w.WriteAttributeString(A_States, entry.SwitchStateCount);

                    for (int iState = 0; iState < entry.SwitchStateCount; iState++) {
                        if (entry[iState] != null && entry[iState].Count > 0) {
                            w.WriteStartElement(E_State);
                            w.WriteAttributeString(A_Index, iState);

                            foreach (KeyValuePair<TrackEdgeId, int> dState in entry[iState]) {
                                TrackEdgeId stateKeyEdgeID = dState.Key;

                                w.WriteStartElement(E_Route);
                                w.WriteAttributeString(A_DestID, stateKeyEdgeID.TrackId);
                                w.WriteAttributeString(A_DestCp, stateKeyEdgeID.ConnectionPoint.ToString());
                                w.WriteAttributeString(A_Penalty, dState.Value);
                                w.WriteEndElement();
                            }

                            w.WriteEndElement();
                        }
                    }

                    w.WriteEndElement();
                }

                w.WriteEndElement();
            }

            public void Dump() {
                Debug.WriteLine("Routing table");

                foreach (KeyValuePair<TrackEdgeId, RoutingTableEntry> d in routingTable) {
                    TrackEdgeId keyEdgeID = d.Key;
                    RoutingTableEntry r = d.Value;

                    Debug.WriteLine(keyEdgeID.ToString());

                    for (int iState = 0; iState < r.SwitchStateCount; iState++) {
                        if (r.SwitchStateCount == 2)
                            Debug.Write(iState == 0 ? "S: " : "B: ");
                        else
                            Debug.Write("State " + iState + ":");

                        if (r[iState] != null) {
                            Debug.Write("[" + r[iState].Count + "]: ");
                            foreach (KeyValuePair<TrackEdgeId, int> splitPair in r[iState]) {
                                TrackEdgeId destEdgeID = splitPair.Key;
                                var component = LayoutModel.Component<ModelComponent>(destEdgeID.TrackId, LayoutPhase.All);

                                if (component != null)
                                    Debug.Write("X=" + component.Location.X + ",Y=" + component.Location.Y + "/" + destEdgeID.ConnectionPoint.ToString() + " (" + splitPair.Value + ") ");
                                else
                                    Debug.Write($"MISSING component for id {destEdgeID}");
                            }
                        }
                        Debug.WriteLine("");
                    }
                }
            }
        }

        private struct RoutingTableEntry : IEnumerable<Dictionary<TrackEdgeId, int>> {
            private readonly Dictionary<TrackEdgeId, int>[] reachableNodes;

            public RoutingTableEntry(int nSwitchingStates) {
                this.SwitchStateCount = nSwitchingStates;
                this.reachableNodes = new Dictionary<TrackEdgeId, int>[nSwitchingStates];
            }

            public Dictionary<TrackEdgeId, int> this[int switchState] {
                get {
                    return reachableNodes[switchState];
                }

                set {
                    reachableNodes[switchState] = value;
                }
            }

            public int SwitchStateCount { get; }

            #region IEnumerable< Dictionary<TrackEdgeID, int>> Members

            public IEnumerator<Dictionary<TrackEdgeId, int>> GetEnumerator() => Array.AsReadOnly<Dictionary<TrackEdgeId, int>>(reachableNodes).GetEnumerator();

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion
    }
}

