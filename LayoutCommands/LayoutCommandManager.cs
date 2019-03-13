using System;
using System.Collections;
using System.Diagnostics;

namespace LayoutManager {
    /// <summary>
    /// Base class for all layout commands. Basically a command encupsolte an operation
    /// (in most cases on the model). The command has enough information so it can
    /// undo the command
    /// </summary>
    public abstract class LayoutCommand : ILayoutCommand {

        /// <summary>
        /// Preform the operation of this command 
        /// </summary>
        public abstract void Do();

        /// <summary>
        /// Undo the operation done by this command
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// return a display description for the command. Used in Redo ... and
        /// Undo ... menu entries
        /// </summary>
        /// <returns></returns>
        public abstract override String ToString();

        /// <summary>
        /// True if this command returned the model to its presistent state
        /// </summary>
        public bool IsCheckpoint => false;
    }

    /// <summary>
    /// Manage Undo/Redo queue for layout commands
    /// </summary>
    public class LayoutCommandManager : ILayoutCommandManager {
        readonly ArrayList commands = new ArrayList(20);
        int iCommand = 0;
        int changeLevel = 0;

        public int ChangeLevel {
            get {
                return changeLevel;
            }

            set {
                int previousValue = changeLevel;

                changeLevel = value;

                if (changeLevel == 0 && previousValue != 0)
                    EventManager.Event(new LayoutEvent("model-not-modified", this));
                else if (changeLevel != 0 && previousValue == 0)
                    EventManager.Event(new LayoutEvent("model-modified", this));
            }
        }

        /// <summary>
        /// Execute a command and add it to the command stack
        /// </summary>
        /// <param name="command">The command object to be carried out</param>
        public void Do(ILayoutCommand command) {
            // In case that commands where undone, remove those (since you cannot
            // redo a commands after a new command is introduced) from the command
            // list.
            if (iCommand < commands.Count)
                commands.RemoveRange(iCommand, commands.Count - iCommand);

            Debug.Assert(iCommand == commands.Count);
            commands.Add(command);

            iCommand++;

            if (!command.IsCheckpoint)
                command.Do();

            ChangeLevel++;
        }

        /// <summary>
        /// Undo the "last" command
        /// </summary>
        public void Undo() {
            int i = getUndoIndex();

            if (i < 0)
                throw new LayoutCommandException("Undo command stack empty");

            ((LayoutCommand)commands[i]).Undo();
            iCommand = i;
            ChangeLevel--;
        }

        /// <summary>
        /// Redo the last undone command
        /// </summary>
        public void Redo() {
            int i = getRedoIndex();

            if (i >= commands.Count)
                throw new LayoutCommandException("No command to redo");

            ((LayoutCommand)commands[i]).Do();      // Redo the command
            iCommand = i + 1;
            ChangeLevel++;
        }

        /// <summary>
        /// Return true if there is a command that can be undone
        /// </summary>
        public bool CanUndo => !(getUndoIndex() < 0);

        /// <summary>
        /// Return true if there is a command that can be redone
        /// </summary>
        public bool CanRedo => getRedoIndex() < commands.Count;

        /// <summary>
        /// Return the command name that can be undone
        /// </summary>
        public string UndoCommandName {
            get {
                int i = getUndoIndex();

                if (i < 0)
                    throw new LayoutCommandException("No command to undo");

                return ((LayoutCommand)commands[i]).ToString();
            }
        }

        /// <summary>
        /// Return the command name that can be redone
        /// </summary>
        public string RedoCommandName {
            get {
                int i = getRedoIndex();

                if (i >= commands.Count)
                    throw new LayoutCommandException("No command to redo");

                return ((LayoutCommand)commands[i]).ToString();
            }
        }

        /// <summary>
        /// Clear the undo command stack
        /// </summary>
        public void Clear() {
            commands.Clear();
            iCommand = 0;
            ChangeLevel = 0;
        }

        #region Utility functions

        /// <summary>
        /// Get the index of the command to be undone
        /// </summary>
        /// <returns>The index of the command to be undone. -1 is returned if
        /// there is no such command</returns>
        private int getUndoIndex() {
            int i;

            for (i = iCommand - 1; i >= 0 && ((LayoutCommand)commands[i]).IsCheckpoint; i--)
                ;

            return i;
        }

        /// <summary>
        /// Get the index of the next command to be redone
        /// </summary>
        /// <returns>The index, or commands.Count if there is no command that can
        /// be redone</returns>
        private int getRedoIndex() {
            int i;

            for (i = iCommand; i < commands.Count && ((LayoutCommand)commands[i]).IsCheckpoint; i++)
                ;

            return i;
        }

        #endregion
    }

    /// <summary>
    /// A command that represents a series of commands that are done (or done) together. To
    /// the user this series of commands is presented as a single command.
    /// </summary>
    public class LayoutCompoundCommand : LayoutCommand, ILayoutCompoundCommand, IEnumerable {
        readonly ArrayList commands = new ArrayList();
        String description;
        bool executeAtOnce = false;

        public LayoutCompoundCommand(String description) {
            this.description = description;
        }

        public LayoutCompoundCommand(String description, bool executeAtOnce) {
            this.description = description;
            this.executeAtOnce = executeAtOnce;
        }

        public String Description {
            set {
                description = value;
            }

            get {
                return description;
            }
        }

        public void Add(ILayoutCommand command) {
            commands.Add(command);

            if (executeAtOnce)
                command.Do();
        }

        public int Count => commands.Count;

        public override void Do() {
            if (!executeAtOnce) {
                foreach (ILayoutCommand command in commands) {
                    command.Do();
                }
            }
        }

        public override void Undo() {
            for (int i = commands.Count - 1; i >= 0; i--) {
                ILayoutCommand command = (LayoutCommand)commands[i];

                command.Undo();
            }

            executeAtOnce = false;      // So Do (which now really means redo, will execute the commands)
        }

        public override String ToString() => description;

        #region IEnumerable Members

        public IEnumerator GetEnumerator() => commands.GetEnumerator();

        #endregion
    }


    public class LayoutCommandException : Exception {
        public LayoutCommandException(String message) : base(message) {
        }
    }
}
