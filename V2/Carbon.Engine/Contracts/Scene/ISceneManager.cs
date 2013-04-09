using Carbon.Engine.Contracts.Logic;

namespace Carbon.Engine.Contracts.Scene
{
    public enum SceneStatus
    {
        Unknown,
        Inactive,
        Active,
        Suspended,
        Prepared
    }

    public interface ISceneManager : IEngineComponent
    {
        IScene ActiveScene { get; }
        IScene SuspendedScene { get; }

        bool IsPrepared(IScene scene);

        void Register(IScene scene);

        void Activate(IScene scene, bool suspendCurrent = false);
        void Deactivate();

        void Prepare(IScene scene);

        void Reload(IScene scene = null);
    }
}
