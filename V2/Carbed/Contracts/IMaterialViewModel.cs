using System.Windows.Input;
using System.Windows.Media;

namespace Carbed.Contracts
{
    using Carbon.Engine.Contracts.Resource;

    public interface IMaterialViewModel : ICarbedDocument
    {
        int? Id { get; }

        Color Color { get; }

        IResourceViewModel DiffuseTexture { get; }
        IResourceViewModel NormalTexture { get; }
        IResourceViewModel AlphaTexture { get; }
        IResourceViewModel SpecularTexture { get; }

        ICommand CommandSelectColor { get; }
        ICommand CommandSelectDiffuse { get; }
        ICommand CommandSelectNormal { get; }
        ICommand CommandSelectAlpha { get; }
        ICommand CommandSelectSpecular { get; }

        void Save(IContentManager target);
        void Delete(IContentManager target);
    }
}
