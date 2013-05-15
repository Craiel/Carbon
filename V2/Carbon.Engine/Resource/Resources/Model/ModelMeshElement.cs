namespace Carbon.Engine.Resource.Resources.Model
{
    using System.IO;

    using Carbon.Engine.Logic;

    using SlimDX;

    public class ModelMeshElement : ResourceBase
    {
        internal enum MeshFlags
        {
            None = 0,
            HasNormal = 1,
            HasTexture = 2,
            HasTangent = 4,
            HasColor = 8,
        }

        internal const int Version = 1;

        public Vector3 Position { get; set; }
        public Vector3? Normal { get; set; }
        public Vector2? Texture { get; set; }
        public Vector4? Tangent { get; set; }
        public Vector4? Color { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            int version = source.ReadInt();
            if (version != Version)
            {
                throw new InvalidDataException("Model Resource version is not correct: " + version);
            }

            uint flags = source.ReadUInt();
            bool hasNormal = (flags & (int)MeshFlags.HasNormal) == (int)MeshFlags.HasNormal;
            bool hasTexture = (flags & (int)MeshFlags.HasTexture) == (int)MeshFlags.HasTexture;
            bool hasTangent = (flags & (int)MeshFlags.HasTangent) == (int)MeshFlags.HasTangent;
            bool hasColor = (flags & (int)MeshFlags.HasColor) == (int)MeshFlags.HasColor;

            this.Position = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };

            if (hasNormal)
            {
                this.Normal = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };
            }

            if (hasTexture)
            {
                this.Texture = new Vector2 { X = source.ReadSingle(), Y = source.ReadSingle() };
            }

            if (hasTangent)
            {
                this.Tangent = new Vector4
                {
                    X = source.ReadSingle(),
                    Y = source.ReadSingle(),
                    Z = source.ReadSingle(),
                    W = source.ReadSingle()
                };
            }

            if (hasColor)
            {
                this.Color = new Vector4
                {
                    X = source.ReadSingle(),
                    Y = source.ReadSingle(),
                    Z = source.ReadSingle(),
                    W = source.ReadSingle()
                };
            }
        }

        private uint GetMeshFlags()
        {
            uint flags = 0;
            if (this.Normal != null)
            {
                flags |= (uint)MeshFlags.HasNormal;
            }

            if (this.Texture != null)
            {
                flags |= (uint)MeshFlags.HasTexture;
            }

            if (this.Tangent != null)
            {
                flags |= (uint)MeshFlags.HasTangent;
            }

            if (this.Color != null)
            {
                flags |= (uint)MeshFlags.HasColor;
            }

            return flags;
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(Version);
            target.Write(this.GetMeshFlags());

            target.Write(Position.X);
            target.Write(Position.Y);
            target.Write(Position.Z);

            if (Normal != null)
            {
                target.Write(Normal.Value.X);
                target.Write(Normal.Value.Y);
                target.Write(Normal.Value.Z);
            }

            if (Texture != null)
            {
                target.Write(Texture.Value.X);
                target.Write(Texture.Value.Y);
            }

            if (Tangent != null)
            {
                target.Write(Tangent.Value.X);
                target.Write(Tangent.Value.Y);
                target.Write(Tangent.Value.Z);
                target.Write(Tangent.Value.W);
            }

            if (Color != null)
            {
                target.Write(Color.Value.X);
                target.Write(Color.Value.Y);
                target.Write(Color.Value.Z);
                target.Write(Color.Value.W);
            }
        }
    }
}
