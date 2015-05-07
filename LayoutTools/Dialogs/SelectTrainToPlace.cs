using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutManager;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Xml;
using System.Diagnostics;

namespace LayoutManager.Tools.Dialogs {
	public partial class SelectTrainToPlace : Form {
		LayoutBlockDefinitionComponent blockDefinition;
		LocomotiveCatalogInfo catalog;

		string lastSearch = "";
		

		public SelectTrainToPlace(LayoutBlockDefinitionComponent blockDefinition) {
			InitializeComponent();

			this.blockDefinition = blockDefinition;
			this.catalog = LayoutModel.LocomotiveCatalog;

			locomotiveFront.ConnectionPoints = blockDefinition.Track.ConnectionPoints;
			locomotiveFront.Front = blockDefinition.Track.ConnectionPoints[0];
			labelSearchInstructions.Location = listBoxSearchResult.Location;
			labelNoMatch.Location = listBoxSearchResult.Location;

			locomotiveFront.Enabled = false;
			trainLengthDiagram.Enabled = false;

			UpdateInstructions();
		}

		public LayoutNamedTrainObject Selected {
			get {
				SearchResultItem searchResult = (SearchResultItem)listBoxSearchResult.SelectedItem;

				return searchResult != null ? searchResult.NamedObject : null;
			}
		}

        public LayoutComponentConnectionPoint Front => locomotiveFront.Front;

        public TrainLength Length => trainLengthDiagram.Length;

        private void UpdateInstructions() {
			if(textBoxSearch.Text.Trim().Length == 0) {
				listBoxSearchResult.Visible = false;
				labelSearchInstructions.Visible = true;
				labelNoMatch.Visible = false;
				locomotiveFront.Enabled = false;
				trainLengthDiagram.Enabled = false;
			}
			else {
				if(listBoxSearchResult.Items.Count == 0) {
					listBoxSearchResult.Visible = false;
					labelSearchInstructions.Visible = false;
					labelNoMatch.Visible = true;
				}
				else {
					listBoxSearchResult.Visible = true;
					labelSearchInstructions.Visible = false;
					labelNoMatch.Visible = false;
				}
			}
		}

		private string GetSegnificantSearchString() {
			StringBuilder segnificantSearchText = new StringBuilder(textBoxSearch.Text.Length);

			foreach(char c in textBoxSearch.Text) {
				if(c != '(' && c != ')' && c != '[' && c != ']')
					segnificantSearchText.Append(c);
			}

			return segnificantSearchText.ToString();
		}

		private void Search() {
			string newSearch = GetSegnificantSearchString();

			if(newSearch != lastSearch) {
				string searchingFor = textBoxSearch.Text.ToLowerInvariant();

				foreach(SearchResultItem searchResult in listBoxSearchResult.Items)
					searchResult.DeleteMe = true;

				foreach(XmlElement element in LayoutModel.LocomotiveCollection.CollectionElement) {
					if(LayoutModel.StateManager.Trains[element] == null) {		// Not already on track
						CanPlaceTrainResult result;

						result = (CanPlaceTrainResult)EventManager.Event(new LayoutEvent(element, "can-locomotive-be-placed", null, blockDefinition));

						if(result.CanBeResolved) {
							LayoutNamedTrainObject namedObject = null;

							if(element.Name == "Locomotive")
								namedObject = new LocomotiveInfo(element);
							else if(element.Name == "Train")
								namedObject = new TrainCommonInfo(element);
							Debug.Assert(namedObject != null);

							if(namedObject.DisplayName.ToLowerInvariant().Contains(searchingFor)) {
								bool found = false;
								foreach(SearchResultItem searchResult in listBoxSearchResult.Items) {
									if(searchResult.NamedObject.Id == namedObject.Id) {
										searchResult.DeleteMe = false;
										found = true;
										break;
									}
								}

								if(!found)
									listBoxSearchResult.Items.Add(new SearchResultItem(namedObject));
							}
						}
					}
				}

				List<SearchResultItem> itemsToDelete = new List<SearchResultItem>();

				foreach(SearchResultItem searchResult in listBoxSearchResult.Items)
					if(searchResult.DeleteMe)
						itemsToDelete.Add(searchResult);

				foreach(SearchResultItem searchResult in itemsToDelete)
					listBoxSearchResult.Items.Remove(searchResult);

				if(listBoxSearchResult.SelectedItem == null && listBoxSearchResult.Items.Count > 0)
					listBoxSearchResult.SelectedItem = listBoxSearchResult.Items[0];

				lastSearch = newSearch;
				UpdateInstructions();
			}
		}

		private void ForceSearch() {
			lastSearch = "";
			Search();
		}

		class SearchResultItem {
			LayoutNamedTrainObject namedObject;
			bool deleteMe;

			public SearchResultItem(LayoutNamedTrainObject namedObject) {
				this.namedObject = namedObject;
			}

            public LayoutNamedTrainObject NamedObject => namedObject;

            public bool DeleteMe {
				get {
					return deleteMe;
				}

				set {
					deleteMe = value;
				}
			}

            public override string ToString() => NamedObject.DisplayName;
        }

		private void textBoxSearch_TextChanged(object sender, EventArgs e) {
			Search();
		}

		private void listBoxSearchResult_MeasureItem(object sender, MeasureItemEventArgs e) {
			SearchResultItem searchResult = (SearchResultItem)listBoxSearchResult.Items[e.Index];

			LayoutManager.CommonUI.Controls.LocomotiveListItemPainter.Measure(e, searchResult.NamedObject.Element);
		}

		private void listBoxSearchResult_DrawItem(object sender, DrawItemEventArgs e) {
			SearchResultItem searchResult = (SearchResultItem)listBoxSearchResult.Items[e.Index];

			LayoutManager.CommonUI.Controls.LocomotiveListItemPainter.Draw(e, searchResult.NamedObject.Element, catalog, true);
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			if(listBoxSearchResult.SelectedItem == null) {
				MessageBox.Show(this, "You have not selected a locomotive or train", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBoxSearch.Focus();
				return;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void buttonNew_Click(object sender, EventArgs e) {
			ContextMenu m = new ContextMenu();

			m.MenuItems.Add("Locomotive...", delegate(object s, EventArgs a) {
				EventManager.Event(new LayoutEvent(this, "add-new-locomotive-to-collection"));
				ForceSearch();
			});

			m.MenuItems.Add("Train...", delegate(object s, EventArgs a) {
				EventManager.Event(new LayoutEvent(this, "add-new-train-to-collection"));
				ForceSearch();
			});

			m.Show(buttonNew.Parent, new Point(buttonNew.Left, buttonNew.Bottom));
		}

		private void listBoxSearchResult_SelectedIndexChanged(object sender, EventArgs e) {
			LayoutNamedTrainObject selected = Selected;

			if(selected != null) {
				if(selected.Element.Name == "Locomotive")
					trainLengthDiagram.Length = TrainLength.Standard;
				else if(selected.Element.Name == "Train")
					trainLengthDiagram.Length = new TrainCommonInfo(selected.Element).Length;

				locomotiveFront.Enabled = true;
				trainLengthDiagram.Enabled = true;
			}
			else {
				locomotiveFront.Enabled = false;
				trainLengthDiagram.Enabled = false;
			}
		}
	}
}
