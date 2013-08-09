namespace Core.Engine.Rendering
{
    using Core.Engine.Logic;

    using SlimDX.Direct3D11;

    public class DeviceSettings
    {
        public DeviceCreationFlags CreationFlags { get; set; }

        public TypedVector2<int> ScreenSize { get; set; }
    }
}
