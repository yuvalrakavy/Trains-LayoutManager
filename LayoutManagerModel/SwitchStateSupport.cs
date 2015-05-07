using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Components {
	public class SwitchingStateSupport {
		public string StateTopic { get; private set; }
		public IModelComponentHasSwitchingState Component { get; private set; }
		public virtual int SwitchStateCount { get; private set; }

		public SwitchingStateSupport(IModelComponentHasSwitchingState component, string stateTopic = "SwitchState", int switchStateCount = 2) {
			this.Component = component;
			this.StateTopic = stateTopic;
			this.SwitchStateCount = switchStateCount;
		}

		public virtual int CurrentSwitchState {
			get {
				if(LayoutModel.StateManager.Components.Contains(Component.Id, StateTopic))
					return XmlConvert.ToInt32(LayoutModel.StateManager.Components.StateOf(Component.Id, StateTopic).GetAttribute("Value"));
				else
					return 0;
			}
		}

		public virtual void AddSwitchingCommands(IList<SwitchingCommand> switchingCommands, int switchingState) {
			if(switchingState != 0 && switchingState != 1)
				throw new LayoutException(this, "Invalid switch state: " + switchingState);

			int state = switchingState;

			if(ReverseLogic)
				state = 1 - state;

			ControlConnectionPoint connectionPoint = LayoutModel.ControlManager.ConnectionPoints[Component.Id][0];

			switchingCommands.Add(new SwitchingCommand(new ControlConnectionPointReference(connectionPoint), state));
		}

		/// <summary>
		/// This method actually change the run-time state. It should be called only from the state changed
		/// notification handler, and not directly.
		/// </summary>
		/// <param name="switchState">The new switch state</param>
		public virtual void SetSwitchState(ControlConnectionPoint controlConnectionPoint, int switchState) {
			LayoutModel.StateManager.Components.StateOf(Component.Id, StateTopic).SetAttribute("Value", XmlConvert.ToString(switchState));
			Component.OnComponentChanged();
		}

		public bool ReverseLogic {
			get {
				IModelComponentHasReverseLogic componentWithReverseLogic = Component as IModelComponentHasReverseLogic;

				if(componentWithReverseLogic != null)
					return componentWithReverseLogic.ReverseLogic;
				else
					return false;
			}
		}
	}

	public abstract class ModelComponentWithSwitchingState : ModelComponent, IModelComponentHasSwitchingState {
		SwitchingStateSupport switchingStateSupport;

		public ModelComponentWithSwitchingState() {
			switchingStateSupport = GetSwitchingStateSupporter();
		}

        protected virtual SwitchingStateSupport GetSwitchingStateSupporter() => new SwitchingStateSupport(this);

        #region IModelComponentHasSwitchingState Members

        public int CurrentSwitchState => switchingStateSupport.CurrentSwitchState;

        public void SetSwitchState(ControlConnectionPoint connectionPoint, int switchState) {
			switchingStateSupport.SetSwitchState(connectionPoint, switchState);
		}

        public int SwitchStateCount => switchingStateSupport.SwitchStateCount;

        public void AddSwitchingCommands(IList<SwitchingCommand> switchingCommands, int switchingState) {
			switchingStateSupport.AddSwitchingCommands(switchingCommands, switchingState);
		}

		#endregion

		#region IModelComponentHasReverseLogic Members

		public bool ReverseLogic {
			get {
				if(Element.HasAttribute("ReverseLogic"))
					return XmlConvert.ToBoolean(Element.GetAttribute("ReverseLogic"));
				return false;
			}

			set {
				Element.SetAttribute("ReverseLogic", XmlConvert.ToString(value));
			}
		}

		#endregion
	}
}
