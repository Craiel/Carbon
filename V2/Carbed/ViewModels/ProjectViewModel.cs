using System;

using Carbed.Contracts;

using Carbon.Editor.Resource;
using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    public class ProjectViewModel : DocumentViewModel, IProjectViewModel
    {
        private readonly SourceProject data;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProjectViewModel(IEngineFactory factory, SourceProject data)
            : base(factory)
        {
            this.Template = StaticResources.ProjectTemplate;
            this.data = data;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.data.Name))
                {
                    return "<no name>";
                }

                return this.data.Name;
            }
            set
            {
                if (this.data.Name != value)
                {
                    this.CreateUndoState();
                    this.data.Name = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("IsChanged");
                }
            }
        }
        
        public override Uri IconUri
        {
            get
            {
                return StaticResources.ProjectIconUri;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override object CreateMemento()
        {
            return this.data.Clone();
        }

        protected override void RestoreMemento(object memento)
        {
            SourceProject source = memento as SourceProject;
            if (source == null)
            {
                throw new ArgumentException();
            }

            this.data.LoadFrom(source);
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
