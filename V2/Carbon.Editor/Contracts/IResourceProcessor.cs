using Carbon.Editor.Processors;
using Carbon.Editor.Resource.Collada;
using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Contracts
{
    public interface IResourceProcessor
    {
        string TextureToolsPath { get; set; }

        RawResource ProcessRaw(string path);
        RawResource ProcessTexture(string path, TextureProcessingOptions options);
        RawResource ProcessFont(string path, FontProcessingOptions options);
        ModelResource ProcessModel(ColladaInfo info, string element, string texturePath);
    }
}
