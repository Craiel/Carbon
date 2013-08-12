namespace Core.Engine.Scene
{
    using System.Collections.Generic;

    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Logic.Scripting;
    using Core.Engine.Rendering;

    public abstract class Scene : EngineComponent, IScene
    {
        private const int DefaultSceneEntityStack = 1;
        private const int DefaultSceneEntityRenderingList = 1;

        private readonly IList<ISceneEntity> sceneEntities; 
        private readonly IDictionary<int, EngineComponentStack<ISceneEntity>> entityStacks;
        private readonly IDictionary<int, RenderableList<ISceneEntity>> entityRenderLists;

        private bool isActive;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        protected Scene()
        {
            this.sceneEntities = new List<ISceneEntity>();
            this.entityStacks = new Dictionary<int, EngineComponentStack<ISceneEntity>>();
            this.entityRenderLists = new Dictionary<int, RenderableList<ISceneEntity>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                if (this.isActive != value)
                {
                    this.isActive = value;
                    if (value)
                    {
                        this.Activate();
                    }
                    else
                    {
                        this.Deactivate();
                    }
                }
            }
        }

        public bool IsVisible { get; set; }

        public string SceneScriptHash { get; set; }

        public abstract void Render(IFrameManager frameManager);
        public abstract void Resize(TypedVector2<int> size);

        [ScriptingMethod]
        public void RegisterEntity(ISceneEntity entity)
        {
            this.sceneEntities.Add(entity);
        }

        [ScriptingMethod]
        public void UnregisterEntity(ISceneEntity entity)
        {
            this.sceneEntities.Remove(entity);
        }

        [ScriptingMethod]
        public void ClearScene()
        {
            this.sceneEntities.Clear();

            this.entityRenderLists.Clear();
            this.entityStacks.Clear();
        }

        public override void Unload()
        {
            base.Unload();

            this.ClearScene();
        }

        [ScriptingMethod]
        public void InvalidateSceneEntity(ISceneEntity entity, int targetStack = DefaultSceneEntityStack)
        {
            if (!this.entityStacks.ContainsKey(targetStack))
            {
                this.entityStacks.Add(targetStack, new EngineComponentStack<ISceneEntity>());
            }

            this.entityStacks[targetStack].PushUpdate(entity);
        }

        [ScriptingMethod]
        public void AddSceneEntityToRenderingList(ISceneEntity entity, int targetList = DefaultSceneEntityRenderingList)
        {
            if (!this.entityRenderLists.ContainsKey(targetList))
            {
                this.entityRenderLists.Add(targetList, new RenderableList<ISceneEntity>());
            }

            this.entityRenderLists[targetList].Add(entity);
        }

        public override bool Update(Utils.Contracts.ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            foreach (int stack in this.entityStacks.Keys)
            {
                if (!this.entityStacks[stack].Update(gameTime))
                {
                    return false;
                }
            }

            return true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void RenderList(int list, FrameInstructionSet activeSet)
        {
            if (!this.entityRenderLists.ContainsKey(list))
            {
                System.Diagnostics.Trace.TraceWarning("RenderList called with non-existing list!");
                return;
            }

            this.entityRenderLists[list].Render(activeSet);
        }

        protected virtual void Activate()
        {
        }

        protected virtual void Deactivate()
        {
        }

        // Invalidate a node in all update lists
        // Invalidate a node in specific update lists
        // Perform scenegraph calculations,
        //  - Would these ever need to be on the actual node?
        //  - every node should ever exist only once
        //  - SceneGraph / octree / update list all reference nodes
    }
}
