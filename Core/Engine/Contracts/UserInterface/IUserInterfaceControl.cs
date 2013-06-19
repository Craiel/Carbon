using Core.Engine.Contracts.Logic;

using SlimDX;

namespace Core.Engine.Contracts.UserInterface
{
    public interface IUserInterfaceControl : IEngineComponent
    {
        string Name { get; set; }

        bool IsActive { get; set; }
        bool IsVisible { get; set; }

        Vector2 Position { get; set; }
        Vector2 Scale { get; set; }
        Quaternion Rotation { get; set; }

        Vector2 AbsolutePosition { get; }
        Vector2 AbsoluteSize { get; }

        void Invalidate();
    }

}
