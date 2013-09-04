namespace Core.Engine.Logic
{
    using System;

    using Core.Engine.Contracts.Logic;
    using SharpDX;

    public interface IFirstPersonController : IBoundController
    {
        float Speed { get; set; }

        float RotationSpeed { get; set; }

        Vector3 Position { get; set; }

        Quaternion Rotation { get; }
    }
    
    public class FirstPersonController : BoundController, IFirstPersonController
    {
        private Vector3 position;

        private float yaw;
        private float pitch;
        
        private bool isRotating;
        private Quaternion rotation;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FirstPersonController(IInputManager inputManager)
            : base(inputManager)
        {
            this.Speed = 0.1f;
            this.RotationSpeed = 0.01f;
            this.Forward = Vector3.UnitZ;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        internal enum FirstPersonControllerAction
        {
            MoveForward,
            MoveBackward,
            MoveLeft,
            MoveRight,

            RotateLeft,
            RotateRight,
            RotateUp,
            RotateDown,

            ToggleRotation,
        }

        public float Speed { get; set; }

        public float RotationSpeed { get; set; }

        public Vector3 Forward { get; set; }
        
        public Vector3 Position
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

        public override void ReceiveAxisChange(string axis, float value)
        {
            if (!this.isRotating)
            {
                return;
            }

            switch (axis)
            {
                case "MouseX":
                    {
                        this.yaw += value * this.RotationSpeed;
                        break;
                    }

                case "MouseY":
                    {
                        this.pitch += value * this.RotationSpeed;
                        break;
                    }

                default:
                    {
                        // Unknown axis, nothing to do here
                        return;
                    }
            }

            this.rotation = Quaternion.RotationYawPitchRoll(this.yaw, this.pitch, 0);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnBindingsTriggered(System.Collections.Generic.IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            foreach (InputBindingEntry binding in triggeredBindings)
            {
                FirstPersonControllerAction action;
                if (Enum.TryParse(binding.Value, out action))
                {
                    switch (action)
                    {
                        case FirstPersonControllerAction.ToggleRotation:
                            {
                                this.isRotating = true;
                                break;
                            }
                    }
                }
            }
        }

        protected override void OnBindingsTriggeredPersist(System.Collections.Generic.IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            foreach (InputBindingEntry binding in triggeredBindings)
            {
                FirstPersonControllerAction action;
                if (Enum.TryParse(binding.Value, out action))
                {
                    Vector3 side;
                    switch (action)
                    {
                        case FirstPersonControllerAction.MoveForward:
                            side = Vector3.Transform(new Vector3(0, 0, this.Speed), this.rotation);
                            this.position += side;
                            break;
                        case FirstPersonControllerAction.MoveLeft:
                            side = Vector3.Transform(new Vector3(-this.Speed, 0, 0), this.rotation);
                            this.position += side;
                            break;
                        case FirstPersonControllerAction.MoveBackward:
                            side = Vector3.Transform(new Vector3(0, 0, -this.Speed), this.rotation);
                            this.position += side;
                            break;
                        case FirstPersonControllerAction.MoveRight:
                            side = Vector3.Transform(new Vector3(this.Speed, 0, 0), this.rotation);
                            this.position += side;
                            break;
                    }
                }
            }
        }

        protected override void OnBindingsTriggeredRelease(System.Collections.Generic.IReadOnlyCollection<InputBindingEntry> triggeredBindings)
        {
            foreach (InputBindingEntry binding in triggeredBindings)
            {
                FirstPersonControllerAction action;
                if (Enum.TryParse(binding.Value, out action))
                {
                    switch (action)
                    {
                        case FirstPersonControllerAction.ToggleRotation:
                            {
                                this.isRotating = false;
                                break;
                            }
                    }
                }
            }
        }
    }
}
