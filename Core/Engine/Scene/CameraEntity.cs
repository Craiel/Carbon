namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;

    public class CameraEntity : SceneEntity, ICameraEntity
    {
        public IProjectionCamera Camera { get; set; }
        
        public override bool Update(Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.Camera != null)
            {
                this.Camera.Position = this.Position;
                this.Camera.Rotation = this.Rotation;
                this.Camera.Update(gameTime);
            }

            return true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override ISceneEntity DoClone()
        {
            return new CameraEntity { Camera = this.Camera };
        }
    }
}
