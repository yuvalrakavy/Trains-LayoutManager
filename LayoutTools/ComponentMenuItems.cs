using System;
using System.ComponentModel;
using System.Xml;
using System.Drawing;
using LayoutManager.Components;
using LayoutManager.Model;

#pragma warning disable IDE0051
namespace LayoutManager.Tools {
    /// <summary>
    /// Summary description for ComponentMenuItems.
    /// </summary>
    [LayoutModule("Component Menu Items", UserControl = false)]
    public class ComponentMenuItems : System.ComponentModel.Component, ILayoutModuleSetup {
        private const string A_Image = "Image";
        private const string A_TrackCp1 = "TrackCp1";
        private const string A_DiagonalIndex = "DiagonalIndex";
        private const string A_TrackCp2 = "TrackCp2";
        private const string A_NewCp1 = "NewCp1";
        private const string A_NewCp2 = "NewCp2";
        private System.Windows.Forms.ImageList imageListCategories;
        private System.Windows.Forms.ImageList imageListComponents;
        private System.Windows.Forms.ImageList imageListComponentsLarge;
        private IContainer components;

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
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xmlText);
            element.AppendChild(element.OwnerDocument.ImportNode(doc.DocumentElement, true));
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
            XmlElement categories = (XmlElement)e.Sender;

            AddChild(categories, "<Category Name='Tracks' Tooltip='Tracks' Image='0' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 100)]
        private void addComposedComponentCategory(LayoutEvent e) {
            XmlElement categories = (XmlElement)e.Sender;

            AddChild(categories, "<Category Name='ComposedTracks' Tooltip='Turnouts, cross, etc.' Image='4' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 200)]
        private void AddBlockCategory(LayoutEvent e) {
            XmlElement categories = (XmlElement)e.Sender;

            AddChild(categories, "<Category Name='Block' Tooltip='Tracks contacts / Blocks' Image='1' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 300)]
        private void AddAnnotationCategory(LayoutEvent e) {
            XmlElement categories = (XmlElement)e.Sender;

            AddChild(categories, "<Category Name='Annotation' Tooltip='Text &amp; Images' Image='2' />");
        }

        [LayoutEvent("get-component-menu-categories", Order = 400)]
        private void AddControlCategory(LayoutEvent e) {
            XmlElement categories = (XmlElement)e.Sender;

            AddChild(categories, "<Category Name='Control' Tooltip='Layout power control elements' Image='3' />");
        }

        /*--------------------------------------------------------------------------------------*/

        /// <summary>
        /// Paint the categories. The Image attribute is the category's image in the image list
        /// </summary>
        [LayoutEvent("paint-image-menu-category", IfSender = "Category[@Image]")]
        private void PaintAnnotationCategory(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;
            Graphics g = (Graphics)e.Info;
            var imageIndex = (int)categoryElement.AttributeValue(A_Image);

            g.DrawImage(imageListCategories.Images[imageIndex], 0, 0);
        }

        #endregion

        #region Track category components

        private string getTooltip(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            if (LayoutTrackComponent.IsDiagonal(cp1, cp2))
                return "Diagonal track";
            else {
                return LayoutTrackComponent.IsHorizontal(cp1) ? "Horizontal track" : "Vertical track";
            }
        }

        private void addTrackItem(XmlElement categoryElement, LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            AddChild(categoryElement, "<Item Name='track-component' Tooltip='" +
                getTooltip(cp1, cp2) + "' cp1='" + cp1 + "' cp2='" + cp2 + "' />");
        }

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='Tracks']")]
        private void AddTrackCategoryItems(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;

            addTrackItem(categoryElement, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R);
            addTrackItem(categoryElement, LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B);
            addTrackItem(categoryElement, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.T);
            addTrackItem(categoryElement, LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.T);
            addTrackItem(categoryElement, LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.B);
            addTrackItem(categoryElement, LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.B);
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='track-component']")]
        private void PaintTrackItem(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;
            Graphics g = (Graphics)e.Info;

            LayoutComponentConnectionPoint cp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp1"));
            LayoutComponentConnectionPoint cp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp2"));

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            LayoutStraightTrackPainter painter = new LayoutStraightTrackPainter(new Size(32, 32), cp1, cp2);
            g.TranslateTransform(4, 4);
            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='track-component']")]
        private void CreateTrackComponent(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;

            LayoutComponentConnectionPoint cp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp1"));
            LayoutComponentConnectionPoint cp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("cp2"));

            e.Info = new LayoutStraightTrackComponent(cp1, cp2);
        }

        #endregion

        #region Composed Track Components Category Items (turnout etc.)

        private void addTurnoutItem(XmlElement categoryElement, LayoutStraightTrackComponent existingTrack, LayoutComponentConnectionPoint tipCp, LayoutComponentConnectionPoint newCp) {
            string tooltip = "turnout";

            AddChild(categoryElement, "<Item Name='turnout-component' Tooltip='" + tooltip +
                "' TrackCp1='" + existingTrack.ConnectionPoints[0].ToString() +
                "' TrackCp2='" + existingTrack.ConnectionPoints[1].ToString() +
                "' TipCp='" + tipCp.ToString() +
                "' NewCp='" + newCp.ToString() + "' />");
        }

        private void addThreeWayTurnoutItem(XmlElement categoryElement, LayoutStraightTrackComponent existingTrack, LayoutComponentConnectionPoint tipCp) {
            string tooltip = "three way turnout";

            AddChild(categoryElement, "<Item Name='three-way-turnout-component' Tooltip='" + tooltip +
                "' TrackCp1='" + existingTrack.ConnectionPoints[0].ToString() +
                "' TrackCp2='" + existingTrack.ConnectionPoints[1].ToString() +
                "' TipCp='" + tipCp.ToString() + "' />");
        }

        private void addDoubleSlipItem(XmlElement categoryElement, LayoutStraightTrackComponent existingTrack, int diagonalIndex) {
            string tooltip = "double-slip turnout";

            AddChild(categoryElement,
                $"<Item Name='double-slip-component' Tooltip='{tooltip}' TrackCp1='{existingTrack.ConnectionPoints[0]}' TrackCp2='{existingTrack.ConnectionPoints[1]}' DiagonalIndex='{diagonalIndex}' />");
        }

        private void addDoubleTrackComponent(XmlElement categoryElement, string tooltip, LayoutStraightTrackComponent existingTrack, LayoutComponentConnectionPoint newCp1, LayoutComponentConnectionPoint newCp2) {
            AddChild(categoryElement, $"<Item Name='double-track-component' Tooltip='{tooltip}' TrackCp1='{existingTrack.ConnectionPoints[0]}' TrackCp2='{existingTrack.ConnectionPoints[1]}' NewCp1='{newCp1}' NewCp2='{newCp2}' />");
        }

        private LayoutComponentConnectionPoint parallelCp(LayoutComponentConnectionPoint cp1, LayoutComponentConnectionPoint cp2) {
            LayoutComponentConnectionPoint[] diagCp = LayoutTrackComponent.DiagonalConnectionPoints(cp1);

            return diagCp[0] == cp2 ? diagCp[1] : diagCp[0];
        }

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='ComposedTracks']")]
        private void AddComposedTrackCategoryItems(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;

            if (e.Info is LayoutStraightTrackComponent track && track.TrackAnnotation == null) {
                if (track.IsDiagonal()) {
                    addTurnoutItem(categoryElement, track, track.ConnectionPoints[0], LayoutTrackComponent.OppositeConnectPoint(track.ConnectionPoints[0]));
                    addTurnoutItem(categoryElement, track, track.ConnectionPoints[1], LayoutTrackComponent.OppositeConnectPoint(track.ConnectionPoints[1]));

                    addDoubleTrackComponent(categoryElement, "Parallel diagonal tracks", track, parallelCp(track.ConnectionPoints[0], track.ConnectionPoints[1]), parallelCp(track.ConnectionPoints[1], track.ConnectionPoints[0]));
                }
                else {
                    LayoutComponentConnectionPoint[] diagCp = LayoutTrackComponent.DiagonalConnectionPoints(track.ConnectionPoints[0]);

                    addTurnoutItem(categoryElement, track, track.ConnectionPoints[0], diagCp[0]);
                    addTurnoutItem(categoryElement, track, track.ConnectionPoints[0], diagCp[1]);

                    diagCp = LayoutTrackComponent.DiagonalConnectionPoints(track.ConnectionPoints[1]);

                    addTurnoutItem(categoryElement, track, track.ConnectionPoints[1], diagCp[0]);
                    addTurnoutItem(categoryElement, track, track.ConnectionPoints[1], diagCp[1]);

                    addThreeWayTurnoutItem(categoryElement, track, track.ConnectionPoints[0]);
                    addThreeWayTurnoutItem(categoryElement, track, track.ConnectionPoints[1]);

                    addDoubleTrackComponent(categoryElement, "Crossed tracks", track, diagCp[0], diagCp[1]);

                    addDoubleSlipItem(categoryElement, track, 0);
                    addDoubleSlipItem(categoryElement, track, 1);

                    if (CanComposeTrackLink(track))
                        AddChild(categoryElement, "<Item Name='track-link' Tooltip='Track link (link to another part of the model)' />");
                }
            }
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='turnout-component']")]
        private void PaintTurnoutTrackItem(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;
            Graphics g = (Graphics)e.Info;

            LayoutComponentConnectionPoint trackCp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp1));
            LayoutComponentConnectionPoint trackCp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp2));
            LayoutComponentConnectionPoint tipCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("TipCp"));
            LayoutComponentConnectionPoint newCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("NewCp"));

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter existingTrackPainter = new LayoutStraightTrackPainter(new Size(32, 32), trackCp1, trackCp2);
            existingTrackPainter.Paint(g);

            LayoutStraightTrackPainter newTrackPainter = new LayoutStraightTrackPainter(new Size(32, 32), tipCp, newCp) {
                TrackColor = Color.LightGreen,
                TrackColor2 = Color.LightGreen
            };
            newTrackPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='turnout-component']")]
        private void CreateTurnoutComponent(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;
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

            e.Info = new LayoutTurnoutTrackComponent(tipCp, straightCp, branchCp);
        }

        //----

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='three-way-turnout-component']")]
        private void PaintThreeWayTurnoutTrackItem(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;
            Graphics g = (Graphics)e.Info;

            LayoutComponentConnectionPoint trackCp1 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp1));
            LayoutComponentConnectionPoint trackCp2 = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute(A_TrackCp2));
            LayoutComponentConnectionPoint tipCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("TipCp"));

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter existingTrackPainter = new LayoutStraightTrackPainter(new Size(32, 32), trackCp1, trackCp2);
            existingTrackPainter.Paint(g);

            LayoutComponentConnectionPoint[] cps;
            if (LayoutTrackComponent.IsHorizontal(tipCp))
                cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
            else
                cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.R, LayoutComponentConnectionPoint.L };

            foreach (LayoutComponentConnectionPoint cp in cps) {
                LayoutStraightTrackPainter painter = new LayoutStraightTrackPainter(new Size(32, 32), tipCp, cp) {
                    TrackColor = Color.LightGreen,
                    TrackColor2 = Color.LightGreen
                };
                painter.Paint(g);
            }
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='three-way-turnout-component']")]
        private void CreateThreeWayTurnoutComponent(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;
            LayoutComponentConnectionPoint tipCp = LayoutComponentConnectionPoint.Parse(itemElement.GetAttribute("TipCp"));

            e.Info = new LayoutThreeWayTurnoutComponent(tipCp);
        }

        // ----

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='double-slip-component']")]
        private void PaintDoubleSlipTrackItem(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;
            Graphics g = (Graphics)e.Info;

            var trackCp1 = itemElement.AttributeValue(A_TrackCp1).ToComponentConnectionPoint();
            var diagonalIndex = (int)itemElement.AttributeValue(A_DiagonalIndex);

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutDoubleSlipPainter painter = new LayoutDoubleSlipPainter(new Size(32, 32), diagonalIndex, -1) {
                HorizontalTrackColor = LayoutTrackComponent.IsHorizontal(trackCp1) ? Color.Black : Color.LightGreen,
                VerticalTrackColor = LayoutTrackComponent.IsVertical(trackCp1) ? Color.Black : Color.LightGreen,
                LeftBranchColor = Color.LightGreen,
                RightBranchColor = Color.LightGreen
            };

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='double-slip-component']")]
        private void CreateDoubleSlipComponent(LayoutEvent e) {
            var itemElement = Ensure.NotNull<XmlElement>(e.Sender, "itemElement");
            var diagonalIndex = (int)itemElement.AttributeValue(A_DiagonalIndex);

            e.Info = new LayoutDoubleSlipTrackComponent(diagonalIndex);
        }

        //----

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='double-track-component']")]
        private void PaintDoubleTrackItem(LayoutEvent e) {
            var itemElement = Ensure.NotNull<XmlElement>(e.Sender, "itemElement");
            var g = Ensure.NotNull<Graphics>(e.Info, "g");

            var trackCp1 = itemElement.AttributeValue(A_TrackCp1).ToComponentConnectionPoint();
            var trackCp2 = itemElement.AttributeValue(A_TrackCp2).ToComponentConnectionPoint();
            var newCp1 = itemElement.AttributeValue(A_NewCp1).ToComponentConnectionPoint();
            var newCp2 = itemElement.AttributeValue(A_NewCp2).ToComponentConnectionPoint();

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);
            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter existingTrackPainter = new LayoutStraightTrackPainter(new Size(32, 32), trackCp1, trackCp2);
            existingTrackPainter.Paint(g);

            LayoutStraightTrackPainter newTrackPainter = new LayoutStraightTrackPainter(new Size(32, 32), newCp1, newCp2) {
                TrackColor = Color.LightGreen,
                TrackColor2 = Color.LightGreen
            };
            newTrackPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='double-track-component']")]
        private void CreateDoubleTrackComponent(LayoutEvent e) {
            XmlElement itemElement = (XmlElement)e.Sender;
            var trackCp1 = itemElement.AttributeValue(A_TrackCp1).ToComponentConnectionPoint();
            var trackCp2 = itemElement.AttributeValue(A_TrackCp2).ToComponentConnectionPoint();

            e.Info = new LayoutDoubleTrackComponent(trackCp1, trackCp2);
        }

        //----

        private bool CanComposeTrackLink(LayoutStraightTrackComponent existingTrack) => !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) &&
                existingTrack.Spot[ModelComponentKind.TrackLink] == null;

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='track-link']")]
        private void PaintTrackLinkItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R) {
                TrackColor = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutTrackLinkPainter linkPainter = new LayoutTrackLinkPainter(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R });
            linkPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='track-link']")]
        private void CreateTrackLinkComponent(LayoutEvent e) {
            e.Info = new LayoutTrackLinkComponent();
        }

        #endregion

        #region Block category items

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='Block']")]
        private void AddBlockCategoryItems(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;
            LayoutTrackComponent old = (LayoutTrackComponent)e.Info;

            if (CanComposeBlockEdge(old))
                AddChild(categoryElement, "<Item Name='track-contact' Tooltip='Track contact (block edge)' />");

            if (CanComposeBlockEdge(old))
                AddChild(categoryElement, "<Item Name='proximity-sensor' Tooltip='Proximity sensor (block edge)' />");

            if (CanComposeBlockEdge(old))
                AddChild(categoryElement, "<Item Name='block-edge' Tooltip='Block Edge' />");

            if (CanComposeBlockInfo(old))
                AddChild(categoryElement, "<Item Name='block-info' Tooltip='Block Information' />");

            if (CanComposeSignal(old))
                AddChild(categoryElement, "<Item Name='signal' Tooltip='Track Signal' />");

            if (CanComposeGate(old))
                AddChild(categoryElement, "<Item Name='gate' Tooltip='Gate' />");
        }

        //----

        private bool CanComposeBlockEdge(LayoutTrackComponent existingTrack) {
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

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='track-contact']")]
        private void PaintTrackContactItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutTriggerableBlockEdgePainter contactPainter = new LayoutTriggerableBlockEdgePainter(componentType: LayoutTriggerableBlockEdgePainter.ComponentType.TrackContact,
                componentSize: new Size(32, 32), cp: new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                ContactSize = new Size(6, 6)
            };
            contactPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='track-contact']")]
        private void CreateTrackContactComponent(LayoutEvent e) {
            e.Info = new LayoutTrackContactComponent();
        }

        //----
        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='proximity-sensor']")]
        private void PaintProximitySensorItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutTriggerableBlockEdgePainter sensorPainter = new LayoutTriggerableBlockEdgePainter(componentType: LayoutTriggerableBlockEdgePainter.ComponentType.ProximitySensor,
                componentSize: new Size(32, 32), cp: new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                ContactSize = new Size(6, 6)
            };
            sensorPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='proximity-sensor']")]
        private void CreateProximitySensorComponent(LayoutEvent e) {
            e.Info = new LayoutProximitySensorComponent();
        }

        //----

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='block-edge']")]
        private void PaintBlockEdgeItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutBlockEdgePainter painter = new LayoutBlockEdgePainter(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                ContactSize = new Size(6, 6)
            };
            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='block-edge']")]
        private void CreateBlockEdgeComponent(LayoutEvent e) {
            e.Info = new LayoutBlockEdgeComponent();
        }

        //----

        private bool CanComposeBlockInfo(LayoutTrackComponent existingTrack) {
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

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='block-info']")]
        private void PaintBlockInfoItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Gray
            };
            trackPainter.Paint(g);

            using LayoutBlockInfoPainter blockInfoPainter = new LayoutBlockInfoPainter(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B }) {
                InfoBoxSize = new Size(6, 6)
            };
            blockInfoPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='block-info']")]
        private void CreateBlockInfoComponent(LayoutEvent e) {
            e.Info = new LayoutBlockDefinitionComponent();
        }

        //----

        private bool CanComposeSignal(LayoutTrackComponent existingTrack) {
            if (existingTrack != null) {
                if (existingTrack is LayoutStraightTrackComponent track && !track.IsDiagonal()) {
                    if (track.Spot[ModelComponentKind.Signal] == null)
                        return true;
                }
            }

            return false;
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='signal']")]
        private void PaintSignalItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);
            g.DrawImage(imageListComponentsLarge.Images[0], 1, 1);

            g.DrawRectangle(Pens.Black, 0, 0, 32, 32);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='signal']")]
        private void CreateSignalComponent(LayoutEvent e) {
            e.Info = new LayoutSignalComponent();
        }

        //----

        private bool CanComposeGate(LayoutTrackComponent old) {
            if (old != null && old is LayoutStraightTrackComponent && !LayoutStraightTrackComponent.IsDiagonal(old)) {
                if (old.Spot[ModelComponentKind.Gate] == null)
                    return true;
            }

            return false;
        }

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='gate']")]
        private void PaintGateItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutComponentConnectionPoint[] cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.L, LayoutComponentConnectionPoint.R };
            Size componentSize = new Size(32, 32);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(componentSize, cps[0], cps[1]);
            trackPainter.Paint(g);

            LayoutGatePainter gatePainter = new LayoutGatePainter(componentSize, false, false, 60);

            gatePainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='gate']")]
        private void createGateComponent(LayoutEvent e) {
            e.Info = new LayoutGateComponent();
        }

        #endregion

        #region Annotation section components

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='Annotation']")]
        private void AddAnnotationCategoryItems(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;
            LayoutTrackComponent old = (LayoutTrackComponent)e.Info;

            AddChild(categoryElement, "<Item Name='text' Tooltip='Text label' />");
            AddChild(categoryElement, "<Item Name='image' Tooltip='Image (picture)' />");

            if (CanComposeBridge(old))
                AddChild(categoryElement, "<Item Name='bridge' Tooltip='Bridge' />");

            if (CanComposeTunnel(old))
                AddChild(categoryElement, "<Item Name='tunnel' Tooltip='Tunnel' />");
        }

        //----

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='text']")]
        private void PaintTextItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            Font f = new Font("Arial", 16);
            g.DrawString("Aa", f, Brushes.BlueViolet, new Point(1, 4));
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='text']")]
        private void CreateTextComponent(LayoutEvent e) {
            e.Info = new LayoutTextComponent();
        }

        //----

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='image']")]
        private void drawImageItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(8, 8);
            g.DrawImage(imageListComponents.Images[0], 1, 1);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='image']")]
        private void createTextComponent(LayoutEvent e) {
            e.Info = new LayoutImageComponent();
        }

        //----

        private bool CanComposeBridge(LayoutTrackComponent old) => old != null && old is LayoutStraightTrackComponent;

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='bridge']")]
        private void PaintBridgeItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutComponentConnectionPoint[] cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
            Size componentSize = new Size(32, 32);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(componentSize, cps[0], cps[1]);
            trackPainter.Paint(g);

            LayoutBridgePainter bridgePainter = new LayoutBridgePainter(componentSize, cps) {
                Offset = 6
            };

            bridgePainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='bridge']")]
        private void createBridgeComponent(LayoutEvent e) {
            e.Info = new LayoutBridgeComponent();
        }

        //----

        private bool CanComposeTunnel(LayoutTrackComponent old) => old != null && old is LayoutStraightTrackComponent;

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='tunnel']")]
        private void PaintTunnelItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
            g.FillRectangle(Brushes.White, 5, 5, 31, 31);

            g.TranslateTransform(4, 4);

            LayoutComponentConnectionPoint[] cps = new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B };
            Size componentSize = new Size(32, 32);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(componentSize, cps[0], cps[1]);
            trackPainter.Paint(g);

            LayoutTunnelPainter tunnelPainter = new LayoutTunnelPainter(componentSize, cps) {
                Offset = 6
            };

            tunnelPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='tunnel']")]
        private void createTunnelComponent(LayoutEvent e) {
            e.Info = new LayoutTunnelComponent();
        }

        #endregion

        #region Power Control section components

        [LayoutEvent("get-component-menu-category-items", IfSender = "Category[@Name='Control']", Order = 100)]
        private void AddControlCategoryItems(LayoutEvent e) {
            XmlElement categoryElement = (XmlElement)e.Sender;
            LayoutTrackComponent old = (LayoutTrackComponent)e.Info;

            if (canComposeTrackPower(old))
                AddChild(categoryElement, "<Item Name='track-power' Tooltip='Track power connector' />");

            if (canComposeTrackIsolation(old))
                AddChild(categoryElement, "<Item Name='track-isolation' Tooltip='Track power isolation' />");

            if (canComposeTrackReverseLoopModule(old))
                AddChild(categoryElement, "<Item Name='track-reverse-loop-module' Tooltip='Track reverse loop module' />");

            if (canComposeControlModuleLocation(old))
                AddChild(categoryElement, "<Item Name='control-module-location' Tooltip='Location of control modules (turnout and feedback decoders etc.)' />");

            if (old == null) {
                AddChild(categoryElement, "<Item Name='power-selector' Tooltip='Power selector (select between one of two power sources) or Power switch (power On/Off)' />");

                //TODO: Add Power Supply, Power Switch components
            }
        }

        //----

        private bool canComposeTrackPower(LayoutTrackComponent existingTrack) => existingTrack != null &&
                !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) && existingTrack.BlockDefinitionComponent == null;

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='track-power']")]
        private void PaintTrackPowerItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            DrawFrame(g);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Black,
                TrackColor2 = Color.Black
            };
            trackPainter.Paint(g);

            using LayoutPowerConnectorPainter trackPowerPainter = new LayoutPowerConnectorPainter(new Size(32, 32),
                      new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });
            trackPowerPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='track-power']")]
        private void CreateTrackPowerComponent(LayoutEvent e) {
            e.Info = new LayoutTrackPowerConnectorComponent();
        }

        //----

        private bool canComposeTrackIsolation(LayoutTrackComponent existingTrack) => existingTrack != null &&
                !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) && existingTrack.BlockDefinitionComponent == null && !LayoutTrackReverseLoopModule.Is(existingTrack.Spot);

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='track-isolation']")]
        private void PaintTrackIsolationItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            DrawFrame(g);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Black
            };
            trackPainter.Paint(g);

            LayoutTrackIsolationPainter trackIsolationPainter = new LayoutTrackIsolationPainter(new Size(32, 32),
                new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

            trackIsolationPainter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='track-isolation']")]
        private void CreateTrackIsolationComponent(LayoutEvent e) {
            e.Info = new LayoutTrackIsolationComponent();
        }

        //----

        private bool canComposeTrackReverseLoopModule(LayoutTrackComponent existingTrack) => existingTrack != null &&
                !LayoutTrackComponent.IsDiagonal(existingTrack.ConnectionPoints[0], existingTrack.ConnectionPoints[1]) && existingTrack.BlockDefinitionComponent == null && !LayoutTrackIsolationComponent.Is(existingTrack.Spot);

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='track-reverse-loop-module']")]
        private void PaintTrackReverseLoopModuleItem(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            DrawFrame(g);

            LayoutStraightTrackPainter trackPainter = new LayoutStraightTrackPainter(new Size(32, 32),
                LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B) {
                TrackColor = Color.Gray,
                TrackColor2 = Color.Black
            };
            trackPainter.Paint(g);

            var painter = new LayoutTrackReverseLoopModulePainter(new Size(32, 32),
                new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B });

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='track-reverse-loop-module']")]
        private void CreateTrackReverseLoopModuleComponent(LayoutEvent e) {
            e.Info = new LayoutTrackReverseLoopModule();
        }

        //----

        private bool canComposeControlModuleLocation(LayoutTrackComponent exitingTrack) => exitingTrack == null;

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='control-module-location']")]
        private void PaintControlModuleLocation(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            DrawFrame(g);

            ControlModuleLocationPainter painter = new ControlModuleLocationPainter();

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='control-module-location']")]
        private void CreateLayoutControlModuleLocationComponent(LayoutEvent e) {
            e.Info = new LayoutControlModuleLocationComponent();
        }

        //----

        [LayoutEvent("paint-image-menu-item", IfSender = "Item[@Name='power-selector']")]
        private void PaintPowerSelector(LayoutEvent e) {
            Graphics g = (Graphics)e.Info;

            DrawFrame(g);

            PowerSelectorPainter painter = new PowerSelectorPainter();

            painter.Paint(g);
        }

        [LayoutEvent("create-model-component", IfSender = "Item[@Name='power-selector']")]
        private void CreatePowerSelector(LayoutEvent e) {
            e.Info = new LayoutPowerSelectorComponent();
        }

        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentMenuItems));
            this.imageListCategories = new System.Windows.Forms.ImageList(this.components);
            this.imageListComponents = new System.Windows.Forms.ImageList(this.components);
            this.imageListComponentsLarge = new System.Windows.Forms.ImageList(this.components);
            // 
            // imageListCategories
            // 
            this.imageListCategories.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListCategories.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListCategories.ImageStream")));
            this.imageListCategories.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListCategories.Images.SetKeyName(0, "BitMapTrackCategory.bmp");
            this.imageListCategories.Images.SetKeyName(1, "");
            this.imageListCategories.Images.SetKeyName(2, "");
            this.imageListCategories.Images.SetKeyName(3, "");
            this.imageListCategories.Images.SetKeyName(4, "BitmapComposedTracksCategory.bmp");
            // 
            // imageListComponents
            // 
            this.imageListComponents.ImageSize = new System.Drawing.Size(16, 16);
            this.imageListComponents.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponents.ImageStream")));
            this.imageListComponents.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComponents.Images.SetKeyName(0, "");
            this.imageListComponents.Images.SetKeyName(1, "");
            // 
            // imageListComponentsLarge
            // 
            this.imageListComponentsLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.imageListComponentsLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponentsLarge.ImageStream")));
            this.imageListComponentsLarge.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComponentsLarge.Images.SetKeyName(0, "");
        }
        #endregion

    }
}
