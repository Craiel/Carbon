using Core.Engine.Resource.Resources;
using Core.Engine.Resource.Resources.Model;
using Core.Engine.Resource.Resources.Stage;
using Core.Processing.Processors;
using Core.Processing.Resource.Collada;
using Core.Processing.Resource.Xcd;
using Core.Utils.IO;

namespace Core.Processing.Contracts
{
    public interface IResourceProcessor
    {
        CarbonDirectory TextureToolsPath { get; set; }

        RawResource ProcessRaw(CarbonPath path);
        RawResource ProcessTexture(CarbonPath path, TextureProcessingOptions options);
        RawResource ProcessFont(CarbonPath path, FontProcessingOptions options);
        StageResource ProcessStage(CarbonPath path, XcdProcessingOptions options);
        ModelResourceGroup ProcessModel(ColladaInfo info, string element, string texturePath);
        ScriptResource ProcessScript(CarbonPath path, ScriptProcessingOptions options);
        UserInterfaceResource ProcessUserInterface(CarbonPath path, UserInterfaceProcessingOptions options);
    }
}
