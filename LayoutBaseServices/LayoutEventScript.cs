using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;

using System.Diagnostics;

// Reenable warning when switch to .NET 4.8 and range c#8 feature is supported
#pragma warning disable IDE0057

#nullable enable
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
        public LayoutEventScriptErrorInfo(ILayoutScript script, LayoutEventScriptTask task, LayoutEventScriptNode node, LayoutEventScriptExecutionPhase executionPhase, Exception ex) {
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

        public Exception Exception { get; }
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

        public LayoutEventScriptTask(ILayoutScript eventScript, XmlElement? scriptElement, LayoutScriptContext context) {
            this.script = eventScript;
            this.scriptElement = scriptElement ?? throw new ArgumentNullException("Invali script element");

            Root = Parse(this.scriptElement, context);
        }

        public LayoutEventScriptNode Parse(XmlElement elementToParse, LayoutScriptContext context) {
            LayoutEventScriptNode? node = (LayoutEventScriptNode?)EventManager.Event(new LayoutEvent(
                "parse-event-script-definition", new LayoutParseEventScript(script, this, elementToParse, context)));

            if (node == null)
                throw new LayoutEventScriptParseException(script, elementToParse, "Unrecognized event script option: '" + elementToParse.Name + "'");

            return node;
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
                EventManager.Event(new LayoutEvent("event-script-error", EventScript, new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException),
                    null));
            }
            catch (Exception ex) {
                EventManager.Event(new LayoutEvent("event-script-error", EventScript, new LayoutEventScriptErrorInfo(EventScript, this, Root, LayoutEventScriptExecutionPhase.Evaluation, ex),
                    null));
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
                EventManager.Event(new LayoutEvent("event-script-task-reset", EventScript, this));
                EventRoot.Reset();
            }
            catch (LayoutEventScriptException ex) {
                EventManager.Event(new LayoutEvent("event-script-error", EventScript, new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException),
                    null));
            }
            catch (Exception ex) {
                EventManager.Event(new LayoutEvent("event-script-error", EventScript, new LayoutEventScriptErrorInfo(EventScript, this, Root, LayoutEventScriptExecutionPhase.Reset, ex),
                    null));
            }

            if (EventRoot.Occurred && !taskTerminated) {
                EventScript.OnTaskTerminated(this);
                taskTerminated = true;
                Dispose();
            }
        }

        public bool Occurred => EventRoot.Occurred;

        public bool IsErrorState => EventRoot.IsErrorState;

        public string? Description => (string?)EventManager.Event(new LayoutEvent("get-event-script-description", scriptElement));

        public void Cancel() {
            EventRoot.Cancel();
        }

        public void Dispose() {
            try {
                EventManager.Event(new LayoutEvent("event-script-task-dispose", EventScript, this));
                EventRoot.Dispose();
            }
            catch (LayoutEventScriptException ex) {
                EventManager.Event(new LayoutEvent("event-script-error", EventScript, new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException),
                    null));
            }
            catch (Exception ex) {
                EventManager.Event(new LayoutEvent("event-script-error", EventScript, new LayoutEventScriptErrorInfo(EventScript, this, Root, LayoutEventScriptExecutionPhase.Disposing, ex),
                    null));
            }
        }
    }

    public class LayoutEventScript : ILayoutScript, IDisposable, IObjectHasId {
        private readonly LayoutEvent? scriptDoneEvent;
        private readonly LayoutEvent? errorOccurredEvent;
        private readonly XmlElement? scriptElement;
        private LayoutScriptContext? scriptContext;
        private readonly List<LayoutEventScriptTask> tasks = new List<LayoutEventScriptTask>();
        private LayoutEventScriptTask? rootTask;

        public LayoutEventScript(string scriptName, XmlElement? scriptElement,
            ICollection<Guid> scopeIDs, LayoutEvent? scriptDoneEvent, LayoutEvent? errorOccurredEvent) {
            this.Name = scriptName;
            this.scriptElement = scriptElement;
            this.ScopeIDs = scopeIDs;
            this.scriptDoneEvent = scriptDoneEvent;
            this.errorOccurredEvent = errorOccurredEvent;

            this.Id = Guid.NewGuid();
        }

        public LayoutScriptContext ScriptContext {
            get {
                if (scriptContext == null) {
                    if (ParentContext == null)
                        ParentContext = (LayoutScriptContext?)EventManager.Event(new LayoutEvent("get-global-event-script-context", this));

                    scriptContext = new LayoutScriptContext("Script", ParentContext) {
                        CopyOnClone = false
                    };
                }

                return scriptContext;
            }

            set => scriptContext = value;
        }

        public LayoutScriptContext? ParentContext { get; set; }

        public LayoutEventScriptTask RootTask => rootTask ?? (rootTask = AddTask(scriptElement, ScriptContext));

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

        public LayoutEventScriptTask AddTask(XmlElement? scriptElement, LayoutScriptContext context) {
            LayoutEventScriptTask task = new LayoutEventScriptTask(this, scriptElement, context);

            tasks.Add(task);
            EventManager.Event(new LayoutEvent("event-script-task-created", this, task));
            return task;
        }

        internal void OnTaskTerminated(LayoutEventScriptTask task) {
            EventManager.Event(new LayoutEvent("event-script-task-terminated", this, task));

            if (task == RootTask) {
                EventManager.Event(new LayoutEvent("event-script-terminated", this));
                if (task.EventRoot.IsErrorState && errorOccurredEvent != null)
                    EventManager.Event(errorOccurredEvent);
                else if (scriptDoneEvent != null)
                    EventManager.Event(scriptDoneEvent);
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
            EventManager.Event(new LayoutEvent("event-script-reset", this, RootTask));

            RootTask.Reset();
        }

        public bool Occurred => RootTask.Occurred;

        public bool IsErrorState => RootTask.IsErrorState;

        public string? Description => (string?)EventManager.Event(new LayoutEvent("get-event-script-description", scriptElement));

        public void Dispose() {
            EventManager.Event(new LayoutEvent("event-script-dispose", this));

            foreach (LayoutEventScriptTask task in tasks) {
                try {
                    task.Dispose();
                }
                catch (LayoutEventScriptException ex) {
                    EventManager.Event(new LayoutEvent("event-script-error", this, new LayoutEventScriptErrorInfo(this, task, ex.Node, ex.ExecutionPhase, ex.InnerException),
                        null));
                }
                catch (Exception ex) {
                    EventManager.Event(new LayoutEvent("event-script-error", this, new LayoutEventScriptErrorInfo(this, task, task.Root, LayoutEventScriptExecutionPhase.Disposing, ex),
                        null));
                }
            }

            tasks.Clear();
            if (rootTask != null) {
                rootTask.Dispose();
                rootTask = null;
            }
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

            initialize();
        }

        public LayoutConditionScript(XmlElement element, bool defaultValue) {
            this.element = element;
            this.Name = "Condition";
            this.defaultValue = defaultValue;

            initialize();
        }

        public LayoutConditionScript(string name, XmlElement element) {
            this.element = element;
            this.Name = name;

            initialize();
        }

        public LayoutConditionScript(string name, XmlElement element, bool defaultValue) {
            this.element = element;
            this.Name = name;
            this.defaultValue = defaultValue;

            initialize();
        }

        private void initialize() {
        }

        public bool IsTrue {
            get {
                if (element == null || element.ChildNodes.Count < 1)
                    return defaultValue;

                using LayoutEventScriptTask task = new LayoutEventScriptTask(this, (XmlElement)element.ChildNodes[0], ScriptContext);

                return task.ConditionRoot.IsTrue;
            }
        }

        #region ILayoutScript Members

        public string Name { get; set; }

        public LayoutScriptContext ScriptContext {
            get {
                if (scriptContext == null) {
                    LayoutScriptContext? globalContext = (LayoutScriptContext?)EventManager.Event(new LayoutEvent("get-global-event-script-context", this));

                    scriptContext = new LayoutScriptContext("Script", globalContext) {
                        CopyOnClone = false
                    };
                }

                return scriptContext;
            }
        }

        public object? ScriptSubject { get; set; }

        public string? Description => element == null || element.ChildNodes.Count < 1
                    ? ""
                    : (string?)EventManager.Event(new LayoutEvent("get-event-script-description", element.ChildNodes[0]));

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

        public object GetProperty(string symbolName, object symbolValue, string propertyName) {
            PropertyInfo propertyInfo = symbolValue.GetType().GetProperty(propertyName);

            if (propertyInfo == null) {
                PropertyInfo infoPropertyInfo = symbolValue.GetType().GetProperty("Info");

                if (infoPropertyInfo != null) {
                    object infoObject = infoPropertyInfo.GetValue(symbolValue, null);

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

        public object GetSymbolProperty(string symbolName, string propertyName) {
            object? symbolValue = this[symbolName];

            if (symbolValue != null)
                return GetProperty(symbolName, symbolValue, propertyName);
            else
                throw new ArgumentException("Event script context does not contain object named " + symbolName);
        }

        public object? GetAttribute(string symbolName, object symbolValue, string attributeName) {
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

        protected LayoutEventScriptNode(LayoutEvent e) {
            this.parseEventInfo = Ensure.NotNull<LayoutParseEventScript>(e.Sender, "parseEventInfo");
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

        protected LayoutEventScriptParseException ParseErrorException(string message) => new LayoutEventScriptParseException(Script, Element, message);

        #region Common Access methods

        protected object? GetOperand(XmlElement element, string symbolName, object symbolValue, string symbolAccess, string suffix) {
            string tagName = element.GetAttribute("Name" + suffix);

            if (symbolAccess == "Property")
                return Context.GetProperty(symbolName, symbolValue, tagName);
            else if (symbolAccess == "Attribute")
                return Context.GetAttribute(symbolName, symbolValue, tagName);
            else
                throw new ArgumentException("Invalid symbol access method " + symbolAccess + " for symbol " + symbolName);
        }

        protected object? GetOperand(string symbolName, object symbolValue, string symbolAccess, string suffix) => GetOperand(Element, symbolName, symbolValue, symbolAccess, suffix);

        protected object? GetOperand(XmlElement element, string suffix) {
            string symbolAccess = element.GetAttribute("Symbol" + suffix + "Access");

            if (symbolAccess == "Value") {
                var v = element.AttributeValue($"Value{suffix}");

                return (element.GetAttribute("Type" + suffix)) switch
                {
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

        protected LayoutEventScriptNodeEventBase(LayoutEvent e) : base(e) {
            EventManager.AddObjectSubscriptions(this);
        }

        /// <summary>
        /// Recalculate the whether this event occured. If the event took place, set Occured to true
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
        public virtual void Dispose() {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        /// <summary>
        /// Parse 'Condition' element
        /// </summary>
        /// <param name="eventElement">The event XML element</param>
        protected void ParseCondition(XmlElement eventElement) {
            XmlElement conditionTitleElement = eventElement["Condition"];

            if (conditionTitleElement != null) {
                if (conditionTitleElement.ChildNodes.Count != 1)
                    throw ParseErrorException("Missing condition, or more than one condition for event " + Element.Name);

                XmlElement conditionElement = (XmlElement)conditionTitleElement.ChildNodes[0];

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
            XmlElement actionsElement = eventElement["Actions"];

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

        public LayoutEventScriptNodeEvent(LayoutEvent e) : base(e) {
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
                    LayoutEventAttribute subscriptionInfo = new LayoutEventAttribute(EventName);

                    subscription = subscriptionInfo.CreateSubscription();
                    subscription.SetFromLayoutEventAttribute(subscriptionInfo);
                    subscription.Order = 10000;         // Ensure that built in event handler execute first
                    subscription.SetMethod(this, this.GetType().GetMethod("OnEvent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

                    EventManager.Subscriptions.Add(subscription);
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

                    if (!relevantEvent && e.Info is IObjectHasId) {
                        IObjectHasId info = (IObjectHasId)e.Info;

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
                        EventManager.Event(new LayoutEvent("set-script-context", e.Sender, Context));
                    if (e.Info != null)
                        EventManager.Event(new LayoutEvent("set-script-context", e.Info, Context));

                    if (IsConditionTrue) {
                        Cancel();
                        Occurred = true;
                    }
                }
            }
            catch (LayoutEventScriptException ex) {
                EventManager.Event(new LayoutEvent("event-script-error", this.Script, new LayoutEventScriptErrorInfo(this.Script, this.Task, ex.Node, ex.ExecutionPhase, ex.InnerException),
                    null));
            }
            catch (Exception ex) {
                EventManager.Event(new LayoutEvent("event-script-error", this.Script, new LayoutEventScriptErrorInfo(this.Script, this.Task, this, LayoutEventScriptExecutionPhase.EventProcessing, ex),
                    null));
            }
        }
    }

    public abstract class LayoutEventScriptNodeEventContainer : LayoutEventScriptNodeEventBase {
        private readonly List<LayoutEventScriptNodeEventBase> events = new List<LayoutEventScriptNodeEventBase>();

        protected LayoutEventScriptNodeEventContainer(LayoutEvent e) : base(e) {
            XmlElement eventsElement = Element["Events"];

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

        public override void Dispose() {
            foreach (LayoutEventScriptNodeEventBase eventNode in events)
                eventNode.Dispose();

            base.Dispose();
        }
    }

    #endregion

    #region Conditions

    public abstract class LayoutEventScriptNodeCondition : LayoutEventScriptNode {
        protected LayoutEventScriptNodeCondition(LayoutEvent e) : base(e) {
        }

        /// <summary>
        /// Is the condition true
        /// </summary>
        public abstract bool IsTrue {
            get;
        }
    }

    public abstract class LayoutEventScriptNodeConditionContainer : LayoutEventScriptNodeCondition {
        private readonly List<LayoutEventScriptNodeCondition> conditions = new List<LayoutEventScriptNodeCondition>();

        protected LayoutEventScriptNodeConditionContainer(LayoutEvent e) : base(e) {
            foreach (XmlElement conditionElement in Element) {
                if (!(Parse(conditionElement) is LayoutEventScriptNodeCondition condition))
                    throw ParseErrorException("Invalid condition: " + conditionElement.Name);

                conditions.Add(condition);
            }
        }

        public IList<LayoutEventScriptNodeCondition> Conditions => conditions;
    }

    #endregion

    #region Actions

    public abstract class LayoutEventScriptNodeAction : LayoutEventScriptNode {
        protected LayoutEventScriptNodeAction(LayoutEvent e) : base(e) {
        }

        /// <summary>
        /// Execute the action
        /// </summary>
        public abstract void Execute();
    }

    public class LayoutEventScriptNodeActions : LayoutEventScriptNode {
        private readonly ArrayList actions = new ArrayList();

        public LayoutEventScriptNodeActions(LayoutEvent e) : base(e) {
            foreach (XmlElement elementAction in Element) {
                if (!((LayoutEventScriptNodeAction?)Parse(elementAction) is LayoutEventScriptNodeAction action))
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

        [LayoutEvent("parse-event-script-definition", IfSender = "Any")]
        private void ParseAny(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventContainerAny(e);
        }

        private class LayoutEventScriptNodeEventContainerAny : LayoutEventScriptNodeEventContainer {
            public LayoutEventScriptNodeEventContainerAny(LayoutEvent e) : base(e) {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "All")]
        private void ParseAll(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventContainerAll(e);
        }

        private class LayoutEventScriptNodeEventContainerAll : LayoutEventScriptNodeEventContainer {
            public LayoutEventScriptNodeEventContainerAll(LayoutEvent e) : base(e) {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "Sequence")]
        private void ParseSequence(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventContainerSequence(e);
        }

        private class LayoutEventScriptNodeEventContainerSequence : LayoutEventScriptNodeEventContainer {
            private int seqIndex;

            public LayoutEventScriptNodeEventContainerSequence(LayoutEvent e) : base(e) {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "Repeat")]
        private void ParseRepeat(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventContainerRepeat(e);
        }

        private class LayoutEventScriptNodeEventContainerRepeat : LayoutEventScriptNodeEventContainer {
            private const string A_Count = "Count";
            private readonly int numberOfIterations;
            private int iterationCount;
            private readonly LayoutEventScriptNodeEventBase repeatedEvent;

            public LayoutEventScriptNodeEventContainerRepeat(LayoutEvent e) : base(e) {
                if (!Element.HasAttribute(A_Count))
                    throw ParseErrorException("Missing iteration count in repeat (Count)");

                numberOfIterations = (int)Element.AttributeValue(A_Count);

                if (Element.ChildNodes.Count < 1)
                    throw ParseErrorException("Missing element to repeat");
                else if (Element.ChildNodes.Count > 1)
                    throw ParseErrorException("Too many elements to repeat");

                XmlElement repeatedElement = (XmlElement)Element.ChildNodes[0];

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

            public override void Dispose() {
                repeatedEvent.Dispose();
                base.Dispose();
            }
        }

        #endregion

        #region Random Choice

        [LayoutEvent("parse-event-script-definition", IfSender = "RandomChoice")]
        private void ParseRandomChoice(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventContainerRandomChoice(e);
        }

        private class LayoutEventScriptNodeEventContainerRandomChoice : LayoutEventScriptNodeEventContainer {
            private const string A_Choice = "Choice";
            private const string A_Weight = "Weight";
            private readonly List<ChoiceEntry> choices = new List<ChoiceEntry>();
            private LayoutEventScriptNodeEventBase? chosenNode;

            public LayoutEventScriptNodeEventContainerRandomChoice(LayoutEvent e) : base(e) {
                foreach (XmlElement choiceElement in Element.GetElementsByTagName(A_Choice)) {
                    var weight = (int)choiceElement.AttributeValue(A_Weight);
                    var node = (LayoutEventScriptNodeEventBase?)Parse((XmlElement)choiceElement.ChildNodes[0]);

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

            public override void Dispose() {
                chosenNode?.Dispose();
                base.Dispose();
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

        [LayoutEvent("parse-event-script-definition", IfSender = "Task")]
        private void ParseTask(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventContainerTask(e);
        }

        private class LayoutEventScriptNodeEventContainerTask : LayoutEventScriptNodeEventBase {
            private readonly XmlElement taskElement;

            public LayoutEventScriptNodeEventContainerTask(LayoutEvent e) : base(e) {
                if (Element.ChildNodes.Count < 1)
                    throw ParseErrorException("Missing element to repeat");
                else if (Element.ChildNodes.Count > 1)
                    throw ParseErrorException("Too many elements to repeat");

                taskElement = (XmlElement)Element.ChildNodes[0];
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

        [LayoutEvent("parse-event-script-definition", IfSender = "WaitForEvent")]
        private void parseEvent(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEvent(e);
        }

        #endregion

        #region Wait

        [LayoutEvent("parse-event-script-definition", IfSender = "Wait")]
        private void parseWait(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventWait(e);
        }

        private class LayoutEventScriptNodeEventWait : LayoutEventScriptNodeEvent {
            private const string A_MilliSeconds = "MilliSeconds";
            private const string A_Seconds = "Seconds";
            private const string A_Minutes = "Minutes";
            private const string A_RandomSeconds = "RandomSeconds";
            private readonly int delay;
            private LayoutDelayedEvent? delayedEvent;

            public LayoutEventScriptNodeEventWait(LayoutEvent e) : base(e) {
                delay = ((int?)Element.AttributeValue(A_MilliSeconds) ?? 0)
                    + (((int?)Element.AttributeValue(A_Seconds) ?? 0) * 1000)
                    + (((int?)Element.AttributeValue(A_Minutes) ?? 0) * 1000 * 60);

                if (Element.HasAttribute(A_RandomSeconds))
                    delay += new Random().Next((int)Element.AttributeValue(A_RandomSeconds) * 1000);
            }

            public override void Reset() {
                // Note that the reset operation may set Occured to true, if the condition is prechecked and is true
                base.Reset();

                if (delayedEvent != null) {
                    delayedEvent.Cancel();
                    delayedEvent = null;
                }

                if (!Occurred)
                    delayedEvent = EventManager.DelayedEvent(delay, new LayoutEvent("wait-event-condition-occured", this));
            }

            public override void Cancel() {
                if (delayedEvent != null) {
                    delayedEvent.Cancel();
                    delayedEvent = null;
                }
            }

            public override void Dispose() {
                if (delayedEvent != null) {
                    delayedEvent = null;
                }

                base.Dispose();
            }

            [LayoutEvent("wait-event-condition-occured")]
            private void WaitConditionDone(LayoutEvent e) {
                if (e.Sender == this) {
                    try {
                        if (delayedEvent != null) {         // Make sure that the event was not canceled (race condition)
                            Debug.Assert(e.Sender == this);

                            if (IsConditionTrue)
                                Occurred = true;
                            else        // Condition is not true, wait again and check condition again
                                delayedEvent = EventManager.DelayedEvent(delay, new LayoutEvent("wait-event-condition-occured", this));
                        }
                    }
                    catch (Exception ex) {
                        Trace.WriteLine("Exception thrown while processing wait condition");
                        Trace.WriteLine("-- " + ex.Message);
                        Trace.WriteLine("-- " + ex.StackTrace);
                    }
                }
            }
        }

        #endregion

        #region DoNow

        [LayoutEvent("parse-event-script-definition", IfSender = "DoNow")]
        private void parseDoNow(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeEventDoNow(e);
        }

        private class LayoutEventScriptNodeEventDoNow : LayoutEventScriptNodeEvent {
            public LayoutEventScriptNodeEventDoNow(LayoutEvent e) : base(e) {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "And")]
        private void parseAnd(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeConditionContainerAnd(e);
        }

        public class LayoutEventScriptNodeConditionContainerAnd : LayoutEventScriptNodeConditionContainer {
            public LayoutEventScriptNodeConditionContainerAnd(LayoutEvent e) : base(e) {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "Or")]
        private void parseOr(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeConditionContainerOr(e);
        }

        public class LayoutEventScriptNodeConditionContainerOr : LayoutEventScriptNodeConditionContainer {
            public LayoutEventScriptNodeConditionContainerOr(LayoutEvent e) : base(e) {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "Not")]
        private void parseNot(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeConditionContainerNot(e);
        }

        public class LayoutEventScriptNodeConditionContainerNot : LayoutEventScriptNodeConditionContainer {
            public LayoutEventScriptNodeConditionContainerNot(LayoutEvent e) : base(e) {
                if (Conditions.Count != 1)
                    throw ParseErrorException("Invalid number of sub-conditions for 'not'");
            }

            public override bool IsTrue => !Conditions[0].IsTrue;
        }

        #endregion

        #region Condition

        #region Base class for If conditions

        public abstract class LayoutEventScriptNodeIf : LayoutEventScriptNodeCondition {
            protected LayoutEventScriptNodeIf(LayoutEvent e) : base(e) {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "IfString")]
        private void parseIfString(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeIfString(e);
        }

        public class LayoutEventScriptNodeIfString : LayoutEventScriptNodeIf {
            public LayoutEventScriptNodeIfString(LayoutEvent e) : base(e) {
            }

            protected override bool Compare(object? operand1, string compareOperator, object? operand2) {
                if (operand1 == null || operand2 == null)
                    return false;

                var s1 = operand1.ToString();
                var s2 = operand2.ToString();

                return compareOperator switch
                {
                    "Equal" => s1 == s2,
                    "NotEqual" => s1 != s2,
                    "Match" => System.Text.RegularExpressions.Regex.IsMatch(s1, s2),
                    _ => throw new ArgumentException("Invalid compare operation: " + compareOperator)
                };
            }
        }

        #endregion

        #region IfNumber

        [LayoutEvent("parse-event-script-definition", IfSender = "IfNumber")]
        private void parseIfNumber(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeIfNumber(e);
        }

        public class LayoutEventScriptNodeIfNumber : LayoutEventScriptNodeIf {
            public LayoutEventScriptNodeIfNumber(LayoutEvent e) : base(e) {
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

                return compareOperator switch
                {
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

        [LayoutEvent("parse-event-script-definition", IfSender = "IfBoolean")]
        private void parseIfBoolean(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeIfBoolean(e);
        }

        public class LayoutEventScriptNodeIfBoolean : LayoutEventScriptNodeIf {
            public LayoutEventScriptNodeIfBoolean(LayoutEvent e) : base(e) {
            }

            private bool getValue(object? o) {
                return o switch
                {
                    string s => bool.Parse(s),
                    bool b => b,
                    _ => throw new ArgumentException("Operand has invalid boolean value")
                };
            }

            protected override bool Compare(object? operand1, string compareOperator, object? operand2) {
                if (operand1 == null || operand2 == null)
                    return false;

                bool v1 = getValue(operand1);
                bool v2 = getValue(operand2);

                return compareOperator switch
                {
                    "Equal" => v1 == v2,
                    "NotEqual" => v1 != v2,
                    _ => throw new ArgumentException("Invalid compare operation: " + compareOperator)
                };
            }
        }

        #endregion

        #region IfTime

        [LayoutEvent("parse-event-script-definition", IfSender = "IfTime")]
        private void parseIfTime(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeIfTime(e);
        }

        // Return the DateTime used for IfTime condition. The current implementation returns the
        // current day time. This will allow future implementation to implement "clocks" that
        // are running in different rate then the real one
        [LayoutEvent("get-current-date-time-request")]
        private void getCurrentDateTimeRequest(LayoutEvent e) {
            e.Info = DateTime.Now;
        }

        [LayoutEvent("parse-if-time-element")]
        private void parseIfTimeElement(LayoutEvent e) {
            var element = (XmlElement?)e.Sender;

            if (element != null && e.Info is string constraintName)
                e.Info = LayoutEventScriptNodeIfTime.ParseTimeConstraint(element, constraintName);
        }

        [LayoutEvent("allocate-if-time-node")]
        private void allocateIfTimeNode(LayoutEvent e) {
            var nodeElement = (XmlElement?)e.Sender;

            if (nodeElement != null)
                e.Info = LayoutEventScriptNodeIfTime.AllocateTimeNode(nodeElement);
        }

        public class LayoutEventScriptNodeIfTime : LayoutEventScriptNodeCondition {
            private readonly IIfTimeNode[] seconds;
            private readonly IIfTimeNode[] minutes;
            private readonly IIfTimeNode[] hours;
            private readonly IIfTimeNode[] dayOfWeek;

            public LayoutEventScriptNodeIfTime(LayoutEvent e) : base(e) {
                seconds = ParseTimeConstraint(Element, "Seconds");
                minutes = ParseTimeConstraint(Element, "Minutes");
                hours = ParseTimeConstraint(Element, "Hours");
                dayOfWeek = ParseTimeConstraint(Element, "DayOfWeek");
            }

            public override bool IsTrue {
                get {
                    DateTime dt = (DateTime)(EventManager.Event(new LayoutEvent("get-current-date-time-request", this)) ?? DateTime.Today);

                    return checkTimeNodes(seconds, dt.Second) && checkTimeNodes(minutes, dt.Minute) && checkTimeNodes(hours, dt.Hour)
                        && checkTimeNodes(dayOfWeek, (int)dt.DayOfWeek);
                }
            }

            private bool checkTimeNodes(IIfTimeNode[] nodes, int v) {
                if (nodes.Length == 0)
                    return true;

                foreach (IIfTimeNode node in nodes)
                    if (node.InRange(v))
                        return true;
                return false;
            }

            public static IIfTimeNode[] ParseTimeConstraint(XmlElement element, string constraintName) {
                XmlNodeList nodeElements = element.SelectNodes(constraintName);
                IIfTimeNode[] timeNodes = new IIfTimeNode[nodeElements.Count];

                int i = 0;
                foreach (XmlElement nodeElement in nodeElements)
                    timeNodes[i++] = AllocateTimeNode(nodeElement);

                return timeNodes;
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
                    set => Element.SetAttribute(A_Value, value);
                }

                public int From {
                    get => (int)Element.AttributeValue(A_From);
                    set => Element.SetAttribute(A_From, value);
                }

                public int To {
                    get => (int)Element.AttributeValue(A_To);
                    set => Element.SetAttribute(A_To, value);
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

        [LayoutEvent("parse-event-script-definition", IfSender = "IfDefined")]
        private void parseIfDefined(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeIfDefined(e);
        }

        public class LayoutEventScriptNodeIfDefined : LayoutEventScriptNodeCondition {
            public LayoutEventScriptNodeIfDefined(LayoutEvent e) : base(e) {
            }

            public override bool IsTrue {
                get {
                    string symbol = Element.GetAttribute("Symbol");
                    var symbolValue = Context[symbol];

                    return symbolValue == null
                        ? false
                        : Element.HasAttribute("Attribute")
                        ? symbolValue is IObjectHasAttributes symbolWithAttributes && symbolWithAttributes.Attributes.ContainsKey(Element.GetAttribute("Attribute"))
                        : true;
                }
            }
        }

        #endregion

        #endregion

        #region Actions

        #region Actions (section)

        [LayoutEvent("parse-event-script-definition", IfSender = "Actions")]
        private void parseActions(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActions(e);
        }

        #endregion

        #region Message

        [LayoutEvent("parse-event-script-definition", IfSender = "ShowMessage")]
        private void parseShowMessage(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionShowMessage(e);
        }

        private class LayoutEventScriptNodeActionShowMessage : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionShowMessage(LayoutEvent e) : base(e) {
            }

            public string ExpandMessage(string text) {
                int s = 0;
                int e;
                ArrayList parts = new ArrayList();

                while (true) {
                    for (e = s; e < text.Length && text[e] != '[' && text[e] != '<'; e++)
                        ;

                    parts.Add(text.Substring(s, e - s));

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

                        string reference = text.Substring(s, e - s);

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

        [LayoutEvent("parse-event-script-definition", IfSender = "SetAttribute")]
        private void parseSetAttribute(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionSetAttribute(e);
        }

        private class LayoutEventScriptNodeActionSetAttribute : LayoutEventScriptNodeAction {
            private const string A_SetTo = "SetTo";
            private const string A_Value = "Value";
            private const string A_Symbol = "Symbol";
            private const string A_Attribute = "Attribute";

            public LayoutEventScriptNodeActionSetAttribute(LayoutEvent e) : base(e) {
            }

            public override void Execute() {
                var symbol = Element.GetAttribute(A_Symbol);
                var attribute = Element.GetAttribute(A_Attribute);
                object? symbolValue = Context[symbol];

                if (symbolValue == null)
                    throw new ArgumentException("Event script context does not contain object named " + symbol);

                if (!(symbolValue is IObjectHasAttributes symbolWithAttributes))
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

                            if (oldValue != null && oldValue is int)
                                number += (int)oldValue;
                        }

                        attributes[attribute] = number;
                        break;

                    case "Boolean":
                        var boolean = (bool)Element.AttributeValue(A_Value);

                        attributes[attribute] = boolean;
                        break;

                    case "ValueOf":

                        var v = (Element.GetAttribute("SymbolToAccess")) switch
                        {
                            "Property" => Context.GetProperty(symbol, symbolValue, Element.GetAttribute("NameTo")),
                            _ => Context.GetAttribute(symbol, symbolValue, Element.GetAttribute("NameTo")),
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

        [LayoutEvent("parse-event-script-definition", IfSender = "GenerateEvent")]
        private void parseGenerateEvent(LayoutEvent e) {
            e.Info = new LayoutEventScriptNodeActionGenerateEvent(e);
        }

        private class LayoutEventScriptNodeActionGenerateEvent : LayoutEventScriptNodeAction {
            public LayoutEventScriptNodeActionGenerateEvent(LayoutEvent e) : base(e) {
            }

            protected object? GetArgument(string name) => Element.GetAttribute(name + "Type") switch
            {
                "Null" => null,
                "ValueOf" => GetOperand(name),
                "Reference" => Context[Element.GetAttribute(name + "SymbolName")],
                "Context" => Context,
                _ => new ArgumentException("Unknown argument type (" + name + "Type) = " + Element.GetAttribute(name + "Type"))
            };

            public override void Execute() {
                var sender = GetArgument("Sender");
                var info = GetArgument("Info");
                LayoutEvent theEvent = new LayoutEvent(Element.GetAttribute("EventName"), sender, info);

                XmlElement optionsElement = Element["Options"];

                if (optionsElement != null) {
                    foreach (XmlElement optionElement in optionsElement) {
                        var optionName = optionElement.GetAttribute("Name");
                        var optionValue = GetOperand(optionElement, "Option");

                        if (optionValue != null)
                            theEvent.SetOption(optionName, optionValue.ToString());
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

