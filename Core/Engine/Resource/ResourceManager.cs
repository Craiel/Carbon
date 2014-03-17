namespace Core.Engine.Resource
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Utils.IO;

    using Core.Engine.Contracts.Resource;
    
    public class ResourceManager : IResourceManager
    {
        private readonly IList<ResourceContent> content;

        private readonly IDictionary<string, ICarbonResource> cache;
        private readonly IDictionary<string, ResourceInfo> infoCache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceManager(CarbonDirectory root)
        {
            this.content = new List<ResourceContent> { new FolderContent(root, true) };

            this.cache = new Dictionary<string, ICarbonResource>();
            this.infoCache = new Dictionary<string, ResourceInfo>();
        }

        public void Dispose()
        {
            // Todo
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Load<T>(string hash) where T : ICarbonResource
        {
            System.Diagnostics.Trace.TraceWarning("Loading Resource {0}", hash);
            if (!this.cache.ContainsKey(hash))
            {
                for (int i = 0; i < this.content.Count; i++)
                {
                    ResourceContent resourceContent = this.content[i];
                    Stream dataStream = resourceContent.Load(hash);
                    if (dataStream != null)
                    {
                        dataStream.Position = 0;
                        var resource = (T)Activator.CreateInstance(typeof(T));
                        resource.Load(dataStream);
                        this.cache.Add(hash, resource);
                        return resource;
                    }
                }

                System.Diagnostics.Trace.TraceWarning("Resource not found {0}", hash);
                return default(T);
            }

            if (this.cache[hash].GetType() != typeof(T))
            {
                System.Diagnostics.Trace.TraceError("Resource was not in the requested format, was {0} but expected {1}", this.cache.GetType(), typeof(T));
                return default(T);
            }

            return (T)this.cache[hash];
        }

        public void Store(string hash, ICarbonResource resource)
        {
            System.Diagnostics.Trace.TraceWarning("Storing Resource {0} ({1})", hash, resource.GetType());
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Store(hash, resource))
                {
                    this.cache.Add(hash, resource);
                    return;
                }
            }
        }

        public void Replace(string hash, ICarbonResource resource)
        {
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Replace(hash, resource))
                {
                    this.UpdateCache(hash, resource);
                    return;
                }
            }

            throw new InvalidDataException("Resource could not be replaced, no existing resource was found");
        }

        public void StoreOrReplace(string hash, ICarbonResource resource)
        {
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Replace(hash, resource))
                {
                    this.UpdateCache(hash, resource);
                    return;
                }
            }

            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Store(hash, resource))
                {
                    this.cache.Add(hash, resource);
                    return;
                }
            }

            throw new InvalidDataException("Resource could not be stored");
        }

        public void Delete(string hash)
        {
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Delete(hash))
                {
                    if (this.cache.ContainsKey(hash))
                    {
                        this.cache.Remove(hash);
                    }

                    if (this.infoCache.ContainsKey(hash))
                    {
                        this.infoCache.Remove(hash);
                    }

                    return;
                }
            }

            System.Diagnostics.Trace.TraceWarning("Resource could not be deleted, no existing resource was found");
        }

        public ResourceInfo GetInfo(string hash)
        {
            if (this.infoCache.ContainsKey(hash))
            {
                return this.infoCache[hash];
            }

            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                ResourceInfo info = resourceContent.GetInfo(hash);
                if (info != null)
                {
                    lock (this.infoCache)
                    {
                        this.infoCache.Add(hash, info);
                    }

                    return info;
                }
            }

            return null;
        }

        public void ClearCache()
        {
            foreach (ICarbonResource carbonResource in this.cache.Values)
            {
                carbonResource.Dispose();
            }

            this.cache.Clear();
            this.infoCache.Clear();
        }

        public void AddContent(ResourceContent newContent)
        {
            if (this.content.Contains(newContent))
            {
                throw new InvalidOperationException("Content was already added");
            }

            this.content.Add(newContent);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateCache(string hash, ICarbonResource resource)
        {
            if (!this.cache.ContainsKey(hash))
            {
                this.cache.Add(hash, resource);
            }
            else
            {
                if (this.cache[hash] == resource)
                {
                    return;
                }

                this.cache[hash].Dispose();
                this.cache[hash] = resource;
                if (this.infoCache.ContainsKey(hash))
                {
                    this.infoCache.Remove(hash);
                }
            }
        }
    }
}
