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
        private readonly IDictionary<string, IList<int>> entityDictionary;

        private readonly IDictionary<int, ISceneEntity> entityRegister;
        private readonly IDictionary<ISceneEntity, int> entityRegisterReverse; 
 
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
            this.entityDictionary = new Dictionary<string, IList<int>>();

            this.entityRegister = new Dictionary<int, ISceneEntity>();
            this.entityRegisterReverse = new Dictionary<ISceneEntity, int>();
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
            this.entityDictionary.Clear();

            this.entityRegister.Clear();
            this.entityRegisterReverse.Clear();
        }

        public IList<ILightEntity> GetLights()
        {
            return this.GetEntityList<ILightEntity>(this.lights);
        }

        public IList<ICameraEntity> GetCameras()
        {
            return this.GetEntityList<ICameraEntity>(this.cameras);
        }

        public IList<IModelEntity> GetModels()
        {
            return this.GetEntityList<IModelEntity>(this.models);
        }

        public IList<ILightEntity> GetLightsById(string id)
        {
            if (!this.lightDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.GetEntityList<ILightEntity>(this.lightDictionary[id]);
        }

        public IList<ICameraEntity> GetCamerasById(string id)
        {
            if (!this.cameraDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.GetEntityList<ICameraEntity>(this.cameraDictionary[id]);
        }

        public IList<IModelEntity> GetModelsById(string id)
        {
            if (!this.modelDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.GetEntityList<IModelEntity>(this.modelDictionary[id]);
        }

        public IList<ISceneEntity> GetEntitiesById(string id)
        {
            if (!this.entityDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.GetentityList(this.entityDictionary[id]);
        }

        public IList<ISceneEntity> GetEntities()
        {
            return new List<ISceneEntity>(this.entityRegisterReverse.Keys);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Add(ISceneEntity entity)
        {
            int id = this.nextId++;
            this.entityRegister.Add(id, entity);
            this.entityRegisterReverse.Add(entity, id);

            bool registerName = !string.IsNullOrEmpty(entity.Name);
            if (registerName)
            {
                if (!this.entityDictionary.ContainsKey(entity.Name))
                {
                    this.entityDictionary.Add(entity.Name, new List<int>());
                }

                this.entityDictionary[entity.Name].Add(id);
            }
            
            // Register the entity lookups
            if (entity as ILightEntity != null)
            {
                this.lights.Add(id);

                if (registerName)
                {
                    if (!this.lightDictionary.ContainsKey(entity.Name))
                    {
                        this.lightDictionary.Add(entity.Name, new List<int>());
                    }

                    this.lightDictionary[entity.Name].Add(id);
                }

                return;
            }

            if (entity as ICameraEntity != null)
            {
                this.cameras.Add(id);

                if (registerName)
                {
                    if (!this.cameraDictionary.ContainsKey(entity.Name))
                    {
                        this.cameraDictionary.Add(entity.Name, new List<int>());
                    }

                    this.cameraDictionary[entity.Name].Add(id);
                }

                return;
            }

            if (entity as IModelEntity != null)
            {
                this.models.Add(id);

                if (registerName)
                {
                    if (!this.modelDictionary.ContainsKey(entity.Name))
                    {
                        this.modelDictionary.Add(entity.Name, new List<int>());
                    }

                    this.modelDictionary[entity.Name].Add(id);
                }
            }
        }

        protected void Remove(ISceneEntity entity)
        {
            // Get the key and invalidate the lookup, we deal with it in other places
            int key = this.entityRegisterReverse[entity];
            this.entityRegister.Remove(key);
            this.entityRegisterReverse.Remove(entity);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private IList<T> GetEntityList<T>(IList<int> source)
        {
            IList<T> results = new List<T>();
            IList<int> invalidateList = new List<int>();
            foreach (int entry in source)
            {
                if (!this.entityRegister.ContainsKey(entry))
                {
                    invalidateList.Add(entry);
                    continue;
                }

                results.Add((T)this.entityRegister[entry]);
            }

            // Remove every invalid entry to reduce the next lookup time
            foreach (int invalidKey in invalidateList)
            {
                source.Remove(invalidKey);
            }

            return results;
        }

        private IList<ISceneEntity> GetentityList(IList<int> source)
        {
            IList<ISceneEntity> results = new List<ISceneEntity>();
            IList<int> invalidateList = new List<int>();
            foreach (int entry in source)
            {
                if (!this.entityRegister.ContainsKey(entry))
                {
                    invalidateList.Add(entry);
                    continue;
                }

                results.Add(this.entityRegister[entry]);
            }

            foreach (int invalidKey in invalidateList)
            {
                source.Remove(invalidKey);
            }

            return results;
        }
    }
}
