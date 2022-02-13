using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using MethodDispatcher;

namespace LayoutManager.Model {
    [LayoutModule("Programming Operations Manager", UserControl = false)]
    internal class ActionManager : LayoutModuleBase {
        [LayoutAsyncEvent("do-command-station-actions")]
        private async Task<object?> DoProgrammingActions(LayoutEvent e) {
            var actions = Ensure.NotNull<ILayoutActionContainer>(e.Sender, "actions");
            var commandStation = Ensure.NotNull<IModelComponentCanProgramLocomotives>(e.Info, "commandStation");

            actions.PrepareForProgramming();        // Ensure that actions are prepared for programming

            foreach (var action in actions) {
                LayoutActionResult result = LayoutActionResult.Ok;

                action.Status = ActionStatus.InProgress;

                if (action is ILayoutDccProgrammingAction layoutDccProgrammingAction)
                    result = await (layoutDccProgrammingAction).Execute(commandStation, (bool?)e.GetOption("UsePOM") ?? false);

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
        ILayoutAction? Add(string actionType);

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
        /// Prepare actions for actual programming. Action may for example, generate the correct CVs settings based on its
        /// properties
        /// </summary>
        void PrepareForProgramming();
    }

    public class LayoutActionContainer<ActionsOwnerType> : LayoutXmlWrapper, ILayoutActionContainer where ActionsOwnerType : notnull {
        private bool preparedForProgramming = false;

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

        private ILayoutAction? GetAction(XmlElement actionElement) {
            var actionType = (string?)actionElement.AttributeValue("Type");

            return actionType != null ? Dispatch.Call.GetAction(actionElement, Owner) : null;
        }

        public ILayoutAction? Add(string actionType) {
            if (preparedForProgramming)
                throw new ApplicationException("Trying to add action after preparing for programming");

            XmlElement actionElement = Element.OwnerDocument.CreateElement("Action");

            actionElement.SetAttribute("Type", actionType);
            Element.AppendChild(actionElement);

            return GetAction(actionElement);
        }

        public void Remove(ILayoutAction action) => Element.RemoveChild(action.Element);

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
                var action = GetAction(actionElement);

                if (action != null)
                    yield return (LayoutAction)action;
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
        private const string A_Status = "Status";

        protected LayoutAction(XmlElement actionElement)
            : base(actionElement) {
            Status = ActionStatus.Pending;
        }

        public virtual string Description => "Action: " + GetType().Name;

        public ActionStatus Status {
            get => AttributeValue(A_Status).Enum<ActionStatus>() ?? ActionStatus.Pending;

            set {
                SetAttributeValue(A_Status, value);
                if (value != ActionStatus.Pending)
                    EventManager.Event("action-status-changed", this, value);
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
        public static bool HasAction(string actionName, IHasDecoder target) => Dispatch.Call.QueryAction(target, actionName);
    }

    public abstract class LayoutProgrammingAction : LayoutAction, ILayoutProgrammingAction {
        private const string A_IgnoreNoResponse = "IgnoreNoResponse";

        protected LayoutProgrammingAction(XmlElement actionElement)
            : base(actionElement) {
        }

        /// <summary>
        /// Target must be connected to (or on) a track for action to be applied
        /// </summary>
        public virtual bool RequiresTrack => true;

        /// <summary>
        /// Do what ever is needed to prepare action for programming (for example, generate CVs values)
        /// </summary>
        public virtual void PrepareProgramming() {
        }

        /// <summary>
        /// (If true) Treat NoResponse result as OK
        /// </summary>
        public bool IgnoreNoResponseResult {
            get => (bool?)AttributeValue(A_IgnoreNoResponse) ?? false;
            set => SetAttributeValue(A_IgnoreNoResponse, value, removeIf: false);
        }
    }

    #region CV (DCC control values) handling class

    public class DccProgrammingCV : LayoutXmlWrapper {
        private const string A_Number = "Number";
        private const string A_Value = "Value";

        public DccProgrammingCV(XmlElement element)
            : base(element) {
        }

        public DccProgrammingCV(XmlElement parentElement, int cvNumber, byte value)
            : base(parentElement, "CV", alwaysAppend: true) {
            Number = cvNumber;
            Value = value;
        }

        public int Number {
            get => (int)AttributeValue(A_Number);
            set => SetAttributeValue(A_Number, value);
        }

        public byte Value {
            get => (byte)AttributeValue(A_Value);
            set => SetAttributeValue(A_Value, value);
        }
    }

    #endregion

    public class LayoutDccProgrammingAction<TProgrammingTargetType> : LayoutProgrammingAction, ILayoutDccProgrammingAction where TProgrammingTargetType : IHasDecoder {
        private const string A_ProgrammingTargetAddress = "ProgrammingTargetAddress";

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
            get => (int?)AttributeValue(LayoutDccProgrammingAction<TProgrammingTargetType>.A_ProgrammingTargetAddress) ?? -1;
            set => SetAttributeValue(LayoutDccProgrammingAction<TProgrammingTargetType>.A_ProgrammingTargetAddress, value);
        }

        /// <summary>
        /// Set CV to be programmed
        /// </summary>
        /// <param name="cvNumber">Control variable (CV) number</param>
        /// <param name="value">The value to be programmed</param>
        public DccProgrammingCV SetCV(int cvNumber, byte value) => new(Element, cvNumber, value);

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
                if (ProgrammingTarget.DecoderType is not DccDecoderTypeInfo dccDecoder || !dccDecoder.SupportPOMprogramming)
                    return false;           // Not DCC or does not support POM

                if (ProgrammingTargetAddress < 0)
                    return false;

                if (CVs.Any(cv => cv.Number == 1 || cv.Number == 17 || cv.Number == 18))
                    return false;       // Trying to program address register (short or long address)

                return true;
            }
        }

        public async Task<LayoutActionResult> Execute(IModelComponentCanProgramLocomotives commandStation, bool useProgramOnMain) {
            LayoutActionResult result = LayoutActionResult.Ok;

            if (ProgrammingTarget.DecoderType is not DccDecoderTypeInfo decoderType)
                throw new LayoutException("Decoder is not DCC compatibile");

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
            StringBuilder s = new(Description + ": ");
            string sep = "";

            foreach (DccProgrammingCV cv in CVs) {
                s.Append(sep).Append("CV").Append(cv.Number).Append('=').Append(cv.Value.ToString("d")).Append(" (").Append(cv.Value.ToString("x2")).Append(')');
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
        private const string A_Address = "Address";
        private const string A_SpeedSteps = "SpeedSteps";
        private const string A_ReverseDirection = "ReverseDirection";

        public LayoutDccChangeLocomotiveAddressAction(XmlElement actionElement, LocomotiveInfo locomotive)
            : base(actionElement, locomotive) {
        }

        public int Address {
            get => (int)AttributeValue(A_Address);
            set => SetAttributeValue(A_Address, value);
        }

        public int SpeedSteps {
            get => (int?)AttributeValue(A_SpeedSteps) ?? 28;
            set => SetAttributeValue(A_SpeedSteps, value);
        }

        public bool ReverseDirection {
            get => (bool?)AttributeValue(A_ReverseDirection) ?? false;
            set => SetAttributeValue(A_ReverseDirection, value, removeIf: false);
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
                ProgrammingTarget.AddressProvider.Element = LayoutInfo.CreateProviderElement(ProgrammingTarget.Element, A_Address, null);

            ProgrammingTarget.AddressProvider.Unit = Address;
            ProgrammingTarget.SpeedSteps = SpeedSteps;

            LayoutModel.LocomotiveCollection.Save();
            EventManager.Event(new LayoutEventInfoValueType<LocomotiveInfo, int>("locomotive-address-changed", this.ProgrammingTarget, Address));
        }
    }
}

