using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for TripPlanList.
    /// </summary>
    public partial class TripPlanList : System.Windows.Forms.UserControl {

        public event EventHandler? SelectedTripPlanChanged = null;

        public TripPlanList() {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }

        public XmlElement ApplicableTripPlansElement {
            get {
                return applicableTripPlansElement;
            }

            set {
                applicableTripPlansElement = value;
                if (initialized)
                    FillList();
            }
        }

        public TripPlanInfo? SelectedTripPlan {
            get {
                if (initialized) {
                    if (listViewTripPlans.SelectedItems.Count > 0) {
                        TripPlanItem selected = (TripPlanItem)listViewTripPlans.SelectedItems[0];

                        return selected.TripPlan;
                    }

                    return null;
                }
                else
                    return tripPlanToSelect;
            }

            set {
                if (initialized) {
                    foreach (TripPlanItem item in listViewTripPlans.Items)
                        item.Selected = false;

                    if (value != null) {
                        foreach (TripPlanItem item in listViewTripPlans.Items) {
                            if (item.TripPlan.Id == value.Id) {
                                item.Selected = true;

                                SelectedTripPlanChanged?.Invoke(this, EventArgs.Empty);

                                break;
                            }
                        }
                    }
                }
                else
                    tripPlanToSelect = value;
            }
        }

        public bool ShouldReverseSelectedTripPlan {
            get {
                if (initialized) {
                    if (listViewTripPlans.SelectedItems.Count > 0) {
                        TripPlanItem selected = (TripPlanItem)listViewTripPlans.SelectedItems[0];

                        return selected.Reversed;
                    }
                    else
                        throw new ApplicationException("No trip plan is selected");
                }
                else
                    throw new ApplicationException("You should first Initialize the control");
            }
        }

        public void Initialize() {
            tripPlanIconList = LayoutModel.Instance.TripPlanIconList;
            listViewTripPlans.LargeImageList = tripPlanIconList.LargeIconImageList;
            listViewTripPlans.StateImageList = imageListState;

            if (applicableTripPlansElement != null)
                FillList();

            initialized = true;

            if (tripPlanToSelect != null)
                SelectedTripPlan = tripPlanToSelect;
        }

        private void FillList() {
            listViewTripPlans.Items.Clear();

            foreach (XmlElement applicableTripPlanElement in applicableTripPlansElement) {
                var originalTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[(Guid)applicableTripPlanElement.AttributeValue(A_TripPlanID)];
                var shouldReverse = (bool)applicableTripPlanElement.AttributeValue(A_ShouldReverse);
                var tripPlan = originalTripPlan;

                if(tripPlan != null)
                    listViewTripPlans.Items.Add(new TripPlanItem(tripPlan, shouldReverse, tripPlanIconList));
            }

            if (listViewTripPlans.Items.Count > 0)
                SelectedTripPlan = ((TripPlanItem)listViewTripPlans.Items[0]).TripPlan;

            UpdateButtons();
        }

        private void UpdateButtons() {
            if (listViewTripPlans.SelectedItems.Count > 0) {
                buttonDuplicate.Enabled = listViewTripPlans.SelectedItems.Count == 1;
                buttonRename.Enabled = listViewTripPlans.SelectedItems.Count == 1;
                buttonDelete.Enabled = true;
                buttonChangeIcon.Enabled = listViewTripPlans.SelectedItems.Count == 1;
            }
            else {
                buttonDuplicate.Enabled = false;
                buttonRename.Enabled = false;
                buttonDelete.Enabled = false;
                buttonChangeIcon.Enabled = false;
            }
        }

        private void UpdateAll() {
            foreach (TripPlanItem item in listViewTripPlans.Items)
                item.Update();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Event Handlers

        private void ButtonRemove_Click(object? sender, System.EventArgs e) {
            if (listViewTripPlans.SelectedItems.Count > 0) {
                foreach (TripPlanItem tripPlanItem in listViewTripPlans.SelectedItems) {
                    LayoutModel.StateManager.TripPlansCatalog.TripPlans.Remove(tripPlanItem.TripPlan.Id);
                    listViewTripPlans.Items.Remove(tripPlanItem);
                }
            }
        }

        private void ListViewTripPlans_SelectedIndexChanged(object? sender, System.EventArgs e) {
            if (listViewTripPlans.SelectedItems.Count > 0) {
                TripPlanItem tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];

                SelectedTripPlanChanged?.Invoke(this, EventArgs.Empty);
            }
            else
                SelectedTripPlan = null;

            UpdateButtons();
        }

        private void ButtonChangeIcon_Click(object? sender, System.EventArgs e) {
            if (listViewTripPlans.SelectedItems.Count > 0) {
                var tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];
                var tripPlan = Ensure.NotNull<TripPlanInfo>(LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanItem.TripPlan.Id]);  // Get real version (non-reversed)

                LayoutManager.Tools.Dialogs.TripPlanChangeIcon d = new(tripPlan, tripPlanIconList);

                tripPlanIconList.IconListModified = false;

                if (d.ShowDialog(this) == DialogResult.OK)
                    tripPlanItem.Update();

                tripPlanItem.TripPlan.IconId = tripPlan.IconId;     // Make sure that the reversed version has the same icon ID

                if (tripPlanIconList.IconListModified) {
                    LayoutModel.WriteModelXmlInfo();
                    UpdateAll();
                }
            }
        }

        private void ButtonRename_Click(object? sender, System.EventArgs e) {
            if (listViewTripPlans.SelectedItems.Count > 0) {
                TripPlanItem tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];

                tripPlanItem.BeginEdit();
            }
        }

        private void ListViewTripPlans_AfterLabelEdit(object? sender, System.Windows.Forms.LabelEditEventArgs e) {
            var selected = (TripPlanItem)listViewTripPlans.Items[e.Item];
            var existingTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[e.Label];

            if (existingTripPlan != null && existingTripPlan.Id != selected.TripPlan.Id) {
                MessageBox.Show(this, "A trip plan with this name already exists", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
            }
            else {
                var tripPlan = Ensure.NotNull<TripPlanInfo>(LayoutModel.StateManager.TripPlansCatalog.TripPlans[selected.TripPlan.Id]);

                tripPlan.Name = e.Label;
                selected.TripPlan.Name = e.Label;
                selected.Update();
            }
        }

        private void ButtonDuplicate_Click(object? sender, System.EventArgs e) {
            if (listViewTripPlans.SelectedItems.Count > 0) {
                TripPlanItem tripPlanItem = (TripPlanItem)listViewTripPlans.SelectedItems[0];
                XmlElement newTripPlanElement;

                if (tripPlanItem.TripPlan.Element.OwnerDocument != LayoutModel.StateManager.TripPlansCatalog.Element.OwnerDocument)
                    newTripPlanElement = (XmlElement)LayoutModel.StateManager.TripPlansCatalog.Element.OwnerDocument.ImportNode(tripPlanItem.TripPlan.Element, true);
                else
                    newTripPlanElement = (XmlElement)tripPlanItem.TripPlan.Element.CloneNode(true);

                TripPlanInfo newTripPlan = new(newTripPlanElement);

                newTripPlan.Name = "Copy of " + newTripPlan.Name;
                LayoutModel.StateManager.TripPlansCatalog.TripPlans.Add(newTripPlan);
                listViewTripPlans.Items.Add(new TripPlanItem(newTripPlan, false, tripPlanIconList));
            }
        }

        #endregion

        #region TripPlanItem class

        private class TripPlanItem : ListViewItem {
            private readonly TripPlanIconListInfo tripPlanIconList;

            public TripPlanItem(TripPlanInfo tripPlan, bool reversed, TripPlanIconListInfo tripPlanIconList) {
                this.TripPlan = tripPlan;
                this.Reversed = reversed;
                this.tripPlanIconList = tripPlanIconList;

                Text = tripPlan.Name;
                ImageIndex = tripPlanIconList[tripPlan.IconId];

                IList<TripPlanWaypointInfo> wayPoints = tripPlan.Waypoints;
                SubItems.Add(wayPoints[wayPoints.Count - 1].Destination.Name);

                if (reversed)
                    StateImageIndex = 0;
            }

            public void Update() {
                Text = TripPlan.Name;
                ImageIndex = tripPlanIconList[TripPlan.IconId];
            }

            public TripPlanInfo TripPlan { get; }

            public bool Reversed { get; }
        }

        #endregion
    }
}
