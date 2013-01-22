using System.Collections.Generic;
using System.IO;

using SlimDX;

namespace Carbon.Engine.Resource
{
    public enum SceneResourceNodeType
    {
        Unknown = 0,
        Model = 1,
    }

    public class SceneResourceNode
    {
        public SceneResourceNode()
        {
            this.Scale = new Vector3(1);

            this.Children = new List<SceneResourceNode>();
        }

        public SceneResourceNode(Stream source)
        {
            using (var reader = new BinaryReader(source))
            {
                this.Type = (SceneResourceNodeType)reader.ReadInt32();

                this.Position = new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() };
                this.Scale = new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() };
                this.Rotation = new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() };

                this.MeshId = reader.ReadInt32();
                this.MaterialId = reader.ReadInt32();

                int childCount = reader.ReadInt32();
                for (int i = 0; i < childCount; i++)
                {
                    this.Children.Add(new SceneResourceNode(source));
                }
            }
        }

        public SceneResourceNodeType Type { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }

        public int? MeshId { get; set; }
        public int? MaterialId { get; set; }

        public IList<SceneResourceNode> Children { get; private set; }

        public long Save(Stream target)
        {
            long size;
            using (var dataStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(dataStream))
                {
                    writer.Write((int)this.Type);

                    writer.Write(this.Position.X);
                    writer.Write(this.Position.Y);
                    writer.Write(this.Position.Z);

                    writer.Write(this.Scale.X);
                    writer.Write(this.Scale.Y);
                    writer.Write(this.Scale.Z);

                    writer.Write(this.Rotation.X);
                    writer.Write(this.Rotation.Y);
                    writer.Write(this.Rotation.Z);

                    writer.Write(MeshId ?? -1);
                    writer.Write(MaterialId ?? -1);

                    writer.Write(Children.Count);
                    foreach (SceneResourceNode child in Children)
                    {
                        child.Save(target);
                    }

                    size = dataStream.Position;
                    dataStream.Position = 0;
                    dataStream.WriteTo(target);
                }
            }

            return size;
        }
    }

    public class SceneResource
    {
        private const int Version = 1;
        
        public SceneResource()
        {
            this.Scale = new Vector3(1);

            this.Meshes = new Dictionary<int, string>();
            this.Materials = new Dictionary<int, string>();

            this.Nodes = new List<SceneResourceNode>();
        }

        public SceneResource(Stream source)
            : this()
        {
            using (var reader = new BinaryReader(source))
            {
                if (reader.ReadInt32() != Version)
                {
                    throw new InvalidDataException("Model version is not known");
                }

                this.Name = reader.ReadString();

                this.Scale = new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() };
                this.Rotation = new Vector3 { X = reader.ReadSingle(), Y = reader.ReadSingle(), Z = reader.ReadSingle() };

                int meshCount = reader.ReadInt32();
                for (int i = 0; i < meshCount; i++)
                {
                    int key = reader.ReadInt32();
                    this.Meshes.Add(key, reader.ReadString());
                }

                int materialCount = reader.ReadInt32();
                for (int i = 0; i < materialCount; i++)
                {
                    int key = reader.ReadInt32();
                    this.Materials.Add(key, reader.ReadString());
                }

                int nodeCount = reader.ReadInt32();
                for (int i = 0; i < nodeCount; i++)
                {
                    this.Nodes.Add(new SceneResourceNode(source));
                }
            }
        }

        public string Name { get; set; }

        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }

        public IDictionary<int, string> Meshes { get; private set; }
        public IDictionary<int, string> Materials { get; private set; }

        public IList<SceneResourceNode> Nodes { get; private set; }
        
        public long Save(Stream target)
        {
            long size;
            using (var dataStream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(dataStream))
                {
                    writer.Write(Version);
                    writer.Write(this.Name);
                    
                    writer.Write(this.Scale.X);
                    writer.Write(this.Scale.Y);
                    writer.Write(this.Scale.Z);

                    writer.Write(this.Rotation.X);
                    writer.Write(this.Rotation.Y);
                    writer.Write(this.Rotation.Z);

                    writer.Write(this.Meshes.Count);
                    foreach (KeyValuePair<int, string> pair in Meshes)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }

                    writer.Write(this.Materials.Count);
                    foreach (KeyValuePair<int, string> pair in Materials)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }

                    writer.Write(Nodes.Count);
                    foreach (SceneResourceNode node in Nodes)
                    {
                        node.Save(target);
                    }
                    
                    size = dataStream.Position;
                    dataStream.Position = 0;
                    dataStream.WriteTo(target);
                }
            }

            return size;
        }
    }
}
