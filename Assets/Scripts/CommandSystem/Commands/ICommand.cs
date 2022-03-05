namespace CommandSystem.Commands
{
    public interface ICommand
    {
        void Redo();
        void Undo();
    }
}