namespace Core.Engine.Logic
{
    using System;
    using System.IO;

    using SharpDX.D3DCompiler;

    internal class ShaderIncludeHandler : Include
    {
        public IDisposable Shadow { get; set; }

        public Stream Open(IncludeType type, string fileName, Stream parentStream)
        {
            string sourceFile = Path.Combine(ShaderManager.ShaderLocation, fileName);
            return File.OpenRead(sourceFile);
        }

        public void Close(Stream stream)
        {
            stream.Dispose();
        }

        public void Dispose()
        {
            // Nothing to do here, yet
        }
    }
}
