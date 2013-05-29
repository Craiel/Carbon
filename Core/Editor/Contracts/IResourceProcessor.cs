using Core.Engine.Resource.Resources;

namespace Core.Editor.Contracts
{
    using Core.Engine.Resource.Resources.Model;
    using Core.Engine.Resource.Resources.Stage;

    using Core.Editor.Processors;
    using Core.Editor.Resource.Collada;
    using Core.Editor.Resource.Xcd;

    public interface IResourceProcessor
    {
        string TextureToolsPath { get; set; }

        RawResource ProcessRaw(string path);
        RawResource ProcessTexture(string path, TextureProcessingOptions options);
        RawResource ProcessFont(string path, FontProcessingOptions options);
        StageResource ProcessStage(string path, XcdProcessingOptions options);
        ModelResourceGroup ProcessModel(ColladaInfo info, string element, string texturePath);
    }
}
