using System.Windows.Controls;

namespace Carbed.Contracts
{
    public interface IPropertyViewModel : ICarbedTool
    {
        ICarbedBase ActiveContext { get; set; }

        Control PropertyControl { get; }

        void SetActivation(ICarbedBase source, bool active);
    }
}
