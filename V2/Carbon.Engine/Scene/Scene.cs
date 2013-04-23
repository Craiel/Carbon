using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Logic;

namespace Carbon.Engine.Scene
{
    using Carbon.Engine.Contracts.Rendering;

    public abstract class Scene : EngineComponent, IScene
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }

        public abstract void Render(IFrameManager frameManager);

        public abstract void Resize(TypedVector2<int> size);
    }
}
