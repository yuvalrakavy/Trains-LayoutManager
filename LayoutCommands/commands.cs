using System;
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
        private readonly LayoutModelArea area;
        private Point areaLocation;
        private readonly ModelComponent component;
        private ModelComponent? previousComponent;
        private LayoutPhase previousPhase;
        private readonly string description;
        private readonly LayoutPhase phase;

        public LayoutComponentPlacmentCommand(LayoutModelArea area, Point ml, ModelComponent c, string description, LayoutPhase phase) {
            this.area = area;
            this.areaLocation = ml;
            this.component = c;
            this.description = description;
            this.phase = phase;
        }

        override public void Do() {
            var spot = area[areaLocation, LayoutPhase.All];

            if (spot != null) {
                previousComponent = spot[component.Kind];
                previousPhase = spot.Phase;
            }

            area.Add(areaLocation, component, phase);
            component.OnXmlInfoChanged();
        }

        override public void Undo() {
            component.EraseImage();
            component.Spot.Remove(component);
            if (previousComponent != null)
                area.Add(areaLocation, previousComponent, previousPhase);

            if (component.Spot.Count == 0)
                area.RemoveSpot(component.Spot);
        }

        override public string ToString() => description;
    }

    public class LayoutComponentRemovalCommand : LayoutCommand {
        private readonly LayoutComponentPlacmentCommand placementCommand;

        public LayoutComponentRemovalCommand(ModelComponent component, string description) {
            placementCommand = new LayoutComponentPlacmentCommand(component.Spot.Area, component.Location, component, description, component.Spot.Phase);
        }

        public override void Do() {
            placementCommand.Undo();
        }

        public override void Undo() {
            placementCommand.Do();
        }

        public override string ToString() => placementCommand.ToString();
    }

    public class LayoutComponentSelectCommand : LayoutCommand {
        private readonly ModelComponent component;
        private readonly LayoutSelection selection;
        private readonly string description;

        public LayoutComponentSelectCommand(LayoutSelection selection, ModelComponent component, string description) {
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

        public override string ToString() => description;
    }

    public class LayoutComponentDeselectCommand : LayoutCommand {
        private readonly LayoutComponentSelectCommand selectCommand;

        public LayoutComponentDeselectCommand(LayoutSelection selection, ModelComponent component, string description) {
            selectCommand = new LayoutComponentSelectCommand(selection, component, description);
        }

        public override void Do() {
            selectCommand.Undo();
        }

        public override void Undo() {
            selectCommand.Do();
        }

        public override string ToString() => selectCommand.ToString();
    }

    public class LayoutComponentLinkCommand : LayoutCommand {
        private readonly LayoutTrackLinkComponent trackLinkComponent;
        private readonly LayoutTrackLink destinationTrackLink;

        public LayoutComponentLinkCommand(LayoutTrackLinkComponent trackLinkComponent, LayoutTrackLink destinationTrackLink) {
            this.trackLinkComponent = trackLinkComponent;
            this.destinationTrackLink = destinationTrackLink ?? throw new ArgumentException("desintation TrackLink is null", nameof(destinationTrackLink));
        }

        public override void Do() {
            LayoutTrackLinkComponent destinationTrackLinkComponent = destinationTrackLink.ResolveLink();

            if (destinationTrackLinkComponent.Link != null)
                throw new ApplicationException("Destination track link is already linked");

            trackLinkComponent.EraseImage();
            destinationTrackLinkComponent.EraseImage();

            trackLinkComponent.Link = destinationTrackLink;
            destinationTrackLinkComponent.Link = trackLinkComponent.ThisLink;

            trackLinkComponent.Redraw();
            destinationTrackLinkComponent.Redraw();
        }

        public override void Undo() {
            var unlinkCommand = new LayoutComponentUnlinkCommand(trackLinkComponent);

            unlinkCommand.Do();
        }

        public override string ToString() => "link track";
    }

    public class LayoutComponentUnlinkCommand : LayoutCommand {
        private readonly LayoutTrackLinkComponent trackLinkComponent;
        private LayoutTrackLink? savedLink = null;

        public LayoutComponentUnlinkCommand(LayoutTrackLinkComponent trackLinkComponent) {
            this.trackLinkComponent = trackLinkComponent;
        }

        public override void Do() {
            savedLink = trackLinkComponent.Link;

            if (savedLink != null) {
                LayoutTrackLinkComponent otherComponent = savedLink.ResolveLink();

                trackLinkComponent.EraseImage();
                otherComponent.EraseImage();

                otherComponent.Link = null;
                trackLinkComponent.Link = null;

                trackLinkComponent.Redraw();
                otherComponent.Redraw();
            }
        }

        public override void Undo() {
            if (savedLink != null) {
                var linkCommand = new LayoutComponentLinkCommand(trackLinkComponent, savedLink);

                linkCommand.Do();
            }
        }

        public override string ToString() => "unlink Track";
    }

    public class LayoutModifyComponentDocumentCommand : LayoutCommand {
        private readonly ModelComponent component;
        private readonly LayoutXmlInfo newXmlInfo;
        private System.Xml.XmlDocument? prevDocument = null;

        public LayoutModifyComponentDocumentCommand(ModelComponent component, LayoutXmlInfo newXmlInfo) {
            this.component = component;
            this.newXmlInfo = newXmlInfo;
        }

        public override void Do() {
            component.EraseImage();
            prevDocument = component.XmlInfo.XmlDocument;
            component.XmlInfo.XmlDocument = newXmlInfo.XmlDocument;

            if (component.Spot != null)
                EventManager.Instance.RefreshObjectSubscriptions(component);

            component.OnXmlInfoChanged();
            component.Redraw();

            EventManager.Event(new LayoutEvent("component-configuration-changed", component));
        }

        public override void Undo() {
            component.EraseImage();
            component.XmlInfo.XmlDocument = prevDocument!;

            if (component.Spot != null)
                EventManager.Instance.RefreshObjectSubscriptions(component);

            component.OnXmlInfoChanged();
            component.Redraw();

            EventManager.Event(new LayoutEvent("component-configuration-changed", component));
        }

        public override string ToString() => "Edit " + component.ToString() + " properties";
    }

    public class LayoutMoveModelSpotCommand : LayoutCommand {
        private Point location;

        public LayoutMoveModelSpotCommand(LayoutModelSpotComponentCollection spot, Point newLocation) {
            this.Spot = spot;
            this.location = newLocation;
        }

        public LayoutModelSpotComponentCollection Spot { get; }

        public Point Location => location;

        public override void Do() {
            Point newLocation = this.location;

            this.location = Spot.Location;
            Spot.MoveTo(newLocation);
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Move spot to " + location;
    }

    public class RedrawLayoutModelAreaCommand : LayoutCommand {
        private readonly LayoutModelArea area;

        public RedrawLayoutModelAreaCommand(LayoutModelArea area) {
            this.area = area;
        }

        public override void Do() {
            area.Redraw();
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Redraw";
    }

    #region Layout Control Related commands

    public class AddControlModuleCommand : LayoutCommand {
        private readonly ControlBus bus;
        private readonly string moduleTypeName;
        private readonly int address = -1;
        private readonly ControlModuleReference? insertBefore = null;
        private ControlModuleReference? addedModuleReference = null;
        private readonly Guid? moduleLocationID;

        private XmlElement? moduleElement;

        public AddControlModuleCommand(ControlBus bus, string moduleTypeName, Guid? moduleLocationID) {
            this.bus = bus;
            this.moduleTypeName = moduleTypeName;
            this.moduleLocationID = moduleLocationID;
        }

        public AddControlModuleCommand(ControlBus bus, string moduleTypeName, Guid? moduleLocationID, int address) {
            this.bus = bus;
            this.moduleTypeName = moduleTypeName;
            this.moduleLocationID = moduleLocationID;
            this.address = address;
        }

        public AddControlModuleCommand(ControlBus bus, string moduleTypeName, Guid? moduleLocationID, ControlModule insertBefore) {
            this.bus = bus;
            this.moduleTypeName = moduleTypeName;
            this.moduleLocationID = moduleLocationID;
            this.insertBefore = new ControlModuleReference(insertBefore);
        }

        public override void Do() {
            if (moduleElement == null) {
                addedModuleReference = bus.BusType.Topology switch
                {
                    ControlBusTopology.DaisyChain when insertBefore == null => new ControlModuleReference(bus.Add(moduleLocationID, moduleTypeName)),
                    ControlBusTopology.DaisyChain => new ControlModuleReference(bus.Insert(moduleLocationID, insertBefore!, moduleTypeName)),
                    var t when t == ControlBusTopology.Fixed || t == ControlBusTopology.RandomAddressing => new ControlModuleReference(bus.Add(moduleLocationID, moduleTypeName, address)),
                    _ => throw new ArgumentException($"Unexpected bus topology: {bus.BusType.Topology}")
                };

                var xmlDoc = new XmlDocument();
                moduleElement = (XmlElement)xmlDoc.ImportNode(addedModuleReference.Module.Element, true);
            }
            else {
                if (insertBefore != null)
                    bus.Insert(insertBefore.Module.Address, moduleElement);
                else
                    bus.Add(moduleElement);
            }
        }

        public ControlModuleReference AddedModule => Ensure.NotNull<ControlModuleReference>(addedModuleReference);

        public override void Undo() {
            Debug.Assert(addedModuleReference != null);
            if(addedModuleReference != null)
                bus.Remove(Ensure.NotNull<ControlModule>(addedModuleReference.Module));
        }

        public override string ToString() => $"Add {moduleTypeName} control module";
    }

    public class RemoveControlModuleCommand : LayoutCommand {
        private readonly ControlModuleReference moduleReference;
        private readonly ControlBus bus;
        private readonly XmlElement moduleElement;
        private readonly int insertBeforeAddress = -1;
        private readonly string moduleTypeName;

        public RemoveControlModuleCommand(ControlModule module) {
            moduleReference = new ControlModuleReference(module);
            bus = module.Bus;
            moduleTypeName = module.ModuleTypeName;
            var xmlDocument = new XmlDocument();
            this.moduleElement = (XmlElement)xmlDocument.ImportNode(module.Element, true);

            if (bus.BusType.Topology == ControlBusTopology.DaisyChain && module.Address != bus.BusType.LastAddress)
                insertBeforeAddress = module.Address;
        }

        public override void Do() {
            bus.Remove(moduleReference.Module);
        }

        public override void Undo() {
            if (insertBeforeAddress >= 0)
                bus.Insert(insertBeforeAddress, moduleElement);
            else
                bus.Add(moduleElement);
        }

        public override string ToString() => "Remove " + moduleTypeName + " control module";
    }

    public class SetControlModuleAddressCommand : LayoutCommand {
        private readonly ControlModuleReference moduleRef;
        private int address;

        public SetControlModuleAddressCommand(ControlModule module, int address) {
            this.moduleRef = new ControlModuleReference(module);
            this.address = address;
        }

        public override void Do() {
            int previousAddress = moduleRef.Module.Address;

            moduleRef.Module.Address = address;
            address = previousAddress;

            EventManager.Event(new LayoutEvent("control-module-address-changed", moduleRef.Module));
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Set control module address";
    }

    public class AssignControlModuleLocationCommand : LayoutCommand {
        private readonly ControlModuleReference moduleRef;
        private Guid? locationID;

        public AssignControlModuleLocationCommand(ControlModule module, Guid locationID) {
            this.moduleRef = new ControlModuleReference(module);
            this.locationID = locationID;
        }

        public override void Do() {
            var previousLocationID = moduleRef.Module.LocationId;

            moduleRef.Module.LocationId = locationID;
            locationID = previousLocationID;
            EventManager.Event(new LayoutEvent("control-module-location-changed", moduleRef.Module));
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Assign module location";
    }

    public class SetControlModuleConnectionPointsCount : LayoutCommand {
        private readonly ControlModuleReference controlModuleRef;
        private int connectionPointsCount;

        public SetControlModuleConnectionPointsCount(ControlModule module, int connectionPointsCount) {
            this.controlModuleRef = new ControlModuleReference(module);
            this.connectionPointsCount = connectionPointsCount;
        }

        public override void Do() {
            var module = this.controlModuleRef.Module;
            int previousConnectionPointsCount = module.NumberOfConnectionPoints;

            module.NumberOfConnectionPoints = this.connectionPointsCount;
            this.connectionPointsCount = previousConnectionPointsCount;
            EventManager.Event(new LayoutEvent("control-module-connection-point-count-changed", controlModuleRef.Module));
            EventManager.Event(new LayoutEvent("control-module-modified", controlModuleRef.Module));
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => $"Define {connectionPointsCount} connection points for {controlModuleRef.Module?.ModuleTypeName ?? "(?)"}";
    }

    public class ConnectComponentToControlConnectionPointCommand : LayoutCommand {
        private readonly IModelComponentConnectToControl? component;
        private readonly string? name;
        private readonly string displayName;
        private readonly ControlModuleReference moduleReference;
        private readonly int index;
        private XmlElement? connectionPointElement = null;

        public ConnectComponentToControlConnectionPointCommand(ControlModule module, int index, IModelComponentConnectToControl component, string connectionName, string displayName) {
            this.moduleReference = new ControlModuleReference(module);
            this.index = index;
            this.component = component;
            this.name = connectionName;
            this.displayName = displayName;
        }

        public ConnectComponentToControlConnectionPointCommand(ControlModule module, XmlElement connectionPointElement) {
            this.moduleReference = new ControlModuleReference(module);

            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            connectionPointElement = (XmlElement)doc.ImportNode(connectionPointElement, true);

            var connectionPoint = new ControlConnectionPoint(module, connectionPointElement);
            this.index = connectionPoint.Index;
            this.component = connectionPoint.Component;
            this.displayName = connectionPoint.DisplayName;
        }

        public override void Do() {
            if (connectionPointElement == null) {
                ControlConnectionPoint connectionPoint = moduleReference.Module.ConnectionPoints[index];

                connectionPoint.Component = component;
                connectionPoint.Name = name;
                connectionPoint.DisplayName = displayName;
            }
            else
                moduleReference.Module.ConnectionPoints.Add(connectionPointElement);
        }

        public override void Undo() {
            XmlDocument doc = LayoutXmlInfo.XmlImplementation.CreateDocument();

            connectionPointElement = (XmlElement)doc.ImportNode(moduleReference.Module.ConnectionPoints[index].Element, true);

            doc.AppendChild(connectionPointElement);

            moduleReference.Module.ConnectionPoints.Disconnect(index);
        }

        public override string ToString() => "Connect component";
    }

    public class DisconnectComponentFromConnectionPointCommand : LayoutCommand {
        private readonly ConnectComponentToControlConnectionPointCommand[] commands;
        private readonly bool disconnectComponent = false;

        public DisconnectComponentFromConnectionPointCommand(IModelComponentConnectToControl component) {
            var connectionPoints = Ensure.NotNull<IList<ControlConnectionPoint>>(LayoutModel.ControlManager.ConnectionPoints[component]);

            commands = new ConnectComponentToControlConnectionPointCommand[connectionPoints.Count];

            int i = 0;
            foreach (ControlConnectionPoint connectionPoint in connectionPoints)
                commands[i++] = new ConnectComponentToControlConnectionPointCommand(connectionPoint.Module, connectionPoint.Element);

            disconnectComponent = true;
        }

        public DisconnectComponentFromConnectionPointCommand(ControlConnectionPoint connectionPoint) {
            commands = new ConnectComponentToControlConnectionPointCommand[1];

            commands[0] = new ConnectComponentToControlConnectionPointCommand(connectionPoint.Module, connectionPoint.Element);
        }

        public override void Do() {
            foreach (ConnectComponentToControlConnectionPointCommand command in commands)
                command.Undo();
        }

        public override void Undo() {
            foreach (ConnectComponentToControlConnectionPointCommand command in commands)
                command.Do();
        }

        public override string ToString() => "Disconnect " + (disconnectComponent ? "component" : "module connection");
    }

    public class ReconnectBusToAnotherCommandBusProvider : LayoutCommand {
        private readonly ControlBus bus;
        private IModelComponentIsBusProvider otherBusProvider;

        public ReconnectBusToAnotherCommandBusProvider(ControlBus bus, IModelComponentIsBusProvider otherBusProvider) {
            this.bus = bus;
            this.otherBusProvider = otherBusProvider;
        }

        public override void Do() {
            var current = bus.BusProvider;
            var otherBus = Ensure.NotNull<ControlBus>(bus.ControlManager.Buses.GetBus(otherBusProvider, bus.BusTypeName));

            bus.BusProvider = otherBusProvider;
            otherBus.BusProvider = current;

            this.otherBusProvider = current;

            EventManager.Event(new LayoutEvent("control-bus-reconnected", bus));
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Reconnect control bus";
    }

    public class MoveControlModuleInDaisyChainBusCommand : LayoutCommand {
        private readonly ControlBus bus;
        private int address;
        private int delta;
        private readonly string commandName;

        public MoveControlModuleInDaisyChainBusCommand(ControlModule module, int delta, string commandName) {
            this.bus = module.Bus;
            this.address = module.Address;
            this.delta = delta;
            this.commandName = commandName;
        }

        public override void Do() {
            var thisModule = bus.GetModuleUsingAddress(address);
            var otherModule = bus.GetModuleUsingAddress(address + delta);

            if (thisModule != null && otherModule != null) {
                thisModule.Address = address + delta;
                otherModule.Address = address;

                bus.ResetAddressModuleMap();

                this.address = address + delta;
                this.delta = -delta;

                EventManager.Event(new LayoutEvent("control-module-address-changed", thisModule).SetOption("ModuleTypeName", thisModule.ModuleTypeName));
            }
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => commandName;
    }

    public class SetControlModuleLabelCommand : LayoutCommand {
        private readonly ControlModuleReference moduleRef;
        private string? label;

        public SetControlModuleLabelCommand(ControlModule module, string? label) {
            this.moduleRef = new ControlModuleReference(module);
            this.label = label;
        }

        public override void Do() {
            var module = Ensure.NotNull<ControlModule>(moduleRef.Module);
            var previousLabel = module.Label;

            module.Label = label;
            EventManager.Event(new LayoutEvent("control-module-label-changed", module, label).SetOption("ModuleTypeName", module.ModuleTypeName));

            label = previousLabel;
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Set control module label";
    }

    public class SetControlUserActionRequiredCommand : LayoutCommand {
        private readonly ControlModuleReference? moduleRef = null;
        private readonly ControlConnectionPointReference? connectionPointRef = null;
        private bool userActionRequired;

        public SetControlUserActionRequiredCommand(IControlSupportUserAction subject, bool userActionRequired) {
            if (subject is ControlConnectionPoint controlConnectionPoint)
                connectionPointRef = new ControlConnectionPointReference(controlConnectionPoint);
            else if (subject is ControlConnectionPointReference controlConnectionPointReference)
                connectionPointRef = controlConnectionPointReference;
            else if (subject is ControlModule controlModule)
                moduleRef = new ControlModuleReference(controlModule);
            else
                throw new ArgumentException("Subject is not supported");

            this.userActionRequired = userActionRequired;
        }

        private IControlSupportUserAction Subject {
            get {
                if (moduleRef != null)
                    return moduleRef.Module;
                else if (connectionPointRef != null)
                    return Ensure.NotNull<IControlSupportUserAction>(connectionPointRef.ConnectionPoint);
                else
                    throw new ArgumentOutOfRangeException("both moduleRef and connectionPointRef are null");
            }
        }

        public override void Do() {
            IControlSupportUserAction subject = Subject;
            bool previousUserActionRequired = Subject.UserActionRequired;

            subject.UserActionRequired = this.userActionRequired;
            this.userActionRequired = previousUserActionRequired;

            EventManager.Event(new LayoutEvent("control-user-action-required-changed", subject));
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Change user action required";
    }

    public class SetControlAddressProgrammingRequiredCommand : LayoutCommand {
        private bool addressProgrammingRequired;
        private readonly ControlModule controlModule;

        public SetControlAddressProgrammingRequiredCommand(ControlModule controlModule, bool addressProgrammingRequired) {
            this.controlModule = controlModule;
            this.addressProgrammingRequired = addressProgrammingRequired;
        }

        public override void Do() {
            bool previousValue = controlModule.AddressProgrammingRequired;

            controlModule.AddressProgrammingRequired = this.addressProgrammingRequired;
            this.addressProgrammingRequired = previousValue;
            EventManager.Event(new LayoutEvent("control-address-programming-required-changed", this.controlModule).SetOption("ModuleTypeName", controlModule.ModuleTypeName));
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() {
            return addressProgrammingRequired ? "Set module address programming required" : "Reset module address programming required";
        }
    }

    public class ChangeDefaultPhaseCommand : LayoutCommand {
        private LayoutPhase phase;

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

        public override string ToString() => phase switch
        {
            LayoutPhase.Planned => "Set default phase to 'Planned'",
            LayoutPhase.Construction => "Set default phase to 'In construction'",
            LayoutPhase.Operational => "Set default phase to 'Operational'",
            _ => "Set default phase to ***UNKNOWN***",
        };
    }

    public class ChangePhaseCommand : LayoutCommand {
        private readonly LayoutModelSpotComponentCollection spot;
        private LayoutPhase phase;

        public ChangePhaseCommand(LayoutModelSpotComponentCollection spot, LayoutPhase phase) {
            this.spot = spot;
            this.phase = phase;
        }

        public override void Do() {
            LayoutPhase previousPhase = spot.Phase;
            spot.Phase = phase;
            phase = previousPhase;

            foreach (var component in spot.Components)
                component.Redraw();
        }

        public override void Undo() {
            Do();
        }

        public override string ToString() => "Change component phase";
    }

#endregion
}
