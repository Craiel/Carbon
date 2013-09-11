namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Scene;

    public interface ISceneDebugOverlay : IScene
    {
        bool EnableController { get; set; }
        bool EnableRendering { get; set; }

        IProjectionCamera Camera { get; }

        void UpdateEntityData(IEnumerable<SceneEntityDebugEntry> entities);
    }
}
