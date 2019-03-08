using System;
using System.Collections.Generic;
using System.Diagnostics;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic {
    /// <summary>
    /// Summary description for LayoutCompiler.
    /// </summary>
    [LayoutModule("Layout Block Manager", UserControl = false)]
    class LayoutBlockManager : LayoutModuleBase {
        static readonly LayoutTraceSwitch traceBlocks = new LayoutTraceSwitch("BlockIdentification", "Block Identification");
        static readonly LayoutTraceSwitch traceBlockInfo = new LayoutTraceSwitch("BlockInfo", "Block Info directions");

        ILayoutTopologyServices _topologyServices;

        ILayoutTopologyServices TopologyServices {
            get {
                if (_topologyServices == null)
                    _topologyServices = (ILayoutTopologyServices)EventManager.Event(new LayoutEvent("get-topology-services", this));

                return _topologyServices;
            }
        }

        #region Build block graph

        /// <summary>
        /// Build the track to block association
        /// </summary>
        /// <returns>True if there are no fatal errors</returns>
        [LayoutEvent("check-layout", Order = 200)]
        void LocateBlocks(LayoutEvent e) {
            LayoutPhase phases = e.GetPhases();
            IEnumerable<LayoutBlockEdgeBase> blockEdges = LayoutModel.Components<LayoutBlockEdgeBase>(phases);
            TrackEdgeDictionary scannedBlockBoundries = new TrackEdgeDictionary();

            LayoutModel.Blocks.Clear();

            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    LayoutTrackComponent track = spot.Track;

                    if (track != null) {
                        foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints)
                            track.SetBlock(cp, null);
                    }
                }
            }

            foreach (LayoutBlockEdgeBase blockEdge in blockEdges) {
                LayoutTrackComponent track = blockEdge.Track;
                TrackEdge scanFrom;

                scanFrom = new TrackEdge(track, track.ConnectionPoints[0]);
                if (!scannedBlockBoundries.ContainsKey(scanFrom)) {
                    LayoutBlock block = scanBlock(scannedBlockBoundries, scanFrom, null);

                    if (block == null || !checkBlockForBlockInfo(block))
                        e.Info = false;

                    if (!scannedBlockBoundries.ContainsKey(scanFrom))
                        scannedBlockBoundries.Add(scanFrom, scanFrom);
                }

                scanFrom = new TrackEdge(track, track.ConnectionPoints[1]);
                if (!scannedBlockBoundries.ContainsKey(scanFrom)) {
                    LayoutBlock block = scanBlock(scannedBlockBoundries, scanFrom, null);

                    if (block == null || !checkBlockForBlockInfo(block))
                        e.Info = false;

                    if (!scannedBlockBoundries.ContainsKey(scanFrom))
                        scannedBlockBoundries.Add(scanFrom, scanFrom);
                }
            }

            // Handle special case where there are tracks with no block edge components (should not really happend). Those
            // tracks should be placed in a block
            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    if ((spot.Phase & phases) != 0) {
                        LayoutTrackComponent track = spot.Track;

                        if (track != null) {
                            if (spot[ModelComponentKind.TrackLink] == null) {
                                foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints) {
                                    if (track.GetBlock(cp) == null) {
                                        LayoutBlock block = null;

                                        foreach (LayoutComponentConnectionPoint scanCp in track.ConnectionPoints) {
                                            block = scanBlock(scannedBlockBoundries, new TrackEdge(track, scanCp), block);
                                            if (block == null) {
                                                e.Info = false;
                                                break;
                                            }
                                        }

                                        if (block != null && !checkBlockForBlockInfo(block))
                                            e.Info = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!(bool)e.Info)
                e.ContinueProcessing = false;
        }

        private bool checkBlockForBlockInfo(LayoutBlock block) {
            if (block.BlockDefinintion == null) {
                Error(block, "This block does not contain any block definition component");
                return false;
            }

            return true;
        }

        private LayoutBlock scanBlock(TrackEdgeDictionary scannedBlockBoundres, TrackEdge scanFrom, LayoutBlock block) {
            TrackEdgeDictionary scannedEdges = new TrackEdgeDictionary();
            Stack<TrackEdge> scanStack = new Stack<TrackEdge>();
            bool ok = true;

            if (block == null)
                block = new LayoutBlock();

            block.Add(scanFrom);
            checkIfBlockDefinitionTrack(block, scanFrom);

            TrackEdge startFrom = TopologyServices.FindTrackConnectingAt(scanFrom);

            if (startFrom == TrackEdge.Empty)
                return block;

            scanStack.Push(startFrom);

            while (scanStack.Count > 0) {
                TrackEdge edge = scanStack.Pop();

                if (!scannedEdges.ContainsKey(edge)) {
                    scannedEdges.Add(edge, null);

                    block.Add(edge);

                    if (edge.Track.BlockEdgeBase != null) {
                        if (!scannedBlockBoundres.ContainsKey(edge))
                            scannedBlockBoundres.Add(edge, null);
                    }
                    else {
                        checkIfBlockDefinitionTrack(block, edge);

                        LayoutComponentConnectionPoint[] cps = edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Topology);

                        foreach (LayoutComponentConnectionPoint cp in cps) {
                            TrackEdge nextEdge = TopologyServices.FindTrackConnectingAt(new TrackEdge(edge.Track, cp));

                            block.Add(new TrackEdge(edge.Track, cp));

                            if (nextEdge != TrackEdge.Empty && !scannedEdges.ContainsKey(nextEdge))
                                scanStack.Push(nextEdge);
                        }
                    }
                }
            }

            if (!ok)
                return null;

            return block;
        }

        private static void checkIfBlockDefinitionTrack(LayoutBlock block, TrackEdge edge) {

            if (edge.Track.Spot[ModelComponentKind.BlockInfo] is LayoutBlockDefinitionComponent blockInfo) {
                if (block.BlockDefinintion != null && block.BlockDefinintion.Id != blockInfo.Id)
                    Error(blockInfo, "Block contains more than one block definition component");
                else {
                    if (!LayoutModel.Blocks.ContainsKey(blockInfo.Id)) {
                        block.BlockDefinintion = blockInfo;
                        LayoutModel.Blocks.Add(block);
                    }
                }
            }
        }

        #endregion

        #region Process block Info

        [LayoutEvent("check-layout", Order = 300)]
        void ProcessBlockInfo(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();

            foreach (LayoutBlockDefinitionComponent blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)) {
                blockDefinition.ClearBlockEdges();

                for (int cpIndex = 0; cpIndex < blockDefinition.Track.ConnectionPoints.Count; cpIndex++)
                    addBlockInfoBlockEdges(blockDefinition, cpIndex);

#if DUMP_BLOCK_EDGE_MAP
				Trace.WriteLine("Block " + blockInfo.FullDescription + " edges:");
				for(int cpIndex = 0; cpIndex < blockInfo.Track.ConnectionPoints.Length; cpIndex++) {
					Trace.WriteLine(" Connection point " + blockInfo.Track.ConnectionPoints[cpIndex].ToString() + ":");
					foreach(LayoutBlockEdgeBase blockEdge in blockInfo.GetBlockEdges(cpIndex))
						Trace.WriteLine("   " + blockEdge.FullDescription);
				}
#endif

                // Validate that all track contacts that bound the block are reachable from the block info
                // (if not, generate a warning)
                LayoutSelection unreachedBlockEdges = new LayoutSelection();
                LayoutSelection ambiguousBlockEdges = new LayoutSelection();

                foreach (TrackEdge edge in blockDefinition.Block.TrackEdges) {
                    LayoutBlockEdgeBase blockEdge = edge.Track.BlockEdgeBase;

                    if (blockEdge != null) {
                        bool reachable = false;
                        bool ambiguous = false;

                        for (int cpIndex = 0; cpIndex < blockDefinition.Track.ConnectionPoints.Count; cpIndex++) {
                            if (blockDefinition.ContainsBlockEdge(cpIndex, blockEdge)) {
                                if (reachable)
                                    ambiguous = true;
                                else
                                    reachable = true;
                            }
                        }

                        if (!reachable)
                            unreachedBlockEdges.Add(blockEdge);
                        else if (ambiguous)
                            ambiguousBlockEdges.Add(blockEdge);
                    }
                }

                if (unreachedBlockEdges.Count > 0) {
                    unreachedBlockEdges.Add(blockDefinition);
                    Warning(unreachedBlockEdges, "The locomotive direction is misleading if the locomotive enters the block by crossing the indicated track contact. You should move the block definition component");
                }

                if (ambiguousBlockEdges.Count > 0) {
                    ambiguousBlockEdges.Add(blockDefinition);
                    Warning(ambiguousBlockEdges, "The locomotive direction is ambiguous when this track contact is crossed. Add more blocks");
                }

#if NO_WARNING_ON_NO_BLOCK_TRACK_CONTACT
                if(blockDefinition.GetBlockEdges(0).Length == 0 && blockDefinition.GetBlockEdges(1).Length == 0)
					blockDefinition.Warning("This block definition is not inside a block. Add block edge components (e.g. track contacts)");
#endif

                if (traceBlockInfo.TraceInfo) {
                    Trace.WriteLine("Block Info: " + blockDefinition.NameProvider.Name);

                    for (int i = 0; i < 2; i++) {
                        Trace.WriteLine(" Direction: " + blockDefinition.Track.ConnectionPoints[i] + " is reachable from: ");

                        foreach (LayoutBlockEdgeBase blockEdge in blockDefinition.GetBlockEdges(i))
                            Trace.WriteLine("   block edge: " + blockEdge.FullDescription);
                    }
                }
            }
        }

        private void addBlockInfoBlockEdges(LayoutBlockDefinitionComponent blockInfo, int cpIndex) {
            Stack<TrackEdge> scanStack = new Stack<TrackEdge>();
            TrackEdgeDictionary scanned = new TrackEdgeDictionary();
            LayoutTopologyServices ts = (LayoutTopologyServices)EventManager.Event(new LayoutEvent("get-topology-services", this));

            scanStack.Push(ts.FindTrackConnectingAt(new TrackEdge(blockInfo.Track, blockInfo.Track.ConnectionPoints[cpIndex])));

            while (scanStack.Count > 0) {
                TrackEdge edge = scanStack.Pop();

                if (edge != TrackEdge.Empty) {
                    LayoutBlockEdgeBase blockEdge = edge.Track.BlockEdgeBase;

                    if (blockEdge != null)
                        blockInfo.AddBlockEdge(cpIndex, blockEdge);
                    else {
                        if (!scanned.ContainsKey(edge)) {
                            TrackEdge[] connectingEdges = TopologyServices.FindConnectingTracks(edge, LayoutComponentConnectionType.Passage);

                            scanned.Add(edge, null);

                            foreach (TrackEdge connectingEdge in connectingEdges)
                                scanStack.Push(connectingEdge);
                        }
                    }
                }
            }
        }

        #endregion

        #region Build Occupancy (train detection blocks) data structures

        [LayoutEvent("check-layout", Order = 320)]
        private void locateNoFeedbackBlocks(LayoutEvent e) {
            foreach (LayoutBlock block in LayoutModel.Blocks) {
                LayoutBlockDefinitionComponentInfo info = block.BlockDefinintion.Info;

                if (!info.IsOccupancyDetectionBlock) {
                    info.NoFeedback = false;

                    // Check if all edges are non-track contact, if this is the case then the block is a no-feedback block
                    foreach (LayoutBlockEdgeBase blockEdge in block.BlockEdges)
                        if (!blockEdge.IsTrackContact()) {
                            info.NoFeedback = true;
                            break;
                        }
                }
                else
                    info.NoFeedback = false;
            }
        }

        [LayoutEvent("check-layout", Order = 350)]
        private void buildTrainDetectionBlocks(LayoutEvent e) {
            LayoutPhase phase = e.GetPhases();

            foreach (LayoutBlockDefinitionComponent blockInfo in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)) {
                if (blockInfo.Info.IsOccupancyDetectionBlock) {
                    LayoutOccupancyBlock occupancyBlock = new LayoutOccupancyBlock();
                    LayoutTrackComponent track = blockInfo.Track;

                    occupancyBlock.BlockDefinintion = blockInfo;

                    scanTrainDetectionBlock(new TrackEdge(track, track.ConnectionPoints[0]), occupancyBlock);
                    scanTrainDetectionBlock(new TrackEdge(track, track.ConnectionPoints[1]), occupancyBlock);
                }
            }
        }

        private void scanTrainDetectionBlock(TrackEdge scanFrom, LayoutOccupancyBlock occupancyBlock) {
            TrackEdgeDictionary scannedEdges = new TrackEdgeDictionary();
            Stack<TrackEdge> scanStack = new Stack<TrackEdge>();

            scanStack.Push(scanFrom);

            while (scanStack.Count > 0) {
                TrackEdge edge = scanStack.Pop();

                if (!scannedEdges.ContainsKey(edge)) {
                    scannedEdges.Add(edge, null);


                    if (edge.Track.Spot[ModelComponentKind.BlockInfo] is LayoutBlockDefinitionComponent blockDefinition) {
                        occupancyBlock.AddBlock(blockDefinition.Block);

                        if (blockDefinition.Info.IsOccupancyDetectionBlock && blockDefinition != occupancyBlock.BlockDefinintion)
                            Error(blockDefinition, "Train detection block has more than one block definition component that is flagged as train detection block");
                    }

                    if (edge.Track.BlockEdgeComponent == null) {
                        TrackEdge[] connectingEdges = TopologyServices.FindConnectingTracks(edge, LayoutComponentConnectionType.Passage);

                        foreach (TrackEdge connectingEdge in connectingEdges)
                            scanStack.Push(connectingEdge);
                    }
                    else
                        occupancyBlock.TrackEdges.Add(edge);
                }
            }
        }

        #endregion

        #region Handle block policy

        [LayoutEvent("train-enter-block")]
        private void activateBlockPolicy(LayoutEvent e) {
            TrainStateInfo train = (TrainStateInfo)e.Sender;
            LayoutBlock block = (LayoutBlock)e.Info;

            if (block.BlockDefinintion != null) {
                foreach (Guid policyID in block.BlockDefinintion.Info.Policies) {
                    LayoutPolicyInfo policy = LayoutModel.StateManager.BlockInfoPolicies[policyID];

                    if (policy != null) {
                        LayoutEventScript eventScript;

                        eventScript = EventManager.EventScript("Policy " + policy.Name + " activated by train " + train.DisplayName + " entering block " + block.BlockDefinintion.Name,
                            policy.EventScriptElement, new Guid[] { block.Id, train.Id }, null);

                        LayoutScriptContext scriptContext = eventScript.ScriptContext;

                        EventManager.Event(new LayoutEvent("set-script-context", train, scriptContext, null));
                        EventManager.Event(new LayoutEvent("set-script-context", block, scriptContext, null));

                        eventScript.Reset();
                    }
                }
            }
        }

        #endregion
    }
}
