using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace LayoutManager {

	/// <summary>
	/// A Layout Manager event
	/// </summary>
	public class LayoutEvent : LayoutObject {
		Object				sender;
		private String		eventName;		// Cache the event name
		bool				continueProcessing = true;
		Object				info;			// Additional information for this event
		String				ifTarget;
		Type				targetType;

		/// <summary>
		/// Construct a new event
		/// </summary>
		/// <param name="sender">The entity generating the event</param>
		/// <param name="name">The event name</param>
		public LayoutEvent(Object sender, String eventName) {
			InitializeEventObject(sender, eventName, null, null, null, null);
		}

		/// <summary>
		/// Constructs a new event with attached XML document
		/// </summary>
		/// <param name="sender">The entity sending the event</param>
		/// <param name="name">The event name (optional)</param>
		/// <param name="xmlDocument">The event inner XML document</param>
		public LayoutEvent(Object sender, String eventName, String xmlDocument) {
			InitializeEventObject(sender, eventName, xmlDocument, null, null, null);
		}

		/// <summary>
		/// Constructs a new event with optional XML document and additional information
		/// </summary>
		/// <param name="sender">The entity sending the event</param>
		/// <param name="name">The event name (optional)</param>
		/// <param name="xmlDocument">The event inner XML document (can be null)</param>
		/// <param name="info">Additional information</param>
		public LayoutEvent(Object sender, String eventName, String xmlDocument, Object info) {
			InitializeEventObject(sender, eventName, xmlDocument, info, null, null);
		}

		/// <summary>
		/// Construct a new event, with constraint on the type of object type that will get this event
		/// </summary>
		/// <param name="sender">The entity generating the event</param>
		/// <param name="name">The event name</param>
		/// <param name="targetType">The type of objects that receive this event</param>
		public LayoutEvent(Object sender, String eventName, Type targetType) {
			InitializeEventObject(sender, eventName, null, null, targetType, null);
		}

		/// <summary>
		/// Constructs a new event with attached XML document
		/// </summary>
		/// <param name="sender">The entity sending the event</param>
		/// <param name="name">The event name (optional)</param>
		/// <param name="xmlDocument">The event inner XML document</param>
		/// <param name="targetType">The type of objects that receive this event</param>
		public LayoutEvent(Object sender, String eventName, String xmlDocument, Type targetType) {
			InitializeEventObject(sender, eventName, xmlDocument, null, targetType, null);
		}

		/// <summary>
		/// Constructs a new event with optional XML document and additional information,
		/// with constraint on the type of object type that will get this event
		/// </summary>
		/// <param name="sender">The entity sending the event</param>
		/// <param name="name">The event name (optional)</param>
		/// <param name="xmlDocument">The event inner XML document (can be null)</param>
		/// <param name="info">Additional information</param>
		/// <param name="targetType">The type of objects that receive this event</param>
		public LayoutEvent(Object sender, String eventName, String xmlDocument, Object info, Type targetType) {
			InitializeEventObject(sender, eventName, xmlDocument, info, targetType, null);
		}

		/// <summary>
		/// Construct a new event, with constraint of the type that will receive the event, and with
		/// XPath expression filtering the objects that can receive the event based on their XML
		/// document
		/// </summary>
		/// <param name="sender">The entity generating the event</param>
		/// <param name="name">The event name</param>
		/// <param name="targetType">The type of objects that receive this event (can be null)</param>
		/// <param name="ifType">An XPath expression filtering the object that will receive the event based on
		/// their XML document</param>
		public LayoutEvent(Object sender, String eventName, Type targetType, Type ifType) {
			InitializeEventObject(sender, eventName, null, null, targetType, ifTarget);
		}

		/// <summary>
		/// Constructs a new event with attached XML document, with constraint of the type that will receive 
		/// the event, and with XPath expression filtering the objects that can receive the event based on
		/// their XML document
		/// </summary>
		/// <param name="sender">The entity sending the event</param>
		/// <param name="name">The event name (optional)</param>
		/// <param name="xmlDocument">The event inner XML document</param>
		/// <param name="targetType">The type of objects that receive this event</param>
		/// <param name="ifType">An XPath expression filtering the object that will receive the event based on
		/// their XML document</param>
		public LayoutEvent(Object sender, String eventName, String xmlDocument, Type targetType, String ifTarget) {
			InitializeEventObject(sender, eventName, xmlDocument, null, targetType, ifTarget);
		}

		/// <summary>
		/// Constructs a new event with optional XML document and additional information, with constraint of the type
		/// that will receive the event, and with XPath expression filtering the objects that can receive the 
		/// event based on their XML document
		/// </summary>
		/// <param name="sender">The entity sending the event</param>
		/// <param name="name">The event name (optional)</param>
		/// <param name="xmlDocument">The event inner XML document (can be null)</param>
		/// <param name="info">Additional information</param>
		/// <param name="targetType">The type of objects that receive this event</param>
		/// <param name="ifType">An XPath expression filtering the object that will receive the event based on
		/// their XML document</param>
		public LayoutEvent(Object sender, String eventName, String xmlDocument, Object info, Type targetType, String ifTarget) {
			InitializeEventObject(sender, eventName, xmlDocument, info, targetType, ifTarget);
		}

		/// <summary>
		/// New event that is based on another event, but with a different event name
		/// </summary>
		/// <param name="eventName">The new event name</param>
		/// <param name="baseEvent">The base event containing all other parameters</param>
		public LayoutEvent(string eventName, LayoutEvent baseEvent)
			: this(baseEvent.Sender, eventName, baseEvent.Element.InnerXml, baseEvent.Info) {
		}

		/// <summary>
		/// Initialize the event object
		/// </summary>
		/// <param name="sender">The sending object</param>
		/// <param name="eventName">The event's name</param>
		/// <param name="xmlDocument">Event inner XML document</param>
		/// <param name="info">Additional info</param>
		/// <param name="targetType">Limit the type of object that can receive the event</param>
		/// <param name="ifTarget">Limit the objects receiving the event based on their XML document</param>
		protected void InitializeEventObject(Object sender, String eventName, String xmlDocument, Object info, Type targetType, String ifTarget) {
			XmlInfo.XmlDocument.LoadXml("<LayoutEvent />");

			this.eventName = eventName;
			DocumentElement.SetAttribute("EventName", eventName);

			this.sender = sender;
			
			if(xmlDocument != null)
				DocumentElement.InnerXml = xmlDocument;

			this.info = info;

			IfTarget = ifTarget;
			TargetType = targetType;
		}

		/// <summary>
		/// The entity sending the event
		/// </summary>
		public Object Sender {
			get {
				return sender;
			}

			set {
				sender = value;
			}
		}

		/// <summary>
		/// Optional additional information
		/// </summary>
		public Object Info {
			get {
				return info;
			}

			set {
				info = value;
			}
		}

		/// <summary>
		/// The event name
		/// </summary>
		public String EventName {
			get {
				if(eventName == null)
					eventName = DocumentElement.GetAttribute("EventName");
				return eventName;
			}

			set {
				DocumentElement.SetAttribute("EventName", value);
				eventName = value;
			}
		}

		/// <summary>
		/// Determine if more subscriptions should be checked for applicability to this event
		/// </summary>
		public bool ContinueProcessing {
			get {
				return continueProcessing;
			}

			set {
				continueProcessing = value;
			}
		}

		/// <summary>
		/// Limit the objects that will receive the event based on their XML document
		/// </summary>
		public String IfTarget {
			get {
				return ifTarget;
			}

			set {
				ifTarget = value;
				if(ifTarget == null)
					DocumentElement.RemoveAttribute("IfTarget");
				else
					DocumentElement.SetAttribute("IfTarget", value);
			}
		}

		/// <summary>
		/// Limit the objects that will receive the event based on the receiving object type
		/// </summary>
		public Type TargetType {
			get {
				return targetType;
			}

			set {
				targetType = value;
				if(targetType == null)
					DocumentElement.RemoveAttribute("TargetType");
				else
					DocumentElement.SetAttribute("TargetType", targetType.AssemblyQualifiedName);
			}
		}

		/// <summary>
		/// Check if an XML element matches a given XPath pattern
		/// </summary>
		/// <param name="element">The element</param>
		/// <param name="xPathExpr">The XPath pattern</param>
		/// <returns></returns>
		protected bool Matches(XmlElement element, String xpathExpression) {
			try {
				return element.CreateNavigator().Matches(xpathExpression);
			} catch(XPathException ex) {
				Trace.WriteLine("XPath error: (" + ex.Message + ") " + xpathExpression + 
					" Targeted Event: " + EventName + " Sender " + Sender + " (" + Sender.GetType().FullName + ")");
				return false;
			}
		}

		/// <summary>
		/// Check if a given subscription target (the object who registered the event handler method) is
		/// applicable for this event
		/// </summary>
		/// <param name="subscription">The subscription</param>
		/// <returns>True - the subscription target should receive this event, false otherwise</returns>
		public virtual bool IsSubscriptionApplicable(LayoutEventSubscriptionBase subscription) {
			Object	target = subscription.TargetObject;

			if(TargetType != null && !TargetType.IsInstanceOfType(target))
				return false;

			IObjectHasXml	targetXml = target as IObjectHasXml;

			if(IfTarget != null && targetXml != null && !Matches(targetXml.Element, IfTarget))
				return false;

			return true;
		}

		#region Methods to get/set event parameters

		public bool HasOption(string elementName, string optionName) {
			XmlElement	optionElement = Element[elementName];

			if(optionElement != null)
				return optionElement.HasAttribute(optionName);
			return false;
		}

        public bool HasOption(string optionName) => HasOption("Options", optionName);

        public string GetOption(string optionName, string elementName = "Options", string defaultValue = null) {
			XmlElement	optionElement = Element[elementName];

			if(optionElement != null && optionElement.HasAttribute(optionName))
				return optionElement.GetAttribute(optionName);
			return defaultValue;
		}

		public LayoutEvent CopyOptions(LayoutEvent other, string elementName = "Options") {
			XmlElement otherElement = other.Element[elementName];

			if(otherElement != null)
				Element.AppendChild(Element.OwnerDocument.ImportNode(otherElement, true));

			return this;
		}

        public bool GetBoolOption(string elementName, string optionName, bool defaultValue) => XmlConvert.ToBoolean(GetOption(optionName, elementName, XmlConvert.ToString(defaultValue)));

        public bool GetBoolOption(string elementName, string optionName) => GetBoolOption(elementName, optionName, false);

        public bool GetBoolOption(string optionName) => GetBoolOption("Options", optionName);

        public bool GetBoolOption(string optionName, bool defaultValue) => GetBoolOption("Options", optionName, defaultValue);

        public int GetIntOption(string elementName, string optionName, int defaultValue) => XmlConvert.ToInt32(GetOption(optionName, elementName, XmlConvert.ToString(defaultValue)));

        public int GetIntOption(string elementName, string optionName) {
			return GetIntOption(elementName, optionName, -1);
		}

        public int GetIntOption(string optionName, int defaultValue) => GetIntOption("Options", optionName, defaultValue);

        public int GetIntOption(string optionName) => GetIntOption("Options", optionName);

        public LayoutEvent SetOption(string elementName, string optionName, string value) {
			XmlElement	optionElement = Element[elementName];

			if(optionElement == null) {
				optionElement = Element.OwnerDocument.CreateElement(elementName);
				Element.AppendChild(optionElement);
			}

			optionElement.SetAttribute(optionName, value);

			return this;
		}

        public LayoutEvent SetOption(string optionName, string value) => SetOption("Options", optionName, value);

        public LayoutEvent SetOption(string elementName, string optionName, bool value) => SetOption(elementName, optionName, XmlConvert.ToString(value));

        public LayoutEvent SetOption(string optionName, bool value) => SetOption("Options", optionName, value);

        public LayoutEvent SetOption(string elementName, string optionName, int value) => SetOption(elementName, optionName, XmlConvert.ToString(value));

        public LayoutEvent SetOption(string optionName, int value) => SetOption("Options", optionName, value);

        public LayoutEvent SetOption(string elementName, string optionName, Guid id) => SetOption(elementName, optionName, XmlConvert.ToString(id));

        public LayoutEvent SetOption(string optionName, Guid id) => SetOption("Options", optionName, id);

        #endregion

    }

	public class LayoutEvent<TSender, TInfo> : LayoutEvent {
		public LayoutEvent(string eventName, TSender sender = default(TSender), TInfo info = default(TInfo))
			: base(sender, eventName, null, info) {

		}

		public new TSender Sender {
			get {
				return (TSender)base.Sender;
			}

			set {
				base.Sender = value;
			}
		}

		public new TInfo Info {
			get {
				return (TInfo)base.Info;
			}

			set {
				base.Info = value;
			}
		}
	}

	public class LayoutEvent<TSender> : LayoutEvent<TSender, object> {
		public LayoutEvent(string eventName, TSender sender = default(TSender))
			: base(eventName, sender, default(object)) {
		}
	}

	public class LayoutEvent<TSender, TInfo, TResult> : LayoutEvent<TSender, TInfo> {
		public LayoutEvent(string eventName, TSender sender = default(TSender), TInfo info = default(TInfo))
			: base(eventName, sender, info) {
		}

		public TResult Result {
			get {
				return (TResult)((LayoutEvent)this).Info;
			}

			set {
				((LayoutEvent)this).Info = value;
			}
		}
	}

	/// <summary>
	/// A delegate (function pointer) to a method for handling a layout event
	/// </summary>
	public delegate void LayoutEventHandler(LayoutEvent e);

	/// <summary>
	/// Delegate to method for async event handlers
	/// </summary>
	/// <param name="e">The event</param>
	/// <returns>Task that will compute the final values of the event</returns>
	public delegate Task LayoutVoidAsyncEventHandler(LayoutEvent e);
	public delegate Task<object> LayoutAsyncEventHandler(LayoutEvent e);

	/// <summary>
	/// Base for subscription to receive a layout event.
	/// </summary>
	public abstract class LayoutEventSubscriptionBase : LayoutObject {
		Type				senderType;
		Type				eventType;
		Type				infoType;
		String				eventName;
		String				ifSender;
		String				ifEvent;
		String				ifInfo;
		int					order;

		/// <summary>
		/// construct a new subscription
		/// </summary>
		protected LayoutEventSubscriptionBase() {
			InitializeSubscriptionObject();
		}

		/// <summary>
		/// Constuct a new event subscription, for a given event
		/// </summary>
		/// <param name="setupString">The setup string (see Parse)</param>
		public LayoutEventSubscriptionBase(String eventName) {
			InitializeSubscriptionObject();
			EventName = eventName;
		}

		/// <summary>
		/// Construct a subscription using information extract from LayoutEvent attribute
		/// </summary>
		/// <param name="ea">The LayoutEvent attribute</param>
		public LayoutEventSubscriptionBase(LayoutEventAttributeBase ea) {
			InitializeSubscriptionObject();

			SetFromLayoutEventAttribute(ea);
		}

		public void SetFromLayoutEventAttribute(LayoutEventAttributeBase ea) {
			EventName = ea.EventName;
			SenderType = ea.SenderType;
			EventType = ea.EventType;
			InfoType = ea.InfoType;
			IfSender = ea.IfSender;
			IfEvent = ea.IfEvent;
			IfInfo = ea.IfInfo;
			Order = ea.Order;
		}

		/// <summary>
		/// Expand XPath expression so it contain values extracted from the XML document of the object that
		/// is subscribing to the event. Strings enclosed between ` (back qoute characters) are evaluated
		/// on the subscriber object XML document, and their result replaces the expression.
		/// </summary>
		/// <example>
		/// Say that you want a XPath pattern to match when the ID attribute is the same as the subscriber object
		/// ID, you may use the following XPath pattern:
		///		SenderObj[@ID='`string(@ID)`']
		///	The expression `string(@ID)` will be replaced with the current value of the ID attribute of the
		///	subscriber object.
		/// </example>
		/// <remarks>
		/// If the subscriber object XML document is changed the subscription refresh method
		/// should be called
		/// </remarks>
		/// <param name="xPath">The XPath expression to expand</param>
		/// <returns>The expanded XPath</returns>
		protected String ExpandXPath(String xpath) {
			if(xpath == null || xpath.IndexOf('`') < 0)
				return xpath;

			IObjectHasXml	subscriberXml = TargetObject as IObjectHasXml;

			if(subscriberXml == null)
				return xpath;

			XPathNavigator				xpn = subscriberXml.Element.CreateNavigator();
			System.Text.StringBuilder	result = new System.Text.StringBuilder(xpath.Length);
			int							pos = 0;

			for(int nextPos = 0; (nextPos = xpath.IndexOf('`', pos)) >= 0; pos = nextPos) {
				result.Append(xpath.Substring(pos, nextPos-pos));

				if(xpath[nextPos+1] == '`') {
					result.Append('`');		// `` is converted to a single `
					nextPos += 2;
				}
				else {
					int		s = nextPos;	// s is first ` index
					String	expandXpath = null;

					nextPos = xpath.IndexOf('`', nextPos+1);		// nextPos is the index of the terminating `
					if(nextPos < 0)
						throw new ArgumentException("XPath missing a ` character for expanded string", nameof(xpath));

					expandXpath = xpath.Substring(s+1, nextPos-s-1);

					String	expandedXpath = xpn.Evaluate(expandXpath).ToString();
					result.Append(expandedXpath);

					nextPos++;		// skip the closing `
				}
			}

			result.Append(xpath.Substring(pos, xpath.Length-pos));

			return result.ToString();
		}

		/// <summary>
		/// Re-expand the IfEvent and IfSender XPath patterns.
		/// </summary>
		/// <remarks>
		/// You should call this method if the subscriber object XML document was changed
		/// </remarks>
		public void Refresh() {
			if(DocumentElement.HasAttribute("IfSender"))
				ifSender = ExpandXPath(DocumentElement.GetAttribute("IfSender"));
			if(DocumentElement.HasAttribute("IfEvent"))
				ifEvent = ExpandXPath(DocumentElement.GetAttribute("IfEvent"));
			if(DocumentElement.HasAttribute("IfInfo"))
				ifInfo = ExpandXPath(DocumentElement.GetAttribute("IfInfo"));
		}

		/// <summary>
		/// Get/Set the event name for which this subscription is applicable.
		/// </summary>
		public String EventName {
			get {
				return eventName;
			}

			set {
				eventName = value;

				if(value == null)
					DocumentElement.RemoveAttribute("EventName");
				else
					DocumentElement.SetAttribute("EventName", value);
			}
		}

		/// <summary>
		/// Get/Set the sender entity type filter
		/// </summary>
		public Type SenderType {
			get {
				return senderType;
			}

			set {
				senderType = value;

				if(value == null)
					DocumentElement.RemoveAttribute("SenderType");
				else
					DocumentElement.SetAttribute("SenderType", value.AssemblyQualifiedName);
			}
		}

		/// <summary>
		/// Get/Set the event object type filter
		/// </summary>
		public Type EventType {
			get {
				return eventType;
			}

			set {
				eventType = value;

				if(value == null)
					DocumentElement.RemoveAttribute("EventType");
				else
					DocumentElement.SetAttribute("EventType", value.FullName);
			}
		}

		/// <summary>
		/// Get/Set the info object type filter
		/// </summary>
		public Type InfoType {
			get {
				return infoType;
			}

			set {
				infoType = value;

				if(value == null)
					DocumentElement.RemoveAttribute("InfoType");
				else
					DocumentElement.SetAttribute("InfoType", value.FullName);
			}
		}

		/// <summary>
		/// Get/Set the XPath for filtering the sender XML document
		/// </summary>
		public String IfSender {
			get {
				return ifSender;
			}

			set {
				ifSender = ExpandXPath(value);

				if(value == null)
					DocumentElement.RemoveAttribute("IfSender");
				else
					DocumentElement.SetAttribute("IfSender", value);		// Note, this is the non-expanded XPath
			}
		}

        public String NonExpandedIfSender => DocumentElement.GetAttribute("IfSender");

        /// <summary>
        /// Get/Set the XPath for filtering the event XML document
        /// </summary>
        public String IfEvent {
			get {
				return ifEvent;
			}

			set {
				ifEvent = ExpandXPath(value);

				if(value == null)
					DocumentElement.RemoveAttribute("IfEvent");
				else
					DocumentElement.SetAttribute("IfEvent", value);		// Note this is the non-expanded XPath
			}
		}

        public String NonExpandedIfEvent => DocumentElement.GetAttribute("IfEvent");

        /// <summary>
        /// Get/Set the XPath for filtering the info object
        /// </summary>
        public String IfInfo {
			get {
				return ifInfo;
			}

			set {
				ifInfo = ExpandXPath(value);

				if(value == null)
					DocumentElement.RemoveAttribute("IfInfo");
				else
					DocumentElement.SetAttribute("IfInfo", value);		// Note this is the non-expanded XPath
			}
		}

        public String NonExpandedIfInfo => DocumentElement.GetAttribute("IfInfo");

        /// <summary>
        /// Get/Set subscription processing order
        /// </summary>
        public int Order {
			get {
				return order;
			}

			set {
				order = value;
				DocumentElement.SetAttribute("Order", XmlConvert.ToString(value));
			}
		}

		/// <summary>
		/// The object that is the target of this event handler
		/// </summary>
		public abstract object TargetObject {
			get;
		}

		/// <summary>
		/// Event handler method name
		/// </summary>
		public abstract string MethodName {
			get;
		}

		/// <summary>
		/// Set the event handler method
		/// </summary>
		/// <param name="objectInstance">Event handler instance (null if this is static method)</param>
		/// <param name="method">The method</param>
		public abstract void SetMethod(object objectInstance, MethodInfo method);

		protected void InitializeSubscriptionObject() {
			XmlDocument.LoadXml("<LayoutSubscription />");
		}

		protected bool Matches(XmlElement element, String xpathExpression) {
			try {
				return element.CreateNavigator().Matches(xpathExpression);
			} catch(XPathException ex) {
				Trace.WriteLine("XPath error: (" + ex.Message + ") " + xpathExpression + " Subscription for " + MethodName);
				return false;
			}
		}

		/// <summary>
		/// Check if a given event is applicable to this subscription
		/// </summary>
		/// <param name="e">The event to be checked</param>
		/// <returns>True - this subscription should process this event</returns>
		virtual public bool IsEventApplicable(LayoutEvent e) {
			if(EventName != null && e.EventName != EventName)
				return false;

			if(SenderType != null && e.Sender != null) {
				if(e.Sender is Type) {
					if((Type)e.Sender != SenderType && !((Type)e.Sender).IsSubclassOf(SenderType))
						return false;
				}
				else {
					if(!SenderType.IsInstanceOfType(e.Sender))
						return false;
				}
			}

			if(EventType != null && EventType.IsInstanceOfType(e))
				return false;

			if(InfoType != null && e.Info != null) {
				if(e.Info is Type) {
					if((Type)e.Info == InfoType && !((Type)e.Info).IsSubclassOf(InfoType))
						return false;
				}
				else {
					if(!InfoType.IsInstanceOfType(e.Info))
						return false;
				}
			}

			if(e.Sender != null && IfSender != null) {
				XmlElement	element = null;

				if(e.Sender is IObjectHasXml)
					element = ((IObjectHasXml)e.Sender).Element;
				else if(e.Sender is XmlElement)
					element = (XmlElement)e.Sender;

				if(element != null && !Matches(element, IfSender))
						return false;
			}

			if(IfEvent != null && !Matches(e.DocumentElement, IfEvent))
				return false;

			if(IfInfo != null && e.Info != null) {
				XmlElement	element = null;

				if(e.Info is IObjectHasXml)
					element = ((IObjectHasXml)e.Info).Element;
				else if(e.Info is XmlElement)
					element = (XmlElement)e.Info;

				if(element != null && !Matches(element, IfInfo))
					return false;
			}

			return true;		// Everything matches, the event is applicable to this subscription
		}
	}

	public class LayoutEventSubscription : LayoutEventSubscriptionBase {

		public LayoutEventSubscription() : base() { }

		public LayoutEventSubscription(string eventName)
			: base(eventName) {
		}

		public LayoutEventSubscription(LayoutEventAttribute ea)
			: base(ea) {
		}

		/// <summary>
		/// Get/Set the delegate to the event handler to be called
		/// </summary>
		public LayoutEventHandler EventHandler { get; private set; }

        /// <summary>
        /// The object in which the event handler reside
        /// </summary>
        public override object TargetObject => EventHandler != null ? EventHandler.Target : null;

        /// <summary>
        /// Event handler method name
        /// </summary>
        public override string MethodName => EventHandler.Method.Name;

        public override void SetMethod(object objectInstance, MethodInfo method) {
			if(objectInstance == null)
				EventHandler = (LayoutEventHandler)Delegate.CreateDelegate(typeof(LayoutEventHandler), method);
			else
				EventHandler = (LayoutEventHandler)Delegate.CreateDelegate(typeof(LayoutEventHandler), objectInstance, method.Name);
		}
	}

	public class LayoutAsyncEventSubscription : LayoutEventSubscriptionBase {
		LayoutVoidAsyncEventHandler _voidEventHandler;
		LayoutAsyncEventHandler _eventHandler;

		public LayoutAsyncEventSubscription()
			: base() {

		}

		public LayoutAsyncEventSubscription(string eventName) : base(eventName) {
		}

		public LayoutAsyncEventSubscription(LayoutEventAttribute ea)
			: base(ea) {
		}

		public Delegate EventHandler {
			get {
				if(_voidEventHandler != null)
					return _voidEventHandler;
				else
					return _eventHandler;
			}
		}

		public Task InvokeEventHandler(LayoutEvent e) {
			if(_voidEventHandler != null)
				return _voidEventHandler(e);
			else
				return _eventHandler(e);
		}


        /// <summary>
        /// The object in which the event handler reside
        /// </summary>
        public override object TargetObject => EventHandler != null ? EventHandler.Target : null;

        /// <summary>
        /// Event handler method name
        /// </summary>
        public override string MethodName => EventHandler.Method.Name;

        public override void SetMethod(object objectInstance, MethodInfo method) {
			if(objectInstance == null) {
				_voidEventHandler = (LayoutVoidAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutVoidAsyncEventHandler), method, false);
				if(_voidEventHandler == null)
					_eventHandler = (LayoutAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutAsyncEventHandler), method);
			}
			else {
				_voidEventHandler = (LayoutVoidAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutVoidAsyncEventHandler), objectInstance, method, false);
				if(_voidEventHandler == null)
					_eventHandler = (LayoutAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutAsyncEventHandler), objectInstance, method);
			}
		}
	}

	/// <summary>
	/// Allow event subscription by annotating event handler methods.
	/// </summary>
	/// TODO: Add SubscriptionType={Type} for creating custom subscription objects
	public abstract class LayoutEventAttributeBase : System.Attribute {
		Type	subscriptionType;
		String	eventName;
		Type	senderType;
		Type	eventType;
		Type	infoType;
		String	ifSender;
		String	ifEvent;
		String	ifInfo;
		int		order;

		public LayoutEventAttributeBase() {
		}

		/// <summary>
		/// Construct an event subscription based on setup string
		/// </summary>
		/// <param name="setupString"></param>
		public LayoutEventAttributeBase(String eventName) {
			this.eventName = eventName;
		}

		/// <summary>
		/// Accept only events having this name
		/// </summary>
		public String EventName {
			get {
				return eventName;
			}

			set {
				eventName = value;
			}
		}

		/// <summary>
		/// Accept only event which are sent by this type of object (or a type derived from it)
		/// </summary>
		public Type SenderType {
			get {
				return senderType;
			}

			set {
				senderType = value;
			}
		}

		/// <summary>
		/// Accept only events whose event object is of this type (or derived from it)
		/// </summary>
		public Type EventType {
			get {
				return eventType;
			}

			set {
				eventType = value;
			}
		}

		/// <summary>
		/// Accept only events whose info object is of this type (or derived from it)
		/// </summary>
		public Type InfoType {
			get {
				return infoType;
			}

			set {
				infoType = value;
			}
		}

		/// <summary>
		/// Accept events in which the sender XML document matches this XPath expression
		/// </summary>
		public String IfSender {
			get {
				return ifSender;
			}

			set {
				ifSender = value;
			}
		}

		/// <summary>
		/// Accept events in which the event object XML document matches this XPath expression
		/// </summary>
		public String IfEvent {
			get {
				return ifEvent;
			}

			set {
				ifEvent = value;
			}
		}

		/// <summary>
		/// Accept events in which the info object XML matches this XPath expression
		/// </summary>
		public String IfInfo {
			get {
				return ifInfo;
			}

			set {
				ifInfo = value;
			}
		}

		/// <summary>
		/// Provide the processing order of this subscription. Subscriptiosn with smaller
		/// order values will be processed first
		/// </summary>
		public int Order {
			get {
				return order;
			}

			set {
				order = value;
			}
		}

		/// <summary>
		/// The type of the subscription class to create. The type must be derived from LayoutSubscription
		/// </summary>
		public Type SubscriptionType {
			get {
				return subscriptionType;
			}

			set {
				subscriptionType = value;
			}
		}

		public abstract LayoutEventSubscriptionBase CreateSubscription();
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class LayoutEventAttribute : LayoutEventAttributeBase {
		public LayoutEventAttribute()
			: base() {
		}

		public LayoutEventAttribute(string eventName)
			: base(eventName) {
		}

        /// <summary>
        /// Create the subscription initialized by this attribute
        /// </summary>
        /// <returns>The new subscription object</returns>
        public override LayoutEventSubscriptionBase CreateSubscription() => new LayoutEventSubscription();
    }

	public class LayoutAsyncEventAttribute : LayoutEventAttributeBase {
		public LayoutAsyncEventAttribute()
			: base() {
		}

		public LayoutAsyncEventAttribute(string eventName)
			: base(eventName) {
		}

        /// <summary>
        /// Create the subscription initialized by this attribute
        /// </summary>
        /// <returns>The new subscription object</returns>
        public override LayoutEventSubscriptionBase CreateSubscription() => new LayoutAsyncEventSubscription();
    }

	public enum LayoutEventRole {
		Notification,
		Request,
		AsyncRequest,
	}

	/// <summary>
	/// Use this attribute to define properties of a layout event
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly|AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=true)]
	public sealed class LayoutEventDefAttribute : System.Attribute {
		string				name;
		LayoutEventRole		role = LayoutEventRole.Notification;
		Type				senderType;
		Type				infoType;
		LayoutEventScope	scope = LayoutEventScope.MyProcess;

		public LayoutEventDefAttribute(string name) {
			this.name = name;
		}

		public LayoutEventRole Role {
			get {
				return role;
			}

			set {
				role = value;
			}
		}

		public Type SenderType {
			get {
				return senderType;
			}

			set {
				senderType = value;
			}
		}

		public Type InfoType {
			get {
				return infoType;
			}

			set {
				infoType = value;
			}
		}

        public string Name => name;

        public LayoutEventScope Scope {
			get {
				return scope;
			}

			set {
				scope = value;
			}
		}
	}

	/// <summary>
	/// Define an interface for a collection of subscription events
	/// </summary>
	public interface ILayoutSubscriptionCollection : IEnumerable<LayoutEventSubscriptionBase> {
		/// <summary>
		/// Add a subscription
		/// </summary>
		/// <param name="subscription">The subscription to add</param>
		void Add(LayoutEventSubscriptionBase subscription);

		/// <summary>
		/// Remove a subscription
		/// </summary>
		/// <param name="subscription">The subscription to be removed</param>
		void Remove(LayoutEventSubscriptionBase subscription);

		/// <summary>
		/// Remove all subscriptions whose event handlers are in a particular instance
		/// </summary>
		/// <param name="classInstance"></param>
		void RemoveObjectSubscriptions(Object instance);

		/// <summary>
		/// Add all applicable subscriptions for a given event to an array
		/// </summary>
		/// <param name="applicableSubscriptions">The array to which the subscriptions will be added</param>
		/// <param name="e">The event</param>
		void AddApplicableSubscriptions<TSubscription>(ICollection<LayoutEventSubscriptionBase> applicableSubscriptions, LayoutEvent e);
	}

	/// <summary>
	/// Where an event should be propagated
	/// </summary>
	public enum LayoutEventScope {
		/// <summary>
		/// The event should first be propagated in the model (server) and then in the clients (viewers)
		/// </summary>
		SystemWide,

		/// <summary>
		/// The event should be propagated only in the server
		/// </summary>
		Model,

		/// <summary>
		/// The event should be propagated only in the clients
		/// </summary>
		Clients,

		/// <summary>
		/// The event should be propagated in the calling process only
		/// </summary>
		MyProcess
	};

	public class LayoutDelayedEvent {
		public enum DelayedEventStatus {
			NotYetCalled,
			Called,
			Canceled,
		}

		LayoutEvent					theEvent;
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public DelayedEventStatus Status { get; private set; }
		public Guid Id { get; private set; }

		internal LayoutDelayedEvent(int delayTime, LayoutEvent theEvent) {
			this.theEvent = theEvent;

			Id = Guid.NewGuid();
			DoDelay(delayTime, cancellationTokenSource.Token);
		}

		async void DoDelay(int delayTime, CancellationToken cancellationToken) {
			try {
				var tcs = new TaskCompletionSource<object>();

				using(cancellationToken.Register(() => tcs.TrySetCanceled())) {
					EventManager.Instance.RegisterDelayedEvent(this);

					Status = DelayedEventStatus.NotYetCalled;
					await Task.WhenAny(Task.Delay(delayTime), tcs.Task);
					Status = DelayedEventStatus.Called;
					EventManager.Event(theEvent);
				}
			}
			catch(OperationCanceledException) {
				Status = DelayedEventStatus.Canceled;
			}
			finally { 
				EventManager.Instance.UnregisterDelayedEvent(this); 
			}
		}


		public void Cancel() {
			if(Status != DelayedEventStatus.Called)
				cancellationTokenSource.Cancel();
		}
	}

	/// <summary>
	/// Manage events and subscriptions
	/// </summary>
	public class LayoutEventManager {
		ILayoutSubscriptionCollection	subscriptions;
		bool							traceEvents;
		ILayoutInterThreadEventInvoker		invoker;
		Dictionary<Guid, LayoutDelayedEvent> activeDelayedEvents = new Dictionary<Guid,LayoutDelayedEvent>();
		LayoutEventDefAttribute[]			eventDefs;
		LayoutModuleManager moduleManager;

		public LayoutEventManager(LayoutModuleManager moduleManager) {
			this.subscriptions = new LayoutSubscriptionHashtableByEventName();
			this.moduleManager = moduleManager;
		}

        /// <summary>
        /// A collection of active event subscriptions
        /// </summary>
        public ILayoutSubscriptionCollection Subscriptions => subscriptions;

        /// <summary>
        /// If true, event tracing is enabled
        /// </summary>
        public bool TraceEvents {
			get {
				return traceEvents;
			}

			set {
				traceEvents = value;
			}
		}

		public ILayoutInterThreadEventInvoker InterThreadEventInvoker {
			get {
				if(invoker == null)
					invoker = (ILayoutInterThreadEventInvoker)Event(new LayoutEvent(this, "get-inter-thread-event-invoker"));
				return invoker;
			}
		}

		#region Synchronous (normal) events

		/// <summary>
		/// Internal method for generating event
		/// </summary>
		/// <param name="e">The event to generate</param>
		/// <param name="scope">The event scope</param>
		/// <param name="traceEvent">Optional trace event to be generated</param>
		protected void GenerateEvent(LayoutEvent e, LayoutEventScope scope, LayoutEventTraceEvent traceEvent) {
			List<LayoutEventSubscriptionBase>	applicableSubscriptions = new List<LayoutEventSubscriptionBase>();

			subscriptions.AddApplicableSubscriptions<LayoutEventSubscription>(applicableSubscriptions, e);
			if(applicableSubscriptions.Count > 1)
				applicableSubscriptions.Sort((s1, s2) => s1.Order - s2.Order);

			if(traceEvent != null) {
				traceEvent.ApplicableSubscriptions = applicableSubscriptions;
				traceEvent.Scope = scope;

				GenerateEvent(traceEvent, LayoutEventScope.MyProcess, null);
			}

			foreach(LayoutEventSubscription subscription in applicableSubscriptions) {
				if(subscription.EventHandler != null)
					subscription.EventHandler(e);

				if(!e.ContinueProcessing)
					break;
			}
		}

		/// <summary>
		/// Send an event
		/// </summary>
		/// <param name="e">The event to be sent</param>
		/// <param name="scope">The scope in which the event should be sent <see cref="LayoutEventScope"/></param>
		/// <returns>The event info field</returns>
		public Object Event(LayoutEvent e, LayoutEventScope scope) {
			LayoutEventTraceEvent	traceEvent = null;

			if(traceEvents)
				traceEvent = new LayoutEventTraceEvent(e, "trace-event");

			GenerateEvent(e, scope, traceEvent);
			return e.Info;
		}

        public Object Event(LayoutEvent e) => Event(e, LayoutEventScope.MyProcess);

        #endregion

        #region Async events

        /// <summary>
        /// Internal method for generating async event
        /// </summary>
        /// <param name="e">The event to generate</param>
        /// <param name="scope">The event scope</param>
        /// <param name="traceEvent">Optional trace event to be generated</param>
        protected List<Task> GenerateAsyncEvent(LayoutEvent e, LayoutEventScope scope, LayoutEventTraceEvent traceEvent) {
			var applicableSubscriptions = new List<LayoutEventSubscriptionBase>();

			subscriptions.AddApplicableSubscriptions<LayoutAsyncEventSubscription>(applicableSubscriptions, e);
			if(applicableSubscriptions.Count > 1)
				applicableSubscriptions.Sort();

			if(traceEvent != null) {
				traceEvent.ApplicableSubscriptions = applicableSubscriptions;
				traceEvent.Scope = scope;

				GenerateEvent(traceEvent, LayoutEventScope.MyProcess, null);
			}

			var eventTasks = new List<Task>(applicableSubscriptions.Count);

			foreach(LayoutAsyncEventSubscription subscription in applicableSubscriptions) {
				if(subscription.EventHandler != null)
					eventTasks.Add(subscription.InvokeEventHandler(e));

				if(!e.ContinueProcessing)
					break;
			}

			return eventTasks;
		}


		public Task AsyncEvent(LayoutEvent e, LayoutEventScope scope) {
			LayoutEventTraceEvent	traceEvent = null;

			if(traceEvents)
				traceEvent = new LayoutEventTraceEvent(e, "trace-event");

			List<Task> tasks = GenerateAsyncEvent(e, scope, traceEvent);

			if(tasks.Count == 1)
				return tasks[0];

			throw new ApplicationException("AsyncEvent " + e.EventName + " has no or more than one event handlers - (consider using AsyncEventBroadcast)");
		}

        public Task AsyncEvent(LayoutEvent e) => AsyncEvent(e, LayoutEventScope.MyProcess);

        public Task[] AsyncEventBroadcast(LayoutEvent e, LayoutEventScope scope) {
			LayoutEventTraceEvent	traceEvent = null;

			if(traceEvents)
				traceEvent = new LayoutEventTraceEvent(e, "trace-event");

			return GenerateAsyncEvent(e, scope, traceEvent).ToArray();
		}

        public Task[] AsyncEventBroadcast(LayoutEvent e) => AsyncEventBroadcast(e, LayoutEventScope.MyProcess);

        #endregion


        public LayoutDelayedEvent DelayedEvent(int delayTime, LayoutEvent e) => new LayoutDelayedEvent(delayTime, e);

        internal void RegisterDelayedEvent(LayoutDelayedEvent delayedEvent) {
			lock(activeDelayedEvents) {
				activeDelayedEvents.Add(delayedEvent.Id, delayedEvent);
			}
		}

		internal void UnregisterDelayedEvent(LayoutDelayedEvent delayedEvent) {
			lock(activeDelayedEvents) {
				activeDelayedEvents.Remove(delayedEvent.Id);
			}
		}

		/// <summary>
		/// Try to find a delayed event based on its ID
		/// </summary>
		public LayoutDelayedEvent this[Guid delayedEventID] {
			get {
				lock(activeDelayedEvents)
					return activeDelayedEvents[delayedEventID];
			}
		}

        public LayoutEventScript EventScript(string scriptName, XmlElement conditionElement, ICollection<Guid> scopeIDs, LayoutEvent scriptDoneEvent, LayoutEvent errorOccurredEvent) => new LayoutEventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, errorOccurredEvent);

        public LayoutEventScript EventScript(string scriptName, XmlElement conditionElement, ICollection<Guid> scopeIDs, LayoutEvent scriptDoneEvent) => EventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, null);

        protected void AddMethodSubscriptions<TSubscriptionAttribute, TSubscription>(ILayoutSubscriptionCollection subscriptions, object objectInstance, MethodInfo methodInfo) where TSubscriptionAttribute : LayoutEventAttributeBase where TSubscription : LayoutEventSubscriptionBase {
			var eventAttributes = (TSubscriptionAttribute[] )methodInfo.GetCustomAttributes(typeof(TSubscriptionAttribute), true);

			foreach(var eventAttribute in eventAttributes) {
				var subscription = (TSubscription)eventAttribute.CreateSubscription();

				subscription.SetMethod(methodInfo.IsStatic ? null : objectInstance, methodInfo);
				subscription.SetFromLayoutEventAttribute(eventAttribute);
				subscriptions.Add(subscription);
			}
		}

		/// <summary>
		/// Inspect the type of a given object and add subscriptions for all methods which are annotated with a
		/// LayoutEvent attribute.
		/// </summary>
		/// <param name="classInstance">The instance to be inspected for subscriptions</param>
		public void AddObjectSubscriptions(Object classInstance) {
			MethodInfo[]	methodsInfo = classInstance.GetType().GetMethods(
				BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
			
			foreach(MethodInfo methodInfo in methodsInfo) {
				AddMethodSubscriptions<LayoutEventAttribute, LayoutEventSubscription>(Subscriptions, classInstance, methodInfo);
				AddMethodSubscriptions<LayoutAsyncEventAttribute, LayoutAsyncEventSubscription>(Subscriptions, classInstance, methodInfo);
			}
		}

		/// <summary>
		/// Get all the subscriptions to event handler of a given object instance
		/// </summary>
		/// <param name="instance">The object instance</param>
		/// <returns>Array of LayoutEventSubscription to event handler of the given object</returns>
		public LayoutEventSubscriptionBase[] GetObjectSubscriptions(Object instance) {
			ArrayList	instanceSubscriptions = new ArrayList();

			foreach(LayoutEventSubscriptionBase subscription in subscriptions)
				if(subscription.TargetObject == instance)
					instanceSubscriptions.Add(subscription);

			return (LayoutEventSubscriptionBase[])instanceSubscriptions.ToArray(typeof(LayoutEventSubscriptionBase));
		}

		/// <summary>
		/// Refresh all the subscriptions of a given object. This method should be called after changing
		/// the object XML document
		/// </summary>
		/// <param name="instance"></param>
		public void RefreshObjectSubscriptions(Object instance) {
			foreach(LayoutEventSubscriptionBase subscription in GetObjectSubscriptions(instance))
				subscription.Refresh();
		}

		/// <summary>
		/// Hint the event manager that a given event should be optimized for filtering based on the
		/// sender type. Calling this method will cause the event to be tested on subscription which
		/// are found by looking in a hash table where the sender type is the key.
		/// </summary>
		/// <remarks>
		/// Do not optimize event where the sender is a subclass of the filtered sender type. For example
		/// if you expected to catch all event which are of type B then if an event is sent by a D (which
		/// is a subclass of B), it will not be "catched" if the event name is optimized for filtering by
		/// sender type.
		/// </remarks>
		/// <param name="eventName">The event name to optimize</param>
		public void OptimizeForFilteringBySenderType(String eventName) {
			((LayoutSubscriptionHashtableByEventName)subscriptions).OptimizeForFilteringBySenderType(eventName);
		}

		public LayoutEventDefAttribute[] GetEventDefinitions(LayoutEventRole requiredRole) {
			if(eventDefs == null) {
				ArrayList			eventDefsList = new ArrayList();
				LayoutModuleManager	moduleManager = (LayoutModuleManager)Event(new LayoutEvent(this, "get-module-manager"));

				foreach(LayoutAssembly layoutAssembly in moduleManager.LayoutAssemblies) {
					LayoutEventDefAttribute[]	assemblyEventDefs = (LayoutEventDefAttribute[])layoutAssembly.Assembly.GetCustomAttributes(typeof(LayoutEventDefAttribute), true);

					addEventDefs(assemblyEventDefs, requiredRole, eventDefsList);
					addEventDefs(layoutAssembly.Assembly.GetTypes(), requiredRole, eventDefsList);
				}

				eventDefs = (LayoutEventDefAttribute[] )eventDefsList.ToArray(typeof(LayoutEventDefAttribute));
			}

			return eventDefs;
		}

		public void FlushEventDefinitions() {
			eventDefs = null;
		}

		#region Helper methods for getting event attributes

		private void addEventDefs(LayoutEventDefAttribute[] eventDefsToAdd, LayoutEventRole requiredRole, ArrayList eventDefs) {
			foreach(LayoutEventDefAttribute eventDef in eventDefsToAdd) {
				if(eventDef.Role == requiredRole)
					eventDefs.Add(eventDef);
			}
		}

		private void addEventDefs(Type[] types, LayoutEventRole requiredRole, ArrayList eventDefs) {
			foreach(Type type in types) {
				if(type.IsClass || type.IsInterface) {
					LayoutEventDefAttribute[]	typeEventDefs = (LayoutEventDefAttribute[])type.GetCustomAttributes(typeof(LayoutEventDefAttribute), true);

					addEventDefs(typeEventDefs, requiredRole, eventDefs);

					foreach(MethodInfo method in type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.DeclaredOnly)) {
						LayoutEventDefAttribute[]	methodEventDefs = (LayoutEventDefAttribute[])method.GetCustomAttributes(typeof(LayoutEventDefAttribute), true);

						addEventDefs(methodEventDefs, requiredRole, eventDefs);
					}

					addEventDefs(type.GetNestedTypes(), requiredRole, eventDefs);
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// Event sent when event tracing is enabled. The sender object is the traced event
	/// </summary>
	public class LayoutEventTraceEvent : LayoutEvent {
		public LayoutEventTraceEvent(LayoutEvent theEvent, String traceEventName) : base(theEvent, traceEventName) {
		}

		/// <summary>
		/// The subscriptions which are applicable for the traced event
		/// </summary>
		ICollection<LayoutEventSubscriptionBase>	_applicableSubscriptions;

		/// <summary>
		/// The scope in which the trace event was sent
		/// </summary>
		LayoutEventScope			_scope;

		public ICollection<LayoutEventSubscriptionBase> ApplicableSubscriptions {
			get {
				return _applicableSubscriptions;
			}

			set {
				_applicableSubscriptions = value;
			}
		}

		public LayoutEventScope Scope {
			get {
				return _scope;
			}

			set {
				_scope = value;
			}
		}
	}

	public static class EventManager {
		static LayoutEventManager eventManager;

		public static LayoutEventManager Instance {
			get {
				return eventManager;
			}

			set {
				eventManager = value;
			}
		}

        /// <summary>
        /// Send event to subscribers
        /// </summary>
        /// <param name="e">The event to be sent</param>
        /// <returns>The value of the event class Info field</returns>
        public static object Event(LayoutEvent e) => Instance.Event(e);

        /// <summary>
        /// Invoke asynchronous event
        /// </summary>
        /// <param name="e">The event</param>
        /// <returns>A task representing the ongoing event</returns>
        /// <remarks>Call this method if there is one and only one event handler</remarks>
        public static Task AsyncEvent(LayoutEvent e) => Instance.AsyncEvent(e);

        /// <summary>
        /// Invoke multiple asynchrouns event
        /// </summary>
        /// <param name="e">The event</param>
        /// <returns>An array of tasks representing the ongoing events</returns>
        public static Task[] AsyncEventBroadcast(LayoutEvent e) => Instance.AsyncEventBroadcast(e);

        /// <summary>
        /// Send event to subscribers after waiting some time
        /// </summary>
        /// <param name="delayTime">The time to wait</param>
        /// <param name="e">The event to send</param>
        /// <returns>Delay event object</returns>
        public static LayoutDelayedEvent DelayedEvent(int delayTime, LayoutEvent e) => Instance.DelayedEvent(delayTime, e);

        /// <summary>
        /// Hook up event handlers annotated with the LayoutEvent attribute in a given object instance
        /// </summary>
        /// <param name="classInstance">The object instance</param>
        public static void AddObjectSubscriptions(Object classInstance) {
			Instance.AddObjectSubscriptions(classInstance);
		}

        /// <summary>
        /// Return a collection of all events subscription.
        /// </summary>
        public static ILayoutSubscriptionCollection Subscriptions => Instance.Subscriptions;

        public static LayoutEventScript EventScript(string scriptName, XmlElement conditionElement, ICollection<Guid> scopeIDs, LayoutEvent scriptDoneEvent, LayoutEvent errorOccurredEvent) => new LayoutEventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, errorOccurredEvent);

        public static LayoutEventScript EventScript(string scriptName, XmlElement conditionElement, ICollection<Guid> scopeIDs, LayoutEvent scriptDoneEvent) => EventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, null);

        public static object Event(string eventName) => Instance.Event(new LayoutEvent(null, eventName));

        public static object Event(object sender, string eventName) => Instance.Event(new LayoutEvent(sender, eventName));

        public static object Event(string eventName, object info) => Instance.Event(new LayoutEvent(null, eventName, null, info));

        public static object Event(object sender, string eventName, object info) => Instance.Event(new LayoutEvent(sender, eventName, null, info));
    }

	/// <summary>
	/// A subscription collection implemented as an array. <see cref="ILayoutSubscriptionCollection"/>
	/// </summary>
	class LayoutSubscriptionArray : ILayoutSubscriptionCollection {
		List<LayoutEventSubscriptionBase>		subscriptions = new List<LayoutEventSubscriptionBase>();

		public void Add(LayoutEventSubscriptionBase subscription) {
			subscriptions.Add(subscription);
			// TODO: Generate an event that a subscription was added
		}

		public void Remove(LayoutEventSubscriptionBase subscription) {
			subscriptions.Remove(subscription);
			// TODO: Generate an event that a subscription was removed
		}

		public void RemoveObjectSubscriptions(Object classInstance) {
			List<LayoutEventSubscriptionBase>	subscriptionsToRemove = new List<LayoutEventSubscriptionBase>();

			foreach(LayoutEventSubscriptionBase subscription in subscriptions)
				if(subscription.TargetObject == classInstance)
					subscriptionsToRemove.Add(subscription);

			foreach(LayoutEventSubscriptionBase subscription in subscriptionsToRemove)
					Remove(subscription);
		}

		public void AddApplicableSubscriptions<TSubscription>(ICollection<LayoutEventSubscriptionBase> applicableSubscriptions, LayoutEvent e) {
			foreach(LayoutEventSubscriptionBase subscription in subscriptions)
				if(e.IsSubscriptionApplicable(subscription) && subscription.IsEventApplicable(e) && subscription is TSubscription)
					applicableSubscriptions.Add(subscription);
		}

        public IEnumerator<LayoutEventSubscriptionBase> GetEnumerator() => subscriptions.GetEnumerator();

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

	/// <summary>
	/// A subscription collection implemented as hash table for subscriptions which filter on a particular
	/// event name, all other subscriptions are stored in a array based subscription collection.
	/// </summary>
	class LayoutSubscriptionHashtableByEventName : ILayoutSubscriptionCollection {
		Dictionary<string, ILayoutSubscriptionCollection> subscriptionByEventName = new Dictionary<string, ILayoutSubscriptionCollection>();
		LayoutSubscriptionArray noEventNameFilterSubscriptions = new LayoutSubscriptionArray();

		private ILayoutSubscriptionCollection getSlot(LayoutEventSubscriptionBase subscription) {
			if(!string.IsNullOrEmpty(subscription.EventName)) {
				ILayoutSubscriptionCollection hashEntry;

				if(!subscriptionByEventName.TryGetValue(subscription.EventName, out hashEntry)) {
					hashEntry = new LayoutSubscriptionArray();
					subscriptionByEventName[subscription.EventName] = hashEntry;
				}

				return hashEntry;
			}
			else
				return noEventNameFilterSubscriptions;
		}

		public void Add(LayoutEventSubscriptionBase subscription) {
			getSlot(subscription).Add(subscription);
		}

		public void Remove(LayoutEventSubscriptionBase subscription) {
			getSlot(subscription).Remove(subscription);
		}

		public void RemoveObjectSubscriptions(Object classInstance) {
			foreach(ILayoutSubscriptionCollection hashEntry in subscriptionByEventName.Values)
				hashEntry.RemoveObjectSubscriptions(classInstance);
			noEventNameFilterSubscriptions.RemoveObjectSubscriptions(classInstance);
		}

		public void AddApplicableSubscriptions<TSubscription>(ICollection<LayoutEventSubscriptionBase> applicableSubscriptions, LayoutEvent e) {
			ILayoutSubscriptionCollection hashEntry;

			if(subscriptionByEventName.TryGetValue(e.EventName, out hashEntry))
				hashEntry.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);

			noEventNameFilterSubscriptions.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);
		}

		public IEnumerator<LayoutEventSubscriptionBase> GetEnumerator() {
			List<LayoutEventSubscriptionBase> allSubscriptions = new List<LayoutEventSubscriptionBase>();

			foreach(ILayoutSubscriptionCollection hashEntry in subscriptionByEventName.Values) {
				foreach(LayoutEventSubscriptionBase subscription in hashEntry)
					allSubscriptions.Add(subscription);
			}

			foreach(LayoutEventSubscriptionBase subscription in noEventNameFilterSubscriptions)
				allSubscriptions.Add(subscription);

			return allSubscriptions.GetEnumerator();
		}

		public void OptimizeForFilteringBySenderType(String eventName) {
			ILayoutSubscriptionCollection hashEntry = (ILayoutSubscriptionCollection)subscriptionByEventName[eventName];
			ILayoutSubscriptionCollection newEntry = new LayoutSubscriptionHashtableBySenderType();

			subscriptionByEventName.Remove(eventName);
			subscriptionByEventName[eventName] = newEntry;

			if(hashEntry != null) {
				foreach(LayoutEventSubscriptionBase subscription in hashEntry)
					newEntry.Add(subscription);
			}
		}

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

	/// <summary>
	/// A subscription collection implemented as hash table for subscriptions which filter sender type,
	/// all other subscriptions are stored in a array based subscription collection.
	/// </summary>
	class LayoutSubscriptionHashtableBySenderType : ILayoutSubscriptionCollection {
		Dictionary<Type, ILayoutSubscriptionCollection> subscriptionBySenderType = new Dictionary<Type,ILayoutSubscriptionCollection>();
		LayoutSubscriptionArray		noSenderTypeSubscriptions = new LayoutSubscriptionArray();

		private ILayoutSubscriptionCollection getSlot(LayoutEventSubscriptionBase subscription) {
			if(subscription.SenderType != null) {
				ILayoutSubscriptionCollection hashEntry;

				if(!subscriptionBySenderType.TryGetValue(subscription.SenderType, out hashEntry)) {
					hashEntry = new LayoutSubscriptionArray();
					subscriptionBySenderType[subscription.SenderType] = hashEntry;
				}

				return hashEntry;
			}
			else
				return noSenderTypeSubscriptions;
		}

		public void Add(LayoutEventSubscriptionBase subscription) {
			getSlot(subscription).Add(subscription);
		}

		public void Remove(LayoutEventSubscriptionBase subscription) {
			getSlot(subscription).Remove(subscription);
		}

		public void RemoveObjectSubscriptions(Object classInstance) {
			foreach(ILayoutSubscriptionCollection hashEntry in subscriptionBySenderType.Values)
				hashEntry.RemoveObjectSubscriptions(classInstance);
			noSenderTypeSubscriptions.RemoveObjectSubscriptions(classInstance);
		}

		public void AddApplicableSubscriptions<TSubscription>(ICollection<LayoutEventSubscriptionBase> applicableSubscriptions, LayoutEvent e) {
			ILayoutSubscriptionCollection hashEntry = (ILayoutSubscriptionCollection)subscriptionBySenderType[e.Sender.GetType()];

			if(subscriptionBySenderType.TryGetValue(e.Sender.GetType(), out hashEntry))
				hashEntry.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);

			noSenderTypeSubscriptions.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);
		}

		public IEnumerator<LayoutEventSubscriptionBase> GetEnumerator() {
			List<LayoutEventSubscriptionBase>	allSubscriptions = new List<LayoutEventSubscriptionBase>();

			foreach(ILayoutSubscriptionCollection hashEntry in subscriptionBySenderType.Values) {
				foreach(LayoutEventSubscriptionBase subscription in hashEntry)
					allSubscriptions.Add(subscription);
			}

			foreach(LayoutEventSubscriptionBase subscription in noSenderTypeSubscriptions)
				allSubscriptions.Add(subscription);

			return allSubscriptions.GetEnumerator();
		}

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

    }

	/// <summary>
	/// Interface for defining methods to invoke events in the context of a given thread.
	/// </summary>
	public interface ILayoutInterThreadEventInvoker {
		/// <summary>
		/// Queue an event to be delivered in the context of the main thread (UI thread)
		/// </summary>
		/// <param name="e">The event</param>
		/// <remarks>
		/// Use this method when you need to invoke an event from thread different than the
		/// main thread. For example upon completing asynchronous I/O operation
		/// </remarks>
		void QueueEvent(LayoutEvent e);
	}
};

