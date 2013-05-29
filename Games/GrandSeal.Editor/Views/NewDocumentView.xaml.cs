using System.Windows;

namespace GrandSeal.Editor.Views
{
    /// <summary>
    /// Interaction logic for NewDocumentView.xaml
    /// </summary>
    public partial class NewDocumentView
    {
        public NewDocumentView()
        {
            InitializeComponent();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnCreateClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
