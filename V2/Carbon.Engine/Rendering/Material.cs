using System;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Carbon.Engine.Resource.Content;
using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    using Carbon.Engine.Contracts.Resource;

    public class Material : IDisposable
    {
        private readonly ICarbonGraphics graphics;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Material(ICarbonGraphics graphics, IContentManager contentManager, MaterialEntry content)
        {
            this.graphics = graphics;

            this.Color = new Vector4(content.ColorR, content.ColorG, content.ColorB, content.ColorA);

            if (content.DiffuseTexture != null)
            {
                var resource = contentManager.Load<ResourceEntry>(content.DiffuseTexture);
                this.DiffuseTexture = graphics.TextureManager.Register(resource.Hash);
            }

            if (content.NormalTexture != null)
            {
                var resource = contentManager.Load<ResourceEntry>(content.NormalTexture);
                this.NormalTexture = graphics.TextureManager.Register(resource.Hash);
            }

            if (content.AlphaTexture != null)
            {
                var resource = contentManager.Load<ResourceEntry>(content.AlphaTexture);
                this.AlphaTexture = graphics.TextureManager.Register(resource.Hash);
            }

            if (content.SpecularTexture != null)
            {
                var resource = contentManager.Load<ResourceEntry>(content.SpecularTexture);
                this.SpecularTexture = graphics.TextureManager.Register(resource.Hash);
            }
        }

        public Material(ICarbonGraphics graphics, MaterialElement content)
        {
            this.graphics = graphics;

            if (content.DiffuseTexture != null)
            {
                this.DiffuseTexture = graphics.TextureManager.Register(content.DiffuseTexture);
            }

            if (content.NormalTexture != null)
            {
                this.NormalTexture = graphics.TextureManager.Register(content.NormalTexture);
            }

            if (content.SpecularTexture != null)
            {
                this.SpecularTexture = graphics.TextureManager.Register(content.SpecularTexture);
            }

            if (content.AlphaTexture != null)
            {
                this.AlphaTexture = graphics.TextureManager.Register(content.AlphaTexture);
            }
        }

        public Material(TextureReference diffuseReference)
        {
            this.DiffuseTexture = diffuseReference;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector4? Color { get; private set; }

        public TextureReference DiffuseTexture { get; private set; }
        public TextureReference NormalTexture { get; private set; }
        public TextureReference SpecularTexture { get; private set; }
        public TextureReference AlphaTexture { get; private set; }

        public void Dispose()
        {
            // No graphics means we where initialized directly and do not manage
            if (this.graphics == null)
            {
                return;
            }

            this.graphics.TextureManager.Unregister(this.DiffuseTexture.Register);

            if (this.NormalTexture != null)
            {
                this.graphics.TextureManager.Unregister(this.NormalTexture.Register);
            }

            if (this.SpecularTexture != null)
            {
                this.graphics.TextureManager.Unregister(this.SpecularTexture.Register);
            }

            if (this.AlphaTexture != null)
            {
                this.graphics.TextureManager.Unregister(this.AlphaTexture.Register);
            }
        }
    }
}
