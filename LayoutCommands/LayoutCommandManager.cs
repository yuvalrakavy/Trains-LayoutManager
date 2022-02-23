using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MethodDispatcher;

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
        public abstract override string ToString();

        /// <summary>
        /// True if this command returned the model to its presistent state
        /// </summary>
        public bool IsCheckpoint => false;
    }

    /// <summary>
    /// Manage Undo/Redo queue for layout commands
    /// </summary>
    public class LayoutCommandManager : ILayoutCommandManager {
        private readonly List<ILayoutCommand> commands = new(capacity: 20);
        private int iCommand = 0;
        private int changeLevel = 0;

        public int ChangeLevel {
            get {
                return changeLevel;
            }

            set {
                int previousValue = changeLevel;

                changeLevel = value;

                if (changeLevel == 0 && previousValue != 0)
                    Dispatch.Notification.OnModelNotModified();
                else if (changeLevel != 0 && previousValue == 0)
                    Dispatch.Notification.OnModelModified();
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
            int i = GetUndoIndex();

            if (i < 0)
                throw new LayoutCommandException("Undo command stack empty");

            commands[i].Undo();
            iCommand = i;
            ChangeLevel--;
        }

        /// <summary>
        /// Redo the last undone command
        /// </summary>
        public void Redo() {
            int i = GetRedoIndex();

            if (i >= commands.Count)
                throw new LayoutCommandException("No command to redo");

            commands[i].Do();      // Redo the command
            iCommand = i + 1;
            ChangeLevel++;
        }

        /// <summary>
        /// Return true if there is a command that can be undone
        /// </summary>
        public bool CanUndo => !(GetUndoIndex() < 0);

        /// <summary>
        /// Return true if there is a command that can be redone
        /// </summary>
        public bool CanRedo => GetRedoIndex() < commands.Count;

        /// <summary>
        /// Return the command name that can be undone
        /// </summary>
        public string UndoCommandName {
            get {
                int i = GetUndoIndex();

                if (i < 0)
                    throw new LayoutCommandException("No command to undo");

                return commands[i].ToString();
            }
        }

        /// <summary>
        /// Return the command name that can be redone
        /// </summary>
        public string RedoCommandName {
            get {
                int i = GetRedoIndex();

                if (i >= commands.Count)
                    throw new LayoutCommandException("No command to redo");

                return commands[i].ToString();
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
        private int GetUndoIndex() {
            int i;

            for (i = iCommand - 1; i >= 0 && commands[i].IsCheckpoint; i--)
                ;

            return i;
        }

        /// <summary>
        /// Get the index of the next command to be redone
        /// </summary>
        /// <returns>The index, or commands.Count if there is no command that can
        /// be redone</returns>
        private int GetRedoIndex() {
            int i;

            for (i = iCommand; i < commands.Count && commands[i].IsCheckpoint; i++)
                ;

            return i;
        }

        #endregion
    }

    /// <summary>
    /// A command that represents a series of commands that are done (or done) together. To
    /// the user this series of commands is presented as a single command.
    /// </summary>
    public class LayoutCompoundCommand : LayoutCommand, ILayoutCompoundCommand, IEnumerable<ILayoutCommand> {
        private readonly List<ILayoutCommand> commands = new();
        private bool executeAtOnce = false;

        public LayoutCompoundCommand(String description) {
            this.Description = description;
        }

        public LayoutCompoundCommand(String description, bool executeAtOnce) {
            this.Description = description;
            this.executeAtOnce = executeAtOnce;
        }

        public string Description { set; get; }

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
            for (int i = commands.Count - 1; i >= 0; i--)
                commands[i].Undo();

            executeAtOnce = false;      // So Do (which now really means redo, will execute the commands)
        }

        public override string ToString() => Description;

        #region IEnumerable Members

        public IEnumerator<ILayoutCommand> GetEnumerator() => commands.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    public class LayoutCommandException : Exception {
        public LayoutCommandException(String message) : base(message) {
        }

        public LayoutCommandException() : base() {
        }

        public LayoutCommandException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
