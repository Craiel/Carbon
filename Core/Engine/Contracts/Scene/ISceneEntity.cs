namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;

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
    }
}
