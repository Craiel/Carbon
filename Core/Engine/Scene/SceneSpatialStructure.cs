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

        public IList<INode> GetNodesById(string id)
        {
            if (!this.nodeDictionary.ContainsKey(id))
            {
                return null;
            }

            return this.GetNodeList(this.nodeDictionary[id]);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Add(INode node)
        {
            int id = this.nextId++;
            this.nodeRegister.Add(id, node);

            bool registerName = !string.IsNullOrEmpty(node.Name);
            if (registerName)
            {
                if (!this.nodeDictionary.ContainsKey(node.Name))
                {
                    this.nodeDictionary.Add(node.Name, new List<int>());
                }

                this.nodeDictionary[node.Name].Add(id);
            }
            
            // Register the entity lookups
            if (node as IEntityNode != null)
            {
                var typed = (IEntityNode)node;
                if (typed.Entity as ILightEntity != null)
                {
                    this.lights.Add(id);

                    if (registerName)
                    {
                        if (!this.lightDictionary.ContainsKey(node.Name))
                        {
                            this.lightDictionary.Add(node.Name, new List<int>());
                        }

                        this.lightDictionary[node.Name].Add(id);
                    }

                    return;
                }

                if (typed.Entity as ICameraEntity != null)
                {
                    this.cameras.Add(id);

                    if (registerName)
                    {
                        if (!this.cameraDictionary.ContainsKey(node.Name))
                        {
                            this.cameraDictionary.Add(node.Name, new List<int>());
                        }

                        this.cameraDictionary[node.Name].Add(id);
                    }

                    return;
                }

                if (typed.Entity as IModelEntity != null)
                {
                    this.models.Add(id);

                    if (registerName)
                    {
                        if (!this.modelDictionary.ContainsKey(node.Name))
                        {
                            this.modelDictionary.Add(node.Name, new List<int>());
                        }

                        this.modelDictionary[node.Name].Add(id);
                    }
                }
            }
        }

        protected void Remove(INode entity)
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private IList<T> GetEntityList<T>(IEnumerable<int> source)
        {
            IList<T> results = new List<T>();
            foreach (int entry in source)
            {
                results.Add((T)((IEntityNode)this.nodeRegister[entry]).Entity);
            }

            return results;
        }

        private IList<INode> GetNodeList(IEnumerable<int> source)
        {
            IList<INode> results = new List<INode>();
            foreach (int entry in source)
            {
                results.Add(this.nodeRegister[entry]);
            }

            return results;
        }
    }
}
