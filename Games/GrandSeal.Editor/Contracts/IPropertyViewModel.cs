using System.Windows.Controls;

namespace GrandSeal.Editor.Contracts
{
    public interface IPropertyViewModel : IEditorTool
    {
        IEditorBase ActiveContext { get; set; }

        Control PropertyControl { get; }

        void SetActivation(IEditorBase source, bool active);
    }
}
