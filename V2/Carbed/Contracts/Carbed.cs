using Carbed.Views;

namespace Carbed.Contracts
{
    public interface ICarbed
    {
        MainView MainView { get; }

        void Run();
    }
}
