using System.IO;

using SlimDX;

namespace Carbon.Engine.Resource
{
    public class MaterialResource
    {
        private const int Version = 1;
        
        public MaterialResource()
        {
        }

        public MaterialResource(Stream source)
            : this()
        {
            using (var reader = new BinaryReader(source))
            {
                if (reader.ReadInt32() != Version)
                {
                    throw new InvalidDataException("Material version is not known");
                }

                this.Name = reader.ReadString();

                bool hasNormal = reader.ReadBoolean();
                bool hasSpecular = reader.ReadBoolean();
                bool hasAlpha = reader.ReadBoolean();

                Vector4 color = new Vector4
                                    {
                                        X = reader.ReadSingle(),
                                        Y = reader.ReadSingle(),
                                        Z = reader.ReadSingle(),
                                        W = reader.ReadSingle()
                                    };
                this.Color = color;

                this.DiffuseTexture = reader.ReadString();
                if (hasNormal)
                {
                    this.NormalTexture = reader.ReadString();
                }

                if (hasSpecular)
                {
                    this.SpecularTexture = reader.ReadString();
                }

                if (hasAlpha)
                {
                    this.AlphaTexture = reader.ReadString();
                }
            }
        }

        public Vector4 Color { get; set; }

        public string Name { get; set; }

        public string DiffuseTexture { get; set; }

        public string NormalTexture { get; set; }

        public string SpecularTexture { get; set; }

        public string AlphaTexture { get; set; }

        public long Save(Stream target)
        {
            long size;
            using (var dataStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(dataStream))
                {
                    writer.Write(Version);
                    writer.Write(this.Name);

                    writer.Write(!string.IsNullOrEmpty(this.NormalTexture));
                    writer.Write(!string.IsNullOrEmpty(this.SpecularTexture));
                    writer.Write(!string.IsNullOrEmpty(this.AlphaTexture));

                    writer.Write(this.Color.X);
                    writer.Write(this.Color.Y);
                    writer.Write(this.Color.Z);
                    writer.Write(this.Color.W);

                    writer.Write(this.DiffuseTexture);
                    if (!string.IsNullOrEmpty(this.NormalTexture))
                    {
                        writer.Write(this.NormalTexture);
                    }

                    if (!string.IsNullOrEmpty(this.SpecularTexture))
                    {
                        writer.Write(this.SpecularTexture);
                    }

                    if (!string.IsNullOrEmpty(this.AlphaTexture))
                    {
                        writer.Write(this.AlphaTexture);
                    }

                    size = dataStream.Position;
                    dataStream.Position = 0;
                    dataStream.WriteTo(target);
                }
            }

            return size;
        }
    }
}
