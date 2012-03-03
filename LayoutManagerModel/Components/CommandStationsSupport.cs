using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

using LayoutManager.Model;

namespace LayoutManager {

	public static class CommandStationLayoutEventExtender {
		/// <summary>
		/// Add command station identifier to event. This should be call for each event that should be handled by a specific command station.
		/// It adds CommandStation option section with the command station's ID and name. The command station's event handler can use this information 
		/// to filter events that it should handle
		/// </summary>
		/// <param name="theEvent"></param>
		/// <param name="commandStation"></param>
		/// <returns></returns>
		public static LayoutEvent SetCommandStation(this LayoutEvent theEvent, IModelComponentHasNameAndId commandStation) {
			return theEvent.SetOption(elementName: "CommandStation", optionName: "ID", id: commandStation.Id).SetOption(elementName: "CommandStation", optionName: "Name", value: commandStation.NameProvider.Name);
		}

		/// <summary>
		/// Add command station information based on a command station controlling a train
		/// </summary>
		/// <param name="theEvent">the event</param>
		/// <param name="train">the train that its controlling command station should be used</param>
		/// <returns></returns>
		public static LayoutEvent SetCommandStation(this LayoutEvent theEvent, TrainStateInfo train) {
			return SetCommandStation(theEvent, train.CommandStation);
		}

		/// <summary>
		/// Add command station information based on the control bus the command station is connected to
		/// </summary>
		/// <param name="theEvent"></param>
		/// <param name="bus"></param>
		/// <returns></returns>
		public static LayoutEvent SetCommandStation(this LayoutEvent theEvent, ControlBus bus) {
			var commandStation = (IModelComponentHasNameAndId)bus.BusProvider;

			return SetCommandStation(theEvent, commandStation);
		}

		/// <summary>
		/// Add command station information based on a control connection point reference
		/// </summary>
		/// <param name="theEvent"></param>
		/// <param name="connectionPointRef"></param>
		/// <returns></returns>
		public static LayoutEvent SetCommandStation(this LayoutEvent theEvent, ControlConnectionPointReference connectionPointRef) {
			return SetCommandStation(theEvent, connectionPointRef.Module.Bus);
		}
	}
}

namespace LayoutManager.Components {

	#region Base class for command station components

	/// <summary>
	///Base class for command station components. This class implements the functionality that is common to command station components,
	///Thus making it simpler to implement those components.
	/// </summary>
	public abstract class LayoutCommandStationComponent : ModelComponent, IModelComponentIsCommandStation, IDisposable, ILayoutLockResource {
		FileStream						commStream;
		LayoutPowerOutlet				trackPowerOutlet;
		LayoutPowerOutlet				programmingPowerOutlet;
		ILayoutInterThreadEventInvoker	invoker;
		bool							operationMode;
		ILayoutEmulatorServices			_layoutEmulationServices;
		System.Threading.Timer			animatedTrainsTimer;						
		LayoutSelection					animatedTrainsSelection;
		ILayoutCommandStationEmulator	commandStationEmulator;

		#region Public component properties & methods

		public override ModelComponentKind Kind {
			get {
				return ModelComponentKind.ControlComponent;
			}
		}

		public LayoutTextInfo NameProvider {
			get {
				return new LayoutTextInfo(this);
			}
		}

		public override bool DrawOutOfGrid {
			get {
				return NameProvider.Element != null && NameProvider.Visible == true;
			}
		}

		public string Name {
			get {
				return NameProvider.Name;
			}
		}

		public ILayoutEmulatorServices LayoutEmulationServices {
			get {
				if(_layoutEmulationServices == null)
					_layoutEmulationServices = (ILayoutEmulatorServices)EventManager.Event(new LayoutEvent(this, "get-layout-emulation-services"));
				return _layoutEmulationServices;
			}
		}

		public bool EmulateLayout {
			get; set;
		}

		public int EmulationTickTime {
			get {
				return XmlConvert.ToInt32(Element.GetAttribute("EmulationTickTime"));
			}
		}

		public bool AnimateTrainMotion {
			get {
				return XmlConvert.ToBoolean(Element.GetAttribute("AnimateTrainMotion"));
			}
		}

		public abstract IList<string> BusTypeNames {
			get;
		}

		public IList<ILayoutPowerOutlet> PowerOutlets {
			get {
				if(this is IModelComponentCanProgramLocomotives)
					return Array.AsReadOnly<ILayoutPowerOutlet>(new ILayoutPowerOutlet[] { trackPowerOutlet, programmingPowerOutlet });
				else
					return Array.AsReadOnly<ILayoutPowerOutlet>(new ILayoutPowerOutlet[] { trackPowerOutlet }); ;
			}
		}

		#endregion

		#region Properties accessible from derived concrete command station component classes

		public FileStream CommunicationStream {
			get {
				return commStream;
			}
		}

		public ILayoutInterThreadEventInvoker InterThreadEventInvoker {
			get {
				return invoker;
			}
		}

		public  bool OperationMode {
			get {
				return operationMode;
			}
		}

		#endregion

		#region Methods callable from derived concrete command station component classes

		public override void OnAddedToModel() {
			base.OnAddedToModel();

			trackPowerOutlet = new LayoutPowerOutlet("Track", new LayoutPower(this, LayoutPowerType.Digital, this.SupportedDigitalPowerFormats, "Track"));
			if(this is IModelComponentCanProgramLocomotives)
				programmingPowerOutlet = new LayoutPowerOutlet("Programming", new LayoutPower(this, LayoutPowerType.Programmer, this.SupportedDigitalPowerFormats, "Programming"));

			LayoutModel.ControlManager.Buses.AddBusProvider(this);
			invoker = EventManager.Instance.InterThreadEventInvoker;
		}

		public override void OnRemovingFromModel() {
			base.OnRemovingFromModel();

			LayoutModel.ControlManager.Buses.RemoveBusProvider(this);
			trackPowerOutlet = null;
			programmingPowerOutlet = null;
		}

		protected void PowerOn() {
			if(OperationMode) {
				trackPowerOutlet.Power = new LayoutPower(this, LayoutPowerType.Digital, this.SupportedDigitalPowerFormats, Name);

				if(this is IModelComponentCanProgramLocomotives)
					programmingPowerOutlet.Power = new LayoutPower(this, LayoutPowerType.Programmer, this.SupportedDigitalPowerFormats, Name);
			}
			EventManager.Event(new LayoutEvent<IModelComponentIsCommandStation>("command-station-power-on-notification", this));
		}

		protected void PowerOff() {
			if(OperationMode)
				trackPowerOutlet.Power = new LayoutPower(this, LayoutPowerType.Disconnected, DigitalPowerFormats.None, Name);
			EventManager.Event(new LayoutEvent<IModelComponentIsCommandStation>("command-station-power-off-notification", this));
		}

		protected void ProgrammingPowerOn() {
			if(OperationMode)
				programmingPowerOutlet.Power = new LayoutPower(this, LayoutPowerType.Programmer, this.SupportedDigitalPowerFormats, Name + "_programming");
		}

		protected void ProgrammingPowerOff() {
			if(OperationMode)
				programmingPowerOutlet.Power = new LayoutPower(this, LayoutPowerType.Disconnected, DigitalPowerFormats.None, Name + "_programming");
		}

		protected void OpenCommunicationStream() {
			if(EmulateLayout && LayoutEmulationSupported) {
				string	pipeName = @"\\.\pipe\CommandStationEmulationFor_" + Name;
				bool	overlappedIO = GetBool("OverlappedIO"); 

				SafeFileHandle	handle = (SafeFileHandle)EventManager.Event(new LayoutEvent(pipeName, "create-named-pipe-request",
					null, overlappedIO));

				commandStationEmulator = CreateCommandStationEmulator(pipeName);

				commStream = (FileStream)EventManager.Event(new LayoutEvent(handle, "wait-named-pipe-client-to-connect-request", null, overlappedIO));

				if(AnimateTrainMotion) {
					animatedTrainsSelection = new LayoutSelection();
					animatedTrainsSelection.Display(new LayoutSelectionLook(Color.DarkSlateBlue));

					animatedTrainsTimer = new System.Threading.Timer(new TimerCallback(animationTimerCallback), null, 500, 200);
				}
			}
			else
				commStream = (FileStream)EventManager.Event(new LayoutEvent(Element, "open-serial-communication-device-request"));
		}

		protected void CloseCommunicationStream() {
			if(commandStationEmulator != null) {
				if(animatedTrainsTimer != null) {
					animatedTrainsTimer.Dispose();
					animatedTrainsTimer = null;
				}

				if(animatedTrainsSelection != null) {
					animatedTrainsSelection.Hide();
					animatedTrainsSelection.Clear();
					animatedTrainsSelection = null;
				}

				commandStationEmulator.Dispose();
				commandStationEmulator = null;

				EventManager.Event(new LayoutEvent(CommunicationStream, "disconnect-named-pipe-request"));
			}

			if(commStream != null) {
				SafeFileHandle sh = commStream.SafeFileHandle;

				commStream.SafeFileHandle.Close();
				CommunicationStream.Close();
				System.GC.Collect();
				if(!sh.IsClosed)
					Trace.WriteLine("Warning: PORT WAS NOT CLOSED");
				commStream = null;
			}
		}

		// Error and warning methods that can be called from any thread

		public override void Error(object subject, string message) {
			if(InterThreadEventInvoker == null)
				base.Error(subject, message);
			else
				InterThreadEventInvoker.QueueEvent(new LayoutEvent(subject, "add-error", null, message));
		}

		public override void Warning(object subject, string message) {
			if(InterThreadEventInvoker == null)
				base.Warning(subject, message);
			else
				InterThreadEventInvoker.QueueEvent(new LayoutEvent(subject, "add-warning", null, message));
		}

		#region Animate Train (when using emulation)

		TrackEdgeDictionary currentShownTrainPositions = new TrackEdgeDictionary();

		[LayoutEvent("show-emulated-locomotive-locations")]
		protected virtual void showTrainPositions(LayoutEvent e) {
			if(e.Sender == this) {
				if(LayoutEmulationServices != null && animatedTrainsSelection != null) {
					IList<ILocomotiveLocation> locomotiveLocations = LayoutEmulationServices.GetLocomotiveLocations(Id);

					foreach(ILocomotiveLocation locomotiveLocation in locomotiveLocations) {
						if(currentShownTrainPositions.Contains(locomotiveLocation.Edge))	// Is is already in selection, do nothing
							currentShownTrainPositions.Remove(locomotiveLocation.Edge);
						else
							animatedTrainsSelection.Add(locomotiveLocation.Track);		// Not found in currently shown, add it to the selection
					}

					// All edges that are left in the hash table are no longer displayed, remove them from the selection
					foreach(TrackEdge edge in currentShownTrainPositions.Keys)
						animatedTrainsSelection.Remove(edge.Track);

					currentShownTrainPositions.Clear();
					foreach(ILocomotiveLocation locomotiveLocation in locomotiveLocations) {
						if(!currentShownTrainPositions.ContainsKey(locomotiveLocation.Edge))
							currentShownTrainPositions.Add(locomotiveLocation.Edge);
					}
				}
			}
		}

		private void animationTimerCallback(object state) {
			invoker.QueueEvent(new LayoutEvent(this, "show-emulated-locomotive-locations"));
		}

		#endregion

		#endregion

		#region Overridable properties

		/// <summary>
		/// Returns true if this command station component supports layout emulation
		/// </summary>
		public virtual bool LayoutEmulationSupported {
			get {
				return false;
			}
		}

		/// <summary>
		/// If design time layout activation supported
		/// </summary>
		/// <value>true if yes</value>
		public virtual bool DesignTimeLayoutActivationSupported {
			get {
				return false;
			}
		}

		/// <summary>
		/// Does this command station supports processing of a batch of multipath (e.g. turnout) switching
		/// </summary>
		public virtual bool BatchMultipathSwitchingSupported {
			get {
				return false;
			}
		}

		/// <summary>
		/// Does this command station perform train analysis before entering operational mode. Layout analysis is
		/// for example, checking what occupancy blocks are occupied when starting up
		/// </summary>
		public virtual bool TrainsAnalysisSupported {
			get {
				return false;
			}
		}

		public abstract DigitalPowerFormats SupportedDigitalPowerFormats {
			get;
		}

		#endregion

		#region Overridable methods

		/// <summary>
		/// Set attributes/elements of this command station XML element to provide all the needed setup information
		/// for opening the communication channel to the command station.
		/// </summary>
		protected virtual void OnCommunicationSetup() {
		}

		/// <summary>
		/// Called after the communication stream is opened, initialize command station operation
		/// </summary>
		protected virtual void OnInitialize() {
		}

		/// <summary>
		/// Called after operation mode has been entered, command station initialized, and power was set on
		/// </summary>
		protected virtual void OnEnteredOperationMode() {
		}

		/// <summary>
		/// Called just before closing the communication stream, ensure that all commands queue are empty etc.
		/// Default implementation is returning a completed task that does noting
		/// </summary>
		protected virtual Task OnTerminateCommunication() {
			var tcs = new TaskCompletionSource<object>();

			tcs.SetResult(null);
			return tcs.Task;
		}

		/// <summary>
		/// Terminate command station operation. Called before terminating power to the layout
		/// </summary>
		protected virtual void OnCleanup() {
		}

		public virtual int GetLowestLocomotiveAddress(DigitalPowerFormats format) {
			return 1;
		}

		public virtual int GetHighestLocomotiveAddress(DigitalPowerFormats format) {
			if(format == DigitalPowerFormats.NRMA)
				return 10239;
			else if(format == DigitalPowerFormats.Motorola)
				return 80;
			else
				throw new ArgumentException("Unsupported digital power format: " + format.ToString());
		}

		protected virtual ILayoutCommandStationEmulator CreateCommandStationEmulator(string pipeName) {
			throw new ArgumentException("You must implement a CreateCommandStationEmulator method, or not use layout emulation");
		}

		#endregion

		#region Event handlers

		[LayoutEvent("query-perform-trains-analysis")]
		protected virtual void QueryPerformLayoutAnalysis(LayoutEvent e) {
			if(TrainsAnalysisSupported) {
				List<IModelComponentIsCommandStation> needAnalysis = (List<IModelComponentIsCommandStation>)e.Info;

				needAnalysis.Add(this);
			}
		}

		[LayoutEvent("enter-operation-mode")]
		protected virtual void EnterOperationMode(LayoutEvent e0) {
			var e = (LayoutEvent<OperationModeParameters>)e0;

			EmulateLayout = e.Sender.Simulation;

			OnCommunicationSetup();
			OpenCommunicationStream();

			operationMode = true;
			OnInitialize();

			// Connect power to the layout
			EventManager.Event(new LayoutEvent(this, "connect-power-request"));
			EventManager.Event(new LayoutEvent(this, "reset-layout-emulation"));

			OnEnteredOperationMode();
		}

		[LayoutAsyncEvent("exit-operation-mode-async")]
		protected async virtual Task ExitOperationModeAsync(LayoutEvent e) {
			if(OperationMode) {
				OnCleanup();

				// Disconnect power to the layout
				Trace.WriteLine("Disconnect track request");
				EventManager.Event(new LayoutEvent(this, "disconnect-power-request"));

				await OnTerminateCommunication();
				CloseCommunicationStream();

				Trace.WriteLine("After CloseCommunicationStream");

				operationMode = false;
			}
		}

		[LayoutEvent("begin-design-time-layout-activation")]
		protected virtual void BeginDesignTimeLayoutActivation(LayoutEvent e) {
			if(DesignTimeLayoutActivationSupported) {
				if(!LayoutController.IsDesignTimeActivation) {
					OnCommunicationSetup();
					OpenCommunicationStream();
					OnInitialize();

					EventManager.Event(new LayoutEvent(this, "connect-track-power-request"));

				}
				e.Info = true;
			}
			else
				e.Info = false;
		}

		[LayoutEvent("end-design-time-layout-activation")]
		protected virtual void EndDesignTimeLayoutActivation(LayoutEvent e) {
			if(DesignTimeLayoutActivationSupported) {
				OnCleanup();

				EventManager.Event(new LayoutEvent(this, "disconnect-track-power-request"));
				OnTerminateCommunication();
				CloseCommunicationStream();
				e.Info = true;
			}
			else
				e.Info = false;
		}

		[LayoutEvent("connect-track-power-request")]
		virtual protected void OnConnectTrackPower(LayoutEvent e) {
			if(e.Sender == this)
				EventManager.Event(new LayoutEvent(this, "connect-power-request"));
		}

		[LayoutEvent("disconnect-track-power-request")]
		virtual protected void OnDisconnectTrackPower(LayoutEvent e) {
			if(e.Sender == this)
				EventManager.Event(new LayoutEvent(this, "disconnect-power-request"));
		}

		#endregion

		#region Utility methods

		bool GetBool(string name, bool defaultValue) {
			if(Element.HasAttribute(name))
				return XmlConvert.ToBoolean(Element.GetAttribute(name));
			return defaultValue;
		}

		bool GetBool(string name) {
			return GetBool(name, false);
		}

		#endregion

		#region IDisposable Members

		public void Dispose() {
			if(animatedTrainsTimer != null) {
				animatedTrainsTimer.Dispose();
				animatedTrainsTimer = null;
			}

			if(trackPowerOutlet != null)
				trackPowerOutlet = null;

			if(programmingPowerOutlet != null)
				programmingPowerOutlet = null;
		}

		#endregion

		#region ILayoutLockResource Members

		/// <summary>
		/// Command station is locked when it is used for programming. This prevent a command station from being used
		/// to perform multiple programming sequences in the same time.
		/// </summary>
		public LayoutLockRequest LockRequest {
			get; set;
		}

		public bool IsResourceReady() {
			return true;
		}

		#endregion
	}

	#endregion

	#region Classes used for design time layout activation (for example learn layout)

	/// <summary>
	/// Information about input received from the layout when the in design time layout activation.
	/// </summary>
	public class CommandStationInputEvent : IComparable<CommandStationInputEvent> {
		ModelComponent commandStation;
		ControlBus bus;
		int address;
		int index = -1;
		int state;

		public CommandStationInputEvent(ModelComponent commandStation, ControlBus bus, int address) {
			this.commandStation = commandStation;
			this.bus = bus;
			this.address = address;
		}

		public CommandStationInputEvent(ModelComponent commandStation, ControlBus bus, int address, int state) {
			this.commandStation = commandStation;
			this.bus = bus;
			this.address = address;
			this.state = state;
		}

		public CommandStationInputEvent(ModelComponent commandStation, ControlBus bus, int address, int index, int state) {
			this.commandStation = commandStation;
			this.bus = bus;
			this.address = address;
			this.index = index;
			this.state = state;
		}

		public ModelComponent CommandStation {
			get {
				return commandStation;
			}
		}

		public ControlBus Bus {
			get {
				return bus;
			}
		}

		public int Address {
			get {
				return address;
			}
		}

		public int Index {
			get {
				return index;
			}
		}

		public int State {
			get {
				return state;
			}
		}

		ControlConnectionPointReference FindConnectionPointRef(ControlBus bus) {
			ControlModule module = bus.GetModuleUsingAddress(address);

			if(module == null)
				return null;
			else {
				int theIndex;

				if(Bus.BusType.AddressingMethod == ControlAddressingMethod.DirectConnectionPointAddressing) {
					theIndex = address - module.Address;

					if(module.ModuleType.ConnectionPointsPerAddress > 1)
						theIndex = theIndex * module.ModuleType.ConnectionPointsPerAddress + state;
				}
				else {
					if(Index < 0)
						throw new ArgumentException("Index was not defined although bus uses ModuleConnectionPointAddressing method");

					theIndex = Index;
				}

				return new ControlConnectionPointReference(module, theIndex);
			}
		}

		public ControlConnectionPointReference ConnectionPointRef {
			get {
				return FindConnectionPointRef(bus);
			}
		}

		public ModelComponent ConnectedComponent {
			get {
				ControlConnectionPointReference cpr = ConnectionPointRef;

				if(cpr != null && cpr.IsConnected)
					return (ModelComponent)cpr.ConnectionPoint.Component;
				else
					return null;
			}
		}

		public string GetAddressTextForModuleTypes(IList<ControlModuleType> moduleTypes) {
			// For each bus, get the module types that can be attached to this bus. For each module type get the format
			// then convert the address/unit 

			string result = "";

			foreach(ControlModuleType moduleType in moduleTypes) {
				if(result.Length > 0)
					result += ", ";

				int baseAddress = (address / moduleType.AddressAlignment) * moduleType.AddressAlignment;
				int theIndex;

				if(Bus.BusType.AddressingMethod == ControlAddressingMethod.DirectConnectionPointAddressing) {
					theIndex = (address % moduleType.NumberOfAddresses) * moduleType.ConnectionPointsPerAddress;

					if(moduleType.ConnectionPointsPerAddress > 1)
						theIndex += state;
				}
				else
					theIndex = Index;

				result += moduleType.GetConnectionPointAddressText(baseAddress, theIndex, true);
			}

			return result;
		}

		public string GetAddressTextForComponent(ControlConnectionPointDestination connectionDestination) {
			List<ControlModuleType> moduleTypes = new List<ControlModuleType>();

			if(ConnectionPointRef == null) {
				IList<string> moduleTypeNames = Bus.BusType.GetConnectableControlModuleTypeNames(connectionDestination);

				foreach(string moduleTypeName in moduleTypeNames)
					moduleTypes.Add(LayoutModel.ControlManager.GetModuleType(moduleTypeName));
			}
			else
				moduleTypes.Add(ConnectionPointRef.Module.ModuleType);

			return GetAddressTextForModuleTypes(moduleTypes);
		}

		public string AddressText {
			get {
				List<ControlModuleType> moduleTypes = new List<ControlModuleType>();

				if(ConnectionPointRef != null)
					moduleTypes.Add(ConnectionPointRef.Module.ModuleType);
				else {
					foreach(ControlModuleType moduleType in bus.BusType.ModuleTypes)
						if(!moduleTypes.Exists(delegate(ControlModuleType moduleTypeToGetString) {
							return moduleType.ConnectionPointLabelFormat == moduleTypeToGetString.ConnectionPointLabelFormat &&
								moduleType.AddressAlignment == moduleTypeToGetString.AddressAlignment &&
								moduleType.ConnectionPointArrangement == moduleTypeToGetString.ConnectionPointArrangement &&
								moduleType.ConnectionPointIndexBase == moduleTypeToGetString.ConnectionPointIndexBase &&
								moduleType.ConnectionPointsPerAddress == moduleTypeToGetString.ConnectionPointsPerAddress;
						}))
							moduleTypes.Add(moduleType);
				
				}

				return GetAddressTextForModuleTypes(moduleTypes);
			}
		}

		#region IComparable<CommandStationInputEvent> Members

		public int CompareTo(CommandStationInputEvent other) {
			if(CommandStation != other.CommandStation) {
				IModelComponentHasName	thisCommandStation = (IModelComponentHasName)CommandStation;
				IModelComponentHasName	otherCommandStation = (IModelComponentHasName)other;

				return thisCommandStation.NameProvider.Name.CompareTo(otherCommandStation.NameProvider.Name);
			}

			if(bus != other.bus) {
				var busNameDiff = bus.Name.CompareTo(other.bus.Name);

				if(busNameDiff != 0)
					return busNameDiff;
			}

			if(address != other.address)
				return address - other.address;

			if(bus.BusType.AddressingMethod == ControlAddressingMethod.ModuleConnectionPointAddressing) {
				if(index != other.index)
					return index - other.index;
			}
			else {
				ControlConnectionPointReference cpr = ConnectionPointRef;

				if(cpr == null || cpr.Module.ModuleType.ConnectionPointsPerAddress > 1) {
					if(state != other.state)
						return state - other.state;
				}
			}

			return 0;
		}

		public bool Equals(CommandStationInputEvent other) {
			return CompareTo(other) == 0;
		}

		#endregion
	}

	#endregion

	#region Support for queuing of commands generated by command station components

	public interface IOutputCommand {
		void Do();

		int WaitPeriod {
			get;
		}

		/// <summary>
		/// Task that is completed after the command was sent (or in case of command with reply, after the reply is received)
		/// </summary>
		Task Task {
			get;
		}

		/// <summary>
		/// Called when the command is completed
		/// </summary>
		/// <param name="reply"></param>
		void Completed(object result);
	}

	public interface IOutputCommandWithReply : IOutputCommand {
		/// <summary>
		/// The timeout in milliseconds to wait for reply
		/// </summary>
		int Timeout {
			get;
		}

		/// <summary>
		/// Called when got a reply
		/// </summary>
		/// <param name="reply">The received reply</param>
		void OnReply(object replyPacket);

		/// <summary>
		/// Called when no reply has been received within the timeout period
		/// </summary>
		void OnTimeout();
	}

	public interface IOutputCommandReply {

	}

	public abstract class OutputCommandBase : IOutputCommand {
		int		waitPeriod;
		protected TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

		/// <summary>
		/// Execute the action associated with the command
		/// </summary>
		public abstract void Do();

		/// <summary>
		/// Number of milliseconds to wait after this command before next command from this queue can execute. Default
		/// is not to wait
		/// </summary>
		public virtual int WaitPeriod {
			get {
				return waitPeriod;
			}

			set {
				waitPeriod = value;
			}
		}

		public Task Task {
			get { return tcs.Task; }
		}

		public void Completed(object result = null) {
			tcs.TrySetResult(result);
		}
	}

	/// <summary>
	/// Base class for commands that need to get a reply before the next command can be sent.
	/// </summary>
	public abstract class OutputSynchronousCommandBase: OutputCommandBase, IOutputCommandWithReply {

		/// <summary>
		/// Timeout (in milliseconds) for waiting for the reply (default is 2 seconds)
		/// </summary>
		public virtual int Timeout {
			get { return 2000;  }
		}

		public virtual void OnTimeout() {
			EventManager.Instance.InterThreadEventInvoker.QueueEvent(new LayoutEvent(null, "add-error", null, "Reply was not received on time for " + this.GetType().Name + " command"));
		}

		public abstract void OnReply(object replyPacket);
	}

	public class EndTrainsAnalysisCommandStationCommand : OutputCommandBase, IOutputIdlecommand {
		IModelComponentIsCommandStation commandStation;
		int passCount;

		public EndTrainsAnalysisCommandStationCommand(IModelComponentIsCommandStation commandStation, int passCount) {
			this.commandStation = commandStation;
			this.passCount = passCount;
			WaitPeriod = 250;
		}

		public EndTrainsAnalysisCommandStationCommand(IModelComponentIsCommandStation commandStation)
			: this(commandStation, 1) {
		}

		public override void Do() {
			if(passCount > 0) {
				if(--passCount == 0) {
					passCount = -1;
					if(!LayoutController.IsDesignTimeActivation)
						commandStation.InterThreadEventInvoker.QueueEvent(new LayoutEvent(this, "command-station-trains-analysis-phase-done"));
				}
			}
		}

		#region ICommandStationIdlecommand Members

		public bool RemoveFromQueue {
			get {
				return passCount == -1;
			}
		}

		#endregion
	}


	public interface IOutputIdlecommand : IOutputCommand {
		bool RemoveFromQueue { get; }
	}

	public class OutputManager {
		string name;
		CommandManagerQueue[] queues;
		ManualResetEvent workToDo = new ManualResetEvent(false);
		ManualResetEvent nothingToDo = new ManualResetEvent(true);
		ManualResetEvent waitToGetReply = new ManualResetEvent(false);
		CommandManagerQueue idleCommands;
		bool doIdleCommands = true;
		Thread commandManagerThread;
		static LayoutTraceSwitch traceOutputManager = new LayoutTraceSwitch("OutputManager", "Output Manager");
		IOutputCommandWithReply pendingCommandWithReply = null;


		public OutputManager(string name, int numberOfQueues) {
			this.name = name;

			queues = new CommandManagerQueue[numberOfQueues];
			idleCommands = new CommandManagerQueue(workToDo, queues);

			for(int i = 0; i < queues.Length; i++)
				queues[i] = new CommandManagerQueue(workToDo, queues);
		}

		#region Operations

		public void Start() {
			Debug.Assert(commandManagerThread == null, "Command Manager thread already running");

			if(commandManagerThread == null) {
				commandManagerThread = new Thread(new ThreadStart(CommandManagerThread));
				commandManagerThread.Name = "CommandManagerThread for " + name;
				commandManagerThread.Priority = ThreadPriority.AboveNormal;
				commandManagerThread.Start();
			}
		}

		public void Terminate() {
			if(commandManagerThread != null)
				commandManagerThread.Abort();
		}

		/// <summary>
		/// Add command to be executed to a command queue. Commands queues in queues with a lower index, are executed before
		/// commands in queues with higher index.
		/// </summary>
		/// <param name="iQueue">The queue number</param>
		/// <param name="command">The command to add</param>
		public Task AddCommand(int queueNumber, OutputCommandBase command) {
			if(queueNumber < 0 || queueNumber >= queues.Length)
				throw new ArgumentException("Invalid queue index: " + queueNumber);

			Trace.WriteLineIf(traceOutputManager.TraceInfo, "Add command " + command.GetType().Name);

			lock(queues) {
				queues[queueNumber].Enqueue(command);
				workToDo.Set();
				nothingToDo.Reset();
			}

			return command.Task;
		}

		public Task AddCommand(OutputCommandBase command) {
			return AddCommand(0, command);
		}

		/// <summary>
		/// Add a command to be executed when no commands can be found in the queues.
		/// </summary>
		/// <param name="command">The command to execute</param>
		public void AddIdleCommand(IOutputIdlecommand command) {
			lock(queues) {
				idleCommands.Enqueue(command);
				workToDo.Set();
				nothingToDo.Reset();
			}
		}

		/// <summary>
		/// Called when getting a reply from the communication stream. If there was a pending synchronous command, it will get the reply
		/// and more commands (if any) will be sent to the workstation. If there is no pending synchronous command, the reply is just ignored
		/// </summary>
		/// <param name="reply"></param>
		public void SetReply(IOutputCommandReply reply) {
			lock(this) {
				if(pendingCommandWithReply != null)
					pendingCommandWithReply.OnReply(reply);
				waitToGetReply.Set();		// Wait is done
			}
		}

		/// <summary>
		/// Returns true if waiting for a synchronous command reply
		/// </summary>
		public bool IsWaitingForReply {
			get {
				bool result = false;

				lock(this)
					result = pendingCommandWithReply != null;

				return result;
			}
		}

		/// <summary>
		/// If true, idle commands are executed if queues do not contain any other pending commands
		/// </summary>
		public bool DoIdleCommands {
			get {
				return doIdleCommands;
			}

			set {
				lock(queues)
					doIdleCommands = value;
			}
		}

		/// <summary>
		/// Wait until there are no commands in the queue. This will also stop idle command execution
		/// </summary>
		public async Task WaitForIdle() {
			DoIdleCommands = false;

			var pendingCommands = new List<Task>();

			lock(queues) {
				foreach(var queue in queues) {
					foreach(var command in queue)
						pendingCommands.Add(command.Task);
				}
			}

			if(pendingCommands.Count > 0) {
				Trace.WriteLineIf(traceOutputManager.TraceVerbose, "Waiting for for command station pending commands");

				var pendingCommandsTask = TaskEx.WhenAll(pendingCommands);

				if(await TaskEx.WhenAny(pendingCommandsTask, TaskEx.Delay(5000)) != pendingCommandsTask) {
					LayoutModuleBase.Warning("Unable to complete all command station's pending commands");
					Trace.WriteLine("Wait for command station pending commands has timed out after 5 seconds");
				}
				else
					Trace.WriteLineIf(traceOutputManager.TraceVerbose, "Waiting for idle done");
			}
		}

		#endregion

		#region Command Manager Thread and methods that run in thread context

		protected void CommandManagerThread() {
			long previousIdleCommandTime = 0;

			try {
				while(true) {
					IOutputCommand	command = null;

					//Trace.WriteLineIf(traceCommandStationManager.TraceVerbose, " CommandManagerThread: Wait for work to do");
					workToDo.WaitOne();
					//Trace.WriteLineIf(traceCommandStationManager.TraceVerbose, " CommandManagerThread: Got something to do");

					lock(queues) {
						// Scan the queues and look for the first queue with a command ready to execute
						foreach(CommandManagerQueue queue in queues) {
							if(queue.IsCommandReady) {
								command = queue.GetCommand();
								//Trace.WriteLineIf(traceCommandStationManager.TraceVerbose, " CommandManagerThread: Found command in queue");
								break;
							}
						}

						// No command was found in the queue, check if an idle command is ready for execution
						if(command == null && DoIdleCommands && idleCommands.IsCommandReady) {
							command = idleCommands.GetCommand();

							long idleCommandTime = DateTime.Now.Ticks;

							if(previousIdleCommandTime != 0) {
								long diff = (idleCommandTime - previousIdleCommandTime) / 10000;    // Diff in milliseconds

#if ALERT_ON_SLOW_POLLING
								if(diff > 70)
									Trace.WriteLine("Idle command gap is more than 70 milliseconds: " + diff);
#endif
							}

							previousIdleCommandTime = idleCommandTime;

							//Trace.WriteLineIf(traceCommandStationManager.TraceVerbose, " CommandManagerThread: Found command in idle commands queue");
						}

						// No command is available, reset the work to do event
						if(command == null) {
							//Trace.WriteLineIf(traceCommandStationManager.TraceVerbose, " CommandManagerThread: Command not found, reset work to do event");
							workToDo.Reset();
							nothingToDo.Set();
						}
					}

					if(command != null) {
						IOutputIdlecommand idleCommand = command as IOutputIdlecommand;

						if(idleCommand == null || traceOutputManager.TraceVerbose)
							Trace.WriteLineIf(traceOutputManager.TraceInfo, " CommandManagerThread: execute command " + command.GetType().Name + ": " + command.ToString());

						IOutputCommandWithReply commandWithReply = command as IOutputCommandWithReply;

						if(commandWithReply != null)
							pendingCommandWithReply = commandWithReply;

						command.Do();
						Trace.WriteLineIf(traceOutputManager.TraceVerbose, " CommandManagerThread: command done " + command.GetType().Name);

						if(commandWithReply != null) {
							bool timeout = !waitToGetReply.WaitOne(commandWithReply.Timeout);

							waitToGetReply.Reset();
							pendingCommandWithReply = null;

							if(timeout)
								commandWithReply.OnTimeout();

						}
						else if(idleCommand == null)
							command.Completed(null);

						if(idleCommand != null && !idleCommand.RemoveFromQueue)
							AddIdleCommand(idleCommand);
					}
				}
			} catch(ThreadAbortException) {
				commandManagerThread = null;
			}

			Trace.WriteLineIf(traceOutputManager.TraceInfo, "CommandManagerThread: Terminated");
		}

		#endregion

		class CommandManagerQueue : Queue<IOutputCommand> {
			ManualResetEvent	workToDo;
			Timer				queueSuspendedTimer;
			object				sync;

			public CommandManagerQueue(ManualResetEvent workToDo, object sync) {
				this.workToDo = workToDo;
				this.sync = sync;
			}

			public bool IsCommandReady {
				get {
					return queueSuspendedTimer == null && Count > 0;
				}
			}

			public IOutputCommand GetCommand() {
				IOutputCommand command = null;

				if(!IsCommandReady)
					return null;

				command = Dequeue();

				if(command.WaitPeriod > 0) {
					lock(sync) {
						queueSuspendedTimer = new Timer(new TimerCallback(enableQueue), null, command.WaitPeriod, Timeout.Infinite);
					}
				}

				return command;
			}

			private void enableQueue(object state) {
				lock(sync) {
					if(queueSuspendedTimer != null)
						queueSuspendedTimer.Dispose();
					queueSuspendedTimer = null;

					if(Count > 0)
						workToDo.Set();
				}
			}
		}
	}

	#endregion

	#region Classes used turnout switching manager

	/// <summary>
	/// A structure holding a switching command for a multi-path component
	/// </summary>
	public class SwitchingCommand {
		ControlConnectionPointReference controlPointReference;
		int								switchState;

		/// <summary>
		/// Initialize a new switching command structure
		/// </summary>
		/// <param name="controlPointReference">The control connection point to set</param>
		/// <param name="switchState">The switching state to set this connection point to</param>
		public SwitchingCommand(ControlConnectionPointReference controlPointReference, int switchState) {
			this.controlPointReference = controlPointReference;
			this.switchState = switchState;
		}

		/// <summary>
		/// The switched turnout
		/// </summary>
		public ControlConnectionPointReference ControlPointReference {
			get {
				return controlPointReference;
			}
		}

		/// <summary>
		/// The required switch state
		/// </summary>
		public int SwitchState {
			get {
				return switchState;
			}
		}


		public IModelComponentIsMultiPath Turnout {
			get {
				return controlPointReference.ConnectionPoint.Component as IModelComponentIsMultiPath;
			}
		}

		/// <summary>
		/// The command station controlling the turnout
		/// </summary>
		public IModelComponentIsBusProvider BusProvider {
			get {
				return controlPointReference.Module.Bus.BusProvider;
			}
		}

		/// <summary>
		/// The ID of the command station associated with this command station
		/// </summary>
		public Guid CommandStationId {
			get {
				return controlPointReference.Module.Bus.BusProviderId;
			}
		}
	}

	#endregion
}
