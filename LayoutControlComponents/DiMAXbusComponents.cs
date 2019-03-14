﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using LayoutManager.Model;

#pragma warning disable IDE0051
#nullable enable
namespace LayoutManager.ControlComponents {

    [LayoutModule("DiMAX Bus Control Components")]
    class DiMAXBusComponents : LayoutModuleBase {

        [LayoutEvent("get-control-module-type", IfEvent = "LayoutEvent[./Options/@ModuleTypeName='Massoth8170001']")]
        [LayoutEvent("enum-control-module-types")]
        private void getMassoth8170001(LayoutEvent e) {
            var parentElement = Ensure.NotNull<XmlElement>(e.Sender, "parentElement");

            ControlModuleType moduleType = new ControlModuleType(parentElement, "Massoth8170001", "Massoth Feedback Interface") {
                AddressAlignment = 4
            };
            EventManager.Event(new LayoutEvent("add-bus-connectable-to-module", moduleType).SetOption("GenericBusType", "DiMAXBus"));
            moduleType.ConnectionPointsPerAddress = 2;
            moduleType.NumberOfAddresses = 4;
            moduleType.DefaultControlConnectionPointType = ControlConnectionPointTypes.InputDryContact;
            moduleType.ConnectionPointIndexBase = 0;
            moduleType.ConnectionPointLabelFormat = ControlConnectionPointLabelFormatOptions.Alpha | ControlConnectionPointLabelFormatOptions.AlphaLowercase | ControlConnectionPointLabelFormatOptions.AttachAddress;
            moduleType.LastAddress = 2048;
            moduleType.DecoderTypeName = "GenericDCC";
        }

        [LayoutEvent("recommend-control-module-types", IfEvent = "LayoutEvent[./Options/@BusFamily='DiMAXBus']")]
        private void recommendLGBcompatibleBusControlModuleType(LayoutEvent e) {
            var connectionDestination = Ensure.NotNull<ControlConnectionPointDestination>(e.Sender, "connectionDestination");
            var moduleTypeNames = Ensure.NotNull<IList<string>>(e.Info, "moduleTypeNames");

            if (connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
                moduleTypeNames.Add("Massoth8170001");
        }

        [LayoutEvent("query-action", IfEvent = "LayoutEvent[Options/@Action='set-address']", SenderType = typeof(ControlModule), IfSender = "*[@ModuleTypeName='Massoth8170001']")]
        private void querySetMassothSwitchDecoderAddress(LayoutEvent e0) {
            var e = (LayoutEventInfoResultValueType<IHasDecoder, bool, bool>)e0;

            e.Result = true;
        }

        [LayoutEvent("get-action", IfSender = "Action[@Type='set-address']", InfoType = typeof(ControlModule), IfInfo = "*[@ModuleTypeName='Massoth8170001']")]
        private void getProgramMassothSwitchDecoderAddressAction(LayoutEvent e) {
            var actionElement = Ensure.NotNull<XmlElement>(e.Sender, "actionElement");
            var module = Ensure.NotNull<ControlModule>(e.Info, "module");

            if (e.Info != null)
                e.Info = new ProgramMassothFeedbackDecoderAddress(actionElement, module);
        }

        [LayoutEvent("edit-action-settings", InfoType = typeof(IMassothFeedbackDecoderSetAddress))]
        private void editMassothFeedbackSetAddressSettings(LayoutEvent e0) {
            var e = (LayoutEventResultValueType<object, ILayoutAction, bool>)e0;
            var action = Ensure.NotNull<IMassothFeedbackDecoderSetAddress>(e.Info, "action");
            var controlModule = Ensure.NotNull<ControlModule>(e.Sender, "controlModule");

            var d = new Dialogs.MassothFeedbackDecoderAddressSettings(action, controlModule);

            if (d.ShowDialog() == DialogResult.OK)
                e.Result = true;
            else
                e.Result = false;
        }
    }

    class ProgramMassothFeedbackDecoder : LayoutDccProgrammingAction<ControlModule> {
        public ProgramMassothFeedbackDecoder(XmlElement actionElement, ControlModule feedbackDecoder)
            : base(actionElement, feedbackDecoder) {
        }
    }

    /// <summary>
    /// How the feedback module is connected: Master - it is connected to a DiMAX bus port, Slave - it is chained to a "master" feedback module
    /// </summary>
    public enum MassothFeedbackDecoderBusConnectionMethod {
        Master, Slave
    }

    public interface IMassothFeedbackDecoderSetAddress : ILayoutAction {
        MassothFeedbackDecoderBusConnectionMethod BusConnectionMethod { get; set; }

        int BusId { get; set; }
    }

    public class MassothFeedbackModule : ControlModule {
        public MassothFeedbackModule(ControlModule module)
            : base(module.ControlManager, module.Element) {
        }

        public bool HasDiMAX_BusConnectionMethod => HasAttribute("DiMAXBusConnectionMethod");

        public MassothFeedbackDecoderBusConnectionMethod DiMAX_BusConnectionMethod {
            get {
                return (MassothFeedbackDecoderBusConnectionMethod)Enum.Parse(typeof(MassothFeedbackDecoderBusConnectionMethod), GetAttribute("DiMAXBusConnectionMethod", "Slave"));
            }

            set {
                SetAttribute("DiMAXBusConnectionMethod", value.ToString());
            }
        }

        public bool HasDiMAX_BusId => HasAttribute("DiMAXBusID");

        public int DiMAX_BusId {
            get {
                return XmlConvert.ToInt32(GetAttribute("DiMAXBusID", "12"));
            }

            set {
                SetAttribute("DiMAXBusID", XmlConvert.ToString(value));
            }
        }

        /// <summary>
        /// Allocate DiMAX bus ID for using on a master feedback module
        /// </summary>
        /// <param name="commandStation"></param>
        /// <returns></returns>
        public static int AllocateDiMAX_BusID(IModelComponentIsBusProvider commandStation) {
            var bus = LayoutModel.ControlManager.Buses.GetBus(commandStation, "DiMAXBUS");

            if (bus != null) {
                var masterFeedbackModules = from module in bus.Modules where module.ModuleTypeName == "Massoth8170001" && new MassothFeedbackModule(module).DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master select new MassothFeedbackModule(module);

                for (int busId = 11; busId <= 20; busId++)
                    if (!masterFeedbackModules.Any(m => m.DiMAX_BusId == busId))
                        return busId;
            }

            return -1;
        }

        public static MassothFeedbackModule GetMasterUsingBusId(IModelComponentIsBusProvider commandStation, int busId) {
            var bus = Ensure.NotNull<ControlBus>(LayoutModel.ControlManager.Buses.GetBus(commandStation, "DiMAXBUS"), "DiMAXBUS");

                return (from module in bus.Modules
                        where module.ModuleTypeName == "Massoth8170001" &&
                        new MassothFeedbackModule(module).DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master &&
                        new MassothFeedbackModule(module).DiMAX_BusId == busId
                        select new MassothFeedbackModule(module)).FirstOrDefault();
        }
    }

    class ProgramMassothFeedbackDecoderAddress : ProgramMassothFeedbackDecoder, IMassothFeedbackDecoderSetAddress {
        public ProgramMassothFeedbackDecoderAddress(XmlElement actionElement, ControlModule feedbackModule)
            : base(actionElement, feedbackModule) {
        }

        public override void PrepareProgramming() {
            SetCV(1, 0);        // DiMAX mode

            if (BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master)
                SetCV(2, (byte)BusId);

            SetCV(3, (byte)(BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master ? 1 : 2));

            int address = ProgrammingTarget.Address;
            for (int i = 0; i < 4; i++) {
                SetCV(51 + i * 10, (byte)(address >> 8));
                SetCV(51 + i * 10 + 1, (byte)(address & 0xff));
                address++;
            }
        }

        public override void Commit() {
            var module = new MassothFeedbackModule(ProgrammingTarget) {
                DiMAX_BusConnectionMethod = BusConnectionMethod
            };

            string label = module.Label ?? "";
            label = Regex.Replace(label, "\\[.*\\]", "");

            if (label.Length > 0)
                label += " ";

            if (BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Slave)
                label += "[Slave]";
            else
                label += $"[Master ID: {module.DiMAX_BusId}]";

            module.Label = label;
            module.AddressProgrammingRequired = false;
        }

        public override string Description => "Set feedback module address to " + ProgrammingTarget.Address + " to " + (ProgrammingTarget.Address + ProgrammingTarget.ModuleType.NumberOfAddresses - 1);

        #region IMassothFeedbackDecoderSetAddress Members
        const string BusConnectionAttribute = "BusConnectionMethod";


        public MassothFeedbackDecoderBusConnectionMethod BusConnectionMethod {
            get {
                return Element.HasAttribute(BusConnectionAttribute) ? (MassothFeedbackDecoderBusConnectionMethod)Enum.Parse(typeof(MassothFeedbackDecoderBusConnectionMethod), Element.GetAttribute(BusConnectionAttribute)) : MassothFeedbackDecoderBusConnectionMethod.Slave;
            }

            set {
                Element.SetAttribute("BusConnectionMethod", value.ToString());
            }
        }

        public int BusId {
            get {
                return int.Parse(Element.GetAttribute("BusID"));
            }

            set {
                Element.SetAttribute("BusID", value.ToString());
            }
        }

        #endregion
    }

}
