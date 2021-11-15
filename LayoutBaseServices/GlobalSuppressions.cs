// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

using System.Diagnostics.CodeAnalysis;

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type", Target = "~T:LayoutBaseServices.InterThreadsEvents")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Scope = "member", Target = "~M:LayoutManager.AttributeInfo.#ctor(System.Xml.XmlElement)")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:LayoutBaseServices.InterThreadsEvents.InitializeEventInterthreadRelay(LayoutManager.LayoutEvent)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>", Scope = "member", Target = "~M:LayoutBaseServices.InterThreadsEvents.InitializeEventInterthreadRelay(LayoutManager.LayoutEvent)")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>", Scope = "type", Target = "~T:LayoutManager.LayoutEventScriptParseException")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>", Scope = "type", Target = "~T:LayoutManager.LayoutEventScriptException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "RCS1146:Use conditional access.", Justification = "<Pending>", Scope = "member", Target = "~M:LayoutManager.LayoutEvent.IsSubscriptionApplicable(LayoutManager.LayoutEventSubscriptionBase)~System.Boolean")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "RCS1146:Use conditional access.", Justification = "<Pending>", Scope = "member", Target = "~M:LayoutManager.LayoutEventSubscriptionBase.IsEventApplicable(LayoutManager.LayoutEvent)~System.Boolean")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Redundancy", "RCS1163:Unused parameter.", Justification = "<Pending>", Scope = "member", Target = "~M:LayoutManager.LayoutEventManager.#ctor(LayoutManager.LayoutModuleManager)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>", Scope = "type", Target = "~T:LayoutManager.LayoutException")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "RCS1194:Implement exception constructors.", Justification = "<Pending>", Scope = "type", Target = "~T:LayoutManager.FileParseException")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "module")]
[assembly: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>", Scope = "module")]
