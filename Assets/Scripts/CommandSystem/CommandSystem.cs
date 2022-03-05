using System;
using System.Collections.Generic;
using System.Linq;
using AllUtils;
using CommandSystem.Commands;
using JetBrains.Annotations;
using UnityEngine;

namespace CommandSystem
{
    public class CommandSystem
    {
        private readonly List<ICommand> commandRedoPull = new List<ICommand>();
        private readonly List<ICommand> commandUndoPull = new List<ICommand>();

        public event Action<ICommand> OnCommand;
        public event Action<ICommand> OnUndo;
        public event Action<ICommand> OnRedo;

        [CanBeNull]
        public ICommand LastCommand => commandRedoPull.FirstOrDefault();

        protected virtual void RedoCommand(ICommand cmd, bool clearUndo = true, bool invokeUpdate = true)
        {
            if(clearUndo)
                commandUndoPull.Clear();
            // if(commandRedoPull.Count == commandRedoPull.Capacity)
                // commandRedoPull.RemoveAt(commandRedoPull.Capacity - 1);
            commandRedoPull.Insert(0, cmd);
            
            cmd.Redo();

            if (invokeUpdate)
            {
                OnRedo?.Invoke(cmd);
                OnCommand?.Invoke(cmd);
            }
        }

        protected virtual void UndoCommand(ICommand cmd)
        {
            // if(commandUndoPull.Count == commandUndoPull.Capacity)
                // commandUndoPull.RemoveAt(commandUndoPull.Capacity - 1);
            commandUndoPull.Insert(0, cmd);
            
            cmd.Undo();
            OnUndo?.Invoke(cmd);
            OnCommand?.Invoke(cmd);
        }
        
        public void Undo()
        {
            if(commandRedoPull.Count > 0)
                UndoCommand(commandRedoPull.Shift());
        }

        public void Redo()
        {
            if(commandUndoPull.Count > 0)
                RedoCommand(commandUndoPull.Shift(), false);
        }
    }
}