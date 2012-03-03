using System;
using System.Collections;
using System.Diagnostics;

namespace LayoutManager
{
	public interface IObjectFactoryProduct {
		ObjectFactory Factory {
			get;
			set;
		}

		Object ProductKey {
			get;
			set;
		}
	}

	/// <summary>
	/// Summary description for ObjectFactory.
	/// </summary>
	public class ObjectFactory
	{
		public ObjectFactory()
		{
		}

		/// <summary>
		/// An entry in the hash table that maps component type to component view type.
		/// </summary>
		class FactoryMapEntry {
			internal Type		productType;					// The type of the of the class that the factory will produce
			internal int		nMaxObjects;					// Max objects that can exist in a given time

			internal Stack		productObjects = new Stack();	// Allocated objects
			internal int		nAllocated;						// Number of objects that are allocated

			internal FactoryMapEntry(Type productType, int nMaxObjects) {
				this.productType = productType;
				this.nMaxObjects = nMaxObjects;
			}
		}

		Hashtable		factoryMap = new Hashtable();

		/// <summary>
		/// Helper method to initialize the component to component view map hash table.
		/// </summary>
		/// <param name="componentType">the component type</param>
		/// <param name="viewType">the component view type</param>
		public void Add(Object key, Type productType) {
			factoryMap.Add(key, new FactoryMapEntry(productType, 2));
		}

		public void Add(Object key, Type productType, int maxObjects) {
			factoryMap.Add(key, new FactoryMapEntry(productType, maxObjects));
		}

		/// <summary>
		/// Return an instance of component view class that can be used to draw a given model
		/// component
		/// </summary>
		/// <param name="component">The key for deciding what type of object should be produced</param>
		/// <returns>The produced object</returns>
		public IObjectFactoryProduct ProduceObject(Object key) {
			FactoryMapEntry			entry = (FactoryMapEntry)factoryMap[key];
			IObjectFactoryProduct	result = null;

			if(entry != null) {
				// Check if it possible to reuse an instance of a component view class
				if(entry.productObjects.Count > 0)
					result = (IObjectFactoryProduct)entry.productObjects.Pop();
				else {
					// Allocate a new one, for sanity check that the number of allocated
					// instances is below a preset upper limit.
					if(++entry.nAllocated >= entry.nMaxObjects)
						throw new ApplicationException(String.Format("Too many objects for of type {0} (key {1}) are created", entry.productType, key));

					result = (IObjectFactoryProduct)Activator.CreateInstance(entry.productType);
					result.Factory = this;
				}

				result.ProductKey = key;
			}

			return result;
		}

		/// <summary>
		/// Return an instance of a component view to the pool. This allows object reuse and
		/// save the need to recreate a new one each time (and avoid generating a lot of garbage)
		/// </summary>
		/// <param name="view">The component view to return to the pool</param>
		public void ReleaseProduct(IObjectFactoryProduct product) {
			Debug.Assert(product.ProductKey != null);
			FactoryMapEntry	entry = (FactoryMapEntry)factoryMap[product.ProductKey];

			product.ProductKey = null;			// disassociate the view from the component
			entry.productObjects.Push(product);
		}

		/// <summary>
		/// Release an object was was product by an object factory, The object is returned to the object
		/// pool
		/// </summary>
		/// <param name="product">An object that was produced by an object factory</param>
		static public void ReleaseObject(IObjectFactoryProduct product) {
			product.Factory.ReleaseProduct(product);
		}
	}
}
