using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public abstract class ResourceContent
    {
        public abstract Stream Load(string hash, string key);
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
        public ResourceManager()
        {
            this.hashProvider = SHA1.Create();
            this.cache = new Hashtable();
            this.content = new List<ResourceContent> { new FolderContent("Data", true) };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Load<T>(string key) where T : ICarbonResource
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key can not be null or emtpy!");
            }

            System.Diagnostics.Trace.TraceWarning("Loading Resource {0}", key);
            byte[] hashData = this.hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            string hash = Convert.ToBase64String(hashData);
            if (!this.cache.ContainsKey(hash))
            {
                for (int i = 0; i < this.content.Count; i++)
                {
                    ResourceContent resourceContent = this.content[i];
                    Stream dataStream = resourceContent.Load(hash, key);
                    if (dataStream != null)
                    {
                        dataStream.Position = 0;
                        T resource = (T)Activator.CreateInstance(typeof(T), new[] { dataStream });
                        this.cache.Add(hash, resource);
                        return resource;
                    }
                }

                System.Diagnostics.Trace.TraceWarning("Resource not found {0}", key);
                return default(T);
            }

            if (this.cache[hash].GetType() != typeof(T))
            {
                System.Diagnostics.Trace.TraceError("Resource was not in the requested format, was {0} but expected {1}", this.cache.GetType(), typeof(T));
                return default(T);
            }

            return (T)this.cache[hash];
        }

        public void Store(string key, ICarbonResource resource)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key can not be null or emtpy!");
            }

            System.Diagnostics.Trace.TraceWarning("Storing Resource {0} ({1})", key, resource.GetType());
            byte[] hashData = this.hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            string hash = Convert.ToBase64String(hashData);
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

        public void Replace(string key, ICarbonResource resource)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key can not be null or emtpy!");
            }

            byte[] hashData = this.hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            string hash = Convert.ToBase64String(hashData);
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Replace(hash, resource))
                {
                    if (!this.cache.ContainsKey(hash))
                    {
                        this.cache.Add(hash, resource);
                    }

                    return;
                }
            }

            throw new InvalidDataException("Resource could not be replaced, no existing resource was found");
        }

        public void StoreOrReplace(string key, ICarbonResource resource)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key can not be null or empty!");
            }

            byte[] hashData = this.hashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            string hash = Convert.ToBase64String(hashData);
            for (int i = 0; i < this.content.Count; i++)
            {
                ResourceContent resourceContent = this.content[i];
                if (resourceContent.Replace(hash, resource))
                {
                    if (!this.cache.ContainsKey(hash))
                    {
                        this.cache.Add(hash, resource);
                    }

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
