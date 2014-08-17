namespace GrandSeal.Editor.ViewModels
{
    using System.Drawing;

    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource.Content;
    using Core.Engine.Resource.Resources;
    using Core.Processing.Contracts;
    using Core.Processing.Processors;
    using global::GrandSeal.Editor.Contracts;
    using GrandSeal.Editor.Logic.MVVM;

    public class ResourceFontViewModel : ResourceViewModel, IResourceFontViewModel
    {
        private readonly IResourceProcessor resourceProcessor;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceFontViewModel(IFactory factory)
            : base(factory)
        {
            this.resourceProcessor = factory.Resolve<IResourceProcessor>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public FontStyle FontStyle
        {
            get
            {
                int value = this.GetMetaValueInt(MetaDataKey.FontStyle) ?? 0;
                return (FontStyle)value;
            }

            set
            {
                if (this.GetMetaValueInt(MetaDataKey.FontStyle) != (int)value)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.FontStyle, (int)value);
                    this.NeedSave = true;
                    this.PreviewImage = null;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int FontSize
        {
            get
            {
                return this.GetMetaValueInt(MetaDataKey.FontSize) ?? 0;
            }

            set
            {
                if (this.GetMetaValueInt(MetaDataKey.FontSize) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.FontSize, value);
                    this.NeedSave = true;
                    this.PreviewImage = null;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public int FontCharactersPerRow
        {
            get
            {
                return this.GetMetaValueInt(MetaDataKey.FontCharactersPerRow) ?? 0;
            }

            set
            {
                if (this.GetMetaValueInt(MetaDataKey.FontCharactersPerRow) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.FontCharactersPerRow, value);
                    this.NeedSave = true;
                    this.PreviewImage = null;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public override void SelectFile(CarbonFile file)
        {
            base.SelectFile(file);

            // Set some defaults that make sense
            if (this.FontCharactersPerRow == 0)
            {
                this.FontCharactersPerRow = 10;
            }

            if (this.FontSize == 0)
            {
                this.FontSize = 10;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        protected override void DoSave(IContentManager target, IResourceManager resourceTarget)
        {
            var options = new FontProcessingOptions
            {
                Style = this.FontStyle,
                Size = this.FontSize,
                CharactersPerRow = this.FontCharactersPerRow,
            };
            ICarbonResource resource = this.resourceProcessor.ProcessFont(this.SourceFile, options);
            
            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);

                // Todo: Create the actual font entry..

                this.NeedSave = false;
            }
            else
            {
                this.Log.Error("Failed to export font resource {0}", null, this.SourcePath);
            }
        }

        protected override System.Windows.Media.ImageSource GetPreviewImage()
        {
            var options = new FontProcessingOptions
            {
                Style = this.FontStyle,
                Size = this.FontSize,
                CharactersPerRow = this.FontCharactersPerRow,
            };
            RawResource resource = this.resourceProcessor.ProcessFont(this.SourceFile, options);
            if (resource != null)
            {
                return WPFUtilities.DataToImage(resource.Data);
            }

            return null;
        }
    }
}
