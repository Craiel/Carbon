using System;
using System.IO;
using System.Windows.Input;

using GrandSeal.Editor.Contracts;
using GrandSeal.Editor.Logic.MVVM;

using Core.Engine.Contracts;

namespace GrandSeal.Editor.ViewModels
{
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.UtilsWPF;

    using Core.Engine.Resource.Content;

    public class StageViewModel : ContentViewModel, IStageViewModel
    {
        private readonly StageEntry data;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StageViewModel(IFactory factory)
            : base(factory)
        {
            this.data = data;

            this.CommandUpdatePreview = new RelayCommand(this.OnUpdatePreview, this.CanPreview);

            this.Template = StaticResources.StageTemplate;

            this.UpdatePreview();
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
                return this.data.IsChanged;
            }
        }
        
        public override Uri IconUri
        {
            get
            {
                return StaticResources.ContentStageIconUri;
            }
        }
        
        public ICommand CommandUpdatePreview { get; private set; }

        protected override void RestoreMemento(object memento)
        {
            base.RestoreMemento(memento);

            this.UpdatePreview();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnUpdatePreview()
        {
            this.UpdatePreview();
        }

        private bool CanPreview()
        {
            return false;
        }
        
        private void UpdatePreview()
        {
            using (var stream = new MemoryStream())
            {
                /*this.builder.Process(stream, this.data);
                stream.Position = 0;
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                this.previewImage = image;
                this.NotifyPropertyChanged("PreviewImage");*/
            }
        }
    }
}
