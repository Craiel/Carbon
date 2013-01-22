using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace Carbon.Engine.Rendering
{
    public struct PositionVertex
    {
        public static int Size = Marshal.SizeOf(typeof(PositionVertex));

        public Vector3 Position;
    }

    public struct PositionNormalVertex
    {
        public static int Size = Marshal.SizeOf(typeof(PositionNormalVertex));

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
    }

    public struct PositionNormalTangentVertex
    {
        public static int Size = Marshal.SizeOf(typeof(PositionNormalTangentVertex));

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public Vector4 Tangent;
    }

    public static class InputStructures
    {
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
