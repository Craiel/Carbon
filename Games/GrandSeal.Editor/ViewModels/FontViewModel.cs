using System;
using System.Windows.Input;
using System.Windows.Media;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic.MVVM;

using Core.Engine.Contracts;
using Core.Engine.Resource.Content;

namespace GrandSeal.Editor.ViewModels
{
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.UtilsWPF;

    using Core.Engine.Contracts.Resource;

    using global::GrandSeal.Editor.Views;

    public class FontViewModel : ContentViewModel, IFontViewModel
    {
        private readonly IEditorLogic logic;
        private readonly FontEntry data;
        
        private IResourceFontViewModel fontResource;
        
        private ICommand commandSelectResource;

        private bool needSave;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FontViewModel(IFactory factory)
            : base(factory)
        {
            this.logic = factory.Resolve<IEditorLogic>();
            
            this.Template = StaticResources.FontTemplate;
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

        public override bool IsChanged
        {
            get
            {
                return base.IsChanged || this.needSave;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
                this.NotifyPropertyChanged("Title");
            }
        }
        
        public override Uri IconUri
        {
            get
            {
                return StaticResources.ContentFontIconUri;
            }
        }

        public int? Id
        {
            get
            {
                return this.data.Id;
            }
        }

        public IResourceFontViewModel Resource
        {
            get
            {
                return this.fontResource;
            }

            private set
            {
                if (this.fontResource != value)
                {
                    this.CreateUndoState();
                    this.fontResource = value;
                    this.needSave = true;
                    this.NotifyPropertyChanged("IsChanged");
                    this.NotifyPropertyChanged();
                }
            }
        }

        public ICommand CommandSelectResource
        {
            get
            {
                return this.commandSelectResource ?? (this.commandSelectResource = new RelayCommand(() => this.Resource = this.SelectResource()));
            }
        }
        
        public new void Save(IContentManager target)
        {
            int? id = this.fontResource == null ? null : this.fontResource.Id;
            this.data.Resource = ProcessContentLink(this.data.Resource, ContentLinkType.Resource, id, target);

            this.data.CharactersPerRow = this.fontResource == null ? 0 : this.fontResource.FontCharactersPerRow;
            base.Save(target);
            this.NotifyPropertyChanged();
        }

        public new void Delete(IContentManager target)
        {
            base.Delete(target);
            this.NotifyPropertyChanged();
        }

        public override void Load()
        {
            base.Load();

            if (this.data.Resource != null)
            {
                this.fontResource = this.logic.LocateResource((int)this.data.Resource.ContentId) as IResourceFontViewModel;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnSave(bool force)
        {
            this.logic.Save(this);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private IResourceFontViewModel SelectResource()
        {
            var browser = new ResourceBrowser(this.logic);
            browser.CheckSelection = true;
            if (browser.ShowDialog() == true)
            {
                return (IResourceFontViewModel)browser.SelectedResource;
            }

            return null;
        }
    }
}
