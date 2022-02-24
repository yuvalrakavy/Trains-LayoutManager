using MethodDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

namespace LayoutManager {
    #region Event Script Implementations

    #region Event script error handling

    public enum LayoutEventScriptExecutionPhase {
        Unknown, Evaluation, Reset, Cancel, Disposing, EventProcessing, ActionProcessing, ConditionProcessing,
    }

    public class LayoutEventScriptException : Exception {
        public LayoutEventScriptException(LayoutEventScriptNode node, LayoutEventScriptExecutionPhase executionPhase, Exception ex) : base("Event Script Exception", ex) {
            this.Node = node;
            this.ExecutionPhase = executionPhase;
        }

        public LayoutEventScriptException(LayoutEventScriptNode node, string message) : base(message) {
            this.Node = node;
        }

        public LayoutEventScriptNode Node { get; }

        public LayoutEventScriptExecutionPhase ExecutionPhase { get; } = LayoutEventScriptExecutionPhase.Unknown;
    }

    public class LayoutEventScriptErrorInfo {
        public LayoutEventScriptErrorInfo(ILayoutScript script, LayoutEventScriptTask task, LayoutEventScriptNode node, LayoutEventScriptExecutionPhase executionPhase, Exception? ex) {
            this.Script = script;
            this.Task = task;
            this.Node = node;
            this.ExecutionPhase = executionPhase;
            this.Exception = ex;
        }

        public ILayoutScript Script { get; }

        public LayoutEventScriptTask Task { get; }

        public LayoutEventScriptNode Node { get; }

        public LayoutEventScriptExecutionPhase ExecutionPhase { get; }

        public Exception? Exception { get; }
    }

    #endregion

    #region Layout Event Script

    public interface ILayoutScript {
        string Name {
            get;
        }

        LayoutScriptContext ScriptContext {
            get;
        }

        object? ScriptSubject {
            get;
            set;
        }

        string? Description {
            get;
        }
    }

    public class LayoutEventScriptTask : IDisposable {
        private readonly XmlElement scriptElement;
        private readonly ILayoutScript script;
        private bool taskTerminated;

        public LayoutEventScriptTask(ILayoutScript eventScript, XmlElement scriptElement, LayoutScriptContext context) {
            this.script = eventScript;
            this.scriptElement = scriptElement;

            Root = Parse(this.scriptElement, context);
        }

        public LayoutEventScriptNode Parse(XmlElement elementToParse, LayoutScriptContext context) {
            return Dispatch.Call.ParseEventScriptDefinition(new LayoutParseEventScript(script, this, elementToParse, context));
        }

        public LayoutEventScriptNode Root { get; }

        public LayoutEventScriptNodeEventBase EventRoot => (LayoutEventScriptNodeEventBase)Root;

        public LayoutEventScriptNodeCondition ConditionRoot => (LayoutEventScriptNodeCondition)Root;

        private LayoutEventScript EventScript => (LayoutEventScript)script;

        public void RecalculateTask() {
            try {
                EventRoot.Recalculate();
            }
            catch (LayoutEventScriptException ex) {
                Dispatch.Call.EventScriptError(EventScript, new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException));
            }
            catch (Exception ex) {
                Dispatch.Call.EventScriptError(EventScript, new LayoutEventScriptErrorInfo(EventScript, this, Root, LayoutEventScriptExecutionPhase.Evaluation, ex));
            }

            if (EventRoot.Occurred && !taskTerminated) {
                EventScript.OnTaskTerminated(this);
                taskTerminated = true;
                Dispose();
            }
        }

        public void Reset() {
            taskTerminated = false;

            try {
                EventRoot.Reset();
            }
            catch (LayoutEventScriptException ex) {
                Dispatch.Call.EventScriptError(EventScript, new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException));
            }
            catch (Exception ex) {
                Dispatch.Call.EventScriptError(EventScript, new LayoutEventScriptErrorInfo(EventScript, this, Root, LayoutEventScriptExecutionPhase.Reset, ex));
            }

            if (EventRoot.Occurred && !taskTerminated) {
                EventScript.OnTaskTerminated(this);
                taskTerminated = true;
                Dispose();
            }
        }

        public bool Occurred => EventRoot.Occurred;

        public bool IsErrorState => EventRoot.IsErrorState;

        public string Description => Dispatch.Call.GetEventScriptDescription(scriptElement) ?? $"?({scriptElement})?";

        public void Cancel() {
            EventRoot.Cancel();
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                try {
                    EventRoot.Dispose();
                }
                catch (LayoutEventScriptException ex) {
                    Dispatch.Call.EventScriptError(EventScript, new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException));
                }
                catch (Exception ex) {
                    Dispatch.Call.EventScriptError(EventScript, new LayoutEventScriptErrorInfo(EventScript, this, Root, LayoutEventScriptExecutionPhase.Disposing, ex));
                }
            }
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
    }

    public class LayoutEventScript : ILayoutScript, IDisposable, IObjectHasId {
        private readonly Action? scriptDoneAction;
        private readonly Action? errorOccurredAction;
        private readonly XmlElement scriptElement;
        private LayoutScriptContext? scriptContext;
        private readonly List<LayoutEventScriptTask> tasks = new();
        private LayoutEventScriptTask? rootTask;

        public LayoutEventScript(string scriptName, XmlElement scriptElement,
            ICollection<Guid> scopeIDs, Action? scriptDoneAction, Action? errorOccurredAction) {
            this.Name = scriptName;
            this.scriptElement = scriptElement;
            this.ScopeIDs = scopeIDs;
            this.scriptDoneAction = scriptDoneAction;
            this.errorOccurredAction = errorOccurredAction;

            this.Id = Guid.NewGuid();
        }

        public LayoutScriptContext ScriptContext {
            get {
                if (scriptContext == null) {
                    if (ParentContext == null)
                        ParentContext = Dispatch.Call.GetGlobalEventScriptContext();

                    scriptContext = new LayoutScriptContext("Script", ParentContext) {
                        CopyOnClone = false
                    };
                }

                return scriptContext;
            }

            set => scriptContext = value;
        }

        public LayoutScriptContext? ParentContext { get; set; }

        public LayoutEventScriptTask RootTask => rootTask ??= AddTask(scriptElement, ScriptContext);

        public Guid Id { get; set; }

        /// <summary>
        /// The scope for which this event script belongs. This mean that unless otherwise specified, the script
        /// will process only events that are sent (or their info field) is an object with this ID.
        /// </summary>
        public ICollection<Guid> ScopeIDs { get; }

        public object? ScriptSubject { get; set; }

        /// <summary>
        /// The script name, used for identification in scenarios such as debugging
        /// </summary>
        public string Name { get; }

        public LayoutEventScriptTask AddTask(XmlElement scriptElement, LayoutScriptContext context) {
            LayoutEventScriptTask task = new(this, scriptElement, context);

            tasks.Add(task);
            return task;
        }

        internal void OnTaskTerminated(LayoutEventScriptTask task) {
            if (task == RootTask) {
                Dispatch.Notification.OnEventScriptTerminated(this);

                if (task.EventRoot.IsErrorState && errorOccurredAction != null)
                    errorOccurredAction();
                else
                    scriptDoneAction?.Invoke();
            }
            else
                tasks.Remove(task);
        }

        public void Reset() {
            foreach (LayoutEventScriptTask task in tasks)
                if (task != RootTask)
                    task.Cancel();
            tasks.Clear();

            tasks.Add(RootTask);
            Dispatch.Notification.OnEventScriptReset(this, RootTask);

            RootTask.Reset();
        }

        public bool Occurred => RootTask.Occurred;

        public bool IsErrorState => RootTask.IsErrorState;

        public string Description => scriptElement != null ? ((string?)Dispatch.Call.GetEventScriptDescription(scriptElement) ?? $"({scriptElement.Name})") : "(null)";

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                Dispatch.Notification.OnEventScriptDispose(this);

                foreach (LayoutEventScriptTask task in tasks) {
                    try {
                        task.Dispose();
                    }
                    catch (LayoutEventScriptException ex) {
                        Dispatch.Call.EventScriptError(this, new LayoutEventScriptErrorInfo(this, task, ex.Node, ex.ExecutionPhase, ex.InnerException));
                    }
                    catch (Exception ex) {
                        Dispatch.Call.EventScriptError(this, new LayoutEventScriptErrorInfo(this, task, task.Root, LayoutEventScriptExecutionPhase.Disposing, ex));
                    }
                }

                tasks.Clear();
                if (rootTask != null) {
                    rootTask.Dispose();
                    rootTask = null;
                }

            }
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
    }

    #endregion

    #region Layout Condition Script

    public class LayoutConditionScript : ILayoutScript {
        private readonly XmlElement element;
        private LayoutScriptContext? scriptContext;
        private readonly bool defaultValue = true;

        public LayoutConditionScript(XmlElement element) {
            this.element = element;
            this.Name = "Condition";

            Initialize();
        }

        public LayoutConditionScript(XmlElement element, bool defaultValue) {
            this.element = element;
            this.Name = "Condition";
            this.defaultValue = defaultValue;

            Initialize();
        }

        public LayoutConditionScript(string name, XmlElement element) {
            this.element = element;
            this.Name = name;

            Initialize();
        }

        public LayoutConditionScript(string name, XmlElement element, bool defaultValue) {
            this.element = element;
            this.Name = name;
            this.defaultValue = defaultValue;

            Initialize();
        }

        private void Initialize() {
        }

        public bool IsTrue {
            get {
                if (element == null || element.ChildNodes.Count < 1)
                    return defaultValue;

                using LayoutEventScriptTask task = new(this, (XmlElement)element.ChildNodes[0]!, ScriptContext);

                return task.ConditionRoot.IsTrue;
            }
        }

        #region ILayoutScript Members

        public string Name { get; set; }

        public LayoutScriptContext ScriptContext {
            get {
                if (scriptContext == null) {
                    LayoutScriptContext? globalContext = Dispatch.Call.GetGlobalEventScriptContext();

                    scriptContext = new LayoutScriptContext("Script", globalContext) {
                        CopyOnClone = false
                    };
                }

                return scriptContext;
            }
        }

        public object? ScriptSubject { get; set; }

        public string Description {
            get {
                if (element != null && element.ChildNodes.Count > 0 && element.ChildNodes[0] is XmlElement child)
                    return Dispatch.Call.GetEventScriptDescription(child) ?? $"({child.Name})";
                else
                    return "";
            }
        }

        #endregion
    }

    #endregion

    #region Utility classes for event script parsing

    /// <summary>
    /// A reference to this object is passed as the sender of the parse-conditional-event-defintion
    /// event.
    /// </summary>
    public class LayoutParseEventScript : IObjectHasXml {
        public LayoutParseEventScript(ILayoutScript script, LayoutEventScriptTask eventScriptTask, XmlElement element, LayoutScriptContext context) {
            this.Script = script;
            this.Task = eventScriptTask;
            this.Element = element;
            this.Context = context;
        }

        /// <summary>
        /// The conditonal event being built
        /// </summary>
        public ILayoutScript Script { get; }

        /// <summary>
        /// Return the event script task for which this element is parsed
        /// </summary>
        public LayoutEventScriptTask Task { get; }

        /// <summary>
        /// The XML element representing the condition
        /// </summary>
        public XmlElement Element { get; }
        public XmlElement? OptionalElement => Element;

        /// <summary>
        /// Return the context associated with this node
        /// </summary>
        public LayoutScriptContext Context { get; set; }
    }

    public class LayoutEventScriptParseException : Exception {
        public LayoutEventScriptParseException(ILayoutScript script,
            XmlElement parsedNode, string message) : base(message) {
            this.Script = script;
            this.ParsedNode = parsedNode;
        }

        public ILayoutScript Script { get; }

        public XmlElement ParsedNode { get; }
    }

    #endregion

    #region Context

    public interface ILayoutScriptContextResolver {
        object? Resolve(LayoutScriptContext context, string symbolName);
    }

    public class LayoutScriptContext : ICloneable {
        private readonly Dictionary<string, object?> symbols;

        public LayoutScriptContext(string name, LayoutScriptContext? parentContext) {
            this.Name = name;
            this.ParentContext = parentContext;
            symbols = new Dictionary<string, object?>();
        }

        protected LayoutScriptContext(string name, LayoutScriptContext? parentContext, Dictionary<string, object?> symbols) {
            this.Name = name;
            this.ParentContext = parentContext;
            this.symbols = symbols;
        }

        public string Name { get; }

        public bool CopyOnClone { get; set; } = true;

        public LayoutScriptContext? ParentContext { get; }

        public object? SearchSymbol(string symbolName) {
            symbols.TryGetValue(symbolName, out object? symbolValue);
            return symbolValue;
        }

        public LayoutScriptContext? SearchContext(string name) {
            if (name == "Parent") {
                if (ParentContext != null)
                    return ParentContext;
            }
            else if (name == "This")
                return this;
            else {
                for (LayoutScriptContext? context = this; context != null; context = context.ParentContext)
                    if (name == context.Name)
                        return context;
            }

            return null;
        }

        public object? this[string symbolName] {
            get {
                string[] symbolParts = symbolName.Split(':');
                object? symbolValue = null;
                string name = symbolName;

                if (symbolParts.Length == 1) {
                    for (LayoutScriptContext? context = this; context != null; context = context.ParentContext) {
                        symbolValue = context.SearchSymbol(symbolName);
                        if (symbolValue != null)
                            break;
                    }
                }
                else if (symbolParts.Length == 2) {
                    LayoutScriptContext? context = SearchContext(symbolParts[0]);

                    if (context != null) {
                        name = symbolParts[1];
                        symbolValue = context.SearchSymbol(name);
                    }
                }
                else
                    throw new ArgumentException("Invalid event script symbol reference: " + symbolName);

                if (symbolValue is ILayoutScriptContextResolver symbolIsContextResolver)
                    symbolValue = symbolIsContextResolver.Resolve(this, name);

                return symbolValue;
            }

            set {
                string[] symbolParts = symbolName.Split(':');

                if (symbolParts.Length == 1)
                    symbols[symbolName] = value;
                else {
                    LayoutScriptContext? context = SearchContext(symbolParts[0]);

                    if (context != null)
                        context[symbolParts[1]] = value;
                    else
                        throw new ArgumentException("Event script context named '" + symbolParts[0] + "' is not defined");
                }
            }
        }

        public void Remove(string symbolName) {
            symbols.Remove(symbolName);
        }

        /// <summary>
        /// Check if a given instance of symbol is already in the context. This can save time by eliminating creation
        /// of duplicate context entries (for example, the Script context probably contains definition for symbol "Train"
        /// this object is also the sender of multiple events. Rather of each event reinsert this symbol into the context
        /// it can do so, only if this instance of train is not already in the context)
        /// </summary>
        /// <param name="symbolName">The symbol name to check</param>
        /// <param name="oSymbolValue">The symbol value</param>
        /// <returns></returns>
        public bool Contains(string symbolName, object rawSymbolValue) => rawSymbolValue is IObjectHasId symbolValue
              && symbols.TryGetValue(symbolName, out object? previousValueObject) && previousValueObject is IObjectHasId previousValue && previousValue.Id == symbolValue.Id;

        public static object? GetProperty(string symbolName, object symbolValue, string propertyName) {
            var propertyInfo = symbolValue.GetType()?.GetProperty(propertyName);

            if (propertyInfo == null) {
                var infoPropertyInfo = symbolValue.GetType()?.GetProperty("Info");

                if (infoPropertyInfo != null) {
                    var infoObject = infoPropertyInfo.GetValue(symbolValue, null);

                    if (infoObject != null) {
                        propertyInfo = infoObject.GetType().GetProperty(propertyName);

                        if (propertyInfo != null)
                            return propertyInfo.GetValue(infoObject, null);
                    }
                }

                throw new ArgumentException("Object " + symbolName + " does not have property named " + propertyName);
            }

            return propertyInfo.GetValue(symbolValue, null);
        }

        public object? GetSymbolProperty(string symbolName, string propertyName) {
            object? symbolValue = this[symbolName];

            if (symbolValue != null)
                return GetProperty(symbolName, symbolValue, propertyName);
            else
                throw new ArgumentException("Event script context does not contain object named " + symbolName);
        }

        public static object? GetAttribute(string symbolName, object symbolValue, string attributeName) {
            if (symbolValue is IObjectHasAttributes symbolWithAttributes)
                return symbolWithAttributes.Attributes[attributeName];
            else
                throw new ArgumentException("Object " + symbolName + " does not support attributes");
        }

        public object? GetSymbolAttribute(string symbolName, string attributeName) {
            object? symbolValue = this[symbolName];

            if (symbolValue != null)
                return GetAttribute(symbolName, symbolValue, attributeName);
            else
                throw new ArgumentException("Event script context does not contain object named " + symbolName);
        }

        public object Clone() {
            if (!CopyOnClone)
                return this;

            var newSymbols = new Dictionary<string, object?>(symbols.Count);

            foreach (KeyValuePair<string, object?> d in symbols)
                newSymbols.Add(d.Key, d.Value);

            return new LayoutScriptContext(Name, (LayoutScriptContext?)ParentContext?.Clone(), newSymbols);
        }
    }

    #endregion

    #region Base classes for the various nodes (event, event container, condition, condition container, acion)

    public interface IIfTimeNode : IObjectHasXml {
        bool InRange(int v);

        string Description { get; }

        bool IsRange { get; }

        int Value { get; set; }

        int From { get; set; }

        int To { get; set; }
    }

    /// <summary>
    /// Base class for all nodes
    /// </summary>
    public abstract class LayoutEventScriptNode : IObjectHasXml {
        private readonly LayoutParseEventScript parseEventInfo;

        protected LayoutEventScriptNode(LayoutParseEventScript parseEventInfo) {
            this.parseEventInfo = parseEventInfo;
        }

        public XmlElement Element => parseEventInfo.Element;
        public XmlElement? OptionalElement => Element;

        public ILayoutScript Script => parseEventInfo.Script;

        public LayoutEventScriptTask Task => parseEventInfo.Task;

        public LayoutScriptContext Context {
            get => parseEventInfo.Context;
            set => parseEventInfo.Context = value;
        }

        protected LayoutEventScriptNode? Parse(XmlElement elementToParse) => Task.Parse(elementToParse, Context);

        protected LayoutEventScriptParseException ParseErrorException(string message) => new(Script, Element, message);

        #region Common Access methods

        protected static object? GetOperand(XmlElement element, string symbolName, object symbolValue, string symbolAccess, string suffix) {
            string tagName = element.GetAttribute("Name" + suffix);

            if (symbolAccess == "Property")
                return LayoutScriptContext.GetProperty(symbolName, symbolValue, tagName);
            else if (symbolAccess == "Attribute")
                return LayoutScriptContext.GetAttribute(symbolName, symbolValue, tagName);
            else
                throw new ArgumentException("Invalid symbol access method " + symbolAccess + " for symbol " + symbolName);
        }

        protected object? GetOperand(string symbolName, object symbolValue, string symbolAccess, string suffix) => GetOperand(Element, symbolName, symbolValue, symbolAccess, suffix);

        protected object? GetOperand(XmlElement element, string suffix) {
            string symbolAccess = element.GetAttribute("Symbol" + suffix + "Access");

            if (symbolAccess == "Value") {
                var v = element.AttributeValue($"Value{suffix}");

                return (element.GetAttribute("Type" + suffix)) switch {
                    "Boolean" => (bool)v,
                    "Integer" => (int)v,
                    "Double" => (double)v,
                    _ => (string?)v ?? "",
                };
            }
            else {
                string symbolName = element.GetAttribute("Symbol" + suffix);
                var symbolValue = Context[symbolName];

                return symbolValue == null
                    ? null
                    : symbolValue.GetType().IsArray ? symbolValue : GetOperand(element, symbolName, symbolValue, symbolAccess, suffix);
            }
        }

        protected object? GetOperand(string suffix) => GetOperand(Element, suffix);

        #endregion
    }

    #region Events

    public abstract class LayoutEventScriptNodeEventBase : LayoutEventScriptNode, IDisposable {
        private const string A_Optional = "Optional";
        private const string A_IsError = "IsError";
        private bool _occurred;
        private bool _isErrorState;
        private LayoutEventScriptNodeCondition? condition;
        private LayoutEventScriptNodeActions? actions;

        protected LayoutEventScriptNodeEventBase(LayoutParseEventScript  parseEventInfo) : base(parseEventInfo) {
            EventManager.AddObjectSubscriptions(this);
            Dispatch.AddObjectInstanceDispatcherTargets(this);
        }

        /// <summary>
        /// Recalculate the whether this event occurred. If the event took place, set Occurred to true
        /// and call RecalcTask().
        /// </summary>
        public virtual void Recalculate() {
        }

        /// <summary>
        /// Cancel the event. (For example, cancel timed wait, remove subscription etc.)
        /// </summary>
        public abstract void Cancel();

        /// <summary>
        /// Release all resources allocated by the node, do not forget to call base.Dispose()
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing)
                EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        /// <summary>
        /// Parse 'Condition' element
        /// </summary>
        /// <param name="eventElement">The event XML element</param>
        protected void ParseCondition(XmlElement eventElement) {
            var conditionTitleElement = eventElement["Condition"];

            if (conditionTitleElement != null) {
                if (conditionTitleElement.ChildNodes.Count != 1)
                    throw ParseErrorException("Missing condition, or more than one condition for event " + Element.Name);

                XmlElement conditionElement = (XmlElement)conditionTitleElement.ChildNodes[0]!;

                condition = Parse(conditionElement) as LayoutEventScriptNodeCondition;

                if (condition == null)
                    throw ParseErrorException("Invalid condition or condition container: " + conditionElement.Name);
            }
        }

        /// <summary>
        /// Parse 'Actions' element
        /// </summary>
        /// <param name="eventElement">The event XML element</param>
        protected void ParseActions(XmlElement eventElement) {
            var actionsElement = eventElement["Actions"];

            if (actionsElement != null) {
                actions = (LayoutEventScriptNodeActions?)Parse(actionsElement);
            }
        }

        /// <summary>
        /// Get/Set whether the event occured
        /// </summary>
        public bool Occurred {
            get => _occurred;

            set {
                if (value && !_occurred) {
                    StateChanged = true;
                    _occurred = true;

                    try {
                        if (Actions != null && !CancelActions)
                            Actions.Execute();
                    }
                    catch (Exception ex) {
                        throw new LayoutEventScriptException(this, LayoutEventScriptExecutionPhase.ActionProcessing, ex);
                    }

                    RecalculateTask();
                }
                else
                    _occurred = value;
            }
        }

        /// <summary>
        /// If set to true, actions will not be performed when the event occurs
        /// </summary>
        public bool CancelActions { get; set; }

        /// <summary>
        /// Check the condition. If the event is not associated with condition, the condition assumed to be true
        /// </summary>
        protected bool IsConditionTrue {
            get {
                try {
                    if (Condition?.IsTrue != false)
                        return true;
                }
                catch (Exception ex) {
                    throw new LayoutEventScriptException(this, LayoutEventScriptExecutionPhase.ConditionProcessing, ex);
                }

                return false;
            }
        }

        /// <summary>
        /// Get/Set whether the occurrence of this event signify an error state
        /// </summary>
        public bool IsErrorState {
            get => _isErrorState;

            set {
                if (value != _isErrorState && value)
                    StateChanged = true;

                _isErrorState = value;
            }
        }

        public bool IsOptional => (bool?)Element.AttributeValue(A_Optional) ?? false;

        /// <summary>
        /// Get/Set whether the event state has changed (you should not normally set this directly)
        /// </summary>
        protected bool StateChanged { get; set; }

        /// <summary>
        /// Reset the event
        /// </summary>
        public virtual void Reset() {
            Occurred = false;
            CancelActions = false;
            IsErrorState = (bool?)Element.AttributeValue(A_IsError) ?? false;
        }

        /// <summary>
        /// Re-calculate the script. This should be called whenever an event occurred.
        /// </summary>
        protected void RecalculateTask() {
            bool shouldRecalc = StateChanged;

            StateChanged = false;

            if (shouldRecalc)
                Task.RecalculateTask();
        }

        /// <summary>
        /// Get the condition associated with this event
        /// </summary>
        protected LayoutEventScriptNodeCondition? Condition => condition;

        /// <summary>
        /// Get the actions to perform when this event occurred
        /// </summary>
        protected LayoutEventScriptNodeActions? Actions => actions;
    }

    /// <summary>
    /// Base class events (or event containers)
    /// </summary>
    public class LayoutEventScriptNodeEvent : LayoutEventScriptNodeEventBase {
        private const string A_Name = "Name";
        private const string A_LimitToScope = "LimitToScope";
        private readonly string? eventName;
        private LayoutEventSubscriptionBase? subscription;

        public LayoutEventScriptNodeEvent(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            eventName = (string?)Element.AttributeValue(A_Name);
            ParseCondition(Element);
            ParseActions(Element);
        }

        protected bool LimitToScope => (bool?)Element.AttributeValue(A_LimitToScope) ?? true;

        /// <summary>
        /// You may override this with the event name to subscribe to
        /// </summary>
        protected virtual string? EventName => eventName;

        public override void Reset() {
            base.Reset();

            try {
                if (!Occurred && EventName != null) {
                    LayoutEventAttribute subscriptionInfo = new(EventName);

                    subscription = subscriptionInfo.CreateSubscription();
                    subscription.SetFromLayoutEventAttribute(subscriptionInfo);
                    subscription.Order = 10000;         // Ensure that built in event handler execute first
                    var method = this.GetType().GetMethod("OnEvent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (method != null) {
                        subscription.SetMethod(this, method);

                        EventManager.Subscriptions.Add(subscription);
                    }
                }

                EventManager.Event(new LayoutEvent("event-script-wait-event-reset", this, subscription));
            }
            catch (Exception ex) {
                throw new LayoutEventScriptException(this, LayoutEventScriptExecutionPhase.Reset, ex);
            }
        }

        public override void Cancel() {
            if (subscription != null) {
                EventManager.Subscriptions.Remove(subscription);
                subscription = null;
            }
        }

        protected virtual void OnEvent(LayoutEvent e) {
            bool relevantEvent = false;

            try {
                LayoutEventScript eventScript = (LayoutEventScript)Script;

                // Check if the event need to be limited to scope
                if (LimitToScope && eventScript.ScopeIDs != null && eventScript.ScopeIDs.Count != 0) {
                    // Check that the sender or info ID is matching the scopeID. If this is not the case,
                    // the event is not relevant to the scope of the current event script

                    if (e.Sender is IObjectHasId sender) {
                        foreach (Guid scopeID in eventScript.ScopeIDs)
                            if (scopeID == sender.Id) {
                                relevantEvent = true;
                                break;
                            }
                    }

                    if (!relevantEvent && e.Info is IObjectHasId id) {
                        IObjectHasId info = id;

                        foreach (Guid scopeID in eventScript.ScopeIDs)
                            if (scopeID == info.Id) {
                                relevantEvent = true;
                                break;
                            }
                    }
                }
                else
                    relevantEvent = true;

                if (relevantEvent) {
                    if (e.Sender != null)
                        Dispatch.Call.SetScriptContext(e.Sender, Context);
                    if (e.Info != null)
                        Dispatch.Call.SetScriptContext(e.Info, Context);

                    if (IsConditionTrue) {
                        Cancel();
                        Occurred = true;
                    }
                }
            }
            catch (LayoutEventScriptException ex) {
                Dispatch.Call.EventScriptError((LayoutEventScript)this.Script, new LayoutEventScriptErrorInfo(this.Script, this.Task, ex.Node, ex.ExecutionPhase, ex.InnerException));
            }
            catch (Exception ex) {
                Dispatch.Call.EventScriptError((LayoutEventScript)this.Script, new LayoutEventScriptErrorInfo(this.Script, this.Task, this, LayoutEventScriptExecutionPhase.EventProcessing, ex));
            }
        }
    }

    public abstract class LayoutEventScriptNodeEventContainer : LayoutEventScriptNodeEventBase {
        private readonly List<LayoutEventScriptNodeEventBase> events = new();

        protected LayoutEventScriptNodeEventContainer(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            var eventsElement = Element["Events"];

            // Create new context for the event container
            Context = new LayoutScriptContext(Element.Name, Context);

            if (eventsElement != null) {
                foreach (XmlElement eventElement in eventsElement) {
                    if (Parse(eventElement) is LayoutEventScriptNodeEventBase eventNode)
                        Add(eventNode);
                    else
                        throw ParseErrorException("Invalid event or event container: " + eventElement.Name);
                }
            }

            ParseActions(Element);
            ParseCondition(Element);
        }

        public void Add(LayoutEventScriptNodeEventBase eventNode) {
            events.Add(eventNode);
        }

        public IList<LayoutEventScriptNodeEventBase> Events => events.AsReadOnly();

        public override void Cancel() {
            foreach (LayoutEventScriptNodeEventBase eventNode in events)
                eventNode.Cancel();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                foreach (LayoutEventScriptNodeEventBase eventNode in events)
                    eventNode.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    #endregion

    #region Conditions

    public abstract class LayoutEventScriptNodeCondition : LayoutEventScriptNode {
        protected LayoutEventScriptNodeCondition(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
        }

        /// <summary>
        /// Is the condition true
        /// </summary>
        public abstract bool IsTrue {
            get;
        }
    }

    public abstract class LayoutEventScriptNodeConditionContainer : LayoutEventScriptNodeCondition {
        private readonly List<LayoutEventScriptNodeCondition> conditions = new();

        protected LayoutEventScriptNodeConditionContainer(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            foreach (XmlElement conditionElement in Element) {
                if (Parse(conditionElement) is not LayoutEventScriptNodeCondition condition)
                    throw ParseErrorException("Invalid condition: " + conditionElement.Name);

                conditions.Add(condition);
            }
        }

        public IList<LayoutEventScriptNodeCondition> Conditions => conditions;
    }

    #endregion

    #region Actions

    public abstract class LayoutEventScriptNodeAction : LayoutEventScriptNode {
        protected LayoutEventScriptNodeAction(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
        }

        /// <summary>
        /// Execute the action
        /// </summary>
        public abstract void Execute();
    }

    public class LayoutEventScriptNodeActions : LayoutEventScriptNode {
        private readonly ArrayList actions = new();

        public LayoutEventScriptNodeActions(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            foreach (XmlElement elementAction in Element) {
                if ((LayoutEventScriptNodeAction?)Parse(elementAction) is not LayoutEventScriptNodeAction action)
                    throw ParseErrorException("Invalid action: " + elementAction.Name);

                actions.Add(action);
            }
        }

        public void Execute() {
            foreach (LayoutEventScriptNodeAction action in actions)
                action.Execute();
        }
    }

    #endregion

    #endregion

    #endregion

    #region Standard event nodes
#pragma warning disable IDE0051

    [LayoutModule("Common Event Condition Handlers")]

    internal class CommonEventConditionHandler : LayoutModuleBase {
        #region Event Containers

        #region Any - Any condition occured

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Any([DispatchFilter("XPath", "Any")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventContainerAny(parseEventInfo);

        private class LayoutEventScriptNodeEventContainerAny : LayoutEventScriptNodeEventContainer {
            public LayoutEventScriptNodeEventContainerAny(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            }

            public override void Recalculate() {
                if (!Occurred) {
                    LayoutEventScriptNodeEventBase? occuredEvent = null;

                    foreach (LayoutEventScriptNodeEventBase eventNode in Events) {
                        eventNode.Recalculate();

                        if (eventNode.Occurred && !eventNode.IsOptional) {
                            occuredEvent = eventNode;
                            break;
                        }
                    }

                    if (occuredEvent != null) {
                        foreach (LayoutEventScriptNodeEventBase eventNode in Events) {
                            if (eventNode != occuredEvent)
                                eventNode.Cancel();
                        }

                        if (!IsConditionTrue)
                            CancelActions = true;

                        Occurred = true;
                    }
                }
            }

            public override void Reset() {
                base.Reset();

                foreach (LayoutEventScriptNodeEventBase eventNode in Events)
                    eventNode.Reset();
            }
        }

        #endregion

        #region All - All conditions occured (order does not matter)

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_All([DispatchFilter("XPath", "All")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventContainerAll(parseEventInfo);

        private class LayoutEventScriptNodeEventContainerAll : LayoutEventScriptNodeEventContainer {
            public LayoutEventScriptNodeEventContainerAll(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            }

            public override void Recalculate() {
                if (!Occurred) {
                    bool failed = false;
                    bool isErrorState = false;

                    foreach (LayoutEventScriptNodeEventBase eventNode in Events) {
                        eventNode.Recalculate();

                        if (!eventNode.Occurred && !eventNode.IsOptional)
                            failed = true;
                        else {
                            if (eventNode.IsErrorState)
                                isErrorState = true;
                        }
                    }

                    if (!failed) {
                        foreach (LayoutEventScriptNodeEventBase eventNode in Events) {
                            if (!eventNode.Occurred)
                                eventNode.Cancel();
                        }

                        IsErrorState = isErrorState;

                        if (!IsConditionTrue)
                            CancelActions = true;

                        Occurred = true;
                    }
                }
            }

            public override void Reset() {
                base.Reset();

                foreach (LayoutEventScriptNodeEventBase eventNode in Events) {
                    eventNode.Reset();
                }
            }
        }

        #endregion

        #region Sequence - All conditions occured in sequence (order matters)

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Sequence([DispatchFilter("XPath", "Sequence")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventContainerSequence(parseEventInfo);

        private class LayoutEventScriptNodeEventContainerSequence : LayoutEventScriptNodeEventContainer {
            private int seqIndex;

            public LayoutEventScriptNodeEventContainerSequence(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            }

            public override void Recalculate() {
                if (!Occurred) {
                    bool proceed = true;

                    while (proceed) {
                        Events[seqIndex].Recalculate();

                        if (Events[seqIndex].Occurred) {
                            if (Events[seqIndex].IsErrorState)
                                IsErrorState = true;

                            if (seqIndex < Events.Count - 1) {
                                seqIndex++;
                                Events[seqIndex].Reset();
                            }
                            else {
                                Occurred = true;        // Sequence is done
                                proceed = false;
                            }
                        }
                        else
                            proceed = false;
                    }
                }
            }

            public override void Reset() {
                base.Reset();

                seqIndex = 0;

                if (IsConditionTrue) {
                    if (Events.Count > 0)
                        Events[seqIndex].Reset();
                }
                else {
                    CancelActions = true;
                    Occurred = true;

                    // TODO: Add support for ElseEvents and ElseActions sections
                }
            }
        }

        #endregion

        #region Repeat - condition happends given number of times

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Repeat([DispatchFilter("XPath", "Repeat")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventContainerRepeat(parseEventInfo);

        private class LayoutEventScriptNodeEventContainerRepeat : LayoutEventScriptNodeEventContainer {
            private const string A_Count = "Count";
            private readonly int numberOfIterations;
            private int iterationCount;
            private readonly LayoutEventScriptNodeEventBase repeatedEvent;

            public LayoutEventScriptNodeEventContainerRepeat(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
                if (!Element.HasAttribute(A_Count))
                    throw ParseErrorException("Missing iteration count in repeat (Count)");

                numberOfIterations = (int)Element.AttributeValue(A_Count);

                if (Element.ChildNodes.Count < 1)
                    throw ParseErrorException("Missing element to repeat");
                else if (Element.ChildNodes.Count > 1)
                    throw ParseErrorException("Too many elements to repeat");

                var repeatedElement = (XmlElement)Element.ChildNodes[0]!;

                repeatedEvent = Parse(repeatedElement) as LayoutEventScriptNodeEventBase ?? throw ParseErrorException("Repeated element " + repeatedElement.Name + " is not valid event definition");
            }

            public override void Reset() {
                base.Reset();

                iterationCount = 0;
                repeatedEvent.Reset();
            }

            public override void Recalculate() {
                if (!Occurred) {
                    repeatedEvent.Recalculate();

                    if (repeatedEvent.Occurred) {
                        if (repeatedEvent.IsErrorState)
                            IsErrorState = true;

                        if (numberOfIterations >= 0 && iterationCount++ >= numberOfIterations)
                            Occurred = true;
                        else
                            repeatedEvent.Reset();
                    }
                }
            }

            public override void Cancel() {
                repeatedEvent.Cancel();
                base.Cancel();
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    repeatedEvent.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        #endregion

        #region Random Choice

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_RandomChoice([DispatchFilter("XPath", "RandomChoice")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventContainerRandomChoice(parseEventInfo);

        private class LayoutEventScriptNodeEventContainerRandomChoice : LayoutEventScriptNodeEventContainer {
            private const string A_Choice = "Choice";
            private const string A_Weight = "Weight";
            private readonly List<ChoiceEntry> choices = new();
            private LayoutEventScriptNodeEventBase? chosenNode;

            public LayoutEventScriptNodeEventContainerRandomChoice(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
                foreach (XmlElement choiceElement in Element.GetElementsByTagName(A_Choice)) {
                    var weight = (int)choiceElement.AttributeValue(A_Weight);
                    var node = choiceElement.ChildNodes[0] is XmlElement childElement ? (LayoutEventScriptNodeEventBase?)Parse(childElement) : null;

                    if (node != null)
                        choices.Add(new ChoiceEntry(weight, node));
                }
            }

            public override void Reset() {
                base.Reset();

                if (choices.Count == 1) {
                    int random = new Random().Next(100);
                    ChoiceEntry choice = choices[0];

                    if (random < choice.Weight)
                        chosenNode = choice.Node;
                }
                else {
                    int total = 0;

                    foreach (ChoiceEntry choice in choices)
                        total += choice.Weight;

                    int random = new Random().Next(total);

                    total = 0;
                    foreach (ChoiceEntry choice in choices) {
                        if (total + choice.Weight > random) {
                            chosenNode = choice.Node;
                            break;
                        }
                        else
                            total += choice.Weight;
                    }
                }

                if (chosenNode != null) {
                    chosenNode.Reset();
                }
                else
                    Occurred = true;
            }

            public override void Recalculate() {
                if (!Occurred && chosenNode != null) {
                    chosenNode.Recalculate();

                    if (chosenNode.Occurred)
                        Occurred = true;
                }
            }

            public override void Cancel() {
                chosenNode?.Cancel();
                base.Cancel();
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    chosenNode?.Dispose();
                }

                base.Dispose(disposing);
            }

            private class ChoiceEntry {
                public ChoiceEntry(int weight, LayoutEventScriptNodeEventBase node) {
                    this.Weight = weight;
                    this.Node = node;
                }

                public int Weight { get; }

                public LayoutEventScriptNodeEventBase Node { get; }
            }
        }

        #endregion

        #region Task

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Task([DispatchFilter("XPath", "Task")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventContainerTask(parseEventInfo);

        private class LayoutEventScriptNodeEventContainerTask : LayoutEventScriptNodeEventBase {
            private readonly XmlElement taskElement;

            public LayoutEventScriptNodeEventContainerTask(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
                if (Element.ChildNodes.Count < 1)
                    throw ParseErrorException("Missing element to repeat");
                else if (Element.ChildNodes.Count > 1)
                    throw ParseErrorException("Too many elements to repeat");

                taskElement = (XmlElement)Element.ChildNodes[0]!;
            }

            public override void Recalculate() {
                if (!Occurred) {
                    LayoutEventScript eventScript = (LayoutEventScript)Script;

                    LayoutEventScriptTask task = eventScript.AddTask(taskElement, (LayoutScriptContext)Context.Clone());

                    task.Reset();
                    Occurred = true;
                }
            }

            public override void Cancel() {
            }

            public override void Reset() {
                base.Reset();
                Recalculate();
            }
        }

        #endregion

        #endregion

        #region Events

        #region Wait for Event

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_WaitForEvent([DispatchFilter("XPath", "WaitForEvent")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEvent(parseEventInfo);

        #endregion

        #region Wait

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Wait([DispatchFilter("XPath", "Wait")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventWait(parseEventInfo);

        private class LayoutEventScriptNodeEventWait : LayoutEventScriptNodeEvent {
            private const string A_MilliSeconds = "MilliSeconds";
            private const string A_Seconds = "Seconds";
            private const string A_Minutes = "Minutes";
            private const string A_RandomSeconds = "RandomSeconds";
            private readonly int delay;
            private LayoutDelayedEvent? delayedEvent;

            public LayoutEventScriptNodeEventWait(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
                delay = ((int?)Element.AttributeValue(A_MilliSeconds) ?? 0)
                    + (((int?)Element.AttributeValue(A_Seconds) ?? 0) * 1000)
                    + (((int?)Element.AttributeValue(A_Minutes) ?? 0) * 1000 * 60);

                if (Element.HasAttribute(A_RandomSeconds))
                    delay += new Random().Next((int)Element.AttributeValue(A_RandomSeconds) * 1000);
            }

            public override void Reset() {
                // Note that the reset operation may set Occurred to true, if the condition is pre-checked and is true
                base.Reset();

                if (delayedEvent != null) {
                    delayedEvent.Cancel();
                    delayedEvent = null;
                }

                if (!Occurred)
                    delayedEvent = EventManager.DelayedEvent(delay, () => WaitConditionDone());
            }

            public override void Cancel() {
                if (delayedEvent != null) {
                    delayedEvent.Cancel();
                    delayedEvent = null;
                }
            }

            protected override void Dispose(bool disposing) {
                if (disposing) {
                    if (delayedEvent != null) {
                        delayedEvent = null;
                    }
                }

                base.Dispose(disposing);
            }

            private void WaitConditionDone() {
                try {
                    if (delayedEvent != null) {         // Make sure that the event was not canceled (race condition)
                        if (IsConditionTrue)
                            Occurred = true;
                        else        // Condition is not true, wait again and check condition again
                            delayedEvent = EventManager.DelayedEvent(delay, () => WaitConditionDone());
                    }
                }
                catch (Exception ex) {
                    Trace.WriteLine("Exception thrown while processing wait condition");
                    Trace.WriteLine("-- " + ex.Message);
                    Trace.WriteLine("-- " + ex.StackTrace);
                }
            }
        }

        #endregion

        #region DoNow

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_DoNow([DispatchFilter("XPath", "DoNow")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeEventDoNow(parseEventInfo);

        private class LayoutEventScriptNodeEventDoNow : LayoutEventScriptNodeEvent {
            public LayoutEventScriptNodeEventDoNow(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            }

            public override void Reset() {
                base.Reset();

                if (!IsConditionTrue)
                    CancelActions = true;

                Occurred = true;
            }
        }

        #endregion

        #endregion

        #region Condition Containers

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_And([DispatchFilter("XPath", "And")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeConditionContainerAnd(parseEventInfo);

        public class LayoutEventScriptNodeConditionContainerAnd : LayoutEventScriptNodeConditionContainer {
            public LayoutEventScriptNodeConditionContainerAnd(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override bool IsTrue {
                get {
                    foreach (LayoutEventScriptNodeCondition condition in Conditions)
                        if (!condition.IsTrue)
                            return false;
                    return true;
                }
            }
        }

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Or([DispatchFilter("XPath", "Or")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeConditionContainerOr(parseEventInfo);

        public class LayoutEventScriptNodeConditionContainerOr : LayoutEventScriptNodeConditionContainer {
            public LayoutEventScriptNodeConditionContainerOr(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override bool IsTrue {
                get {
                    foreach (LayoutEventScriptNodeCondition condition in Conditions)
                        if (condition.IsTrue)
                            return true;
                    return false;
                }
            }
        }

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Not([DispatchFilter("XPath", "Not")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeConditionContainerNot(parseEventInfo);

        public class LayoutEventScriptNodeConditionContainerNot : LayoutEventScriptNodeConditionContainer {
            public LayoutEventScriptNodeConditionContainerNot(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
                if (Conditions.Count != 1)
                    throw ParseErrorException("Invalid number of sub-conditions for 'not'");
            }

            public override bool IsTrue => !Conditions[0].IsTrue;
        }

        #endregion

        #region Condition

        #region Base class for If conditions

        public abstract class LayoutEventScriptNodeIf : LayoutEventScriptNodeCondition {
            protected LayoutEventScriptNodeIf(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            }

            public override bool IsTrue {
                get {
                    var operand1 = GetOperand("1");
                    var operand2 = GetOperand("2");
                    string compareOperator = Element.GetAttribute("Operation");

                    if (operand2?.GetType().IsArray == true)
                        throw new ArgumentException("Operand2 cannot be array");

                    if (operand1?.GetType().IsArray == true) {
                        string symbolName = Element.GetAttribute("Symbol1");
                        string symbolAccess = Element.GetAttribute("Symbol1Access");

                        foreach (object symbolValue in (Array)operand1) {
                            var operand1value = GetOperand(symbolName, symbolValue, symbolAccess, "1");

                            if (Compare(operand1value, compareOperator, operand2))
                                return true;
                        }

                        return false;
                    }
                    else
                        return Compare(operand1, compareOperator, operand2);
                }
            }

            protected abstract bool Compare(object? operand1, string compareOperator, object? operand2);
        }

        #endregion

        #region IfString 

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_IfString([DispatchFilter("XPath", "IfString")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeIfString(parseEventInfo);

        public class LayoutEventScriptNodeIfString : LayoutEventScriptNodeIf {
            public LayoutEventScriptNodeIfString(LayoutParseEventScript parseEventInfo) : base(parseEventInfo) {
            }

            protected override bool Compare(object? operand1, string compareOperator, object? operand2) {
                if (operand1 == null || operand2 == null)
                    return false;

                var s1 = operand1.ToString() ?? "Invalid-operand1";
                var s2 = operand2.ToString() ?? "Invalid-operand2";

                return compareOperator switch {
                    "Equal" => s1 == s2,
                    "NotEqual" => s1 != s2,
                    "Match" => System.Text.RegularExpressions.Regex.IsMatch(s1, s2),
                    _ => throw new ArgumentException("Invalid compare operation: " + compareOperator)
                };
            }
        }

        #endregion

        #region IfNumber

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_IfNumber([DispatchFilter("XPath", "IfNumber")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeIfNumber(parseEventInfo);

        public class LayoutEventScriptNodeIfNumber : LayoutEventScriptNodeIf {
            public LayoutEventScriptNodeIfNumber(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            protected int GetNumber(object? rawNumber) {
                if (rawNumber is string rawStringValue)
                    return int.Parse(rawStringValue);
                else return rawNumber == null ? 0 : (int)rawNumber;
            }

            protected override bool Compare(object? operand1, string compareOperator, object? operand2) {
                if (operand1 == null || operand2 == null)
                    return false;

                int i1 = GetNumber(operand1);
                int i2 = GetNumber(operand2);

                return compareOperator switch {
                    "eq" => i1 == i2,
                    "ne" => i1 != i2,
                    "gt" => i1 > i2,
                    "ge" => i1 >= i2,
                    "le" => i1 <= i2,
                    "lt" => i1 < i2,
                    _ => throw new ArgumentException("Invalid compare operation: " + compareOperator)
                };
            }
        }

        #endregion

        #region IfBoolean

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_IfBoolean([DispatchFilter("XPath", "IfBoolean")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeIfBoolean(parseEventInfo);

        public class LayoutEventScriptNodeIfBoolean : LayoutEventScriptNodeIf {
            public LayoutEventScriptNodeIfBoolean(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            private bool GetValue(object? o) {
                return o switch {
                    string s => bool.Parse(s),
                    bool b => b,
                    _ => throw new ArgumentException("Operand has invalid boolean value")
                };
            }

            protected override bool Compare(object? operand1, string compareOperator, object? operand2) {
                if (operand1 == null || operand2 == null)
                    return false;

                bool v1 = GetValue(operand1);
                bool v2 = GetValue(operand2);

                return compareOperator switch {
                    "Equal" => v1 == v2,
                    "NotEqual" => v1 != v2,
                    _ => throw new ArgumentException("Invalid compare operation: " + compareOperator)
                };
            }
        }

        #endregion

        #region IfTime

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_IfTime([DispatchFilter("XPath", "IfTime")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeIfTime(parseEventInfo);

        // Return the DateTime used for IfTime condition. The current implementation returns the
        // current day time. This will allow future implementation to implement "clocks" that
        // are running in different rate then the real one
        [DispatchTarget]
        private DateTime GetCurrentDateTimeRequest() => DateTime.Now;

        [DispatchTarget]
        private IIfTimeNode[] ParseIfTimeElement(XmlElement element, string constraintName) => LayoutEventScriptNodeIfTime.ParseTimeConstraint(element, constraintName);

        [DispatchTarget]
        private IIfTimeNode AllocateIfTimeNode(XmlElement nodeElement) => LayoutEventScriptNodeIfTime.AllocateTimeNode(nodeElement);

        public class LayoutEventScriptNodeIfTime : LayoutEventScriptNodeCondition {
            private readonly IIfTimeNode[] seconds;
            private readonly IIfTimeNode[] minutes;
            private readonly IIfTimeNode[] hours;
            private readonly IIfTimeNode[] dayOfWeek;

            public LayoutEventScriptNodeIfTime(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
                seconds = ParseTimeConstraint(Element, "Seconds");
                minutes = ParseTimeConstraint(Element, "Minutes");
                hours = ParseTimeConstraint(Element, "Hours");
                dayOfWeek = ParseTimeConstraint(Element, "DayOfWeek");
            }

            public override bool IsTrue {
                get {
                    DateTime dt = Dispatch.Call.GetCurrentDateTimeRequest();

                    return CheckTimeNodes(seconds, dt.Second) && CheckTimeNodes(minutes, dt.Minute) && CheckTimeNodes(hours, dt.Hour)
                        && CheckTimeNodes(dayOfWeek, (int)dt.DayOfWeek);
                }
            }

            private bool CheckTimeNodes(IIfTimeNode[] nodes, int v) {
                if (nodes.Length == 0)
                    return true;

                foreach (IIfTimeNode node in nodes)
                    if (node.InRange(v))
                        return true;
                return false;
            }

            public static IIfTimeNode[] ParseTimeConstraint(XmlElement element, string constraintName) {
                XmlNodeList? nodeElements = element.SelectNodes(constraintName);

                if (nodeElements != null) {
                    IIfTimeNode[] timeNodes = new IIfTimeNode[nodeElements.Count];

                    int i = 0;
                    foreach (XmlElement nodeElement in nodeElements)
                        timeNodes[i++] = AllocateTimeNode(nodeElement);

                    return timeNodes;
                }
                else
                    return Array.Empty<IIfTimeNode>();
            }

            public static IIfTimeNode AllocateTimeNode(XmlElement e) {
                return e.Name == "DayOfWeek" ? new IfTimeDayOfWeekNode(e) : new IfTimeNode(e);
            }

            private class IfTimeNode : IIfTimeNode {
                private const string A_Value = "Value";
                private const string A_From = "From";
                private const string A_To = "To";

                public IfTimeNode(XmlElement element) {
                    this.Element = element;
                }

                public XmlElement Element { get; }
                public XmlElement? OptionalElement => Element;

                public bool IsRange => Element.HasAttribute(A_From);

                public int Value {
                    get => (int)Element.AttributeValue(A_Value);
                    set => Element.SetAttributeValue(A_Value, value);
                }

                public int From {
                    get => (int)Element.AttributeValue(A_From);
                    set => Element.SetAttributeValue(A_From, value);
                }

                public int To {
                    get => (int)Element.AttributeValue(A_To);
                    set => Element.SetAttributeValue(A_To, value);
                }

                public bool InRange(int v) => IsRange ? From <= v && v <= To : v == Value;

                public virtual string Description => IsRange ? From.ToString() + "-" + To.ToString() : Value.ToString();
            }

            private class IfTimeDayOfWeekNode : IfTimeNode {
                public IfTimeDayOfWeekNode(XmlElement element) : base(element) {
                }

                public string ToDay(int i) => new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }[i];

                public override string Description => IsRange ? ToDay(From) + "-" + ToDay(To) : ToDay(Value);
            }
        }

        #endregion

        #region IfExist

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_IfDefined([DispatchFilter("XPath", "IfDefined")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeIfDefined(parseEventInfo);

        public class LayoutEventScriptNodeIfDefined : LayoutEventScriptNodeCondition {
            public LayoutEventScriptNodeIfDefined(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override bool IsTrue {
                get {
                    string symbol = Element.GetAttribute("Symbol");
                    var symbolValue = Context[symbol];

                    return symbolValue != null && (
                        !Element.HasAttribute("Attribute") ||
                        symbolValue is IObjectHasAttributes symbolWithAttributes &&
                        symbolWithAttributes.Attributes.ContainsKey(Element.GetAttribute("Attribute"))
                        );
                }
            }
        }

        #endregion

        #endregion

        #region Actions

        #region Actions (section)

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_Actions([DispatchFilter("XPath", "Actions")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActions(parseEventInfo);

        #endregion

        #region Message

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_ShowMessage([DispatchFilter("XPath", "ShowMessage")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionShowMessage(parseEventInfo);

        private class LayoutEventScriptNodeActionShowMessage : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionShowMessage(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public string ExpandMessage(string text) {
                int s = 0;
                int e;
                ArrayList parts = new();

                while (true) {
                    for (e = s; e < text.Length && text[e] != '[' && text[e] != '<'; e++)
                        ;

                    parts.Add(text[s..e]);

                    if (e >= text.Length)
                        break;
                    else {      // Expand reference
                        bool expandProperty = text[e] == '[';
                        char closingDelimiter = expandProperty ? ']' : '>';
                        object? obj;

                        s = e + 1;
                        for (e = s; s < text.Length && text[e] != closingDelimiter; e++)
                            ;

                        if (e >= text.Length)
                            throw new ArgumentException("Missing ] to terminate reference, text: " + text);

                        string reference = text[s..e];

                        s = e + 1;

                        // Now expand reference
                        string[] referenceParts = reference.Split('.');

                        if (referenceParts.Length != 2)
                            throw new ArgumentException("Invalid symbol reference " + reference + " in text: " + text);

                        if (expandProperty)
                            obj = Context.GetSymbolProperty(referenceParts[0], referenceParts[1]);
                        else
                            obj = Context.GetSymbolAttribute(referenceParts[0], referenceParts[1]);

                        if (obj != null) {
                            if (obj is string)
                                parts.Add(obj);
                            else
                                parts.Add(obj.ToString());
                        }
                        else
                            parts.Add("*UNDEFINED*");
                    }
                }

                return String.Concat((string[])parts.ToArray(typeof(string)));
            }

            public override void Execute() {
                string messageText = ExpandMessage(Element.GetAttribute("Message"));
                object? subject = Context["Train"];

                switch (Element.GetAttribute("MessageType")) {
                    case "Error":
                        LayoutModuleBase.Error(subject, messageText);
                        break;

                    case "Message":
                    default:
                        LayoutModuleBase.Message(subject, messageText);
                        break;

                    case "Warning":
                        LayoutModuleBase.Warning(subject, messageText);
                        break;
                }
            }
        }

        #endregion

        #region Set Attribute

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_SetAttribute([DispatchFilter("XPath", "ShowMessage")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionSetAttribute(parseEventInfo);

        private class LayoutEventScriptNodeActionSetAttribute : LayoutEventScriptNodeAction {
            private const string A_SetTo = "SetTo";
            private const string A_Value = "Value";
            private const string A_Symbol = "Symbol";
            private const string A_Attribute = "Attribute";

            public LayoutEventScriptNodeActionSetAttribute(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            public override void Execute() {
                var symbol = Element.GetAttribute(A_Symbol);
                var attribute = Element.GetAttribute(A_Attribute);
                object? symbolValue = Context[symbol];

                if (symbolValue == null)
                    throw new ArgumentException("Event script context does not contain object named " + symbol);

                if (symbolValue is not IObjectHasAttributes symbolWithAttributes)
                    throw new ArgumentException("The object named " + symbol + " does not support attributes");

                var setTo = Element.GetAttribute(A_SetTo);
                var attributes = symbolWithAttributes.Attributes;

                switch (setTo) {
                    case "Text":
                        attributes[attribute] = Element.GetAttribute(A_Value);
                        break;

                    case "Number":
                        var number = (int)Element.AttributeValue(A_Value);

                        if (Element.GetAttribute("Op") == "Add") {
                            var oldValue = attributes[attribute];

                            if (oldValue != null && oldValue is int aNumber)
                                number += aNumber;
                        }

                        attributes[attribute] = number;
                        break;

                    case "Boolean":
                        var boolean = (bool)Element.AttributeValue(A_Value);

                        attributes[attribute] = boolean;
                        break;

                    case "ValueOf":

                        var v = (Element.GetAttribute("SymbolToAccess")) switch {
                            "Property" => LayoutScriptContext.GetProperty(symbol, symbolValue, Element.GetAttribute("NameTo")),
                            _ => LayoutScriptContext.GetAttribute(symbol, symbolValue, Element.GetAttribute("NameTo")),
                        };
                        attributes[attribute] = v;
                        break;

                    case "Remove":
                        attributes.Remove(attribute);
                        break;
                }
            }
        }

        #endregion

        #region GenerareEvent

        #endregion

        [DispatchTarget]
        private LayoutEventScriptNode ParseEventScriptDefinition_GenerateEvent([DispatchFilter("XPath", "GenerateEvent")] LayoutParseEventScript parseEventInfo) => new LayoutEventScriptNodeActionGenerateEvent(parseEventInfo);

        private class LayoutEventScriptNodeActionGenerateEvent : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionGenerateEvent(LayoutParseEventScript parseEventScript) : base(parseEventScript) {
            }

            protected object? GetArgument(string name) => Element.GetAttribute(name + "Type") switch {
                "Null" => null,
                "ValueOf" => GetOperand(name),
                "Reference" => Context[Element.GetAttribute(name + "SymbolName")],
                "Context" => Context,
                _ => new ArgumentException("Unknown argument type (" + name + "Type) = " + Element.GetAttribute(name + "Type"))
            };

            public override void Execute() {
                var sender = GetArgument("Sender");
                var info = GetArgument("Info");
                LayoutEvent theEvent = new(Element.GetAttribute("EventName"), sender, info);

                var optionsElement = Element["Options"];

                if (optionsElement != null) {
                    foreach (XmlElement optionElement in optionsElement) {
                        var optionName = optionElement.GetAttribute("Name");
                        var optionValue = GetOperand(optionElement, "Option");

                        if (optionValue != null)
                            theEvent.SetOption(optionName, optionValue.ToString() ?? "Invalid-String");
                    }
                }

                EventManager.Event(theEvent);
            }
        }

        #endregion

    }

#pragma warning restore IDE0051
    #endregion
}

