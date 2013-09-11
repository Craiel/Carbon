namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Rendering;

    public interface ILightEntity : ISceneEntity
    {
        ILight Light { get; }
    }
}
