namespace Core.Engine.Scene
{
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;

    public class CameraEntity : SceneEntity, ICameraEntity
    {
        public IProjectionCamera Camera { get; set; }

        public override void Dispose()
        {
            if (this.Camera != null)
            {
                this.Camera.Dispose();
            }

            base.Dispose();
        }

        public override bool Update(Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.Camera != null)
            {
                this.Camera.Update(gameTime);
            }

            return true;
        }
    }
}
