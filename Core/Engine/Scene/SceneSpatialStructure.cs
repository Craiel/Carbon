namespace Core.Engine.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Scene;

    public abstract class SceneSpatialStructure : ISceneSpatialStructure
    {
        private readonly List<int> lights;
        private readonly List<int> cameras;
        private readonly List<int> models;

        private readonly IDictionary<string, IList<int>> lightDictionary;
        private readonly IDictionary<string, IList<int>> cameraDictionary;
        private readonly IDictionary<string, IList<int>> modelDictionary;
        private readonly IDictionary<string, IList<int>> nodeDictionary;

        private readonly IDictionary<int, INode> nodeRegister;
 
        private int nextId;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected SceneSpatialStructure()
        {
            this.lights = new List<int>();
            this.cameras = new List<int>();
            this.models = new List<int>();

            this.lightDictionary = new Dictionary<string, IList<int>>();
            this.cameraDictionary = new Dictionary<string, IList<int>>();
            this.modelDictionary = new Dictionary<string, IList<int>>();
            this.nodeDictionary = new Dictionary<string, IList<int>>();

            this.nodeRegister = new Dictionary<int, INode>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.lights.Clear();
            this.cameras.Clear();
            this.models.Clear();

            this.lightDictionary.Clear();
            this.cameraDictionary.Clear();
            this.modelDictionary.Clear();
            this.nodeDictionary.Clear();

            this.nodeRegister.Clear();
        }

        public IReadOnlyCollection<ILightEntity> GetLights()
        {
            return this.lights.AsReadOnly();
        }

        public IReadOnlyCollection<ICameraEntity> GetCameras()
        {
            return this.cameras.AsReadOnly();
        }

        public IReadOnlyCollection<IModelEntity> GetModels()
        {
            return this.models.AsReadOnly();
        }

        public ILightEntity GetLightById(string id)
        {
            if (!this.lightDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.lightDictionary[id];
        }

        public ICameraEntity GetCameraById(string id)
        {
            if (!this.cameraDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.cameraDictionary[id];
        }

        public IModelEntity GetModelById(string id)
        {
            if (!this.modelDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.modelDictionary[id];
        }

        public INode GetNodeById(string id)
        {
            if (!this.nodeDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.nodeDictionary[id];
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Add(INode entity)
        {
            this.nodeDictionary.Add(entity.Name, entity);

            // Register the entity lookups
            if (entity as IEntityNode != null)
            {
                var typed = (IEntityNode)entity;
                if (typed.Entity as ILightEntity != null)
                {
                    this.lights.Add((ILightEntity)typed.Entity);
                    this.lightDictionary.Add(entity.Name, (ILightEntity)typed.Entity);
                    return;
                }

                if (typed.Entity as ICameraEntity != null)
                {
                    this.cameras.Add((ICameraEntity)typed.Entity);
                    this.cameraDictionary.Add(entity.Name, (ICameraEntity)typed.Entity);
                    return;
                }

                if (typed.Entity as IModelEntity != null)
                {
                    this.models.Add((IModelEntity)typed.Entity);
                    this.modelDictionary.Add(entity.Name, (IModelEntity)typed.Entity);
                }
            }
        }

        protected void Remove(INode entity)
        {
        }
    }
}
