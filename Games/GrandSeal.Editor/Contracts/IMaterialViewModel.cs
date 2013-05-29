using System.Windows.Input;
using System.Windows.Media;

namespace GrandSeal.Editor.Contracts
{
    using Core.Engine.Contracts.Resource;

    public interface IMaterialViewModel : IEditorDocument
    {
        int? Id { get; }

        Color Color { get; set; }

        IResourceViewModel DiffuseTexture { get; }
        IResourceViewModel NormalTexture { get; }
        IResourceViewModel AlphaTexture { get; }
        IResourceViewModel SpecularTexture { get; }

        ICommand CommandSelectDiffuse { get; }
        ICommand CommandSelectNormal { get; }
        ICommand CommandSelectAlpha { get; }
        ICommand CommandSelectSpecular { get; }

        void Save(IContentManager target);
        void Delete(IContentManager target);
    }
}
