namespace Core.Engine.Contracts.Logic
{
    using System;

    using CarbonCore.Utils.Compat.Contracts;
    
    public interface IEngineComponent : IDisposable
    {
        void Initialize(ICarbonGraphics graphics);
        void Unload();

        bool Update(ITimer gameTime);
    }
}
