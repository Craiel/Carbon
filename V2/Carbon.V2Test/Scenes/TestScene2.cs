using System;
using System.Linq;

using Carbon.Engine.Contracts;
using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Contracts.Resource;
using Carbon.Engine.Contracts.Scene;
using Carbon.Engine.Contracts.UserInterface;
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
    public interface ITestScene2 : IScene
    {
    }

    public class TestScene2 : Scene, ITestScene2
    {
        private readonly ILog log;
        private readonly ICarbonGraphics graphics;
        private readonly IV2TestGameState gameState;

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

        private LuaInterface.Lua consoleContext;
        
        public override void Dispose()
        {
            this.controller.Dispose();
            this.camera.Dispose();
            this.overlayCamera.Dispose();
            this.console.Dispose();

            base.Dispose();
        }
        
        public TestScene2(IEngineFactory factory)
        {
            this.log = factory.Get<IApplicationLog>().AquireContextLog("TestScene2");
            this.graphics = factory.Get<ICarbonGraphics>();
            this.gameState = factory.Get<IV2TestGameState>();
            
            this.controller = factory.Get<IFirstPersonController>();
            this.camera = factory.Get<IProjectionCamera>();
            this.overlayCamera = factory.Get<IOrthographicCamera>();

            // Create a manual console for testing purpose
            this.console = factory.Get<IUserInterfaceConsole>();
        }

        public override void Unload()
        {
            base.Unload();

            this.console.IsActive = false;
            this.console.OnLineEntered -= this.OnConsoleLineEntered;
            this.console.OnRequestCompletion -= this.OnConsoleCompletionRequested;
        }
        
        public override void Initialize(ICarbonGraphics graphics)
        {
            base.Initialize(graphics);

            this.camera.Initialize(graphics);
            this.controller.Initialize(graphics);
            
            this.controller.Position = new Vector4(0, 5, -10, 1.0f);
            this.controller.Speed = 0.1f;
            
            var scriptData = this.gameState.ResourceManager.Load<RawResource>(HashUtils.BuildResourceHash(@"Scripts\init.lua"));
            var script = new CarbonScript(scriptData);
            this.gameState.ScriptingEngine.Execute(script);
            this.controller.SetInputBindings("worldmap_controls");
            this.controller.IsActive = true;

            this.consoleFont =
                this.gameState.ContentManager.TypedLoad(new ContentQuery<FontEntry>().IsEqual("Id", 1)).UniqueResult<FontEntry>();
            this.consoleTestNode = (IModelNode)this.gameState.NodeManager.AddStaticText(1, " ", new Vector2(1, 1.2f));
            this.gameState.NodeManager.RootNode.RemoveChild(this.consoleTestNode);
            this.consoleTestNode.Position = new Vector4(0, 20, 0, 1);
            this.console.Initialize(graphics);
            this.console.IsActive = true;
            this.console.IsVisible = true;
            this.console.OnLineEntered += this.OnConsoleLineEntered;
            this.console.OnRequestCompletion += this.OnConsoleCompletionRequested;
            
            // Setup the hard textures for internals
            this.forwardDebugTexture = new Material { DiffuseTexture = this.graphics.TextureManager.GetRegisterReference(1001) };
            this.normalDebugTexture = new Material { DiffuseTexture = this.graphics.TextureManager.GetRegisterReference(1002) };

            this.gBufferNormalTexture = new Material { DiffuseTexture = this.graphics.TextureManager.GetRegisterReference((int)StaticTextureRegister.GBufferNormal) };
            this.gBufferDiffuseAlbedoTexture = new Material { DiffuseTexture = this.graphics.TextureManager.GetRegisterReference((int)StaticTextureRegister.GBufferDiffuse) };
            this.gBufferSpecularAlbedoTexture = new Material { DiffuseTexture = this.graphics.TextureManager.GetRegisterReference((int)StaticTextureRegister.GBufferSpecular) };
            this.gBufferDepthTexture = new Material { DiffuseTexture = this.graphics.TextureManager.GetRegisterReference((int)StaticTextureRegister.GBufferDepth) };
            this.deferredLightTexture = new Material { DiffuseTexture = this.graphics.TextureManager.GetRegisterReference((int)StaticTextureRegister.DeferredLight) };

            scriptData = this.gameState.ResourceManager.Load<RawResource>(HashUtils.BuildResourceHash(@"Scripts\TestScene2.lua"));
            script = new CarbonScript(scriptData);
            this.gameState.ScriptingEngine.Execute(script);

            this.consoleContext = this.gameState.ScriptingEngine.GetContext();
        }

        private string OnConsoleCompletionRequested(string arg)
        {
            var matches = this.consoleContext.Globals.Where(x => x.StartsWith(arg)).ToList();
            if (matches.Count == 1)
            {
                return matches[0];
            }

            if (matches.Count > 0)
            {
                this.console.AddSystemLine(string.Join(", ", matches));
            }

            return arg;
        }

        private void OnConsoleLineEntered(string line)
        {
            try
            {
                object[] output = this.consoleContext.DoString(line);
                if (output != null)
                {
                    foreach (var o in output)
                    {
                        if (o is string)
                        {
                            continue;
                        }

                        this.console.AddLine((string)o);
                    }
                }
            }
            catch (Exception e)
            {
                string error = string.Format("Exception in Console script execution: {0}", e.Message);
                this.console.AddSystemLine(error);
                System.Diagnostics.Trace.TraceError(error);
            }
        }

        public override void Resize(TypedVector2<int> size)
        {
            this.camera.SetPerspective(size, 0.05f, 200.0f);
            this.overlayCamera.SetPerspective(size, 0.05f, 200.0f);

            this.screenQuad = new Mesh(Quad.CreateScreen(new Vector2(0), size));
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

            this.console.Update(gameTime);
            string consoleText = string.Join(Environment.NewLine, string.Join(Environment.NewLine, this.console.History), this.console.Text);
            if (this.lastConsoleUpdate != consoleText.GetHashCode() && !string.IsNullOrEmpty(consoleText))
            {
                var mesh =
                    new Mesh(
                        FontBuilder.Build(
                            consoleText, new Vector2(12f, 17f), this.consoleFont));
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

            this.gameState.NodeManager.RootNode.Update(gameTime);
        }

        public override void Render(IFrameManager frameManager)
        {
            // The scene to deferred
            FrameInstructionSet set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Deferred;
            this.gameState.NodeManager.RootNode.Render(set);
            frameManager.RenderSet(set);

            // The scene to Forward
            var windowSize = new TypedVector2<int>((int)this.graphics.WindowViewport.Width, (int)this.graphics.WindowViewport.Height);
            set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.Forward;
            set.DesiredTarget = RenderTargetDescription.Texture(1001, windowSize);
            this.gameState.NodeManager.RootNode.Render(set);
            frameManager.RenderSet(set);

            // The scene to Debug Normal
            set = frameManager.BeginSet(this.camera);
            set.Technique = FrameTechnique.DebugNormal;
            set.DesiredTarget = RenderTargetDescription.Texture(1002, windowSize);
            this.gameState.NodeManager.RootNode.Render(set);
            frameManager.RenderSet(set);
            
            this.RenderDebugScreens(frameManager);

            // The UI
            set = frameManager.BeginSet(this.overlayCamera);
            set.LightingEnabled = false;
            set.Technique = FrameTechnique.Forward;
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

        private void RenderDebugScreens(IFrameManager frameManager)
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
