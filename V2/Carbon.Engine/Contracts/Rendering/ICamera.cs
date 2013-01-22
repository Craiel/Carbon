using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Rendering;
using SlimDX;

namespace Carbon.Engine.Contracts.Rendering
{
    public interface ICamera : IEngineComponent
    {
        Vector4 Position { get; set; }

        Matrix View { get; }
        Matrix Projection { get; }
        BoundingFrustum Frustum { get; }

        float Near { get; }
        float Far { get; }

        void SetPerspective(float width, float height, float near, float far);
    }

    public interface IOrthographicCamera : ICamera
    {
    }

    public interface IProjectionCamera : ICamera
    {
        Quaternion Rotation { get; set; }
    }
}
