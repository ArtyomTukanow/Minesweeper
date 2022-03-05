using System;
using CommandSystem.Commands;

namespace Exceptions
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(ICommand cmd, string reason) : base($"Command: {cmd.GetType().Name} can't execute case: {reason}")
        {
            
        }
        public InvalidCommandException(ICommand cmd) : base($"Command: {cmd.GetType().Name} can't execute!")
        {
            
        }
    }
}