namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Resource;
    using Core.Engine.Resource.Resources.Model;
    using Core.Engine.Resource.Resources.Stage;
    
    public class Stage : EngineComponent, IStage
    {
        private readonly ISceneEntityFactory entityFactory;
        private readonly IGameState gameState;
        private readonly StageResource data;

        private readonly IDictionary<string, IProjectionCamera> cameras;
        private readonly IDictionary<string, ILightEntity> lights;
        
        private readonly IList<ResourceInfo> unusedReferences;

        private readonly ModelEntityLoader modelLoader;

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
            
            this.unusedReferences = new List<ResourceInfo>();
            this.modelLoader = new ModelEntityLoader();
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

        public IList<IModelEntity> Models
        {
            get
            {
                return this.modelLoader.Models;
            }
        }

        public IList<IModelEntity> RootModels
        {
            get
            {
                return this.modelLoader.RootModels;
            }
        }

        public IDictionary<IModelEntity, IList<IModelEntity>> ModelHirarchy
        {
            get
            {
                return this.modelLoader.ModelHirarchy;
            }
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            // Initialize the model loader for access to textures and other gpu resources
            this.modelLoader.Initialize(graphics);

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

                System.Diagnostics.Trace.TraceInformation("Stage loaded {0} models", this.modelLoader.Models.Count);
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
                this.modelLoader.LoadModelGroup(resource);
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

        
    }
}
