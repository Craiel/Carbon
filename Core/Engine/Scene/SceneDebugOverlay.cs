namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;

    using Core.Engine.Contracts;
    using Core.Engine.Contracts.Logic;
    using Core.Engine.Contracts.Rendering;
    using Core.Engine.Contracts.Scene;
    using Core.Engine.Contracts.UserInterface;
    using Core.Engine.Logic;
    using Core.Engine.Logic.Scripting;
    using Core.Engine.Rendering;
    using Core.Engine.Rendering.Primitives;
    using Core.Engine.Resource.Resources;
    using Core.Engine.Resource.Resources.Model;
    using Core.Engine.UserInterface;
    using Core.Utils.Contracts;

    using LuaInterface;

    using SharpDX;
    using SharpDX.Direct3D;

    public class SceneDebugOverlay : Scene, ISceneDebugOverlay
    {
        private const int EntityRenderingList = 10;

        private static readonly Vector4 ColorModelEntity = new Vector4(0, 1, 0, 0.5f);
        private static readonly Vector4 ColorLightEntity = new Vector4(1, 1, 0, 0.5f);

        private readonly IEngineFactory factory;
        private readonly ILog log;

        private readonly ModelResource lightEntityModel;

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

            this.lightEntityModel = Sphere.Create(0, ColorLightEntity);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool EnableController
        {
            get
            {
                return this.debugController.IsActive;
            }

            set
            {
                // Push the camera values and activate the controller
                this.debugController.Position = this.Camera.Position;
                this.debugController.Rotation = this.Camera.Rotation;
                this.debugController.IsActive = value;
            }
        }

        public bool EnableRendering { get; set; }

        public IProjectionCamera Camera
        {
            get
            {
                return this.debugCamera;
            }
        }

        public void UpdateEntityData(IEnumerable<SceneEntityDebugEntry> entities)
        {
            this.ClearRenderingList(EntityRenderingList);
            foreach (SceneEntityDebugEntry entry in entities)
            {
                switch (entry.Type)
                {
                    case EntityDebugType.Model:
                        {
                            this.AddModelEntity(entry);
                            break;
                        }

                    case EntityDebugType.Light:
                        {
                            this.AddLightEntity(entry);
                            break;
                        }
                }
            }
        }

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
            this.debugController.SetInputBindings(InputManager.DefaultBindingDebugController);
            
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

            // We are explicitly not calling update on this, will be done by the active scene if desired
            this.debugCamera.Position = this.debugController.Position;
            this.debugCamera.Rotation = this.debugController.Rotation;

            return base.Update(gameTime);
        }

        public override void Render(IFrameManager frameManager)
        {
            if (!this.EnableRendering)
            {
                return;
            }

            // Render the entity data
            FrameInstructionSet set = frameManager.BeginSet(this.debugCamera);
            set.Technique = FrameTechnique.Plain;
            set.LightingEnabled = false;
            set.Topology = PrimitiveTopology.LineList;
            this.RenderList(EntityRenderingList, set);
            frameManager.RenderSet(set);

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

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void AddModelEntity(SceneEntityDebugEntry entry)
        {
            ISceneEntity source;
            if (!entry.Source.TryGetTarget(out source))
            {
                return;
            }

            // Todo: handle this case
            if (source.BoundingBox == null)
            {
                return;
            }
            
            ModelResource resource = Cube.CreateBoundingBoxLines(source.BoundingBox.Value, ColorModelEntity);
            var entity = new ModelEntity
            {
                Position = source.Position,
                Scale = new Vector3(1.1f),
                Rotation = source.Rotation,
                Mesh = new Mesh(resource)
            };

            this.RegisterAndInvalidate(entity);
            this.AddSceneEntityToRenderingList(entity, EntityRenderingList);
        }

        private void AddLightEntity(SceneEntityDebugEntry entry)
        {
            ISceneEntity source;
            if (!entry.Source.TryGetTarget(out source))
            {
                return;
            }

            var entity = new ModelEntity
            {
                Position = source.Position,
                Scale = new Vector3(0.5f),
                Rotation = source.Rotation,
                Mesh = new Mesh(this.lightEntityModel)
            };

            this.RegisterAndInvalidate(entity);
            this.AddSceneEntityToRenderingList(entity, EntityRenderingList);
        }
    }
}
