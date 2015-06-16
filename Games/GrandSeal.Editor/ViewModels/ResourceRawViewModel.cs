namespace GrandSeal.Editor.ViewModels
{
    using CarbonCore.Utils.Compat.Contracts.IoC;

    using Core.Engine.Resource.Content;

    using GrandSeal.Editor.Contracts;

    public class ResourceRawViewModel : ResourceViewModel, IResourceRawViewModel
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceRawViewModel(IFactory factory, ResourceEntry data)
            : base(factory)
        {
        }
    }
}
