using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

using LayoutManager.Model;

namespace LayoutManager.Tools.Controls
{
	/// <summary>
	/// Summary description for LocomotiveList.
	/// </summary>
	public class ActiveLocomotiveList : XmlQueryListbox
	{
		LayoutEventManager			eventManager;
		LocomotiveCatalogInfo	catalog = null;
		LayoutModel					model = null;

		public ActiveLocomotiveList()
		{
			if(!DesignMode) {
				AddLayout(new ListLayoutSimple());
			}

			DefaultSortField = "Name";
		}

		public LayoutEventManager EventManager {
			get {
				return eventManager;
			}

			set {
				eventManager = value;
			}
		}

		public LayoutModel Model {
			get {
				if(model == null)
					model = (LayoutModel)EventManager.Event(new LayoutEvent(this, "get-model"));
				return model;
			}
		}

		public LayoutStateManager StateManager {
			get {
				return Model.LayoutStateManager;
			}
		}

		public override IXmlQueryListboxItem CreateItem(QueryItem query, XmlElement itemElement) {
			return new ActiveLocomotiveItem(this, query, new TrainStateInfo(StateManager, itemElement));
		}

		public XmlElement SelectedXmlElement {
			get {
				if(SelectedXmlItem != null)
					return ((IXmlQueryListBoxXmlElementItem)SelectedXmlItem).Element;
				return null;
			}
		}

		public TrainStateInfo SelectedLocomotiveState {
			get {
				XmlElement	element = SelectedXmlElement;

				if(element != null)
					return new TrainStateInfo(StateManager, element);
				return null;
			}
		}

		protected LocomotiveCatalogInfo Catalog {
			get {
				if(catalog == null)
					catalog = Model.LocomotiveCatalog;
				return catalog;
			}
		}

		#region Item classes

		class ActiveLocomotiveItem : IXmlQueryListBoxXmlElementItem {
			ActiveLocomotiveList	list;
			TrainStateInfo	trainState;
			QueryItem				query;

			public ActiveLocomotiveItem(ActiveLocomotiveList list, QueryItem queryItem, TrainStateInfo trainState) {
				this.list = list;
				this.query = query;
				this.trainState = trainState;
			}

			public XmlElement Element {
				get {
					return trainState.Element;
				}
			}

			public void Measure(MeasureItemEventArgs e) {
				e.ItemHeight = 50;
			}

			public void Draw(DrawItemEventArgs e) {
				e.DrawBackground();
				e.DrawFocusRectangle();

				GraphicsState	gs = e.Graphics.Save();

				e.Graphics.TranslateTransform(e.Bounds.Left, e.Bounds.Top);

				// Draw the locomotive image and name. There are two possible layouts one
				// for locomotive and one for locomotive set
				int		xText;
				float	yText;
				String	name;

				if(trainState.IsLocomotive) {
					LocomotiveInfo	loco = trainState.Locomotive;

					using(LocomotiveImagePainter locoPainter = new LocomotiveImagePainter(list.Catalog)) {
						locoPainter.Draw(e.Graphics, new Point(2, 2), new Size(50, 36), loco.Element);
					};

					yText = 2;
					xText = 55;
					name = loco.Name;
				}
				else if(trainState.IsLocomotiveSet) {
					LocomotiveSetInfo	locoSet = trainState.LocomotiveSet;

					using(LocomotiveImagePainter	locoPainter = new LocomotiveImagePainter(list.Catalog)) {
						int		x = 2;

						locoPainter.FrameSize = new Size(28, 20);

						foreach(XmlElement memberElement in locoSet.MemberElements) {
							LocomotiveSetMemberInfo	member = new LocomotiveSetMemberInfo(memberElement);
						
							locoPainter.LocomotiveElement = list.Model.LocomotiveCollection[member.LocomotiveID];
							locoPainter.FlipImage = (member.Orientation == LocomotiveOrientation.Backward);
							locoPainter.Origin = new Point(x, 2);
							locoPainter.Draw(e.Graphics);

							x += locoPainter.FrameSize.Width + 2;
						}
					}

					xText = 2;
					yText = 24;
					name = locoSet.Name;
				}
				else
					throw new ApplicationException("Invalid trainState");

				using(Brush textBrush = new SolidBrush((e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.WindowText)) {
					SizeF	textSize; 

					using(Font titleFont = new Font("Arial", 8, FontStyle.Bold)) {
						textSize = e.Graphics.MeasureString(name, titleFont);
						e.Graphics.DrawString(name, titleFont, textBrush, new PointF(xText, yText));
					}

					yText += textSize.Height;
					String	status = "Not moving";		// TODO: Get a real description of locomotive status

					using(Font typeFont = new Font("Arial", 6.5F, FontStyle.Regular)) {
						textSize = e.Graphics.MeasureString(status, typeFont);
						e.Graphics.DrawString(status, typeFont, textBrush, new PointF(xText, yText));
					}

					yText += textSize.Height;
				}

				e.Graphics.Restore(gs);

				using(Pen p = new Pen(Color.Black, 2.0F))
					e.Graphics.DrawLine(p, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Bottom);

			}

			public object Bookmark { 
				get {
					return trainState.ID.ToString();
				}
			}

			public bool IsBookmarkEqual(object bookmark) {
				if(bookmark is String)
					return (String)bookmark == trainState.Element.GetAttribute("ID");
				return false;
			}
		}

		#endregion

		#region ListLayout classes

		class ListLayoutSimple : ListLayout {

			public override String LayoutName {
				get {
					return "Simple";
				}
			}

			public override void ApplyLayout(XmlQueryListbox list) {
				list.AddQuery("Locomotives", "*[@Kind='Locomotive']").Expand();
				list.AddQuery("Locomotive Sets", "*[@Kind='LocomotiveSet']").Expand();
			}
		}

		#endregion
	}
}
