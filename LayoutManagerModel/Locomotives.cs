using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

#pragma warning disable IDE0051, IDE0060
#nullable enable
namespace LayoutManager.Model {
    #region Enumerations

    public enum LocomotiveKind {
        Steam, Diesel, Electric, SoundUnit
    };

    public enum LocomotiveOrigin {
        Europe, US
    };

    [Flags]
    public enum TrackGauges {
        Unknown = 0,
        G = 0x0001,
        I = 0x0002,
        O = 0x0004,
        Proto48 = 0x0008,
        S = 0x0010,
        OO = 0x0020,
        HO = 0x0040,
        TT = 0x0080,
        N = 0x0100,
        Z = 0x0200,

        G_or_I = G | I,
    }

    /// <summary>
    /// The orientation of a locomotive in a placeableItem. For example, F7-ABA placeable
    /// item will have three pleacable item members, oriented Forward, Forward, Backward.
    /// </summary>
    public enum LocomotiveOrientation {
        Forward, Backward, Unknown
    };

    [Flags]
    public enum TrainObjectDisplayNameFormat {
        Plain = 0x0000,                     // Just return the name (no adorments what so ever)
        IncludeAddress = 0x0001,            // Include locomotive(s) address(es)
        IncludeCollectionId = 0x0002,       // Include collection ID
    }

    #endregion

    #region Locomotive Catalog & Collection

    /// <summary>
    /// Represent a locomotive catalog storage elemenet. The locomotive catalog is built
    /// from several XML documents each containing locomotive type elements. This allow
    /// separation between locomotive types that are defined say by LGB to those defined
    /// by the user.
    /// </summary>
    public class LocomotiveStorageInfo : LayoutInfo {
        private const string A_Name = "Name";
        private const string A_File = "File";

        public LocomotiveStorageInfo(XmlElement element) : base(element) {
        }

        public string StorageName {
            get => Element.GetAttribute(A_Name);
            set => Element.SetAttribute(A_Name, value);
        }

        public string Filename {
            get => Element.GetAttribute(A_File);
            set => Element.SetAttribute(A_File, value);
        }
    }

    public abstract class LocomotiveCollectionBaseInfo : LayoutInfo {
        private const string E_Stores = "Stores";
        private const string A_DefaultStore = "DefaultStore";
        private const string A_File = "File";
        private const string A_Name = "Name";
        private const string A_Store = "Store";
        private const string A_Id = "ID";
        private LayoutXmlInfo? collection;
        private bool modified;
        private int loadCount;

        protected LocomotiveCollectionBaseInfo(XmlElement element) : base(element) {
        }

        public bool IsLoaded => collection != null;

        public XmlElement CollectionElement => Ensure.NotNull<LayoutXmlInfo>(collection, "collection").DocumentElement;

        public XmlDocument CollectionDocument => Ensure.NotNull<LayoutXmlInfo>(collection, "collection").XmlDocument;

        public abstract string CollectionElementName { get; }

        public abstract string CollectionName { get; }

        public abstract string DefaultStoreDirectory { get; }

        /// <summary>
        /// Get standard image to show for a locomotive of a given kind and a given origin
        /// </summary>
        /// <param name="kind">Locomotive kind</param>
        /// <param name="origin">Locomotive origin</param>
        /// <returns>The image to display</returns>
        public abstract Image? GetStandardImage(LocomotiveKind kind, LocomotiveOrigin origin);

        /// <summary>
        /// Get the path of a data directory subdirectory
        /// </summary>
        /// <param name="name">subdirectory name</param>
        /// <returns>Path to the data directory subdirectory</returns>
        /// <remarks>The subdirectory is created if it does not exist</remarks>
        protected string GetDataSubdirectoryPath(string name) {
            string path = Path.Combine(LayoutController.CommonDataFolderName, name);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// The default storage to which new locomotive type elements are added
        /// </summary>
        public int DefaultStore => (int)Element[E_Stores].AttributeValue(A_DefaultStore);

        public void Load() {
            if (collection == null) {
                collection = new LayoutXmlInfo();

                CollectionDocument.LoadXml("<" + CollectionElementName + "/>");

                // Load the catalog from its stores
                int iStore = 0;

                foreach (XmlElement storeElement in Element[E_Stores]) {
                    string filename = LayoutAssembly.ValueToFilePath(storeElement.GetAttribute(A_File), DefaultStoreDirectory);

                    Debug.WriteLine("-Loading collection from store: '" + storeElement.GetAttribute(A_Name) + "' from file: " + storeElement.GetAttribute(A_File));

                    try {
                        XmlTextReader r = new XmlTextReader(filename) {
                            WhitespaceHandling = WhitespaceHandling.None
                        };

                        // Skip all kind of declaration stuff
                        while (r.Read() && r.NodeType == XmlNodeType.XmlDeclaration)
                            ;

                        r.Read();       // <`CollectionElementName`>

                        while (r.NodeType == XmlNodeType.Element) {
                            XmlElement newNode = (XmlElement)CollectionDocument.ReadNode(r);

                            newNode.SetAttribute(A_Store, iStore);
                            CollectionElement.AppendChild(newNode);
                        }

                        iStore++;
                        r.Close();
                    }
                    catch (IOException ex) {
                        EventManager.Event(new LayoutEvent("add-error", null, $"Unable to load {CollectionName} file: {filename} - {ex.Message}"));
                    }
                }

                // TODO: Flag types which are used by locomotive in the collection, and types which are used
                // by locomotives that are on the layout

                // Catch event of modifying the catalog, this will cause the catalog to be saved if it is modified
                CollectionDocument.NodeChanged += this.OnCollectionNodeChanged;
                modified = false;
            }

            loadCount++;
        }

        private void OnCollectionNodeChanged(Object sender, XmlNodeChangedEventArgs e) {
            // TODO: Handle the case in which the type is used by a locomotive in the collection

            modified = true;
        }

        public void Save() {
            int iStore = 0;

            // TODO: Save only stores which are actually modified

            foreach (XmlElement storeElement in Element[E_Stores]) {
                Debug.WriteLine("-Saving collection in store: '" + storeElement.GetAttribute(A_Name) + "' to file: " + storeElement.GetAttribute(A_File));

                XmlNodeList elementsInStore = CollectionElement.SelectNodes("*[@Store='" + iStore + "']");

                try {
                    string filename = LayoutAssembly.ValueToFilePath(storeElement.GetAttribute(A_File), DefaultStoreDirectory);
                    XmlTextWriter w = new XmlTextWriter(filename, new System.Text.UTF8Encoding());

                    w.WriteStartDocument();
                    w.WriteStartElement(CollectionElementName);

                    foreach (XmlElement collectionElement in elementsInStore)
                        collectionElement.WriteTo(w);

                    w.WriteEndElement();
                    w.WriteEndDocument();

                    w.Close();
                }
                catch (IOException ex) {
                    EventManager.Event(new LayoutEvent("add-error", null,
                        "Cannot store locomotives store '" + storeElement.GetAttribute(A_Name) + "' in file: " + storeElement.GetAttribute(A_File) +
                        " - " + ex.Message));
                }

                iStore++;
            }

            modified = false;
        }

        public void Unload() {
            if (--loadCount == 0) {
                if (modified)
                    Save();

                collection = null;
            }
        }

        public XmlElement CreateCollectionElement() {
            Debug.Assert(IsLoaded, "New collection elements can be created only if the collection is loaded");

            XmlElement collectionElement = CollectionDocument.CreateElement(CollectionElementName);

            collectionElement.SetAttribute(A_Store, DefaultStore);
            collectionElement.SetAttribute(A_Id, Guid.NewGuid());
            return collectionElement;
        }

        public XmlElement this[string name] => (XmlElement)CollectionElement.SelectSingleNode("*/[./Name='" + name + "']");

        public XmlElement this[string elementKind, string name] => (XmlElement)CollectionElement.SelectSingleNode(elementKind + "[./Name='" + name + "']");
    }

    /// <summary>
    /// Represent the locomotive catalog element in the model document
    /// </summary>
    /// <remarks>
    /// The catalog can be either loaded (in which case the locomotive type elements are
    /// read from disk and are held in memory), or unloaded. Usually the catalog is unloaded.
    /// It is loaded only when locomotive type elements need to be accessed.
    /// </remarks>
    public class LocomotiveCatalogInfo : LocomotiveCollectionBaseInfo {
        private const string E_LocomotiveCatalog = "LocomotiveCatalog";
        private const string E_CommonFunctionNames = "CommonFunctionNames";
        private const string E_Images = "Images";
        private const string E_Image = "Image";
        private const string A_Kind = "Kind";
        private const string A_Origin = "Origin";

        public LocomotiveCatalogInfo() : base(LayoutModel.Instance.XmlInfo.DocumentElement[E_LocomotiveCatalog]) {
            if (Element == null) {
                Element = LayoutInfo.CreateProviderElement(LayoutModel.Instance.XmlInfo, E_LocomotiveCatalog, null);

                Element.InnerXml = "<Stores DefaultStore='0'><Store Name='Standard Locomotives' File='" +
                    Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.ProductName +
                    Path.DirectorySeparatorChar + "StandardLocomotives.LocomotiveCatalog' /></Stores><Images />";
            }
        }

        public override string CollectionElementName => "LocomotiveTypes";

        public override string CollectionName => "Locomotive Catalog";

        public override string DefaultStoreDirectory => GetDataSubdirectoryPath("Catalog");

        /// <summary>
        /// Common locotive function names
        /// </summary>
        public XmlElement LocomotiveFunctionNames {
            get {
                XmlElement e = Element[E_CommonFunctionNames];

                if (e == null) {
                    e = Element.OwnerDocument.CreateElement(E_CommonFunctionNames);
                    Element.AppendChild(e);
                }

                return e;
            }
        }

        /// <summary>
        /// Get standard image to show for a locomotive of a given kind and a given origin
        /// </summary>
        /// <param name="kind">Locomotive kind</param>
        /// <param name="origin">Locomotive origin</param>
        /// <returns>The image to display</returns>
        public override Image? GetStandardImage(LocomotiveKind kind, LocomotiveOrigin origin) {
            XmlElement imagesElement = Element[E_Images];
            XmlElement imageElement = (XmlElement)imagesElement.SelectSingleNode("Image[@Kind='" + kind.ToString() + "' and @Origin='" + origin.ToString() + "']");

            return imageElement != null ? Image.FromStream(new MemoryStream(Convert.FromBase64String(imageElement.InnerText))) : null;
        }

        public void SetStandardImage(LocomotiveKind kind, LocomotiveOrigin origin, Image image) {
            XmlElement imagesElement = Element[E_Images];
            XmlElement imageElement = (XmlElement)imagesElement.SelectSingleNode("Image[@Kind='" + kind.ToString() + "' and @Origin='" + origin.ToString() + "']");

            if (image != null) {
                if (imageElement == null) {
                    imageElement = Element.OwnerDocument.CreateElement(E_Image);
                    imageElement.SetAttribute(A_Kind, kind);
                    imageElement.SetAttribute(A_Origin, origin);
                    imagesElement.AppendChild(imageElement);
                }

                using MemoryStream s = new MemoryStream();
                image.Save(s, image.RawFormat);
                imageElement.InnerText = Convert.ToBase64String(s.GetBuffer());
            }
            else {
                if (imageElement != null)
                    imageElement.ParentNode.RemoveChild(imageElement);
            }
        }
    }

    /// <summary>
    /// Represent the locomotive collection in the model document
    /// </summary>
    public class LocomotiveCollectionInfo : LocomotiveCollectionBaseInfo {
        private const string E_Locomotive = "Locomotive";
        private const string E_Train = "Train";

        public LocomotiveCollectionInfo() : base(LayoutModel.Instance.XmlInfo.DocumentElement["LocomotiveCollection"]) {
            if (Element == null) {
                Element = LayoutInfo.CreateProviderElement(LayoutModel.Instance.XmlInfo, "LocomotiveCollection", null);

                Element.InnerXml = "<Stores DefaultStore='0'><Store Name='My collection' File='" +
                    Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) +
                    Path.DirectorySeparatorChar + System.Windows.Forms.Application.ProductName +
                    Path.DirectorySeparatorChar + "MyCollection.LocomotiveCollection' /></Stores>";
            }
        }

        public override string CollectionElementName => "Locomotives";

        public override string CollectionName => "Locomotive Collection";

        public override string DefaultStoreDirectory => GetDataSubdirectoryPath("Collection");

        public XmlElement this[Guid id] => (XmlElement)CollectionElement.SelectSingleNode("*[@ID='" + id.ToString() + "']");

        public override Image? GetStandardImage(LocomotiveKind kind, LocomotiveOrigin origin) {
            LocomotiveCatalogInfo catalog = LayoutModel.LocomotiveCatalog;

            return catalog.GetStandardImage(kind, origin);
        }

        [LayoutEvent("locomotive-type-updated")]
        private void locomotiveTypeUpdated(LayoutEvent e) {
            var element = Ensure.NotNull<XmlElement>(e.Sender, "element");
            LocomotiveTypeInfo locoType = new LocomotiveTypeInfo(element);
            XmlNodeList locoWithTypeElements = CollectionElement.SelectNodes($"Locomotive[@TypeID='{locoType.Id}']");

            foreach (XmlElement locoWithTypeElement in locoWithTypeElements) {
                LocomotiveInfo loco = new LocomotiveInfo(locoWithTypeElement);

                if (loco.LinkedToType) {
                    Debug.WriteLine("Updating type information of locomotive " + loco.DisplayName);
                    loco.UpdateFromLocomotiveType(locoType);
                    e.Info = true;
                }
            }

            if ((bool)(e.Info ?? false))
                Save();
        }

        /// <summary>
        /// Check that all members of locotive set actually exist. If a member that is not
        /// defined is found, it is removed from the locomotive set, if as a result, the
        /// locomotive set becomes empty, the locomotive set is removed
        /// </summary>
        /// <param name="eventManager"></param>
        public bool EnsureReferentialIntegrity() {
            bool modified = false;
            var trainsToRemove = new List<TrainInCollectionInfo>();

            foreach (XmlElement trainElement in CollectionElement.GetElementsByTagName(E_Train)) {
                TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(trainElement);
                var membersToRemove = new List<TrainLocomotiveInfo>();

                foreach (TrainLocomotiveInfo trainLocomotive in trainInCollection.Locomotives) {
                    if (this[trainLocomotive.LocomotiveId] == null) {
                        LayoutModuleBase.Warning("Removing a locomotive from a locomotive set '" + trainInCollection.DisplayName + "' - Locomotive definition not found");
                        membersToRemove.Add(trainLocomotive);
                        modified = true;
                    }
                    else {
                        LocomotiveInfo loco = new LocomotiveInfo(LayoutModel.LocomotiveCollection[trainLocomotive.LocomotiveId]);

                        if (loco.NotManaged)        // Do not allow non managed locomotive in a locomotive set
                            membersToRemove.Add(trainLocomotive);
                    }
                }

                foreach (TrainLocomotiveInfo trainLocomotive in membersToRemove)
                    trainInCollection.RemoveLocomotive(trainLocomotive);

                if (trainInCollection.Locomotives.Count == 0) {
                    LayoutModuleBase.Warning("Deleting locomotive set '" + trainInCollection.DisplayName + "' which has became empty");
                    trainsToRemove.Add(trainInCollection);
                    modified = true;
                }
            }

            foreach (TrainInCollectionInfo trainInCollection in trainsToRemove)
                CollectionElement.RemoveChild(trainInCollection.Element);

            return modified;
        }

        /// <summary>
        /// Return the ID of the locomotive or locomotive set represented by element
        /// </summary>
        /// <param name="element">Locomotive or Locomotive set element</param>
        /// <returns>The element GUID</returns>
        public Guid GetElementId(XmlElement element) {
            if (element.Name == E_Locomotive) {
                LocomotiveInfo loco = new LocomotiveInfo(element);

                return loco.Id;
            }
            else if (element.Name == E_Train) {
                TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(element);

                return trainInCollection.Id;
            }
            else
                throw new ArgumentException("Invalid locomotive/train element");
        }
    }

    #endregion

    #region LayoutNamedTrainObject

    abstract public class LayoutNamedTrainObject : LayoutXmlWrapper {
        protected LayoutNamedTrainObject(XmlElement element)
            : base(element) {
        }

        public abstract string GetDisplayName(TrainObjectDisplayNameFormat format);

        public virtual TrainObjectDisplayNameFormat DefaultDisplayNameFormat => TrainObjectDisplayNameFormat.IncludeAddress | TrainObjectDisplayNameFormat.IncludeCollectionId;

        public string DisplayName => GetDisplayName(DefaultDisplayNameFormat);
    }

    #endregion

    #region Locomotive Type

    /// <summary>
    /// Represent a locomotive type as stored in the locomotive catalog
    /// </summary>
    public class LocomotiveTypeInfo : LayoutNamedTrainObject, IObjectHasName, IObjectHasAttributes, IHasDecoder {
        private const string E_TypeName = "TypeName";
        private const string E_Attributes = "Attributes";
        private const string E_Image = "Image";
        private const string E_Functions = "Functions";
        private const string A_Kind = "Kind";
        private const string A_Length = "Length";
        private const string A_Guage = "Guage";
        private const string A_SpeedLimit = "SpeedLimit";
        private const string A_Origin = "Origin";
        private const string A_Store = "Store";
        private const string A_Light = "Light";
        private const string A_DecoderType = "DecoderType";

        public LocomotiveTypeInfo(XmlElement element) : base(element) {
        }

        public string TypeName {
            get => Element[E_TypeName] != null ? Element[E_TypeName].InnerText : "";

            set {
                if (Element[E_TypeName] == null)
                    Element.AppendChild(Element.OwnerDocument.CreateElement(E_TypeName));
                Element[E_TypeName].InnerText = value;
            }
        }

        public LayoutTextInfo NameProvider => new LayoutTextInfo(Element, E_TypeName);

        public override string GetDisplayName(TrainObjectDisplayNameFormat format) => NameProvider.Name;

        public LocomotiveKind Kind {
            get => AttributeValue(A_Kind).Enum<LocomotiveKind>() ?? LocomotiveKind.Steam;
            set => Element.SetAttribute(A_Kind, value.ToString());
        }

        public double Length {
            get => (int?)AttributeValue(A_Length) ?? 60;
            set => SetAttribute(A_Length, value);
        }

        public TrackGauges Guage {
            get => AttributeValue(A_Guage).Enum<TrackGauges>() ?? TrackGauges.HO;
            set => SetAttribute(A_Guage, value);
        }

        public bool HasAttributes => Element[E_Attributes] != null;

        /// <summary>
        /// User attributes associated with this locomotive type
        /// </summary>
        public AttributesInfo Attributes {
            get {
                XmlElement attributesElement = Element[E_Attributes];

                if (attributesElement == null) {
                    attributesElement = Element.OwnerDocument.CreateElement(E_Attributes);

                    Element.AppendChild(attributesElement);
                }

                return new AttributesInfo(attributesElement);
            }
        }

        public int SpeedLimit {
            get => (int?)AttributeValue(A_SpeedLimit) ?? 0;
            set => SetAttribute(A_SpeedLimit, value, removeIf: 0);
        }

        public LocomotiveOrigin Origin {
            get => AttributeValue(A_Origin).Enum<LocomotiveOrigin>() ?? LocomotiveOrigin.Europe;
            set => Element.SetAttribute(A_Origin, value);
        }

        public Image? Image {
            set {
                if (value == null) {
                    XmlNode imageNode = Element[E_Image];

                    if (imageNode != null)
                        Element.RemoveChild(imageNode);
                }
                else {
                    using MemoryStream s = new MemoryStream();
                    value.Save(s, System.Drawing.Imaging.ImageFormat.Bmp);

                    if (Element[E_Image] == null)
                        Element.AppendChild(Element.OwnerDocument.CreateElement(E_Image));
                    Element[E_Image].InnerText = Convert.ToBase64String(s.GetBuffer());
                }
            }

            get {
                if (Element[E_Image] == null)
                    return null;
                else {
                    MemoryStream s = new MemoryStream(Convert.FromBase64String(Element[E_Image].InnerText));
                    return Image.FromStream(s);
                }
            }
        }

        public int Store {
            get => (int?)AttributeValue(A_Store) ?? 0;
            set => SetAttribute(A_Store, value);
        }

        public XmlElement Functions => Element[E_Functions];

        public LocomotiveFunctionInfo? GetFunctionByName(String functionName) {
            if (Functions != null) {
                XmlElement functionElement = (XmlElement)Functions.SelectSingleNode("Function[@Name = '" + functionName + "' ]");

                return functionElement == null ? null : new LocomotiveFunctionInfo(functionElement);
            }
            else
                return null;
        }

        public LocomotiveFunctionInfo? GetFunctionByNumber(int functionNumber) {
            if (Functions != null) {
                XmlElement functionElement = (XmlElement)Functions.SelectSingleNode("Function[@Number = " + functionNumber + "]");

                return functionElement == null ? null : new LocomotiveFunctionInfo(functionElement);
            }
            else
                return null;
        }

        public bool HasLights => (bool?)AttributeValue(A_Light) ?? false;

        /// <summary>
        /// Does this locomotive type has a built-in decoder. If it does, then decoder type name is the
        /// type of the built in decoder.
        /// </summary>
        public bool HasBuiltInDecoder => HasAttribute(A_DecoderType);

        /// <summary>
        /// Decoder type name (if locomotive has built in decoder) or default decoder type for this type of locomotive
        /// </summary>
        public string? DecoderTypeName {
            get => GetOptionalAttribute(A_DecoderType);       // TODO: Get default decoder type from the catalog
            set => SetAttribute(A_DecoderType, value, removeIf: null);
        }

        /// <summary>
        /// Decoder type object for the decoder used by this locomotive (type) or null if no decoder is defined
        /// </summary>
        public DecoderTypeInfo DecoderType => DecoderTypeInfo.GetDecoderType(DecoderTypeName);
    }

    public enum LocomotiveFunctionType {
        Trigger,
        OnOff
    };

    public class LocomotiveFunctionInfo : LayoutInfo {
        private const string A_Number = "Number";
        private const string A_Type = "Type";
        private const string A_Name = "Name";
        private const string A_Description = "Description";
        private const string A_State = "State";

        public LocomotiveFunctionInfo(XmlElement element) : base(element) {
        }

        public int Number {
            get => (int?)AttributeValue(A_Number) ?? 1;
            set => SetAttribute(A_Number, value);
        }

        public LocomotiveFunctionType Type {
            get => AttributeValue(A_Type).Enum<LocomotiveFunctionType>() ?? LocomotiveFunctionType.Trigger;
            set => SetAttribute(A_Type, value);
        }

        public string Name {
            get => GetOptionalAttribute(A_Name) ?? "";
            set => SetAttribute(A_Name, value);
        }

        public String? Description {
            get => GetOptionalAttribute(A_Description);
            set => SetAttribute(A_Description, value);
        }

        public bool State {
            get => (bool?)AttributeValue(A_State) ?? false;
            set => SetAttribute(A_State, value);
        }

        public override string ToString() {
            string description = !string.IsNullOrEmpty(this.Description) ? $" - {this.Description}" : "";
            return $"{this.Name}{description}";
        }
    }

    #endregion

    #region Locomotive

    /// <summary>
    /// Represent a locomotive in the locomotive collection. 
    /// </summary>
    public class LocomotiveInfo : LocomotiveTypeInfo, IObjectHasName, IObjectHasAddress {
        private const string E_Name = "Name";
        private const string E_TypeName = "TypeName";
        private const string A_TypeId = "TypeID";
        private const string A_LinkedToType = "LinkedToType";
        private const string A_CollectionId = "CollectionID";
        private const string A_SpeedSteps = "SpeedSteps";
        private const string A_NotManaged = "NotManaged";
        private const string A_CanTriggerTrackContact = "CanTriggerTrackContact";
        private const string E_Locomotive = "Locomotive";
        private const string E_Train = "Train";
        private const string A_Id = "ID";

        public LocomotiveInfo(XmlElement element) : base(element) {
        }

        /// <summary>
        /// The locomotive type from which this locomotive definition originates
        /// </summary>
        public Guid TypeId {
            get => (Guid)AttributeValue(A_TypeId);
            set => SetAttribute(A_TypeId, value);
        }

        /// <summary>
        /// Should the locomotive definition be automatically updated when the locomotive
        /// type on which it is based is updated
        /// </summary>
        public bool LinkedToType {
            get => (bool?)AttributeValue(A_LinkedToType) ?? true;
            set => SetAttribute(A_LinkedToType, value);
        }

        public string Name {
            get {
                XmlElement nameElement = Element[E_Name];

                return nameElement != null ? nameElement.InnerText : "";
            }

            set {
                XmlElement nameElement = Element[E_Name];

                if (nameElement == null) {
                    nameElement = Element.OwnerDocument.CreateElement(E_Name);
                    Element.AppendChild(nameElement);
                }
                nameElement.InnerText = value;
            }
        }

        public override string GetDisplayName(TrainObjectDisplayNameFormat format) {
            string name = this.Name;

            if ((format & TrainObjectDisplayNameFormat.IncludeCollectionId) != 0 && !string.IsNullOrEmpty(CollectionId))
                name += " [" + CollectionId + "]";

            if ((format & TrainObjectDisplayNameFormat.IncludeAddress) != 0 && AddressProvider != null && AddressProvider.Unit > 0)
                name += " (" + AddressProvider.Unit.ToString() + ")";

            return name;
        }

        public new LayoutTextInfo NameProvider => new LayoutTextInfo(Element, E_Name);

        public LayoutTextInfo TypeNameProvider => new LayoutTextInfo(Element, E_TypeName);

        public LayoutAddressInfo AddressProvider => new LayoutAddressInfo(Element);

        public string CollectionId {
            get => GetOptionalAttribute(A_CollectionId) ?? "";
            set => SetAttribute(A_CollectionId, value);
        }

        public int SpeedSteps {
            get => (int?)AttributeValue(A_SpeedSteps) ?? 28;
            set => SetAttribute(A_SpeedSteps, value);
        }

        /// <summary>
        /// Is this locomotive managed by the software or not. For example, if this is a life steam
        /// engine without any type of computer control (that can be used by this software) then it
        /// is non-managed locomotive. If such a locomotive trigger track BLOCK_EDGEs, then the software
        /// can still track its location.
        /// </summary>
        public bool NotManaged {
            get => (bool?)AttributeValue(A_NotManaged) ?? false;
            set => SetAttribute(A_NotManaged, value, removeIf: false);
        }

        /// <summary>
        /// Return true is this locomotive can trigger track contact (i.e. it has a track magnet)
        /// </summary>
        public bool CanTriggerTrackContact {
            get => (bool?)AttributeValue(A_CanTriggerTrackContact) ?? Kind != LocomotiveKind.SoundUnit;
            set => SetAttribute(A_CanTriggerTrackContact, value);
        }

        public LayoutActionContainer<LocomotiveInfo> Actions => new LayoutActionContainer<LocomotiveInfo>(Element, this);

        /// <summary>
        /// Check if this locomotive has compatible operation characteristics as of another locomotive or
        /// to those of all the members in a locomotive set. All members in a locomotive set must have
        /// compatible opertion characteristics.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool IsCompatibleOperationCharacteristics(XmlElement element) {
            if (element.Name == E_Locomotive) {
                LocomotiveInfo otherLoco = new LocomotiveInfo(element);

                return SpeedSteps == otherLoco.SpeedSteps && Guage == otherLoco.Guage;
            }
            else if (element.Name == E_Train) {
                TrainInCollectionInfo trainInCollection = new TrainInCollectionInfo(element);

                foreach (TrainLocomotiveInfo trainLocomotive in trainInCollection.Locomotives) {
                    XmlElement locoElement = LayoutModel.LocomotiveCollection[trainLocomotive.LocomotiveId];

                    if (!IsCompatibleOperationCharacteristics(locoElement))
                        return false;
                }

                return true;
            }
            else
                throw new ArgumentException("Invalid element (not locomotive or Train)");
        }

        public void UpdateFromLocomotiveType(LocomotiveTypeInfo locoType) {
            foreach (XmlElement typeElement in locoType.Element) {
                XmlElement locoElement = (XmlElement)Element.OwnerDocument.ImportNode(typeElement, true);

                if (Element[locoElement.Name] != null)
                    Element.RemoveChild(Element[locoElement.Name]);

                Element.AppendChild(locoElement);
            }

            foreach (XmlAttribute a in locoType.Element.Attributes) {
                string name = a.Name;

                if (name == A_Id)
                    name = A_TypeId;

                if (Element.HasAttribute(name))
                    Element.RemoveAttribute(name);

                Element.SetAttribute(name, a.Value);
            }
        }

        /// <summary>
        /// Get lowest address that can be programmed for this locomotive
        /// </summary>
        /// <param name="commandStation">Optional command station that will be used to drive this locomotive</param>
        /// <returns>Lowest locomotive address</returns>
        public int GetLowestAddress(IModelComponentIsCommandStation commandStation) {
            if (DecoderType is DecoderWithNumericAddressTypeInfo decoderType) {
                return commandStation == null
                    ? decoderType.LowestAddress
                    : Math.Max(decoderType.LowestAddress, commandStation.GetLowestLocomotiveAddress(decoderType.SupportedDigitalPowerFormats));
            }
            else
                return -1;
        }

        /// <summary>
        /// Get highest address that can be programmed for this locomotive
        /// </summary>
        /// <param name="commandStation">Optional command station that will be used to drive this locomotive</param>
        /// <returns>Highest locomotive address</returns>
        public int GetHighestAddress(IModelComponentIsCommandStation commandStation) {
            if (DecoderType is DecoderWithNumericAddressTypeInfo decoderType) {
                return commandStation == null
                    ? decoderType.HighestAddress
                    : Math.Min(commandStation.GetHighestLocomotiveAddress(decoderType.SupportedDigitalPowerFormats), decoderType.HighestAddress);
            }
            else
                return -1;
        }
    }

    #endregion

    #region Saved Train Info and Train info base class (base class for saved train and active train)

    #region Objects referenced by train. Locomotivs & Cars

    public class TrainLocomotiveInfo : LayoutInfo {
        private const string A_LocomotiveID = "LocomotiveID";
        private const string A_Orientation = "Orientation";
        private const string A_CollectionElementId = "CollectionElementID";

        public TrainCommonInfo Train { get; }

        internal TrainLocomotiveInfo(TrainCommonInfo train, XmlElement element) : base(element) {
            this.Train = train;
        }

        public Guid LocomotiveId {
            get => (Guid)AttributeValue(A_LocomotiveID);
            set => SetAttribute(A_LocomotiveID, value);
        }

        public LocomotiveInfo? OptionalLocomotive {
            get {
                XmlElement element = LayoutModel.LocomotiveCollection[LocomotiveId];

                return element == null ? null : new LocomotiveInfo(element);
            }
        }

        public LocomotiveInfo Locomotive => Ensure.NotNull<LocomotiveInfo>(OptionalLocomotive, nameof(OptionalLocomotive));

        public LocomotiveOrientation Orientation {
            get => AttributeValue(A_Orientation).Enum<LocomotiveOrientation>() ?? LocomotiveOrientation.Forward;
            set => SetAttribute(A_Orientation, value);
        }

        public Guid CollectionElementId {
            get => (Guid?)AttributeValue(A_CollectionElementId) ?? LocomotiveId;
            set => SetAttribute(A_CollectionElementId, value);
        }

        // The following properties are for allowing easy access from event scripts
        public string Name => Locomotive.Name;

        public string TypeName => Locomotive.TypeName;

        public string CollectionId => Locomotive.CollectionId;
    }

    public class TrainCarsInfo : LayoutInfo {
        private const string A_Count = "Count";
        private const string A_Description = "Description";
        private const string A_CarLength = "CarLength";

        public TrainCarsInfo(TrainCommonInfo train, XmlElement element) : base(element) {
            this.Train = train;
        }

        public int Count {
            get => (int?)AttributeValue(A_Count) ?? 1;
            set => SetAttribute(A_Count, value);
        }

        public string Description {
            get => GetOptionalAttribute(A_Description) ?? "";
            set => SetAttribute(A_Description, value);
        }

        public double CarLength {
            get => (int?)AttributeValue(A_CarLength) ?? 46;
            set => SetAttribute(A_CarLength, value);
        }

        public double Length => Count * CarLength;

        public TrainCommonInfo Train { get; }
    }

    public class TrainDriverInfo : LayoutInfo {
        private const string A_Type = "Type";
        private const string A_TypeName = "TypeName";
        private const string A_DriverId = "DriverID";
        private const string A_ComputerDriven = "ComputerDriven";

        public TrainDriverInfo(XmlElement element) : base(element) {
        }

        public TrainDriverInfo(TrainCommonInfo train) : base(train.DriverElement) {
        }

        public string Type => GetOptionalAttribute(A_Type) ?? "Automatic";

        public string TypeName => GetAttribute(A_TypeName);

        public string? DriverId {
            get => GetOptionalAttribute(A_DriverId);
            set => SetAttribute(A_DriverId, value, removeIf: null);
        }

        /// <summary>
        /// Return true if locomotive is driven by the computer (true), or by external mean
        /// </summary>
        public bool ComputerDriven => (bool?)AttributeValue(A_ComputerDriven) ?? true;
    }

    #endregion

    public enum KnownTrainLength {
        LocomotiveOnly,
        VeryShort,
        Short,
        Standard,
        Long,
        VeryLong,
    };

    /// <summary>
    /// Indication of train length
    /// </summary>
    public struct TrainLength {
        private readonly KnownTrainLength length;

        public TrainLength(KnownTrainLength length) {
            this.length = length;
        }

        public TrainLength(int l) {
            this.length = (KnownTrainLength)l;
        }

        public TrainLength(string t) {
            length = Parse(t).length;
        }

        public static TrainLength LocomotiveOnly => new TrainLength(KnownTrainLength.LocomotiveOnly);
        public static TrainLength VeryShort => new TrainLength(KnownTrainLength.VeryShort);
        public static TrainLength Short => new TrainLength(KnownTrainLength.Short);
        public static TrainLength Standard => new TrainLength(KnownTrainLength.Standard);
        public static TrainLength Long => new TrainLength(KnownTrainLength.Long);
        public static TrainLength VeryLong => new TrainLength(KnownTrainLength.VeryLong);

        public static implicit operator KnownTrainLength(TrainLength l) => l.length;

        public static explicit operator int(TrainLength l) => (int)l.length;

        public static bool operator ==(TrainLength l1, TrainLength l2) => l1.length == l2.length;

        public static bool operator !=(TrainLength l1, TrainLength l2) => !(l1 == l2);

        public static bool operator >(TrainLength l1, TrainLength l2) => (int)l1.length > (int)l2.length;

        public static bool operator <(TrainLength l1, TrainLength l2) => (int)l1.length < (int)l2.length;

        public static bool operator >=(TrainLength l1, TrainLength l2) => l1 > l2 || l1 == l2;

        public static bool operator <=(TrainLength l1, TrainLength l2) => l1 < l2 || l1 == l2;

        public override bool Equals(object obj) {
            return obj == null || !(obj is TrainLength) ? false : ((TrainLength)obj).length == length;
        }

        public override int GetHashCode() => length.GetHashCode();

        public override string ToString() => length switch
        {
            KnownTrainLength.LocomotiveOnly => "LocomotiveOnly",
            KnownTrainLength.VeryShort => "VeryShort",
            KnownTrainLength.Short => "Short",
            KnownTrainLength.Standard => "Standard",
            KnownTrainLength.Long => "Long",
            KnownTrainLength.VeryLong => "VeryLong",
            _ => "Unknown"
        };

        public static bool TryParse(string t, out TrainLength length) {
            bool result = true;

            switch (t) {
                case "LocomotiveOnly": length = TrainLength.LocomotiveOnly; break;
                case "VeryShort": length = TrainLength.VeryShort; break;
                case "Short": length = TrainLength.Short; break;
                case "Standard": length = TrainLength.Standard; break;
                case "Long": length = TrainLength.Long; break;
                case "VeryLong": length = TrainLength.VeryLong; break;
                default:
                    length = TrainLength.Standard;
                    result = false;
                    break;
            }

            return result;
        }

        public static TrainLength Parse(string t) {
            if (!TryParse(t, out TrainLength length))
                throw new FormatException("Invalid TrainLength value: " + t);

            return length;
        }

        public string ToDisplayString(bool lowerCase) {
            string t = "Unknown";

            switch (length) {
                case KnownTrainLength.LocomotiveOnly: t = "Locomotive only"; break;
                case KnownTrainLength.VeryShort: t = "Very short"; break;
                case KnownTrainLength.Short: t = "Short"; break;
                case KnownTrainLength.Standard: t = "Standard"; break;
                case KnownTrainLength.Long: t = "Long"; break;
                case KnownTrainLength.VeryLong: t = "Very long"; break;
            }

            return lowerCase ? $"{t.Substring(0, 1).ToLower()}{t.Substring(1)}" : t;
        }

        public string ToDisplayString() => ToDisplayString(false);
    }

    public static class ConvertableStringExtension {
        public static TrainLength? ToTrainLength(this ConvertableString s) {
            return (string?)s != null ? (TrainLength?)TrainLength.Parse(s.ValidString()) : null;
        }
    }

    /// <summary>
    /// Comparision operations on train length
    /// </summary>
    public enum TrainLengthComparison {
        None,
        NotLonger,
        Longer
    }

    public class TrainFrontAndLength {
        public LayoutComponentConnectionPoint Front;
        public TrainLength Length;

        public TrainFrontAndLength(LayoutComponentConnectionPoint front, TrainLength length) {
            this.Front = front;
            this.Length = length;
        }
    }

    public class TrainCommonInfo : LayoutNamedTrainObject, IObjectHasId, IObjectHasAttributes {
        private const string E_Attributes = "Attributes";
        private const string E_Name = "Name";
        private const string A__TriggerCount = "_TriggerCount";
        private const string A_LastCarTriggerBlockEdge = "LastCarTriggerBlockEdge";
        private const string A_LastCarDetected = "LastCarDetected";
        private const string A_Length = "Length";
        private const string A__LocomotivesSpeedLimit = "_LocomotivesSpeedLimit";
        private const string A_SpeedLimit = "SpeedLimit";
        private const string A_SlowDownSpeed = "SlowDownSpeed";
        private const string A_TargetSpeed = "TargetSpeed";
        private const string E_Driver = "Driver";
        private const string E_Locomotives = "Locomotives";
        private const string E_Locomotive = "Locomotive";
        private const string A_Id = "ID";
        private const string A_Store = "Store";
        private const string E_Cars = "Cars";
        private const string E_CarsInfo = "CarsInfo";
        #region Properties

        public TrainCommonInfo(XmlElement element)
            : base(element) {
        }

        /// <summary>
        /// The collection of locomotives in this train
        /// </summary>
        public IList<TrainLocomotiveInfo> Locomotives {
            get {
                List<TrainLocomotiveInfo> trainLocomotives = new List<TrainLocomotiveInfo>(LocomotivesElement.ChildNodes.Count);

                foreach (XmlElement trainLocomotiveElement in LocomotivesElement)
                    trainLocomotives.Add(new TrainLocomotiveInfo(this, trainLocomotiveElement));

                return trainLocomotives.AsReadOnly();
            }
        }

        public bool HasAttributes => Element[E_Attributes] != null;

        /// <summary>
        /// User attributes associated with this train
        /// </summary>
        public AttributesInfo Attributes {
            get {
                XmlElement attributesElement = Element[E_Attributes];

                if (attributesElement == null) {
                    attributesElement = Element.OwnerDocument.CreateElement(E_Attributes);

                    Element.AppendChild(attributesElement);
                }

                return new AttributesInfo(attributesElement);
            }
        }

        /// <summary>
        /// The train name
        /// </summary>
        public string Name {
            get {
                XmlElement nameElement = Element[E_Name];

                return nameElement != null ? nameElement.InnerText : "";
            }

            set {
                XmlElement nameElement = Element[E_Name];

                EraseImage();

                if (nameElement == null) {
                    nameElement = Element.OwnerDocument.CreateElement(E_Name);
                    Element.AppendChild(nameElement);
                }
                nameElement.InnerText = value;

                Redraw();
            }
        }

        public override string GetDisplayName(TrainObjectDisplayNameFormat format) {
            string name = Name;

            if (!string.IsNullOrEmpty(name)) {
                if (format != TrainObjectDisplayNameFormat.Plain) {
                    foreach (TrainLocomotiveInfo trainLoco in Locomotives) {
                        LocomotiveInfo loco = trainLoco.Locomotive;

                        if ((format & TrainObjectDisplayNameFormat.IncludeCollectionId) != 0 && !string.IsNullOrEmpty(loco.CollectionId))
                            name += " [" + loco.CollectionId + "]";

                        if ((format & TrainObjectDisplayNameFormat.IncludeAddress) != 0 && loco.AddressProvider != null)
                            name += " (" + loco.AddressProvider.Unit + ")";
                    }
                }
                return name;
            }
            else
                return "";
        }

        public LayoutTextInfo NameProvider => new LayoutTextInfo(Element, E_Name);

        /// <summary>
        /// The train is not managed. There is no information about the train motion direction or speed
        /// </summary>
        public bool NotManaged {
            get {
                foreach (TrainLocomotiveInfo trainLoco in Locomotives) {
                    if (trainLoco.Locomotive.NotManaged)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Return true if train is managed, meaning that it is possible to control its speed and direction
        /// </summary>
        public bool Managed => !NotManaged;

        /// <summary>
        /// Return the number of time a track contact will be triggered when this train passes.
        /// </summary>
        /// <remarks>
        /// This number may be larger than 1 if the train contains more than one magnet. For example a magnet
        /// on the locomotive and a magnet on the last car. Another possibility is that the train has more than
        /// one locomotive, each with a track contact magnet
        /// </remarks>
        public int TrackContactTriggerCount {
            get {
                if (!Element.HasAttribute(A__TriggerCount)) {
                    int triggerCount = 0;

                    foreach (TrainLocomotiveInfo trainLocomotive in Locomotives)
                        if (trainLocomotive.Locomotive.CanTriggerTrackContact)
                            triggerCount++;

                    if (LastCarTriggerBlockEdge)
                        triggerCount++;

                    SetAttribute(A__TriggerCount, triggerCount);
                }

                return (int)AttributeValue(A__TriggerCount);
            }
        }

        public bool LastCarTriggerBlockEdge {
            get => (bool?)AttributeValue(A_LastCarTriggerBlockEdge) ?? false;

            set {
                SetAttribute(A_LastCarTriggerBlockEdge, value);
                FlushCachedValues();
            }
        }

        public bool LastCarDetectedByOccupancyBlock {
            get => (bool?)AttributeValue(A_LastCarDetected) ?? false;

            set {
                SetAttribute(A_LastCarDetected, value);
                FlushCachedValues();
            }
        }

        public TrainLength Length {
            get => AttributeValue(A_Length).ToTrainLength() ?? TrainLength.Standard;
            set => SetAttribute(A_Length, value);
        }

        public bool HasLights {
            get {
                foreach (TrainLocomotiveInfo trainLocomotive in Locomotives)
                    if (trainLocomotive.Locomotive != null && trainLocomotive.Locomotive.HasLights)
                        return true;

                return false;
            }
        }

        public int SpeedSteps {
            get {
                if (Locomotives.Count > 0) {
                    var firstLoco = Locomotives[0].Locomotive;

                    return firstLoco?.SpeedSteps ?? 0;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// The speed limit imposed by this train locomotives. The returned value is the speed limit of the "slowest" locomotive
        /// </summary>
        public int LocomotivesSpeedLimit {
            get {
                if (!Element.HasAttribute(A__LocomotivesSpeedLimit)) {
                    int speedLimit = 0;

                    foreach (TrainLocomotiveInfo trainLocomotive in Locomotives) {
                        var loco = trainLocomotive.Locomotive;

                        if (loco != null) {
                            int locomotiveSpeedLimit = loco.SpeedLimit;

                            if (speedLimit == 0)
                                speedLimit = locomotiveSpeedLimit;
                            else if (locomotiveSpeedLimit != 0 && locomotiveSpeedLimit < speedLimit)
                                speedLimit = locomotiveSpeedLimit;
                        }
                    }

                    SetAttribute(A__LocomotivesSpeedLimit, speedLimit);
                }

                return (int)AttributeValue(A__LocomotivesSpeedLimit);
            }
        }

        public virtual int SpeedLimit {
            get => (int?)AttributeValue(A_SpeedLimit) ?? 0;
            set => SetAttribute(A_SpeedLimit, value, removeIf: 0);
        }

        public int SlowdownSpeed {
            get => (int?)AttributeValue(A_SlowDownSpeed) ?? 0;
            set => SetAttribute(A_SlowDownSpeed, value, removeIf: 0);
        }

        /// <summary>
        /// The speed which the driver should keep. The train should go in this speed unless limited by
        /// speed limit, slowing down, or stopped
        /// </summary>
        public virtual int TargetSpeed {
            get => (int?)AttributeValue(A_TargetSpeed) ?? LayoutModel.Instance.LogicalSpeedSteps / 2;
            set => SetAttribute(A_TargetSpeed, value);
        }

        public virtual void RefreshSpeedLimit() {
        }

        public XmlElement DriverElement {
            get {
                XmlElement driverElement = Element[E_Driver];

                if (driverElement == null) {
                    driverElement = Element.OwnerDocument.CreateElement(E_Driver);

                    Element.AppendChild(driverElement);
                }

                return driverElement;
            }

            set {
                XmlElement driverElement = Element[E_Driver];

                if (driverElement != null)
                    Element.RemoveChild(driverElement);

                driverElement = (XmlElement)Element.OwnerDocument.ImportNode(value, true);
                Element.AppendChild(driverElement);
            }
        }

        public TrainDriverInfo Driver => new TrainDriverInfo(DriverElement);

        #endregion

        #region Operations

        #region Train Locomotives Management

        internal XmlElement LocomotivesElement {
            get {
                XmlElement locomotivesElement = Element[E_Locomotives];

                if (locomotivesElement == null) {
                    locomotivesElement = Element.OwnerDocument.CreateElement(E_Locomotives);
                    Element.AppendChild(locomotivesElement);
                }

                return locomotivesElement;
            }
        }

        public bool ContainsLocomotive(Guid locomotiveId) {
            foreach (TrainLocomotiveInfo trainLocomotive in Locomotives)
                if (trainLocomotive.LocomotiveId == locomotiveId)
                    return true;
            return false;
        }

        public bool ContainsLocomotive(LocomotiveInfo loco) => ContainsLocomotive(loco.Id);

        #region Add Locomotive

        public virtual CanPlaceTrainResult AddLocomotive(LocomotiveInfo loco, LocomotiveOrientation orientation, LayoutBlock? block, bool validateAddress) {
            CanPlaceTrainResult result = new CanPlaceTrainResult();

            if (Locomotives.Count > 0 && !Locomotives[0].Locomotive.IsCompatibleOperationCharacteristics(loco.Element)) {
                result.Status = CanPlaceTrainStatus.LocomotiveNotCompatible;
                result.Locomotive = loco;
                result.Train = this;
            }
            else {
                XmlElement trainLocomotiveElement = Element.OwnerDocument.CreateElement(E_Locomotive);
                TrainLocomotiveInfo trainLocomotive = new TrainLocomotiveInfo(this, trainLocomotiveElement) {
                    LocomotiveId = loco.Id,
                    Orientation = orientation
                };

                LocomotivesElement.AppendChild(trainLocomotiveElement);
                result.TrainLocomotive = trainLocomotive;
                FlushCachedValues();
            }

            return result;
        }

        public CanPlaceTrainResult AddLocomotive(LocomotiveInfo loco, bool validateAddress = true) => AddLocomotive(loco, LocomotiveOrientation.Forward, null, validateAddress);

        #endregion

        public virtual void RemoveLocomotive(TrainLocomotiveInfo removedTrainLoco) {
            LocomotivesElement.RemoveChild(removedTrainLoco.Element);
            FlushCachedValues();
        }

        public void Initialize() {
            Element.SetAttribute(A_Id, Guid.NewGuid());
            Element.SetAttribute(A_Store, LayoutModel.LocomotiveCollection.DefaultStore);
        }

        public virtual void CopyFrom(TrainCommonInfo otherTrain, LayoutBlock? block, bool validateAddress) {
            LocomotivesElement.RemoveAll();

            foreach (TrainLocomotiveInfo trainLocomotive in otherTrain.Locomotives)
                this.AddLocomotive(trainLocomotive.Locomotive, trainLocomotive.Orientation, block, validateAddress);

            this.Length = otherTrain.Length;
            this.LastCarTriggerBlockEdge = otherTrain.LastCarTriggerBlockEdge;

            // Copy user attributes
            if (otherTrain.Element[E_Attributes] != null) {
                XmlElement attributesElement = (XmlElement)Element.OwnerDocument.ImportNode(otherTrain.Element[E_Attributes], true);

                if (Element[E_Attributes] != null)
                    Element.RemoveChild(Element[E_Attributes]);
                Element.AppendChild(attributesElement);
            }

            // Copy driver parameters
            this.SpeedLimit = otherTrain.SpeedLimit;
            this.SlowdownSpeed = otherTrain.SlowdownSpeed;

            // Copy motion ramps
            XmlNodeList otherTrainRampElements = otherTrain.Element.SelectNodes("Ramp");

            foreach (XmlElement otherTrainRampElement in otherTrainRampElements) {
                XmlElement rampElement = (XmlElement)Element.OwnerDocument.ImportNode(otherTrainRampElement, true);

                Element.AppendChild(rampElement);
            }
        }

        public void CopyFrom(TrainCommonInfo otherTrain, bool validateAddress) {
            CopyFrom(otherTrain, null, validateAddress);
        }

        #endregion

        #region Train Cars Management

        public XmlElement CarsElement {
            get {
                XmlElement carsElement = Element[E_Cars];

                if (carsElement == null) {
                    carsElement = Element.OwnerDocument.CreateElement(E_Cars);

                    Element.AppendChild(carsElement);
                }

                return carsElement;
            }
        }

        public TrainCarsInfo AddCars() {
            XmlElement carsInfoElement = Element.OwnerDocument.CreateElement(E_CarsInfo);

            CarsElement.AppendChild(carsInfoElement);
            return new TrainCarsInfo(this, carsInfoElement);
        }

        public void RemoveCars(TrainCarsInfo cars) {
            CarsElement.RemoveChild(cars.Element);
            FlushCachedValues();
        }

        public IList<TrainCarsInfo> Cars {
            get {
                List<TrainCarsInfo> cars = new List<TrainCarsInfo>(CarsElement.ChildNodes.Count);

                foreach (XmlElement carsInfoElement in CarsElement)
                    cars.Add(new TrainCarsInfo(this, carsInfoElement));

                return cars.AsReadOnly();
            }
        }

        #endregion

        public virtual void EraseImage() {
        }

        public virtual void Redraw() {
        }

        /// <summary>
        /// Erase all cached values. Force recalculation
        /// </summary>
        public virtual void FlushCachedValues() {
            Element.RemoveAttribute("_Length");
            Element.RemoveAttribute(A__TriggerCount);
            Element.RemoveAttribute(A__LocomotivesSpeedLimit);
        }

        #endregion
    }

    public class TrainInCollectionInfo : TrainCommonInfo {
        public TrainInCollectionInfo(XmlElement element) : base(element) {
        }
    }

    #endregion
}

