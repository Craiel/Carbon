namespace Core.Engine.Resource
{
    using System.IO;
    using System.IO.Compression;

    using CarbonCore.Processing.Contracts;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.IO;
    
    public class FolderContent : ResourceContent
    {
        private readonly CarbonDirectory folder;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FolderContent(CarbonDirectory folder, bool create = false)
        {
            if (!folder.Exists)
            {
                if (!create)
                {
                    throw new DirectoryNotFoundException("FolderContent could not find the directory specified: " + folder);
                }

                folder.Create();
            }

            this.folder = folder;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override Stream Load(string hash)
        {
            CarbonFile file = this.GetFileName(hash);
            if (!file.Exists)
            {
                return null;
            }

            System.Diagnostics.Trace.TraceInformation("Loading {0}", file);
            var dataStream = new MemoryStream();
            using (FileStream stream = file.OpenRead())
            {
                using (var compression = new GZipStream(stream, CompressionMode.Decompress, false))
                {
                    compression.CopyTo(dataStream);
                    System.Diagnostics.Trace.TraceInformation(" -> {0} bytes read and uncompressed to {1}", stream.Length, dataStream.Length);
                    return dataStream;
                }
            }
        }

        public override bool Store(string hash, ICarbonResource data)
        {
            CarbonFile file = this.GetFileName(hash);
            file.DeleteIfExists();

            using (FileStream stream = file.OpenWrite())
            {
                long size;
                using (var compression = new GZipStream(stream, CompressionLevel.Optimal, true))
                {
                    size = data.Save(compression);
                    compression.Flush();
                }

                System.Diagnostics.Trace.TraceInformation("Stored {0} bytes as {1} ({2})", size, file, stream.Length);
                return true;
            }
        }

        public override bool Replace(string hash, ICarbonResource data)
        {
            CarbonFile file = this.GetFileName(hash);
            if (!file.Exists)
            {
                return false;
            }

            file.Delete();
            using (FileStream stream = file.OpenWrite())
            {
                long size;
                using (var compression = new GZipStream(stream, CompressionLevel.Optimal, true))
                {
                    size = data.Save(compression);
                    compression.Flush();
                }

                System.Diagnostics.Trace.TraceInformation("Stored {0} bytes as {1} ({2})", size, file.ToString(), stream.Length);
                return true;
            }
        }

        public override bool Delete(string hash)
        {
            CarbonFile file = this.GetFileName(hash);
            if (!file.Exists)
            {
                return false;
            }

            file.Delete();
            return true;
        }

        public override ResourceInfo GetInfo(string hash)
        {
            CarbonFile file = this.GetFileName(hash);
            if (!file.Exists)
            {
                return null;
            }

            using (FileStream stream = file.OpenRead())
            {
                return new ResourceInfo(hash, stream.Length, HashUtils.GetMd5(stream));
            }
        }

        private CarbonFile GetFileName(string hash)
        {
            return this.folder.ToFile(hash.Replace('/', '.'));
        }
    }
}
