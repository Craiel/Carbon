using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.UserInterface;

using Core.Utils.Contracts;

using SlimDX;

namespace Carbon.Engine.UserInterface
{
    public class UserInterfaceControl : IUserInterfaceControl
    {
        public virtual void Dispose()
        {
        }

        public virtual void Initialize(ICarbonGraphics graphics)
        {
        }

        public virtual void Update(ITimer gameTime)
        {
        }

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
