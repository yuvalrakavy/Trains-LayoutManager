
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TrackLinkTree.
    /// </summary>
    public partial class TrackLinkTree : TreeView {

        #region Exposed properties

        public TrackLinkTree() {
            InitializeComponent();

            foreach (LayoutModelArea area in LayoutModel.Areas)
                Nodes.Add(new AreaTreeNode(area));
        }

        public LayoutTrackLink ThisComponentLink {
            set {
                if (value != null) {
                    var thisComponentNode = FindNode(value);

                    if (thisComponentNode != null)
                        thisComponentNode.Remove();     // Remove it from the tree, so user cannot do circular link
                }
            }
        }

        public LayoutTrackLink? SelectedTrackLink {
            get {
                var selectedObj = base.SelectedNode;

                return selectedObj is TrackLinkTreeNode node ? node.Link : null;
            }

            set {
                if (value != null) {
                    var selectedNode = FindNode(value);

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

                if (trackLink.Link != null && trackLink.LinkedComponent != null) {
                    LayoutTextInfo linkedName = new(trackLink.LinkedComponent, "Name");

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

        private TrackLinkTreeNode? FindNode(LayoutTrackLink trackLink) {
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


        protected override void OnPaint(PaintEventArgs pe) {
            // TODO: Add custom paint code here
        }
    }
}
