using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using System.Diagnostics;

using LayoutManager.Model;

#nullable enable
namespace LayoutManager.Components {
    /// <summary>
    /// Generic location on the grid and a connection point in this grid. A component can
    /// map this to a concrete component connection point. For simple tracks, this mapping is
    /// very simple, since the track component occupies exactly one grid location. For complex
    /// components (for example turntable) the mapping map be more complex
    /// </summary>
    public class LayoutGridConnectionPoint {
        private Point ml;

        public LayoutGridConnectionPoint(Point ml, LayoutComponentConnectionPoint cp) {
            if (cp != LayoutComponentConnectionPoint.T &&
                cp != LayoutComponentConnectionPoint.B &&
                cp != LayoutComponentConnectionPoint.R &&
                cp != LayoutComponentConnectionPoint.L)
                throw new ArgumentException("Invalid connection point for GridConnectionPoint", nameof(cp));

            this.ConnectionPoint = cp;
            this.ml = ml;
        }

        public Point Location => ml;

        public LayoutComponentConnectionPoint ConnectionPoint { get; }
    }

    public enum LayoutComponentConnectionType {
        /// <summary>
        /// The connection point(s) to which a train may pass from a given point
        /// </summary>
        Passage,

        /// <summary>
        /// The connection point(s) to which there is electrical connection from a given point
        /// </summary>
        /// <remarks>
        /// Usually the returned value will be the same as for topological connection type except
        /// if the track is isolated.
        /// </remarks>
        Electrical,

        /// <summary>
        /// The connection point(s) to which there is electrical connection without reverse loops
        /// </summary>
        /// <remarks>
        /// Usually the returned value will be the same as for topological connection type except
        /// if the track is isolated or there is a reverse loop module.
        /// </remarks>
        ReverseLoop,

        /// <summary>
        /// The connection point(s) to which there this points connects to.
        /// </summary>
        /// <remarks>
        /// This connection type is used when one need to traverse the layout
        /// </remarks>
        Topology,
    }

    /// <summary>
    /// This interface describe a component that can be connected to other components
    /// (for example tracks
    /// </summary>
    public interface ILayoutConnectableComponent {
        /// <summary>
        /// Get a bit mask describing the points to which this component connects
        /// </summary>
        IList<LayoutComponentConnectionPoint> ConnectionPoints {
            get;
        }

        /// <summary>
        /// Get the connections possible from a given point. ArgumentException is thrown
        /// if the component does not have the given connection point. For example, on a
        /// simple track this function will return the "other" side of the track. For
        /// a branch, this function may return a mask containing two connection points.
        /// </summary>
        /// <param name="from">The connection point to start from</param>
        /// <returns>The connection points that this track connects to</returns>
        LayoutComponentConnectionPoint[] ConnectTo(LayoutComponentConnectionPoint from, LayoutComponentConnectionType type);

        /// <summary>
        /// Return the layout block associated with a given cp
        /// </summary>
        /// <param name="cp">The connection point</param>
        /// <remarks>
        /// Only cross and parallel diagnonal components can be part of two blocks. Therefore
        /// the connection point is required to indicate which block
        /// </remarks>
        LayoutBlock GetBlock(LayoutComponentConnectionPoint cp);

        /// <summary>
        /// Set the layout block to associate with a given layout connection point
        /// </summary>
        /// <param name="cp">The connection point</param>
        /// <param name="block">The block</param>
        /// <remarks>
        /// Only cross and parallel diagnonal components can be part of two blocks. Therefore
        /// the connection point is required to indicate which block
        /// </remarks>
        void SetBlock(LayoutComponentConnectionPoint cp, LayoutBlock? block);

        /// <summary>
        /// Check if the component connects to the given connection point
        /// </summary>
        /// <param name="cp">The connection point</param>
        /// <returns>True if the component connects to this connection point</returns>
        bool HasConnectionPoint(LayoutComponentConnectionPoint cp);

        /// <summary>
        /// The component free all references to connected components information. This method is called when
        /// switching out from operation mode
        /// </summary>
        void CleanupComponentEdgeConnections();

        /// <summary>
        /// Get a component connected to a given connection point. 
        /// </summary>
        /// <param name="cp">The connection point</param>
        /// <returns>The connected component edge, or TrackEdge.Empty if this connection point is not connected</returns>
        TrackEdge GetConnectedComponentEdge(LayoutComponentConnectionPoint cp);

        /// <summary>
        /// Set the component which is connected to a given component
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="component"></param>
        void SetConnectedComponentEdge(LayoutComponentConnectionPoint cp, TrackEdge edge);
    }

    /// <summary>
    /// Base class for track components. It implements few methods of the ILayoutConnectableComponent
    /// interface.
    /// </summary>
    public abstract class LayoutTrackComponent : ModelComponent, ILayoutConnectableComponent {
        private ModelComponent? trackAnnotation; // A component such as track contact, block edge etc. that is on this track			
        private TrackEdge[]? connectedEdges;     // The components connected to this one indexed by the connection point

        public override ModelComponentKind Kind => ModelComponentKind.Track;

        public abstract IList<LayoutComponentConnectionPoint> ConnectionPoints {
            get;
        }

        public abstract LayoutComponentConnectionPoint[] ConnectTo(LayoutComponentConnectionPoint from, LayoutComponentConnectionType type);

        public abstract LayoutBlock? GetOptionalBlock(LayoutComponentConnectionPoint cp);

        public LayoutBlock GetBlock(LayoutComponentConnectionPoint cp) => GetOptionalBlock(cp) ?? throw new ArgumentNullException("GetBlock called when block is null");

        public abstract void SetBlock(LayoutComponentConnectionPoint cp, LayoutBlock? block);

        public abstract LayoutTrackPowerConnectorComponent? GetPowerConnector(LayoutComponentConnectionPoint cp);

        public abstract void SetPowerConnector(LayoutComponentConnectionPoint cp, LayoutTrackPowerConnectorComponent? powerConnectorComponent);

        public virtual int GetSwitchState(LayoutComponentConnectionPoint cpSource, LayoutComponentConnectionPoint cpDestination) {
            Debug.Fail("Component is not multi-path component");
            return 0;
        }

        public ILayoutPower? GetOptionalPower(LayoutComponentConnectionPoint cp) {
            var powerConnector = GetPowerConnector(cp);

            if(powerConnector != null)
                return powerConnector.Inlet.ConnectedOutlet.OptionalPower;
            return null;
        }

        public ILayoutPower GetPower(LayoutComponentConnectionPoint cp) => Ensure.NotNull<ILayoutPower>(GetOptionalPower(cp), "GetOptionalPower");

        /// <summary>
        /// Given a connection point, returns the offset to get to the component that
        /// connects to this connection. For example L returns (-1, 0)
        /// </summary>
        /// <param name="point">The connection point</param>
        /// <returns>The offset</returns>
        public static Size GetConnectionOffset(LayoutComponentConnectionPoint point) => (int)point switch
        {
            LayoutComponentConnectionPoint.T => new Size(0, -1),
            LayoutComponentConnectionPoint.B => new Size(0, 1),
            LayoutComponentConnectionPoint.R => new Size(1, 0),
            LayoutComponentConnectionPoint.L => new Size(-1, 0),
            _ => throw new ArgumentException("Invalid track connection point"),
        };

        public static LayoutComponentConnectionPoint GetPointConnectingTo(LayoutComponentConnectionPoint cp) => (int)cp switch
        {
            LayoutComponentConnectionPoint.T => LayoutComponentConnectionPoint.B,
            LayoutComponentConnectionPoint.B => LayoutComponentConnectionPoint.T,
            LayoutComponentConnectionPoint.R => LayoutComponentConnectionPoint.L,
            LayoutComponentConnectionPoint.L => LayoutComponentConnectionPoint.R,
            _ => throw new ArgumentException("Invalid track connection point"),
        };

        public static bool IsHorizontal(LayoutComponentConnectionPoint cp) => cp == LayoutComponentConnectionPoint.L || cp == LayoutComponentConnectionPoint.R;

        public static bool IsHorizontal(IList<LayoutComponentConnectionPoint> cps) {
            foreach (LayoutComponentConnectionPoint cp in cps)
                if (!IsHorizontal(cp))
                    return false;
            return true;
        }

        public static bool IsHorizontal(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) => IsHorizontal(cp1) && IsHorizontal(cp2);

        public static bool IsHorizontal(LayoutTrackComponent component) => IsHorizontal(component.ConnectionPoints);

        public static bool IsVertical(LayoutComponentConnectionPoint cp) => cp == LayoutComponentConnectionPoint.T || cp == LayoutComponentConnectionPoint.B;

        public static bool IsVertical(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) => IsVertical(cp1) && IsVertical(cp2);

        public static bool IsVertical(IList<LayoutComponentConnectionPoint> cps) {
            foreach (LayoutComponentConnectionPoint cp in cps)
                if (!IsVertical(cp))
                    return false;
            return true;
        }

        public static bool IsVertical(LayoutTrackComponent component) => IsVertical(component.ConnectionPoints);

        public static bool IsDiagonal(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) => (IsHorizontal(cp1) && IsVertical(cp2)) || (IsHorizontal(cp2) && IsVertical(cp1));

        public static bool IsDiagonal(IList<LayoutComponentConnectionPoint> cps) {
            Debug.Assert(cps.Count == 2, "IsDiagonal cps.Count != 2");
            return IsDiagonal(cps[0], cps[1]);
        }

        public static bool IsDiagonal(LayoutTrackComponent component) => IsDiagonal(component.ConnectionPoints);

        public static LayoutComponentConnectionPoint OppositeConnectPoint(LayoutComponentConnectionPoint cp) => (int)cp switch
        {
            LayoutComponentConnectionPoint.B => LayoutComponentConnectionPoint.T,
            LayoutComponentConnectionPoint.T => LayoutComponentConnectionPoint.B,
            LayoutComponentConnectionPoint.L => LayoutComponentConnectionPoint.R,
            LayoutComponentConnectionPoint.R => LayoutComponentConnectionPoint.L,

            _ => throw new ArgumentException("No opposite connection point is defined for " + cp.ToString()),
        };

        public static LayoutComponentConnectionPoint[] DiagonalConnectionPoints(LayoutComponentConnectionPoint cp) {
            switch (cp) {
                case LayoutComponentConnectionPoint.B:
                case LayoutComponentConnectionPoint.T:
                    return new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R };

                case LayoutComponentConnectionPoint.L:
                case LayoutComponentConnectionPoint.R:
                    return new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };

                default:
                    throw new ArgumentException("No diagonal connection points are defined for " + cp.ToString());
            }
        }

        /// <summary>
        /// Check if the component connects to the given connection point
        /// </summary>
        /// <param name="cpToCheck">The connection point</param>
        /// <returns>True if the component connects to this connection point</returns>
        public bool HasConnectionPoint(LayoutComponentConnectionPoint cpToCheck) {
            foreach (LayoutComponentConnectionPoint cp in ConnectionPoints)
                if (cp == cpToCheck)
                    return true;
            return false;
        }

        public void CleanupComponentEdgeConnections() {
            connectedEdges = null;
        }

        public void SetConnectedComponentEdge(LayoutComponentConnectionPoint cp, TrackEdge edge) {
            if (connectedEdges == null) {
                connectedEdges = new TrackEdge[4];

                for (int i = 0; i < connectedEdges.Length; i++)
                    connectedEdges[i] = TrackEdge.Empty;
            }

            Debug.Assert(connectedEdges[cp] == TrackEdge.Empty);

            connectedEdges[cp] = edge;
        }

        public TrackEdge GetConnectedComponentEdge(LayoutComponentConnectionPoint cp) => connectedEdges![cp];

        public override bool DrawOutOfGrid => false;

        #region Track Annoation methods and properties

        public void SetTrackAnnotation() =>
            trackAnnotation = (((Spot[ModelComponentKind.BlockEdge] ?? Spot[ModelComponentKind.BlockInfo]) ?? Spot[ModelComponentKind.TrackIsolation]) ?? Spot[ModelComponentKind.TrackLink]) ?? (this);

        public ModelComponent? TrackAnnotation => trackAnnotation == this ? null : trackAnnotation;

        public ModelComponent? TrackBackground {
            get {
                var background = Spot[ModelComponentKind.Background];

                return background;
            }
        }

        public LayoutBlockEdgeBase? BlockEdgeBase => trackAnnotation as LayoutBlockEdgeBase;

        public LayoutBlockEdgeComponent? BlockEdgeComponent => trackAnnotation as LayoutBlockEdgeComponent;

        public LayoutTrackContactComponent? TrackContactComponent => trackAnnotation as LayoutTrackContactComponent;

        public LayoutProximitySensorComponent? ProximitySensorComponent => trackAnnotation as LayoutProximitySensorComponent;

        public LayoutBlockDefinitionComponent? BlockDefinitionComponent => trackAnnotation as LayoutBlockDefinitionComponent;

        public LayoutTrackIsolationComponent? TrackIsolationComponent => trackAnnotation as LayoutTrackIsolationComponent;

        public LayoutTrackLinkComponent? TrackLinkComponent => trackAnnotation as LayoutTrackLinkComponent;

        #endregion
    }

    /// <summary>
    /// Track connecting cp1 to cp2
    /// </summary>
    public class LayoutStraightTrackComponent : LayoutTrackComponent {
        private LayoutComponentConnectionPoint cp1, cp2;

        private LayoutBlock? block;
        private LayoutBlock? optionalContactBlock;

        private LayoutTrackPowerConnectorComponent? powerConnector;
        private LayoutTrackPowerConnectorComponent? optionalPowerConnector;

        public LayoutStraightTrackComponent(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            this.cp1 = cp1;
            this.cp2 = cp2;
        }

        public LayoutStraightTrackComponent() {
        }

        // Implement ILayoutConnectableComponent
        public override IList<LayoutComponentConnectionPoint> ConnectionPoints => Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] { cp1, cp2 });

        public override LayoutComponentConnectionPoint[] ConnectTo(LayoutComponentConnectionPoint from, LayoutComponentConnectionType type) {
            // For topology type connection, track with track contacts are disconnection points
            if ((type == LayoutComponentConnectionType.Electrical && LayoutTrackIsolationComponent.Is(Spot)) || (type == LayoutComponentConnectionType.ReverseLoop && LayoutTrackReverseLoopModule.Is(Spot)))
                return Array.Empty<LayoutComponentConnectionPoint>();
            else {
                if (from == cp1)
                    return new LayoutComponentConnectionPoint[] { cp2 };
                else return from == cp2 ? (new LayoutComponentConnectionPoint[] { cp1 }) : (Array.Empty<LayoutComponentConnectionPoint>());
            }
        }

        /// <summary>
        /// Return the block associated with a given connection point.
        /// </summary>
        /// <param name="cp">The connection point</param>
        /// <returns>The layout block</returns>
        /// <remarks>
        /// Straight track are are part of one block, except when this track has a BLOCK_EDGE (track contact).
        /// If the track is associated with a BLOCK_EDGE, then it separate two blocks. One if found in cp1, and the
        /// other in cp2.
        /// </remarks>
        public override LayoutBlock? GetOptionalBlock(LayoutComponentConnectionPoint cp) {
            if (BlockEdgeBase != null) {
                if (cp == cp1)
                    return block;
                else if (cp == cp2)
                    return optionalContactBlock;
            }
            else if (cp == cp1 || cp == cp2) {
                return block;
            }

            throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override void SetBlock(LayoutComponentConnectionPoint cp, LayoutBlock? block) {
            if (BlockEdgeBase != null) {
                if (cp == cp1)
                    this.block = block;
                else if (cp == cp2)
                    this.optionalContactBlock = block;
            }
            else if (cp == cp1 || cp == cp2)
                this.block = block;
            else
                throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        /// <summary>
        /// Return the power source associated with a given connection point.
        /// </summary>
        /// <param name="cp">The connection point</param>
        /// <returns>The power source</returns>
        /// <remarks>
        /// Usually a track has one power source, unless it is composed with a track isolation component.
        /// In this case, the track has two power sources, one for the first connection point and another
        /// for the second one
        /// </remarks>
        public override LayoutTrackPowerConnectorComponent? GetPowerConnector(LayoutComponentConnectionPoint cp) {
            if (LayoutTrackIsolationComponent.Is(Spot)) {
                if (cp == cp1) {
                    return powerConnector;
                }
                else if (cp == cp2) {
                    Debug.Assert(optionalPowerConnector != null);
                    return optionalPowerConnector;
                }
            }
            else if (cp == cp1 || cp == cp2)
                return powerConnector;

            throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override void SetPowerConnector(LayoutComponentConnectionPoint cp, LayoutTrackPowerConnectorComponent? powerConnectorComponent) {
            if (TrackIsolationComponent != null) {
                if (cp == cp1)
                    this.powerConnector = powerConnectorComponent;
                else if (cp == cp2)
                    this.optionalPowerConnector = powerConnectorComponent;
            }
            else if (cp == cp1 || cp == cp2)
                this.powerConnector = powerConnectorComponent;
            else
                throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public bool IsDiagonal() => IsDiagonal(cp1, cp2);

        public override string ToString() => "track";

        public override void WriteXmlFields(XmlWriter w) {
            w.WriteStartElement("Connections");
            w.WriteAttributeString("cp1", cp1.ToString());
            w.WriteAttributeString("cp2", cp2.ToString());
            w.WriteEndElement();

            // TODO: Write block association (may not be needed if layout is compiled after loading)
        }

        protected override bool ReadXmlField(XmlReader r) {
            if (r.Name == "Connections") {
                cp1 = LayoutComponentConnectionPoint.Parse(r.GetAttribute("cp1"));
                cp2 = LayoutComponentConnectionPoint.Parse(r.GetAttribute("cp2"));
                r.Read();
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// A component that contains two tracks. One connecting between cp1 and cp2
    /// and the other connects between cp3 and cp4. For example cross, or two diagnal
    /// tracks.
    /// </summary>
    public class LayoutDoubleTrackComponent : LayoutTrackComponent {
        private LayoutComponentConnectionPoint cp1, cp2;
        private LayoutBlock? cp12block;
        private LayoutTrackPowerConnectorComponent? cp12powerConnector;
        private LayoutComponentConnectionPoint cp3, cp4;
        private LayoutBlock? cp34block;
        private LayoutTrackPowerConnectorComponent? cp34powerConnector;
        private bool isCross;

        public LayoutDoubleTrackComponent(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            // Given cp1 and cp2, figure out cp3 and cp4
            if (IsDiagonal(cp1, cp2)) {
                this.cp1 = IsVertical(cp1) ? cp1 : cp2;
                this.cp2 = IsVertical(cp1) ? cp2 : cp1;

                cp3 = (this.cp1 == LayoutComponentConnectionPoint.T) ?
                    LayoutComponentConnectionPoint.B : LayoutComponentConnectionPoint.T;
                cp4 = (this.cp2 == LayoutComponentConnectionPoint.R) ?
                    LayoutComponentConnectionPoint.L : LayoutComponentConnectionPoint.R;
            }
            else {
                this.cp1 = cp1;
                this.cp2 = cp2;

                if (IsHorizontal(this.cp1)) {
                    cp3 = LayoutComponentConnectionPoint.T;
                    cp4 = LayoutComponentConnectionPoint.B;
                }
                else {
                    cp3 = LayoutComponentConnectionPoint.L;
                    cp4 = LayoutComponentConnectionPoint.R;
                }
            }

            isCross = !LayoutTrackComponent.IsDiagonal(cp1, cp2);
        }

        public LayoutDoubleTrackComponent() {
        }

        public bool IsCross => isCross;

        // Implement ILayoutConnectableComponent
        public override IList<LayoutComponentConnectionPoint> ConnectionPoints => Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] { cp1, cp2, cp3, cp4 });

        public LayoutComponentConnectionPoint[]? GetTrackPath(int n) {
            if (n == 0)
                return new LayoutComponentConnectionPoint[] { cp1, cp2 };
            else return n == 1 ? (new LayoutComponentConnectionPoint[] { cp3, cp4 }) : null;
        }

        public override LayoutComponentConnectionPoint[] ConnectTo(LayoutComponentConnectionPoint from, LayoutComponentConnectionType type) {
            if (from == cp1)
                return new LayoutComponentConnectionPoint[] { cp2 };
            else if (from == cp2)
                return new LayoutComponentConnectionPoint[] { cp1 };
            else if (from == cp3)
                return new LayoutComponentConnectionPoint[] { cp4 };
            else if (from == cp4)
                return new LayoutComponentConnectionPoint[] { cp3 };

            throw new LayoutException("Invalid source connection point");
        }

        public override LayoutBlock? GetOptionalBlock(LayoutComponentConnectionPoint cp) {
            if (cp == cp1 || cp == cp2)
                return cp12block!;
            else if (cp == cp3 || cp == cp4)
                return cp34block!;
            throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override void SetBlock(LayoutComponentConnectionPoint cp, LayoutBlock? block) {
            if (cp == cp1 || cp == cp2)
                cp12block = block;
            else if (cp == cp3 || cp == cp4)
                cp34block = block;
            else
                throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public LayoutBlock GetOtherBlock(LayoutComponentConnectionPoint cp) {
            if (cp == cp1 || cp == cp2)
                return cp34block!;
            else if (cp == cp3 || cp == cp4)
                return cp12block!;
            throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override LayoutTrackPowerConnectorComponent? GetPowerConnector(LayoutComponentConnectionPoint cp) {
            if (cp == cp1 || cp == cp2)
                return cp12powerConnector;
            else if (cp == cp3 || cp == cp4)
                return cp34powerConnector;
            throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override void SetPowerConnector(LayoutComponentConnectionPoint cp, LayoutTrackPowerConnectorComponent? powerConnectorComponent) {
            if (cp == cp1 || cp == cp2)
                cp12powerConnector = powerConnectorComponent;
            else if (cp == cp3 || cp == cp4)
                cp34powerConnector = powerConnectorComponent;
            else
                throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override string ToString() {
            return isCross ? "cross" : "track";
        }

        public override void WriteXmlFields(XmlWriter w) {
            w.WriteStartElement("Connections");
            w.WriteAttributeString("cp1", cp1.ToString());
            w.WriteAttributeString("cp2", cp2.ToString());
            w.WriteAttributeString("cp3", cp3.ToString());
            w.WriteAttributeString("cp4", cp4.ToString());
            w.WriteEndElement();

            // TODO: Write block association (may not be needed if layout is compiled after loading)
        }

        protected override bool ReadXmlField(XmlReader r) {
            if (r.Name == "Connections") {
                cp1 = LayoutComponentConnectionPoint.Parse(r.GetAttribute("cp1"));
                cp2 = LayoutComponentConnectionPoint.Parse(r.GetAttribute("cp2"));
                cp3 = LayoutComponentConnectionPoint.Parse(r.GetAttribute("cp3"));
                cp4 = LayoutComponentConnectionPoint.Parse(r.GetAttribute("cp4"));
                r.Read();

                isCross = !LayoutTrackComponent.IsDiagonal(cp1, cp2);
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Base class for components that are multi-path (e.g. turnout, double-slip)
    /// </summary>
    public abstract class LayoutMultiPathTrackComponent : LayoutTrackComponent, IModelComponentHasSwitchingState, IModelComponentHasId {
        private const string A_ReverseLogic = "ReverseLogic";
        private const string A_BuiltinDecoderTypeName = "BuiltinDecoderTypeName";
        private LayoutBlock? block;
        private LayoutTrackPowerConnectorComponent? powerConnector;
        private readonly SwitchingStateSupport switchingStateSupport;

        protected LayoutMultiPathTrackComponent() {
            switchingStateSupport = GetSwitchingStateSupporter();
        }

        protected virtual SwitchingStateSupport GetSwitchingStateSupporter() => new SwitchingStateSupport(this);

        public virtual int SwitchStateCount => switchingStateSupport.SwitchStateCount;

        /// <summary>
        /// Get or set 
        /// </summary>
        public virtual int CurrentSwitchState => switchingStateSupport.CurrentSwitchState;

        public virtual void AddSwitchingCommands(IList<SwitchingCommand> switchingCommands, int switchingState, string connectionPointName) {
            switchingStateSupport.AddSwitchingCommands(switchingCommands, switchingState, connectionPointName);
        }

        /// <summary>
        /// This method actually change the turnout run-time state. It should be called only from the turnout state changed
        /// notification handler, and not directly.
        /// </summary>
        /// <param name="switchState">The new switch state</param>
        public virtual void SetSwitchState(ControlConnectionPoint connectionPoint, int switchState, string connectionPointName) {
            switchingStateSupport.SetSwitchState(connectionPoint, switchState, connectionPointName);
        }

        public override LayoutBlock? GetOptionalBlock(LayoutComponentConnectionPoint cp) {
            if (HasConnectionPoint(cp))
                return block;
            throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override void SetBlock(LayoutComponentConnectionPoint cp, LayoutBlock? block) {
            if (HasConnectionPoint(cp))
                this.block = block;
        }

        public override LayoutTrackPowerConnectorComponent? GetPowerConnector(LayoutComponentConnectionPoint cp) {
            if (HasConnectionPoint(cp)) {
                Debug.Assert(powerConnector != null);
                return powerConnector;
            }
            throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public override void SetPowerConnector(LayoutComponentConnectionPoint cp, LayoutTrackPowerConnectorComponent? powerConnectorComponent) {
            if (HasConnectionPoint(cp))
                this.powerConnector = powerConnectorComponent;
            else
                throw new ArgumentException("Track does not have this connection point", nameof(cp));
        }

        public abstract LayoutComponentConnectionPoint ConnectTo(LayoutComponentConnectionPoint from, int switchState);

        public override int GetSwitchState(LayoutComponentConnectionPoint cpSource, LayoutComponentConnectionPoint cpDestination) {
            for (int switchState = 0; switchState < SwitchStateCount; switchState++)
                if (ConnectTo(cpSource, switchState) == cpDestination)
                    return switchState;

            throw new LayoutException(this, "No passage between " + cpSource.ToString() + " and " + cpDestination.ToString());
        }

        public bool IsSplitPoint(LayoutComponentConnectionPoint cp) {
            LayoutComponentConnectionPoint[] connectTo = ConnectTo(cp, LayoutComponentConnectionType.Passage);

            return connectTo != null && connectTo.Length > 1;
        }

        /// <summary>
        /// Check if this connection point is merging into another path
        /// </summary>
        /// <param name="cp"></param>
        /// <remarks>Check if any connection point that this connection point connects to can connect to more than one connection point</remarks>
        /// <returns></returns>
        public bool IsMergePoint(LayoutComponentConnectionPoint cp) {
            LayoutComponentConnectionPoint[] connectTo = ConnectTo(cp, LayoutComponentConnectionType.Passage);

            if (connectTo != null) {
                foreach (LayoutComponentConnectionPoint cpDest in connectTo) {
                    if (IsSplitPoint(cpDest))
                        return true;
                }
            }

            return false;
        }

        public bool ReverseLogic {
            get => (bool?)Element.AttributeValue(A_ReverseLogic) ?? false;
            set => Element.SetAttributeValue(A_ReverseLogic, value);
        }

        public override string? RequiredControlModuleTypeName => (string?)Element.AttributeValue(A_BuiltinDecoderTypeName) ?? null;
    }

    /// <summary>
    /// A component that contains two tracks. One connecting between cp1 and cp2
    /// and the other connects between cp3 and cp4. For example cross, or two diagonal
    /// tracks.
    /// </summary>
    public class LayoutTurnoutTrackComponent : LayoutMultiPathTrackComponent, IModelComponentIsDualState, IModelComponentHasReverseLogic, IModelComponentConnectToControl {
        private const string A_Branch = "branch";
        private const string A_HasFeedback = "HasFeedback";
        private const string A_Straight = "straight";
        private const string A_Tip = "tip";
        private const string E_Connections = "Connections";
        private LayoutComponentConnectionPoint tip;
        private LayoutComponentConnectionPoint straight, branch;

        private static readonly IList<ModelComponentControlConnectionDescription> controlConnectionsNoFeedback = Array.AsReadOnly<ModelComponentControlConnectionDescription>(
            new ModelComponentControlConnectionDescription[] { new ModelComponentControlConnectionDescription("Solenoid", "Turnout", "Turnout Control") });

        private static readonly IList<ModelComponentControlConnectionDescription> controlConnectionsWithFeedback = Array.AsReadOnly<ModelComponentControlConnectionDescription>(
            new ModelComponentControlConnectionDescription[] {
                new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.OutputSolenoid, "Turnout", "turnout control"),
                new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.Input, "TurnoutStraight", "turnout straight feedback"),
                new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.Input, "TurnoutDivert", "turnout divert feedback"),
            });

        public LayoutTurnoutTrackComponent(LayoutComponentConnectionPoint tip,
            LayoutComponentConnectionPoint straight, LayoutComponentConnectionPoint branch) {
            XmlDocument.LoadXml("<Turnout />");

            this.tip = tip;
            this.straight = straight;
            this.branch = branch;
        }

        public LayoutTurnoutTrackComponent() {
            XmlDocument.LoadXml("<Turnout />");
        }

        public LayoutComponentConnectionPoint Tip => tip;

        public LayoutComponentConnectionPoint Straight => straight;

        public LayoutComponentConnectionPoint Branch => branch;

        public override IList<LayoutComponentConnectionPoint> ConnectionPoints => Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] { tip, straight, branch });

        public override LayoutComponentConnectionPoint[] ConnectTo(LayoutComponentConnectionPoint from, LayoutComponentConnectionType type) {
            if (type == LayoutComponentConnectionType.Passage || type == LayoutComponentConnectionType.ReverseLoop) {
                if (from == tip)
                    return new LayoutComponentConnectionPoint[] { straight, branch };
                else return from == straight || from == branch ? (new LayoutComponentConnectionPoint[] { tip }) : (Array.Empty<LayoutComponentConnectionPoint>());
            }
            else {
                if (from == tip)
                    return new LayoutComponentConnectionPoint[] { straight, branch };
                else if (from == straight)
                    return new LayoutComponentConnectionPoint[] { tip, branch };
                else return from == branch ? (new LayoutComponentConnectionPoint[] { straight, tip }) : (Array.Empty<LayoutComponentConnectionPoint>());
            }
        }

        public override LayoutComponentConnectionPoint ConnectTo(LayoutComponentConnectionPoint from, int switchState) {
            if (switchState == 0) {
                if (from == straight)
                    return tip;
                if (from == tip)
                    return straight;
            }
            else if (switchState == 1) {
                if (from == branch)
                    return tip;
                if (from == tip)
                    return branch;
            }

            return LayoutComponentConnectionPoint.Empty;
        }

        public override string ToString() => "turnout";

        public bool HasFeedback => (bool?)Element.AttributeValue(A_HasFeedback) ?? false;

        public override void WriteXmlFields(XmlWriter w) {
            w.WriteStartElement(E_Connections);
            w.WriteAttributeString(A_Tip, tip.ToString());
            w.WriteAttributeString(A_Straight, straight.ToString());
            w.WriteAttributeString(A_Branch, branch.ToString());
            w.WriteEndElement();

            // TODO: Write block association (may not be needed if layout is compiled after loading)
            // TODO: Store default switch setting
        }

        protected override bool ReadXmlField(XmlReader r) {
            if (r.Name == E_Connections) {
                tip = LayoutComponentConnectionPoint.Parse(r.GetAttribute(A_Tip));
                straight = LayoutComponentConnectionPoint.Parse(r.GetAttribute(A_Straight));
                branch = LayoutComponentConnectionPoint.Parse(r.GetAttribute(A_Branch));
                r.Read();
                return true;
            }

            return false;
        }

        #region IModelComponentConnectToControl Members

        public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions {
            get {
                if (RequiredControlModuleTypeName != null) {
                    List<ModelComponentControlConnectionDescription> connectionDescriptions = new List<ModelComponentControlConnectionDescription>(
                        HasFeedback ? controlConnectionsWithFeedback : controlConnectionsNoFeedback);

                    connectionDescriptions[0] = new ModelComponentControlConnectionDescription(connectionDescriptions[0].Name, connectionDescriptions[0].DisplayName, RequiredControlModuleTypeName);

                    return connectionDescriptions.AsReadOnly();
                }
                else
                    return HasFeedback ? controlConnectionsWithFeedback : controlConnectionsNoFeedback;
            }
        }

        #endregion
    }

    /// <summary>
    /// Three way turnout with two control points one for left/straight the other is straight/right.
    /// </summary>
    public class LayoutThreeWayTurnoutComponent : LayoutMultiPathTrackComponent, IModelComponentIsMultiPath, IModelComponentConnectToControl {
        private LayoutComponentConnectionPoint tip;

        private static readonly IList<LayoutComponentConnectionPoint> _connectionPoints = Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] {
                    LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

        public LayoutThreeWayTurnoutComponent(LayoutComponentConnectionPoint tip) {
            XmlDocument.LoadXml("<ThreeWayTurnout />");

            this.tip = tip;
        }

        public LayoutThreeWayTurnoutComponent() {
            XmlDocument.LoadXml("<ThreeWayTurnout />");
        }

        public LayoutComponentConnectionPoint Tip => tip;

        public override IList<LayoutComponentConnectionPoint> ConnectionPoints => _connectionPoints;

        public override LayoutComponentConnectionPoint[] ConnectTo(LayoutComponentConnectionPoint from, LayoutComponentConnectionType type) {
            // If connection type is not passage or it is passage and from == tip, then connection can be made to all
            // other three connection points
            if ((type != LayoutComponentConnectionType.Passage && type != LayoutComponentConnectionType.ReverseLoop) || from == tip) {
                // Each connection points connect to all the other connection points
                LayoutComponentConnectionPoint[] result = new LayoutComponentConnectionPoint[3];

                int i = 0;
                foreach (LayoutComponentConnectionPoint cp in ConnectionPoints)
                    if (cp != from)
                        result[i++] = cp;
                return result;
            }
            else // Connection can be made to the tip
                return new LayoutComponentConnectionPoint[] { tip };
        }

        protected override SwitchingStateSupport GetSwitchingStateSupporter() => new ThreeWaySwitchingStateSupport(this);

        protected class ThreeWaySwitchingStateSupport : SwitchingStateSupport {
            private const string A_Value = "Value";
            private int state0;
            private int state1;

            public ThreeWaySwitchingStateSupport(IModelComponentHasSwitchingState component)
                : base(component, switchStateCount: 3) {
            }

            public override void AddSwitchingCommands(IList<SwitchingCommand> switchingCommands, int switchingState, string connectionPointName) {
                if (switchingState < 0 || switchingState > 2)
                    throw new LayoutException(this, "Invalid switch state: " + switchingState);

                Trace.WriteLine("CurrentSwitchState <- " + switchingState);
                var connectionPointRight = LayoutModel.ControlManager.ConnectionPoints[Component.Id, "ControlRight"];
                var connectionPointLeft = LayoutModel.ControlManager.ConnectionPoints[Component.Id, "ControlLeft"];
                int rightState = 0;
                int leftState = 0;

                if (switchingState == 1)      // Turn right
                    rightState = 1;
                else if (switchingState == 2) // Turn left
                    leftState = 1;

                if (connectionPointRight != null)
                    switchingCommands.Add(new SwitchingCommand(new ControlConnectionPointReference(connectionPointRight), rightState));
                if (connectionPointLeft != null)
                    switchingCommands.Add(new SwitchingCommand(new ControlConnectionPointReference(connectionPointLeft), leftState));
            }

            public override void SetSwitchState(ControlConnectionPoint controlConnectionPoint, int switchState, string connectionPointName) {
                Trace.WriteLine($"ThreeWayTurnout::SetSwitchState for {controlConnectionPoint.Name} to {switchState}");

                if (controlConnectionPoint.Name == "Right")
                    state0 = switchState;
                else
                    state1 = switchState;

                int realSwitchState = -1;

                if (state0 == 0 && state1 == 0)
                    realSwitchState = 0;
                else if (state0 == 1 && state1 == 0)
                    realSwitchState = 1;
                else if (state0 == 0 && state1 == 1)
                    realSwitchState = 2;

                if (realSwitchState >= 0) {
                    LayoutModel.StateManager.Components.StateOf(Component.Id, StateTopic).SetAttributeValue(A_Value, realSwitchState);
                    Component.OnComponentChanged();
                }
            }
        }

        public override LayoutComponentConnectionPoint ConnectTo(LayoutComponentConnectionPoint from, int switchState) {
            switch (switchState) {
                case 0: // Straight
                    if (from == tip)
                        return LayoutTrackComponent.OppositeConnectPoint(tip);
                    else if (from == LayoutTrackComponent.OppositeConnectPoint(tip))
                        return tip;
                    break;

                case 1: // Right
                    if (from == tip) {
                        switch (tip) {
                            case LayoutComponentConnectionPoint.T: return LayoutComponentConnectionPoint.L;
                            case LayoutComponentConnectionPoint.R: return LayoutComponentConnectionPoint.T;
                            case LayoutComponentConnectionPoint.B: return LayoutComponentConnectionPoint.R;
                            case LayoutComponentConnectionPoint.L: return LayoutComponentConnectionPoint.B;
                        }
                    }
                    else if (from == ConnectTo(tip, switchState))
                        return tip;
                    break;

                case 2: // Left
                    if (from == tip) {
                        switch (tip) {
                            case LayoutComponentConnectionPoint.T: return LayoutComponentConnectionPoint.R;
                            case LayoutComponentConnectionPoint.R: return LayoutComponentConnectionPoint.B;
                            case LayoutComponentConnectionPoint.B: return LayoutComponentConnectionPoint.L;
                            case LayoutComponentConnectionPoint.L: return LayoutComponentConnectionPoint.T;
                        }
                    }
                    else if (from == ConnectTo(tip, switchState))
                        return tip;
                    break;
            }

            return LayoutComponentConnectionPoint.Empty;
        }

        public override int CurrentSwitchState => base.CurrentSwitchState;

        public override string ToString() => "Three way turnout";

        public override void WriteXmlFields(XmlWriter w) {
            w.WriteStartElement("Connections");
            w.WriteAttributeString("tip", tip.ToString());
            w.WriteEndElement();
        }

        protected override bool ReadXmlField(XmlReader r) {
            if (r.Name == "Connections") {
                tip = LayoutComponentConnectionPoint.Parse(r.GetAttribute("tip"));
                r.Read();
                return true;
            }

            return false;
        }

        #region IModelComponentConnectToControl Members

        public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions => Array.AsReadOnly<ModelComponentControlConnectionDescription>(new ModelComponentControlConnectionDescription[] {
                    new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.OutputSolenoid, "Right", "Control Straight/right"),
                    new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.OutputSolenoid, "Left", "Control Straight/left"),
                });

        #endregion

    }

    /// <summary>
    /// A component that is either cross, or connect to diagonal connection point
    /// </summary>
    public class LayoutDoubleSlipTrackComponent : LayoutMultiPathTrackComponent, IModelComponentIsDualState, IModelComponentHasReverseLogic, IModelComponentConnectToControl {
        private const string E_Connections = "Connections";
        private const string A_DiagonalIndex = "DiagonalIndex";
        private int diagonalIndex;

        private static readonly IList<ModelComponentControlConnectionDescription> controlConnections = Array.AsReadOnly<ModelComponentControlConnectionDescription>(
            new ModelComponentControlConnectionDescription[] { new ModelComponentControlConnectionDescription(ControlConnectionPointTypes.OutputSolenoid, "Doubleslip", "double-slip control") });

        public LayoutDoubleSlipTrackComponent(int diagonalIndex) {
            XmlDocument.LoadXml("<DoubleSlip />");
            this.diagonalIndex = diagonalIndex;
        }

        public LayoutDoubleSlipTrackComponent() {
            XmlDocument.LoadXml("<DoubleSlip />");
        }

        public int DiagonalIndex => this.diagonalIndex;

        private static readonly IList<LayoutComponentConnectionPoint> AllConnectionPoints = Array.AsReadOnly<LayoutComponentConnectionPoint>(new LayoutComponentConnectionPoint[] {
            LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.T,
            LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.B });

        public override IList<LayoutComponentConnectionPoint> ConnectionPoints => AllConnectionPoints;

        public override LayoutComponentConnectionPoint[] ConnectTo(LayoutComponentConnectionPoint from, LayoutComponentConnectionType type) {
            if (type == LayoutComponentConnectionType.Passage || type == LayoutComponentConnectionType.ReverseLoop) {
                return (int)from switch
                {
                    LayoutComponentConnectionPoint.L => new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.R,
                            DiagonalConnectionPoints(LayoutComponentConnectionPoint.L)[diagonalIndex] },

                    LayoutComponentConnectionPoint.R => new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L,
                            DiagonalConnectionPoints(LayoutComponentConnectionPoint.L)[1-diagonalIndex] },

                    LayoutComponentConnectionPoint.T => new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.B,
                            DiagonalConnectionPoints(LayoutComponentConnectionPoint.T)[diagonalIndex] },

                    LayoutComponentConnectionPoint.B => new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T,
                            DiagonalConnectionPoints(LayoutComponentConnectionPoint.T)[1-diagonalIndex] },

                    _ => throw new LayoutException($"Invalid source connection point {from}")
                };
            }
            else {
                LayoutComponentConnectionPoint[] result = new LayoutComponentConnectionPoint[3];

                int index = 0;
                foreach (LayoutComponentConnectionPoint cp in ConnectionPoints)
                    if (cp != from)
                        result[index++] = cp;

                return result;
            }
        }

        public override LayoutComponentConnectionPoint ConnectTo(LayoutComponentConnectionPoint from, int switchState) {
            LayoutComponentConnectionPoint[] connectTo = ConnectTo(from, LayoutComponentConnectionType.Passage);

            if (connectTo != null && switchState < connectTo.Length)
                return connectTo[switchState];
            throw new ArgumentException("No connection from " + from.ToString() + " for switch state " + switchState);
        }

        public override string ToString() => "double slip";

        public override void WriteXmlFields(XmlWriter w) {
            w.WriteStartElement(E_Connections);
            w.WriteAttributeString(A_DiagonalIndex, diagonalIndex);
            w.WriteEndElement();
        }

        protected override bool ReadXmlField(XmlReader r) {
            ConvertableString GetAttribute(string name) => new ConvertableString(r.GetAttribute(name), $"Attribute {name}");

            if (r.Name == E_Connections) {
                diagonalIndex = (int)GetAttribute(A_DiagonalIndex);
                r.Read();
                return true;
            }

            return false;
        }

        #region IModelComponentConnectToControl Members
        public IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions => controlConnections;

        #endregion

    }
}
