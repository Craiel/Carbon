using System;
using System.Collections.Generic;

using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    public class Mesh : IDisposable
    {
        private readonly ModelResource resource;

        private readonly IDictionary<Type, DataContainer> uploadCache;
        private readonly StaticDataContainer<uint> indexUploadCache;

        private bool cacheInvalid;
        
        public Mesh(ModelResource resource)
        {
            this.resource = resource;

            this.uploadCache = new Dictionary<Type, DataContainer>();
            this.indexUploadCache = new StaticDataContainer<uint>();

            uint offset = 0;
            this.RefreshIndexUploadCache(ref offset);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name
        {
            get
            {
                return this.resource.Name;
            }
        }

        public bool AllowInstancing { get; set; }

        public int ElementCount
        {
            get
            {
                return this.resource.ElementCount;
            }
        }

        public int IndexCount
        {
            get
            {
                return this.resource.IndexCount;
            }
        }

        public int IndexSize
        {
            get
            {
                return this.resource.IndexSize;
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return this.resource.BoundingBox;
            }
        }

        public void Dispose()
        {
        }
        
        public int GetSizeAs<T>()
        {
            if (typeof(T) == typeof(PositionVertex))
            {
                return PositionVertex.Size * this.ElementCount;
            }

            if (typeof(T) == typeof(PositionNormalVertex))
            {
                return PositionNormalVertex.Size * this.ElementCount;
            }

            if (typeof(T) == typeof(PositionNormalTangentVertex))
            {
                return PositionNormalTangentVertex.Size * this.ElementCount;
            }

            throw new NotImplementedException("Format not implemented yet for " + this.Name);
        }

        public void WriteData<T>(DataStream target)
        {
            if (this.ElementCount == 0)
            {
                throw new InvalidOperationException("WriteData called on empty Mesh part " + this.Name);
            }
            
            if (!this.uploadCache.ContainsKey(typeof(T)) || this.cacheInvalid)
            {
                this.RefreshUploadCache(typeof(T));
            }

            this.uploadCache[typeof(T)].WriteData(target);
        }

        public void WriteIndexData(DataStream target)
        {
            this.indexUploadCache.WriteData(target);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private DataContainer SelectCacheContainer(Type type)
        {
            DataContainer container;
            if (this.uploadCache.ContainsKey(type))
            {
                container = this.uploadCache[type];
            }
            else
            {
                if (type == typeof(PositionVertex))
                {
                    container = new StaticDataContainer<PositionVertex>();
                }
                else if (type == typeof(PositionNormalVertex))
                {
                    container = new StaticDataContainer<PositionNormalVertex>();
                }
                else if (type == typeof(PositionNormalTangentVertex))
                {
                    container = new StaticDataContainer<PositionNormalTangentVertex>();
                }
                else
                {
                    throw new NotImplementedException("Vertex Format not implemented to write yet for " + this.Name);
                }

                this.uploadCache.Add(type, container);
            }

            return container;
        }

        private void RefreshUploadCache(Type type)
        {
            DataContainer container = this.SelectCacheContainer(type);

            container.Clear();

            bool generateTangents = type == typeof(PositionNormalTangentVertex);
            if (generateTangents)
            {
                this.resource.CalculateTangents();
            }

            for (int i = 0; i < this.resource.Elements.Count; i++)
            {
                MeshElement element = this.resource.Elements[i];
                if (type == typeof(PositionVertex))
                {
                    container.Add(new PositionVertex { Position = element.Position });
                }
                else if (type == typeof(PositionNormalVertex))
                {
                    container.Add(
                        new PositionNormalVertex
                        {
                            Position = element.Position,
                            Normal = element.Normal ?? Vector3.Zero,
                            Texture = element.Texture ?? Vector2.Zero
                        });
                }
                else if (type == typeof(PositionNormalTangentVertex))
                {
                    container.Add(
                        new PositionNormalTangentVertex
                        {
                            Position = element.Position,
                            Normal = element.Normal ?? Vector3.Zero,
                            Texture = element.Texture ?? Vector2.Zero,
                            Tangent = element.Tangent ?? Vector4.Zero
                        });
                }
            }

            this.cacheInvalid = false;
        }

        private void RefreshIndexUploadCache(ref uint offset)
        {
            for (int i = 0; i < this.resource.Indices.Length; i++)
            {
                this.indexUploadCache.Add(this.resource.Indices[i] + offset);
            }
        }
    }
}
