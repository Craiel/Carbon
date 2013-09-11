namespace GrandSeal.Contracts
{
    using System;

    using Core.Engine.Contracts.Logic;

    public interface IGrandSealSettings : IEngineComponent
    {
        event Action SettingsChanged;

        void Reload();
    }
}
