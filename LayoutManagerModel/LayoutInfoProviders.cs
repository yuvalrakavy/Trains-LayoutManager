using System;
using System.Collections;
using System.Drawing;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

//using LayoutManager.Components;

using LayoutManager.Components;
using LayoutManager;

namespace LayoutManager.Model {
	/// <summary>
	/// Base class for information providers. Information providers wrap XML element and add symantics to them.
	/// </summary>
	public class LayoutInfo : LayoutXmlWrapper {
		bool					elementInComponentDocument;
		XmlElement				container;
		ModelComponent			component;

		[System.ComponentModel.Browsable(false)]
		public String ElementPath {
			get {
				if(Ref != null)
					return "//" + Element.Name + "[@Ref=\"" + Ref + "\"]";
				return "//" + Element.Name + "[@ID=\"" + GetAttribute("ID") + "\"]";
			}
		}

		public ModelComponent Component {
			get {
				return component;
			}

			set {
				component = value;
			}
		}

		public XmlElement ContainerElement {
			get {
				return container;
			}

			set {
				container = value;
			}
		}

		/// <summary>
		/// True if the provider element was found in the component document, false otherwise.
		/// This property has meaning only if the provider element was found (Element is not null)
		/// </summary>
		[Browsable(false)]
		public bool FoundInComponentDocument {
			get {
				return elementInComponentDocument;
			}
		}

		/// <summary>
		/// Locate the given element. The element is first searched in the component's document.
		/// If the element cannot be found, it will be searched in the model document
		/// </summary>
		/// <param name="xmlInfo">The component XML document in which the provider is searched</param>
		/// <param name="elementName">The element</param>
		public LayoutInfo(IObjectHasXml container, string elementPath) {
			SearchProviderElement(container.Element, elementPath);
		}

		public LayoutInfo(XmlElement container, string elementPath) {
			SearchProviderElement(container, elementPath);
		}

		public LayoutInfo(ModelComponent component, String elementPath) {
			this.Component = component;
			SearchProviderElement(component.Element, elementPath);
		}

		public void SearchProviderElement(XmlElement container, String elementPath) {
			if(container != null) {
				XmlNodeList	elements = container.SelectNodes(elementPath);

				Element = null;
				this.container = container;

				if(elements.Count == 1) {
					Element = (XmlElement)elements[0];
					elementInComponentDocument = true;
				}
				else if(elements.Count > 1)
					throw new ArgumentException("More than one provider element found in component");

				// If the element was not found in the component XML document, search the model document
				if(Element == null) {
					elements = LayoutModel.Instance.XmlInfo.DocumentElement.SelectNodes(elementPath);

					if(elements.Count == 1) {
						Element = (XmlElement)elements[0];
						elementInComponentDocument = false;
					}
					else if(elements.Count > 1)
						throw new ArgumentException("More than one provider element found in model document");
				}
			}
		}

		/// <summary>
		/// Initialize the Info provider based on a given element
		/// </summary>
		/// <param name="element"></param>
		public LayoutInfo(XmlElement element) : base(element) {
		}

		/// <summary>
		/// Initialize an empty info provider. You should probably call CreateComponentElement
		/// or CreateModelElement to initialize the provider
		/// </summary>
		public LayoutInfo() {
			Element = null;
		}

		private static void attachNewElement(XmlElement container, String parentPath, XmlElement element) {
			XmlElement	parentElement = container;

			element.SetAttribute("ID", XmlConvert.ToString(Guid.NewGuid()));

			if(parentPath != null) {
				XmlNodeList	parentNodes = container.SelectNodes(parentPath);

				if(parentNodes.Count > 0)
					parentElement = (XmlElement)parentNodes[0];
				else
					throw new ArgumentException("Could not find parent node");
			}

			parentElement.AppendChild(element);
		}

		/// <summary>
		/// Create a new element for the component. The provider is based on this element
		/// </summary>
		/// <param name="xmlInfo">The XML document in which to create the provider</param>
		/// <param name="parentPath">The XPath to the new provider's element parent</param>
		/// <param name="elementName">The provider's element name</param>
		public static XmlElement CreateProviderElement(XmlElement container, string elementName, string parentPath = null) {
			XmlElement	element = container.OwnerDocument.CreateElement(elementName);

			attachNewElement(container, parentPath, element);
			return element;
		}

		public static XmlElement CreateProviderElement(LayoutXmlInfo xmlInfo, string elementName, string parentPath = null) {
			return CreateProviderElement(xmlInfo.DocumentElement, elementName, parentPath);
		}

		public static XmlElement CreateProviderElement(ModelComponent component, string elementName, string parentPath = null) {
			return CreateProviderElement(component.Element, elementName, parentPath);
		}

		/// <summary>
		/// Set/Get well known reference to this provider
		/// </summary>
		public string Ref {
			get {
				return GetAttribute("Ref");
			}

			set {
				if(value == null)
					Element.RemoveAttribute("Ref");
				else
					SetAttribute("Ref", value);
			}
		}
	}

	/// <summary>
	/// Represent a XML element that provide information about a font
	/// </summary>
	public class LayoutFontInfo : LayoutInfo {
		public LayoutFontInfo(ModelComponent component, String elementName) : base(component, elementName) {
		}

		public LayoutFontInfo(XmlElement containerElement, String elementPath) : base(containerElement, elementPath) {
		}

		public LayoutFontInfo(XmlElement element) {
			this.Element = element;
		}

		public LayoutFontInfo() {
		}

		public FontStyle Style {
			get {
				String	v = GetAttribute("Style");

				if(v == null)
					return FontStyle.Regular;
				else
					return (FontStyle)Enum.Parse(typeof(FontStyle), v);
			}

			set {
				SetAttribute("Style", value.ToString());
			}
		}

		public String Name {
			get {
				return GetAttribute("Name", "Arial");
			}

			set {
				SetAttribute("Name", value);
			}
		}

		public float Size {
			get {
				return (float)XmlConvert.ToDouble(GetAttribute("Size", "8"));
			}

			set {
				SetAttribute("Size", XmlConvert.ToString(value));
			}
		}

		public Font Font {
			get {
				return new Font(this.Name, this.Size, this.Style, GraphicsUnit.World);
			}

			set {
				this.Name = value.Name;
				this.Size = value.Size;
				this.Style = value.Style;
			}
		}

		public String Description {
			get {
				Font	f = this.Font;
				String	d = f.Name + " " + f.Size;

				if(f.Bold)
					d += ", Bold";
				if(f.Italic)
					d += ", Italic";
				if(f.Underline)
					d += ", Underline";

				if(this.Color != Color.Black)
					d += " (" + this.Color.Name + ")";

				return d;
			}
		}

		public Color Color {
			get {
				String	colorName = GetAttribute("Color");

				if(colorName == null)
					return Color.Black;
				else
					return Color.FromName(colorName);
			}

			set {
				SetAttribute("Color", value.Name);
			}
		}

		public override String ToString() {
			String	title = GetAttribute("Title");
			
			if(title == null)
				return Description;
			else
				return title + " (" + Description + ")";
		}
	}

	public enum LayoutDrawingAnchorPoint {
		Left, Right, Center
	};

	public enum LayoutDrawingSide {
		Top, Bottom, Left, Right, Center
	};

	/// <summary>
	/// Represents an element containing relative drawing position.
	/// </summary>
	public class LayoutPositionInfo : LayoutInfo {
		public LayoutPositionInfo(ModelComponent component, String elementPath) : base(component, elementPath) {
		}

		public LayoutPositionInfo(XmlElement containerElement ,String elementPath) : base(containerElement, elementPath) {
		}

		public LayoutPositionInfo(XmlElement element) : base(element) {
		}

		public LayoutPositionInfo() {
		}

		public int Distance {
			get {
				return XmlConvert.ToInt32(GetAttribute("Distance", "8"));
			}

			set {
				SetAttribute("Distance", XmlConvert.ToString(value));
			}
		}

		public int Width {
			get {
				return XmlConvert.ToInt32(GetAttribute("Width", "0"));
			}

			set {
				SetAttribute("Width", XmlConvert.ToString(value));
			}
		}

			public LayoutDrawingAnchorPoint AnchorPoint {
			get {
				return (LayoutDrawingAnchorPoint)Enum.Parse(typeof(LayoutDrawingAnchorPoint), GetAttribute("Anchor", "Center"));
			}

			set {
				SetAttribute("Anchor", value.ToString());
			}
		}

		public LayoutDrawingSide Side {
			get {
				return (LayoutDrawingSide)Enum.Parse(typeof(LayoutDrawingSide), GetAttribute("Side", "Bottom"));
			}

			set {
				SetAttribute("Side", value.ToString());
			}
		}

		public override String ToString() {
			return GetAttribute("Title", "No title");
		}

		/// <summary>
		/// Get the other "side" of a given side.
		/// </summary>
		/// <param name="side"></param>
		/// <returns>The other side</returns>
		static public LayoutDrawingSide GetAlternateLayoutDrawingSide(LayoutDrawingSide side) {
			switch(side) {
				case LayoutDrawingSide.Left:
					return LayoutDrawingSide.Right;

				case LayoutDrawingSide.Right:
					return LayoutDrawingSide.Left;

				case LayoutDrawingSide.Top:
					return LayoutDrawingSide.Bottom;

				case LayoutDrawingSide.Bottom:
					return LayoutDrawingSide.Top;

				default:
					throw new ArgumentException("Invalid relative position");
			}
		}

		/// <summary>
		/// Get the path for a default position definition based on a side
		/// </summary>
		/// <param name="side">A side</param>
		/// <returns>A XPath to a position element for this side</returns>
		static public String GetElementPathPositionPath(LayoutDrawingSide side) {
			String	positionStyleName = "Bottom";

			switch(side) {
				case LayoutDrawingSide.Bottom:
					positionStyleName = "Bottom";
					break;

				case LayoutDrawingSide.Top:
					positionStyleName = "Top";
					break;

				case LayoutDrawingSide.Left:
					positionStyleName = "Left";
					break;

				case LayoutDrawingSide.Right:
					positionStyleName = "Right";
					break;
			}

			return "//Position[@Ref=\"" + positionStyleName + "\"]";
		}

	}

	public class LayoutTextInfo : LayoutInfo {
		LayoutPositionInfo	positionProvider;
		LayoutFontInfo		fontProvider;

		public LayoutTextInfo(ModelComponent component, String elementName) : base(component, elementName) {
		}

		public LayoutTextInfo(ModelComponent component) : base(component, "Name") {
		}

		public LayoutTextInfo(XmlElement containerElement, String elementName) : base(containerElement, elementName) {
		}

		public LayoutTextInfo(XmlElement containerElement) : base(containerElement, "Name") {
		}

		public virtual String Name {
			get {
				if(Element == null || Element.ChildNodes.Count == 0)
					return "";
				return Element.FirstChild.Value;
			}

			set {
				XmlText	textNode = Element.OwnerDocument.CreateTextNode(value);

				if(Element.ChildNodes.Count > 0)
					Element.ReplaceChild(textNode, Element.FirstChild);
				else
					Element.AppendChild(textNode);
			}
		}

		public virtual String Text {
			get {
				return this.Name;
			}

			set {
				this.Name = value;
			}
		}

		public bool Visible {
			get {
				return XmlConvert.ToBoolean(GetAttribute("Visible", "0"));
			}

			set {
				SetAttribute("Visible", XmlConvert.ToString(value));
			}
		}

		public virtual String FontElementPath {
			get {
				return GetAttribute("Font", "//Font[@Ref=\"Default\"]");
			}

			set {
				SetAttribute("Font", value);
				fontProvider = null;
			}
		}

		public LayoutDrawingSide DefaultLayoutDrawingSide {
			get {
				LayoutDrawingSide	result = LayoutDrawingSide.Bottom;

				if(Component != null) {
					LayoutTrackComponent	track = null;

					if(Component.Spot != null)
						track = Component.Spot.Track;

					if(track != null) {
						if(track is LayoutStraightTrackComponent) {
							if(LayoutTrackComponent.IsDiagonal(track.ConnectionPoints[0], track.ConnectionPoints[1])) {
								LayoutComponentConnectionPoint	vertical = LayoutTrackComponent.IsVertical(track.ConnectionPoints[0]) ? track.ConnectionPoints[0] : track.ConnectionPoints[1];

								if(vertical == LayoutComponentConnectionPoint.T)
									result = LayoutDrawingSide.Top;
								else
									result = LayoutDrawingSide.Bottom;
							}
							else {
								if(LayoutTrackComponent.IsHorizontal(track.ConnectionPoints[0]))
									result = LayoutDrawingSide.Bottom;
								else
									result = LayoutDrawingSide.Left;
							}
						}
						else {
							LayoutTurnoutTrackComponent	t = track as LayoutTurnoutTrackComponent;

							if(t != null) {
								switch(t.Branch) {
									case LayoutComponentConnectionPoint.B:
										result = LayoutDrawingSide.Bottom;
										break;

									case LayoutComponentConnectionPoint.T:
										result = LayoutDrawingSide.Top;
										break;

									case LayoutComponentConnectionPoint.L:
										result = LayoutDrawingSide.Left;
										break;

									case LayoutComponentConnectionPoint.R:
										result = LayoutDrawingSide.Right;
										break;
								}
							}
						}
					}
				}

				return result;
			}
		}

		public virtual String PositionElementPath {
			get {
				return GetAttribute("Position", LayoutPositionInfo.GetElementPathPositionPath(DefaultLayoutDrawingSide));
			}

			set {
				SetAttribute("Position", value);
				positionProvider = null;
			}
		}

		public LayoutFontInfo FontProvider {
			get {
				if(fontProvider == null) {
					if(Component != null)
						fontProvider = new LayoutFontInfo(Component, FontElementPath);
					else
						fontProvider = new LayoutFontInfo(ContainerElement, FontElementPath);
				}
			
				return fontProvider;
			}
		}

		public LayoutPositionInfo PositionProvider {
			get {
				if(positionProvider == null) {
					if(Component != null)
						positionProvider = new LayoutPositionInfo(Component, PositionElementPath);
					else
						positionProvider = new LayoutPositionInfo(ContainerElement, PositionElementPath);
				}

				return positionProvider;
			}
		}
	}

	public class LayoutBlockBallon : IOperationState  {
		public enum TerminationReason {
			Hidden, AnotherBallon, Clicked, TrainDetected, Canceled,
		}

		TaskCompletionSource<TerminationReason> tcs = new TaskCompletionSource<TerminationReason>();

		public LayoutBlockBallon() {
			FillColor = Color.LightYellow;
			TextColor = Color.Black;
			RemoveOnClick = false;
			RemoveOnTrainDetected = false;
			FontSize = 13;
			CancellationToken = CancellationToken.None;
		}

		public LayoutBlockBallon(string text)
			: this() {
			Text = text;
		}

		internal void Remove(TerminationReason terminationReason) {
			if(terminationReason == TerminationReason.Canceled)
				tcs.TrySetCanceled();
			else
				tcs.TrySetResult(terminationReason);
		}

		public static bool IsDisplayed(LayoutBlockDefinitionComponent blockDefinition) {
			return LayoutModel.StateManager.OperationStates["SimpleBallon"].HasState(blockDefinition.Id);
		}

		public static LayoutBlockBallon Get(LayoutBlockDefinitionComponent blockDefinition) {
			return LayoutModel.StateManager.OperationStates["SimpleBallon"].Get<LayoutBlockBallon>(blockDefinition.Id);
		}

		public static void Remove(LayoutBlockDefinitionComponent blockDefinition, TerminationReason terminationReason) {
			LayoutBlockBallon ballon = Get(blockDefinition);

			if(ballon != null) {
				blockDefinition.EraseImage();
				ballon.Remove(terminationReason);
				LayoutModel.StateManager.OperationStates["SimpleBallon"].Remove(blockDefinition.Id);
				blockDefinition.Redraw();
			}
		}

		public static void Show(LayoutBlockDefinitionComponent blockDefinition, LayoutBlockBallon ballon) {
			PerOperationStates simpleBallons = LayoutModel.StateManager.OperationStates["SimpleBallon"];

			if(simpleBallons.HasState(blockDefinition.Id))
				Remove(blockDefinition, TerminationReason.AnotherBallon);

			if(ballon.CancellationToken.CanBeCanceled)
				ballon.CancellationToken.Register(
					() =>
					{
						if(LayoutBlockBallon.IsDisplayed(blockDefinition) && LayoutBlockBallon.Get(blockDefinition) == ballon)
							LayoutBlockBallon.Remove(blockDefinition, TerminationReason.Canceled);
					}
				);

			simpleBallons.Set(blockDefinition.Id, ballon);
			blockDefinition.Redraw();
		}

		public string Text { get; set; }
		public Color FillColor { get; set; }
		public Color TextColor { get; set; }
		public bool RemoveOnClick { get; set; }
		public bool RemoveOnTrainDetected { get; set; }
		public float FontSize { get; set; }
		public CancellationToken CancellationToken { get; set; }

		public Task<TerminationReason> Task {
			get { return tcs.Task; }
		}

		/// <summary>
		/// Convert ballon to its task, so one could do 'await Ballon'...
		/// </summary>
		/// <param name="ballon"></param>
		/// <returns></returns>
		public static implicit operator Task<TerminationReason>(LayoutBlockBallon ballon) {
			return ballon.Task;
		}
	}

	public class LayoutAddressInfo : LayoutTextInfo {
		public LayoutAddressInfo(ModelComponent component, String elementName) : base(component, elementName) {
		}

		public LayoutAddressInfo(ModelComponent component) : base(component, "Address") {
		}

		public LayoutAddressInfo(XmlElement containerElement, String elementName) : base(containerElement, elementName) {
		}

		public LayoutAddressInfo(XmlElement containerElement) : base(containerElement, "Address") {
		}

		public int Unit {
			get {
				return XmlConvert.ToInt32(GetAttribute("Unit", "0"));
			}

			set {
				SetAttribute("Unit", XmlConvert.ToString(value));
			}
		}

		public int Subunit {
			get {
				return XmlConvert.ToInt32(GetAttribute("Subunit", "0"));
			}

			set {
				SetAttribute("Subunit", XmlConvert.ToString(value));
			}
		}

		public bool HasSubunit {
			get {
				return Element.HasAttribute("Subunit");
			}

			set {
				if(value == false)
					Element.RemoveAttribute("Subunit");
				else {
					if(!Element.HasAttribute("Subunit"))
						SetAttribute("Subunit", "0");
				}
			}
		}

		public override string Text {
			get {
				return GetAttribute("Text", "***");
			}

			set {
				SetAttribute("Text", value);
			}
		}

		/// <summary>
		/// The position element path for the address, by default it is the alternate
		/// position for a possible name element
		/// </summary>
		public override String PositionElementPath {
			get {
				if(Element.HasAttribute("Position"))
					return base.PositionElementPath;
				else {
					LayoutDrawingSide	usedSide;

					if(Element.HasAttribute("ImageSide"))
						usedSide = (LayoutDrawingSide)Enum.Parse(typeof(LayoutDrawingSide), Element.GetAttribute("ImageSide"));
					else {
						// Check if this component has a 'Name' element. If it does,
						// position the address in a different position
						LayoutTextInfo		nameProvider;

						if(Component != null)
							nameProvider = new LayoutTextInfo(Component, "Name");
						else
							nameProvider = new LayoutTextInfo(ContainerElement, "Name");

						LayoutPositionInfo	namePositionProvider = nameProvider.PositionProvider;
					
						usedSide = namePositionProvider.Side;
					}

					return LayoutPositionInfo.GetElementPathPositionPath(LayoutPositionInfo.GetAlternateLayoutDrawingSide(usedSide));
				}
			}

			set {
				base.PositionElementPath = value;
			}
		}

		public override String FontElementPath {
			get {
				if(Element.HasAttribute("Font"))
					return base.FontElementPath;
				else
					return "//Font[@Ref=\"AddressFont\"]";
			}

			set {
				base.FontElementPath = value;
			}
		}

	}

	public class LayoutImageInfo : LayoutInfo {
		public LayoutImageInfo(ModelComponent component, String elementPath) : base(component, elementPath) {
		}

		public LayoutImageInfo(ModelComponent component) : base(component, "Image") {
		}

		public LayoutImageInfo(XmlElement containerElement, String elementPath) : base(containerElement, elementPath) {
		}

		public LayoutImageInfo(XmlElement element) : base(element) {
		}

		public enum ImageSizeUnit {
			Pixels,
			GridUnits
		}

		public enum ImageOriginMethod {
			Center,
			TopLeft
		}

		public enum ImageFillEffect {
			Stretch,
			Tile
		}

		public enum ImageHorizontalAlignment {
			Left, Center, Right
		}

		public enum ImageVerticalAlignment {
			Top, Middle, Bottom
		}

		public String ImageFile {
			get {
				return GetAttribute("ImageFile");
			}

			set {
				SetAttribute("ImageFile", value);
			}
		}

		public Size Size {
			get {
				Size	result = new Size();

				result.Width = XmlConvert.ToInt32(GetAttribute("Width", "-1"));
				result.Height = XmlConvert.ToInt32(GetAttribute("Height", "-1"));

				return result;
			}

			set {
				if(value.Width < 0)
					Element.RemoveAttribute("Width");
				else
					SetAttribute("Width", XmlConvert.ToString(value.Width));

				if(value.Height < 0)
					Element.RemoveAttribute("Height");
				else
					SetAttribute("Height", XmlConvert.ToString(value.Height));
			}
		}

		public ImageSizeUnit WidthSizeUnit {
			get {
				return (ImageSizeUnit)Enum.Parse(typeof(ImageSizeUnit), GetAttribute("WidthSizeUnit", "GridUnits"));
			}

			set {
				SetAttribute("WidthSizeUnit", value.ToString());
			}
		}

		public ImageSizeUnit HeightSizeUnit {
			get {
				return (ImageSizeUnit)Enum.Parse(typeof(ImageSizeUnit), GetAttribute("HeightSizeUnit", "GridUnits"));
			}

			set {
				SetAttribute("HeightSizeUnit", value.ToString());
			}
		}

		public Size Offset {
			get {
				return new Size(XmlConvert.ToInt32(GetAttribute("OffsetWidth", "0")),
					XmlConvert.ToInt32(GetAttribute("OffsetHeight", "0")));
			}

			set {
				SetAttribute("OffsetWidth", XmlConvert.ToString(value.Width));
				SetAttribute("OffsetHeight", XmlConvert.ToString(value.Height));
			}
		}

		public ImageOriginMethod OriginMethod {
			get {
				return (ImageOriginMethod)Enum.Parse(typeof(ImageOriginMethod), GetAttribute("OriginMethod", "TopLeft"));
			}

			set {
				SetAttribute("OriginMethod", value.ToString());
			}
		}

		public ImageHorizontalAlignment HorizontalAlignment {
			get {
				return (ImageHorizontalAlignment)Enum.Parse(typeof(ImageHorizontalAlignment), GetAttribute("HorizontalAlignment", "Left"));
			}
			
			set {
				SetAttribute("HorizontalAlignment", value.ToString());
			}
		}

		public ImageVerticalAlignment VerticalAlignment {
			get {
				return (ImageVerticalAlignment)Enum.Parse(typeof(ImageVerticalAlignment), GetAttribute("VerticalAlignment", "Top"));
			}
			
			set {
				SetAttribute("VerticalAlignment", value.ToString());
			}
		}

		public ImageFillEffect FillEffect {
			get {
				if(Element.HasAttribute("ImageFillEffect"))
					return (ImageFillEffect)Enum.Parse(typeof(ImageFillEffect), GetAttribute("ImageFillEffect"));
				else
					return ImageFillEffect.Stretch;
			}

			set {
				SetAttribute("ImageFillEffect", value.ToString());
			}
		}

		public RotateFlipType RotateFlipEffect {
			get {
				if(Element.HasAttribute("RotateFlipEffect"))
					return (RotateFlipType)Enum.Parse(typeof(RotateFlipType), GetAttribute("RotateFlipEffect", "RotateNoneFlipNone"));
				else
					return RotateFlipType.RotateNoneFlipNone;
			}

			set {
				SetAttribute("RotateFlipEffect", value.ToString());
			}
		}

		public String ImageCacheEventXml {
			get {
				return "<Effect Type='" + RotateFlipEffect.ToString() + "' />";
			}
		}
		
		public bool ImageError {
			get {
				return XmlConvert.ToBoolean(GetAttribute("ImageError", "false"));
			}

			set {
				if(value == false)
					Element.RemoveAttribute("ImageError");
				else
					SetAttribute("ImageError", XmlConvert.ToString(value));
			}
		}
	}

}

