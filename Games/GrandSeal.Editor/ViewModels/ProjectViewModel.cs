using System;

using GrandSeal.Editor.Contracts;

using Core.Engine.Contracts;
using Core.Engine.Resource.Content;

namespace GrandSeal.Editor.ViewModels
{
    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class ProjectViewModel : ContentViewModel, IProjectViewModel
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProjectViewModel(IFactory factory)
            : base(factory)
        {
            this.Template = StaticResources.ProjectTemplate;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return this.Name;
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
            ProjectEntry source = memento as ProjectEntry;
            if (source == null)
            {
                throw new ArgumentException();
            }

            this.data.LoadFrom(source);
            this.NotifyPropertyChanged(string.Empty);
        }
    }
}
