namespace Carbon.Engine.Rendering
{
    public interface IFirstPersonCamera : ICamera
    {
    }

    public class FirstPersonCamera : Camera
    {
        private float movementSpeed;

        public override void Update(Contracts.Logic.ITimer gameTime)
        {
            // Todo: Do fps camera stuff here

            base.Update(gameTime);
        }
    }
}
