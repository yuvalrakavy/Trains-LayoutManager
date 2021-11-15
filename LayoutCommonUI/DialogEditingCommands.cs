namespace LayoutManager.CommonUI {
    public interface IControlSupportViewOnly {
        bool ViewOnly { get; set; }
    }

    public abstract class DialogEditingCommandBase {
        protected Control? control;

        protected DialogEditingCommandBase() {
            this.control = null;
        }

        protected DialogEditingCommandBase(Control control) {
            this.control = control;
        }

        public abstract void Do();

        public abstract void Undo();
    }

    public class DialogEditingRemoveControl : DialogEditingCommandBase {
        public DialogEditingRemoveControl(Control control) : base(control) {
        }

        public override void Do() {
            if(control != null)
                control.Visible = false;
        }

        public override void Undo() {
            if(control != null)
                control.Visible = true;
        }
    }

    public class DialogEditingDisableControl : DialogEditingCommandBase {
        public DialogEditingDisableControl(Control control) : base(control) {
        }

        public override void Do() {
            if(control != null)
                control.Enabled = false;
        }

        public override void Undo() {
            if(control != null)
                control.Enabled = true;
        }
    }

    public class DialogEditingChangeText : DialogEditingCommandBase {
        private string text;

        public DialogEditingChangeText(Control control, string newText) : base(control) {
            this.text = newText;
        }

        public override void Do() {
            if (control != null) {
                string oldText = control.Text;

                control.Text = text;
                text = oldText;
            }
        }

        public override void Undo() {
            Do();
        }
    }

    public class DialogEditingMoveControl : DialogEditingCommandBase {
        private Point location;

        public DialogEditingMoveControl(Control control, Point newLocation) : base(control) {
            location = newLocation;
        }

        public override void Do() {
            if (control != null) {
                Point oldLocation = control.Location;

                control.Parent.SuspendLayout();

                control.Location = this.location;
                this.location = oldLocation;

                control.Parent.ResumeLayout();
            }
        }

        public override void Undo() {
            Do();
        }
    }

    public class DialogEditingRemoveMenuEntry : DialogEditingCommandBase {
        private readonly ToolStripDropDownMenu menu;
        private readonly ToolStripMenuItem item;
        private int index;

        public DialogEditingRemoveMenuEntry(ToolStripDropDownMenu menu, ToolStripMenuItem item) {
            this.menu = menu;
            this.item = item;
        }

        public override void Do() {
            index = menu.Items.IndexOf(item);
            menu.Items.Remove(item);
        }

        public override void Undo() {
            menu.Items.Insert(index, item);
        }
    }

    public class DialogEditingResizeControl : DialogEditingCommandBase {
        private Size size;

        public DialogEditingResizeControl(Control control, Size newSize) : base(control) {
            this.size = newSize;
        }

        public override void Do() {
            if (control != null) {
                Size oldSize = control.Size;

                control.Parent.SuspendLayout();

                control.Size = size;
                size = oldSize;

                control.Parent.ResumeLayout();
            }
        }

        public override void Undo() {
            Do();
        }
    }

    public class CheckBoxWithViewOnly : CheckBox, IControlSupportViewOnly {
        private bool viewOnly = false;

        protected override void OnClick(EventArgs e) {
            if (!viewOnly)
                base.OnClick(e);
        }

        public bool ViewOnly {
            get {
                return !GetStyle(ControlStyles.Selectable);
            }

            set {
                if (value)
                    SetStyle(ControlStyles.Selectable, false);
                else
                    SetStyle(ControlStyles.Selectable, true);
                viewOnly = value;
            }
        }
    }

    public class RadioButtonWithViewOnly : RadioButton, IControlSupportViewOnly {
        private bool viewOnly = false;

        protected override void OnClick(EventArgs e) {
            if (!viewOnly)
                base.OnClick(e);
        }

        public bool ViewOnly {
            get {
                return !GetStyle(ControlStyles.Selectable);
            }

            set {
                if (value)
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
        private readonly ContainerControl form;
        private readonly DialogEditingCommandBase[] editingCommands;
        private bool viewOnly = false;

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

        private void SetViewOnly(Control control, bool viewOnly) {
            if (control is IControlSupportViewOnly viewOnlyControl)
                viewOnlyControl.ViewOnly = viewOnly;
            else {
                foreach (Control c in control.Controls)
                    SetViewOnly(c, viewOnly);
            }
        }

        public bool ViewOnly {
            set {
                foreach (Control c in form.Controls)
                    SetViewOnly(c, value);
                viewOnly = value;
            }

            get {
                return viewOnly;
            }
        }
    }
}
