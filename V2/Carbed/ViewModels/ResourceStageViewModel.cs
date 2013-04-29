using Carbed.Contracts;

using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public class ResourceStageViewModel : ResourceViewModel, IResourceStageViewModel
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStageViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
        }
    }
}
