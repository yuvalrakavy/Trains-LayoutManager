using System;
using System.Collections.Generic;
using System.Diagnostics;
using MethodDispatcher;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic {
    /// <summary>
    /// Summary description for LayoutCompiler.
    /// </summary>
    [LayoutModule("Layout Block Manager", UserControl = false)]
    internal class LayoutBlockManager : LayoutModuleBase {
#pragma warning disable IDE0052, RCS1213
        private static readonly LayoutTraceSwitch traceBlocks = new("BlockIdentification", "Block Identification");
        private static readonly LayoutTraceSwitch traceBlockInfo = new("BlockInfo", "Block Info directions");
#pragma warning restore IDE0052, RCS1213

        private ILayoutTopologyServices? _topologyServices;

        private ILayoutTopologyServices TopologyServices => _topologyServices ??= Dispatch.Call.GetTopologyServices();

        #region Build block graph

        /// <summary>
        /// Build the track to block association
        /// </summary>
        /// <returns>True if there are no fatal errors</returns>
        [DispatchTarget(Order = 200)]
        private bool CheckLayout_LocateBlocks(LayoutPhase phases) {
            IEnumerable<LayoutBlockEdgeBase> blockEdges = LayoutModel.Components<LayoutBlockEdgeBase>(phases);
            TrackEdgeDictionary scannedBlockBoundries = new();
            bool ok = true;

            LayoutModel.Blocks.Clear();

            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    var track = spot.Track;

                    if (track != null) {
                        foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints)
                            track.SetBlock(cp, null);
                    }
                }
            }

            foreach (LayoutBlockEdgeBase blockEdge in blockEdges) {
                var track = blockEdge.Track;
                TrackEdge scanFrom;

                if (track != null) {
                    scanFrom = new TrackEdge(track, track.ConnectionPoints[0]);
                    if (!scannedBlockBoundries.ContainsKey(scanFrom)) {
                        var block = ScanBlock(scannedBlockBoundries, scanFrom, null);

                        if (block == null || !CheckBlockForBlockInfo(block))
                            ok = false;

                        if (!scannedBlockBoundries.ContainsKey(scanFrom))
                            scannedBlockBoundries.Add(scanFrom, scanFrom);
                    }

                    scanFrom = new TrackEdge(track, track.ConnectionPoints[1]);
                    if (!scannedBlockBoundries.ContainsKey(scanFrom)) {
                        var block = ScanBlock(scannedBlockBoundries, scanFrom, null);

                        if (block == null || !CheckBlockForBlockInfo(block))
                            ok = false;

                        if (!scannedBlockBoundries.ContainsKey(scanFrom))
                            scannedBlockBoundries.Add(scanFrom, scanFrom);
                    }
                }
            }

            // Handle special case where there are tracks with no block edge components (should not really happend). Those
            // tracks should be placed in a block
            foreach (LayoutModelArea area in LayoutModel.Areas) {
                foreach (LayoutModelSpotComponentCollection spot in area.Grid.Values) {
                    if ((spot.Phase & phases) != 0) {
                        var track = spot.Track;

                        if (track != null) {
                            if (spot[ModelComponentKind.TrackLink] == null) {
                                foreach (LayoutComponentConnectionPoint cp in track.ConnectionPoints) {
                                    if (track.GetOptionalBlock(cp) == null) {
                                        LayoutBlock? block = null;

                                        foreach (LayoutComponentConnectionPoint scanCp in track.ConnectionPoints) {
                                            block = ScanBlock(scannedBlockBoundries, new TrackEdge(track, scanCp), block);
                                            if (block == null) {
                                                ok = false;
                                                break;
                                            }
                                        }

                                        if (block != null && !CheckBlockForBlockInfo(block))
                                            ok = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ok;
        }

        private bool CheckBlockForBlockInfo(LayoutBlock block) {
            if (block.OptionalBlockDefinition == null) {
                Error(block, "This block does not contain any block definition component");
                return false;
            }

            return true;
        }

        private LayoutBlock? ScanBlock(TrackEdgeDictionary scannedBlockBoundres, TrackEdge scanFrom, LayoutBlock? block) {
            TrackEdgeDictionary scannedEdges = new();
            Stack<TrackEdge> scanStack = new();
            const bool ok = true;

            (block ??= new LayoutBlock()).Add(scanFrom);
            CheckIfBlockDefinitionTrack(block, scanFrom);

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
                        CheckIfBlockDefinitionTrack(block, edge);

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

            return !ok ? null : block;
        }

        private static void CheckIfBlockDefinitionTrack(LayoutBlock block, TrackEdge edge) {
            if (edge.Track.Spot[ModelComponentKind.BlockInfo] is LayoutBlockDefinitionComponent blockInfo) {
                if (block.OptionalBlockDefinition != null && block.BlockDefinintion.Id != blockInfo.Id)
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

        [DispatchTarget(Order = 300)]
        private bool CheckLayout_ProcessBlockInfo(LayoutPhase phase) {

            foreach (LayoutBlockDefinitionComponent blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)) {
                blockDefinition.ClearBlockEdges();

                for (int cpIndex = 0; cpIndex < blockDefinition.Track.ConnectionPoints.Count; cpIndex++)
                    AddBlockInfoBlockEdges(blockDefinition, cpIndex);

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
                LayoutSelection unreachedBlockEdges = new();
                LayoutSelection ambiguousBlockEdges = new();

                foreach (TrackEdge edge in blockDefinition.Block.TrackEdges) {
                    var blockEdge = edge.Track.BlockEdgeBase;

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

            return true;
        }

        private void AddBlockInfoBlockEdges(LayoutBlockDefinitionComponent blockInfo, int cpIndex) {
            Stack<TrackEdge> scanStack = new();
            TrackEdgeDictionary scanned = new();

            scanStack.Push(TopologyServices.FindTrackConnectingAt(new TrackEdge(blockInfo.Track, blockInfo.Track.ConnectionPoints[cpIndex])));

            while (scanStack.Count > 0) {
                TrackEdge edge = scanStack.Pop();

                if (edge != TrackEdge.Empty) {
                    var blockEdge = edge.Track.BlockEdgeBase;

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

        [DispatchTarget(Order = 320)]
        private bool CheckLayout_LocateNoFeedbackBlocks(LayoutPhase phase) {
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

            return true;
        }

        [DispatchTarget(Order = 350)]
        private bool CheckLayout_BuildTrainDetectionBlocks(LayoutPhase phase) {
            foreach (LayoutBlockDefinitionComponent blockInfo in LayoutModel.Components<LayoutBlockDefinitionComponent>(phase)) {
                if (blockInfo.Info.IsOccupancyDetectionBlock) {
                    LayoutOccupancyBlock occupancyBlock = new();
                    LayoutTrackComponent track = blockInfo.Track;

                    occupancyBlock.BlockDefinintion = blockInfo;

                    ScanTrainDetectionBlock(new TrackEdge(track, track.ConnectionPoints[0]), occupancyBlock);
                    ScanTrainDetectionBlock(new TrackEdge(track, track.ConnectionPoints[1]), occupancyBlock);
                }
            }

            return true;
        }

        private void ScanTrainDetectionBlock(TrackEdge scanFrom, LayoutOccupancyBlock occupancyBlock) {
            TrackEdgeDictionary scannedEdges = new();
            Stack<TrackEdge> scanStack = new();

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

        [DispatchTarget]
        private void OnTrainEnteredBlock_ActivateBlockPolicy(TrainStateInfo train, LayoutBlock block) {
            if (block.BlockDefinintion != null) {
                foreach (Guid policyID in block.BlockDefinintion.Info.Policies) {
                    var policy = LayoutModel.StateManager.BlockInfoPolicies[policyID];

                    if (policy != null && policy.EventScriptElement != null) {
                        LayoutEventScript eventScript = EventManager.EventScript($"Policy {policy.Name} activated by train {train.DisplayName} entering block {block.BlockDefinintion.Name}",
                            policy.EventScriptElement, new Guid[] { block.Id, train.Id }, null);

                        LayoutScriptContext scriptContext = eventScript.ScriptContext;

                        Dispatch.Call.SetScriptContext(train, scriptContext);
                        Dispatch.Call.SetScriptContext(block, scriptContext);

                        eventScript.Reset();
                    }
                }
            }
        }

        #endregion
    }
}
