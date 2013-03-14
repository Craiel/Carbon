using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;

namespace Carbon.Engine.Scene
{
    public interface IScene : IEngineComponent
    {
        void Render();
        void Resize(int width, int height);
    }

    public abstract class Scene : EngineComponent, IScene
    {        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract void Render();

        public abstract void Resize(int width, int height);
    }
}
