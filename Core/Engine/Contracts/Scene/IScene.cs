namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

    public interface IScene : IRenderableComponent, IScriptingProvider
    {
        bool IsActive { get; set; }
        bool IsVisible { get; set; }

        string SceneScriptHash { get; set; }

        void CheckState();
    }
}
