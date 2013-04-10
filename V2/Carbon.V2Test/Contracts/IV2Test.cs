using Carbon.Engine.Contracts.Logic;

namespace Carbon.V2Test.Contracts
{
    using Carbon.V2Test.Logic;

    public interface IV2Test : ICarbonGame
    {
        void SwitchScene(SceneKeys key);
        void Reload();
    }
}
