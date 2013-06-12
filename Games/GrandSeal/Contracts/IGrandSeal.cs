using Core.Engine.Contracts.Logic;

namespace GrandSeal.Contracts
{
    public interface IGrandSeal : ICarbonGame
    {
        void SwitchScene(int key);
        void Reload();
    }
}
