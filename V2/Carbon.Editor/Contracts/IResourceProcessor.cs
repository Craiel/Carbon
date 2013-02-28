using Carbon.Editor.Resource.Collada;
using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Contracts
{
    public interface IResourceProcessor
    {
        RawResource ProcessRaw(string path);

        ModelResource ProcessModel(ColladaInfo info, string element);
    }
}
