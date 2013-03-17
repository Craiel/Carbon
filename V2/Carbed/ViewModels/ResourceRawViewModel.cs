using Carbed.Contracts;

using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
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
