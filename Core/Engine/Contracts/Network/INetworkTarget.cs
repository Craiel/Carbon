namespace Core.Engine.Contracts.Network
{
    public interface INetworkTarget
    {
        string Address { get; }

        int Port { get; }
    }
}
