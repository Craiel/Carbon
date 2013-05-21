using Core.Utils;

using SlimDX;

namespace Carbon.Engine.Resource.Resources.Model
{
    public class ModelResourceElement
    {
        internal const int Version = 1;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelResourceElement()
        {
        }

        public ModelResourceElement(Protocol.Resource.ModelElement data)
            : this()
        {
            System.Diagnostics.Debug.Assert(data.PositionCount == 3, "Position data has invalid count");
            
            this.Position = VectorExtension.Vector3FromList(data.PositionList);

            if (data.NormalCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.NormalCount == 4, "Normal data has invalid count");
                this.Normal = VectorExtension.Vector3FromList(data.NormalList);
            }

            if (data.TextureCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.TextureCount == 2, "Texture data has invalid count");
                this.Texture = VectorExtension.Vector2FromList(data.TextureList);
            }

            if (data.TangentCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.TangentCount == 4, "Tangent data has invalid count");
                this.Tangent = VectorExtension.Vector4FromList(data.TangentList);
            }

            if (data.ColorCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.ColorCount == 4, "Color data has invalid count");
                this.Color = VectorExtension.Vector4FromList(data.ColorList);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector3 Position { get; set; }
        public Vector3? Normal { get; set; }
        public Vector2? Texture { get; set; }
        public Vector4? Tangent { get; set; }
        public Vector4? Color { get; set; }

        public Protocol.Resource.ModelElement.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.ModelElement.Builder();

            builder.AddRangePosition(this.Position.ToList());

            if (this.Normal != null)
            {
                builder.AddRangeNormal(this.Normal.Value.ToList());
            }

            if (this.Texture != null)
            {
                builder.AddRangeTexture(this.Texture.Value.ToList());
            }

            if (this.Tangent != null)
            {
                builder.AddRangeTangent(this.Tangent.Value.ToList());
            }

            if (this.Color != null)
            {
                builder.AddRangeColor(this.Color.Value.ToList());
            }

            return builder;
        }
    }
}
