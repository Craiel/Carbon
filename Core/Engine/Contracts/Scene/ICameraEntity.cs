namespace Core.Engine.Contracts.Scene
{
    using Core.Engine.Contracts.Rendering;

    public interface ICameraEntity : ISceneEntity
    {
        ICamera Camera { get; }
    }
}
