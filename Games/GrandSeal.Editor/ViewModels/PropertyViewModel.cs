using System.Windows.Controls;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Views.Properties;

namespace GrandSeal.Editor.ViewModels
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public class PropertyViewModel : ToolViewModel, IPropertyViewModel
    {
        private IBaseViewModel activeContext;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return "Properties";
            }
        }

        public IBaseViewModel ActiveContext
        {
            get
            {
                return this.activeContext;
            }

            set
            {
                if (this.activeContext != value)
                {
                    this.activeContext = value;
                    this.UpdatePropertyControl();
                    this.NotifyPropertyChanged();
                }
            }
        }

        public Control PropertyControl { get; private set; }

        public void SetActivation(IBaseViewModel source, bool active)
        {
            if (active)
            {
                this.ActiveContext = source;
            }
            else
            {
                if (this.activeContext == source)
                {
                    this.ActiveContext = null;
                }
            }
        }

        private void UpdatePropertyControl()
        {
            if (this.activeContext == null)
            {
                this.PropertyControl = null;
            }
            else
            {
                if (this.activeContext is IProjectViewModel)
                {
                    this.PropertyControl = new ProjectProperties { DataContext = this.activeContext };
                }

                if (this.activeContext is IFolderViewModel)
                {
                    this.PropertyControl = new FolderProperties { DataContext = this.activeContext };
                }

                if (this.activeContext is IResourceViewModel)
                {
                    this.PropertyControl = new ResourceProperties { DataContext = this.activeContext };
                }

                if (this.activeContext is IMaterialViewModel)
                {
                    this.PropertyControl = new MaterialProperties { DataContext = this.activeContext };
                }

                if (this.activeContext is IFontViewModel)
                {
                    this.PropertyControl = new FontProperties { DataContext = this.activeContext };
                }
            }

            this.NotifyPropertyChanged("PropertyControl");
        }
    }
}
