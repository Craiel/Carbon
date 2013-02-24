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
        private ImageSource previewAlphaImage;

        private ResourceEntry diffuseResource;
        private ResourceEntry normalResource;
        private ResourceEntry specularResource;
        private ResourceEntry alphaResource;
        
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
        public override bool IsChanged
        {
            get
            {
                return this.data.IsChanged || this.diffuseResource.IsChanged || this.normalResource.IsChanged ||
                       this.specularResource.IsChanged || this.alphaResource.IsChanged;
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

        public ImageSource PreviewAlphaImage
        {
            get
            {
                return this.previewAlphaImage;
            }
        }
        
        public ICommand CommandUpdatePreview { get; private set; }

        public new void Save(IContentManager target)
        {
            base.Save(target);

            if (this.diffuseResource != null && (this.diffuseResource.IsNew || this.diffuseResource.IsChanged))
            {
                target.Save(this.diffuseResource);
            }

            if (this.normalResource != null && (this.normalResource.IsNew || this.normalResource.IsChanged))
            {
                target.Save(this.normalResource);
            }

            if (this.specularResource != null && (this.specularResource.IsNew || this.specularResource.IsChanged))
            {
                target.Save(this.specularResource);
            }

            if (this.alphaResource != null && (this.alphaResource.IsNew || this.alphaResource.IsChanged))
            {
                target.Save(this.alphaResource);
            }

            this.NotifyPropertyChanged();
        }

        public new void Delete(IContentManager target)
        {
            base.Delete(target);

            if (this.diffuseResource != null && !this.diffuseResource.IsNew)
            {
                target.Delete(this.diffuseResource);
            }

            if (this.normalResource != null && !this.normalResource.IsNew)
            {
                target.Delete(this.diffuseResource);
            }

            if (this.specularResource != null && !this.specularResource.IsNew)
            {
                target.Delete(this.diffuseResource);
            }

            if (this.alphaResource != null && !this.alphaResource.IsNew)
            {
                target.Delete(this.diffuseResource);
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
