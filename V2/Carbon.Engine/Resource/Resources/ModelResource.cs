using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using Carbon.Engine.Logic;

using Core.Utils;

using SlimDX;

namespace Carbon.Engine.Resource.Resources
{
    public class MeshElement : ResourceBase
    {
        public Vector3 Position { get; set; }
        public Vector3? Normal { get; set; }
        public Vector2? Texture { get; set; }
        public Vector4? Tangent { get; set; }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            bool hasNormal = source.ReadBoolean();
            bool hasTexture = source.ReadBoolean();
            bool hasTangent = source.ReadBoolean();

            this.Position = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };

            if (hasNormal)
            {
                this.Normal = new Vector3 { X = source.ReadSingle(), Y = source.ReadSingle(), Z = source.ReadSingle() };
            }

            if (hasTexture)
            {
                this.Texture = new Vector2 { X = source.ReadSingle(), Y = source.ReadSingle() };
            }

            if (hasTangent)
            {
                this.Tangent = new Vector4
                    {
                        X = source.ReadSingle(),
                        Y = source.ReadSingle(),
                        Z = source.ReadSingle(),
                        W = source.ReadSingle()
                    };
            }
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Normal != null);
            target.Write(this.Texture != null);
            target.Write(this.Tangent != null);

            target.Write(Position.X);
            target.Write(Position.Y);
            target.Write(Position.Z);

            if (Normal != null)
            {
                target.Write(Normal.Value.X);
                target.Write(Normal.Value.Y);
                target.Write(Normal.Value.Z);
            }

            if (Texture != null)
            {
                target.Write(Texture.Value.X);
                target.Write(Texture.Value.Y);
            }

            if (Tangent != null)
            {
                target.Write(Tangent.Value.X);
                target.Write(Tangent.Value.Y);
                target.Write(Tangent.Value.Z);
                target.Write(Tangent.Value.W);
            }
        }
    }

    public class MaterialElement : ResourceBase
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }
        public string DiffuseTexture { get; set; }
        public string NormalTexture { get; set; }
        public string AlphaTexture { get; set; }
        public string SpecularTexture { get; set; }

        public MaterialElement Clone()
        {
            return new MaterialElement
                       {
                           Name = this.Name,
                           DiffuseTexture = this.DiffuseTexture,
                           NormalTexture = this.NormalTexture,
                           AlphaTexture = this.AlphaTexture,
                           SpecularTexture = this.SpecularTexture
                       };
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.Name = source.ReadString();
            this.DiffuseTexture = source.ReadString();
            this.NormalTexture = source.ReadString();
            this.AlphaTexture = source.ReadString();
            this.SpecularTexture = source.ReadString();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.Name);
            target.Write(this.DiffuseTexture);
            target.Write(this.NormalTexture);
            target.Write(this.AlphaTexture);
            target.Write(this.SpecularTexture);
        }
    }

    public class ModelResource : ResourceBase
    {
        private const int Version = 1;

        private readonly List<ModelResource> subParts;
        private readonly List<MeshElement> elements;
        private readonly List<MaterialElement> materials;

        private uint[] indices;

        private bool tangentsCalculated;

        public ModelResource(IEnumerable<MeshElement> elements, uint[] indices)
        {
            this.elements = new List<MeshElement>(elements);
            this.subParts = new List<ModelResource>();
            this.materials = new List<MaterialElement>();
            this.indices = indices;

            this.ElementCount = this.elements.Count;
            this.IndexCount = this.indices.Length;
            this.IndexSize = this.IndexCount * 4;
        }

        public ModelResource(IEnumerable<ModelResource> parts)
        {
            this.elements = new List<MeshElement>();
            this.indices = new uint[0];
            this.subParts = new List<ModelResource>(parts);
            this.materials = new List<MaterialElement>();

            foreach (ModelResource part in this.subParts)
            {
                this.ElementCount += part.ElementCount;
                this.IndexCount += part.IndexCount;
                this.IndexSize += part.IndexSize;
            }
        }

        public ModelResource()
        {
            this.subParts = new List<ModelResource>();
            this.elements = new List<MeshElement>();
            this.materials = new List<MaterialElement>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public Vector3 Offset { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public int ElementCount { get; private set; }
        public int IndexCount { get; private set; }
        public int IndexSize { get; private set; }

        public BoundingBox BoundingBox { get; private set; }

        public ReadOnlyCollection<ModelResource> SubParts
        {
            get
            {
                return this.subParts.AsReadOnly();
            }
        }

        public ReadOnlyCollection<MeshElement> Elements
        {
            get
            {
                return this.elements.AsReadOnly();
            }
        }

        public ReadOnlyCollection<MaterialElement> Materials
        {
            get
            {
                return this.materials.AsReadOnly();
            }
        }

        public uint[] Indices
        {
            get
            {
                return this.indices;
            }
        }

        public void CalculateTangents()
        {
            if (this.tangentsCalculated)
            {
                return;
            }

            this.DoCalculateTangents();
            foreach (ModelResource part in SubParts)
            {
                part.DoCalculateTangents();
            }

            this.tangentsCalculated = true;
        }

        public void AddPart(ModelResource part)
        {
            this.subParts.Add(part);

            this.ElementCount += part.ElementCount;
            this.IndexCount += part.IndexCount;
            this.IndexSize += part.IndexSize;
        }

        public void AddMaterial(MaterialElement material)
        {
            this.materials.Add(material);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(Version);
            target.Write(this.Name);

            target.Write(this.IndexCount);
            target.Write(this.ElementCount);
            target.Write(this.tangentsCalculated);

            target.Write(this.Offset.X);
            target.Write(this.Offset.Y);
            target.Write(this.Offset.Z);

            target.Write(this.Scale.X);
            target.Write(this.Scale.Y);
            target.Write(this.Scale.Z);

            target.Write(this.Rotation.X);
            target.Write(this.Rotation.Y);
            target.Write(this.Rotation.Z);
            target.Write(this.Rotation.W);

            target.Write(this.BoundingBox.Minimum.X);
            target.Write(this.BoundingBox.Minimum.Y);
            target.Write(this.BoundingBox.Minimum.Z);

            target.Write(this.BoundingBox.Maximum.X);
            target.Write(this.BoundingBox.Maximum.Y);
            target.Write(this.BoundingBox.Maximum.Z);

            target.Write(this.elements.Count);
            for (int i = 0; i < this.elements.Count; i++)
            {
                this.elements[i].Save(target);
            }

            target.Write(this.indices.Length);
            for (int i = 0; i < this.indices.Length; i++)
            {
                target.Write(this.indices[i]);
            }

            target.Write(this.materials.Count);
            for (int i = 0; i < this.materials.Count; i++)
            {
                this.materials[i].Save(target);
            }

            target.Write(this.subParts.Count);
            for (int i = 0; i < this.subParts.Count; i++)
            {
                this.subParts[i].Save(target);
            }
        }

        protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.indices = null;
            this.elements.Clear();
            this.subParts.Clear();
            this.materials.Clear();
            
            int version = source.ReadInt();
            if (version != Version)
            {
                throw new InvalidDataException("Mesh version is not supported: " + version);
            }

            this.Name = source.ReadString();
            this.IndexCount = source.ReadInt();
            this.IndexSize = this.IndexCount * 4;
            this.ElementCount = source.ReadInt();
            this.tangentsCalculated = source.ReadBoolean();

            this.Offset = new Vector3
            {
                X = source.ReadSingle(),
                Y = source.ReadSingle(),
                Z = source.ReadSingle()
            };

            this.Scale = new Vector3
            {
                X = source.ReadSingle(),
                Y = source.ReadSingle(),
                Z = source.ReadSingle()
            };

            this.Rotation = new Quaternion(
                source.ReadSingle(), source.ReadSingle(), source.ReadSingle(), source.ReadSingle());

            Vector3 boundingMin = new Vector3
                {
                    X = source.ReadSingle(),
                    Y = source.ReadSingle(),
                    Z = source.ReadSingle()
                };

            Vector3 boundingMax = new Vector3
                {
                    X = source.ReadSingle(),
                    Y = source.ReadSingle(),
                    Z = source.ReadSingle()
                };

            this.BoundingBox = new BoundingBox(boundingMin, boundingMax);

            int entries = source.ReadInt();
            for (int i = 0; i < entries; i++)
            {
                var element = new MeshElement();
                element.Load(source);
                this.elements.Add(element);
            }

            entries = source.ReadInt();
            this.indices = new uint[entries];
            for (int i = 0; i < entries; i++)
            {
                this.indices[i] = source.ReadUInt();
            }

            entries = source.ReadInt();
            for (int i = 0; i < entries; i++)
            {
                var element = new MaterialElement();
                element.Load(source);
                this.materials.Add(element);
            }

            entries = source.ReadInt();
            for (int i = 0; i < entries; i++)
            {
                var element = new ModelResource();
                element.Load(source);
                this.subParts.Add(element);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoCalculateTangents()
        {
            if (this.indices == null || this.indices.Length <= 0)
            {
                return;
            }

            var tan1 = new Vector3[this.elements.Count];
            var tan2 = new Vector3[this.elements.Count];
            int triangleCount = this.IndexCount;
            for (int i = 0; i < triangleCount; i += 3)
            {
                uint i1 = this.indices[i];
                uint i2 = this.indices[i + 1];
                uint i3 = this.indices[i + 2];

                MeshElement e1 = this.elements[(int)i1];
                MeshElement e2 = this.elements[(int)i2];
                MeshElement e3 = this.elements[(int)i3];

                float x1 = e2.Position.X - e1.Position.X;
                float x2 = e3.Position.X - e1.Position.X;
                float y1 = e2.Position.Y - e1.Position.Y;
                float y2 = e3.Position.Y - e1.Position.Y;
                float z1 = e2.Position.Z - e1.Position.Z;
                float z2 = e3.Position.Z - e1.Position.Z;

                float s1 = e2.Texture.Value.X - e1.Texture.Value.X;
                float s2 = e3.Texture.Value.X - e1.Texture.Value.X;
                float t1 = e2.Texture.Value.Y - e1.Texture.Value.Y;
                float t2 = e3.Texture.Value.Y - e1.Texture.Value.Y;

                float r = 1.0f / ((s1 * t2) - (s2 * t1));

                Vector3 sdir = new Vector3(
                    ((t2 * x1) - (t1 * x2)) * r, ((t2 * y1) - (t1 * y2)) * r, ((t2 * z1) - (t1 * z2)) * r);
                Vector3 tdir = new Vector3(
                    ((s1 * x2) - (s2 * x1)) * r, ((s1 * y2) - (s2 * y1)) * r, ((s1 * z2) - (s2 * z1)) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (int i = 0; i < this.elements.Count; ++i)
            {
                if (elements[i].Normal == null)
                {
                    // Todo: throw
                    continue;
                }

                MeshElement element = this.elements[i];
                Vector3 n = element.Normal.Value;
                Vector3 t = tan1[i];

                MathExtension.OrthoNormalize(ref n, ref t);

                float w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
                this.elements[i].Tangent = new Vector4(tan1[i], w);
            }
        }
    }
}
