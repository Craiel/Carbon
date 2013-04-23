using System;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;
using SlimDX;

namespace Carbon.Engine.Contracts.Rendering
{
    public static class CameraConstants
    {
        public const float DefaultFoV = (float)Math.PI / 4;
    }

    public interface ICamera : IEngineComponent
    {
        Vector4 Position { get; set; }

        Matrix View { get; }
        Matrix Projection { get; }
        BoundingFrustum Frustum { get; }

        float Near { get; }
        float Far { get; }

        void SetPerspective(TypedVector2<int> size, float near, float far, float fov = CameraConstants.DefaultFoV);
    }

    public interface IOrthographicCamera : ICamera
    {
    }

    public interface IProjectionCamera : ICamera
    {
        Quaternion Rotation { get; set; }

        void LookAt(Vector3 target);
    }
}
