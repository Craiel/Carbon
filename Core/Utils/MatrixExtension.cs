namespace Core.Utils
{
    using SharpDX;

    public static class MatrixExtension
    {
        public static Matrix GetLocalMatrix(Vector3 scale, Quaternion rotation, Vector3 position)
        {
            return Matrix.Scaling(scale) * Matrix.RotationQuaternion(rotation)
                         * Matrix.Translation(new Vector3(position.X, position.Y, position.Z));
        }
    }
}
