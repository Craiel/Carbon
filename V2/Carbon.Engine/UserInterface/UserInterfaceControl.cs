using Carbon.Engine.Contracts.Logic;

using SlimDX;

namespace Carbon.Engine.UserInterface
{
    public interface IUserInterfaceControl : IEngineComponent
    {
        Vector2 Position { get; set; }
        Vector2 Scale { get; set; }
        Quaternion Rotation { get; set; }

        Vector2 AbsolutePosition { get; }
        Vector2 AbsoluteSize { get; }

        void Invalidate();
    }

    public class UserInterfaceControl : IUserInterfaceControl
    {
    }
}
