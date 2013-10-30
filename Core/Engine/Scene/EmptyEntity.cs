namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Scene;

    public class EmptyEntity : SceneEntity, IEmptyEntity
    {
        public override bool CanRender
        {
            get
            {
                return false;
            }
        }

        protected override ISceneEntity DoClone()
        {
            return new EmptyEntity();
        }
    }
}
