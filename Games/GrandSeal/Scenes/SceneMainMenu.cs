using System;

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

using GrandSeal.Contracts;

namespace GrandSeal.Scenes
{
    /// <summary>
    /// Entry Point scene for GrandSeal
    /// </summary>
    public class SceneMainMenu : Scene, ISceneMainMenu
    {
        private readonly IEngineFactory factory;
        private readonly ILog log;
        private readonly IGrandSealGameState gameState;

        private ICarbonGraphics graphics;

        // Loaded Resources
        private UserInterfaceResource userInterfaceResource;
        private StageResource stageResource;

        // Components
        private IUserInterface userInterface;
        private IStage stage;

        private IProjectionCamera camera;
        private IOrthographicCamera userInterfaceCamera;

        // --------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneMainMenu(IEngineFactory factory)
        {
            this.factory = factory;
            this.log = factory.Get<IApplicationLog>().AquireContextLog("EntryScene");
            this.gameState = factory.Get<IGrandSealGameState>();
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
            this.camera = this.factory.Get<IProjectionCamera>();
            this.camera.Initialize(graphic);
            this.userInterfaceCamera = this.factory.Get<IOrthographicCamera>();
            this.userInterfaceCamera.Initialize(graphic);

            // Load the init script for the scene
            this.gameState.ScriptingEngine.Register(this);
            var resource = this.gameState.ResourceManager.Load<ScriptResource>(this.SceneScriptHash);
            var script = new CarbonScript(resource);
            this.gameState.ScriptingEngine.Execute(script);
            this.gameState.ScriptingEngine.Unregister(this);

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
                this.stage = new Stage(this.factory, this.stageResource);
                this.stage.Initialize(this.graphics);

                // Register the stage's entities and add them to the rendering list
                foreach (ISceneEntity entity in this.stage.Entities)
                {
                    this.RegisterEntity(entity);
                    this.AddSceneEntityToRenderingList(entity, targetList: 1);
                }
            }
        }

        public override void Render(IFrameManager frameManager)
        {
            // The scene to deferred
            FrameInstructionSet set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Deferred;
            this.RenderList(1, set);
            frameManager.RenderSet(set);

            // User Interface as overlay on top
            set = frameManager.BeginSet(this.userInterfaceCamera);
            set.LightingEnabled = false;
            set.Technique = FrameTechnique.Forward;
            this.RenderList(2, set);
            frameManager.RenderSet(set);
        }

        public override void Resize(TypedVector2<int> size)
        {
            // Update the camera perspectives
            this.camera.SetPerspective(size, 0.05f, 200.0f);
            this.userInterfaceCamera.SetPerspective(size, 0.05f, 200.0f);
        }

        public override void Unload()
        {
            base.Unload();

            // release Camera's
            this.camera.Dispose();
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

            this.userInterfaceResource = this.gameState.ResourceManager.Load<UserInterfaceResource>(hash);
        }

        [ScriptingMethod]
        public void SetStage(string hash)
        {
            if (this.stageResource != null)
            {
                this.log.Warning("Stage resource changed without unloading!");
            }

            this.stageResource = this.gameState.ResourceManager.Load<StageResource>(hash);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Activate()
        {
            base.Activate();

            this.gameState.ScriptingEngine.Register(this);
        }

        protected override void Deactivate()
        {
            this.gameState.ScriptingEngine.Unregister(this);

            base.Deactivate();
        }
    }
}
