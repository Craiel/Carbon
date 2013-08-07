namespace Core.Engine.Contracts.Logic
{
    using System;

    using Core.Engine.Logic;
    using SlimDX.Direct3D11;
    using SlimDX.DXGI;

    public interface ICarbonGraphics : IDisposable
    {
        IntPtr TargetHandle { get; set; }

        DeviceContext ImmediateContext { get; }

        RenderTargetView BackBufferView { get; }

        DeviceStateManager StateManager { get; }
        ShaderManager ShaderManager { get; }
        TextureManager TextureManager { get; }

        DeviceCreationFlags CreationFlags { get; set; }

        CullMode CullMode { get; set; }
        FillMode FillMode { get; set; }

        bool IsDepthEnabled { get; set; }

        Viewport WindowViewport { get; }
        
        void Reset();

        void Resize(TypedVector2<int> size);
        
        void UpdateStates();

        void Present(PresentFlags flags);

        void ClearCache();
    }
}
