using System;
using System.IO;

using Carbon.Engine.Contracts.Resource;

using SlimDX.D3DCompiler;

namespace Carbon.Engine.Resource
{
    internal class CompiledShader : ICarbonResource
    {
        internal const int CurrentVersion = 1;

        public CompiledShader(byte[] md5, ShaderBytecode shaderBytecode)
        {
            this.SourceMd5 = md5;

            this.ShaderData = new byte[shaderBytecode.Data.Length];
            shaderBytecode.Data.Position = 0;
            shaderBytecode.Data.Read(this.ShaderData, 0, this.ShaderData.Length);

            this.Version = CurrentVersion;
        }

        public CompiledShader(Stream source)
        {
            using (var reader = new BinaryReader(source))
            {
                this.Version = reader.ReadInt32();
                byte md5Length = reader.ReadByte();
                this.SourceMd5 = reader.ReadBytes(md5Length);
                this.ShaderData = reader.ReadBytes((int)source.Length - (int)source.Position);
            }
        }

        public byte[] SourceMd5 { get; private set; }
        public byte[] ShaderData { get; private set; }
        public int Version { get; private set; }

        public long Save(Stream target)
        {
            long size;
            using (var dataStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(dataStream))
                {
                    writer.Write(this.Version);
                    writer.Write((byte)this.SourceMd5.Length);
                    writer.Write(this.SourceMd5);
                    writer.Write(this.ShaderData);

                    size = dataStream.Position;
                    dataStream.Position = 0;
                    dataStream.WriteTo(target);
                }
            }

            return size;
        }
    }
}
