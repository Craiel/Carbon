using SlimDX.DirectInput;

namespace Carbon.Engine.Contracts.Logic
{
    public interface IKeyStateReceiver
    {
        int Order { get; }

        void ReceivePersists(Key key, ref bool isHandled);
        void ReceivePressed(Key key, ref bool isHandled);
        void ReceiveReleased(Key key, ref bool isHandled);
    }

    public interface IKeyStateManager : IEngineComponent
    {
        void RegisterReceiver(IKeyStateReceiver receiver);
        void UnregisterReceiver(IKeyStateReceiver receiver);
    }
}
