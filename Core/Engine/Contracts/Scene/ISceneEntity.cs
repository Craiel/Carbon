namespace Core.Engine.Contracts.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Scene;

    using SharpDX;

    public interface ISceneEntity : IEngineComponent, IRenderable
    {
        string Name { get; set; }

        Vector3 Position { get; set; }
        Vector3 Scale { get; set; }
        Quaternion Rotation { get; set; }

        Matrix Local { get; }
        Matrix World { get; set; }
        
        BoundingSphere? BoundingSphere { get; }
        BoundingBox? BoundingBox { get; }

        IReadOnlyCollection<ISceneEntity> Parents { get; }
        IReadOnlyCollection<ISceneEntity> Children { get; }

        void AddChild(ISceneEntity parent);
        void RemoveChild(ISceneEntity parent);

        void AddParent(ISceneEntity parent);
        void RemoveParent(ISceneEntity parent);

        ISceneEntity Clone();

        void Link(Scene scene, int targetStack);
        void Unlink();
    }
}
