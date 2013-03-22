using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Logic;
using Carbon.Engine.Logic.Scripting;
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

    using Carbon.Engine.UserInterface;

    public interface ITestScene : IScene
    {
    }

    public class TestScene2 : Scene, ITestScene
    {
        private readonly ILog log;
        private readonly IResourceManager resourceManager;
        private readonly IContentManager contentManager;
        private readonly IFrameManager frameManager;
        private readonly IRenderer renderer;
        private readonly ICarbonGraphics graphics;
        private readonly IScriptingEngine scriptingEngine;

        private INodeManager nodeManager;
        
        private Material deferredLightTexture;
        private Material forwardDebugTexture;
        private Material normalDebugTexture;

        private Material gBufferNormalTexture;
        private Material gBufferDiffuseAlbedoTexture;
        private Material gBufferSpecularAlbedoTexture;
        private Material gBufferDepthTexture;
        
        private readonly IFirstPersonController controller;
        private readonly IProjectionCamera camera;
        private readonly IOrthographicCamera overlayCamera;

        private IUserInterfaceConsole console;

        private FontEntry consoleFont;
        private IModelNode consoleTestNode;
        private int lastConsoleUpdate;

        private Mesh screenQuad;
        
        public override void Dispose()
        {
            this.controller.Dispose();
            this.camera.Dispose();            

            base.Dispose();
        }
        
        public TestScene2(IEngineFactory factory)
        {
            this.log = factory.Get<IApplicationLog>().AquireContextLog("TestScene");
            this.frameManager = factory.Get<IFrameManager>();
            this.renderer = factory.Get<IRenderer>();
            this.graphics = factory.Get<ICarbonGraphics>();
            
            this.controller = factory.Get<IFirstPersonController>();
            this.camera = factory.Get<IProjectionCamera>();
            this.overlayCamera = factory.Get<IOrthographicCamera>();

            this.resourceManager = factory.GetResourceManager("Data");
            this.contentManager = factory.GetContentManager(this.resourceManager, "Main.db");
            
            // Setup the basic scripting environment for the scene
            this.scriptingEngine = factory.Get<IScriptingEngine>();
            this.scriptingEngine.Register(new ScriptingCoreProvider(factory.Get<IApplicationLog>()));

            // Create a manual console for testing purpose
            this.console = factory.Get<IUserInterfaceConsole>();
        }
        
        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.camera.Initialize(graphics);
            this.controller.Initialize(graphics);
            
            this.controller.Position = new Vector4(0, 5, -10, 1.0f);
            this.controller.Speed = 0.1f;

            // Setup additional scripting and managers
            this.nodeManager = new NodeManager(graphics, this.contentManager, this.resourceManager);
            this.scriptingEngine.Register(this.nodeManager);

            var testScriptData = this.resourceManager.Load<RawResource>(HashUtils.BuildResourceHash(@"Scripts\init.lua"));
            var testScript = new CarbonScript(testScriptData);
            this.scriptingEngine.Execute(testScript);

            this.consoleFont =
                this.contentManager.TypedLoad(new ContentQuery<FontEntry>().IsEqual("Id", 4)).UniqueResult<FontEntry>();
            this.consoleTestNode = (IModelNode)this.nodeManager.AddStaticText(4, " ", new Vector2(1, 1.2f));
            this.nodeManager.RootNode.RemoveChild(this.consoleTestNode);
            this.consoleTestNode.Position = new Vector4(0, 20, 0, 1);
            this.console.Initialize(graphics);
            this.console.IsActive = true;
            this.console.IsVisible = true;
            
            // Setup the hard textures for internals
            this.forwardDebugTexture = new Material(this.graphics.TextureManager.GetRegisterReference(1001));
            this.normalDebugTexture = new Material(this.graphics.TextureManager.GetRegisterReference(1002));

            this.gBufferNormalTexture = new Material(this.graphics.TextureManager.GetRegisterReference(11));
            this.gBufferDiffuseAlbedoTexture = new Material(this.graphics.TextureManager.GetRegisterReference(12));
            this.gBufferSpecularAlbedoTexture = new Material(this.graphics.TextureManager.GetRegisterReference(13));
            this.gBufferDepthTexture = new Material(this.graphics.TextureManager.GetRegisterReference(14));
            this.deferredLightTexture = new Material(this.graphics.TextureManager.GetRegisterReference(15));

            /*PositionNormalVertex[] meshData;
            uint[] indices;
            Mesh cube = this.CreateMesh();
            cube.SetData(Cube.Data, Cube.Indices);
            this.root.AddChild(new ModelNode { Mesh = cube, Position = new Vector3(5, 10, 2)});*/

            /*Mesh sphere = new Mesh(Sphere.Create(3)) { AllowInstancing = false };
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(0), Material = testMaterial });
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(0, 10, 0, 1), Material = testMaterial });
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(6, 10, 6, 1), Material = testMaterial });
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(-6, 4, -6, 1), Material = testMaterial });*/

            /*Mesh quad2 = Quad.Create(new Vector3(0), -Vector3.UnitZ, Vector3.UnitY, 100, 100);
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(8, 0, 2, 1), Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(90)), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(-90)), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 8, 2, 1), Rotation = Quaternion.RotationAxis(Vector3.UnitX, MathExtension.DegreesToRadians(-90)), Material = this.checkerboardMaterial });*/
            //this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Material = this.checkerboardMaterial });
            //this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Material = this.checkerboardMaterial });

            // Instance test
            /*Mesh instanceMesh = new Mesh(Cube.Create(new Vector3(0), 2)) { AllowInstancing = true };
            Vector3 offset = new Vector3(-50, 5, 50);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        this.root.AddChild(
                            new ModelNode
                                {
                                    Mesh = instanceMesh,
                                    Position = new Vector4(offset + new Vector3(x * 5.0f, y * 5.0f, z * 5.0f), 1.0f),
                                    Material = this.stoneMaterial,
                                    Scale = new Vector3(0.2f)
                                });
                    }
                }
            }*/

            /*this.testFont = new FontBuilder();
            Mesh mesh = this.testFont.Build("test font 123\nsecond line\nthird line\n  < - > 1234567890\nabc-def-ghi-jkl-mno-pqr-stu-vwx-yz\nThe Big Brown Fox jumped over the hedge!?", new Vector2(0.1f, 0.2f));
            TextureReference reference = TextureReference.NewFile(@"Textures\font1.dds");
            Material fontMaterial = new Material { DiffuseTexture = reference, AlphaTexture = reference, Color = new Vector4(1.0f, 0, 0, 1.0f) };
            var fontNode = new ModelNode { Mesh = mesh, Material = fontMaterial };
            fontNode.Position = new Vector4(5, 20, 5, 1.0f);
            fontNode.Scale = new Vector3(2.0f);
            this.root.AddChild(fontNode);*/

            /*Mesh cone = new Mesh(Cone.Create(20)) { AllowInstancing = true };
            this.root.AddChild(new ModelNode { Mesh = cone, Position = new Vector4(10, 2, 14, 1), Material = this.stoneMaterial });
            this.root.AddChild(new ModelNode { Mesh = cone, Position = new Vector4(14, 2, 10, 1), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = cone, Position = new Vector4(-16, 2, 8, 1) });
            this.root.AddChild(new ModelNode { Mesh = cone, Position = new Vector4(-18, 1, 10, 1), Rotation = Quaternion.RotationAxis(Vector3.UnitZ, MathExtension.DegreesToRadians(45)), Material = this.stoneMaterial });

            this.hirarchyTestNode = new ModelNode { Mesh = cone, Position = new Vector4(10, 1, -10, 1), Scale = new Vector3(0.5f), Material = this.stoneMaterial };
            INode parent = this.hirarchyTestNode;
            this.root.AddChild(this.hirarchyTestNode);
            for (int i = 0; i < 50; i++)
            {
                INode child = new ModelNode { Mesh = cone, Position = new Vector4(1, 1, 0, 1), Material = this.stoneMaterial };
                if (i == 25)
                {
                    this.middleNode = child;
                }

                parent.AddChild(child);
                parent = child;
            }*/

            //this.testMesh = Quad.CreateScreen(new Vector2(0), new Vector2(1));
            //this.testMesh = Cube.Create(new Vector3(0, 0, 10), 10);

            /*Mesh cube = Cube.Create(new Vector3(0), 2);
            this.root.AddChild(new ModelNode { Mesh = cube, Position = new Vector4(50, 10, 2, 1), Scale = new Vector3(5, 5, 5), Material = this.checkerboardMaterial });
            */
            /*Mesh quad = new Mesh(Quad.Create(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ, 10.0f, 10.0f));
            this.root.AddChild(new ModelNode { Mesh = quad, Scale = new Vector3(50, 1, 50), Material = this.checkerboardMaterial });
            */
        }

        public override void Resize(int width, int height)
        {
            this.camera.SetPerspective(width, height, 0.05f, 200.0f);
            this.overlayCamera.SetPerspective(width, height, 0.05f, 200.0f);

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

            string consoleText = string.Join(Environment.NewLine, this.console.Text);
            if (this.lastConsoleUpdate != consoleText.GetHashCode() && !string.IsNullOrEmpty(consoleText))
            {
                var mesh =
                    new Mesh(
                        FontBuilder.Build(
                            string.Join(Environment.NewLine, this.console.Text), new Vector2(10f, 11f), this.consoleFont));
                this.consoleTestNode.Mesh = mesh;
                this.lastConsoleUpdate = consoleText.GetHashCode();
            }
            this.consoleTestNode.Update(gameTime);
            
            /*this.testRotation += 0.01f;
            this.hirarchyTestNode.Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(this.testRotation));
            this.middleNode.Rotation = Quaternion.RotationAxis(Vector3.UnitX, MathExtension.DegreesToRadians(this.testRotation));*/

            /*lightTesting.Position -= new Vector4(0f, 0.005f, 0, 1);
            if (lightTesting.Position.Y <= 1)
            {
                lightTesting.Position = new Vector4(5f, 7.0f, 0, 1);
            }*/

            this.nodeManager.RootNode.Update(gameTime);
        }

        public override void Render()
        {
            // The scene to deferred
            FrameInstructionSet set = this.frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Deferred;
            this.nodeManager.RootNode.Render(set);
            frameManager.RenderSet(set);

            // The scene to Forward
            set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Forward;
            set.DesiredTarget = RenderTargetDescription.Texture(1001, 1024, 1024);
            this.nodeManager.RootNode.Render(set);
            frameManager.RenderSet(set);

            // The scene to Debug Normal
            set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.DebugNormal;
            set.DesiredTarget = RenderTargetDescription.Texture(1002, 1024, 1024);
            this.nodeManager.RootNode.Render(set);
            frameManager.RenderSet(set);
            
            this.RenderDebugScreens();

            // The UI
            set = frameManager.BeginSet(this.overlayCamera);
            set.LightingEnabled = false;
            //set.Technique = FrameTechnique.Forward;
            this.consoleTestNode.Render(set);
            frameManager.RenderSet(set);

            /*set = frameManager.BeginSet(this.overlayCamera);
            set.LightingEnabled = false;
                IList<FrameStatistics> stats = renderer.FrameStatistics;
                for (int i = 0; i < stats.Count; i++)
                {
                    set.Instructions.Add(
                      new FrameInstruction
                      {
                          Material = this.stoneMaterial,
                          Mesh = this.testMesh,
                          World = Matrix.Scaling(2, (float)stats[i].Duration * 4, 2) * Matrix.Translation(10+(i*3), 5, 0)
                      });
                }

            frameManager.RenderSet(set);*/
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
            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.forwardDebugTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(0, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.normalDebugTexture,
                    Mesh = this.screenQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(this.graphics.WindowViewport.Width * scale + 10, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });

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
