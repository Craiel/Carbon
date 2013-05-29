using Core.Engine.Contracts.Logic;
using GrandSeal.Logic;

namespace GrandSeal.Contracts
{
    public interface IGrandSeal : ICarbonGame
    {
        void SwitchScene(SceneKeys key);
        void Reload();
    }
}
