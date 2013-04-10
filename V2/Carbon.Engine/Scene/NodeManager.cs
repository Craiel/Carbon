using System;
using System.IO;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Logic.Scripting;
using Carbon.Engine.Rendering;
using Carbon.Engine.Rendering.Primitives;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;
using Carbon.Engine.Resource.Resources;

using Core.Utils;

using SlimDX;

namespace Carbon.Engine.Scene
{
    using System.Data;

    using Carbon.Engine.Contracts.Scene;
    using Carbon.Engine.Logic;

    public interface INodeManager : IEngineComponent, IScriptingProvider
    {
        INode RootNode { get; }

        void InitializeContent(IContentManager contentManager, IResourceManager resourceManager);

        void RotateNode(INode node, Vector3 axis, float angle);

        INode AddAmbientLight(Vector4 color, float specularPower = 1.0f, INode parent = null);
        INode AddDirectionalLight(Vector4 color, Vector3 direction, float specularPower = 1.0f, INode parent = null);
        INode AddPointLight(Vector4 color, float range = 1.0f, float specularPower = 1.0f, INode parent = null);
        INode AddSpotLight(Vector4 color, Vector2 angles, Vector3 direction, float range = 1.0f, float specularPower = 1.0f, INode parent = null);

        INode AddNode(INode parent = null);
        INode AddModel(string path, INode parent = null);
        INode AddSphere(int detailLevel, INode parent = null);
        INode AddPlane(INode parent = null);
        INode AddStaticText(int fontId, string text, Vector2 charSize, INode parent = null);

        void Clear();
    }

    public class NodeManager : EngineComponent, INodeManager
    {
        private readonly INode root;

        private IContentManager contentManager;
        private IResourceManager resourceManager;

        private ICarbonGraphics graphics;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NodeManager()
        {
            this.root = new Node();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ScriptingProperty]
        public INode RootNode
        {
            get
            {
                return this.root;
            }
        }

        public override void Dispose()
        {
            this.root.Dispose();

            base.Dispose();
        }

        public void InitializeContent(IContentManager content, IResourceManager resource)
        {
            this.contentManager = content;
            this.resourceManager = resource;
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.graphics = graphics;
        }

        [ScriptingMethod]
        public void RotateNode(INode node, Vector3 axis, float angle)
        {
            node.Rotation += Quaternion.RotationAxis(axis, MathExtension.DegreesToRadians(angle));
        }

        [ScriptingMethod]
        public INode AddAmbientLight(Vector4 color, float specularPower = 1.0f, INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            var light = new Light { Color = color, Type = LightType.Ambient, SpecularPower = specularPower };
            var node = new LightNode { Light = light };
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddDirectionalLight(Vector4 color, Vector3 direction, float specularPower = 1.0f, INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            var light = new Light { Color = color, Type = LightType.Direction, Direction = direction, SpecularPower = specularPower };
            var node = new LightNode { Light = light };
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddPointLight(Vector4 color, float range = 1.0f, float specularPower = 1.0f, INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            var light = new Light { Color = color, Type = LightType.Point, Range = range, SpecularPower = specularPower };
            var node = new LightNode { Light = light };
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddSpotLight(Vector4 color, Vector2 angles, Vector3 direction, float range = 1.0f, float specularPower = 1.0f, INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            var light = new Light { Color = color, Type = LightType.Spot, SpotAngles = angles, Direction = direction, Range = range, SpecularPower = specularPower };
            var node = new LightNode { Light = light };
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddNode(INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            var node = new Node();
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddModel(string path, INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }
            
            var resource = this.resourceManager.Load<ModelResource>(HashUtils.BuildResourceHash(path));
            if (resource == null)
            {
                return null;
            }

            ModelNode node = this.CreateModelNode(resource);
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddSphere(int detailLevel, INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            var resource = Sphere.Create(detailLevel);
            if (resource == null)
            {
                return null;
            }

            ModelNode node = this.CreateModelNode(resource);
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddPlane(INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            var resource = Quad.Create(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY, 1, 1);
            if (resource == null) 
            {
                return null;
            }

            ModelNode node = this.CreateModelNode(resource);
            parent.AddChild(node);
            return node;
        }

        [ScriptingMethod]
        public INode AddStaticText(int fontId, string text, Vector2 size, INode parent = null)
        {
            if (parent == null)
            {
                parent = this.root;
            }

            FontEntry font = this.contentManager.TypedLoad(new ContentQuery<FontEntry>().IsEqual("Id", fontId)).UniqueResult<FontEntry>();
            if (font == null)
            {
                throw new DataException("Font was not found for id " + fontId);
            }

            var resource = FontBuilder.Build(text, size, font);
            if (resource == null)
            {
                return null;
            }
            
            ModelNode node = this.CreateModelNode(resource);

            // Todo: This is more for testing than anything else..., needs refactoring at some point
            if (font.Resource != null)
            {
                var resourceEntry = this.contentManager.Load<ResourceEntry>(font.Resource);
                if (resourceEntry != null)
                {
                    TextureReference reference = this.graphics.TextureManager.Register(resourceEntry.Hash);
                    node.Material = new Material(reference) { AlphaTexture = reference };
                }
            }

            parent.AddChild(node);
            return node;
        }

        public void Clear()
        {
            this.root.Clear();
        }

        [ScriptingMethod]
        public void ChangeMaterial(INode node, int materialId, bool recursive = false)
        {
            if (!node.GetType().Implements<IModelNode>())
            {
                throw new ArgumentException("Material change not supported for node with type " + node.GetType());
            }

            var materialData = this.contentManager.TypedLoad(new ContentQuery<MaterialEntry>().IsEqual("Id", materialId)).UniqueResult<MaterialEntry>();
            if (materialData == null)
            {
                throw new InvalidDataException("No material with id " + materialId);
            }

            var material = new Material(this.graphics, this.contentManager, materialData);
            if (recursive)
            {
                this.ApplyMaterialRecurse((IModelNode)node, material);
            }
            else
            {
                ((IModelNode)node).Material = material;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private ModelNode CreateModelNode(ModelResource resource)
        {
            var node = new ModelNode
            {
                Mesh = new Mesh(resource),
                Position = new Vector4(resource.Offset, 1.0f),
                Scale = resource.Scale,
                Rotation = resource.Rotation
            };

            if (resource.Materials != null && resource.Materials.Count > 0)
            {
                if (resource.Materials.Count > 1)
                {
                    throw new NotImplementedException();
                }

                node.Material = new Material(graphics, resource.Materials[0]);
            }

            if (resource.SubParts != null && resource.SubParts.Count > 0)
            {
                foreach (ModelResource part in resource.SubParts)
                {
                    ModelNode child = this.CreateModelNode(part);
                    if (child != null)
                    {
                        node.AddChild(child);
                    }
                }
            }

            return node;
        }

        private void ApplyMaterialRecurse(IModelNode node, Material material)
        {
            node.Material = material;
            foreach (IEntity child in node.Children)
            {
                if (child as IModelNode != null)
                {
                    this.ApplyMaterialRecurse(child as IModelNode, material);
                }
            }
        }
    }
}
