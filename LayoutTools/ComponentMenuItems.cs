using System.ComponentModel;
using System.Drawing;
using System.Xml;
using MethodDispatcher;
using LayoutManager.Components;
using LayoutManager.Model;

namespace LayoutManager.Tools {
    /// <summary>
    /// Summary description for ComponentMenuItems.
    /// </summary>
    [LayoutModule("Component Menu Items", UserControl = false)]
    public partial class ComponentMenuItems : System.ComponentModel.Component, ILayoutModuleSetup {
        private const string A_Image = "Image";
        private const string A_TrackCp1 = "TrackCp1";
        private const string A_DiagonalIndex = "DiagonalIndex";
        private const string A_TrackCp2 = "TrackCp2";
        private const string A_NewCp1 = "NewCp1";
        private const string A_NewCp2 = "NewCp2";

        #region Constructors

        public ComponentMenuItems(IContainer container) {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            container.Add(this);
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public ComponentMenuItems() {
            /// <summary>
            /// Required for Windows.Forms Class Composition Designer support
            /// </summary>
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        #endregion

        /// <summary>
        /// Add a child to a node, the child is provided as XML text
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xmlText"></param>
        private void AddChild(XmlElement element, string xmlText) {
            XmlDocument doc = new();

            doc.LoadXml(xmlText);
            element.AppendChild(element.OwnerDocument.ImportNode(doc.DocumentElement!, true));
        }

        private static void DrawFrame(Graphics g) {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);
        }

        #region Add categories

        // Add categories, please note that rather than adding all the categories in a single
        // event handler (which could have been done), each category is added in its own event
        // handler. This will allow to add categories in evey position in the menu simply by
        // setting the 'Order' parameter of the event handler.

        [LayoutEvent("get-component-menu-categories", Order = 0)]
        private void AddTrackCategory(LayoutEvent e) {
            var categories = Ensure.NotNull<XmlElement>(e.Sender);

            AddChild(categories, "<Category Name='Tracks' Tooltip='Tracks' Image='0' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 100)]
        private void AddComposedComponentCategory(LayoutEvent e) {
            var categories = Ensure.NotNull<XmlElement>(e.Sender);

            AddChild(categories, "<Category Name='ComposedTracks' Tooltip='Turnouts, cross, etc.' Image='4' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 200)]
        private void AddBlockCategory(LayoutEvent e) {
            var categories = Ensure.NotNull<XmlElement>(e.Sender);

            AddChild(categories, "<Category Name='Block' Tooltip='Tracks contacts / Blocks' Image='1' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 300)]
        private void AddAnnotationCategory(LayoutEvent e) {
            var categories = Ensure.NotNull<XmlElement>(e.Sender);

            AddChild(categories, "<Category Name='Annotation' Tooltip='Text &amp; Images' Image='2' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 400)]
        private void AddControlCategory(LayoutEvent e) {
            var categories = Ensure.NotNull<XmlElement>(e.Sender);

            AddChild(categories, "<Category Name='Control' Tooltip='Layout power control elements' Image='3' />");
        }

        /*--------------------------------------------------------------------------------------*/

        /// <summary>
        /// Paint the categories. The Image attribute is the category's image in the image list
        /// </summary>
        [LayoutEvent("paint-image-menu-category", IfSender = "Category[@Image]")]
        private void PaintAnnotationCategory(LayoutEvent e) {
            var categoryElement = Ensure.NotNull<XmlElement>(e.Sender);
            var g = Ensure.NotNull<Graphics>(e.Info);
            var imageIndex = (int)categoryElement.AttributeValue(A_Image);

            g.DrawImage(imageListCategories.Images[imageIndex], 0, 0);
        }

        #endregion

        #region Track category components

        private string GetTooltip(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            if (LayoutTrackComponent.IsDiagonal(cp1, cp2))
                return "Diagonal track";
            else {
                return LayoutTrackComponent.IsHorizontal(cp1) ? "Horizontal track" : "Vertical track";
            }
        }

        private void AddTrackItem(XmlElement categoryElement, LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            AddChild(categoryElement, "<Item Name='track-component' Tooltip='" +
                GetTooltip(cp1, cp2) + "' cp1='" + cp1 + "' cp2='" + cp2 + "' />");
        }

        [DispatchTarget]
        private void GetComponentMenuCategoryItems_Tracks(ModelComponent? track, XmlElement categoryElement, [DispatchFilter] string categoryName = "Tracks") {
            AddTrackItem(categoryElement, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R);
            AddTrackItem(categoryElement, LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B);
            AddTrackItem(categoryElement, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.T);
            AddTrackItem(categoryElement, LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.T);
            AddTrackItem(categoryElement, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.B);
            AddTrackItem(categoryElement, LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.B);
        }

        [DispatchTarget]
        private void PaintImageMenuItem_TrackComponent(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "track-component") {
            LayoutComponentConnectionPoint cp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp1"));
            LayoutComponentConnectionPoint cp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp2"));

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            LayoutStraightTrackPainter painter = new(new Size(32, 32), cp1, cp2);
            g.TranslateTransform(4, 4);
            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TrackComponent(XmlElement itemElement, [DispatchFilter] string name = "track-component") {
            LayoutComponentConnectionPoint cp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp1"));
            LayoutComponentConnectionPoint cp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp2"));

            return new LayoutStraightTrackComponent(cp1, cp2);
        }

        #endregion

        #region Composed Track Components Category Items (turnout etc.)

        private void AddTurnoutItem(XmlElement categoryElement, LayoutStraightTrackComponent existingTrack, LayoutComponentConnectionPoint tipCp, LayoutComponentConnectionPoint newCp) {
            string tooltip = "turnout";

            AddChild(categoryElement, "<Item Name='turnout-component' Tooltip='" + tooltip +
                "' TrackCp1='" + existingTrack.ConnectionPoints[0].ToString() +
                "' TrackCp2='" + existingTrack.ConnectionPoints[1].ToString() +
                "' TipCp='" + tipCp.ToString() +
                "' NewCp='" + newCp.ToString() + "' />");
        }

        private void AddThreeWayTurnoutItem(XmlElement categoryElement, LayoutStraightTrackComponent existingTrack, LayoutComponentConnectionPoint tipCp) {
            string tooltip = "three way turnout";

            AddChild(categoryElement, "<Item Name='three-way-turnout-component' Tooltip='" + tooltip +
                "' TrackCp1='" + existingTrack.ConnectionPoints[0].ToString() +
                "' TrackCp2='" + existingTrack.ConnectionPoints[1].ToString() +
                "' TipCp='" + tipCp.ToString() + "' />");
        }

        private void AddDoubleSlipItem(XmlElement categoryElement, LayoutStraightTrackComponent existingTrack, int diagonalIndex) {
            string tooltip = "double-slip turnout";

            AddChild(categoryElement,
                $"<Item Name='double-slip-component' Tooltip='{tooltip}' TrackCp1='{existingTrack.ConnectionPoints[0]}' TrackCp2='{existingTrack.ConnectionPoints[1]}' DiagonalIndex='{diagonalIndex}' />");
        }

        private void AddDoubleTrackComponent(XmlElement categoryElement, string tooltip, LayoutStraightTrackComponent existingTrack, LayoutComponentConnectionPoint newCp1, LayoutComponentConnectionPoint newCp2) {
            AddChild(categoryElement, $"<Item Name='double-track-component' Tooltip='{tooltip}' TrackCp1='{existingTrack.ConnectionPoints[0]}' TrackCp2='{existingTrack.ConnectionPoints[1]}' NewCp1='{newCp1}' NewCp2='{newCp2}' />");
        }

        private LayoutComponentConnectionPoint ParallelCp(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            LayoutComponentConnectionPoint[] diagCp = LayoutTrackComponent.DiagonalConnectionPoints(cp1);

            return diagCp[0] == cp2 ? diagCp[1] : diagCp[0];
        }

        [DispatchTarget]
        private void GetComponentMenuCategoryItems_ComposeTracks(ModelComponent? existingTrack, XmlElement categoryElement, [DispatchFilter] string categoryName = "ComposedTracks") {
            if (existingTrack is LayoutStraightTrackComponent track && track.TrackAnnotation == null) {
                if (track.IsDiagonal()) {
                    AddTurnoutItem(categoryElement, track, track.ConnectionPoints[0], LayoutTrackComponent.OppositeConnectPoint(track.ConnectionPoints[0]));
                    AddTurnoutItem(categoryElement, track, track.ConnectionPoints[1], LayoutTrackComponent.OppositeConnectPoint(track.ConnectionPoints[1]));

                    AddDoubleTrackComponent(categoryElement, "Parallel diagonal tracks", track, ParallelCp(track.ConnectionPoints[0], track.ConnectionPoints[1]), ParallelCp(track.ConnectionPoints[1], track.ConnectionPoints[0]));
                }
                else {
                    LayoutComponentConnectionPoint[] diagCp = LayoutTrackComponent.DiagonalConnectionPoints(track.ConnectionPoints[0]);

                    AddTurnoutItem(categoryElement, track, track.ConnectionPoints[0], diagCp[0]);
                    AddTurnoutItem(categoryElement, track, track.ConnectionPoints[0], diagCp[1]);

                    diagCp = LayoutTrackComponent.DiagonalConnectionPoints(track.ConnectionPoints[1]);

                    AddTurnoutItem(categoryElement, track, track.ConnectionPoints[1], diagCp[0]);
                    AddTurnoutItem(categoryElement, track, track.ConnectionPoints[1], diagCp[1]);

                    AddThreeWayTurnoutItem(categoryElement, track, track.ConnectionPoints[0]);
                    AddThreeWayTurnoutItem(categoryElement, track, track.ConnectionPoints[1]);

                    AddDoubleTrackComponent(categoryElement, "Crossed tracks", track, diagCp[0], diagCp[1]);

                    AddDoubleSlipItem(categoryElement, track, 0);
                    AddDoubleSlipItem(categoryElement, track, 1);

                    if (CanComposeTrackLink(track))
                        AddChild(categoryElement, "<Item Name='track-link' Tooltip='Track link (link to another part of the model)' />");
                }
            }
        }

        [DispatchTarget]
        private void PaintImageMenuItem_TurnoutComponent(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "turnout-component") {
            LayoutComponentConnectionPoint trackCp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp1));
            LayoutComponentConnectionPoint trackCp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp2));
            LayoutComponentConnectionPoint tipCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("TipCp"));
            LayoutComponentConnectionPoint newCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("NewCp"));

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter existingTrackPainter = new(new Size(32, 32), trackCp1, trackCp2);
            existingTrackPainter.Paint(g);

            LayoutStraightTrackPainter newTrackPainter = new(new Size(32, 32), tipCp, newCp) {
                TrackColor = Color.LightGreen,
                TrackColor2 = Color.LightGreen
            };
            newTrackPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TurnoutComponent(XmlElement itemElement, [DispatchFilter] string name = "turnout-component") {
            LayoutComponentConnectionPoint trackCp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp1));
            LayoutComponentConnectionPoint trackCp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp2));
            LayoutComponentConnectionPoint tipCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("TipCp"));
            LayoutComponentConnectionPoint newCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("NewCp"));
            LayoutComponentConnectionPoint straightCp;
            LayoutComponentConnectionPoint branchCp;

            if (LayoutTrackComponent.IsDiagonal(tipCp, newCp)) {
                straightCp = LayoutTrackComponent.OppositeConnectPoint(tipCp);
                branchCp = newCp;
            }
            else {
                straightCp = newCp;
                branchCp = (tipCp == trackCp1) ? trackCp2 : trackCp1;
            }

            return new LayoutTurnoutTrackComponent(tipCp, straightCp, branchCp);
        }

        //----

        [DispatchTarget]
        private void PaintImageMenuItem_ThreeWayTurnoutComponent(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "three-way-turnout-component") {
            LayoutComponentConnectionPoint trackCp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp1));
            LayoutComponentConnectionPoint trackCp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp2));
            LayoutComponentConnectionPoint tipCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("TipCp"));

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter existingTrackPainter = new(new Size(32, 32), trackCp1, trackCp2);
            existingTrackPainter.Paint(g);

            LayoutComponentConnectionPoint[] cps;
            if (LayoutTrackComponent.IsHorizontal(tipCp))
                cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
            else
                cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.L };

            foreach (LayoutComponentConnectionPoint cp in cps) {
                LayoutStraightTrackPainter painter = new(new Size(32, 32), tipCp, cp) {
                    TrackColor = Color.LightGreen,
                    TrackColor2 = Color.LightGreen
                };
                painter.Paint(g);
            }
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_ThreeWayTurnoutComponent(XmlElement itemElement, [DispatchFilter] string name = "three-way-turnout-component") {
            LayoutComponentConnectionPoint tipCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("TipCp"));

            return new LayoutThreeWayTurnoutComponent(tipCp);
        }

        // ----

        [DispatchTarget]
        private void PaintImageMenuItem_DoubleSlipComponent(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "double-slip-component") {
            var trackCp1 = itemElement.AttributeValue(A_TrackCp1).ToComponentConnectionPoint();
            var diagonalIndex = (int)itemElement.AttributeValue(A_DiagonalIndex);

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutDoubleSlipPainter painter = new(new Size(32, 32), diagonalIndex, -1) {
                HorizontalTrackColor = LayoutTrackComponent.IsHorizontal(trackCp1) ? Color.Black : Color.LightGreen,
                VerticalTrackColor = LayoutTrackComponent.IsVertical(trackCp1) ? Color.Black : Color.LightGreen,
                LeftBranchColor = Color.LightGreen,
                RightBranchColor = Color.LightGreen
            };

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_DoubleSlipComponent(XmlElement itemElement, [DispatchFilter] string name = "double-slip-component") {
            var diagonalIndex = (int)itemElement.AttributeValue(A_DiagonalIndex);

            return new LayoutDoubleSlipTrackComponent(diagonalIndex);
        }

        //----

        [DispatchTarget]
        private void PaintImageMenuItem_DoubleTrackComponent(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "double-track-component") {
            var trackCp1 = itemElement.AttributeValue(A_TrackCp1).ToComponentConnectionPoint();
            var trackCp2 = itemElement.AttributeValue(A_TrackCp2).ToComponentConnectionPoint();
            var newCp1 = itemElement.AttributeValue(A_NewCp1).ToComponentConnectionPoint();
            var newCp2 = itemElement.AttributeValue(A_NewCp2).ToComponentConnectionPoint();

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter existingTrackPainter = new(new Size(32, 32), trackCp1, trackCp2);
            existingTrackPainter.Paint(g);

            LayoutStraightTrackPainter newTrackPainter = new(new Size(32, 32), newCp1, newCp2) {
                TrackColor = Color.LightGreen,
                TrackColor2 = Color.LightGreen
            };
            newTrackPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_DoubleTrackComponent(XmlElement itemElement, [DispatchFilter] string name = "double-track-component") {
            var trackCp1 = itemElement.AttributeValue(A_TrackCp1).ToComponentConnectionPoint();
            var trackCp2 = itemElement.AttributeValue(A_TrackCp2).ToComponentConnectionPoint();

            return new LayoutDoubleTrackComponent(trackCp1, trackCp2);
        }

        //----

        private bool CanComposeTrackLink(LayoutStraightTrackComponent existingTrack) => !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) &&
                existingTrack.Spot[ModelComponentKind.TrackLink] == null;

        [DispatchTarget]
        private void PaintImageMenuItem_TrackLink(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "track-link") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R) {
                TrackColor = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutTrackLinkPainter linkPainter = new(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R });
            linkPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TrackLink(XmlElement itemElement, [DispatchFilter] string name = "track-link") => new LayoutTrackLinkComponent();

        #endregion

        #region Block category items

        [DispatchTarget]
        private void GetComponentMenuCategoryItems_Block(ModelComponent? track, XmlElement categoryElement, [DispatchFilter] string categoryName = "Block") {
            if (track is LayoutTrackComponent existingTrack) {
                if (CanComposeBlockEdge(existingTrack))
                    AddChild(categoryElement, "<Item Name='track-contact' Tooltip='Track contact (block edge)' />");

                if (CanComposeBlockEdge(existingTrack))
                    AddChild(categoryElement, "<Item Name='proximity-sensor' Tooltip='Proximity sensor (block edge)' />");

                if (CanComposeBlockEdge(existingTrack))
                    AddChild(categoryElement, "<Item Name='block-edge' Tooltip='Block Edge' />");

                if (CanComposeBlockInfo(existingTrack))
                    AddChild(categoryElement, "<Item Name='block-info' Tooltip='Block Information' />");

                if (CanComposeSignal(existingTrack))
                    AddChild(categoryElement, "<Item Name='signal' Tooltip='Track Signal' />");

                if (CanComposeGate(existingTrack))
                    AddChild(categoryElement, "<Item Name='gate' Tooltip='Gate' />");
            }
        }

        //----

        private bool CanComposeBlockEdge(LayoutTrackComponent? existingTrack) {
            if (existingTrack != null) {
                if (existingTrack is LayoutStraightTrackComponent track) {
                    track.SetTrackAnnotation();

                    if (track.TrackAnnotation == null)
                        return true;
                }
            }

            return false;
        }

        //----

        [DispatchTarget]
        private void PaintImageMenuItem_TrackContact(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "track-contact") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutTriggerableBlockEdgePainter contactPainter = new(componentType: LayoutTriggerableBlockEdgePainter.ComponentType.TrackContact,
                componentSize: new Size(32, 32), cp: new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                ContactSize = new Size(6, 6)
            };
            contactPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TrackContact(XmlElement itemElement, [DispatchFilter] string name = "track-contact") => new LayoutTrackContactComponent();

        //----
        [DispatchTarget]
        private void PaintImageMenuItem_ProximitySensor(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "proximity-sensor") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutTriggerableBlockEdgePainter sensorPainter = new(componentType: LayoutTriggerableBlockEdgePainter.ComponentType.ProximitySensor,
                componentSize: new Size(32, 32), cp: new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                ContactSize = new Size(6, 6)
            };
            sensorPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_ProximitySensor(XmlElement itemElement, [DispatchFilter] string name = "proximity-sensor") => new LayoutProximitySensorComponent();

        //----

        [DispatchTarget]
        private void PaintImageMenuItem_BlockEdge(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "block-edge") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutBlockEdgePainter painter = new(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                ContactSize = new Size(6, 6)
            };
            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_BlockEdge(XmlElement itemElement, [DispatchFilter] string name = "block-edge") => new LayoutBlockEdgeComponent();

        //----

        private bool CanComposeBlockInfo(LayoutTrackComponent? existingTrack) {
            if (existingTrack != null) {
                if (existingTrack is LayoutStraightTrackComponent track && !track.IsDiagonal()) {
                    track.SetTrackAnnotation();

                    if (track.TrackAnnotation == null)
                        return true;
                }
                else if (existingTrack is LayoutDoubleSlipTrackComponent && existingTrack.TrackAnnotation == null)
                    return true;
            }

            return false;
        }

        [DispatchTarget]
        private void PaintImageMenuItem_BlockInfo(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "block-info") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutBlockInfoPainter blockInfoPainter = new(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                InfoBoxSize = new Size(6, 6)
            };
            blockInfoPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_BlockInfo(XmlElement itemElement, [DispatchFilter] string name = "block-info") => new LayoutBlockDefinitionComponent();

        //----

        private bool CanComposeSignal(LayoutTrackComponent? existingTrack) {
            if (existingTrack != null) {
                if (existingTrack is LayoutStraightTrackComponent track && !track.IsDiagonal()) {
                    if (track.Spot[ModelComponentKind.Signal] == null)
                        return true;
                }
            }

            return false;
        }

        [DispatchTarget]
        private void PaintImageMenuItem_Signal(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "signal") {
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);
            g.DrawImage(imageListComponentsLarge.Images[0], 1, 1);

            g.DrawRectangle(Pens.Black, 0, 0, 32, 32);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_Signal(XmlElement itemElement, [DispatchFilter] string name = "signal") => new LayoutSignalComponent();

        //----

        private bool CanComposeGate(LayoutTrackComponent? old) {
            if (old != null && old is LayoutStraightTrackComponent && !LayoutStraightTrackComponent.IsDiagonal(old)) {
                if (old.Spot[ModelComponentKind.Gate] == null)
                    return true;
            }

            return false;
        }

        [DispatchTarget]
        private void PaintImageMenuItem_Gate(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "gate") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutComponentConnectionPoint[] cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R };
            Size componentSize = new(32, 32);

            LayoutStraightTrackPainter trackPainter = new(componentSize, cps[0], cps[1]);
            trackPainter.Paint(g);

            LayoutGatePainter gatePainter = new(componentSize, false, false, 60);

            gatePainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_Gate(XmlElement itemElement, [DispatchFilter] string name = "gate") => new LayoutGateComponent();

        #endregion

        #region Annotation section components

        [DispatchTarget]
        private void GetComponentMenuCategoryItems_Annotation(ModelComponent? existingTrack, XmlElement categoryElement, [DispatchFilter] string categoryName = "Annotation") {
            AddChild(categoryElement, "<Item Name='text' Tooltip='Text label' />");
            AddChild(categoryElement, "<Item Name='image' Tooltip='Image (picture)' />");

            if (existingTrack is LayoutTrackComponent track) {
                if (CanComposeBridge(track))
                    AddChild(categoryElement, "<Item Name='bridge' Tooltip='Bridge' />");

                if (CanComposeTunnel(track))
                    AddChild(categoryElement, "<Item Name='tunnel' Tooltip='Tunnel' />");
            }
        }

        //----

        [DispatchTarget]
        private void PaintImageMenuItem_Text(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "text") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            Font f = new("Arial", 16);
            g.DrawString("Aa", f, Brushes.BlueViolet, new Point(1, 4));
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_Text(XmlElement itemElement, [DispatchFilter] string name = "text") => new LayoutTextComponent();

        //----

        [DispatchTarget]
        private void PaintImageMenuItem_Image(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "image") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(8, 8);
            g.DrawImage(imageListComponents.Images[0], 1, 1);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_Image(XmlElement itemElement, [DispatchFilter] string name = "image") => new LayoutImageComponent();

        //----

        private bool CanComposeBridge(LayoutTrackComponent? old) => old != null && old is LayoutStraightTrackComponent;

        [DispatchTarget]
        private void PaintImageMenuItem_Bridge(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "bridge") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutComponentConnectionPoint[] cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
            Size componentSize = new(32, 32);

            LayoutStraightTrackPainter trackPainter = new(componentSize, cps[0], cps[1]);
            trackPainter.Paint(g);

            LayoutBridgePainter bridgePainter = new(componentSize, cps) {
                Offset = 6
            };

            bridgePainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_Bridge(XmlElement itemElement, [DispatchFilter] string name = "bridge") => new LayoutBridgeComponent();

        //----

        private bool CanComposeTunnel(LayoutTrackComponent? old) => old != null && old is LayoutStraightTrackComponent;

        [DispatchTarget]
        private void PaintImageMenuItem_Tunnel(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "tunnel") {
            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutComponentConnectionPoint[] cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
            Size componentSize = new(32, 32);

            LayoutStraightTrackPainter trackPainter = new(componentSize, cps[0], cps[1]);
            trackPainter.Paint(g);

            LayoutTunnelPainter tunnelPainter = new(componentSize, cps) {
                Offset = 6
            };

            tunnelPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_Tunnle(XmlElement itemElement, [DispatchFilter] string name = "tunnel") => new LayoutTunnelComponent();

        #endregion

        #region Power Control section components

        [DispatchTarget]
        private void GetComponentMenuCategoryItems_Control(ModelComponent? existingTrack, XmlElement categoryElement, [DispatchFilter] string categoryName = "Control") {

            if (existingTrack is LayoutTrackComponent track) {
                if (CanComposeTrackPower(track))
                    AddChild(categoryElement, "<Item Name='track-power' Tooltip='Track power connector' />");

                if (CanComposeTrackIsolation(track))
                    AddChild(categoryElement, "<Item Name='track-isolation' Tooltip='Track power isolation' />");

                if (CanComposeTrackReverseLoopModule(track))
                    AddChild(categoryElement, "<Item Name='track-reverse-loop-module' Tooltip='Track reverse loop module' />");

                if (CanComposeControlModuleLocation(track))
                    AddChild(categoryElement, "<Item Name='control-module-location' Tooltip='Location of control modules (turnout and feedback decoders etc.)' />");

                if (track == null) {
                    AddChild(categoryElement, "<Item Name='power-selector' Tooltip='Power selector (select between one of two power sources) or Power switch (power On/Off)' />");

                    //TODO: Add Power Supply, Power Switch components
                }
            }
        }

        //----

        private bool CanComposeTrackPower(LayoutTrackComponent? existingTrack) => existingTrack != null &&
                !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) && existingTrack.BlockDefinitionComponent == null;

        [DispatchTarget]
        private void PaintImageMenuItem_TrackPower(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "track-power") {
            DrawFrame(g);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Black,
                TrackColor2 = Color.Black
            };
            trackPainter.Paint(g);

            using LayoutPowerConnectorPainter trackPowerPainter = new(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });
            trackPowerPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TrackPower(XmlElement itemElement, [DispatchFilter] string name = "track-power") => new LayoutTrackPowerConnectorComponent();

        //----

        private bool CanComposeTrackIsolation(LayoutTrackComponent? existingTrack) => existingTrack != null &&
                !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) && existingTrack.BlockDefinitionComponent == null && !LayoutTrackReverseLoopModule.Is(existingTrack.Spot);

        [DispatchTarget]
        private void PaintImageMenuItem_TrackIsolation(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "track-isolation") {
            DrawFrame(g);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Black
            };
            trackPainter.Paint(g);

            LayoutTrackIsolationPainter trackIsolationPainter = new(new Size(32, 32),
                new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

            trackIsolationPainter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TrackIsolation(XmlElement itemElement, [DispatchFilter] string name = "track-isolation") => new LayoutTrackIsolationComponent();

        //----

        private bool CanComposeTrackReverseLoopModule(LayoutTrackComponent? existingTrack) => existingTrack != null &&
                !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) && existingTrack.BlockDefinitionComponent == null && !LayoutTrackIsolationComponent.Is(existingTrack.Spot);

        [DispatchTarget]
        private void PaintImageMenuItem_TrackReverseLoopModule(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "track-reverse-loop-module") {
            DrawFrame(g);

            LayoutStraightTrackPainter trackPainter = new(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Black
            };
            trackPainter.Paint(g);

            var painter = new LayoutTrackReverseLoopModulePainter(new Size(32, 32),
                new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_TrackReverseLoopModule(XmlElement itemElement, [DispatchFilter] string name = "track-reverse-loop-module") => new LayoutTrackReverseLoopModule();

        //----

        private bool CanComposeControlModuleLocation(LayoutTrackComponent? exitingTrack) => exitingTrack == null;

        [DispatchTarget]
        private void PaintImageMenuItem_ControlModuleLocation(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "control-module-location") {
            DrawFrame(g);

            ControlModuleLocationPainter painter = new();

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_ControlModuleLocation(XmlElement itemElement, [DispatchFilter] string name = "control-module-location") => new LayoutControlModuleLocationComponent();

        //----

        [DispatchTarget]
        private void PaintImageMenuItem_PowerSelector(Graphics g, XmlElement itemElement, [DispatchFilter] string name = "power-selector") {
            DrawFrame(g);

            PowerSelectorPainter painter = new();

            painter.Paint(g);
        }

        [DispatchTarget]
        private ModelComponent CreateModelComponent_PowerSelector(XmlElement itemElement, [DispatchFilter] string name = "power-selector") => new LayoutPowerSelectorComponent();

        #endregion

    }
}
