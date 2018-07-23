using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools {
	/// <summary>
	/// Summary description for LocomotiveTools.
	/// </summary>
	[LayoutModule("Locomotive Operation Tools", UserControl = false)]
	public class LocomotiveOperationTools : System.ComponentModel.Component, ILayoutModuleSetup {

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		#region Constructors

		public LocomotiveOperationTools(IContainer container) {
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();
		}

		public LocomotiveOperationTools() {
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
		}

		#endregion

		enum PlacementProblem {
			NoProblem, NoAddress, AddressAlreadyUsed, Unknown
		};

		/// <summary>
		/// Build the menu with suggested blocks on which locomotives can be placed on track
		/// If the blocks are all within a single area, a one level menu is builts. If the blocks
		/// are in more than one area, then two level menu is builts. The first with the area names
		/// and the second level is with the block names.
		/// </summary>
		[LayoutEvent("add-placeble-blocks-menu-entries")]
		private void addPlacebleBlocksMenuEntries(LayoutEvent e) {
			XmlElement placedElement = (XmlElement)e.Sender;
			Menu m = (Menu)e.Info;
			CanPlaceTrainResult result;

			result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent(placedElement, "can-locomotive-be-placed-on-track", (string)null, null));

			if(result.Status != CanPlaceTrainStatus.CanPlaceTrain) {
				MenuItem problemItem = new MenuItem(result.ToString());

				problemItem.Enabled = false;
				m.MenuItems.Add(problemItem);
				return;
			}

			PlacementProblem problem = PlacementProblem.NoProblem;
			String errorMessage = null;
			var areaToBlockList = new Dictionary<LayoutModelArea, List<LayoutBlockDefinitionComponent>>();

			foreach(LayoutBlockDefinitionComponent blockInfo in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases)) {
				if(!blockInfo.Block.HasTrains && blockInfo.Info.SuggestForPlacement) {
					result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent(placedElement, "can-locomotive-be-placed", null, blockInfo));

					problem = PlacementProblem.NoProblem;

					if(!result.CanBeResolved) {
						problem = PlacementProblem.Unknown;
						errorMessage = result.ToString();
						break;
					}
					else {
						List<LayoutBlockDefinitionComponent> blocksInArea;

						if(!areaToBlockList.TryGetValue(blockInfo.Spot.Area, out blocksInArea)) {
							blocksInArea = new List<LayoutBlockDefinitionComponent>();
							areaToBlockList[blockInfo.Spot.Area] = blocksInArea;
						}

						blocksInArea.Add(blockInfo);
						errorMessage = null;
					}
				}
			}

			if(problem == PlacementProblem.NoProblem) {
				if(areaToBlockList.Count == 0) {
					MenuItem problemItem = new MenuItem("No empty block was found");

					problemItem.Enabled = false;
					m.MenuItems.Add(problemItem);
				}
				else if(areaToBlockList.Count == 1) {
					// If there is only block info in one area, just add those to the menu
					foreach(var blockInfoList in areaToBlockList.Values) {
						blockInfoList.Sort((b1, b2) => b1.NameProvider.Name.CompareTo(b2.NameProvider.Name));

						foreach(var blockInfo in blockInfoList)
							m.MenuItems.Add(new PlaceOnTrackMenuItem(placedElement, blockInfo));
					}
				}
				else {
					LayoutModelArea[] areas = new LayoutModelArea[areaToBlockList.Count];

					areaToBlockList.Keys.CopyTo(areas, 0);

					Array.Sort(areas, (a1, a2) => a1.Name.CompareTo(a2.Name));

					foreach(LayoutModelArea area in areas) {
						MenuItem areaMenuItem = new MenuItem(area.Name);
						var blockInfoList = areaToBlockList[area];

						blockInfoList.Sort((b1, b2) => b1.NameProvider.Name.CompareTo(b2.NameProvider.Name));

						foreach(LayoutBlockDefinitionComponent blockInfo in blockInfoList)
							areaMenuItem.MenuItems.Add(new PlaceOnTrackMenuItem(placedElement, blockInfo));

						m.MenuItems.Add(areaMenuItem);
					}
				}
			}
			else {
				MenuItem problemItem = new MenuItem(errorMessage);

				problemItem.Enabled = false;
				m.MenuItems.Add(problemItem);
			}
		}

		[LayoutEvent("show-locomotive-controller")]
		private void showLocomotiveController(LayoutEvent e) {
			TrainStateInfo trainState = (TrainStateInfo)EventManager.Event(new LayoutEvent(e.Sender, "extract-train-state"));

			if(trainState.NotManaged)
				throw new LocomotiveNotManagedException(trainState.Locomotives[0].Locomotive);

			if(EventManager.Event(new LayoutEvent(trainState, "activate-locomotive-controller")) == null) {
				Dialogs.LocomotiveController locoController = new Dialogs.LocomotiveController(trainState);

				locoController.Show();
			}
		}

		/// <summary>
		/// Default handler for adding menu items to the locomotive collection context
		/// menu.
		/// </summary>
		[LayoutEvent("add-locomotive-collection-operation-context-menu-entries")]
		protected void AddLocomotiveCollectionOperationContextMenuEntries(LayoutEvent e) {
			XmlElement placedElement = (XmlElement)e.Sender;
			Menu m = (Menu)e.Info;
			TrainStateInfo train = LayoutModel.StateManager.Trains[LayoutModel.LocomotiveCollection.GetElementId(placedElement)];

			if(train == null) {
				var context = LayoutOperationContext.GetPendingOperation("TrainPlacement", new LayoutXmlWithIdWrapper(placedElement));

				if(context == null) {
					MenuItem placeOnTrack = new MenuItem("Place on track");

					EventManager.Event(new LayoutEvent(placedElement, "add-placeble-blocks-menu-entries", null, placeOnTrack));
					placeOnTrack.Enabled = placeOnTrack.MenuItems.Count > 0;
					m.MenuItems.Add(placeOnTrack);
				}
				else
					m.MenuItems.Add("Cancel " + context.Description, (s, ea) => context.Cancel());
			}
			else {
				var context = LayoutOperationContext.GetPendingOperation("TrainPlacement", new LayoutXmlWithIdWrapper(placedElement));

				if(context != null)
					m.MenuItems.Add("Cancel " + context.Description, (s, ea) => context.Cancel());

				if(train.IsPowered)
					m.MenuItems.Add(new ShowLocomotiveControllerMenuItem(train));

				m.MenuItems.Add(new TrainPropertiesMenuItem(train));

				if(train.OnTrack)
					m.MenuItems.Add(new LocateLocomotiveMenuItem(train.LocomotiveBlock.BlockDefinintion));

				if(LayoutOperationContext.HasPendingOperation("TrainRemoval", train))
					m.MenuItems.Add(new CancelTrainRemovalMenuItem(train));
				else if(train.OnTrack)
					m.MenuItems.Add(new RemoveFromTrackMenuItem(train));
			}

			if(placedElement.Name == "Locomotive") {
				var locomotive = new LocomotiveInfo(placedElement);
				var context = LayoutOperationContext.GetPendingOperation("LocomotiveProgramming", locomotive);

				if(context != null)
					m.MenuItems.Add("Cancel " + context.Description, (s, ea) => context.Cancel());
				else {

					var locomotiveProgrammingMenu = new MenuItem("&Program locomotive");

					EventManager.Event(new LayoutEvent<LocomotiveInfo, Menu>("add-locomotive-programming-menu-entries", locomotive, locomotiveProgrammingMenu));

					if(locomotiveProgrammingMenu.MenuItems.Count == 1)
						m.MenuItems.Add(locomotiveProgrammingMenu.MenuItems[0]);
					else if(locomotiveProgrammingMenu.MenuItems.Count > 1)
						m.MenuItems.Add(locomotiveProgrammingMenu);
				}
			}
		}

		[LayoutEvent("edit-train-properties")]
		private void editTrainProperties(LayoutEvent e) {
			TrainStateInfo train = (TrainStateInfo)e.Sender;
			Dialogs.TrainProperties trainProperties = new Dialogs.TrainProperties(train);

			trainProperties.ShowDialog();
		}

		[LayoutEvent("get-locomotive-front")]
		private void getLocomotiveFront(LayoutEvent e) {
			LayoutBlockDefinitionComponent blockDefinition;
			String name;

			blockDefinition = (LayoutBlockDefinitionComponent)e.Sender;

			EventManager.Event(new LayoutEvent(blockDefinition, "ensure-component-visible", null, false));

			if(e.Info is XmlElement) {
				XmlElement placedElement = (XmlElement)e.Info;

				if(placedElement.Name == "Locomotive")
					name = new LocomotiveInfo(placedElement).DisplayName;
				else if(placedElement.Name == "Train")
					name = new TrainInCollectionInfo(placedElement).DisplayName;
				else if(placedElement.Name == "TrainState")
					name = new TrainStateInfo(placedElement).DisplayName;
				else
					throw new ArgumentException("Invalid placed element");
			}
			else if(e.Info is string)
				name = (string)e.Info;
			else
				throw new ArgumentException("Invalid locomotive name");

			Dialogs.LocomotiveFront locoFront = new Dialogs.LocomotiveFront(blockDefinition, name);
			if(locoFront.ShowDialog() == DialogResult.OK)
				e.Info = locoFront.Front;
			else
				e.Info = null;
		}

		[LayoutEvent("get-waypoint-front")]
		private void getWaypointFront(LayoutEvent e) {
			LayoutStraightTrackComponent track = (LayoutStraightTrackComponent)e.Sender;

			Dialogs.LocomotiveFront locoFront = new Dialogs.LocomotiveFront(track, "", "");

			if(locoFront.ShowDialog() == DialogResult.OK)
				e.Info = locoFront.Front;
			else
				e.Info = null;
		}

		[LayoutEvent("get-train-front-and-length")]
		private void getTrainFrontAndLength(LayoutEvent e) {
			LayoutBlockDefinitionComponent blockInfo;
			String name;

			blockInfo = (LayoutBlockDefinitionComponent)e.Sender;

			EventManager.Event(new LayoutEvent(blockInfo, "ensure-component-visible", null, false));

			if(e.Info is XmlElement) {
				XmlElement placedElement = (XmlElement)e.Info;

				if(placedElement.Name == "Locomotive")
					name = new LocomotiveInfo(placedElement).DisplayName;
				else if(placedElement.Name == "Train")
					name = new TrainInCollectionInfo(placedElement).DisplayName;
				else if(placedElement.Name == "TrainState")
					name = new TrainStateInfo(placedElement).DisplayName;
				else
					throw new ArgumentException("Invalid placed element");
			}
			else if(e.Info is string)
				name = (string)e.Info;
			else
				throw new ArgumentException("Invalid locomotive name");

			Dialogs.TrainFrontAndLength trainFrontAndLength = new Dialogs.TrainFrontAndLength(blockInfo, name);
			if(trainFrontAndLength.ShowDialog() == DialogResult.OK)
				e.Info = new TrainFrontAndLength(trainFrontAndLength.Front, trainFrontAndLength.Length);
			else
				e.Info = null;
		}

		[LayoutEvent("get-programming-location")]
		private void GetProgrammingLocation(LayoutEvent e) {
			var possibleLocations = from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.Info.SuggestForProgramming select blockDefinition;
			int count = possibleLocations.Count();

			if(count == 0) {
				var selection = new LayoutSelection(from blockDefinition in LayoutModel.Components<LayoutBlockDefinitionComponent>(LayoutModel.ActivePhases) where blockDefinition.PowerConnector.Inlet.ConnectedOutlet.ObtainablePowers.Any(p => p.Type == LayoutPowerType.Programmer) select blockDefinition);

				if(selection.Count == 0)
					LayoutModuleBase.Error("This operation need track that can receive command station's programming power - no such track is defined");
				else
					LayoutModuleBase.Error(selection, "No block is designated as suggested for programming - you should edit at least one block's properties and set it as 'Suggest for programming'");

				e.Info = null;
			}
			else if(count == 1)
				e.Info = possibleLocations.First();
			else {
				var d = new Dialogs.SelectProgrammingLocation(possibleLocations);

				if(d.ShowDialog() == DialogResult.OK)
					e.Info = d.SelectedProgrammingLocation;
				else
					e.Info = null;
			}
		}

		#region Locomoative Programming menu entries

		[LayoutEvent("add-locomotive-programming-menu-entries")]
		private void addSetLocomotiveSetAddress(LayoutEvent e0) {
			var e = (LayoutEvent<LocomotiveInfo, Menu>)e0;
			LocomotiveInfo locomotive = e.Sender;

			if(locomotive.DecoderType is DecoderWithNumericAddressTypeInfo)
				e.Info.MenuItems.Add(new MenuItem("Set address...", (s, ea) => doSetLocomotiveAddress(locomotive)));
		}

		private async void doSetLocomotiveAddress(LocomotiveInfo locomotive) {
			var d = new Dialogs.ChangeLocomotiveAddress(locomotive);

			if(d.ShowDialog() == DialogResult.OK) {
				var programmingState = new LocomotiveProgrammingState(locomotive);

				programmingState.ProgrammingActions = new LayoutActionContainer<LocomotiveInfo>(programmingState.Locomotive);
				var changeAddressAction = (ILayoutLocomotiveAddressChangeAction)programmingState.ProgrammingActions.Add("set-address");

				changeAddressAction.Address = d.Address;
                changeAddressAction.SpeedSteps = d.SpeedSteps;

				if(d.ProgramLocomotive) {
                    try {
                        using (var context = new LayoutOperationContext("LocomotiveProgramming", "set locomotive address", locomotive)) {
                            var train = (TrainStateInfo)await (Task<object>)EventManager.AsyncEvent(new LayoutEvent(programmingState, "program-locomotive").SetOperationContext(context));
                        }
                    } catch(LayoutException) { }
				}
				else
					programmingState.ProgrammingActions.Commit();
			}
		}

		#endregion

		#region Menu Items

		#region Locate Locomotive menu item

		class LocateLocomotiveMenuItem : MenuItem {
			LayoutBlockDefinitionComponent blockInfo;

			public LocateLocomotiveMenuItem(LayoutBlockDefinitionComponent blockInfo) {
				this.blockInfo = blockInfo;

				Text = "Locate locomotive";
				Enabled = blockInfo != null;
			}

			protected override void OnClick(EventArgs e) {
				base.OnClick(e);

				EventManager.Event(new LayoutEvent(blockInfo, "ensure-component-visible", null, true));
			}
		}

		#endregion

		#region Place on track menu item

		/// <summary>
		/// Menu item for placing locomotive on track
		/// </summary>
		class PlaceOnTrackMenuItem : MenuItem {
			XmlElement placedElement;
			LayoutBlockDefinitionComponent blockInfo;

			public PlaceOnTrackMenuItem(XmlElement placedElement, LayoutBlockDefinitionComponent blockInfo) {
				this.placedElement = placedElement;
				this.blockInfo = blockInfo;

				this.Text = blockInfo.NameProvider.Text;
			}

			protected override void OnClick(EventArgs e) {
				base.OnClick(e);

				ComponentOperationTools.DoValidateAndPlaceTrain(new LayoutEvent<LayoutBlockDefinitionComponent, XmlElement>("validate-and-place-train-request", blockInfo, placedElement));
			}
		}

		#endregion

		#region Remove from track menu item

		public class RemoveFromTrackMenuItem : MenuItem {
			TrainStateInfo train;

			public RemoveFromTrackMenuItem(TrainStateInfo train) {
				this.train = train;

				this.Text = "Remove from track";
			}

			protected override async void OnClick(EventArgs e) {
				try {
					using(var context = new LayoutOperationContext("TrainRemoval", "removal of " + train.Name + " from track", train)) {
						await EventManager.AsyncEvent(new LayoutEvent<object, string>("remove-from-track-request", train, "Remove train from track").SetOperationContext(context));
					}
				}
				catch(LayoutException lex) {
					lex.Report();
				}
				catch(OperationCanceledException) {
					Trace.WriteLine("Removal of train " + train.Name + " canceled");
				}
			}
		}

		public class CancelTrainRemovalMenuItem : MenuItem {
			TrainStateInfo train;

			public CancelTrainRemovalMenuItem(TrainStateInfo train) {
				this.train = train;
				Text = "Cancel removal of train " + train.Name;
			}

			protected override void OnClick(EventArgs e) {
				var context = LayoutOperationContext.GetPendingOperation("TrainRemoval", train);

				if(context != null)
					context.Cancel();
			}
		}

		#endregion

		#region Train properties menu item

		public class TrainPropertiesMenuItem : MenuItem {
			TrainStateInfo train;

			public TrainPropertiesMenuItem(TrainStateInfo train) {
				this.train = train;

				this.Text = "&Train properties...";
			}

			protected override void OnClick(EventArgs e) {
				EventManager.Event(new LayoutEvent(train, "edit-train-properties"));
			}
		}

		#endregion

		#region Show locomotive controller menu item

		public class ShowLocomotiveControllerMenuItem : MenuItem {
			TrainStateInfo state;

			public ShowLocomotiveControllerMenuItem(TrainStateInfo state) {
				this.state = state;
				this.Text = "Show &Controller";

				if(state.NotManaged || state.Locomotives.Count == 0)
					this.Enabled = false;
			}

			protected override void OnClick(EventArgs e) {
				try {
					EventManager.Event(new LayoutEvent(state, "show-locomotive-controller"));
				}
				catch(LayoutException lex) {
					lex.Report();
				}
			}
		}


		#endregion

		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new Container();
		}
		#endregion
	}
}
