using System;
using System.Drawing;
using System.Windows.Forms;

namespace LayoutManager.CommonUI {

    public interface IControlSupportViewOnly {

        bool ViewOnly { get; set; }
    }

    public abstract class DialogEditingCommandBase {
        protected Control control;

        public DialogEditingCommandBase() {
            this.control = null;
        }

        public DialogEditingCommandBase(Control control) {
            this.control = control;
        }

        public abstract void Do();

        public abstract void Undo();
    }

    public class DialogEditingRemoveControl : DialogEditingCommandBase {
        public DialogEditingRemoveControl(Control control) : base(control) {
        }

        public override void Do() {
            control.Visible = false;
        }

        public override void Undo() {
            control.Visible = true;
        }
    }

    public class DialogEditingDisableControl : DialogEditingCommandBase {
        public DialogEditingDisableControl(Control control) : base(control) {
        }

        public override void Do() {
            control.Enabled = false;
        }

        public override void Undo() {
            control.Enabled = true;
        }
    }

    public class DialogEditingChangeText : DialogEditingCommandBase {
        string text;

        public DialogEditingChangeText(Control control, string newText) : base(control) {
            this.text = newText;
        }

        public override void Do() {
            string oldText = control.Text;

            control.Text = text;
            text = oldText;
        }

        public override void Undo() {
            Do();
        }
    }

    public class DialogEditingMoveControl : DialogEditingCommandBase {
        Point location;

        public DialogEditingMoveControl(Control control, Point newLocation) : base(control) {
            location = newLocation;
        }

        public override void Do() {
            Point oldLocation = control.Location;

            control.Parent.SuspendLayout();

            control.Location = this.location;
            this.location = oldLocation;

            control.Parent.ResumeLayout();
        }

        public override void Undo() {
            Do();
        }
    }

    public class DialogEditingRemoveMenuEntry : DialogEditingCommandBase {
        readonly Menu menu;
        readonly MenuItem item;
        int index;

        public DialogEditingRemoveMenuEntry(Menu menu, MenuItem item) {
            this.menu = menu;
            this.item = item;
        }

        public override void Do() {
            index = item.Index;
            menu.MenuItems.Remove(item);
        }

        public override void Undo() {
            menu.MenuItems.Add(index, item);
        }
    }

    public class DialogEditingResizeControl : DialogEditingCommandBase {
        Size size;

        public DialogEditingResizeControl(Control control, Size newSize) : base(control) {
            this.size = newSize;
        }

        public override void Do() {
            Size oldSize = control.Size;

            control.Parent.SuspendLayout();

            control.Size = size;
            size = oldSize;

            control.Parent.ResumeLayout();
        }

        public override void Undo() {
            Do();
        }
    }

    public class CheckBoxWithViewOnly : CheckBox, IControlSupportViewOnly {
        bool viewOnly = false;

        protected override void OnClick(EventArgs e) {
            if (!viewOnly)
                base.OnClick(e);
        }

        public bool ViewOnly {
            get {
                if (GetStyle(ControlStyles.Selectable))
                    return false;
                return true;
            }

            set {
                if (value == true)
                    SetStyle(ControlStyles.Selectable, false);
                else
                    SetStyle(ControlStyles.Selectable, true);
                viewOnly = value;
            }
        }
    }

    public class RadioButtonWithViewOnly : RadioButton, IControlSupportViewOnly {
        bool viewOnly = false;

        protected override void OnClick(EventArgs e) {
            if (!viewOnly)
                base.OnClick(e);
        }

        public bool ViewOnly {
            get {
                if (GetStyle(ControlStyles.Selectable))
                    return false;
                return true;
            }

            set {
                if (value == true)
                    SetStyle(ControlStyles.Selectable, false);
                else
                    SetStyle(ControlStyles.Selectable, true);
                viewOnly = value;
            }
        }
    }

    public class TextBoxWithViewOnly : TextBox, IControlSupportViewOnly {
        public bool ViewOnly {
            set {
                this.ReadOnly = value;
            }

            get {
                return this.ReadOnly;
            }
        }
    }

    public class DialogEditing : IControlSupportViewOnly {
        readonly ContainerControl form;
        readonly DialogEditingCommandBase[] editingCommands;
        bool viewOnly = false;

        public DialogEditing(ContainerControl form, DialogEditingCommandBase[] editingCommands) {
            this.form = form;
            this.editingCommands = editingCommands;
        }

        public void Do() {
            form.SuspendLayout();
            foreach (DialogEditingCommandBase command in editingCommands)
                command.Do();
            form.ResumeLayout();
        }

        public void Undo() {
            form.SuspendLayout();
            for (int i = editingCommands.Length - 1; i >= 0; i--)
                editingCommands[i].Undo();
            form.ResumeLayout();
        }

        private void setViewOnly(Control control, bool viewOnly) {
            if (control is IControlSupportViewOnly)
                ((IControlSupportViewOnly)control).ViewOnly = viewOnly;
            else {
                foreach (Control c in control.Controls)
                    setViewOnly(c, viewOnly);
            }
        }

        public bool ViewOnly {
            set {
                foreach (Control c in form.Controls)
                    setViewOnly(c, value);
                viewOnly = value;
            }

            get {
                return viewOnly;
            }
        }
    }
}
