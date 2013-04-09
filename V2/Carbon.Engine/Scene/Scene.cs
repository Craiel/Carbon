using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Logic;

namespace Carbon.Engine.Scene
{
    public abstract class Scene : EngineComponent, IScene
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }

        public abstract void Render();

        public abstract void Resize(int width, int height);
    }
}
