using Carbon.Engine.Logic;

namespace Carbon.Engine.Contracts.Logic
{
    public interface IInputReceiver
    {
        bool IsActive { get; }
        
        void ReceivePersists(string input, object argument = null);
        void ReceivePressed(string input, object argument = null);
        void ReceiveReleased(string input, object argument = null);

        void ReceiveAxisChange(string axis, float value);
    }

    public interface IInputManager : IEngineComponent, IScriptingProvider
    {
        void RegisterReceiver(IInputReceiver receiver);
        void UnregisterReceiver(IInputReceiver receiver);

        InputBindings RegisterBinding(string name);
        InputBindings GetBindings(string name);
    }
}
