namespace Core.Engine.Contracts.Rendering
{
    using System;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;

    using SlimDX;
    using SlimDX.Direct3D11;


    public interface IRenderTarget : IDisposable
    {
        Viewport Viewport { get; }

        void Resize(ICarbonGraphics graphics, TypedVector2<int> size);

        void Clear(ICarbonGraphics graphics, Vector4 color);
        void Set(ICarbonGraphics graphics);
    }
}
