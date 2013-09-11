namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Rendering;
    
    public interface ISceneDebugOverlay : IScene
    {
        bool EnableController { get; set; }

        IProjectionCamera Camera { get; }
    }
}
