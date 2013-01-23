﻿using System;

using SlimDX;
using SlimDX.Direct3D11;

namespace Carbon.Engine.Rendering
{
    public enum RenderMode
    {
        Default,
        GBuffer,
        Light,
        Normal,
        Depth,
        Blend
    }

    public struct RenderParameters
    {
        public Vector4 CameraPosition;

        public Matrix View;
        public Matrix Projection;

        public bool LightingEnabled;

        public RenderMode Mode;
    }

    public sealed class RenderLightInstruction
    {
        public LightType Type { get; set; }

        public Vector4 Position { get; set; }
        public Vector4 Color { get; set; }

        public Vector3 Direction { get; set; }
        public Vector2 SpotAngles { get; set; }

        public float Range { get; set; }
        public float SpecularPower { get; set; }

        public Mesh Mesh { get; set; }
    }

    public sealed class RenderInstruction
    {
        private Matrix?[] instances;
        private int nextInstancePosition;

        public RenderInstruction()
        {
            this.Topology = PrimitiveTopology.TriangleList;
        }

        public const int MaxInstanceCount = 1000;

        public Matrix World { get; set; }

        public Mesh Mesh { get; set; }

        public PrimitiveTopology Topology { get; set; }

        public ShaderResourceView DiffuseTexture { get; set; }
        public ShaderResourceView NormalTexture { get; set; }
        public ShaderResourceView SpecularTexture { get; set; }
        public ShaderResourceView DepthMap { get; set; }
        public ShaderResourceView ShadowMap { get; set; }
        public ShaderResourceView AlphaTexture { get; set; }
        
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