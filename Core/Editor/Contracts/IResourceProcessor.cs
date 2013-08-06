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

        RawResource ProcessRaw(CarbonDirectory path);

        RawResource ProcessRaw(CarbonFile file);
        RawResource ProcessTexture(CarbonFile file, TextureProcessingOptions options);
        RawResource ProcessFont(CarbonFile file, FontProcessingOptions options);
        StageResource ProcessStage(CarbonFile file, XcdProcessingOptions options);
        ModelResourceGroup ProcessModel(ColladaInfo info, string element, CarbonDirectory texturePath);
        ScriptResource ProcessScript(CarbonFile file, ScriptProcessingOptions options);
        UserInterfaceResource ProcessUserInterface(CarbonFile file, UserInterfaceProcessingOptions options);
    }
}
