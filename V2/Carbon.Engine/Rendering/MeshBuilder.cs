using System;
using System.Collections.Generic;
using System.IO;

using Carbon.Engine.Resource.Resources;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    /// <summary>
    /// Helper class to construct Mesh Structures from Vectors and indices
    /// Todo: 
    ///  - Check vertex duplication
    ///  - Generate Normals with MathExtension.CalculateSurfaceNormal()
    /// </summary>
    public class MeshBuilder
    {
        private readonly IList<MeshElement> pendingElements;

        private readonly IList<MeshElement> elements;
        private readonly IList<uint> elementIndices;

        private readonly string name;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MeshBuilder(string name)
        {
            this.name = name;

            this.IsIndexed = false;

            this.pendingElements = new List<MeshElement>();
            this.elements = new List<MeshElement>();
            this.elementIndices = new List<uint>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool DoubleSided { get; set; }
        public bool IsIndexed { get; set; }
        
        public void BeginPolygon()
        {
            if (this.IsIndexed)
            {
                throw new InvalidOperationException("Don't use Begin/End Polygon in indexed mode");
            }

            if (this.pendingElements.Count > 0)
            {
                throw new InvalidOperationException("BeginPolygon called with data still present, call EndPolygon before starting a new one!");
            }
        }

        public void EndPolygon()
        {
            if (this.IsIndexed)
            {
                throw new InvalidOperationException("Don't use Begin/End Polygon in indexed mode");
            }

            if (this.pendingElements.Count < 3)
            {
                throw new InvalidDataException("Polygon must have at least 3 vertices set!");
            }

            if (this.pendingElements.Count > 3)
            {
                this.elements.Add(this.pendingElements[0]);
                this.elements.Add(this.pendingElements[1]);
                this.elements.Add(this.pendingElements[3]);

                this.elements.Add(this.pendingElements[3]);
                this.elements.Add(this.pendingElements[1]);
                this.elements.Add(this.pendingElements[2]);

                if (this.DoubleSided)
                {
                    this.elements.Add(this.pendingElements[3]);
                    this.elements.Add(this.pendingElements[1]);
                    this.elements.Add(this.pendingElements[0]);

                    this.elements.Add(this.pendingElements[2]);
                    this.elements.Add(this.pendingElements[1]);
                    this.elements.Add(this.pendingElements[3]);
                }
            }
            else
            {
                for (int i = 0; i < this.pendingElements.Count; i++)
                {
                    this.elements.Add(this.pendingElements[i]);
                }
            }

            this.pendingElements.Clear();
        }

        public void DiscardCurrent()
        {
            this.pendingElements.Clear();
        }

        public void Clear()
        {
            this.pendingElements.Clear();
            this.elements.Clear();
        }

        public void AddVertex(Vector3 position, Vector3 normal, Vector2 texture)
        {
            this.pendingElements.Add(new MeshElement { Position = position, Normal = normal, Texture = texture });
        }

        public void AddIndices(uint[] indices)
        {
            if (!this.IsIndexed)
            {
                throw new InvalidOperationException("Mesh Builder is set to generate indices, can't add custom");
            }

            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] >= this.pendingElements.Count)
                {
                    throw new InvalidDataException("Index outside of element range "+indices[i]);
                }

                this.elementIndices.Add(indices[i]);
            }
        }

        public ModelResource ToMesh()
        {
            uint[] indexData;

            if (this.IsIndexed)
            {
                for (int i = 0; i < this.pendingElements.Count; i++)
                {
                    this.elements.Add(this.pendingElements[i]);
                }

                indexData = new uint[this.elementIndices.Count];
                for (int i = 0; i < this.elementIndices.Count; i++)
                {
                    indexData[i] = this.elementIndices[i];
                }

                return new ModelResource(this.elements, indexData) { Name = this.name };
            }

            if (this.pendingElements.Count > 0)
            {
                throw new InvalidOperationException("Elements are still pending, ToMesh call invalid");
            }

            indexData = new uint[this.elements.Count];
            for (uint i = 0; i < this.elements.Count; i++)
            {
                indexData[i] = i;
            }

            return new ModelResource(this.elements, indexData) { Name = this.name };
        }
    }
}
