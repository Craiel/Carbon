using System;
using SlimDX;

namespace Core.Utils
{
    public static class QuaternionExtension
    {
        /*public static Quaternion RotateTo(Vector3 source, Vector3 target)
        {
            Vector3 axis = Vector3.Cross(source, target);
            axis.Normalize();
            var angle = (float)Math.Acos(Vector3.Dot(source, target) / source.Length() / target.Length());
            return Quaternion.RotationAxis(axis, angle);
        }*/

        public static Quaternion RotateTo(Vector3 source, Vector3 dest, Vector3 up)
        {
            float dot = Vector3.Dot(source, dest);

            if (Math.Abs(dot - (-1.0f)) < float.Epsilon)
            {
                return new Quaternion(up, MathExtension.DegreesToRadians(180.0f));
            }

            if (Math.Abs(dot - 1.0f) < float.Epsilon)
            {
                return Quaternion.Identity;
            }

            var angle = (float)Math.Acos(dot);
            Vector3 axis = Vector3.Cross(source, dest);
            axis = Vector3.Normalize(axis);
            return Quaternion.RotationAxis(axis, angle);
        }
    }
}
