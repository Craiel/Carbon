using Carbon.Engine.Contracts.Logic;

namespace Carbon.Engine.Contracts.Scene
{
    using Carbon.Engine.Contracts.Rendering;

    public interface IScene : IEngineComponent, IRenderableComponent, IScriptingProvider
    {
        bool IsActive { get; set; }
        bool IsVisible { get; set; }
    }
}
