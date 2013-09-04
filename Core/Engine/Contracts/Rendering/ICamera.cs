﻿namespace Core.Engine.Contracts.Rendering
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

    public static class CameraConstants
    {
        public const float DefaultFoV = (float)Math.PI / 4;
    }
}
