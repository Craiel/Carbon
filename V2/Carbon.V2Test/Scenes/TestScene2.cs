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
using Carbon.Engine.Scene;
using Carbon.V2Test.Contracts;
using Core.Utils;
using Core.Utils.Contracts;
using SlimDX;

namespace Carbon.V2Test.Scenes
{
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

        private Material stoneMaterial;
        private Material checkerboardMaterial;
        private Material deferredLightTexture;
        private Material forwardDebugTexture;
        private Material normalDebugTexture;

        private Material gBufferNormalTexture;
        private Material gBufferDiffuseAlbedoTexture;
        private Material gBufferSpecularAlbedoTexture;
        private Material gBufferDepthTexture;

        private Material shadowMapTexture;
        
        private float testRotation;
        private LightNode lightTesting;
        private LightNode screenAmbient;
        private INode root;

        private readonly IFirstPersonController controller;
        private readonly IProjectionCamera camera;
        private readonly IOrthographicCamera overlayCamera;

        private INode hirarchyTestNode;
        private INode middleNode;

        private Mesh testQuad;
        private Mesh testMesh;
        
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

            this.resourceManager = factory.Get<IResourceManager>();
            this.contentManager = factory.GetContentManager(new ResourceLink { Source = "v2test_master.db" });
            
            this.root = new Node();
        }
        
        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.camera.Initialize(graphics);
            this.controller.Initialize(graphics);

            this.controller.Position = new Vector4(0, 5, -10, 1.0f);
            this.controller.Speed = 0.1f;

            // Ambient Light testing
            this.root.AddChild(new LightNode { Light = new Light { Type = LightType.Ambient, Color = new Vector4(0.2f) } });

            Light testLight;
            testLight = new Light { Color = new Vector4(1, 1, 0.5f, 1), Direction = new Vector3(0.5f, -1, 1), Type = LightType.Direction, SpecularPower = 1};
            this.root.AddChild(new LightNode { Light = testLight });

            testLight = new Light { Color = new Vector4(1, 1, 1, 0.2f), Direction = new Vector3(-1), Type = LightType.Direction, SpecularPower = 1 };
            this.root.AddChild(new LightNode { Light = testLight });
            
            testLight = new Light { Color = new Vector4(0.5f, 1.0f, 1.0f, 1.0f), Type = LightType.Point, Range = 20.0f, SpecularPower = 10.0f };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(5, 1, 0, 1) });

            testLight = new Light { Color = new Vector4(1.0f), Type = LightType.Point, Range = 20.0f, SpecularPower = 10.0f };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(15, 1, 0, 1) });

            testLight = new Light { Color = new Vector4(1.0f), Type = LightType.Point, Range = 20.0f, SpecularPower = 10.0f };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(10f, 1, 5f, 1) });

            testLight = new Light { Color = new Vector4(1.0f, 1.0f, 0.5f, 1.0f), Type = LightType.Point, Range = 10.0f, SpecularPower = 10.0f };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(11f, 1, 5f, 1) });

            testLight = new Light { Color = new Vector4(1), Type = LightType.Spot, Range = 20.0f, SpecularPower = 10.0f, Direction = -Vector3.UnitY, SpotAngles = new Vector2(MathExtension.DegreesToRadians(50), MathExtension.DegreesToRadians(90)) };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(5f, 7, 20f, 1) });

            testLight = new Light { Color = new Vector4(1), Type = LightType.Spot, Range = 10.0f, SpecularPower = 10.0f, Direction = Vector3.UnitY, SpotAngles = new Vector2(5, 10) };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(15f, 5, 20f, 1) });

            testLight = new Light { Color = new Vector4(1), Type = LightType.Spot, Range = 10.0f, SpecularPower = 10.0f, Direction = Vector3.UnitY, SpotAngles = new Vector2(5, 10) };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(25f, 2, 20f, 1) });

            testLight = new Light { Color = new Vector4(1), Type = LightType.Spot, Range = 10.0f, SpecularPower = 10.0f, Direction = Vector3.UnitY, SpotAngles = new Vector2(5, 10) };
            this.root.AddChild(new LightNode { Light = testLight, Position = new Vector4(35f, 0.5f, 20f, 1) });

            var materialResource = new MaterialEntry { DiffuseTexture = new ResourceLink { Source = @"..\SourceData\Textures\checkerboard.dds" } };
            this.contentManager.Save(materialResource);

            this.checkerboardMaterial = new Material(graphics, materialResource);
            this.stoneMaterial = new Material(
                graphics,
                new MaterialEntry { DiffuseTexture = new ResourceLink { Source = @"Textures\stone.dds" } });
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

            Mesh sphere = new Mesh(Sphere.Create(3)) { AllowInstancing = false };
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(0), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(0, 10, 0, 1), Material = this.stoneMaterial });
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(6, 10, 6, 1), Material = this.stoneMaterial });
            this.root.AddChild(new ModelNode { Mesh = sphere, Position = new Vector4(-6, 4, -6, 1), Material = this.checkerboardMaterial });

            /*Mesh quad2 = Quad.Create(new Vector3(0), -Vector3.UnitZ, Vector3.UnitY, 100, 100);
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(8, 0, 2, 1), Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(90)), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(-90)), Material = this.checkerboardMaterial });
            this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 8, 2, 1), Rotation = Quaternion.RotationAxis(Vector3.UnitX, MathExtension.DegreesToRadians(-90)), Material = this.checkerboardMaterial });*/
            //this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Material = this.checkerboardMaterial });
            //this.root.AddChild(new ModelNode { Mesh = quad2, Position = new Vector4(0, 0, 2, 1), Material = this.checkerboardMaterial });

            // Instance test
            Mesh instanceMesh = new Mesh(Cube.Create(new Vector3(0), 2)) { AllowInstancing = true };
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
            }

            /*this.testFont = new FontBuilder();
            Mesh mesh = this.testFont.Build("test font 123\nsecond line\nthird line\n  < - > 1234567890\nabc-def-ghi-jkl-mno-pqr-stu-vwx-yz\nThe Big Brown Fox jumped over the hedge!?", new Vector2(0.1f, 0.2f));
            TextureReference reference = TextureReference.NewFile(@"Textures\font1.dds");
            Material fontMaterial = new Material { DiffuseTexture = reference, AlphaTexture = reference, Color = new Vector4(1.0f, 0, 0, 1.0f) };
            var fontNode = new ModelNode { Mesh = mesh, Material = fontMaterial };
            fontNode.Position = new Vector4(5, 20, 5, 1.0f);
            fontNode.Scale = new Vector3(2.0f);
            this.root.AddChild(fontNode);*/

            Mesh cone = new Mesh(Cone.Create(20)) { AllowInstancing = true };
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
            }

            //this.testMesh = Quad.CreateScreen(new Vector2(0), new Vector2(1));
            //this.testMesh = Cube.Create(new Vector3(0, 0, 10), 10);

            /*Mesh cube = Cube.Create(new Vector3(0), 2);
            this.root.AddChild(new ModelNode { Mesh = cube, Position = new Vector4(50, 10, 2, 1), Scale = new Vector3(5, 5, 5), Material = this.checkerboardMaterial });
            */
            Mesh quad = new Mesh(Quad.Create(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ, 10.0f, 10.0f));
            this.root.AddChild(new ModelNode { Mesh = quad, Scale = new Vector3(50, 1, 50), Material = this.checkerboardMaterial });

            //this.contentManager.Save(materialResource);
            var testCriteria = new ContentQuery<MaterialEntry>().IsEqual("Id", 0);
            this.contentManager.Load(testCriteria).UniqueResult<MaterialEntry>();

            /*RawResource resource;
            resource = this.resourceManager.Load<RawResource>(@"Models\room.dae");
            if (resource != null)
            {
                ColladaModel testModel = ColladaModel.Load(resource.Data);
                ColladaCarbonConverter.Convert("room", testModel);
                node.Material = this.defaultMaterial;
                node.Rotation = Quaternion.RotationAxis(Vector3.UnitX, MathExtension.DegreesToRadians(-90));
                node.Position = new Vector4(25, 3, 10, 1);
                this.root.AddChild(node);
            }*/

            /*resource = this.resourceManager.Load<RawResource>(@"Models\sponza_vase.dae");
            if (resource != null)
            {
                ColladaModel testModel = ColladaModel.Load(resource.Data);
                ColladaCarbonConverter.Convert("sponza", testModel);
                node.Material = this.redColorMaterial;
                node.Scale = new Vector3(0.02f);
                //node.Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(30));
                node.Position = new Vector4(6, 0, 0, 1);
                this.root.AddChild(node);
            }*/

            /*resource = this.resourceManager.Load<RawResource>(@"Models\house6.dae");
            if (resource != null)
            {
                ColladaModel testModel = ColladaModel.Load(resource.Data);
                ModelNode node = ColladaCarbonConverter.Convert("house6", testModel, this.checkerboardMaterial);
                //node.Material = this.redColorMaterial;
                //node.Scale = new Vector3(0.01f);
                //node.Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(30));
                node.Position = new Vector4(0, 0, 0, 1);
                this.root.AddChild(node);
            }*/

            /*resource = this.resourceManager.Load<RawResource>(@"Models\character.dae");
            if (resource != null)
            {
                ColladaModel testModel = ColladaModel.Load(resource.Data);
                ModelNode node = ColladaCarbonConverter.Convert("character", testModel);
                //node.Material = this.redColorMaterial;
                node.Scale = new Vector3(4f);
                node.Rotation = Quaternion.RotationYawPitchRoll(MathExtension.DegreesToRadians(-180), MathExtension.DegreesToRadians(-90), 0);
                node.Position = new Vector4(5, 1, 15, 1);
                this.root.AddChild(node);
            }

            resource = this.resourceManager.Load<RawResource>(@"Models\dodge.dae");
            if (resource != null)
            {
                ColladaModel testModel = ColladaModel.Load(resource.Data);
                ModelNode node = ColladaCarbonConverter.Convert("dodge", testModel);
                //node.Material = this.redColorMaterial;
                //node.Scale = new Vector3(4f);
                node.Rotation = Quaternion.RotationYawPitchRoll(MathExtension.DegreesToRadians(-180), MathExtension.DegreesToRadians(-90), 0);
                node.Position = new Vector4(7, 1, 0, 1);
                this.root.AddChild(node);
            }

            resource = this.resourceManager.Load<RawResource>(@"Models\test_rotation.dae");
            if (resource != null)
            {
                ColladaModel testModel = ColladaModel.Load(resource.Data);
                ModelNode node = ColladaCarbonConverter.Convert("test_rotation", testModel);
                //node.Material = this.redColorMaterial;
                node.Scale = new Vector3(0.2f);
                //node.Rotation = Quaternion.RotationYawPitchRoll(MathExtension.DegreesToRadians(-180), MathExtension.DegreesToRadians(-90), 0);
                node.Position = new Vector4(-10, 1, -30, 1);
                this.root.AddChild(node);
            }*/
        }

        public override void Resize(int width, int height)
        {
            this.camera.SetPerspective(width, height, 0.01f, 2000.0f);
            this.overlayCamera.SetPerspective(width, height, 0.01f, 2000.0f);

            this.testQuad = new Mesh(Quad.CreateScreen(new Vector2(0), new Vector2(width, height)));
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

            this.testRotation += 0.01f;
            if (this.testRotation > 360)
            {
                this.testRotation = 0;
            }

            /*this.testRotation += 0.01f;
            this.hirarchyTestNode.Rotation = Quaternion.RotationAxis(Vector3.UnitY, MathExtension.DegreesToRadians(this.testRotation));
            this.middleNode.Rotation = Quaternion.RotationAxis(Vector3.UnitX, MathExtension.DegreesToRadians(this.testRotation));*/

            /*lightTesting.Position -= new Vector4(0f, 0.005f, 0, 1);
            if (lightTesting.Position.Y <= 1)
            {
                lightTesting.Position = new Vector4(5f, 7.0f, 0, 1);
            }*/

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
            set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Forward;
            set.DesiredTarget = RenderTargetDescription.Texture(1001, 1024, 1024);
            this.root.Render(set);
            frameManager.RenderSet(set);

            // The scene to Debug Normal
            set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.DebugNormal;
            set.DesiredTarget = RenderTargetDescription.Texture(1002, 1024, 1024);
            this.root.Render(set);
            frameManager.RenderSet(set);
            
            this.RenderDebugScreens();

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
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(0, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.gBufferDiffuseAlbedoTexture,
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(this.graphics.WindowViewport.Width * scale + 10, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.gBufferSpecularAlbedoTexture,
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation((this.graphics.WindowViewport.Width * scale) * 2 + 20, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.deferredLightTexture,
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation((this.graphics.WindowViewport.Width * scale) * 3 + 30, this.graphics.WindowViewport.Height - this.graphics.WindowViewport.Height * scale, 0)
                });

            // Second Row
            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.forwardDebugTexture,
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(0, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.normalDebugTexture,
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation(this.graphics.WindowViewport.Width * scale + 10, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });

            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.shadowMapTexture,
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation((this.graphics.WindowViewport.Width * scale) * 2 + 20, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });

            frameManager.RenderSet(set);

            // Render the depth
            set = frameManager.BeginSet(this.overlayCamera);
            set.Technique = FrameTechnique.DebugDepth;
            set.Instructions.Add(
                new FrameInstruction
                {
                    Material = this.gBufferDepthTexture,
                    Mesh = this.testQuad,
                    World = Matrix.Scaling(new Vector3(scale)) * Matrix.Translation((this.graphics.WindowViewport.Width * scale) * 3 + 30, this.graphics.WindowViewport.Height - (this.graphics.WindowViewport.Height * scale) * 2 - 10, 0)
                });
            frameManager.RenderSet(set);
        }
    }
}
