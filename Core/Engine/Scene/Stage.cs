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

        private readonly IDictionary<string, ICamera> cameras;
        private readonly IDictionary<string, ILight> lights;
        private readonly IDictionary<string, IList<IModelEntity>> models;

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

            this.cameras = new Dictionary<string, ICamera>();
            this.lights = new Dictionary<string, ILight>();
            this.models = new Dictionary<string, IList<IModelEntity>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IDictionary<string, ICamera> Cameras
        {
            get
            {
                return this.cameras;
            }
        }

        public IDictionary<string, ILight> Lights
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

        public override void Initialize(ICarbonGraphics graphics)
        {
            // Build and initialize all the scene components
            if (this.data.Cameras != null)
            {
                foreach (StageCameraElement cameraElement in this.data.Cameras)
                {
                    ICamera camera = this.entityFactory.BuildCamera(cameraElement);
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
                    ILight light = this.entityFactory.BuildLight(lightElement);
                    this.lights.Add(lightElement.Id, light);
                }

                System.Diagnostics.Trace.TraceInformation("Stage loaded {0} lights", this.lights.Count);
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Stage has no lights");
            }

            IList<ResourceInfo> unusedReferences = new List<ResourceInfo>();
            ResourceInfo[] referenceInfos = null;
            if (this.data.References != null)
            {
                referenceInfos = new ResourceInfo[this.data.References.Count];
                for (int i = 0; i < this.data.References.Count; i++)
                {
                    ResourceInfo info = gameState.ResourceManager.GetInfo(this.data.References[i]);
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
                    IModelEntity model = this.entityFactory.BuildModel(modelElement);
                    if (modelElement.ReferenceId >= 0)
                    {
                        if (referenceInfos == null)
                        {
                            System.Diagnostics.Trace.TraceError("Error! Model had reference but no references where loaded");
                            continue;
                        }

                        if (modelElement.ReferenceId >= referenceInfos.Length)
                        {
                            System.Diagnostics.Trace.TraceWarning("Error! Model reference does not match with reference count");
                            continue;
                        }

                        ResourceInfo reference = referenceInfos[modelElement.ReferenceId];
                        if (reference == null)
                        {
                            // We already warned about this earlier so just skip here
                            continue;
                        }

                        var resource = this.gameState.ResourceManager.Load<ModelResourceGroup>(reference.Hash);
                        // model.Mesh = new Mesh();
                        if (unusedReferences.Contains(reference))
                        {
                            unusedReferences.Remove(reference);
                        }
                    }

                    if (!this.models.ContainsKey(modelElement.Id))
                    {
                        this.models.Add(modelElement.Id, new List<IModelEntity>());
                    }

                    this.models[modelElement.Id].Add(model);
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
    }
}
