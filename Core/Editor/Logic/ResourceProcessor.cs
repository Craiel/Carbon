namespace Core.Processing.Logic
{
    using System;

    using Core.Engine.Resource.Resources;
    using Core.Engine.Resource.Resources.Model;
    using Core.Engine.Resource.Resources.Stage;
    using Core.Processing.Contracts;
    using Core.Processing.Processors;
    using Core.Processing.Resource.Collada;
    using Core.Processing.Resource.Xcd;
    using Core.Utils.IO;

    public delegate string ReferenceResolveDelegate(string reference);

    public class ResourceProcessor : IResourceProcessor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory TextureToolsPath
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

        public RawResource ProcessRaw(CarbonDirectory path)
        {
            throw new NotImplementedException();
        }

        public RawResource ProcessRaw(CarbonFile file)
        {
            if (file.IsNull || !file.Exists)
            {
                throw new ArgumentException("Given path is invalid");
            }

            using (var stream = file.OpenRead())
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return new RawResource { Data = data };
            }
        }

        public RawResource ProcessTexture(CarbonFile file, TextureProcessingOptions options)
        {
            return TextureProcessor.Process(file, options);
        }

        public RawResource ProcessFont(CarbonFile file, FontProcessingOptions options)
        {
            return FontProcessor.Process(file, options);
        }

        public ModelResourceGroup ProcessModel(ColladaInfo info, string element, CarbonDirectory texturePath)
        {
            if (info == null)
            {
                throw new ArgumentException();
            }

            return ColladaProcessor.Process(info, element, texturePath);
        }

        public StageResource ProcessStage(CarbonFile file, XcdProcessingOptions options)
        {
            return XcdProcessor.Process(file, options);
        }

        public ScriptResource ProcessScript(CarbonFile file, ScriptProcessingOptions options)
        {
            return ScriptProcessor.Process(file, options);
        }

        public UserInterfaceResource ProcessUserInterface(CarbonFile file, UserInterfaceProcessingOptions options)
        {
            return UserInterfaceProcessor.Process(file, options);
        }
    }
}
