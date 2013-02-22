using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    using Carbon.Engine.Resource.Content;

    public abstract class ResourceContent
    {
        public abstract Stream Load(string hash);
        public abstract bool Store(string hash, ICarbonResource data);
        public abstract bool Replace(string hash, ICarbonResource data);
    }

    public class ResourceManager : IResourceManager
    {
        private readonly IList<ResourceContent> content;
        
        private readonly SHA1 hashProvider;

        private readonly Hashtable cache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceManager(string root)
        {
            this.hashProvider = SHA1.Create();
            this.cache = new Hashtable();
            this.content = new List<ResourceContent> { new FolderContent(root, true) };
        }

        public void Dispose()
        {
            // Todo
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceLink GetLink(string path)
        {
            byte[] hashData = this.hashProvider.ComputeHash(Encoding.UTF8.GetBytes(path));
            var link = new ResourceLink { Hash = Convert.ToBase64String(hashData) };
            return link;
        }

        public T Load<T>(ResourceLink link) where T : ICarbonResource
        {
            System.Diagnostics.Trace.TraceWarning("Loading Resource {0}", link);
            if (!this.cache.ContainsKey(link.Hash))
            {
                for (int i = 0; i < this.content.Count; i++)
                {
                    ResourceContent resourceContent = this.content[i];
                    Stream dataStream = resourceContent.Load(link.Hash);
                    if (dataStream != null)
                    {
                        dataStream.Position = 0;
                        T resource = (T)Activator.CreateInstance(typeof(T), new[] { dataStream });
                        this.cache.Add(link.Hash, resource);
                        return resource;
                    }
                }

                System.Diagnostics.Trace.TraceWarning("Resource not found {0}", link);
                return default(T);
            }

            if (this.cache[link.Hash].GetType() != typeof(T))
            {
                System.Diagnostics.Trace.TraceError("Resource was not in the requested format, was {0} but expected {1}", this.cache.GetType(), typeof(T));
                return default(T);
            }

            return (T)this.cache[link.Hash];
        }

        public void Store(ResourceLink link, ICarbonResource resource)
        {
            System.Diagnostics.Trace.TraceWarning("Storing Resource {0} ({1})", link, resource.GetType());
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Store(link.Hash, resource))
                {
                    this.cache.Add(link.Hash, resource);
                    return;
                }
            }
        }

        public void Replace(ResourceLink link, ICarbonResource resource)
        {
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Replace(link.Hash, resource))
                {
                    if (!this.cache.ContainsKey(link.Hash))
                    {
                        this.cache.Add(link.Hash, resource);
                    }

                    return;
                }
            }

            throw new InvalidDataException("Resource could not be replaced, no existing resource was found");
        }

        public void StoreOrReplace(ResourceLink link, ICarbonResource resource)
        {
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Replace(link.Hash, resource))
                {
                    if (!this.cache.ContainsKey(link.Hash))
                    {
                        this.cache.Add(link.Hash, resource);
                    }

                    return;
                }
            }

            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Store(link.Hash, resource))
                {
                    this.cache.Add(link.Hash, resource);
                    return;
                }
            }

            throw new InvalidDataException("Resource could not be stored");
        }

        public void Clear()
        {
            // Todo: Unload all resources and clear caches
            this.content.Clear();
        }

        public void AddContent(ResourceContent newContent)
        {
            if (this.content.Contains(newContent))
            {
                throw new InvalidOperationException("Content was already added");
            }

            this.content.Add(newContent);
        }
    }
}
