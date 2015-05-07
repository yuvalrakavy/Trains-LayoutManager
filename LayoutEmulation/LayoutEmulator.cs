using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Threading;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutEmulation {

	#region Interfaces

	/// <summary>
	/// The ILayoutEmulationEnvironment is used by internal classes to access common services
	/// </summary>
	/// 
	public interface ILayoutEmulationEnvironment {
		/// <summary>
		/// Number of speed steps for the emulated locomotives
		/// </summary>
		int	SpeedSteps { get; }

		/// <summary>
		/// Number of timer ticks each speed steps adds to the time between motions
		/// </summary>
		int	TicksPerSpeedStep { get; }

		/// <summary>
		/// Object used for synchronizing access to the emulation data structures
		/// </summary>
		object Sync { get; }

		/// <summary>
		/// Topology services
		/// </summary>
		ILayoutTopologyServices TopologyServices { get; }

		////// Operations
		///
		/// <summary>
		/// Return the connection point to which split point from is to be connected. This is based on the current setting of the turnout
		/// </summary>
		LayoutComponentConnectionPoint FindRouteThroughMultiPathComponent(IModelComponentIsMultiPath turnout, LayoutComponentConnectionPoint from);

		////// Callback methods - called from operations in order to report events

		/// <summary>
		/// Callback that is called if the locomotive fall from the track
		/// </summary>
		/// <param name="locomotive"></param>
		void FireLocomotiveFallFromTrack(LocomotiveState locomotive, LocomotiveOrientation direction, int speed);

		/// <summary>
		/// Called when a locomotive is moved
		/// </summary>
		/// <param name="locomotive">The locomotive</param>
		/// <param name="location">The new location, connection point is the front</param>
		/// <param name="rear">The rear of the locomotive</param>
		void FireLocomotiveMoved(LocomotiveState locomotive, LocomotiveOrientation direction, int speed);
	}

	#endregion

	[LayoutModule("Layout Emulator")]
	public class LayoutEmulator : LayoutModuleBase, ILayoutEmulationEnvironment, ILayoutEmulatorServices {
		ILayoutTopologyServices	_topologyServices;
		Dictionary<Guid, CommandStationObjects> commandStations = new Dictionary<Guid,CommandStationObjects>();
		IDictionary				turnouts = new Hashtable();						// Map from turnout ID to turnout state
		Timer					emulationTimer = null;
		int						operationNesting = 0;
		int						tickTime = 200;

		// The initialize method must run in the context of the main thread
		void Initialize() {
		}


		#region Implementation of ILayoutEmulationEnvironment

		#region Properties

		public ILayoutTopologyServices TopologyServices {
			get {
				if(_topologyServices == null)
					_topologyServices = (ILayoutTopologyServices)EventManager.Event(new LayoutEvent(this, "get-topology-services"));
				return _topologyServices;
			}
		}

        public int SpeedSteps => 15;            // 0 - 14

        public int TicksPerSpeedStep => 1;

        public object Sync => this;

        #endregion

        #region Operations


        public LayoutComponentConnectionPoint FindRouteThroughMultiPathComponent(IModelComponentIsMultiPath turnout, LayoutComponentConnectionPoint from) {
            int switchState = 0;

            if(turnout is LayoutThreeWayTurnoutComponent) {
                int stateRight = GetConnectionPointSwitchState(LayoutModel.ControlManager.ConnectionPoints[turnout, "ControlRight"]);
                int stateLeft = GetConnectionPointSwitchState(LayoutModel.ControlManager.ConnectionPoints[turnout, "ControlLeft"]);

                if(stateRight == 0 && stateLeft == 0)
                    switchState = 0;
                else if(stateRight == 1 && stateLeft == 0)
                    switchState = 1;
                else if(stateRight == 0 && stateLeft == 1)
                    switchState = 2;
            }
            else {
                ControlConnectionPoint connectionPoint = LayoutModel.ControlManager.ConnectionPoints[turnout][0];

                switchState = GetConnectionPointSwitchState(connectionPoint);
            }

            if(switchState < 0)
                return turnout.ConnectTo(from, LayoutComponentConnectionType.Passage)[0];
            else {
                if(turnout is IModelComponentHasReverseLogic && ((IModelComponentHasReverseLogic)turnout).ReverseLogic)
                    switchState = 1 - switchState;

                return turnout.ConnectTo(from, switchState);
            }
        }

        private int GetConnectionPointSwitchState(ControlConnectionPoint connectionPoint) {
            int address = connectionPoint.Module.Address + connectionPoint.Index;

            int switchState = GetTurnoutSwitchState(connectionPoint.Module.Bus.BusProviderId, address);
            return switchState;
        }

		#endregion

		#region Callbacks

		public void FireLocomotiveFallFromTrack(LocomotiveState locomotive, LocomotiveOrientation direction, int speed) {
			if(LocomotiveFallFromTrack != null)
				LocomotiveFallFromTrack(this, new LocomotiveMovedEventArgs(locomotive.CommandStationId, locomotive.Unit, locomotive.Location, direction, speed));
		}

		public void FireLocomotiveMoved(LocomotiveState locomotive, LocomotiveOrientation direction, int speed) {
			if(LocomotiveMoved != null)
				LocomotiveMoved(this, new LocomotiveMovedEventArgs(locomotive.CommandStationId, locomotive.Unit, locomotive.Location, direction, speed));
		}

		#endregion

		#endregion

		#region Implementation of ILayoutEmulationServices

		public void StartEmulation() {
			lock(Sync) {
				if(operationNesting == 0) {
					emulationTimer = new Timer(new TimerCallback(this.onEmulationTick), null, 0, tickTime);
					operationNesting++;
				}
			}
		}

		public void StopEmulation() {
			lock(Sync) {
				if(--operationNesting == 0) {
					emulationTimer.Dispose();
					emulationTimer = null;
				}
			}
		}

		public void SetLocomotiveSpeed(Guid commandStationId, int unit, int speed) {
			lock(Sync) {
				LocomotiveState	locomotive = GetLocomotive(commandStationId, unit);

				if(locomotive != null)
					locomotive.Speed = speed;
			}
		}

		public void SetLocomotiveDirection(Guid commandStationId, int unit, LocomotiveOrientation direction) {
			lock(Sync) {
				LocomotiveState	locomotive = GetLocomotive(commandStationId, unit);

				if(locomotive != null)
					locomotive.Direction = direction;
			}
		}

		public void ToggleLocomotiveDirection(Guid commandStationId, int unit) {
			lock(Sync) {
				LocomotiveState	locomotive = GetLocomotive(commandStationId, unit);

				locomotive.Direction = (locomotive.Direction == LocomotiveOrientation.Forward) ? LocomotiveOrientation.Backward : LocomotiveOrientation.Forward;
			}
		}

		CommandStationObjects GetCommandStationObjects(Guid commandStationId) {
			CommandStationObjects	commandStationObjects;
			
			commandStations.TryGetValue(commandStationId, out commandStationObjects);

			if(commandStationObjects == null) {
				commandStationObjects = new CommandStationObjects(this, commandStationId);

				commandStations[commandStationId] = commandStationObjects;
			}

			return commandStationObjects;
		}

		public void SetTurnoutState(Guid commandStationId, int unit, int switchState) {
			lock(Sync) {
				CommandStationObjects	commandStationObjects = GetCommandStationObjects(commandStationId);

				commandStationObjects.SetTurnoutSwitchState(unit, switchState);
			}
		}

		public ILocomotiveLocation GetLocomotiveLocation(Guid commandStationId, int unit) {
			lock(Sync) {
				LocomotiveState	locomotive = GetLocomotive(commandStationId, unit);

				if(locomotive == null) {
					var commandStation = LayoutModel.Component<IModelComponentIsCommandStation>(commandStationId, LayoutModel.ActivePhases);
					var commandStationName = commandStation != null ? commandStation.Name : "**UNDEFINED COMMAND STATION**";

					throw new ArgumentException("GetLocomotiveLocation of undefined locomotive (commandStation: " + commandStationName + ", unit: " + unit + ")");
				}

				return locomotive.Location;
			}
		}

		public IList<ILocomotiveLocation> GetLocomotiveLocations(Guid commandStationId) {
			lock(Sync) {
				CommandStationObjects	commandStationObjects;

				commandStations.TryGetValue(commandStationId, out commandStationObjects);

				if(commandStationObjects == null)
					return Array.AsReadOnly<ILocomotiveLocation>(new LocomotiveLocation[0]);
				else
					return commandStationObjects.GetLocomotiveLocations();
			}
		}

		public event EventHandler<LocomotiveMovedEventArgs> LocomotiveMoved;
		public event EventHandler<LocomotiveMovedEventArgs> LocomotiveFallFromTrack;

		#endregion

		#region Helper methods

		public LocomotiveState AddLocomotive(Guid commandStationId, int unit, TrackEdge location) {
			CommandStationObjects	commandStationObjects = GetCommandStationObjects(commandStationId);

			return commandStationObjects.AddLocomotive(unit, location);
		}

		public LocomotiveState GetLocomotive(Guid commandStationId, int unit) {
			LocomotiveState		locomotive = null;

			var commandStationObjects = commandStations[commandStationId];

			if(commandStationObjects != null)
				locomotive = commandStationObjects.GetLocomotive(unit);

			return locomotive;
		}

		private void RemoveLocomotive(Guid commandStationId, int unit) {
			CommandStationObjects	commandStationObjects;
			
			if(commandStations.TryGetValue(commandStationId, out commandStationObjects))
				commandStationObjects.RemoveLocomotive(unit);
		}

		private int GetTurnoutSwitchState(Guid commandStationId, int unit) {
			var commandStationObjects = commandStations[commandStationId];

			if(commandStationObjects != null)
				return commandStationObjects.GetTurnoutSwitchState(unit);
			return -1;
		}

		private void ResetEmulator() {
			lock(Sync) {
				commandStations.Clear();

				foreach(XmlElement trainStateElement in LayoutModel.StateManager.Trains.Element) {
					TrainStateInfo	train = new TrainStateInfo(trainStateElement);
					Guid commandStationId = train.CommandStation.Id;

					foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
						LocomotiveInfo			loco = trainLocomotive.Locomotive;
						LayoutTrackComponent	locoTrack = train.LocomotiveLocation.Block.BlockDefinintion.Track;
						ILayoutPower			locoPower = locoTrack.GetPower(locoTrack.ConnectionPoints[0]);

						LocomotiveState locomotive = AddLocomotive(commandStationId, loco.AddressProvider.Unit, 
							new TrackEdge(train.LocomotiveBlock.BlockDefinintion.Track, train.LocomotiveLocation.DisplayFront));

						locomotive.Direction = train.MotionDirection;
						if(trainLocomotive.Orientation == LocomotiveOrientation.Backward)
							locomotive.Direction = (locomotive.Direction == LocomotiveOrientation.Forward ? LocomotiveOrientation.Backward : 
								LocomotiveOrientation.Forward);
					}
				}

				foreach(IModelComponentIsMultiPath turnout in LayoutModel.Components<IModelComponentIsMultiPath>(LayoutModel.ActivePhases)) {
					if(LayoutModel.StateManager.Components.Contains(turnout.Id, "SwitchState")) {
						ControlConnectionPoint	connectionPoint = LayoutModel.ControlManager.ConnectionPoints[turnout][0];
						int						address = connectionPoint.Module.Address + connectionPoint.Index;

						SetTurnoutState(connectionPoint.Module.Bus.BusProviderId, address, turnout.CurrentSwitchState);
					}
				}
			}
		}

		private void onEmulationTick(object state) {
			lock(Sync) {
				foreach(CommandStationObjects commandStationObjects in commandStations.Values)
					commandStationObjects.Tick();
			}
		}

		#endregion

		#region External Operations

		[LayoutEvent("initialize-layout-emulation")]
		private void startLayoutEmulation(LayoutEvent e) {
			tickTime = (int)e.Info;
		}

		[LayoutEvent("get-layout-emulation-services")]
		private void getLayoutEmulationServices(LayoutEvent e) {
			e.Info = (ILayoutEmulatorServices)this;
		}

		[LayoutEvent("reset-layout-emulation")]
		private void resetLayoutEmulation(LayoutEvent e) {
			ResetEmulator();
		}

		#endregion

		#region Trapped events

		[LayoutEvent("train-created")]
		private void trainCreated(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;

			lock(Sync) {
				if(train.CommandStation != null) {
					Guid commandStationId = train.CommandStation.Id;

					foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
						LocomotiveInfo loco = trainLocomotive.Locomotive;

						AddLocomotive(commandStationId, loco.AddressProvider.Unit, new TrackEdge(train.LocomotiveBlock.BlockDefinintion.Track,
							train.LocomotiveLocation.DisplayFront));
					}
				}
			}
		}

		[LayoutEvent("train-relocated")]
		private void trainRelocated(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;

			lock(Sync) {
				if(train.CommandStation != null) {
					var commandStationId = train.CommandStation.Id;

					foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
						LocomotiveState locomotive = GetLocomotive(commandStationId, trainLocomotive.Locomotive.AddressProvider.Unit);

						if(locomotive != null)
							locomotive.Location = new LocomotiveLocation(new TrackEdge(train.LocomotiveBlock.BlockDefinintion.Track, train.LocomotiveLocation.DisplayFront));
					}
				}
			}
		}

		[LayoutEvent("train-is-removed")]
		private void trainRemoved(LayoutEvent e) {
			TrainStateInfo	train = (TrainStateInfo)e.Sender;

			lock(Sync) {
				if(train.CommandStation != null) {
					var commandStationId = train.CommandStation.Id;

					foreach(TrainLocomotiveInfo trainLocomotive in train.Locomotives) {
						LocomotiveInfo loco = trainLocomotive.Locomotive;

						RemoveLocomotive(commandStationId, loco.AddressProvider.Unit);
					}
				}
			}
		}

		#endregion
	}

	#region Data structures

	class CommandStationObjects {
		ILayoutEmulationEnvironment	environment;
		Guid commandStationId;
		Dictionary<int, LocomotiveState> locomotives = new Dictionary<int,LocomotiveState>();
		Dictionary<int, int>		turnouts = new Dictionary<int,int>();

		public CommandStationObjects(ILayoutEmulationEnvironment environment, Guid commandStationId) {
			this.environment = environment;
			this.commandStationId = commandStationId;
		}

		public LocomotiveState AddLocomotive(int unit, TrackEdge location) {
			LocomotiveState	locomotive;

			locomotives.TryGetValue(unit, out locomotive);

			if(locomotive == null) {
				locomotive = new LocomotiveState(environment, commandStationId, unit, location);
				locomotives.Add(unit, locomotive);
			}
			else
				locomotive.Location = new LocomotiveLocation(location);

			return locomotive;
		}

		public LocomotiveState GetLocomotive(int unit) {
            LocomotiveState loco;

            if(!locomotives.TryGetValue(unit, out loco))
                return null;
            return loco;
		}

		public void RemoveLocomotive(int unit) {
			locomotives.Remove(unit);
		}

		public IList<ILocomotiveLocation> GetLocomotiveLocations() {
			LocomotiveLocation[]	locations = new LocomotiveLocation[locomotives.Count];

			int	i = 0;
			foreach(LocomotiveState locomotive in locomotives.Values)
				locations[i++] = locomotive.Location;

			return Array.AsReadOnly<ILocomotiveLocation>(locations);
		}

		public void SetTurnoutSwitchState(int unit, int switchState) {
			turnouts[unit] = switchState;
		}

		public int GetTurnoutSwitchState(int unit) {
			int switchState;

			if(turnouts.TryGetValue(unit, out switchState))
				return switchState;
			return -1;
		}

		// Apply emulation "tick" on each locomotive
		public void Tick() {
			foreach(LocomotiveState locomotive in locomotives.Values)
				locomotive.Tick();
		}
	}

	public class LocomotiveLocation : ILocomotiveLocation {
		LayoutTrackComponent	_track;
		LayoutComponentConnectionPoint _front;
		LayoutComponentConnectionPoint _rear;

		public LocomotiveLocation(LayoutTrackComponent track, LayoutComponentConnectionPoint front, LayoutComponentConnectionPoint rear) {
			_track = track;
			_front = front;
			_rear = rear;
		}

		public LocomotiveLocation(TrackEdge edge) {
			_track = edge.Track;
			_front = edge.ConnectionPoint;
			_rear = _track.ConnectTo(_front, LayoutComponentConnectionType.Passage)[0];
		}

        public LayoutTrackComponent Track => _track;

        public LayoutComponentConnectionPoint Front => _front;

        public LayoutComponentConnectionPoint Rear => _rear;

        public TrackEdge Edge => new TrackEdge(_track, _front);
    }

	public class LocomotiveState {
		ILayoutEmulationEnvironment _environment;
		Guid _commandStationId;
		int _unit;
		int _speed;				// the locomotive current speed (and direction)
		LocomotiveLocation _location;			// Where the locomotive is on the layout (when there is support for longer trains, this will become an array)
//		LayoutComponentConnectionPoint	rear;				// Where is the rear of the train
		LocomotiveOrientation _direction = LocomotiveOrientation.Forward;

		int			ticksPerMotion;		// Number of timer ticks between each locomotive motion
		int			tickCount;			// Number of ticks remaining until the next locomotive motion

		public LocomotiveState(ILayoutEmulationEnvironment environment, Guid commandStationId, int unit, TrackEdge edge) {
			this._environment = environment;
			this._commandStationId = commandStationId;
			this._unit = unit;

			_location = new LocomotiveLocation(edge);
			Speed = 0;
		}

        public Guid CommandStationId => _commandStationId;

        public int Unit => _unit;

        public int Speed {
			get {
				return _speed;
			}

			set {
				lock(_environment.Sync) {
					int	previousSpeed = this._speed;

					this._speed = value;

					if(_speed == 0) {
						ticksPerMotion = 0;
						tickCount = 0;
					}
					else {
						ticksPerMotion = (_environment.SpeedSteps - Math.Abs(_speed)) * _environment.TicksPerSpeedStep;

						if(tickCount > ticksPerMotion)
							tickCount = ticksPerMotion;
					}

					if(previousSpeed == 0 && _speed != 0)
						_environment.FireLocomotiveMoved(this, _direction, _speed);
				}
			}
		}

		public LocomotiveOrientation Direction {
			get {
				return _direction;
			}

			set {
				lock(_environment.Sync) {
					_direction = value;
				}
			}
		}

		public LocomotiveLocation Location {
			get {
				return _location;
			}

			set {
				lock(_environment.Sync) {
					_location = value;
					Debug.Assert(_location.Track is LayoutStraightTrackComponent);
//					rear = location.Track.ConnectTo(location.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];
				}
			}
		}

		public void Tick() {
			if(ticksPerMotion > 0) {
				if(--tickCount <= 0) {
					TrackEdge	nextEdge;

					// Locomotive needs to move
					if(_direction == LocomotiveOrientation.Forward)
						nextEdge = _environment.TopologyServices.FindTrackConnectingAt(_location.Edge);
					else
						nextEdge = _environment.TopologyServices.FindTrackConnectingAt(new TrackEdge(_location.Track, _location.Rear));

					if(nextEdge == TrackEdge.Empty) {
						_environment.FireLocomotiveFallFromTrack(this, _direction, _speed);
						Speed = 0;
					}
					else {
						LayoutComponentConnectionPoint	cp;

						if(nextEdge.Track is IModelComponentIsMultiPath) {
							IModelComponentIsMultiPath	turnout = (IModelComponentIsMultiPath)nextEdge.Track;

							if(turnout.IsSplitPoint(nextEdge.ConnectionPoint))
								cp = _environment.FindRouteThroughMultiPathComponent(turnout, nextEdge.ConnectionPoint);
							else
								cp = nextEdge.Track.ConnectTo(nextEdge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];
						}
						else
							cp = nextEdge.Track.ConnectTo(nextEdge.ConnectionPoint, LayoutComponentConnectionType.Passage)[0];

						if(Direction == LocomotiveOrientation.Forward)
							_location = new LocomotiveLocation(nextEdge.Track, cp, nextEdge.ConnectionPoint);
						else
							_location = new LocomotiveLocation(nextEdge.Track, nextEdge.ConnectionPoint, cp);

						_environment.FireLocomotiveMoved(this, _direction, _speed);
					}

					tickCount = ticksPerMotion;
				}
			}
		}
	}

	#endregion

}
