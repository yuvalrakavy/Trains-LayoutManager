using System;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using MethodDispatcher;

namespace LayoutManager {
    public static class LayoutModuleDispatchExtensions {
        [DispatchSource]
        public static void OnModuleEnabled(this Dispatcher d) {
            d[nameof(OnModuleEnabled)].CallVoid();
        }

        [DispatchSource]
        public static bool? OnModuleDisableRequest(this Dispatcher d, object moduleInstance) {
            return d[nameof(OnModuleDisableRequest)].CallNullable<bool>(moduleInstance);
        }

        [DispatchSource]
        public static void OnModuleDisabled(this Dispatcher d, object moduleInstance) {
            d[nameof(OnModuleDisabled)].CallVoid(moduleInstance);
        }
    }

    /// <summary>
    /// Layout module information
    /// </summary>
    public class LayoutModule : LayoutObject {
        private const string A_UserControl = "UserControl";
        private const string A_Enabled = "Enabled";
        private readonly Type moduleType;             // The type used to create an instance of the module
        private readonly LayoutAssembly layoutAssembly;         // The assembly in which the module is found
        private Object? moduleInstance;          // The class instance associated with this module

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
        public string ModuleName {
            get => XmlInfo.DocumentElement.GetAttribute("ModuleName");

            set => XmlInfo.DocumentElement.SetAttribute("ModuleName", value);
        }

        /// <summary>
        /// If true, the user can enable/disable the module
        /// </summary>
        public bool UserControl {
            get => (bool)XmlInfo.DocumentElement.AttributeValue(A_UserControl);
            set => XmlInfo.DocumentElement.SetAttributeValue(A_UserControl, value);
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
            get => (bool?)XmlInfo.DocumentElement.AttributeValue(A_Enabled) ?? true;

            set {
                bool ok = true;

                if (value)
                    EnableModule();
                else {
                    if (!DisableModule())
                        ok = false;
                }

                if (ok)
                    XmlInfo.DocumentElement.SetAttributeValue(A_Enabled, value);
            }
        }

        /// <summary>
        /// Return the instance of the module class (or null if the module is in disabled state)
        /// </summary>
        public Object? ModuleInstance => moduleInstance;

        /// <summary>
        /// Internal method to enable the module
        /// </summary>
        protected void EnableModule() {
            if (moduleInstance == null) {
                moduleInstance = layoutAssembly.Assembly?.CreateInstance(Ensure.NotNull<string>(moduleType.FullName));

                if (layoutAssembly.Assembly == null || moduleInstance == null)
                    throw new LayoutException($"Could not load module {moduleType.FullName}");

                try {
                    Dispatch.InitializeDispatcherForAssembly(layoutAssembly.Assembly);
                    Dispatch.AddObjectInstanceDispatcherTargets(moduleInstance);
                }
                catch (DispatcherErrorsException ex) {
                    ex.Save($"Initializing dispatch for assembly: {layoutAssembly.Assembly.FullName}");
                    throw new LayoutException($"Errors initializing dynamic dispatch for assembly {layoutAssembly.Assembly.FullName} (see dispatcher_errors.txt)");
                }

                Dispatch.Call.OnModuleEnabled();
                               
                // Create subscriptions for event handlers in the new module instance (will this one day become obsolete...)
                EventManager.AddObjectSubscriptions(moduleInstance);
            }
        }

        /// <summary>
        /// Internal method to disable the module
        /// </summary>
        /// <returns>True if the module successful disabled, false if the module refused to be disabled</returns>
        protected bool DisableModule() {
            if (moduleInstance != null) {
                if (!(Dispatch.Call.OnModuleDisableRequest(moduleInstance) ?? true))
                    return false;           // Module refuses to be disabled

                Dispatch.Call.OnModuleDisabled(moduleInstance);
                Dispatch.RemoveObjectInstanceDispatcherTargets(moduleInstance);

                // Remove all subscriptions for this instance (will this become obsolete one day?)
                EventManager.Subscriptions.RemoveObjectSubscriptions(moduleInstance);

                if (moduleInstance is IDisposable module)
                    module.Dispose();

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
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class LayoutModuleAttribute : System.Attribute {
        /// <summary>
        /// Annotate class as a layout module providing its name
        /// </summary>
        /// <param name="moduleName"></param>
        public LayoutModuleAttribute(String moduleName) {
            this.ModuleName = moduleName;
        }

        public LayoutModuleAttribute() {
            this.ModuleName = "Unnamed-Module";
        }

        /// <summary>
        /// The module name (as shown to the user)
        /// </summary>
        public string ModuleName { get; }

        /// <summary>
        /// If true, the user can enable/disable the module via the UI
        /// </summary>
        public bool UserControl { set; get; } = true;

        /// <summary>
        /// The state of the module. If True, the module is initially enabled
        /// </summary>
        public bool Enabled { set; get; } = true;
    }

    /// <summary>
    /// Represent an assembly that contains layout modules
    /// </summary>
    public class LayoutAssembly : LayoutObject, IDisposable {
        private const string A_Save = "Save";
        private Assembly? assembly;
        private readonly List<LayoutModule> layoutModules = new();

        public enum AssemblyOrigin {
            BuiltIn, InModulesDirectory, UserLoaded
        };

        private static string? _modulesDirectory;

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
            var layoutAssemblyNode = Ensure.NotNull<XmlNode>(XmlInfo.XmlDocument.ReadNode(r));
            XmlInfo.XmlDocument.AppendChild(layoutAssemblyNode);
            ScanAssembly();
        }

        /// <summary>
        /// Convert file path to value that can be stored.
        /// </summary>
        /// <remarks>
        /// The returned value is not rooted (relative) if the path referred to a file in the default directory, otherwise
        /// the returned value is the rooted value
        /// </remarks>
        /// <param name="path">Path</param>
        /// <param name="root">Default directory for storing this kind of files</param>
        /// <returns>The value to store</returns>
        public static string FilePathToValue(string path, string root) {
            string v;

            path = Path.GetFullPath(path);

            if (Ensure.NotNull<String>(Path.GetDirectoryName(path)).StartsWith(root, true, null)) {
                v = path[root.Length..];

                if (v[0] == Path.DirectorySeparatorChar)
                    v = v[1..];
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
            return Path.IsPathRooted(v) ? v : Path.Combine(root, v);
        }

        /// <summary>
        /// Get the default directory in which LayoutManager modules are stored
        /// </summary>
        public static string ModulesDirectory {
            get {
                if (_modulesDirectory == null) {
                    _modulesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");

                    if (!Directory.Exists(_modulesDirectory))
                        _modulesDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }

                return _modulesDirectory;
            }
        }

        /// <summary>
        /// Get the file name containing the assembly
        /// </summary>
        public string AssemblyFilename {
            get => ValueToFilePath(Element.GetAttribute("AssemblyFilename"), ModulesDirectory);

            set => Element.SetAttribute("AssemblyFilename", FilePathToValue(value, ModulesDirectory));
        }

        /// <summary>
        /// Return the assembly object
        /// </summary>
        public Assembly? Assembly => assembly;

        public AssemblyOrigin Origin { get; set; }

        /// <summary>
        /// If true, this a reference to this assembly will be saved, so it will be scanned next time
        /// </summary>
        public bool SaveAssemblyReference {
            get => (bool?)XmlInfo.DocumentElement.AttributeValue(A_Save) ?? false;
            set => XmlInfo.DocumentElement.SetAttributeValue(A_Save, value);
        }

        /// <summary>
        /// Return an array with all the layout modules in this assembly
        /// </summary>
        public IList<LayoutModule> LayoutModules => layoutModules.AsReadOnly();

        /// <summary>
        /// Scan assembly for classes annotated with the LayoutModule attribute
        /// </summary>
        protected void ScanAssembly() {
            if (assembly == null)
                assembly = Assembly.LoadFrom(AssemblyFilename);

            Type[] types = assembly.GetTypes();

            foreach (Type type in types) {
                // Check if the type is a LayoutModule
                LayoutModuleAttribute[] layoutModuleAttributes =
                    (LayoutModuleAttribute[])type.GetCustomAttributes(typeof(LayoutModuleAttribute), false);

                foreach (LayoutModuleAttribute layoutModuleAttribute in layoutModuleAttributes) {
                    LayoutModule layoutModule = new(this, type, layoutModuleAttribute);

                    layoutModules.Add(layoutModule);
                }

                // TODO: Check for factory association
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                foreach (LayoutModule module in layoutModules)
                    module.Enabled = false;
            }
        }

        /// <summary>
        /// Dispose the assembly, disable all modules
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Save information about this assembly in an XML document
        /// </summary>
        /// <param name="w">The XML document writer</param>
        public void WriteXml(XmlWriter w) {
            if (SaveAssemblyReference)
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

            return Exists((LayoutAssembly a) => Path.GetFileName(a.AssemblyFilename).Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Add layout assembly to the collection
        /// </summary>
        /// <param name="layoutAssembly">The layout assembly to be added</param>
        public new void Add(LayoutAssembly layoutAssembly) {
            if (layoutAssembly.LayoutModules.Count > 0)
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
            int i = IndexOf(layoutAssembly);

            if (i > 0) {
                LayoutAssembly temp;

                temp = this[i - 1];
                this[i - 1] = this[i];
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
            int i = IndexOf(layoutAssembly);

            if (i + 1 < Count) {
                LayoutAssembly temp;

                temp = this[i + 1];
                this[i + 1] = this[i];
                this[i] = temp;
            }
        }

        /// <summary>
        /// Store the collection in an XML document
        /// </summary>
        /// <param name="w"></param>
        public void WriteXml(XmlWriter w) {
            w.WriteStartElement("LayoutAssemblies");

            foreach (LayoutAssembly layoutAssembly in this)
                layoutAssembly.WriteXml(w);

            w.WriteEndElement();
        }

        /// <summary>
        /// Initialize the collection content from an XML document
        /// </summary>
        /// <param name="r"></param>
        public void ReadXml(XmlReader r) {
            r.Read();       // <LayoutAssemblies
            while (r.NodeType == XmlNodeType.Element) {
                LayoutAssembly layoutAssembly = new(r);

                if (!IsDefined(Path.GetFileName(layoutAssembly.AssemblyFilename)))
                    Add(layoutAssembly);
            }
        }
    }

    /// <summary>
    /// Manage assemblies with Layout modules
    /// </summary>
    public class LayoutModuleManager {
        /// <summary>
        /// Construct a layout module manager
        /// </summary>
        /// 
        public LayoutModuleManager() {
            LayoutAssemblies = new LayoutAssemblyCollection();
        }

        /// <summary>
        /// The file name of the document used to save the state of the module manager.
        /// </summary>
        public string? DocumentFilename { get; set; }

        /// <summary>
        /// A collection of all the referenced layout assemblies
        /// </summary>
        public LayoutAssemblyCollection LayoutAssemblies { get; }

        /// <summary>
        /// Store the layout module manager state on a XML document
        /// </summary>
        /// <param name="w">XML document writer</param>
        public void WriteXml(XmlWriter w) {
            w.WriteStartElement("LayoutModuleManager");
            LayoutAssemblies.WriteXml(w);
            w.WriteEndElement();
        }

        /// <summary>
        /// Load the state of the module manager from an XML document
        /// </summary>
        /// <param name="r">XML document reader</param>
        public void ReadXml(XmlReader r) {
            r.Read();       // <LayoutModuleManager>
            r.Read();

            while (r.NodeType == XmlNodeType.Element) {
                if (r.Name == "LayoutAssemblies") {
                    if (!r.IsEmptyElement)
                        LayoutAssemblies.ReadXml(r);
                    else
                        break;
                }
                else
                    throw new FileParseException(String.Format("Unexpected element {0}: {1}", r.NodeType, r.Name));
            }
        }

        /// <summary>
        /// Save the module manager state (the old version is kept as backup)
        /// </summary>
        public void SaveState() {
            if (DocumentFilename == null)
                throw new ArgumentException("No document name was set");

            string backupFilename = DocumentFilename + ".backup";
            FileInfo fileInfo = new(DocumentFilename);

            new FileInfo(backupFilename).Delete();

            if (fileInfo.Exists)
                fileInfo.MoveTo(backupFilename);

            using FileStream fileOut = new(DocumentFilename, FileMode.Create, FileAccess.Write);
            XmlTextWriter w = new(fileOut, System.Text.Encoding.UTF8);
            w.WriteStartDocument();
            WriteXml(w);
            w.WriteEndDocument();
            w.Close();
        }

        /// <summary>
        /// Load the module manager state from a file
        /// </summary>
        public void LoadState() {
            if (DocumentFilename == null)
                throw new ArgumentException("No document name was set");

            try {
                using FileStream fileIn = new(DocumentFilename, FileMode.Open, FileAccess.Read);
                XmlTextReader r = new(fileIn) {
                    WhitespaceHandling = WhitespaceHandling.None
                };
                r.Read();       // <?XML stuff >

                ReadXml(r);
                r.Close();
            }
            catch (DirectoryNotFoundException) {
                Directory.CreateDirectory(Ensure.NotNull<string>(Path.GetDirectoryName(DocumentFilename)));
            }
            catch (FileNotFoundException) {
            }
        }
    }

    /// <summary>
    /// A convenience class for defining modules. It implements ILayoutModuleSetup.
    /// </summary>
    public class LayoutModuleBase : ILayoutModuleSetup {
        public static void Error(Object? subject, string message) {
            EventManager.Event(new LayoutEvent("add-error", subject, message));
        }

        public static void Error(String message) {
            Error(null, message);
        }

        public static void Warning(Object? subject, string message) {
            EventManager.Event(new LayoutEvent("add-warning", subject, message));
        }

        public static void Warning(String message) {
            Warning(null, message);
        }

        public static void Message(Object? subject, string messageText) {
            EventManager.Event(new LayoutEvent("add-message", subject, messageText));
        }

        public static void Message(String messageText) {
            Message(null, messageText);
        }
    }
}
