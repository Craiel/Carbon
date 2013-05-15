using System;
using System.Collections.Generic;

using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    using Carbon.Engine.Resource.Resources.Model;

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
        
        public int GetSizeAs(Type type)
        {
            return InputStructures.InputLayoutSizes[type] * this.ElementCount;
        }

        public int GetSizeAs<T>()
            where T : IMeshStructure
        {
            return this.GetSizeAs(typeof(T));
        }

        public void WriteData(Type type, DataStream target)
        {
            if (this.ElementCount == 0)
            {
                throw new InvalidOperationException("WriteData called on empty Mesh part " + this.Name);
            }

            if (!this.uploadCache.ContainsKey(type) || this.cacheInvalid)
            {
                this.RefreshUploadCache(type);
            }

            this.uploadCache[type].WriteData(target);
        }

        public void WriteData<T>(DataStream target)
        {
            this.WriteData(typeof(T), target);
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
                else if (type == typeof(PositionColorVertex))
                {
                    container = new StaticDataContainer<PositionColorVertex>();
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
                ModelMeshElement element = this.resource.Elements[i];
                if (type == typeof(PositionVertex))
                {
                    container.Add(new PositionVertex { Position = element.Position });
                }
                else if (type == typeof(PositionColorVertex))
                {
                    container.Add(
                        new PositionColorVertex
                            {
                                Position = element.Position,
                                Texture = element.Texture ?? Vector2.Zero,
                                Color = element.Color ?? Vector4.Zero
                            });
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
