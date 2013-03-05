using System.IO;
using System.IO.Compression;

using Carbon.Engine.Contracts.Resource;

using Core.Utils;

namespace Carbon.Engine.Resource
{
    public class FolderContent : ResourceContent
    {
        private readonly string folder;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FolderContent(string folder, bool create = false)
        {
            if (!Directory.Exists(folder))
            {
                if (!create)
                {
                    throw new DirectoryNotFoundException("FolderContent could not find the directory specified: " + folder);
                }

                Directory.CreateDirectory(folder);
            }

            this.folder = folder;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override Stream Load(string hash)
        {
            string fileName = this.GetFileName(hash);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return null;
            }

            System.Diagnostics.Trace.TraceInformation("Loading {0}", fileName);
            MemoryStream dataStream = new MemoryStream();
            using (FileStream stream = File.OpenRead(fileName))
            {
                using (GZipStream compression = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    compression.CopyTo(dataStream);
                    System.Diagnostics.Trace.TraceInformation(" -> {0} bytes read and uncompressed to {1}", stream.Length, dataStream.Length);
                    return dataStream;
                }
            }
        }

        public override bool Store(string hash, ICarbonResource data)
        {
            string fileName = this.GetFileName(hash);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            long size;
            using (FileStream stream = File.OpenWrite(fileName))
            {
                using (GZipStream compression = new GZipStream(stream, CompressionLevel.Optimal, true))
                {
                    size = data.Save(compression);
                    compression.Flush();
                }

                System.Diagnostics.Trace.TraceInformation("Stored {0} bytes as {1} ({2})", size, fileName, stream.Length);
                return true;
            }
        }

        public override bool Replace(string hash, ICarbonResource data)
        {
            string fileName = this.GetFileName(hash);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return false;
            }

            long size;
            File.Delete(fileName);
            using (FileStream stream = File.OpenWrite(fileName))
            {
                using (GZipStream compression = new GZipStream(stream, CompressionLevel.Optimal, true))
                {
                    size = data.Save(compression);
                    compression.Flush();
                }

                System.Diagnostics.Trace.TraceInformation("Stored {0} bytes as {1} ({2})", size, fileName, stream.Length);
                return true;
            }
        }

        public override ResourceInfo GetInfo(string hash)
        {
            string fileName = this.GetFileName(hash);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return null;
            }

            using (FileStream stream = File.OpenRead(fileName))
            {
                return new ResourceInfo(hash, stream.Length, HashUtils.GetMd5(stream));
            }
        }

        private string GetFileName(string hash)
        {
            return Path.Combine(this.folder, hash.Replace('/', '.'));
        }
    }
}
