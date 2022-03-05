using UserData.TileMap;

namespace CommandSystem.Commands
{
    public interface IUserCommand
    {
        string Type { get; }
        UserMap UserMap { get; set; }

        string Serialize();
        void Deserialize(string[] data);
    }
}