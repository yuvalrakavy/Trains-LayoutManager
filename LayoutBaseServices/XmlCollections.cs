using System;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;

#nullable enable
namespace LayoutManager {
    /// <summary>
    /// Base class for collections that store there data inside XML element
    /// </summary>
    /// <typeparam name="T">The collection item type</typeparam>
    public abstract class XmlCollection<T> : ICollection<T>, IObjectHasXml {
        /// <summary>
        /// Construct a collection whose items are child items of a given XML element
        /// </summary>
        /// <param name="element">The element that contains the collection item elements</param>
        protected XmlCollection(XmlElement element) {
            this.Element = element;
        }

        /// <summary>
        /// Construct a collection whose items are child items of an element whose name is given. If the
        /// collection element does not exist, it is created.
        /// </summary>
        /// <param name="parentElement">Element containing the collection element</param>
        /// <param name="collectionElementName">The collection element name</param>
        protected XmlCollection(XmlElement parentElement, string collectionElementName) {
            if ((this.Element = parentElement[collectionElementName]) == null) {
                this.Element = parentElement.OwnerDocument.CreateElement(collectionElementName);
                parentElement.AppendChild(this.Element);
            }
        }

        /// <summary>
        /// Create an XML element storing a collection item data
        /// </summary>
        /// <param name="item">The item for which the element need to be created</param>
        /// <returns>XML element to store the item in</returns>
        virtual protected XmlElement CreateElement(T item) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return collection item initialized with data in a XML element
        /// </summary>
        /// <param name="itemElement">The XML element storing the collection item data</param>
        /// <returns>The collection item</returns>
        abstract protected T FromElement(XmlElement itemElement);

        /// <summary>
        /// Do the actual work of adding an item to the collection. If the item is already associated with Xml Element
        /// avoid creating new element for the item, and just append the Xml element to the collection
        /// </summary>
        /// <param name="item">The item to be added</param>
        /// <returns>Xml element of the added item</returns>
        protected XmlElement AddItem(T item) {
            var itemElement = item as XmlElement;

            if (itemElement == null) {
                if (item is IObjectHasXml itemWithXml)
                    itemElement = itemWithXml.Element;
            }

            if (itemElement != null) {
                if (itemElement.OwnerDocument != Element.OwnerDocument)
                    itemElement = (XmlElement)Element.OwnerDocument.ImportNode(itemElement, true);
            }
            else
                itemElement = CreateElement(item);

            if (itemElement.ParentNode != Element)
                Element.AppendChild(itemElement);

            return itemElement;
        }

        #region IObjectHasXml Members

        /// <summary>
        /// The collection XML element. The child of this element are the collection items' elements
        /// </summary>
        /// <value></value>
        public XmlElement Element { get; }
        public XmlElement? OptionalElement => Element;

        #endregion

        #region ICollection<T> Members

        /// <summary>
        /// Add new item to the collection
        /// </summary>
        /// <param name="item">Item to be added</param>
        public virtual void Add(T item) {
            AddItem(item);
        }

        /// <summary>
        /// Remove all items from the collection
        /// </summary>
        public virtual void Clear() {
            Element.RemoveAll();
        }

        /// <summary>
        /// Check if the collection contains a given item
        /// </summary>
        /// <param name="possibleItem">The item to check</param>
        /// <returns>True if the collection contains this item</returns>
        virtual public bool Contains(T possibleItem) {
            foreach (XmlElement itemElement in Element) {
                var item = FromElement(itemElement);

                if (item != null && item.Equals(possibleItem))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Copy items in the collection to an array
        /// </summary>
        /// <param name="array">The array to copy the items to</param>
        /// <param name="arrayIndex">The array index for storing the first item</param>
        public void CopyTo(T[] array, int arrayIndex) {
            foreach (XmlElement itemElement in Element)
                array[arrayIndex++] = FromElement(itemElement);
        }

        /// <summary>
        /// Get the collection size
        /// </summary>
        /// <value>Number of items in the collection</value>
        public int Count => Element.ChildNodes.Count;

        /// <summary>
        /// Is this collection read-only
        /// </summary>
        /// <value>True if read-only collection</value>
        virtual public bool IsReadOnly => false;

        /// <summary>
        /// Remove the first instance of a given item from the collection
        /// </summary>
        /// <param name="itemToRemove">The item to be removed</param>
        /// <returns>True if item was removed, false if item was not found</returns>
        virtual public bool Remove(T itemToRemove) {
            foreach (XmlElement itemElement in Element) {
                var item = FromElement(itemElement);

                if (item != null && item.Equals(itemToRemove)) {
                    Element.RemoveChild(itemElement);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Return enumerator on the collection items
        /// </summary>
        /// <returns>Enumerator on the collection items</returns>
        public IEnumerator<T> GetEnumerator() {
            foreach (XmlElement itemElement in Element)
                yield return FromElement(itemElement);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    /// <summary>
    /// Base class for collections that store there data inside XML element. Items in the collection are indexed by
    /// key, and can directly accessed.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <typeparam name="KeyT">The type of the key used to index the collection</typeparam>
    public abstract class XmlIndexedCollection<T, KeyT> : XmlCollection<T>, ICollection<T> where T : class {
        private Dictionary<KeyT, XmlElement>? index;
        private List<KeyValuePair<KeyT, XmlElement>>? sorted = null;

        /// <summary>
        /// Construct a collection whose items are child items of a given XML element
        /// </summary>
        /// <param name="element">The element that contains the collection item elements</param>
        protected XmlIndexedCollection(XmlElement element) : base(element) {
        }

        /// <summary>
        /// Construct a collection whose items are child items of an element whose name is given. If the
        /// collection element does not exist, it is created.
        /// </summary>
        /// <param name="parentElement">Element containing the collection element</param>
        /// <param name="collectionElementName">The collection element name</param>
        protected XmlIndexedCollection(XmlElement parentElement, string collectionElementName) : base(parentElement, collectionElementName) {
            InitializeIndex();
        }

        /// <summary>
        /// Get the key associated with a given item
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The key associated with this item</returns>
        abstract protected KeyT GetItemKey(T item);

        protected void CreateIndex() {
            if (index == null) {
                index = new Dictionary<KeyT, XmlElement>();
                InitializeIndex();
            }
        }

        public IDictionary<KeyT, XmlElement> ItemsDictionary {
            get {
                CreateIndex();
                Debug.Assert(index != null);
                return index;
            }
        }

        /// <summary>
        /// Get the key associated with a given item XML element.
        /// </summary>
        /// <remarks>
        /// The default implementation create the item (using FromElement() method), and get its key (using GetItemKey()).
        /// However, in some cases, it is desired to get the key directly from the XML data instead of creating the whole item.
        /// </remarks>
        /// <param name="itemElement">The XML data associated with the item</param>
        /// <returns>The key associated with the item</returns>
        virtual protected KeyT GetElementKey(XmlElement itemElement) => GetItemKey(FromElement(itemElement));

        /// <summary>
        /// Add new item to the collection
        /// </summary>
        /// <param name="item">Item to be added</param>
        public override void Add(T item) {
            CreateIndex();
            Debug.Assert(index != null);

            var itemElement = AddItem(item);
            index.Add(GetItemKey(item), itemElement);
            sorted = null;
        }

        /// <summary>
        /// Check if the collection contains a given item
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>True if the collection contains this item</returns>
        public override bool Contains(T item) => ItemsDictionary.ContainsKey(GetItemKey(item));

        /// <summary>
        /// Check if an item associated with a key is contained in the collection
        /// </summary>
        /// <param name="key">The item's key</param>
        /// <returns>True if item is in the collection</returns>
        public bool ContainsKey(KeyT key) => ItemsDictionary.ContainsKey(key);

        /// <summary>
        /// Remove all items from the collection
        /// </summary>
        public override void Clear() {
            base.Clear();
            ItemsDictionary.Clear();
            sorted = null;
        }

        /// <summary>
        /// Remove the first instance of a given item from the collection
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <returns>True if item was removed, false if item was not found</returns>
        public override bool Remove(T item) {
            KeyT key = GetItemKey(item);

            return Remove(key);
        }

        /// <summary>
        /// Remove the first instance of a given item from the collection
        /// </summary>
        /// <param name="key">The key associated with the item to be removed</param>
        /// <returns>True if item was removed, false if item was not found</returns>
        public bool Remove(KeyT key) {
            if (index != null) {
                if (index.TryGetValue(key, out XmlElement itemElement)) {
                    ItemsDictionary.Remove(key);
                    sorted = null;
                    Element.RemoveChild(itemElement);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get/set the item associated with a key
        /// </summary>
        /// <param name="key">The key associated with the item</param>
        /// <returns>The collection's item associated with the key</returns>
        public T? this[KeyT key] {
            get {
                CreateIndex();
                Debug.Assert(index != null);

                return index.TryGetValue(key, out XmlElement itemElement) ? FromElement(itemElement) : (default);
            }

            set {
                if (value == null)
                    throw new ArgumentException("Cannot set Xml collection element to null");

                Debug.Assert(index != null);
                XmlElement oldItemElement = index[key];
                XmlElement newItemElement = CreateElement(value);

                if (oldItemElement != null)
                    Element.ReplaceChild(newItemElement, oldItemElement);
                else
                    Element.AppendChild(newItemElement);

                ItemsDictionary[key] = newItemElement;
            }
        }

        /// <summary>
        /// Initialize the index with based on the collection XML data
        /// </summary>
        protected virtual void InitializeIndex() {
            Debug.Assert(index != null);
            foreach (XmlElement itemElement in Element)
                index.Add(GetElementKey(itemElement), itemElement);
            sorted = null;
        }

        /// <summary>
        /// Get enumerator of sorted items in the collection. 
        /// </summary>
        /// <param name="comparison">A delegate for function comparing two key/value pairs</param>
        /// <returns>Enumerator for the sorted items</returns>
        protected IEnumerator<T> GetSortedIterator(Comparison<KeyValuePair<KeyT, XmlElement>> comparison) {
            if (sorted == null) {
                sorted = new List<KeyValuePair<KeyT, XmlElement>>(ItemsDictionary);
                sorted.Sort(comparison);
            }

            foreach (KeyValuePair<KeyT, XmlElement> pair in sorted)
                yield return FromElement(pair.Value);
        }
    }

    public class XmlAttributeListCollection : ICollection<string> {
        private readonly XmlElement element;
        private readonly string attributeName;
        private readonly List<string> items;

        public XmlAttributeListCollection(XmlElement element, string attributeName) {
            string[] itemsArray;

            this.element = element;
            this.attributeName = attributeName;

            if (element.HasAttribute(attributeName)) {
                itemsArray = element.GetAttribute(attributeName).Split(' ', ',');
                items = new List<string>(itemsArray);
            }
            else
                items = new List<string>();
        }

        private void updateAttribute() {
            element.SetAttribute(attributeName, string.Join(",", items.ToArray()));
        }

        #region ICollection<string> Members

        public void Add(string item) {
            items.Add(item);
            updateAttribute();
        }

        public void Clear() {
            items.Clear();
            updateAttribute();
        }

        public bool Contains(string item) => items.Contains(item);

        public void CopyTo(string[] array, int arrayIndex) {
            items.CopyTo(array, arrayIndex);
        }

        public int Count => items.Count;

        public bool IsReadOnly => false;

        public bool Remove(string item) {
            bool result = items.Remove(item);
            updateAttribute();

            return result;
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator() => items.GetEnumerator();

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
