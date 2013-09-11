namespace Core.Engine.Rendering.Camera
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;

    using Core.Utils.Contracts;
    using SharpDX;

    public class OrthographicCamera : BaseCamera, IOrthographicCamera
    {
        private readonly Vector3 position = -Vector3.UnitZ;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private readonly Vector3 upVector = Vector3.UnitY;

        private readonly Vector3 targetVector = Vector3.UnitZ;

        private bool needUpdate = true;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override Vector3 Position
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
        
        public override bool Update(ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.needUpdate)
            {
                this.View = Matrix.LookAtLH(
                    new Vector3(this.position.X, this.position.Y, this.position.Z), 
                    this.targetVector,
                    this.upVector);

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
            this.Projection = Matrix.OrthoOffCenterLH(0, this.ViewPort.X, 0, this.ViewPort.Y, this.Near, this.Far);
            this.needUpdate = true;
        }

        public override void CopyFrom(ICamera source)
        {
            base.CopyFrom(source);

            this.needUpdate = true;
        }
    }
}
