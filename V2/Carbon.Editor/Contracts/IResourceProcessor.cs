using Carbon.Editor.Processors;
using Carbon.Editor.Resource.Collada;
using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Contracts
{
    using Carbon.Editor.Resource.Xcd;
    using Carbon.Engine.Resource.Resources.Model;
    using Carbon.Engine.Resource.Resources.Stage;

    public interface IResourceProcessor
    {
        string TextureToolsPath { get; set; }

        RawResource ProcessRaw(string path);
        RawResource ProcessTexture(string path, TextureProcessingOptions options);
        RawResource ProcessFont(string path, FontProcessingOptions options);
        StageResource ProcessStage(string path, XcdProcessingOptions options);
        ModelResource ProcessModel(ColladaInfo info, string element, string texturePath);
    }
}
