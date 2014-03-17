namespace Core.Engine.Rendering.Camera
{
    using System.Diagnostics.CodeAnalysis;

    using CarbonCore.Utils.Contracts;

    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;

    using SharpDX;

    public class ProjectionCamera : BaseCamera, IProjectionCamera
    {
        // This camera object is used to perform runtime operations like shadow map calculations
        private static readonly IProjectionCamera StaticCamera = new ProjectionCamera();

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private readonly Vector3 upVector = Vector3.Up;

        private Vector3 targetVector = Vector3.ForwardLH;
        
        private Quaternion rotation;

        private Vector3 position;

        private bool needUpdate = true;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // Static camera used for shadowmapping
        public static IProjectionCamera Camera
        {
            get
            {
                return StaticCamera;
            }
        }
        
        public override Vector3 Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (this.position != value)
                {
                    this.position = value;
                    this.needUpdate = true;
                }
            }
        }

        public Vector3 Forward
        {
            get
            {
                return this.targetVector;
            }
        }

        public Vector3 Up
        {
            get
            {
                return this.upVector;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return this.rotation;
            }

            set
            {
                if (this.rotation != value)
                {
                    this.rotation = value;
                    this.needUpdate = true;
                }
            }
        }

        public void LookAt(Vector3 target)
        {
            this.targetVector = target;

            // Todo: do we need this: this.rotation = QuaternionExtension.RotateTo(this.targetVector, target, this.upVector);
            this.needUpdate = true;
        }

        public override bool Update(ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.needUpdate)
            {
                // Get the rotation
                Matrix rotationMatrix = Matrix.RotationQuaternion(this.rotation);

                // Calculate the rotated target and up vectors
                var positionVector = new Vector3(this.position.X, this.position.Y, this.position.Z);
                Vector4 rotatedTarget = Vector3.Transform(this.targetVector, rotationMatrix * Matrix.Translation(positionVector));
                Vector4 rotatedUp = Vector3.Transform(this.upVector, rotationMatrix);

                this.View = Matrix.LookAtLH(
                    positionVector,
                    new Vector3(rotatedTarget.X, rotatedTarget.Y, rotatedTarget.Z),
                    new Vector3(rotatedUp.X, rotatedUp.Y, rotatedUp.Z));

                this.Frustum = new BoundingFrustum(this.View * this.Projection);
                
                this.needUpdate = false;
            }

            return true;
        }

        public override void SetPerspective(TypedVector2<int> newViewPort, float newNear, float newFar, float fov = CameraConstants.DefaultFoV)
        {
            this.ViewPort = newViewPort;
            this.Near = newNear;
            this.Far = newFar;
            this.FieldOfView = fov;
            this.Projection = Matrix.PerspectiveFovLH(fov, (float)this.ViewPort.X / this.ViewPort.Y, this.Near, this.Far);
            this.needUpdate = true;
        }

        public override void CopyFrom(ICamera source)
        {
            base.CopyFrom(source);

            var typed = source as IProjectionCamera;
            if (typed != null)
            {
                this.rotation = typed.Rotation;
            }

            this.needUpdate = true;
        }
    }
}
