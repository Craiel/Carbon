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
    public interface INodeManager : IScriptingProvider
    {
        INode RootNode { get; }

        void RotateNode(INode node, Vector3 axis, float angle);

        INode AddAmbientLight(Vector4 color, float specularPower = 1.0f, INode parent = null);
        INode AddDirectionalLight(Vector4 color, Vector3 direction, float specularPower = 1.0f, INode parent = null);
        INode AddPointLight(Vector4 color, float range = 1.0f, float specularPower = 1.0f, INode parent = null);
        INode AddSpotLight(Vector4 color, Vector2 angles, Vector3 direction, float range = 1.0f, float specularPower = 1.0f, INode parent = null);

        INode AddNode(INode parent = null);
        INode AddModel(string path, INode parent = null);
        INode AddSphere(int detailLevel, INode parent = null);
    }

    public class NodeManager : INodeManager
    {
        private readonly ICarbonGraphics graphics;
        private readonly IContentManager contentManager;
        private readonly IResourceManager resourceManager;
        private readonly INode root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NodeManager(ICarbonGraphics graphics, IContentManager contentManager, IResourceManager resourceManager)
        {
            this.graphics = graphics;
            this.contentManager = contentManager;
            this.resourceManager = resourceManager;

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
