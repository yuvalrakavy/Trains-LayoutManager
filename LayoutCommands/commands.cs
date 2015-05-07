using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Xml;

using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager {

	/// <summary>
	/// Place a component in the model
	/// </summary>
	public class LayoutComponentPlacmentCommand : LayoutCommand {
		LayoutModelArea		area;
		Point				areaLocation;
		ModelComponent		component;
		ModelComponent		previousComponent;
		LayoutPhase			previousPhase;
		String				description;
		LayoutPhase			phase;

		public LayoutComponentPlacmentCommand(LayoutModelArea area, Point ml, ModelComponent c, String description, LayoutPhase phase) {
			this.area = area;
			this.areaLocation = ml;
			this.component = c;
			this.description = description;
			this.phase = phase;
		}

		override public void Do() {
			LayoutModelSpotComponentCollection spot = area[areaLocation, LayoutPhase.All];

			if(spot != null) {
				previousComponent = spot[component.Kind];
				previousPhase = spot.Phase;
			}

			area.Add(areaLocation, component, phase);
			component.OnXmlInfoChanged();
		}

		override public void Undo() {
			component.EraseImage();
			component.Spot.Remove(component);
			if(previousComponent != null)
				area.Add(areaLocation, previousComponent, previousPhase);

			if(component.Spot.Count == 0)
				area.RemoveSpot(component.Spot);
		}

		override public String ToString() {
			return description;
		}
	}

	public class LayoutComponentRemovalCommand : LayoutCommand {
		LayoutComponentPlacmentCommand	placementCommand;

		public LayoutComponentRemovalCommand(ModelComponent component, String description) {
			placementCommand = new LayoutComponentPlacmentCommand(component.Spot.Area, component.Location, component, description, component.Spot.Phase);
		}

		public override void Do() {
			placementCommand.Undo();
		}

		public override void Undo() {
			placementCommand.Do();
		}

		public override String ToString() {
			return placementCommand.ToString();
		}
	}

	public class LayoutComponentSelectCommand : LayoutCommand {
		ModelComponent		component;
		LayoutSelection		selection;
		String				description;

		public LayoutComponentSelectCommand(LayoutSelection selection, ModelComponent component, String description) {
			this.component = component;
			this.selection = selection;
			this.description = description;
		}

		public override void Do() {
			selection.Add(component);
		}

		public override void Undo() {
			selection.Remove(component);
		}

		public override String ToString() {
			return description;
		}
	}

	public class LayoutComponentDeselectCommand : LayoutCommand {
		LayoutComponentSelectCommand	selectCommand;

		public LayoutComponentDeselectCommand(LayoutSelection selection, ModelComponent component, String description) {
			selectCommand = new LayoutComponentSelectCommand(selection, component, description);
		}

		public override void Do() {
			selectCommand.Undo();
		}

		public override void Undo() {
			selectCommand.Do();
		}

		public override String ToString() {
			return selectCommand.ToString();
		}
	}

	public class LayoutComponentLinkCommand : LayoutCommand {
		LayoutTrackLinkComponent	trackLinkComponent;
		LayoutTrackLink				destinationTrackLink;

		public LayoutComponentLinkCommand(LayoutTrackLinkComponent trackLinkComponent, LayoutTrackLink destinationTrackLink) {
			if(destinationTrackLink == null)
				throw new ArgumentException("desintation TrackLink is null", nameof(destinationTrackLink));

			this.trackLinkComponent = trackLinkComponent;
			this.destinationTrackLink = destinationTrackLink;
		}

		public override void Do() {
			LayoutTrackLinkComponent	destinationTrackLinkComponent = destinationTrackLink.ResolveLink();

			if(destinationTrackLinkComponent.Link != null)
				throw new ApplicationException("Destination track link is already linked");

			trackLinkComponent.EraseImage();
			destinationTrackLinkComponent.EraseImage();

			trackLinkComponent.Link = destinationTrackLink;
			destinationTrackLinkComponent.Link = trackLinkComponent.ThisLink;

			trackLinkComponent.Redraw();
			destinationTrackLinkComponent.Redraw();
		}

		public override void Undo() {
			LayoutComponentUnlinkCommand	unlinkCommand = new LayoutComponentUnlinkCommand(trackLinkComponent);

			unlinkCommand.Do();
		}

		public override String ToString() {
			return "link track";
		}
	}

	public class LayoutComponentUnlinkCommand : LayoutCommand {
		LayoutTrackLinkComponent	trackLinkComponent;
		LayoutTrackLink				savedLink = null;
	
		public LayoutComponentUnlinkCommand(LayoutTrackLinkComponent trackLinkComponent) {
			this.trackLinkComponent = trackLinkComponent;
		}

		public override void Do() {
			savedLink = trackLinkComponent.Link;

			if(savedLink != null) {
				LayoutTrackLinkComponent	otherComponent = savedLink.ResolveLink();

				trackLinkComponent.EraseImage();
				otherComponent.EraseImage();

				otherComponent.Link = null;
				trackLinkComponent.Link = null;

				trackLinkComponent.Redraw();
				otherComponent.Redraw();
			}
		}

		public override void Undo() {
			if(savedLink != null) {
				LayoutComponentLinkCommand	linkCommand = new LayoutComponentLinkCommand(trackLinkComponent, savedLink);

				linkCommand.Do();
			}
		}

		public override String ToString() {
			return "unlink Track";
		}
	}

	public class LayoutModifyComponentDocumentCommand : LayoutCommand {
		ModelComponent			component;
		LayoutXmlInfo			newXmlInfo;
		System.Xml.XmlDocument	prevDocument = null;

		public LayoutModifyComponentDocumentCommand(ModelComponent component, LayoutXmlInfo newXmlInfo) {
			this.component = component;
			this.newXmlInfo = newXmlInfo;
		}

		public override void Do() {
			component.EraseImage();
			prevDocument = component.XmlInfo.XmlDocument;
			component.XmlInfo.XmlDocument = newXmlInfo.XmlDocument;

			if(component.Spot != null)
				EventManager.Instance.RefreshObjectSubscriptions(component);

			component.OnXmlInfoChanged();
			component.Redraw();

			EventManager.Event(new LayoutEvent(component, "component-configuration-changed"));
		}

		public override void Undo() {
			component.EraseImage();
			component.XmlInfo.XmlDocument = prevDocument;

			if(component.Spot != null)
				EventManager.Instance.RefreshObjectSubscriptions(component);

			component.OnXmlInfoChanged();
			component.Redraw();

			EventManager.Event(new LayoutEvent(component, "component-configuration-changed"));
		}

		public override String ToString() {
			return "Edit " + component.ToString() + " properties";
		}
	}

	public class LayoutMoveModelSpotCommand : LayoutCommand {
		LayoutModelSpotComponentCollection		spot;
		Point				location;

		public LayoutMoveModelSpotCommand(LayoutModelSpotComponentCollection spot, Point newLocation) {
			this.spot = spot;
			this.location = newLocation;
		}

		public LayoutModelSpotComponentCollection Spot {
			get {
				return spot;
			}
		}

		public Point Location {
			get {
				return location;
			}
		}

		public override void Do() {
			Point	newLocation = this.location;

			this.location = spot.Location;
			spot.MoveTo(newLocation);
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Move spot to " + location;
		}
	}

	public class RedrawLayoutModelAreaCommand : LayoutCommand {
		LayoutModelArea	area;

		public RedrawLayoutModelAreaCommand(LayoutModelArea area) {
			this.area = area;
		}

		public override void Do() {
			area.Redraw();
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Redraw";
		}
	}

	#region Layout Control Related commands

	public class AddControlModuleCommand : LayoutCommand {
		ControlBus				bus;
		string					moduleTypeName;
		int						address = -1;
		ControlModuleReference	insertBefore = null;
		ControlModuleReference	addedModule = null;
		Guid					moduleLocationID;

		public AddControlModuleCommand(ControlBus bus, string moduleTypeName, Guid moduleLocationID) {
			this.bus = bus;
			this.moduleTypeName = moduleTypeName;
			this.moduleLocationID = moduleLocationID;
		}

		public AddControlModuleCommand(ControlBus bus, string moduleTypeName, Guid moduleLocationID, int address) {
			this.bus = bus;
			this.moduleTypeName = moduleTypeName;
			this.moduleLocationID = moduleLocationID;
			this.address = address;
		}

		public AddControlModuleCommand(ControlBus bus, string moduleTypeName, Guid moduleLocationID, ControlModule insertBefore) {
			this.bus = bus;
			this.moduleTypeName = moduleTypeName;
			this.moduleLocationID = moduleLocationID;
			this.insertBefore = new ControlModuleReference(insertBefore);
		}

		public override void Do() {
			if(bus.BusType.Topology == ControlBusTopology.DaisyChain) {
				if(insertBefore == null)
					addedModule = new ControlModuleReference(bus.Add(moduleLocationID, moduleTypeName));
				else
					addedModule = new ControlModuleReference(bus.Insert(moduleLocationID, insertBefore, moduleTypeName));
			}
			else
				addedModule = new ControlModuleReference(bus.Add(moduleLocationID, moduleTypeName, address));
		}

		public ControlModuleReference AddModule {
			get {
				return addedModule;
			}
		}

		public override void Undo() {
			bus.Remove(addedModule);
		}

		public override string ToString() {
			return "Add " + addedModule.Module.ModuleType.Name + " control module";
		}
	}

	public class RemoveControlModuleCommand : LayoutCommand {
		ControlModuleReference	module;
		ControlBus				bus;
		string					moduleTypeName;
		ControlModuleReference	insertBefore;
		int						address;
		Guid					moduleLocationID;

		public RemoveControlModuleCommand(ControlModule module) {
			this.module = new ControlModuleReference(module);
			bus = module.Bus;
			moduleTypeName = module.ModuleTypeName;
			moduleLocationID = module.LocationId;

			if(bus.BusType.Topology == ControlBusTopology.DaisyChain) {
				if(module.Address == bus.BusType.LastAddress)
					insertBefore = null;
				else {
					ControlModule	insertBeforeModule = bus.GetModuleUsingAddress(module.Address + module.ModuleType.NumberOfAddresses);

					if(insertBeforeModule != null)
						insertBefore = new ControlModuleReference(insertBeforeModule);
				}
			}
			else
				address = module.Address;

			this.module = new ControlModuleReference(module);
		}

		public override void Do() {
			bus.Remove(module);
		}

		public override void Undo() {
			ControlModule	addedModule;

			if(bus.BusType.Topology == ControlBusTopology.DaisyChain) {
				if(insertBefore == null)
					addedModule = bus.Add(moduleLocationID, moduleTypeName);
				else
					addedModule = bus.Insert(moduleLocationID, insertBefore, moduleTypeName);
			}
			else
				addedModule = bus.Add(moduleLocationID, moduleTypeName, address);

			addedModule.Id = module.ModuleId;
			module.Module.LocationId = moduleLocationID;
		}

		public override string ToString() {
			return "Remove " + module.Module.ModuleType.Name + " control module";
		}
	}

	public class SetControlModuleAddressCommand : LayoutCommand {
		ControlModuleReference	module;
		int						address;

		public SetControlModuleAddressCommand(ControlModule module, int address) {
			this.module = new ControlModuleReference(module);
			this.address = address;
		}

		public override void Do() {
			int	previousAddress = module.Module.Address;

			module.Module.Address = address;
			address = previousAddress;

			EventManager.Event(new LayoutEvent(module.Module, "control-module-address-changed"));
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Set control module address";
		}
	}

	public class AssignControlModuleLocationCommand : LayoutCommand {
		ControlModuleReference	module;
		Guid					locationID;

		public AssignControlModuleLocationCommand(ControlModule module, Guid locationID) {
			this.module = new ControlModuleReference(module);
			this.locationID = locationID;
		}

		public override void Do() {
			Guid	previousLocationID = module.Module.LocationId;

			module.Module.LocationId = locationID;
			locationID = previousLocationID;
			EventManager.Event(new LayoutEvent(module.Module, "control-module-location-changed"));
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Assign module location";
		}
	}

	public class ConnectComponentToControlConnectionPointCommand : LayoutCommand {
		IModelComponentConnectToControl	component;
		string						name;
		string						displayName;
		ControlModuleReference		module;
		int							index;
		XmlElement					connectionPointElement = null;

		public ConnectComponentToControlConnectionPointCommand(ControlModule module, int index, IModelComponentConnectToControl component, string connectionName, string displayName) {
			this.module = new ControlModuleReference(module);
			this.index = index;
			this.component = component;
			this.name = connectionName;
			this.displayName = displayName;
		}

		public ConnectComponentToControlConnectionPointCommand(ControlModule module, XmlElement connectionPointElement) {
			this.module = new ControlModuleReference(module);

			XmlDocument	doc = LayoutXmlInfo.XmlImplementation.CreateDocument();
			connectionPointElement = (XmlElement)doc.ImportNode(connectionPointElement, true);
			
			ControlConnectionPoint	connectionPoint = new ControlConnectionPoint(module, connectionPointElement);
			this.index = connectionPoint.Index;
			this.component = connectionPoint.Component;
		}

		public override void Do() {
			if(connectionPointElement == null) {
				ControlConnectionPoint	connectionPoint = module.Module.ConnectionPoints[index];

				connectionPoint.Component = component;
				connectionPoint.Name = name;
				connectionPoint.DisplayName = displayName;
			}
			else
				module.Module.ConnectionPoints.Add(connectionPointElement);
		}

		public override void Undo() {
			XmlDocument		doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

			connectionPointElement = (XmlElement)doc.ImportNode(module.Module.ConnectionPoints[index].Element, true);

			doc.AppendChild(connectionPointElement);

			module.Module.ConnectionPoints.Disconnect(index);
		}

		public override string ToString() {
			return "Connect component";
		}
	}

	public class DisconnectComponentFromConnectionPointCommand : LayoutCommand {
		ConnectComponentToControlConnectionPointCommand[]	commands = null;
		bool												disconnectComponent = false;

		public DisconnectComponentFromConnectionPointCommand(IModelComponentConnectToControl component) {
			IList<ControlConnectionPoint>	connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component];

			commands = new ConnectComponentToControlConnectionPointCommand[connectionPoints.Count];

			int		i = 0;
			foreach(ControlConnectionPoint connectionPoint in connectionPoints) 
				commands[i++] = new ConnectComponentToControlConnectionPointCommand(connectionPoint.Module, connectionPoint.Element);

			disconnectComponent = true;
		}

		public DisconnectComponentFromConnectionPointCommand(ControlConnectionPoint connectionPoint) {
			commands = new ConnectComponentToControlConnectionPointCommand[1];

			commands[0] = new ConnectComponentToControlConnectionPointCommand(connectionPoint.Module, connectionPoint.Element);
		}

		public override void Do() {
			foreach(ConnectComponentToControlConnectionPointCommand command in commands)
				command.Undo();
		}

		public override void Undo() {
			foreach(ConnectComponentToControlConnectionPointCommand command in commands)
				command.Do();
		}

		public override string ToString() {
			return "Disconnect " + (disconnectComponent ? "component" : "module connection");
		}
	}

	public class ReconnectBusToAnotherCommandBusProvider : LayoutCommand {
		ControlBus						bus;
		IModelComponentIsBusProvider    otherBusProvider;

		public ReconnectBusToAnotherCommandBusProvider(ControlBus bus, IModelComponentIsBusProvider otherBusProvider) {
			this.bus = bus;
			this.otherBusProvider = otherBusProvider;
		}

		public override void Do() {
			var current = bus.BusProvider;
			ControlBus	otherBus = bus.ControlManager.Buses.GetBus(otherBusProvider, bus.BusTypeName);

			bus.BusProvider = otherBusProvider;
			otherBus.BusProvider = current;

			this.otherBusProvider = current;

			EventManager.Event(new LayoutEvent(bus, "control-bus-reconnected"));
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Reconnect control bus";
		}
	}

	public class MoveControlModuleInDaisyChainBusCommand : LayoutCommand {
		ControlBus	bus;
		int			address;
		int			delta;
		string		commandName;

		public MoveControlModuleInDaisyChainBusCommand(ControlModule module, int delta, string commandName) {
			this.bus = module.Bus;
			this.address = module.Address;
			this.delta = delta;
			this.commandName = commandName;
		}

		public override void Do() {
			ControlModule	thisModule = bus.GetModuleUsingAddress(address);
			ControlModule	otherModule = bus.GetModuleUsingAddress(address + delta);

			thisModule.Address = address + delta;
			otherModule.Address = address;

			bus.ResetAddressModuleMap();

			this.address = address + delta;
			this.delta = -delta;

			EventManager.Event(new LayoutEvent(thisModule, "control-module-address-changed"));
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return commandName;
		}
	}

	public class SetControlModuleLabelCommand : LayoutCommand {
		ControlModuleReference	moduleRef;
		string					label;

		public SetControlModuleLabelCommand(ControlModule module, string label) {
			this.moduleRef = new ControlModuleReference(module);
			this.label = label;
		}

		public override void Do() {
			ControlModule	module = moduleRef.Module;
			string			previousLabel = module.Label;

			moduleRef.Module.Label = label;
			label = previousLabel;

			EventManager.Event(new LayoutEvent(module, "control-module-label-changed"));
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Set control module label";
		}
	}

	public class SetControlUserActionRequiredCommand : LayoutCommand {
		ControlModuleReference			moduleRef = null;
		ControlConnectionPointReference	connectionPointRef = null;
		bool							userActionRequired;

		public SetControlUserActionRequiredCommand(IControlSupportUserAction subject, bool userActionRequired) {
			if(subject is ControlConnectionPoint)
				connectionPointRef = new ControlConnectionPointReference((ControlConnectionPoint)subject);
			else if(subject is ControlConnectionPointReference)
				connectionPointRef = (ControlConnectionPointReference)subject;
			else if(subject is ControlModule)
				moduleRef = new ControlModuleReference((ControlModule)subject);
			else
				throw new ArgumentException("Subject is not supported");

			this.userActionRequired = userActionRequired;
		}

		IControlSupportUserAction Subject {
			get {
				if(moduleRef != null)
					return moduleRef.Module;
				else if(connectionPointRef != null)
					return connectionPointRef.ConnectionPoint;
				else
					return null;
			}
		}

		public override void Do() {
			IControlSupportUserAction	subject = Subject;
			bool						previousUserActionRequired = Subject.UserActionRequired;

			subject.UserActionRequired = this.userActionRequired;
			this.userActionRequired = previousUserActionRequired;

			EventManager.Event(new LayoutEvent(subject, "control-user-action-required-changed"));
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Change user action required";
		}
	}

	public class SetControlAddressProgrammingRequiredCommand : LayoutCommand {
		bool addressProgrammingRequired;
		ControlModule controlModule;

		public SetControlAddressProgrammingRequiredCommand(ControlModule controlModule, bool addressProgrammingRequired) {
			this.controlModule = controlModule;
			this.addressProgrammingRequired = addressProgrammingRequired;
		}

		public override void Do() {
			bool previousValue = controlModule.AddressProgrammingRequired;

			controlModule.AddressProgrammingRequired = this.addressProgrammingRequired;
			this.addressProgrammingRequired = previousValue;
			EventManager.Event(new LayoutEvent(this.controlModule, "control-address-programming-required-changed"));
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			if(addressProgrammingRequired)
				return "Set module address programming required";
			else
				return "Reset module address programming required";
		}
	}

	public class ChangeDefaultPhaseCommand : LayoutCommand {
		LayoutPhase phase;

		public ChangeDefaultPhaseCommand(LayoutPhase phase) {
			this.phase = phase;
		}

		public override void Do() {
			LayoutPhase previousPhase = LayoutModel.Instance.DefaultPhase;

			LayoutModel.Instance.DefaultPhase = phase;
			phase = previousPhase;
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			switch(phase) {
				case LayoutPhase.Planned: return "Set default phase to 'Planned'";
				case LayoutPhase.Construction: return "Set default phase to 'In construction'";
				case LayoutPhase.Operational: return "Set default phase to 'Operational'";
				default: return "Set default phase to ***UNKNOWN***";
			}
		}
	}

	public class ChangePhaseCommand : LayoutCommand {
		LayoutModelSpotComponentCollection spot;
		LayoutPhase phase;

		public ChangePhaseCommand(LayoutModelSpotComponentCollection spot, LayoutPhase phase) {
			this.spot = spot;
			this.phase = phase;
		}

		public override void Do() {
			LayoutPhase previousPhase = spot.Phase;
			spot.Phase = phase;
			phase = previousPhase;

			foreach(var component in spot.Components)
				component.Redraw();
		}

		public override void Undo() {
			Do();
		}

		public override string ToString() {
			return "Change component phase";
		}
	}

	#endregion
}
