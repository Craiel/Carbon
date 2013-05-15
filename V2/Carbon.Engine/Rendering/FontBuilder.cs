using System;
using System.Collections.Generic;

using Carbon.Engine.Resource.Resources;
using Carbon.Engine.Scene;

using SlimDX;

namespace Carbon.Engine.Rendering
{
    using Carbon.Engine.Resource.Content;
    using Carbon.Engine.Resource.Resources.Model;

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
        // String can have processing information
        public static ModelResource Build(string text, Vector2 characterSize, FontEntry font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            ProcessText(text);

            /*
             * - Process into segments
             * - Convert segments into Meshes
             * - Create material for segment
             * - convert mesh + material to node
             * - return wrapper node
             */

            int rowCount = byte.MaxValue / font.CharactersPerRow;
            Vector2 characterUVSize = new Vector2(1.0f / font.CharactersPerRow, 1.0f / rowCount);
            var builder = new MeshBuilder("Font");

            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            
            float x = 0;
            float y = 0;
            for (int l = lines.Length - 1; l >= 0; l--)
            {
                x = 0;
                foreach (char c in lines[l])
                {
                    Vector2 uvx;
                    uvx.Y = ((byte)c / font.CharactersPerRow);
                    uvx.X = (byte)c - (uvx.Y * font.CharactersPerRow);
                    if ((byte)c >= font.CharactersPerRow)
                    {
                        uvx.Y++;
                    }

                    uvx.Y = (uvx.Y / rowCount);
                    uvx.X = uvx.X / font.CharactersPerRow;

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

            return builder.ToMesh();

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
