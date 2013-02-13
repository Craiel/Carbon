using System.Linq;
using System.Security.Cryptography;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System.Collections;
using System.IO;
using System.Text;

namespace Carbon.Engine.Logic
{
    using Carbon.Engine.Resource.Content;

    internal class ShaderIncludeHandler : Include
    {
        public void Open(IncludeType type, string fileName, Stream parentStream, out Stream stream)
        {
            string sourceFile = Path.Combine(ShaderManager.ShaderLocation, fileName);
            stream = File.OpenRead(sourceFile);
        }

        public void Close(Stream stream)
        {
            stream.Dispose();
        }
    }

    public class ShaderManager
    {
        // Todo: Yeah right, hard coded path's...
        internal const string ShaderLocation = @"Data\shaders";
        internal const string ShaderCacheKeyPrefix = @"ShaderCache";

        private readonly IResourceManager resourceManager;
        private readonly Hashtable vertexShaderCache;
        private readonly Hashtable pixelShaderCache;
        private readonly Hashtable signatureCache;
        private readonly Device device;

        private readonly ShaderIncludeHandler includeHandler;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ShaderManager(IResourceManager resourceManager, Device device)
        {
            this.resourceManager = resourceManager;

            this.vertexShaderCache = new Hashtable();
            this.pixelShaderCache = new Hashtable();
            this.signatureCache = new Hashtable();
            this.device = device;

            this.includeHandler = new ShaderIncludeHandler();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            foreach (VertexShader entry in this.vertexShaderCache.Values)
            {
                entry.Dispose();
            }

            this.vertexShaderCache.Clear();

            foreach (PixelShader entry in this.pixelShaderCache.Values)
            {
                entry.Dispose();
            }

            this.pixelShaderCache.Clear();

            foreach (ShaderSignature entry in this.signatureCache.Values)
            {
                entry.Dispose();
            }

            this.signatureCache.Clear();
        }

        public VertexShader GetVertexShader(CarbonShaderDescription description)
        {
            int key = description.GetHashCode();
            if (this.vertexShaderCache.ContainsKey(key))
            {
                return this.vertexShaderCache[key] as VertexShader;
            }

            CompiledShader data = this.GetData(description);
            using (var stream = new DataStream(data.ShaderData, true, false))
            {
                using (var byteCode = new ShaderBytecode(stream))
                {
                    var shader = new VertexShader(this.device, byteCode);
                    this.vertexShaderCache.Add(key, shader);
                    return shader;
                }
            }
        }

        public ShaderSignature GetShaderSignature(CarbonShaderDescription description)
        {
            if (this.signatureCache.ContainsKey(description))
            {
                return this.signatureCache[description] as ShaderSignature;
            }

            CompiledShader data = this.GetData(description);
            using (var stream = new DataStream(data.ShaderData, true, false))
            {
                using (var byteCode = new ShaderBytecode(stream))
                {
                    var signature = ShaderSignature.GetInputSignature(byteCode);
                    this.signatureCache.Add(description, signature);
                    return signature;
                }
            }
        }

        public PixelShader GetPixelShader(CarbonShaderDescription description)
        {
            if (this.pixelShaderCache.ContainsKey(description))
            {
                return this.pixelShaderCache[description] as PixelShader;
            }

            CompiledShader data = this.GetData(description);
            using (var stream = new DataStream(data.ShaderData, true, false))
            {
                using (var byteCode = new ShaderBytecode(stream))
                {
                    var shader = new PixelShader(this.device, byteCode);
                    this.pixelShaderCache.Add(description, shader);
                    return shader;
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private CompiledShader GetData(CarbonShaderDescription description)
        {
            string cachedKey = Path.Combine(ShaderCacheKeyPrefix, description.GetCacheFileName());
            string sourceFile = Path.Combine(ShaderLocation, description.File);

            byte[] md5;
            string sourceData = this.ReadSource(sourceFile, out md5);
            var key = new ResourceLink { Source = cachedKey };
            var shader = this.resourceManager.Load<CompiledShader>(ref key);
            if (shader != null)
            {
                if (md5.SequenceEqual(shader.SourceMd5))
                {
                    return shader;
                }

                System.Diagnostics.Trace.TraceInformation("Re-Compiling shader {0} -> {1}", sourceFile, cachedKey);
                ShaderBytecode shaderData = ShaderBytecode.Compile(sourceData, description.Entry, description.Profile, ShaderFlags.None, EffectFlags.None, description.Macros, this.includeHandler);
                shader = new CompiledShader(md5, shaderData);
                this.resourceManager.Replace(ref key, shader);
            }
            else
            {
                System.Diagnostics.Trace.TraceInformation("Compiling shader {0} -> {1}", sourceFile, cachedKey);
                ShaderBytecode shaderData = ShaderBytecode.Compile(sourceData, description.Entry, description.Profile, ShaderFlags.None, EffectFlags.None, description.Macros, this.includeHandler);
                shader = new CompiledShader(md5, shaderData);
                this.resourceManager.Store(ref key, shader);
            }
            
            return shader;
        }

        private string ReadSource(string file, out byte[] md5)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                using (MD5 hashProvider = MD5.Create())
                {
                    md5 = hashProvider.ComputeHash(stream);
                }

                stream.Position = 0;
                using (var reader = new StreamReader(stream, Encoding.ASCII))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
