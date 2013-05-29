using Core.Engine.Contracts.UserInterface;
using Core.Engine.Logic;

using SlimDX;

namespace Core.Engine.UserInterface
{
    public class UserInterfaceControl : EngineComponent, IUserInterfaceControl
    {
        public virtual bool IsActive { get; set; }

        public virtual bool IsVisible { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector2 AbsolutePosition { get; private set; }

        public Vector2 AbsoluteSize { get; private set; }

        public virtual void Invalidate()
        {
            throw new System.NotImplementedException();
        }
    }
}
