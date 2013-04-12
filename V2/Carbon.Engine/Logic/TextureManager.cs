using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource.Resources;

using SlimDX.Direct3D11;

namespace Carbon.Engine.Logic
{
    public enum TextureReferenceType
    {
        Resource,
        Memory,
        View,
        Register
    }

    public class TextureReference
    {
        internal TextureReference(int register, bool registerReference)
        {
            this.Register = register;
            this.Type = registerReference ? TextureReferenceType.Register : TextureReferenceType.View;
        }

        internal TextureReference(string resourceHash, int register)
        {
            this.ResourceHash = resourceHash;
            this.Register = register;
            this.Type = TextureReferenceType.Resource;
        }

        internal TextureReference(byte[] data, int register)
        {
            this.Data = data;
            this.Register = register;
            this.Type = TextureReferenceType.Memory;
        }

        public TextureReferenceType Type { get; private set; }

        public string ResourceHash { get; private set; }

        public byte[] Data { get; private set; }

        public int Register { get; private set; }

        internal int ReferenceCount { get; set; }

        public override int GetHashCode()
        {
            return Tuple.Create(this.ResourceHash).GetHashCode();
        }
    }

    public class TextureManager : IDisposable
    {
        private const int StaticRegisterLimit = 4999;

        private readonly IResourceManager resourceManager;
        private readonly Device device;

        private readonly IDictionary<string, TextureReference> managedReferenceCache;
        private readonly IDictionary<TextureReference, ShaderResourceView> textureCache;
        private readonly IDictionary<int, TextureReference> textureRegister;

        private int nextRegister = StaticRegisterLimit + 1;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextureManager(IResourceManager resourceManager, Device device)
        {
            this.resourceManager = resourceManager;
            this.device = device;

            this.managedReferenceCache = new Dictionary<string, TextureReference>();
            this.textureCache = new Dictionary<TextureReference, ShaderResourceView>();
            this.textureRegister = new Dictionary<int, TextureReference>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TextureReference Fallback { get; set; }

        public void Dispose()
        {
            this.ClearCache();
        }

        public TextureReference RegisterStatic(ShaderResourceView view, int register)
        {
            this.CheckStaticRegister(register);

            var reference = new TextureReference(register, false);
            this.textureRegister.Add(register, reference);
            this.textureCache.Add(reference, view);
            return reference;
        }

        public TextureReference RegisterStatic(string hash, int register)
        {
            this.CheckStaticRegister(register);

            var reference = new TextureReference(hash, register);
            this.textureRegister.Add(register, reference);
            return reference;
        }

        public TextureReference RegisterStatic(byte[] data, int register)
        {
            this.CheckStaticRegister(register);

            var reference = new TextureReference(data, register);
            this.textureRegister.Add(register, reference);
            return reference;
        }

        public TextureReference Register(string hash)
        {
            // Check if we have a managed reference for this hash, during runtime this will not have to change since texture references hold no runtime info thats relevant
            if (this.managedReferenceCache.ContainsKey(hash))
            {
                return this.managedReferenceCache[hash];
            }

            var reference = new TextureReference(hash, this.nextRegister++);
            this.textureRegister.Add(reference.Register, reference);
            this.managedReferenceCache.Add(hash, reference);
            return reference;
        }

        public TextureReference Register(byte[] data)
        {
            var reference = new TextureReference(data, this.nextRegister++);
            this.textureRegister.Add(reference.Register, reference);
            return reference;
        }

        public void Unregister(int register)
        {
            if (!this.textureRegister.ContainsKey(register))
            {
                // This is perfectly fine since the same texture can be used plenty of times
                return;
            }

            TextureReference reference = this.textureRegister[register];
            this.textureRegister.Remove(register);

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

        public TextureReference GetRegisterReference(int register)
        {
            if (register > StaticRegisterLimit)
            {
                throw new InvalidOperationException("Can't get register reference outside of static range");
            }

            return new TextureReference(register, true);
        }
        
        public ShaderResourceView GetTexture(TextureReference reference)
        {
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
                this.textureCache[reference] = this.Load(textureReference);
            }

            reference.ReferenceCount++;
            return this.textureCache[textureReference];
        }

        public void ReleaseTexture(TextureReference reference)
        {
            if (reference.ReferenceCount > 0)
            {
                reference.ReferenceCount--;
            }
        }

        public void ClearUnclaimed()
        {
            lock (this.textureCache)
            {
                IList<TextureReference> clearList = this.textureCache.Keys.Where(reference => reference.ReferenceCount <= 0 && reference.Type != TextureReferenceType.View).ToList();

                for (int i = 0; i < clearList.Count; i++)
                {
                    this.textureCache[clearList[i]].Dispose();
                    this.textureCache.Remove(clearList[i]);
                }
            }
        }

        public void ClearCache()
        {
            lock (this.textureCache)
            {
                IList<TextureReference> clearQueue = new List<TextureReference>();
                foreach (TextureReference reference in this.textureCache.Keys)
                {
                    if (reference.Type == TextureReferenceType.View)
                    {
                        continue;
                    }

                    clearQueue.Add(reference);
                }

                foreach (TextureReference textureReference in clearQueue)
                {
                    this.textureCache[textureReference].Dispose();
                    this.textureCache.Remove(textureReference);
                }
            }
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

        private ShaderResourceView Load(TextureReference reference)
        {
            byte[] data = this.LoadData(reference);
            return ShaderResourceView.FromMemory(this.device, data);
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

                case TextureReferenceType.Memory:
                    {
                        if (reference.Data != null && reference.Data.Length > 0)
                        {
                            return reference.Data;
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
