using Carbed.Contracts;

using Carbon.Editor.Contracts;
using Carbon.Editor.Processors;
using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Content;

namespace Carbed.ViewModels
{
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
        public ResourceTextureViewModel(IEngineFactory factory, ResourceEntry data)
            : base(factory, data)
        {
            this.resourceProcessor = factory.Get<IResourceProcessor>();
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
                    this.NeedReExport = true;
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
                    this.NeedReExport = true;
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
                    this.NeedReExport = true;
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
                    this.NeedReExport = true;
                    this.NotifyPropertyChanged();
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoExport(IContentManager target, IResourceManager resourceTarget)
        {
            ICarbonResource resource;
            if (!this.CompressTexture)
            {
                resource = this.resourceProcessor.ProcessRaw(this.SourcePath);
            }
            else
            {
                var options = new TextureProcessingOptions
                {
                    Format = this.TextureTargetFormat,
                    ConvertToNormalMap = this.ConvertToNormalMap,
                    IsNormalMap = this.IsNormalMap
                };
                resource = this.resourceProcessor.ProcessTexture(this.SourcePath, options);
            }

            if (resource != null)
            {
                resourceTarget.StoreOrReplace(this.Data.Hash, resource);
                this.NeedReExport = false;
            }
            else
            {
                this.Log.Error("Failed to export raw resource {0}", null, this.SourcePath);
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
    }
}
