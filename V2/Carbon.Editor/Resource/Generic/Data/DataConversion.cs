using System;
using System.Diagnostics;
using System.Globalization;

using SlimDX;

namespace Carbon.Editor.Resource.Generic.Data
{
    public static class DataConversion
    {
        public static float[] ConvertFloat(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
            {
                throw new InvalidOperationException("Can not convert Float Array, Null RawData given");
            }

            string[] rawValues = rawData.Trim().Split(' ');
            float[] values = new float[rawValues.Length];
            for (int i = 0; i < rawValues.Length; i++)
            {
                values[i] = float.Parse(rawValues[i]);
            }

            return values;
        }

        public static int[] ConvertInt(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
            {
                throw new InvalidOperationException("Can not convert Int Array, Null RawData given");
            }

            string[] rawValues = rawData.Trim().Split(' ');
            int[] values = new int[rawValues.Length];
            for (int i = 0; i < rawValues.Length; i++)
            {
                values[i] = int.Parse(rawValues[i]);
            }

            return values;
        }

        public static bool[] ConvertBool(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
            {
                throw new InvalidOperationException("Can not convert Bool Array, Null RawData given");
            }

            string[] rawValues = rawData.Trim().Split(' ');
            var values = new bool[rawValues.Length];
            string comparison = 1.ToString(CultureInfo.InvariantCulture);
            for (int i = 0; i < rawValues.Length; i++)
            {
                values[i] = rawValues[i].Equals(comparison);
            }

            return values;
        }

        public static Vector4[] ToVector4(float[] data)
        {
            Debug.Assert(data.Length % 4 == 0, "ToVector4() must be called on FloatArray not multiple of four");

            int vertexCount = data.Length / 4;
            var vertices = new Vector4[vertexCount];
            int index = 0;
            for (int i = 0; i < data.Length; i += 4)
            {
                vertices[index] = new Vector4(data[i], data[i + 1], data[i + 2], data[i + 3]);
                index++;
            }

            return vertices;
        }

        public static Vector3[] ToVector3(float[] data)
        {
            Debug.Assert(data.Length % 3 == 0, "ToVector3() must be called on FloatArray not multiple of three");

            int vertexCount = data.Length / 3;
            var vertices = new Vector3[vertexCount];
            int index = 0;
            for (int i = 0; i < data.Length; i += 3)
            {
                vertices[index] = new Vector3(data[i], data[i + 1], data[i + 2]);
                index++;
            }

            return vertices;
        }

        public static Vector2[] ToVector2(float[] data)
        {
            Debug.Assert(data.Length % 2 == 0, "ToVector2() must be called on FloatArray not multiple of two");

            int vertexCount = data.Length / 2;
            var vertices = new Vector2[vertexCount];
            int index = 0;
            for (int i = 0; i < data.Length; i += 2)
            {
                vertices[index] = new Vector2(data[i], data[i + 1]);
                index++;
            }

            return vertices;
        }
    }
}
