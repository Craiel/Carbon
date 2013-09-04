namespace Core.Engine.Scene
{
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
    using Core.Engine.UserInterface;
    using Core.Utils.Contracts;

    using LuaInterface;

    public class SceneDebugOverlay : Scene, ISceneDebugOverlay
    {
        private readonly IEngineFactory factory;
        private readonly ILog log;

        private ICarbonGraphics graphics;

        // Loaded Resources
        private UserInterfaceResource userInterfaceResource;

        // Components
        private IUserInterface userInterface;
        private IOrthographicCamera userInterfaceCamera;

        private IProjectionCamera debugCamera;
        private IFirstPersonController debugController;

        // --------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneDebugOverlay(IEngineFactory factory)
        {
            this.factory = factory;
            this.log = factory.Get<IEngineLog>().AquireContextLog("SceneDebugOverlay");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize(ICarbonGraphics graphic)
        {
            base.Initialize(graphic);

            this.graphics = graphic;

            // Initialize our camera's
            this.userInterfaceCamera = this.factory.Get<IOrthographicCamera>();
            this.userInterfaceCamera.Initialize(graphic);

            // Create the debug ui
            this.userInterface = new UserInterface(this.factory, this.userInterfaceResource);
            this.userInterface.Initialize(this.graphics);

            // Todo: Register the UI entities and add them to the rendering list
            /*foreach (ISceneEntity entity in this.userInterface.Entities)
            {
                this.RegisterEntity(entity);
                this.AddSceneEntityToRenderingList(entity, 2);
            }*/

            this.debugCamera = this.factory.Get<IProjectionCamera>();
            this.debugController = this.factory.Get<IFirstPersonController>();
            this.debugController.Initialize(graphic);
            this.debugController.SetInputBindings("debugController");
            this.debugController.IsActive = true;
            
            // Todo: Register the stage's entities and add them to the rendering list
            /*foreach (IList<IModelEntity> entityList in this.stage.Models.Values)
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
            }*/
        }

        public override bool Update(ITimer gameTime)
        {
            this.debugController.Update(gameTime);
            this.debugCamera.Position = this.debugController.Position;
            this.debugCamera.Rotation = this.debugController.Rotation;
            this.debugCamera.Update(gameTime);

            return base.Update(gameTime);
        }

        public override void Render(IFrameManager frameManager)
        {
            // The scene to deferred
            FrameInstructionSet set = frameManager.BeginSet(this.debugCamera);
            set.Technique = FrameTechnique.Forward;
            set.LightingEnabled = true;
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
            this.debugCamera.SetPerspective(size, 0.05f, 200.0f);
            this.userInterfaceCamera.SetPerspective(size, 0.05f, 200.0f);
        }

        public override void Unload()
        {
            base.Unload();

            // release Camera's
            this.userInterfaceCamera.Dispose();

            // release the components of the scene
            if (this.userInterface != null)
            {
                this.userInterface.Dispose();
            }
            
            if (this.userInterfaceResource != null)
            {
                this.userInterfaceResource.Dispose();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool HasRuntime
        {
            get
            {
                return false;
            }
        }

        protected override CarbonScript LoadRuntimeScript(string scriptHash)
        {
            // We don't load a runtime for this scene
            throw new NotSupportedException();
        }

        protected override Lua LoadRuntime(CarbonScript script)
        {
            throw new NotSupportedException();
        }
    }
}
