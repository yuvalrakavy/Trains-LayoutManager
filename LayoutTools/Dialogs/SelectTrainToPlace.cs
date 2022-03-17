using LayoutManager.Components;
using LayoutManager.Model;
using MethodDispatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace LayoutManager.Tools.Dialogs {
    public partial class SelectTrainToPlace : Form {
        private readonly LayoutBlockDefinitionComponent blockDefinition;
        private readonly LocomotiveCatalogInfo catalog;

        private string lastSearch = "";

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

        public LayoutNamedTrainObject? Selected {
            get {
                SearchResultItem searchResult = (SearchResultItem)listBoxSearchResult.SelectedItem;

                return searchResult?.NamedObject;
            }
        }

        public LayoutComponentConnectionPoint Front => locomotiveFront.Front;

        public TrainLength Length => trainLengthDiagram.Length;

        private void UpdateInstructions() {
            if (textBoxSearch.Text.Trim().Length == 0) {
                listBoxSearchResult.Visible = false;
                labelSearchInstructions.Visible = true;
                labelNoMatch.Visible = false;
                locomotiveFront.Enabled = false;
                trainLengthDiagram.Enabled = false;
            }
            else {
                if (listBoxSearchResult.Items.Count == 0) {
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
            StringBuilder segnificantSearchText = new(textBoxSearch.Text.Length);

            foreach (char c in textBoxSearch.Text) {
                if (c != '(' && c != ')' && c != '[' && c != ']')
                    segnificantSearchText.Append(c);
            }

            return segnificantSearchText.ToString();
        }

        private void Search() {
            string newSearch = GetSegnificantSearchString();

            if (newSearch != lastSearch) {
                string searchingFor = textBoxSearch.Text.ToLowerInvariant();

                foreach (SearchResultItem searchResult in listBoxSearchResult.Items)
                    searchResult.DeleteMe = true;

                foreach (XmlElement element in LayoutModel.LocomotiveCollection.CollectionElement) {
                    if (LayoutModel.StateManager.Trains[element] == null) {     // Not already on track
                        CanPlaceTrainResult result;

                        result = Dispatch.Call.CanLocomotiveBePlaced(element, blockDefinition);

                        if (result.CanBeResolved) {
                            LayoutNamedTrainObject? namedObject = null;

                            if (element.Name == "Locomotive")
                                namedObject = new LocomotiveInfo(element);
                            else if (element.Name == "Train")
                                namedObject = new TrainCommonInfo(element);
                            Debug.Assert(namedObject != null);

                            if (namedObject.DisplayName.ToLowerInvariant().Contains(searchingFor)) {
                                bool found = false;
                                foreach (SearchResultItem searchResult in listBoxSearchResult.Items) {
                                    if (searchResult.NamedObject.Id == namedObject.Id) {
                                        searchResult.DeleteMe = false;
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                    listBoxSearchResult.Items.Add(new SearchResultItem(namedObject));
                            }
                        }
                    }
                }

                List<SearchResultItem> itemsToDelete = new();

                foreach (SearchResultItem searchResult in listBoxSearchResult.Items)
                    if (searchResult.DeleteMe)
                        itemsToDelete.Add(searchResult);

                foreach (SearchResultItem searchResult in itemsToDelete)
                    listBoxSearchResult.Items.Remove(searchResult);

                if (listBoxSearchResult.SelectedItem == null && listBoxSearchResult.Items.Count > 0)
                    listBoxSearchResult.SelectedItem = listBoxSearchResult.Items[0];

                lastSearch = newSearch;
                UpdateInstructions();
            }
        }

        private void ForceSearch() {
            lastSearch = "";
            Search();
        }

        private class SearchResultItem {
            public SearchResultItem(LayoutNamedTrainObject namedObject) {
                this.NamedObject = namedObject;
            }

            public LayoutNamedTrainObject NamedObject { get; }

            public bool DeleteMe { get; set; }

            public override string ToString() => NamedObject.DisplayName;
        }

        private void TextBoxSearch_TextChanged(object? sender, EventArgs e) {
            Search();
        }

        private void ListBoxSearchResult_MeasureItem(object? sender, MeasureItemEventArgs e) {
            SearchResultItem searchResult = (SearchResultItem)listBoxSearchResult.Items[e.Index];

            LayoutManager.CommonUI.Controls.LocomotiveListItemPainter.Measure(e, searchResult.NamedObject.Element);
        }

        private void ListBoxSearchResult_DrawItem(object? sender, DrawItemEventArgs e) {
            SearchResultItem searchResult = (SearchResultItem)listBoxSearchResult.Items[e.Index];

            LayoutManager.CommonUI.Controls.LocomotiveListItemPainter.Draw(e, searchResult.NamedObject.Element, catalog, true);
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (listBoxSearchResult.SelectedItem == null) {
                MessageBox.Show(this, "You have not selected a locomotive or train", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBoxSearch.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonNew_Click(object? sender, EventArgs e) {
            var m = new ContextMenuStrip();

            m.Items.Add("Locomotive...", null, (_, _) => {
                Dispatch.Call.AddNewLocomotiveToCollection();
                ForceSearch();
            });

            m.Items.Add("Train...", null, (_, _) => {
                Dispatch.Call.AddNewTrainToCollection();
                ForceSearch();
            });

            m.Show(buttonNew.Parent, new Point(buttonNew.Left, buttonNew.Bottom));
        }

        private void ListBoxSearchResult_SelectedIndexChanged(object? sender, EventArgs e) {
            var selected = Selected;

            if (selected != null) {
                if (selected.Element.Name == "Locomotive")
                    trainLengthDiagram.Length = TrainLength.Standard;
                else if (selected.Element.Name == "Train")
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
