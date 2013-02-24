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
        private static readonly SHA1 HashProvider = SHA1.Create();

        private readonly IList<ResourceContent> content;

        private readonly Hashtable cache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceManager(string root)
        {
            this.cache = new Hashtable();
            this.content = new List<ResourceContent> { new FolderContent(root, true) };
        }

        public static string BuildResourceHash(string path)
        {
            byte[] hashData = HashProvider.ComputeHash(Encoding.UTF8.GetBytes(path));
            return Convert.ToBase64String(hashData);
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
                        T resource = (T)Activator.CreateInstance(typeof(T), new[] { dataStream });
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
                    if (!this.cache.ContainsKey(hash))
                    {
                        this.cache.Add(hash, resource);
                    }

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
