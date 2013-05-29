using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using GrandSeal.Editor.Contracts;

namespace GrandSeal.Editor.Logic
{
    public class UndoRedoOperation
    {
        private readonly Action action;

        public UndoRedoOperation(Action action, string name)
        {
            this.action = action;
            this.Name = name;
        }

        public string Name { get; private set; }

        public void Execute()
        {
            this.action.Invoke();
        }
    }

    internal enum UndoRedoMode
    {
        Normal,
        Undo,
        Redo
    }

    internal class UndoRedoGroup : EditorBase, IUndoRedoGroup
    {
        private readonly Stack<UndoRedoOperation> undoStack;
        private readonly Stack<UndoRedoOperation> redoStack;

        private UndoRedoMode mode = UndoRedoMode.Normal;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UndoRedoGroup()
        {
            this.undoStack = new Stack<UndoRedoOperation>();
            this.redoStack = new Stack<UndoRedoOperation>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool CanUndo
        {
            get
            {
                return this.undoStack.Count > 0;
            }
        }

        public bool CanRedo
        {
            get
            {
                return this.redoStack.Count > 0;
            }
        }

        public ReadOnlyCollection<UndoRedoOperation> UndoOperations
        {
            get
            {
                return new List<UndoRedoOperation>(this.undoStack).AsReadOnly();
            }
        }

        public ReadOnlyCollection<UndoRedoOperation> RedoOperations
        {
            get
            {
                return new List<UndoRedoOperation>(this.redoStack).AsReadOnly();
            }
        }

        public void Undo()
        {
            UndoRedoOperation operation = this.undoStack.Pop();
            this.mode = UndoRedoMode.Undo;
            operation.Execute();
            this.mode = UndoRedoMode.Normal;
        }

        public void Redo()
        {
            UndoRedoOperation operation = this.redoStack.Pop();
            this.mode = UndoRedoMode.Redo;
            operation.Execute();
            this.mode = UndoRedoMode.Normal;
        }

        public void AddOperation(UndoRedoOperation operation)
        {
            switch (this.mode)
            {
                case UndoRedoMode.Normal:
                    {
                        this.undoStack.Push(operation);
                        this.redoStack.Clear();
                        break;
                    }

                case UndoRedoMode.Redo:
                    {
                        this.undoStack.Push(operation);
                        break;
                    }

                case UndoRedoMode.Undo:
                    {
                        this.redoStack.Push(operation);
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            this.NotifyPropertyChanged(string.Empty);
        }
    }

    public class UndoRedoManager : EditorBase, IUndoRedoManager
    {
        private readonly IDictionary<object, IUndoRedoGroup> undoRedoGroups;

        private IUndoRedoGroup activeGroup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UndoRedoManager()
        {
            this.undoRedoGroups = new Dictionary<object, IUndoRedoGroup>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IUndoRedoGroup ActiveGroup
        {
            get
            {
                return this.activeGroup;
            }

            private set
            {
                if (this.activeGroup != value)
                {
                    this.activeGroup = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public void RegisterGroup(object target)
        {
            if (this.undoRedoGroups.ContainsKey(target))
            {
                throw new InvalidOperationException("Target already has a group registered");
            }

            this.undoRedoGroups.Add(target, new UndoRedoGroup());
        }

        public void ActivateGroup(object target)
        {
            if (!this.undoRedoGroups.ContainsKey(target))
            {
                throw new InvalidOperationException("target has no group to activate");
            }

            this.ActiveGroup = this.undoRedoGroups[target];
        }

        public void ReleaseGroup(object target)
        {
            if (!this.undoRedoGroups.ContainsKey(target))
            {
                throw new InvalidOperationException("Target has no group to release");
            }

            this.undoRedoGroups.Remove(target);
            if (this.ActiveGroup == target)
            {
                this.ActiveGroup = null;
            }
        }

        public void AddOperation(Action action, string name = "unknown")
        {
            if (this.activeGroup == null)
            {
                return;
            }

            this.activeGroup.AddOperation(new UndoRedoOperation(action, name));
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
