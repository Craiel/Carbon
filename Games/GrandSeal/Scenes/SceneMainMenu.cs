namespace GrandSeal.Scenes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Contracts.UserInterface;
    using Core.Engine.Logic;
    using Core.Engine.Logic.Scripting;
    using Core.Engine.Rendering;
    using Core.Engine.Resource.Resources;
    using Core.Engine.Resource.Resources.Stage;
    using Core.Engine.Scene;
    using Core.Engine.UserInterface;
    using Core.Utils.Contracts;

    using Logic;

    public class SceneMainMenu : SceneBase, ISceneMainMenu
    {
        private readonly IEngineFactory factory;
        private readonly ILog log;
        private readonly ISceneDebugOverlay debugOverlay;
        private readonly IGrandSealSystemController systemController;

        private ICarbonGraphics graphics;

        // Loaded Resources
        private UserInterfaceResource userInterfaceResource;
        private StageResource stageResource;

        // Components
        private IUserInterface userInterface;
        private IStage stage;

        private IProjectionCamera activeCamera;
        private IOrthographicCamera userInterfaceCamera;

        private bool useDebugCamera;

        private IProjectionCamera activeSceneCamera;

        // --------------------------------------------------------------------
        // Constructor
        // --------------------------------------------------------------------
        public SceneMainMenu(IEngineFactory factory)
            : base(factory)
        {
            this.factory = factory;
            this.debugOverlay = this.factory.Get<ISceneDebugOverlay>();

            this.systemController = this.factory.Get<IGrandSealSystemController>();
            
            this.log = factory.Get<IGrandSealLog>().AquireContextLog("MainMenuScene");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Dispose()
        {
            if (this.IsActive)
            {
                this.systemController.ActionTriggered -= this.OnSystemAction;
            }

            base.Dispose();
        }

        public override void Initialize(ICarbonGraphics graphic)
        {
            if (string.IsNullOrEmpty(this.SceneScriptHash))
            {
                throw new InvalidOperationException("Scene init called without Scene Script present");
            }

            base.Initialize(graphic);

            this.graphics = graphic;

            // Initialize our camera's
            this.userInterfaceCamera = this.factory.Get<IOrthographicCamera>();
            this.userInterfaceCamera.Initialize(graphic);

            // Load the init script for the scene
            this.GameState.ScriptingEngine.Register(this);
            var resource = this.GameState.ResourceManager.Load<ScriptResource>(this.SceneScriptHash);
            var script = new CarbonScript(resource);
            this.GameState.ScriptingEngine.ExecuteOneshot(script);
            this.GameState.ScriptingEngine.Unregister(this);

            // Now all the user data should be loaded and set so proceed initializing
            if (this.userInterfaceResource != null)
            {
                this.userInterface = new UserInterface(this.factory, this.userInterfaceResource);
                this.userInterface.Initialize(this.graphics);

                // Register the UI entities and add them to the rendering list
                foreach (ISceneEntity entity in this.userInterface.Entities)
                {
                    this.RegisterAndInvalidate(entity);
                    this.AddSceneEntityToRenderingList(entity, 2);
                }
            }

            if (this.stageResource != null)
            {
                this.stage = new Stage(this.factory, this.GameState, this.stageResource);
                this.stage.Initialize(this.graphics);

                // Todo: for testing use debug camera and controller for now
                this.activeCamera = this.stage.Cameras.FirstOrDefault().Value;

                // Register the stage's entities and add them to the rendering list
                foreach (IModelEntity entity in this.stage.Models)
                {
                    this.RegisterAndInvalidate(entity);
                    this.AddSceneEntityToRenderingList(entity);
                }
                
                foreach (ILightEntity light in this.stage.Lights.Values)
                {
                    this.RegisterAndInvalidate(light);
                    this.AddSceneEntityToRenderingList(light);
                }
            }
        }

        public override bool Update(ITimer gameTime)
        {
            this.stage.Update(gameTime);

            this.activeCamera.Update(gameTime);

            foreach (IModelEntity node in this.stage.RootModels)
            {
                this.TestUpdate(node, null);
            }

            return base.Update(gameTime);
        }

        private void TestUpdate(IModelEntity entity, IModelEntity parent)
        {
            this.InvalidateSceneEntity(entity);
            if (parent != null)
            {
                entity.World = entity.Local * parent.World;
            }
            else
            {
                entity.World = entity.Local;
            }

            /*if (this.stage.ModelHirarchy.ContainsKey(entity))
            {
                foreach (IModelEntity child in this.stage.ModelHirarchy[entity])
                {
                    this.TestUpdate(child, entity);
                }
            }*/
        }

        public override void Render(IFrameManager frameManager)
        {
            FrameInstructionSet set;

            // The scene to deferred
            if (this.activeCamera != null)
            {
                set = frameManager.BeginSet(this.activeCamera);
                set.Technique = FrameTechnique.Forward;
                set.LightingEnabled = true;
                this.RenderList(1, set);
                
                frameManager.RenderSet(set);
            }

            // User Interface as overlay on top
            /*set = frameManager.BeginSet(this.userInterfaceCamera);
            set.LightingEnabled = false;
            set.Technique = FrameTechnique.Forward;
            this.RenderList(2, set);
            frameManager.RenderSet(set);*/
        }

        public override void Resize(TypedVector2<int> size)
        {
            // Update the camera perspectives
            if (this.activeCamera != null)
            {
                this.activeCamera.SetPerspective(size, 0.05f, 200.0f);
            }

            this.userInterfaceCamera.SetPerspective(size, 0.05f, 200.0f);
        }

        public override void Unload()
        {
            base.Unload();

            // release Camera's
            this.userInterfaceCamera.Dispose();

            // release the components of the scene
            if (this.stage != null)
            {
                this.stage.Dispose();
            }

            if (this.userInterface != null)
            {
                this.userInterface.Dispose();
            }

            // release the resources
            if (this.stageResource != null)
            {
                this.stageResource.Dispose();
            }

            if (this.userInterfaceResource != null)
            {
                this.userInterfaceResource.Dispose();
            }
        }

        [ScriptingMethod]
        public void SetUserInterface(string hash)
        {
            if (this.userInterfaceResource != null)
            {
                this.log.Warning("UserInterface resource changed without unloading!");
            }

            this.userInterfaceResource = this.GameState.ResourceManager.Load<UserInterfaceResource>(hash);
        }

        [ScriptingMethod]
        public void SetStage(string hash)
        {
            if (this.stageResource != null)
            {
                this.log.Warning("Stage resource changed without unloading!");
            }

            this.stageResource = this.GameState.ResourceManager.Load<StageResource>(hash);
        }

        // --------------------------------------------------------------------
        // Protected
        // --------------------------------------------------------------------
        protected override void Activate()
        {
            this.systemController.ActionTriggered += this.OnSystemAction;

            base.Activate();
        }

        protected override void Deactivate()
        {
            base.Deactivate();

            this.systemController.ActionTriggered -= this.OnSystemAction;
        }

        // --------------------------------------------------------------------
        // Private
        // --------------------------------------------------------------------
        private void OnSystemAction(GrandSealSystemAction obj)
        {
            switch (obj)
            {
                case GrandSealSystemAction.ToggleDebugOverlay:
                    {
                        // Not active means it is in the process of being activated
                        if (!this.debugOverlay.IsActive)
                        {
                            this.RefreshDebugData();
                        }

                        break;
                    }

                case GrandSealSystemAction.ToggleDebugCamera:
                    {
                        if (!this.debugOverlay.IsActive)
                        {
                            return;
                        }

                        if (this.useDebugCamera)
                        {
                            // Switch back to the scene camera
                            this.debugOverlay.EnableController = false;
                            this.activeCamera = this.activeSceneCamera;
                        }
                        else
                        {
                            this.activeSceneCamera = this.activeCamera;

                            this.activeCamera = this.debugOverlay.Camera;
                            this.activeCamera.CopyFrom(this.activeSceneCamera);
                            this.debugOverlay.EnableController = true;
                        }

                        this.useDebugCamera = !this.useDebugCamera;
                        break;
                    }
            }
        }

        private void RefreshDebugData()
        {
            // refresh and upload our entity information to the debug overlay
            IList<SceneEntityDebugEntry> entityData = new List<SceneEntityDebugEntry>();
            foreach (IModelEntity entity in this.stage.Models)
            {
                var entry = new SceneEntityDebugEntry("<TODO>", EntityDebugType.Model, new WeakReference<ISceneEntity>(entity));
                entityData.Add(entry);
            }

            foreach (string key in this.stage.Lights.Keys)
            {
                var entry = new SceneEntityDebugEntry(
                    key,
                    EntityDebugType.Light,
                    new WeakReference<ISceneEntity>(this.stage.Lights[key]));
                entityData.Add(entry);
            }

            this.debugOverlay.UpdateEntityData(entityData);
        }
    }
}
