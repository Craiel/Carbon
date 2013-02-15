using Carbed.Contracts;

using Carbon.Engine.Contracts;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public class TextureViewModel : ResourceViewModel, ITextureViewModel
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextureViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
        }
    }
}
