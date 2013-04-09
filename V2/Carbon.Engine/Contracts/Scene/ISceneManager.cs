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

        bool IsPrepared(int key);

        void Register(int key, IScene scene);

        void Activate(int key, bool suspendCurrent = false);
        void Deactivate();

        void Prepare(int key);

        void Reload(int? key = null);
    }
}
