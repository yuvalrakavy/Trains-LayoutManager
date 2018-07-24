using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Logic {

    public class ModelTopology {
		Dictionary<TrackEdgeId, ModelTopologyEntry> topology = new Dictionary<TrackEdgeId, ModelTopologyEntry>();
		bool compiled;

		/// <summary>
		/// Get the topology of the given model
		/// </summary>
		/// <param name="model"></param>
		public ModelTopology(LayoutPhase phase) {
			foreach(var switchingTrack in LayoutModel.Components<IModelComponentIsMultiPath>(phase)) {
				foreach(LayoutComponentConnectionPoint cp in switchingTrack.ConnectionPoints)
					if(switchingTrack.IsSplitPoint(cp)) {	// Note that for simple turnout, only one connection point is a split point
						LayoutComponentConnectionPoint[] connectedPoints = switchingTrack.ConnectTo(cp, LayoutComponentConnectionType.Passage);
						ModelTopologyEntry entry = new ModelTopologyEntry(connectedPoints.Length);

						for(int i = 0; i < entry.SwitchingStateCount; i++)
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
            if (!r.IsEmptyElement) {
                r.Read();

                do {
                    if (r.NodeType == XmlNodeType.Element && r.Name == "Component") {
                        Guid id = XmlConvert.ToGuid(r.GetAttribute("TrackID"));
                        LayoutComponentConnectionPoint cp = LayoutComponentConnectionPoint.Parse(r.GetAttribute("Cp"));
                        int states = XmlConvert.ToInt32(r.GetAttribute("States"));
                        ModelTopologyEntry entry = new ModelTopologyEntry(states);

                        if (!r.IsEmptyElement) {
                            do {
                                r.Read();

                                if (r.NodeType == XmlNodeType.Element && r.Name == "ConnectTo") {
                                    int state = XmlConvert.ToInt32(r.GetAttribute("State"));
                                    Guid destinationID = XmlConvert.ToGuid(r.GetAttribute("TrackID"));
                                    LayoutComponentConnectionPoint destinationCp = LayoutComponentConnectionPoint.Parse(r.GetAttribute("Cp"));
                                    int penalty = XmlConvert.ToInt32(r.GetAttribute("Penalty"));

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
			if(!compiled) {
				foreach(ModelTopologyEntry entry in topology.Values) {
					for(int switchState = 0; switchState < entry.SwitchingStateCount; switchState++) {
						if(entry[switchState].Destination != TrackEdgeId.Empty)
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
			ModelTopology other = obj as ModelTopology;

			if(other == null)
				return false;

			foreach(KeyValuePair<TrackEdgeId, ModelTopologyEntry> d in topology) {
				TrackEdgeId edgeID = d.Key;
				ModelTopologyEntry entry = d.Value;

				if(other.Contains(edgeID)) {
					ModelTopologyEntry otherEntry = other[edgeID];

					if(!entry.Equals(otherEntry))
						return false;
				}
				else
					return false;
			}

			return true;
		}

		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("Topology");

			foreach(KeyValuePair<TrackEdgeId, ModelTopologyEntry> d in topology) {
				TrackEdgeId edgeID = d.Key;
				ModelTopologyEntry entry = d.Value;

				w.WriteStartElement("Component");
				w.WriteAttributeString("TrackID", XmlConvert.ToString(edgeID.TrackId));
				w.WriteAttributeString("Cp", edgeID.ConnectionPoint.ToString());
				w.WriteAttributeString("States", XmlConvert.ToString(entry.SwitchingStateCount));

				for(int i = 0; i < entry.SwitchingStateCount; i++) {
					if(entry[i].Destination != TrackEdgeId.Empty) {
						w.WriteStartElement("ConnectTo");
						w.WriteAttributeString("State", XmlConvert.ToString(i));
						w.WriteAttributeString("Cp", entry[i].Destination.ConnectionPoint.ToString());
						w.WriteAttributeString("TrackID", XmlConvert.ToString(entry[i].Destination.TrackId));
						w.WriteAttributeString("Penalty", XmlConvert.ToString(entry[i].Penalty));
						w.WriteEndElement();
					}
				}

				w.WriteEndElement();
			}

			w.WriteEndElement();
		}

        public override int GetHashCode() => base.GetHashCode();

        static ModelTopologyConnectionEntry getConnectedSplit(TrackEdge edge) {
			TrackEdgeDictionary visitedMergePoints = new TrackEdgeDictionary();
			int penalty = 0;

			while(edge != TrackEdge.Empty) {
				if(edge.Track is IModelComponentIsMultiPath) {
					IModelComponentIsMultiPath switchingTrack = (IModelComponentIsMultiPath)edge.Track;

					if(switchingTrack.IsSplitPoint(edge.ConnectionPoint))
						return new ModelTopologyConnectionEntry(new TrackEdgeId(edge), penalty);
					else {
						if(visitedMergePoints.ContainsKey(edge))
							edge = TrackEdge.Empty;
						else {
							visitedMergePoints.Add(edge, null);
							edge = edge.Track.GetConnectedComponentEdge(edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0]);
						}
					}
				}
				else {
					LayoutBlockDefinitionComponent blockInfo = edge.Track.BlockDefinitionComponent;

					if(blockInfo != null)
						penalty += blockInfo.Info.LengthInCM;

					edge = edge.Track.GetConnectedComponentEdge(edge.OtherConnectionPoint);
				}
			}

			return new ModelTopologyConnectionEntry(TrackEdgeId.Empty, 0);
		}
	}

	public class ModelTopologyConnectionEntry {
		ModelTopologyEntry destinationTopologyEntry;	// the destination entry
		TrackEdgeId destination;				// Where this connects to
		int penalty;					// connection penalty

		public ModelTopologyConnectionEntry(TrackEdgeId destination, int penalty) {
			this.destination = destination;
			this.penalty = penalty;
		}

        public TrackEdgeId Destination => destination;

        public int Penalty => penalty;

        public bool IsEmpty => destination.IsEmpty;

        public ModelTopologyEntry DestinationTopologyEntry {
			get {
				return destinationTopologyEntry;
			}

			set {
				destinationTopologyEntry = value;
			}
		}

		public override bool Equals(object obj) {
			if(obj == null)
				return destination == TrackEdgeId.Empty;

			ModelTopologyConnectionEntry other = obj as ModelTopologyConnectionEntry;

			if(other == null)
				return false;

			return destination.Equals(other.destination) && penalty == other.penalty;
		}

        public override int GetHashCode() => destination.GetHashCode() ^ penalty;
    }

	public class ModelTopologyEntry {
		int nStates;
		ModelTopologyConnectionEntry[] connections;
		bool visited;

		public ModelTopologyEntry(int stateCount) {
			this.nStates = stateCount;
			connections = new ModelTopologyConnectionEntry[stateCount];
		}

        public int SwitchingStateCount => nStates;

        public bool Visited {
			get {
				return visited;
			}

			set {
				visited = value;
			}
		}

		public ModelTopologyConnectionEntry this[int switchState] {
			get {
				return connections[switchState];
			}

			set {
				connections[switchState] = value;
			}
		}

		public override bool Equals(object obj) {
			ModelTopologyEntry other = obj as ModelTopologyEntry;

			if(other == null)
				return false;

			if(nStates != other.SwitchingStateCount)
				return false;

			for(int i = 0; i < nStates; i++)
				if(!this[i].Equals(other[i]))
					return false;

			return true;
		}

		public override int GetHashCode() {
			Debug.Assert(false);
			return base.GetHashCode();
		}
	}
}
