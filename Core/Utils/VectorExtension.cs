using SlimDX;

namespace Core.Utils
{
    public static class VectorExtension
    {
        static VectorExtension()
        {
            Vector3Identity = new Vector3(1.0f);
            Vector4Identity = new Vector4(1.0f);
        }

        public static Vector3 Vector3Identity { get; private set; }
        public static Vector4 Vector4Identity { get; private set; }

        public static Vector3 Invert(this Vector3 vector)
        {
            return new Vector3(-vector.X, -vector.Y, -vector.Z);
        }

        public static Vector3 XYZ(this Vector4 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }
    }
}
