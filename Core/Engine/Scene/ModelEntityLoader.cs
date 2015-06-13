namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Processing.Resource.Model;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;

    using SharpDX;

    public class ModelEntityLoader : EngineComponent
    {
        private readonly IList<IModelEntity> models;
        private readonly IList<IModelEntity> rootModels;
        private readonly IDictionary<IModelEntity, IList<IModelEntity>> modelHirarchy;

        private ICarbonGraphics graphics;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelEntityLoader()
        {
            this.models = new List<IModelEntity>();
            this.modelHirarchy = new Dictionary<IModelEntity, IList<IModelEntity>>();
            this.rootModels = new List<IModelEntity>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SceneGraph LoadModelGroup(ModelResourceGroup group)
        {
            this.models.Clear();
            this.rootModels.Clear();
            this.modelHirarchy.Clear();

            if (this.graphics == null)
            {
                throw new InvalidOperationException("ModelEntityLoader was not initialized properly");
            }

            this.DoLoadModelGroup(group, null);
            return this.BuildGraph();
        }

        public override void Initialize(ICarbonGraphics graphicsHandler)
        {
            base.Initialize(graphicsHandler);

            this.graphics = graphicsHandler;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private SceneGraph BuildGraph()
        {
            var graph = new SceneGraph(new EmptyEntity { Name = "TEMP" });
            foreach (IModelEntity model in this.rootModels)
            {
                this.FillGraph(graph, null, model);
            }

            return graph;
        }

        private void FillGraph(SceneGraph graph, ISceneEntity parent, IModelEntity model)
        {
            graph.Add(model, parent);
            if (this.modelHirarchy.ContainsKey(model))
            {
                foreach (IModelEntity child in this.modelHirarchy[model])
                {
                    this.FillGraph(graph, model, child);
                }
            }
        }

        private void DoLoadModelGroup(ModelResourceGroup group, IModelEntity parent)
        {
            var groupNode = new ModelEntity
            {
                Name = group.Name,
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

            // Add the custom transformations next
            if (group.Transformations != null)
            {
                foreach (Matrix transformation in group.Transformations)
                {
                    groupNode.AddTransform(transformation);
                }
            }

            if (group.Models != null)
            {
                foreach (ModelResource modelResource in group.Models)
                {
                    var model = new ModelEntity();

                    // Todo: Bounding box generation should probably be on editor for this
                    modelResource.CalculateBoundingBox();
                    model.Mesh = new Mesh(modelResource);

                    if (modelResource.Materials == null || modelResource.Materials.Count <= 0)
                    {
                        System.Diagnostics.Trace.TraceWarning("Model has no material! " + modelResource.Name);
                    }
                    else
                    {
                        if (modelResource.Materials.Count > 1)
                        {
                            System.Diagnostics.Trace.TraceWarning("Model has more than one material, currently not supported! " + modelResource.Name);
                        }

                        model.Material = new Material(this.graphics, modelResource.Materials[0]); 
                    }
                    
                    this.modelHirarchy[groupNode].Add(model);
                    this.models.Add(model);
                }
            }

            if (group.Groups != null)
            {
                foreach (ModelResourceGroup subGroup in group.Groups)
                {
                    this.DoLoadModelGroup(subGroup, groupNode);
                }
            }
        }
    }
}
