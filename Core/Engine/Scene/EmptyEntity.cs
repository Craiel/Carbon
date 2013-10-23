namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Scene;

    public class EmptyEntity : SceneEntity, IEmptyEntity
    {
        protected override ISceneEntity DoClone()
        {
            return new EmptyEntity();
        }
    }
}
