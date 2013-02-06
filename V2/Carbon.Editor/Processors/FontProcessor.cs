using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

using Carbon.Editor.Resource;
using Carbon.Engine.Rendering;
using Carbon.Engine.Resource;

namespace Carbon.Editor.Processors
{
    using Carbon.Editor.Contracts;
    using Carbon.Editor.Logic;

    public class FontProcessor : IContentProcessor
    {
        private const int CharactersPerRow = 20;
        private const int RowCount = byte.MaxValue / CharactersPerRow;

        private static readonly bool[] chars;

        private Font font;

        static FontProcessor()
        {
            var charSelection = new List<char>();
            for (byte c = 0; c < byte.MaxValue; c++)
            {
                char current = (char)c;
                if (char.IsLetterOrDigit(current) || char.IsPunctuation(current) || char.IsSymbol(current))
                {
                    charSelection.Add(current);
                }
            }

            chars = new bool[255];
            foreach (char c in charSelection)
            {
                chars[(byte)c] = true;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ProcessingResult Process(CarbonBuilderEntry entry)
        {
            return null;
        }

        public void Process(Stream target, CarbonBuilderEntry entry)
        {
            /*SourceTextureFont source = (SourceTextureFont)entry.Content;

            this.font = new Font(source.Font.Reference, source.FontSize, FontStyle.Regular);*/
            
            // Todo: Evaluate reference

            // - Pre-process -> texture -> save md5 of texture -> store reference to texture -> 

            // Draw and return
            using (Bitmap image = this.Draw())
            {
                // Todo: Move this into pre-processing and use it as a referenced resource
                /*switch (format)
                {
                    case ProcessingTargetFormat.Preview:
                        {
                            image.Save(target, ImageFormat.Png);
                            return;
                        }

                    case ProcessingTargetFormat.Baked:
                        {
                            using (var stream = new MemoryStream())
                            {
                                image.Save(stream, ImageFormat.Png);
                                stream.Position = 0;
                                byte[] textureData = new byte[stream.Length];
                                stream.Read(textureData, 0, (int)stream.Length);
                                var processedResource = new FontResource
                                    {
                                        CharactersPerRow = CharactersPerRow,
                                        RowCount = byte.MaxValue / CharactersPerRow,
                                        TextureData = textureData
                                    };
                                processedResource.Save(target);
                            }
                            return;
                        }

                    default:
                        {
                            throw new NotImplementedException();
                        }
                }*/
            }
        }
        
        private Point Measure()
        {
            Bitmap image = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(image);
            int width = 0;
            int height = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (!chars[i])
                {
                    continue;
                }

                SizeF size = graphics.MeasureString(new string((char)i, 1), this.font);
                if (size.Width > width)
                {
                    width = (int)size.Width;
                }
                if (size.Height > height)
                {
                    height = (int)size.Height;
                }
            }

            return new Point(width, height);
        }

        private Bitmap Draw()
        {
            Point glyphSize = this.Measure();

            var image = new Bitmap(glyphSize.X * FontBuilder.CharactersPerRow, glyphSize.Y * RowCount);
            var graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            int rowCounter = 0;
            int x = 0;
            int y = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                string str = chars[i] ? new string((char)i, 1) : "";

                graphics.DrawString(str, font, new SolidBrush(Color.White), x, y);
                x += glyphSize.X;
                rowCounter++;
                if (rowCounter >= FontBuilder.CharactersPerRow)
                {
                    rowCounter = 0;
                    y += glyphSize.Y;
                    x = 0;
                }
            }

            graphics.Flush();
            return image;
        }
    }
}
