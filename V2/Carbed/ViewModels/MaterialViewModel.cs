using System;
using System.Windows.Input;
using System.Windows.Media;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
    public class MaterialViewModel : ContentViewModel, IMaterialViewModel
    {
        private readonly ICarbedLogic logic;
        private MaterialEntry data;

        private ImageSource previewDiffuseImage;
        private ImageSource previewNormalImage;
        private ImageSource previewSpecularImage;
        private ImageSource previewTransparencyImage;

        private ResourceEntry diffuseResource;
        private ResourceEntry normalResource;
        private ResourceEntry specularResource;
        private ResourceEntry transparencyResource;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MaterialViewModel(IEngineFactory factory, MaterialEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<ICarbedLogic>();

            this.data = data;

            this.CommandUpdatePreview = new RelayCommand(this.OnUpdatePreview, this.CanPreview);

            this.Template = StaticResources.MaterialTemplate;

            this.UpdatePreview();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Title
        {
            get
            {
                return this.Name ?? "<no name>";
            }
        }
        
        public override Uri IconUri
        {
            get
            {
                return StaticResources.ContentMaterialIconUri;
            }
        }

        public ImageSource PreviewDiffuseImage
        {
            get
            {
                return this.previewDiffuseImage;
            }
        }

        public ImageSource PreviewNormalImage
        {
            get
            {
                return this.previewNormalImage;
            }
        }

        public ImageSource PreviewSpecularImage
        {
            get
            {
                return this.previewSpecularImage;
            }
        }

        public ImageSource PreviewTransparencyImage
        {
            get
            {
                return this.previewTransparencyImage;
            }
        }
        
        public ICommand CommandUpdatePreview { get; private set; }

        public new void Save(IContentManager target)
        {
            base.Save(target);

            target.Save(ref this.data);
            if (this.diffuseResource != null)
            {
                target.Save(ref this.diffuseResource);
            }

            if (this.normalResource != null)
            {
                target.Save(ref this.normalResource);
            }

            if (this.specularResource != null)
            {
                target.Save(ref this.specularResource);
            }

            if (this.transparencyResource != null)
            {
                target.Save(ref this.transparencyResource);
            }

            this.NotifyPropertyChanged();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void RestoreMemento(object memento)
        {
            base.RestoreMemento(memento);

            this.UpdatePreview();
        }

        protected override void OnSave(object obj)
        {
            this.logic.Save(this);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnUpdatePreview(object obj)
        {
            this.UpdatePreview();
        }

        private bool CanPreview(object obj)
        {
            return false;
        }

        private void UpdatePreview()
        {
            // Todo
        }
    }
}
