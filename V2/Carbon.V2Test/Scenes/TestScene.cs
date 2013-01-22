using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Rendering.Primitives;
using Carbon.Engine.Scene;
using Carbon.V2Test.Contracts;
using Core.Utils.Contracts;
using Carbon.Engine.Contracts;
using Carbon.Engine.Rendering;
using Carbon.Engine.Logic;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace Carbon.V2Test.Scenes
{
    public interface ITestScene : IScene
    {
    }

    public class TestScene : Scene, ITestScene
    {
        private readonly ILog log;
        private readonly ICarbonGraphics graphics;

        private TimeSpan lastProcessingTime;
        private TimeSpan processingInterval = TimeSpan.FromMilliseconds(100);
        private TimeSpan lastLevelingTime;

        private IFirstPersonController controller;
        private ICamera camera;

        private int level;
        private double experience;
        private double nextLevel;

        private float progress;
        private float progressSpeed = 1.0f;

        private Random random;

        private IMesh mesh;
        private IShader shader;
        private SlimDX.Direct3D11.Buffer cameraConstantBuffer;
        private SlimDX.Direct3D11.Buffer resizeConstantBuffer;
        private SlimDX.Direct3D11.Buffer perFrameConstantBuffer;

        public TestScene(IEngineFactory factory)
        {
            this.controller = factory.Get<IFirstPersonController>();
            this.camera = factory.Get<ICamera>();
            this.graphics = factory.Get<ICarbonGraphics>();

            this.log = factory.Get<IApplicationLog>().AquireContextLog("TestScene");

            this.random = new Random((int)DateTime.Now.Ticks);
        }

        public override void Initialize()
        {
            this.mesh = this.graphics.CreateMesh();
            this.mesh.SetVertices(Cube.Data, PrimitiveTopology.TriangleList);
            this.mesh.SetIndices(Cube.Indices);

            var bytecode = ShaderBytecode.CompileFromFile("Color.fx", "fx_5_0", ShaderFlags.Debug, EffectFlags.None);
            var effect = new Effect(this.graphics.Context.Device, bytecode);
            this.shader = this.graphics.CreateShader();
            this.shader.SetEffect(effect);

            this.InitializeCamera(this.graphics.Context);

            base.Initialize();
        }

        public override void Update(ITimer gameTime)
        {
            this.controller.Update(gameTime);

            if (gameTime.ElapsedTime - this.lastProcessingTime > this.processingInterval)
            {
                this.UpdateProgress();
                
                this.lastProcessingTime = gameTime.ElapsedTime;
            }

            // Now update final bits
            this.camera.Position = this.controller.Position;
            this.camera.Rotation = this.controller.Rotation;
            this.camera.Update(gameTime);
        }

        private void InitializeCamera(CarbonDeviceContext context)
        {
            // Temporary garbage to get their sizes.
            ConstantBufferCamera perCamera;
            ConstantBufferResize perResize;
            ConstantBufferFrame perFrame;

            perCamera.View = Matrix.Identity;
            perResize.Projection = Matrix.Identity;
            perFrame.World = Matrix.Identity;

            // Create the constant buffers.
            BufferDescription bd = new BufferDescription();
            bd.Usage = ResourceUsage.Default;
            bd.SizeInBytes = Marshal.SizeOf(perCamera);
            bd.BindFlags = BindFlags.ConstantBuffer;
            bd.CpuAccessFlags = CpuAccessFlags.None;

            this.cameraConstantBuffer = new SlimDX.Direct3D11.Buffer(context.Device, bd);

            bd.SizeInBytes = Marshal.SizeOf(perResize);
            this.resizeConstantBuffer = new SlimDX.Direct3D11.Buffer(context.Device, bd);

            //bd.SizeInBytes = Marshal.SizeOf(perFrame);
            //this.perFrameConstantBuffer = new SlimDX.Direct3D11.Buffer(context.Device, bd);

            // Initialize the world matrix.
            var world = Matrix.Identity;

            // Initialize the view matrix.
            Vector3 eye = new Vector3(0.0f, 3.0f, -6.0f);
            Vector3 at = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
            var view = Matrix.LookAtLH(eye, at, up);

            perCamera.View = Matrix.Transpose(view);
            using (DataStream data = new DataStream(Marshal.SizeOf(perCamera), true, true))
            {
                data.Write(perCamera);
                data.Position = 0;
                context.Device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, data), this.cameraConstantBuffer, 0);
            }

            // Initialize the projection matrix.
            var projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)800 / (float)600, 0.01f, 100.0f);

            perResize.Projection = Matrix.Transpose(projection);
            using (DataStream data = new DataStream(Marshal.SizeOf(perResize), true, true))
            {
                data.Write(perResize);
                data.Position = 0;
                context.Device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, data), this.resizeConstantBuffer, 0);
            }
        }

        public override void Render(CarbonDeviceContext context)
        {
            var world = Matrix.Identity;
            ConstantBufferFrame perFrame;
            perFrame.World = Matrix.Transpose(world);
            //perFrame.MeshColor = meshColor;
            /*using (DataStream data = new DataStream(Marshal.SizeOf(perFrame), true, true))
            {
                data.Write(perFrame);
                data.Position = 0;
                context.Device.ImmediateContext.UpdateSubresource(new DataBox(0, 0, data), this.perFrameConstantBuffer, 0);
            }

            context.Device.ImmediateContext.VertexShader.SetConstantBuffers(new[] { this.cameraConstantBuffer }, 0, 1);
            context.Device.ImmediateContext.VertexShader.SetConstantBuffers(new[] { this.resizeConstantBuffer }, 1, 1);
            context.Device.ImmediateContext.VertexShader.SetConstantBuffers(new[] { this.perFrameConstantBuffer }, 2, 1);

            context.Device.ImmediateContext.PixelShader.SetConstantBuffers(new[] { this.perFrameConstantBuffer }, 2, 1);
            
            this.mesh.Apply(context.Device.ImmediateContext.InputAssembler);
            this.shader.Apply();*/
            
            
        }

        private void UpdateProgress()
        {
            this.progress += this.progressSpeed;

            if (this.progress >= 100.0f)
            {
                this.log.Info("Giving Progress Reward!");

                this.progress = 0.0f;
                this.AddExperience(this.random.Next(200, 2000) + this.random.Next(this.level, this.level * 100));
            }
        }

        private void AddExperience(double value)
        {
            this.log.Info("Add Experience {0}", value);
            this.experience += value;

            if (this.experience >= this.nextLevel)
            {
                this.level += 1;

                TimeSpan timeForLevel = this.lastProcessingTime - this.lastLevelingTime;
                this.lastLevelingTime = this.lastProcessingTime;
                this.log.Info("Level Up to {0} in {1} seconds", this.level, timeForLevel.TotalSeconds);

                this.nextLevel += 1000 + this.level * 1000;
                this.progressSpeed = 1.0f - (this.level / 1000.0f);
                if (this.progressSpeed <= 0.0f)
                {
                    this.progressSpeed = 0.01f;
                }

                this.log.Info("Next Level at {0}", this.nextLevel);
            }
        }
    }
}
