using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;
using SlimDX;

namespace Carbon.Engine.Rendering.Camera
{  
    public abstract class BaseCamera : EngineComponent, ICamera
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract Vector4 Position { get; set; }
        public abstract Matrix View { get; }
        public abstract Matrix Projection { get; }
        public abstract BoundingFrustum Frustum { get; }

        public abstract float Near { get; }
        public abstract float Far { get; }

        public abstract void SetPerspective(float width, float height, float near, float far);
    }
}
