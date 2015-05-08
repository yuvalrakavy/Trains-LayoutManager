using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LayoutManager {

	/// <summary>
	/// Layout module information
	/// </summary>
	public class LayoutModule : LayoutObject {
		Type				moduleType;				// The type used to create an instance of the module
		LayoutAssembly		layoutAssembly;			// The assembly in which the module is found
		Object				moduleInstance;			// The class instance associated with this module

		/// <summary>
		/// Construct a new layout module
		/// </summary>
		/// 
		/// <param name="layoutAssembly"Reference to the object representing the assembly></param>
		/// <param name="moduleType">The object type of the module</param>
		/// <param name="moduleAttribute">Settings for the module</param>
		public LayoutModule(LayoutAssembly layoutAssembly, Type moduleType, LayoutModuleAttribute moduleAttribute) {
			this.layoutAssembly = layoutAssembly;
			this.moduleType = moduleType;

			XmlInfo.XmlDocument.LoadXml("<LayoutModule />");
			ModuleName = moduleAttribute.ModuleName;
			UserControl = moduleAttribute.UserControl;
			Enabled = moduleAttribute.Enabled;
		}

		/// <summary>
		/// The module name
		/// </summary>
		public String ModuleName {
			get {
				return XmlInfo.DocumentElement.GetAttribute("ModuleName");
			}

			set {
				XmlInfo.DocumentElement.SetAttribute("ModuleName", value);
			}
		}

		/// <summary>
		/// If true, the user can enable/disable the module
		/// </summary>
		public bool UserControl {
			get {
				return XmlConvert.ToBoolean(XmlInfo.DocumentElement.GetAttribute("UserControl"));
			}

			set {
				XmlInfo.DocumentElement.SetAttribute("UserControl", XmlConvert.ToString(value));
			}
		}

		/// <summary>
		/// Set/Get whether the module is enabled.
		/// </summary>
		/// <remarks>
		/// Please note that disabling the module may fail if the module refused to be disabled.
		/// After disabling the module you should check this property and verify that indeed it was set 
		/// to false
		/// </remarks>
		public bool Enabled {
			get {
				if(XmlInfo.DocumentElement.HasAttribute("Enabled"))
					return XmlConvert.ToBoolean(XmlInfo.DocumentElement.GetAttribute("Enabled"));
				else
					return true;		// By default the module is enabled
			}

			set {
				bool	ok = true;

				if(value == true)
					EnableModule();
				else {
					if(!DisableModule())
						ok = false;
				}

				if(ok)		
					XmlInfo.DocumentElement.SetAttribute("Enabled", XmlConvert.ToString(value));
			}
		}

        /// <summary>
        /// Return the instance of the module class (or null if the module is in disabled state)
        /// </summary>
        public Object ModuleInstance => moduleInstance;

        /// <summary>
        /// Internal method to enable the module
        /// </summary>
        protected void EnableModule() {
			if(moduleInstance == null) {
				moduleInstance = layoutAssembly.Assembly.CreateInstance(moduleType.FullName);

				// Create subscriptions for event handlers in the new module instance
				EventManager.AddObjectSubscriptions(moduleInstance);
				EventManager.Event(new LayoutEvent(moduleInstance, "module-enabled"));
			}
		}

		/// <summary>
		/// Internal method to disable the module
		/// </summary>
		/// <returns>True if the module successfuly disabled, false if the module refused to be disabled</returns>
		protected bool DisableModule() {
			if(moduleInstance != null) {
				LayoutEvent	moduleDisableRequestEvent = new LayoutEvent(moduleInstance, "module-disable-request", null, true);

				EventManager.Event(moduleDisableRequestEvent);
				if((bool)moduleDisableRequestEvent.Info == false)
					return false;			// Module refuses to be disabled

				EventManager.Event(new LayoutEvent(moduleInstance, "module-disabled"));

				// Remove all subscriptions for this instance
				EventManager.Subscriptions.RemoveObjectSubscriptions(moduleInstance);

				if(moduleInstance is IDisposable)
					((IDisposable)moduleInstance).Dispose();

				moduleInstance = null;
				Enabled = false;

			}
			return true;
		}
	};

	/// <summary>
	/// Interface that can be implemented by module class. If this interface is implemented, the module manager
	/// will set the EventManager and Module properties
	/// </summary>
	public interface ILayoutModuleSetup {
	}

	/// <summary>
	/// Attribute for annotating a class as a layout module
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited=false)]
	public sealed class LayoutModuleAttribute : System.Attribute {
		String	moduleName;
		bool	userControl = true;
		bool	enabled = true;

		/// <summary>
		/// Annotate class as a layout module providing its name
		/// </summary>
		/// <param name="moduleName"></param>
		public LayoutModuleAttribute(String moduleName) {
			this.moduleName = moduleName;
		}

		public LayoutModuleAttribute() {
		}

        /// <summary>
        /// The module name (as shown to the user)
        /// </summary>
        public String ModuleName => moduleName;

        /// <summary>
        /// If true, the user can enable/disable the module via the UI
        /// </summary>
        public bool UserControl {
			set {
				userControl = value;
			}

			get {
				return userControl;
			}
		}

		/// <summary>
		/// The state of the module. If True, the module is initially enabled
		/// </summary>
		public bool Enabled {
			set {
				enabled = value;
			}

			get {
				return enabled;
			}
		}
	}

	/// <summary>
	/// Represent an assembly that contains layout modules
	/// </summary>
	public class LayoutAssembly : LayoutObject, IDisposable {
		Assembly			assembly;
		List<LayoutModule>	layoutModules = new List<LayoutModule>();

		public enum AssemblyOrigin {
			BuiltIn, InModulesDirectory, UserLoaded
		};

		AssemblyOrigin		_origin;

		static string		_modulesDirectory;

		/// <summary>
		/// Define a new assembly
		/// </summary>
		/// 
		/// <param name="assemblyFilename">The name of a file containing the assembly</param>
		/// <param name="origin">Source of this assembly</param>
		public LayoutAssembly(String assemblyFilename, AssemblyOrigin origin) {
			XmlInfo.XmlDocument.LoadXml("<LayoutAssembly />");
			
			AssemblyFilename = assemblyFilename;
			Origin = origin;
			ScanAssembly();
		}

		/// <summary>
		/// Define a new assembly
		/// </summary>
		/// 
		/// <param name="assemblyFilename">The name of a file containing the assembly</param>
		public LayoutAssembly(String assemblyFilename)
			: this(assemblyFilename, AssemblyOrigin.UserLoaded) {
		}

		/// <summary>
		/// Define a new assembly
		/// </summary>
		/// 
		/// <param name="assembly">A reference to an assembly object</param>
		public LayoutAssembly(Assembly assembly) {
			this.assembly = assembly;

			Origin = AssemblyOrigin.BuiltIn;
			XmlInfo.XmlDocument.LoadXml("<LayoutAssembly />");
			AssemblyFilename = this.assembly.Location;
			ScanAssembly();
		}

		public LayoutAssembly(XmlReader r) {
			XmlNode	layoutAssemblyNode = XmlInfo.XmlDocument.ReadNode(r);
			XmlInfo.XmlDocument.AppendChild(layoutAssemblyNode);
			ScanAssembly();
		}

		/// <summary>
		/// Convert file path to value that can be stored.
		/// </summary>
		/// <remarks>
		/// The returned value is not rooted (relative) if the path refered to a file in the default directory, otherwise
		/// the returned value is the rooted value
		/// </remarks>
		/// <param name="path">Path</param>
		/// <param name="root">Default directory for storing this kind of files</param>
		/// <returns>The value to store</returns>
		public static string FilePathToValue(string path, string root) {
			string	v;

			path = Path.GetFullPath(path);

			if(Path.GetDirectoryName(path).StartsWith(root, true, null)) {
				v = path.Substring(root.Length);

				if(v[0] == Path.DirectorySeparatorChar)
					v = v.Substring(1);
			}
			else
				v = path;

			return v;
		}

		/// <summary>
		/// Convert stored value stored to path.
		/// </summary>
		/// <remarks>
		/// If the value is not rooted (it is relative path), assume that the value is a name of a file in the default directory,
		/// otherwise the path is the actual value
		/// </remarks>
		/// <param name="v">The value</param>
		/// <param name="root">Default directory for storing this kind of files</param>
		/// <returns>Path for the needed file</returns>
		public static string ValueToFilePath(string v, string root) {
			if(Path.IsPathRooted(v))
				return v;
			else
				return Path.Combine(root, v);
		}

		/// <summary>
		/// Get the default directory in which LayoutManager modules are stored
		/// </summary>
		public static string ModulesDirectory {
			get {
				if(_modulesDirectory == null) {
					_modulesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");

					if(!Directory.Exists(_modulesDirectory))
						_modulesDirectory = AppDomain.CurrentDomain.BaseDirectory;
				}

				return _modulesDirectory;
			}
		}

		/// <summary>
		/// Get the file name containing the assembly
		/// </summary>
		public String AssemblyFilename {
			get {
				return ValueToFilePath(Element.GetAttribute("AssemblyFilename"), ModulesDirectory);
			}

			set {
				Element.SetAttribute("AssemblyFilename", FilePathToValue(value, ModulesDirectory));
			}
		}

        /// <summary>
        /// Return the assembly object
        /// </summary>
        public Assembly Assembly => assembly;

        public AssemblyOrigin Origin {
			get {
				return _origin;
			}

			set {
				_origin = value;
			}
		}

		/// <summary>
		/// If true, this a reference to this assembly will be saved, so it will be scanned next time
		/// </summary>
		public bool SaveAssemblyReference {
			get {
				if(XmlInfo.DocumentElement.HasAttribute("Save"))
					return XmlConvert.ToBoolean(XmlInfo.DocumentElement.GetAttribute("Save"));
				else
					return false;
			}

			set {
				XmlInfo.DocumentElement.SetAttribute("Save", XmlConvert.ToString(value));
			}
		}

        /// <summary>
        /// Return an array with all the layout modules in this assembly
        /// </summary>
        public IList<LayoutModule> LayoutModules => layoutModules.AsReadOnly();

        /// <summary>
        /// Scan assembly for classes annotated with the LayoutModule attribute
        /// </summary>
        protected void ScanAssembly() {
			if(assembly == null)
				assembly = Assembly.LoadFrom(AssemblyFilename);

			Type[]	types = assembly.GetTypes();
			
			foreach(Type type in types) {
				// Check if the type is a LayoutModule
				LayoutModuleAttribute[]	layoutModuleAttributes = 
					(LayoutModuleAttribute[] )type.GetCustomAttributes(typeof(LayoutModuleAttribute), false);

				foreach(LayoutModuleAttribute layoutModuleAttribute in layoutModuleAttributes) {
					LayoutModule	layoutModule = new LayoutModule(this, type, layoutModuleAttribute);

					layoutModules.Add(layoutModule);
				}

				// TODO: Check for factory association
			}
		}

		/// <summary>
		/// Dispose the assembly, disable all modules
		/// </summary>
		public void Dispose() {
			foreach(LayoutModule module in layoutModules)
				module.Enabled = false;
		}

		/// <summary>
		/// Save information about this assembly in an XML document
		/// </summary>
		/// <param name="w">The XML document writer</param>
		public void WriteXml(XmlWriter w) {
			if(SaveAssemblyReference)
				XmlInfo.XmlDocument.Save(w);
		}
	}

	/// <summary>
	/// A collection of all assemblies which are currently active
	/// </summary>
	public class LayoutAssemblyCollection : List<LayoutAssembly> {

		/// <summary>
		/// Construct the collection
		/// </summary>
		/// 
		public LayoutAssemblyCollection() {
		}

		public bool IsDefined(string filename) {
			string name = Path.GetFileName(filename);

			return Exists(delegate(LayoutAssembly a) { return Path.GetFileName(a.AssemblyFilename).Equals(name, StringComparison.InvariantCultureIgnoreCase); });
		}

		/// <summary>
		/// Add layout assembly to the collection
		/// </summary>
		/// <param name="layoutAssembly">The layout assembly to be added</param>
		public new void Add(LayoutAssembly layoutAssembly) {
			if(layoutAssembly.LayoutModules.Count > 0)
				base.Add(layoutAssembly);
			EventManager.Instance.FlushEventDefinitions();
		}

		/// <summary>
		/// Remove a layout assembly from the collection
		/// </summary>
		/// <param name="layoutAssembly">The layout assembly to be removed</param>
		public new void Remove(LayoutAssembly layoutAssembly) {
			base.Remove(layoutAssembly);
			layoutAssembly.Dispose();
			EventManager.Instance.FlushEventDefinitions();
		}

		/// <summary>
		/// Move up (towards the beginning of the collection) a given layout assembly.
		/// </summary>
		/// <remarks>
		/// Layout assemblies are initialized in order. Thus if a layout assembly initialization
		/// process depends on services provided by other assemblies, the other assembly should appear in
		/// the list first
		/// </remarks>
		/// <param name="layoutAssembly">The layout assembly to be "moved"</param>
		public void MoveUp(LayoutAssembly layoutAssembly) {
			int		i = IndexOf(layoutAssembly);

			if(i > 0) {
				LayoutAssembly	temp;

				temp = this[i-1];
				this[i-1] = this[i];
				this[i] = temp;
			}
		}

		/// <summary>
		/// Move down (towards the end of the collection) a given layout assembly.
		/// </summary>
		/// <remarks>
		/// Layout assemblies are initialized in order. Thus if a layout assembly initialization
		/// process depends on services provided by other assemblies, the other assembly should appear in
		/// the list first
		/// </remarks>
		/// <param name="layoutAssembly">The layout assembly to be "moved"</param>
		public void MoveDown(LayoutAssembly layoutAssembly) {
			int		i = IndexOf(layoutAssembly);

			if(i+1 < Count) {
				LayoutAssembly	temp;

				temp = this[i+1];
				this[i+1] = this[i];
				this[i] = temp;
			}
		}

		/// <summary>
		/// Store the collection in an XML document
		/// </summary>
		/// <param name="w"></param>
		public void WriteXml(XmlWriter w) {
			w.WriteStartElement("LayoutAssemblies");

			foreach(LayoutAssembly layoutAssembly in this)
				layoutAssembly.WriteXml(w);

			w.WriteEndElement();
		}

		/// <summary>
		/// Initialize the collection content from an XML document
		/// </summary>
		/// <param name="r"></param>
		public void ReadXml(XmlReader r) {
			r.Read();		// <LayoutAssemblies
			while(r.NodeType == XmlNodeType.Element) {
				LayoutAssembly	layoutAssembly = new LayoutAssembly(r);

				if(!IsDefined(Path.GetFileName(layoutAssembly.AssemblyFilename)))
					Add(layoutAssembly);
			}
		}
	}

	/// <summary>
	/// Manage assemblies with Layout modules
	/// </summary>
	public class LayoutModuleManager {
		LayoutAssemblyCollection	layoutAssemblies;
		string						documentFilename;

		/// <summary>
		/// Construct a layout module manager
		/// </summary>
		/// 
		public LayoutModuleManager() {
			layoutAssemblies = new LayoutAssemblyCollection();
		}

		/// <summary>
		/// The file name of the document used to save the state of the module manager.
		/// </summary>
		public String DocumentFilename {
			get {
				return documentFilename;
			}

			set {
				documentFilename = value;
			}
		}

        /// <summary>
        /// A collection of all the referenced layout assemblies
        /// </summary>
        public LayoutAssemblyCollection LayoutAssemblies => layoutAssemblies;

        /// <summary>
        /// Store the layout module manager state on a XML document
        /// </summary>
        /// <param name="w">XML document writer</param>
        public void WriteXml(XmlWriter w) {
			w.WriteStartElement("LayoutModuleManager");
			layoutAssemblies.WriteXml(w);
			w.WriteEndElement();
		}

		/// <summary>
		/// Load the state of the module manager from an XML document
		/// </summary>
		/// <param name="r">XML document reader</param>
		public void ReadXml(XmlReader r) {
			r.Read();		// <LayoutModuleManager>
			r.Read();

			while(r.NodeType == XmlNodeType.Element) {
				if(r.Name == "LayoutAssemblies") {
                    if (!r.IsEmptyElement)
                        layoutAssemblies.ReadXml(r);
                    else
                        break;
				}
				else
					throw new FileParseException(String.Format("Unpexected element {0}: {1}", r.NodeType, r.Name));
			}
		}

		/// <summary>
		/// Save the module manager state (the old version is kept as backup)
		/// </summary>
		public void SaveState() {
			if(documentFilename == null)
				throw new ArgumentException("No document name was set");

			String					backupFilename = documentFilename + ".backup";
			FileInfo				fileInfo = new FileInfo(documentFilename);

			new FileInfo(backupFilename).Delete();
				
			if(fileInfo.Exists)
				fileInfo.MoveTo(backupFilename);

			using(FileStream fileOut = new FileStream(documentFilename, FileMode.Create, FileAccess.Write)) {
				XmlTextWriter	w = new XmlTextWriter(fileOut, System.Text.Encoding.UTF8);
				w.WriteStartDocument();
				WriteXml(w);
				w.WriteEndDocument();
				w.Close();
			}
		}

		/// <summary>
		/// Load the module manager state from a file
		/// </summary>
		public void LoadState() {
			try {
				using(FileStream fileIn = new FileStream(documentFilename, FileMode.Open, FileAccess.Read)) {
					XmlTextReader r = new XmlTextReader(fileIn);
					r.WhitespaceHandling = WhitespaceHandling.None;
					r.Read();		// <?XML stuff >

					ReadXml(r);
					r.Close();
				}
            } catch(DirectoryNotFoundException) {
                Directory.CreateDirectory(Path.GetDirectoryName(documentFilename));
			} catch(FileNotFoundException) {
			}
		}
	}

	/// <summary>
	/// A convinence class for defining modules. It implements ILayoutModuleSetup.
	/// </summary>
	public class LayoutModuleBase : ILayoutModuleSetup {
		public static void Error(Object subject, String message) {
			EventManager.Event(new LayoutEvent(subject, "add-error", null, message));
		}

		public static void Error(String message) {
			Error(null, message);
		}

		public static void Warning(Object subject, String message) {
			EventManager.Event(new LayoutEvent(subject, "add-warning", null, message));
		}

		public static void Warning(String message) {
			Warning(null, message);
		}

		public static void Message(Object subject, String messageText) {
			EventManager.Event(new LayoutEvent(subject, "add-message", null, messageText));
		}

		public static void Message(String messageText) {
			Message(null, messageText);
		}

	}

}
