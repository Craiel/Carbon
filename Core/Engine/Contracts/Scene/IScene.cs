﻿namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

    public interface IScene : IEngineComponent, IRenderableComponent, IScriptingProvider
    {
        bool IsActive { get; set; }
        bool IsVisible { get; set; }

        string SceneScriptHash { get; set; }

        void CheckState();

        void LinkEntity(ISceneEntity entity, int targetStack);
        void InvalidateSceneEntity(ISceneEntity entity, int targetStack);

        void AddToRenderingList(ISceneEntity entity, int targetList);
    }
}
