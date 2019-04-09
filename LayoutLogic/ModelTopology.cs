using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager;

#pragma warning disable IDE0051,IDE0060
#nullable enable
namespace LayoutManager.Logic {
    public class ModelTopology {
        private const string A_Cp = "Cp";
        private const string A_Penalty = "Penalty";
        private const string A_State = "State";
        private const string A_States = "States";
        private const string A_TrackId = "TrackID";
        private const string E_Component = "Component";
        private const string E_ConnectTo = "ConnectTo";
        private readonly Dictionary<TrackEdgeId, ModelTopologyEntry> topology = new Dictionary<TrackEdgeId, ModelTopologyEntry>();
        private bool compiled;

        /// <summary>
        /// Get the topology of the given model
        /// </summary>
        /// <param name="model"></param>
        public ModelTopology(LayoutPhase phase) {
            foreach (var switchingTrack in LayoutModel.Components<IModelComponentIsMultiPath>(phase)) {
                foreach (LayoutComponentConnectionPoint cp in switchingTrack.ConnectionPoints)
                    if (switchingTrack.IsSplitPoint(cp)) {  // Note that for simple turnout, only one connection point is a split point
                        LayoutComponentConnectionPoint[] connectedPoints = switchingTrack.ConnectTo(cp, LayoutComponentConnectionType.Passage);
                        ModelTopologyEntry entry = new ModelTopologyEntry(connectedPoints.Length);

                        for (int i = 0; i < entry.SwitchingStateCount; i++)
                            entry[i] = getConnectedSplit(switchingTrack.GetConnectedComponentEdge(switchingTrack.ConnectTo(cp, i)));

                        topology.Add(new TrackEdgeId(switchingTrack.Id, cp), entry);
                    }
            }
        }

        /// <summary>
        /// Load a model topology from saved XML data
        /// </summary>
        /// <remarks>Assume current element is "Topology"</remarks>
        /// <param name="r"></param>
        public ModelTopology(XmlReader r) {
            ConvertableString GetAttribute(string name) => new ConvertableString(r.GetAttribute(name), $"Attribute {name}");

            if (!r.IsEmptyElement) {
                r.Read();

                do {
                    if (r.NodeType == XmlNodeType.Element && r.Name == E_Component) {
                        var id = (Guid)GetAttribute(A_TrackId);
                        var cp = GetAttribute(A_Cp).ToComponentConnectionPoint();
                        var states = (int)GetAttribute(A_States);
                        var entry = new ModelTopologyEntry(states);

                        if (!r.IsEmptyElement) {
                            do {
                                r.Read();

                                if (r.NodeType == XmlNodeType.Element && r.Name == E_ConnectTo) {
                                    var state = (int)GetAttribute(A_State);
                                    var destinationID = (Guid)GetAttribute(A_TrackId);
                                    var destinationCp = LayoutComponentConnectionPoint.Parse(r.GetAttribute(A_Cp));
                                    var penalty = (int)GetAttribute(A_Penalty);

                                    entry[state] = new ModelTopologyConnectionEntry(new TrackEdgeId(destinationID, destinationCp), penalty);

                                    if (!r.IsEmptyElement)
                                        r.Skip();
                                }
                            } while (r.NodeType != XmlNodeType.EndElement);
                        }

                        topology.Add(new TrackEdgeId(id, cp), entry);
                    }
                    r.Read();
                } while (r.NodeType != XmlNodeType.EndElement);
            }
        }

        public void Compile() {
            if (!compiled) {
                foreach (ModelTopologyEntry entry in topology.Values) {
                    for (int switchState = 0; switchState < entry.SwitchingStateCount; switchState++) {
                        if (entry[switchState].Destination != TrackEdgeId.Empty)
                            entry[switchState].DestinationTopologyEntry = topology[entry[switchState].Destination];
                    }
                }

                compiled = true;
            }
        }

        public bool Contains(TrackEdgeId trackEdgeId) => topology.ContainsKey(trackEdgeId);

        public bool Contains(TrackEdge edge) => Contains(new TrackEdgeId(edge));

        public ModelTopologyEntry this[TrackEdgeId trackEdgeId] => topology[trackEdgeId];

        public ModelTopologyEntry this[TrackEdge edge] => topology[new TrackEdgeId(edge)];

        /// <summary>
        /// Check if this topology is the same as a given other topology
        /// </summary>
        /// <param name="obj">The other topology</param>
        /// <returns>True - the two topologies are the same</returns>
        public override bool Equals(object obj) {
            if (!(obj is ModelTopology other))
                return false;

            foreach (KeyValuePair<TrackEdgeId, ModelTopologyEntry> d in topology) {
                TrackEdgeId edgeID = d.Key;
                ModelTopologyEntry entry = d.Value;

                if (other.Contains(edgeID)) {
                    ModelTopologyEntry otherEntry = other[edgeID];

                    if (!entry.Equals(otherEntry))
                        return false;
                }
                else
                    return false;
            }

            return true;
        }

        public void WriteXml(XmlWriter w) {
            w.WriteStartElement("Topology");

            foreach (KeyValuePair<TrackEdgeId, ModelTopologyEntry> d in topology) {
                TrackEdgeId edgeID = d.Key;
                ModelTopologyEntry entry = d.Value;

                w.WriteStartElement(E_Component);
                w.WriteAttributeString(A_TrackId, edgeID.TrackId);
                w.WriteAttributeString(A_Cp, edgeID.ConnectionPoint.ToString());
                w.WriteAttributeString(A_States, entry.SwitchingStateCount);

                for (int i = 0; i < entry.SwitchingStateCount; i++) {
                    if (entry[i].Destination != TrackEdgeId.Empty) {
                        w.WriteStartElement(E_ConnectTo);
                        w.WriteAttributeString(A_State, XmlConvert.ToString(i));
                        w.WriteAttributeString(A_Cp, entry[i].Destination.ConnectionPoint.ToString());
                        w.WriteAttributeString(A_TrackId, entry[i].Destination.TrackId);
                        w.WriteAttributeString(A_Penalty, entry[i].Penalty);
                        w.WriteEndElement();
                    }
                }

                w.WriteEndElement();
            }

            w.WriteEndElement();
        }

        public override int GetHashCode() => base.GetHashCode();

        private static ModelTopologyConnectionEntry getConnectedSplit(TrackEdge edge) {
            TrackEdgeDictionary visitedMergePoints = new TrackEdgeDictionary();
            int penalty = 0;

            while (edge != TrackEdge.Empty) {
                if (edge.Track is IModelComponentIsMultiPath switchingTrack) {
                    if (switchingTrack.IsSplitPoint(edge.ConnectionPoint))
                        return new ModelTopologyConnectionEntry(new TrackEdgeId(edge), penalty);
                    else {
                        if (visitedMergePoints.ContainsKey(edge))
                            edge = TrackEdge.Empty;
                        else {
                            visitedMergePoints.Add(edge, null);
                            edge = edge.Track.GetConnectedComponentEdge(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);
                        }
                    }
                }
                else {
                    var blockDefinition = edge.Track.BlockDefinitionComponent;

                    if (blockDefinition != null)
                        penalty += blockDefinition.Info.LengthInCM;

                    edge = edge.Track.GetConnectedComponentEdge(edge.OtherConnectionPoint);
                }
            }

            return new ModelTopologyConnectionEntry(TrackEdgeId.Empty, 0);
        }
    }

    public class ModelTopologyConnectionEntry {
        private TrackEdgeId destination;                // Where this connects to

        public ModelTopologyConnectionEntry(TrackEdgeId destination, int penalty) {
            this.destination = destination;
            this.Penalty = penalty;
        }

        public TrackEdgeId Destination => destination;

        public int Penalty { get; }

        public bool IsEmpty => destination.IsEmpty;

        public ModelTopologyEntry? DestinationTopologyEntry { get; set; }

        public override bool Equals(object obj) {
            if (obj == null)
                return destination == TrackEdgeId.Empty;

            if (!(obj is ModelTopologyConnectionEntry other))
                return false;

            return destination.Equals(other.destination) && Penalty == other.Penalty;
        }

        public override int GetHashCode() => destination.GetHashCode() ^ Penalty;
    }

    public class ModelTopologyEntry {
        private readonly ModelTopologyConnectionEntry[] connections;

        public ModelTopologyEntry(int stateCount) {
            this.SwitchingStateCount = stateCount;
            connections = new ModelTopologyConnectionEntry[stateCount];
        }

        public int SwitchingStateCount { get; }

        public bool Visited { get; set; }

        public ModelTopologyConnectionEntry this[int switchState] {
            get {
                return connections[switchState];
            }

            set {
                connections[switchState] = value;
            }
        }

        public override bool Equals(object obj) {
            if (!(obj is ModelTopologyEntry other))
                return false;

            if (SwitchingStateCount != other.SwitchingStateCount)
                return false;

            for (int i = 0; i < SwitchingStateCount; i++)
                if (!this[i].Equals(other[i]))
                    return false;

            return true;
        }

        public override int GetHashCode() {
            Debug.Assert(false);
            return base.GetHashCode();
        }
    }
}
