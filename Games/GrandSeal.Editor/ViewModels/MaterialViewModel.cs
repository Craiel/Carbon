using System;
using System.Windows.Input;
using System.Windows.Media;

using Core.Engine.Contracts;
using Core.Engine.Contracts.Resource;
using Core.Engine.Resource.Content;
using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic.MVVM;
using GrandSeal.Editor.Views;

namespace GrandSeal.Editor.ViewModels
{
    public class MaterialViewModel : ContentViewModel, IMaterialViewModel
    {
        private readonly IEditorLogic logic;
        private readonly MaterialEntry data;
        
        private IResourceViewModel diffuseTexture;
        private IResourceViewModel normalTexture;
        private IResourceViewModel specularTexture;
        private IResourceViewModel alphaTexture;
        
        private ICommand commandSelectDiffuse;
        private ICommand commandSelectNormal;
        private ICommand commandSelectAlpha;
        private ICommand commandSelectSpecular;

        private bool needSave;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MaterialViewModel(IEngineFactory factory, MaterialEntry data)
            : base(factory, data)
        {
            this.logic = factory.Get<IEditorLogic>();

            this.data = data;

            this.Template = StaticResources.MaterialTemplate;
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

                // ReSharper disable ExplicitCallerInfoArgument
                this.NotifyPropertyChanged("Title");
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }
        
        public override Uri IconUri
        {
            get
            {
                return StaticResources.ContentMaterialIconUri;
            }
        }

        public int? Id
        {
            get
            {
                return this.data.Id;
            }
        }

        public Color Color
        {
            get
            {
                return Color.FromScRgb(this.data.ColorA, this.data.ColorR, this.data.ColorG, this.data.ColorB);
            }

            set
            {
                this.data.ColorA = value.ScA;
                this.data.ColorR = value.ScR;
                this.data.ColorG = value.ScG;
                this.data.ColorB = value.ScB;
                this.NotifyPropertyChanged();
            }
        }

        public IResourceViewModel DiffuseTexture
        {
            get
            {
                return this.diffuseTexture;
            }

            private set
            {
                if (this.diffuseTexture != value)
                {
                    this.CreateUndoState();
                    this.diffuseTexture = value;
                    this.needSave = true;
                    this.NotifyPropertyChanged();

                    // ReSharper disable ExplicitCallerInfoArgument
                    this.NotifyPropertyChanged("IsChanged");
                    // ReSharper restore ExplicitCallerInfoArgument
                }
            }
        }

        public IResourceViewModel NormalTexture
        {
            get
            {
                return this.normalTexture;
            }

            private set
            {
                if (this.normalTexture != value)
                {
                    this.CreateUndoState();
                    this.normalTexture = value;
                    this.needSave = true;
                    this.NotifyPropertyChanged("IsChanged");
                    this.NotifyPropertyChanged();
                }
            }
        }

        public IResourceViewModel AlphaTexture
        {
            get
            {
                return this.alphaTexture;
            }

            private set
            {
                if (this.alphaTexture != value)
                {
                    this.CreateUndoState();
                    this.alphaTexture = value;
                    this.needSave = true;
                    this.NotifyPropertyChanged("IsChanged");
                    this.NotifyPropertyChanged();
                }
            }
        }

        public IResourceViewModel SpecularTexture
        {
            get
            {
                return this.specularTexture;
            }

            private set
            {
                if (this.specularTexture != value)
                {
                    this.CreateUndoState();
                    this.specularTexture = value;
                    this.needSave = true;
                    this.NotifyPropertyChanged("IsChanged");
                    this.NotifyPropertyChanged();
                }
            }
        }
        
        public ICommand CommandSelectDiffuse
        {
            get
            {
                return this.commandSelectDiffuse ?? (this.commandSelectDiffuse = new RelayCommand(x => this.DiffuseTexture = this.SelectTexture()));
            }
        }

        public ICommand CommandSelectNormal
        {
            get
            {
                return this.commandSelectNormal ?? (this.commandSelectNormal = new RelayCommand(x => this.NormalTexture = this.SelectTexture())); ;
            }
        }

        public ICommand CommandSelectAlpha
        {
            get
            {
                return this.commandSelectAlpha ?? (this.commandSelectAlpha = new RelayCommand(x => this.AlphaTexture = this.SelectTexture())); ;
            }
        }

        public ICommand CommandSelectSpecular
        {
            get
            {
                return this.commandSelectSpecular ?? (this.commandSelectSpecular = new RelayCommand(x => this.SpecularTexture = this.SelectTexture())); ;
            }
        }
        
        public new void Save(IContentManager target)
        {
            int? id = this.diffuseTexture == null ? null : this.diffuseTexture.Id;
            this.data.DiffuseTexture = ProcessContentLink(this.data.DiffuseTexture, ContentLinkType.Resource, id, target);

            id = this.normalTexture == null ? null : this.normalTexture.Id;
            this.data.NormalTexture = ProcessContentLink(this.data.NormalTexture, ContentLinkType.Resource, id, target);

            id = this.alphaTexture == null ? null : this.alphaTexture.Id;
            this.data.AlphaTexture = ProcessContentLink(this.data.AlphaTexture, ContentLinkType.Resource, id, target);

            id = this.specularTexture == null ? null : this.specularTexture.Id;
            this.data.SpecularTexture = ProcessContentLink(this.data.SpecularTexture, ContentLinkType.Resource, id, target);

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

            if (this.data.DiffuseTexture != null)
            {
                this.diffuseTexture = this.logic.LocateResource((int)this.data.DiffuseTexture.ContentId);
            }

            if (this.data.AlphaTexture != null)
            {
                this.alphaTexture = this.logic.LocateResource((int)this.data.AlphaTexture.ContentId);
            }

            if (this.data.NormalTexture != null)
            {
                this.normalTexture = this.logic.LocateResource((int)this.data.NormalTexture.ContentId);
            }

            if (this.data.SpecularTexture != null)
            {
                this.specularTexture = this.logic.LocateResource((int)this.data.SpecularTexture.ContentId);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void OnSave(object obj)
        {
            this.logic.Save(this);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private IResourceViewModel SelectTexture()
        {
            var browser = new ResourceBrowser(this.logic);
            browser.CheckSelection = true;
            if (browser.ShowDialog() == true)
            {
                return browser.SelectedResource;
            }

            return null;
        }
    }
}
