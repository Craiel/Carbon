namespace Core.Engine.Rendering
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Resource.Resources.Model;

    using SharpDX;

    /* Todo:
     - this will only deal with a single resource
     - sub parts will be evaluated before and put into a container mesh holder which will deal with transformations and rendering
     - Merge meshes with same material together
    */
    public class Mesh : IDisposable
    {
        private readonly ModelResource resourceReference; 
 
        private readonly IDictionary<Type, DataContainer> uploadCache;
        private readonly StaticDataContainer<uint> indexUploadCache;

        private bool cacheInvalid;
        
        public Mesh(ModelResource resource)
        {
            this.resourceReference = resource;

            this.Name = resource.Name;

            this.uploadCache = new Dictionary<Type, DataContainer>();
            this.indexUploadCache = new StaticDataContainer<uint>();

            this.IndexCount = resource.Indices.Count;
            this.IndexSize = this.IndexCount * sizeof(int);
            this.ElementCount = resource.Elements.Count;

            for (int i = 0; i < resource.Indices.Count; i++)
            {
                this.indexUploadCache.Add(resource.Indices[i]);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public bool AllowInstancing { get; set; }

        public int IndexCount { get; private set; }

        public int IndexSize { get; private set; }

        public int ElementCount { get; private set; }

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
                throw new InvalidOperationException("WriteData called on empty Mesh " + this.Name);
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
            ModelResource resource = this.resourceReference;

            // Todo:
            /*if (!this.resourceReference.TryGetTarget(out resource))
            {
                throw new InvalidOperationException("Model resource is no longer valid");
            }*/

            DataContainer container = this.SelectCacheContainer(type);

            container.Clear();

            bool generateTangents = type == typeof(PositionNormalTangentVertex);
            if (generateTangents && !resource.HasTangents)
            {
                resource.CalculateTangents();
            }

            for (int i = 0; i < this.ElementCount; i++)
            {
                ModelResourceElement element = resource.Elements[i];
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
    }
}
