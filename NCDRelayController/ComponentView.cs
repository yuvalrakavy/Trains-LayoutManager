using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Xml;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.View;

namespace NCDRelayController
{
	/// <summary>
	/// Summary description for ComponentView.
	/// </summary>
	[LayoutModule("NCD Relay controller Component View", UserControl=false)]
	public class ComponentView : System.ComponentModel.Component, ILayoutModuleSetup
	{
		private ImageList imageListComponents;
		private IContainer components = null;

		#region Implementation of ILayoutModuleSetup

		#endregion

		#region Constructors

		public ComponentView()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
		}

		#endregion

		#region Component menu Item

		[LayoutEvent("get-component-menu-category-items", IfSender="Category[@Name='Control']")]
		void AddRelayControllerItem(LayoutEvent e) {
			XmlElement		categoryElement = (XmlElement)e.Sender;
			ModelComponent	old = (ModelComponent)e.Info;

			if(old == null)
				categoryElement.InnerXml += "<Item Name='NCDRelayController' Tooltip='NCD Relay Controller' />";
		}

		[LayoutEvent("paint-image-menu-item", IfSender="Item[@Name='NCDRelayController']")]
		void PaintCentralStationItem(LayoutEvent e) {
			Graphics	g = (Graphics)e.Info;

			g.DrawRectangle(Pens.Black, 4, 4, 32, 32);
			g.FillRectangle(Brushes.White, 5, 5, 31, 31);

			g.TranslateTransform(4, 4);

			NCDRelayControllerPainter painter = new NCDRelayControllerPainter(new Size(32, 32));

			painter.Paint(g);
		}

		[LayoutEvent("create-model-component", IfSender="Item[@Name='NCDRelayController']")]
		void CreateCentralStationComponent(LayoutEvent e) {
			e.Info = new NCDRelayController();
		}

		#endregion

		#region Component view

		[LayoutEvent("get-model-component-drawing-regions", SenderType=typeof(NCDRelayController))]
		void GetDiMAXcommandStationDrawingRegions(LayoutEvent eBase) {
			LayoutGetDrawingRegionsEvent	e = (LayoutGetDrawingRegionsEvent)eBase;

			if(LayoutDrawingRegionGrid.IsComponentGridVisible(e))
				e.AddRegion(new DrawingRegionNCDRelayController(e.Component, e.View));

			LayoutTextInfo textProvider = new LayoutTextInfo(e.Component);

			if(textProvider.Element != null)
				e.AddRegion(new LayoutDrawingRegionText(e, textProvider));

		}

		class DrawingRegionNCDRelayController : LayoutDrawingRegionGrid {
			NCDRelayController component;

			internal DrawingRegionNCDRelayController(ModelComponent component, ILayoutView view) : base(component, view) {
				this.component = (NCDRelayController)component;
			}

			public override void Draw(ILayoutView view, ViewDetailLevel detailLevel, ILayoutSelectionLook selectionLook, Graphics g) {
				NCDRelayControllerPainter painter = new NCDRelayControllerPainter(view.GridSizeInModelCoordinates);

				painter.Paint(g);
				base.Draw(view, detailLevel, selectionLook, g);
			}
		}

		#endregion

		#region Component Painter

		[LayoutEvent("get-image", SenderType = typeof(NCDRelayControllerPainter))]
		void GetCentralStationImage(LayoutEvent e) {
			e.Info = imageListComponents.Images[0];
		}

		class NCDRelayControllerPainter {
			Size				componentSize;

			internal NCDRelayControllerPainter(Size componentSize) {
				this.componentSize = componentSize;
			}

			internal void Paint(Graphics g) {
				Image	image = (Image)EventManager.Event(new LayoutEvent(this, "get-image"));

				g.DrawImage(image, new Rectangle(new Point(1, 1), image.Size));
			}
		}

		#endregion

		#region Address Format Handler

		[LayoutEvent("get-command-station-address-format", IfEvent="*[CommandStation/@Type='DiMAX']")]
		[LayoutEvent("get-command-station-address-format", IfEvent="*[CommandStation/@Type='Any']")]
		private void getCommandStationFormat(LayoutEvent e) {
			if(e.Info == null) {
				AddressUsage		usage = (AddressUsage)e.Sender;
				AddressFormatInfo	addressFormat = new AddressFormatInfo();

				switch(usage) {
					case AddressUsage.Locomotive:
						addressFormat.Namespace = "Locomotives";
						addressFormat.UnitMin = 1;
						addressFormat.UnitMax = 10239;
						break;

					case AddressUsage.Signal:
					case AddressUsage.Turnout:
						addressFormat.Namespace = "Accessories";
						addressFormat.UnitMin = 0;
						addressFormat.UnitMax = 2047;
						break;

					case AddressUsage.TrainDetectionBlock:
						addressFormat.Namespace = "Accessories";
						addressFormat.UnitMin = 0;
						addressFormat.UnitMax = 2047;
						break;

					case AddressUsage.TrackContact:
						addressFormat.Namespace = "Accessories";
						addressFormat.UnitMin = 0;
						addressFormat.UnitMax = 2047;
						addressFormat.ShowSubunit = true;
						addressFormat.SubunitMin = 0;
						addressFormat.SubunitMax = 1;
						addressFormat.SubunitFormat = AddressFormatInfo.SubunitFormatValue.Alphabet;
						break;
				}

				e.Info = addressFormat;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentView));
			this.imageListComponents = new ImageList(this.components);
			// 
			// imageListComponents
			// 
			this.imageListComponents.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComponents.ImageStream")));
			this.imageListComponents.TransparentColor = System.Drawing.Color.Lime;
			this.imageListComponents.Images.SetKeyName(0, "NCDrelayController.bmp");

		}
		#endregion

	}
}
