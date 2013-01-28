using System;
using System.Collections.Generic;

using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    public class Mesh
    {
        private readonly MeshResource resource;

        private readonly IDictionary<Type, DataContainer> uploadCache;
        private readonly StaticDataContainer<uint> indexUploadCache;

        private bool cacheInvalid;
        
        public Mesh(MeshResource resource)
        {
            this.resource = resource;

            this.uploadCache = new Dictionary<Type, DataContainer>();
            this.indexUploadCache = new StaticDataContainer<uint>();

            uint offset = 0;
            this.RefreshIndexUploadCache(resource, ref offset);
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
                this.RefreshUploadCache(typeof(T), this.resource);
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

        private void RefreshUploadCache(Type type, MeshResource part)
        {
            DataContainer container = this.SelectCacheContainer(type);

            container.Clear();

            bool generateTangents = type == typeof(PositionNormalTangentVertex);
            this.AddUploadCache(container, type, part, generateTangents);

            this.cacheInvalid = false;
        }

        private void AddUploadCache(DataContainer target, Type type, MeshResource part, bool generateTangents)
        {
            if (generateTangents)
            {
                part.CalculateTangents();
            }

            for (int i = 0; i < part.Elements.Count; i++)
            {
                MeshElement element = part.Elements[i];
                if (type == typeof(PositionVertex))
                {
                    target.Add(new PositionVertex { Position = element.Position });
                }
                else if (type == typeof(PositionNormalVertex))
                {
                    target.Add(
                        new PositionNormalVertex
                        {
                            Position = element.Position,
                            Normal = element.Normal ?? Vector3.Zero,
                            Texture = element.Texture ?? Vector2.Zero
                        });
                }
                else if (type == typeof(PositionNormalTangentVertex))
                {
                    target.Add(
                        new PositionNormalTangentVertex
                        {
                            Position = element.Position,
                            Normal = element.Normal ?? Vector3.Zero,
                            Texture = element.Texture ?? Vector2.Zero,
                            Tangent = element.Tangent ?? Vector4.Zero
                        });
                }
            }

            for (int p = 0; p < part.SubParts.Count; p++)
            {
                this.AddUploadCache(target, type, part.SubParts[p], generateTangents);
            }
        }

        private void RefreshIndexUploadCache(MeshResource part, ref uint offset)
        {
            for (int i = 0; i < part.Indices.Length; i++)
            {
                this.indexUploadCache.Add(resource.Indices[i] + offset);
            }

            for (int p = 0; p < part.SubParts.Count; p++)
            {
                offset += (uint)part.ElementCount;
                this.RefreshIndexUploadCache(part.SubParts[p], ref offset);
            }
        }
    }
}
