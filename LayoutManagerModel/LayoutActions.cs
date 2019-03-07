using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace LayoutManager.Model {
#pragma warning disable IDE0051
    [LayoutModule("Programming Operations Manager", UserControl = false)]
    class ActionManager : LayoutModuleBase {

        [LayoutAsyncEvent("do-command-station-actions")]
        private async Task<object> doProgrammingActions(LayoutEvent e0) {
            var e = (LayoutEvent<ILayoutActionContainer, IModelComponentCanProgramLocomotives>)e0;
            var actions = e.Sender;
            var commandStation = e.Info;

            actions.PrepareForProgramming();        // Ensure that actions are prepared for programming

            foreach (var action in actions) {
                LayoutActionResult result = LayoutActionResult.Ok;

                action.Status = ActionStatus.InProgress;

                if (action is ILayoutDccProgrammingAction)
                    result = await ((ILayoutDccProgrammingAction)action).Execute(commandStation, e.GetBoolOption("UsePOM"));

                if (result != LayoutActionResult.Ok) {
                    action.Status = ActionStatus.Failed;
                    return new LayoutActionFailure(result, action);
                }

                action.Commit();
                action.Status = ActionStatus.Done;
                Message("Done: " + action.ToString());
            }

            return null;
        }
    }

    public interface ILayoutActionContainer : IEnumerable<LayoutAction> {
        /// <summary>
        /// Add action
        /// </summary>
        /// <param name="actionType">The action type to add</param>
        /// <returns>The added action</returns>
        ILayoutAction Add(string actionType);

        /// <summary>
        /// Remove an action (usually after it has been commited)
        /// </summary>
        /// <param name="action">The action to remove</param>
        void Remove(ILayoutAction action);

        /// <summary>
        /// Check whether any action in this container requires that tagret to be connected to (or on) track
        /// </summary>
        /// <returns>True - target need to be placed on track</returns>
        bool IsTrackRequired();

        /// <summary>
        /// Check whether allocation of programing track is needed for applying the actions stored in this container
        /// </summary>
        /// <returns>True = programming track is needed, false = no programming track is needed</returns>
        bool IsProgrammingTrackRequired();

        /// <summary>
        /// Commit all actions in the container
        /// </summary>
        void Commit();

        /// <summary>
        /// Prepapre actions for actual programming. Action may for example, generate the correct CVs settings based on its
        /// properties
        /// </summary>
        void PrepareForProgramming();
    }

    public class LayoutActionContainer<ActionsOwnerType> : LayoutXmlWrapper, ILayoutActionContainer {
        bool preparedForProgramming = false;

        public LayoutActionContainer(ActionsOwnerType actionsOwner)
            : base("Actions") {
            Owner = actionsOwner;
        }

        public LayoutActionContainer(XmlElement parentElement, ActionsOwnerType actionsOwner)
            : base(parentElement, "Actions") {
            Owner = actionsOwner;
        }

        public ActionsOwnerType Owner {
            get;

        }

        ILayoutAction GetAction(XmlElement actionElement) {
            LayoutAction action = null;
            string actionType = actionElement.GetAttribute("Type");

            if (actionType != null)
                action = EventManager.Event(new LayoutEvent(actionElement, "get-action", null, Owner)) as LayoutAction;

            return action;
        }

        public ILayoutAction Add(string actionType) {
            if (preparedForProgramming)
                throw new ApplicationException("Trying to add action after preparing for programming");

            XmlElement actionElement = Element.OwnerDocument.CreateElement("Action");

            actionElement.SetAttribute("Type", actionType);
            Element.AppendChild(actionElement);

            return GetAction(actionElement);
        }

        public void Remove(ILayoutAction theAction) {
            LayoutAction action = (LayoutAction)theAction;

            Element.RemoveChild(action.Element);
        }

        public void Commit() {
            foreach (var action in this)
                action.Commit();
        }

        public void PrepareForProgramming() {
            if (!preparedForProgramming) {
                foreach (var action in this) {
                    if (action is ILayoutProgrammingAction programmingAction)
                        programmingAction.PrepareProgramming();
                }

                preparedForProgramming = true;
            }
        }

        /// <summary>
        /// Check whether any action in this container requires that tagret to be connected to (or on) track
        /// </summary>
        /// <returns>True - target need to be placed on track</returns>
        public bool IsTrackRequired() => (from action in this where action is ILayoutProgrammingAction select action as ILayoutProgrammingAction).Any(action => action.RequiresTrack);

        /// <summary>
        /// Check whether allocation of programing track is needed for applying the actions stored in this container
        /// </summary>
        /// <returns>True = programming track is needed, false = no programming track is needed</returns>
        public bool IsProgrammingTrackRequired() => (from action in this where action is ILayoutDccProgrammingAction select action as ILayoutDccProgrammingAction).Any(action => !action.CanUseProgramOnMain);

        #region IEnumerable<LayoutAction> Members

        public IEnumerator<LayoutAction> GetEnumerator() {
            // Pass on a static list so it would be possible to delete action during the loop
            foreach (XmlElement actionElement in Element) {
                LayoutAction action = (LayoutAction)GetAction(actionElement);

                if (action != null)
                    yield return action;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    public enum LayoutActionResult {
        Ok,
        UnexpectedReply,
        Timeout,
        Overload,
        NoResponse,
    }

    public class LayoutActionFailure {
        public LayoutActionResult Result { get; }
        public ILayoutAction Action { get; }

        public LayoutActionFailure(LayoutActionResult result, ILayoutAction action) {
            this.Result = result;
            this.Action = action;
        }

        public override string ToString() => "Action: " + Action.ToString() + " failed (" + Result.ToString() + ")";
    }

    public enum ActionStatus {
        Pending, InProgress, Done, Failed
    }

    public interface ILayoutAction : IObjectHasId, IObjectHasXml {
        string Description { get; }
        void Commit();
        ActionStatus Status { get; }
    }

    public interface ILayoutProgrammingAction : ILayoutAction {
        /// <summary>
        /// Target must be connected to (or on) a track for action to be applied
        /// </summary>
        bool RequiresTrack { get; }

        /// <summary>
        /// Prepare action for programming operation (for example, generate CV values)
        /// </summary>
        void PrepareProgramming();

        /// <summary>
        /// (If true) Treat NoResponse result as Ok
        /// </summary>
        bool IgnoreNoResponseResult { get; set; }
    }

    public interface ILayoutDccProgrammingAction : ILayoutProgrammingAction {
        /// <summary>
        /// Return true if it is possible to use "Program On Main track (POM)" method, thus not requiring that the locomotive is position on
        /// a programming track
        /// </summary>
        bool CanUseProgramOnMain { get; }

        /// <summary>
        /// Perform the actual programming
        /// </summary>
        /// <param name="commandStation">Programming unit to perform the programming (usually a command station)</param>
        /// <param name="useProgramOnMain">Use Program on main track (POM) in order to perform the programming</param>
        /// <returns></returns>
        Task<LayoutActionResult> Execute(IModelComponentCanProgramLocomotives commandStation, bool useProgramOnMain);
    }

    public interface ILayoutLocomotiveAddressChangeAction : ILayoutProgrammingAction {
        int Address { get; set; }
        int SpeedSteps { get; set; }
    }

    public abstract class LayoutAction : LayoutXmlWrapper, ILayoutAction {
        public LayoutAction(XmlElement actionElement)
            : base(actionElement) {
            Status = ActionStatus.Pending;
        }

        public virtual string Description => "Action: " + GetType().Name;

        public ActionStatus Status {
            get {
                return (ActionStatus)Enum.Parse(typeof(ActionStatus), GetAttribute("Status"));
            }

            set {
                SetAttribute("Status", value.ToString());
                if (value != ActionStatus.Pending)
                    EventManager.Event(new LayoutEventInfoValueType<ILayoutAction, ActionStatus>("action-status-changed", this, value));
            }
        }

        /// <summary>
        /// Called after applying the action to commit it (for example to change locomotive address after address is programmed)
        /// </summary>
        public virtual void Commit() {

        }

        public override string ToString() => Description ?? "Action: " + this.GetType().Name;

        /// <summary>
        /// Return true if a given action is defined for a given target
        /// </summary>
        /// <param name="actionName">Action name (e.g. set-address)</param>
        /// <param name="target">The object on which the action should be done</param>
        /// <returns>True - action is defined, false action is not defined</returns>
        public static bool HasAction(string actionName, IHasDecoder target) => (bool)EventManager.Event(new LayoutEventInfoResultValueType<IHasDecoder, bool, bool>("query-action", target, false).SetOption("Action", actionName));
    }

    public abstract class LayoutProgrammingAction : LayoutAction, ILayoutProgrammingAction {
        public LayoutProgrammingAction(XmlElement actionElement)
            : base(actionElement) {
        }

        /// <summary>
        /// Target must be connected to (or on) a track for action to be applied
        /// </summary>
        public virtual bool RequiresTrack => true;

        /// <summary>
        /// Do what ever is needed to preprare action for programming (for example, generate CVs values)
        /// </summary>
        public virtual void PrepareProgramming() {
        }

        /// <summary>
        /// (If true) Treat NoResponse result as Ok
        /// </summary>
        public bool IgnoreNoResponseResult {
            get {
                return XmlConvert.ToBoolean(GetAttribute("IgnoreNoResponse", "false"));
            }

            set {
                SetAttribute("IgnoreNoResponse", value, removeIf: false);
            }
        }
    }

    #region CV (DCC control values) handling class

    public class DccProgrammingCV : LayoutXmlWrapper {
        public DccProgrammingCV(XmlElement element)
            : base(element) {
        }

        public DccProgrammingCV(XmlElement parentElement, int cvNumber, byte value)
            : base(parentElement, "CV", alwaysAppend: true) {
            Number = cvNumber;
            Value = value;
        }

        public int Number {
            get {
                return XmlConvert.ToInt32(GetAttribute("Number"));
            }

            set {
                SetAttribute("Number", XmlConvert.ToString(value));
            }
        }

        public byte Value {
            get {
                return XmlConvert.ToByte(GetAttribute("Value"));
            }

            set {
                SetAttribute("Value", XmlConvert.ToString(value));
            }
        }
    }

    #endregion

    public class LayoutDccProgrammingAction<TProgrammingTargetType> : LayoutProgrammingAction, ILayoutDccProgrammingAction where TProgrammingTargetType : IHasDecoder {
        public LayoutDccProgrammingAction(XmlElement actionElement, TProgrammingTargetType programmingTarget) : base(actionElement) {
            this.ProgrammingTarget = programmingTarget;
        }

        public TProgrammingTargetType ProgrammingTarget {
            get;

        }

        /// <summary>
        /// Address to use if using Programming on Main (POM). If not set then POM will not be possible
        /// </summary>
        public int ProgrammingTargetAddress {
            get {
                return XmlConvert.ToInt32(GetAttribute("ProgrammingTargetAddress", "-1"));
            }

            set {
                SetAttribute("ProgrammingTargetAddress", XmlConvert.ToString(value));
            }
        }

        /// <summary>
        /// Set CV to be programmed
        /// </summary>
        /// <param name="cvNumber">Control variable (CV) number</param>
        /// <param name="value">The value to be programmed</param>
        public DccProgrammingCV SetCV(int cvNumber, byte value) => new DccProgrammingCV(Element, cvNumber, value);

        public IEnumerable<DccProgrammingCV> CVs {
            get {
                foreach (XmlElement cvElement in Element.GetElementsByTagName("CV"))
                    yield return new DccProgrammingCV(cvElement);
            }
        }

        /// <summary>
        /// Return true if it is possible to use "Program On Main track (POM)" method, thus not requiring that the locomotive is position on
        /// a programming track
        /// </summary>
        public bool CanUseProgramOnMain {
            get {
                if (!(ProgrammingTarget.DecoderType is DccDecoderTypeInfo dccDecoder) || !dccDecoder.SupportPOMprogramming)
                    return false;           // Not DCC or does not support POM

                if (ProgrammingTargetAddress < 0)
                    return false;

                if (CVs.Any(cv => cv.Number == 1 || cv.Number == 17 || cv.Number == 18))
                    return false;       // Trying to program address register (short or long address)

                return true;
            }
        }

        public async Task<LayoutActionResult> Execute(IModelComponentCanProgramLocomotives commandStation, bool useProgramOnMain) {
            var decoderType = ProgrammingTarget.DecoderType as DccDecoderTypeInfo;
            LayoutActionResult result = LayoutActionResult.Ok;

            Debug.Assert(decoderType != null);

            foreach (var cv in CVs) {
                if (useProgramOnMain)
                    result = (LayoutActionResult)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<DccProgrammingCV, DccDecoderTypeInfo>("program-CV-POM-request", cv, decoderType).SetCommandStation(commandStation).SetOption("Address", ProgrammingTargetAddress));
                else if (decoderType.ProgrammingMethod == DccProgrammingMethod.Cv)
                    result = (LayoutActionResult)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<DccProgrammingCV, DccDecoderTypeInfo>("program-CV-direct-request", cv, decoderType).SetCommandStation(commandStation));
                else
                    result = (LayoutActionResult)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent<DccProgrammingCV, DccDecoderTypeInfo>("program-CV-register-requst", cv, decoderType).SetCommandStation(commandStation));

                if (IgnoreNoResponseResult && result == LayoutActionResult.NoResponse)
                    result = LayoutActionResult.Ok;

                if (result != LayoutActionResult.Ok)
                    break;
            }

            return result;
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder(Description + ": ");
            string sep = "";

            foreach (DccProgrammingCV cv in CVs) {
                s.Append(sep + "CV" + cv.Number + "=" + cv.Value.ToString("d") + " (" + cv.Value.ToString("x2") + ")");
                sep = ", ";
            }

            return s.ToString();
        }
    }

    public class LayoutDccLocomotiveProgrammingAction : LayoutDccProgrammingAction<LocomotiveInfo> {
        public LayoutDccLocomotiveProgrammingAction(XmlElement actionElement, LocomotiveInfo locomotive)
            : base(actionElement, locomotive) {
            if (locomotive.AddressProvider.Unit > 0)
                ProgrammingTargetAddress = locomotive.AddressProvider.Unit;
        }
    }

    public class LayoutDccChangeLocomotiveAddressAction : LayoutDccLocomotiveProgrammingAction, ILayoutLocomotiveAddressChangeAction {
        public LayoutDccChangeLocomotiveAddressAction(XmlElement actionElement, LocomotiveInfo locomotive)
            : base(actionElement, locomotive) {

        }

        public int Address {
            get {
                return XmlConvert.ToInt32(GetAttribute("Address"));
            }

            set {
                SetAttribute("Address", XmlConvert.ToString(value));
            }
        }

        public int SpeedSteps {
            get {
                if (HasAttribute("SpeedSteps"))
                    return XmlConvert.ToInt32(GetAttribute("SpeedSteps"));
                else
                    return 28;
            }

            set {
                SetAttribute("SpeedSteps", XmlConvert.ToString(value));
            }
        }

        public bool ReverseDirection {
            get {
                if (HasAttribute("ReverseDirection"))
                    return XmlConvert.ToBoolean(GetAttribute("ReverseDirection"));
                else
                    return false;
            }

            set {
                SetAttribute("ReverseDirection", XmlConvert.ToString(value));
            }
        }

        public override string Description => "Set address to " + Address + ", " + SpeedSteps + " speed steps" + (ReverseDirection ? ", reverse direction" : "");

        public override void PrepareProgramming() {
            byte cv29 = 0;

            if (Address > 127)
                cv29 |= 0x10;           // Long address
            if (ReverseDirection)
                cv29 |= 0x01;           // Reverse direction
            if (SpeedSteps == 28)
                cv29 |= 0x02;           // 28 speed steps

            SetCV(29, cv29);

            if (Address <= 127)         // Short loco address
                SetCV(1, (byte)Address);
            else {                      // Long address
                SetCV(17, (byte)(Address >> 8));
                SetCV(18, (byte)(Address & 0xff));
            }
        }

        public override void Commit() {
            if (ProgrammingTarget.AddressProvider.Element == null)
                ProgrammingTarget.AddressProvider.Element = LayoutInfo.CreateProviderElement(ProgrammingTarget.Element, "Address", null);

            ProgrammingTarget.AddressProvider.Unit = Address;
            ProgrammingTarget.SpeedSteps = SpeedSteps;

            LayoutModel.LocomotiveCollection.Save();
        }
    }
}

