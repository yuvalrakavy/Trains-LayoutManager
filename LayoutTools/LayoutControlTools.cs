using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using MethodDispatcher;

using LayoutManager.Model;
using LayoutManager.Components;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools {
    [LayoutModule("Layout Control Tools", UserControl = false)]
    internal class LayoutControlTools : LayoutModuleBase {
        #region Component/Control connection manager

        private ControlConnectionPointDestination? componentToConnect = null;
        private ControlConnectionPointReference? controlPointToConnect = null;

        [LayoutEvent("request-component-to-control-connect")]
        private void RequestComponentToControlConnect(LayoutEvent e) {
            componentToConnect = (ControlConnectionPointDestination?)e.Sender;
        }

        [LayoutEvent("get-component-to-control-connect")]
        private void GetComponentToControlConnect(LayoutEvent e) {
            e.Info = componentToConnect;
        }

        [LayoutEvent("cancel-component-to-control-connect")]
        private void CancelComponentToControlConnect(LayoutEvent _) {
            componentToConnect = null;
        }

        [LayoutEvent("request-control-to-component-connect")]
        private void RequestControlToComponentConnect(LayoutEvent e) {
            controlPointToConnect = (ControlConnectionPointReference?)e.Sender;
        }

        [LayoutEvent("get-control-to-component-connect")]
        private void GetControlToComponentConnect(LayoutEvent e) {
            e.Info = controlPointToConnect;
        }

        [LayoutEvent("cancel-control-to-component-connect")]
        private void CancelControlToComponentConnect(LayoutEvent _) {
            controlPointToConnect = null;
        }

        #endregion

        #region Auto connect handling

        #region Data structures

        private class ControlAutoConnectRequest : ICloneable {
            public LayoutPhase Phase { get; set; }                                  // What is the scope of the connection
            public IModelComponentConnectToControl? OptionalComponent { get; set; }          // The component to be connected
            public IModelComponentIsCommandStation? CommandStation { get; set; }     // default: If more than one, prompt
            public bool PromptBeforeAddingModule { get; set; }
            public bool SetUserActionRequired { get; set; }
            public bool SetProgrammingRequired { get; set; }
            public int State { get; set; }                                          // Needed in case address contains more than one connection point
            public int Address { get; set; }                                        // Connect to address (default find free address)
            public int Index { get; set; }                                          // Connect to index (default no constraint)
            public string? ModuleTypeName { get; set; }                              // Default: If more than one applicable, prompt
            public ControlBus? Bus { get; set; }                                     // Default: component determins
            public LayoutControlModuleLocationComponent? ModuleLocation { get; set; }// Find the nearest
            public ModelComponentControlConnectionDescription? OptionalConnectionDescription { get; set; } // The connection's description

            public ModelComponentControlConnectionDescription ConnectionDescription => Ensure.ValueNotNull<ModelComponentControlConnectionDescription>(OptionalConnectionDescription);

            public IModelComponentConnectToControl Component => Ensure.NotNull<IModelComponentConnectToControl>(OptionalComponent);

            public ControlAutoConnectRequest(LayoutPhase phase) {
                PromptBeforeAddingModule = true;
                SetUserActionRequired = true;
                SetProgrammingRequired = true;
                State = 0;
                Address = -1;
                Index = -1;
                this.Phase = phase;
            }

            public ControlAutoConnectRequest(IModelComponentConnectToControl component) : this(ModuleLocationPhase(component)) {
                this.OptionalComponent = component;
            }

            public ControlAutoConnectRequest(IModelComponentConnectToControl component, LayoutControlModuleLocationComponent moduleLocation) : this(component) {
                this.ModuleLocation = moduleLocation;
            }

            public ControlAutoConnectRequest(IModelComponentConnectToControl component, ModelComponentControlConnectionDescription connectionDescription) : this(component) {
                this.OptionalConnectionDescription = connectionDescription;
            }

            public ControlAutoConnectRequest(IModelComponentConnectToControl component, ModelComponentControlConnectionDescription connectionDescription, LayoutControlModuleLocationComponent moduleLocation) : this(component, connectionDescription) {
                this.ModuleLocation = moduleLocation;
            }

            public ControlAutoConnectRequest(ControlAutoConnectRequest other) {
                this.Phase = other.Phase;
                this.OptionalComponent = other.OptionalComponent;
                this.OptionalConnectionDescription = other.OptionalConnectionDescription;
                this.CommandStation = other.CommandStation;
                this.ModuleLocation = other.ModuleLocation;
                this.ModuleTypeName = other.ModuleTypeName;
                this.Bus = other.Bus;
                this.Address = other.Address;
                this.State = other.State;
                this.PromptBeforeAddingModule = other.PromptBeforeAddingModule;
                this.SetUserActionRequired = other.SetUserActionRequired;
                this.SetProgrammingRequired = other.SetProgrammingRequired;
            }

            public static LayoutPhase ModuleLocationPhase(LayoutPhase componentPhase) {
                return componentPhase switch
                {
                    LayoutPhase.Operational => LayoutPhase.Operational,
                    LayoutPhase.Construction => LayoutPhase.NotPlanned,
                    LayoutPhase.Planned => LayoutPhase.All,
                    _ => throw new ApplicationException("Unexpected component phase " + componentPhase.ToString()),
                };
            }

            /// <summary>
            /// Return the phase in which module location for a given component is allowed to be
            /// </summary>
            /// <param name="component">The component</param>
            /// <returns></returns>
            public static LayoutPhase ModuleLocationPhase(IModelComponentConnectToControl component) => ModuleLocationPhase(component.Spot.Phase);

            public ControlAutoConnectRequest WithAnotherModuleLocation(LayoutControlModuleLocationComponent? moduleLocation) {
                ControlAutoConnectRequest newCopy = new(this) {
                    ModuleLocation = moduleLocation
                };
                return newCopy;
            }

            #region ICloneable Members

            public object Clone() => MemberwiseClone();

            #endregion
        }

        private struct ModuleLocationDistance : IComparable<ModuleLocationDistance> {
            public ModuleLocationDistance(IModelComponentConnectToControl aComponent, LayoutControlModuleLocationComponent moduleLocation) {
                ModelComponent component = (ModelComponent)aComponent;

                Debug.Assert(component.Spot.Area == moduleLocation.Spot.Area);

                this.ModuleLocation = moduleLocation;

                int dx = moduleLocation.Spot.Location.X - component.Spot.Location.X;
                int dy = moduleLocation.Spot.Location.Y - component.Spot.Location.Y;

                this.Distance = Math.Sqrt((dx * dx) + (dy * dy));
            }

            public LayoutControlModuleLocationComponent ModuleLocation { get; }

            public double Distance { get; }

            #region IComparable<ModuleLocationDistance> Members

            public int CompareTo(ModuleLocationDistance other) => (int)(Distance - other.Distance);

            public bool Equals(ModuleLocationDistance other) => ModuleLocation == other.ModuleLocation && Distance == other.Distance;

            #endregion
        }

        #endregion

        [LayoutEvent("request-control-auto-connect")]
        private void RequestControlAutoConnect(LayoutEvent e) {
            var request = Ensure.NotNull<ControlAutoConnectRequest>(e.Sender);
            var command = (LayoutCompoundCommand?)e.Info;

            if (command == null)
                command = new LayoutCompoundCommand("Automatically connect " + request.ConnectionDescription.DisplayName, true);

            if(command != null)
                e.Info = DoAutoConnect(request, false, command);
        }

        [LayoutEvent("connect-component-to-control-module-address-request")]
        private void ConnectComponentToControlModuleAddressRequest(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender);
            var csEvent = Ensure.NotNull<CommandStationInputEvent>(e.Info);

            ControlAutoConnectRequest request = new(connectionDestination.Component, connectionDestination.ConnectionDescription) {
                CommandStation = (IModelComponentIsCommandStation)csEvent.CommandStation,
                Bus = csEvent.Bus,
                Address = csEvent.Address,
                Index = csEvent.Index,
                State = csEvent.State,
                SetProgrammingRequired = false
            };

            LayoutCompoundCommand command = new("Connect " + request.ConnectionDescription.DisplayName + " to address " + csEvent.GetAddressTextForComponent(connectionDestination), true);
            e.Info = DoAutoConnect(request, false, command);
        }

        [LayoutEvent("get-nearest-control-module-location")]
        private void GetNearestControlModuleLocation(LayoutEvent e) {
            var component = Ensure.NotNull< IModelComponentConnectToControl>(e.Sender);

            e.Info = FindNearestModuleLocation(ControlAutoConnectRequest.ModuleLocationPhase(component), component);
        }

        private ControlConnectionPoint? DoAutoConnect(ControlAutoConnectRequest request, bool connectLayout, LayoutCompoundCommand command) {
            var defaultModules = new Dictionary<Guid, string>();

            if (request.CommandStation == null) {
                IList<IModelComponentIsCommandStation> commandStations = LayoutModel.Components<IModelComponentIsCommandStation>(request.Phase).ToList<IModelComponentIsCommandStation>();

                if (commandStations.Count == 0)
                    throw new LayoutException("No command station components are defined, it is not possible to connect components");
                else if (commandStations.Count == 1)
                    request.CommandStation = commandStations[0];
                else {
                    if (request.ModuleLocation != null && request.ModuleLocation.Info.CommandStation != null)
                        request.CommandStation = request.ModuleLocation.Info.CommandStation;
                    else {
                        using AutoConnectDialogs.GetCommandStation d = new(commandStations);

                        EnsureVisible(request);
                        if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                            return null;

                        request.CommandStation = d.CommandStation;
                    }
                }
            }

            if (!request.OptionalConnectionDescription.HasValue && request.OptionalComponent != null) {
                foreach (ModelComponentControlConnectionDescription connectionDescription in request.Component.ControlConnectionDescriptions) {
                    if (!LayoutModel.ControlManager.ConnectionPoints.IsConnected(request.Component, connectionDescription.Name)) {
                        request.OptionalConnectionDescription = connectionDescription;
                        break;
                    }
                }
            }

            if (request.Bus != null) {
                // Check that that the component can be connected to this bus
                if (request.Bus.BusType.GetConnectableControlModuleTypeNames(new ControlConnectionPointDestination(request.Component, request.ConnectionDescription)).Count == 0)
                    throw new LayoutException(request.Component,
                        "This component " + request.ConnectionDescription.DisplayName + " cannot be connected to control modules that are connected to the command station using " + request.Bus.Name);
            }

            if (request.ModuleLocation == null && request.Address < 0) {
                request.ModuleLocation = FindNearestModuleLocation(request.Phase, request.Component);

                if (request.ModuleLocation == null) {
                    IEnumerable<LayoutControlModuleLocationComponent> moduleLocations = LayoutModel.Components<LayoutControlModuleLocationComponent>(request.Phase);

                    if (moduleLocations.Any()) {
                        AutoConnectDialogs.GetModuleLocation d = new(moduleLocations);

                        EnsureVisible(request);
                        if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                            return null;

                        request.ModuleLocation = d.ModuleLocation;
                    }
                }

                // At this stage if module location is null, then it is not to be considered for locating a connection point
            }

            var connectionPointRef = FindConnectionPoint(request);

            if (connectionPointRef == null) {
                var otherModuleLocations = new List<LayoutControlModuleLocationComponent?>();

                if (request.ModuleLocation != null && request.Address < 0) {
                    foreach (LayoutControlModuleLocationComponent moduleLocation in LayoutModel.Components<LayoutControlModuleLocationComponent>(request.Phase))
                        if (moduleLocation.Id != request.ModuleLocation.Id && FindConnectionPoint(request.WithAnotherModuleLocation(moduleLocation)) != null)
                            otherModuleLocations.Add(moduleLocation);

                    if (otherModuleLocations.Count == 0 && FindConnectionPoint(request.WithAnotherModuleLocation(null)) != null)
                        otherModuleLocations.Add(null);

                    if (otherModuleLocations.Count > 0 && request.PromptBeforeAddingModule) {
                        // Prompt the user, that there is no space in the current module location, however, there is space
                        // in one or more other module location, at this point he can select to connect to one of those other
                        // module locations, or to create a new module.
                        var d = new AutoConnectDialogs.GetNoSpaceSelection(request.ModuleLocation, otherModuleLocations);

                        EnsureVisible(request);
                        if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                            return null;

                        if (!d.AddNewModule)
                            connectionPointRef = FindConnectionPoint(request.WithAnotherModuleLocation(d.ModuleLocation));
                        else if (connectLayout)
                            request.PromptBeforeAddingModule = false;

                        // If connectionPointRef is null, the user has selected to create a new module
                    }
                }

                if (connectionPointRef == null) {       // An existing connection point could not be found, a new module is to be added
                    List<ControlBus> buses = new();

                    if (request.Bus != null)
                        buses.Add(request.Bus);
                    else if(request.CommandStation != null)
                        buses.AddRange(LayoutModel.ControlManager.Buses.Buses(request.CommandStation));

                    foreach (ControlBus bus in buses) {
                        IList<string> moduleTypeNames = bus.BusType.GetConnectableControlModuleTypeNames(new ControlConnectionPointDestination(request.Component, request.ConnectionDescription));
                        string? moduleTypeName = null;

                        if (moduleTypeNames != null && moduleTypeNames.Count > 0) {
                            ControlModuleLocationBusDefaultInfo? busDefault = null;
                            Guid moduleLocationID = (request.ModuleLocation == null) ? Guid.Empty : request.ModuleLocation.Id;

                            if (request.ModuleLocation != null)
                                busDefault = request.ModuleLocation.Info.Defaults[bus.Id];

                            if (moduleTypeNames.Count == 1) {
                                moduleTypeName = moduleTypeNames[0];

                                if (!connectLayout) {
                                    var d = new AutoConnectDialogs.AnnounceCreateNewModule(LayoutControlManager.GetModuleType(moduleTypeName), request.ModuleLocation);

                                    EnsureVisible(request);
                                    if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                                        return null;
                                }
                            }
                            else if (moduleTypeNames.Count > 1) {
                                string? useThisModuleTypeName = null;

                                if (request.ConnectionDescription.RequiredControlModuleTypeName != null)
                                    useThisModuleTypeName = request.ConnectionDescription.RequiredControlModuleTypeName;
                                else if (busDefault != null && busDefault.DefaultModuleTypeName != null)
                                    useThisModuleTypeName = busDefault.DefaultModuleTypeName;
                                else if (defaultModules.ContainsKey(bus.Id))
                                    useThisModuleTypeName = defaultModules[bus.Id];

                                if (useThisModuleTypeName != null) {
                                    foreach (string mtn in moduleTypeNames)
                                        if (mtn == useThisModuleTypeName) {
                                            moduleTypeName = mtn;
                                            break;
                                        }
                                }

                                if (moduleTypeName == null) {
                                    AutoConnectDialogs.GetModuleType d = new(moduleTypeNames);

                                    EnsureVisible(request);
                                    if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                                        return null;

                                    moduleTypeName = d.ModuleType?.TypeName;

                                    if (d.UseAsDefault && moduleTypeName != null)
                                        defaultModules.Add(bus.Id, moduleTypeName);
                                }
                            }

                            AddControlModuleCommand? addCommand = null;

                            if (request.Address < 0) {      // No address constraint
                                                            // Create the new module
                                if (bus.BusType.Topology == ControlBusTopology.DaisyChain) {
                                    ControlModule? insertBefore = null;
                                    ControlModule? lastModuleInLocation = null;

                                    if (request.ModuleLocation != null) {
                                        foreach (ControlModule module in bus.Modules)
                                            if (module.LocationId == request.ModuleLocation.Id) {
                                                if (lastModuleInLocation == null || module.Address > lastModuleInLocation.Address)
                                                    lastModuleInLocation = module;
                                            }
                                    }

                                    if (lastModuleInLocation != null) {
                                        int addressOfLastModuleInBus = -1;

                                        // Find the last module in the bus
                                        foreach (ControlModule module in bus.Modules)
                                            if (module.Address > addressOfLastModuleInBus)
                                                addressOfLastModuleInBus = module.Address;

                                        if (addressOfLastModuleInBus > lastModuleInLocation.Address) {
                                            // Prompt the user whether he would like to "insert" the module in this location
                                            // or to add it to the end of the chain.
                                            var d = new AutoConnectDialogs.GetDaisyChainBusAddOrInsert(request.ModuleLocation);

                                            EnsureVisible(request);
                                            if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                                                return null;

                                            if (d.InsertModule) {
                                                foreach (ControlModule module in bus.Modules)
                                                    if (module.Address == lastModuleInLocation.Address + lastModuleInLocation.ModuleType.NumberOfAddresses) {
                                                        insertBefore = module;
                                                        break;
                                                    }
                                            }
                                        }
                                    }

                                    if (moduleTypeName != null) {
                                        if (insertBefore == null)
                                            addCommand = new AddControlModuleCommand(bus, moduleTypeName, moduleLocationID);
                                        else
                                            addCommand = new AddControlModuleCommand(bus, moduleTypeName, moduleLocationID, insertBefore);
                                    }
                                }
                                else {      // Not daisy chain bus
                                    int address = moduleTypeName != null ? bus.AllocateFreeAddress(LayoutControlManager.GetModuleType(moduleTypeName), moduleLocationID) : -1;

                                    if (address < 0) {
                                        EnsureVisible(request);
                                        MessageBox.Show("It is not possible to add any more control modules to this command station.", "Command Station Bus Full",
                                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                        return null;
                                    }
                                    else if(moduleTypeName != null)
                                        addCommand = new AddControlModuleCommand(bus, moduleTypeName, moduleLocationID, address);
                                }
                            }
                            else {      // There is an address constraint
                                if (moduleTypeName != null) {
                                    ControlModuleType moduleType = LayoutControlManager.GetModuleType(moduleTypeName);

                                    if (bus.BusType.Topology == ControlBusTopology.DaisyChain) {
                                        if (request.Bus != null) {
                                            int nextModuleAddress = request.Bus.NextModuleAddress;

                                            // Fill the gap so the added module will have the correct address
                                            while (request.Address > nextModuleAddress) {
                                                command.Add(new AddControlModuleCommand(bus, moduleTypeName, moduleLocationID));
                                                nextModuleAddress += moduleType.NumberOfAddresses;
                                            }

                                            addCommand = new AddControlModuleCommand(bus, moduleTypeName, moduleLocationID);
                                        }
                                    }
                                    else {
                                        int moduleAddress = request.Address - ((request.Address - bus.BusType.FirstAddress) % moduleType.AddressAlignment);

                                        if(moduleTypeName !=null)
                                            addCommand = new AddControlModuleCommand(bus, moduleTypeName, moduleLocationID, moduleAddress);
                                    }
                                }
                            }

                            if (addCommand != null) {
                                try {
                                    command.Add(addCommand);        // Note: this will also execute the command
                                }
                                catch (Exception ex) {
                                    EnsureVisible(request);
                                    MessageBox.Show(ex.Message, "Unable to add module", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return null;
                                }
                            }

                            break;      // Module was created, stop scanning the bus
                        }   // found suitable module types for this bus
                    }   // Command station bus scanning loop

                    connectionPointRef = FindConnectionPoint(request);
                }   // Creating new module
            }

            if (connectionPointRef != null && connectionPointRef.Module != null) {
                ConnectComponentToControlConnectionPointCommand connectCommand = new(connectionPointRef.Module, connectionPointRef.Index, request.Component,
                    request.ConnectionDescription.Name, request.ConnectionDescription.DisplayName);

                // Do the connect
                command.Add(connectCommand);

                if (request.SetUserActionRequired)
                    command.Add(new SetControlUserActionRequiredCommand(connectionPointRef, true));

                if (request.SetProgrammingRequired)
                    command.Add(new SetControlAddressProgrammingRequiredCommand(connectionPointRef.Module, true));
            }

            if (command.Count > 0)
                LayoutController.Do(command);

            return connectionPointRef?.ConnectionPoint;
        }

        private void EnsureVisible(ControlAutoConnectRequest request) {
            if (request.Component is ModelComponent component)
                Dispatch.Call.EnsureComponentVisible(LayoutController.ActiveFrameWindow.Id, component);
        }

        private double Distance(IModelComponent component1, IModelComponent component2) {
            double dx = component1.Spot.Location.X - component2.Spot.Location.X;
            double dy = component1.Spot.Location.Y - component2.Spot.Location.Y;

            return Math.Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// Find the module location that is "nearest" the component to be connected. Null is returned if no module location
        /// component is found in the same area as the component
        /// </summary>
        /// <param name="component">The component</param>
        /// <returns>module location component or null</returns>
        private LayoutControlModuleLocationComponent? FindNearestModuleLocation(LayoutPhase phase, IModelComponentConnectToControl component) {
            var areaModuleLocations = LayoutModel.Components<LayoutControlModuleLocationComponent>(phase, c => c.Spot.Area == component.Spot.Area);

            return areaModuleLocations.Any()
                ? areaModuleLocations.Aggregate((nearest, moduleLocation) => Distance(moduleLocation, component) < Distance(nearest, component) ? moduleLocation : nearest)
                : null;
        }

        private ControlConnectionPointReference? FindConnectionPoint(ControlAutoConnectRequest request) {
            if (request.Address >= 0) {
                if (request.Bus == null)
                    throw new LayoutException("Connection type (bus) must be provided when connecting to specific address");

                var module = request.Bus.GetModuleUsingAddress(request.Address);

                if (module == null)
                    return null;            // Connection point not found, module need to be added

                int index;

                if (request.Bus.BusType.AddressingMethod == ControlAddressingMethod.DirectConnectionPointAddressing) {
                    index = request.Address - module.Address;
                    if (module.ModuleType.ConnectionPointsPerAddress > 1)
                        index = (index * module.ModuleType.ConnectionPointsPerAddress) + request.State;
                }
                else
                    index = request.Index;

                ControlConnectionPointReference result = new(module, index);

                if (result.IsConnected)
                    throw new LayoutException(request.Component, "You cannot connect this component to address " + module.ModuleType.GetConnectionPointAddressText(module.ModuleType, module.Address, index) + ", this address is connected to another component");

                return result;
            }
            else {
                List<ControlBus> buses = new();
                ControlConnectionPointDestination connectionDestination = new(request.Component, request.ConnectionDescription);

                if (request.Bus != null)
                    buses.Add(request.Bus);
                else if(request.CommandStation != null)
                    buses.AddRange(LayoutModel.ControlManager.Buses.Buses(request.CommandStation));

                if (request.CommandStation != null) {
                    foreach (ControlBus bus in LayoutModel.ControlManager.Buses.Buses(request.CommandStation)) {
                        // Check that at least one control module is recommended for this bus for this type of component. Otherwise
                        // there is no point in scanning this bus
                        if (bus.BusType.GetConnectableControlModuleTypeNames(connectionDestination).Count > 0) {
                            foreach (ControlModule module in bus.Modules) {
                                if (request.ModuleLocation == null || module.LocationId == request.ModuleLocation.Id) {
                                    for (int index = 0; index < module.NumberOfConnectionPoints; index++)
                                        if (module.ConnectionPoints.CanBeConnected(connectionDestination, index))
                                            return new ControlConnectionPointReference(module, index);
                                }
                            }
                        }
                    }
                }

                return null;
            }
        }

        #endregion

        #region Connect Layout Handling

        #region Data structures

        private struct LayoutConnectEntry {
            private readonly List<IModelComponentConnectToControl> components;

            public LayoutConnectEntry(LayoutControlModuleLocationComponent? moduleLocation) {
                this.ModuleLocation = moduleLocation;
                components = new List<IModelComponentConnectToControl>();
            }

            public void Add(IModelComponentConnectToControl component) {
                components.Add(component);
            }

            private static double GetDistance(IModelComponent component1, IModelComponent component2) {
                int dx = component1.Location.X - component2.Location.X;
                int dy = component1.Location.Y - component2.Location.Y;

                return Math.Sqrt((dx * dx) + (dy * dy));
            }

            private int FindIndexOfNearest(IModelComponent aComponent, int startIndex) {
                int nearestIndex = -1;
                double nearestDistance = 0;

                for (int i = startIndex; i < components.Count; i++) {
                    IModelComponent component = components[i];
                    double distance = GetDistance(aComponent, component);

                    if (nearestIndex < 0 || distance < nearestDistance) {
                        nearestIndex = i;
                        nearestDistance = distance;
                    }
                }

                Debug.WriteLine("The closed component to " + aComponent.FullDescription + " is " + components[nearestIndex].FullDescription + " nearest distance " + nearestDistance);

                return nearestIndex;
            }

            private void Swap(int i1, int i2) {
                (components[i2], components[i1]) = (components[i1], components[i2]);
            }

            public void Sort() {
                if (ModuleLocation != null)
                    Swap(0, FindIndexOfNearest(ModuleLocation, 0));     // So first component is the closest to the module location

                for (int i = 0; i < components.Count - 1; i++)
                    Swap(i, FindIndexOfNearest(components[i], i + 1));
            }

            private ControlConnectionPointDestination GetConnectionPointDestinationRecommendation(IModelComponentConnectToControl component) {
                foreach (ModelComponentControlConnectionDescription connectionDescription in component.ControlConnectionDescriptions)
                    if (!LayoutModel.ControlManager.ConnectionPoints.IsConnected(component.Id, connectionDescription.Name))
                        return new ControlConnectionPointDestination(component, connectionDescription);

                throw new ArgumentException("Called for component that is fully connected");
            }

            /// <summary>
            /// Return the next component (see remark) to be connected. It should be the nearest connection point that can be connected
            /// to the previous control module or if it is full, the nearest one that connects to the same bus, or if none can be found
            /// the nearest the control module location, or if non is defined, the first non connected componnent, or null if all
            /// components are connected.
            /// </summary>
            /// <remarks>
            /// TODO: The return value should be a component and a connection since component may have more than one
            /// connection and may have more than one connection type (for example turnout with control and feedback)
            /// </remarks>
            /// <param name="previousConnectionPoint">The previouslly connected point or null if none</param>
            /// <returns>The next component to connect</returns>
            public ControlConnectionPointDestination? GetNextComponent(ControlConnectionPoint? previousConnectionPoint) {
                if (previousConnectionPoint == null) {
                    if (components.Count == 0)
                        return null;

                    return ModuleLocation == null
                        ? GetConnectionPointDestinationRecommendation(components[0])
                        : GetConnectionPointDestinationRecommendation(components[FindIndexOfNearest(ModuleLocation, 0)]);
                }
                else {
                    // Check if the module that the previous component was connected to is full or not
                    ControlModule previousModule = previousConnectionPoint.Module;
                    ControlBus previousBus = previousConnectionPoint.Module.Bus;
                    bool previousModuleHasSpace = false;

                    foreach (var cp in previousModule.ConnectionPoints)
                        if (cp == null || !cp.IsConnected) {
                            previousModuleHasSpace = true;
                            break;
                        }

                    ControlConnectionPointDestination? nearestInModule = null;
                    ControlConnectionPointDestination? nearestInBus = null;

                    double nearestInModuleDistance = 0;
                    double nearestInBusDistance = 0;

                    foreach (IModelComponentConnectToControl component in Components) {
                        if (!component.FullyConnected && previousConnectionPoint.Component != null) {
                            ControlConnectionPointDestination freeConnection = GetConnectionPointDestinationRecommendation(component);

                            IList<string> moduleTypeNames = previousBus.BusType.GetConnectableControlModuleTypeNames(freeConnection);
                            double distance = GetDistance(previousConnectionPoint.Component, component);

                            if (moduleTypeNames.Count > 0) {        // This component can connect to the same bus
                                if (nearestInBus == null || distance < nearestInBusDistance) {
                                    nearestInBus = freeConnection;
                                    nearestInBusDistance = distance;
                                }

                                if (previousModuleHasSpace) {
                                    foreach (string moduleTypeName in moduleTypeNames) {
                                        if (moduleTypeName == previousModule.ModuleTypeName) {  // This component can connect to the same module
                                            if (nearestInModule == null || distance < nearestInModuleDistance) {
                                                nearestInModule = freeConnection;
                                                nearestInModuleDistance = distance;
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (nearestInModule != null)
                        return nearestInModule;
                    else if (nearestInBus != null)
                        return nearestInBus;
                    else {
                        if (ModuleLocation == null) {
                            foreach (IModelComponentConnectToControl component in Components)
                                if (!component.FullyConnected)
                                    return GetConnectionPointDestinationRecommendation(component);
                            return null;            // Non can be found
                        }
                        else {
                            IModelComponentConnectToControl? nearestComponent = null;
                            double nearestDistance = 0;

                            foreach (IModelComponentConnectToControl component in Components) {
                                if (!component.FullyConnected) {
                                    double distance;

                                    distance = GetDistance(ModuleLocation, component);
                                    if (nearestComponent == null || distance < nearestDistance) {
                                        nearestDistance = distance;
                                        nearestComponent = component;
                                    }
                                }
                            }

                            return nearestComponent != null ? GetConnectionPointDestinationRecommendation(nearestComponent) : null;
                        }
                    }
                }
            }

            public LayoutControlModuleLocationComponent? ModuleLocation { get; }

            public IList<IModelComponentConnectToControl> Components => components.AsReadOnly();
        }

        #endregion

        [LayoutEvent("connect-layout-to-control-request")]
        private void ConnectLayoutToControlRequest(LayoutEvent _) {
            using AutoConnectDialogs.GetConnectLayoutOptions d = new();

            if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                return;

            try {
                bool connectAllLayout = d.ConnectAllLayout;
                bool setUserActionRequired = d.SetUserActionRequired;
                bool setProgrammingRequired = d.SetProgrammingRequired;
                LayoutPhase phase = d.Phase;
                LayoutCompoundCommand command = new("Connect layout", true);

                if (connectAllLayout)
                    DisconnectLayout(command);

                foreach (LayoutModelArea area in LayoutModel.Areas) {
                    IEnumerable<LayoutControlModuleLocationComponent> moduleLocations = LayoutModel.Components<LayoutControlModuleLocationComponent>(phase, c => c.Spot.Area == area);
                    Dictionary<Guid, LayoutConnectEntry> moduleLocationMap = new();

                    if (moduleLocations != null && moduleLocations.Any()) {
                        foreach (IModelComponentConnectToControl component in LayoutModel.Components<IModelComponentConnectToControl>(phase, c => c.Spot.Area == area)) {
                            if (!component.FullyConnected) {
                                var moduleLocation = FindNearestModuleLocation(phase, component);

                                if (moduleLocation != null) {
                                    LayoutConnectEntry entry;

                                    if (!moduleLocationMap.ContainsKey(moduleLocation.Id)) {
                                        entry = new LayoutConnectEntry(moduleLocation);

                                        moduleLocationMap.Add(moduleLocation.Id, entry);
                                    }
                                    else
                                        entry = moduleLocationMap[moduleLocation.Id];

                                    entry.Add(component);
                                }
                            }
                        }
                    }
                    else {
                        LayoutConnectEntry entry = new(null);

                        moduleLocationMap.Add(Guid.Empty, entry);
                        foreach (IModelComponentConnectToControl component in LayoutModel.Components<IModelComponentConnectToControl>(phase, c => c.Spot.Area == area))
                            if (!component.FullyConnected)
                                entry.Add(component);
                    }

                    ControlAutoConnectRequest request = new(phase);
                    bool aborted = false;
                    LayoutSelection connectedComponentSelection = new();

                    request.SetUserActionRequired = setUserActionRequired;
                    request.SetProgrammingRequired = setProgrammingRequired;

                    connectedComponentSelection.Display(new LayoutSelectionLook(System.Drawing.Color.SteelBlue));

                    foreach (LayoutConnectEntry entry in moduleLocationMap.Values) {
                        ControlConnectionPointDestination? nextConnection = null;
                        ControlConnectionPoint? lastConnectionPoint = null;

                        request.ModuleLocation = entry.ModuleLocation;
                        while ((nextConnection = entry.GetNextComponent(lastConnectionPoint)) != null) {
                            request.OptionalComponent = nextConnection.Component;
                            request.OptionalConnectionDescription = nextConnection.ConnectionDescription;

                            connectedComponentSelection.Add(request.Component);

                            if ((lastConnectionPoint = DoAutoConnect(request, true, command)) == null)
                                aborted = true;

                            connectedComponentSelection.Clear();

                            if (aborted)
                                break;
                        }

                        if (aborted)
                            break;
                    }

                    if (aborted) {
                        command.Undo();
                        break;
                    }
                    else
                        LayoutController.Do(command);
                }
            }
            catch (LayoutException ex) {
                MessageBox.Show(LayoutController.ActiveFrameWindow, "Error when connecting layout: " + ex.Message, "Connect Layout Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisconnectLayout(LayoutCompoundCommand command) {
            foreach (IModelComponentIsCommandStation commandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All)) {
                foreach (ControlBus bus in LayoutModel.ControlManager.Buses.Buses(commandStation)) {
                    foreach (ControlModule module in bus.Modules) {
                        for (int index = 0; index < module.NumberOfConnectionPoints; index++) {
                            if (module.ConnectionPoints.IsConnected(index)) {
                                DisconnectComponentFromConnectionPointCommand disconnectCommand = new(module.ConnectionPoints[index]);

                                command.Add(disconnectCommand);
                            }
                        }

                        RemoveControlModuleCommand removeCommand = new(module);
                        command.Add(removeCommand);
                    }
                }
            }
        }

        #endregion

        #region Simulate Command station input event

        [DispatchTarget(Order = 1000)]
        [DispatchFilter("InDesignTimeActivation")]
        private void ToolsMenuOpenRequest(MenuOrMenuItem toolsMenu) {
            // Check if it make sense to add this menu entry
            if (LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All).Any(commandStation => commandStation.DesignTimeLayoutActivationSupported)) {
                toolsMenu.Items.Add("Emulate &Command station Event...", null,
                    (object? sender, EventArgs ea) => new Dialogs.SimulateCommandStationInputEvent().ShowDialog(LayoutController.ActiveFrameWindow));
            }
        }

        #endregion

        #region Event Handlers

        #region "Add ons" and referential integrity to non-control components

        [LayoutEvent("prepare-for-component-remove-command", SenderType = typeof(LayoutControlModuleLocationComponent))]
        private void PrepareForLayoutControlModuleLocationComponentRemove(LayoutEvent e) {
            var component = Ensure.NotNull<LayoutControlModuleLocationComponent>(e.Sender);
            var deleteCommand = Ensure.NotNull<LayoutCompoundCommand>(e.Info);

            ControlModule[] modulesAtLocation = LayoutModel.ControlManager.GetModulesAtLocation(component);

            // Add commands to assign all components that were at this location to no location
            foreach (ControlModule module in modulesAtLocation)
                deleteCommand.Add(new AssignControlModuleLocationCommand(module, Guid.Empty));
        }

        [LayoutEvent("prepare-for-component-remove-command", SenderType = typeof(IModelComponentConnectToControl))]
        private void PrepareForConnectableComponentRemove(LayoutEvent e) {
            var component = Ensure.NotNull<IModelComponentConnectToControl>(e.Sender);
            var connections = LayoutModel.ControlManager.ConnectionPoints[component];
            var deleteCommand = Ensure.NotNull<LayoutCompoundCommand>(e.Info);

            if (connections != null)
                foreach (ControlConnectionPoint connectionPoint in connections)
                    deleteCommand.Add(new DisconnectComponentFromConnectionPointCommand(connectionPoint));
        }

        [DispatchTarget]
        private void OnRemovedFromModel_AvoidConnectingRemovedComponents([DispatchFilter] IModelComponentConnectToControl component) {
            var pendingConnectComponent = (ControlConnectionPointDestination?)EventManager.Event(new LayoutEvent("get-component-to-control-connect", this));

            if (pendingConnectComponent != null && component.Id == pendingConnectComponent.Component.Id)
                EventManager.Event(new LayoutEvent("cancel-component-to-control-connect", component));
        }

        [DispatchTarget]
        private void OnRemovedFromModel_CommandStation([DispatchFilter] IModelComponentIsCommandStation commandStation) {
            foreach (LayoutControlModuleLocationComponent controlModuleLocation in LayoutModel.Components<LayoutControlModuleLocationComponent>(LayoutPhase.All))
                if (controlModuleLocation.Info.CommandStationId == commandStation.Id)
                    controlModuleLocation.Info.CommandStationId = Guid.Empty;
        }

        #endregion

        #region Connectible components context menu

        [DispatchTarget]
        private bool IncludeInComponentContextMenu([DispatchFilter] IModelComponentConnectToControl component) => true;

        [DispatchTarget(Order = 600)]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries_Design(Guid frameWindowId, [DispatchFilter] IModelComponentConnectToControl component, MenuOrMenuItem menu) {
            if (component.ControlConnectionDescriptions.Count > 0) {
                if (menu.Items.Count > 0)
                    menu.Items.Add(new ToolStripSeparator());

                if (!component.FullyConnected) {
                    List<IControlConnectionPointDestinationReceiverDialog> dialogs = new();
                    EventManager.Event(new LayoutEvent("query-learn-layout-pick-component-dialog", component, dialogs));

                    if (dialogs.Count > 0) {
                        IControlConnectionPointDestinationReceiverDialog dialog = dialogs[0];

                        List<ModelComponentControlConnectionDescription> applicableConnectionDescriptions = new();

                        foreach (ModelComponentControlConnectionDescription connectionDescription in component.ControlConnectionDescriptions)
                            if (dialog.Bus.BusType.CanBeConnected(new ControlConnectionPointDestination(component, connectionDescription)))
                                applicableConnectionDescriptions.Add(connectionDescription);

                        if (applicableConnectionDescriptions.Count == 1) {
                            ControlConnectionPointDestination connectionDestination = new(component, applicableConnectionDescriptions[0]);

                            var item = new LayoutMenuItem($"Connect {applicableConnectionDescriptions[0].DisplayName} to address {dialog.DialogName(connectionDestination)}", null,
                                (sender, eArgs) => dialog.AddControlConnectionPointDestination(connectionDestination)) {
                                DefaultItem = true
                            };
                            menu.Items.Add(item);
                        }
                        else if (applicableConnectionDescriptions.Count > 1) {
                            var item = new LayoutMenuItem("Connect control module");

                            foreach (ModelComponentControlConnectionDescription connectionDescription in applicableConnectionDescriptions) {
                                ControlConnectionPointDestination connectionDestination = new(component, connectionDescription);

                                item.DropDownItems.Add(new LayoutMenuItem($"{connectionDescription.DisplayName} to address {dialog.DialogName(connectionDestination)}", null,
                                (sender, eArgs) => dialog.AddControlConnectionPointDestination(connectionDestination)));
                            }

                            menu.Items.Add(item);
                        }
                    }

                    var pendingComponentConnect = (ControlConnectionPointDestination?)EventManager.Event(new LayoutEvent("get-component-to-control-connect", this));
                    var pendingConnectionPointConnect = (ControlConnectionPointReference?)EventManager.Event(new LayoutEvent("get-control-to-component-connect", this));

                    if (pendingComponentConnect != null || pendingConnectionPointConnect != null) {
                        if (pendingConnectionPointConnect != null) {
                            var possibleConnections = pendingConnectionPointConnect.GetPossibleConnectionDescriptions(component, pendingConnectionPointConnect.Index);

                            if (possibleConnections != null && pendingConnectionPointConnect.Module != null) {
                                if (possibleConnections.Count == 0) {
                                    var item = new LayoutMenuItem($"Connect control module {pendingConnectionPointConnect.Module.ConnectionPoints.GetLabel(pendingConnectionPointConnect.Index, true)}") {
                                        Enabled = false
                                    };
                                    menu.Items.Add(item);
                                }
                                else if (possibleConnections.Count == 1) {
                                    menu.Items.Add(new ConnectControlConnectionPointMenuItem($"Connect control module {pendingConnectionPointConnect.Module.ConnectionPoints.GetLabel(pendingConnectionPointConnect.Index, true)} to {possibleConnections[0].DisplayName}",
                                        pendingConnectionPointConnect, component, possibleConnections[0]));
                                }
                                else {
                                    var item = new LayoutMenuItem($"Connect control module {pendingConnectionPointConnect.Module.ConnectionPoints.GetLabel(pendingConnectionPointConnect.Index, true)} to");

                                    foreach (ModelComponentControlConnectionDescription possibleConnection in possibleConnections)
                                        item.DropDownItems.Add(new ConnectControlConnectionPointMenuItem(possibleConnection.DisplayName, pendingConnectionPointConnect,
                                            component, possibleConnection));

                                    menu.Items.Add(item);
                                }
                            }
                        }

                        if (pendingComponentConnect != null)
                            menu.Items.Add(new CancelPendingConnectMenuItem());
                    }
                    else {
                        List<ModelComponentControlConnectionDescription> possibleConnections = new();

                        foreach (ModelComponentControlConnectionDescription connectionDescription in component.ControlConnectionDescriptions)
                            if (!LayoutModel.ControlManager.ConnectionPoints.IsConnected(component.Id, connectionDescription.Name))
                                possibleConnections.Add(connectionDescription);

                        if (possibleConnections.Count == 1) {
                            menu.Items.Add(new ManualConnectComponentMenuItem($"Manually connect control module to {possibleConnections[0].DisplayName}", component, possibleConnections[0]));
                            menu.Items.Add(new AutomaticConnectComponentMenuItem(frameWindowId, $"Automatically connect control module to {possibleConnections[0].DisplayName}", component, possibleConnections[0]));
                        }
                        else {
                            var manualConnectItem = new LayoutMenuItem("Manually connect control module to");
                            var automaticConnectItem = new LayoutMenuItem("Automatically connect control module to");

                            foreach (ModelComponentControlConnectionDescription connectionDescription in possibleConnections) {
                                manualConnectItem.DropDownItems.Add(new ManualConnectComponentMenuItem(connectionDescription.DisplayName, component, connectionDescription));
                                automaticConnectItem.DropDownItems.Add(new AutomaticConnectComponentMenuItem(frameWindowId, connectionDescription.DisplayName, component, connectionDescription));
                            }

                            menu.Items.Add(manualConnectItem);
                            menu.Items.Add(automaticConnectItem);
                        }
                    }
                }

                if (component.IsConnected) {
                    var connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component];

                    if (connectionPoints != null) {
                        if (connectionPoints.Count == 1) {
                            ControlConnectionPoint connectionPoint = connectionPoints[0];

                            menu.Items.Add(new ShowConnectionPointMenuItem(frameWindowId, $"Show control module connected to {connectionPoint.DisplayName}", connectionPoint));
                            menu.Items.Add(new DisconnectConnectionPointMenuItem($"Disconnect {connectionPoint.DisplayName} from control module", connectionPoint));
                        }
                        else {
                            var showConnectionPointItem = new LayoutMenuItem("Show control module connected to");
                            var disconnectMenuItem = new LayoutMenuItem("Disconnect control module connected to");

                            foreach (ControlConnectionPoint connectionPoint in connectionPoints) {
                                showConnectionPointItem.DropDownItems.Add(new ShowConnectionPointMenuItem(frameWindowId, connectionPoint.DisplayName, connectionPoint));
                                disconnectMenuItem.DropDownItems.Add(new DisconnectConnectionPointMenuItem(connectionPoint.DisplayName, connectionPoint));
                            }

                            menu.Items.Add(showConnectionPointItem);
                            menu.Items.Add(disconnectMenuItem);
                        }
                    }
                }
            }
        }

        [DispatchTarget(Order =200)]
        [DispatchFilter(Type = "InOperationMode")]
        private void AddComponentContextMenuEntries_Operation(Guid frameWindowId, [DispatchFilter] IModelComponentConnectToControl component, MenuOrMenuItem menu) {
            var connectionPoints = LayoutModel.ControlManager.ConnectionPoints[component];

            if (connectionPoints != null) {
                foreach (ControlConnectionPoint connectionPoint in connectionPoints)
                    menu.Items.Add(new ShowConnectionPointMenuItem(frameWindowId, $"Show {connectionPoint.DisplayName} control module", connectionPoint));
            }
        }

        #endregion

        #region Connectible components default action

        [LayoutEvent("query-editing-default-action", SenderType = typeof(IModelComponentConnectToControl))]
        private void ConnectableComponentQueryEditingDefaultAction(LayoutEvent e) {
            var connectableComponent = Ensure.NotNull<IModelComponentConnectToControl>(e.Sender);

            if (!connectableComponent.FullyConnected) {
                List<IControlConnectionPointDestinationReceiverDialog> dialogs = new();
                EventManager.Event(new LayoutEvent("query-learn-layout-pick-component-dialog", connectableComponent, dialogs));

                if (dialogs.Count > 0)
                    e.Info = true;
            }
        }

        [DispatchTarget]
        private void EditingDefaultActionCommand([DispatchFilter] IModelComponentConnectToControl component, LayoutHitTestResult hitTestResult) {
            ModelComponentControlConnectionDescription? feedbackConnection = null;

            foreach (ModelComponentControlConnectionDescription connectionDescription in component.ControlConnectionDescriptions)
                if (connectionDescription.Name.StartsWith("Feedback")) {
                    feedbackConnection = connectionDescription;
                    break;
                }

            if (feedbackConnection.HasValue) {
                List<IControlConnectionPointDestinationReceiverDialog> dialogs = new();
                EventManager.Event(new LayoutEvent("query-learn-layout-pick-component-dialog", component, dialogs));

                if (dialogs.Count > 0)
                    dialogs[0].AddControlConnectionPointDestination(new ControlConnectionPointDestination(component, feedbackConnection.Value));
            }
        }

        #endregion

        #region Command Station context menu

        [DispatchTarget]
        [DispatchFilter("InDesignMode")]
        private bool IncludeInComponentContextMenu([DispatchFilter] IModelComponentIsCommandStation component) => true;

        [DispatchTarget(Order = 600)]
        [DispatchFilter(Type = "InDesignMode")]
        private void AddComponentContextMenuEntries(Guid frameWindowId, [DispatchFilter] IModelComponentIsCommandStation component, MenuOrMenuItem menu) {

            // Check if this command station can upgrade any other command station
            var replacableCommandStations = new List<IModelComponentIsCommandStation>();
            foreach (IModelComponentIsCommandStation otherCommandStation in LayoutModel.Components<IModelComponentIsCommandStation>(LayoutPhase.All)) {
                if (otherCommandStation.Id != component.Id) {
                    // Check that all other command station buses exists in this command station, and that they are empty
                    bool busCanBeMoved = true;

                    foreach (ControlBus otherStationBus in LayoutModel.ControlManager.Buses.Buses(otherCommandStation)) {
                        if (otherStationBus.Modules.Count > 0) {
                            var thisBus = LayoutModel.ControlManager.Buses.GetBus(component, otherStationBus.BusTypeName);

                            if (thisBus == null || thisBus.Modules.Count != 0) {
                                busCanBeMoved = false;
                                break;
                            }
                        }
                    }

                    // Check that the power sources are compatible
                    bool powerSourcesAreCompatible = true;

                    foreach (ILayoutPowerOutlet otherPowerSource in otherCommandStation.PowerOutlets) {
                        bool powerSourceFound = false;

                        foreach (ILayoutPowerOutlet thisPowerSource in component.PowerOutlets) {
                            if (otherPowerSource.OutletDescription == thisPowerSource.OutletDescription) {
                                powerSourceFound = true;
                                break;
                            }
                        }

                        if (!powerSourceFound) {
                            powerSourcesAreCompatible = false;
                            break;
                        }
                    }

                    if (busCanBeMoved && powerSourcesAreCompatible)
                        replacableCommandStations.Add(otherCommandStation);
                }
            }

            if (replacableCommandStations.Count == 1) {
                var otherCommandStation = replacableCommandStations[0];

                menu.Items.Add(new ReplaceCommandStationMenuItem(component, otherCommandStation, $"This command station replaces command station '{otherCommandStation.NameProvider.Name}'"));
            }
            else if (replacableCommandStations.Count > 1) {
                var item = new LayoutMenuItem("This command station replace");

                foreach (IModelComponentIsCommandStation otherCommandStation in replacableCommandStations)
                    item.DropDownItems.Add(new ReplaceCommandStationMenuItem(component, otherCommandStation, otherCommandStation.NameProvider.Name));

                menu.Items.Add(item);
            }
        }

        #endregion

        #region Layout Control viewer handlers

        #region Click to add module

        [DispatchTarget]
        private void ControlModuleDefaultAction([DispatchFilter] DrawControlClickToAddModule drawObject) {
            ControlBus bus = drawObject.Bus;

            EventManager.Event(bus.BusType.ClickToAddModuleEventName, drawObject);
        }

        [LayoutEvent("control-module-click-to-add")]
        private void DefaultClickToAddAction(LayoutEvent e) {
            DrawControlClickToAddModule drawObject = Ensure.NotNull<DrawControlClickToAddModule>(e.Sender);
            ControlBus bus = drawObject.Bus;
            IList<ControlModuleType> moduleTypes = bus.BusType.ModuleTypes;

            if (moduleTypes.Count == 1) {
                AddControlModuleMenuItem m = new(drawObject.Viewer.ModuleLocationID, bus, moduleTypes[0]);

                m.Do();
            }
            else if (moduleTypes.Count > 1) {
                var menu = new ContextMenuStrip();

                foreach (ControlModuleType moduleType in moduleTypes)
                    menu.Items.Add(new AddControlModuleMenuItem(drawObject.Viewer.ModuleLocationID, bus, moduleType));

                menu.Show(drawObject.Viewer, drawObject.Viewer.PointToClient(Control.MousePosition));
            }
        }

        #endregion

        #region Module

        [DispatchTarget]
        private void AddControlModuleEditingContextMenuEntries([DispatchFilter] DrawControlModule drawModule, MenuOrMenuItem menu) {
            var module = drawModule.Module;

            drawModule.Selected = true;

            if (module.Bus.BusType.Topology == ControlBusTopology.DaisyChain) {
                var insertModuleMenuItem = new InsertControlModuleMenuItem(drawModule.Viewer.ModuleLocationID, module);

                menu.Items.Add(insertModuleMenuItem);

                // Check if bus is full.
                if (module.Bus.GetModuleUsingAddress(module.Bus.BusType.LastAddress) != null)
                    insertModuleMenuItem.Enabled = false;

                menu.Items.Add(new MoveControlModuleInDaisyChainMenuItem(module, -1, "Move module &Up the chain"));
                menu.Items.Add(new MoveControlModuleInDaisyChainMenuItem(module, 1, "Move module &Down the chain"));
            }
            else {
                if (module.Bus.BusType.Topology != ControlBusTopology.Fixed && module.Bus.BusType.CanChangeAddress)
                    menu.Items.Add(new SetControlModuleAddressMenuItem(drawModule.Viewer.ModuleLocationID, module));

                var programmers = LayoutModel.Components<IModelComponentCanProgramLocomotives>(LayoutPhase.Operational);

                if (programmers.Any()) {
                    var controlProgrammingActions = new List<ControlProgrammingAction>();
                    EventManager.Event(new LayoutEvent<ControlModule, IList<ControlProgrammingAction>>("get-control-module-programming-actions", module, controlProgrammingActions));

                    if (programmers.Count() == 1) {
                        foreach (var programmingAction in controlProgrammingActions)
                            menu.Items.Add(programmingAction.Description, null, (s, ea) => programmingAction.DoProgramming(programmers.First(), module));
                    }
                    else {
                        foreach (var programmingAction in controlProgrammingActions) {
                            var programAddress = new LayoutMenuItem(programmingAction.Description);

                            foreach (var programmer in programmers)
                                programAddress.DropDownItems.Add("Using " + programmer.NameProvider.Name, null, (s, ea) => programmingAction.DoProgramming(programmer, module));

                            menu.Items.Add(programAddress);
                        }
                    }
                }
            }

            if (menu.Items.Count > 0)
                menu.Items.Add(new ToolStripSeparator());

            menu.Items.Add(new SetModuleLocationMenuItem(drawModule.Viewer.ModuleLocationID == Guid.Empty, module));

            if(drawModule.Module.Bus.BusType.CanChangeLabel)
                menu.Items.Add(new SetModuleLabelMenuItem(module));

            menu.Items.Add(new ToggleUserActionRequiredMenuItem(module));

            if (module.DecoderType != null)
                menu.Items.Add(new LayoutMenuItem(module.AddressProgrammingRequired ? "Clear address programming required" : "Set address programming required", null,
                    (s, ea) => LayoutController.Do(new SetControlAddressProgrammingRequiredCommand(module, !module.AddressProgrammingRequired))));

            if (module.Bus.BusType.Topology != ControlBusTopology.Fixed) {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(new RemoveControlModuleMenuItem(module));
            }
        }

        [DispatchTarget]
        private void ControlModuleDefaultAction([DispatchFilter] DrawControlModule drawObject) {
            drawObject.Selected = !drawObject.Selected;
        }

        [LayoutEvent("control-module-removed")]
        private void ControlModuleRemoved(LayoutEvent e) {
            var module = Ensure.NotNull<ControlModule>(e.Sender);
            var pendingConnectionPointConnect = (ControlConnectionPointReference?)EventManager.Event(new LayoutEvent("get-control-to-component-connect", this));

            if (pendingConnectionPointConnect != null && pendingConnectionPointConnect.Module?.Id == module.Id)
                EventManager.Event(new LayoutEvent("cancel-control-to-component-connect", this));
        }

        [LayoutEvent("get-control-module-programming-actions")]
        private void GetControlModuleProgrammingActions(LayoutEvent e) {
            var module = Ensure.NotNull<ControlModule>(e.Sender);
            var list = Ensure.NotNull<IList<ControlProgrammingAction>>(e.Info);

            if (LayoutAction.HasAction("set-address", module)) {
                list.Add(new ControlProgrammingAction("Program address", (aProgrammer, aModule) => {
                    var programmingState = new ControlModuleProgrammingState(aModule, aProgrammer) {
                        ProgrammingActions = new LayoutActionContainer<ControlModule>(aModule)
                    };

                    var setAddressAction = programmingState.ProgrammingActions.Add("set-address");

                    Debug.Assert(programmingState.ProgrammingActions.Count() == 1);

                    // Note: If there is no edit-action-settings, then the result will not be bool.
                    object? r = EventManager.Event(new LayoutEventResultValueType<object, ILayoutAction, bool>("edit-action-settings", module, setAddressAction));

                    if (r is not bool || (bool)r) {
                        programmingState.ProgrammingActions.PrepareForProgramming();

                        var d = new Dialogs.ControlModuleProgrammingProgressDialog(programmingState, doProgramming: async () => {
                            if (LayoutController.Instance.BeginDesignTimeActivation()) {
                                await Dispatch.Call.DoCommandStationActions(aProgrammer, programmingState.ProgrammingActions, false);
                                LayoutController.Instance.EndDesignTimeActivation();
                                await LayoutController.Instance.EnterDesignModeRequest();
                            }
                            else {
                                foreach (var action in programmingState.ProgrammingActions)
                                    action.Status = ActionStatus.Failed;
                            }
                        });

                        d.ShowDialog();
                    }
                }));
            }
        }
        #endregion

        #region Connection Point

        [DispatchTarget]
        private void ControlModuleDefaultAction([DispatchFilter] DrawControlConnectionPoint drawObject) {
            var connectWhat = (ControlConnectionPointDestination?)EventManager.Event(new LayoutEvent("get-component-to-control-connect", this));

            if (connectWhat != null && drawObject.Module.ConnectionPoints.CanBeConnected(connectWhat, drawObject.Index)) {
                LayoutCompoundCommand command = new("Connect " + connectWhat.ConnectionDescription.Name + " to control module");

                ConnectComponentToControlConnectionPointCommand connectCommand = new(drawObject.Module, drawObject.Index, connectWhat.Component,
                    connectWhat.ConnectionDescription.Name, connectWhat.ConnectionDescription.DisplayName);

                EventManager.Event(new LayoutEvent("cancel-component-to-control-connect", this));

                command.Add(connectCommand);
                command.Add(new SetControlUserActionRequiredCommand(new ControlConnectionPointReference(drawObject.Module, drawObject.Index), true));

                LayoutController.Do(command);

                drawObject.Viewer.Cursor = drawObject.Cursor;
                drawObject.Selected = true;
            }
            else if (drawObject.Module.ConnectionPoints.IsConnected(drawObject.Index))
                drawObject.Selected = !drawObject.Selected;
        }

        [DispatchTarget]
        private void AddControlModuleEditingContextMenuEntries([DispatchFilter] DrawControlConnectionPoint drawObject, MenuOrMenuItem menu) {

            if (drawObject.Module.ConnectionPoints.IsConnected(drawObject.Index))
                menu.Items.Add(new DisconnectComponentMenuItem(drawObject.Module.ConnectionPoints[drawObject.Index]));
            else {
                var pendingConnectionPointConnect = (ControlConnectionPointReference?)EventManager.Event(new LayoutEvent("get-control-to-component-connect", this));

                if (pendingConnectionPointConnect != null)
                    menu.Items.Add(new CancelPendingConnectionPointConnectMenuItem());
                else
                    menu.Items.Add(new ConnectConnectionPointMenuItem(new ControlConnectionPointReference(drawObject.Module, drawObject.Index)));
            }

            if (drawObject.Module.ConnectionPoints.Usage(drawObject.Index) == ControlConnectionPointUsage.Output)
                menu.Items.Add(new LayoutMenuItem("&Test", null, (sender, ea) => 
                    Dispatch.Call.TestLayoutObjectRequest(LayoutController.ActiveFrameWindow.Id, new ControlConnectionPointReference(drawObject.Module, drawObject.Index))));

            menu.Items.Add(new ToggleUserActionRequiredMenuItem(new ControlConnectionPointReference(drawObject.Module, drawObject.Index)));
        }

        #endregion

        #region Bus

        [DispatchTarget]
        private void AddControlModuleEditingContextMenuEntries([DispatchFilter] DrawControlBus drawObject, MenuOrMenuItem menu) {
            var otherProviders = new List<IModelComponentIsBusProvider>();

            foreach (var busProvider in LayoutModel.Components<IModelComponentIsBusProvider>(LayoutPhase.All)) {
                if (drawObject.Bus.BusProviderId != busProvider.Id) {
                    var otherProviderBus = LayoutModel.ControlManager.Buses.GetBus(busProvider, drawObject.Bus.BusTypeName);

                    if (otherProviderBus != null && otherProviderBus.Modules.Count == 0)
                        otherProviders.Add(busProvider);
                }
            }

            if (otherProviders.Count > 0) {
                if (otherProviders.Count == 1)
                    menu.Items.Add(new ConnectBusToAnotherCommandStation(drawObject.Bus, (IModelComponentIsCommandStation)otherProviders[0], "Reconnect bus to command station " + ((IModelComponentIsCommandStation)otherProviders[0]).NameProvider.Name));
                else {
                    var moveItem = new LayoutMenuItem("Reconnect bus to another command station");

                    foreach (IModelComponentIsCommandStation commandStation in otherProviders)
                        moveItem.DropDownItems.Add(new ConnectBusToAnotherCommandStation(drawObject.Bus, commandStation, commandStation.NameProvider.Name));

                    menu.Items.Add(moveItem);
                }
            }

            menu.Items.Add(new ClearAllUserActionRequiredMenuItem(drawObject));
        }

        #endregion

        #region Command station

        [DispatchTarget]
        private void AddComponentInModuleLocationContextMenuEntries(Guid frameWindowId, ModelComponent component, Guid ModuleLocationId, MenuOrMenuItem menu) {
            Dispatch.Call.AddComponentContextMenuEntries(frameWindowId, component, menu);
        }

        [DispatchTarget]
        private void AddControlModuleEditingContextMenuEntries([DispatchFilter] DrawControlBusProvider drawObject, MenuOrMenuItem menu) {
            menu.Items.Add(new ClearAllUserActionRequiredMenuItem(drawObject));
            Dispatch.Call.AddComponentInModuleLocationContextMenuEntries(LayoutController.ActiveFrameWindow.Id, (ModelComponent)drawObject.BusProvider, drawObject.Viewer.ModuleLocationID, menu);
        }

        #endregion

        #endregion

        #endregion

        #region Menu Items or actions

        public class ControlProgrammingAction {
            public string Description { get; }
            public Action<IModelComponentCanProgramLocomotives, ControlModule> DoProgramming { get; }

            public ControlProgrammingAction(string description, Action<IModelComponentCanProgramLocomotives, ControlModule> doProgramming) {
                this.Description = description;
                this.DoProgramming = doProgramming;
            }
        }

        private class AddControlModuleMenuItem : LayoutMenuItem {
            private readonly Guid moduleLocationID;
            private readonly ControlBus bus;
            private readonly ControlModuleType moduleType;

            public AddControlModuleMenuItem(Guid moduleLocationID, ControlBus bus, ControlModuleType moduleType) {
                this.moduleLocationID = moduleLocationID;
                this.bus = bus;
                this.moduleType = moduleType;

                Text = moduleType.Name;
            }

            public void Do() {
                LayoutCompoundCommand command = new("Add Control Module", true);
                bool setUserActionRequired = true;
                int count = 1;

                AddControlModuleCommand? addCommand = null;

                if (bus.BusType.Topology == ControlBusTopology.DaisyChain) {
                    Dialogs.GetNumberOfModules d = new(moduleType.Name, bus.BusType.LastAddress - bus.BusType.FirstAddress + 1 - bus.Modules.Count);

                    if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.Cancel)
                        return;

                    addCommand = new AddControlModuleCommand(bus, moduleType.TypeName, moduleLocationID);
                    count = d.Count;
                }
                else {
                    int address = 0;
                    bool userActionRequired = true; 
                    bool addModule = true;

                    var d = new Dialogs.SetControlModuleAddress(bus, moduleType, moduleLocationID, null);

                    if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.OK) {
                        address = d.Address;
                        userActionRequired = d.UserActionRequired;
                    }
                    else
                        addModule = false;

                    if (addModule) {
                        addCommand = new AddControlModuleCommand(bus, moduleType.TypeName, moduleLocationID, address);
                        setUserActionRequired = userActionRequired;
                    }
                }

                if (addCommand != null) {
                    for (int i = 0; i < count; i++) {
                        command.Add(addCommand);

                        if (setUserActionRequired)
                            command.Add(new SetControlUserActionRequiredCommand(addCommand.AddedModule.Module, true));
                    }

                    LayoutController.Do(command);
                }
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                try {
                    Do();
                }
                catch (Exception ex) {
                    Error(ex.Message);
                    throw;
                }
            }
        }

        private class InsertControlModuleMenuItem : LayoutMenuItem {
            private readonly Guid moduleLocationID;
            private readonly ControlModule insertBefore;
            private readonly ControlModuleType? moduleType;

            public InsertControlModuleMenuItem(Guid moduleLocationID, ControlModule insertBefore) {
                this.moduleLocationID = moduleLocationID;
                this.insertBefore = insertBefore;

                IList<ControlModuleType> moduleTypes = insertBefore.Bus.BusType.ModuleTypes;

                if (moduleTypes.Count == 1) {
                    this.moduleType = moduleTypes[0];

                    Text = "Insert " + moduleType.Name + " before this module";
                }
                else {
                    Text = "Insert before this module";

                    foreach (ControlModuleType moduleType in moduleTypes)
                        DropDownItems.Add(new InsertControlModuleMenuItem(moduleLocationID, insertBefore, moduleType));
                }
            }

            public InsertControlModuleMenuItem(Guid moduleLocationID, ControlModule insertBefore, ControlModuleType moduleType) {
                this.moduleLocationID = moduleLocationID;
                this.insertBefore = insertBefore;
                this.moduleType = moduleType;

                Text = moduleType.Name;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                if (moduleType != null) {
                    AddControlModuleCommand addCommand = new(insertBefore.Bus, moduleType.TypeName, moduleLocationID, insertBefore);

                    LayoutController.Do(addCommand);
                }
            }
        }

        private class RemoveControlModuleMenuItem : LayoutMenuItem {
            private readonly ControlModule module;

            public RemoveControlModuleMenuItem(ControlModule module) {
                this.module = module;

                Text = "Remove module";
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                bool showWarning = false;

                for (int index = 0; index < module.NumberOfConnectionPoints; index++)
                    if (module.ConnectionPoints.IsConnected(index))
                        showWarning = true;

                bool doit = true;

                if (showWarning)
                    doit = MessageBox.Show(null, "This " + module.ModuleType.Name + " module is still connected to some components. Are you sure you want to remove it?", "Module still connected",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

                if (doit) {
                    LayoutCompoundCommand removeCommand = new("Remove control module");

                    foreach (var connectionPoint in module.ConnectionPoints)
                        if (connectionPoint != null && connectionPoint.IsConnected) {
                            DisconnectComponentFromConnectionPointCommand disconnectCommand = new(connectionPoint);

                            removeCommand.Add(disconnectCommand);
                        }

                    removeCommand.Add(new RemoveControlModuleCommand(module));
                    LayoutController.Do(removeCommand);
                }
            }
        }

        private class SetControlModuleAddressMenuItem : LayoutMenuItem {
            private readonly Guid moduleLocationID;
            private readonly ControlModule module;

            public SetControlModuleAddressMenuItem(Guid moduleLocationID, ControlModule module) {
                this.moduleLocationID = moduleLocationID;
                this.module = module;

                Text = "Change module address";
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                Dialogs.SetControlModuleAddress d = new(module.Bus, module.ModuleType, moduleLocationID, module);

                if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.OK) {
                    LayoutCompoundCommand command = new("Set control module address");

                    var setCommand = new SetControlModuleAddressCommand(module, d.Address);
                    command.Add(setCommand);

                    if (d.UserActionRequired)
                        command.Add(new SetControlUserActionRequiredCommand(module, true));

                    if (d.Address != module.Address && module.DecoderType != null)
                        command.Add(new SetControlAddressProgrammingRequiredCommand(module, true));

                    LayoutController.Do(command);
                }
            }
        }

        private class SetModuleLocationMenuItem : LayoutMenuItem {
            private readonly ControlModule module;
            private readonly Guid controlModuleLocationId = Guid.Empty;

            public SetModuleLocationMenuItem(bool showAll, ControlModule module) {
                this.module = module;
                LayoutMenuItem item = new SetModuleLocationMenuItem(module, "(None)", Guid.Empty);

                if (module.LocationId == Guid.Empty) {
                    //item.RadioCheck = true;
                    item.Checked = true;
                }

                DropDownItems.Add(item);

                foreach (LayoutControlModuleLocationComponent controlModuleLocation in LayoutModel.Components<LayoutControlModuleLocationComponent>(LayoutPhase.All)) {
                    item = new SetModuleLocationMenuItem(module, controlModuleLocation);

                    if (showAll && module.LocationId == controlModuleLocation.Id) {
                        //item.RadioCheck = true;
                        item.Checked = true;
                    }

                    if (showAll || module.LocationId != controlModuleLocation.Id)
                        DropDownItems.Add(item);
                }

                if (showAll)
                    Text = "Set module location";
                else
                    Text = "Move module to location";

                if (DropDownItems.Count == 0)
                    Enabled = false;
            }

            public SetModuleLocationMenuItem(ControlModule module, LayoutControlModuleLocationComponent controlModuleLocation) {
                this.module = module;
                this.controlModuleLocationId = controlModuleLocation.Id;

                Text = controlModuleLocation.Name;
            }

            public SetModuleLocationMenuItem(ControlModule module, string name, Guid id) {
                this.module = module;
                this.controlModuleLocationId = id;

                Text = name;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                AssignControlModuleLocationCommand assignCommand = new(module, controlModuleLocationId);

                LayoutController.Do(assignCommand);
            }
        }

        private class ManualConnectComponentMenuItem : LayoutMenuItem {
            private readonly IModelComponentConnectToControl component;
            private readonly ModelComponentControlConnectionDescription connectionDescription;

            public ManualConnectComponentMenuItem(string text, IModelComponentConnectToControl component, ModelComponentControlConnectionDescription connectionDescription) {
                this.component = component;
                this.connectionDescription = connectionDescription;
                Text = text;
            }

            protected override void OnClick(EventArgs e) {
                EventManager.Event(new LayoutEvent("request-component-to-control-connect", new ControlConnectionPointDestination(component, connectionDescription)));
                Dispatch.Call.ShowLayoutControl(LayoutController.ActiveFrameWindow.Id);
            }
        }

        private class AutomaticConnectComponentMenuItem : LayoutMenuItem {
            private readonly Guid frameWindowId;
            private ControlAutoConnectRequest? request = null;

            public AutomaticConnectComponentMenuItem(Guid frameWindowId, string text, IModelComponentConnectToControl component, ModelComponentControlConnectionDescription connectionDescription) {
                this.frameWindowId = frameWindowId;
                this.Text = text;

                Initialize(component, connectionDescription);
            }

            public AutomaticConnectComponentMenuItem(Guid frameWindowId, string text, ControlAutoConnectRequest request) {
                this.frameWindowId = frameWindowId;
                this.Text = text;
                this.request = request;
            }

            protected void Initialize(IModelComponentConnectToControl component, ModelComponentControlConnectionDescription connectionDescription) {
                IEnumerable<LayoutControlModuleLocationComponent> moduleLocations = LayoutModel.Components<LayoutControlModuleLocationComponent>(ControlAutoConnectRequest.ModuleLocationPhase(component.Spot.Phase));

                if (moduleLocations != null && moduleLocations.Any()) {
                    var defaultModuleLocation = (LayoutControlModuleLocationComponent?)EventManager.Event(new LayoutEvent<IModelComponentConnectToControl>("get-nearest-control-module-location", component));

                    DropDownItems.Add(new AutomaticConnectComponentMenuItem(frameWindowId, "Default control module location", new ControlAutoConnectRequest(component, connectionDescription)));
                    DropDownItems.Add(new ToolStripSeparator());

                    foreach (LayoutControlModuleLocationComponent moduleLocation in moduleLocations) {
                        var item = new AutomaticConnectComponentMenuItem(frameWindowId, moduleLocation.Name,
                            new ControlAutoConnectRequest(component, connectionDescription, moduleLocation));

                        DropDownItems.Add(item);
                        if (moduleLocation.Id == defaultModuleLocation?.Id)
                            item.DefaultItem = true;
                    }
                }
                else
                    request = new ControlAutoConnectRequest(component, connectionDescription);
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                try {
                    var connectionPoint = (ControlConnectionPoint?)EventManager.Event(new LayoutEvent("request-control-auto-connect", request));

                    if (connectionPoint != null)
                        Dispatch.Call.ShowControlConnectionPoint(frameWindowId, new ControlConnectionPointReference(connectionPoint));
                }
                catch (LayoutException ex) {
                    ex.Report();
                }
            }
        }

        private class ShowConnectionPointMenuItem : LayoutMenuItem {
            public ShowConnectionPointMenuItem(Guid frameWindowId, string text, ControlConnectionPoint connectionPoint)
                : base(text, null, (s, ea) => Dispatch.Call.ShowControlConnectionPoint(frameWindowId, new ControlConnectionPointReference(connectionPoint))) {
            }
        }

        private class DisconnectConnectionPointMenuItem : LayoutMenuItem {
            private readonly ControlConnectionPoint connectionPoint;

            public DisconnectConnectionPointMenuItem(string text, ControlConnectionPoint connectionPoint) {
                this.connectionPoint = connectionPoint;
                Text = text;
            }

            protected override void OnClick(EventArgs e) {
                DisconnectComponentFromConnectionPointCommand disconnectCommand = new(connectionPoint);

                LayoutController.Do(disconnectCommand);
            }
        }

        private class CancelPendingConnectMenuItem : LayoutMenuItem {
            public CancelPendingConnectMenuItem() {
                Text = "Cancel component to control connect";
            }

            protected override void OnClick(EventArgs e) {
                EventManager.Event(new LayoutEvent("cancel-component-to-control-connect", this));
            }
        }

        private class ConnectControlConnectionPointMenuItem : LayoutMenuItem {
            private readonly ControlConnectionPointReference connectionPointReference;
            private readonly IModelComponentConnectToControl component;
            private readonly ModelComponentControlConnectionDescription connectionDescription;

            public ConnectControlConnectionPointMenuItem(string text, ControlConnectionPointReference connectionPoint, IModelComponentConnectToControl component, ModelComponentControlConnectionDescription connectionDescription) {
                this.connectionPointReference = connectionPoint;
                this.component = component;
                this.connectionDescription = connectionDescription;

                Text = text;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                if (connectionPointReference.Module != null) {
                    ConnectComponentToControlConnectionPointCommand command = new(connectionPointReference.Module, connectionPointReference.Index, component,
                        connectionDescription.Name, connectionDescription.DisplayName);

                    LayoutController.Do(command);
                    EventManager.Event(new LayoutEvent("cancel-control-to-component-connect", this));
                }
            }
        }

        public class DisconnectComponentMenuItem : LayoutMenuItem {
            private readonly ControlConnectionPoint connectionPoint;

            public DisconnectComponentMenuItem(ControlConnectionPoint connectionPoint) {
                this.connectionPoint = connectionPoint;

                Text = "Disconnect from " + connectionPoint.DisplayName;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                DisconnectComponentFromConnectionPointCommand command = new(connectionPoint);

                LayoutController.Do(command);
            }
        }

        public class CancelPendingConnectionPointConnectMenuItem : LayoutMenuItem {
            public CancelPendingConnectionPointConnectMenuItem() {
                Text = "Cancel control module connection";
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                EventManager.Event(new LayoutEvent("cancel-control-to-component-connect", this));
            }
        }

        public class ConnectConnectionPointMenuItem : LayoutMenuItem {
            private readonly ControlConnectionPointReference connectionPoint;

            public ConnectConnectionPointMenuItem(ControlConnectionPointReference connectionPoint) {
                this.connectionPoint = connectionPoint;
                Text = "Connect to component";
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                EventManager.Event(new LayoutEvent("request-control-to-component-connect", connectionPoint));
            }
        }

        public class ConnectBusToAnotherCommandStation : LayoutMenuItem {
            private readonly ControlBus bus;
            private readonly IModelComponentIsCommandStation commandStation;

            public ConnectBusToAnotherCommandStation(ControlBus bus, IModelComponentIsCommandStation commandStation, string text) {
                this.bus = bus;
                this.commandStation = commandStation;

                Text = text;
            }

            protected override void OnClick(EventArgs e) {
                ReconnectBusToAnotherCommandBusProvider command = new(bus, commandStation);

                LayoutController.Do(command);
            }
        }

        public class ReplaceCommandStationMenuItem : LayoutMenuItem {
            private readonly IModelComponentIsCommandStation thisCommandStation;
            private readonly IModelComponentIsCommandStation otherCommandStation;

            public ReplaceCommandStationMenuItem(IModelComponentIsCommandStation thisCommandStation, IModelComponentIsCommandStation otherCommandStation, string text) {
                this.thisCommandStation = thisCommandStation;
                this.otherCommandStation = otherCommandStation;

                Text = text;
            }

            protected override void OnClick(EventArgs e) {
                LayoutCompoundCommand commands = new("Replace command station");

                foreach (ControlBus otherBus in LayoutModel.ControlManager.Buses.Buses(otherCommandStation)) {
                    if (otherBus.Modules.Count > 0) {
                        ReconnectBusToAnotherCommandBusProvider reconnectBusCommand = new(otherBus, thisCommandStation);

                        commands.Add(reconnectBusCommand);
                    }
                }

                foreach (LayoutTrackPowerConnectorComponent powerConnector in LayoutModel.Components<LayoutTrackPowerConnectorComponent>(LayoutPhase.All)) {
                    if (powerConnector.Inlet.IsConnected && powerConnector.Inlet.OutletComponentId == otherCommandStation.Id) {
                        LayoutXmlInfo xmlInfo = new(powerConnector);

                        powerConnector.Inlet.OutletComponent = thisCommandStation;

                        LayoutModifyComponentDocumentCommand updatePowerConnector = new(powerConnector, xmlInfo);

                        commands.Add(updatePowerConnector);
                    }
                }

                LayoutController.Do(commands);
            }
        }

        public class MoveControlModuleInDaisyChainMenuItem : LayoutMenuItem {
            private readonly ControlModule module;
            private readonly int delta;

            public MoveControlModuleInDaisyChainMenuItem(ControlModule module, int delta, string text) {
                this.module = module;
                this.delta = delta;

                Text = text;

                if (module.Bus.GetModuleUsingAddress(module.Address + delta) == null)
                    Enabled = false;
            }

            protected override void OnClick(EventArgs e) {
                MoveControlModuleInDaisyChainBusCommand command = new(module, delta, "Move control module " + (delta < 0 ? "up" : "down"));

                LayoutController.Do(command);
            }
        }

        public class SetModuleLabelMenuItem : LayoutMenuItem {
            private readonly ControlModule module;

            public SetModuleLabelMenuItem(ControlModule module) {
                this.module = module;

                Text = "Set module label";
            }

            protected override void OnClick(EventArgs e) {
                Dialogs.SetControlModuleLabel d = new(module);
                
                if (d.ShowDialog(LayoutController.ActiveFrameWindow) == DialogResult.OK) {
                    SetControlModuleLabelCommand command = new(module, d.Label);

                    LayoutController.Do(command);
                }
            }
        }

        public class ToggleUserActionRequiredMenuItem : LayoutMenuItem {
            private readonly IControlSupportUserAction subject;

            public ToggleUserActionRequiredMenuItem(IControlSupportUserAction subject) {
                this.subject = subject;

                if (subject.UserActionRequired)
                    Text = "Clear \"Action Required\" flag";
                else
                    Text = "Set \"Action Required\" flag";
            }

            protected override void OnClick(EventArgs e) {
                LayoutCompoundCommand command = new("Set control module action required");

                bool newValue = !subject.UserActionRequired;

                if (!newValue && subject is ControlModule module) {
                    // Clear all connection point 
                    for (int index = 0; index < module.NumberOfConnectionPoints; index++)
                        if (module.ConnectionPoints.IsUserActionRequired(index)) {
                            SetControlUserActionRequiredCommand clearCommand = new(module.ConnectionPoints[index], false);

                            command.Add(clearCommand);
                        }
                }

                SetControlUserActionRequiredCommand setCommand = new(subject, newValue);
                command.Add(setCommand);

                LayoutController.Do(command);
            }
        }

        public class ClearAllUserActionRequiredMenuItem : LayoutMenuItem {
            private readonly DrawControlBase drawObject;

            public ClearAllUserActionRequiredMenuItem(DrawControlBase drawObject) {
                this.drawObject = drawObject;

                Text = "Clear all \"User Action Required\" flags";
            }

            private void DoClear(LayoutCompoundCommand command, DrawControlBase drawObject) {
                if (drawObject is DrawControlModule drawModule) {
                    if (drawModule.Module.UserActionRequired)
                        command.Add(new SetControlUserActionRequiredCommand(drawModule.Module, false));
                }
                else if (drawObject is DrawControlConnectionPoint drawConnectionPoint) {
                    if (drawConnectionPoint.Module.ConnectionPoints.IsUserActionRequired(drawConnectionPoint.Index))
                        command.Add(new SetControlUserActionRequiredCommand(new ControlConnectionPointReference(drawConnectionPoint.Module, drawConnectionPoint.Index), false));
                }

                foreach (DrawControlBase childDrawObject in drawObject.DrawObjects)
                    DoClear(command, childDrawObject);
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);

                LayoutCompoundCommand command = new("clear \"User Action Required\" flags");

                DoClear(command, drawObject);
                LayoutController.Do(command);
            }
        }

        #endregion
    }
}
