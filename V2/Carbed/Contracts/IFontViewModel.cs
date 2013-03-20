using System.Windows.Input;
using System.Windows.Media;

namespace Carbed.Contracts
{
    using Carbon.Engine.Contracts.Resource;

    public interface IFontViewModel : ICarbedDocument
    {
        int? Id { get; }

        IResourceFontViewModel Resource { get; }

        ICommand CommandSelectResource { get; }

        void Save(IContentManager target);
        void Delete(IContentManager target);
    }
}
