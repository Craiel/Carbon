namespace Core.Engine.Logic
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource.Resources;
    using Core.Utils;

    using SlimDX;
    using SlimDX.D3DCompiler;
    using SlimDX.Direct3D11;

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
        internal const string ShaderLocation = @"Data\Shaders";
        internal const string ShaderCacheKeyPrefix = @"_ShaderCache";

        private readonly IResourceManager resourceManager;
        private readonly IDictionary<int, VertexShader> vertexShaderCache;
        private readonly IDictionary<int, PixelShader> pixelShaderCache;
        private readonly IDictionary<int, ShaderSignature> signatureCache;
        private readonly Device device;

        private readonly ShaderIncludeHandler includeHandler;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ShaderManager(IResourceManager resourceManager, Device device)
        {
            this.resourceManager = resourceManager;

            this.vertexShaderCache = new Dictionary<int, VertexShader>();
            this.pixelShaderCache = new Dictionary<int, PixelShader>();
            this.signatureCache = new Dictionary<int, ShaderSignature>();
            this.device = device;

            this.includeHandler = new ShaderIncludeHandler();

            this.UsePrecompiledShaders = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool UsePrecompiledShaders { get; set; }

        public void Dispose()
        {
            this.ClearCache();
        }

        public VertexShader GetVertexShader(CarbonShaderDescription description)
        {
            int key = description.GetHashCode();
            if (this.vertexShaderCache.ContainsKey(key))
            {
                return this.vertexShaderCache[key];
            }

            CompiledShaderResource data = this.GetData(description);
            using (var stream = new DataStream(data.Data, true, false))
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
            int key = description.GetHashCode();
            if (this.signatureCache.ContainsKey(key))
            {
                return this.signatureCache[key];
            }

            CompiledShaderResource data = this.GetData(description);
            using (var stream = new DataStream(data.Data, true, false))
            {
                using (var byteCode = new ShaderBytecode(stream))
                {
                    var signature = ShaderSignature.GetInputSignature(byteCode);
                    this.signatureCache.Add(key, signature);
                    return signature;
                }
            }
        }

        public PixelShader GetPixelShader(CarbonShaderDescription description)
        {
            int key = description.GetHashCode();
            if (this.pixelShaderCache.ContainsKey(key))
            {
                return this.pixelShaderCache[key];
            }

            CompiledShaderResource data = this.GetData(description);
            using (var stream = new DataStream(data.Data, true, false))
            {
                using (var byteCode = new ShaderBytecode(stream))
                {
                    var shader = new PixelShader(this.device, byteCode);
                    this.pixelShaderCache.Add(key, shader);
                    return shader;
                }
            }
        }

        public void ClearCache()
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

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private CompiledShaderResource GetData(CarbonShaderDescription description)
        {
            string cachedKey = Path.Combine(ShaderCacheKeyPrefix, description.GetCacheFileName());
            string sourceFile = Path.Combine(ShaderLocation, description.File);

            byte[] md5;
            string sourceData = this.ReadSource(sourceFile, out md5);
            string hash = HashUtils.BuildResourceHash(cachedKey);
            var shader = this.resourceManager.Load<CompiledShaderResource>(hash);
            if (shader != null)
            {
                if (this.UsePrecompiledShaders && md5.SequenceEqual(shader.Md5))
                {
                    return shader;
                }

                System.Diagnostics.Trace.TraceInformation("Re-Compiling shader {0} -> {1}", sourceFile, cachedKey);
                using (ShaderBytecode shaderBytecode = ShaderBytecode.Compile(
                        sourceData,
                        description.Entry,
                        description.Profile,
                        ShaderFlags.None,
                        EffectFlags.None,
                        description.Macros,
                        this.includeHandler))
                {
                    var data = new byte[shaderBytecode.Data.Length];
                    shaderBytecode.Data.Position = 0;
                    shaderBytecode.Data.Read(data, 0, data.Length);

                    shader = new CompiledShaderResource { Md5 = md5, Data = data };
                    this.resourceManager.Replace(hash, shader);
                }
            }
            else
            {
                System.Diagnostics.Trace.TraceInformation("Compiling shader {0} -> {1}", sourceFile, cachedKey);
                using (ShaderBytecode shaderBytecode = ShaderBytecode.Compile(
                        sourceData,
                        description.Entry,
                        description.Profile,
                        ShaderFlags.None,
                        EffectFlags.None,
                        description.Macros,
                        this.includeHandler))
                {
                    var data = new byte[shaderBytecode.Data.Length];
                    shaderBytecode.Data.Position = 0;
                    shaderBytecode.Data.Read(data, 0, data.Length);

                    shader = new CompiledShaderResource { Md5 = md5, Data = data };
                    this.resourceManager.Store(hash, shader);
                }
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
