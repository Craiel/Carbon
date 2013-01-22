using System;

using Carbon.Engine.Contracts.Logic;

using SlimDX;
using SlimDX.Direct3D11;

namespace Carbon.Engine.Contracts.Rendering
{
    public interface IRenderTarget : IDisposable
    {
        Viewport Viewport { get; }

        void Resize(ICarbonGraphics graphics, int width, int height);

        void Clear(ICarbonGraphics graphics, Vector4 color);
        void Set(ICarbonGraphics graphics);
    }
}
