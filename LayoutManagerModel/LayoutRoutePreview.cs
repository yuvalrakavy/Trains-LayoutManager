using LayoutManager.Components;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace LayoutManager.Model {
    public struct TrackSegment {
        private LayoutComponentConnectionPoint cp1, cp2;

        static readonly LayoutStraightTrackComponent noTrack = new();
        public static readonly TrackSegment Empty = new(noTrack, 0, 0);

        public TrackSegment(LayoutTrackComponent track, LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            this.Track = track;
            this.cp1 = cp1;
            this.cp2 = cp2;
            InitalizeConnectionPoints(cp1, cp2);
        }

        public TrackSegment(LayoutTrackComponent track, IList<LayoutComponentConnectionPoint> cps) {
            if (cps.Count != 2)
                throw new ArgumentException("Track segment must be defined using two connection points");

            this.Track = track;
            this.cp1 = cps[0];
            this.cp2 = cps[1];
            InitalizeConnectionPoints(cps[0], cps[1]);
        }

        // Ensure order between cp1 and cp2.
        // If vertical then cp1=top cp2=bottom
        // If horizontal then cp1=left, cp2=right
        // If diagonal then cp1={horizontal} cp2={vertical}
        private void InitalizeConnectionPoints(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            if (LayoutTrackComponent.IsVertical(cp1, cp2)) {
                if (cp1 == LayoutComponentConnectionPoint.B) {
                    this.cp1 = cp2;
                    this.cp2 = cp1;
                }
            }
            else if (LayoutTrackComponent.IsHorizontal(cp1, cp2)) {
                if (cp1 == LayoutComponentConnectionPoint.R) {
                    this.cp1 = cp2;
                    this.cp2 = cp1;
                }
            }
            else {      // Diagonal
                if (LayoutTrackComponent.IsVertical(cp1)) {
                    this.cp1 = cp2;
                    this.cp2 = cp1;
                }
            }
        }

        public LayoutTrackComponent Track { get; }

        public IList<LayoutComponentConnectionPoint> ConnectionPoints => Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] { cp1, cp2 });

        public LayoutComponentConnectionPoint Cp1 => cp1;

        public LayoutComponentConnectionPoint Cp2 => cp2;

        public override string ToString() => $"{Track.FullDescription} ({cp1} to {cp2})";

        public override bool Equals(object? obj) {
            return obj is not TrackSegment other || (Track == other.Track && cp1 == other.cp1 && cp2 == other.cp2);
        }

        static public bool operator ==(TrackSegment s1, TrackSegment s2) => s1.Equals(s2);

        static public bool operator !=(TrackSegment s1, TrackSegment s2) => !s1.Equals(s2);

        public override int GetHashCode() => Track.GetHashCode() ^ cp1.GetHashCode() ^ cp2.GetHashCode();
    }

    /// <summary>
    /// Information needed to draw annotation on the preview route showing the locomotive orientation and motion direction.
    /// </summary>
    public class RoutePreviewAnnotation {
        public RoutePreviewAnnotation(LayoutComponentConnectionPoint front, LocomotiveOrientation direction) {
            Front = front;
            Direction = direction;
        }

        public LayoutComponentConnectionPoint Front { get; set; }

        public LocomotiveOrientation Direction { get; set; }
    }

    internal class TrackSegmentPreviewInfo {
        internal TrackSegmentPreviewInfo(RoutePreviewRequest request, RoutePreviewAnnotation? annotation) {
            Request = request;
            Annotation = annotation;
        }

        public RoutePreviewRequest Request { get; }

        public RoutePreviewAnnotation? Annotation { get; }
    }

    /// <summary>
    /// An object of this type is returned when the route preview manager is queried for a preview information
    /// for a given track segment
    /// </summary>
    public class TrackSegmentPreviewResult {
        private readonly RoutePreviewAnnotation[] _annotations;

        internal TrackSegmentPreviewResult(RoutePreviewRequest request, RoutePreviewAnnotation[] annotations) {
            Request = request;
            _annotations = annotations;
        }

        public RoutePreviewRequest Request { get; }

        public IList<RoutePreviewAnnotation> Annotations => _annotations;
    }

    internal struct PreviewRequestEntry {
        private TrackSegment _trackSegment;

        public PreviewRequestEntry(TrackSegment trackSegment) {
            _trackSegment = trackSegment;
            PreviewAnnotation = null;
        }

        public PreviewRequestEntry(TrackSegment trackSegment, RoutePreviewAnnotation previewAnnotation) {
            _trackSegment = trackSegment;
            PreviewAnnotation = previewAnnotation;
        }

        public TrackSegment TrackSegment => _trackSegment;

        public RoutePreviewAnnotation? PreviewAnnotation { get; }
    }

    public class RoutePreviewRequest {
        private readonly List<PreviewRequestEntry> requestEntries = new();

        public RoutePreviewRequest(Color color) {
            this.Color = color;
        }

        public RoutePreviewRequest(Color color, ITripRoute route, int insertAnnotationEvery) {
            this.Color = color;
            Add(route, insertAnnotationEvery);
        }

        public void Add(TrackSegment trackSegment, RoutePreviewAnnotation previewAnnotation) {
            requestEntries.Add(new PreviewRequestEntry(trackSegment, previewAnnotation));
        }

        public void Add(TrackSegment trackSegment) {
            requestEntries.Add(new PreviewRequestEntry(trackSegment));
        }

        public void Add(ITripRoute route, int insertAnnotationEvery) {
            ILayoutTopologyServices ts = Dispatch.Call.GetTopologyServices();
            TrackEdge edge = route.SourceEdge;
            TrackEdge destinationEdge = route.DestinationEdge;
            IList<int> switchStates = route.SwitchStates;
            int switchStateIndex = 0;
            bool continueScanning = true;
            int insertCount = 0;

            do {
                if (edge.Track == destinationEdge.Track)
                    continueScanning = false;
                else {
                    LayoutComponentConnectionPoint otherCp;

                    if (edge.Track is IModelComponentIsMultiPath turnout)
                        otherCp = turnout.ConnectTo(edge.ConnectionPoint, switchStates[switchStateIndex++]);
                    else
                        otherCp = edge.Track.ConnectTo(edge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];

                    TrackSegment trackSegment = new(edge.Track, edge.ConnectionPoint, otherCp);
                    bool trackSegmentAdded = false;

                    if (insertAnnotationEvery >= 0) {
                        if (insertCount == insertAnnotationEvery) {
                            if (edge.Track is LayoutStraightTrackComponent && !LayoutStraightTrackComponent.IsDiagonal(edge.Track.ConnectionPoints)) {
                                Add(trackSegment, new RoutePreviewAnnotation(route.Direction == LocomotiveOrientation.Forward ? otherCp : edge.ConnectionPoint,
                                    route.Direction));
                                insertCount = 0;
                                trackSegmentAdded = true;
                            }
                        }
                        else
                            insertCount++;
                    }

                    if (!trackSegmentAdded)
                        Add(trackSegment);

                    edge = ts.FindTrackConnectingAt(new TrackEdge(edge.Track, otherCp));
                }
            } while (continueScanning);
        }

        public void Add(ITripRoute route) {
            Add(route, -1);
        }

        internal IList<PreviewRequestEntry> RequestEntries => requestEntries;

        public Color Color { get; } = Color.Green;

        public bool Selected { get; set; }

        public void Redraw() {
            foreach (PreviewRequestEntry requestEntry in RequestEntries)
                requestEntry.TrackSegment.Track.Redraw();
        }

        public void EraseImage() {
            foreach (PreviewRequestEntry requestEntry in RequestEntries)
                requestEntry.TrackSegment.Track.EraseImage();
        }

        public void Dump() {
            Debug.Write(" Color: " + Color.ToString());
        }
    }

    public class PreviewRouteManager {
        private readonly Dictionary<TrackSegment, TrackSegmentMapEntry> trackSegmentMap = new();

        public void Add(RoutePreviewRequest previewRequest) {
            foreach (PreviewRequestEntry requestEntry in previewRequest.RequestEntries) {
                if (!trackSegmentMap.TryGetValue(requestEntry.TrackSegment, out TrackSegmentMapEntry? entry)) {
                    entry = new TrackSegmentMapEntry();
                    trackSegmentMap.Add(requestEntry.TrackSegment, entry);
                }

                entry.Add(new TrackSegmentPreviewInfo(previewRequest, requestEntry.PreviewAnnotation));
            }

            previewRequest.Redraw();
        }

        public void Remove(RoutePreviewRequest previewRequest) {
            foreach (PreviewRequestEntry requestEntry in previewRequest.RequestEntries) {
                TrackSegmentMapEntry entry = trackSegmentMap[requestEntry.TrackSegment];

                Debug.Assert(entry != null);

                if (entry.AnnotationCount > 0)
                    requestEntry.TrackSegment.Track.EraseImage();

                entry.Remove(previewRequest);
                if (entry.IsEmpty)
                    trackSegmentMap.Remove(requestEntry.TrackSegment);
            }

            previewRequest.Redraw();
        }

        public TrackSegmentPreviewResult? this[TrackSegment trackSegment] {
            get {
                // TODO: May need to merge the preview annotation info from all preview requests that are displayed for this track segment

                return trackSegmentMap.TryGetValue(trackSegment, out TrackSegmentMapEntry? entry) && entry.TopPreviewInfo != null
                    ? new TrackSegmentPreviewResult(entry.TopPreviewInfo.Request, entry.Annotations)
                    : null;
            }
        }

        public void Dump() {
            Debug.WriteLine("Route Preview Manager Dump");

            foreach (KeyValuePair<TrackSegment, TrackSegmentMapEntry> d in trackSegmentMap) {
                TrackSegment trackSegment = d.Key;
                TrackSegmentMapEntry entry = d.Value;

                Debug.WriteLine("Segment: " + trackSegment.ToString());
                entry.Dump();
                Debug.WriteLine("");
            }
        }

        private class TrackSegmentMapEntry {
            private TrackSegmentPreviewInfo? topPreviewInfo;
            private List<TrackSegmentPreviewInfo>? otherPreviewInfos;
            private int annotationCount = -1;

            public void Add(TrackSegmentPreviewInfo previewInfo) {
                annotationCount = -1;

                if (topPreviewInfo == null)
                    topPreviewInfo = previewInfo;
                else {
                    if (otherPreviewInfos == null) {
                        otherPreviewInfos = new List<TrackSegmentPreviewInfo> {
                            topPreviewInfo
                        };
                    }
                    otherPreviewInfos.Add(previewInfo);
                    topPreviewInfo = previewInfo;
                }
            }

            public void Remove(RoutePreviewRequest previewRequest) {
                bool setTopPreviewInfo = false;

                annotationCount = -1;

                if (topPreviewInfo != null && previewRequest == topPreviewInfo.Request)
                    setTopPreviewInfo = true;

                if (otherPreviewInfos != null) {
                    TrackSegmentPreviewInfo? previewInfoToRemove = null;

                    foreach (TrackSegmentPreviewInfo previewInfo in otherPreviewInfos)
                        if (previewInfo.Request == previewRequest) {
                            previewInfoToRemove = previewInfo;
                            break;
                        }

                    if (previewInfoToRemove != null) {
                        otherPreviewInfos.Remove(previewInfoToRemove);
                        if (otherPreviewInfos.Count == 0)
                            otherPreviewInfos = null;
                    }
                }

                if (setTopPreviewInfo) {
                    if (otherPreviewInfos != null)
                        topPreviewInfo = otherPreviewInfos[^1];
                    else
                        topPreviewInfo = null;
                }
            }

            public TrackSegmentPreviewInfo? TopPreviewInfo => topPreviewInfo;

            public bool IsEmpty => topPreviewInfo == null && otherPreviewInfos == null;

            public int AnnotationCount {
                get {
                    if (annotationCount < 0) {
                        annotationCount = 0;

                        if (otherPreviewInfos != null) {
                            foreach (TrackSegmentPreviewInfo previewInfo in otherPreviewInfos)
                                if (previewInfo.Annotation != null)
                                    annotationCount++;
                        }
                        else if (topPreviewInfo != null && topPreviewInfo.Annotation != null)
                            annotationCount++;
                    }

                    return annotationCount;
                }
            }

            public RoutePreviewAnnotation[] Annotations {
                get {
                    RoutePreviewAnnotation[] annotations = new RoutePreviewAnnotation[AnnotationCount];

                    if (otherPreviewInfos == null) {
                        if (topPreviewInfo != null && topPreviewInfo.Annotation != null)
                            annotations[0] = topPreviewInfo.Annotation;
                    }
                    else {
                        int i = 0;

                        foreach (TrackSegmentPreviewInfo previewInfo in otherPreviewInfos) {
                            if (previewInfo.Annotation != null)
                                annotations[i++] = previewInfo.Annotation;
                        }
                    }

                    return annotations;
                }
            }

            public void Dump() {
                Debug.Write("TopEntry: ");
                if (topPreviewInfo == null)
                    Debug.Write("<null> ");
                else
                    topPreviewInfo.Request.Dump();

                Debug.Write(" OtherRequests: ");
                if (otherPreviewInfos == null)
                    Debug.Write("<null> ");
                else {
                    foreach (TrackSegmentPreviewInfo previewInfo in otherPreviewInfos)
                        previewInfo.Request.Dump();
                }
            }
        }
    }
}
