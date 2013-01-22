using Carbon.Engine.Contracts.Logic;
using Core.Utils.Contracts;
using SlimDX;
using SlimDX.DirectInput;

namespace Carbon.Engine.Logic
{
    public interface IFirstPersonController : IEngineComponent, IKeyStateReceiver
    {
        float Speed { get; set; }

        float RotationSpeed { get; set; }

        Vector4 Position { get; set; }

        Quaternion Rotation { get; }
    }
    
    public class FirstPersonController : EngineComponent, IFirstPersonController
    {
        private readonly IKeyStateManager keyStateManager;
        private readonly ICursor cursor;
        
        private Vector4 position;

        private float yaw;
        private float pitch;
        
        private bool isRotating;
        private Quaternion rotation;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FirstPersonController(ICursor cursor, IKeyStateManager keyStateManager)
        {
            this.keyStateManager = keyStateManager;
            this.keyStateManager.RegisterReceiver(this);

            this.cursor = cursor;
            this.cursor.ButtonChanged += this.OnButtonChanged;

            this.Speed = 0.1f;
            this.RotationSpeed = 0.01f;
            this.Forward = Vector3.UnitZ;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Order
        {
            get
            {
                return 100;
            }
        }

        public float Speed { get; set; }

        public float RotationSpeed { get; set; }

        public Vector3 Forward { get; set; }
        
        public Vector4 Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return this.rotation;
            }
        }
        
        public void ReceivePersists(Key key, ref bool isHandled)
        {
            Vector4 side;
            switch (key)
            {
                case Key.W:
                    side = Vector3.Transform(new Vector3(0, 0, this.Speed), this.rotation);
                    this.position += side;
                    break;
                case Key.A:
                    side = Vector3.Transform(new Vector3(-this.Speed, 0, 0), this.rotation);
                    this.position += side;
                    break;
                case Key.S:
                    side = Vector3.Transform(new Vector3(0, 0, -this.Speed), this.rotation);
                    this.position += side;
                    break;
                case Key.D:
                    side = Vector3.Transform(new Vector3(this.Speed, 0, 0), this.rotation);
                    this.position += side;
                    break;
            }
        }

        public void ReceivePressed(Key key, ref bool isHandled)
        {
        }

        public void ReceiveReleased(Key key, ref bool isHandled)
        {
        }
        
        public override void Update(ITimer gameTime)
        {
            if (this.isRotating)
            {
                this.yaw += this.cursor.LastDelta.X * this.RotationSpeed;
                this.pitch += this.cursor.LastDelta.Y * this.RotationSpeed;

                this.rotation = Quaternion.RotationYawPitchRoll(this.yaw, this.pitch, 0);
            }
        }

        public override void Dispose()
        {
            this.keyStateManager.UnregisterReceiver(this);

            base.Dispose();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnButtonChanged(int button, bool pressed)
        {
            switch (button)
            {
                case 1:
                    {
                        this.isRotating = pressed;
                        break;
                    }
            }
        }
    }
}
