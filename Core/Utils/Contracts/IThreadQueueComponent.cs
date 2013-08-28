namespace Core.Utils.Contracts
{
    public interface IThreadQueueComponent
    {
        bool HasQueuedOperations { get; }
    }
}
