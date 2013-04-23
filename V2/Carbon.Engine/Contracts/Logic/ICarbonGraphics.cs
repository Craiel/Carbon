using System;
using Carbon.Engine.Logic;

using SlimDX.DXGI;
using SlimDX.Direct3D11;

namespace Carbon.Engine.Contracts.Logic
{
    public interface ICarbonGraphics : IDisposable
    {
        IntPtr TargetHandle { get; set; }

        DeviceContext ImmediateContext { get; }

        RenderTargetView BackBufferView { get; }

        DeviceStateManager StateManager { get; }
        ShaderManager ShaderManager { get; }
        TextureManager TextureManager { get; }

        DeviceCreationFlags CreationFlags { get; set; }

        Viewport WindowViewport { get; }
        
        void Reset();

        void Resize(TypedVector2<int> size);

        void SetCulling(CullMode mode);

        void EnableWireframe();
        void DisableWireframe();

        void EnableDepth();
        void DisableDepth();

        void UpdateStates();

        void Present(PresentFlags flags);

        void ClearCache();
    }
}
