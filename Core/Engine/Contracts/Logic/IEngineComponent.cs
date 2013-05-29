using System;

using Core.Utils.Contracts;

namespace Core.Engine.Contracts.Logic
{
    public interface IEngineComponent : IDisposable
    {
        void Initialize(ICarbonGraphics graphics);
        void Unload();

        bool Update(ITimer gameTime);
    }
}
