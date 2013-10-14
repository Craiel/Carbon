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
    
    public class Stage : EngineComponent, IStage
    {
        private readonly ISceneEntityFactory entityFactory;
        private readonly IGameState gameState;
        private readonly StageResource data;

        private readonly IList<ICameraEntity> cameras;
        private readonly IList<ILightEntity> lights;
        
        private readonly IList<ResourceInfo> unusedReferences;

        private readonly ModelEntityLoader modelLoader;
        private readonly SceneGraph modelGraph;

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

            this.cameras = new List<ICameraEntity>();
            this.lights = new List<ILightEntity>();
            
            this.unusedReferences = new List<ResourceInfo>();
            this.modelLoader = new ModelEntityLoader();

            this.modelGraph = new SceneGraph();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SceneGraph BuildGraph()
        {
            var graph = new SceneGraph();
            var lightRoot = new Node { Name = "Lights" };
            graph.Add(lightRoot);
            foreach (ILightEntity light in this.lights)
            {
                graph.Add(new EntityNode(light), lightRoot);
            }

            var cameraRoot = new Node { Name = "Cameras" };
            graph.Add(cameraRoot);
            foreach (ICameraEntity camera in this.cameras)
            {
                graph.Add(new EntityNode(camera), cameraRoot);
            }

            var modelRoot = new Node { Name = "Models" };
            graph.Add(modelRoot);

            return graph;
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
                    ICameraEntity camera = this.entityFactory.BuildCamera(cameraElement);
                    this.cameras.Add(camera);
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
                    this.lights.Add(light);
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
                    this.LoadModelElement(modelElement, referenceInfos, null);
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

        private void LoadModelElement(StageModelElement element, ResourceInfo[] referenceInfos, INode parent)
        {
            // Create a plain node first and register in the graph
            INode elementNode = new Node { Name = element.Id };
            this.modelGraph.Add(elementNode, parent);

            // See if we have an actual object attached to this
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
                
                // Create the element and set its local properties
                SceneGraph modelGraph = this.modelLoader.LoadModelGroup(resource);
                /*model.Position = element.Translation;
                model.Rotation = element.Rotation;
                model.Scale = element.Scale;*/
                // Todo: join the graphs

                if (this.unusedReferences.Contains(reference))
                {
                    this.unusedReferences.Remove(reference);
                }
            }
            
            if (element.Children != null)
            {
                // Todo: Save the hierarchy info
                foreach (StageModelElement child in element.Children)
                {
                    this.LoadModelElement(child, referenceInfos, elementNode);
                }
            }
        }
    }
}
