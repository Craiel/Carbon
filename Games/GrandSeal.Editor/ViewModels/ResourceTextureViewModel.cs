namespace GrandSeal.Editor.ViewModels
{
    using CarbonCore.Utils.Contracts.IoC;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Resource;
    using Core.Engine.Resource.Content;
    using Core.Processing.Contracts;
    using Core.Processing.Processors;

    using GrandSeal.Editor.Contracts;

    using global::GrandSeal.Editor.Logic.MVVM;

    public class ResourceTextureViewModel : ResourceViewModel, IResourceTextureViewModel
    {
        private enum Flags
        {
            CompressTexture = 0,
            IsNormalMap = 1,
            ConvertToNormalMap = 2,
        }

        private readonly IResourceProcessor resourceProcessor;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceTextureViewModel(IFactory factory)
            : base(factory)
        {
            this.resourceProcessor = factory.Resolve<IResourceProcessor>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool CompressTexture
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.CompressTexture) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.CompressTexture) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)Flags.CompressTexture, value);
                    this.NeedSave = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool IsNormalMap
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.IsNormalMap) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.IsNormalMap) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)Flags.IsNormalMap, value);
                    this.NeedSave = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public bool ConvertToNormalMap
        {
            get
            {
                return this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.ConvertToNormalMap) ?? false;
            }

            set
            {
                if (this.GetMetaValueBit(MetaDataKey.ResourceFlags, (int)Flags.ConvertToNormalMap) != value)
                {
                    this.CreateUndoState();
                    this.SetMetaBitValue(MetaDataKey.ResourceFlags, (int)Flags.ConvertToNormalMap, value);
                    this.NeedSave = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        public TextureTargetFormat TextureTargetFormat
        {
            get
            {
                long value = this.GetMetaValueInt(MetaDataKey.TextureTargetFormat) ?? 0;
                return (TextureTargetFormat)value;
            }

            set
            {
                if (this.GetMetaValueInt(MetaDataKey.TextureTargetFormat) != (int)value)
                {
                    this.CreateUndoState();
                    this.SetMetaValue(MetaDataKey.TextureTargetFormat, (int)value);
                    this.NeedSave = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoSave(IContentManager target, IResourceManager resourceTarget)
        {
            ICarbonResource resource;
            if (!this.CompressTexture)
            {
                resource = this.resourceProcessor.ProcessRaw(this.SourceFile);
            }
            else
            {
                var options = new TextureProcessingOptions
                {
                    Format = this.TextureTargetFormat,
                    ConvertToNormalMap = this.ConvertToNormalMap,
                    IsNormalMap = this.IsNormalMap
                };
                resource = this.resourceProcessor.ProcessTexture(this.SourceFile, options);
            }

            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);
                this.NeedSave = false;
            }
            else
            {
                System.Diagnostics.Trace.TraceError("Failed to export raw resource {0}", null, this.SourcePath);
            }
        }

        protected override void SetSettingsByExtension(string extension)
        {
            base.SetSettingsByExtension(extension);
            
            switch (extension.ToLower())
            {
                case ".dds":
                    {
                        this.CompressTexture = false;
                        break;
                    }

                case ".png":
                case ".tif":
                case ".jpg":
                case ".tga":
                    {
                        this.CompressTexture = true;
                        this.TextureTargetFormat = TextureTargetFormat.DDSDxt1;
                        break;
                    }
            }
        }

        protected override System.Windows.Media.ImageSource GetPreviewImage()
        {
            switch (this.SourceFile.Extension)
            {
                case ".png":
                case ".tif":
                case ".jpg":
                    {
                        return WPFUtilities.FileToImage(this.SourcePath);
                    }
            }

            return null;
        }
    }
}
