namespace Core.Engine.Rendering
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D;
    using SharpDX.Direct3D11;

    public enum RenderMode
    {
        Default,
        GBuffer,
        Light,
        Normal,
        Depth,
        Blend,
        ShadowMap,
        Plain,
    }

    public struct RenderParameters
    {
        public Vector3 CameraPosition;

        public Matrix View;
        public Matrix Projection;

        public bool DepthEnabled;
        public bool LightingEnabled;
        public bool RenderSolid;
        public CullMode CullMode;

        public PrimitiveTopology Topology;

        public RenderMode Mode;
    }

    public sealed class RenderInstruction
    {
        public const int MaxInstanceCount = 1000;

        private Matrix?[] instances;
        private int nextInstancePosition;

        public Matrix World { get; set; }

        public Mesh Mesh { get; set; }

        public TextureData DiffuseTexture { get; set; }
        public TextureData NormalTexture { get; set; }
        public TextureData SpecularTexture { get; set; }
        public TextureData DepthMap { get; set; }
        public TextureData ShadowMap { get; set; }
        public TextureData AlphaTexture { get; set; }
        
        public Vector4? Color { get; set; }
        
        public Matrix?[] Instances
        {
            get
            {
                return this.instances;
            }
        }

        public int InstanceCount
        {
            get
            {
                return this.nextInstancePosition;
            }
        }

        public void AddInstance(Matrix world)
        {
            if (this.instances == null)
            {
                this.instances = new Matrix?[MaxInstanceCount];
            }

            if (this.nextInstancePosition > MaxInstanceCount)
            {
                throw new InvalidOperationException("Instance limit exceeded for this instruction");
            }

            this.instances[this.nextInstancePosition++] = world;
        }
    }
}
