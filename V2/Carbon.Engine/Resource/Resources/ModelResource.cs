using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using Carbon.Engine.Contracts.Resource;

using Core.Utils;

using SlimDX;

namespace Carbon.Engine.Resource.Resources
{
    public class MeshElement
    {
        public MeshElement()
        {
        }

        public MeshElement(Stream source)
            : this()
        {
            using (var reader = new BinaryReader(source))
            {
                bool hasNormal = reader.ReadBoolean();
                bool hasTexture = reader.ReadBoolean();
                bool hasTangent = reader.ReadBoolean();

                this.Position = new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() };

                if (hasNormal)
                {
                    this.Normal = new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() };
                }

                if (hasTexture)
                {
                    this.Texture = new Vector2 { X = reader.ReadSingle(), Y = reader.ReadSingle() };
                }

                if (hasTangent)
                {
                    this.Tangent = new Vector4
                        {
                            X = reader.ReadSingle(),
                            Y = reader.ReadSingle(),
                            Z = reader.ReadSingle(),
                            W = reader.ReadSingle()
                        };
                }
            }
        }

        public Vector3 Position { get; set; }
        public Vector3? Normal { get; set; }
        public Vector2? Texture { get; set; }
        public Vector4? Tangent { get; set; }

        public void Save(Stream target)
        {
            using (var writer = new BinaryWriter(target))
            {
                writer.Write(this.Normal != null);
                writer.Write(this.Texture != null);
                writer.Write(this.Tangent != null);

                writer.Write(Position.X);
                writer.Write(Position.Y);
                writer.Write(Position.Z);

                if (Normal != null)
                {
                    writer.Write(Normal.Value.X);
                    writer.Write(Normal.Value.Y);
                    writer.Write(Normal.Value.Z);
                }

                if (Texture != null)
                {
                    writer.Write(Texture.Value.X);
                    writer.Write(Texture.Value.Y);
                }

                if (Tangent != null)
                {
                    writer.Write(Tangent.Value.X);
                    writer.Write(Tangent.Value.Y);
                    writer.Write(Tangent.Value.Z);
                    writer.Write(Tangent.Value.W);
                }
            }
        }
    }

    public class MaterialElement
    {
        public MaterialElement()
        {
        }

        public MaterialElement(Stream source)
            : this()
        {
            using (var reader = new BinaryReader(source))
            {
                this.Name = reader.ReadString();
            }
        }

        public string Name { get; set; }

        public void Save(Stream target)
        {
            using (var writer = new BinaryWriter(target))
            {
                writer.Write(this.Name);
            }
        }
    }

    public class ModelResource : ICarbonResource
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

        public ModelResource(Stream source)
        {
            this.subParts = new List<ModelResource>();
            this.elements = new List<MeshElement>();
            this.materials = new List<MaterialElement>();

            this.Load(source);
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
            this.tangentsCalculated = true;
        }
        
        public long Save(Stream target)
        {
            long size;
            using (var dataStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(dataStream))
                {
                    writer.Write(Version);
                    writer.Write(this.Name);

                    writer.Write(this.IndexCount);
                    writer.Write(this.ElementCount);
                    writer.Write(this.tangentsCalculated);

                    writer.Write(this.Offset.X);
                    writer.Write(this.Offset.Y);
                    writer.Write(this.Offset.Z);

                    writer.Write(this.Scale.X);
                    writer.Write(this.Scale.Y);
                    writer.Write(this.Scale.Z);

                    writer.Write(this.Rotation.X);
                    writer.Write(this.Rotation.Y);
                    writer.Write(this.Rotation.Z);
                    writer.Write(this.Rotation.W);
                    
                    writer.Write(this.BoundingBox.Minimum.X);
                    writer.Write(this.BoundingBox.Minimum.Y);
                    writer.Write(this.BoundingBox.Minimum.Z);

                    writer.Write(this.BoundingBox.Maximum.X);
                    writer.Write(this.BoundingBox.Maximum.Y);
                    writer.Write(this.BoundingBox.Maximum.Z);

                    writer.Write(this.elements.Count);
                    for (int i = 0; i < this.elements.Count; i++)
                    {
                        this.elements[i].Save(dataStream);
                    }

                    writer.Write(this.indices.Length);
                    for (int i = 0; i < this.indices.Length; i++)
                    {
                        writer.Write(this.indices[i]);
                    }

                    writer.Write(this.materials.Count);
                    for (int i = 0; i < this.materials.Count; i++)
                    {
                        this.materials[i].Save(dataStream);
                    }

                    writer.Write(this.subParts.Count);
                    for (int i = 0; i < this.subParts.Count; i++)
                    {
                        this.subParts[i].Save(dataStream);
                    }
                    
                    size = dataStream.Position;
                    dataStream.Position = 0;
                    dataStream.WriteTo(target);
                }
            }

            return size;
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
        // Private
        // -------------------------------------------------------------------
        private void Load(Stream source)
        {
            using (var reader = new BinaryReader(source))
            {
                int version = reader.ReadInt32();
                if (version != Version)
                {
                    throw new InvalidDataException("Mesh version is not supported: " + version);
                }

                this.Name = reader.ReadString();
                this.IndexCount = reader.ReadInt32();
                this.IndexSize = this.IndexCount * 4;
                this.ElementCount = reader.ReadInt32();
                this.tangentsCalculated = reader.ReadBoolean();

                this.Offset = new Vector3
                {
                    X = reader.ReadSingle(),
                    Y = reader.ReadSingle(),
                    Z = reader.ReadSingle()
                };

                this.Scale = new Vector3
                {
                    X = reader.ReadSingle(),
                    Y = reader.ReadSingle(),
                    Z = reader.ReadSingle()
                };

                this.Rotation = new Quaternion(
                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                Vector3 boundingMin = new Vector3
                    {
                        X = reader.ReadSingle(),
                        Y = reader.ReadSingle(),
                        Z = reader.ReadSingle()
                    };

                Vector3 boundingMax = new Vector3
                    {
                        X = reader.ReadSingle(),
                        Y = reader.ReadSingle(),
                        Z = reader.ReadSingle()
                    };

                this.BoundingBox = new BoundingBox(boundingMin, boundingMax);

                int entries = reader.ReadInt32();
                for (int i = 0; i < entries; i++)
                {
                    this.elements.Add(new MeshElement(source));
                }

                entries = reader.ReadInt32();
                this.indices = new uint[entries];
                for (int i = 0; i < entries; i++)
                {
                    this.indices[i] = reader.ReadUInt32();
                }

                entries = reader.ReadInt32();
                for (int i = 0; i < entries; i++)
                {
                    this.materials.Add(new MaterialElement(source));
                }

                entries = reader.ReadInt32();
                for (int i = 0; i < entries; i++)
                {
                    this.subParts.Add(new ModelResource(source));
                }
            }
        }

        private void DoCalculateTangents()
        {
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
