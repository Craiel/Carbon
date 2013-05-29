using GrandSeal.Editor.Views;

namespace GrandSeal.Editor.Contracts
{
    public interface IEditor
    {
        MainView MainView { get; }

        void Run();
    }
}
