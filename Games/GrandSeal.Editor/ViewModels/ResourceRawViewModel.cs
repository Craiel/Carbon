using GrandSeal.Editor.Contracts;

using Core.Engine.Contracts;
using Core.Engine.Resource.Content;

namespace GrandSeal.Editor.ViewModels
{
    public class ResourceRawViewModel : ResourceViewModel, IResourceRawViewModel
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceRawViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
        }
    }
}
