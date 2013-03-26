using Carbon.Engine.Logic;

using SlimDX.DirectInput;

namespace Carbon.Engine.Contracts.Logic
{
    public interface IKeyStateReceiver
    {
        void ReceivePersists(Key key);
        void ReceivePressed(Key key);
        void ReceiveReleased(Key key);
    }

    public interface IInputManager : IEngineComponent, IScriptingProvider
    {
        void RegisterReceiver(IKeyStateReceiver receiver);
        void UnregisterReceiver(IKeyStateReceiver receiver);

        InputBindings RegisterBinding(string name);
        InputBindings GetBindings(string name);
    }
}
