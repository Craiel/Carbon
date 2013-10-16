namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Resource.Resources.Model;
    using Core.Engine.Scene;

    public interface ISceneDebugOverlay : IScene
    {
        bool EnableController { get; set; }
        bool EnableRendering { get; set; }

        ICameraEntity Camera { get; }

        void SetDebugCompass(ModelResourceGroup resource);
        void UpdateEntityData(IEnumerable<SceneEntityDebugEntry> entities);
    }
}
