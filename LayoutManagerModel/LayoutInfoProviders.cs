//using LayoutManager.Components;

using LayoutManager.Components;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

#nullable enable
namespace LayoutManager.Model {
    /// <summary>
    /// Base class for information providers. Information providers wrap XML element and add semantics to them.
    /// </summary>
    public class LayoutInfo : LayoutXmlWrapper {
        private const string A_Id = "ID";
        private bool elementInComponentDocument;
        private XmlElement? container;
        private ModelComponent? component;

        [System.ComponentModel.Browsable(false)]
        public string ElementPath => Ref != null ? "//" + Element.Name + "[@Ref=\"" + Ref + "\"]" : "//" + Element.Name + "[@ID=\"" + GetAttribute(A_Id) + "\"]";

        public ModelComponent? OptionalComponent => component;

        public ModelComponent Component {
            get => Ensure.NotNull<ModelComponent>(OptionalComponent, "component");

            set => component = value;
        }

        public XmlElement ContainerElement {
            get => container ?? throw new LayoutException("LayoutInfo - container is null");
            set => container = value;
        }

        /// <summary>
        /// True if the provider element was found in the component document, false otherwise.
        /// This property has meaning only if the provider element was found (Element is not null)
        /// </summary>
        [Browsable(false)]
        public bool FoundInComponentDocument => elementInComponentDocument;

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

        public LayoutInfo(ModelComponent component, string elementPath) {
            this.Component = component;
            SearchProviderElement(component.OptionalElement, elementPath);
        }

        public void SearchProviderElement(XmlElement? container, string elementPath) {
            if (container != null) {
                var elements = Ensure.NotNull<XmlNodeList>(container.SelectNodes(elementPath));

                OptionalElement = null;
                this.container = container;

                if (elements.Count == 1) {
                    OptionalElement = (XmlElement?)elements[0];
                    elementInComponentDocument = true;
                }
                else if (elements.Count > 1)
                    throw new ArgumentException("More than one provider element found in component");

                // If the element was not found in the component XML document, search the model document
                if (OptionalElement == null) {
                    elements = Ensure.NotNull<XmlNodeList>(LayoutModel.Instance.XmlInfo.DocumentElement.SelectNodes(elementPath));

                    if (elements.Count == 1) {
                        OptionalElement = (XmlElement?)elements[0];
                        elementInComponentDocument = false;
                    }
                    else if (elements.Count > 1)
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
            OptionalElement = null;
        }

        public LayoutInfo(string name) : base(name) {
        }

        private static void AttachNewElement(XmlElement container, String? parentPath, XmlElement element) {
            XmlElement parentElement = container;

            element.SetAttributeValue(A_Id, Guid.NewGuid());

            if (parentPath != null) {
                var parentNodes = container.SelectNodes(parentPath);

                if (parentNodes != null && parentNodes.Count > 0)
                    parentElement = (XmlElement)parentNodes[0]!;
                else {
                    int index = parentPath.IndexOf('/');
                    string parentName;
                    string? tail = null;

                    if (index < 0)
                        parentName = parentPath;
                    else {
                        parentName = parentPath[..index];
                        tail = parentPath[(index + 1)..];
                    }

                    parentElement = container.OwnerDocument.CreateElement(parentName);
                    container.AppendChild(parentElement);

                    if (!string.IsNullOrEmpty(tail)) {
                        AttachNewElement(parentElement, tail, element);
                        return;
                    }
                }
            }

            parentElement.AppendChild(element);
        }

        /// <summary>
        /// Create a new element for the component. The provider is based on this element
        /// </summary>
        /// <param name="container">The XML element in which to create the provider</param>
        /// <param name="elementName">The provider's element name</param>
        /// <param name="parentPath">The XPath to the new provider's element parent</param>
        public static XmlElement CreateProviderElement(XmlElement container, string elementName, string? parentPath = null) {
            XmlElement element = container.OwnerDocument.CreateElement(elementName);

            AttachNewElement(container, parentPath, element);
            return element;
        }

        public static XmlElement CreateProviderElement(LayoutXmlInfo xmlInfo, string elementName, string? parentPath = null) => CreateProviderElement(xmlInfo.DocumentElement, elementName, parentPath);

        public static XmlElement CreateProviderElement(ModelComponent component, string elementName, string? parentPath = null) => CreateProviderElement(component.Element, elementName, parentPath);

        /// <summary>
        /// Set/Get well known reference to this provider
        /// </summary>
        public string? Ref {
            get => (string?)AttributeValue("Ref");

            set => SetAttributValue("Ref", value, removeIf: null);
        }
    }

    /// <summary>
    /// Represent a XML element that provide information about a font
    /// </summary>
    public class LayoutFontInfo : LayoutInfo {
        private const string A_Style = "Style";
        private const string A_Name = "Name";
        private const string A_Size = "Size";
        private const string A_Color = "Color";
        private const string A_Title = "Title";

        public LayoutFontInfo(ModelComponent component, string elementName) : base(component, elementName) {
        }

        public LayoutFontInfo(XmlElement containerElement, string elementPath) : base(containerElement, elementPath) {
        }

        public LayoutFontInfo(XmlElement element) {
            this.Element = element;
        }

        public LayoutFontInfo() {
        }

        public FontStyle Style {
            get => OptionalElement?.AttributeValue(A_Style).Enum<FontStyle>() ?? FontStyle.Regular;
            set => SetAttributeValue(A_Style, value);
        }

        public string Name {
            get => (string?)OptionalElement?.AttributeValue(A_Name) ?? "Arial";
            set => SetAttributeValue(A_Name, value);
        }

        public float Size {
            get => (float?)OptionalElement?.AttributeValue(A_Size) ?? 8;
            set => SetAttributeValue(A_Size, value);
        }

        public Font Font {
            get => new(this.Name, this.Size, this.Style, GraphicsUnit.World);

            set {
                this.Name = value.Name;
                this.Size = value.Size;
                this.Style = value.Style;
            }
        }

        public string Description {
            get {
                Font f = this.Font;
                string d = f.Name + " " + f.Size;

                if (f.Bold)
                    d += ", Bold";
                if (f.Italic)
                    d += ", Italic";
                if (f.Underline)
                    d += ", Underline";

                if (this.Color != Color.Black)
                    d += " (" + this.Color.Name + ")";

                return d;
            }
        }

        public Color Color {
            get {
                var colorName = (string?)OptionalElement?.AttributeValue(A_Color);
                return colorName == null ? Color.Black : Color.FromName(colorName);
            }

            set => SetAttributeValue(A_Color, value.Name);
        }

        public override string ToString() {
            var title = (string?)OptionalElement?.AttributeValue(A_Title);

            return title == null ? Description : title + " (" + Description + ")";
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
        private const string A_Distance = "Distance";
        private const string A_Width = "Width";
        private const string A_Anchor = "Anchor";
        private const string A_Side = "Side";
        private const string A_Title = "Title";

        public LayoutPositionInfo(ModelComponent component, string elementPath) : base(component, elementPath) {
        }

        public LayoutPositionInfo(XmlElement containerElement, string elementPath) : base(containerElement, elementPath) {
        }

        public LayoutPositionInfo(XmlElement element) : base(element) {
        }

        public LayoutPositionInfo() {
        }

        public int Distance {
            get => (int?)OptionalElement?.AttributeValue(A_Distance) ?? 8;
            set => SetAttributeValue(A_Distance, value);
        }

        public int Width {
            get => (int?)OptionalElement?.AttributeValue(A_Width) ?? 0;
            set => SetAttributeValue(A_Width, value);
        }

        public LayoutDrawingAnchorPoint AnchorPoint {
            get => OptionalElement?.AttributeValue(A_Anchor).Enum<LayoutDrawingAnchorPoint>() ?? LayoutDrawingAnchorPoint.Center;
            set => SetAttributeValue(A_Anchor, value);
        }

        public LayoutDrawingSide Side {
            get => OptionalElement?.AttributeValue(A_Side).Enum<LayoutDrawingSide>() ?? LayoutDrawingSide.Bottom;
            set => SetAttributeValue(A_Side, value);
        }

        public override string ToString() => (string?)OptionalElement?.AttributeValue(A_Title) ?? "No title";

        /// <summary>
        /// Get the other "side" of a given side.
        /// </summary>
        /// <param name="side"></param>
        /// <returns>The other side</returns>
        static public LayoutDrawingSide GetAlternateLayoutDrawingSide(LayoutDrawingSide side) => side switch {
            LayoutDrawingSide.Left => LayoutDrawingSide.Right,
            LayoutDrawingSide.Right => LayoutDrawingSide.Left,
            LayoutDrawingSide.Top => LayoutDrawingSide.Bottom,
            LayoutDrawingSide.Bottom => LayoutDrawingSide.Top,
            _ => throw new ArgumentException("Invalid relative position")
        };

        /// <summary>
        /// Get the path for a default position definition based on a side
        /// </summary>
        /// <param name="side">A side</param>
        /// <returns>A XPath to a position element for this side</returns>
        static public string GetElementPathPositionPath(LayoutDrawingSide side) {
            var positionStyleName = side switch {
                LayoutDrawingSide.Top => "Top",
                LayoutDrawingSide.Left => "Left",
                LayoutDrawingSide.Right => "Right",
                _ => "Bottom",
            };

            return "//Position[@Ref=\"" + positionStyleName + "\"]";
        }
    }

    public class LayoutTextInfo : LayoutInfo {
        private const string A_Visible = "Visible";
        private LayoutPositionInfo? positionProvider;
        private LayoutFontInfo? fontProvider;

        public LayoutTextInfo(ModelComponent component, string elementName) : base(component, elementName) {
        }

        public LayoutTextInfo(ModelComponent component) : base(component, "Name") {
        }

        public LayoutTextInfo(XmlElement containerElement, string elementName) : base(containerElement, elementName) {
        }

        public LayoutTextInfo(XmlElement containerElement) : base(containerElement, "Name") {
        }

        public virtual string Name {
            get => (OptionalElement == null || Element.ChildNodes.Count == 0) ? "" : Element.FirstChild?.Value ?? "";

            set {
                XmlText textNode = Element.OwnerDocument.CreateTextNode(value);

                if (Element.ChildNodes.Count > 0)
                    Element.ReplaceChild(textNode, Element.FirstChild!);
                else
                    Element.AppendChild(textNode);
            }
        }

        public virtual string Text {
            get => this.Name;

            set => this.Name = value;
        }

        public bool Visible {
            get => (bool?)AttributeValue(A_Visible) ?? false;
            set => SetAttributeValue(A_Visible, value);
        }

        public virtual string FontElementPath {
            get => GetOptionalAttribute("Font") ?? "//Font[@Ref=\"Default\"]";

            set {
                SetAttributeValue("Font", value);
                fontProvider = null;
            }
        }

        public LayoutDrawingSide DefaultLayoutDrawingSide {
            get {
                LayoutDrawingSide result = LayoutDrawingSide.Bottom;

                if (OptionalComponent != null) {
                    LayoutTrackComponent? track = null;

                    if (Component.OptionalSpot != null)
                        track = Component.Spot.Track;

                    if (track != null) {
                        if (track is LayoutStraightTrackComponent) {
                            if (LayoutTrackComponent.IsDiagonal(track.ConnectionPoints[0], track.ConnectionPoints[1])) {
                                LayoutComponentConnectionPoint vertical = LayoutTrackComponent.IsVertical(track.ConnectionPoints[0]) ? track.ConnectionPoints[0] : track.ConnectionPoints[1];

                                if (vertical == LayoutComponentConnectionPoint.T)
                                    result = LayoutDrawingSide.Top;
                                else
                                    result = LayoutDrawingSide.Bottom;
                            }
                            else {
                                if (LayoutTrackComponent.IsHorizontal(track.ConnectionPoints[0]))
                                    result = LayoutDrawingSide.Bottom;
                                else
                                    result = LayoutDrawingSide.Left;
                            }
                        }
                        else {
                            if (track is LayoutTurnoutTrackComponent t) {
                                switch (t.Branch) {
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

        public virtual string PositionElementPath {
            get => GetOptionalAttribute("Position") ?? LayoutPositionInfo.GetElementPathPositionPath(DefaultLayoutDrawingSide);

            set {
                SetAttributeValue("Position", value);
                positionProvider = null;
            }
        }

        public LayoutFontInfo FontProvider {
            get {
                if (fontProvider == null) {
                    if (OptionalComponent != null)
                        fontProvider = new LayoutFontInfo(Component, FontElementPath);
                    else
                        fontProvider = new LayoutFontInfo(ContainerElement, FontElementPath);
                }

                return fontProvider;
            }
        }

        public LayoutPositionInfo PositionProvider {
            get {
                if (positionProvider == null) {
                    if (OptionalComponent != null)
                        positionProvider = new LayoutPositionInfo(Component, PositionElementPath);
                    else
                        positionProvider = new LayoutPositionInfo(ContainerElement, PositionElementPath);
                }

                return positionProvider;
            }
        }
    }

    public class LayoutBlockBalloon : IOperationState {
        public enum TerminationReason {
            Hidden, AnotherBalloon, Clicked, TrainDetected, Canceled,
        }

        private readonly TaskCompletionSource<TerminationReason> tcs = new();

        public LayoutBlockBalloon() {
            FillColor = Color.LightYellow;
            TextColor = Color.Black;
            RemoveOnClick = false;
            RemoveOnTrainDetected = false;
            FontSize = 13;
            CancellationToken = CancellationToken.None;
            Text = "(None)";
        }

        public LayoutBlockBalloon(string text)
            : this() {
            Text = text;
        }

        internal void Remove(TerminationReason terminationReason) {
            if (terminationReason == TerminationReason.Canceled)
                tcs.TrySetCanceled();
            else
                tcs.TrySetResult(terminationReason);
        }

        public static bool IsDisplayed(LayoutBlockDefinitionComponent blockDefinition) => LayoutModel.StateManager.OperationStates["SimpleBalloon"].HasState(blockDefinition.Id);

        public static LayoutBlockBalloon Get(LayoutBlockDefinitionComponent blockDefinition) =>
            Ensure.NotNull<LayoutBlockBalloon>(LayoutModel.StateManager.OperationStates["SimpleBalloon"].Get<LayoutBlockBalloon>(blockDefinition.Id), "BlockBalloon");

        public static void Remove(LayoutBlockDefinitionComponent blockDefinition, TerminationReason terminationReason) {
            LayoutBlockBalloon balloon = Get(blockDefinition);

            if (balloon != null) {
                blockDefinition.EraseImage();
                balloon.Remove(terminationReason);
                LayoutModel.StateManager.OperationStates["SimpleBalloon"].Remove(blockDefinition.Id);
                blockDefinition.Redraw();
            }
        }

        public static void Show(LayoutBlockDefinitionComponent blockDefinition, LayoutBlockBalloon balloon) {
            PerOperationStates simpleBalloons = LayoutModel.StateManager.OperationStates["SimpleBalloon"];

            if (simpleBalloons.HasState(blockDefinition.Id))
                Remove(blockDefinition, TerminationReason.AnotherBalloon);

            if (balloon.CancellationToken.CanBeCanceled)
                balloon.CancellationToken.Register(
                    () => {
                        if (LayoutBlockBalloon.IsDisplayed(blockDefinition) && LayoutBlockBalloon.Get(blockDefinition) == balloon)
                            LayoutBlockBalloon.Remove(blockDefinition, TerminationReason.Canceled);
                    }
                );

            simpleBalloons.Set(blockDefinition.Id, balloon);
            blockDefinition.Redraw();
        }

        public string Text { get; set; }
        public Color FillColor { get; set; }
        public Color TextColor { get; set; }
        public bool RemoveOnClick { get; set; }
        public bool RemoveOnTrainDetected { get; set; }
        public float FontSize { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public Task<TerminationReason> Task => tcs.Task;

        /// <summary>
        /// Convert balloon to its task, so one could do 'await Balloon'...
        /// </summary>
        /// <param name="balloon"></param>
        /// <returns></returns>
        public static implicit operator Task<TerminationReason>(LayoutBlockBalloon balloon) => balloon.Task;
    }

    public class LayoutAddressInfo : LayoutTextInfo {
        private const string E_Address = "Address";
        private const string A_Unit = "Unit";
        private const string A_Subunit = "Subunit";
        private const string A_Text = "Text";
        private const string A_Position = "Position";
        private const string A_ImageSide = "ImageSide";

        public LayoutAddressInfo(ModelComponent component, string elementName) : base(component, elementName) {
        }

        public LayoutAddressInfo(ModelComponent component) : base(component, E_Address) {
        }

        public LayoutAddressInfo(XmlElement containerElement, string elementName) : base(containerElement, elementName) {
        }

        public LayoutAddressInfo(XmlElement containerElement) : base(containerElement, E_Address) {
        }

        public int Unit {
            get => (int?)AttributeValue(A_Unit) ?? 0;
            set => SetAttributeValue(A_Unit, value);
        }

        public int Subunit {
            get => (int?)AttributeValue(A_Subunit) ?? 0;
            set => SetAttributeValue(A_Subunit, value);
        }

        public bool HasSubunit {
            get => Element.HasAttribute(A_Subunit);

            set {
                if (!value)
                    Element.RemoveAttribute(A_Subunit);
                else {
                    if (!Element.HasAttribute(A_Subunit))
                        SetAttributeValue(A_Subunit, 0);
                }
            }
        }

        public override string Text {
            get => GetOptionalAttribute(A_Text) ?? "***";
            set => SetAttributeValue(A_Text, value);
        }

        /// <summary>
        /// The position element path for the address, by default it is the alternate
        /// position for a possible name element
        /// </summary>
        public override string PositionElementPath {
            get {
                if (Element.HasAttribute(A_Position))
                    return base.PositionElementPath;
                else {
                    LayoutDrawingSide GetDefaultSide() {
                        // Check if this component has a 'Name' element. If it does,
                        // position the address in a different position
                        var nameProvider = Component != null ? new LayoutTextInfo(Component, "Name") : new LayoutTextInfo(ContainerElement, "Name");

                        return nameProvider.PositionProvider.Side;
                    }

                    LayoutDrawingSide usedSide = AttributeValue(A_ImageSide).Enum<LayoutDrawingSide>() ?? GetDefaultSide();
                    return LayoutPositionInfo.GetElementPathPositionPath(LayoutPositionInfo.GetAlternateLayoutDrawingSide(usedSide));
                }
            }

            set => base.PositionElementPath = value;
        }

        public override string FontElementPath {
            get {
                return Element.HasAttribute("Font") ? base.FontElementPath : "//Font[@Ref=\"AddressFont\"]";
            }

            set => base.FontElementPath = value;
        }
    }

    public class LayoutImageInfo : LayoutInfo {
        private const string E_Image = "Image";
        private const string A_ImageFile = "ImageFile";
        private const string A_Width = "Width";
        private const string A_Height = "Height";
        private const string A_WidthSizeUnit = "WidthSizeUnit";
        private const string A_HeightSideUnit = "HeightSizeUnit";
        private const string A_OffsetWidth = "OffsetWidth";
        private const string A_OffsetHeight = "OffsetHeight";
        private const string A_OriginMethod = "OriginMethod";
        private const string A_HorizontalAlignment = "HorizontalAlignment";
        private const string A_VerticalAlignment = "VerticalAlignment";
        private const string A_ImageFillEffect = "ImageFillEffect";
        private const string A_RotateFlipEffect = "RotateFlipEffect";
        private const string A_ImageError = "ImageError";

        public LayoutImageInfo(ModelComponent component, string elementPath) : base(component, elementPath) {
        }

        public LayoutImageInfo(ModelComponent component) : base(component, E_Image) {
        }

        public LayoutImageInfo(XmlElement containerElement, string elementPath) : base(containerElement, elementPath) {
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

        public string ImageFile {
            get => GetOptionalAttribute(A_ImageFile) ?? String.Empty;
            set => SetAttributeValue(A_ImageFile, value);
        }

        public Size Size {
            get => new() {
                Width = (int?)AttributeValue(A_Width) ?? -1,
                Height = (int?)AttributeValue(A_Height) ?? -1
            };

            set {
                SetAttributeValue(A_Width, value.Width, removeIf: -1);
                SetAttributeValue(A_Height, value.Height, removeIf: -1);
            }
        }

        public ImageSizeUnit WidthSizeUnit {
            get => AttributeValue(A_WidthSizeUnit).Enum<ImageSizeUnit>() ?? ImageSizeUnit.GridUnits;
            set => SetAttributeValue(A_WidthSizeUnit, value);
        }

        public ImageSizeUnit HeightSizeUnit {
            get => AttributeValue(A_HeightSideUnit).Enum<ImageSizeUnit>() ?? ImageSizeUnit.GridUnits;
            set => SetAttributeValue(A_HeightSideUnit, value);
        }

        public Size Offset {
            get => new((int?)AttributeValue(A_OffsetWidth) ?? 0, (int?)AttributeValue(A_OffsetHeight) ?? 0);

            set {
                SetAttributeValue(A_OffsetWidth, value.Width);
                SetAttributeValue(A_OffsetHeight, value.Height);
            }
        }

        public ImageOriginMethod OriginMethod {
            get => AttributeValue(A_OriginMethod).Enum<ImageOriginMethod>() ?? ImageOriginMethod.TopLeft;
            set => SetAttributeValue(A_OriginMethod, value);
        }

        public ImageHorizontalAlignment HorizontalAlignment {
            get => AttributeValue(A_HorizontalAlignment).Enum<ImageHorizontalAlignment>() ?? ImageHorizontalAlignment.Left;
            set => SetAttributeValue(A_HorizontalAlignment, value);
        }

        public ImageVerticalAlignment VerticalAlignment {
            get => AttributeValue(A_VerticalAlignment).Enum<ImageVerticalAlignment>() ?? ImageVerticalAlignment.Top;
            set => SetAttributeValue(A_VerticalAlignment, value);
        }

        public ImageFillEffect FillEffect {
            get => AttributeValue(A_ImageFillEffect).Enum<ImageFillEffect>() ?? ImageFillEffect.Stretch;
            set => SetAttributeValue(A_ImageFillEffect, value);
        }

        public RotateFlipType RotateFlipEffect {
            get => AttributeValue(A_RotateFlipEffect).Enum<RotateFlipType>() ?? RotateFlipType.RotateNoneFlipNone;
            set => SetAttributeValue(A_RotateFlipEffect, value);
        }

        public string ImageCacheEventXml => "<Effect Type='" + RotateFlipEffect.ToString() + "' />";

        public bool ImageError {
            get => (bool?)AttributeValue(A_ImageError) ?? false;
            set => SetAttributeValue(A_ImageError, value, removeIf: false);
        }
    }
}

