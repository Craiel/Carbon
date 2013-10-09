namespace Core.Engine.Contracts.Rendering
{
    using System;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Logic;
    using SharpDX;

    public interface ICamera : IEngineComponent
    {
        Vector3 Position { get; set; }

        Matrix View { get; }
        Matrix Projection { get; }
        BoundingFrustum Frustum { get; }

        TypedVector2<int> ViewPort { get; }

        float Near { get; }
        float Far { get; }
        float FieldOfView { get; }

        void SetPerspective(TypedVector2<int> size, float near, float far, float fov = CameraConstants.DefaultFoV);

        void CopyFrom(ICamera source);
    }

    public interface IOrthographicCamera : ICamera
    {
    }

    public interface IProjectionCamera : ICamera
    {
        Vector3 Forward { get; }
        Vector3 Up { get; }

        Quaternion Rotation { get; set; }

        void LookAt(Vector3 target);
    }

    public static class CameraConstants
    {
        public const float DefaultFoV = (float)Math.PI / 4;
    }
}
