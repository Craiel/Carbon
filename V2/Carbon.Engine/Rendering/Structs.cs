using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace Carbon.Engine.Rendering
{
    public interface IMeshStructure
    {
    }

    public struct PositionVertex : IMeshStructure
    {
        public Vector3 Position;
    }

    public struct PositionColorVertex : IMeshStructure
    {
        public Vector3 Position;
        public Vector2 Texture;
        public Vector4 Color;
    }

    public struct PositionNormalVertex : IMeshStructure
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
    }

    public struct PositionNormalTangentVertex : IMeshStructure
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public Vector4 Tangent;
    }

    public static class InputStructures
    {
        public static IDictionary<Type, int> InputLayoutSizes = new Dictionary<Type, int>
                                                                    {
                                                                        { typeof(PositionVertex), Marshal.SizeOf(typeof(PositionVertex)) },
                                                                        { typeof(PositionColorVertex), Marshal.SizeOf(typeof(PositionColorVertex)) },
                                                                        { typeof(PositionNormalVertex), Marshal.SizeOf(typeof(PositionNormalVertex)) },
                                                                        { typeof(PositionNormalTangentVertex), Marshal.SizeOf(typeof(PositionNormalTangentVertex)) },
                                                                    };

        public static IDictionary<Type, InputElement[]> InputLayouts = new Dictionary<Type, InputElement[]>
            {
                {
                    typeof(PositionVertex),
                    new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                            new InputElement("NORMAL", 0, Format.R32G32B32_Float, 0),
                            new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0)
                        }
                },
                {
                    typeof(PositionColorVertex),
                    new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                            new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0),
                            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0), 
                        }
                },
                {
                    typeof(PositionNormalVertex),
                    new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                            new InputElement("NORMAL", 0, Format.R32G32B32_Float, 0),
                            new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0)
                        }
                },
                {
                    typeof(PositionNormalTangentVertex),
                    new[]
                        {
                            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
                            new InputElement("NORMAL", 0, Format.R32G32B32_Float, 0),
                            new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0),
                            new InputElement("TANGENT", 0, Format.R32G32B32_Float, 0)
                        }
                }
            };
    }
}
