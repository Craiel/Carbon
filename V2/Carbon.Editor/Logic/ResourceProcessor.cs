using System;
using System.IO;

using Carbon.Editor.Contracts;
using Carbon.Editor.Processors;
using Carbon.Editor.Resource.Collada;
using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Logic
{
    using Carbon.Editor.Resource.Xcd;
    using Carbon.Engine.Resource.Resources.Model;
    using Carbon.Engine.Resource.Resources.Stage;

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
                var data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return new RawResource { Data = data };
            }
        }

        public RawResource ProcessTexture(string path, TextureProcessingOptions options)
        {
            return TextureProcessor.Process(path, options);
        }

        public RawResource ProcessFont(string path, FontProcessingOptions options)
        {
            return FontProcessor.Process(path, options);
        }

        public ModelResourceGroup ProcessModel(ColladaInfo info, string element, string texturePath)
        {
            if (info == null || string.IsNullOrEmpty(info.Source) || !File.Exists(info.Source))
            {
                throw new ArgumentException();
            }

            return ColladaProcessor.Process(info, element, texturePath);
        }

        public StageResource ProcessStage(string path, XcdProcessingOptions options)
        {
            return XcdProcessor.Process(path, options);
        }
    }
}
