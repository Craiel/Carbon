using System.IO;

using Core.Engine.Resource.Resources;

using Google.ProtocolBuffers;

namespace Core.Engine.Resource
{
    internal class CompiledShader : ProtocolResource
    {
        internal const int Version = 1;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public byte[] SourceMd5 { get; set; }
        public byte[] ShaderData { get; set; }

        public override void Load(Stream source)
        {
            Protocol.Resource.CompiledShader entry = Protocol.Resource.CompiledShader.ParseFrom(source);
            if (entry.Version != Version)
            {
                throw new InvalidDataException("Compiled shader version is not correct: " + entry.Version);
            }

            if (!entry.HasData || !entry.HasMD5)
            {
                throw new InvalidDataException("Compiled shader resource was missing either md5 or data");
            }

            this.SourceMd5 = entry.MD5.ToByteArray();
            this.ShaderData = entry.Data.ToByteArray();
        }

        public override long Save(Stream target)
        {
            var builder = new Protocol.Resource.CompiledShader.Builder() { Version = Version };

            builder.SetData(ByteString.CopyFrom(this.ShaderData));
            builder.SetMD5(ByteString.CopyFrom(this.SourceMd5));

            Protocol.Resource.CompiledShader entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }
    }
}
