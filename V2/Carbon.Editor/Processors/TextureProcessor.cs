using System;

using Carbon.Engine.Resource.Resources;

namespace Carbon.Editor.Processors
{
    public enum TextureTargetFormat
    {
        Undefined,

        DDSRgb,
        DDSDxt1,
        DDSDxt3,
        DDSDxt5
    }

    public static class TextureProcessor
    {
        private const string CompressionTool = "nvcompress.exe";
        public static string TextureToolsPath { get; set; }

        public static RawResource Process(string path, TextureTargetFormat format)
        {
            if (format == TextureTargetFormat.Undefined)
            {
                throw new ArgumentException("Target format was not defined properly");
            }

            if (!System.IO.Directory.Exists(TextureToolsPath))
            {
                throw new InvalidOperationException("Texture tools have not been set, can not process data");
            }

            string tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
            System.IO.Directory.CreateDirectory(tempDir);
            string commandLine = string.Format("");
            

            System.IO.Directory.Delete(tempDir, true);
        }
    }
}
