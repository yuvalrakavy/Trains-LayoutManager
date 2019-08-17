using System;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TrackLinkTree.
    /// </summary>
    public class TrackLinkTree : TreeView {
        private IContainer components;
        private ImageList imageListTrackLinks;

        #region Exposed properties

        public TrackLinkTree() {
            InitializeComponent();

            foreach (LayoutModelArea area in LayoutModel.Areas)
                Nodes.Add(new AreaTreeNode(area));
        }

        public LayoutTrackLink ThisComponentLink {
            set {
                if (value != null) {
                    TrackLinkTreeNode thisComponentNode = findNode(value);

                    if (thisComponentNode != null)
                        thisComponentNode.Remove();     // Remove it from the tree, so user cannot do circular link
                }
            }
        }

        public LayoutTrackLink SelectedTrackLink {
            get {
                Object selectedObj = base.SelectedNode;

                return selectedObj is TrackLinkTreeNode ? ((TrackLinkTreeNode)selectedObj).Link : null;
            }

            set {
                if (value != null) {
                    TrackLinkTreeNode selectedNode = findNode(value);

                    if (selectedNode == null)
                        throw new ApplicationException("Link not found in tree");
                    else
                        SelectedNode = selectedNode;
                }
            }
        }
        #endregion

        #region Tree node that represents an area and a track link

        private class TrackLinkTreeNode : TreeNode {
            internal TrackLinkTreeNode(LayoutTrackLinkComponent trackLink) {
                Link = trackLink.ThisLink;

                string text = new LayoutTextInfo(trackLink).Text;

                if (trackLink.Link != null) {
                    LayoutTextInfo linkedName = new LayoutTextInfo(trackLink.LinkedComponent, "Name");

                    text += " (to " + linkedName.Text + ")";
                }

                this.Text = text;

                if (trackLink.Link == null) {
                    this.ImageIndex = 2;
                    this.SelectedImageIndex = 4;
                }
                else {
                    this.ImageIndex = 1;
                    this.SelectedImageIndex = 3;
                }
            }

            internal LayoutTrackLink Link { get; }
        }

        private class AreaTreeNode : TreeNode {
            internal AreaTreeNode(LayoutModelArea area) {
                this.Text = area.Name;
                this.ImageIndex = 0;
                this.SelectedImageIndex = 0;

                foreach (LayoutTrackLinkComponent trackLink in area.TrackLinks)
                    this.Nodes.Add(new TrackLinkTreeNode(trackLink));
            }
        }

        #endregion

        private TrackLinkTreeNode findNode(LayoutTrackLink trackLink) {
            foreach (AreaTreeNode areaNode in this.Nodes) {
                foreach (TrackLinkTreeNode linkNode in areaNode.Nodes)
                    if (linkNode.Link.CompareTo(trackLink) == 0)
                        return linkNode;
            }

            return null;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TrackLinkTree));
            this.imageListTrackLinks = new ImageList(this.components) {
                // 
                // imageListTrackLinks
                // 
                ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit,
                ImageSize = new System.Drawing.Size(16, 16),
                ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListTrackLinks.ImageStream"),
                TransparentColor = System.Drawing.Color.Transparent
            };
            // 
            // TrackLinkTree
            // 
            this.ImageIndex = 0;
            this.ImageList = this.imageListTrackLinks;
            this.SelectedImageIndex = 0;
        }
        #endregion

        protected override void OnPaint(PaintEventArgs pe) {
            // TODO: Add custom paint code here
        }
    }
}
