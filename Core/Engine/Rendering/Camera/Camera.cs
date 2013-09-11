namespace Core.Engine.Rendering.Camera
{
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;

    using SharpDX;

    public abstract class BaseCamera : EngineComponent, ICamera
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual Vector3 Position { get; set; }

        public Matrix View { get; protected set; }
        public Matrix Projection { get; protected set; }
        public BoundingFrustum Frustum { get; protected set; }

        public TypedVector2<int> ViewPort { get; protected set; }
        public float Near { get; protected set; }
        public float Far { get; protected set; }
        public float FieldOfView { get; protected set; }

        public abstract void SetPerspective(TypedVector2<int> viewPort, float near, float far, float fov = CameraConstants.DefaultFoV);

        public virtual void CopyFrom(ICamera source)
        {
            this.Position = source.Position;

            this.SetPerspective(source.ViewPort, source.Near, source.Far, source.FieldOfView);
        }
    }
}
