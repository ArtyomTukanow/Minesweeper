using System;
using JetBrains.Annotations;
using UserData.TileMap;

namespace CommandSystem.Commands
{
    public abstract class UserCommandBase : ICommand, IUserCommand
    {
        public const string CMD_SEPARATOR = ":";
        public const char SEPARATOR = ',';

        public const int TYPE_INDEX = 0;
        
        public abstract void Redo();
        public abstract void Undo();

        public abstract string Type { get; }
        
        public UserMap UserMap { get; set; }

        public UserCommandBase(UserMap userMap)
        {
            UserMap = userMap;
        }

        public UserCommandBase([NotNull]string[] data)
        {
            if(data[0] != Type)
                throw new Exception("Type: " + Type + " does not match with cmd: " + string.Join(",", data));
            Deserialize(data);
        }
        
        
        public virtual string Serialize()
        {
            return Type + CMD_SEPARATOR;
        }

        public abstract void Deserialize(string[] data);
    }
}