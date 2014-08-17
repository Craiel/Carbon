namespace Core.Engine.Scene
{
    using System;
    using System.Collections.Generic;
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
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
    using NLua;
    using SharpDX;
    using SharpDX.Direct3D;

    public class SceneDebugOverlay : Scene, ISceneDebugOverlay
    {
        private static readonly Vector4 ColorModelEntity = new Vector4(0, 1, 0, 0.5f);
        private static readonly Vector4 ColorLightEntity = new Vector4(1, 1, 0, 0.5f);

        private readonly IFactory factory;
        private readonly ILog log;

        private readonly ModelResource lightEntityModel;

        private readonly ISceneGraph sceneGraph;
        
        private ModelEntityLoader modelEntityLoader;

        private ICarbonGraphics graphics;

        // Loaded Resources
        private UserInterfaceResource userInterfaceResource;

        // Components
        private IUserInterface userInterface;
        private IOrthographicCamera userInterfaceCamera;

        private ICameraEntity debugCamera;
        private IFirstPersonController debugController;

        private ModelResourceGroup compassResource;
        private ISceneEntity compass;

        // --------------------------------------------------------------------
        // Constructor
        // --------------------------------------------------------------------
        public SceneDebugOverlay(IFactory factory)
        {
            this.factory = factory;
            this.log = factory.Resolve<IEngineLog>().AquireContextLog("SceneDebugOverlay");

            this.lightEntityModel = Sphere.Create(0, ColorLightEntity);
            
            this.sceneGraph = new SceneGraph(new EmptyEntity { Name = "DebugOverlayRoot" });
        }

        // --------------------------------------------------------------------
        // Enums
        // --------------------------------------------------------------------
        internal enum RenderingList
        {
            Entity = 10,
            UserInterface = 20
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
                this.debugController.Position = this.Camera.Camera.Position;
                this.debugController.Rotation = this.Camera.Camera.Rotation;
                this.debugController.IsActive = value;
            }
        }

        public bool EnableRendering { get; set; }

        public ICameraEntity Camera
        {
            get
            {
                return this.debugCamera;
            }
        }

        public void UpdateEntityData(IEnumerable<SceneEntityDebugEntry> entities)
        {
            this.ClearRenderingList((int)RenderingList.Entity);
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

            // Create and initialize the model loader
            this.modelEntityLoader = new ModelEntityLoader();
            this.modelEntityLoader.Initialize(graphic);

            // Initialize our camera's
            this.userInterfaceCamera = this.factory.Resolve<IOrthographicCamera>();
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

            this.debugCamera = new CameraEntity { Camera = this.factory.Resolve<IProjectionCamera>() };
            this.debugController = this.factory.Resolve<IFirstPersonController>();
            this.debugController.Initialize(graphic);
            this.debugController.SetInputBindings(InputManager.DefaultBindingDebugController);

            if (this.compassResource != null)
            {
                this.InitializeCompass();
            }
            else
            {
                this.log.Warning("Debug compass resource is not set");
            }

            // Now we need to register all the contents of the graph
            this.LinkEntity(this.sceneGraph.Root);
        }

        public override bool Update(ITimer gameTime)
        {
            this.debugController.Update(gameTime);
            this.userInterfaceCamera.Update(gameTime);

            // We are explicitly not calling update on this, will be done by the active scene if desired
            this.debugCamera.Position = this.debugController.Position;
            this.debugCamera.Rotation = this.debugController.Rotation;

            if (this.compass != null)
            {
                this.compass.Rotation = this.debugCamera.Rotation;
                this.InvalidateSceneEntity(this.compass);
            }

            return base.Update(gameTime);
        }

        public override void Render(IFrameManager frameManager)
        {
            if (!this.EnableRendering)
            {
                return;
            }

            // Render the entity data
            FrameInstructionSet set = frameManager.BeginSet(this.debugCamera.Camera);
            set.Technique = FrameTechnique.Plain;
            set.LightingEnabled = false;
            set.Topology = PrimitiveTopology.LineList;
            this.RenderList((int)RenderingList.Entity, set);
            frameManager.RenderSet(set);

            // User Interface as overlay on top
            set = frameManager.BeginSet(this.userInterfaceCamera);
            set.LightingEnabled = false;
            set.Technique = FrameTechnique.Forward;
            this.RenderList((int)RenderingList.UserInterface, set);
            frameManager.RenderSet(set);
        }

        public override void Resize(TypedVector2<int> size)
        {
            // Update the camera perspectives
            this.debugCamera.Camera.SetPerspective(size, 0.05f, 200.0f);
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

        public void SetDebugCompass(ModelResourceGroup resource)
        {
            if (this.compassResource != null || this.compass != null)
            {
                System.Diagnostics.Trace.TraceError("Error, compass resource set after initialize!");
                return;
            }

            this.compassResource = resource;
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
            BoundingBox boundingBox = source.BoundingBox ?? new BoundingBox(new Vector3(-1), new Vector3(1));

            ModelResource resource = Cube.CreateBoundingBoxLines(boundingBox, ColorModelEntity);
            var entity = new ModelEntity
            {
                Position = source.Position,
                Scale = new Vector3(1.1f),
                Rotation = source.Rotation,
                Mesh = new Mesh(resource),
                OverrideWorld = source.GetWorld()
            };

            this.LinkEntity(entity);
            this.AddToRenderingList(entity, (int)RenderingList.Entity);
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
                Mesh = new Mesh(this.lightEntityModel),
                OverrideWorld = source.GetWorld()
            };

            this.LinkEntity(entity);
            this.AddToRenderingList(entity, (int)RenderingList.Entity);
        }

        private void InitializeCompass()
        {
            ISceneGraph modelGraph = this.modelEntityLoader.LoadModelGroup(this.compassResource);
            this.compass = this.sceneGraph.AppendInto(modelGraph, "Compass");
            this.compass.Position = new Vector3(100);
            this.compass.Scale = new Vector3(50);
            this.AddToRenderingList(this.compass, (int)RenderingList.UserInterface);
        }
    }
}
