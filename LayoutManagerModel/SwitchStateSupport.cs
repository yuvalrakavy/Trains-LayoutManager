﻿using System;
using System.Collections.Generic;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Components {
    public class SwitchingStateSupport {
        public string StateTopic { get; }
        public IModelComponentHasSwitchingState Component { get; }
        public virtual int SwitchStateCount { get; }

        public SwitchingStateSupport(IModelComponentHasSwitchingState component, string stateTopic = "SwitchState", int switchStateCount = 2) {
            this.Component = component;
            this.StateTopic = stateTopic;
            this.SwitchStateCount = switchStateCount;
        }

        public string DefaultConnectionPointName {
            get {

                if (Component is IModelComponentConnectToControl c && c.ControlConnectionDescriptions.Count > 0)
                    return c.ControlConnectionDescriptions[0].Name;

                throw new ApplicationException($"Cannot get default connection point name for {Component} - no connection points are defined");
            }
        }
        public virtual int CurrentSwitchState => GetSwitchState(DefaultConnectionPointName);

        public virtual void AddSwitchingCommands(IList<SwitchingCommand> switchingCommands, int switchingState, string connectionPointName = null) {
            if (switchingState != 0 && switchingState != 1)
                throw new LayoutException(this, "Invalid switch state: " + switchingState);

            int state = switchingState;

            if (ReverseLogic && connectionPointName == null)
                state = 1 - state;

            ControlConnectionPoint connectionPoint = LayoutModel.ControlManager.ConnectionPoints[Component.Id, connectionPointName];

            switchingCommands.Add(new SwitchingCommand(new ControlConnectionPointReference(connectionPoint), state));
        }

        /// <summary>
        /// This method actually change the run-time state. It should be called only from the state changed
        /// notification handler, and not directly.
        /// </summary>
        /// <param name="switchState">The new switch state</param>
        public virtual void SetSwitchState(ControlConnectionPoint controlConnectionPoint, int switchState, string connectionPointName = null) {
            LayoutModel.StateManager.Components.StateOf(Component.Id, StateTopic).SetAttribute($"Value{connectionPointName ?? ""}", XmlConvert.ToString(switchState));
            Component.OnComponentChanged();
        }

        /// <summary>
        /// Get switch state of a given connection point
        /// </summary>
        /// <param name="connectionPointName">Connection point name or null for the default connection point</param>
        /// <returns>Connection point state</returns>
        public virtual int GetSwitchState(string connectionPointName) {
            if (LayoutModel.StateManager.Components.Contains(Component.Id, StateTopic)) {
                var state = LayoutModel.StateManager.Components.StateOf(Component.Id, StateTopic).GetAttribute(($"Value{connectionPointName}"));

                return string.IsNullOrWhiteSpace(state) ? 0 : XmlConvert.ToInt32(state);
            }
            else
                return 0;

        }

        public bool ReverseLogic {
            get {

                if (Component is IModelComponentHasReverseLogic componentWithReverseLogic)
                    return componentWithReverseLogic.ReverseLogic;
                else
                    return false;
            }
        }
    }

    public abstract class ModelComponentWithSwitchingState : ModelComponent, IModelComponentHasSwitchingState {
        readonly SwitchingStateSupport switchingStateSupport;

        public ModelComponentWithSwitchingState() {
            switchingStateSupport = GetSwitchingStateSupporter();
        }

        protected virtual SwitchingStateSupport GetSwitchingStateSupporter() => new SwitchingStateSupport(this);

        #region IModelComponentHasSwitchingState Members

        public int CurrentSwitchState => switchingStateSupport.CurrentSwitchState;

        public void SetSwitchState(ControlConnectionPoint connectionPoint, int switchState, string connectionPointName = null) {
            switchingStateSupport.SetSwitchState(connectionPoint, switchState, connectionPointName);
        }

        public int GetSwitchState(string connectionPointName = null) => switchingStateSupport.GetSwitchState(connectionPointName);

        public int SwitchStateCount => switchingStateSupport.SwitchStateCount;

        public void AddSwitchingCommands(IList<SwitchingCommand> switchingCommands, int switchingState, string connectionPointName = null) {
            switchingStateSupport.AddSwitchingCommands(switchingCommands, switchingState, connectionPointName);
        }

        #endregion

        #region IModelComponentHasReverseLogic Members

        public bool ReverseLogic {
            get {
                if (Element.HasAttribute("ReverseLogic"))
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
