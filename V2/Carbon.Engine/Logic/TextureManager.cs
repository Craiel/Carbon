using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Resources;

using Core.Utils.Contracts;

using SlimDX.Direct3D11;

namespace Carbon.Engine.Logic
{
    using Carbon.Engine.Resource.Content;

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

        internal TextureReference(ResourceLink resource, int register)
        {
            this.Resource = resource;
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

        public ResourceLink Resource { get; private set; }

        public byte[] Data { get; private set; }

        public int Register { get; private set; }

        internal int ReferenceCount { get; set; }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Resource).GetHashCode();
        }
    }

    public class TextureManager : IDisposable
    {
        private const int StaticRegisterLimit = 4999;

        private readonly ILog log;
        private readonly IResourceManager resourceManager;
        private readonly Device device;

        private readonly IDictionary<TextureReference, ShaderResourceView> textureCache;
        private readonly IDictionary<int, TextureReference> textureRegister;

        private int nextRegister = StaticRegisterLimit + 1;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TextureManager(IEngineFactory factory, Device device)
        {
            this.resourceManager = factory.Get<IResourceManager>();
            this.device = device;
            this.log = factory.Get<IEngineLog>().AquireContextLog("TextureManager");

            this.textureCache = new Dictionary<TextureReference, ShaderResourceView>();
            this.textureRegister = new Dictionary<int, TextureReference>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TextureReference Fallback { get; set; }

        public void Dispose()
        {
        }

        public TextureReference RegisterStatic(ShaderResourceView view, int register)
        {
            this.CheckStaticRegister(register);

            var reference = new TextureReference(register, false);
            this.textureRegister.Add(register, reference);
            this.textureCache.Add(reference, view);
            return reference;
        }

        public TextureReference RegisterStatic(ResourceLink resource, int register)
        {
            this.CheckStaticRegister(register);

            var reference = new TextureReference(resource, register);
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

        public TextureReference Register(ResourceLink resource)
        {
            var reference = new TextureReference(resource, this.nextRegister);
            this.textureRegister.Add(reference.Register, reference);
            this.nextRegister++;
            return reference;
        }

        public TextureReference Register(byte[] data)
        {
            var reference = new TextureReference(data, this.nextRegister);
            this.textureRegister.Add(reference.Register, reference);
            this.nextRegister++;
            return reference;
        }

        public void Unregister(int register)
        {
            if (!this.textureRegister.ContainsKey(register))
            {
                throw new InvalidDataException("No texture in register " + register);
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
                throw new ArgumentException(string.Format("Static register is already taken ({0}) by {1}", register, this.textureRegister[register].Resource));
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
                        var res = reference.Resource;
                        var textureResource = this.resourceManager.Load<RawResource>(ref res);
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
                this.log.Warning("Loading fallback for {0}", reference);
                return this.LoadData(this.Fallback);
            }

            throw new InvalidDataException("Texture could not be loaded and no fallback is present");
        }
    }
}
