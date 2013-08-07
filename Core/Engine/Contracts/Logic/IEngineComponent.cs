namespace Core.Engine.Contracts.Logic
{
    using System;

    using Core.Utils.Contracts;

    public interface IEngineComponent : IDisposable
    {
        void Initialize(ICarbonGraphics graphics);
        void Unload();

        bool Update(ITimer gameTime);
    }
}
