using Carbon.Engine.Contracts.Logic;

namespace Carbon.Engine.Contracts.Scene
{
    public interface IScene : IEngineComponent
    {
        bool IsActive { get; set; }
        bool IsVisible { get; set; }

        void Render();
        void Resize(int width, int height);
    }
}
