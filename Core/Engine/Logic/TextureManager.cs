namespace Core.Engine.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Core.Engine.Contracts.Resource;
    using Core.Engine.Rendering;
    using Core.Engine.Resource.Resources;

    using SharpDX.Direct3D11;

    using SharpDX.Direct3D11;

    public enum TextureReferenceType
    {
        Resource,
        View,
        Register
    }

    public enum StaticTextureRegister
    {
        Unknown = 0,

        GBufferNormal = 11,
        GBufferDiffuse = 12,
        GBufferSpecular = 13,
        GBufferDepth = 14,

        DeferredLight = 15,
        ShadowMapTarget = 16,
    }

    public struct TextureReferenceDescription
    {
        public int Register;
        public TextureReferenceType Type;
        public TypedVector2<int> Size;
    }

    public class TextureManager : IDisposable
    {
        private const int StaticRegisterLimit = 4999;

        private readonly IResourceManager resourceManager;

        private readonly IDictionary<string, TextureReference> managedReferenceCache;
        private readonly IDictionary<TextureReference, TextureData> textureCache;
        private readonly IDictionary<int, TextureReference> textureRegister;
        private readonly IDictionary<TextureReference, int> referenceCount;

        private int nextRegister = StaticRegisterLimit + 1;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextureManager(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;

            this.managedReferenceCache = new Dictionary<string, TextureReference>();
            this.textureCache = new Dictionary<TextureReference, TextureData>();
            this.textureRegister = new Dictionary<int, TextureReference>();
            this.referenceCount = new Dictionary<TextureReference, int>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TextureReference Fallback { get; private set; }

        public void Dispose()
        {
            if (this.Fallback != null)
            {
                this.Release(this.Fallback);
            }

            foreach (TextureReference reference in this.referenceCount.Keys)
            {
                if (this.referenceCount[reference] > 0)
                {
                    System.Diagnostics.Trace.TraceError("Texture register {0} has {1} references on dispose", reference.Register, this.referenceCount[reference]);
                }
            }

            this.ClearCache();
        }

        public void RegisterStatic(TextureData data, int register, TypedVector2<int> size)
        {
            this.CheckStaticRegister(register);

            var description = new TextureReferenceDescription
                                  {
                                      Register = register,
                                      Type = TextureReferenceType.View,
                                      Size = size
                                  };
            var reference = new TextureReference(description);
            this.textureRegister.Add(register, reference);
            this.textureCache.Add(reference, data);
            this.referenceCount.Add(reference, 0);
        }

        public void RegisterStatic(string hash, int register)
        {
            this.CheckStaticRegister(register);

            var description = new TextureReferenceDescription
                                  {
                                      Register = register,
                                      Type = TextureReferenceType.Resource
                                  };
            var reference = new TextureReference(hash, description);
            this.textureRegister.Add(register, reference);
            this.referenceCount.Add(reference, 0);
        }

        public void Register(string hash)
        {
            // Check if we have a managed reference for this hash, during runtime this will not have to change since texture references hold no runtime info thats relevant
            if (this.managedReferenceCache.ContainsKey(hash))
            {
                return;
            }

            var description = new TextureReferenceDescription
            {
                Register = this.nextRegister++,
                Type = TextureReferenceType.Resource
            };
            var reference = new TextureReference(hash, description);
            this.textureRegister.Add(reference.Register, reference);
            this.referenceCount.Add(reference, 0);
            this.managedReferenceCache.Add(hash, reference);
        }
        
        public bool IsRegistered(string hash)
        {
            return this.managedReferenceCache.ContainsKey(hash);
        }
        
        public void Release(TextureReference reference)
        {
            if (!this.referenceCount.ContainsKey(reference))
            {
                throw new ArgumentException("Release called with unknown texture reference");
            }

            this.referenceCount[reference]--;
        }

        public void Unregister(int register)
        {
            if (!this.textureRegister.ContainsKey(register))
            {
                throw new InvalidOperationException("Unregister called with non-existing reference");
            }

            TextureReference reference = this.textureRegister[register];
            if (this.referenceCount[reference] > 0)
            {
                throw new InvalidOperationException("Unregister called on referenced texture: " + this.referenceCount[reference]);
            }

            this.textureRegister.Remove(register);
            this.referenceCount.Remove(reference);
            reference.Invalidate();

            if (this.textureCache.ContainsKey(reference))
            {
                // Free the resource if it's a managed one
                if (reference.Type != TextureReferenceType.View)
                {
                    this.textureCache[reference].Dispose();
                }

                this.textureCache.Remove(reference);
            }
        }

        public TextureReference GetReference(string hash)
        {
            if (!this.managedReferenceCache.ContainsKey(hash))
            {
                throw new InvalidDataException("No texture registered for hash " + hash);
            }

            this.referenceCount[this.managedReferenceCache[hash]]++;
            return this.managedReferenceCache[hash];
        }
        
        public TextureReference GetRegisterReference(int register)
        {
            if (register > StaticRegisterLimit)
            {
                throw new InvalidOperationException("Can't get register reference outside of static range");
            }

            return new TextureReference(new TextureReferenceDescription { Register = register, Type = TextureReferenceType.Register });
        }

        public TextureData GetTexture(TextureReference reference, ImageLoadInformation? information = null, ShaderResourceViewDescription? viewDescription = null)
        {
            if (!reference.IsValid)
            {
                throw new ArgumentException("Invalid texture reference was passed, did you keep the reference alive too long?");
            }

            TextureReference textureReference = reference;
            if (reference.Type == TextureReferenceType.Register)
            {
                if (!this.textureRegister.ContainsKey(reference.Register))
                {
                    throw new InvalidDataException("No texture registered in texture reference registered as " + reference.Register);
                }

                textureReference = this.textureRegister[reference.Register];
            }

            if (!this.textureCache.ContainsKey(textureReference))
            {
                this.textureCache[reference] = this.Load(textureReference, information, viewDescription);
            }

            return this.textureCache[textureReference];
        }
        
        public void ClearCache()
        {
            lock (this.textureCache)
            {
                IList<TextureReference> clearQueue = new List<TextureReference>();
                foreach (TextureReference reference in this.textureCache.Keys)
                {
                    // Keep all valid View type references
                    if ((reference.Type == TextureReferenceType.View && reference.IsValid) || this.referenceCount[reference] > 0)
                    {
                        continue;
                    }

                    clearQueue.Add(reference);
                }

                foreach (TextureReference textureReference in clearQueue)
                {
                    this.textureCache[textureReference].Dispose();
                    this.textureCache.Remove(textureReference);
                    textureReference.Invalidate();
                }
            }
        }

        public void SetFallback(string resource)
        {
            if (this.Fallback != null)
            {
                this.Release(this.Fallback);
            }

            if (!this.IsRegistered(resource))
            {
                this.Register(resource);
            }

            this.Fallback = this.GetReference(resource);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void CheckStaticRegister(int register)
        {
            if (register > StaticRegisterLimit)
            {
                throw new ArgumentException("Static resources can not be set above " + StaticRegisterLimit);
            }

            if (this.textureRegister.ContainsKey(register))
            {
                throw new ArgumentException(string.Format("Static register is already taken ({0}) by {1}", register, this.textureRegister[register].ResourceHash));
            }
        }
        
        private TextureData Load(TextureReference reference, ImageLoadInformation? information, ShaderResourceViewDescription? viewDescription)
        {
            if (viewDescription != null && information == null)
            {
                throw new InvalidOperationException("Can not supply custom view description without custom load information");
            }

            if (reference.Data == null)
            {
                reference.Data = this.LoadData(reference);
            }

            if (information == null)
            {
                return new TextureData(TextureDataType.Texture2D, reference.Data);
            }

            if (viewDescription == null)
            {
                return new TextureData(TextureDataType.Texture2D, reference.Data, (ImageLoadInformation)information);
            }

            return new TextureData(TextureDataType.Texture2D, reference.Data, (ImageLoadInformation)information, (ShaderResourceViewDescription)viewDescription);
        }

        private byte[] LoadData(TextureReference reference)
        {
            switch (reference.Type)
            {
                case TextureReferenceType.Resource:
                    {
                        var textureResource = this.resourceManager.Load<RawResource>(reference.ResourceHash);
                        if (textureResource != null)
                        {
                            return textureResource.Data;
                        }

                        break;
                    }

                case TextureReferenceType.View:
                    {
                        throw new InvalidOperationException("LoadData called with view typed reference, this is not valid!");
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            if (reference != this.Fallback && this.Fallback != null)
            {
                System.Diagnostics.Trace.TraceWarning("Loading fallback for {0}", reference);
                return this.LoadData(this.Fallback);
            }

            throw new InvalidDataException("Texture could not be loaded and no fallback is present");
        }
    }
}
