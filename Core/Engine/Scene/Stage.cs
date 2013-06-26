namespace Core.Engine.Scene
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Resource.Resources.Stage;

    public class Stage : EngineComponent, IStage
    {
        private readonly IEngineFactory factory;

        private readonly StageResource data;

        private readonly List<ISceneEntity> entities;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Stage(IEngineFactory factory, StageResource data)
        {
            this.factory = factory;
            this.data = data;

            this.entities = new List<ISceneEntity>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Initialize()
        {
        }

        public ReadOnlyCollection<ISceneEntity> Entities
        {
            get
            {
                return this.entities.AsReadOnly();
            }
        }
    }
}
