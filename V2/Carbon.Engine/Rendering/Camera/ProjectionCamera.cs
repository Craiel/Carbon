using System;

using Carbon.Engine.Logic;

using Core.Utils;
using Core.Utils.Contracts;

using SlimDX;
using Carbon.Engine.Contracts.Rendering;

namespace Carbon.Engine.Rendering.Camera
{
    class ProjectionCamera : BaseCamera, IProjectionCamera
    {
        // This camera object is used to perform runtime operations like shadow map calculations
        private static readonly IProjectionCamera StaticCamera = new ProjectionCamera();

        private Vector3 upVector = Vector3.UnitY;
        private Vector3 targetVector = Vector3.UnitZ;

        private Matrix view;
        private Matrix projection;
        private BoundingFrustum frustum;

        private TypedVector2<int> viewPort;
        private float near;
        private float far;

        private Quaternion rotation;

        private Vector4 position;

        private bool needUpdate = true;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IProjectionCamera Camera
        {
            get
            {
                return StaticCamera;
            }
        }

        public override Matrix View
        {
            get
            {
                return this.view;
            }
        }

        public override Matrix Projection
        {
            get
            {
                return this.projection;
            }
        }

        public override BoundingFrustum Frustum
        {
            get
            {
                return this.frustum;
            }
        }

        public override TypedVector2<int> ViewPort
        {
            get
            {
                return this.viewPort;
            }
        }

        public override float Near
        {
            get
            {
                return this.near;
            }
        }

        public override float Far
        {
            get
            {
                return this.far;
            }
        }

        public override Vector4 Position
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
            //this.rotation = QuaternionExtension.RotateTo(this.targetVector, target, this.upVector);
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
                Vector3 positionVector = new Vector3(this.position.X, this.position.Y, this.position.Z);
                Vector4 rotatedTarget = Vector3.Transform(this.targetVector, rotationMatrix * Matrix.Translation(positionVector));
                Vector4 rotatedUp = Vector3.Transform(this.upVector, rotationMatrix);

                this.view = Matrix.LookAtLH(
                    positionVector,
                    new Vector3(rotatedTarget.X, rotatedTarget.Y, rotatedTarget.Z),
                    new Vector3(rotatedUp.X, rotatedUp.Y, rotatedUp.Z));

                this.frustum = new BoundingFrustum(this.View, this.Projection, this.Far);
                
                this.needUpdate = false;
            }

            return true;
        }

        public override void SetPerspective(TypedVector2<int> newViewPort, float newNear, float newFar, float fov = CameraConstants.DefaultFoV)
        {
            this.viewPort = newViewPort;
            this.near = newNear;
            this.far = newFar;
            this.projection = Matrix.PerspectiveFovLH(fov, this.viewPort.X / this.viewPort.Y, this.near, this.far);
            this.needUpdate = true;
        }
    }
}
