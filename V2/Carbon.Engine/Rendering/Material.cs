﻿using System;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Logic;
using Carbon.Engine.Resource.Content;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    using Carbon.Engine.Contracts.Resource;
    using Carbon.Engine.Resource.Resources.Model;

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
                graphics.TextureManager.Register(resource.Hash);
                this.DiffuseTexture = graphics.TextureManager.GetReference(resource.Hash);
            }

            if (content.NormalTexture != null)
            {
                var resource = contentManager.Load<ResourceEntry>(content.NormalTexture);
                graphics.TextureManager.Register(resource.Hash);
                this.NormalTexture = graphics.TextureManager.GetReference(resource.Hash);
            }

            if (content.AlphaTexture != null)
            {
                var resource = contentManager.Load<ResourceEntry>(content.AlphaTexture);
                graphics.TextureManager.Register(resource.Hash);
                this.AlphaTexture = graphics.TextureManager.GetReference(resource.Hash);
            }

            if (content.SpecularTexture != null)
            {
                var resource = contentManager.Load<ResourceEntry>(content.SpecularTexture);
                graphics.TextureManager.Register(resource.Hash);
                this.SpecularTexture = graphics.TextureManager.GetReference(resource.Hash);
            }
        }

        public Material(ICarbonGraphics graphics, ModelMaterialElement content)
        {
            this.graphics = graphics;

            if (!string.IsNullOrEmpty(content.DiffuseTexture))
            {
                graphics.TextureManager.Register(content.DiffuseTexture);
                this.DiffuseTexture = graphics.TextureManager.GetReference(content.DiffuseTexture);
            }

            if (!string.IsNullOrEmpty(content.NormalTexture))
            {
                graphics.TextureManager.Register(content.NormalTexture);
                this.NormalTexture = graphics.TextureManager.GetReference(content.NormalTexture);
            }

            if (!string.IsNullOrEmpty(content.SpecularTexture))
            {
                graphics.TextureManager.Register(content.SpecularTexture);
                this.SpecularTexture = graphics.TextureManager.GetReference(content.SpecularTexture);
            }

            if (!string.IsNullOrEmpty(content.AlphaTexture))
            {
                graphics.TextureManager.Register(content.AlphaTexture);
                this.AlphaTexture = graphics.TextureManager.GetReference(content.AlphaTexture);
            }
        }

        public Material(ICarbonGraphics graphics, string diffuse = null, string normal = null, string alpha = null, string specular = null)
        {
            this.graphics = graphics;
            if (!string.IsNullOrEmpty(diffuse))
            {
                graphics.TextureManager.Register(diffuse);
                this.DiffuseTexture = graphics.TextureManager.GetReference(diffuse);
            }

            if (!string.IsNullOrEmpty(normal))
            {
                graphics.TextureManager.Register(normal);
                this.NormalTexture = graphics.TextureManager.GetReference(normal);
            }

            if (!string.IsNullOrEmpty(alpha))
            {
                graphics.TextureManager.Register(alpha);
                this.AlphaTexture = graphics.TextureManager.GetReference(alpha);
            }

            if (!string.IsNullOrEmpty(specular))
            {
                graphics.TextureManager.Register(specular);
                this.SpecularTexture = graphics.TextureManager.GetReference(specular);
            }
        }

        public Material()
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector4? Color { get; private set; }

        public TextureReference DiffuseTexture { get; set; }
        public TextureReference NormalTexture { get; set; }
        public TextureReference SpecularTexture { get; set; }
        public TextureReference AlphaTexture { get; set; }

        public void Dispose()
        {
            // No graphics means we where initialized directly and do not manage
            if (this.graphics == null)
            {
                return;
            }

            if (this.DiffuseTexture != null)
            {
                this.graphics.TextureManager.Release(this.DiffuseTexture);
                this.DiffuseTexture = null;
            }

            if (this.NormalTexture != null)
            {
                this.graphics.TextureManager.Release(this.NormalTexture);
                this.NormalTexture = null;
            }

            if (this.SpecularTexture != null)
            {
                this.graphics.TextureManager.Release(this.SpecularTexture);
                this.SpecularTexture = null;
            }

            if (this.AlphaTexture != null)
            {
                this.graphics.TextureManager.Release(this.AlphaTexture);
                this.AlphaTexture = null;
            }
        }
    }
}
