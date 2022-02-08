﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace MethodDispatcher {
    /// <summary>
    /// Dispatcher - a way to call functions without knowing where they reside...
    /// </summary>
    /// 
    /// To define a callable function annotate it with the DispatchTarget attribute.
    /// 
    /// <example>
    ///     [DispatchTarget]
    ///     void IamAfunction(void) {}
    /// </example>
    /// 
    /// Please note that multiple functions with this name (in different classes) can be defined
    /// 
    /// To dispatch call to function you need to define a dispatcher source.
    /// 
    /// for examples how to define Dispatch source see: <see cref="DispatchSource"/>
    /// 
    /// <example>
    /// public static class Extensions {
    /// 
    ///     [DispatchSource]
    ///     public static IamAfunction(this Dispatcher d) {
    ///         d[nameof(IamFunction)].CallVoid();
    ///     }
    ///     
    ///     [DispatchSource]
    ///     public static string GetAppName(this Dispatcher d, string directory) {
    ///         return d[nameof(GetAppName)].Call<string>(directory);
    ///     }
    ///     
    ///     [DispatchSource]
    ///     public IEnumerate<string> CollectNames(this Dispatcher d) {
    ///         return d[nameof(CollectNames)].CallCollect
    /// }
    /// </example>"
    ///
    public class Dispatcher {
        readonly Dictionary<string, DispatchSource> dispatchSources = new();

        /// <summary>
        /// Initialize the displatcher system
        /// 
        /// Add dispatch sources and static targets for the currently loaded assemblies.
        /// If you dynamically load assemblies, you will need to call InitializeDispatcherForAssembly for each loaded assembly
        ///
        /// </summary>
        /// 
        /// <exception cref="DispatcherErrorsException">
        /// This exception contains a list of errors. For example mismatch in types or nullability between dispatch source
        /// and dispatch targets
        /// </exception>
        public void InitializeDispatcher() {
            DefineSources();
            VerifyTargets();
            AddStaticTargets();
        }

        /// <summary>
        /// Add dispatch sources defined in assembly, verify targets against sources (check types/nullability) and add
        /// static dispatch targets
        /// </summary>
        /// <param name="assembly">The assembly to add</param>
        ///
        /// <exception cref="DispatcherErrorsException">
        /// This exception contains a list of errors. For example mismatch in types or nullability between dispatch source
        /// and dispatch targets
        /// </exception>
        /// 
        public void InitializeDispatcherForAssembly(Assembly assembly) {
            DefineAssemblySources(assembly);
            VerifyAssemblyTargets(assembly);
            AddStaticAssemblyTargets(assembly);
        }

        public DispatchSource this[string sourceName] {
            get => dispatchSources[sourceName];
        }

        private void DefineSources() {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                DefineAssemblySources(assembly);
        }

        private void DefineAssemblySources(Assembly assembly) {
            var types = assembly.GetTypes();
            var query = from type in types
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == typeof(Dispatcher)
                        select method;

            foreach (var method in query)
                DefineSourceMethod(method);
        }

        private void DefineSourceMethod(MethodInfo method) {
            if(!dispatchSources.ContainsKey(method.Name))
                dispatchSources.Add(method.Name, new DispatchSource(method));
        }

        private void VerifyTargets() {
            List<DispatcherException> errors = new();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                try {
                    VerifyAssemblyTargets(assembly);
                }
                catch (DispatcherErrorsException veryfyErrors) {
                    errors.AddRange(veryfyErrors.Errors);
                }
                catch (DispatcherException ex) {
                    errors.Add(ex);
                }

            if (errors.Count > 0)
                throw new DispatcherErrorsException("Verify targets", errors);
        }

        private void VerifyAssemblyTargets(Assembly assembly) {
            List<DispatcherException> errors = new();
            var types = assembly.GetTypes();
            var query = from type in types
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.GetCustomAttribute(typeof(DispatchTargetAttribute), true) != null
                        select method;

            foreach (var method in query) {
                foreach (var dispatchTargetAttribute in (DispatchTargetAttribute[])method.GetCustomAttributes(typeof(DispatchTargetAttribute), true)) {
                    try {
                        GetDispatchSource(dispatchTargetAttribute, method).VerifyTarget(method, dispatchTargetAttribute);
                    }
                    catch (DispatcherException ex) {
                        errors.Add(ex);
                    }
                }
            }

            if (errors.Count > 0)
                throw new DispatcherErrorsException($"Errors verifying assembly {assembly.FullName} dispatch targets", errors);
        }

        private void AddStaticTargets() {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                AddStaticAssemblyTargets(assembly);
        }

        private void AddStaticAssemblyTargets(Assembly assembly) {
            var types = assembly.GetTypes();
            var query = from type in types
                        from method in type.GetMethods(BindingFlags.Static |BindingFlags.Public | BindingFlags.NonPublic)
                        where method.GetCustomAttribute(typeof(DispatchTargetAttribute), true) != null
                        select method;

            foreach (var method in query) {
                foreach (var dispatchTargetAttribute in (DispatchTargetAttribute[])method.GetCustomAttributes(typeof(DispatchTargetAttribute), true)) {
                    GetDispatchSource(dispatchTargetAttribute, method).AddStaticTarget(method, dispatchTargetAttribute);
                }
            }

        }

        public void AddObjectInstanceDispatchTargets(object objectInstance) {
            List<DispatcherException>? errors = null;

            MethodInfo[] methodsInfo = objectInstance.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            try {
                foreach (MethodInfo method in methodsInfo) {
                    var dispatchTargetAttributes = (DispatchTargetAttribute[])method.GetCustomAttributes(typeof(DispatchTargetAttribute), true);

                    foreach (var dispatchTargetAttribute in dispatchTargetAttributes) {
                        GetDispatchSource(dispatchTargetAttribute, method).AddInstanceTarget(objectInstance, method, dispatchTargetAttribute);
                    }
                }
            } catch(DispatcherException ex) {
                (errors ??= new()).Add(ex);
            }

            if (errors != null)
                throw new DispatcherErrorsException($"Adding dispatch targets for object of type: {objectInstance.GetType().Name}", errors);
        }

        public void RemoveObjectInstanceDispatchTargets(object objectInstance) {
            foreach (var dispatchSource in dispatchSources.Values)
                dispatchSource.RemoveTargetsInInstance(objectInstance);
        }

        private DispatchSource GetDispatchSource(DispatchTargetAttribute dispatchTargetAttribute, MethodInfo method) {
            var name = dispatchTargetAttribute.Name ?? method.Name;
            var inDispatchSources = (from part in name.Split('_') where dispatchSources.ContainsKey(part) select dispatchSources[part]).ToArray();

            return inDispatchSources.Length switch {
                1 => inDispatchSources[0],
                0 => throw new UndefinedDispatchSourceException(dispatchTargetAttribute.SourceFile, name),
                _ => throw new AmbiguousDispatchSourcesNameException(dispatchTargetAttribute.SourceFile, inDispatchSources)
            };
        }
    }

    /// <summary>
    /// Static access to dispatch system
    ///
    /// Start by calling Dispatch.InitializeDispatcher.
    /// Make sure to handle possible DispatcherErrorsException.
    /// 
    /// Dispatch sources are added as Dispatcher extension methods. So to dispatch a method (call its targets)
    /// use:
    /// 
    /// Dispatch.Call.AdispatchedMethod(...)
    /// 
    /// If you dyamically load additional assemblies after calling Dispatch.InitializeDispatcher, make sure to call
    /// Dispatch.InitializeDispatcherForAssembly for each such loaded assembly (remember to handle DispatchErrorsException)
    /// 
    /// To add targets in an object instance, call: Dispatch.AddObjectInstanceDispatchTargets. You can optionally call
    /// Dispatch.RemoveObjectInstanceDispatchTargets when disposing the object
    /// 
    /// </summary>
    public static class Dispatch {
        static readonly Dispatcher _instance;

        static Dispatch() {
            _instance = new Dispatcher();

        }

        static public Dispatcher Instance => _instance;

        /// <summary>
        /// For nice invocation of dispatch source methods (which are defined as extension to the Dispatcher type)
        /// </summary>
        /// 
        /// <example>
        ///     Dispatch.Call.EnterOperationMode();
        /// </example>
        ///
        static public Dispatcher Call => _instance;

        /// <summary>
        /// Initialize the method dispatch system
        ///
        /// This will add dispatch sources, verify targets (type/nullability vs. the source) for the currenly loaded
        /// assemblies
        /// </summary>
        /// <exception cref="DispatcherErrorsException">is thrown if there are errors in the verification process</exception>
        static public void InitializeDispatcher() => _instance.InitializeDispatcher();

        /// <summary>
        /// Add dispatch sources, verify targets and add static targets in a given assembly
        /// </summary>
        /// <remarks>You need to call this for any assembly you dynamically loade after calling InitializeDispatcher</remarks>
        /// <param name="assembly">The assembly to use</param>
        /// <exception cref="DispatcherErrorsException">is thrown if there are errors in the verification process</exception>
        static public void InitializeDispatcherForAssembly(Assembly assembly) => _instance.InitializeDispatcherForAssembly(assembly);

        /// <summary>
        /// Add dispatch targets in an object instance
        /// </summary>
        /// <remarks>
        /// You don't have to remove these targets upon disposing the object. The dispatch system hold weak reference to the object
        /// and those targets will be automatically removed when the object is garbage collected
        /// </remarks>
        /// <param name="objectInstance">the object instance containing the targets</param>
        static public void AddObjectInstanceDispatcherTargets(object objectInstance) => _instance.AddObjectInstanceDispatchTargets(objectInstance);

        /// <summary>
        /// Remove dispatch targets in residing in an object instance.
        /// </summary>
        /// <param name="objectInstance">the object instance with the targets to be removed</param>
        static public void RemoveObjectInstanceDispatcherTargets(object objectInstance) => _instance.RemoveObjectInstanceDispatchTargets(objectInstance);
    }


    #region Attributes definitions

    public class SourceFileLocation {
        public string Filename { get; private set; }
        public string MemberName { get; private set; }
        public int LineNumber { get; private set; }

        public SourceFileLocation(string filename, string memberName, int lineNumber) {
            this.Filename = filename;
            this.MemberName = memberName;
            this.LineNumber = lineNumber;
        }

        override public string ToString() => $"{Filename} (Line {LineNumber}) - {MemberName}";
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DispatchTargetAttribute : Attribute {
        public SourceFileLocation SourceFile { get; private set; }

        public DispatchTargetAttribute([CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0) {
            SourceFile = new(filePath, memberName, lineNumber);
        }

        public string? Name { get; set; }

        public int? Order { set; get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DispatchSourceAttribute : Attribute {
        public SourceFileLocation SourceFile { get; private set; }

        public DispatchSourceAttribute([CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0) {
            SourceFile = new(filePath, memberName, lineNumber);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DispatchFilterAttribute : Attribute {

    }

    #endregion

    public class DispatchSource {
        public MethodInfo SourceMethod { get; private set; }
        public DispatchSourceAttribute? DispatchSourceAttribute { get; private set; }

        public DispatchSource(MethodInfo definition) {
            this.SourceMethod = definition;
            this.DispatchSourceAttribute = definition.GetCustomAttribute(typeof(DispatchSourceAttribute)) as DispatchSourceAttribute;
            this.Targets = new DispatchTargets(this);
        }

        public void CallVoid(params object?[] parameters) => Targets.Invoke(parameters);

        public IEnumerable<T> CallCollect<T>(params object?[] parameters) => Targets.InvokeAndCollect(parameters).Cast<T>();

        public T Call<T>(params object?[] parameters) => (T)Targets.Invoke(parameters)!;

        public T? CallNullable<T>(params object?[] parameters) => (T?)Targets.Invoke(parameters);


        internal DispatchTargets Targets { get; private set; }

        internal void VerifyTarget(MethodInfo targetMethod, DispatchTargetAttribute dispatchTargetAttribute) {
            // Ignore the source first parameter (extension method)
            if (SourceMethod.GetParameters().Length - 1 != targetMethod.GetParameters().Length)
                throw new WrongParameterCountException(dispatchTargetAttribute.SourceFile, this, targetMethod);

            VerifyReturnValue(targetMethod, dispatchTargetAttribute);
            SetTargetsRequirment();

            var sourceParameters = SourceMethod.GetParameters();
            var targetParameters = targetMethod.GetParameters();
            var errors = new List<DispatcherException>();

            for (var i = 0; i < targetParameters.Length; i++) {
                try {
                    VerifyParameter(sourceParameters[i+1], dispatchTargetAttribute.SourceFile, targetParameters[i]);
                }
                catch (DispatcherException ex) {
                    errors.Add(ex);
                }
            }

            if (errors.Count > 0)
                throw new DispatcherErrorsException("Verify parameters", errors);
        }

        internal void AddStaticTarget(MethodInfo targetMethod, DispatchTargetAttribute dispatchTargetAttribute) {
            Targets.Add(new StaticDispatchTarget(targetMethod, dispatchTargetAttribute));
        }

        internal void AddInstanceTarget(object objectInstance, MethodInfo method, DispatchTargetAttribute dispatchTargetAttribute) {
            Targets.Add(new ObjectInstanceDispatchTarget(objectInstance, method, dispatchTargetAttribute));
        }

        internal void RemoveTargetsInInstance(object instance) => Targets.RemoveTargetsInObjectInstance(instance);

        internal string SourceFile => $"{SourceMethod.Name}{(DispatchSourceAttribute != null ? $" (at {DispatchSourceAttribute.SourceFile})" : "")}";

        private void SetTargetsRequirment() {
            var returnType = SourceMethod.ReturnType;

            if (returnType == typeof(void) || GetCollectedReturnType(returnType) != null) {
                Targets.AllowMoreThanOneTarget = true;
                Targets.AllowNoTargets = true;
            }
            else {
                Targets.AllowMoreThanOneTarget = false;

                var nullabilityContext = new NullabilityInfoContext().Create(SourceMethod.ReturnParameter);

                // If return type can be null, then 
                Targets.AllowNoTargets =
                    nullabilityContext.ReadState != NullabilityState.NotNull &&
                    (returnType.BaseType == typeof(Task) && nullabilityContext.GenericTypeArguments[0].ReadState != NullabilityState.NotNull);
            }
        }

        // If returnType is IList<T> return T, otherwise return null'
        //
        private static Type? GetCollectedReturnType(Type returnType) =>
            returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>) ?
                returnType.GetGenericArguments().First() :
                returnType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))?.GetGenericArguments().First();

        private void VerifyReturnValue(MethodInfo targetMethod, DispatchTargetAttribute dispatchTargetAttribute) {
            // Check dispatch source has no return value and the target also has no return value, there is nothing to check
            if (SourceMethod.ReturnType == typeof(void)) {
                if (targetMethod.ReturnType != typeof(void))
                    throw new NonCompatibleReturnTypeException(dispatchTargetAttribute.SourceFile, this, targetMethod);
                else
                    return;
            }

            // DispatchSource can either return single return value (if no more than one target is allowed)
            // or a list of values in case multiple targets can be dispatched
            var sourceReturnNullabiliyInfo = new NullabilityInfoContext().Create(SourceMethod.ReturnParameter);
            var collectedType = GetCollectedReturnType(SourceMethod.ReturnType);

            var (actualReturnType, actalReturnNullabilityInfo) = collectedType != null ?
                (collectedType, sourceReturnNullabiliyInfo.GenericTypeArguments[0]) :
                (SourceMethod.ReturnType, sourceReturnNullabiliyInfo);

            bool ok = actualReturnType.Equals(targetMethod.ReturnType);
            ok = ok || targetMethod.ReturnType.IsSubclassOf(actualReturnType);

            if (!ok)
                throw new NonCompatibleReturnTypeException(dispatchTargetAttribute.SourceFile, this, targetMethod);

            if (IsInvalidNullPossible(new NullabilityInfoContext().Create(targetMethod.ReturnParameter), actalReturnNullabilityInfo))
                throw new ReturnValueNullabilityException(dispatchTargetAttribute.SourceFile, this);
        }

        private void VerifyParameter(ParameterInfo sourceParameter, SourceFileLocation targetSourceFile, ParameterInfo targetParameter) {
            bool ok = sourceParameter.ParameterType.Equals(targetParameter.ParameterType);

            if (!ok) {
                if (targetParameter.ParameterType.IsSubclassOf(sourceParameter.ParameterType) || targetParameter.ParameterType.IsInterface) {
                    if (targetParameter.GetCustomAttribute(typeof(DispatchFilterAttribute)) != null)
                        ok = true;
                    else if (targetParameter.ParameterType.IsInterface)
                        throw new InterfaceParameterTypeException(targetSourceFile, this, sourceParameter, targetParameter);
                    else
                        throw new DerviedParameterTypeException(targetSourceFile, this, sourceParameter, targetParameter);
                }
            }

            if (!ok)
                throw new MismatchParameterTypeException(targetSourceFile, this, sourceParameter, targetParameter);

            var context = new NullabilityInfoContext();

            if (IsInvalidNullPossible(context.Create(sourceParameter), context.Create(targetParameter)))
                throw new ParameterNullabilityException(targetSourceFile, this, sourceParameter, targetParameter);

            if (targetParameter.HasDefaultValue && targetParameter.GetCustomAttribute(typeof(DispatchFilterAttribute)) == null)
                throw new TargetWithDefaultValueException(targetSourceFile, targetParameter);
        }

        private static bool IsInvalidNullPossible(NullabilityInfo from, NullabilityInfo to) {
            if (from.ReadState != NullabilityState.NotNull && to.ReadState == NullabilityState.NotNull)
                return true;

            if (from.ElementType != null && to.ElementType != null && from.ElementType.ReadState != NullabilityState.NotNull && to.ElementType.ReadState == NullabilityState.NotNull)
                return true;

            foreach (var (fromTypeParam, toTypeParam) in from.GenericTypeArguments.Zip(to.GenericTypeArguments)) {
                if (fromTypeParam.ReadState != NullabilityState.NotNull && toTypeParam.ReadState == NullabilityState.NotNull)
                    return false;
            }

            return false;
        }
    }

    class DispatchTargets {
        readonly List<DispatchTarget> _targets = new();
        readonly DispatchSource _source;
        int _withOrderCount = 0;
        bool _sorted = true;
        bool _invokeVerified = false;


        public DispatchTargets(DispatchSource source) {
            _source = source;
        }


        internal List<DispatchTarget> Targets {
            get {
                if (_withOrderCount > 0 && !_sorted) {
                    _targets.Sort((t1, t2) => t1.OrderValue - t2.OrderValue);
                    _sorted = true;
                }

                return _targets;
            }
        }

        public int Count => _targets.Count;

        public bool AllowNoTargets { get; set; } = true;

        public bool AllowMoreThanOneTarget { get; set; } = true;

        public void Add(DispatchTarget target) {
            if (!AllowMoreThanOneTarget && _targets.Count > 0)
                throw new MultipleTargetsNotAllowedException(target.SourceFile, _source, _targets[0]);

            _targets.Add(target);

            if (target.Order.HasValue)
                _withOrderCount++;
            _sorted = false;
        }

        public void Remove(DispatchTarget target) {
            if (target.Order.HasValue)
                _withOrderCount--;
            _targets.Remove(target);
        }

        public void RemoveTargetsInObjectInstance(object instance) {
            var removeList = from target in _targets where target is ObjectInstanceDispatchTarget instanceTarget && instanceTarget.Instance == instance select target;

            foreach (var target in removeList)
                Remove(target);
        }

        private void VerifyInvokeParameters(object?[] parameters) {
            if(!_invokeVerified) {
                if(parameters.Length != _source.SourceMethod.GetParameters().Length-1)
                    throw new InvokeIncorrectParametersCount(_source, parameters.Length);

                var sourceParameters = _source.SourceMethod.GetParameters();

                for (var i = 0; i < parameters.Length; i++) {
                    var parameter = parameters[i];

                    if(parameter != null) {
                        if (!sourceParameters[i+1].ParameterType.Equals(parameter.GetType()))
                            throw new InvokeWrongParameterType(_source, sourceParameters[i+1], parameter.GetType());
                    }
                }

                _invokeVerified = true;
            }
        }
        public List<object?> InvokeAndCollect(object?[] parameters) {
            if (!_invokeVerified)
                VerifyInvokeParameters(parameters);

            List<DispatchTarget>? invalidTargets = null;
            List<object?> results = new();

            foreach (var target in _targets) {
                if (target.IsAlive && target.IsApplicable(parameters)) {
                    results.Add(target.Invoke(parameters));
                }
                else {
                    invalidTargets ??= new List<DispatchTarget>();
                    invalidTargets.Add(target);
                }
            }

            invalidTargets?.ForEach(Remove);

            return results;
        }

        void RemoveDeadTargets() {
            var deadTargets = from target in _targets where !target.IsAlive select target;

            foreach (var target in deadTargets)
                Remove(target);
        }

        public object? Invoke(object?[] parameters) {
            if (!_invokeVerified)
                VerifyInvokeParameters(parameters);

            RemoveDeadTargets();

            var applicableTargets = (from target in Targets where target.IsApplicable(parameters) select target).ToArray();

            if (applicableTargets.Length == 0 && !AllowNoTargets)
                throw new ZeroTargetsNotAllowedException(_source, AllowMoreThanOneTarget ? "at least one applicable target" : "one applicable target");

            Debug.Assert(AllowNoTargets || applicableTargets.Length == 1);
            return applicableTargets.Length  > 0 ? applicableTargets[0].Invoke(parameters) : null;
        }
    }

    internal abstract class DispatchTarget {
        bool? _hasFilters;

        protected MethodInfo Method { get; private set; }

        public int? Order { get; private set; }

        public int OrderValue { get => Order ?? 1000; }

        public SourceFileLocation SourceFile { get; private set; }

        protected DispatchTarget(MethodInfo method, DispatchTargetAttribute dispatchTargetAttribute) {
            Method = method;
            Order = dispatchTargetAttribute.Order;
            SourceFile = dispatchTargetAttribute.SourceFile;
        }

        public abstract bool IsAlive { get; }

        public abstract object? Invoke(object?[] parameters);

        public bool IsApplicable(object?[] parameters) {
            Debug.Assert(parameters.Length == Method.GetParameters().Length);

            _hasFilters ??= Method.GetParameters().Any(p => p.GetCustomAttribute(typeof(DispatchFilterAttribute)) != null);

            if (_hasFilters.Value) {
                foreach (var p in Method.GetParameters().Select((parameter, i) => new { TargetParameter = parameter, ParameterValue = parameters[i] })) {
                    if (p.TargetParameter.GetCustomAttribute(typeof(DispatchFilterAttribute)) != null) {

                        // Filter by value
                        if (p.TargetParameter.HasDefaultValue) {
                            if (p.TargetParameter.DefaultValue == null) {
                                if (p.ParameterValue != null)
                                    return false;
                            }
                            else if (!p.TargetParameter.DefaultValue.Equals(p.ParameterValue))
                                return false;
                        }

                        // Filter by type
                        if (p.ParameterValue != null &&  !p.TargetParameter.ParameterType.IsAssignableFrom(p.ParameterValue.GetType()))
                            return false;
                    }
                }
            }

            return true;
        }

    }

    internal class StaticDispatchTarget : DispatchTarget {

        public StaticDispatchTarget(MethodInfo method, DispatchTargetAttribute dispatchTargetAttribute) : base(method, dispatchTargetAttribute) {
        }

        public override bool IsAlive => true;

        public override object? Invoke(object?[] parameters) => Method.Invoke(null, parameters);
    }

    internal class ObjectInstanceDispatchTarget : DispatchTarget {
        readonly WeakReference _instance;

        public ObjectInstanceDispatchTarget(object instance, MethodInfo method, DispatchTargetAttribute dispatchTargetAttribute) : base(method, dispatchTargetAttribute) {
            _instance = new WeakReference(instance);
        }

        public object? Instance => _instance.Target;

        override public bool IsAlive => _instance.IsAlive;

        public override object? Invoke(object?[] parameters) => Instance != null
                ? Method.Invoke(Instance, parameters)
                : throw new ObjectDisposedException($"Instance of dispatch target {Method.Name}");
    }

    #region Exceptions

    public class DispatcherException : Exception {
        protected DispatcherException(string? message) : base(message) {
        }

        protected DispatcherException(SourceFileLocation? location, string? message) : base($"{(location != null ? $"{location}: " : "")}{message ?? ""}") {
        }
    }

    public class DispatcherErrorsException : Exception {
        public List<DispatcherException> Errors { get; private set; }

        internal DispatcherErrorsException(string message, IEnumerable<DispatcherException> dispatcherExceptions) : base(message) {
            Errors = new List<DispatcherException>(dispatcherExceptions);
        }

        /// <summary>
        /// Save the list of dispatch errors to a writer
        /// </summary>
        /// <param name="w">TextWriter to use</param>
        /// <param name="title">Error list title</param>
        public void Save(TextWriter w, string title = "Initializing dispatcher") {
            w.WriteLine($"Errors while {title}:");
            w.WriteLine();

            foreach(var error in Errors)
                w.WriteLine(error.Message);

        }

        /// <summary>
        /// Save the list of errors to a file
        /// </summary>
        /// <param name="title">Error list title</param>
        /// <param name="filename"The file's name></param>
        public void Save(string title = "Initializing dispatcher", string filename = "dispatcher_errors.txt") {
            using var w = new StreamWriter(filename);
            Save(w, title);
        }
    }

    public class AmbiguousDispatchSourcesNameException : DispatcherException {
        internal AmbiguousDispatchSourcesNameException(SourceFileLocation targetSourceFile, DispatchSource[] ambiguousSources) : base(targetSourceFile, $"Ambiguous dispatch sources ({string.Join(' ', ambiguousSources.Select(source => source.SourceMethod.Name))})") {
        }
    }

    public class UndefinedDispatchSourceException : DispatcherException {
        internal UndefinedDispatchSourceException(SourceFileLocation targetSourceFile, string name) : base(targetSourceFile, $"No dispatch source for '{name}'") {

        }
    }

    public class WrongParameterCountException : DispatcherException {
        internal WrongParameterCountException(SourceFileLocation targetSourceFile, DispatchSource source, MethodInfo target) : base(targetSourceFile,
            $"Wrong number of parameters - dispatch source {source.SourceFile} has {source.SourceMethod.GetParameters().Length-1} parameters, dispatch target has {target.GetParameters().Length} parameters") {
        }
    }

    public class NonCompatibleReturnTypeException : DispatcherException {
        internal NonCompatibleReturnTypeException(SourceFileLocation targetSourceFile, DispatchSource source, MethodInfo targetMethod) : base(targetSourceFile,
            $"dispatch target return value has non compatible type ({targetMethod.ReturnType.Name}) with dispatch source return type {source.SourceFile} {source.SourceMethod.ReturnType.Name}") {
        }
    }

    public class ReturnValueNullabilityException : DispatcherException {
        internal ReturnValueNullabilityException(SourceFileLocation targetSourceFile, DispatchSource source) : base(targetSourceFile,
            $"dispatch target may return null as a non-nullable return value of dispatch source {source.SourceFile}") {

        }
    }

    public class MismatchParameterTypeException : DispatcherException {
        internal MismatchParameterTypeException(SourceFileLocation targetSourceFile, DispatchSource source, ParameterInfo sourceParamater, ParameterInfo targetParamter) : base(targetSourceFile,
            $"Dispatch target parameter '{targetParamter.Name}' type ({targetParamter.ParameterType}) is not compatible with source {source.SourceFile} parameter '{sourceParamater.Name}' type ({sourceParamater.ParameterType})") {
        }
    }

    public class DerviedParameterTypeException : DispatcherException {
        internal DerviedParameterTypeException(SourceFileLocation targetSourceFile, DispatchSource source, ParameterInfo sourceParamater, ParameterInfo targetParamter) : base(targetSourceFile,
            $"Dispatch target parameter '{targetParamter.Name}' type ({targetParamter.ParameterType}) " +
            $"is derived from source {source.SourceFile} parameter '{sourceParamater.Name}' type ({sourceParamater.ParameterType}), use [DispatchFilter] to filter by parameter type") {
        }

    }

    public class InterfaceParameterTypeException : DispatcherException {
        internal InterfaceParameterTypeException(SourceFileLocation targetSourceFile, DispatchSource source, ParameterInfo sourceParamater, ParameterInfo targetParamter) : base(targetSourceFile,
            $"Dispatch target parameter '{targetParamter.Name}' type ({targetParamter.ParameterType}) " +
            $"is interface, use [DispatchFilter] to filter source {source.SourceFile} parameter '{sourceParamater.Name}' by interface implementation") {
        }
    }

    public class ParameterNullabilityException : DispatcherException {
        internal ParameterNullabilityException(SourceFileLocation targetSourceFile, DispatchSource source, ParameterInfo sourceParameter, ParameterInfo targetParameter) : base(targetSourceFile,
            $"Dispatch source {source.SourceFile} parameter '{sourceParameter.Name}' is nullable when target parameter '{targetParameter.Name}' is non-nullable") {
        }
    }

    public class TargetWithDefaultValueException : DispatcherException {
        internal TargetWithDefaultValueException(SourceFileLocation targetSourceFile, ParameterInfo targetParameter) : base(targetSourceFile,
            $"Target parameter '{targetParameter.Name} has default value but is not marked with [DispatchFilter]") {
        }
    }

    public class MultipleTargetsNotAllowedException : DispatcherException {
        internal MultipleTargetsNotAllowedException(SourceFileLocation targetSourceFile, DispatchSource source, DispatchTarget alreadyAddedTarget) : base(targetSourceFile,
            $"Dispatch source {source.SourceFile} cannot have multiple dispatch targets, dispatch target {alreadyAddedTarget.SourceFile} was already added") {
        }
    }

    public class ZeroTargetsNotAllowedException : DispatcherException {
        internal ZeroTargetsNotAllowedException(DispatchSource source, string targetRequiement) : base(
            $"Dispatch source {source.SourceFile} must have {targetRequiement}") {

        }
    }

    public class InvokeIncorrectParametersCount : DispatcherException {
        internal InvokeIncorrectParametersCount(DispatchSource source, int count) : base(
            $"Dispatch source {source.SourceFile} called Invoke with wrong number of parameters (called with {count}, expected {source.SourceMethod.GetParameters().Length-1}). Probably bug in your dispatch source implementation") {
        }
    }

    public class InvokeWrongParameterType : DispatcherException {
        internal InvokeWrongParameterType(DispatchSource source, ParameterInfo sourceParameter, Type calledWith) : base(
            $"Dispatch source {source.SourceFile} called invoke with type for parameter {sourceParameter.Name} (expected: {sourceParameter.ParameterType.Name}, called with: {calledWith.Name}, probably bug in your dispatch source implementation") {
        }
    }

    #endregion
}
