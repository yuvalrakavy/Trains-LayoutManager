using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Threading;

using System.Diagnostics;

namespace LayoutManager {

	#region Event Script Implementations

	#region Event script error handling

	public enum LayoutEventScriptExecutionPhase {
		Unknown , Evaluation, Reset, Cancel, Disposing, EventProcessing, ActionProcessing, ConditionProcessing,
	}

	public class LayoutEventScriptException : Exception {
		LayoutEventScriptExecutionPhase	executionPhase = LayoutEventScriptExecutionPhase.Unknown;

		public LayoutEventScriptException(LayoutEventScriptNode node, LayoutEventScriptExecutionPhase executionPhase, Exception ex) : base("Event Script Exception", ex) {
			this.Node = node;
			this.executionPhase = executionPhase;
		}

		public LayoutEventScriptException(LayoutEventScriptNode node, string message) : base(message) {
			this.Node = node;
		}

        public LayoutEventScriptNode Node { get; }

        public LayoutEventScriptExecutionPhase ExecutionPhase => executionPhase;
    }

	public class LayoutEventScriptErrorInfo {
		ILayoutScript					script;
		LayoutEventScriptTask			task;
		Exception						ex;
		LayoutEventScriptExecutionPhase	executionPhase;
		LayoutEventScriptNode			node;


		public LayoutEventScriptErrorInfo(ILayoutScript script, LayoutEventScriptTask task, LayoutEventScriptNode node, LayoutEventScriptExecutionPhase executionPhase, Exception ex) {
			this.script = script;
			this.task = task;
			this.node = node;
			this.executionPhase = executionPhase;
			this.ex = ex;
		}

        public ILayoutScript Script => script;

        public LayoutEventScriptTask Task => task;

        public LayoutEventScriptNode Node => node;

        public LayoutEventScriptExecutionPhase ExecutionPhase => executionPhase;

        public Exception Exception => ex;
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

		object ScriptSubject {
			get;
			set;
		}

		string Description {
			get;
		}
	}

	public class LayoutEventScriptTask : IDisposable {
		XmlElement						scriptElement;
		LayoutEventScriptNode			root;
		ILayoutScript					script;
		bool							taskTerminated;

		public LayoutEventScriptTask(ILayoutScript eventScript, XmlElement scriptElement, LayoutScriptContext context) {
			this.script = eventScript;
			this.scriptElement = scriptElement;

			root = Parse(scriptElement, context);
		}

		public LayoutEventScriptNode Parse(XmlElement elementToParse, LayoutScriptContext context) {
			LayoutEventScriptNode	node = (LayoutEventScriptNode)EventManager.Event(new LayoutEvent(
				new LayoutParseEventScript(script, this, elementToParse, context), "parse-event-script-definition"));

			if(node == null)
				throw new LayoutEventScriptParseException(script, elementToParse, "Unrecognized event script option: '" + elementToParse.Name + "'");

			return node;
		}

        public LayoutEventScriptNode Root => root;

        public LayoutEventScriptNodeEventBase EventRoot => Root as LayoutEventScriptNodeEventBase;

        public LayoutEventScriptNodeCondition ConditionRoot => Root as LayoutEventScriptNodeCondition;

        LayoutEventScript EventScript => script as LayoutEventScript;

        public void RecalculateTask() {
			try {
				EventRoot.Recalculate();
			} catch(LayoutEventScriptException ex) {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-error", null, 
					new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException)));
			} catch(Exception ex) {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-error", null,
					new LayoutEventScriptErrorInfo(EventScript, this, root, LayoutEventScriptExecutionPhase.Evaluation, ex)));
			}

			if(EventRoot.Occurred && !taskTerminated) {
				EventScript.OnTaskTerminated(this);
				taskTerminated = true;
				Dispose();
			}
		}

		public void Reset() {
			taskTerminated  = false;

			try {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-task-reset", null, this));
				EventRoot.Reset();
			} catch(LayoutEventScriptException ex) {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-error", null, 
					new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException)));
			} catch(Exception ex) {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-error", null, 
					new LayoutEventScriptErrorInfo(EventScript, this, root, LayoutEventScriptExecutionPhase.Reset, ex)));
			}

			if(EventRoot.Occurred && !taskTerminated) {
				EventScript.OnTaskTerminated(this);
				taskTerminated = true;
				Dispose();
			}
		}

        public bool Occurred => EventRoot.Occurred;

        public bool IsErrorState => EventRoot.IsErrorState;

        public string Description => (string)EventManager.Event(new LayoutEvent(scriptElement, "get-event-script-description"));

        public void Cancel() {
			EventRoot.Cancel();
		}

		public void Dispose() {
			try {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-task-dispose", null, this));
				EventRoot.Dispose();
			} catch(LayoutEventScriptException ex) {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-error", null, 
					new LayoutEventScriptErrorInfo(EventScript, this, ex.Node, ex.ExecutionPhase, ex.InnerException)));
			} catch(Exception ex) {
				EventManager.Event(new LayoutEvent(EventScript, "event-script-error", null, 
					new LayoutEventScriptErrorInfo(EventScript, this, root, LayoutEventScriptExecutionPhase.Disposing, ex)));
			}
		}
	}

	public class LayoutEventScript : ILayoutScript, IDisposable, IObjectHasId {
		LayoutEvent						scriptDoneEvent;
		LayoutEvent						errorOccurredEvent;
		XmlElement						scriptElement;
		ICollection<Guid>				scopeIDs;
		Guid							scriptID;
		string							name;
		object							scriptSubject;
		LayoutScriptContext				scriptContext;
		LayoutScriptContext				parentContext;

		List<LayoutEventScriptTask>		tasks = new List<LayoutEventScriptTask>();
		LayoutEventScriptTask			rootTask;

		public LayoutEventScript(string scriptName, XmlElement scriptElement, 
			ICollection<Guid> scopeIDs, LayoutEvent scriptDoneEvent, LayoutEvent errorOccurredEvent) {
			this.name = scriptName;
			this.scriptElement = scriptElement;
			this.scopeIDs = scopeIDs;
			this.scriptDoneEvent = scriptDoneEvent;
			this.errorOccurredEvent = errorOccurredEvent;

			this.scriptID = Guid.NewGuid();
		}

		public LayoutScriptContext ScriptContext {
			get {
				if(scriptContext == null) {
					if(parentContext == null)
						parentContext = (LayoutScriptContext)EventManager.Event(new LayoutEvent(this, "get-global-event-script-context"));

					scriptContext = new LayoutScriptContext("Script", parentContext);
					scriptContext.CopyOnClone = false;
				}

				return scriptContext;
			}

			set {
				scriptContext = value;
			}
		}

		public LayoutScriptContext ParentContext {
			get {
				return parentContext;
			}

			set {
				parentContext = value;
			}
		}

		public LayoutEventScriptTask RootTask {
			get {
				if(rootTask == null)
					rootTask = AddTask(scriptElement, ScriptContext);

				return rootTask;
			}
		}

		public Guid Id {
			get {
				return scriptID;
			}

			set {
				scriptID = value;
			}
		}

        /// <summary>
        /// The scope for which this event script belongs. This mean that unless otherwise specified, the script
        /// will process only events that are sent (or their info field) is an object with this ID.
        /// </summary>
        public ICollection<Guid> ScopeIDs => scopeIDs;

        public object ScriptSubject {
			get {
				return scriptSubject;
			}

			set {
				scriptSubject = value;
			}
		}

        /// <summary>
        /// The script name, used for identification in scenarios such as debugging
        /// </summary>
        public string Name => name;

        public LayoutEventScriptTask AddTask(XmlElement scriptElement, LayoutScriptContext context) {
			LayoutEventScriptTask	task = new LayoutEventScriptTask(this, scriptElement, context);

			tasks.Add(task);
			EventManager.Event(new LayoutEvent(this, "event-script-task-created", null, task));
			return task;
		}

		internal void OnTaskTerminated(LayoutEventScriptTask task) {
			EventManager.Event(new LayoutEvent(this, "event-script-task-terminated", null, task));

			if(task == RootTask) {
				EventManager.Event(new LayoutEvent(this, "event-script-terminated"));
				if(task.EventRoot.IsErrorState && errorOccurredEvent != null)
					EventManager.Event(errorOccurredEvent);
				else if(scriptDoneEvent != null)
					EventManager.Event(scriptDoneEvent);
			}
			else
				tasks.Remove(task);
		}

		public void Reset() {
			foreach(LayoutEventScriptTask task in tasks)
				if(task != RootTask)
					task.Cancel();
			tasks.Clear();

			tasks.Add(RootTask);
			EventManager.Event(new LayoutEvent(this, "event-script-reset", null, RootTask));

			RootTask.Reset();

		}

        public bool Occurred => RootTask.Occurred;

        public bool IsErrorState => RootTask.IsErrorState;

        public string Description => (string)EventManager.Event(new LayoutEvent(scriptElement, "get-event-script-description"));

        public void Dispose() {
			EventManager.Event(new LayoutEvent(this, "event-script-dispose"));

			foreach(LayoutEventScriptTask task in tasks) {
				try {
					task.Dispose();
				} catch(LayoutEventScriptException ex) {
					EventManager.Event(new LayoutEvent(this, "event-script-error", null, 
						new LayoutEventScriptErrorInfo(this, task, ex.Node, ex.ExecutionPhase, ex.InnerException)));
				} catch(Exception ex) {
					EventManager.Event(new LayoutEvent(this, "event-script-error", null, 
						new LayoutEventScriptErrorInfo(this, task, task.Root, LayoutEventScriptExecutionPhase.Disposing, ex)));
				}
			}

			tasks.Clear();
			if(rootTask != null) {
				rootTask.Dispose();
				rootTask = null;
			}
		}
	}

	#endregion

	#region Layout Condition Script

	public class LayoutConditionScript : ILayoutScript {
		XmlElement						element;
		string							name;
		object							scriptSubject;
		LayoutScriptContext				scriptContext;
		bool							defaultValue = true;

		public LayoutConditionScript(XmlElement element) {
			this.element = element;
			this.name = "Condition";

			initialize();
		}

		public LayoutConditionScript(XmlElement element, bool defaultValue) {
			this.element = element;
			this.name = "Condition";
			this.defaultValue = defaultValue;

			initialize();
		}

		public LayoutConditionScript(string name, XmlElement element) {
			this.element = element;
			this.name = name;

			initialize();
		}

		public LayoutConditionScript(string name, XmlElement element, bool defaultValue) {
			this.element = element;
			this.name = name;
			this.defaultValue = defaultValue;

			initialize();
		}

		private void initialize() {
		}

		public bool IsTrue {
			get {
				if(element == null || element.ChildNodes.Count < 1)
					return defaultValue;

				LayoutEventScriptTask	task = new LayoutEventScriptTask(this, (XmlElement)element.ChildNodes[0], ScriptContext);

				return task.ConditionRoot.IsTrue;
			}
		}

		#region ILayoutScript Members

		public string Name {
			get {
				return name;
			}

			set {
				name = value;
			}
		}

		public LayoutScriptContext ScriptContext {
			get {
				if(scriptContext == null) {
					LayoutScriptContext	globalContext = (LayoutScriptContext)EventManager.Event(new LayoutEvent(this, "get-global-event-script-context"));

					scriptContext = new LayoutScriptContext("Script", globalContext);
					scriptContext.CopyOnClone = false;
				}

				return scriptContext;
			}
		}

		public object ScriptSubject {
			get {
				return scriptSubject;
			}

			set {
				scriptSubject = value;
			}
		}

		public string Description {
			get {
				if(element == null || element.ChildNodes.Count < 1)
					return "";

				return (string)EventManager.Event(new LayoutEvent(element.ChildNodes[0], "get-event-script-description"));
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
		ILayoutScript				script;
		LayoutEventScriptTask		eventScriptTask;
		XmlElement					element;
		LayoutScriptContext	context;

		public LayoutParseEventScript(ILayoutScript script, LayoutEventScriptTask eventScriptTask, XmlElement element, LayoutScriptContext context) {
			this.script = script;
			this.eventScriptTask = eventScriptTask;
			this.element = element;
			this.context = context;
		}

        /// <summary>
        /// The conditonal event being built
        /// </summary>
        public ILayoutScript Script => script;

        /// <summary>
        /// Return the event script task for which this element is parsed
        /// </summary>
        public LayoutEventScriptTask Task => eventScriptTask;

        /// <summary>
        /// The XML element representing the condition
        /// </summary>
        public XmlElement Element => element;

        /// <summary>
        /// Return the context associated with this node
        /// </summary>
        public LayoutScriptContext Context {
			get {
				return context;
			}

			set {
				context = value;
			}
		}
	}

	public class LayoutEventScriptParseException : Exception {
		ILayoutScript			script;
		XmlElement				parsedNode;

		public LayoutEventScriptParseException(ILayoutScript script,
			XmlElement parsedNode, string message) : base(message) {
			this.script = script;
			this.parsedNode = parsedNode;
		}

        public ILayoutScript Script => script;

        public XmlElement ParsedNode => parsedNode;
    }

	#endregion

	#region Context

	public interface ILayoutScriptContextResolver {
		object Resolve(LayoutScriptContext context, string symbolName);
	}

	public class LayoutScriptContext : ICloneable {
		string						name;
		Dictionary<string, object>	symbols;
		LayoutScriptContext			parentContext;
		bool						copyOnClone = true;

		public LayoutScriptContext(string name, LayoutScriptContext parentContext) {
			this.name = name;
			this.parentContext = parentContext;
			symbols = new Dictionary<string,object>();
		}

		protected LayoutScriptContext(string name, LayoutScriptContext parentContext, Dictionary<string, object> symbols) {
			this.name = name;
			this.parentContext = parentContext;
			this.symbols = symbols;
		}

        public string Name => name;

        public bool CopyOnClone {
			get {
				return copyOnClone;
			}

			set {
				copyOnClone = value;
			}
		}

        public LayoutScriptContext ParentContext => parentContext;

        public object SearchSymbol(string symbolName) {
			object symbolValue;

			symbols.TryGetValue(symbolName, out symbolValue);
			return symbolValue;
		}


		public LayoutScriptContext SearchContext(string name) {
			if(name == "Parent") {
				if(parentContext != null)
					return parentContext;
			}
			else if(name == "This")
				return this;
			else {
				for(LayoutScriptContext context = this; context != null; context = context.ParentContext)
					if(name == context.Name)
						return context;
			}

			return null;
		}

		public object this[string symbolName] {
			get {
				string[]	symbolParts = symbolName.Split(':');
				object		symbolValue = null;
				string		name = symbolName;

				if(symbolParts.Length == 1) {
					for(LayoutScriptContext context = this; context != null; context = context.ParentContext) {
						symbolValue = context.SearchSymbol(symbolName);
						if(symbolValue != null)
							break;
					}
				}
				else if(symbolParts.Length == 2) {
					LayoutScriptContext	context = SearchContext(symbolParts[0]);

					if(context != null) {
						name = symbolParts[1];
						symbolValue = context.SearchSymbol(name);
					}
				}
				else
					throw new ArgumentException("Invalid event script symbol reference: " + symbolName);

				ILayoutScriptContextResolver symbolIsContextResolver = symbolValue as ILayoutScriptContextResolver;

				if(symbolIsContextResolver != null)
					symbolValue = symbolIsContextResolver.Resolve(this, name);

				return symbolValue;
			}

			set {
				string[]	symbolParts = symbolName.Split(':');

				if(symbolParts.Length == 1)
					symbols[symbolName] = value;
				else {
					LayoutScriptContext	context = SearchContext(symbolParts[0]);

					if(context != null)
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
		public bool Contains(string symbolName, object rawSymbolValue) {
			IObjectHasId	symbolValue = rawSymbolValue as IObjectHasId;

			if(symbolValue != null) {
				object previousValueObject;
				IObjectHasId	previousValue;

				if(symbols.TryGetValue(symbolName, out previousValueObject) && (previousValue = previousValueObject as IObjectHasId) != null && previousValue.Id == symbolValue.Id)
					return true;
			}

			return false;
		}

		public object GetProperty(string symbolName, object symbolValue, string propertyName) {
			PropertyInfo	propertyInfo = symbolValue.GetType().GetProperty(propertyName);

			if(propertyInfo == null) {
				PropertyInfo	infoPropertyInfo = symbolValue.GetType().GetProperty("Info");

				if(infoPropertyInfo != null) {
					object	infoObject = infoPropertyInfo.GetValue(symbolValue, null);

					if(infoObject != null) {
						propertyInfo = infoObject.GetType().GetProperty(propertyName);

						if(propertyInfo != null)
							return propertyInfo.GetValue(infoObject, null);
					}
				}

				throw new ArgumentException("Object " + symbolName + " does not have property named " + propertyName);
			}

			return propertyInfo.GetValue(symbolValue, null);
		}

		public object GetSymbolProperty(string symbolName, string propertyName) {
			object	symbolValue = this[symbolName];

			if(symbolValue != null)
				return GetProperty(symbolName, symbolValue, propertyName);
			else
				throw new ArgumentException("Event script context does not contain object named " + symbolName);
		}

		public object GetAttribute(string symbolName, object symbolValue, string attributeName) {
			IObjectHasAttributes symbolWithAttributes = symbolValue as IObjectHasAttributes;

			if(symbolWithAttributes != null)
				return symbolWithAttributes.Attributes[attributeName];
			else
				throw new ArgumentException("Object " + symbolName + " does not support attributes");
		}

		public object GetSymbolAttribute(string symbolName, string attributeName) {
			object	symbolValue = this[symbolName];

			if(symbolValue != null)
				return GetAttribute(symbolName, symbolValue, attributeName);
			else
				throw new ArgumentException("Event script context does not contain object named " + symbolName);
		}

		public object Clone() {
			if(!copyOnClone)
				return this;

			Dictionary<string, object>	newSymbols = new Dictionary<string, object>(symbols.Count);

			foreach(KeyValuePair<string, object> d in symbols)
				newSymbols.Add(d.Key, d.Value);

			return new LayoutScriptContext(name, (LayoutScriptContext)parentContext.Clone(), newSymbols);
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
		LayoutParseEventScript		parseEventInfo;

		protected LayoutEventScriptNode(LayoutEvent e) {
			this.parseEventInfo = (LayoutParseEventScript)e.Sender;
		}

        public XmlElement Element => parseEventInfo.Element;

        public ILayoutScript Script => parseEventInfo.Script;

        public LayoutEventScriptTask Task => parseEventInfo.Task;

        public LayoutScriptContext Context {
			get {
				return parseEventInfo.Context;
			}

			set {
				parseEventInfo.Context = value;
			}
		}

        protected LayoutEventScriptNode Parse(XmlElement elementToParse) => Task.Parse(elementToParse, Context);

        protected LayoutEventScriptParseException ParseErrorException(string message) => new LayoutEventScriptParseException(Script, Element, message);

        #region Common Access methods

        protected object GetOperand(XmlElement element, string symbolName, object symbolValue, string symbolAccess, string suffix) {
			string	tagName = element.GetAttribute("Name" + suffix);

			if(symbolAccess == "Property")
				return Context.GetProperty(symbolName, symbolValue, tagName);
			else if(symbolAccess == "Attribute")
				return Context.GetAttribute(symbolName, symbolValue, tagName);
			else
				throw new ArgumentException("Invalid symbol access method " + symbolAccess + " for symbol " + symbolName);
		}

        protected object GetOperand(string symbolName, object symbolValue, string symbolAccess, string suffix) => GetOperand(Element, symbolName, symbolValue, symbolAccess, suffix);

        protected object GetOperand(XmlElement element, string suffix) {
			string	symbolAccess = element.GetAttribute("Symbol" + suffix + "Access");

			if(symbolAccess == "Value") {
				string	v = element.GetAttribute("Value" + suffix);

				switch(element.GetAttribute("Type" + suffix)) {

					case "Boolean":
						return XmlConvert.ToBoolean(v);

					case "Integer":
						return XmlConvert.ToInt32(v);

					case "Double":
						return XmlConvert.ToDouble(v);

					default:
						return v;
				}
			}
			else {
				string	symbolName = element.GetAttribute("Symbol" + suffix);
				object	symbolValue = Context[symbolName];

				if(symbolValue == null)
					return null;
				else {
					if(symbolValue.GetType().IsArray)
						return symbolValue;
					else
						return GetOperand(element, symbolName, symbolValue, symbolAccess, suffix);
				}
			}
		}

        protected object GetOperand(string suffix) => GetOperand(Element, suffix);

        #endregion
    }

	#region Events

	public abstract class LayoutEventScriptNodeEventBase : LayoutEventScriptNode, IDisposable {
		bool			_occurred;
		bool			_isErrorState;
		bool			_stateChanged;
		bool			_cancelActions;
		LayoutEventScriptNodeCondition	condition;
		LayoutEventScriptNodeActions	actions;

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
			XmlElement	conditionTitleElement = eventElement["Condition"];

			if(conditionTitleElement != null) {
				if(conditionTitleElement.ChildNodes.Count != 1)
					throw ParseErrorException("Missing condition, or more than one condition for event " + Element.Name);

				XmlElement	conditionElement = (XmlElement)conditionTitleElement.ChildNodes[0];

				condition = Parse(conditionElement) as LayoutEventScriptNodeCondition;

				if(condition == null)
					throw ParseErrorException("Invalid condition or condition container: " + conditionElement.Name);
			}
		}

		/// <summary>
		/// Parse 'Actions' element
		/// </summary>
		/// <param name="eventElement">The event XML element</param>
		protected void ParseActions(XmlElement eventElement) {
			XmlElement	actionsElement = eventElement["Actions"];

			if(actionsElement != null) {
				actions = (LayoutEventScriptNodeActions)Parse(actionsElement);
			}
		}

		/// <summary>
		/// Get/Set whether the event occured
		/// </summary>
		public bool Occurred {
			get {
				return _occurred;
			}

			set {
				if(value == true && _occurred == false) {
					_stateChanged = true;
					_occurred = true;

					try {
						if(Actions != null && !_cancelActions)
							Actions.Execute();
					} catch(Exception ex) {
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
		public bool CancelActions {
			get {
				return _cancelActions;
			}

			set {
				_cancelActions = value;
			}
		}

		/// <summary>
		/// Check the condition. If the event is not associated with condition, the condition assumed to be true
		/// </summary>
		protected bool IsConditionTrue {
			get {
				try {
					if(Condition == null || Condition.IsTrue)
						return true;
				} catch(Exception ex) {
					throw new LayoutEventScriptException(this, LayoutEventScriptExecutionPhase.ConditionProcessing, ex);
				}

				return false;
			}
		}

		/// <summary>
		/// Get/Set whether the occurrence of this event signify an error state
		/// </summary>
		public bool IsErrorState {
			get {
				return _isErrorState;
			}

			set {
				if(value != _isErrorState && value != false)
					_stateChanged = true;

				_isErrorState = value;
			}
		}

		public bool IsOptional {
			get {
				if(Element.HasAttribute("Optional"))
					return XmlConvert.ToBoolean(Element.GetAttribute("Optional"));
				return false;
			}
		}

		/// <summary>
		/// Get/Set whether the event state has changed (you should not normally set this directly)
		/// </summary>
		protected bool StateChanged {
			get {
				return _stateChanged;
			}

			set {
				_stateChanged = value;
			}
		}

		/// <summary>
		/// Reset the event
		/// </summary>
		public virtual void Reset() {
			Occurred = false;
			CancelActions = false;

			if(Element.HasAttribute("IsError"))
				IsErrorState = XmlConvert.ToBoolean(Element.GetAttribute("IsError"));
		}

		/// <summary>
		/// Re-calculate the script. This should be called whenever an event occurred.
		/// </summary>
		protected void RecalculateTask() {
			bool	shouldRecalc = StateChanged;

			StateChanged = false;

			if(shouldRecalc)
				Task.RecalculateTask();
		}

        /// <summary>
        /// Get the condition associated with this event
        /// </summary>
        protected LayoutEventScriptNodeCondition Condition => condition;

        /// <summary>
        /// Get the actions to perform when this event occurred
        /// </summary>
        protected LayoutEventScriptNodeActions Actions => actions;
    }

	/// <summary>
	/// Base class events (or event containers)
	/// </summary>
	public class LayoutEventScriptNodeEvent : LayoutEventScriptNodeEventBase {
		string					eventName;
		LayoutEventSubscriptionBase	subscription;

		public LayoutEventScriptNodeEvent(LayoutEvent e) : base(e) {
			if(Element.HasAttribute("Name"))
				eventName = Element.GetAttribute("Name");

			ParseCondition(Element);
			ParseActions(Element);
		}

		protected bool LimitToScope {
			get {
				if(Element.HasAttribute("LimitToScope"))
					return XmlConvert.ToBoolean(Element.GetAttribute("LimitToScope"));
				return true;
			}
		}

        /// <summary>
        /// You may override this with the event name to subscribe to
        /// </summary>
        protected virtual string EventName => eventName;

        public override void Reset() {
			base.Reset();

			try {
				if(!Occurred && EventName != null) {
					LayoutEventAttribute subscriptionInfo = new LayoutEventAttribute(EventName);

					subscription = subscriptionInfo.CreateSubscription();
					subscription.SetFromLayoutEventAttribute(subscriptionInfo);
					subscription.Order = 10000;			// Ensure that built in event handler execute first
					subscription.SetMethod(this, this.GetType().GetMethod("OnEvent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

					EventManager.Subscriptions.Add(subscription);
				}

				EventManager.Event(new LayoutEvent(this, "event-script-wait-event-reset", null, subscription));
			}
			catch(Exception ex) {
				throw new LayoutEventScriptException(this, LayoutEventScriptExecutionPhase.Reset, ex);
			}
		}

		public override void Cancel() {
			if(subscription != null) {
				EventManager.Subscriptions.Remove(subscription);
				subscription = null;
			}
		}

		protected virtual void OnEvent(LayoutEvent e) {
			bool	relevantEvent = false;

			try {
				LayoutEventScript eventScript = (LayoutEventScript)Script;

				// Check if the event need to be limited to scope
				if(LimitToScope && eventScript.ScopeIDs != null && eventScript.ScopeIDs.Count != 0) {

					// Check that the sender or info ID is matching the scopeID. If this is not the case,
					// the event is not relevant to the scope of the current event script

					if(e.Sender is IObjectHasId) {
						IObjectHasId sender = (IObjectHasId)e.Sender;

						foreach(Guid scopeID in eventScript.ScopeIDs)
							if(scopeID == sender.Id) {
								relevantEvent = true;
								break;
							}
					}

					if(!relevantEvent && e.Info is IObjectHasId) {
						IObjectHasId info = (IObjectHasId)e.Info;

						foreach(Guid scopeID in eventScript.ScopeIDs)
							if(scopeID == info.Id) {
								relevantEvent = true;
								break;
							}
					}
				}
				else
					relevantEvent = true;

				if(relevantEvent) {
					if(e.Sender != null)
						EventManager.Event(new LayoutEvent(e.Sender, "set-script-context", null, Context));
					if(e.Info != null)
						EventManager.Event(new LayoutEvent(e.Info, "set-script-context", null, Context));

					if(IsConditionTrue) {
						Cancel();
						Occurred = true;
					}
				}
			}
			catch(LayoutEventScriptException ex) {
				EventManager.Event(new LayoutEvent(this.Script, "event-script-error", null,
					new LayoutEventScriptErrorInfo(this.Script, this.Task, ex.Node, ex.ExecutionPhase, ex.InnerException)));
			}
			catch(Exception ex) {
				EventManager.Event(new LayoutEvent(this.Script, "event-script-error", null,
					new LayoutEventScriptErrorInfo(this.Script, this.Task, this, LayoutEventScriptExecutionPhase.EventProcessing, ex)));
			}
		}
	}

	public abstract class LayoutEventScriptNodeEventContainer : LayoutEventScriptNodeEventBase {
		List<LayoutEventScriptNodeEventBase> events = new List<LayoutEventScriptNodeEventBase>();

		protected LayoutEventScriptNodeEventContainer(LayoutEvent e) : base(e) {
			XmlElement	eventsElement = Element["Events"];

			// Create new context for the event container
			Context = new LayoutScriptContext(Element.Name, Context);

			if(eventsElement != null) {
				foreach(XmlElement eventElement in eventsElement) {
					LayoutEventScriptNodeEventBase	eventNode = Parse(eventElement) as LayoutEventScriptNodeEventBase;

					if(eventNode != null)
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
			foreach(LayoutEventScriptNodeEventBase eventNode in events)
				eventNode.Cancel();
		}

		public override void Dispose() {
			foreach(LayoutEventScriptNodeEventBase eventNode in events)
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
		List<LayoutEventScriptNodeCondition>		conditions = new List<LayoutEventScriptNodeCondition>();

		protected LayoutEventScriptNodeConditionContainer(LayoutEvent e) : base(e) {
			foreach(XmlElement conditionElement in Element) {
				LayoutEventScriptNodeCondition	condition = Parse(conditionElement) as LayoutEventScriptNodeCondition;

				if(condition == null)
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
		ArrayList	actions = new ArrayList();

		public LayoutEventScriptNodeActions(LayoutEvent e) : base(e) {
			foreach(XmlElement elementAction in Element) {
				LayoutEventScriptNodeAction	action = (LayoutEventScriptNodeAction)Parse(elementAction) as LayoutEventScriptNodeAction;

				if(action == null)
					throw ParseErrorException("Invalid action: " + elementAction.Name);

				actions.Add(action);
			}
		}

		public void Execute() {
			foreach(LayoutEventScriptNodeAction action in actions)
				action.Execute();
		}
	}

	#endregion

	#endregion

	#endregion

	#region Standard event nodes

	[LayoutModule("Common Event Condition Handlers")]
	class CommonEventConditionHandler : LayoutModuleBase {

		#region Event Containers

		#region Any - Any condition occured

		[LayoutEvent("parse-event-script-definition", IfSender="Any")]
		private void ParseAny(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEventContainerAny(e);
		}

		class LayoutEventScriptNodeEventContainerAny : LayoutEventScriptNodeEventContainer {
			public LayoutEventScriptNodeEventContainerAny(LayoutEvent e) : base(e) {
			}

			public override void Recalculate() {
				if(!Occurred) {
					LayoutEventScriptNodeEventBase	occuredEvent = null;

					foreach(LayoutEventScriptNodeEventBase eventNode in Events) {
						eventNode.Recalculate();

						if(eventNode.Occurred && !eventNode.IsOptional) {
							occuredEvent = eventNode;
							break;
						}
					}

					if(occuredEvent != null) {
						foreach(LayoutEventScriptNodeEventBase eventNode in Events) {
							if(eventNode != occuredEvent)
								eventNode.Cancel();
						}

						if(!IsConditionTrue)
							CancelActions = true;

						Occurred = true;
					}
				}
			}

			public override void Reset() {
				base.Reset();

				foreach(LayoutEventScriptNodeEventBase eventNode in Events)
					eventNode.Reset();
			}
		}

		#endregion

		#region All - All conditions occured (order does not matter)

		[LayoutEvent("parse-event-script-definition", IfSender="All")]
		private void ParseAll(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEventContainerAll(e);
		}

		class LayoutEventScriptNodeEventContainerAll : LayoutEventScriptNodeEventContainer {
			public LayoutEventScriptNodeEventContainerAll(LayoutEvent e) : base(e) {
			}

			public override void Recalculate() {
				if(!Occurred) {
					bool	failed = false;
					bool	isErrorState = false;

					foreach(LayoutEventScriptNodeEventBase eventNode in Events) {
						eventNode.Recalculate();

						if(!eventNode.Occurred && !eventNode.IsOptional)
							failed = true;
						else {
							if(eventNode.IsErrorState)
								isErrorState = true;
						}
					}

					if(!failed) {
						foreach(LayoutEventScriptNodeEventBase eventNode in Events) {
							if(!eventNode.Occurred)
								eventNode.Cancel();
						}

						IsErrorState = isErrorState;

						if(!IsConditionTrue)
							CancelActions = true;

						Occurred = true;
					}
				}
			}

			public override void Reset() {
				base.Reset();

				foreach(LayoutEventScriptNodeEventBase eventNode in Events) {
					eventNode.Reset();
				}
			}
		}

		#endregion

		#region Sequence - All conditions occured in sequence (order matters)

		[LayoutEvent("parse-event-script-definition", IfSender="Sequence")]
		private void ParseSequence(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEventContainerSequence(e);
		}

		class LayoutEventScriptNodeEventContainerSequence : LayoutEventScriptNodeEventContainer {
			int		seqIndex;

			public LayoutEventScriptNodeEventContainerSequence(LayoutEvent e) : base(e) {
			}

			public override void Recalculate() {
				if(!Occurred) {
					bool	proceed = true;

					while(proceed) {
						Events[seqIndex].Recalculate();

						if(Events[seqIndex].Occurred) {
							if(Events[seqIndex].IsErrorState)
								IsErrorState = true;

							if(seqIndex < Events.Count-1) {
								seqIndex++;
								Events[seqIndex].Reset();
							}
							else {
								Occurred = true;		// Sequence is done
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

				if(IsConditionTrue) {
					if(Events.Count > 0)
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

		[LayoutEvent("parse-event-script-definition", IfSender="Repeat")]
		private void ParseRepeat(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEventContainerRepeat(e);
		}

		class LayoutEventScriptNodeEventContainerRepeat : LayoutEventScriptNodeEventContainer {
			int								numberOfIterations;
			int								iterationCount;
			LayoutEventScriptNodeEventBase	repeatedEvent;

			public LayoutEventScriptNodeEventContainerRepeat(LayoutEvent e) : base(e) {
				if(!Element.HasAttribute("Count"))
					throw ParseErrorException("Missing iteration count in repeat (Count)");

				numberOfIterations = XmlConvert.ToInt32(Element.GetAttribute("Count"));

				if(Element.ChildNodes.Count < 1)
					throw ParseErrorException("Missing element to repeat");
				else if(Element.ChildNodes.Count > 1)
					throw ParseErrorException("Too many elements to repeat");

				XmlElement	repeatedElement = (XmlElement)Element.ChildNodes[0];

				repeatedEvent = Parse(repeatedElement) as LayoutEventScriptNodeEventBase;

				if(repeatedEvent == null)
					throw ParseErrorException("Repeated element " + repeatedElement.Name + " is not valid event definition");
			}

			public override void Reset() {
				base.Reset();

				iterationCount = 0;
				repeatedEvent.Reset();
			}

			public override void Recalculate() {
				if(!Occurred) {
					repeatedEvent.Recalculate();

					if(repeatedEvent.Occurred) {
						if(repeatedEvent.IsErrorState)
							IsErrorState = true;

						if(numberOfIterations >= 0 && iterationCount++ >= numberOfIterations)
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

		[LayoutEvent("parse-event-script-definition", IfSender="RandomChoice")]
		private void ParseRandomChoice(LayoutEvent e) 
		{
			e.Info = new LayoutEventScriptNodeEventContainerRandomChoice(e);
		}

		class LayoutEventScriptNodeEventContainerRandomChoice : LayoutEventScriptNodeEventContainer 
		{
			List<ChoiceEntry> choices = new List<ChoiceEntry>();
			LayoutEventScriptNodeEventBase	chosenNode;

			public LayoutEventScriptNodeEventContainerRandomChoice(LayoutEvent e) : base(e) {
				foreach(XmlElement choiceElement in Element.GetElementsByTagName("Choice")) {
					int								weight = XmlConvert.ToInt32(choiceElement.GetAttribute("Weight"));
					LayoutEventScriptNodeEventBase	node = (LayoutEventScriptNodeEventBase)Parse((XmlElement)choiceElement.ChildNodes[0]);

					choices.Add(new ChoiceEntry(weight, node));
				}
			}

			public override void Reset() {
				base.Reset();

				if(choices.Count == 1) {
					int			random = new Random().Next(100);
					ChoiceEntry	choice = choices[0];

					if(random < choice.Weight)
						chosenNode = choice.Node;
				}
				else {
					int	total = 0;

					foreach(ChoiceEntry choice in choices)
						total += choice.Weight;

					int	random = new Random().Next(total);

					total = 0;
					foreach(ChoiceEntry choice in choices) {
						if(total + choice.Weight > random) {
							chosenNode = choice.Node;
							break;
						}
						else
							total += choice.Weight;
					}
				}

				if(chosenNode != null) {
					chosenNode.Reset();
				}
				else
					Occurred = true;
			}

			public override void Recalculate() {
				if(!Occurred) {
					chosenNode.Recalculate();

					if(chosenNode.Occurred)
						Occurred = true;
				}
			}

			public override void Cancel() {
				chosenNode.Cancel();
				base.Cancel();
			}

			public override void Dispose() {
				chosenNode.Dispose();
				base.Dispose();
			}

			class ChoiceEntry {
				int								weight;
				LayoutEventScriptNodeEventBase	node;

				public ChoiceEntry(int weight, LayoutEventScriptNodeEventBase node) {
					this.weight = weight;
					this.node = node;
				}

                public int Weight => weight;

                public LayoutEventScriptNodeEventBase Node => node;
            }
		}

		#endregion

		#region Task

		[LayoutEvent("parse-event-script-definition", IfSender="Task")]
		private void ParseTask(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEventContainerTask(e);
		}

		class LayoutEventScriptNodeEventContainerTask : LayoutEventScriptNodeEventBase {
			XmlElement	taskElement;
			string		taskName = "Task";

			public LayoutEventScriptNodeEventContainerTask(LayoutEvent e) : base(e) {
				if(Element.ChildNodes.Count < 1)
					throw ParseErrorException("Missing element to repeat");
				else if(Element.ChildNodes.Count > 1)
					throw ParseErrorException("Too many elements to repeat");

				if(Element.HasAttribute("Name"))
					taskName = Element.GetAttribute("Name");

				taskElement = (XmlElement)Element.ChildNodes[0];
			}

			public override void Recalculate() {
				if(!Occurred) {
					LayoutEventScript	eventScript = (LayoutEventScript)Script;

					LayoutEventScriptTask	task = eventScript.AddTask(taskElement, (LayoutScriptContext)Context.Clone());
					
					task.Reset();
					Occurred = true;
				}
			}

			public override void Cancel() {
			}

			public override void Reset() {
				base.Reset ();
				Recalculate();
			}

		}

		#endregion

		#endregion

		#region Events

		#region Wait for Event

		[LayoutEvent("parse-event-script-definition", IfSender="WaitForEvent")]
		private void parseEvent(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEvent(e);
		}

		#endregion

		#region Wait

		[LayoutEvent("parse-event-script-definition", IfSender="Wait")]
		private void parseWait(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEventWait(e);
		}

		class LayoutEventScriptNodeEventWait : LayoutEventScriptNodeEvent {
			int					delay;
			LayoutDelayedEvent	delayedEvent;

			public LayoutEventScriptNodeEventWait(LayoutEvent e) : base(e) {
				if(Element.HasAttribute("MilliSeconds"))
					delay += XmlConvert.ToInt32(Element.GetAttribute("MilliSeconds"));
				if(Element.HasAttribute("Seconds"))
					delay += XmlConvert.ToInt32(Element.GetAttribute("Seconds")) * 1000;
				if(Element.HasAttribute("Minutes"))
					delay += XmlConvert.ToInt32(Element.GetAttribute("Minutes")) * 1000 * 60;

				if(Element.HasAttribute("RandomSeconds"))
					delay += new Random().Next(XmlConvert.ToInt32(Element.GetAttribute("RandomSeconds")) * 1000);
			}

			public override void Reset() {
				// Note that the reset operation may set Occured to true, if the condition is prechecked and is true
				base.Reset();

				if(delayedEvent != null) {
					delayedEvent.Cancel();
					delayedEvent = null;
				}

				if(!Occurred)
					delayedEvent = EventManager.DelayedEvent(delay, new LayoutEvent(this, "wait-event-condition-occured"));
			}

			public override void Cancel() {
				if(delayedEvent != null) {
					delayedEvent.Cancel();
					delayedEvent = null;
				}
			}

			public override void Dispose() {
				if(delayedEvent != null) {
					delayedEvent = null;
				}

				base.Dispose();
			}

			[LayoutEvent("wait-event-condition-occured")]
			private void WaitConditionDone(LayoutEvent e) {
				if(e.Sender == this) {
					try {
						if(delayedEvent != null) {			// Make sure that the event was not canceled (race condition)
							Debug.Assert(e.Sender == this);

							if(IsConditionTrue)
								Occurred = true;
							else		// Condition is not true, wait again and check condition again
								delayedEvent = EventManager.DelayedEvent(delay, new LayoutEvent(this, "wait-event-condition-occured"));
						}
					} catch(Exception ex) {
						Trace.WriteLine("Exception thrown while processing wait condition");
						Trace.WriteLine("-- " + ex.Message);
						Trace.WriteLine("-- " + ex.StackTrace);
					}
				}
			}
		}

		#endregion

		#region DoNow

		[LayoutEvent("parse-event-script-definition", IfSender="DoNow")]
		private void parseDoNow(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeEventDoNow(e);
		}

		class LayoutEventScriptNodeEventDoNow : LayoutEventScriptNodeEvent {
			public LayoutEventScriptNodeEventDoNow(LayoutEvent e) : base(e) {
			}

			public override void Reset() {
				base.Reset();

				if(!IsConditionTrue)
					CancelActions = true;

				Occurred = true;
			}
		}

		#endregion

		#endregion

		#region Condition Containers

		[LayoutEvent("parse-event-script-definition", IfSender="And")]
		private void parseAnd(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeConditionContainerAnd(e);
		}

		public class LayoutEventScriptNodeConditionContainerAnd : LayoutEventScriptNodeConditionContainer {

			public LayoutEventScriptNodeConditionContainerAnd(LayoutEvent e) : base(e) {
			}

			public override bool IsTrue {
				get {
					foreach(LayoutEventScriptNodeCondition condition in Conditions)
						if(!condition.IsTrue)
							return false;
					return true;
				}
			}
		}

		[LayoutEvent("parse-event-script-definition", IfSender="Or")]
		private void parseOr(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeConditionContainerOr(e);
		}

		public class LayoutEventScriptNodeConditionContainerOr : LayoutEventScriptNodeConditionContainer {

			public LayoutEventScriptNodeConditionContainerOr(LayoutEvent e) : base(e) {
			}

			public override bool IsTrue {
				get {
					foreach(LayoutEventScriptNodeCondition condition in Conditions)
						if(condition.IsTrue)
							return true;
					return false;
				}
			}
		}

		[LayoutEvent("parse-event-script-definition", IfSender="Not")]
		private void parseNot(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeConditionContainerNot(e);
		}

		public class LayoutEventScriptNodeConditionContainerNot : LayoutEventScriptNodeConditionContainer {

			public LayoutEventScriptNodeConditionContainerNot(LayoutEvent e) : base(e) {
				if(Conditions.Count != 1)
					throw ParseErrorException("Invalid number of sub-conditions for 'not'");
			}

            public override bool IsTrue => !Conditions[0].IsTrue;
        }

		#endregion

		#region Condition

		#region Base class for If conditions

		public abstract class LayoutEventScriptNodeIf : LayoutEventScriptNodeCondition {
			public LayoutEventScriptNodeIf(LayoutEvent e) : base(e) {
			}

			public override bool IsTrue {
				get {
					object	operand1 = GetOperand("1");
					object	operand2 = GetOperand("2");
					string	compareOperator = Element.GetAttribute("Operation");

					if(operand2 != null && operand2.GetType().IsArray)
						throw new ArgumentException("Operand2 cannot be array");

					if(operand1 != null && operand1.GetType().IsArray) {
						string	symbolName = Element.GetAttribute("Symbol1");
						string	symbolAccess = Element.GetAttribute("Symbol1Access");

						foreach(object symbolValue in (Array)operand1) {
							object	operand1value = GetOperand(symbolName, symbolValue, symbolAccess, "1");

							if(Compare(operand1value, compareOperator, operand2))
								return true;
						}

						return false;
					}
					else
						return Compare(operand1, compareOperator, operand2);
				}
			}

			protected abstract bool Compare(object operand1, string compareOperator, object operand2);
		}

		#endregion

		#region IfString 

		[LayoutEvent("parse-event-script-definition", IfSender="IfString")]
		private void parseIfString(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeIfString(e);
		}


		public class LayoutEventScriptNodeIfString : LayoutEventScriptNodeIf {

			public LayoutEventScriptNodeIfString(LayoutEvent e) : base(e) {
			}

			protected override bool Compare(object oOperand1, string compareOperation, object oOperand2) {
				if(oOperand1 == null || oOperand2 == null)
					return false;

				string	operand1 = oOperand1.ToString();
				string	operand2 = oOperand2.ToString();

				switch(compareOperation) {

					case "Equal":
						return operand1 == operand2;

					case "NotEqual":
						return operand1 != operand2;

					case "Match":
						return System.Text.RegularExpressions.Regex.IsMatch(operand1, operand2);

					default:
						throw new ArgumentException("Invalid compare operation: " + compareOperation);
				}
			}
		}

		#endregion

		#region IfNumber

		[LayoutEvent("parse-event-script-definition", IfSender="IfNumber")]
		private void parseIfNumber(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeIfNumber(e);
		}


		public class LayoutEventScriptNodeIfNumber : LayoutEventScriptNodeIf {

			public LayoutEventScriptNodeIfNumber(LayoutEvent e) : base(e) {
			}

			protected int GetNumber(object rawNumber) {
				string rawStringValue = rawNumber as string;

				if(rawStringValue != null)
					return int.Parse(rawStringValue);
				else if(rawNumber == null)
					return 0;
				else
					return (int)rawNumber;
			}

			protected override bool Compare(object rawOperand1, string compareOperation, object rawOperand2) {
				if(rawOperand1 == null || rawOperand2 == null)
					return false;

				int	operand1 = GetNumber(rawOperand1);
				int operand2 = GetNumber(rawOperand2);

				switch(compareOperation) {

					case "eq":
						return operand1 == operand2;

					case "ne":
						return operand1 != operand2;

					case "gt":
						return operand1 > operand2;

					case "ge":
						return operand1 >= operand2;

					case "le":
						return operand1 <= operand2;

					case "lt":
						return operand1 < operand2;

					default:
						throw new ArgumentException("Invalid compare operation: " + compareOperation);
				}
			}
		}

		#endregion

		#region IfBoolean

		[LayoutEvent("parse-event-script-definition", IfSender="IfBoolean")]
		private void parseIfBoolean(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeIfBoolean(e);
		}


		public class LayoutEventScriptNodeIfBoolean : LayoutEventScriptNodeIf {

			public LayoutEventScriptNodeIfBoolean(LayoutEvent e) : base(e) {
			}

			private bool getValue(object o) {
				string s = o as string;

				if(s != null)
					return bool.Parse(s);
				else if(o is bool)
					return (bool)o;

				throw new ArgumentException("Operand has invalid boolean value");
			}

			protected override bool Compare(object rawOperand1, string compareOperation, object rawOperand2) {
				if(rawOperand1 == null || rawOperand2 == null)
					return false;

				bool	operand1 = getValue(rawOperand1);
				bool	operand2 = getValue(rawOperand2);

				switch(compareOperation) {

					case "Equal":
						return operand1 == operand2;

					case "NotEqual":
						return operand1 != operand2;

					default:
						throw new ArgumentException("Invalid compare operation: " + compareOperation);
				}
			}
		}

		#endregion

		#region IfTime

		[LayoutEvent("parse-event-script-definition", IfSender="IfTime")]
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
			XmlElement	element = (XmlElement)e.Sender;
			string		constraintName = (string)e.Info;

			e.Info = LayoutEventScriptNodeIfTime.ParseTimeConstraint(element, constraintName);
		}

		[LayoutEvent("allocate-if-time-node")]
		private void allocateIfTimeNode(LayoutEvent e) {
			XmlElement	nodeElement = (XmlElement)e.Sender;

			e.Info = LayoutEventScriptNodeIfTime.AllocateTimeNode(nodeElement);
		}

		public class LayoutEventScriptNodeIfTime : LayoutEventScriptNodeCondition {
			IIfTimeNode[]		seconds;
			IIfTimeNode[]		minutes;
			IIfTimeNode[]		hours;
			IIfTimeNode[]		dayOfWeek;

			public LayoutEventScriptNodeIfTime(LayoutEvent e) : base(e) {
				seconds = ParseTimeConstraint(Element, "Seconds");
				minutes = ParseTimeConstraint(Element, "Minutes");
				hours = ParseTimeConstraint(Element, "Hours");
				dayOfWeek = ParseTimeConstraint(Element, "DayOfWeek");
			}

			public override bool IsTrue {
				get {
					DateTime	dt = (DateTime)EventManager.Event(new LayoutEvent(this, "get-current-date-time-request"));

					return checkTimeNodes(seconds, dt.Second) && checkTimeNodes(minutes, dt.Minute) && checkTimeNodes(hours, dt.Hour) &&
						checkTimeNodes(dayOfWeek, (int)dt.DayOfWeek);
				}
			}

			private bool checkTimeNodes(IIfTimeNode[] nodes, int v) {
				if(nodes.Length == 0)
					return true;

				foreach(IIfTimeNode node in nodes)
					if(node.InRange(v))
						return true;
				return false;
			}

			public static IIfTimeNode[] ParseTimeConstraint(XmlElement element, string constraintName) {
				XmlNodeList			nodeElements = element.SelectNodes(constraintName);
				IIfTimeNode[]		timeNodes = new IIfTimeNode[nodeElements.Count];

				int	i = 0;
				foreach(XmlElement nodeElement in nodeElements)
					timeNodes[i++] = AllocateTimeNode(nodeElement);

				return timeNodes;
			}

			public static IIfTimeNode AllocateTimeNode(XmlElement e) {
				if(e.Name == "DayOfWeek")
					return new IfTimeDayOfWeekNode(e);
				else
					return new IfTimeNode(e);
			}

			class IfTimeNode : IIfTimeNode {
				XmlElement		element;

				public IfTimeNode(XmlElement element) {
					this.element = element;
				}

                public XmlElement Element => element;

                public bool IsRange => element.HasAttribute("From");

                public int Value {
					get {
						return XmlConvert.ToInt32(Element.GetAttribute("Value"));
					}

					set {
						Element.SetAttribute("Value", XmlConvert.ToString(value));
					}
				}

				public int From {
					get {
						return XmlConvert.ToInt32(element.GetAttribute("From"));
					}

					set {
						element.SetAttribute("From", XmlConvert.ToString(value));
					}
				}

				public int To {
					get {
						return XmlConvert.ToInt32(element.GetAttribute("To"));
					}

					set {
						element.SetAttribute("To", XmlConvert.ToString(value));
					}
				}

				public bool InRange(int x) {
					if(IsRange)
						return From <= x && x <= To;
					else
						return x == Value;
				}

				public virtual string Description {
					get {
						if(IsRange)
							return From.ToString() + "-" + To.ToString();
						else
							return Value.ToString();
					}
				}
			}

			class IfTimeDayOfWeekNode : IfTimeNode {
				public IfTimeDayOfWeekNode(XmlElement element) : base(element) {
				}

                public string toDay(int i) => new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" }[i];

                public override string Description {
					get {
						if(IsRange)
							return toDay(From) + "-" + toDay(To);
						else
							return toDay(Value);
					}
				}
			}
		}

		#endregion

		#region IfExist

		[LayoutEvent("parse-event-script-definition", IfSender="IfDefined")]
		private void parseIfDefined(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeIfDefined(e);
		}

		public class LayoutEventScriptNodeIfDefined : LayoutEventScriptNodeCondition {
			public LayoutEventScriptNodeIfDefined(LayoutEvent e) : base(e) {
			}

			public override bool IsTrue {
				get {
					string	symbol = Element.GetAttribute("Symbol");
					object	symbolValue = Context[symbol];

					if(symbolValue == null)
						return false;

					if(Element.HasAttribute("Attribute")) {
						IObjectHasAttributes symbolWithAttributes = symbolValue as IObjectHasAttributes;

						if(symbolWithAttributes != null && symbolWithAttributes.Attributes.ContainsKey(Element.GetAttribute("Attribute")))
							return true;
						return false;
					}
					else
						return true;
				}
			}

		}

		#endregion

		#endregion

		#region Actions

		#region Actions (section)

		[LayoutEvent("parse-event-script-definition", IfSender="Actions")]
		private void parseActions(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeActions(e);
		}

		#endregion

		#region Message

		[LayoutEvent("parse-event-script-definition", IfSender="ShowMessage")]
		private void parseShowMessage(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeActionShowMessage(e);
		}

		class LayoutEventScriptNodeActionShowMessage : LayoutEventScriptNodeAction {

			public LayoutEventScriptNodeActionShowMessage(LayoutEvent e) : base(e) {
			}

			public string expandMessage(string text) {
				int		s = 0;
				int		e;
				ArrayList	parts = new ArrayList();

				while(true) {
					for(e = s; e < text.Length && text[e] != '[' && text[e] != '<'; e++)
						;

					parts.Add(text.Substring(s, e-s));

					if(e >= text.Length)
						break;
					else {		// Expand reference
						bool	expandProperty = text[e] == '[';
						char	closingDelimiter = (expandProperty ? ']' : '>');
						object	obj;

						s = e+1;
						for(e = s; s < text.Length && text[e] != closingDelimiter; e++)
							;

						if(e >= text.Length)
							throw new ArgumentException("Missing ] to terminate reference, text: " + text);

						string	reference = text.Substring(s, e-s);

						s = e+1;

						// Now expand reference
						string[]	referenceParts = reference.Split('.');

						if(referenceParts.Length != 2)
							throw new ArgumentException("Invalid symbol reference " + reference + " in text: " + text);

						if(expandProperty)
							obj = Context.GetSymbolProperty(referenceParts[0], referenceParts[1]);
						else
							obj = Context.GetSymbolAttribute(referenceParts[0], referenceParts[1]);

						if(obj != null) {
							if(obj is string)
								parts.Add(obj);
							else
								parts.Add(obj.ToString());
						}
						else
							parts.Add("*UNDEFINED*");
					}
				}

				return String.Concat((string[] )parts.ToArray(typeof(string)));
			}

			public override void Execute() {
				string				messageText = expandMessage(Element.GetAttribute("Message"));
				object				subject = Context["Train"];

				switch(Element.GetAttribute("MessageType")) {

					case "Error":
						LayoutModuleBase.Error(subject, messageText);
						break;

					default:
					case "Message":
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

		[LayoutEvent("parse-event-script-definition", IfSender="SetAttribute")]
		private void parseSetAttribute(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeActionSetAttribute(e);
		}

		class LayoutEventScriptNodeActionSetAttribute : LayoutEventScriptNodeAction {

			public LayoutEventScriptNodeActionSetAttribute(LayoutEvent e) : base(e) {
			}

			public override void Execute() {
				string	symbol = Element.GetAttribute("Symbol");
				string	attribute = Element.GetAttribute("Attribute");
				object	symbolValue = Context[symbol];

				if(symbolValue == null)
					throw new ArgumentException("Event script context does not contain object named " + symbol);

				IObjectHasAttributes symbolWithAttributes = symbolValue as IObjectHasAttributes;

				if(symbolWithAttributes == null)
					throw new ArgumentException("The object named " + symbol + " does not support attributes");

				string			setTo = Element.GetAttribute("SetTo");
				AttributesInfo	attributes = symbolWithAttributes.Attributes;

				switch(setTo) {
					case "Text":
						attributes[attribute] = Element.GetAttribute("Value");
						break;

					case "Number":
						int		number = XmlConvert.ToInt32(Element.GetAttribute("Value"));

						if(Element.GetAttribute("Op") == "Add") {
							object	oldValue = attributes[attribute];

							if(oldValue != null && oldValue is int)
								number += (int)oldValue;
						}

						attributes[attribute] = number;
						break;

					case "Boolean":
						bool	boolean = XmlConvert.ToBoolean(Element.GetAttribute("Value"));

						attributes[attribute] = boolean;
						break;

					case "ValueOf":
						object	v;

						switch(Element.GetAttribute("SymbolToAccess")) {
							case "Property":
								v = Context.GetProperty(symbol, symbolValue, Element.GetAttribute("NameTo"));
								break;

							default:
								v = Context.GetAttribute(symbol, symbolValue, Element.GetAttribute("NameTo"));
								break;
						}
									
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

		[LayoutEvent("parse-event-script-definition", IfSender="GenerateEvent")]
		private void parseGenerateEvent(LayoutEvent e) {
			e.Info = new LayoutEventScriptNodeActionGenerateEvent(e);
		}

		class LayoutEventScriptNodeActionGenerateEvent : LayoutEventScriptNodeAction {

			public LayoutEventScriptNodeActionGenerateEvent(LayoutEvent e) : base(e) {
			}

			protected object GetArgument(string name) {
				switch(Element.GetAttribute(name + "Type")) {

					case "Null":
						return null;

					case "ValueOf":
						return GetOperand(name);
						
					case "Reference":
						return Context[Element.GetAttribute(name + "SymbolName")];

					case "Context":
						return Context;

					default:
						throw new ArgumentException("Unknown argument type (" + name + "Type) = " + Element.GetAttribute(name + "Type"));
				}
			}

			public override void Execute() {
				object		sender = GetArgument("Sender");
				object		info = GetArgument("Info");
				LayoutEvent	theEvent = new LayoutEvent(sender, Element.GetAttribute("EventName"), null, info);

				XmlElement	optionsElement = Element["Options"];

				if(optionsElement != null) {
					foreach(XmlElement optionElement in optionsElement) {
						string	optionName = optionElement.GetAttribute("Name");
						object	optionValue = GetOperand(optionElement, "Option");

						if(optionValue != null)
							theEvent.SetOption(optionName, optionValue.ToString());
					}
				}

				EventManager.Event(theEvent);
			}
		}

		#endregion

	}

	#endregion
}

