using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;
using Carbon.Engine.Rendering.Primitives;
using Carbon.Engine.Rendering.RenderTarget;
using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Content;
using Carbon.Engine.Resource.Resources;
using Carbon.Engine.Scene;
using Carbon.V2Test.Contracts;
using Core.Utils;
using Core.Utils.Contracts;
using SlimDX;

namespace Carbon.V2Test.Scenes
{
    using System;

    using Carbon.Editor.Resource.Collada;

    public interface ITestSceneSponza : IScene
    {
    }

    public class TestSceneSponza : Scene, ITestSceneSponza
    {
        private readonly ILog log;
        private readonly IResourceManager resourceManager;
        private readonly IFrameManager frameManager;
        private readonly ICarbonGraphics graphics;

        private Material checkerboardMaterial;
        private Material lightPointMaterial;
        private Material deferredLightTexture;
        private Material forwardDebugTexture;
        private Material normalDebugTexture;

        private Material gBufferNormalTexture;
        private Material gBufferDiffuseAlbedoTexture;
        private Material gBufferSpecularAlbedoTexture;
        private Material gBufferDepthTexture;
        
        private INode root;

        private Mesh screenQuad;

        private TimeSpan lightUpdateTime;
        private float sunRotation;
        private LightNode sunLight;

        private readonly IFirstPersonController controller;
        private readonly IProjectionCamera camera;
        private readonly IOrthographicCamera overlayCamera;
        
        public override void Dispose()
        {
            this.controller.Dispose();
            this.camera.Dispose();

            base.Dispose();
        }

        public TestSceneSponza(IEngineFactory factory)
        {
            this.log = factory.Get<IApplicationLog>().AquireContextLog("TestSceneSponza");
            this.frameManager = factory.Get<IFrameManager>();
            this.graphics = factory.Get<ICarbonGraphics>();

            this.controller = factory.Get<IFirstPersonController>();
            this.camera = factory.Get<IProjectionCamera>();
            this.overlayCamera = factory.Get<IOrthographicCamera>();

            this.resourceManager = factory.Get<IResourceManager>();

            this.root = new Node();
        }

        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.camera.Initialize(graphics);
            this.controller.Initialize(graphics);

            this.controller.Position = new Vector4(0, 5, -10, 1.0f);
            this.controller.Speed = 0.1f;

            Light light;
            LightNode lightNode;
            Mesh lightPointMesh = new Mesh(Sphere.Create(1));

            // Directional Sunlight
            light = new Light { Color = new Vector4(1, 1, 0.5f, 1), Direction = new Vector3(0.5f, -1, 1), Type = LightType.Direction };
            this.sunLight = new LightNode { Light = light };
            this.root.AddChild(this.sunLight);

            // Point Light Grid
            Vector4 pos = new Vector4(-10.0f, 1.0f, -10.0f, 1.0f);
            for (int x = 0; x < 10; x++)
            {
                pos.X += 4f;
                pos.Z = -10.0f;
                for (int y = 0; y < 10; y++)
                {
                    pos.Z += 4f;
                    light = new Light { Color = new Vector4(1.0f, 0.4f, 1.0f, 1.0f), Type = LightType.Point, Range = 10.0f, SpecularPower = 10.0f };
                    lightNode = new LightNode { Light = light, Position = pos };
                    this.root.AddChild(new ModelNode { Mesh = lightPointMesh, Scale = new Vector3(0.1f), Position = lightNode.Position, Material = this.lightPointMaterial });
                    this.root.AddChild(lightNode);
                }
            }

            var materialResource = new MaterialEntry { DiffuseTexture = new ResourceLink { Path = @"Textures\checkerboard.dds" } };
            this.checkerboardMaterial = new Material(graphics, materialResource);
            this.forwardDebugTexture = new Material(this.graphics.TextureManager.GetRegisterReference(1001));
            this.normalDebugTexture = new Material(this.graphics.TextureManager.GetRegisterReference(1002));

            this.gBufferNormalTexture = new Material(this.graphics.TextureManager.GetRegisterReference(11));
            this.gBufferDiffuseAlbedoTexture = new Material(this.graphics.TextureManager.GetRegisterReference(12));
            this.gBufferSpecularAlbedoTexture = new Material(this.graphics.TextureManager.GetRegisterReference(13));
            this.gBufferDepthTexture = new Material(this.graphics.TextureManager.GetRegisterReference(14));
            this.deferredLightTexture = new Material(this.graphics.TextureManager.GetRegisterReference(15));

            var source = new ResourceLink { Path = @"Models\sponza.dae" };
            var resource = this.resourceManager.Load<RawResource>(ref source);
            if (resource != null)
            {
                /*ColladaModel testModel = ColladaModel.Load(resource.Data);
                ColladaCarbonConverter.Convert("sponza", testModel);
                node.Material = this.redColorMaterial;
                node.Scale = new Vector3(0.01f);
                //node.Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(30));
                node.Position = new Vector4(0, 0, 0, 1);
                this.root.AddChild(node);*/
            }
        }

        public override void Resize(int width, int height)
        {
            this.camera.SetPerspective(width, height, 0.01f, 2000.0f);
            this.overlayCamera.SetPerspective(width, height, 0.01f, 2000.0f);

            this.screenQuad = new Mesh(Quad.CreateScreen(new Vector2(0), new Vector2(width, height)));
        }

        public override void Update(ITimer gameTime)
        {
            // Update the controller first
            this.controller.Update(gameTime);

            // Set the camera to the controller state and update
            this.camera.Position = this.controller.Position;
            this.camera.Rotation = this.controller.Rotation;
            this.camera.Update(gameTime);

            this.overlayCamera.Update(gameTime);

            this.lightUpdateTime += gameTime.ElapsedTime;
            if (this.lightUpdateTime > TimeSpan.FromMilliseconds(100))
            {
                this.lightUpdateTime = TimeSpan.Zero;
                this.sunRotation += 0.1f;
                if (this.sunRotation >= 360)
                {
                    this.sunRotation = 0.0f;
                }

                Vector2 pos;
                pos.X = (float)(0 + 160.0f * Math.Cos(MathExtension.DegreesToRadians(this.sunRotation)));
                pos.Y = (float)(0 - 160 * Math.Sin(MathExtension.DegreesToRadians(this.sunRotation)));
                Vector2 dir = Vector2.Normalize(pos - new Vector2(0));
                this.sunLight.Light.Direction = new Vector3(dir, 0);
            }

            this.root.Update(gameTime);
        }

        public override void Render()
        {
            // The scene to deferred
            FrameInstructionSet set = this.frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Deferred;
            this.root.Render(set);
            frameManager.RenderSet(set);

            // The scene to Forward
            /*set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Forward;
            set.DesiredTarget = RenderTargetDescription.Texture(1, 1024, 1024);
            this.root.Render(set);
            frameManager.RenderSet(set);*/

            // The scene to Debug Normal
            /*set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.DebugNormal;
            set.DesiredTarget = RenderTargetDescription.Texture(2, 1024, 1024);
            this.root.Render(set);
            frameManager.RenderSet(set);*/

            this.RenderDebugScreens();
        }

        private void RenderDebugScreens()
        {
            float scale = 0.2f;
            var set = frameManager.BeginSet(this.overlayCamera);
            set.LightingEnabled = false;

            // Top Row
            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.gBufferNormalTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(0, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.gBufferDiffuseAlbedoTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(this.graphics.WindowViewport.Width * scale + 10, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.gBufferSpecularAlbedoTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation((this.graphics.WindowViewport.Width * scale) * 2 + 20, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.deferredLightTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation((this.graphics.WindowViewport.Width * scale) * 3 + 30, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            // Second Row
            /*set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.forwardDebugTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(0, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });*/

            /*set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.normalDebugTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(this.graphics.WindowViewport.Width * scale + 10, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });*/

            frameManager.RenderSet(set);

            // Render the depth
            set = frameManager.BeginSet(this.overlayCamera);
            set.Technique = FrameTechnique.DebugDepth;
            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.gBufferDepthTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation((this.graphics.WindowViewport.Width * scale) * 3 + 30, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });
            frameManager.RenderSet(set);
        }
    }
}
