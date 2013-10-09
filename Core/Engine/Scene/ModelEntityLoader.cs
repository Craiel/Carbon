namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Rendering;
    using Core.Engine.Resource.Resources.Model;

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
        public IList<IModelEntity> Models
        {
            get
            {
                return this.models;
            }
        }

        public IList<IModelEntity> RootModels
        {
            get
            {
                return this.rootModels;
            }
        }

        public IDictionary<IModelEntity, IList<IModelEntity>> ModelHirarchy
        {
            get
            {
                return this.modelHirarchy;
            }
        }

        public void LoadModelGroup(ModelResourceGroup group)
        {
            if (this.graphics == null)
            {
                throw new InvalidOperationException("ModelEntityLoader was not initialized properly");
            }

            this.LoadModelGroup(group, null);
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.graphics = graphics;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void LoadModelGroup(ModelResourceGroup group, IModelEntity parent)
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
                    this.LoadModelGroup(subGroup, groupNode);
                }
            }
        }
    }
}
