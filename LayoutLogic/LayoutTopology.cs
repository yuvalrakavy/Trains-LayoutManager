using System;
using System.Collections.Generic;
using System.Drawing;
using LayoutManager.Components;
using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace LayoutManager.Logic {
    [LayoutModule("Layout Topology Services", UserControl = false)]
    public class LayoutTopologyServices : LayoutModuleBase, ILayoutTopologyServices {
        public static string TopologyServicesVersion = "1.0";

        [LayoutEvent("get-topology-services")]
        private void GetTopologyServices(LayoutEvent e) {
            e.Info = (ILayoutTopologyServices)this;
        }

        /// <summary>
        /// Find the TrackEdge (a combination of a track component and a connection point) that connects
        /// to a given track edge. First looking for track in the same layer. If one cannot be found, look
        /// to see if there is one in another layer. The function will issue a warning if more than one
        /// track could connect to the given track
        /// </summary>
        /// <param name="trackEdge">The track edge for which a track connection is looked for</param>
        /// <param name="trackLinks">If true, track links are followed when looking for connection</param>
        public TrackEdge FindTrackConnectingAt(TrackEdge trackEdge, LayoutPhase phase, bool trackLinks) {
            LayoutTrackLinkComponent? trackLink = null;

            if (trackLinks)
                trackLink = trackEdge.Track.Spot[ModelComponentKind.TrackLink] as LayoutTrackLinkComponent;

            if (trackLink == null) {
                LayoutTrackComponent? newTrack = null;
                Point newTrackLocation = trackEdge.Track.Location + LayoutTrackComponent.GetConnectionOffset(trackEdge.ConnectionPoint);
                LayoutComponentConnectionPoint newTrackConnectionPoint = LayoutTrackComponent.GetPointConnectingTo(trackEdge.ConnectionPoint);
                var spot = trackEdge.Track.Spot.Area[newTrackLocation, phase];

                if (spot != null && (spot.Phase & phase) != 0) {
                    // See if there is a connecting track in the current track layer
                    newTrack = spot.Track;

                    if (newTrack != null) {
                        if (newTrack.ConnectTo(newTrackConnectionPoint, LayoutComponentConnectionType.Topology) == null)
                            newTrack = null;
                    }
                }

                if (newTrack != null)
                    return new TrackEdge(newTrack, newTrackConnectionPoint);
            }
            else {
                var linkedTrack = trackLink.LinkedTrack;

                if (linkedTrack != null) {
                    // The linked was indeed linked to another track.

                    // Look for if there is track that connected to the linked track. If there is, this is
                    // the one we want. Please note that findTrackConnectingTo is called with the option
                    // to track links set to false. This is in order to avoid recursive link tracking
                    foreach (LayoutComponentConnectionPoint cp in linkedTrack.ConnectionPoints) {
                        TrackEdge edge = FindTrackConnectingAt(new TrackEdge(linkedTrack, cp), phase, false);

                        if (edge != TrackEdge.Empty)
                            return edge;
                    }
                }
            }

            return TrackEdge.Empty;
        }

        /// <summary>
        /// Find the TrackEdge (a combination of a track component and a connection point) that connects
        /// to a given track edge. First looking for track in the same layer. If one cannot be found, look
        /// to see if there is one in another layer. The function will issue a warning if more than one
        /// track could connect to the given track. This function follows track links
        /// </summary>
        /// <param name="edge">The track edge for which a track connection is looked for</param>
        public TrackEdge FindTrackConnectingAt(TrackEdge edge) => edge.Track.GetConnectedComponentEdge(edge.ConnectionPoint);

        /// <summary>
        /// Find all track and connection points that connects to the given track. First, all connection points
        /// that connect to the given connection point are found. Then tracks that connect to those connection
        /// points are looked for.
        /// </summary>
        /// <param name="edge">The track/connection point combination to look from</param>
        /// <param name="type">The type of connection</param>
        /// <returns>An array of all track that connect to the current track</returns>
        public TrackEdge[] FindConnectingTracks(TrackEdge edge, LayoutComponentConnectionType type) {
            LayoutComponentConnectionPoint[] connectedCp = edge.Track.ConnectTo(edge.ConnectionPoint, type);

            List<TrackEdge> connectedEdges = new(connectedCp.Length);

            foreach (LayoutComponentConnectionPoint cp in connectedCp) {
                TrackEdge otherEdge = new(edge.Track, cp);
                TrackEdge otherTrackEdge = FindTrackConnectingAt(otherEdge);

                if (otherTrackEdge != TrackEdge.Empty)
                    connectedEdges.Add(otherTrackEdge);
            }

            return connectedEdges.ToArray();
        }
    }
}
