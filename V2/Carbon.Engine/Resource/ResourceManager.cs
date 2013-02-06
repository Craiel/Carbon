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
        public T Load<T>(ref ResourceLink link) where T : ICarbonResource
        {
            System.Diagnostics.Trace.TraceWarning("Loading Resource {0}", link);
            this.ProcessResourceLink(ref link);

            if (!this.cache.ContainsKey(link.Hash))
            {
                for (int i = 0; i < this.content.Count; i++)
                {
                    ResourceContent resourceContent = this.content[i];
                    Stream dataStream = resourceContent.Load(link.Hash, link.Source);
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

        public void Store(ref ResourceLink link, ICarbonResource resource)
        {
            System.Diagnostics.Trace.TraceWarning("Storing Resource {0} ({1})", link, resource.GetType());
            this.ProcessResourceLink(ref link);

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

        public void Replace(ref ResourceLink link, ICarbonResource resource)
        {
            this.ProcessResourceLink(ref link);
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

        public void StoreOrReplace(ref ResourceLink link, ICarbonResource resource)
        {
            this.ProcessResourceLink(ref link);
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

        private void ProcessResourceLink(ref ResourceLink link)
        {
            if (link.Hash == null && string.IsNullOrEmpty(link.Source))
            {
                throw new ArgumentException("Resource link data is invalid, source has to be supplied");
            }

            if (link.Hash == null)
            {
                byte[] hashData = this.hashProvider.ComputeHash(Encoding.UTF8.GetBytes(link.Source));
                link.Hash = Convert.ToBase64String(hashData);
            }
        }
    }
}
