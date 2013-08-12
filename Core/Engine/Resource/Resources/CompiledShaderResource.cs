namespace Core.Engine.Resource.Resources
{
    using System.IO;

    using Google.ProtocolBuffers;

    public class CompiledShaderResource : ProtocolResource
    {
        internal const int Version = 1;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public byte[] Data { get; set; }
        public byte[] Md5 { get; set; }

        public void Load(Protocol.Resource.CompiledShader shaderData)
        {
            if (shaderData.Version != Version)
            {
                throw new InvalidDataException("Shader version is not correct: " + shaderData.Version);
            }

            this.Data = shaderData.Data.ToByteArray();
            this.Md5 = shaderData.MD5.ToByteArray();
        }

        public override void Load(Stream source)
        {
            this.Load(Protocol.Resource.CompiledShader.ParseFrom(source));
        }

        public Protocol.Resource.CompiledShader Save()
        {
            if (this.Data == null || this.Md5 == null)
            {
                throw new InvalidDataException("Shader data was empty or invalid on Save");
            }

            var builder = new Protocol.Resource.CompiledShader.Builder
            {
                Version = Version,
                Data = ByteString.CopyFrom(this.Data),
                MD5 = ByteString.CopyFrom(this.Md5),
            };

            return builder.Build();
        }

        public override long Save(Stream target)
        {
            Protocol.Resource.CompiledShader entry = this.Save();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }
    }
}
