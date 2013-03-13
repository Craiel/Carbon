namespace Carbed.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;

    using global::Carbed.Contracts;
    using global::Carbed.Logic.MVVM;

    public partial class ResourceBrowser
    {
        private readonly ICarbedLogic logic;

        private readonly ObservableCollection<IResourceViewModel> filteredList;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceBrowser(ICarbedLogic logic)
        {
            this.logic = logic;

            this.DataContext = this;

            this.CommandRefresh = new RelayCommand(this.OnRefresh);

            this.filteredList = new ObservableCollection<IResourceViewModel>();

            InitializeComponent();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool CheckSelection { get; set; }

        public string FilterText { get; set; }

        public ReadOnlyObservableCollection<IResourceViewModel> FilteredList
        {
            get
            {
                return new ReadOnlyObservableCollection<IResourceViewModel>(this.filteredList);
            }
        }

        public IResourceViewModel SelectedResource { get; set; }

        public ICommand CommandRefresh { get; private set; }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnRefresh(object obj)
        {
            this.filteredList.Clear();
            IList<IResourceViewModel> resources = this.logic.LocateResources(this.FilterText);
            foreach (IResourceViewModel resource in resources)
            {
                this.filteredList.Add(resource);
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OnSelectClick(object sender, RoutedEventArgs e)
        {
            if (this.CheckSelection && this.SelectedResource == null)
            {
                MessageBox.Show("Please Select one or more resources!");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
