using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

using Carbed.Contracts;
using Carbed.Logic.MVVM;

using Carbon.Editor.Contracts;
using Carbon.Engine.Contracts;

namespace Carbed.ViewModels
{
    using Carbon.Engine.Resource.Content;

    public class FontViewModel : ContentViewModel, IFontViewModel
    {
        private readonly FontEntry data;
        private readonly List<Font> selectableFonts;
        private readonly ICarbonBuilder builder;

        private ImageSource previewImage;

        private MetaDataEntry font;
        private MetaDataEntry size;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FontViewModel(IEngineFactory factory, FontEntry data)
            : base(factory, data)
        {
            this.data = data;
            this.builder = factory.Get<ICarbonBuilder>();
            this.selectableFonts = new List<Font>
                {
                    new Font("Arial", 10, FontStyle.Regular),
                    new Font("Courier New", 10, FontStyle.Regular),
                    new Font("Times new Roman", 10, FontStyle.Regular)
                };

            this.CommandUpdatePreview = new RelayCommand(this.OnUpdatePreview, this.CanPreview);

            this.Template = StaticResources.FontTemplate;

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
                return StaticResources.ResourceFontIconUri;
            }
        }
        
        public int? FontSize
        {
            get
            {
                return this.size.ValueInt;
            }

            set
            {
                if (this.size.ValueInt != value)
                {
                    this.CreateUndoState();
                    this.size.ValueInt = value;
                    this.UpdatePreview();
                    this.NotifyPropertyChanged();
                }
            }
        }
        
        public IReadOnlyCollection<Font> SelectableFonts
        {
            get
            {
                return this.selectableFonts.AsReadOnly();
            }
        }

        public Font SelectedFont
        {
            get
            {
                if (this.font.Value == null)
                {
                    return null;
                }

                return this.selectableFonts.FirstOrDefault(x => x.Name == this.font.Value);
            }

            set
            {
                this.CreateUndoState();
                this.font.Value = value == null ? null : value.Name;
                this.UpdatePreview();
                this.NotifyPropertyChanged();
            }
        }

        public ImageSource PreviewImage
        {
            get
            {
                return this.previewImage;
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
            return this.SelectedFont != null;
        }

        private void UpdatePreview()
        {
            this.previewImage = null;
            if (this.font.Value == null || this.FontSize <= 0)
            {
                return;
            }

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
