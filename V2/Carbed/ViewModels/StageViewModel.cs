using System;
using System.IO;
using System.Windows.Input;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Editor.Contracts;
using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    using Carbon.Engine.Resource.Content;

    public class StageViewModel : ContentViewModel, IStageViewModel
    {
        private readonly PlayfieldEntry data;
        private readonly ICarbonBuilder builder;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StageViewModel(IEngineFactory factory, PlayfieldEntry data)
            : base(factory, data)
        {
            this.data = data;
            this.builder = factory.Get<ICarbonBuilder>();

            this.CommandUpdatePreview = new RelayCommand(this.OnUpdatePreview, this.CanPreview);

            this.Template = StaticResources.PlayfieldTemplate;

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
                return StaticResources.ResourcePlayfieldIconUri;
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
