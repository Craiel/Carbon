using System;
using System.IO;

using Carbon.Editor.Contracts;
using Carbon.Editor.Processors;
using Carbon.Editor.Resource.Collada;
using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Logic
{
    public class ResourceProcessor : IResourceProcessor
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceProcessor(IEngineFactory factory)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string TextureToolsPath
        {
            get
            {
                return TextureProcessor.TextureToolsPath;
            }

            set
            {
                TextureProcessor.TextureToolsPath = value;
            }
        }

        public RawResource ProcessRaw(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new ArgumentException("Given path is invalid");
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var resource = new RawResource();
                resource.Load(stream);
                return resource;
            }
        }

        public RawResource ProcessTexture(string path, TextureProcessingOptions options)
        {
            return TextureProcessor.Process(path, options);
        }

        public ModelResource ProcessModel(ColladaInfo info, string element, string texturePath)
        {
            if (info == null || string.IsNullOrEmpty(info.Source) || !File.Exists(info.Source))
            {
                throw new ArgumentException();
            }

            return ColladaProcessor.Process(info, element, texturePath);
        }
    }
}
