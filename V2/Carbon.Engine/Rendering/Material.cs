using System;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Carbon.Engine.Resource.Content;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    public class Material : IDisposable
    {
        private readonly ICarbonGraphics graphics;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Material(ICarbonGraphics graphics, MaterialEntry content)
        {
            this.graphics = graphics;

            this.Color = new Vector4(content.ColorR, content.ColorG, content.ColorB, content.ColorA);

            // Todo: ContentLink change requires more to get the registration working
            /*if (content.DiffuseTexture != null)
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
            }*/
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
