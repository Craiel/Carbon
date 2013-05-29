using System.Windows.Input;
using System.Windows.Media;

namespace GrandSeal.Editor.Contracts
{
    using Core.Engine.Contracts.Resource;

    public interface IFontViewModel : IEditorDocument
    {
        int? Id { get; }

        IResourceFontViewModel Resource { get; }

        ICommand CommandSelectResource { get; }

        void Save(IContentManager target);
        void Delete(IContentManager target);
    }
}
