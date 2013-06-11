using Core.Editor.Processors;
using Core.Editor.Resource.Collada;
using Core.Editor.Resource.Xcd;
using Core.Engine.Resource.Resources;
using Core.Engine.Resource.Resources.Model;
using Core.Engine.Resource.Resources.Stage;

namespace Core.Editor.Contracts
{
    public interface IResourceProcessor
    {
        string TextureToolsPath { get; set; }

        RawResource ProcessRaw(string path);
        RawResource ProcessTexture(string path, TextureProcessingOptions options);
        RawResource ProcessFont(string path, FontProcessingOptions options);
        StageResource ProcessStage(string path, XcdProcessingOptions options);
        ModelResourceGroup ProcessModel(ColladaInfo info, string element, string texturePath);
        ScriptResource ProcessScript(string path, ScriptProcessingOptions options);
        UserInterfaceResource ProcessUserInterface(string path, UserInterfaceProcessingOptions options);
    }
}
