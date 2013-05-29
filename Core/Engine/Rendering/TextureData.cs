using System;
using SlimDX.Direct3D11;

namespace Core.Engine.Rendering
{
    public enum TextureDataType
    {
        Unknown,
        Texture2D
    }

    public class TextureData : IDisposable
    {
        private readonly byte[] data;

        private readonly ImageLoadInformation? loadInformation;
        private readonly ShaderResourceViewDescription? viewDescription;

        private readonly bool locked;

        public TextureData(TextureDataType type, byte[] data, ImageLoadInformation loadInfo, ShaderResourceViewDescription viewDescription)
            : this(type, data, loadInfo)
        {
            this.viewDescription = viewDescription;
        }

        public TextureData(TextureDataType type, byte[] data, ImageLoadInformation loadInfo)
            : this(type, data)
        {
            this.loadInformation = loadInfo;
        }

        public TextureData(TextureDataType type, byte[] data)
        {
            this.Type = type;
            this.data = data;
        }

        public TextureData(Texture2D data, ShaderResourceView view)
        {
            if (data == null)
            {
                throw new ArgumentException();
            }

            this.View = view;
            this.Texture2D = data;
            this.Type = TextureDataType.Texture2D;
            this.locked = true;
        }

        public TextureDataType Type { get; private set; }
        public Texture2D Texture2D { get; private set; }
        public ShaderResourceView View { get; private set; }

        public void Dispose()
        {
            if (this.locked)
            {
                return;
            }

            this.ReleaseView();
        }

        public void ReleaseView()
        {
            if (this.locked)
            {
                throw new InvalidOperationException();
            }

            this.View.Dispose();
            this.View = null;
        }

        public void ReleaseTexture()
        {
            if (this.locked)
            {
                throw new InvalidOperationException();
            }

            if (this.View != null)
            {
                this.ReleaseView();
            }

            switch (this.Type)
            {
                case TextureDataType.Texture2D:
                    {
                        if (this.Texture2D != null)
                        {
                            this.Texture2D.Dispose();
                            this.Texture2D = null;
                        }

                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public void InitializeView(Device graphics)
        {
            switch (this.Type)
            {
                case TextureDataType.Texture2D:
                    {
                        if (this.Texture2D == null)
                        {
                            this.InitializeTexture(graphics);
                        }

                        if (this.viewDescription != null)
                        {
                            this.View = new ShaderResourceView(graphics, this.Texture2D, (ShaderResourceViewDescription)this.viewDescription);
                        }
                        else
                        {
                            this.View = new ShaderResourceView(graphics, this.Texture2D);
                        }

                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public void InitializeTexture(Device graphics)
        {
            switch (this.Type)
            {
                case TextureDataType.Texture2D:
                    {
                        this.Texture2D = this.loadInformation != null ? Texture2D.FromMemory(graphics, this.data, (ImageLoadInformation)this.loadInformation) : Texture2D.FromMemory(graphics, this.data);
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
