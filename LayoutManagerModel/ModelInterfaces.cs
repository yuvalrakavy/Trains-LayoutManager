using System;
using System.Drawing;
using System.Collections.Generic;

using LayoutManager.Components;

namespace LayoutManager.Model {

	public interface IModelComponent : IObjectHasXml {
		/// <summary>
		/// Find out if another component is "above" this component
		/// </summary>
		/// <param name="objModelComponent">The component to compare to</param>
		/// <returns>+n if this component is above, 0 if same, -n if below</returns>
		int CompareTo(ModelComponent other);

		/// <summary>
		/// Called to erase this component image. This is needed if the component draw outside its
		/// grid location, and the image will be modified.
		/// </summary>
		void EraseImage();

		/// <summary>
		/// Generate error message in the messages window
		/// </summary>
		void Error(String message);
		void Error(Object subject, String message);

		/// <summary>
		/// Return a full description text for this component, useful for debugging output
		/// </summary>
		String FullDescription { get; }

		/// <summary>
		/// Return whether this component has associated attributes
		/// </summary>
		bool HasAttributes { get; }

		/// <summary>
		/// Return the kind of this component. Each layer in a given spot may
		/// contain at most one component of a given kind
		/// </summary>
		ModelComponentKind Kind { get; }

		/// <summary>
		/// The location (x, y) in which this component is located in the model
		/// grid.
		/// </summary>
		Point Location { get; }

		/// <summary>
		/// Cause the component to redraw itself (send event to all views)
		/// </summary>
		void Redraw();

		/// <summary>
		/// Should be called when component runtime state changes.
		/// </summary>
		void OnComponentChanged();

		/// <summary>
		/// The in which this component is located on the layout model grid.
		/// Using the spot, you can gain access to other components which are
		/// located in other layers or of another kind in this location
		/// </summary>
		LayoutModelSpotComponentCollection Spot { get; }

		/// <summary>
		/// Generate wanring message in the messages window
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="message"></param>
		void Warning(Object subject, String message);
		void Warning(String message);

		/// <summary>
		/// Return the Z order of this component. When presented, components with smaller Z order
		/// should be shown behind those of a larger Z order.
		/// </summary>
		int ZOrder { get; }
	}

	public interface IModelComponentHasId : IModelComponent, IObjectHasId {
	};

	public interface IModelComponentHasAttributes : IModelComponent, IObjectHasAttributes {
	};

	/// <summary>
	/// If a component has a name, it should implement this interface
	/// </summary>
	public interface IModelComponentHasName : IModelComponent, IObjectHasName {
	}

	public interface IModelComponentHasNameAndId : IModelComponent, IModelComponentHasName, IModelComponentHasId {
	}

	public interface IModelComponentCanProgramLocomotives : IModelComponentHasId, IModelComponentHasName, IModelComponentHasPowerOutlets {
		/// <summary>
		/// Does this locomotive programmer support programming on main programming method
		/// </summary>
		bool POMprogrammingSupported { get; }
	}

	public interface IModelComponentIsBusProvider : IModelComponentHasNameAndId {
		/// <summary>
		/// The bus types that this component connects to
		/// </summary>
		IList<string> BusTypeNames { get; }

		/// <summary>
		/// Does this bus provider supports processing of a batch of multipath (e.g. turnout) switching
		/// </summary>
		bool BatchMultipathSwitchingSupported { get; }
	}

	public interface IModelComponentIsCommandStation : IModelComponentHasId, IModelComponentHasName, IModelComponentIsBusProvider, IModelComponentHasPowerOutlets {
		/// <summary>
		/// The command station name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Is layout emulation supported (operation when off-line)
		/// </summary>
		/// <value>True if yes</value>
		bool LayoutEmulationSupported { get; }

		/// <summary>
		/// If design time layout activation supported
		/// </summary>
		/// <value>true if yes</value>
		bool DesignTimeLayoutActivationSupported { get;	}

		/// <summary>
		/// Does command station supports train analysis. This is a phase where train position is analyzed
		/// </summary>
		bool TrainsAnalysisSupported { get; }

		/// <summary>
		/// Allow relaying events to main UI thread
		/// </summary>
		ILayoutInterThreadEventInvoker InterThreadEventInvoker { get; }

		/// <summary>
		/// True if command station is in operation mode
		/// </summary>
		bool OperationMode { get; }

		/// <summary>
		/// Lowest locomotive address supported by command station
		/// </summary>
		int GetLowestLocomotiveAddress(DigitalPowerFormats format);

		/// <summary>
		/// Highest locomotive address supported by command station
		/// </summary>
		int GetHighestLocomotiveAddress(DigitalPowerFormats format);
	}

	/// <summary>
	/// This interface is implemented by a component that "produce" power (for example, command station).
	/// </summary>
	public interface IModelComponentHasPowerOutlets : IModelComponent, IModelComponentHasNameAndId, IObjectHasXml {
		/// <summary>
		/// Return an array of all the power sources that this component generates
		/// </summary>
		IList<ILayoutPowerOutlet> PowerOutlets {
			get;
		}
	}

	/// <summary>
	/// A model component that need to be exclusively allocated to train should implement this
	/// interface
	/// </summary>
	public interface IModelComponentLayoutLockResource : ILayoutLockResource, IModelComponentHasId, IModelComponentHasName {
	};

	public interface IModelComponentToControlConnection {
	}

	public interface IModelComponentConnectToControl : IModelComponentHasId {

		/// <summary>
		/// Description of connections that are supported by this component
		/// </summary>
		/// <value></value>
		IList<ModelComponentControlConnectionDescription> ControlConnectionDescriptions {
			get;
		}

		/// <summary>
		/// True if this component is fully connected
		/// </summary>
		bool FullyConnected { get; }

		/// <summary>
		/// If true, the component is connected
		/// </summary>
		bool IsConnected { get; }
	}

	/// <summary>
	/// A component has a state which is control by a control module. For example turnouts, power selector etc.
	/// </summary>
	public interface IModelComponentHasSwitchingState : IModelComponentHasId {
		/// <summary>
		/// The current switching state
		/// </summary>
		int CurrentSwitchState {
			get;
		}

		/// <summary>
		/// Change switch state without triggering layout commands. For example, call this method when the hardware sends notification
		/// that the state was changed
		/// </summary>
		/// <param name="switchState"></param>
		void SetSwitchState(ControlConnectionPoint connectionPoint, int switchState);

		/// <summary>
		/// Number of possible switch states
		/// </summary>
		/// <value></value>
		int SwitchStateCount {
			get;
		}

		/// <summary>
		/// Add the necessary control point setting for switching the component to a given switching state
		/// </summary>
		/// <param name="switchingCommands">The list of switching commands</param>
		/// <param name="switchingState">The target switching state</param>
		/// <remarks>
		/// In simple components like two way turnout, a single switching command is added, however, in a more complex
		/// component like three way turnouts (that is controlled by two solenoids) more than one switching command
		/// may be added
		/// </remarks>
		void AddSwitchingCommands(IList<SwitchingCommand> switchingCommands, int switchingState);
	}

	/// <summary>
	/// A multi path component is any component that a connection point may connect to more than one other connection point.
	/// A classic example is turnout.
	/// </summary>
	public interface IModelComponentIsMultiPath : IModelComponent, IModelComponentHasId, IModelComponentHasSwitchingState, ILayoutConnectableComponent, IModelComponentConnectToControl {
		/// <summary>
		/// Return the switch state that should be used in order to connect cpSource to cpDest
		/// </summary>
		/// <param name="cpSource">Source connection point</param>
		/// <param name="cpDestination">Destination connection point</param>
		/// <returns></returns>
		int GetSwitchState(LayoutComponentConnectionPoint cpSource, LayoutComponentConnectionPoint cpDestination);

		/// <summary>
		/// To what connection point a given connection point will connect given a switch state
		/// </summary>
		LayoutComponentConnectionPoint ConnectTo(LayoutComponentConnectionPoint cp, int switchState);

		/// <summary>
		/// Return true if the connection point may connect to more than one other connection point. For example
		/// the tip of a turnout
		/// </summary>
		/// <param name="cp">The connection point</param>
		/// <returns>True if the connection point is a split point</returns>
		bool IsSplitPoint(LayoutComponentConnectionPoint cp);

		/// <summary>
		/// Return true if the connection point into another path. For example, the straigt and branch of a turnout
		/// will return true.
		/// </summary>
		/// <param name="cp">The connection point</param>
		/// <returns>True if the connection point is a merge point</returns>
		bool IsMergePoint(LayoutComponentConnectionPoint cp);
	}

	public interface IModelComponentHasReverseLogic : IModelComponent {
		bool ReverseLogic {
			get;
		}
	}

	public interface IModelComponentIsDualState : IModelComponentIsMultiPath {
	}

	public interface IHasDecoder {
		DecoderTypeInfo DecoderType { get; }
	}
}