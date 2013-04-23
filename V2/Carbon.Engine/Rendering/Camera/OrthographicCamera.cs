using System;

using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;

using Core.Utils.Contracts;
using SlimDX;

namespace Carbon.Engine.Rendering.Camera
{
    public class OrthographicCamera : BaseCamera, IOrthographicCamera
    {
        private readonly Vector4 position = new Vector4(-Vector3.UnitZ, 1.0f);
        private readonly Vector3 upVector = Vector3.UnitY;
        private readonly Vector3 targetVector = Vector3.UnitZ;

        private Matrix view;
        private Matrix projection;
        private BoundingFrustum frustum;

        private TypedVector2<int> viewPort;
        private float near;
        private float far;

        private bool needUpdate = true;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override Vector4 Position
        {
            get
            {
                return this.position;
            }

            set
            {
                throw new InvalidOperationException("Setting position is not allowed for Orthographic Camera");
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

        public override void Update(ITimer gameTime)
        {
            if (this.needUpdate)
            {
                this.view = Matrix.LookAtLH(
                    new Vector3(this.position.X, this.position.Y, this.position.Z), 
                    this.targetVector,
                    this.upVector);

                this.frustum = new BoundingFrustum(this.View, this.Projection, this.Far);

                this.needUpdate = false;
            }
        }

        public override void SetPerspective(TypedVector2<int> newViewPort, float newNear, float newFar, float fov = CameraConstants.DefaultFoV)
        {
            this.viewPort = newViewPort;
            this.near = newNear;
            this.far = newFar;
            this.projection = Matrix.OrthoOffCenterLH(0, this.viewPort.X, 0, this.viewPort.Y, this.near, this.far);
            this.needUpdate = true;
        }
    }
}
