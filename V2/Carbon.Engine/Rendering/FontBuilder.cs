using System;
using System.Collections.Generic;

using Carbon.Engine.Resource;
using Carbon.Engine.Resource.Resources;
using Carbon.Engine.Scene;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    internal struct FontBuilderLine
    {
        public Vector2 Position { get; set; }
        
        public string Text { get; set; }
    }

    internal class FontBuilderSegment
    {
        public FontBuilderSegment()
        {
            this.Lines = new List<FontBuilderLine>();
        }

        public Vector4 Color { get; set; }

        public IList<FontBuilderLine> Lines { get; private set; }
    }

    public static class FontBuilder
    {
        public const int CharactersPerRow = 20;
        public const int RowCount = byte.MaxValue / CharactersPerRow;

        private static Vector2 characterUVSize;

        private static Vector2 currentOffset;

        static FontBuilder()
        {
            characterUVSize = new Vector2(1.0f / CharactersPerRow, 1.0f / RowCount);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // String can have processing information
        public static ModelNode Build(string text, Vector2 characterSize, FontResource font)
        {
            currentOffset = new Vector2(0);

            ProcessText(text);

            /*
             * - Process into segments
             * - Convert segments into Meshes
             * - Create material for segment
             * - convert mesh + material to node
             * - return wrapper node
             */

            /*var builder = new MeshBuilder("Font");

            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            
            float x = 0;
            float y = 0;
            for (int l = lines.Length - 1; l >= 0; l--)
            {
                x = 0;
                foreach (char c in lines[l])
                {
                    Vector2 uvx;
                    uvx.Y = (int)((byte)c / CharactersPerRow);
                    uvx.X = (byte)c - (uvx.Y * CharactersPerRow);
                    if ((byte)c >= CharactersPerRow)
                    {
                        uvx.Y++;
                    }

                    uvx.Y = (uvx.Y / RowCount);
                    uvx.X = uvx.X / CharactersPerRow;

                    builder.BeginPolygon();
                    builder.AddVertex(new Vector3(x, y, 0), Vector3.UnitZ, uvx);
                    builder.AddVertex(new Vector3(x + characterSize.X, y + characterSize.Y, 0), Vector3.UnitZ, new Vector2(uvx.X + characterUVSize.X, uvx.Y - characterUVSize.Y));
                    builder.AddVertex(new Vector3(x + characterSize.X, y, 0), Vector3.UnitZ, new Vector2(uvx.X + characterUVSize.X, uvx.Y));
                    builder.EndPolygon();

                    builder.BeginPolygon();
                    builder.AddVertex(new Vector3(x, y, 0), Vector3.UnitZ, uvx);
                    builder.AddVertex(new Vector3(x, y + characterSize.Y, 0), Vector3.UnitZ, new Vector2(uvx.X, uvx.Y - characterUVSize.Y));
                    builder.AddVertex(new Vector3(x + characterSize.X, y + characterSize.Y, 0), Vector3.UnitZ, new Vector2(uvx.X + characterUVSize.X, uvx.Y - characterUVSize.Y));
                    builder.EndPolygon();

                    x += characterSize.X;
                }

                y += characterSize.Y;
            }

            return builder.ToMesh();*/

            return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void ProcessText(string text)
        {
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        }
    }
}
