using LayoutManager.CommonUI;
using LayoutManager.Model;
using System;
using System.Drawing;
using System.Windows.Forms;
using MethodDispatcher;

/// This file contain implementation of commands that are carried out on a selection
/// 

namespace LayoutManager {
    public class MenuItemDeleteSelection : LayoutMenuItem {
        public MenuItemDeleteSelection() {
            this.Text = "&Delete";
        }

        protected override void OnClick(EventArgs e) {
            LayoutCompoundCommand deleteSelectionCommand = new("delete selection");

            foreach (ModelComponent component in LayoutController.UserSelection) {
                deleteSelectionCommand.Add(new LayoutComponentDeselectCommand(LayoutController.UserSelection, component, "unselect"));
                Dispatch.Call.PrepareForComponentRemoveCommand(component, deleteSelectionCommand);       
                deleteSelectionCommand.Add(new LayoutComponentRemovalCommand(component, "delete"));
            }

            LayoutController.Do(deleteSelectionCommand);
        }
    }

    public class MenuItemCopySelection : LayoutMenuItem {
        private Point location;

        public MenuItemCopySelection(Point location) {
            this.location = location;

            this.Text = "&Copy";

            if (LayoutController.UserSelection.Count == 0)
                this.Enabled = false;
        }

        public void Copy() {
            Clipboard.SetDataObject(LayoutController.UserSelection.GetDataObject(location));
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            Copy();
        }
    }

    public class MenuItemCutSelection : LayoutMenuItem {
        private Point location;

        public MenuItemCutSelection(Point location) {
            this.location = location;

            this.Text = "Cu&t";

            if (LayoutController.UserSelection.Count == 0)
                this.Enabled = false;
        }

        public void Cut() {
            Clipboard.SetDataObject(LayoutController.UserSelection.GetDataObject(location));

            LayoutCompoundCommand deleteSelectionCommand = new("Cut selection");

            foreach (ModelComponent component in LayoutController.UserSelection) {
                deleteSelectionCommand.Add(new LayoutComponentDeselectCommand(LayoutController.UserSelection, component, "unselect"));
                Dispatch.Call.PrepareForComponentRemoveCommand(component, deleteSelectionCommand);
                deleteSelectionCommand.Add(new LayoutComponentRemovalCommand(component, "delete"));
            }

            LayoutController.Do(deleteSelectionCommand);
        }

        protected override void OnClick(EventArgs e) {
            base.OnClick(e);
            Cut();
        }
    }
}



