using System;

using Core.Utils.Contracts;

namespace Carbon.Engine.Contracts.Logic
{
    public interface IEngineComponent : IDisposable
    {
        void Initialize(ICarbonGraphics graphics);
        void Update(ITimer gameTime);
    }
}
