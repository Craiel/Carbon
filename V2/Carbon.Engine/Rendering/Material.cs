using SlimDX;

namespace Carbon.Engine.Rendering
{
    using System;

    using Carbon.Engine.Contracts.Logic;
    using Carbon.Engine.Logic;
    using Carbon.Engine.Resource;

    public class Material : IDisposable
    {
        private readonly ICarbonGraphics graphics;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Material(ICarbonGraphics graphics, MaterialResource resource)
        {
            this.graphics = graphics;

            this.Color = resource.Color;

            this.DiffuseTexture = graphics.TextureManager.Register(resource.DiffuseTexture);

            if (!string.IsNullOrEmpty(resource.NormalTexture))
            {
                this.NormalTexture = graphics.TextureManager.Register(resource.NormalTexture);
            }

            if (!string.IsNullOrEmpty(resource.SpecularTexture))
            {
                this.SpecularTexture = graphics.TextureManager.Register(resource.SpecularTexture);
            }

            if (!string.IsNullOrEmpty(resource.AlphaTexture))
            {
                this.AlphaTexture = graphics.TextureManager.Register(resource.AlphaTexture);
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
