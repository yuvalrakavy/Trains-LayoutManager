using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.View {

	public class LayoutStraightTrackComponentView : LayoutComponentView {
		public override ILayoutDrawingRegion[] GetDrawingRegions(ModelComponent component, ILayoutView view, Graphics g) {
			if(IsComponentGridVisible(view, component))
				return new ILayoutDrawingRegion[] { new LayoutStraightTrackComponentView.LayoutDrawingRegionStraightTrack(component, view) };
			else
				return new ILayoutDrawingRegion[] { };
		}
		
		class LayoutDrawingRegionStraightTrack : LayoutDrawingRegionGrid {
			LayoutStraightTrackComponent	component;

			public LayoutDrawingRegionStraightTrack(ModelComponent component, ILayoutView view) : base(component, view) {
				this.component = (LayoutStraightTrackComponent)component;
			}

			public override void Draw(ILayoutView view, ILayoutSelectionLook selectionLook, Graphics g) {
				LayoutComponentConnectionPoint[]	cp = component.ConnectionPoints;

				LayoutStraightTrackPainter	painter = new LayoutStraightTrackPainter(view.MpGridSize, cp[0], cp[1]);
				painter.Paint(g);

				base.Draw(view, selectionLook, g);
			}
		}

	}

	public class LayoutDoubleTrackComponentView : LayoutComponentView {

		public override ILayoutDrawingRegion[] GetDrawingRegions(ModelComponent component, ILayoutView view, Graphics g) {
			if(IsComponentGridVisible(view, component))
				return new ILayoutDrawingRegion[] { new LayoutDoubleTrackComponentView.LayoutDrawingRegionDoubleTrack(component, view) };
			else
				return new ILayoutDrawingRegion[] { };
		}

		class LayoutDrawingRegionDoubleTrack : LayoutDrawingRegionGrid {
			LayoutDoubleTrackComponent	component;

			public LayoutDrawingRegionDoubleTrack(ModelComponent component, ILayoutView view) : base(component, view) {
				this.component = (LayoutDoubleTrackComponent)component;
			}

			public override void Draw(ILayoutView view, ILayoutSelectionLook selectionLook, Graphics g) {
				LayoutComponentConnectionPoint[]	path1 = component.GetTrackPath(0);
				LayoutComponentConnectionPoint[]	path2 = component.GetTrackPath(1);

				LayoutStraightTrackPainter	painter = new LayoutStraightTrackPainter(view.MpGridSize, path1[0], path1[1]);

				painter.Paint(g);
				painter = new LayoutStraightTrackPainter(view.MpGridSize, path2[0], path2[1]);
				painter.Paint(g);

				base.Draw(view, selectionLook, g);
			}
		}
	}

	public class LayoutSwitchTrackComponentView : LayoutComponentView {

		public override ILayoutDrawingRegion[] GetDrawingRegions(ModelComponent component, ILayoutView view, Graphics g) {
			if(IsComponentGridVisible(view, component))
				return new ILayoutDrawingRegion[] { new LayoutSwitchTrackComponentView.LayoutDrawingRegionSwitchTrack(component, view) };
			else
				return new ILayoutDrawingRegion[] { };
		}

		class LayoutDrawingRegionSwitchTrack : LayoutDrawingRegionGrid {
			LayoutSwitchTrackComponent	component;

			public LayoutDrawingRegionSwitchTrack(ModelComponent component, ILayoutView view) : base(component, view) {
				this.component = (LayoutSwitchTrackComponent)component;
			}

			public override void Draw(ILayoutView view, ILayoutSelectionLook selectionLook, Graphics g) {
				LayoutSwitchTrackPainter	painter = new LayoutSwitchTrackPainter(view.MpGridSize,
					component.Tip, component.Straight, component.Branch, component.SwitchPosition);

				painter.Paint(g);
				base.Draw(view, selectionLook, g);
			}
		}
	}

	public class LayoutTrackContactComponentView : LayoutComponentView {

		public override ILayoutDrawingRegion[] GetDrawingRegions(ModelComponent component, ILayoutView view, Graphics g) {
			if(IsComponentGridVisible(view, component))
				return new ILayoutDrawingRegion[] { new LayoutTrackContactComponentView.LayoutDrawingRegionTrackContact(component, view) };
			else
				return new ILayoutDrawingRegion[] { };
		}

		class LayoutDrawingRegionTrackContact : LayoutDrawingRegionGrid {
			LayoutTrackContactComponent	component;

			public LayoutDrawingRegionTrackContact(ModelComponent component, ILayoutView view) : base(component, view) {
				this.component = (LayoutTrackContactComponent)component;
			}

			public override void Draw(ILayoutView view, ILayoutSelectionLook selectionLook, Graphics g) {
				if(component.Track != null) {
					LayoutTrackContactPainter	painter = new LayoutTrackContactPainter(view.MpGridSize, component.Track.ConnectionPoints);

					painter.Paint(g);
				}
				else {
					// If there is no track, paint a large contact in the middle of the component. This case should
					// not really happend
					LayoutTrackContactPainter	painter = new LayoutTrackContactPainter(view.MpGridSize, 
						new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B } );

					painter.ContactSize = new Size(12, 12);
					painter.Paint(g);
				}
				base.Draw(view, selectionLook, g);
			}
		}
	}

	public class LayoutTrackLinkComponentView : LayoutComponentView {

		public override ILayoutDrawingRegion[] GetDrawingRegions(ModelComponent component, ILayoutView view, Graphics g) {
			ArrayList	regions = new ArrayList(2);

			if(IsComponentGridVisible(view, component))
				regions.Add(new LayoutTrackLinkComponentView.LayoutDrawingRegionTrackLink(component, view));

			LayoutTrackLinkTextInfo textProvider = new LayoutTrackLinkTextInfo(component, "Name");

			if(textProvider.Element != null)
				regions.Add(new LayoutDrawingRegionText(component, view, g, textProvider));

			return (ILayoutDrawingRegion[])regions.ToArray(typeof(ILayoutDrawingRegion));
		}

		// Drawing regions

		class LayoutDrawingRegionTrackLink : LayoutDrawingRegionGrid {
			LayoutTrackLinkComponent	component;

			public LayoutDrawingRegionTrackLink(ModelComponent component, ILayoutView view) : base(component, view) {
				this.component = (LayoutTrackLinkComponent)component;
			}

			public override void Draw(ILayoutView view, ILayoutSelectionLook selectionLook, Graphics g) {
				LayoutTrackComponent	track = (LayoutTrackComponent)component.Spot[component.Layer, ModelComponentKind.TRACK];

				if(track != null) {
					LayoutTrackLinkPainter	painter = new LayoutTrackLinkPainter(view.MpGridSize, track.ConnectionPoints);

					if(component.Link == null)
						painter.Fill = Brushes.Red;

					painter.Paint(g);
				}
				else {
					// If there is no track, This case should not really happend
					LayoutTrackLinkPainter	painter = new LayoutTrackLinkPainter(view.MpGridSize, 
						new LayoutComponentConnectionPoint[] { LayoutComponentConnectionPoint.T, LayoutComponentConnectionPoint.B } );

					painter.Paint(g);
				}

				base.Draw(view, selectionLook, g);
			}
		}
	}

	public class LayoutTextComponentView : LayoutComponentView {
		public override ILayoutDrawingRegion[] GetDrawingRegions(ModelComponent component, ILayoutView view, Graphics g) {
			LayoutTextInfo textProvider = new LayoutTextInfo(component, "Text");

			if(textProvider.Element != null)
				return new ILayoutDrawingRegion[] { new LayoutDrawingRegionText(component, view, g, textProvider) };
			else
				return new ILayoutDrawingRegion[] { };
		}
	}
}
