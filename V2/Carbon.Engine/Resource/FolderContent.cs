using System.IO;

using Carbon.Engine.Contracts.Resource;

namespace Carbon.Engine.Resource
{
    public class FolderContent : ResourceContent
    {
        private readonly string folder;
        private readonly bool useSources;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FolderContent(string folder, bool create = false, bool useSources = false)
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
            this.useSources = useSources;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override Stream Load(string hash, string key)
        {
            string fileName = Path.Combine(this.folder, hash.Replace('/', '.'));
            string sourceFileName = Path.Combine(this.folder, key);

            if (!File.Exists(fileName))
            {
                if (!this.useSources || !this.ConvertSource(sourceFileName, fileName))
                {
                    return null;
                }
            }

            System.Diagnostics.Trace.TraceInformation("Loading {0}", fileName);
            MemoryStream dataStream = new MemoryStream();
            using (FileStream stream = File.OpenRead(fileName))
            {
                stream.CopyTo(dataStream);
                System.Diagnostics.Trace.TraceInformation(" -> {0} bytes read", dataStream.Length);
                return dataStream;
            }
        }

        public override bool Store(string hash, ICarbonResource data)
        {
            string fileName = Path.Combine(this.folder, hash.Replace('/', '.'));

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            long size;
            using (FileStream stream = File.OpenWrite(fileName))
            {
                size = data.Save(stream);
                stream.Flush(true);
            }

            System.Diagnostics.Trace.TraceInformation("Stored {0} bytes as {1}", size, fileName);

            return true;
        }

        public override bool Replace(string hash, ICarbonResource data)
        {
            string fileName = Path.Combine(this.folder, hash.Replace('/', '.'));

            if (!File.Exists(fileName))
            {
                return false;
            }

            long size;
            File.Delete(fileName);
            using (FileStream stream = File.OpenWrite(fileName))
            {
                size = data.Save(stream);
                stream.Flush(true);
            }

            System.Diagnostics.Trace.TraceInformation("Stored {0} bytes as {1}", size, fileName);

            return true;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool ConvertSource(string sourceName, string targetName)
        {
            if (!File.Exists(sourceName))
            {
                return false;
            }

            System.Diagnostics.Trace.TraceInformation("Converting Source File {0} to {1}", sourceName, targetName);
            File.Copy(sourceName, targetName);
            return true;
        }
    }
}
