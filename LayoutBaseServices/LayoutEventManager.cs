using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

// Reenable warning when switch to .NET 4.8 and range c#8 feature is supported
#pragma warning disable IDE0057

#nullable enable
namespace LayoutManager {
    /// <summary>
    /// A Layout Manager event
    /// </summary>
    public class LayoutEvent : LayoutObject {
        private string eventName;       // Cache the event name
        private string? ifTarget;
        private Type? targetType;

        /// <summary>
        /// Constructs a new event with optional XML document and additional information, with constraint of the type
        /// that will receive the event, and with XPath expression filtering the objects that can receive the 
        /// event based on their XML document
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="sender">The entity sending the event (optional)</param>
        /// <param name="info">Additional information (optional)</param>
        /// <param name="xmlDocument">The event inner XML document (can be null)</param>
        /// <param name="targetType">The type of objects that receive this event (optional)</param>
        /// <param name="ifType">An XPath expression filtering the object that will receive the event based on
        /// their XML document</param>
        public LayoutEvent(string eventName, object? sender = null, object? info = null, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null) {
            XmlInfo.XmlDocument.LoadXml("<LayoutEvent />");

            this.eventName = eventName;
            DocumentElement.SetAttribute("EventName", eventName);

            this.Sender = sender;
            this.ifTarget = ifTarget;
            this.Info = info;

            if (xmlDocument != null)
                DocumentElement.InnerXml = xmlDocument;

            this.targetType = targetType;
            this.ifTarget = ifTarget;
        }

        /// <summary>
        /// New event that is based on another event, but with a different event name
        /// </summary>
        /// <param name="eventName">The new event name</param>
        /// <param name="baseEvent">The base event containing all other parameters</param>
        public LayoutEvent(string eventName, LayoutEvent baseEvent)
            : this(eventName, baseEvent.Sender, baseEvent.Info, baseEvent.Element.InnerXml) {
        }

        /// <summary>
        /// The entity sending the event
        /// </summary>
        public object? Sender { get; set; }

        /// <summary>
        /// Optional additional information
        /// </summary>
        public object? Info { get; set; }

        /// <summary>
        /// The event name
        /// </summary>
        public string EventName => eventName ?? (eventName = DocumentElement.GetAttribute("EventName"));

        /// <summary>
        /// Determine if more subscriptions should be checked for applicability to this event
        /// </summary>
        public bool ContinueProcessing { get; set; } = true;

        /// <summary>
        /// Limit the objects that will receive the event based on their XML document
        /// </summary>
        public string? IfTarget {
            get {
                return ifTarget;
            }

            set {
                ifTarget = value;
                if (ifTarget == null)
                    DocumentElement.RemoveAttribute("IfTarget");
                else
                    DocumentElement.SetAttribute("IfTarget", value);
            }
        }

        /// <summary>
        /// Limit the objects that will receive the event based on the receiving object type
        /// </summary>
        public Type? TargetType {
            get {
                return targetType;
            }

            set {
                targetType = value;
                if (targetType == null)
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
        protected bool Matches(XmlElement element, string xpathExpression) {
            try {
                return element.CreateNavigator().Matches(xpathExpression);
            }
            catch (XPathException ex) {
                Trace.WriteLine($"XPath error: ({ex.Message}) {xpathExpression} Targeted Event: {EventName} Sender {(Sender != null ? $"{Sender} ({Sender.GetType().FullName})" : "(Null)")}");

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
            object? target = subscription.TargetObject;

            if (target == null || (TargetType != null && !TargetType.IsInstanceOfType(target)))
                return false;

            return IfTarget == null || !(target is IObjectHasXml targetXml) || Matches(targetXml.Element, IfTarget);
        }
    }

    public static class LayoutEventExtensions {
        private const string DefaultOptionsElementName = "Options";
        #region Methods to get/set event parameters

        public static bool HasOption<TEvent>(this TEvent e, string elementName, string optionName) where TEvent : LayoutEvent {
            XmlElement optionElement = e.Element[elementName];

            return optionElement?.HasAttribute(optionName) ?? false;
        }

        public static bool HasOption<TEvent>(this TEvent e, string optionName) where TEvent : LayoutEvent => e.HasOption(DefaultOptionsElementName, optionName);

        public static ConvertableString GetOption<TEvent>(this TEvent e, string optionName, string elementName = DefaultOptionsElementName) where TEvent : LayoutEvent {
            var optionElement = e.Element[elementName];

            return new XmlElementWrapper(optionElement).AttributeValue(optionName);
        }

        public static TEvent CopyOptions<TEvent>(this TEvent e, LayoutEvent other, string elementName = DefaultOptionsElementName) where TEvent : LayoutEvent {
            XmlElement otherElement = other.Element[elementName];

            if (otherElement != null)
                e.Element.AppendChild(e.Element.OwnerDocument.ImportNode(otherElement, true));

            return e;
        }

        public static TEvent SetOption<TEvent>(this TEvent e, string elementName, string optionName, string value) where TEvent : LayoutEvent {
            XmlElement optionElement = e.Element[elementName];

            if (optionElement == null) {
                optionElement = e.Element.OwnerDocument.CreateElement(elementName);
                e.Element.AppendChild(optionElement);
            }

            optionElement.SetAttribute(optionName, value);

            return e;
        }

        public static TEvent SetOption<TEvent>(this TEvent e, string optionName, string value) where TEvent : LayoutEvent => e.SetOption(DefaultOptionsElementName, optionName, value);

        public static TEvent SetOption<TEvent>(this TEvent e, string elementName, string optionName, bool value) where TEvent : LayoutEvent => e.SetOption(elementName, optionName, XmlConvert.ToString(value));

        public static TEvent SetOption<TEvent>(this TEvent e, string optionName, bool value) where TEvent : LayoutEvent => e.SetOption(DefaultOptionsElementName, optionName, value);

        public static TEvent SetOption<TEvent>(this TEvent e, string elementName, string optionName, int value) where TEvent : LayoutEvent => e.SetOption(elementName, optionName, XmlConvert.ToString(value));

        public static TEvent SetOption<TEvent>(this TEvent e, string optionName, int value) where TEvent : LayoutEvent => e.SetOption(DefaultOptionsElementName, optionName, value);

        public static TEvent SetOption<TEvent>(this TEvent e, string elementName, string optionName, Guid id) where TEvent : LayoutEvent => e.SetOption(elementName, optionName, XmlConvert.ToString(id));

        public static TEvent SetOption<TEvent>(this TEvent e, string optionName, Guid id) where TEvent : LayoutEvent => e.SetOption(DefaultOptionsElementName, optionName, id);

        public static TEvent SetOption<TEvent>(this TEvent e, string elementName, string optionName, Enum v) where TEvent : LayoutEvent => e.SetOption(elementName, optionName, v.ToString());

        public static TEvent SetOption<TEvent>(this TEvent e, string optionName, Enum v) where TEvent : LayoutEvent => e.SetOption(DefaultOptionsElementName, optionName, v.ToString());

        #endregion
    }

    public class LayoutEvent<TSender, TInfo> : LayoutEvent where TSender : class where TInfo : class {
        public LayoutEvent(string eventName, TSender? sender, TInfo? info = default, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null)
            : base(eventName, sender, info, xmlDocument, targetType, ifTarget) {
        }

        public new TSender? Sender {
            get {
                return base.Sender as TSender;
            }
        }

        public new TInfo? Info {
            get {
                return (TInfo?)base.Info;
            }

            set {
                base.Info = value;
            }
        }
    }

    public class LayoutEvent<TSender> : LayoutEvent<TSender, object> where TSender : class {
        public LayoutEvent(string eventName, TSender sender, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null)
            : base(eventName, sender, default, xmlDocument, targetType, ifTarget) {
        }
    }

    public class LayoutEventInfoValueType<TSender, TInfo> : LayoutEvent where TSender : class where TInfo : struct {
        public LayoutEventInfoValueType(string eventName, TSender? sender, TInfo info, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null) :
            base(eventName, sender, info, xmlDocument, targetType, ifTarget) {
        }

        public new TSender? Sender {
            get {
                return base.Sender as TSender;
            }
        }

        public new TInfo Info {
            get => (TInfo?)(base.Info) ?? default;

            set {
                base.Info = value;
            }
        }
    }

    public class LayoutEvent<TSender, TInfo, TResult> : LayoutEvent<TSender, TInfo> where TSender : class where TInfo : class where TResult : class {
        public LayoutEvent(string eventName, TSender? sender, TInfo? info = default, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null)
            : base(eventName, sender, info, xmlDocument, targetType, ifTarget) {
        }

        public TResult? Result {
            get {
                return (TResult?)((LayoutEvent)this).Info;
            }

            set {
                ((LayoutEvent)this).Info = value;
            }
        }
    }

    public class LayoutEventResultValueType<TSender, TInfo, TResult> : LayoutEvent<TSender, TInfo> where TSender : class where TInfo : class where TResult : struct {
        public LayoutEventResultValueType(string eventName, TSender? sender = null, TInfo? info = null, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null)
            : base(eventName, sender, info, xmlDocument, targetType, ifTarget) {
        }

        public TResult? Result {
            get {
                return (TResult?)((LayoutEvent)this).Info;
            }

            set {
                ((LayoutEvent)this).Info = value;
            }
        }
    }

    public class LayoutEventInfoResultValueType<TSender, TInfo, TResult> : LayoutEventInfoValueType<TSender, TInfo> where TSender : class where TInfo : struct where TResult : struct {
        public LayoutEventInfoResultValueType(string eventName, TSender sender, TInfo info, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null)
            : base(eventName, sender, info, xmlDocument, targetType, ifTarget) {
        }

        public TResult? Result {
            get {
                return (TResult?)((LayoutEvent)this).Info;
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

    public enum LayoutEventRole {
        Unspecified,
        Notification,
        Request,
        AsyncRequest,
    }

    /// <summary>
    /// Base for subscription to receive a layout event.
    /// </summary>
    public abstract class LayoutEventSubscriptionBase : LayoutObject {
        private const string A_Order = "Order";
        private const string A_Role = "Role";
        private LayoutEventRole role = LayoutEventRole.Unspecified;
        private Type? senderType;
        private Type? eventType;
        private Type? infoType;
        private string? ifSender;
        private string? ifEvent;
        private string? ifInfo;
        private int order;

#if NOTDEF
        /// <summary>
        /// construct a new subscription
        /// </summary>
        protected LayoutEventSubscriptionBase() {
        }
#endif

        /// <summary>
        /// Constuct a new event subscription, for a given event
        /// </summary>
        /// <param name="setupString">The setup string (see Parse)</param>
        protected LayoutEventSubscriptionBase(string eventName) {
            XmlDocument.LoadXml("<LayoutSubscription />");
            this.EventName = eventName;
            DocumentElement.SetAttribute("EventName", eventName);
        }

        /// <summary>
        /// Construct a subscription using information extract from LayoutEvent attribute
        /// </summary>
        /// <param name="ea">The LayoutEvent attribute</param>
        protected LayoutEventSubscriptionBase(LayoutEventAttributeBase ea) : this(ea.EventName) {
            SetFromLayoutEventAttribute(ea);
        }

        public void SetFromLayoutEventAttribute(LayoutEventAttributeBase ea) {
            Role = ea.Role;
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
        protected string? ExpandXPath(string? xpath) {
            if (xpath == null || xpath.IndexOf('`') < 0)
                return xpath;

            if (!(TargetObject is IObjectHasXml subscriberXml))
                return xpath;

            XPathNavigator xpn = subscriberXml.Element.CreateNavigator();
            System.Text.StringBuilder result = new System.Text.StringBuilder(xpath.Length);
            int pos = 0;

            for (int nextPos; (nextPos = xpath.IndexOf('`', pos)) >= 0; pos = nextPos) {
                result.Append(xpath, pos, nextPos - pos);

                if (xpath[nextPos + 1] == '`') {
                    result.Append('`');     // `` is converted to a single `
                    nextPos += 2;
                }
                else {
                    int s = nextPos;    // s is first ` index
                    string expandXpath;

                    nextPos = xpath.IndexOf('`', nextPos + 1);      // nextPos is the index of the terminating `
                    if (nextPos < 0)
                        throw new ArgumentException("XPath missing a ` character for expanded string", nameof(xpath));

                    expandXpath = xpath.Substring(s + 1, nextPos - s - 1);

                    string expandedXpath = xpn.Evaluate(expandXpath).ToString();
                    result.Append(expandedXpath);

                    nextPos++;      // skip the closing `
                }
            }

            result.Append(xpath, pos, xpath.Length - pos);

            return result.ToString();
        }

        /// <summary>
        /// Re-expand the IfEvent and IfSender XPath patterns.
        /// </summary>
        /// <remarks>
        /// You should call this method if the subscriber object XML document was changed
        /// </remarks>
        public void Refresh() {
            if (DocumentElement.HasAttribute("IfSender"))
                ifSender = ExpandXPath(DocumentElement.GetAttribute("IfSender"));
            if (DocumentElement.HasAttribute("IfEvent"))
                ifEvent = ExpandXPath(DocumentElement.GetAttribute("IfEvent"));
            if (DocumentElement.HasAttribute("IfInfo"))
                ifInfo = ExpandXPath(DocumentElement.GetAttribute("IfInfo"));
        }

        /// <summary>
        /// Get/Set the event name for which this subscription is applicable.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Set/Get event role (is it a request or notification)
        /// </summary>
        public LayoutEventRole Role {
            get => role;

            set {
                role = value;

                DocumentElement.SetAttribute(A_Role, value, removeIf: LayoutEventRole.Unspecified);
            }
        }

        /// <summary>
        /// Get/Set the sender entity type filter
        /// </summary>
        public Type? SenderType {
            get {
                return senderType;
            }

            set {
                senderType = value;

                if (value == null)
                    DocumentElement.RemoveAttribute("SenderType");
                else
                    DocumentElement.SetAttribute("SenderType", value.AssemblyQualifiedName);
            }
        }

        /// <summary>
        /// Get/Set the event object type filter
        /// </summary>
        public Type? EventType {
            get {
                return eventType;
            }

            set {
                eventType = value;

                if (value == null)
                    DocumentElement.RemoveAttribute("EventType");
                else
                    DocumentElement.SetAttribute("EventType", value.FullName);
            }
        }

        /// <summary>
        /// Get/Set the info object type filter
        /// </summary>
        public Type? InfoType {
            get => infoType;

            set {
                infoType = value;

                if (value == null)
                    DocumentElement.RemoveAttribute("InfoType");
                else
                    DocumentElement.SetAttribute("InfoType", value.FullName);
            }
        }

        /// <summary>
        /// Get/Set the XPath for filtering the sender XML document
        /// </summary>
        public string? IfSender {
            get => ifSender;

            set {
                ifSender = ExpandXPath(value);

                if (value == null)
                    DocumentElement.RemoveAttribute("IfSender");
                else
                    DocumentElement.SetAttribute("IfSender", value);        // Note, this is the non-expanded XPath
            }
        }

        public string NonExpandedIfSender => DocumentElement.GetAttribute("IfSender");

        /// <summary>
        /// Get/Set the XPath for filtering the event XML document
        /// </summary>
        public string? IfEvent {
            get {
                return ifEvent;
            }

            set {
                ifEvent = ExpandXPath(value);

                if (value == null)
                    DocumentElement.RemoveAttribute("IfEvent");
                else
                    DocumentElement.SetAttribute("IfEvent", value);     // Note this is the non-expanded XPath
            }
        }

        public string NonExpandedIfEvent => DocumentElement.GetAttribute("IfEvent");

        /// <summary>
        /// Get/Set the XPath for filtering the info object
        /// </summary>
        public string? IfInfo {
            get {
                return ifInfo;
            }

            set {
                ifInfo = ExpandXPath(value);

                if (value == null)
                    DocumentElement.RemoveAttribute("IfInfo");
                else
                    DocumentElement.SetAttribute("IfInfo", value);      // Note this is the non-expanded XPath
            }
        }

        public string NonExpandedIfInfo => DocumentElement.GetAttribute("IfInfo");

        /// <summary>
        /// Get/Set subscription processing order
        /// </summary>
        public int Order {
            get {
                return order;
            }

            set {
                order = value;
                DocumentElement.SetAttribute(A_Order, value);
            }
        }

        /// <summary>
        /// The object that is the target of this event handler
        /// </summary>
        public abstract object? TargetObject {
            get;
        }

        /// <summary>
        /// Event handler method name
        /// </summary>
        public abstract string? MethodName {
            get;
        }

        /// <summary>
        /// Set the event handler method
        /// </summary>
        /// <param name="objectInstance">Event handler instance (null if this is static method)</param>
        /// <param name="method">The method</param>
        public abstract void SetMethod(object? objectInstance, MethodInfo method);

        protected void InitializeSubscriptionObject() {
            XmlDocument.LoadXml("<LayoutSubscription />");
        }

        protected bool Matches(XmlElement element, string xpathExpression) {
            try {
                return element.CreateNavigator().Matches(xpathExpression);
            }
            catch (XPathException ex) {
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
            if (EventName != null && e.EventName != EventName)
                return false;

            if (SenderType != null && e.Sender != null) {
                if (e.Sender is Type) {
                    if ((Type)e.Sender != SenderType && !((Type)e.Sender).IsSubclassOf(SenderType))
                        return false;
                }
                else {
                    if (!SenderType.IsInstanceOfType(e.Sender))
                        return false;
                }
            }

            if (EventType != null && EventType.IsInstanceOfType(e))
                return false;

            if (InfoType != null && e.Info != null) {
                if (e.Info is Type) {
                    if ((Type)e.Info == InfoType && !((Type)e.Info).IsSubclassOf(InfoType))
                        return false;
                }
                else {
                    if (!InfoType.IsInstanceOfType(e.Info))
                        return false;
                }
            }

            if (e.Sender != null && IfSender != null) {
                XmlElement? element = null;

                if (e.Sender is IObjectHasXml)
                    element = ((IObjectHasXml)e.Sender).Element;
                else if (e.Sender is XmlElement)
                    element = (XmlElement)e.Sender;

                if (element != null && !Matches(element, IfSender))
                    return false;
            }

            if (IfEvent != null && !Matches(e.DocumentElement, IfEvent))
                return false;

            if (IfInfo != null && e.Info != null) {
                XmlElement? element = null;

                if (e.Info is IObjectHasXml)
                    element = ((IObjectHasXml)e.Info).Element;
                else if (e.Info is XmlElement)
                    element = (XmlElement)e.Info;

                if (element != null && !Matches(element, IfInfo))
                    return false;
            }

            return true;        // Everything matches, the event is applicable to this subscription
        }
    }

    public class LayoutEventSubscription : LayoutEventSubscriptionBase {
        //		public LayoutEventSubscription() : base() { }

        public LayoutEventSubscription(string eventName)
            : base(eventName) {
        }

        public LayoutEventSubscription(LayoutEventAttribute ea)
            : base(ea) {
        }

        /// <summary>
        /// Get/Set the delegate to the event handler to be called
        /// </summary>
        public LayoutEventHandler? EventHandler { get; private set; }

        /// <summary>
        /// The object in which the event handler reside
        /// </summary>
        public override object? TargetObject => EventHandler?.Target;

        /// <summary>
        /// Event handler method name
        /// </summary>
        public override string? MethodName => EventHandler?.Method.Name;

        public override void SetMethod(object? objectInstance, MethodInfo method) {
            if (objectInstance == null)
                EventHandler = (LayoutEventHandler)Delegate.CreateDelegate(typeof(LayoutEventHandler), method);
            else
                EventHandler = (LayoutEventHandler)Delegate.CreateDelegate(typeof(LayoutEventHandler), objectInstance, method.Name);
        }
    }

    public class LayoutAsyncEventSubscription : LayoutEventSubscriptionBase {
        private LayoutVoidAsyncEventHandler? _voidEventHandler;
        private LayoutAsyncEventHandler? _eventHandler;

#if NOT
        public LayoutAsyncEventSubscription()
			: base() {
		}
#endif

        public LayoutAsyncEventSubscription(string eventName) : base(eventName) {
        }

        public LayoutAsyncEventSubscription(LayoutEventAttribute ea)
            : base(ea) {
        }

        public Delegate EventHandler {
            get {
                if (_voidEventHandler != null)
                    return _voidEventHandler;
                else if (_eventHandler != null)
                    return _eventHandler;

                throw new ApplicationException("EventHandler and voidEventHandlers are null");
            }
        }

        public Task InvokeEventHandler(LayoutEvent e) {
            if (_voidEventHandler != null)
                return _voidEventHandler(e);
            else if (_eventHandler != null)
                return _eventHandler(e);

            throw new ApplicationException("EventHandler and voidEventHandlers are null");
        }

        /// <summary>
        /// The object in which the event handler reside
        /// </summary>
        public override object? TargetObject => EventHandler?.Target;

        /// <summary>
        /// Event handler method name
        /// </summary>
        public override string? MethodName => EventHandler?.Method.Name;

        public override void SetMethod(object? objectInstance, MethodInfo method) {
            if (objectInstance == null) {
                _voidEventHandler = (LayoutVoidAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutVoidAsyncEventHandler), method, false);
                if (_voidEventHandler == null)
                    _eventHandler = (LayoutAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutAsyncEventHandler), method);
            }
            else {
                _voidEventHandler = (LayoutVoidAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutVoidAsyncEventHandler), objectInstance, method, false);
                if (_voidEventHandler == null)
                    _eventHandler = (LayoutAsyncEventHandler)Delegate.CreateDelegate(typeof(LayoutAsyncEventHandler), objectInstance, method);
            }
        }
    }

    /// <summary>
    /// Allow event subscription by annotating event handler methods.
    /// </summary>
    /// TODO: Add SubscriptionType={Type} for creating custom subscription objects
    public abstract class LayoutEventAttributeBase : System.Attribute {
        /// <summary>
        /// The type of the subscription class to create. The type must be derived from LayoutSubscription
        /// </summary>
        public Type? SubscriptionType { get; set; }

        public string EventName { get; set; }

        /// <summary>
        /// Role - Is it a request or notification
        /// </summary>
        public LayoutEventRole Role { get; set; }

        /// <summary>
        /// Accept only event which are sent by this type of object (or a type derived from it)
        /// </summary>
        public Type? SenderType { get; set; }

        /// <summary>
        /// Accept only events whose event object is of this type (or derived from it)
        /// </summary>
        public Type? EventType { get; set; }

        /// <summary>
        /// Accept only events whose info object is of this type (or derived from it)
        /// </summary>
        public Type? InfoType { get; set; }

        /// <summary>
        /// Accept events in which the sender XML document matches this XPath expression
        /// </summary>
        public string? IfSender { get; set; }

        /// <summary>
        /// Accept events in which the event object XML document matches this XPath expression
        /// </summary>
        public string? IfEvent { get; set; }

        /// <summary>
        /// Accept events in which the info object XML matches this XPath expression
        /// </summary>
        public string? IfInfo { get; set; }

        /// <summary>
        /// Provide the processing order of this subscription. Subscriptiosn with smaller
        /// order values will be processed first
        /// </summary>
        public int Order { get; set; }

#if NOT
        public LayoutEventAttributeBase() {
		}
#endif

        /// <summary>
        /// Construct an event subscription based on setup string
        /// </summary>
        /// <param name="setupString"></param>
        protected LayoutEventAttributeBase(string eventName) {
            this.EventName = eventName;
            this.Role = LayoutEventRole.Unspecified;
        }

        public abstract LayoutEventSubscriptionBase CreateSubscription();
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LayoutEventAttribute : LayoutEventAttributeBase {
        public LayoutEventAttribute(string eventName)
            : base(eventName) {
        }

        /// <summary>
        /// Create the subscription initialized by this attribute
        /// </summary>
        /// <returns>The new subscription object</returns>
        public override LayoutEventSubscriptionBase CreateSubscription() => new LayoutEventSubscription(this.EventName);
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LayoutAsyncEventAttribute : LayoutEventAttributeBase {
#if NOT
        public LayoutAsyncEventAttribute()
			: base() {
		}
#endif

        public LayoutAsyncEventAttribute(string eventName)
            : base(eventName) {
        }

        /// <summary>
        /// Create the subscription initialized by this attribute
        /// </summary>
        /// <returns>The new subscription object</returns>
        public override LayoutEventSubscriptionBase CreateSubscription() => new LayoutAsyncEventSubscription(this.EventName);
    }

    /// <summary>
    /// Use this attribute to define properties of a layout event
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class LayoutEventDefAttribute : System.Attribute {
        public LayoutEventDefAttribute(string name) {
            this.Name = name;
        }

        public LayoutEventRole Role { get; set; } = LayoutEventRole.Notification;

        public Type? SenderType { get; set; }

        public Type? InfoType { get; set; }

        public string Name { get; }

        public LayoutEventScope Scope { get; set; } = LayoutEventScope.MyProcess;
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
        void RemoveObjectSubscriptions(object instance);

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

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public DelayedEventStatus Status { get; private set; }
        public Guid Id { get; }

        internal LayoutDelayedEvent(int delayTime, LayoutEvent theEvent) {
            this.Event = theEvent;

            Id = Guid.NewGuid();
            DoDelay(delayTime, cancellationTokenSource.Token);
        }

        public LayoutEvent Event { get; }

        private async void DoDelay(int delayTime, CancellationToken cancellationToken) {
            try {
                var tcs = new TaskCompletionSource<object>();

                using (cancellationToken.Register(() => tcs.TrySetCanceled())) {
                    EventManager.Instance.RegisterDelayedEvent(this);

                    Status = DelayedEventStatus.NotYetCalled;
                    await Task.WhenAny(Task.Delay(delayTime), tcs.Task).ConfigureAwait(false);
                    Status = DelayedEventStatus.Called;
                    EventManager.Instance.InterThreadEventInvoker.QueueEvent(Event);
                }
            }
            catch (OperationCanceledException) {
                Status = DelayedEventStatus.Canceled;
            }
            finally {
                EventManager.Instance.UnregisterDelayedEvent(this);
            }
        }

        public void Cancel() {
            if (Status != DelayedEventStatus.Called)
                cancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    /// Manage events and subscriptions
    /// </summary>
    public class LayoutEventManager {
        private ILayoutInterThreadEventInvoker? invoker;
        private readonly Dictionary<Guid, LayoutDelayedEvent> activeDelayedEvents = new Dictionary<Guid, LayoutDelayedEvent>();
        private LayoutEventDefAttribute[]? eventDefs;

#pragma warning disable IDE0060 // Remove unused parameter
        public LayoutEventManager(LayoutModuleManager moduleManager) {
#pragma warning restore IDE0060 // Remove unused parameter
            this.Subscriptions = new LayoutSubscriptionHashtableByEventName();
        }

        /// <summary>
        /// A collection of active event subscriptions
        /// </summary>
        public ILayoutSubscriptionCollection Subscriptions { get; }

        /// <summary>
        /// If true, event tracing is enabled
        /// </summary>
        public bool TraceEvents { get; set; }

        public ILayoutInterThreadEventInvoker InterThreadEventInvoker {
            get {
                if (invoker == null)
                    invoker = (ILayoutInterThreadEventInvoker?)Event(new LayoutEvent("get-inter-thread-event-invoker", this));
                return invoker!;
            }
        }

        #region Synchronous (normal) events

        /// <summary>
        /// Internal method for generating event
        /// </summary>
        /// <param name="e">The event to generate</param>
        /// <param name="scope">The event scope</param>
        /// <param name="traceEvent">Optional trace event to be generated</param>
        protected void GenerateEvent(LayoutEvent e, LayoutEventScope scope, LayoutEventTraceEvent? traceEvent) {
            List<LayoutEventSubscriptionBase> applicableSubscriptions = new List<LayoutEventSubscriptionBase>();

            Subscriptions.AddApplicableSubscriptions<LayoutEventSubscription>(applicableSubscriptions, e);
            if (applicableSubscriptions.Count > 1)
                applicableSubscriptions.Sort((s1, s2) => s1.Order - s2.Order);

            if (traceEvent != null) {
                traceEvent.ApplicableSubscriptions = applicableSubscriptions;
                traceEvent.Scope = scope;

                GenerateEvent(traceEvent, LayoutEventScope.MyProcess, null);
            }

            foreach (LayoutEventSubscription subscription in applicableSubscriptions) {
                subscription.EventHandler?.Invoke(e);

                if (!e.ContinueProcessing)
                    break;
            }
        }

        /// <summary>
        /// Send an event
        /// </summary>
        /// <param name="e">The event to be sent</param>
        /// <param name="scope">The scope in which the event should be sent <see cref="LayoutEventScope"/></param>
        /// <returns>The event info field</returns>
        public object? Event(LayoutEvent e, LayoutEventScope scope) {
            LayoutEventTraceEvent? traceEvent = null;

            if (TraceEvents)
                traceEvent = new LayoutEventTraceEvent(e, "trace-event");

            GenerateEvent(e, scope, traceEvent);
            return e.Info;
        }

        public object? Event(LayoutEvent e) => Event(e, LayoutEventScope.MyProcess);

        #endregion

        #region Async events

        /// <summary>
        /// Internal method for generating async event
        /// </summary>
        /// <param name="e">The event to generate</param>
        /// <param name="scope">The event scope</param>
        /// <param name="traceEvent">Optional trace event to be generated</param>
        protected List<Task> GenerateAsyncEvent(LayoutEvent e, LayoutEventScope scope, LayoutEventTraceEvent? traceEvent) {
            var applicableSubscriptions = new List<LayoutEventSubscriptionBase>();

            Subscriptions.AddApplicableSubscriptions<LayoutAsyncEventSubscription>(applicableSubscriptions, e);
            if (applicableSubscriptions.Count > 1)
                applicableSubscriptions.Sort((s1, s2) => s1.Order - s2.Order);

            if (traceEvent != null) {
                traceEvent.ApplicableSubscriptions = applicableSubscriptions;
                traceEvent.Scope = scope;

                GenerateEvent(traceEvent, LayoutEventScope.MyProcess, null);
            }

            var eventTasks = new List<Task>(applicableSubscriptions.Count);

            foreach (LayoutAsyncEventSubscription subscription in applicableSubscriptions) {
                if (subscription.EventHandler != null)
                    eventTasks.Add(subscription.InvokeEventHandler(e));

                if (!e.ContinueProcessing)
                    break;
            }

            return eventTasks;
        }

        public Task AsyncEvent(LayoutEvent e, LayoutEventScope scope) {
            LayoutEventTraceEvent? traceEvent = null;

            if (TraceEvents)
                traceEvent = new LayoutEventTraceEvent(e, "trace-event");

            List<Task> tasks = GenerateAsyncEvent(e, scope, traceEvent);

            if (tasks.Count == 1)
                return tasks[0];

            throw new ApplicationException("AsyncEvent " + e.EventName + " has one or more than one event handlers - (consider using AsyncEventBroadcast)");
        }

        public Task AsyncEvent(LayoutEvent e) => AsyncEvent(e, LayoutEventScope.MyProcess);

        public Task[] AsyncEventBroadcast(LayoutEvent e, LayoutEventScope scope) {
            LayoutEventTraceEvent? traceEvent = null;

            if (TraceEvents)
                traceEvent = new LayoutEventTraceEvent(e, "trace-event");

            return GenerateAsyncEvent(e, scope, traceEvent).ToArray();
        }

        public Task[] AsyncEventBroadcast(LayoutEvent e) => AsyncEventBroadcast(e, LayoutEventScope.MyProcess);

        #endregion

        public LayoutDelayedEvent DelayedEvent(int delayTime, LayoutEvent e) => new LayoutDelayedEvent(delayTime, e);

        internal void RegisterDelayedEvent(LayoutDelayedEvent delayedEvent) {
            lock (activeDelayedEvents) {
                activeDelayedEvents.Add(delayedEvent.Id, delayedEvent);
            }
        }

        internal void UnregisterDelayedEvent(LayoutDelayedEvent delayedEvent) {
            lock (activeDelayedEvents) {
                activeDelayedEvents.Remove(delayedEvent.Id);
            }
        }

        /// <summary>
        /// Try to find a delayed event based on its ID
        /// </summary>
        public LayoutDelayedEvent this[Guid delayedEventID] {
            get {
                lock (activeDelayedEvents)
                    return activeDelayedEvents[delayedEventID];
            }
        }

        public LayoutEventScript EventScript(string scriptName, XmlElement conditionElement, ICollection<Guid> scopeIDs, LayoutEvent scriptDoneEvent, LayoutEvent? errorOccurredEvent) => new LayoutEventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, errorOccurredEvent);

        public LayoutEventScript EventScript(string scriptName, XmlElement conditionElement, ICollection<Guid> scopeIDs, LayoutEvent scriptDoneEvent) => EventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, null);

        protected void AddMethodSubscriptions<TSubscriptionAttribute, TSubscription>(ILayoutSubscriptionCollection subscriptions, object objectInstance, MethodInfo methodInfo) where TSubscriptionAttribute : LayoutEventAttributeBase where TSubscription : LayoutEventSubscriptionBase {
            var eventAttributes = (TSubscriptionAttribute[])methodInfo.GetCustomAttributes(typeof(TSubscriptionAttribute), true);

            foreach (var eventAttribute in eventAttributes) {
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
        public void AddObjectSubscriptions(object classInstance) {
            MethodInfo[] methodsInfo = classInstance.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo methodInfo in methodsInfo) {
                AddMethodSubscriptions<LayoutEventAttribute, LayoutEventSubscription>(Subscriptions, classInstance, methodInfo);
                AddMethodSubscriptions<LayoutAsyncEventAttribute, LayoutAsyncEventSubscription>(Subscriptions, classInstance, methodInfo);
            }
        }

        /// <summary>
        /// Get all the subscriptions to event handler of a given object instance
        /// </summary>
        /// <param name="instance">The object instance</param>
        /// <returns>Array of LayoutEventSubscription to event handler of the given object</returns>
        public LayoutEventSubscriptionBase[] GetObjectSubscriptions(object instance) {
            ArrayList instanceSubscriptions = new ArrayList();

            foreach (LayoutEventSubscriptionBase subscription in Subscriptions)
                if (subscription.TargetObject == instance)
                    instanceSubscriptions.Add(subscription);

            return (LayoutEventSubscriptionBase[])instanceSubscriptions.ToArray(typeof(LayoutEventSubscriptionBase));
        }

        /// <summary>
        /// Refresh all the subscriptions of a given object. This method should be called after changing
        /// the object XML document
        /// </summary>
        /// <param name="instance"></param>
        public void RefreshObjectSubscriptions(object instance) {
            foreach (LayoutEventSubscriptionBase subscription in GetObjectSubscriptions(instance))
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
        public void OptimizeForFilteringBySenderType(string eventName) {
            ((LayoutSubscriptionHashtableByEventName)Subscriptions).OptimizeForFilteringBySenderType(eventName);
        }

        public LayoutEventDefAttribute[] GetEventDefinitions(LayoutEventRole requiredRole) {
            if (eventDefs == null) {
                var eventDefsList = new List<LayoutEventDefAttribute>();
                var moduleManager = (LayoutModuleManager?)Event(new LayoutEvent("get-module-manager", this));

                Debug.Assert(moduleManager != null);

                foreach (LayoutAssembly layoutAssembly in moduleManager.LayoutAssemblies) {
                    LayoutEventDefAttribute[] assemblyEventDefs = (LayoutEventDefAttribute[])layoutAssembly.Assembly.GetCustomAttributes(typeof(LayoutEventDefAttribute), true);

                    addEventDefs(assemblyEventDefs, requiredRole, eventDefsList);
                    addEventDefs(layoutAssembly.Assembly.GetTypes(), requiredRole, eventDefsList);
                }

                eventDefs = eventDefsList.ToArray();
            }

            return eventDefs;
        }

        public void FlushEventDefinitions() {
            eventDefs = null;
        }

        #region Helper methods for getting event attributes

        private void addEventDefs(LayoutEventDefAttribute[] eventDefsToAdd, LayoutEventRole requiredRole, List<LayoutEventDefAttribute> eventDefs) {
            foreach (LayoutEventDefAttribute eventDef in eventDefsToAdd) {
                if (eventDef.Role == requiredRole)
                    eventDefs.Add(eventDef);
            }
        }

        private void addEventDefs(Type[] types, LayoutEventRole requiredRole, List<LayoutEventDefAttribute> eventDefs) {
            foreach (Type type in types) {
                if (type.IsClass || type.IsInterface) {
                    LayoutEventDefAttribute[] typeEventDefs = (LayoutEventDefAttribute[])type.GetCustomAttributes(typeof(LayoutEventDefAttribute), true);

                    addEventDefs(typeEventDefs, requiredRole, eventDefs);

                    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                        LayoutEventDefAttribute[] methodEventDefs = (LayoutEventDefAttribute[])method.GetCustomAttributes(typeof(LayoutEventDefAttribute), true);

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
        public LayoutEventTraceEvent(LayoutEvent theEvent, string traceEventName) : base(traceEventName, theEvent) {
            ApplicableSubscriptions = new List<LayoutEventSubscriptionBase>();
        }

        public ICollection<LayoutEventSubscriptionBase> ApplicableSubscriptions { get; set; }

        public LayoutEventScope Scope { get; set; }
    }

    public static class EventManager {
        private static LayoutEventManager eventManager;

        public static LayoutEventManager Instance {
            get {
                return eventManager!;
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
        public static object? Event(LayoutEvent e) => Instance.Event(e);

        public static TEvent DoEvent<TEvent>(TEvent e) where TEvent : LayoutEvent {
            Instance.Event(e);
            return e;
        }

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
        public static void AddObjectSubscriptions(object classInstance) {
            Instance.AddObjectSubscriptions(classInstance);
        }

        /// <summary>
        /// Return a collection of all events subscription.
        /// </summary>
        public static ILayoutSubscriptionCollection Subscriptions => Instance.Subscriptions;

        public static LayoutEventScript EventScript(string scriptName, XmlElement? conditionElement, ICollection<Guid> scopeIDs, LayoutEvent? scriptDoneEvent, LayoutEvent? errorOccurredEvent) => new LayoutEventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, errorOccurredEvent);

        public static LayoutEventScript EventScript(string scriptName, XmlElement? conditionElement, ICollection<Guid> scopeIDs, LayoutEvent? scriptDoneEvent) => EventScript(scriptName, conditionElement, scopeIDs, scriptDoneEvent, null);

        public static object? Event(string eventName, object? sender = null, object? info = null, string? xmlDocument = null, Type? targetType = null, string? ifTarget = null) =>
            Instance.Event(new LayoutEvent(eventName, sender, info, xmlDocument, targetType, ifTarget));

        public static TResult? Event<TSender, TInfo, TResult>(
            string eventName,
            TSender? sender = null,
            TInfo? info = null,
            string? xmlDocument = null,
            Type? targetType = null,
            string? ifTarget = null) where TSender : class where TInfo : class where TResult : class {
            var theEvent = new LayoutEvent<TSender, TInfo, TResult>(eventName, sender, info, xmlDocument, targetType, ifTarget);
            Instance.Event(theEvent);

            return theEvent.Result;
        }

        public static TResult? EventResultValueType<TSender, TInfo, TResult>(
            string eventName,
            TSender? sender = null,
            TInfo? info = null,
            string? xmlDocument = null,
            Type? targetType = null,
            string? ifTarget = null) where TSender : class where TInfo : class where TResult : struct {
            var theEvent = new LayoutEventResultValueType<TSender, TInfo, TResult>(eventName, sender, info, xmlDocument, targetType, ifTarget);
            Instance.Event(theEvent);

            return theEvent.Result;
        }
    }

    /// <summary>
    /// A subscription collection implemented as an array. <see cref="ILayoutSubscriptionCollection"/>
    /// </summary>
    internal class LayoutSubscriptionArray : ILayoutSubscriptionCollection {
        private readonly List<LayoutEventSubscriptionBase> subscriptions = new List<LayoutEventSubscriptionBase>();

        public void Add(LayoutEventSubscriptionBase subscription) {
            subscriptions.Add(subscription);
            // TODO: Generate an event that a subscription was added
        }

        public void Remove(LayoutEventSubscriptionBase subscription) {
            subscriptions.Remove(subscription);
            // TODO: Generate an event that a subscription was removed
        }

        public void RemoveObjectSubscriptions(object instance) {
            List<LayoutEventSubscriptionBase> subscriptionsToRemove = new List<LayoutEventSubscriptionBase>();

            foreach (LayoutEventSubscriptionBase subscription in subscriptions)
                if (subscription.TargetObject == instance)
                    subscriptionsToRemove.Add(subscription);

            foreach (LayoutEventSubscriptionBase subscription in subscriptionsToRemove)
                Remove(subscription);
        }

        public void AddApplicableSubscriptions<TSubscription>(ICollection<LayoutEventSubscriptionBase> applicableSubscriptions, LayoutEvent e) {
            foreach (LayoutEventSubscriptionBase subscription in subscriptions)
                if (e.IsSubscriptionApplicable(subscription) && subscription.IsEventApplicable(e) && subscription is TSubscription)
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
    internal class LayoutSubscriptionHashtableByEventName : ILayoutSubscriptionCollection {
        private readonly Dictionary<string, ILayoutSubscriptionCollection> subscriptionByEventName = new Dictionary<string, ILayoutSubscriptionCollection>();
        private readonly LayoutSubscriptionArray noEventNameFilterSubscriptions = new LayoutSubscriptionArray();

        private ILayoutSubscriptionCollection getSlot(LayoutEventSubscriptionBase subscription) {
            if (!string.IsNullOrEmpty(subscription.EventName)) {
                if (!subscriptionByEventName.TryGetValue(subscription.EventName, out ILayoutSubscriptionCollection hashEntry)) {
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

        public void RemoveObjectSubscriptions(object instance) {
            foreach (ILayoutSubscriptionCollection hashEntry in subscriptionByEventName.Values)
                hashEntry.RemoveObjectSubscriptions(instance);
            noEventNameFilterSubscriptions.RemoveObjectSubscriptions(instance);
        }

        public void AddApplicableSubscriptions<TSubscription>(ICollection<LayoutEventSubscriptionBase> applicableSubscriptions, LayoutEvent e) {
            if (subscriptionByEventName.TryGetValue(e.EventName, out ILayoutSubscriptionCollection hashEntry))
                hashEntry.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);

            noEventNameFilterSubscriptions.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);
        }

        public IEnumerator<LayoutEventSubscriptionBase> GetEnumerator() {
            List<LayoutEventSubscriptionBase> allSubscriptions = new List<LayoutEventSubscriptionBase>();

            foreach (ILayoutSubscriptionCollection hashEntry in subscriptionByEventName.Values) {
                foreach (LayoutEventSubscriptionBase subscription in hashEntry)
                    allSubscriptions.Add(subscription);
            }

            foreach (LayoutEventSubscriptionBase subscription in noEventNameFilterSubscriptions)
                allSubscriptions.Add(subscription);

            return allSubscriptions.GetEnumerator();
        }

        public void OptimizeForFilteringBySenderType(string eventName) {
            ILayoutSubscriptionCollection hashEntry = (ILayoutSubscriptionCollection)subscriptionByEventName[eventName];
            ILayoutSubscriptionCollection newEntry = new LayoutSubscriptionHashtableBySenderType();

            subscriptionByEventName.Remove(eventName);
            subscriptionByEventName[eventName] = newEntry;

            if (hashEntry != null) {
                foreach (LayoutEventSubscriptionBase subscription in hashEntry)
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
    internal class LayoutSubscriptionHashtableBySenderType : ILayoutSubscriptionCollection {
        private readonly Dictionary<Type, ILayoutSubscriptionCollection> subscriptionBySenderType = new Dictionary<Type, ILayoutSubscriptionCollection>();
        private readonly LayoutSubscriptionArray noSenderTypeSubscriptions = new LayoutSubscriptionArray();

        private ILayoutSubscriptionCollection getSlot(LayoutEventSubscriptionBase subscription) {
            if (subscription.SenderType != null) {
                if (!subscriptionBySenderType.TryGetValue(subscription.SenderType, out ILayoutSubscriptionCollection hashEntry)) {
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

        public void RemoveObjectSubscriptions(object instance) {
            foreach (ILayoutSubscriptionCollection hashEntry in subscriptionBySenderType.Values)
                hashEntry.RemoveObjectSubscriptions(instance);
            noSenderTypeSubscriptions.RemoveObjectSubscriptions(instance);
        }

        public void AddApplicableSubscriptions<TSubscription>(ICollection<LayoutEventSubscriptionBase> applicableSubscriptions, LayoutEvent e) {
            if (e.Sender != null && subscriptionBySenderType.TryGetValue(e.Sender.GetType(), out ILayoutSubscriptionCollection hashEntry))
                hashEntry.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);

            noSenderTypeSubscriptions.AddApplicableSubscriptions<TSubscription>(applicableSubscriptions, e);
        }

        public IEnumerator<LayoutEventSubscriptionBase> GetEnumerator() {
            List<LayoutEventSubscriptionBase> allSubscriptions = new List<LayoutEventSubscriptionBase>();

            foreach (ILayoutSubscriptionCollection hashEntry in subscriptionBySenderType.Values) {
                foreach (LayoutEventSubscriptionBase subscription in hashEntry)
                    allSubscriptions.Add(subscription);
            }

            foreach (LayoutEventSubscriptionBase subscription in noSenderTypeSubscriptions)
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

