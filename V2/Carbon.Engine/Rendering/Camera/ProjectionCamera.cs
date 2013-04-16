using System;

using Core.Utils;
using Core.Utils.Contracts;

using SlimDX;
using Carbon.Engine.Contracts.Rendering;

namespace Carbon.Engine.Rendering.Camera
{
    class ProjectionCamera : BaseCamera, IProjectionCamera
    {
        private Vector3 upVector = Vector3.UnitY;
        private Vector3 targetVector = Vector3.UnitZ;

        private Matrix view;
        private Matrix projection;
        private BoundingFrustum frustum;

        private float width;
        private float height;
        private float near;
        private float far;

        private Quaternion rotation;

        private Vector4 position;

        private bool needUpdate = true;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
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

        public override float Width
        {
            get
            {
                return this.width;
            }
        }

        public override float Height 
        {
            get
            {
                return this.height;
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

        public override void Update(ITimer gameTime)
        {
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
        }

        public override void SetPerspective(float newWidth, float newHeight, float newNear, float newFar)
        {
            this.width = newWidth;
            this.height = newHeight;
            this.near = newNear;
            this.far = newFar;
            this.projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, this.width / this.height, this.near, this.far);
            this.needUpdate = true;
        }
    }
}
