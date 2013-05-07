﻿using System;
using System.Data;
using System.IO;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Logic;
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
    public class SceneEntityFactory : EngineComponent, ISceneEntityFactory
    {
        private readonly IContentManager contentManager;
        private readonly IResourceManager resourceManager;

        private ICarbonGraphics graphics;

        public SceneEntityFactory(IResourceManager resourceManager, IContentManager contentManager)
        {
            this.resourceManager = resourceManager;
            this.contentManager = contentManager;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize(ICarbonGraphics currentGraphics)
        {
            base.Initialize(currentGraphics);

            this.graphics = currentGraphics;
        }

        [ScriptingMethod]
        public void RotateEntity(ISceneEntity entity, Vector3 axis, float angle)
        {
            entity.Rotation += Quaternion.RotationAxis(axis, MathExtension.DegreesToRadians(angle));
        }

        [ScriptingMethod]
        public ISceneEntity AddAmbientLight(Vector4 color, float specularPower = 1.0f)
        {
            var light = new Light { Color = color, Type = LightType.Ambient, SpecularPower = specularPower };
            return new LightEntity { Light = light };
        }

        [ScriptingMethod]
        public ISceneEntity AddDirectionalLight(Vector4 color, Vector3 direction, float specularPower = 1.0f)
        {
            var light = new Light { Color = color, Type = LightType.Direction, Direction = direction, SpecularPower = specularPower };
            return new LightEntity { Light = light };
        }

        [ScriptingMethod]
        public ISceneEntity AddPointLight(Vector4 color, float range = 1.0f, float specularPower = 1.0f)
        {
            var light = new Light { Color = color, Type = LightType.Point, Range = range, SpecularPower = specularPower };
            return new LightEntity { Light = light };
        }

        [ScriptingMethod]
        public ISceneEntity AddSpotLight(Vector4 color, Vector2 angles, Vector3 direction, float range = 1.0f, float specularPower = 1.0f)
        {
            var light = new Light { Color = color, Type = LightType.Spot, SpotAngles = angles, Direction = direction, Range = range, SpecularPower = specularPower };
            return new LightEntity { Light = light };
        }
        
        [ScriptingMethod]
        public ISceneEntity AddModel(ModelResource resource)
        {
            return this.CreateModelEntity(resource);
        }

        [ScriptingMethod]
        public ISceneEntity AddModel(string path)
        {
            var resource = this.resourceManager.Load<ModelResource>(HashUtils.BuildResourceHash(path));
            if (resource == null)
            {
                return null;
            }

            return this.CreateModelEntity(resource);
        }

        [ScriptingMethod]
        public ISceneEntity AddSphere(int detailLevel)
        {
            var resource = Sphere.Create(detailLevel);
            if (resource == null)
            {
                return null;
            }

            return this.CreateModelEntity(resource);
        }

        [ScriptingMethod]
        public ISceneEntity AddPlane()
        {
            var resource = Quad.Create(Vector3.Zero, -Vector3.UnitZ, Vector3.UnitY, new TypedVector2<int>(1));
            if (resource == null) 
            {
                return null;
            }

            return this.CreateModelEntity(resource);
        }

        [ScriptingMethod]
        public ISceneEntity AddStaticText(int fontId, string text, Vector2 size)
        {
            var font = this.contentManager.TypedLoad(new ContentQuery<FontEntry>().IsEqual("Id", fontId)).UniqueResult<FontEntry>();
            if (font == null)
            {
                throw new DataException("Font was not found for id " + fontId);
            }

            var resource = FontBuilder.Build(text, size, font);
            if (resource == null)
            {
                return null;
            }

            ModelEntity entity = this.CreateModelEntity(resource);

            // Todo: This is more for testing than anything else..., needs refactoring at some point
            if (font.Resource != null)
            {
                var resourceEntry = this.contentManager.Load<ResourceEntry>(font.Resource);
                if (resourceEntry != null)
                {
                    if (!this.graphics.TextureManager.IsRegistered(resourceEntry.Hash))
                    {
                        this.graphics.TextureManager.Register(resourceEntry.Hash);
                    }

                    entity.Material = new Material(this.graphics, diffuse: resourceEntry.Hash, normal: resourceEntry.Hash);
                }
            }

            return entity;
        }
        
        [ScriptingMethod]
        public void ChangeMaterial(INode node, int materialId, bool recursive = false)
        {
            if (node.Entity == null || node.Entity as IModelEntity == null)
            {
                // Nothing to do for this node
                return;
            }

            var entity = node.Entity as IModelEntity;
            var materialData = this.contentManager.TypedLoad(new ContentQuery<MaterialEntry>().IsEqual("Id", materialId)).UniqueResult<MaterialEntry>();
            if (materialData == null)
            {
                throw new InvalidDataException("No material with id " + materialId);
            }

            var material = new Material(this.graphics, this.contentManager, materialData);
            entity.Material = material;
            if (recursive && node.Children != null)
            {
                foreach (INode child in node.Children)
                {
                    this.ChangeMaterial(child, materialId, true);
                }
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private ModelEntity CreateModelEntity(ModelResource resource)
        {
            var node = new ModelEntity
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

                node.Material = new Material(this.graphics, resource.Materials[0]);
            }

            if (resource.SubParts != null && resource.SubParts.Count > 0)
            {
                // Todo: Figure out how to deal with this best with the entity revamp
                throw new NotImplementedException();
                /*foreach (ModelResource part in resource.SubParts)
                {
                    ModelEntity child = this.CreateModelNode(part);
                    if (child != null)
                    {
                        node.AddChild(child);
                    }
                }*/
            }

            return node;
        }
    }
}
