using System;
using System.Diagnostics;
using System.IO;
using System.Text;

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

    public struct TextureProcessingOptions
    {
        public TextureTargetFormat Format;

        public bool IsNormalMap;

        public bool ConvertToNormalMap;

        public bool HasAlpha;
    }

    public static class TextureProcessor
    {
        private const string CompressionTool = "nvcompress.exe";
        public static string TextureToolsPath { get; set; }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static RawResource Process(string path, TextureProcessingOptions options)
        {
            if (options.Format == TextureTargetFormat.Undefined)
            {
                throw new ArgumentException("Target format was not defined properly");
            }

            if (!Directory.Exists(TextureToolsPath))
            {
                throw new InvalidOperationException("Texture tools have not been set or directory does not exist");
            }

            string toolPath = Path.Combine(TextureToolsPath, CompressionTool);
            if (!File.Exists(toolPath))
            {
                throw new InvalidOperationException("Texture tool was not found in the expected location: " + toolPath);
            }

            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string tempFile = Path.Combine(tempDir, Path.GetRandomFileName());
            string parameter = BuildCompressionParameter(options);

            Directory.CreateDirectory(tempDir);
            try
            {
                var process = new Process();
                process.StartInfo.FileName = toolPath;
                process.StartInfo.Arguments = string.Format("{0} \"{1}\" \"{2}\"", parameter, path, tempFile);
                process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();
                process.WaitForExit();

                if (!File.Exists(tempFile))
                {
                    throw new InvalidOperationException("Expected result file was not found after texture compression");
                }

                using (FileStream stream = new FileStream(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var resource = new RawResource();
                    resource.Load(stream);
                    return resource;
                }
            }
            finally
            {
                // Make sure we clean up after
                Directory.Delete(tempDir, true);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static string BuildCompressionParameter(TextureProcessingOptions options)
        {
            StringBuilder builder = new StringBuilder();
            if (options.ConvertToNormalMap)
            {
                builder.Append("-tonormal");
            }
            else
            {
                builder.Append(options.IsNormalMap ? "-normal " : "-color ");
            }

            if (options.HasAlpha)
            {
                builder.Append("-alpha ");
            }

            switch (options.Format)
            {
                case TextureTargetFormat.DDSRgb:
                    {
                        builder.Append("-rgb ");
                        break;
                    }

                case TextureTargetFormat.DDSDxt1:
                    {
                        builder.Append(options.IsNormalMap ? "-bc1n " : "-bc1 ");
                        break;
                    }

                case TextureTargetFormat.DDSDxt3:
                    {
                        builder.Append("-bc2 ");
                        break;
                    }

                case TextureTargetFormat.DDSDxt5:
                    {
                        builder.Append(options.IsNormalMap ? "-bc3n " : "-bc3 ");
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException("Compression format has no setting: " + options.Format);
                    }
            }

            return builder.ToString();
        }
    }
}
