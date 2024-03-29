﻿namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Scene;

    using SharpDX;

    public interface ISceneEntity : IEngineComponent, IRenderable
    {
        bool CanRender { get; }

        string Name { get; set; }

        Vector3 Position { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }

        Matrix Local { get; }
        Matrix? OverrideWorld { get; set; }
        
        BoundingSphere? BoundingSphere { get; }
        BoundingBox? BoundingBox { get; }

        IReadOnlyCollection<ISceneEntity> Parents { get; }
        IReadOnlyCollection<ISceneEntity> Children { get; }
        IReadOnlyCollection<Matrix> LocalTransforms { get; }

        void AddChild(ISceneEntity parent);
        void RemoveChild(ISceneEntity parent);
        void ClearChildren();

        void AddParent(ISceneEntity parent);
        void RemoveParent(ISceneEntity parent);
        void ClearParents();

        void AddTransform(Matrix matrix);
        void RemoveTransform(Matrix matrix);
        void ClearTransforms();

        ISceneEntity Clone();

        void Link(Scene scene, int targetStack);
        void Unlink();

        Matrix GetWorld();
    }
}
