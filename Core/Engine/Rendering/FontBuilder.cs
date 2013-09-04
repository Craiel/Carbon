namespace Core.Engine.Rendering
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Core.Engine.Resource;
    using Core.Engine.Resource.Content;
    using Core.Engine.Resource.Resources.Model;

    using SharpDX;

    internal struct FontBuilderLine
    {
        public Vector2 Position { get; set; }
        
        public string Text { get; set; }
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

            // Todo:
            // ProcessText(text);

            /*
             * - Process into segments
             * - Convert segments into Meshes
             * - Create material for segment
             * - convert mesh + material to node
             * - return wrapper node
             */

            int rowCount = byte.MaxValue / font.CharactersPerRow;
            var characterUVSize = new Vector2(1.0f / font.CharactersPerRow, 1.0f / rowCount);
            var builder = new ModelBuilder("Font");

            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            float y = 0;
            for (int l = lines.Length - 1; l >= 0; l--)
            {
                float x = 0;
                foreach (char c in lines[l])
                {
                    Vector2 uvx;

                    // ReSharper disable PossibleLossOfFraction
                    uvx.Y = (byte)c / font.CharactersPerRow;
                    // ReSharper restore PossibleLossOfFraction
                    uvx.X = (byte)c - (uvx.Y * font.CharactersPerRow);
                    if ((byte)c >= font.CharactersPerRow)
                    {
                        uvx.Y++;
                    }

                    uvx.Y = uvx.Y / rowCount;
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

            return builder.ToResource();
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal class FontBuilderSegment
    {
        public FontBuilderSegment()
        {
            this.Lines = new List<FontBuilderLine>();
        }

        public Vector4 Color { get; set; }

        public IList<FontBuilderLine> Lines { get; private set; }
    }
}
