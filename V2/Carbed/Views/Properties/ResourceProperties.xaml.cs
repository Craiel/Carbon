using System.Windows;

namespace Carbed.Views.Properties
{
    public partial class ResourceProperties
    {
        public ResourceProperties()
        {
            InitializeComponent();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            this.elementComboBox.SelectedItem = null;
        }
    }
}
