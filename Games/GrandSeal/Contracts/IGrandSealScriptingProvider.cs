namespace GrandSeal.Contracts
{
    using Core.Engine.Contracts.Logic;

    public interface IGrandSealScriptingProvider : IScriptingProvider
    {
        void SwitchScene(string scene);
        void ToggleFrameRateLimit();
        void SetFrameRate(int value);
        void SetGameSpeed(float value);
        void Reload();
    }
}
