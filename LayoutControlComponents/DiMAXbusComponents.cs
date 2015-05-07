using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using LayoutManager.Model;

namespace LayoutManager.ControlComponents {

    [LayoutModule("DiMAX Bus Control Components")]
	class DiMAXBusComponents : LayoutModuleBase {

		[LayoutEvent("get-control-module-type", IfEvent="LayoutEvent[./Options/@ModuleTypeName='Massoth8170001']")]
		[LayoutEvent("enum-control-module-types")]
		private void getMassoth8170001(LayoutEvent e) {
			XmlElement parentElement = (XmlElement)e.Sender;

			ControlModuleType moduleType = new ControlModuleType(parentElement, "Massoth8170001", "Massoth Feedback Interface");

			moduleType.AddressAlignment = 4;
			EventManager.Event(new LayoutEvent(moduleType, "add-bus-connectable-to-module").SetOption("GenericBusType", "DiMAXBus"));
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
			ControlConnectionPointDestination connectionDestination = (ControlConnectionPointDestination)e.Sender;
			IList<string> moduleTypeNames = (IList<string>)e.Info;

			if(connectionDestination.ConnectionDescription.IsCompatibleWith("Feedback", "DryContact"))
				moduleTypeNames.Add("Massoth8170001");
		}

		[LayoutEvent("query-action", IfEvent = "LayoutEvent[Options/@Action='set-address']", SenderType = typeof(ControlModule), IfSender = "*[@ModuleTypeName='Massoth8170001']")]
		private void querySetMassothSwitchDecoderAddress(LayoutEvent e0) {
			var e = (LayoutEvent<IHasDecoder, bool, bool>)e0;

			e.Result = true;
		}

		[LayoutEvent("get-action", IfSender = "Action[@Type='set-address']", InfoType = typeof(ControlModule), IfInfo = "*[@ModuleTypeName='Massoth8170001']")]
		private void getProgramMassothSwitchDecoderAddressAction(LayoutEvent e) {
			var actionElement = e.Sender as XmlElement;
			var module = e.Info as ControlModule;

			if(e.Info != null)
				e.Info = new ProgramMassothFeedbackDecoderAddress(actionElement, module);
		}

		[LayoutEvent("edit-action-settings", InfoType=typeof(IMassothFeedbackDecoderSetAddress))]
		private void editMassothFeedbackSetAddressSettings(LayoutEvent e0) {
			var e = (LayoutEvent<object, ILayoutAction, bool>)e0;
			var d = new Dialogs.MassothFeedbackDecoderAddressSettings((IMassothFeedbackDecoderSetAddress)e.Info, (ControlModule)e.Sender);

			if(d.ShowDialog() == DialogResult.OK)
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

		public bool HasDiMAX_BusConnectionMethod {
			get {
				return HasAttribute("DiMAXBusConnectionMethod");
			}
		}

		public MassothFeedbackDecoderBusConnectionMethod DiMAX_BusConnectionMethod {
			get {
				return (MassothFeedbackDecoderBusConnectionMethod)Enum.Parse(typeof(MassothFeedbackDecoderBusConnectionMethod), GetAttribute("DiMAXBusConnectionMethod", "Slave"));
			}

			set {
				SetAttribute("DiMAXBusConnectionMethod", value.ToString());
			}
		}

		public bool HasDiMAX_BusId {
			get {
				return HasAttribute("DiMAXBusID");
			}
		}

		public int DiMAX_BusId {
			get {
				return XmlConvert.ToInt32(GetAttribute("DiMAXBusID", "11"));
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
			ControlBus bus = LayoutModel.ControlManager.Buses.GetBus(commandStation, "DiMAXBUS");
			var masterFeedbackModules = from module in bus.Modules where module.ModuleTypeName == "Massoth8170001" && new MassothFeedbackModule(module).DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master select new MassothFeedbackModule(module);

			for(int busId = 11; busId <= 20; busId++)
				if(!masterFeedbackModules.Any(m => m.DiMAX_BusId == busId))
					return busId;

			return -1;
		}

		public static MassothFeedbackModule GetMasterUsingBusId(IModelComponentIsBusProvider commandStation, int busId) {
			ControlBus bus = LayoutModel.ControlManager.Buses.GetBus(commandStation, "DiMAXBUS");
			return (from module in bus.Modules where 
						module.ModuleTypeName == "Massoth8170001" && 
						new MassothFeedbackModule(module).DiMAX_BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master &&
						new MassothFeedbackModule(module).DiMAX_BusId == busId select new MassothFeedbackModule(module)).FirstOrDefault();

		}
	}

	class ProgramMassothFeedbackDecoderAddress : ProgramMassothFeedbackDecoder, IMassothFeedbackDecoderSetAddress {
		public ProgramMassothFeedbackDecoderAddress(XmlElement actionElement, ControlModule feedbackModule)
			: base(actionElement, feedbackModule) {
			BusConnectionMethod = MassothFeedbackDecoderBusConnectionMethod.Slave;
		}

		public override void PrepareProgramming() {
			SetCV(1, 0);		// DiMAX mode

			if(BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master)
				SetCV(2, (byte)BusId);

			SetCV(3, (byte)(BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Master ? 1 : 2));

			int address = ProgrammingTarget.Address;
			for(int i = 0; i < 4; i++) {
				SetCV(51 + i * 10, (byte)(address >> 8));
				SetCV(51 + i * 10 + 1, (byte)(address & 0xff));
				address++;
			}
		}

		public override void Commit() {
			var module = new MassothFeedbackModule(ProgrammingTarget);

			module.DiMAX_BusConnectionMethod = BusConnectionMethod;
			module.DiMAX_BusId = BusId;

            string label = module.Label ?? "";
			label = Regex.Replace(label, "\\[.*\\]", "");

			if(label.Length > 0)
				label += " ";

			if(BusConnectionMethod == MassothFeedbackDecoderBusConnectionMethod.Slave)
				label += "[Slave]";
			else
				label += "[Master ID: " + BusId + "]";

            module.Label = label;
            module.AddressProgrammingRequired = false;
		}

		public override string Description {
			get {
				return "Set feedback module address to " + ProgrammingTarget.Address + " to " + (ProgrammingTarget.Address + ProgrammingTarget.ModuleType.NumberOfAddresses - 1);
			}
		}

		#region IMassothFeedbackDecoderSetAddress Members

		public MassothFeedbackDecoderBusConnectionMethod BusConnectionMethod { get; set; }
		public int BusId { get; set; }

		#endregion
	}

}
