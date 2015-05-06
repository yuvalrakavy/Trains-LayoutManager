using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace Intellibox
{
	/// <summary>
	/// Summary description for ComponentView.
	/// </summary>
	[LayoutModule("Intellibox Component View", UserControl=false)]
	public class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup
	{
		private System.Windows.Forms.ImageList imageListComponents;
		private IContainer components;

		#region Constructors

		public ComponentView(IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public ComponentView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		#endregion

		#region Component menu Item

		[LayoutEvent("get-component-menu-category-items", IfSender="Category[@Name='Control']")]
		void AddCentralStationItem(LayoutEvent e) {
			XmlElement		categoryElement = (XmlElement)e.Sender;
			ModelComponent	old = (ModelComponent)e.Info;

			if(old == null)
				categoryElement.InnerXml += "<Item Name='Intellibox' Tooltip='Intellibox Command Station' />";
		}

		[LayoutEvent("paint-image-menu-item", IfSender="Item[@Name='Intellibox']")]
		void PaintCentralStationItem(LayoutEvent e) {
			Graphics	g = (Graphics)e.Info;

			g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
			g.FillRectangle(Brushes.White, 5, 5, 31, 31);

			g.TranslateTransform(4, 4);

			IntelliboxPainter	painter = new IntelliboxPainter(new Size(32, 32));

			painter.Paint(g);
		}

		[LayoutEvent("create-model-component", IfSender="Item[@Name='Intellibox']")]
		void CreateCentralStationComponent(LayoutEvent e) {
			e.Info = new IntelliboxComponent();
		}

		#endregion

		#region Component view

		[LayoutEvent("get-model-component-drawing-regions", SenderType=typeof(IntelliboxComponent))]
		void GetCentralStationDrawingRegions(LayoutEvent eBase) {
			LayoutGetDrawingRegionsEvent	e = (LayoutGetDrawingRegionsEvent)eBase;

			if(LayoutDrawingRegionGrid.IsComponentGridVisible(e))
				e.AddRegion(new DrawingRegionIntellibox(e.Component, e.View));

			LayoutTextInfo textProvider = new LayoutTextInfo(e.Component);

			if(textProvider.Element != null)
				e.AddRegion(new LayoutDrawingRegionText(e, textProvider));

		}

		class DrawingRegionIntellibox : LayoutDrawingRegionGrid {
			IntelliboxComponent	component;

			internal DrawingRegionIntellibox(ModelComponent component, ILayoutView view) : base(component, view) {
				this.component = (IntelliboxComponent)component;
			}

			public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
				IntelliboxPainter	painter = new IntelliboxPainter(view.GridSizeInModelCoordinates);

				painter.Paint(g);
				base.Draw(view, detailLevel, selectionLook, g);
			}
		}

		#endregion

		#region Component Painter

		[LayoutEvent("get-image", SenderType=typeof(IntelliboxPainter))]
		void GetCentralStationImage(LayoutEvent e) {
			e.Info = imageListComponents.Images[0];
		}

		class IntelliboxPainter {
			Size				componentSize;

			internal IntelliboxPainter(Size componentSize) {
				this.componentSize = componentSize;
			}

			internal void Paint(Graphics g) {
				Image	image = (Image)EventManager.Event(new LayoutEvent(this, "get-image"));

				g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
			}
		}

		#endregion

		#region Address Format Handler

		[LayoutEvent("get-command-station-address-format", IfEvent="*[CommandStation/@Type='IntelliboxMarklin']")]
		[LayoutEvent("get-command-station-address-format", IfEvent="*[CommandStation/@Type='Any']")]
		private void getCommandStationFormat(LayoutEvent e) {
			if(e.Info == null) {
				AddressUsage		usage = (AddressUsage)e.Sender;
				AddressFormatInfo	addressFormat = new AddressFormatInfo();

				switch(usage) {
					case AddressUsage.Locomotive:
						addressFormat.Namespace = "Locomotives";
						addressFormat.UnitMin = 1;
						addressFormat.UnitMax = 80;
						break;

					case AddressUsage.Signal:
					case AddressUsage.Turnout:
						addressFormat.Namespace = "Accessories";
						addressFormat.UnitMin = 0;
						addressFormat.UnitMax = 255;
						break;

					case AddressUsage.TrainDetectionBlock:
					case AddressUsage.TrackContact:
						addressFormat.Namespace = "Feedback";
						addressFormat.UnitMin = 1;
						addressFormat.UnitMax = 31;
						addressFormat.ShowSubunit = true;
						addressFormat.SubunitMin = 1;
						addressFormat.SubunitMax = 16;
						addressFormat.SubunitFormat = AddressFormatInfo.SubunitFormatValue.Number;
						break;
				}

				e.Info = addressFormat;
			}
		}

		#endregion

		#region Internal singleton (i.e. not for each component instance) event handlers

		/// <summary>
		/// This event handler is "invoked" for a command manager thread. When polling the intellibox, the polling handler
		/// create an array of events that need to be invoked. Those events are invoked in the context of the main thread.
		/// </summary>
		/// <param name="e.Info">The array of events to be invoked</param>
		[LayoutEvent("intellibox-invoke-events")]
		private void intelliboxInvokeEvents(LayoutEvent e) {
			List<LayoutEvent> events = (List<LayoutEvent>)e.Info;

			events.ForEach(delegate(LayoutEvent ev) { EventManager.Event(ev); });
		}

		[LayoutEvent("intellibox-notify-locomotive-state")]
		private void intelliboxNotifiyLocomotiveState(LayoutEvent e) {
			IntelliboxComponent			component = (IntelliboxComponent)e.Sender;
			ExternalLocomotiveEventInfo	info = (ExternalLocomotiveEventInfo)e.Info;
			var							addressMap = (OnTrackLocomotiveAddressMap)EventManager.Event(new LayoutEvent(component, "get-on-track-locomotive-address-map"));
			var							addressMapEntry = addressMap[info.Unit];

            if(addressMapEntry == null)
                LayoutModuleBase.Warning("Locomotive status reported for a unrecognized locomotive (unit " + info.Unit + ")");
            else {
				TrainStateInfo train = addressMapEntry.Train;

                int speedInSteps = 0;

                if(info.LogicalSpeed > 1) {
                    double factor = (double)train.SpeedSteps / 126;

                    speedInSteps = (int)Math.Round((info.LogicalSpeed - 2) * factor);

                    if(speedInSteps == 0)
                        speedInSteps = 1;
                    else if(speedInSteps > train.SpeedSteps)
                        speedInSteps = train.SpeedSteps;

                    if(info.Direction == LocomotiveOrientation.Backward)
                        speedInSteps = -speedInSteps;
                }

                EventManager.Event(new LayoutEvent(component, "locomotive-motion-notification",
                    "<Address Unit='" + info.Unit + "' />", speedInSteps));

                if(info.Lights != train.Lights)
                    EventManager.Event(new LayoutEvent(component, "set-locomotive-lights-notification",
                        "<Address Unit='" + info.Unit + "' />", info.Lights));
            }
		}





		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ComponentView));
			this.imageListComponents = new System.Windows.Forms.ImageList(this.components);
			// 
			// imageListComponents
			// 
			this.imageListComponents.ImageSize = new System.Drawing.Size(30, 30);
			this.imageListComponents.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponents.ImageStream")));
			this.imageListComponents.TransparentColor = System.Drawing.Color.Transparent;

		}
		#endregion

	}
}
