namespace GrandSeal.Contracts
{
    using Core.Engine.Contracts.Logic;

    public interface IGrandSeal : ICarbonGame
    {
        void SwitchScene(int key);
        void Reload();
    }
}
