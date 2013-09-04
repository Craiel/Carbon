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

    public class SceneMainMenu : SceneBase, ISceneMainMenu
    {
        private readonly IEngineFactory factory;
        private readonly ILog log;

        private ICarbonGraphics graphics;

        // Loaded Resources
        private UserInterfaceResource userInterfaceResource;
        private StageResource stageResource;

        // Components
        private IUserInterface userInterface;
        private IStage stage;

        private IProjectionCamera activeCamera;
        private IOrthographicCamera userInterfaceCamera;

        private ICamera debugCamera;
        private IFirstPersonController debugController;
        private IModelEntity debugModel;

        // --------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneMainMenu(IEngineFactory factory)
            : base(factory)
        {
            this.factory = factory;
            this.log = factory.Get<IApplicationLog>().AquireContextLog("MainMenuScene");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
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
                    this.RegisterEntity(entity);
                    this.AddSceneEntityToRenderingList(entity, 2);
                }
            }

            if (this.stageResource != null)
            {
                this.stage = new Stage(this.factory, this.GameState, this.stageResource);
                this.stage.Initialize(this.graphics);

                // Todo: for testing use debug camera and controller for now
                this.activeCamera = this.stage.Cameras.FirstOrDefault().Value;
                //this.activeCamera = this.factory.Get<IProjectionCamera>();
                this.debugController = this.factory.Get<IFirstPersonController>();
                this.debugController.Initialize(graphic);
                this.debugController.SetInputBindings("debugController");
                this.debugController.IsActive = true;
                this.debugController.Position = this.activeCamera.Position;

                // Register the stage's entities and add them to the rendering list
                foreach (IList<IModelEntity> entityList in this.stage.Models.Values)
                {
                    foreach (IModelEntity entity in entityList)
                    {
                        this.RegisterEntity(entity);
                        this.AddSceneEntityToRenderingList(entity);
                        this.InvalidateSceneEntity(entity);
                    }
                }

                foreach (ILightEntity light in this.stage.Lights.Values)
                {
                    this.RegisterEntity(light);
                    this.AddSceneEntityToRenderingList(light);
                    this.InvalidateSceneEntity(light);
                }
            }
        }

        public override bool Update(ITimer gameTime)
        {
            this.debugController.Update(gameTime);
            this.activeCamera.Position = this.debugController.Position;
            this.activeCamera.Rotation = this.debugController.Rotation;
            this.activeCamera.Update(gameTime);

            return base.Update(gameTime);
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
    }
}
