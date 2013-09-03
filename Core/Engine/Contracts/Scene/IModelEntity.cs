namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Rendering;

    public interface IModelEntity : ISceneEntity
    {
        Mesh Mesh { get; set; }
        Material Material { get; set; }
    }
}
