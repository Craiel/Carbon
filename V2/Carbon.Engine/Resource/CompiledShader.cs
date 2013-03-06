using Carbon.Engine.Logic;
using Carbon.Engine.Resource.Resources;

using SlimDX.D3DCompiler;

namespace Carbon.Engine.Resource
{
    internal class CompiledShader : ResourceBase
    {
        internal const int CurrentVersion = 1;

        private byte[] sourceMd5;
        private byte[] shaderData;

        public CompiledShader()
        {
        }

        public CompiledShader(byte[] md5, ShaderBytecode shaderBytecode)
        {
            this.sourceMd5 = md5;
            this.shaderData = new byte[shaderBytecode.Data.Length];
            shaderBytecode.Data.Position = 0;
            shaderBytecode.Data.Read(this.ShaderData, 0, this.ShaderData.Length);

            this.Version = CurrentVersion;
        }

        public byte[] SourceMd5
        {
            get
            {
                return this.sourceMd5;
            }
        }

        public byte[] ShaderData
        {
            get
            {
                return this.shaderData;
            }
        }

        public int Version { get; private set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.Version = source.ReadInt();
            byte md5Length = source.ReadByte();
            source.Read(out this.sourceMd5, md5Length);
            source.Read(out this.shaderData);
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Version);
            target.Write((byte)this.SourceMd5.Length);
            target.Write(this.SourceMd5);
            target.Write(this.ShaderData);
        }
    }
}
