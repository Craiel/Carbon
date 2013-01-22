using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using Carbon.Engine.Contracts.Logic;
using Carbon.Engine.Contracts.Rendering;
using Carbon.Engine.Logic;
using Carbon.Engine.Rendering;
using Carbon.Engine.Rendering.Primitives;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.DXGI;
using SlimDX.Direct3D11;
using SlimDX.Windows;

using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;

namespace Carbon.V2Test.Scenes
{
    public class TestScene3 : ITestScene
    {
        //const int width = 1600;
        //const int height = 900;

        // ----------------------------------------------------------------------------------------------------
        // Structures.

        struct SimpleVertex
        {
            public Vector3 Pos;
            public Vector2 Tex;
        }

        struct CBPerCamera
        {
            public Matrix View;
        }

        struct CBPerResize
        {
            public Matrix Projection;
        }

        struct CBPerFrame
        {
            public Matrix World;
            public Vector4 MeshColor;
        }
        

        // ----------------------------------------------------------------------------------------------------
        // Device, swapchain, rendertarget, viewport and depth stencil.
        /*static RenderForm form;
        static Device device;
        static DeviceContext context;
        static SwapChain swapChain;

        static FeatureLevel[] featureLevels;
        static DriverType driverType;*/

        //static Texture2D depthStencil;
        //static DepthStencilView depthStencilView;

        //static RenderTargetView renderTarget;
        //static SlimDX.Direct3D11.Viewport viewport;

        // ----------------------------------------------------------------------------------------------------
        // Vertex/index buffers, Vertex/Pixel shaders and shader resources.
        static Buffer vertexBuffer;
        static Buffer indexBuffer;

        static InputElement[] elements;
        static InputLayout layout;

        static SlimDX.Direct3D11.VertexShader vertexShader;
        static PixelShader pixelShader;

        static ShaderResourceView textureRV;
        static SamplerState samplerLinear;


        // ----------------------------------------------------------------------------------------------------
        // Constant buffers and matrices.
        static Buffer cbPerCamera;
        static Buffer cbPerResize;
        static Buffer cbPerFrame;

        static Matrix world;
        static Matrix view;
        static Matrix projection;
        static SlimDX.Vector4 meshColor;

        private IMesh mesh;
        private ICarbonGraphics graphics;
        
        public TestScene3(ICarbonGraphics graphics)
        {
            this.graphics = graphics;
        }
        
        public void Cleanup()
        {
            //if (context != null) context.ClearState();

            if (cbPerFrame != null) cbPerFrame.Dispose();
            if (cbPerResize != null) cbPerResize.Dispose();
            if (cbPerCamera != null) cbPerCamera.Dispose();

            if (samplerLinear != null) samplerLinear.Dispose();
            if (textureRV != null) textureRV.Dispose();
            
            if (vertexShader != null) vertexShader.Dispose();
            if (pixelShader != null) pixelShader.Dispose();
            
            if (layout != null) layout.Dispose();
            
            if (indexBuffer != null) indexBuffer.Dispose();
            if (vertexBuffer != null) vertexBuffer.Dispose();
            
            /*if (renderTarget != null) renderTarget.Dispose();

            if (depthStencilView != null) depthStencilView.Dispose();
            if (depthStencil != null) depthStencil.Dispose();*/

            //if (swapChain != null) swapChain.Dispose();
            //if (context != null) context.Dispose();
            //if (device != null) device.Dispose();
            //if (form != null) form.Dispose();

        }

        /*public void InitWindow()
        {
            form = new RenderForm("DX SDK Tutorial 7");
            form.Size = new Size(width, height);
        }*/

        public void Initialize()
        {
            /*DriverType[] driverTypes = new DriverType[]
            {
                DriverType.Hardware,
                DriverType.Warp,
                DriverType.Reference,
            };

            featureLevels = new FeatureLevel[]
            {
                FeatureLevel.Level_11_0,
                FeatureLevel.Level_10_1,
                FeatureLevel.Level_10_0,
            };

            // Create a swap chain description.
            SwapChainDescription sd = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                Usage = Usage.RenderTargetOutput,
                OutputHandle = form.Handle,
                SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0),
                IsWindowed = true,
            };

            // Attempt to create a device using different driver types.
            foreach (DriverType type in driverTypes)
            {
                driverType = type;
                try
                {
                    if (Device.CreateWithSwapChain(driverType, DeviceCreationFlags.Debug, featureLevels, sd, out device, out swapChain).IsSuccess)
                        break;
                }
                catch (Direct3D11Exception ex)
                {
                    if (driverType == DriverType.Reference)
                        throw;
                }
            }*/

            var device = graphics.Context.Device;
            var context = graphics.Context.Device.ImmediateContext;

            /*// Create a render target view.
            using (Texture2D backBuffer = graphics.Context.CreateTexture())
                renderTarget = new RenderTargetView(device, backBuffer);

            // Create the depth stencil texture.
            Texture2DDescription descDepth = new Texture2DDescription()
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.D24_UNorm_S8_UInt,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            };
            depthStencil = new Texture2D(device, descDepth);

            // Create the depth stencil view.
            DepthStencilViewDescription descDSV = new DepthStencilViewDescription()
            {
                Format = descDepth.Format,
                Dimension = DepthStencilViewDimension.Texture2D,
                MipSlice = 0,
            };
            depthStencilView = new DepthStencilView(device, depthStencil, descDSV);

            context.OutputMerger.SetTargets(depthStencilView, renderTarget);

            // Set up the viewport.
            viewport = new Viewport(0.0f, 0.0f, width, height, 0.0f, 1.0f);
            context.Rasterizer.SetViewports(viewport);*/

            ShaderSignature inputSignature;

            // Compile the vertex shader.
            using (ShaderBytecode bytecode = ShaderBytecode.CompileFromFile("Textured.fx", "VS", "vs_4_0", ShaderFlags.None, EffectFlags.None))
            {
                inputSignature = ShaderSignature.GetInputSignature(bytecode);
                vertexShader = new VertexShader(device, bytecode);
            }

            // Compile the pixel shader.
            using (ShaderBytecode bytecode = ShaderBytecode.CompileFromFile("Textured.fx", "PS", "ps_4_0", ShaderFlags.None, EffectFlags.None))
                pixelShader = new PixelShader(device, bytecode);

            // Define the input layout.
            elements = new InputElement[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0),
            };

            // Create the input layout.
            layout = new InputLayout(device, elements, inputSignature);

            // Set the input layout.
            context.InputAssembler.InputLayout = layout;

            int vertexCount = 24;
            int indexCount = 36;

            // Create the vertex buffer.
            this.mesh = this.graphics.CreateMesh();
            this.mesh.SetVertices(Cube.Data, PrimitiveTopology.TriangleList);
            this.mesh.SetIndices(Cube.Indices);
            this.mesh.Draw(context);
            /*using (DataStream data = new DataStream(24 * vertexCount, true, true))
            {
                data.WriteRange(Cube.Data);
                data.Position = 0;
                vertexBuffer = new Buffer(device, data, (int)data.Length, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            };

            // Create the index buffer.
            using (DataStream data = new DataStream(4 * 36, true, true))
            {
                data.WriteRange(Cube.Indices);
                data.Position = 0;
                indexBuffer = new Buffer(device, data, (int)data.Length, ResourceUsage.Default, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            };
            
            // Set the vertex and index buffers.
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Marshal.SizeOf(typeof(SimpleVertex)), 0));
            context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);

            // Set the primitive topology.
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;*/

            // Temporary garbage to get their sizes.
            CBPerCamera perCamera;
            CBPerResize perResize;
            CBPerFrame perFrame;

            perCamera.View = Matrix.Identity;
            perResize.Projection = Matrix.Identity;
            perFrame.MeshColor = new Vector4();
            perFrame.World = Matrix.Identity;

            // Create the constant buffers.
            BufferDescription bd = new BufferDescription();
            bd.Usage = ResourceUsage.Default;
            bd.SizeInBytes = Marshal.SizeOf(perCamera);
            bd.BindFlags = BindFlags.ConstantBuffer;
            bd.CpuAccessFlags = CpuAccessFlags.None;

            cbPerCamera = new Buffer(device, bd);

            bd.SizeInBytes = Marshal.SizeOf(perResize);
            cbPerResize = new Buffer(device, bd);

            bd.SizeInBytes = Marshal.SizeOf(perFrame);
            cbPerFrame = new Buffer(device, bd);

            // Load the texture.
            textureRV = ShaderResourceView.FromFile(device, "test.dds");

            // Create the sample state.
            SamplerDescription sampDesc = new SamplerDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                ComparisonFunction = Comparison.Never,
                MinimumLod = 0,
                MaximumLod = float.MaxValue,
            };
            samplerLinear = SamplerState.FromDescription(device, sampDesc);

            // Initialize the world matrix.
            world = Matrix.Identity;

            // Initialize the view matrix.
            Vector3 eye = new Vector3(0.0f, 3.0f, -6.0f);
            Vector3 at = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
            view = Matrix.LookAtLH(eye, at, up);

            perCamera.View = Matrix.Transpose(view);
            using (DataStream data = new DataStream(Marshal.SizeOf(perCamera), true, true))
            {
                data.Write(perCamera);
                data.Position = 0;
                context.UpdateSubresource(new DataBox(0, 0, data), cbPerCamera, 0);
            }

            // Initialize the projection matrix.
            projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)1600 / (float)800, 0.01f, 100.0f);

            perResize.Projection = Matrix.Transpose(projection);
            using (DataStream data = new DataStream(Marshal.SizeOf(perResize), true, true))
            {
                data.Write(perResize);
                data.Position = 0;
                context.UpdateSubresource(new DataBox(0, 0, data), cbPerResize, 0);
            }

            /*MessagePump.Run
                (
                    form,
                    () =>
                    {
                        Render();
                    }
                );*/
        }

        public void LoadData()
        {
        }

        public void UnloadData()
        {
        }

        public void Update(ITimer gameTime)
        {
        }

        static Stopwatch sw;

        public void Render(CarbonDeviceContext deviceContext)
        {
            if (sw == null)
            {
                sw = new Stopwatch();
                sw.Start();
            }

            var context = deviceContext.Device.ImmediateContext;

            float t = (float)sw.Elapsed.TotalSeconds;

            // Rotate cube around the origin.
            world = Matrix.RotationY(t);

            // Modify the color.
            meshColor.X = ((float)Math.Sin(t * 1.0f) + 1.0f) * 0.5f;
            meshColor.Y = ((float)Math.Cos(t * 3.0f) + 1.0f) * 0.5f;
            meshColor.Z = ((float)Math.Sin(t * 5.0f) + 1.0f) * 0.5f;
            meshColor.W = 1.0f;

            this.graphics.PrepareFrame();
            /*// Clear the back buffer.
            context.ClearRenderTargetView(renderTarget, new Color4(0.0f, 0.1f, 0.2f));

            // Clear the depth buffer to 1.0 (max depth)
            context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);*/

            // Update variables that change per frame.
            CBPerFrame perFrame;
            perFrame.World = Matrix.Transpose(world);
            perFrame.MeshColor = meshColor;
            using (DataStream data = new DataStream(Marshal.SizeOf(perFrame), true, true))
            {
                data.Write(perFrame);
                data.Position = 0;
                context.UpdateSubresource(new DataBox(0, 0, data), cbPerFrame, 0);
            }

            // Render the cube.
            context.VertexShader.Set(vertexShader);
            context.VertexShader.SetConstantBuffers(new Buffer[] { cbPerCamera }, 0, 1);
            context.VertexShader.SetConstantBuffers(new Buffer[] { cbPerResize }, 1, 1);
            context.VertexShader.SetConstantBuffers(new Buffer[] { cbPerFrame }, 2, 1);
            context.PixelShader.Set(pixelShader);
            context.PixelShader.SetConstantBuffers(new Buffer[] { cbPerFrame }, 2, 1);
            context.PixelShader.SetShaderResources(new ShaderResourceView[] { textureRV }, 0, 1);
            context.PixelShader.SetSamplers(new SamplerState[] { samplerLinear }, 0, 1);
            context.DrawIndexed(36, 0, 0);

            // Present out back buffer to the screen.
            this.graphics.Context.Present(PresentFlags.None);
            //swapChain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            this.Cleanup();
        }
    }
}
