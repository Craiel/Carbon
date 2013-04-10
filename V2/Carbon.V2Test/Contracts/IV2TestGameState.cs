using Carbon.Engine.Contracts.Logic;

namespace Carbon.V2Test.Contracts
{
    using Carbon.Engine.Scene;

    public interface IV2TestGameState : IGameState
    {
        INodeManager NodeManager { get; }
    }
}
