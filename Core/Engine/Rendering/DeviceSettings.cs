using Core.Engine.Logic;

using SlimDX.Direct3D11;

namespace Core.Engine.Rendering
{
    public class DeviceSettings
    {
        public DeviceCreationFlags CreationFlags { get; set; }

        public TypedVector2<int> ScreenSize { get; set; }
    }
}
