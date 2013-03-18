namespace Carbed.ViewModels
{
    using System.Drawing;

    using Carbon.Editor.Contracts;
    using Carbon.Editor.Processors;
    using Carbon.Engine.Contracts;
    using Carbon.Engine.Contracts.Resource;
    using Carbon.Engine.Resource.Content;

    using global::Carbed.Contracts;

    public class ResourceFontViewModel : ResourceViewModel, IResourceFontViewModel
    {
        private readonly IResourceProcessor resourceProcessor;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceFontViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.resourceProcessor = factory.Get<IResourceProcessor>();
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
                    this.NeedReExport = true;
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
                    this.NeedReExport = true;
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
                    this.NeedReExport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public override void SelectFile(string path)
        {
            base.SelectFile(path);

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
        protected override void DoExport(IContentManager target, IResourceManager resourceTarget)
        {
            var options = new FontProcessingOptions
            {
                Style = this.FontStyle,
                Size = this.FontSize,
                CharactersPerRow = this.FontCharactersPerRow,
            };
            ICarbonResource resource = this.resourceProcessor.ProcessFont(this.SourcePath, options);
            
            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);

                // Todo: Create the actual font entry..

                this.NeedReExport = false;
            }
            else
            {
                this.Log.Error("Failed to export font resource {0}", null, this.SourcePath);
            }
        }
    }
}
