namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;
    using Core.Engine.Resource;
    using Core.Engine.Resource.Resources.Model;
    using Core.Engine.Resource.Resources.Stage;

    using SharpDX;

    public class Stage : EngineComponent, IStage
    {
        private readonly ISceneEntityFactory entityFactory;
        private readonly IGameState gameState;
        private readonly StageResource data;

        private readonly IDictionary<string, IProjectionCamera> cameras;
        private readonly IDictionary<string, ILightEntity> lights;
        private readonly IDictionary<string, IList<IModelEntity>> models;
        private readonly IDictionary<IModelEntity, IList<IModelEntity>> modelHirarchy;
        private readonly IList<IModelEntity> rootModels; 
        
        private readonly IList<ResourceInfo> unusedReferences;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Stage(IEngineFactory factory, IGameState gameState, StageResource data)
        {
            if (data == null)
            {
                throw new ArgumentException();
            }

            this.entityFactory = factory.Get<ISceneEntityFactory>();
            this.gameState = gameState;
            this.data = data;

            this.cameras = new Dictionary<string, IProjectionCamera>();
            this.lights = new Dictionary<string, ILightEntity>();
            this.models = new Dictionary<string, IList<IModelEntity>>();
            this.modelHirarchy = new Dictionary<IModelEntity, IList<IModelEntity>>();
            this.rootModels = new List<IModelEntity>();
            
            this.unusedReferences = new List<ResourceInfo>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IDictionary<string, IProjectionCamera> Cameras
        {
            get
            {
                return this.cameras;
            }
        }

        public IDictionary<string, ILightEntity> Lights
        {
            get
            {
                return this.lights;
            }
        }

        public IDictionary<string, IList<IModelEntity>> Models
        {
            get
            {
                return this.models;
            }
        }

        public IDictionary<IModelEntity, IList<IModelEntity>> ModelHirarchy
        {
            get
            {
                return this.modelHirarchy;
            }
        }

        public IList<IModelEntity> RootModels
        {
            get
            {
                return this.rootModels;
            }
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            // Build and initialize all the scene components
            if (this.data.Cameras != null)
            {
                foreach (StageCameraElement cameraElement in this.data.Cameras)
                {
                    IProjectionCamera camera = this.entityFactory.BuildCamera(cameraElement);
                    this.cameras.Add(cameraElement.Id, camera);
                }

                System.Diagnostics.Trace.TraceInformation("Stage loaded {0} cameras", this.cameras.Count);
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Stage has no cameras");
            }
            
            if (this.data.Lights != null)
            {
                foreach (StageLightElement lightElement in this.data.Lights)
                {
                    ILightEntity light = this.entityFactory.BuildLight(lightElement);
                    this.lights.Add(light.Name, light);
                }

                System.Diagnostics.Trace.TraceInformation("Stage loaded {0} lights", this.lights.Count);
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Stage has no lights");
            }

            ResourceInfo[] referenceInfos = null;
            if (this.data.References != null)
            {
                referenceInfos = new ResourceInfo[this.data.References.Count];
                for (int i = 0; i < this.data.References.Count; i++)
                {
                    ResourceInfo info = this.gameState.ResourceManager.GetInfo(this.data.References[i]);
                    if (info == null)
                    {
                        System.Diagnostics.Trace.TraceError("Could not get info for reference: ");
                        continue;
                    }

                    referenceInfos[i] = info;
                    unusedReferences.Add(info);
                }

                System.Diagnostics.Trace.TraceInformation("Stage has {0} references", referenceInfos.Length);
            }

            if (this.data.Models != null)
            {
                foreach (StageModelElement modelElement in this.data.Models)
                {
                    this.LoadModelElement(modelElement, referenceInfos);
                }

                System.Diagnostics.Trace.TraceInformation("Stage loaded {0} models", this.models.Count);
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Stage has no models");
            }

            foreach (ResourceInfo unusedReference in unusedReferences)
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Reference in stage was not used: {0}", unusedReference.Hash);
            }
        }

        private void LoadModelElement(StageModelElement element, ResourceInfo[] referenceInfos)
        {
            // Todo: Have the scene graph handle this!
            if (element.ReferenceId >= 0)
            {
                if (referenceInfos == null)
                {
                    System.Diagnostics.Trace.TraceError("Error! Model had reference but no references where loaded");
                    return;
                }

                if (element.ReferenceId >= referenceInfos.Length)
                {
                    System.Diagnostics.Trace.TraceWarning("Error! Model reference does not match with reference count");
                    return;
                }

                ResourceInfo reference = referenceInfos[element.ReferenceId];
                if (reference == null)
                {
                    // We already warned about this earlier so just skip here
                    return;
                }

                var resource = this.gameState.ResourceManager.Load<ModelResourceGroup>(reference.Hash);
                this.LoadModelGroup(element, resource, null);
                if (unusedReferences.Contains(reference))
                {
                    unusedReferences.Remove(reference);
                }
            }

            /*if (parent != null)
            {
                model.World = MatrixExtension.GetLocalMatrix(parent.Scale, parent.Rotation, parent.Position);
            }*/

            if (element.Children != null)
            {
                // Todo: Save the hierarchy info
                foreach (StageModelElement child in element.Children)
                {
                    this.LoadModelElement(child, referenceInfos);
                }
            }
        }

        // Todo: need to put the models into hirarchy
        private void LoadModelGroup(StageModelElement host, ModelResourceGroup group, IModelEntity parent)
        {
            var groupNode = new ModelEntity
            {
                Position = group.Offset,
                Scale = group.Scale,
                Rotation =
                    Quaternion.RotationYawPitchRoll(
                        group.Rotation.X,
                        group.Rotation.Y,
                        group.Rotation.Z)
            };

            this.modelHirarchy.Add(groupNode, new List<IModelEntity>());
            if (parent != null)
            {
                this.modelHirarchy[parent].Add(groupNode);
            }
            else
            {
                this.rootModels.Add(groupNode);
            }

            if (group.Models != null)
            {
                foreach (ModelResource modelResource in group.Models)
                {
                    var model = new ModelEntity();

                    // Todo: Bounding box generation should probably be on editor for this
                    modelResource.CalculateBoundingBox();
                    model.Mesh = new Mesh(modelResource);
                    
                    if (!this.models.ContainsKey(host.Id))
                    {
                        this.models.Add(host.Id, new List<IModelEntity>());
                    }

                    this.modelHirarchy[groupNode].Add(model);
                    this.models[host.Id].Add(model);
                }
            }

            if (group.Groups != null)
            {
                foreach (ModelResourceGroup subGroup in group.Groups)
                {
                    this.LoadModelGroup(host, subGroup, groupNode);
                }
            }
        }
    }
}
