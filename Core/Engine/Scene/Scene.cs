namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts;

    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Logic;
    using Core.Engine.Logic.Scripting;
    using Core.Engine.Rendering;
    
    using NLua;

    public abstract class Scene : EngineComponent, IScene
    {
        private const string FunctionHookUpdate = "Update";

        private const int DefaultSceneEntityStack = 1;
        private const int DefaultSceneEntityRenderingList = 1;

        private readonly IList<ISceneEntity> sceneEntities; 
        private readonly IDictionary<int, EngineComponentStack<ISceneEntity>> entityStacks;
        private readonly IDictionary<int, RenderableList<ISceneEntity>> entityRenderLists;

        private bool isActive;

        private CarbonScript runtimeScript;

        private Lua runtime;
        private LuaFunction functionUpdate;

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
        public void SetRuntime(string scriptHash)
        {
            this.runtimeScript = this.LoadRuntimeScript(scriptHash);
        }

        public void ClearScene()
        {
            // Unlink all entities before clear to free them from this scene
            foreach (ISceneEntity entity in this.sceneEntities)
            {
                entity.Unlink();
            }

            this.sceneEntities.Clear();

            this.entityRenderLists.Clear();
            this.entityStacks.Clear();
        }

        public override void Unload()
        {
            base.Unload();

            this.ClearScene();
        }

        public void LinkEntity(ISceneEntity entity, int targetStack = DefaultSceneEntityStack)
        {
            this.EntityRecurse(
                entity,
                sceneEntity =>
                    {
                        sceneEntity.Link(this, targetStack);
                        this.sceneEntities.Add(sceneEntity);
                    });
        }

        public override bool Update(ITimer gameTime)
        {
            if (!base.Update(gameTime))
            {
                return false;
            }

            if (this.functionUpdate != null)
            {
                this.functionUpdate.Call(gameTime);
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

        public virtual void CheckState()
        {
            if (this.runtimeScript == null && this.HasRuntime)
            {
                throw new InvalidSceneStateException("Runtime script is missing");
            }
        }

        public void InvalidateSceneEntity(ISceneEntity entity, int targetStack = DefaultSceneEntityStack)
        {
            if (!this.entityStacks.ContainsKey(targetStack))
            {
                this.entityStacks.Add(targetStack, new EngineComponentStack<ISceneEntity>());
            }

            this.InvalidateSceneEntity(entity, this.entityStacks[targetStack]);
        }

        public void AddToRenderingList(ISceneEntity entity, int targetList = DefaultSceneEntityRenderingList)
        {
            if (!this.entityRenderLists.ContainsKey(targetList))
            {
                this.entityRenderLists.Add(targetList, new RenderableList<ISceneEntity>());
            }

            this.EntityRecurse(
                entity,
                sceneEntity =>
                    {
                        if (sceneEntity.CanRender)
                        {
                            this.entityRenderLists[targetList].Add(sceneEntity);
                        }
                    });
        }

        private void EntityRecurse<T>(T entity, Action<ISceneEntity> action)
            where T : ISceneEntity
        {
            var queue = new Queue<T>();
            queue.Enqueue(entity);
            while (queue.Count > 0)
            {
                ISceneEntity current = queue.Dequeue();
                action(current);
                if (current.Children != null)
                {
                    foreach (T child in current.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract bool HasRuntime { get; }

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
            this.CheckState();

            if (this.HasRuntime)
            {
                this.runtime = this.LoadRuntime(this.runtimeScript);
                if (this.runtime == null)
                {
                    throw new InvalidSceneStateException("Runtime was not loaded properly");
                }

                this.functionUpdate = this.runtime.GetFunction(FunctionHookUpdate);
            }
        }

        protected virtual void Deactivate()
        {
            if (this.runtime != null)
            {
                this.functionUpdate.Dispose();
                this.functionUpdate = null;

                this.runtime.Dispose();
                this.runtime = null;
            }
        }

        protected abstract CarbonScript LoadRuntimeScript(string scriptHash);
        protected abstract Lua LoadRuntime(CarbonScript script);

        protected void UnlinkEntity(ISceneEntity entity)
        {
            this.EntityRecurse(
                entity,
                sceneEntity =>
                    {
                        sceneEntity.Unlink();
                        this.sceneEntities.Remove(sceneEntity);
                    });
        }

        protected void ClearRenderingList(int targetList = DefaultSceneEntityRenderingList)
        {
            if (this.entityRenderLists.ContainsKey(targetList))
            {
                this.entityRenderLists[targetList].Clear();
            }
        }

        private void InvalidateSceneEntity(ISceneEntity entity, EngineComponentStack<ISceneEntity> stack)
        {
            this.EntityRecurse(entity, stack.PushUpdate);
        }
    }
}
