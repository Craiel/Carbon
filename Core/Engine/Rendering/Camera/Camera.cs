namespace Core.Engine.Rendering.Camera
{
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Logic;

    using SlimDX;

    public abstract class BaseCamera : EngineComponent, ICamera
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract Vector4 Position { get; set; }
        public abstract Matrix View { get; }
        public abstract Matrix Projection { get; }
        public abstract BoundingFrustum Frustum { get; }

        public abstract TypedVector2<int> ViewPort { get; }
        public abstract float Near { get; }
        public abstract float Far { get; }

        public abstract void SetPerspective(TypedVector2<int> viewPort, float near, float far, float fov = CameraConstants.DefaultFoV);
    }
}
