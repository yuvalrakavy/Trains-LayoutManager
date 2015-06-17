using System;
using System.Collections.Generic;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Model {

	public class LocomotiveMovedEventArgs : EventArgs {
		public Guid CommandStationId { get; }
        public int Unit { get; }
        public ILocomotiveLocation Location { get; }
        public LocomotiveOrientation Direction { get; }
        public int Speed { get; }

        public LocomotiveMovedEventArgs(Guid commandStationId, int unit, ILocomotiveLocation location, LocomotiveOrientation direction, int speed) {
			CommandStationId = commandStationId;
			Unit = unit;
			Location = location;
			Direction = direction;
			Speed = speed;
		}
	}

	public interface ILocomotiveLocation {
		LayoutTrackComponent Track { get; }

		LayoutComponentConnectionPoint Front { get; }

		LayoutComponentConnectionPoint Rear { get; }

		TrackEdge Edge { get; }
	}

	/// <summary>
	/// This interface is used by interface emulator to communicate with the layout emulator. Unless other specified all those
	/// methods are thread safe and can be called by any thread
	/// </summary>
	public interface ILayoutEmulatorServices {
		/// <summary>
		/// Start emulation - this method starts layout emulation
		/// </summary>
		void StartEmulation();

		/// <summary>
		/// Stop (suspend) the layout emulation, the layout emulation can be resumed by calling StartEmulation
		/// </summary>
		void StopEmulation();

		/// <summary>
		/// Set the speed of a given locomotive
		/// </summary>
		/// <param name="locomotiveID">The ID of the locomotive whose speed it to be set</param>
		/// <param name="speed">The locomotive speed (0..NumberOfSpeedSteps-1)</param>
		void SetLocomotiveSpeed(Guid commandStationId, int unit, int speed);

		/// <summary>
		/// Set the direction of a given locomotive
		/// </summary>
		/// <param name="locomotiveID">The locomotive whose direction is to be set</param>
		/// <param name="direction">The new direction</param>
		void SetLocomotiveDirection(Guid commandStationId, int unit, LocomotiveOrientation direction);

		/// <summary>
		/// Toggle the direction of a given locomotive
		/// </summary>
		/// <param name="commandStation"></param>
		/// <param name="unit"></param>
		void ToggleLocomotiveDirection(Guid commandStationId, int unit);

		/// <summary>
		/// Set the current switch state for a turnout
		/// </summary>
		/// <param name="commandStation">Command station controlling the turnout</param>
		/// <param name="unit">The turnout unit address</param>
		/// <param name="switchState"></param>
		void SetTurnoutState(Guid commandStationId, int unit, int switchState);

		/// <summary>
		/// Get the current location of a given locomotive
		/// </summary>
		ILocomotiveLocation GetLocomotiveLocation(Guid commandStationId, int unit);

		/// <summary>
		/// Get an array with the locations with tracks that are occupied by all the locomotives controlled via the given command station
		/// </summary>
		IList<ILocomotiveLocation> GetLocomotiveLocations(Guid commandStationId);

		/// <summary>
		/// An event to be called whenever a locomotive is moved
		/// </summary>
		event EventHandler<LocomotiveMovedEventArgs> LocomotiveMoved;

		/// <summary>
		/// An event that is invoked when an emulated locomotive fall from the edge of a track
		/// </summary>
		event EventHandler<LocomotiveMovedEventArgs> LocomotiveFallFromTrack;
	}
}
