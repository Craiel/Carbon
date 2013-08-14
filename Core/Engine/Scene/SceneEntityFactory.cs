namespace Core.Engine.Scene
{
    using System;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;
    using Core.Engine.Resource.Resources.Stage;
    using Core.Protocol.Resource;

    using SlimDX;

    public class SceneEntityFactory : EngineComponent, ISceneEntityFactory
    {
        private readonly IEngineFactory factory;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneEntityFactory(IEngineFactory factory)
        {
            this.factory = factory;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICamera BuildCamera(StageCameraElement cameraElement)
        {
            var camera = this.factory.Get<IProjectionCamera>();
            camera.Position = new Vector4(cameraElement.Position, 1);
            camera.Rotation = Quaternion.RotationYawPitchRoll(cameraElement.Orientation.X, cameraElement.Orientation.Y, cameraElement.Orientation.Z);

            // Todo: near / far plane and fov
            return camera;
        }

        public ILight BuildLight(StageLightElement lightElement)
        {
            var light = new Light();
            if (lightElement.Location != null)
            {
                light.Position = new Vector4((Vector3)lightElement.Location, 1);
            }

            light.Range = lightElement.Radius;
            if (lightElement.Direction != null)
            { 
                light.Direction = (Vector3)lightElement.Direction;
            }

            if (lightElement.Color != null)
            {
                light.Color = new Vector4((Vector3)lightElement.Color, 1);
            }

            switch (lightElement.Type)
            {
                case StageLight.Types.StageLightType.Directional:
                    {
                        light.Type = LightType.Direction;
                        break;
                    }

                case StageLight.Types.StageLightType.Point:
                    {
                        light.Type = LightType.Point;
                        break;
                    }

                case StageLight.Types.StageLightType.Spot:
                    {
                        light.Type = LightType.Spot;
                        break;
                    }

                default:
                    {
                        System.Diagnostics.Trace.TraceError("Unknown Light type: {0}", lightElement.Type);
                        break;
                    }
            }

            // Todo:
            // lightElement.SpotSize
            // lightElement.Intensity;
            // lightElement.AmbientIntensity
            return light;
        }

        public IModelEntity BuildModel(StageModelElement modelElement)
        {
            var model = new ModelEntity
                            {
                                Position = new Vector4(modelElement.Translation, 1),
                                Scale = modelElement.Scale,
                                Rotation =
                                    Quaternion.RotationYawPitchRoll(
                                        modelElement.Rotation.X,
                                        modelElement.Rotation.Y,
                                        modelElement.Rotation.Z)
                            };
            return model;
        }
    }



    /*using Core.Engine.Resource.Resources.Model;

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
            if (resource.SubParts != null && resource.SubParts.Count > 0)
            {
                var entities = new SceneEntityCollection<IModelEntity>();
                this.CreateModelEntities(resource, ref entities);
                return entities;
            }

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

            if (resource.SubParts != null && resource.SubParts.Count > 0)
            {
                var entities = new SceneEntityCollection<IModelEntity>();
                this.CreateModelEntities(resource, ref entities);
                return entities;
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
        public void ChangeMaterial(ISceneEntity node, int materialId)
        {
            if (node == null)
            {
                // Nothing to do for this node
                return;
            }

            var materialData = this.contentManager.TypedLoad(new ContentQuery<MaterialEntry>().IsEqual("Id", materialId)).UniqueResult<MaterialEntry>();
            if (materialData == null)
            {
                throw new InvalidDataException("No material with id " + materialId);
            }
            
            var material = new Material(this.graphics, this.contentManager, materialData);

            if (node as IModelEntity != null)
            {
                ((IModelEntity)node).Material = material;
            } 
            else if (node as SceneEntityCollection<IModelEntity> != null)
            {
                foreach (IModelEntity entry in ((SceneEntityCollection<IModelEntity>)node).Entries)
                {
                    entry.Material = material;
                }
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private ModelEntity CreateModelEntity(ModelResource resource)
        {
            if (resource.SubParts != null && resource.SubParts.Count > 0)
            {
                throw new ArgumentException("Use CreateModelEntities for resources with sub-parts!");
            }

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

            return node;
        }

        private void CreateModelEntities(ModelResource resource, ref SceneEntityCollection<IModelEntity> targetList)
        {
            var node = new ModelEntity
            {
                Mesh = new Mesh(resource),
                Position = new Vector4(resource.Offset, 1.0f),
                Scale = resource.Scale,
                Rotation = resource.Rotation
            };

            targetList.Add(node);

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
                foreach (ModelResource part in resource.SubParts)
                {
                    this.CreateModelEntities(part, ref targetList);
                }
            }
        }
    }*/
}
