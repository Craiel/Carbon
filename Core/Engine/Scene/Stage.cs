namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Resource;
    using Core.Engine.Resource.Resources.Model;
    using Core.Engine.Resource.Resources.Stage;
    using CarbonCore.Utils.Contracts.IoC;
    
    public class Stage : EngineComponent, IStage
    {
        private const string LightRoot = "Lights";
        private const string CameraRoot = "Cameras";
        private const string ModelRoot = "Models";

        private readonly ISceneEntityFactory entityFactory;
        private readonly IGameState gameState;
        private readonly StageResource data;
        
        private readonly IList<ResourceInfo> unusedReferences;

        private readonly ModelEntityLoader modelLoader;

        private readonly IList<ISceneEntity> stageEntities;

        private ISceneGraph stageGraph;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Stage(IFactory factory, IGameState gameState, StageResource data)
        {
            if (data == null)
            {
                throw new ArgumentException();
            }

            this.entityFactory = factory.Resolve<ISceneEntityFactory>();
            this.gameState = gameState;
            this.data = data;
            
            this.unusedReferences = new List<ResourceInfo>();
            this.modelLoader = new ModelEntityLoader();
            this.stageEntities = new List<ISceneEntity>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ISceneGraph Graph
        {
            get
            {
                return this.stageGraph;
            }
        }
        
        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            var rootEntity = new EmptyEntity { Name = "StageRoot" };
            this.stageEntities.Add(rootEntity);
            this.stageGraph = new SceneGraph(rootEntity);

            // Initialize the model loader for access to textures and other gpu resources
            this.modelLoader.Initialize(graphics);

            // Build and initialize all the scene components
            if (this.data.Cameras != null)
            {
                var cameraRoot = new EmptyEntity { Name = CameraRoot };
                var cameraRootNode = cameraRoot;
                this.stageEntities.Add(cameraRoot);
                this.stageGraph.Add(cameraRootNode);
                foreach (StageCameraElement cameraElement in this.data.Cameras)
                {
                    ICameraEntity camera = this.entityFactory.BuildCamera(cameraElement);
                    this.stageGraph.Add(camera, cameraRootNode);
                }

                System.Diagnostics.Trace.TraceInformation("Stage loaded cameras");
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Stage has no cameras");
            }
            
            if (this.data.Lights != null)
            {
                var lightRoot = new EmptyEntity { Name = LightRoot };
                var lightRootNode = lightRoot;
                this.stageGraph.Add(lightRootNode);
                foreach (StageLightElement lightElement in this.data.Lights)
                {
                    ILightEntity light = this.entityFactory.BuildLight(lightElement);
                    this.stageGraph.Add(light, lightRootNode);
                }

                System.Diagnostics.Trace.TraceInformation("Stage loaded lights");
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
                    this.unusedReferences.Add(info);
                }

                System.Diagnostics.Trace.TraceInformation("Stage has {0} references", referenceInfos.Length);
            }

            if (this.data.Models != null)
            {
                var modelRoot = new EmptyEntity { Name = ModelRoot };
                var modelRootNode = modelRoot;
                this.stageGraph.Add(modelRootNode);
                foreach (StageModelElement modelElement in this.data.Models)
                {
                    this.LoadModelElement(modelElement, referenceInfos, modelRootNode);
                }

                System.Diagnostics.Trace.TraceInformation("Stage loaded models");
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Stage has no models");
            }

            foreach (ResourceInfo unusedReference in this.unusedReferences)
            {
                System.Diagnostics.Trace.TraceWarning("Warning! Reference in stage was not used: {0}", unusedReference.Hash);
            }
        }

        private void LoadModelElement(StageModelElement element, ResourceInfo[] referenceInfos, ISceneEntity parent)
        {
            // Create a plain node first and register in the graph
            var elementEntity = new EmptyEntity { Name = element.Id };
            this.stageGraph.Add(elementEntity, parent);

            // See if we have an actual object attached to this
            if (element.ReferenceId != null)
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

                ResourceInfo reference = referenceInfos[(int)element.ReferenceId];
                if (reference == null)
                {
                    // We already warned about this earlier so just skip here
                    return;
                }

                var resource = this.gameState.ResourceManager.Load<ModelResourceGroup>(reference.Hash);
                
                // Create the element and set its local properties
                ISceneGraph modelGroupGraph = this.modelLoader.LoadModelGroup(resource);
                if (modelGroupGraph.Root.Children != null)
                {
                    IList<ISceneEntity> childNodes = new List<ISceneEntity>(modelGroupGraph.Root.Children);
                    foreach (ISceneEntity modelElement in childNodes)
                    {
                        // Properly unroot the children before we move them over, to avoid unexpected behavior
                        modelGroupGraph.Remove(modelElement, modelGroupGraph.Root);
                        elementEntity.AddChild(modelElement);
                    }
                }
                else
                {
                    System.Diagnostics.Trace.TraceWarning("Model group returned no scene graph elements");
                }

                elementEntity.Position = element.Translation;
                elementEntity.Rotation = element.Rotation;
                elementEntity.Scale = element.Scale;

                if (this.unusedReferences.Contains(reference))
                {
                    this.unusedReferences.Remove(reference);
                }
            }
            
            if (element.Children != null)
            {
                foreach (StageModelElement child in element.Children)
                {
                    this.LoadModelElement(child, referenceInfos, elementEntity);
                }
            }
        }
    }
}
