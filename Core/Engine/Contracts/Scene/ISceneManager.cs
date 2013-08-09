namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

    public enum SceneStatus
    {
        Unknown,
        Inactive,
        Active,
        Suspended,
        Prepared
    }

    public interface ISceneManager : IEngineComponent, IRenderableComponent
    {
        IScene ActiveScene { get; }
        IScene SuspendedScene { get; }

        bool IsPrepared(int key);

        void Register(int key, IScene scene);

        void Activate(int key, bool suspendCurrent = false);
        void Deactivate();

        void Prepare(int key);

        void Reload(int? key = null);
    }
}
