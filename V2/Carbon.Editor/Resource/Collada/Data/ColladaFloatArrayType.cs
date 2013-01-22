using System;
using System.Diagnostics;

using SlimDX;

namespace Carbon.Editor.Resource.Collada.Data
{
    using System.Xml.Serialization;

    [Serializable]
    public class ColladaFloatArrayType
    {
        [XmlText]
        public string RawData
        {
            get
            {
                return string.Empty;
            }

            set
            {
                this.Data = ColladaDataConversion.ConvertFloat(value);
            }
        }

        [XmlIgnore]
        public float[] Data { get; private set; }

        public Vector4[] ToVector4()
        {
            Debug.Assert(this.Data.Length % 4 == 0, "ToVector4() must be called on FloatArray not multiple of four");

            int vertexCount = this.Data.Length / 4;
            Vector4[] vertices = new Vector4[vertexCount];
            int index = 0;
            for (int i = 0; i < this.Data.Length; i += 4)
            {
                vertices[index] = new Vector4(this.Data[i], this.Data[i + 1], this.Data[i + 2], this.Data[i + 3]);
                index++;
            }

            return vertices;
        }

        public Vector3[] ToVector3()
        {
            Debug.Assert(this.Data.Length % 3 == 0, "ToVector3() must be called on FloatArray not multiple of three");

            int vertexCount = this.Data.Length / 3;
            Vector3[] vertices = new Vector3[vertexCount];
            int index = 0;
            for (int i = 0; i < this.Data.Length; i += 3)
            {
                vertices[index] = new Vector3(this.Data[i], this.Data[i + 1], this.Data[i + 2]);
                index++;
            }

            return vertices;
        }

        public Vector2[] ToVector2()
        {
            Debug.Assert(this.Data.Length % 2 == 0, "ToVector2() must be called on FloatArray not multiple of two");

            int vertexCount = this.Data.Length / 2;
            Vector2[] vertices = new Vector2[vertexCount];
            int index = 0;
            for (int i = 0; i < this.Data.Length; i += 2)
            {
                vertices[index] = new Vector2(this.Data[i], this.Data[i + 1]);
                index++;
            }

            return vertices;
        }
    }
}
